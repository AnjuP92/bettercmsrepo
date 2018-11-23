﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRoleService.cs" company="Devbridge Group LLC">
// 
// Copyright (C) 2015,2016 Devbridge Group LLC
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
// 
// <summary>
// Better CMS is a publishing focused and developer friendly .NET open source CMS.
// 
// Website: https://www.bettercms.com 
// GitHub: https://github.com/devbridge/bettercms
// Email: info@bettercms.com
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Linq;

using BetterCms.Core.WebServices;
using BetterCms.Core.Exceptions.Mvc;

using BetterCms.Module.Root.Mvc;
using BetterCms.Module.Users.Content.Resources;
using BetterCms.Module.Users.Models;

using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.DataAccess.DataContext.Fetching;
using BetterModules.Core.Exceptions.DataTier;

namespace BetterCms.Module.Users.Services
{
    /// <summary>
    /// Service implementation for managing user roles
    /// </summary>
    public class DefaultRoleService : IRoleService
    {
        /// <summary>
        /// The repository
        /// </summary>
        private readonly IRepository repository;
        private ITWebClient _webClient;
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRoleService" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public DefaultRoleService(IRepository repository)
        {
            this.repository = repository;
            _webClient = new TWebClient();
        }

        /// <summary>
        /// Creates the role.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <returns>
        /// Created role entity
        /// </returns>
        public Role CreateRole(string name, string description = null)
        {
            bool newEntityCreated;
            return SaveRole(Guid.Empty, 0, name, description, out newEntityCreated);
        }

        /// <summary>
        /// Updates the role.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="version">The version.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <returns>
        /// Updated role entity
        /// </returns>
        public Role UpdateRole(Guid id, int version, string name, string description = null)
        {
            bool newEntityCreated;
            return SaveRole(id, version, name, description, out newEntityCreated);
        }

        /// <summary>
        /// Deletes the role by specified role name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="throwOnPopulatedRole">If true, throw an exception if role has one or more members and do not delete role.</param>
        /// <returns>
        /// Deleted role entity
        /// </returns>
        public Role DeleteRole(string name, bool throwOnPopulatedRole)
        {
            var role = repository
                .AsQueryable<Role>(r => name == r.Name)
                .FetchMany(r => r.UserRoles)
                .ToList()
                .FirstOne();

            DeleteRole(role, throwOnPopulatedRole);
            role.IsDeleted = true;
            return role;
        }

        /// <summary>
        /// Deletes the role by specified role id and version.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="version">The version.</param>
        /// <param name="throwOnPopulatedRole">If true, throw an exception if role has one or more members and do not delete role.</param>
        /// <returns>
        /// Deleted role entity
        /// </returns>
        public Role DeleteRole(Guid id, int version, bool throwOnPopulatedRole)
        {
            var role = repository
                .AsQueryable<Role>(r => id == r.Id)
                .FetchMany(r => r.UserRoles)
                .ToList()
                .FirstOne();

            if (version > 0 && role.Version != version)
            {
                throw new ConcurrentDataException(role);
            }

            DeleteRole(role, throwOnPopulatedRole);
            role.IsDeleted = true;
            return role;
        }

        /// <summary>
        /// Saves the role.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="version">The version.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="newEntityCreated">if set to <c>true</c> new entity was created.</param>
        /// <param name="createIfNotExists">if set to <c>true</c> create entity if not exists.</param>
        /// <returns>
        /// Saved role entity
        /// </returns>
        /// <exception cref="EntityNotFoundException"></exception>
        /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException"></exception>
        public Role SaveRole(Guid id, int version, string name, string description, out bool newEntityCreated, bool createIfNotExists = false)
        {

            // Check if such role doesn't exist
            ValidateRoleName(id, name);

            Role role = null;
            newEntityCreated = id.HasDefaultValue();
            if (!newEntityCreated)
            {
                role = repository.AsQueryable<Role>(f => f.Id == id).FirstOrDefault();
                newEntityCreated = role == null;

                if (newEntityCreated && !createIfNotExists)
                {
                    throw new EntityNotFoundException(typeof(Role), id);
                }
            }

            if (newEntityCreated)
            {
                role = new Role { Id = id };
            }

            if (role.IsSystematic)
            {
                var logMessage = string.Format("Cannot save systematic role: {0} {1}", role.Name, role.Description);
                var message = string.Format(UsersGlobalization.SaveRole_Cannot_Save_Systematic_Role, role.Description ?? role.Name);

                throw new ValidationException(() => message, logMessage);
            }

            if (version > 0)
            {
                role.Version = version;
            }
            role.Name = name;
            role.Description = description;

            repository.Save(role);

            return role;

            //var model = _webClient.DownloadData<string>("Role/SaveRole", new { id = id, name = name, description = description, version = version });
            //role.Id = new Guid(model);
            //return role;
        }

        /// <summary>
        /// Validates the name of the role.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        private void ValidateRoleName(Guid id, string name)
        {

            var query = repository
                .AsQueryable<Role>(r => r.Name == name.Trim());

            if (!id.HasDefaultValue())
            {
                query = query.Where(r => r.Id != id);
            }

            var existIngId = query
                .Select(r => r.Id)
                .FirstOrDefault();

            if (!existIngId.HasDefaultValue())
            {
                var message = string.Format(UsersGlobalization.SaveRole_RoleExists_Message, name);
                var logMessage = string.Format("Role already exists. Role name: {0}, Id: {1}", name, id);

                throw new ValidationException(() => message, logMessage);
            }
            //var model = _webClient.DownloadData<int>("Role/ValidateRoleName", new { Id = id, Name = name });

            //if (model == 0)
            //{
            //    var message = string.Format(UsersGlobalization.SaveRole_RoleExists_Message, name);
            //    var logMessage = string.Format("Role already exists. Role name: {0}, Id: {1}", name, id);

            //    throw new ValidationException(() => message, logMessage);
            //}
        }

        /// <summary>
        /// Deletes the role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="throwOnPopulatedRole">If true, throw an exception if role has one or more members and do not delete role.</param>
        private void DeleteRole(Role role, bool throwOnPopulatedRole)
        {
            if (throwOnPopulatedRole && role.UserRoles.Any())
            {
                var logMessage = string.Format("Cannot delete populated role: {0} {1}", role.Name, role.Description);
                var message = string.Format(UsersGlobalization.DeleteRole_Cannot_Delete_Populated_Role, role.Description ?? role.Name);

                throw new ValidationException(() => message, logMessage);
            }

            if (role.IsSystematic)
            {
                var logMessage = string.Format("Cannot delete systematic role: {0} {1}", role.Name, role.Description);
                var message = string.Format(UsersGlobalization.DeleteRole_Cannot_Delete_Systematic_Role, role.Description ?? role.Name);

                throw new ValidationException(() => message, logMessage);
            }

            repository.Delete(role);

            foreach (var userRole in role.UserRoles)
            {
                repository.Delete(userRole);
            }


            //var model = _webClient.DownloadData<int>("Role/DeleteRole", new { RoleId = role.Id });
            // role.Id = new Guid(model);
        }
    }
}
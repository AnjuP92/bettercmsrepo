﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveUserCommand.cs" company="Devbridge Group LLC">
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
using BetterCms.Module.Root.Mvc;

using BetterCms.Module.Users.Services;
using BetterCms.Module.Users.ViewModels.User;

using BetterModules.Core.Web.Mvc.Commands;

namespace BetterCms.Module.Users.Commands.User.SaveUser
{
    /// <summary>
    /// A command to save user item.
    /// </summary>
    public class SaveUserCommand : CommandBase, ICommand<EditUserViewModel, SaveUserCommandResponse>
    {
        /// <summary>
        /// The user service
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveUserCommand" /> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        public SaveUserCommand(IUserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// Executes a command to save user.
        /// </summary>
        /// <param name="request">The user item.</param>
        /// <returns>
        /// true if user saved successfully; false otherwise.
        /// </returns>
        public SaveUserCommandResponse Execute(EditUserViewModel request)
        {
            var user = userService.SaveUser(request);

            return new SaveUserCommandResponse
                       {
                           Id = user.Id, 
                           UserName = user.UserName, 
                           Version = user.Version,
                           Email = user.Email,
                           FullName = user.FirstName + " " + user.LastName
                       };
        }

        /// <summary>
        /// Executes a command to save user.
        /// </summary>
        /// <param name="request">The user item.</param>
        /// <returns>
        /// true if user saved successfully; false otherwise.
        /// </returns>
        public SaveUserCommandResponse ExecuteSocialUser(EditUserViewModel request)
        {
            var user = userService.SaveSocialUser(request);

            return new SaveUserCommandResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Version = user.Version,
                Email = user.Email,
                FullName = user.FirstName + " " + user.LastName
            };
        }
    }
}
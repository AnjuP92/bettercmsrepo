﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="User.cs" company="Devbridge Group LLC">
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
using System.Collections.Generic;

using BetterCms.Module.MediaManager.Models;

using BetterModules.Core.Models;

namespace BetterCms.Module.Users.Models
{
    [Serializable]
    public class User : EquatableEntity<User>
    {
        public virtual string UserName { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string Email { get; set; }

        public virtual string Password { get; set; }

        public virtual string Salt { get; set; }

        public virtual MediaImage Image { get; set; }

        public virtual IList<UserRole> UserRoles { get; set; }

        public virtual User CopyDataTo(User user)
        {
            user.UserName = UserName;
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.Email = Email;
            user.Password = Password;
            user.Salt = Salt;
            user.Image = Image;

            foreach (var userRole in UserRoles)
            {
                if (user.UserRoles == null)
                {
                    user.UserRoles = new List<UserRole>();
                }

                user.UserRoles.Add(new UserRole { User = user, Role = userRole.Role });
            }

            return user;
        }
    }
}
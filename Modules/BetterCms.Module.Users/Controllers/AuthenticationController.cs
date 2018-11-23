// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationController.cs" company="Devbridge Group LLC">
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
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using BetterCms.Module.Root.Mvc;
using BetterCms.Module.Users.Commands.Authentication;
using BetterCms.Module.Users.Content.Resources;
using BetterCms.Module.Users.Services;
using BetterCms.Module.Users.ViewModels.Authentication;

using Common.Logging;

using Microsoft.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using BetterCms.Core.WebServices;
using BetterCms.Module.Users.Commands.User.SaveUser;

namespace BetterCms.Module.Users.Controllers
{
    /// <summary>
    /// User management.
    /// </summary>    
    [ActionLinkArea(UsersModuleDescriptor.UsersAreaName)]
    public class AuthenticationController : CmsControllerBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IAuthenticationService authenticationService;

        private ITWebClient _webClient;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
            _webClient = new TWebClient();
        }

        /// <summary>
        /// Creates the first user.
        /// </summary>        
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (!FormsAuthentication.IsEnabled)
            {
                Messages.AddError(UsersGlobalization.Login_FormsAuthentication_DisabledMessage);
            }

            if (User.Identity.IsAuthenticated)
            {
                return Redirect(FormsAuthentication.DefaultUrl ?? "/");
            }

            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl,
                IsFormsAuthenticationEnabled = FormsAuthentication.IsEnabled
            });
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {

            if (!FormsAuthentication.IsEnabled)
            {
                Messages.AddError(string.Empty, UsersGlobalization.Login_FormsAuthentication_DisabledMessage);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    HttpCookie authCookie = GetCommand<LoginCommand>().ExecuteCommand(model);
                    if (authCookie != null)
                    {
                        Response.Cookies.Add(authCookie);

                        return Redirect(model.ReturnUrl ?? (FormsAuthentication.DefaultUrl ?? "/"));
                    }
                }
            }
            model.IsFormsAuthenticationEnabled = FormsAuthentication.IsEnabled;
            return View(model);
        }
        [HttpPost]
        public ActionResult SocialLogin(string SocialId, string FirstName, string LastName, string Email, string Type)
        //public ActionResult SocialLogin(string SocialId)
        {
            // string UsernameFromSP = _webClient.DownloadData<string>("Authentication/GetUserDetailsBySocialID", new { socialId = SocialId });
            LoginViewModel model = new LoginViewModel();
            model.UserName = SocialId;
            //model.Password = UsernameFromSP;

            if (!FormsAuthentication.IsEnabled)
            {
                Messages.AddError(string.Empty, UsersGlobalization.Login_FormsAuthentication_DisabledMessage);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        HttpCookie authCookie = GetCommand<LoginCommand>().ExecuteSocialLogin(model);
                        if (authCookie != null)
                        {
                            Response.Cookies.Add(authCookie);

                            return Json(1);//Redirect(model.ReturnUrl ?? (FormsAuthentication.DefaultUrl ?? "/"));
                        }
                        else
                        {
                            //register user
                            BetterCms.Module.Users.ViewModels.User.EditUserViewModel user = new ViewModels.User.EditUserViewModel();
                            user.Email = Email;
                            user.FirstName = FirstName;
                            user.LastName = LastName;
                            user.Password = "";
                            user.RetypedPassword = "";
                            user.UserName = SocialId;
                            user.Version = 0;
                            int status = _webClient.DownloadData<int>("User/ApiValidateUserEmail", new { Id = user.Id, Email = Email });
                            if (status == 1)
                            {
                                var response = GetCommand<SaveUserCommand>().ExecuteSocialUser(user);

                                if (response != null)
                                {
                                    authCookie = GetCommand<LoginCommand>().ExecuteSocialLogin(model);
                                    if (authCookie != null)
                                    {
                                        Response.Cookies.Add(authCookie);

                                        return Json(status);// Redirect(model.ReturnUrl ?? (FormsAuthentication.DefaultUrl ?? "/"));
                                    }
                                    return Redirect("/Login");
                                }
                            }
                            else
                                return Json(status);
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            model.IsFormsAuthenticationEnabled = FormsAuthentication.IsEnabled;
            return View(model);
        }

        [HttpGet]
        // public ActionResult SocialLogin(string SocialId, string FirstName, string LastName, string Email,string Type)
        public JsonResult IsSocialIdExists(string SocialId)
        {
            string UsernameFromSP = _webClient.DownloadData<string>("Authentication/GetUserDetailsBySocialID", new { socialId = SocialId });

            return Json(string.IsNullOrEmpty(UsernameFromSP) ? 0 : 1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Signup()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect(FormsAuthentication.DefaultUrl ?? "/");
            }
            var model = GetCommand<BetterCms.Module.Users.Commands.User.GetUser.GetUserCommand>().ExecuteCommand(System.Guid.Empty);
            return View(model);
        }
        [HttpPost]
        public ActionResult Signup(BetterCms.Module.Users.ViewModels.User.EditUserViewModel model)
        {
           
            if (ModelState.IsValid)
            {
                //validate username
                var IsUserName = _webClient.DownloadData<int>("User/ApiValidateUserName", new { Id = model.Id, UserName = model.UserName });
                if (IsUserName == 0)
                    ModelState.AddModelError("UserName", string.Format(UsersGlobalization.SaveUse_UserNameExists_Message, model.UserName));
                //validate email

                var IsEmail = _webClient.DownloadData<int>("User/ApiValidateUserEmail", new { Id = model.Id, Email = model.Email });
                if (IsEmail == 0)
                    ModelState.AddModelError("Email", string.Format(UsersGlobalization.SaveUse_UserEmailExists_Message, model.Email));
                //if(Isvalid==1)
                SaveUserCommandResponse response = null;
                if (IsUserName == 1 && IsEmail == 1)
                    response = GetCommand<SaveUserCommand>().ExecuteCommand(model);
                if (response != null)
                {
                    model = new BetterCms.Module.Users.ViewModels.User.EditUserViewModel();
                    ViewBag.result = UsersGlobalization.SaveUser_CreatedSuccessfully_Message;
                }
                return View(model);
            }
            return View();
        }


    }
}

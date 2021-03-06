﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplatesController.cs" company="Devbridge Group LLC">
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
using System.Linq;
using System.Web.Mvc;

using BetterCms.Core.Exceptions.Mvc;
using BetterCms.Core.Security;

using BetterCms.Module.Pages.Command.Layout.DeleteTemplate;
using BetterCms.Module.Pages.Command.Layout.GetSiteSettingsTemplates;
using BetterCms.Module.Pages.Command.Layout.GetTemplateForEdit;
using BetterCms.Module.Pages.Command.Layout.SaveTemplate;
using BetterCms.Module.Pages.Command.Page.GetPagesList;
using BetterCms.Module.Pages.Content.Resources;
using BetterCms.Module.Pages.ViewModels.Filter;
using BetterCms.Module.Pages.ViewModels.Templates;

using BetterCms.Module.Root;
using BetterCms.Module.Root.Models;
using BetterCms.Module.Root.Mvc;
using BetterCms.Module.Root.Mvc.Grids.GridOptions;

using BetterModules.Core.Web.Models;

using Microsoft.Web.Mvc;
using System.Collections.Generic;
using System.Xml.Linq;
using BetterCms.Core.WebServices;
using Newtonsoft.Json.Linq;

namespace BetterCms.Module.Pages.Controllers
{
    public class TemplateRegions
    {
        public string Name { get; set; }
        public string Required { get; set; }
    }
    /// <summary>
    /// Template management.
    /// </summary>
    [BcmsAuthorize(RootModuleConstants.UserRoles.Administration)]
    [ActionLinkArea(PagesModuleDescriptor.PagesAreaName)]
    public class TemplatesController : CmsControllerBase
    {
        /// <summary>
        /// Deletes template.
        /// </summary>
        /// <param name="id">The template id.</param>
        /// <param name="version">The version.</param>
        /// <returns>
        /// Json with result status.
        /// </returns>
        [HttpPost]
        public ActionResult DeleteTemplate(string id, string version)
        {
            var request = new DeleteTemplateCommandRequest
            {
                TemplateId = id.ToGuidOrDefault(),
                Version = version.ToIntOrDefault()
            };

            if (GetCommand<DeleteTemplateCommand>().ExecuteCommand(request))
            {
                Messages.AddSuccess(PagesGlobalization.DeleteTemplate_DeletedSuccessfully_Message);
                return Json(new WireJson
                {
                    Success = true
                });
            }
            return Json(new WireJson { Success = false });
        }

        /// <summary>
        /// Creates modal dialog for creating a new template.
        /// </summary>
        /// <returns>
        /// ViewResult to render new template modal dialog.
        /// </returns>
        [HttpGet]
        public ActionResult RegisterTemplate()
        {
            var model = GetCommand<GetTemplateForEditCommand>().ExecuteCommand(null);
            var view = RenderView("EditTemplate", model);

            return ComboWireJson(model != null, view, model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Validates and saves template.
        /// </summary>
        /// <param name="model">The template view model.</param>
        /// <returns>
        /// Json with result status.
        /// </returns>
        [HttpPost]
        public ActionResult RegisterTemplate(TemplateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                List<TemplateRegions> TemplateRegions = new List<TemplateRegions>();
                try
                {
                    string layoutstring = System.IO.File.ReadAllText(Server.MapPath(model.Url));
                    int strLength = layoutstring.Length;
                    int curIndex = 0;
                    int startIndex = 0;
                    int endIndex = 0;
                    while (curIndex < strLength)
                    {
                        startIndex = layoutstring.IndexOf("@*");
                        if (startIndex < 0)
                            break;
                        endIndex = layoutstring.IndexOf("*@", startIndex + 2);
                        layoutstring = layoutstring.Remove(startIndex, endIndex - startIndex + 2);
                        curIndex = endIndex;
                    }
                    curIndex = 0;
                    curIndex = 0;
                    strLength = layoutstring.Length;
                    while (curIndex < strLength)
                    {
                        startIndex = layoutstring.IndexOf("@RenderSection", curIndex);
                        if (startIndex < 0)
                            break;
                        startIndex = layoutstring.IndexOf("(", startIndex);
                        endIndex = layoutstring.IndexOf(")", startIndex);
                        char[] seperators = new char[] { ',' };
                        var list = layoutstring.Substring(startIndex, endIndex - startIndex + 1).Trim(')', '(').Split(',');
                        if (list.Count() > 1)
                            TemplateRegions.Add(new TemplateRegions() { Name = list[0].Trim('"', ' '), Required = list[1].Trim('"', ' ') });
                        curIndex = endIndex;
                    }
                }
                catch { }
                string strAllRegions = string.Join(", ", TemplateRegions.Select(x => x.Name));
                bool allRegionMatched = true;
                foreach (var item in model.Regions)
                {
                    if (!strAllRegions.Contains(item.Identifier))
                    {
                        allRegionMatched = false;
                        break;
                    }
                }
                var count = (from item1 in model.Regions
                             join item2 in TemplateRegions
                             on new { a = item1.Identifier, b = "true" } equals new { a = item2.Name, b = item2.Required }
                             select new
                             {
                                 a = item2.Name
                             }).Count();
                var manRegions = TemplateRegions.Where(x => x.Required == "true");

                var viewEngineResult = ViewEngines.Engines.FindView(ControllerContext, model.Url, null);
                if (viewEngineResult == null || viewEngineResult.View == null)
                {
                    Messages.AddError(string.Format(PagesGlobalization.SaveTemplate_VirtualPathNotExists_Message, model.Url));
                    return Json(new WireJson { Success = false });
                }

                if (model.Regions != null && model.Regions.GroupBy(r => r.Identifier).SelectMany(g => g.Skip(1)).Any())
                {
                    Messages.AddError(PagesGlobalization.SaveTemplate_DublicateRegionIdentificator_Message);
                    return Json(new WireJson { Success = false });
                }
                if (!allRegionMatched)
                {
                    Messages.AddError(string.Format("Only {0} regions are allowed for this template", strAllRegions));
                    return Json(new WireJson { Success = false });
                }
                if (count != manRegions.Count())
                {
                    Messages.AddError(string.Format("Please enter all mandatory({0}) regions for this template", string.Join(", ", manRegions.Select(x => x.Name))));
                    return Json(new WireJson { Success = false });
                }
                var response = GetCommand<SaveTemplateCommand>().ExecuteCommand(model);
                if (response != null)
                {
                    if (model.Id.HasDefaultValue())
                    {
                        Messages.AddSuccess(PagesGlobalization.SaveTemplate_CreatedSuccessfully_Message);
                    }
                    return Json(new WireJson { Success = true, Data = response });
                }
            }

            return Json(new WireJson { Success = false });
        }

        /// <summary>
        /// Creates modal dialog for edit a template.
        /// </summary>
        /// <returns>
        /// ViewResult to render editing tempate modal dialog.
        /// </returns>
        [HttpGet]
        public ActionResult EditTemplate(string id)
        {
            var model = GetCommand<GetTemplateForEditCommand>().ExecuteCommand(id.ToGuidOrDefault());
            var view = RenderView("EditTemplate", model);
            return ComboWireJson(model != null, view, model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Renders a templates list for the site settings dialog.
        /// </summary>
        /// <returns>Rendered templates list.</returns>
        public ActionResult Templates(SearchableGridOptions request)
        {
            request.SetDefaultPaging();
            var Identity = HttpContext.User.Identity;
            var model = GetCommand<GetSiteSettingsTemplatesCommand>().ExecuteCommand(request);
            
            var xml = new XElement("Pages",
           from c in model.Items
           select new XElement("PageId", c.Id));
            ITWebClient _webClient = new TWebClient();
            var PageIds = _webClient.DownloadData<JArray>("User/PageReadOnlyAccess", new { UserName = Identity.Name, Xml = xml });
            var list = PageIds.Select(x => x);
            string ss = string.Join(",", PageIds);
            ViewBag.Ids = ss;
            Response.Headers.Add("ids", ss);

            return View(model);
        }

        /// <summary>
        /// Masters the pages.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// Rendered master pages list.
        /// </returns>
        public ActionResult MasterPages(PagesFilter request)
        {
            request.SetDefaultPaging();
            request.OnlyMasterPages = true;
            var model = GetCommand<GetPagesListCommand>().ExecuteCommand(request);
            var success = model != null;

            var view = RenderView("MasterPages", model);
            var json = new
            {
                Tags = request.Tags,
                IncludeArchived = request.IncludeArchived,
                IncludeMasterPages = request.IncludeMasterPages
            };

            return ComboWireJson(success, view, json, JsonRequestBehavior.AllowGet);
        }
    }
}
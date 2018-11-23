// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultContentService.cs" company="Devbridge Group LLC">
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using BetterModules.Core.DataAccess;

using BetterCms.Core.DataContracts;
using BetterCms.Core.DataContracts.Enums;
using BetterCms.Core.Exceptions;
using BetterCms.Core.Exceptions.Mvc;
using BetterCms.Core.Services;

using BetterCms.Module.Root.Content.Resources;
using BetterCms.Module.Root.Models;
using BetterCms.Module.Root.Mvc;
using BetterCms.Module.Root.Mvc.Helpers;

using BetterModules.Core.Exceptions.DataTier;

using NHibernate.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BetterCms.Core.WebServices;



namespace BetterCms.Module.Root.Services
{
    public class DefaultContentService : IContentService
    {
        /// <summary>
        /// A security service.
        /// </summary>
        private readonly ISecurityService securityService;

        /// <summary>
        /// A repository contract.
        /// </summary>
        private readonly IRepository repository;

        private ITWebClient _webClient;

        /// <summary>
        /// The option service
        /// </summary>
        private readonly IOptionService optionService;

        /// <summary>
        /// The child content service
        /// </summary>
        private readonly IChildContentService childContentService;

        public DefaultContentService(ISecurityService securityService, IRepository repository, IOptionService optionService,
            IChildContentService childContentService)
        {
            this.securityService = securityService;
            this.repository = repository;
            this.optionService = optionService;
            this.childContentService = childContentService;
            _webClient = new TWebClient();
        }

        public Models.Content SaveContentWithStatusUpdate(Models.Content updatedContent, ContentStatus requestedStatus)
        {
            if (updatedContent == null)
            {
                throw new CmsException("Nothing to save.", new ArgumentNullException("updatedContent"));
            }

            if (requestedStatus == ContentStatus.Archived)
            {
                throw new CmsException(string.Format("Can't switch a content to the Archived state directly."));
            }

            // Fill content with dynamic contents info
            UpdateDynamicContainer(updatedContent);

            if (updatedContent.Id == default(Guid))
            {
                /* Just create a new content with requested status.*/
                if (requestedStatus == ContentStatus.Published)
                {
                    if (!updatedContent.PublishedOn.HasValue)
                    {
                        updatedContent.PublishedOn = DateTime.Now;
                    }
                    if (string.IsNullOrWhiteSpace(updatedContent.PublishedByUser))
                    {
                        updatedContent.PublishedByUser = securityService.CurrentPrincipalName;
                    }
                }

                updatedContent.Status = requestedStatus;
                repository.Save(updatedContent);
                //try
                //{
                //    // set flag = 1 to perform delete in pagecategory table
                //    JObject saveitem = new JObject();
                //    saveitem.Add("Id", updatedContent.Id);
                //    saveitem.Add("Version", updatedContent.Version);
                //    saveitem.Add("IsDeleted", updatedContent.IsDeleted);
                //    saveitem.Add("Name", updatedContent.Name);
                //    saveitem.Add("PreviewUrl", updatedContent.PreviewUrl);
                //    saveitem.Add("ModifiedOn", updatedContent.ModifiedOn);
                //    saveitem.Add("ModifiedByUser", securityService.CurrentPrincipalName);
                //    saveitem.Add("Status", updatedContent.Status.ToString());
                //    saveitem.Add("PublishedByUser", updatedContent.PublishedByUser);
                //    if (updatedContent.Original != null)
                //    {
                //        saveitem.Add("OriginalId", updatedContent.Original.Id);
                //    }

                //    string js = JsonConvert.SerializeObject(saveitem);
                //    var model = _webClient.DownloadData<string>("Root/ApiSaveOrUpdateContent", new { JS = js });
                //    //Models.Content original = new Models.Content();

                //    //original.Id = new Guid(model);
                //    //original.Name = updatedContent.Name;
                //    //updatedContent.Original = original;
                //}
                //catch (Exception e)
                //{

                //}

                return updatedContent;
            }
            var originalContent = GetOriginalContent(updatedContent.Id);

            if (originalContent == null)
            {
                throw new EntityNotFoundException(typeof(Models.Content), updatedContent.Id);
            }

            if (originalContent.Original != null)
            {
                originalContent = originalContent.Original;
            }

            originalContent = repository.UnProxy(originalContent);

            if (originalContent.History == null)
            {
                originalContent.History = new List<Models.Content>();
            }

            childContentService.ValidateChildContentsCircularReferences(originalContent, updatedContent);

            /* Update existing content. */
            switch (originalContent.Status)
            {
                case ContentStatus.Published:
                    SavePublishedContentWithStatusUpdate(originalContent, updatedContent, requestedStatus);
                    break;

                case ContentStatus.Preview:
                case ContentStatus.Draft:
                    SavePreviewOrDraftContentWithStatusUpdate(originalContent, updatedContent, requestedStatus);
                    break;

                case ContentStatus.Archived:
                    throw new CmsException(string.Format("Can't edit a content in the {0} state.", originalContent.Status));

                default:
                    throw new CmsException(string.Format("Unknown content status {0}.", updatedContent.Status), new NotSupportedException());
            }

            return originalContent;
        }

        private Models.Content GetOriginalContent(Guid id)
        {
            var content = repository.AsQueryable<Models.Content>().Where(c => c.Id == id).Fetch(c => c.Original).FirstOrDefault();
            //JObject saveitem0 = new JObject();
            //saveitem0.Add("Id", id);
            //string js = JsonConvert.SerializeObject(saveitem0);
            //var model = _webClient.DownloadData<JObject>("Root/ApiSelectContentbyId", new { JS = js });
            //var content1 = new Models.Content();
            //string flag = model["Flag"].ToString();
            //if (flag == "1")
            //{
            //    Blog.Models.BlogPostContent blogpostcontent = new Blog.Models.BlogPostContent();
            //    blogpostcontent.Id = id;
            //    blogpostcontent.Version = (int)model["Version"];
            //    blogpostcontent.IsDeleted = (bool)model["IsDeleted"];
            //    blogpostcontent.CreatedOn = (DateTime)model["CreatedOn"];
            //    blogpostcontent.CreatedByUser = model["CreatedByUser"].ToString();
            //    blogpostcontent.ModifiedOn = (DateTime)model["ModifiedOn"];
            //    blogpostcontent.ModifiedByUser = model["ModifiedByUser"].ToString();
            //    blogpostcontent.Name = model["Name"].ToString();
            //    blogpostcontent.PreviewUrl = model["PreviewUrl"].ToString();
            //    if (model["Status"].ToString() == "Published") { blogpostcontent.Status = ContentStatus.Published; }
            //    else if (model["Status"].ToString() == "Archived") { blogpostcontent.Status = ContentStatus.Archived; }
            //    else if (model["Status"].ToString() == "Draft") { blogpostcontent.Status = ContentStatus.Draft; }
            //    else if (model["Status"].ToString() == "Preview") { blogpostcontent.Status = ContentStatus.Preview; }
            //    blogpostcontent.PublishedByUser = model["PublishedByUser"].ToString();
            //    blogpostcontent.PublishedOn = (DateTime)model["PublishedOn"];
            //    blogpostcontent.ActivationDate = (DateTime)model["ActivationDate"];
            //    if (!string.IsNullOrEmpty(model["ExpirationDate"].ToString()))
            //    {
            //        blogpostcontent.ExpirationDate = (DateTime)model["ExpirationDate"];
            //    }
            //    blogpostcontent.CustomCss = model["CustomCss"].ToString();
            //    blogpostcontent.UseCustomCss = (bool)model["UseCustomCss"];
            //    blogpostcontent.CustomJs = model["CustomJs"].ToString();
            //    blogpostcontent.UseCustomJs = (bool)model["UseCustomJs"];
            //    blogpostcontent.Html = model["Html"].ToString();
            //    blogpostcontent.EditInSourceMode = (bool)model["EditInSourceMode"];
            //    blogpostcontent.OriginalText = model["OriginalText"].ToString();

            //    if (model["ContentTextMode"].ToString() == "1") { blogpostcontent.ContentTextMode = BetterCms.Module.Pages.Models.Enums.ContentTextMode.Html; }
            //    else if (model["ContentTextMode"].ToString() == "2") { blogpostcontent.ContentTextMode = BetterCms.Module.Pages.Models.Enums.ContentTextMode.Markdown; }
            //    else if (model["ContentTextMode"].ToString() == "3") { blogpostcontent.ContentTextMode = BetterCms.Module.Pages.Models.Enums.ContentTextMode.SimpleText; }

            //    if (!string.IsNullOrEmpty(model["OriginalId"].ToString()))
            //    {
            //        JObject saveitem01 = new JObject();
            //        saveitem01.Add("Id", model["OriginalId"].ToString());
            //        string jstring = JsonConvert.SerializeObject(saveitem01);
            //        var originalobject = _webClient.DownloadData<JObject>("Root/ApiFetchContent", new { JS = jstring });
            //        Models.Content originalobj = new Models.Content();
            //        originalobj.Id = new Guid(originalobject["OriginalId"].ToString());
            //        originalobj.Name = originalobject["Name"].ToString();
            //        originalobj.IsDeleted = (bool)originalobject["IsDeleted"];
            //        originalobj.ModifiedByUser = originalobject["ModifiedByUser"].ToString();
            //        if (originalobject["Status"].ToString() == "Published") { originalobj.Status = ContentStatus.Published; }
            //        else if (originalobject["Status"].ToString() == "Archived") { originalobj.Status = ContentStatus.Archived; }
            //        else if (originalobject["Status"].ToString() == "Draft") { originalobj.Status = ContentStatus.Draft; }
            //        else if (originalobject["Status"].ToString() == "Preview") { originalobj.Status = ContentStatus.Preview; }

            //        originalobj.ModifiedOn = (DateTime)originalobject["ModifiedOn"];
            //        originalobj.PreviewUrl = originalobject["PreviewUrl"].ToString();
            //        originalobj.PublishedByUser = originalobject["PublishedByUser"].ToString();
            //        originalobj.PublishedOn = (DateTime)originalobject["PublishedOn"];
            //    }
            //    content1 = blogpostcontent;
            //}

            //if (flag == "2")
            //{
            //    Pages.Models.HtmlContentWidget htmlcontentwidget= new Pages.Models.HtmlContentWidget();
            //    htmlcontentwidget.Id = id;
            //    htmlcontentwidget.Version = (int)model["Version"];
            //    htmlcontentwidget.IsDeleted = (bool)model["IsDeleted"];
            //    htmlcontentwidget.CreatedOn = (DateTime)model["CreatedOn"];
            //    htmlcontentwidget.CreatedByUser = model["CreatedByUser"].ToString();
            //    htmlcontentwidget.ModifiedOn = (DateTime)model["ModifiedOn"];
            //    htmlcontentwidget.ModifiedByUser = model["ModifiedByUser"].ToString();
            //    htmlcontentwidget.Name = model["Name"].ToString();
            //    htmlcontentwidget.PreviewUrl = model["PreviewUrl"].ToString();
            //    if (model["Status"].ToString() == "Published") { htmlcontentwidget.Status = ContentStatus.Published; }
            //    else if (model["Status"].ToString() == "Archived") { htmlcontentwidget.Status = ContentStatus.Archived; }
            //    else if (model["Status"].ToString() == "Draft") { htmlcontentwidget.Status = ContentStatus.Draft; }
            //    else if (model["Status"].ToString() == "Preview") { htmlcontentwidget.Status = ContentStatus.Preview; }
            //    htmlcontentwidget.PublishedByUser = model["PublishedByUser"].ToString();
            //    htmlcontentwidget.PublishedOn = (DateTime)model["PublishedOn"];
            //    htmlcontentwidget.CustomCss = model["CustomCss"].ToString();
            //    htmlcontentwidget.UseCustomCss = (bool)model["UseCustomCss"];
            //    htmlcontentwidget.CustomJs = model["CustomJs"].ToString();
            //    htmlcontentwidget.UseCustomJs = (bool)model["UseCustomJs"];
            //    htmlcontentwidget.Html = model["Html"].ToString();
            //    htmlcontentwidget.EditInSourceMode = (bool)model["EditInSourceMode"];


            //    if (!string.IsNullOrEmpty(model["OriginalId"].ToString()))
            //    {
            //        JObject saveitem01 = new JObject();
            //        saveitem01.Add("Id", model["OriginalId"].ToString());
            //        string jstring = JsonConvert.SerializeObject(saveitem01);
            //        var originalobject = _webClient.DownloadData<JObject>("Root/ApiFetchContent", new { JS = jstring });
            //        Models.Content originalobj = new Models.Content();
            //        originalobj.Id = new Guid(originalobject["OriginalId"].ToString());
            //        originalobj.Name = originalobject["Name"].ToString();
            //        originalobj.IsDeleted = (bool)originalobject["IsDeleted"];
            //        originalobj.ModifiedByUser = originalobject["ModifiedByUser"].ToString();
            //        if (originalobject["Status"].ToString() == "Published") { originalobj.Status = ContentStatus.Published; }
            //        else if (originalobject["Status"].ToString() == "Archived") { originalobj.Status = ContentStatus.Archived; }
            //        else if (originalobject["Status"].ToString() == "Draft") { originalobj.Status = ContentStatus.Draft; }
            //        else if (originalobject["Status"].ToString() == "Preview") { originalobj.Status = ContentStatus.Preview; }

            //        originalobj.ModifiedOn = (DateTime)originalobject["ModifiedOn"];
            //        originalobj.PreviewUrl = originalobject["PreviewUrl"].ToString();
            //        originalobj.PublishedByUser = originalobject["PublishedByUser"].ToString();
            //        originalobj.PublishedOn = (DateTime)originalobject["PublishedOn"];
            //    }
            //    content1 = htmlcontentwidget;
            //}

            //else if (flag == "3")
            //{
            //    Pages.Models.HtmlContent htmlcontent = new Pages.Models.HtmlContent();
            //    htmlcontent.Id = id;
            //    htmlcontent.Version = (int)model["Version"];
            //    htmlcontent.IsDeleted = (bool)model["IsDeleted"];
            //    htmlcontent.CreatedOn = (DateTime)model["CreatedOn"];
            //    htmlcontent.CreatedByUser = model["CreatedByUser"].ToString();
            //    htmlcontent.ModifiedOn = (DateTime)model["ModifiedOn"];
            //    htmlcontent.ModifiedByUser = model["ModifiedByUser"].ToString();
            //    htmlcontent.Name = model["Name"].ToString();
            //    htmlcontent.PreviewUrl = model["PreviewUrl"].ToString();
            //    if (model["Status"].ToString() == "3") { htmlcontent.Status = ContentStatus.Published; }
            //    else if (model["Status"].ToString() == "4") { htmlcontent.Status = ContentStatus.Archived; }
            //    else if (model["Status"].ToString() == "2") { htmlcontent.Status = ContentStatus.Draft; }
            //    else if (model["Status"].ToString() == "1") { htmlcontent.Status = ContentStatus.Preview; }
            //    htmlcontent.PublishedByUser = model["PublishedByUser"].ToString();
            //    htmlcontent.PublishedOn = (DateTime)model["PublishedOn"];
            //    htmlcontent.ActivationDate = (DateTime)model["ActivationDate"];
            //    if (!string.IsNullOrEmpty(model["ExpirationDate"].ToString()))
            //    {
            //        htmlcontent.ExpirationDate = (DateTime)model["ExpirationDate"];
            //    }
            //    htmlcontent.CustomCss = model["CustomCss"].ToString();
            //    htmlcontent.UseCustomCss = (bool)model["UseCustomCss"];
            //    htmlcontent.CustomJs = model["CustomJs"].ToString();
            //    htmlcontent.UseCustomJs = (bool)model["UseCustomJs"];
            //    htmlcontent.Html = model["Html"].ToString();
            //    htmlcontent.EditInSourceMode = (bool)model["EditInSourceMode"];
            //    htmlcontent.OriginalText = model["OriginalText"].ToString();

            //    if (model["ContentTextMode"].ToString() == "1") { htmlcontent.ContentTextMode = BetterCms.Module.Pages.Models.Enums.ContentTextMode.Html; }
            //    else if (model["ContentTextMode"].ToString() == "2") { htmlcontent.ContentTextMode = BetterCms.Module.Pages.Models.Enums.ContentTextMode.Markdown; }
            //    else if (model["ContentTextMode"].ToString() == "3") { htmlcontent.ContentTextMode = BetterCms.Module.Pages.Models.Enums.ContentTextMode.SimpleText; } 

            //    if (!string.IsNullOrEmpty(model["OriginalId"].ToString()))
            //    {
            //        JObject saveitem01 = new JObject();
            //        saveitem01.Add("Id", model["OriginalId"].ToString());
            //        string jstring = JsonConvert.SerializeObject(saveitem01);
            //        var originalobject = _webClient.DownloadData<JObject>("Root/ApiFetchContent", new { JS = jstring });
            //        Models.Content originalobj = new Models.Content();
            //        originalobj.Id = new Guid(originalobject["OriginalId"].ToString());
            //        originalobj.Name = originalobject["Name"].ToString();
            //        originalobj.IsDeleted = (bool)originalobject["IsDeleted"];
            //        originalobj.ModifiedByUser = originalobject["ModifiedByUser"].ToString();
            //        if (originalobject["Status"].ToString() == "Published") { originalobj.Status = ContentStatus.Published; }
            //        else if (originalobject["Status"].ToString() == "Archived") { originalobj.Status = ContentStatus.Archived; }
            //        else if (originalobject["Status"].ToString() == "Draft") { originalobj.Status = ContentStatus.Draft; }
            //        else if (originalobject["Status"].ToString() == "Preview") { originalobj.Status = ContentStatus.Preview; }

            //        originalobj.ModifiedOn = (DateTime)originalobject["ModifiedOn"];
            //        originalobj.PreviewUrl = originalobject["PreviewUrl"].ToString();
            //        originalobj.PublishedByUser = originalobject["PublishedByUser"].ToString();
            //        originalobj.PublishedOn = (DateTime)originalobject["PublishedOn"];
            //    }
            //    content1 = htmlcontent;
            //}


            //IList<Models.Content> IHistoryList = new List<Models.Content>();
            //List<Models.Content> HistoryList = new List<Models.Content>();



            //try
            //{
            //    // set flag = 1 to perform delete in  table
            //    JObject saveitem02 = new JObject();
            //    saveitem02.Add("Id", id);

            //    string jstring02 = JsonConvert.SerializeObject(saveitem02);
            //    var Jarray = _webClient.DownloadData<JArray>("Root/ApiFetchHistoryList", new { JS = jstring02 });
            //    for (int i = 0; i < Jarray.Count; i++)
            //    {

            //        Models.Content originalobj = new Models.Content();
            //        originalobj.Id = new Guid(Jarray[i]["Id"].ToString());
            //        originalobj.Name = Jarray[i]["Name"].ToString();
            //        originalobj.IsDeleted = false;
            //        originalobj.ModifiedByUser = Jarray[i]["ModifiedByUser"].ToString();
            //        originalobj.ModifiedOn = (DateTime)Jarray[i]["ModifiedOn"];
            //        if (Jarray[i]["Status"].ToString() == "Published") { originalobj.Status = ContentStatus.Published; }
            //        else if (Jarray[i]["Status"].ToString() == "Archived") { originalobj.Status = ContentStatus.Archived; }
            //        else if (Jarray[i]["Status"].ToString() == "Draft") { originalobj.Status = ContentStatus.Draft; }
            //        else if (Jarray[i]["Status"].ToString() == "Preview") { originalobj.Status = ContentStatus.Preview; }

            //        originalobj.PreviewUrl = Jarray[i]["PreviewUrl"].ToString();
            //        originalobj.PublishedByUser = Jarray[i]["PublishedByUser"].ToString();
            //        originalobj.PublishedOn = (DateTime)Jarray[i]["PublishedOn"];
            //        HistoryList.Add(originalobj);

            //    }

            //    IHistoryList = HistoryList;
            //    content1.History = IHistoryList;

            //}
            //catch (Exception e)
            //{

            //}

            //IList<ContentOption> IContentOption = new List<ContentOption>();
            //List<ContentOption> ContentOptionList = new List<ContentOption>();


            //try
            //{
            //    // set flag = 1 to perform delete in  table
            //    JObject saveitem03 = new JObject();
            //    saveitem03.Add("Id", id);

            //    string jstring03 = JsonConvert.SerializeObject(saveitem03);
            //    var Jarray01 = _webClient.DownloadData<JArray>("Root/ApiFetchContentoptionList", new { JS = jstring03 });
            //    for (int i = 0; i < Jarray01.Count; i++)
            //    {

            //        ContentOption originalobj = new ContentOption();
            //        originalobj.Id = new Guid(Jarray01[i]["Id"].ToString());
            //        originalobj.Key = Jarray01[i]["Key"].ToString();
            //        originalobj.IsDeletable = (bool)Jarray01[i]["IsDeletable"];
            //        originalobj.DefaultValue = Jarray01[i]["DefaultValue"].ToString();
            //        CustomOption customoption = new CustomOption();
            //        customoption.Id = new Guid(Jarray01[i]["CustomOptionId"].ToString());
            //        originalobj.CustomOption = customoption;
            //        if (Jarray01[i]["Type"].ToString() == "1") { originalobj.Type = OptionType.Text; }
            //        else if (Jarray01[i]["Type"].ToString() == "21") { originalobj.Type = OptionType.MultilineText; }
            //        else if (Jarray01[i]["Type"].ToString() == "51") { originalobj.Type = OptionType.JavaScriptUrl; }
            //        else if (Jarray01[i]["Type"].ToString() == "2") { originalobj.Type = OptionType.Integer; }
            //        else if (Jarray01[i]["Type"].ToString() == "3") { originalobj.Type = OptionType.Float; }
            //        else if (Jarray01[i]["Type"].ToString() == "4") { originalobj.Type = OptionType.DateTime; }
            //        else if (Jarray01[i]["Type"].ToString() == "99") { originalobj.Type = OptionType.Custom; }
            //        else if (Jarray01[i]["Type"].ToString() == "52") { originalobj.Type = OptionType.CssUrl; }
            //        else if (Jarray01[i]["Type"].ToString() == "5") { originalobj.Type = OptionType.Boolean; }
            //        ContentOptionList.Add(originalobj);
            //    }

            //    IContentOption = ContentOptionList;
            //    content1.ContentOptions = IContentOption;

            //}
            //catch (Exception e)
            //{

            //}
            //IList<ChildContent> IChildContentList = new List<ChildContent>();
            //List<ChildContent> ChildContentList = new List<ChildContent>();

            //try
            //{
            //    // set flag = 1 to perform delete in  table
            //    JObject saveitem04 = new JObject();
            //    saveitem04.Add("Id", id);

            //    string jstring04 = JsonConvert.SerializeObject(saveitem04);
            //    var Jarray02 = _webClient.DownloadData<JArray>("Root/ApiFetchChildContentList", new { JS = jstring04 });
            //    for (int i = 0; i < Jarray02.Count; i++)
            //    {
            //        ChildContent child = new ChildContent();
            //        child.Parent = content1;
            //        Models.Content childcontent = new Models.Content();
            //        childcontent.Name = Jarray02[i]["Name"].ToString();
            //        childcontent.IsDeleted = (bool)Jarray02[i]["IsDeleted"];
            //        childcontent.ModifiedByUser = Jarray02[i]["ModifiedByUser"].ToString();
            //        childcontent.ModifiedOn = (DateTime)Jarray02[i]["ModifiedOn"];
            //        childcontent.PreviewUrl = Jarray02[i]["PreviewUrl"].ToString();
            //        if (Jarray02[i]["Status"].ToString() == "Published") { childcontent.Status = ContentStatus.Published; }
            //        else if (Jarray02[i]["Status"].ToString() == "Archived") { childcontent.Status = ContentStatus.Archived; }
            //        else if (Jarray02[i]["Status"].ToString() == "Draft") { childcontent.Status = ContentStatus.Draft; }
            //        else if (Jarray02[i]["Status"].ToString() == "Preview") { childcontent.Status = ContentStatus.Preview; }
            //        childcontent.Version = (int)Jarray02[i]["Version"];
            //        childcontent.PublishedByUser = Jarray02[i]["PublishedByUser"].ToString();
            //        childcontent.PublishedOn = (DateTime)Jarray02[i]["PublishedOn"];
            //        child.Child = childcontent;
            //        ChildContentList.Add(child);
            //    }

            //    IChildContentList = ChildContentList;
            //    content1.ChildContents = IChildContentList;
            //}
            //catch (Exception e)
            //{

            //}


            //try
            //{
            //    IList<ContentRegion> IContentRegionList = new List<ContentRegion>();
            //    List<ContentRegion> ContentRegionList = new List<ContentRegion>();

            //    // set flag = 1 to perform delete in  table
            //    JObject saveitem05 = new JObject();
            //    saveitem05.Add("Id", id);

            //    string jstring05 = JsonConvert.SerializeObject(saveitem05);
            //    var Jarray03 = _webClient.DownloadData<JArray>("Root/ApiFetchContentRegionList", new { JS = jstring05 });
            //    for (int i = 0; i < Jarray03.Count; i++)
            //    {
            //        ContentRegion contentregion = new ContentRegion();
            //        contentregion.Content = content1;


            //        Region region = new Region();
            //        region.Id = new Guid(Jarray03[i]["Id"].ToString());
            //        region.RegionIdentifier = Jarray03[i]["RegionIdentifier"].ToString();
            //        JObject saveitem06 = new JObject();
            //        saveitem06.Add("Id", region.Id);
            //        IList<LayoutRegion> ILayoutRegionList = new List<LayoutRegion>();
            //        List<LayoutRegion> LayoutRegionList = new List<LayoutRegion>();

            //        string jstring06 = JsonConvert.SerializeObject(saveitem06);
            //        var Jarray04 = _webClient.DownloadData<JArray>("Root/ApiFetchLayoutRegionList", new { JS = jstring06 });
            //        for (int j = 0; j < Jarray04.Count; j++)
            //        {
            //            LayoutRegion layoutregion = new LayoutRegion();
            //            layoutregion.Id = new Guid(Jarray04[j]["Id"].ToString());
            //            layoutregion.IsDeleted = (bool)Jarray04[j]["IsDeleted"];
            //            layoutregion.ModifiedOn = (DateTime)Jarray04[j]["ModifiedOn"];
            //            layoutregion.ModifiedByUser = Jarray04[j]["ModifiedByUser"].ToString();
            //            layoutregion.CreatedOn = (DateTime)Jarray04[j]["CreatedOn"];
            //            layoutregion.CreatedByUser = Jarray04[j]["ModifiedByUser"].ToString();
            //            Layout layout = new Layout();
            //            layout.Id = new Guid(Jarray04[j]["LayoutId"].ToString());
            //            layoutregion.Layout = layout;
            //            Region subregion = new Region();
            //            subregion.Id = new Guid(Jarray04[j]["RegionId"].ToString());
            //            layoutregion.Region = subregion;
            //            LayoutRegionList.Add(layoutregion);
            //        }
            //        ILayoutRegionList = LayoutRegionList;
            //        region.LayoutRegion = ILayoutRegionList;

            //        IList<PageContent> IPageContentList = new List<PageContent>();
            //        List<PageContent> PageContentList = new List<PageContent>();
            //        JObject saveitem07 = new JObject();
            //        saveitem07.Add("Id", region.Id);
            //        string jstring07 = JsonConvert.SerializeObject(saveitem07);
            //        var Jarray05 = _webClient.DownloadData<JArray>("Root/ApiFetchPageContentList", new { JS = jstring07 });
            //        for (int k = 0; k < Jarray05.Count; k++)
            //        {
            //            PageContent pagecontent = new PageContent();
            //            pagecontent.Id = new Guid(Jarray05[k]["Id"].ToString());
            //            pagecontent.IsDeleted = (bool)Jarray05[k]["IsDeleted"];
            //            pagecontent.ModifiedOn = (DateTime)Jarray05[k]["ModifiedOn"];
            //            pagecontent.ModifiedByUser = Jarray05[k]["ModifiedByUser"].ToString();
            //            pagecontent.CreatedOn = (DateTime)Jarray05[k]["CreatedOn"];
            //            pagecontent.CreatedByUser = Jarray05[k]["ModifiedByUser"].ToString();
            //            Page page = new Page();
            //            page.Id = new Guid(Jarray05[k]["PageId"].ToString());
            //            pagecontent.Page = page;
            //            PageContent parent = new PageContent();
            //            parent.Id = new Guid(Jarray05[k]["ParentPageContentId"].ToString());
            //            pagecontent.Parent = parent;
            //            pagecontent.Order = (int)Jarray05[k]["Order"];
            //            PageContentList.Add(pagecontent);

            //        }
            //        IPageContentList = PageContentList;
            //        region.PageContents = IPageContentList;
            //        ContentRegionList.Add(contentregion);

            //    }

            //    IContentRegionList = ContentRegionList;
            //    content1.ContentRegions = IContentRegionList;
            //}
            //catch (Exception e)
            //{

            //}
            if (content == null)
            {
                throw new EntityNotFoundException(typeof(Models.Content), id);
            }
            var historyFuture = repository.AsQueryable<Models.Content>().Where(c => c.Original.Id == content.Id).ToFuture();
            var contentOptionsFuture = repository.AsQueryable<ContentOption>().Where(co => co.Content.Id == content.Id).FetchMany(co => co.Translations).ToFuture();
            var childContentsFuture = repository.AsQueryable<ChildContent>().Where(cc => cc.Parent.Id == content.Id).ToFuture();
            var contentRegionsFuture = repository.AsQueryable<ContentRegion>().Where(cr => cr.Content.Id == content.Id).Fetch(cr => cr.Region).ToFuture();
            if (content.Original != null)
            {
                var originalHistoryFuture = repository.AsQueryable<Models.Content>().Where(c => c.Original.Id == content.Original.Id).ToFuture();
                var originalContentOptionsFuture = repository.AsQueryable<ContentOption>().Where(co => co.Content.Id == content.Original.Id).FetchMany(co => co.Translations).ToFuture();

                content.Original.History = originalHistoryFuture as IList<Models.Content> ?? originalHistoryFuture.ToList();
                content.Original.ContentOptions = originalContentOptionsFuture as IList<ContentOption> ?? originalContentOptionsFuture.ToList();
            }
            content.History = historyFuture as IList<Models.Content> ?? historyFuture.ToList();
            content.ContentOptions = contentOptionsFuture as IList<ContentOption> ?? contentOptionsFuture.ToList();
            content.ChildContents = childContentsFuture as IList<ChildContent> ?? childContentsFuture.ToList();
            content.ContentRegions = contentRegionsFuture as IList<ContentRegion> ?? contentRegionsFuture.ToList();

            return content;
        }

        private void SavePublishedContentWithStatusUpdate(Models.Content originalContent, Models.Content updatedContent, ContentStatus requestedStatus)
        {
            /*
             * Edit published content:
             * -> Save as draft, preview - look for draft|preview version in history list or create a new history version with requested status (draft, preview) with reference to an original content.
             * -> Publish - current published version should be cloned to archive version with reference to original (archive state) and original updated with new data (published state).
             *              Look for preview|draft versions - if exists remote it.
             */
            if (requestedStatus == ContentStatus.Preview || requestedStatus == ContentStatus.Draft)
            {
                var contentVersionOfRequestedStatus = originalContent.History.FirstOrDefault(f => f.Status == requestedStatus && !f.IsDeleted);
                if (contentVersionOfRequestedStatus == null)
                {
                    contentVersionOfRequestedStatus = originalContent.Clone(copyCollections: false);
                }

                updatedContent.CopyDataTo(contentVersionOfRequestedStatus, false);
                SetCategories(contentVersionOfRequestedStatus, updatedContent);
                SetContentOptions(contentVersionOfRequestedStatus, updatedContent);
                SetContentRegions(contentVersionOfRequestedStatus, updatedContent);
                childContentService.CopyChildContents(contentVersionOfRequestedStatus, updatedContent);

                contentVersionOfRequestedStatus.Original = originalContent;
                contentVersionOfRequestedStatus.Status = requestedStatus;
                originalContent.History.Add(contentVersionOfRequestedStatus);

                repository.Save(contentVersionOfRequestedStatus);
                //try
                //{
                //    // set flag = 1 to perform delete in pagecategory table
                //    JObject saveitem = new JObject();
                //    saveitem.Add("Id", updatedContent.Id);
                //    saveitem.Add("Version", updatedContent.Version);
                //    saveitem.Add("IsDeleted", updatedContent.IsDeleted);
                //    saveitem.Add("Name", updatedContent.Name);
                //    saveitem.Add("PreviewUrl", updatedContent.PreviewUrl);
                //    saveitem.Add("ModifiedOn", updatedContent.ModifiedOn);
                //    saveitem.Add("ModifiedByUser", securityService.CurrentPrincipalName);
                //    saveitem.Add("Status", updatedContent.Status.ToString());
                //    saveitem.Add("PublishedByUser", updatedContent.PublishedByUser);
                //    if (updatedContent.Original != null)
                //    {
                //        saveitem.Add("OriginalId", updatedContent.Original.Id);
                //    }

                //    string js = JsonConvert.SerializeObject(saveitem);
                //    var model = _webClient.DownloadData<string>("Root/ApiSaveOrUpdateContent", new { JS = js });
                //    //Models.Content original = new Models.Content();

                //    //original.Id = new Guid(model);
                //    //original.Name = updatedContent.Name;
                //    //updatedContent.Original = original;
                //}
                //catch (Exception e)
                //{

                //}
            }

            if (requestedStatus == ContentStatus.Published)
            {
                // Original is copied with options and saved.
                // Removes options from original.
                // Locks new stuff from view model.
                var originalToArchive = originalContent.Clone();
                originalToArchive.Status = ContentStatus.Archived;
                originalToArchive.Original = originalContent;
                originalContent.History.Add(originalToArchive);
                repository.Save(originalToArchive);
                //try
                //{
                //    // set flag = 1 to perform delete in pagecategory table
                //    JObject saveitem = new JObject();
                //    saveitem.Add("Id", originalToArchive.Id);
                //    saveitem.Add("Version", originalToArchive.Version);
                //    saveitem.Add("IsDeleted", originalToArchive.IsDeleted);
                //    saveitem.Add("Name", originalToArchive.Name);
                //    saveitem.Add("PreviewUrl", originalToArchive.PreviewUrl);
                //    saveitem.Add("ModifiedOn", originalToArchive.ModifiedOn);
                //    saveitem.Add("ModifiedByUser", securityService.CurrentPrincipalName);
                //    saveitem.Add("Status", originalToArchive.Status.ToString());
                //    saveitem.Add("PublishedByUser", originalToArchive.PublishedByUser);
                //    if (updatedContent.Original != null)
                //    {
                //        saveitem.Add("OriginalId", originalToArchive.Original.Id);
                //    }

                //    string js = JsonConvert.SerializeObject(saveitem);
                //    var model = _webClient.DownloadData<string>("Root/ApiSaveOrUpdateContent", new { JS = js });
                //    //originalToArchive.Id = new Guid(model);
                //    //Models.Content originaltoarchive = new Models.Content();

                //    //originaltoarchive.Id = new Guid(model);
                //    //originaltoarchive.Name = originalToArchive.Name;
                //    //originalToArchive.Original = originaltoarchive;
                //}
                //catch (Exception e)
                //{

                //}


                // Load draft content's child contents options, if saving from draft to public
                var draftVersion = originalContent.History.FirstOrDefault(f => f.Status == ContentStatus.Draft && !f.IsDeleted);
                if (draftVersion != null)
                {
                    updatedContent
                        .ChildContents
                        .ForEach(cc => cc.Options = draftVersion
                            .ChildContents
                            .Where(cc1 => cc1.AssignmentIdentifier == cc.AssignmentIdentifier)
                            .SelectMany(cc1 => cc1.Options)
                            .ToList());
                }

                updatedContent.CopyDataTo(originalContent, false);
                SetCategories(originalContent, updatedContent);
                SetContentOptions(originalContent, updatedContent);
                SetContentRegions(originalContent, updatedContent);
                childContentService.CopyChildContents(originalContent, updatedContent);

                originalContent.Status = requestedStatus;
                if (!originalContent.PublishedOn.HasValue)
                {
                    originalContent.PublishedOn = DateTime.Now;
                }
                if (string.IsNullOrWhiteSpace(originalContent.PublishedByUser))
                {
                    originalContent.PublishedByUser = securityService.CurrentPrincipalName;
                }
                repository.Save(originalContent);
                //try
                //{
                //    // set flag = 1 to perform delete in pagecategory table
                //    JObject saveitem = new JObject();
                //    saveitem.Add("Id", originalContent.Id);
                //    saveitem.Add("Version", originalContent.Version);
                //    saveitem.Add("IsDeleted", originalContent.IsDeleted);
                //    saveitem.Add("Name", originalContent.Name);
                //    saveitem.Add("PreviewUrl", originalContent.PreviewUrl);
                //    saveitem.Add("ModifiedOn", originalContent.ModifiedOn);
                //    saveitem.Add("ModifiedByUser", securityService.CurrentPrincipalName);
                //    saveitem.Add("Status", originalContent.Status.ToString());
                //    saveitem.Add("PublishedByUser", originalContent.PublishedByUser);
                //    if (originalContent.Original != null)
                //    {
                //        saveitem.Add("OriginalId", originalContent.Original.Id);
                //    }

                //    string js = JsonConvert.SerializeObject(saveitem);
                //    var model = _webClient.DownloadData<string>("Root/ApiSaveOrUpdateContent", new { JS = js });
                //    //Models.Content originalcontent = new Models.Content();

                //    //originalcontent.Id = new Guid(model);
                //    //originalcontent.Name = originalContent.Name;
                //    //originalContent.Original = originalcontent;
                //}
                //catch (Exception e)
                //{

                //}

                IList<Models.Content> contentsToRemove = originalContent.History.Where(f => f.Status == ContentStatus.Preview || f.Status == ContentStatus.Draft).ToList();
                foreach (var redundantContent in contentsToRemove)
                {
                    repository.Delete(redundantContent);
                    originalContent.History.Remove(redundantContent);
                }
            }
        }

        private void SetCategories(Models.Content destinationContent, Models.Content sourceContent)
        {
            var destination = destinationContent as ICategorized;
            var source = sourceContent as ICategorized;
            if (destination == null || source == null)
            {
                return;
            }

            if (destination.Categories != null)
            {
                var categoriesToRemove = destination.Categories.ToList();
                categoriesToRemove.ForEach(repository.Delete);
            }

            if (source.Categories == null)
            {
                return;
            }

            source.Categories.ForEach(destination.AddCategory);
            if (destination.Categories != null)
            {
                destination.Categories.ForEach(e => e.SetEntity(destinationContent));
            }
        }

        private void SavePreviewOrDraftContentWithStatusUpdate(Models.Content originalContent, Models.Content updatedContent, ContentStatus requestedStatus)
        {
            /*
             * Edit preview or draft content:
             * -> Save as preview or draft - look for preview or draft version in a history list or create a new history version with requested preview status with reference to an original content.
             * -> Save draft - just update field and save.
             * -> Publish - look if the published content (look for original) exists:
             *              - published content exits:
             *                  | create a history content version of the published (clone it). update original with draft data and remove draft|preview.
             *              - published content not exists:
             *                  | save draft content as published
             */
            if (requestedStatus == ContentStatus.Preview || requestedStatus == ContentStatus.Draft)
            {
                var previewOrDraftContentVersion = originalContent.History.FirstOrDefault(f => f.Status == requestedStatus && !f.IsDeleted);
                if (previewOrDraftContentVersion == null)
                {
                    if (originalContent.Status == requestedStatus
                        || (originalContent.Status == ContentStatus.Preview && requestedStatus == ContentStatus.Draft))
                    {
                        previewOrDraftContentVersion = originalContent;
                    }
                    else
                    {
                        previewOrDraftContentVersion = originalContent.Clone();
                        previewOrDraftContentVersion.Original = originalContent;
                        originalContent.History.Add(previewOrDraftContentVersion);
                    }
                }

                updatedContent.CopyDataTo(previewOrDraftContentVersion, false);
                SetCategories(previewOrDraftContentVersion, updatedContent);
                SetContentOptions(previewOrDraftContentVersion, updatedContent);
                SetContentRegions(previewOrDraftContentVersion, updatedContent);
                childContentService.CopyChildContents(previewOrDraftContentVersion, updatedContent);

                previewOrDraftContentVersion.Status = requestedStatus;

                 repository.Save(previewOrDraftContentVersion);
                //try
                //{
                //    // set flag = 1 to perform delete in pagecategory table
                //    JObject saveitem = new JObject();
                //    saveitem.Add("Id", previewOrDraftContentVersion.Id);
                //    saveitem.Add("Version", previewOrDraftContentVersion.Version);
                //    saveitem.Add("IsDeleted", previewOrDraftContentVersion.IsDeleted);
                //    saveitem.Add("Name", previewOrDraftContentVersion.Name);
                //    saveitem.Add("PreviewUrl", previewOrDraftContentVersion.PreviewUrl);
                //    saveitem.Add("ModifiedOn", previewOrDraftContentVersion.ModifiedOn);
                //    saveitem.Add("ModifiedByUser", securityService.CurrentPrincipalName);
                //    saveitem.Add("Status", previewOrDraftContentVersion.Status.ToString());
                //    saveitem.Add("PublishedByUser", previewOrDraftContentVersion.PublishedByUser);
                //    if (updatedContent.Original != null)
                //    {
                //        saveitem.Add("OriginalId", previewOrDraftContentVersion.Original.Id);
                //    }

                //    string js = JsonConvert.SerializeObject(saveitem);
                //    var model = _webClient.DownloadData<string>("Root/ApiSaveOrUpdateContent", new { JS = js });
                //    //Models.Content originalcontentpreview = new Models.Content();

                //    //originalcontentpreview.Id = new Guid(model);
                //    //originalcontentpreview.Name = previewOrDraftContentVersion.Name;
                //    //previewOrDraftContentVersion.Original = originalcontentpreview;
                //}
                //catch (Exception e)
                //{

                //}
            }
            else if (requestedStatus == ContentStatus.Published)
            {
                var publishedVersion = originalContent.History.FirstOrDefault(f => f.Status == requestedStatus && !f.IsDeleted);
                if (publishedVersion != null)
                {
                    var originalToArchive = originalContent.Clone();
                    originalToArchive.Status = ContentStatus.Archived;
                    originalToArchive.Original = originalContent;
                    originalContent.History.Add(originalToArchive);
                    repository.Save(originalToArchive);
                }

                updatedContent.CopyDataTo(originalContent, false);
                SetCategories(originalContent, updatedContent);
                SetContentOptions(originalContent, updatedContent);
                SetContentRegions(originalContent, updatedContent);
                childContentService.CopyChildContents(originalContent, updatedContent);

                originalContent.Status = requestedStatus;
                if (!originalContent.PublishedOn.HasValue)
                {
                    originalContent.PublishedOn = DateTime.Now;
                }
                if (string.IsNullOrWhiteSpace(originalContent.PublishedByUser))
                {
                    originalContent.PublishedByUser = securityService.CurrentPrincipalName;
                }

                 repository.Save(originalContent);
                //try
                //{
                //    // set flag = 1 to perform delete in pagecategory table
                //    JObject saveitem = new JObject();
                //    saveitem.Add("Id", originalContent.Id);
                //    saveitem.Add("Version", originalContent.Version);
                //    saveitem.Add("IsDeleted", originalContent.IsDeleted);
                //    saveitem.Add("Name", originalContent.Name);
                //    saveitem.Add("PreviewUrl", originalContent.PreviewUrl);
                //    saveitem.Add("ModifiedOn", originalContent.ModifiedOn);
                //    saveitem.Add("ModifiedByUser", securityService.CurrentPrincipalName);
                //    saveitem.Add("Status", originalContent.Status.ToString());
                //    saveitem.Add("PublishedByUser", originalContent.PublishedByUser);
                //    if (originalContent.Original != null)
                //    {
                //        saveitem.Add("OriginalId", originalContent.Original.Id);
                //    }

                //    string js = JsonConvert.SerializeObject(saveitem);
                //    var model = _webClient.DownloadData<string>("Root/ApiSaveOrUpdateContent", new { JS = js });
                //    //Models.Content originalcontents = new Models.Content();

                //    //originalcontents.Id = new Guid(model);
                //    //originalcontents.Name = originalContent.Name;
                //    //originalContent.Original = originalcontents;
                //}
                //catch (Exception e)
                //{

                //}
            }
        }

        public Models.Content RestoreContentFromArchive(Models.Content restoreFrom)
        {
            if (restoreFrom == null)
            {
                throw new CmsException("Nothing to restore from.", new ArgumentNullException("restoreFrom"));
            }

            if (restoreFrom.Status != ContentStatus.Archived)
            {
                throw new CmsException("A page content can be restored only from the archived version.");
            }

            // Replace original version with restored entity data
            var originalContent = restoreFrom.Clone();
            originalContent.Id = restoreFrom.Original.Id;
            originalContent.Version = restoreFrom.Original.Version;
            originalContent.Status = ContentStatus.Published;
            originalContent.Original = null;
            originalContent.ChildContentsLoaded = true;

            // Save entities
            return SaveContentWithStatusUpdate(originalContent, ContentStatus.Published);
        }

        public System.Tuple<PageContent, Models.Content> GetPageContentForEdit(Guid pageContentId)
        {
            PageContent pageContent = repository.AsQueryable<PageContent>()
                                  .Where(p => p.Id == pageContentId && !p.IsDeleted)
                                  .Fetch(p => p.Content).ThenFetchMany(p => p.History)
                                  .Fetch(p => p.Page)
                                  .Fetch(f => f.Region)
                                  .ToList()
                                  .FirstOrDefault();

            //PageContent pageContent = new PageContent();
            //JObject saveitem00 = new JObject();
            //saveitem00.Add("Id", pageContentId);
            //string js = JsonConvert.SerializeObject(saveitem00);
            //var model = _webClient.DownloadData<JObject>("Root/ApiFetchPagecontent", new { JS = js });

            //pageContent.Id = new Guid(model["Id"].ToString());
            //pageContent.IsDeleted = (bool)model["IsDeleted"];
            //pageContent.ModifiedOn = (DateTime)model["ModifiedOn"];
            //pageContent.ModifiedByUser = model["ModifiedByUser"].ToString();
            //pageContent.CreatedOn = (DateTime)model["CreatedOn"];
            //pageContent.CreatedByUser = model["ModifiedByUser"].ToString();
            //Page page = new Blog.Models.BlogPost();
            //page.Id = new Guid(model["PageId"].ToString());
            //Models.Content contentsub = new Pages.Models.HtmlContent();
            //contentsub.Id = new Guid(model["ContentId"].ToString());
            //JObject saveitem01 = new JObject();
            //saveitem01.Add("Id", model["ContentId"].ToString());
            //string jstring00 = JsonConvert.SerializeObject(saveitem01);
            //var contentmodel = _webClient.DownloadData<JObject>("Root/ApiFetchContent", new { JS = jstring00 });

            //contentsub.Name = contentmodel["Name"].ToString();
            //contentsub.IsDeleted = (bool)contentmodel["IsDeleted"];
            //contentsub.ModifiedByUser = contentmodel["ModifiedByUser"].ToString();
            //contentsub.ModifiedOn = (DateTime)contentmodel["ModifiedOn"];
            //contentsub.PreviewUrl = contentmodel["PreviewUrl"].ToString();
            //if (contentmodel["Status"].ToString() == "Published") { contentsub.Status = ContentStatus.Published; }
            //else if (model["Status"].ToString() == "Archived") { contentsub.Status = ContentStatus.Archived; }
            //else if (model["Status"].ToString() == "Draft") { contentsub.Status = ContentStatus.Draft; }
            //else if (model["Status"].ToString() == "Preview") { contentsub.Status = ContentStatus.Preview; }
            //contentsub.Version = (int)contentmodel["Version"];
            //contentsub.PublishedByUser = contentmodel["PublishedByUser"].ToString();
            //contentsub.PublishedOn = (DateTime)contentmodel["PublishedOn"];
            //if (!string.IsNullOrEmpty(contentmodel["OriginalId"].ToString()))
            //{
            //    JObject saveitem02 = new JObject();
            //    saveitem02.Add("Id", model["OriginalId"].ToString());
            //    string jstring = JsonConvert.SerializeObject(saveitem02);
            //    var originalobject = _webClient.DownloadData<JObject>("Root/ApiFetchContent", new { JS = jstring });
            //    Models.Content originalobj = new Models.Content();
            //    originalobj.Id = new Guid(originalobject["OriginalId"].ToString());
            //    originalobj.Name = originalobject["Name"].ToString();
            //    originalobj.IsDeleted = (bool)originalobject["IsDeleted"];
            //    originalobj.ModifiedByUser = originalobject["ModifiedByUser"].ToString();
            //    originalobj.ModifiedOn = (DateTime)originalobject["ModifiedOn"];
            //    originalobj.PreviewUrl = originalobject["PreviewUrl"].ToString();
            //    originalobj.PublishedByUser = originalobject["PublishedByUser"].ToString();
            //    originalobj.PublishedOn = (DateTime)originalobject["PublishedOn"];
            //    contentsub.Original = originalobj;
            //}

            //pageContent.Content = contentsub;
            //Region regionsub = new Region();
            //regionsub.Id = new Guid(model["RegionId"].ToString());
            //JObject saveitem03 = new JObject();
            //saveitem03.Add("Id", model["RegionId"].ToString());
            //string jstring01 = JsonConvert.SerializeObject(saveitem03);
            //var regionobject = _webClient.DownloadData<JObject>("Root/ApiFetchRegion", new { JS = jstring01 });
            //regionsub.Version = (int)regionobject["Version"];
            //regionsub.IsDeleted = false;
            //regionsub.ModifiedByUser = regionobject["ModifiedByUser"].ToString();
            //regionsub.ModifiedOn = (DateTime)regionobject["ModifiedOn"];
            //regionsub.RegionIdentifier = regionobject["RegionIdentifier"].ToString();
            //pageContent.Region = regionsub;
            //pageContent.Page = page;
            //PageContent parent = new PageContent();
            //if (!string.IsNullOrEmpty(model["ParentPageContentId"].ToString()))
            //{
            //    parent.Id = new Guid(model["ParentPageContentId"].ToString());
            //}


            //JObject requestforregionpagecontent = new JObject();
            //requestforregionpagecontent.Add("regionId", model["RegionId"].ToString());
            //string regionpagecontentobj = JsonConvert.SerializeObject(requestforregionpagecontent);
            //var regionpagecontentmodel = _webClient.DownloadData<JArray>("Root/ApiRegionPageContentDetails", new { Js = regionpagecontentobj });

            //IList<PageContent> pagecontents = new List<PageContent>();
            //List<PageContent> pagecontentslist = new List<PageContent>();
            //for (int k = 0; k < regionpagecontentmodel.Count; k++)
            //{
            //    PageContent regionpagecontent = new PageContent();
            //    regionpagecontent.Id = new Guid(regionpagecontentmodel[k]["PageContentId"].ToString());
            //    regionpagecontent.Version = Convert.ToInt32(regionpagecontentmodel[k]["Version"]);
            //    regionpagecontent.IsDeleted = (bool)regionpagecontentmodel[k]["IsDeleted"];
            //    regionpagecontent.CreatedOn = (DateTime)regionpagecontentmodel[k]["CreatedOn"];
            //    regionpagecontent.CreatedByUser = regionpagecontentmodel[k]["CreatedByUser"].ToString();
            //    regionpagecontent.ModifiedByUser = regionpagecontentmodel[k]["ModifiedByUser"].ToString();
            //    regionpagecontent.ModifiedOn = (DateTime)regionpagecontentmodel[k]["ModifiedOn"];
            //    if (!string.IsNullOrEmpty(regionpagecontentmodel[k]["DeletedOn"].ToString()))
            //    {
            //        regionpagecontent.DeletedOn = (DateTime)regionpagecontentmodel[k]["DeletedOn"];
            //    }
            //    else
            //    {
            //        regionpagecontent.DeletedOn = null;
            //    }
            //    regionpagecontent.DeletedByUser = (!string.IsNullOrEmpty(regionpagecontentmodel[k]["DeletedByUser"].ToString())) ? regionpagecontentmodel[k]["DeletedByUser"].ToString() : null;
            //    regionpagecontent.Order = Convert.ToInt32(regionpagecontentmodel[k]["Order"]);
            //    BlogPost regionpagedetails = new BlogPost();
            //    regionpagedetails.Id = new Guid(regionpagecontentmodel[k]["PageId"].ToString());
            //    regionpagecontent.Page = regionpagedetails;
            //    HtmlContentWidget regioncontentdetails = new HtmlContentWidget();
            //    //BlogPostContent regioncontentdetails= new BlogPostContent();
            //    regioncontentdetails.Id = new Guid(regionpagecontentmodel[k]["ContentId"].ToString());
            //    //blogpostcontentdetails
            //    //Start
            //    JObject requestforcontentdetails = new JObject();
            //    requestforcontentdetails.Add("contentId", regioncontentdetails.Id);


            //    string contentobj = JsonConvert.SerializeObject(requestforcontentdetails);
            //    var contentmodel = _webClient.DownloadData<string>("Blog/GetContentDetailsForLayout", new { Js = contentobj });
            //    dynamic contentInfo = JObject.Parse(contentmodel);


            //    regioncontentdetails.Name = contentInfo.Name;
            //    regioncontentdetails.IsDeleted = contentInfo.IsDeleted;
            //    regioncontentdetails.Status = contentInfo.Status;
            //    regioncontentdetails.Version = contentInfo.Version;
            //    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //    {

            //        regioncontentdetails.Original.Id = new Guid(contentInfo.OriginalId.ToString());
            //    }
            //    else
            //    {
            //        regioncontentdetails.Original = null;
            //    }
            //    regioncontentdetails.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //    regioncontentdetails.CreatedByUser = contentInfo.CreatedByUser;
            //    regioncontentdetails.CreatedOn = contentInfo.CreatedOn;
            //    regioncontentdetails.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //    regioncontentdetails.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //    regioncontentdetails.ModifiedByUser = contentInfo.ModifiedByUser;
            //    regioncontentdetails.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //    regioncontentdetails.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;




            //    regionpagecontent.Content = regioncontentdetails;


            //    pagecontentslist.Add(regionpagecontent);
            //}

            //pageContent.Parent = parent;
            //pageContent.Order = (int)model["Order"];







            if (pageContent != null)
            {
                Models.Content content = pageContent.Content.FindEditableContentVersion();

                if (content == null)
                {
                    return null;
                }

                return new System.Tuple<PageContent, Models.Content>(pageContent, content);
            }

            return null;
        }

        public Models.Content GetContentForEdit(Guid contentId)
        {
            Models.Content content = repository.AsQueryable<Models.Content>()
                                  .Where(p => p.Id == contentId && !p.IsDeleted)
                                  .FetchMany(p => p.History)
                                  .FetchMany(p => p.ContentRegions)
                                  .ThenFetch(cr => cr.Region)
                                  .ToList()
                                  .FirstOrDefault();

            if (content != null)
            {
                return content.FindEditableContentVersion();
            }

            return null;
        }

        public int GetPageContentNextOrderNumber(Guid pageId, Guid? parentPageContentId)
        {
            var page = repository.AsProxy<Page>(pageId);
            PageContent parent = parentPageContentId.HasValue && !parentPageContentId.Value.HasDefaultValue()
                ? repository.AsProxy<PageContent>(parentPageContentId.Value) : null;

            var max = repository
                .AsQueryable<PageContent>()
                .Where(f => f.Page == page && !f.IsDeleted && f.Parent == parent)
                .Select(f => (int?)f.Order)
                .Max();

            if (max == null)
            {
                return 0;
            }

            return max.Value + 1;
        }

        public void PublishDraftContent(Guid pageId)
        {
            var pageContents = repository.AsQueryable<PageContent>()
                .Where(content => content.Page.Id == pageId)
                .Fetch(f => f.Content)
                .ThenFetchMany(f => f.History)
                .ToList();

            var draftContents = pageContents
                .Where(
                    content =>
                    (content.Content.Status == ContentStatus.Draft
                     && (content.Content.History == null || content.Content.History.All(content1 => content1.Status != ContentStatus.Published)))
                    || (content.Content.Status != ContentStatus.Published
                        && content.Content.History.All(content1 => content1.Status != ContentStatus.Published)
                        && content.Content.History.Any(content1 => content1.Status == ContentStatus.Draft)))
                .ToList();

            foreach (var pageContent in draftContents.Where(pageContent => !(pageContent.Content is Widget)))
            {
                var draftContent = pageContent.Content.FindEditableContentVersion();
                if (draftContent != null)
                {
                    pageContent.Content = SaveContentWithStatusUpdate(draftContent, ContentStatus.Published);
                    repository.Save(pageContent);
                }
            }
        }

        private void CollectDynamicRegions(string html, Models.Content content, IList<ContentRegion> contentRegions)
        {
            var regionIdentifiers = GetRegionIds(html);

            if (regionIdentifiers.Length > 0)
            {
                var regionIdentifiersLower = regionIdentifiers.Select(s => s.ToLowerInvariant()).ToArray();
                var existingRegions = repository
                    .AsQueryable<Region>()
                    .Where(r => regionIdentifiersLower.Contains(r.RegionIdentifier.ToLowerInvariant()))
                    .ToArray();

                foreach (var regionId in regionIdentifiers.Where(s => contentRegions.All(region => region.Region.RegionIdentifier != s)))
                {
                    var region = existingRegions.FirstOrDefault(r => r.RegionIdentifier.ToLowerInvariant() == regionId.ToLowerInvariant());

                    if (region == null)
                    {
                        region = contentRegions
                            .Where(cr => cr.Region.RegionIdentifier.ToLowerInvariant() == regionId.ToLowerInvariant())
                            .Select(cr => cr.Region).FirstOrDefault();

                        if (region == null)
                        {
                            region = new Region { RegionIdentifier = regionId };
                        }
                    }

                    var contentRegion = new ContentRegion { Region = region, Content = content };
                    contentRegions.Add(contentRegion);
                }
            }
        }

        private string[] GetRegionIds(string searchIn)
        {
            if (string.IsNullOrWhiteSpace(searchIn))
            {
                return new string[0];
            }

            var ids = new List<string>();

            var matches = Regex.Matches(searchIn, RootModuleConstants.DynamicRegionRegexPattern, RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    ids.Add(match.Groups[1].Value);
                }
            }

            return ids.Distinct().ToArray();
        }

        private void SetContentOptions(IOptionContainer<Models.Content> destination, IOptionContainer<Models.Content> source)
        {
            optionService.SetOptions<ContentOption, Models.Content>(destination, source.Options, () => new ContentOptionTranslation());
        }

        private void SetContentRegions(Models.Content destination, Models.Content source)
        {
            if (destination.ContentRegions == null)
            {
                destination.ContentRegions = new List<ContentRegion>();
            }
            if (source.ContentRegions == null)
            {
                source.ContentRegions = new List<ContentRegion>();
            }

            // Add regions, which not exists in destination.
            source.ContentRegions
                .Where(s => destination.ContentRegions.All(d => s.Region.RegionIdentifier.ToLowerInvariant() != d.Region.RegionIdentifier.ToLowerInvariant()))
                .Distinct().ToList()
                .ForEach(s =>
                {
                    destination.ContentRegions.Add(new ContentRegion { Region = s.Region, Content = destination });
                });

            // Remove regions, which not exist in source.
            var regionsToDelete = destination.ContentRegions
                .Where(s => source.ContentRegions.All(d => s.Region.RegionIdentifier.ToLowerInvariant() != d.Region.RegionIdentifier.ToLowerInvariant()))
                .Distinct().ToList();
            regionsToDelete.ForEach(d =>
            {
                destination.ContentRegions.Remove(d);
                repository.Delete(d);
            });
        }

        /// <summary>
        /// Validates if content has no children.
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <param name="contentId">The content id.</param>
        /// <param name="html">The HTML.</param>
        /// <returns>
        /// Boolean value, indicating, if content has any children contents, which are based on deleting regions
        /// </returns>
        public bool CheckIfContentHasDeletingChildren(Guid? pageId, Guid contentId, string html = null)
        {
            bool hasAnyContents = false;
            var regionIdentifiers = GetRegionIds(html).Select(s => s.ToLowerInvariant()).ToArray();

            // Get regions going to be deleted
            var regionIds = repository.AsQueryable<ContentRegion>()
                .Where(cr => cr.Content.Id == contentId
                    && !regionIdentifiers.Contains(cr.Region.RegionIdentifier.ToLowerInvariant()))
                .Select(cr => cr.Region.Id)
                .ToArray();

            if (regionIds.Length > 0)
            {
                var validationQuery = repository
                    .AsQueryable<PageContent>()
                    .Where(pc => regionIds.Contains(pc.Region.Id));
                if (pageId.HasValue)
                {
                    validationQuery = validationQuery.Where(pc => pc.Page.MasterPage.Id == pageId);
                }

                hasAnyContents = validationQuery.Any();
            }

            return hasAnyContents;
        }

        public void CheckIfContentHasDeletingChildrenWithException(Guid? pageId, Guid contentId, string html = null)
        {
            var hasAnyChildren = CheckIfContentHasDeletingChildren(pageId, contentId, html);
            if (hasAnyChildren)
            {
                var message = RootGlobalization.SaveContent_ContentHasChildrenContents_RegionDeleteConfirmationMessage;
                var logMessage = string.Format("User is trying to delete content regions which has children contents. Confirmation is required. ContentId: {0}, PageId: {1}", contentId, pageId);
                throw new ConfirmationRequestException(() => message, logMessage);
            }
        }

        public void UpdateDynamicContainer(Models.Content content)
        {
            var dynamicContainer = content as IDynamicContentContainer;
            if (dynamicContainer != null)
            {
                if (content.ContentRegions == null)
                {
                    content.ContentRegions = new List<ContentRegion>();
                }
                if (content.ChildContents == null)
                {
                    content.ChildContents = new List<ChildContent>();
                }
                CollectDynamicRegions(dynamicContainer.Html, content, content.ContentRegions);
                childContentService.CollectChildContents(dynamicContainer.Html, content);
            }
        }

        public TEntity GetDraftOrPublishedContent<TEntity>(TEntity content) where TEntity : Models.Content
        {
            if (content.History != null)
            {
                var draft = content.History.FirstOrDefault(c => c.Status == ContentStatus.Draft);
                if (draft != null)
                {
                    return (TEntity)draft;
                }
            }

            return content;
        }
    }
}
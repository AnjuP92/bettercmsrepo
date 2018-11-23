// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMediaService.cs" company="Devbridge Group LLC">
// 
// Copyright (C) 2015,2016 Devbridge Group LLC
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of thef GNU Lesser General Public License as published by
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
using System.Linq;
using System.Security.Principal;

using BetterCms.Core.Security;
using BetterCms.Core.WebServices;
using BetterCms.Core.Exceptions.Mvc;

using BetterModules.Core.DataAccess;
using BetterCms.Module.MediaManager.Models;

using BetterModules.Core.DataAccess.DataContext;
using BetterModules.Core.DataAccess.DataContext.Fetching;
using BetterModules.Core.Exceptions.DataTier;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BetterCms.Module.Root.Models;

using Common.Logging;

namespace BetterCms.Module.MediaManager.Services
{
    /// <summary>
    /// Default media image service.
    /// </summary>
    internal class DefaultMediaService : IMediaService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository repository;

        private readonly IUnitOfWork unitOfWork;

        private readonly IAccessControlService accessControlService;

        private readonly ICmsConfiguration configuration;

        private ITWebClient _webClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMediaService"/> class.
        /// </summary>
        public DefaultMediaService(IRepository repository, IUnitOfWork unitOfWork, IAccessControlService accessControlService, ICmsConfiguration configuration)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
            this.accessControlService = accessControlService;
            this.configuration = configuration;
            _webClient = new TWebClient();
        }

        /// <summary>
        /// Deletes the media.
        /// </summary>
        public bool DeleteMedia(Guid id, int version, bool checkSecurity, IPrincipal currentPrincipal = null)
        {
            //var media = repository.AsQueryable<Media>(f => f.Id == id).FirstOne();
            //var media =GetMediaDetails(id);
            JObject requestformedia = new JObject();
            requestformedia.Add("mediaId", id);
            string mediaobj = JsonConvert.SerializeObject(requestformedia);
            var mediamodel = _webClient.DownloadData<string>("MediaManager/GetMediaDetails", new { Js = mediaobj });
            dynamic mediaInfo = JObject.Parse(mediamodel);
            Media media = new Media();

            if (mediaInfo.Flag == 1)
            {
                MediaImage mediaimage = new MediaImage();
                mediaimage.Id = id;
                mediaimage.Caption = (!string.IsNullOrEmpty(mediaInfo.Caption.ToString())) ? mediaInfo.Caption : null;
                mediaimage.ImageAlign = (!string.IsNullOrEmpty(mediaInfo.ImageAlign.ToString())) ? mediaInfo.ImageAlign : null;
                mediaimage.Width = mediaInfo.Width;
                mediaimage.Height = mediaInfo.Height;
                mediaimage.CropCoordX1 = (!string.IsNullOrEmpty(mediaInfo.CropCoordX1.ToString())) ? mediaInfo.CropCoordX1 : null;
                mediaimage.CropCoordY1 = (!string.IsNullOrEmpty(mediaInfo.CropCoordY1.ToString())) ? mediaInfo.CropCoordY1 : null;
                mediaimage.CropCoordX2 = (!string.IsNullOrEmpty(mediaInfo.CropCoordX2.ToString())) ? mediaInfo.CropCoordX2 : null;
                mediaimage.CropCoordY2 = (!string.IsNullOrEmpty(mediaInfo.CropCoordY2.ToString())) ? mediaInfo.CropCoordY2 : null;
                mediaimage.OriginalWidth = mediaInfo.OriginalWidth;
                mediaimage.OriginalHeight = mediaInfo.OriginalHeight;
                mediaimage.OriginalSize = mediaInfo.OriginalSize;
                mediaimage.OriginalUri = mediaInfo.OriginalUri;
                mediaimage.PublicOriginallUrl = mediaInfo.PublicOriginallUrl;
                mediaimage.IsOriginalUploaded = (!string.IsNullOrEmpty(mediaInfo.IsOriginalUploaded.ToString())) ? mediaInfo.IsOriginalUploaded : null;
                mediaimage.ThumbnailWidth = mediaInfo.ThumbnailWidth;
                mediaimage.ThumbnailHeight = mediaInfo.ThumbnailHeight;
                mediaimage.ThumbnailSize = mediaInfo.ThumbnailSize;
                mediaimage.ThumbnailUri = mediaInfo.ThumbnailUri;
                mediaimage.PublicThumbnailUrl = mediaInfo.PublicThumbnailUrl;
                mediaimage.IsThumbnailUploaded = (!string.IsNullOrEmpty(mediaInfo.IsThumbnailUploaded.ToString())) ? mediaInfo.IsThumbnailUploaded : null;
                mediaimage.Version = mediaInfo.Version;
                mediaimage.IsDeleted = mediaInfo.IsDeleted;
                mediaimage.CreatedOn =Convert.ToDateTime( mediaInfo.CreatedOn);
                mediaimage.CreatedByUser = mediaInfo.CreatedByUser;
                mediaimage.DeletedOn = (!string.IsNullOrEmpty(mediaInfo.DeletedOn.ToString())) ? Convert.ToDateTime(mediaInfo.DeletedOn ):(DateTime?) null;
                mediaimage.DeletedByUser = (!string.IsNullOrEmpty(mediaInfo.DeletedByUser.ToString())) ? mediaInfo.DeletedByUser : null;
                if (!string.IsNullOrEmpty(mediaInfo.FolderId.ToString()))
                {
                    MediaFolder mediafolder = new MediaFolder();
                    mediafolder.Id = new Guid(mediaInfo.FolderId.ToString());
                    mediaimage.Folder = mediafolder;
                }
                else
                {
                    mediaimage.Folder = null;
                }
                mediaimage.Title = mediaInfo.Title;

                mediaimage.Type = mediaInfo.Type;
                mediaimage.ContentType = mediaInfo.ContentType;
                mediaimage.IsArchived = mediaInfo.IsArchived;
                if (!string.IsNullOrEmpty(mediaInfo.OriginalId.ToString()))
                {
                    MediaImage mediaimage1 = new MediaImage();
                    mediaimage1.Id = new Guid(mediaInfo.OriginalId.ToString());
                    mediaimage.Original = mediaimage1;
                }
                else
                {
                    mediaimage.Original = null;
                }
                mediaimage.PublishedOn = (!string.IsNullOrEmpty(mediaInfo.PublishedOn.ToString())) ? Convert.ToDateTime(mediaInfo.PublishedOn) : (DateTime?)null;
                if (!string.IsNullOrEmpty(mediaInfo.ImageId.ToString()))
                {
                    mediaimage.Image.Id = mediaInfo.ImageId;
                }
                else
                {
                    mediaimage.Image = null;
                }
                mediaimage.Description = (!string.IsNullOrEmpty(mediaInfo.Description.ToString())) ? mediaInfo.Description : null;

                mediaimage.OriginalFileName = mediaInfo.OriginalFileName;
                mediaimage.OriginalFileExtension = (!string.IsNullOrEmpty(mediaInfo.OriginalFileExtension.ToString())) ? mediaInfo.OriginalFileExtension : null;
                mediaimage.FileUri = mediaInfo.FileUri;
                mediaimage.PublicUrl = mediaInfo.PublicUrl;
                mediaimage.Size = mediaInfo.Size;
                mediaimage.IsTemporary = mediaInfo.IsTemporary;
                mediaimage.IsUploaded = (!string.IsNullOrEmpty(mediaInfo.IsUploaded.ToString())) ? mediaInfo.IsUploaded : null;
                mediaimage.IsCanceled = mediaInfo.IsCanceled;
                mediaimage.IsMovedToTrash = mediaInfo.IsMovedToTrash;

                if (!string.IsNullOrEmpty(mediaInfo.NextTryToMoveToTrash.ToString()))
                {
                    mediaimage.NextTryToMoveToTrash = mediaInfo.NextTryToMoveToTrash;
                }
                else
                {
                    mediaimage.NextTryToMoveToTrash = null;
                }
                JObject requestformediacategories = new JObject();
                requestformediacategories.Add("mediaId", id);
                string mediacategoriesobj = JsonConvert.SerializeObject(requestformediacategories);
                var mediacategoriesmodel = _webClient.DownloadData<JArray>("MediaManager/GetMediaCategoriesDetails", new { Js = mediacategoriesobj });


                if (mediacategoriesmodel.Count > 0)
                {
                    IList<MediaCategory> mediacategorylist = new List<MediaCategory>();
                    List<MediaCategory> mediacategorylist1 = new List<MediaCategory>();
                    for (int k = 0; k < mediacategoriesmodel.Count; k++)
                    {
                        MediaCategory mediacategory = new MediaCategory();
                        mediacategory.Id = new Guid(mediacategoriesmodel[k]["Id"].ToString());
                        mediacategory.Version = Convert.ToInt32(mediacategoriesmodel[k]["Version"]);
                        mediacategory.IsDeleted = (bool)mediacategoriesmodel[k]["IsDeleted"];
                        mediacategory.CreatedOn = (DateTime)mediacategoriesmodel[k]["CreatedOn"];
                        mediacategory.CreatedByUser = mediacategoriesmodel[k]["CreatedByUser"].ToString();
                        mediacategory.ModifiedOn = (DateTime)mediacategoriesmodel[k]["ModifiedOn"];
                        mediacategory.ModifiedByUser = mediacategoriesmodel[k]["ModifiedByUser"].ToString();
                        if (!string.IsNullOrEmpty(mediacategoriesmodel[k]["DeletedOn"].ToString()))
                        {
                            mediacategory.DeletedOn = (DateTime)mediacategoriesmodel[k]["DeletedOn"];
                        }
                        else
                        {
                            mediacategory.DeletedOn = null;
                        }
                        mediacategory.DeletedByUser = (!string.IsNullOrEmpty(mediacategoriesmodel[k]["DeletedByUser"].ToString())) ? mediacategoriesmodel[k]["DeletedByUser"].ToString() : null;
                        Category category = new Category();
                        category.Id = new Guid(mediacategoriesmodel[k]["CategoryId"].ToString());
                        JObject requestforcategory = new JObject();
                        requestforcategory.Add("categoryId", category.Id);


                        string categoryobj = JsonConvert.SerializeObject(requestforcategory);
                        var categorymodel = _webClient.DownloadData<string>("MediaManager/GetCategoryDetails", new { Js = categoryobj });
                        dynamic categoryInfo = JObject.Parse(categorymodel);
                        category.Version = categoryInfo.Version;
                        category.IsDeleted = categoryInfo.IsDeleted;
                        category.CreatedOn = categoryInfo.CreatedOn;
                        category.CreatedByUser = categoryInfo.CreatedByUser;
                        category.ModifiedOn = categoryInfo.ModifiedOn;
                        category.ModifiedByUser = categoryInfo.ModifiedByUser;
                        category.DeletedOn = (!string.IsNullOrEmpty(categoryInfo.DeletedOn.ToString())) ? categoryInfo.DeletedOn : null;
                        category.DeletedByUser = (!string.IsNullOrEmpty(categoryInfo.DeletedByUser.ToString())) ? categoryInfo.DeletedByUser : null;
                        category.Name = categoryInfo.Name;
                        if (!string.IsNullOrEmpty(categoryInfo.ParentCategoryId.ToString()))
                        {
                            Category parentcategory = new Category();
                            parentcategory.Id = new Guid(categoryInfo.ParentCategoryId.ToString());
                            category.ParentCategory = parentcategory;
                        }
                        else
                        {
                            category.ParentCategory = null;
                        }
                        category.DisplayOrder = categoryInfo.DisplayOrder;
                        category.Macro = (!string.IsNullOrEmpty(categoryInfo.Macro.ToString())) ? categoryInfo.Macro : null;
                        CategoryTree categorytree = new CategoryTree();
                        categorytree.Id = new Guid(categoryInfo.CategoryTreeId.ToString());

                        JObject requestforcategorytree = new JObject();
                        requestforcategorytree.Add("categorytreeId", categorytree.Id);


                        string categorytreeobj = JsonConvert.SerializeObject(requestforcategorytree);
                        var categorytreemodel = _webClient.DownloadData<string>("MediaManager/GetCategoryTreeDetails", new { Js = categorytreeobj });
                        dynamic categorytreeInfo = JObject.Parse(categorytreemodel);
                        categorytree.Version = categorytreeInfo.Version;
                        categorytree.IsDeleted = categorytreeInfo.IsDeleted;
                        categorytree.CreatedOn = categorytreeInfo.CreatedOn;
                        categorytree.CreatedByUser = categorytreeInfo.CreatedByUser;
                        categorytree.ModifiedOn = categorytreeInfo.ModifiedOn;
                        categorytree.ModifiedByUser = categorytreeInfo.ModifiedByUser;
                        categorytree.DeletedOn = (!string.IsNullOrEmpty(categorytreeInfo.DeletedOn.ToString())) ? categorytreeInfo.DeletedOn : null;
                        categorytree.DeletedByUser = (!string.IsNullOrEmpty(categorytreeInfo.DeletedByUser.ToString())) ? categorytreeInfo.DeletedByUser : null;
                        categorytree.Title = categorytreeInfo.Title;
                        categorytree.Macro = (!string.IsNullOrEmpty(categorytreeInfo.Macro.ToString())) ? categorytreeInfo.Macro : null;

                        JObject request = new JObject();
                        request.Add("categorytreeId", categorytree.Id);
                        string requestobj = JsonConvert.SerializeObject(request);
                        var availableformodel = _webClient.DownloadData<JArray>("MediaManager/GetAvailableFor", new { Js = requestobj });
                        if (availableformodel.Count > 0)
                        {
                            IList<CategoryTreeCategorizableItem> CategoryTreeCategorizableItemlist = new List<CategoryTreeCategorizableItem>();
                            List<CategoryTreeCategorizableItem> CategoryTreeCategorizableItemlist1 = new List<CategoryTreeCategorizableItem>();
                            for (int i = 0; i < availableformodel.Count; i++)
                            {
                                CategoryTreeCategorizableItem categorytreecategorizableitem = new CategoryTreeCategorizableItem();
                                categorytreecategorizableitem.Id = new Guid(availableformodel[i]["Id"].ToString());
                                categorytreecategorizableitem.Version = Convert.ToInt32(availableformodel[i]["Version"]);
                                categorytreecategorizableitem.IsDeleted = (bool)availableformodel[i]["IsDeleted"];
                                categorytreecategorizableitem.CreatedOn = (DateTime)availableformodel[i]["CreatedOn"];
                                categorytreecategorizableitem.CreatedByUser = availableformodel[i]["CreatedByUser"].ToString();
                                categorytreecategorizableitem.ModifiedOn = (DateTime)availableformodel[i]["ModifiedOn"];
                                categorytreecategorizableitem.ModifiedByUser = availableformodel[i]["ModifiedByUser"].ToString();
                                if (!string.IsNullOrEmpty(availableformodel[i]["DeletedOn"].ToString()))
                                {
                                    categorytreecategorizableitem.DeletedOn = (DateTime)availableformodel[i]["DeletedOn"];
                                }
                                else
                                {
                                    categorytreecategorizableitem.DeletedOn = null;
                                }
                                categorytreecategorizableitem.DeletedByUser = (!string.IsNullOrEmpty(availableformodel[i]["DeletedByUser"].ToString())) ? availableformodel[i]["DeletedByUser"].ToString() : null;
                                CategorizableItem categorizableitem = new CategorizableItem();
                                categorizableitem.Id = new Guid(availableformodel[i]["CategorizableItemId"].ToString());
                                JObject requestforcategorizableitem = new JObject();
                                requestforcategorizableitem.Add("categorizableitemId", categorizableitem.Id);
                                string categorizableitemobj = JsonConvert.SerializeObject(requestforcategorizableitem);
                                var categorizableitemmodel = _webClient.DownloadData<string>("MediaManager/GetCategorizableItemDetails", new { Js = categorizableitemobj });
                                dynamic categorizableitemInfo = JObject.Parse(categorizableitemmodel);
                                categorizableitem.Version = categorizableitemInfo.Version;
                                categorizableitem.IsDeleted = categorizableitemInfo.IsDeleted;
                                categorizableitem.CreatedOn = categorizableitemInfo.CreatedOn;
                                categorizableitem.CreatedByUser = categorizableitemInfo.CreatedByUser;
                                categorizableitem.ModifiedOn = categorizableitemInfo.ModifiedOn;
                                categorizableitem.ModifiedByUser = categorizableitemInfo.ModifiedByUser;
                                categorizableitem.DeletedOn = (!string.IsNullOrEmpty(categorizableitemInfo.DeletedOn.ToString())) ? categorizableitemInfo.DeletedOn : null;
                                categorizableitem.DeletedByUser = (!string.IsNullOrEmpty(categorizableitemInfo.DeletedByUser.ToString())) ? categorizableitemInfo.DeletedByUser : null;
                                categorizableitem.Name = categorizableitemInfo.Name;

                                categorytreecategorizableitem.CategorizableItem = categorizableitem;
                                categorytreecategorizableitem.CategoryTree = categorytree;
                                CategoryTreeCategorizableItemlist1.Add(categorytreecategorizableitem);
                            }
                            CategoryTreeCategorizableItemlist = CategoryTreeCategorizableItemlist1;
                            categorytree.AvailableFor = CategoryTreeCategorizableItemlist;
                        }
                        JObject requestforcategories = new JObject();
                        requestforcategories.Add("categorytreeId", categorytree.Id);
                        string requestforcategoriesobj = JsonConvert.SerializeObject(requestforcategories);
                        var requestforcategoriesmodel = _webClient.DownloadData<JArray>("MediaManager/GetCategoriesforCategoryTree", new { Js = requestforcategoriesobj });
                        if (requestforcategoriesmodel.Count > 0)
                        {
                            IList<Category> Categorylist = new List<Category>();
                            List<Category> Categorylist1 = new List<Category>();
                            for (int i = 0; i < requestforcategoriesmodel.Count; i++)
                            {
                                Category category1 = new Category();
                                category1.Id = new Guid(requestforcategoriesmodel[i]["Id"].ToString());
                                category1.Version = Convert.ToInt32(requestforcategoriesmodel[i]["Version"]);
                                category1.IsDeleted = (bool)requestforcategoriesmodel[i]["IsDeleted"];
                                category1.CreatedOn = (DateTime)requestforcategoriesmodel[i]["CreatedOn"];
                                category1.CreatedByUser = requestforcategoriesmodel[i]["CreatedByUser"].ToString();
                                if (!string.IsNullOrEmpty(requestforcategoriesmodel[i]["DeletedOn"].ToString()))
                                {
                                    category1.DeletedOn = (DateTime)requestforcategoriesmodel[i]["DeletedOn"];
                                }
                                else
                                {
                                    category1.DeletedOn = null;
                                }
                                category1.DeletedByUser = (!string.IsNullOrEmpty(requestforcategoriesmodel[i]["DeletedByUser"].ToString())) ? requestforcategoriesmodel[i]["DeletedByUser"].ToString() : null;
                                category1.Name = requestforcategoriesmodel[i]["Name"].ToString();
                                if (!string.IsNullOrEmpty(requestforcategoriesmodel[i]["ParentCategoryId"].ToString()))
                                {
                                    Category parentcategory = new Category();
                                    parentcategory.Id = new Guid(requestforcategoriesmodel[i]["ParentCategoryId"].ToString());
                                    category1.ParentCategory = parentcategory;
                                }
                                else
                                {
                                    category1.ParentCategory = null;
                                }
                                category1.DisplayOrder = Convert.ToInt32(requestforcategoriesmodel[i]["DisplayOrder"]);
                                category1.Macro = requestforcategoriesmodel[i]["Macro"].ToString();

                                category1.CategoryTree = categorytree;
                                Categorylist1.Add(category1);
                            }
                            Categorylist = Categorylist1;
                            categorytree.Categories = Categorylist;
                        }
                        category.CategoryTree = categorytree;
                        mediacategory.Category = category;
                        mediacategory.Media = mediaimage;
                        mediacategorylist1.Add(mediacategory);
                    }
                    mediacategorylist = mediacategorylist1;
                    mediaimage.Categories = mediacategorylist;
                }


                media = mediaimage;

            }
            else if (mediaInfo.Flag == 2)
            {
                MediaFile mediafile = new MediaFile();
                mediafile.Id = id;
                mediafile.OriginalFileName = mediaInfo.OriginalFileName;
                mediafile.OriginalFileExtension = (!string.IsNullOrEmpty(mediaInfo.OriginalFileExtension.ToString())) ? mediaInfo.OriginalFileExtension : null;
                mediafile.FileUri = mediaInfo.FileUri;
                mediafile.PublicUrl = mediaInfo.PublicUrl;
                mediafile.Size = mediaInfo.Size;
                mediafile.IsTemporary = mediaInfo.IsTemporary;
                mediafile.IsUploaded = (!string.IsNullOrEmpty(mediaInfo.IsUploaded.ToString())) ? mediaInfo.IsUploaded : null;
                mediafile.IsCanceled = mediaInfo.IsCanceled;
                mediafile.IsMovedToTrash = mediaInfo.IsMovedToTrash;
                if (!string.IsNullOrEmpty(mediaInfo.NextTryToMoveToTrash.ToString()))
                {
                    mediafile.NextTryToMoveToTrash = mediaInfo.NextTryToMoveToTrash;
                }
                else
                {
                    mediafile.NextTryToMoveToTrash = null;
                }
                mediafile.Version = mediaInfo.Version;
                mediafile.IsDeleted = mediaInfo.IsDeleted;
                mediafile.CreatedOn = mediaInfo.CreatedOn;
                mediafile.CreatedByUser = mediaInfo.CreatedByUser;
                mediafile.DeletedOn = (!string.IsNullOrEmpty(mediaInfo.DeletedOn.ToString())) ? mediaInfo.DeletedOn : null;
                mediafile.DeletedByUser = (!string.IsNullOrEmpty(mediaInfo.DeletedByUser.ToString())) ? mediaInfo.DeletedByUser : null;
                if (!string.IsNullOrEmpty(mediaInfo.FolderId.ToString()))
                {
                    MediaFolder mediafolder = new MediaFolder();
                    mediafolder.Id = new Guid(mediaInfo.FolderId.ToString());
                    mediafile.Folder = mediafolder;
                }
                else
                {
                    mediafile.Folder = null;
                }
                mediafile.Title = mediaInfo.Title;

                mediafile.Type = mediaInfo.Type;
                mediafile.ContentType = mediaInfo.ContentType;
                mediafile.IsArchived = mediaInfo.IsArchived;
                if (!string.IsNullOrEmpty(mediaInfo.OriginalId.ToString()))
                {
                    mediafile.Original.Id = mediaInfo.OriginalId;
                }
                else
                {
                    mediafile.Original = null;
                }
                mediafile.PublishedOn = mediaInfo.PublishedOn;
                if (!string.IsNullOrEmpty(mediaInfo.ImageId.ToString()))
                {
                    mediafile.Image.Id = mediaInfo.ImageId;
                }
                else
                {
                    mediafile.Image = null;
                }
                mediafile.Description = (!string.IsNullOrEmpty(mediaInfo.Description.ToString())) ? mediaInfo.Description : null;


                JObject requestformediacategories = new JObject();
                requestformediacategories.Add("mediaId", id);
                string mediacategoriesobj = JsonConvert.SerializeObject(requestformediacategories);
                var mediacategoriesmodel = _webClient.DownloadData<JArray>("MediaManager/GetMediaCategoriesDetails", new { Js = mediacategoriesobj });


                if (mediacategoriesmodel.Count > 0)
                {
                    IList<MediaCategory> mediacategorylist = new List<MediaCategory>();
                    List<MediaCategory> mediacategorylist1 = new List<MediaCategory>();
                    for (int k = 0; k < mediacategoriesmodel.Count; k++)
                    {
                        MediaCategory mediacategory = new MediaCategory();
                        mediacategory.Id = new Guid(mediacategoriesmodel[k]["Id"].ToString());
                        mediacategory.Version = Convert.ToInt32(mediacategoriesmodel[k]["Version"]);
                        mediacategory.IsDeleted = (bool)mediacategoriesmodel[k]["IsDeleted"];
                        mediacategory.CreatedOn = (DateTime)mediacategoriesmodel[k]["CreatedOn"];
                        mediacategory.CreatedByUser = mediacategoriesmodel[k]["CreatedByUser"].ToString();
                        mediacategory.ModifiedOn = (DateTime)mediacategoriesmodel[k]["ModifiedOn"];
                        mediacategory.ModifiedByUser = mediacategoriesmodel[k]["ModifiedByUser"].ToString();
                        if (!string.IsNullOrEmpty(mediacategoriesmodel[k]["DeletedOn"].ToString()))
                        {
                            mediacategory.DeletedOn = (DateTime)mediacategoriesmodel[k]["DeletedOn"];
                        }
                        else
                        {
                            mediacategory.DeletedOn = null;
                        }
                        mediacategory.DeletedByUser = (!string.IsNullOrEmpty(mediacategoriesmodel[k]["DeletedByUser"].ToString())) ? mediacategoriesmodel[k]["DeletedByUser"].ToString() : null;
                        Category category = new Category();
                        category.Id = new Guid(mediacategoriesmodel[k]["CategoryId"].ToString());
                        JObject requestforcategory = new JObject();
                        requestforcategory.Add("categoryId", category.Id);


                        string categoryobj = JsonConvert.SerializeObject(requestforcategory);
                        var categorymodel = _webClient.DownloadData<string>("MediaManager/GetCategoryDetails", new { Js = categoryobj });
                        dynamic categoryInfo = JObject.Parse(categorymodel);
                        category.Version = categoryInfo.Version;
                        category.IsDeleted = categoryInfo.IsDeleted;
                        category.CreatedOn = categoryInfo.CreatedOn;
                        category.CreatedByUser = categoryInfo.CreatedByUser;
                        category.ModifiedOn = categoryInfo.ModifiedOn;
                        category.ModifiedByUser = categoryInfo.ModifiedByUser;
                        category.DeletedOn = (!string.IsNullOrEmpty(categoryInfo.DeletedOn.ToString())) ? categoryInfo.DeletedOn : null;
                        category.DeletedByUser = (!string.IsNullOrEmpty(categoryInfo.DeletedByUser.ToString())) ? categoryInfo.DeletedByUser : null;
                        category.Name = categoryInfo.Name;
                        if (!string.IsNullOrEmpty(categoryInfo.ParentCategoryId.ToString()))
                        {
                            Category parentcategory = new Category();
                            parentcategory.Id = new Guid(categoryInfo.ParentCategoryId.ToString());
                            category.ParentCategory = parentcategory;
                        }
                        else
                        {
                            category.ParentCategory = null;
                        }
                        category.DisplayOrder = categoryInfo.DisplayOrder;
                        category.Macro = (!string.IsNullOrEmpty(categoryInfo.Macro.ToString())) ? categoryInfo.Macro : null;
                        CategoryTree categorytree = new CategoryTree();
                        categorytree.Id = new Guid(categoryInfo.CategoryTreeId.ToString());

                        JObject requestforcategorytree = new JObject();
                        requestforcategorytree.Add("categorytreeId", categorytree.Id);


                        string categorytreeobj = JsonConvert.SerializeObject(requestforcategorytree);
                        var categorytreemodel = _webClient.DownloadData<string>("MediaManager/GetCategoryTreeDetails", new { Js = categorytreeobj });
                        dynamic categorytreeInfo = JObject.Parse(categorytreemodel);
                        categorytree.Version = categorytreeInfo.Version;
                        categorytree.IsDeleted = categorytreeInfo.IsDeleted;
                        categorytree.CreatedOn = categorytreeInfo.CreatedOn;
                        categorytree.CreatedByUser = categorytreeInfo.CreatedByUser;
                        categorytree.ModifiedOn = categorytreeInfo.ModifiedOn;
                        categorytree.ModifiedByUser = categorytreeInfo.ModifiedByUser;
                        categorytree.DeletedOn = (!string.IsNullOrEmpty(categorytreeInfo.DeletedOn.ToString())) ? categorytreeInfo.DeletedOn : null;
                        categorytree.DeletedByUser = (!string.IsNullOrEmpty(categorytreeInfo.DeletedByUser.ToString())) ? categorytreeInfo.DeletedByUser : null;
                        categorytree.Title = categorytreeInfo.Title;
                        categorytree.Macro = (!string.IsNullOrEmpty(categorytreeInfo.Macro.ToString())) ? categorytreeInfo.Macro : null;

                        JObject request = new JObject();
                        request.Add("categorytreeId", categorytree.Id);
                        string requestobj = JsonConvert.SerializeObject(request);
                        var availableformodel = _webClient.DownloadData<JArray>("MediaManager/GetAvailableFor", new { Js = requestobj });
                        if (availableformodel.Count > 0)
                        {
                            IList<CategoryTreeCategorizableItem> CategoryTreeCategorizableItemlist = new List<CategoryTreeCategorizableItem>();
                            List<CategoryTreeCategorizableItem> CategoryTreeCategorizableItemlist1 = new List<CategoryTreeCategorizableItem>();
                            for (int i = 0; i < availableformodel.Count; i++)
                            {
                                CategoryTreeCategorizableItem categorytreecategorizableitem = new CategoryTreeCategorizableItem();
                                categorytreecategorizableitem.Id = new Guid(availableformodel[i]["Id"].ToString());
                                categorytreecategorizableitem.Version = Convert.ToInt32(availableformodel[i]["Version"]);
                                categorytreecategorizableitem.IsDeleted = (bool)availableformodel[i]["IsDeleted"];
                                categorytreecategorizableitem.CreatedOn = (DateTime)availableformodel[i]["CreatedOn"];
                                categorytreecategorizableitem.CreatedByUser = availableformodel[i]["CreatedByUser"].ToString();
                                categorytreecategorizableitem.ModifiedOn = (DateTime)availableformodel[i]["ModifiedOn"];
                                categorytreecategorizableitem.ModifiedByUser = availableformodel[i]["ModifiedByUser"].ToString();
                                if (!string.IsNullOrEmpty(availableformodel[i]["DeletedOn"].ToString()))
                                {
                                    categorytreecategorizableitem.DeletedOn = (DateTime)availableformodel[i]["DeletedOn"];
                                }
                                else
                                {
                                    categorytreecategorizableitem.DeletedOn = null;
                                }
                                categorytreecategorizableitem.DeletedByUser = (!string.IsNullOrEmpty(availableformodel[i]["DeletedByUser"].ToString())) ? availableformodel[i]["DeletedByUser"].ToString() : null;
                                CategorizableItem categorizableitem = new CategorizableItem();
                                categorizableitem.Id = new Guid(availableformodel[i]["CategorizableItemId"].ToString());
                                JObject requestforcategorizableitem = new JObject();
                                requestforcategorizableitem.Add("categorizableitemId", categorizableitem.Id);
                                string categorizableitemobj = JsonConvert.SerializeObject(requestforcategorizableitem);
                                var categorizableitemmodel = _webClient.DownloadData<string>("MediaManager/GetCategorizableItemDetails", new { Js = categorizableitemobj });
                                dynamic categorizableitemInfo = JObject.Parse(categorizableitemmodel);
                                categorizableitem.Version = categorizableitemInfo.Version;
                                categorizableitem.IsDeleted = categorizableitemInfo.IsDeleted;
                                categorizableitem.CreatedOn = categorizableitemInfo.CreatedOn;
                                categorizableitem.CreatedByUser = categorizableitemInfo.CreatedByUser;
                                categorizableitem.ModifiedOn = categorizableitemInfo.ModifiedOn;
                                categorizableitem.ModifiedByUser = categorizableitemInfo.ModifiedByUser;
                                categorizableitem.DeletedOn = (!string.IsNullOrEmpty(categorizableitemInfo.DeletedOn.ToString())) ? categorizableitemInfo.DeletedOn : null;
                                categorizableitem.DeletedByUser = (!string.IsNullOrEmpty(categorizableitemInfo.DeletedByUser.ToString())) ? categorizableitemInfo.DeletedByUser : null;
                                categorizableitem.Name = categorizableitemInfo.Name;

                                categorytreecategorizableitem.CategorizableItem = categorizableitem;
                                categorytreecategorizableitem.CategoryTree = categorytree;
                                CategoryTreeCategorizableItemlist1.Add(categorytreecategorizableitem);
                            }
                            CategoryTreeCategorizableItemlist = CategoryTreeCategorizableItemlist1;
                            categorytree.AvailableFor = CategoryTreeCategorizableItemlist;
                        }
                        JObject requestforcategories = new JObject();
                        requestforcategories.Add("categorytreeId", categorytree.Id);
                        string requestforcategoriesobj = JsonConvert.SerializeObject(requestforcategories);
                        var requestforcategoriesmodel = _webClient.DownloadData<JArray>("MediaManager/GetCategoriesforCategoryTree", new { Js = requestforcategoriesobj });
                        if (requestforcategoriesmodel.Count > 0)
                        {
                            IList<Category> Categorylist = new List<Category>();
                            List<Category> Categorylist1 = new List<Category>();
                            for (int i = 0; i < requestforcategoriesmodel.Count; i++)
                            {
                                Category category1 = new Category();
                                category1.Id = new Guid(requestforcategoriesmodel[i]["Id"].ToString());
                                category1.Version = Convert.ToInt32(requestforcategoriesmodel[i]["Version"]);
                                category1.IsDeleted = (bool)requestforcategoriesmodel[i]["IsDeleted"];
                                category1.CreatedOn = (DateTime)requestforcategoriesmodel[i]["CreatedOn"];
                                category1.CreatedByUser = requestforcategoriesmodel[i]["CreatedByUser"].ToString();
                                if (!string.IsNullOrEmpty(requestforcategoriesmodel[i]["DeletedOn"].ToString()))
                                {
                                    category1.DeletedOn = (DateTime)requestforcategoriesmodel[i]["DeletedOn"];
                                }
                                else
                                {
                                    category1.DeletedOn = null;
                                }
                                category1.DeletedByUser = (!string.IsNullOrEmpty(requestforcategoriesmodel[i]["DeletedByUser"].ToString())) ? requestforcategoriesmodel[i]["DeletedByUser"].ToString() : null;
                                category1.Name = requestforcategoriesmodel[i]["Name"].ToString();
                                if (!string.IsNullOrEmpty(requestforcategoriesmodel[i]["ParentCategoryId"].ToString()))
                                {
                                    Category parentcategory = new Category();
                                    parentcategory.Id = new Guid(requestforcategoriesmodel[i]["ParentCategoryId"].ToString());
                                    category1.ParentCategory = parentcategory;
                                }
                                else
                                {
                                    category1.ParentCategory = null;
                                }
                                category1.DisplayOrder = Convert.ToInt32(requestforcategoriesmodel[i]["DisplayOrder"]);
                                category1.Macro = requestforcategoriesmodel[i]["Macro"].ToString();

                                category1.CategoryTree = categorytree;
                                Categorylist1.Add(category1);
                            }
                            Categorylist = Categorylist1;
                            categorytree.Categories = Categorylist;
                        }
                        category.CategoryTree = categorytree;
                        mediacategory.Category = category;
                        mediacategory.Media = mediafile;
                        mediacategorylist1.Add(mediacategory);
                    }
                    mediacategorylist = mediacategorylist1;
                    mediafile.Categories = mediacategorylist;
                }

                media = mediafile;
            }
            if (version > 0 && media.Version != version)
            {
                throw new ConcurrentDataException(media);
            }

            var file = media as MediaFile;
            var folder = media as MediaFolder;

            if (file != null)
            {
                return DeleteFile(file, checkSecurity, currentPrincipal);
            }

            if (folder != null)
            {
                return DeleteFolder(folder, checkSecurity, currentPrincipal);
            }

            Log.WarnFormat("Media with id={0} is unknown type, so it may be deleted incorrectly.", id);
            return DeleteUnknownTypeMedia(media);
        }
        //private Media GetMediaDetails(Guid id)
        //{
        //    JObject requestformedia = new JObject();
        //    requestformedia.Add("mediaId", id);
        //    string mediaobj = JsonConvert.SerializeObject(requestformedia);
        //    var mediamodel = _webClient.DownloadData<string>("MediaManager/GetMediaDetails", new { Js = mediaobj });
        //    dynamic mediaInfo = JObject.Parse(mediamodel);
        //    Media media = new Media();

        //    if (mediaInfo.Flag == 1)
        //    {
        //        MediaImage mediaimage = new MediaImage();
        //        mediaimage.Id = id;
        //        mediaimage.Caption = (!string.IsNullOrEmpty(mediaInfo.Caption.ToString())) ? mediaInfo.Caption : null;
        //        mediaimage.ImageAlign = (!string.IsNullOrEmpty(mediaInfo.ImageAlign.ToString())) ? mediaInfo.ImageAlign : null;
        //        mediaimage.Width = mediaInfo.Width;
        //        mediaimage.Height = mediaInfo.Height;
        //        mediaimage.CropCoordX1 = (!string.IsNullOrEmpty(mediaInfo.CropCoordX1.ToString())) ? mediaInfo.CropCoordX1 : null;
        //        mediaimage.CropCoordY1 = (!string.IsNullOrEmpty(mediaInfo.CropCoordY1.ToString())) ? mediaInfo.CropCoordY1 : null;
        //        mediaimage.CropCoordX2 = (!string.IsNullOrEmpty(mediaInfo.CropCoordX2.ToString())) ? mediaInfo.CropCoordX2 : null;
        //        mediaimage.CropCoordY2 = (!string.IsNullOrEmpty(mediaInfo.CropCoordY2.ToString())) ? mediaInfo.CropCoordY2 : null;
        //        mediaimage.OriginalWidth = mediaInfo.OriginalWidth;
        //        mediaimage.OriginalHeight = mediaInfo.OriginalHeight;
        //        mediaimage.OriginalSize = mediaInfo.OriginalSize;
        //        mediaimage.OriginalUri = mediaInfo.OriginalUri;
        //        mediaimage.PublicOriginallUrl = mediaInfo.PublicOriginallUrl;
        //        mediaimage.IsOriginalUploaded = (!string.IsNullOrEmpty(mediaInfo.IsOriginalUploaded.ToString())) ? mediaInfo.IsOriginalUploaded : null;
        //        mediaimage.ThumbnailWidth = mediaInfo.ThumbnailWidth;
        //        mediaimage.ThumbnailHeight = mediaInfo.ThumbnailHeight;
        //        mediaimage.ThumbnailSize = mediaInfo.ThumbnailSize;
        //        mediaimage.ThumbnailUri = mediaInfo.ThumbnailUri;
        //        mediaimage.PublicThumbnailUrl = mediaInfo.PublicThumbnailUrl;
        //        mediaimage.IsThumbnailUploaded = (!string.IsNullOrEmpty(mediaInfo.IsThumbnailUploaded.ToString())) ? mediaInfo.IsThumbnailUploaded : null;
        //        mediaimage.Version = mediaInfo.Version;
        //        mediaimage.IsDeleted = mediaInfo.IsDeleted;
        //        mediaimage.CreatedOn = mediaInfo.CreatedOn;
        //        mediaimage.CreatedByUser = mediaInfo.CreatedByUser;
        //        mediaimage.DeletedOn = (!string.IsNullOrEmpty(mediaInfo.DeletedOn.ToString())) ? mediaInfo.DeletedOn : null;
        //        mediaimage.DeletedByUser = (!string.IsNullOrEmpty(mediaInfo.DeletedByUser.ToString())) ? mediaInfo.DeletedByUser : null;
        //        if (!string.IsNullOrEmpty(mediaInfo.FolderId.ToString()))
        //        {
        //            MediaFolder mediafolder = new MediaFolder();
        //            mediafolder.Id = new Guid(mediaInfo.FolderId.ToString());
        //            mediaimage.Folder = mediafolder;
        //        }
        //        else
        //        {
        //            mediaimage.Folder = null;
        //        }
        //        mediaimage.Title = mediaInfo.Title;

        //        mediaimage.Type = mediaInfo.Type;
        //        mediaimage.ContentType = mediaInfo.ContentType;
        //        mediaimage.IsArchived = mediaInfo.IsArchived;
        //        if (!string.IsNullOrEmpty(mediaInfo.OriginalId.ToString()))
        //        {
        //            MediaImage mediaimage1 = new MediaImage();
        //            mediaimage1.Id = new Guid(mediaInfo.OriginalId.ToString());
        //            mediaimage.Original = mediaimage1;
        //        }
        //        else
        //        {
        //            mediaimage.Original = null;
        //        }
        //        mediaimage.PublishedOn = mediaInfo.PublishedOn;
        //        if (!string.IsNullOrEmpty(mediaInfo.ImageId.ToString()))
        //        {
        //            mediaimage.Image.Id = mediaInfo.ImageId;
        //        }
        //        else
        //        {
        //            mediaimage.Image = null;
        //        }
        //        mediaimage.Description = (!string.IsNullOrEmpty(mediaInfo.Description.ToString())) ? mediaInfo.Description : null;

        //        mediaimage.OriginalFileName = mediaInfo.OriginalFileName;
        //        mediaimage.OriginalFileExtension = (!string.IsNullOrEmpty(mediaInfo.OriginalFileExtension.ToString())) ? mediaInfo.OriginalFileExtension : null;
        //        mediaimage.FileUri = mediaInfo.FileUri;
        //        mediaimage.PublicUrl = mediaInfo.PublicUrl;
        //        mediaimage.Size = mediaInfo.Size;
        //        mediaimage.IsTemporary = mediaInfo.IsTemporary;
        //        mediaimage.IsUploaded = (!string.IsNullOrEmpty(mediaInfo.IsUploaded.ToString())) ? mediaInfo.IsUploaded : null;
        //        mediaimage.IsCanceled = mediaInfo.IsCanceled;
        //        mediaimage.IsMovedToTrash = mediaInfo.IsMovedToTrash;

        //        if (!string.IsNullOrEmpty(mediaInfo.NextTryToMoveToTrash.ToString()))
        //        {
        //            mediaimage.NextTryToMoveToTrash = mediaInfo.NextTryToMoveToTrash;
        //        }
        //        else
        //        {
        //            mediaimage.NextTryToMoveToTrash = null;
        //        }
                
        //        JObject requestformediacategories = new JObject();
        //        requestformediacategories.Add("mediaId", id);
        //        string mediacategoriesobj = JsonConvert.SerializeObject(requestformediacategories);
        //        var mediacategoriesmodel = _webClient.DownloadData<JArray>("MediaManager/GetMediaCategoriesDetails", new { Js = mediacategoriesobj });


        //        if (mediacategoriesmodel.Count > 0)
        //        {
        //            IList<MediaCategory> mediacategorylist = new List<MediaCategory>();
        //            List<MediaCategory> mediacategorylist1 = new List<MediaCategory>();
        //            for (int k = 0; k < mediacategoriesmodel.Count; k++)
        //            {
        //                MediaCategory mediacategory = new MediaCategory();
        //                mediacategory.Id = new Guid(mediacategoriesmodel[k]["Id"].ToString());
        //                mediacategory.Version = Convert.ToInt32(mediacategoriesmodel[k]["Version"]);
        //                mediacategory.IsDeleted = (bool)mediacategoriesmodel[k]["IsDeleted"];
        //                mediacategory.CreatedOn = (DateTime)mediacategoriesmodel[k]["CreatedOn"];
        //                mediacategory.CreatedByUser = mediacategoriesmodel[k]["CreatedByUser"].ToString();
        //                mediacategory.ModifiedOn = (DateTime)mediacategoriesmodel[k]["ModifiedOn"];
        //                mediacategory.ModifiedByUser = mediacategoriesmodel[k]["ModifiedByUser"].ToString();
        //                if (!string.IsNullOrEmpty(mediacategoriesmodel[k]["DeletedOn"].ToString()))
        //                {
        //                    mediacategory.DeletedOn = (DateTime)mediacategoriesmodel[k]["DeletedOn"];
        //                }
        //                else
        //                {
        //                    mediacategory.DeletedOn = null;
        //                }
        //                mediacategory.DeletedByUser = (!string.IsNullOrEmpty(mediacategoriesmodel[k]["DeletedByUser"].ToString())) ? mediacategoriesmodel[k]["DeletedByUser"].ToString() : null;
        //                Category category = new Category();
        //                category.Id = new Guid(mediacategoriesmodel[k]["CategoryId"].ToString());
        //                JObject requestforcategory = new JObject();
        //                requestforcategory.Add("categoryId", category.Id);


        //                string categoryobj = JsonConvert.SerializeObject(requestforcategory);
        //                var categorymodel = _webClient.DownloadData<string>("MediaManager/GetCategoryDetails", new { Js = categoryobj });
        //                dynamic categoryInfo = JObject.Parse(categorymodel);
        //                category.Version = categoryInfo.Version;
        //                category.IsDeleted = categoryInfo.IsDeleted;
        //                category.CreatedOn = categoryInfo.CreatedOn;
        //                category.CreatedByUser = categoryInfo.CreatedByUser;
        //                category.ModifiedOn = categoryInfo.ModifiedOn;
        //                category.ModifiedByUser = categoryInfo.ModifiedByUser;
        //                category.DeletedOn = (!string.IsNullOrEmpty(categoryInfo.DeletedOn.ToString())) ? categoryInfo.DeletedOn : null;
        //                category.DeletedByUser = (!string.IsNullOrEmpty(categoryInfo.DeletedByUser.ToString())) ? categoryInfo.DeletedByUser : null;
        //                category.Name = categoryInfo.Name;
        //                if (!string.IsNullOrEmpty(categoryInfo.ParentCategoryId.ToString()))
        //                {
        //                    Category parentcategory = new Category();
        //                    parentcategory.Id = new Guid(categoryInfo.ParentCategoryId.ToString());
        //                    category.ParentCategory = parentcategory;
        //                }
        //                else
        //                {
        //                    category.ParentCategory = null;
        //                }
        //                category.DisplayOrder = categoryInfo.DisplayOrder;
        //                category.Macro = (!string.IsNullOrEmpty(categoryInfo.Macro.ToString())) ? categoryInfo.Macro : null;
        //                CategoryTree categorytree = new CategoryTree();
        //                categorytree.Id = new Guid(categoryInfo.CategoryTreeId.ToString());

        //                JObject requestforcategorytree = new JObject();
        //                requestforcategorytree.Add("categorytreeId", categorytree.Id);


        //                string categorytreeobj = JsonConvert.SerializeObject(requestforcategorytree);
        //                var categorytreemodel = _webClient.DownloadData<string>("MediaManager/GetCategoryTreeDetails", new { Js = categorytreeobj });
        //                dynamic categorytreeInfo = JObject.Parse(categorytreemodel);
        //                categorytree.Version = categorytreeInfo.Version;
        //                categorytree.IsDeleted = categorytreeInfo.IsDeleted;
        //                categorytree.CreatedOn = categorytreeInfo.CreatedOn;
        //                categorytree.CreatedByUser = categorytreeInfo.CreatedByUser;
        //                categorytree.ModifiedOn = categorytreeInfo.ModifiedOn;
        //                categorytree.ModifiedByUser = categorytreeInfo.ModifiedByUser;
        //                categorytree.DeletedOn = (!string.IsNullOrEmpty(categorytreeInfo.DeletedOn.ToString())) ? categorytreeInfo.DeletedOn : null;
        //                categorytree.DeletedByUser = (!string.IsNullOrEmpty(categorytreeInfo.DeletedByUser.ToString())) ? categorytreeInfo.DeletedByUser : null;
        //                categorytree.Title = categorytreeInfo.Title;
        //                categorytree.Macro = (!string.IsNullOrEmpty(categorytreeInfo.Macro.ToString())) ? categorytreeInfo.Macro : null;

        //                JObject request = new JObject();
        //                request.Add("categorytreeId", categorytree.Id);
        //                string requestobj = JsonConvert.SerializeObject(request);
        //                var availableformodel = _webClient.DownloadData<JArray>("MediaManager/GetAvailableFor", new { Js = requestobj });
        //                if (availableformodel.Count > 0)
        //                {
        //                    IList<CategoryTreeCategorizableItem> CategoryTreeCategorizableItemlist = new List<CategoryTreeCategorizableItem>();
        //                    List<CategoryTreeCategorizableItem> CategoryTreeCategorizableItemlist1 = new List<CategoryTreeCategorizableItem>();
        //                    for (int i = 0; i < availableformodel.Count; i++)
        //                    {
        //                        CategoryTreeCategorizableItem categorytreecategorizableitem = new CategoryTreeCategorizableItem();
        //                        categorytreecategorizableitem.Id = new Guid(availableformodel[i]["Id"].ToString());
        //                        categorytreecategorizableitem.Version = Convert.ToInt32(availableformodel[i]["Version"]);
        //                        categorytreecategorizableitem.IsDeleted = (bool)availableformodel[i]["IsDeleted"];
        //                        categorytreecategorizableitem.CreatedOn = (DateTime)availableformodel[i]["CreatedOn"];
        //                        categorytreecategorizableitem.CreatedByUser = availableformodel[i]["CreatedByUser"].ToString();
        //                        categorytreecategorizableitem.ModifiedOn = (DateTime)availableformodel[i]["ModifiedOn"];
        //                        categorytreecategorizableitem.ModifiedByUser = availableformodel[i]["ModifiedByUser"].ToString();
        //                        if (!string.IsNullOrEmpty(availableformodel[i]["DeletedOn"].ToString()))
        //                        {
        //                            categorytreecategorizableitem.DeletedOn = (DateTime)availableformodel[i]["DeletedOn"];
        //                        }
        //                        else
        //                        {
        //                            categorytreecategorizableitem.DeletedOn = null;
        //                        }
        //                        categorytreecategorizableitem.DeletedByUser = (!string.IsNullOrEmpty(availableformodel[i]["DeletedByUser"].ToString())) ? availableformodel[i]["DeletedByUser"].ToString() : null;
        //                        CategorizableItem categorizableitem = new CategorizableItem();
        //                        categorizableitem.Id = new Guid(availableformodel[i]["CategorizableItemId"].ToString());
        //                        JObject requestforcategorizableitem = new JObject();
        //                        requestforcategorizableitem.Add("categorizableitemId", categorizableitem.Id);
        //                        string categorizableitemobj = JsonConvert.SerializeObject(requestforcategorizableitem);
        //                        var categorizableitemmodel = _webClient.DownloadData<string>("MediaManager/GetCategorizableItemDetails", new { Js = categorizableitemobj });
        //                        dynamic categorizableitemInfo = JObject.Parse(categorizableitemmodel);
        //                        categorizableitem.Version = categorizableitemInfo.Version;
        //                        categorizableitem.IsDeleted = categorizableitemInfo.IsDeleted;
        //                        categorizableitem.CreatedOn = categorizableitemInfo.CreatedOn;
        //                        categorizableitem.CreatedByUser = categorizableitemInfo.CreatedByUser;
        //                        categorizableitem.ModifiedOn = categorizableitemInfo.ModifiedOn;
        //                        categorizableitem.ModifiedByUser = categorizableitemInfo.ModifiedByUser;
        //                        categorizableitem.DeletedOn = (!string.IsNullOrEmpty(categorizableitemInfo.DeletedOn.ToString())) ? categorizableitemInfo.DeletedOn : null;
        //                        categorizableitem.DeletedByUser = (!string.IsNullOrEmpty(categorizableitemInfo.DeletedByUser.ToString())) ? categorizableitemInfo.DeletedByUser : null;
        //                        categorizableitem.Name = categorizableitemInfo.Name;

        //                        categorytreecategorizableitem.CategorizableItem = categorizableitem;
        //                        categorytreecategorizableitem.CategoryTree = categorytree;
        //                        CategoryTreeCategorizableItemlist1.Add(categorytreecategorizableitem);
        //                    }
        //                    CategoryTreeCategorizableItemlist = CategoryTreeCategorizableItemlist1;
        //                    categorytree.AvailableFor = CategoryTreeCategorizableItemlist;
        //                }
        //                JObject requestforcategories = new JObject();
        //                requestforcategories.Add("categorytreeId", categorytree.Id);
        //                string requestforcategoriesobj = JsonConvert.SerializeObject(requestforcategories);
        //                var requestforcategoriesmodel = _webClient.DownloadData<JArray>("MediaManager/GetCategoriesforCategoryTree", new { Js = requestforcategoriesobj });
        //                if (requestforcategoriesmodel.Count > 0)
        //                {
        //                    IList<Category> Categorylist = new List<Category>();
        //                    List<Category> Categorylist1 = new List<Category>();
        //                    for (int i = 0; i < requestforcategoriesmodel.Count; i++)
        //                    {
        //                        Category category1 = new Category();
        //                        category1.Id = new Guid(requestforcategoriesmodel[i]["Id"].ToString());
        //                        category1.Version = Convert.ToInt32(requestforcategoriesmodel[i]["Version"]);
        //                        category1.IsDeleted = (bool)requestforcategoriesmodel[i]["IsDeleted"];
        //                        category1.CreatedOn = (DateTime)requestforcategoriesmodel[i]["CreatedOn"];
        //                        category1.CreatedByUser = requestforcategoriesmodel[i]["CreatedByUser"].ToString();
        //                        if (!string.IsNullOrEmpty(requestforcategoriesmodel[i]["DeletedOn"].ToString()))
        //                        {
        //                            category1.DeletedOn = (DateTime)requestforcategoriesmodel[i]["DeletedOn"];
        //                        }
        //                        else
        //                        {
        //                            category1.DeletedOn = null;
        //                        }
        //                        category1.DeletedByUser = (!string.IsNullOrEmpty(requestforcategoriesmodel[i]["DeletedByUser"].ToString())) ? requestforcategoriesmodel[i]["DeletedByUser"].ToString() : null;
        //                        category1.Name = requestforcategoriesmodel[i]["Name"].ToString();
        //                        if (!string.IsNullOrEmpty(requestforcategoriesmodel[i]["ParentCategoryId"].ToString()))
        //                        {
        //                            Category parentcategory = new Category();
        //                            parentcategory.Id = new Guid(requestforcategoriesmodel[i]["ParentCategoryId"].ToString());
        //                            category1.ParentCategory = parentcategory;
        //                        }
        //                        else
        //                        {
        //                            category1.ParentCategory = null;
        //                        }
        //                        category1.DisplayOrder = Convert.ToInt32(requestforcategoriesmodel[i]["DisplayOrder"]);
        //                        category1.Macro = requestforcategoriesmodel[i]["Macro"].ToString();

        //                        category1.CategoryTree = categorytree;
        //                        Categorylist1.Add(category1);
        //                    }
        //                    Categorylist = Categorylist1;
        //                    categorytree.Categories = Categorylist;
        //                }
        //                category.CategoryTree = categorytree;
        //                mediacategory.Category = category;
        //                mediacategory.Media = mediaimage;
        //                mediacategorylist1.Add(mediacategory);
        //            }
        //            mediacategorylist = mediacategorylist1;
        //            mediaimage.Categories = mediacategorylist;
        //        }
        //        //JObject requestforhistorydetails = new JObject();
        //        //requestforhistorydetails.Add("mediaId", id);
        //        //string historydetailsobj = JsonConvert.SerializeObject(requestforhistorydetails);
        //        //var historydetailsmodel = _webClient.DownloadData<JArray>("MediaManager/GetHistoryDetailsId", new { Js = historydetailsobj });
        //        //Media media1 = new Media();
        //        //IList<Media> mediaImagelist = new List<Media>();
        //        //List<Media> mediaImagelist1 = new List<Media>();
        //        //if (historydetailsmodel.Count > 0)
        //        //{

        //        //    for (int i = 0; i < historydetailsmodel.Count; i++)
        //        //    {
        //        //        MediaImage mediaImage2 = new MediaImage();
        //        //        mediaImage2.Id = new Guid(historydetailsmodel[i]["Id"].ToString());
        //        //        media1 = GetMediaDetails(mediaImage2.Id);
        //        //        mediaimage.History = media1;
        //        //        // mediaImagelist1.Add(media1);
        //        //    }
        //        //    //mediaImagelist = mediaImagelist1;

        //        //}
                
        //        media = mediaimage;

        //    }
        //    else if (mediaInfo.Flag == 2)
        //    {
        //        MediaFile mediafile = new MediaFile();
        //        mediafile.Id = id;
        //        mediafile.OriginalFileName = mediaInfo.OriginalFileName;
        //        mediafile.OriginalFileExtension = (!string.IsNullOrEmpty(mediaInfo.OriginalFileExtension.ToString())) ? mediaInfo.OriginalFileExtension : null;
        //        mediafile.FileUri = mediaInfo.FileUri;
        //        mediafile.PublicUrl = mediaInfo.PublicUrl;
        //        mediafile.Size = mediaInfo.Size;
        //        mediafile.IsTemporary = mediaInfo.IsTemporary;
        //        mediafile.IsUploaded = (!string.IsNullOrEmpty(mediaInfo.IsUploaded.ToString())) ? mediaInfo.IsUploaded : null;
        //        mediafile.IsCanceled = mediaInfo.IsCanceled;
        //        mediafile.IsMovedToTrash = mediaInfo.IsMovedToTrash;
        //        mediafile.NextTryToMoveToTrash = mediaInfo.NextTryToMoveToTrash;
        //        mediafile.Version = mediaInfo.Version;
        //        mediafile.IsDeleted = mediaInfo.IsDeleted;
        //        mediafile.CreatedOn = mediaInfo.CreatedOn;
        //        mediafile.CreatedByUser = mediaInfo.CreatedByUser;
        //        mediafile.DeletedOn = (!string.IsNullOrEmpty(mediaInfo.DeletedOn.ToString())) ? mediaInfo.DeletedOn : null;
        //        mediafile.DeletedByUser = (!string.IsNullOrEmpty(mediaInfo.DeletedByUser.ToString())) ? mediaInfo.DeletedByUser : null;
        //        if (!string.IsNullOrEmpty(mediaInfo.FolderId.ToString()))
        //        {
        //            MediaFolder mediafolder = new MediaFolder();
        //            mediafolder.Id = new Guid(mediaInfo.FolderId.ToString());
        //            mediafile.Folder = mediafolder;
        //        }
        //        else
        //        {
        //            mediafile.Folder = null;
        //        }
        //        mediafile.Title = mediaInfo.Title;

        //        mediafile.Type = mediaInfo.Type;
        //        mediafile.ContentType = mediaInfo.ContentType;
        //        mediafile.IsArchived = mediaInfo.IsArchived;
        //        if (!string.IsNullOrEmpty(mediaInfo.OriginalId.ToString()))
        //        {
        //            mediafile.Original.Id = mediaInfo.OriginalId;
        //        }
        //        else
        //        {
        //            mediafile.Original = null;
        //        }
        //        mediafile.PublishedOn = mediaInfo.PublishedOn;
        //        if (!string.IsNullOrEmpty(mediaInfo.ImageId.ToString()))
        //        {
        //            mediafile.Image.Id = mediaInfo.ImageId;
        //        }
        //        else
        //        {
        //            mediafile.Image = null;
        //        }
        //        mediafile.Description = (!string.IsNullOrEmpty(mediaInfo.Description.ToString())) ? mediaInfo.Description : null;
        //        media = mediafile;
        //    }

        //    return media;
        //}
        private bool DeleteFile(MediaFile file, bool checkSecurity, IPrincipal currentPrincipal)
        {
            var allVersions = repository
                .AsQueryable<MediaFile>(f => f.Id == file.Id || f.Original.Id == file.Id)
                .Fetch(f => f.AccessRules)
                .Fetch(f => f.Categories)
                .Fetch(f => f.MediaTags)
                .ToList().Distinct().ToArray();

            //JObject requestformedia = new JObject();
            //requestformedia.Add("mediaId", file.Id);
            //requestformedia.Add("originalId", file.Original.Id);
            //string mediaobj = JsonConvert.SerializeObject(requestformedia);
            //var mediamodel = _webClient.DownloadData<JArray>("MediaManager/GetAllVersionsMediaDetails", new { Js = mediaobj });
            //if (mediamodel.Count > 0)
            //{
            //    IList<Media> Categorylist = new List<Media>();
            //    List<Media> Categorylist1 = new List<Media>();
            //    for (int i = 0; i < mediamodel.Count; i++)
            //    {
            //        if(mediamodel[i]["Type"].ToString() =="Image")
            //        {
            //            MediaImage mediaimage = new MediaImage();
            //            mediaimage.Id = new Guid(mediamodel[i]["Id"].ToString());
            //            mediaimage.Caption = (!string.IsNullOrEmpty(mediamodel[i]["Caption"].ToString())) ? mediamodel[i]["Caption"].ToString() : null;
            //            if (!string.IsNullOrEmpty(mediamodel[i]["ImageAlign"].ToString()))
            //            {
            //               // mediaimage.ImageAlign =mediamodel[i]["ImageAlign"];
            //            }
            //            else
            //            {
            //                mediaimage.ImageAlign = null;
            //            }
                        
            //            mediaimage.Width =Convert.ToInt32(mediamodel[i]["Width"]);
            //            mediaimage.Height = Convert.ToInt32(mediamodel[i]["Height"]);
            //            if (!string.IsNullOrEmpty(mediamodel[i]["CropCoordX1"].ToString()))
            //            {
            //                mediaimage.CropCoordX1 = Convert.ToInt32(mediamodel[i]["CropCoordX1"]);
            //            }
            //            else
            //            {
            //                mediaimage.CropCoordX1 = null;
            //            }
            //            if(!string.IsNullOrEmpty(mediamodel[i]["CropCoordY1"].ToString()))
            //            {
            //                mediaimage.CropCoordY1 = Convert.ToInt32(mediamodel[i]["CropCoordY1"]);
            //            }
            //            else
            //            {
            //                mediaimage.CropCoordY1 = null;
            //            }
            //            if (!string.IsNullOrEmpty(mediamodel[i]["CropCoordX2"].ToString()))
            //            {
            //                mediaimage.CropCoordX2 = Convert.ToInt32(mediamodel[i]["CropCoordX2"]);
            //            }
            //            else
            //            {
            //                mediaimage.CropCoordX2 = null;
            //            }
            //            if (!string.IsNullOrEmpty(mediamodel[i]["CropCoordX2"].ToString()))
            //            {
            //            }

            //            mediaimage.CropCoordX2 = (!string.IsNullOrEmpty(mediaInfo.CropCoordX2.ToString())) ? mediaInfo.CropCoordX2 : null;
            //            mediaimage.CropCoordY2 = (!string.IsNullOrEmpty(mediaInfo.CropCoordY2.ToString())) ? mediaInfo.CropCoordY2 : null;
            //            mediaimage.OriginalWidth = mediaInfo.OriginalWidth;
            //            mediaimage.OriginalHeight = mediaInfo.OriginalHeight;
            //            mediaimage.OriginalSize = mediaInfo.OriginalSize;
            //            mediaimage.OriginalUri = mediaInfo.OriginalUri;
            //            mediaimage.PublicOriginallUrl = mediaInfo.PublicOriginallUrl;
            //            mediaimage.IsOriginalUploaded = (!string.IsNullOrEmpty(mediaInfo.IsOriginalUploaded.ToString())) ? mediaInfo.IsOriginalUploaded : null;
            //            mediaimage.ThumbnailWidth = mediaInfo.ThumbnailWidth;
            //            mediaimage.ThumbnailHeight = mediaInfo.ThumbnailHeight;
            //            mediaimage.ThumbnailSize = mediaInfo.ThumbnailSize;
            //            mediaimage.ThumbnailUri = mediaInfo.ThumbnailUri;
            //            mediaimage.PublicThumbnailUrl = mediaInfo.PublicThumbnailUrl;
            //            mediaimage.IsThumbnailUploaded = (!string.IsNullOrEmpty(mediaInfo.IsThumbnailUploaded.ToString())) ? mediaInfo.IsThumbnailUploaded : null;
            //            mediaimage.Version = mediaInfo.Version;
            //            mediaimage.IsDeleted = mediaInfo.IsDeleted;
            //            mediaimage.CreatedOn = mediaInfo.CreatedOn;
            //            mediaimage.CreatedByUser = mediaInfo.CreatedByUser;
            //            mediaimage.DeletedOn = (!string.IsNullOrEmpty(mediaInfo.DeletedOn.ToString())) ? mediaInfo.DeletedOn : null;
            //            mediaimage.DeletedByUser = (!string.IsNullOrEmpty(mediaInfo.DeletedByUser.ToString())) ? mediaInfo.DeletedByUser : null;
            //            if (!string.IsNullOrEmpty(mediaInfo.FolderId.ToString()))
            //            {
            //                MediaFolder mediafolder = new MediaFolder();
            //                mediafolder.Id = new Guid(mediaInfo.FolderId.ToString());
            //                mediaimage.Folder = mediafolder;
            //            }
            //            else
            //            {
            //                mediaimage.Folder = null;
            //            }
            //            mediaimage.Title = mediaInfo.Title;

            //            mediaimage.Type = mediaInfo.Type;
            //            mediaimage.ContentType = mediaInfo.ContentType;
            //            mediaimage.IsArchived = mediaInfo.IsArchived;
            //            if (!string.IsNullOrEmpty(mediaInfo.OriginalId.ToString()))
            //            {
            //                MediaImage mediaimage1 = new MediaImage();
            //                mediaimage1.Id = new Guid(mediaInfo.OriginalId.ToString());
            //                mediaimage.Original = mediaimage1;
            //            }
            //            else
            //            {
            //                mediaimage.Original = null;
            //            }
            //            mediaimage.PublishedOn = mediaInfo.PublishedOn;
            //            if (!string.IsNullOrEmpty(mediaInfo.ImageId.ToString()))
            //            {
            //                mediaimage.Image.Id = mediaInfo.ImageId;
            //            }
            //            else
            //            {
            //                mediaimage.Image = null;
            //            }
            //            mediaimage.Description = (!string.IsNullOrEmpty(mediaInfo.Description.ToString())) ? mediaInfo.Description : null;

            //            mediaimage.OriginalFileName = mediaInfo.OriginalFileName;
            //            mediaimage.OriginalFileExtension = (!string.IsNullOrEmpty(mediaInfo.OriginalFileExtension.ToString())) ? mediaInfo.OriginalFileExtension : null;
            //            mediaimage.FileUri = mediaInfo.FileUri;
            //            mediaimage.PublicUrl = mediaInfo.PublicUrl;
            //            mediaimage.Size = mediaInfo.Size;
            //            mediaimage.IsTemporary = mediaInfo.IsTemporary;
            //            mediaimage.IsUploaded = (!string.IsNullOrEmpty(mediaInfo.IsUploaded.ToString())) ? mediaInfo.IsUploaded : null;
            //            mediaimage.IsCanceled = mediaInfo.IsCanceled;
            //            mediaimage.IsMovedToTrash = mediaInfo.IsMovedToTrash;

            //            if (!string.IsNullOrEmpty(mediaInfo.NextTryToMoveToTrash.ToString()))
            //            {
            //                mediaimage.NextTryToMoveToTrash = mediaInfo.NextTryToMoveToTrash;
            //            }
            //            else
            //            {
            //                mediaimage.NextTryToMoveToTrash = null;
            //            }

            //        }
                    
            //    }
            //}
            var mainFile = allVersions.FirstOrDefault(t => t.Id == file.Id);
            if (mainFile == null)
            {
                return true; // Already does not exist.
            }

            try
            {
                // Demand access
                if (checkSecurity && configuration.Security.AccessControlEnabled)
                {
                    accessControlService.DemandAccess(mainFile, currentPrincipal, AccessLevel.ReadWrite);
                }

                unitOfWork.BeginTransaction();
                foreach (var media in allVersions)
                {
                    var mediaFile = media;
                    foreach (var category in mediaFile.Categories)
                    {
                        //repository.Delete(category);
                        // delete category 
                        // flag =1 for category
                        var model = _webClient.DownloadData<int>("MediaManager/DeleteQuery", new { id = category.Id, flag = 1 });
                    }
                    foreach (var rule in mediaFile.AccessRules.ToArray())
                    {
                        mediaFile.RemoveRule(rule);
                        //repository.Delete(rule);
                        // delete rule
                        // flag =2 for rule
                        var model = _webClient.DownloadData<int>("MediaManager/DeleteQuery", new { id = rule.Id, flag = 2 });
                    }
                    foreach (var tag in mediaFile.MediaTags)
                    {
                        // repository.Delete(tag);
                        // delete tag 
                        // flag =3 for tag
                        var model = _webClient.DownloadData<int>("MediaManager/DeleteQuery", new { id = tag.Id, flag = 3 });
                    }
                    //repository.Delete(media);
                    // delete media 
                    // flag =4 for media

                    var mediaModel = _webClient.DownloadData<int>("MediaManager/DeleteQuery", new { id = media.Id, flag = 4 });

                }
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Failed to delete file with id={0}.", ex, mainFile.Id);
                return false;
            }

            Events.MediaManagerEvents.Instance.OnMediaFileDeleted(mainFile);
            return true;
        }

        private bool DeleteFolder(MediaFolder folder, bool checkSecurity, IPrincipal currentPrincipal)
        {
            try
            {
                var allItemsDeleted = true;
                var folderItems = folder.Medias.ToList();
                foreach (var item in folderItems)
                {
                    var fileToDelete = item as MediaFile;
                    if (fileToDelete != null && !DeleteFile(fileToDelete, checkSecurity, currentPrincipal))
                    {
                        allItemsDeleted = false;
                    }
                    var folderToDelete = item as MediaFolder;
                    if (folderToDelete != null && !DeleteFolder(folderToDelete, checkSecurity, currentPrincipal))
                    {
                        allItemsDeleted = false;
                    }
                }

                if (!allItemsDeleted)
                {
                    return false;
                }

                unitOfWork.BeginTransaction();
                repository.AsQueryable<MediaFolder>(f => f.Id == folder.Id || f.Original.Id == folder.Id)
                    .Fetch(f => f.Categories)
                    .Fetch(f => f.MediaTags)
                    .ToList()
                    .ForEach(
                        media =>
                        {
                            if (media.MediaTags != null)
                            {
                                foreach (var mediaTag in media.MediaTags)
                                {
                                    // repository.Delete(mediaTag);
                                    // delete tag 
                                    // flag =3 for tag
                                    var model = _webClient.DownloadData<int>("MediaManager/DeleteQuery", new { id = mediaTag.Id, flag = 3 });
                                }
                            }
                            if (media.Categories != null)
                            {
                                foreach (var category in media.Categories)
                                {
                                    //repository.Delete(category);
                                    // delete category 
                                    // flag =1 for category
                                    var model = _webClient.DownloadData<int>("MediaManager/DeleteQuery", new { id = category.Id, flag = 1 });


                                }
                            }

                            //  repository.Delete(media);
                            var Mmodel = _webClient.DownloadData<int>("MediaManager/DeleteQuery", new { id = media.Id, flag = 4 });
                        });
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Failed to delete folder with id={0}.", ex, folder.Id);
                return false;
            }

            Events.MediaManagerEvents.Instance.OnMediaFolderDeleted(folder);
            return true;
        }

        private bool DeleteUnknownTypeMedia(Media media)
        {
            unitOfWork.BeginTransaction();

            if (media.MediaTags != null)
            {
                foreach (var mediaTag in media.MediaTags)
                {
                    //repository.Delete(mediaTag);
                    // delete tag 
                    // flag =3 for tag
                    var model = _webClient.DownloadData<int>("MediaManager/DeleteQuery", new { id = mediaTag.Id, flag = 3 });
                }
            }

            if (media.Categories != null)
            {
                foreach (var category in media.Categories)
                {
                    //repository.Delete(category);
                    // delete category 
                    // flag =1 for category
                    var model = _webClient.DownloadData<int>("MediaManager/DeleteQuery", new { id = category.Id, flag = 1 });

                }
            }

            //repository.Delete(media);
            // delete media 
            // flag =4 for media
            var mediaModel = _webClient.DownloadData<int>("MediaManager/DeleteQuery", new { id = media.Id, flag = 4 });


            unitOfWork.Commit();

            return true;
        }

        /// <summary>
        /// Archives the sub medias.
        /// </summary>
        /// <param name="media">The media.</param>
        /// <param name="archivedMedias">The archived medias.</param>
        public void ArchiveSubMedias(Media media, List<Media> archivedMedias)
        {
            var subItems = repository.AsQueryable<Media>().Where(m => m.Folder != null && m.Folder.Id == media.Id).ToList();
            foreach (var subItem in subItems)
            {
                if (!subItem.IsArchived)
                {
                    subItem.IsArchived = true;
                    archivedMedias.Add(subItem);

                    //  repository.Save(subItem);

                    JObject savemediaitem = new JObject();
                    savemediaitem.Add("Id", subItem.Id);
                    savemediaitem.Add("Version", subItem.Version);
                    savemediaitem.Add("IsDeleted", subItem.IsDeleted);
                    savemediaitem.Add("Title", subItem.Title);
                    savemediaitem.Add("Type", subItem.Type.ToString());
                    savemediaitem.Add("ContentType", subItem.ContentType.ToString());
                    savemediaitem.Add("IsArchived", subItem.IsArchived);
                    savemediaitem.Add("Description", subItem.Description);


                    string js = JsonConvert.SerializeObject(savemediaitem);
                    var model = _webClient.DownloadData<int>("MediaManager/SaveMedias", new { JS = js });

                }

                ArchiveSubMedias(subItem, archivedMedias);
            }
        }

        /// <summary>
        /// Unarchives the sub medias.
        /// </summary>
        /// <param name="media">The media.</param>
        /// <param name="unarchivedMedias">The unarchived medias.</param>
        public void UnarchiveSubMedias(Media media, List<Media> unarchivedMedias)
        {
            var subItems = repository.AsQueryable<Media>().Where(m => m.Folder != null && m.Folder.Id == media.Id).ToList();
            foreach (var subItem in subItems)
            {
                if (subItem.IsArchived)
                {
                    subItem.IsArchived = false;
                    unarchivedMedias.Add(subItem);

                    //  repository.Save(subItem);

                    JObject savemediaitem = new JObject();
                    savemediaitem.Add("Id", subItem.Id);
                    savemediaitem.Add("Version", subItem.Version);
                    savemediaitem.Add("IsDeleted", subItem.IsDeleted);
                    savemediaitem.Add("Title", subItem.Title);
                    savemediaitem.Add("Type", subItem.Type.ToString());
                    savemediaitem.Add("ContentType", subItem.ContentType.ToString());
                    savemediaitem.Add("IsArchived", subItem.IsArchived);
                    savemediaitem.Add("Description", subItem.Description);


                    string js = JsonConvert.SerializeObject(savemediaitem);
                    var model = _webClient.DownloadData<int>("MediaManager/SaveMedias", new { JS = js });


                }

                UnarchiveSubMedias(subItem, unarchivedMedias);
            }
        }
    }
}
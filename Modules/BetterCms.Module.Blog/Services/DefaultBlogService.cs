// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultBlogService.cs" company="Devbridge Group LLC">
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
using System.Linq;
using System.Security.Principal;

using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterCms.Core.DataContracts.Enums;

using BetterCms.Core.Exceptions;
using BetterCms.Core.Exceptions.Mvc;
using BetterCms.Core.Exceptions.Service;
using BetterCms.Core.Security;
using BetterCms.Core.Services;

using BetterCms.Module.Blog.Content.Resources;
using BetterCms.Module.Blog.Models;
using BetterCms.Module.Blog.Models.Events;
using BetterCms.Module.Blog.ViewModels.Blog;

using BetterCms.Module.MediaManager.Models;

using BetterCms.Module.Pages.Helpers;
using BetterCms.Module.Pages.Models;
using BetterCms.Module.Pages.Models.Enums;
using BetterCms.Module.Pages.Services;
using BetterCms.Module.Pages.ViewModels.Filter;

using BetterCms.Module.Root;
using BetterCms.Module.Root.Models;
using BetterCms.Module.Root.Mvc;
using BetterCms.Module.Root.Mvc.Helpers;
using BetterCms.Module.Root.Services;
using BetterCms.Module.Root.ViewModels.Option;

using Common.Logging;

using FluentNHibernate.Conventions;

using NHibernate.Criterion;
using NHibernate.Linq;

using RootOptionService = BetterCms.Module.Root.Services.IOptionService;
using ConfigurationHelper = BetterCms.Module.Blog.Helpers.ConfigurationHelper;
using BetterCms.Core.WebServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BetterCms.Module.Blog.Services
{
    public class DefaultBlogService : IBlogService
    {
        /// <summary>
        /// The blog post region identifier.
        /// </summary>
        private const string RegionIdentifier = BlogModuleConstants.BlogPostMainContentRegionIdentifier;

        /// <summary>
        /// The category service
        /// </summary>
        private ICategoryService categoryService;

        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();

        protected readonly ICmsConfiguration configuration;
        private readonly IUrlService urlService;
        protected readonly IRepository repository;
        private readonly IOptionService blogOptionService;
        private readonly RootOptionService optionService;
        protected readonly IAccessControlService accessControlService;
        private readonly ISecurityService securityService;
        protected readonly IContentService contentService;
        private readonly IPageService pageService;
        private readonly IRedirectService redirectService;
        protected readonly IMasterPageService masterPageService;
        private readonly ITagService tagService;
        private readonly IUnitOfWork unitOfWork;
        private ITWebClient _webClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBlogService" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="urlService">The URL service.</param>
        /// <param name="repository">The repository.</param>
        /// <param name="blogOptionService">The blog option service.</param>
        /// <param name="accessControlService">The access control service.</param>
        /// <param name="securityService">The security service.</param>
        /// <param name="contentService">The content service.</param>
        /// <param name="tagService">The tag service.</param>
        /// <param name="pageService">The page service.</param>
        /// <param name="redirectService">The redirect service.</param>
        /// <param name="masterPageService">The master page service.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="optionService">The option service.</param>
        /// <param name="categoryService">The category service.</param>
        public DefaultBlogService(ICmsConfiguration configuration, IUrlService urlService, IRepository repository,
            IOptionService blogOptionService, IAccessControlService accessControlService, ISecurityService securityService,
            IContentService contentService, ITagService tagService,
            IPageService pageService, IRedirectService redirectService, IMasterPageService masterPageService,
            IUnitOfWork unitOfWork, RootOptionService optionService, ICategoryService categoryService)
        {
            this.configuration = configuration;
            this.urlService = urlService;
            this.repository = repository;
            this.blogOptionService = blogOptionService;
            this.optionService = optionService;
            this.accessControlService = accessControlService;
            this.securityService = securityService;
            this.contentService = contentService;
            this.pageService = pageService;
            this.redirectService = redirectService;
            this.masterPageService = masterPageService;
            this.tagService = tagService;
            this.unitOfWork = unitOfWork;
            this.categoryService = categoryService;
            _webClient = new BetterCms.Core.WebServices.TWebClient();

        }

        /// <summary>
        /// Creates the blog URL from the given blog title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="unsavedUrls">The unsaved urls.</param>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns>
        /// Created blog URL
        /// </returns>
        public string CreateBlogPermalink(string title, List<string> unsavedUrls = null, IEnumerable<Guid> categoryId = null)
        {
            string newUrl = null;
            if (UrlHelper.GeneratePageUrl != null)
            {
                try
                {
                    newUrl =
                        UrlHelper.GeneratePageUrl(
                            new PageUrlGenerationRequest
                            {
                                Title = title,
                                CategoryId = categoryId
                            });
                }
                catch (Exception ex)
                {
                    Logger.Error("Custom blog post url generation failed.", ex);
                }
            }

            if (string.IsNullOrWhiteSpace(newUrl))
            {
                newUrl = CreateBlogPermalinkDefault(title, unsavedUrls);
            }
            else
            {
                newUrl = urlService.AddPageUrlPostfix(newUrl, "{0}");
            }

            return newUrl;
        }

        /// <summary>
        /// Creates the blog permalink default.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="unsavedUrls">The unsaved urls.</param>
        /// <returns>
        /// Created blog URL
        /// </returns>
        private string CreateBlogPermalinkDefault(string title, List<string> unsavedUrls = null)
        {
            var url = title.Transliterate(true);
            url = urlService.AddPageUrlPostfix(url, configuration.ArticleUrlPattern, unsavedUrls);

            return url;
        }

        /// <summary>
        /// Saves the blog post.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="childContentOptionValues">The child content option values.</param>
        /// <param name="principal">The principal.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <param name="updateActivationIfNotChanged">if set to <c>true</c> update activation time even if it was not changed.</param>
        /// <returns>
        /// Saved blog post entity
        /// </returns>
        /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException"></exception>
        /// <exception cref="SecurityException">Forbidden: Access is denied.</exception>
        public BlogPost SaveBlogPost(BlogPostViewModel request, IList<ContentOptionValuesViewModel> childContentOptionValues, IPrincipal principal, out string[] errorMessages, bool updateActivationIfNotChanged = true)
        {
            errorMessages = new string[0];
            string[] roles;
            if (request.DesirableStatus == ContentStatus.Published)
            {
                accessControlService.DemandAccess(principal, RootModuleConstants.UserRoles.PublishContent);
                roles = new[] { RootModuleConstants.UserRoles.PublishContent };
            }
            else
            {
                accessControlService.DemandAccess(principal, RootModuleConstants.UserRoles.EditContent);
                roles = new[] { RootModuleConstants.UserRoles.EditContent };
            }

            var isNew = request.Id.HasDefaultValue();
            var userCanEdit = securityService.IsAuthorized(RootModuleConstants.UserRoles.EditContent);

            ValidateData(isNew, request);

            BlogPost blogPost;
            BlogPostContent content;
            PageContent pageContent;
            GetBlogPostAndContentEntities(request, principal, roles, ref isNew, out content, out pageContent, out blogPost);
            var beforeChange = new UpdatingBlogModel(blogPost);

            // Master page / layout
            Layout layout;
            Page masterPage;
            Region region;
            LoadDefaultLayoutAndRegion(out layout, out masterPage, out region);

            if (masterPage != null)
            {
                var level = accessControlService.GetAccessLevel(masterPage, principal);
                if (level < AccessLevel.Read)
                {
                    var message = BlogGlobalization.SaveBlogPost_FailedToSave_InaccessibleMasterPage;
                    const string logMessage = "Failed to save blog post. Selected template for page layout is inaccessible.";
                    throw new ValidationException(() => message, logMessage);
                }
            }

            if (pageContent.Region == null)
            {
                pageContent.Region = region;
            }

            // Load master pages for updating page's master path and page's children master path
            IList<Guid> newMasterIds;
            IList<Guid> oldMasterIds;
            IList<Guid> childrenPageIds;
            IList<MasterPage> existingChildrenMasterPages;
            PrepareForUpdateChildrenMasterPages(isNew, blogPost, request, out newMasterIds, out oldMasterIds, out childrenPageIds, out existingChildrenMasterPages);

            // TODO: TEST AND TRY TO FIX IT: TRANSACTION HERE IS REQUIRED!
            // UnitOfWork.BeginTransaction(); // NOTE: this causes concurrent data exception.

            Redirect redirectCreated = null;
            if (!isNew && userCanEdit && !string.Equals(blogPost.PageUrl, request.BlogUrl) && !string.IsNullOrWhiteSpace(request.BlogUrl))
            {
                request.BlogUrl = urlService.FixUrl(request.BlogUrl);
                pageService.ValidatePageUrl(request.BlogUrl, request.Id);
                if (request.RedirectFromOldUrl)
                {
                    var redirect = redirectService.CreateRedirectEntity(blogPost.PageUrl, request.BlogUrl);
                    if (redirect != null)
                    {

                        //JObject redirectdetails = new JObject();
                        //redirectdetails.Add("id", redirect.Id);
                        //redirectdetails.Add("version", redirect.Version);
                        //redirectdetails.Add("isDeleted", redirect.IsDeleted);
                        //redirectdetails.Add("pageUrl", redirect.PageUrl);
                        //redirectdetails.Add("redirectUrl", redirect.RedirectUrl);
                        //string redirectjs = JsonConvert.SerializeObject(redirectdetails);

                        //var redirectresponse = _webClient.DownloadData<string>("Blog/SaveRedirect", new { JS = redirectjs });
                        //redirect.Id = new Guid(redirectresponse);
                        repository.Save(redirect);
                        redirectCreated = redirect;
                    }
                }

                blogPost.PageUrl = urlService.FixUrl(request.BlogUrl);
            }

            // Push to change modified data each time.
            blogPost.ModifiedOn = DateTime.Now;

            if (userCanEdit)
            {
                blogPost.Title = request.Title;
                blogPost.Description = request.IntroText;
                blogPost.Author = request.AuthorId.HasValue ? repository.AsProxy<Author>(request.AuthorId.Value) : null;
                //if (request.AuthorId.HasValue)
                //{
                //JObject requestforauthor = new JObject();
                //requestforauthor.Add("AuthorId", request.AuthorId.Value);
                //string authorobj = JsonConvert.SerializeObject(requestforauthor);
                //var authormodel = _webClient.DownloadData<string>("Blog/GetAuthorDetails", new { Js = authorobj });
                //dynamic regionInfo = JObject.Parse(authormodel);
                //Author authordetails = new Author();
                //authordetails.Id = request.AuthorId.Value;
                //authordetails.Version = regionInfo.Version;
                //authordetails.IsDeleted = regionInfo.IsDeleted;
                //authordetails.CreatedOn = regionInfo.CreatedOn;
                //authordetails.CreatedByUser = regionInfo.CreatedByUser;
                //authordetails.ModifiedOn = regionInfo.ModifiedOn;
                //authordetails.ModifiedByUser = regionInfo.ModifiedByUser;
                //authordetails.DeletedOn = (!string.IsNullOrEmpty(regionInfo.DeletedOn.ToString())) ? regionInfo.DeletedOn : null;
                //authordetails.DeletedByUser = (!string.IsNullOrEmpty(regionInfo.DeletedByUser.ToString())) ? regionInfo.DeletedByUser : null;
                //authordetails.Name = regionInfo.Name;

                //if (!string.IsNullOrEmpty(regionInfo.ImageId.ToString()))
                //{
                //    MediaImage image1 = new MediaImage();
                //    image1.Id = new Guid(regionInfo.ImageId.ToString());
                //    JObject requestforimagedetails = new JObject();
                //    requestforimagedetails.Add("imageId", image1.Id);
                //    string imageobj = JsonConvert.SerializeObject(requestforimagedetails);
                //    var ImageModel = _webClient.DownloadData<string>("Blog/GetImageDetails", new { Js = imageobj });
                //    dynamic ImageInfo = JObject.Parse(ImageModel);
                //    image1.Caption = (!string.IsNullOrEmpty(ImageInfo.Caption.ToString())) ? ImageInfo.Caption : null;
                //    image1.ImageAlign = (!string.IsNullOrEmpty(ImageInfo.ImageAlign.ToString())) ? ImageInfo.ImageAlign : null;
                //    image1.Width = ImageInfo.Width;
                //    image1.Height = ImageInfo.Height;
                //    image1.CropCoordX1 = (!string.IsNullOrEmpty(ImageInfo.CropCoordX1.ToString())) ? ImageInfo.CropCoordX1 : null;
                //    image1.CropCoordX2 = (!string.IsNullOrEmpty(ImageInfo.CropCoordX2.ToString())) ? ImageInfo.CropCoordX2 : null;
                //    image1.CropCoordY1 = (!string.IsNullOrEmpty(ImageInfo.CropCoordY1.ToString())) ? ImageInfo.CropCoordY1 : null;
                //    image1.CropCoordY2 = (!string.IsNullOrEmpty(ImageInfo.CropCoordY2.ToString())) ? ImageInfo.CropCoordY2 : null;
                //    image1.OriginalWidth = ImageInfo.OriginalWidth;
                //    image1.OriginalHeight = ImageInfo.OriginalHeight;
                //    image1.OriginalSize = ImageInfo.OriginalSize;
                //    image1.OriginalUri = ImageInfo.OriginalUri;
                //    image1.PublicOriginallUrl = ImageInfo.PublicOriginallUrl;
                //    image1.IsOriginalUploaded = (!string.IsNullOrEmpty(ImageInfo.IsOriginalUploaded.ToString())) ? ImageInfo.IsOriginalUploaded : null;
                //    image1.ThumbnailWidth = ImageInfo.ThumbnailWidth;
                //    image1.ThumbnailHeight = ImageInfo.ThumbnailHeight;
                //    image1.ThumbnailSize = ImageInfo.ThumbnailSize;
                //    image1.ThumbnailUri = ImageInfo.ThumbnailUri;
                //    image1.PublicThumbnailUrl = ImageInfo.PublicThumbnailUrl;
                //    image1.IsThumbnailUploaded = (!string.IsNullOrEmpty(ImageInfo.IsThumbnailUploaded.ToString())) ? ImageInfo.IsThumbnailUploaded : null;
                //    image1.Version = ImageInfo.Version;
                //    image1.IsDeleted = ImageInfo.IsDeleted;
                //    image1.CreatedOn = ImageInfo.CreatedOn;
                //    image1.CreatedByUser = ImageInfo.CreatedByUser;
                //    image1.ModifiedOn = ImageInfo.ModifiedOn;
                //    image1.ModifiedByUser = ImageInfo.ModifiedByUser;
                //    image1.DeletedOn = (!string.IsNullOrEmpty(ImageInfo.DeletedOn.ToString())) ? ImageInfo.DeletedOn : null;
                //    image1.DeletedByUser = (!string.IsNullOrEmpty(ImageInfo.DeletedByUser.ToString())) ? ImageInfo.DeletedByUser : null;
                //    if (!string.IsNullOrEmpty(ImageInfo.FolderId.ToString()))
                //    {
                //        MediaFolder folder = new MediaFolder();
                //        folder.Id = ImageInfo.FolderId;
                //        image1.Folder = folder;
                //    }
                //    else
                //    {
                //        image1.Folder = null;
                //    }
                //    image1.Title = ImageInfo.Title;
                //    image1.Type = ImageInfo.Type;
                //    image1.ContentType = ImageInfo.ContentType;
                //    image1.IsArchived = ImageInfo.IsArchived;
                //    if (!string.IsNullOrEmpty(ImageInfo.FolderId.ToString()))
                //    {
                //        image1.Original.Id = ImageInfo.OriginalId;
                //    }
                //    else
                //    {
                //        image1.Original = null;

                //    }

                //    image1.PublishedOn = ImageInfo.PublishedOn;
                //    //image.Image.Id = ImageInfo.ImageId;
                //    image1.Description = (!string.IsNullOrEmpty(ImageInfo.Description.ToString())) ? ImageInfo.Description : null;

                //    authordetails.Image = image1;

                //}
                //else
                //{
                //    authordetails.Image = null;
                //}

                //authordetails.Description = (!string.IsNullOrEmpty(regionInfo.Description.ToString())) ? regionInfo.Description : null;

                
                //blogPost.Author = authordetails;
                //}
                //else
                //{
                //    blogPost.Author = null;
                //}


                blogPost.Image = (request.Image != null && request.Image.ImageId.HasValue) ? repository.AsProxy<MediaImage>(request.Image.ImageId.Value) : null;
                if (isNew || request.DesirableStatus == ContentStatus.Published)
                {
                    if (updateActivationIfNotChanged)
                    {
                        blogPost.ActivationDate = request.LiveFromDate;
                        blogPost.ExpirationDate = TimeHelper.FormatEndDate(request.LiveToDate);
                    }
                    else
                    {
                        blogPost.ActivationDate = TimeHelper.GetFirstIfTheSameDay(blogPost.ActivationDate, request.LiveFromDate);
                        blogPost.ExpirationDate = TimeHelper.GetFirstIfTheSameDay(blogPost.ExpirationDate, TimeHelper.FormatEndDate(request.LiveToDate));
                    }
                }
            }

            if (isNew)
            {
                if (!string.IsNullOrWhiteSpace(request.BlogUrl))
                {
                    blogPost.PageUrl = urlService.FixUrl(request.BlogUrl);
                    pageService.ValidatePageUrl(blogPost.PageUrl);
                }
                else
                {
                    blogPost.PageUrl = CreateBlogPermalink(request.Title, null, request.Categories != null ? request.Categories.Select(c => Guid.Parse(c.Key)) : null);
                }

                blogPost.MetaTitle = request.MetaTitle ?? request.Title;
                if (masterPage != null)
                {
                    blogPost.MasterPage = masterPage;
                    masterPageService.SetPageMasterPages(blogPost, masterPage.Id);
                }
                else
                {
                    blogPost.Layout = layout;
                }
                UpdateStatus(blogPost, request.DesirableStatus);
                AddDefaultAccessRules(blogPost, principal, masterPage);
            }
            else if (request.DesirableStatus == ContentStatus.Published
                || blogPost.Status == PageStatus.Preview)
            {
                // Update only if publishing or current status is preview.
                // Else do not change, because it may change from published to draft status 
                UpdateStatus(blogPost, request.DesirableStatus);
            }

            // Create content.
            var newContent = new BlogPostContent
            {
                Id = content != null ? content.Id : Guid.Empty,
                Name = request.Title,
                Html = request.Content ?? string.Empty,
                OriginalText = request.OriginalText,
                EditInSourceMode = request.EditInSourceMode,
                ActivationDate = request.LiveFromDate,
                ExpirationDate = TimeHelper.FormatEndDate(request.LiveToDate),
                ContentTextMode = request.ContentTextMode,
            };

            if (!updateActivationIfNotChanged && content != null)
            {
                newContent.ActivationDate = TimeHelper.GetFirstIfTheSameDay(content.ActivationDate, newContent.ActivationDate);
                newContent.ExpirationDate = TimeHelper.GetFirstIfTheSameDay(content.ExpirationDate, newContent.ExpirationDate);
            }

            if (request.ContentTextMode == ContentTextMode.Markdown
                && request.Content == null
                && request.OriginalText != null)
            {
                newContent.Html = MarkdownConverter.ToHtml(request.OriginalText);
            }

            // Preserve content if user is not authorized to change it.
            if (!userCanEdit)
            {
                if (content == null)
                {
                    throw new SecurityException("Forbidden: Access is denied."); // User has no rights to create new content.
                }

                var contentToPublish = (BlogPostContent)(content.History != null
                    ? content.History.FirstOrDefault(c => c.Status == ContentStatus.Draft) ?? content
                    : content);

                newContent.Name = contentToPublish.Name;
                newContent.Html = contentToPublish.Html;
                newContent.ContentTextMode = contentToPublish.ContentTextMode;
                newContent.OriginalText = contentToPublish.OriginalText;
            }

            content = SaveContentWithStatusUpdate(isNew, newContent, request, principal);
            pageContent.Content = content;
            optionService.SaveChildContentOptions(content, childContentOptionValues, request.DesirableStatus);

            blogPost.PageUrlHash = blogPost.PageUrl.UrlHash();
            blogPost.UseCanonicalUrl = request.UseCanonicalUrl;

            MapExtraProperties(isNew, blogPost, content, pageContent, request, principal);

            // Notify about page properties changing.
            var cancelEventArgs = Events.BlogEvents.Instance.OnBlogChanging(beforeChange, new UpdatingBlogModel(blogPost));
            if (cancelEventArgs.Cancel)
            {
                errorMessages = cancelEventArgs.CancellationErrorMessages.ToArray();
                return null;
            }

            if (isNew || userCanEdit)
            {
                categoryService.CombineEntityCategories<BlogPost, PageCategory>(blogPost, request.Categories);
            }

            var oldLanguageId = blogPost.Language != null ? blogPost.Language.Id : (Guid?)null;
            var newLanguageId = request.LanguageId;
            if (oldLanguageId != newLanguageId)
            {
                blogPost.Language = request.LanguageId.HasValue ? repository.AsProxy<Language>(request.LanguageId.Value) : null;
            }

            //JObject blogdetails = new JObject();
            //blogdetails.Add("Id", blogPost.Id);
            //if (blogPost.Author != null)
            //{
            //    blogdetails.Add("authorId", blogPost.Author.Id);
            //}
            //else
            //{
            //    blogdetails.Add("authorId", null);
            //}

            //blogdetails.Add("activationDate", blogPost.ActivationDate.ToString());
            //if (blogPost.ExpirationDate != null)
            //{
            //    blogdetails.Add("expirationDate", blogPost.ExpirationDate.ToString());
            //}
            //else
            //{
            //    blogdetails.Add("expirationDate", null);
            //}

            //blogdetails.Add("version", blogPost.Version);
            //blogdetails.Add("isDeleted", blogPost.IsDeleted);
            //blogdetails.Add("pageUrl", blogPost.PageUrl);
            //blogdetails.Add("title", blogPost.Title);

            //blogdetails.Add("publishedOn", blogPost.PublishedOn.ToString());
            //blogdetails.Add("metaTitle", blogPost.MetaTitle);
            //blogdetails.Add("metaKeywords", blogPost.MetaKeywords);
            //blogdetails.Add("metaDescription", blogPost.MetaDescription);
            //blogdetails.Add("status", blogPost.Status.ToString());
            //blogdetails.Add("pageUrlHash", blogPost.PageUrlHash);
            //if (blogPost.MasterPage != null)
            //{
            //    blogdetails.Add("masterPageId", blogPost.MasterPage.Id);
            //    blogdetails.Add("layoutId", null);
            //}
            //else
            //{
            //    blogdetails.Add("layoutId", blogPost.Layout.Id);
            //    blogdetails.Add("masterPageId", null);
            //}

            //blogdetails.Add("isMasterPage", blogPost.IsMasterPage);
            //if (blogPost.Language != null)
            //{
            //    blogdetails.Add("languageId", blogPost.Language.Id);
            //}
            //else
            //{
            //    blogdetails.Add("languageId", null);
            //}
            //blogdetails.Add("languageGroupIdentifier", blogPost.LanguageGroupIdentifier);
            //blogdetails.Add("forceAccessProtocol", blogPost.ForceAccessProtocol.ToString());

            //blogdetails.Add("description", blogPost.Description);
            //if (blogPost.Image != null)
            //{
            //    blogdetails.Add("imageId", blogPost.Image.Id);

            //}
            //else
            //{
            //    blogdetails.Add("imageId", null);
            //}

            //blogdetails.Add("customCss", blogPost.CustomCss);
            //blogdetails.Add("customJs", blogPost.CustomJS);
            //blogdetails.Add("useCanonicalUrl", blogPost.UseCanonicalUrl);
            //blogdetails.Add("useNoFollow", blogPost.UseNoFollow);
            //blogdetails.Add("useNoIndex", blogPost.UseNoIndex);
            //blogdetails.Add("categoryId", null);
            //if (blogPost.SecondaryImage != null)
            //{
            //    blogdetails.Add("secondaryImageId", blogPost.SecondaryImage.Id);
            //}
            //else
            //{
            //    blogdetails.Add("secondaryImageId", null);
            //}
            //if (blogPost.FeaturedImage != null)
            //{
            //    blogdetails.Add("featuredImageId", blogPost.FeaturedImage.Id);
            //}
            //else
            //{
            //    blogdetails.Add("featuredImageId", null);
            //}
            //blogdetails.Add("isArchived", blogPost.IsArchived);


            //string blogpostjs = JsonConvert.SerializeObject(blogdetails);
            //var blogPostModel = ""; //_webClient.DownloadData<string>("Blog/SaveBlogPost", new { JS = blogpostjs });
            //if (blogPost.Categories != null)
            //{
            //    for (int i = 0; i < blogPost.Categories.Count; i++)
            //    {
            //        var categoryId = blogPost.Categories[i].Category.Id;
            //        var pageId = blogPostModel;
            //        JObject categorydetails = new JObject();
            //        categorydetails.Add("id", blogPost.Categories[i].Id);
            //        categorydetails.Add("version", blogPost.Categories[i].Version);
            //        categorydetails.Add("isDeleted", blogPost.Categories[i].IsDeleted);
            //        categorydetails.Add("pageId", pageId);
            //        categorydetails.Add("categoryId", categoryId);
            //        string categoryjs = JsonConvert.SerializeObject(categorydetails);
            //        var catergoryModel = _webClient.DownloadData<string>("Blog/SaveCategory", new { JS = categoryjs });

            //    }

            //}


            //blogPost.Id = new Guid(blogPostModel);

            repository.Save(blogPost);
           // JObject blogpostcontentdetails = new JObject();
           // blogpostcontentdetails.Add("id", content.Id);
           // blogpostcontentdetails.Add("activationdDate", content.ActivationDate.ToString());
           // if (content.ExpirationDate != null)
           // {
           //     blogpostcontentdetails.Add("expirationDate", content.ExpirationDate.ToString());
           // }
           // else
           // {
           //     blogpostcontentdetails.Add("expirationDate", null);
           // }

           // blogpostcontentdetails.Add("customCss", content.CustomCss);
           // blogpostcontentdetails.Add("useCustomCss", content.UseCustomCss);
           // blogpostcontentdetails.Add("customJs", content.CustomJs);
           // blogpostcontentdetails.Add("useCustomJs", content.UseCustomJs);
           // blogpostcontentdetails.Add("html", content.Html.ToString());
           // blogpostcontentdetails.Add("editInSourceMode", content.EditInSourceMode);
           // blogpostcontentdetails.Add("originalText", content.OriginalText);
           // blogpostcontentdetails.Add("contentTextMode", content.ContentTextMode.ToString());
           // blogpostcontentdetails.Add("version", content.Version);
           // blogpostcontentdetails.Add("isDeleted", content.IsDeleted);
           // blogpostcontentdetails.Add("name", content.Name);
           // blogpostcontentdetails.Add("previewUrl", content.PreviewUrl);
           // blogpostcontentdetails.Add("status", content.Status.ToString());
           // blogpostcontentdetails.Add("publishedOn", content.PublishedOn.ToString());
           // blogpostcontentdetails.Add("publishedByUser", content.PublishedByUser);
           // if (content.Original != null)
           // {
           //     blogpostcontentdetails.Add("originalId", content.Original.Id);
           // }
           // else
           // {
           //     blogpostcontentdetails.Add("originalId", null);
           // }
           // string contentjs = JsonConvert.SerializeObject(blogpostcontentdetails);
           //// var blogPostContentModel = _webClient.DownloadData<string>("Blog/SaveBlogPostContent", new { JS = contentjs });
           // //content.Id = new Guid(blogPostContentModel);
            repository.Save(content);


            //JObject pagecontentobj = new JObject();
            //pagecontentobj.Add("id", pageContent.Id);
            //pagecontentobj.Add("version", pageContent.Version);
            //pagecontentobj.Add("isDeleted", pageContent.IsDeleted);
            //pagecontentobj.Add("pageId", pageContent.Page.Id);
            //pagecontentobj.Add("contentId", pageContent.Content.Id);
            //pagecontentobj.Add("regionId", pageContent.Region.Id);
            //pagecontentobj.Add("order", pageContent.Order);

            //string jsobj = JsonConvert.SerializeObject(pagecontentobj);
            //var blogpagecontentmodel = _webClient.DownloadData<string>("Blog/SavePageContent", new { JS = jsobj });
            ////pageContent.Id = new Guid(blogpagecontentmodel);
            repository.Save(pageContent);

            masterPageService.UpdateChildrenMasterPages(existingChildrenMasterPages, oldMasterIds, newMasterIds, childrenPageIds);

            pageContent.Content = content;
            blogPost.PageContents = new[] { pageContent };

            IList<Tag> newTags = null;
            if (userCanEdit)
            {
                newTags = SaveTags(blogPost, request);
            }

            if (userCanEdit && ConfigurationHelper.IsFillSeoDataFromArticlePropertiesEnabled(configuration))
            {
                FillMetaInfo(blogPost);
            }

            // Commit
           unitOfWork.Commit();

            // Notify about new created tags.
            Events.RootEvents.Instance.OnTagCreated(newTags);

            // Notify about new or updated blog post.
            if (isNew)
            {
                Events.BlogEvents.Instance.OnBlogCreated(blogPost);
            }
            else
            {
                Events.BlogEvents.Instance.OnBlogUpdated(blogPost);
            }

            // Notify about redirect creation.
            if (redirectCreated != null)
            {
                Events.PageEvents.Instance.OnRedirectCreated(redirectCreated);
            }

            return blogPost;
        }

        protected virtual IList<Tag> SaveTags(BlogPost blogPost, BlogPostViewModel request)
        {
            IList<Tag> newTags;
            tagService.SavePageTags(blogPost, request.Tags, out newTags);

            return newTags;
        }

        protected virtual BlogPostContent SaveContentWithStatusUpdate(bool isNew, BlogPostContent newContent, BlogPostViewModel request, IPrincipal principal)
        {
            return (BlogPostContent)contentService.SaveContentWithStatusUpdate(newContent, request.DesirableStatus);
        }

        protected virtual void PrepareForUpdateChildrenMasterPages(bool isNew, BlogPost entity, BlogPostViewModel model, out IList<Guid> newMasterIds,
            out IList<Guid> oldMasterIds, out IList<Guid> childrenPageIds, out IList<MasterPage> existingChildrenMasterPages)
        {
            newMasterIds = null;
            oldMasterIds = null;
            childrenPageIds = null;
            existingChildrenMasterPages = null;
        }

        protected virtual void GetBlogPostAndContentEntities(BlogPostViewModel request, IPrincipal principal, string[] roles,
            ref bool isNew, out BlogPostContent content, out PageContent pageContent, out BlogPost blogPost)
        {
            content = null;
            pageContent = null;

            // Loading blog post and it's content, or creating new, if such not exists
            if (!isNew)
            {
                var blogPostFuture = repository
                    .AsQueryable<BlogPost>(b => b.Id == request.Id)
                    .ToFuture();

                //JObject requestdetails = new JObject();
                //requestdetails.Add("pageId", request.Id);
                //requestdetails.Add("authorId", request.AuthorId);

                //string jsobj = JsonConvert.SerializeObject(requestdetails);
                //var blogPostFuturemodel = _webClient.DownloadData<string>("Blog/GetBlogPostDetails", new { Js = jsobj });
                //dynamic Info = JObject.Parse(blogPostFuturemodel);
                //blogPost = new BlogPost();
                //blogPost.Id = request.Id;
                //blogPost.ActivationDate = Info.ActivationDate;

                //if (string.IsNullOrEmpty(Info.ExpirationDate.ToString()))
                //{
                //    blogPost.ExpirationDate = null;
                //}
                //else
                //{
                //    blogPost.ExpirationDate = Info.ExpirationDate;
                //}
                //if (string.IsNullOrEmpty(Info.Description.ToString()))
                //{
                //    blogPost.Description = null;
                //}
                //else
                //{
                //    blogPost.Description = Info.Description;
                //}
                //if (string.IsNullOrEmpty(Info.ImageId.ToString()))
                //{
                //    blogPost.Image = null;
                //}
                //else
                //{
                //    MediaImage image = new MediaImage();
                //    image.Id = new Guid(Info.ImageId.ToString());
                //    blogPost.Image = image;
                //}
                //if (string.IsNullOrEmpty(Info.CustomCss.ToString()))
                //{
                //    blogPost.CustomCss = null;
                //}
                //else
                //{
                //    blogPost.CustomCss = Info.CustomCss;
                //}
                //if (string.IsNullOrEmpty(Info.CustomJS.ToString()))
                //{
                //    blogPost.CustomJS = null;
                //}
                //else
                //{
                //    blogPost.CustomJS = Info.CustomJS;
                //}
                //blogPost.UseCanonicalUrl = Info.UseCanonicalUrl;
                //blogPost.UseNoFollow = Info.UseNoFollow;
                //blogPost.UseNoIndex = Info.UseNoIndex;
                //IList<PageCategory> blogcategories = new List<PageCategory>();
                //List<PageCategory> blogcategorylist = new List<PageCategory>();

                //JObject requestforcategories = new JObject();
                //requestforcategories.Add("pageId", request.Id);


                //string categoriesobj = JsonConvert.SerializeObject(requestforcategories);
                //var categoriesmodel = _webClient.DownloadData<JArray>("Blog/GetBlogPageCategoryDetails", new { Js = categoriesobj });

                //for (int i = 0; i < categoriesmodel.Count(); i++)
                //{
                //    PageCategory blogcategory = new PageCategory();                    
                //    blogcategory.Id = new Guid(categoriesmodel[i]["PagecategoryId"].ToString());
                //    blogcategory.Version = Convert.ToInt32(categoriesmodel[i]["Version"]);
                //    blogcategory.IsDeleted = (bool)categoriesmodel[i]["IsDeleted"];
                //    blogcategory.CreatedOn = (DateTime)categoriesmodel[i]["CreatedOn"];
                //    blogcategory.CreatedByUser = categoriesmodel[i]["CreatedByUser"].ToString();
                //    blogcategory.ModifiedOn = (DateTime)categoriesmodel[i]["ModifiedOn"];
                //    blogcategory.ModifiedByUser = categoriesmodel[i]["ModifiedByUser"].ToString();
                //    if (!string.IsNullOrEmpty(categoriesmodel[i]["DeletedOn"].ToString()))
                //    {
                //        blogcategory.DeletedOn = (DateTime)categoriesmodel[i]["DeletedOn"];
                //    }
                //    else
                //    {
                //        blogcategory.DeletedOn = null;
                //    }
                //    if (!string.IsNullOrEmpty(categoriesmodel[i]["DeletedByUser"].ToString()))
                //    {
                //        blogcategory.DeletedByUser = categoriesmodel[i]["DeletedByUser"].ToString();
                //    }
                //    else
                //    {
                //        blogcategory.DeletedByUser = null;
                //    }
                //    Category category = new Category();
                //    category.Id = new Guid(categoriesmodel[i]["CategoryId"].ToString());
                //    JObject requestforcategory = new JObject();
                //    requestforcategory.Add("categoryId", category.Id);


                //    string categoryobj = JsonConvert.SerializeObject(requestforcategory);
                //    var categorymodel = _webClient.DownloadData<string>("Blog/GetBlogCategoryDetails", new { Js = categoryobj });
                //    dynamic categoryInfo = JObject.Parse(categorymodel);
                //    category.Version = categoryInfo.Version;
                //    category.IsDeleted = categoryInfo.IsDeleted;
                //    category.CreatedOn = categoryInfo.CreatedOn;
                //    category.CreatedByUser = categoryInfo.CreatedByUser;
                //    category.ModifiedOn = categoryInfo.ModifiedOn;
                //    category.ModifiedByUser = categoryInfo.ModifiedByUser;
                //    category.DeletedOn = (!string.IsNullOrEmpty(categoryInfo.DeletedOn.ToString())) ? categoryInfo.DeletedOn : null;
                //    category.DeletedByUser = (!string.IsNullOrEmpty(categoryInfo.DeletedByUser.ToString())) ? categoryInfo.DeletedByUser : null;
                //    category.Name = categoryInfo.Name;
                //    category.DisplayOrder = categoryInfo.DisplayOrder;
                //    category.Macro = (!string.IsNullOrEmpty(categoryInfo.Macro.ToString())) ? categoryInfo.Macro : null; 
                //    CategoryTree categorytree = new CategoryTree();
                //    categorytree.Id = new Guid(categoryInfo.CategoryTreeId.ToString());
                //    category.CategoryTree = categorytree;

                //    blogcategory.Category = category;
                //    blogcategorylist.Add(blogcategory);
                //}


                ////for (int i = 0; i < request.Categories.Count(); i++)
                ////{
                ////    PageCategory blogcategory = new PageCategory();
                ////    blogcategory.Id = new Guid(request.Categories[i].Key.ToString());

                ////    blogcategorylist.Add(blogcategory);
                ////}
                //blogcategories = blogcategorylist;
                //blogPost.Categories = blogcategories;
                //if (string.IsNullOrEmpty(Info.SecondaryImageId.ToString()))
                //{
                //    blogPost.SecondaryImage = null;
                //}
                //else
                //{
                //    MediaImage secondaryimage = new MediaImage();
                //    secondaryimage.Id = new Guid(Info.SecondaryImageId.ToString());
                //    blogPost.SecondaryImage = secondaryimage;
                //}
                //if (string.IsNullOrEmpty(Info.FeaturedImageId.ToString()))
                //{
                //    blogPost.FeaturedImage = null;
                //}
                //else
                //{
                //    MediaImage featuredimage = new MediaImage();
                //    featuredimage.Id = new Guid(Info.FeaturedImageId.ToString());
                //    blogPost.FeaturedImage = featuredimage;
                //}

                //blogPost.IsArchived = Info.IsArchived;
                //blogPost.Version = Info.Version;
                //blogPost.PageUrl = Info.PageUrl;
                //blogPost.Title = Info.Title;

                //if (string.IsNullOrEmpty(Info.LayoutId.ToString()))
                //{
                //    blogPost.Layout = null;
                //}
                //else
                //{
                //    Layout layoutdetails = new Layout();
                //    layoutdetails.Id = new Guid(Info.LayoutId.ToString());
                //    JObject requestforlayoutdetails = new JObject();
                //    requestforlayoutdetails.Add("layoutId", layoutdetails.Id);
                //    string layoutobj = JsonConvert.SerializeObject(requestforlayoutdetails);
                //    var layoutmodel = _webClient.DownloadData<string>("Blog/GetLayoutDetails", new { Js = layoutobj });
                //    dynamic layoutInfo = JObject.Parse(layoutmodel);

                //    layoutdetails.Version = layoutInfo.Version;
                //    layoutdetails.IsDeleted = layoutInfo.IsDeleted;
                //    layoutdetails.CreatedOn = layoutInfo.CreatedOn;
                //    layoutdetails.CreatedByUser = layoutInfo.CreatedByUser;
                //    layoutdetails.ModifiedOn = layoutInfo.ModifiedOn;
                //    layoutdetails.ModifiedByUser = layoutInfo.ModifiedByUser;
                //    layoutdetails.DeletedOn = (!string.IsNullOrEmpty(layoutInfo.DeletedOn.ToString())) ? layoutInfo.DeletedOn : null;
                //    layoutdetails.DeletedByUser = (!string.IsNullOrEmpty(layoutInfo.DeletedByUser.ToString())) ? layoutInfo.DeletedByUser : null;
                //    layoutdetails.Name = layoutInfo.Name;
                //    layoutdetails.LayoutPath = layoutInfo.LayoutPath;


                //    layoutdetails.PreviewUrl = (!string.IsNullOrEmpty(layoutInfo.PreviewUrl.ToString())) ? layoutInfo.PreviewUrl : null;
                //    blogPost.Layout = layoutdetails;
                //}
                //if (string.IsNullOrEmpty(Info.MetaTitle.ToString()))
                //{
                //    blogPost.MetaTitle = null;
                //}
                //else
                //{
                //    blogPost.MetaTitle = Info.MetaTitle;
                //}
                //if (string.IsNullOrEmpty(Info.MetaKeywords.ToString()))
                //{
                //    blogPost.MetaKeywords = null;
                //}
                //else
                //{
                //    blogPost.MetaKeywords = Info.MetaKeywords;
                //}
                //if (string.IsNullOrEmpty(Info.MetaDescription.ToString()))
                //{
                //    blogPost.MetaDescription = null;
                //}
                //else
                //{
                //    blogPost.MetaDescription = Info.MetaDescription;
                //}

                //blogPost.Status = Info.Status;
                //blogPost.PageUrlHash = Info.PageUrlHash;
                //if (string.IsNullOrEmpty(Info.MasterPageId.ToString()))
                //{
                //    blogPost.MasterPage = null;
                //}
                //else
                //{
                //    Page masterpage = new Page();
                //    masterpage.Id = new Guid(Info.MasterPageId.ToString());
                //    blogPost.MasterPage = masterpage;
                //}
                //blogPost.IsMasterPage = Info.IsMasterPage;
                //if (string.IsNullOrEmpty(Info.LanguageId.ToString()))
                //{
                //    blogPost.Language = null;
                //}
                //else
                //{
                //    Language language = new Language();
                //    language.Id = new Guid(Info.LanguageId.ToString());
                //    blogPost.Language = language;
                //}
                //if (string.IsNullOrEmpty(Info.LanguageGroupIdentifier.ToString()))
                //{
                //    blogPost.LanguageGroupIdentifier = null;
                //}
                //else
                //{
                //    blogPost.LanguageGroupIdentifier = Info.LanguageGroupIdentifier;
                //}
                //blogPost.ForceAccessProtocol = Info.ForceAccessProtocol;
                //JObject requestforruleid = new JObject();
                //requestforruleid.Add("pageId", blogPost.Id);
                //string ruleidobj = JsonConvert.SerializeObject(requestforruleid);
                //var RuleIdModel = _webClient.DownloadData<JArray>("Blog/GetBlogAccessRulesId", new { Js = ruleidobj });
                //if (RuleIdModel.Count > 0)
                //{
                //    IList<AccessRule> accessrules = new List<AccessRule>();
                //    List<AccessRule> accessruleslist = new List<AccessRule>();
                //    for (int k = 0; k < RuleIdModel.Count; k++)
                //    {
                //        Guid accessruleId = new Guid(RuleIdModel[k]["AccessruleId"].ToString());
                //        JObject requestforaccessrule = new JObject();
                //        requestforaccessrule.Add("accessrulesId", accessruleId);
                //        string accessruleobj = JsonConvert.SerializeObject(requestforaccessrule);
                //        var accessruleModel = _webClient.DownloadData<string>("Blog/GetAccessRules", new { Js = accessruleobj });
                //        dynamic accessruleinfo = JObject.Parse(accessruleModel);
                //        AccessRule rule = new AccessRule();
                //        rule.Id = accessruleId;
                //        rule.Version = accessruleinfo.Version;
                //        rule.IsDeleted = accessruleinfo.IsDeleted;
                //        rule.CreatedOn = accessruleinfo.CreatedOn;
                //        rule.CreatedByUser = accessruleinfo.CreatedByUser;
                //        rule.ModifiedOn = accessruleinfo.ModifiedOn;
                //        rule.ModifiedByUser = accessruleinfo.ModifiedByUser;
                //        rule.DeletedOn = (!string.IsNullOrEmpty(accessruleinfo.DeletedOn.ToString())) ? accessruleinfo.DeletedOn : null;
                //        rule.DeletedByUser = (!string.IsNullOrEmpty(accessruleinfo.DeletedByUser.ToString())) ? accessruleinfo.DeletedByUser : null;
                //        rule.Identity = accessruleinfo.Identity;
                //        rule.AccessLevel = accessruleinfo.AccessLevel;
                //        rule.IsForRole = accessruleinfo.IsForRole;
                //        accessruleslist.Add(rule);
                //    }
                //    accessrules = accessruleslist;
                //    blogPost.AccessRules = accessrules;


                //}




                //Author author = new Author();
                //author.Id = new Guid(request.AuthorId.ToString());
                //author.Name = Info.Author_Name;

                //author.Description = (string.IsNullOrEmpty(Info.Author_Description.ToString())) ? null : Info.Author_Description;

                //MediaImage authorimage = new MediaImage();
                //if (string.IsNullOrEmpty(Info.Author_ImageId.ToString()))
                //{
                //    authorimage = null;
                //}
                //else
                //{
                //    authorimage.Id = new Guid(Info.Author_ImageId.ToString());
                //}
                //author.Image = authorimage;
                //author.Version = Info.Author_Version;
                //blogPost.Author = author;


                content = repository
                    .AsQueryable<BlogPostContent>(c => c.PageContents.Any(x => x.Page.Id == request.Id && !x.IsDeleted))
                    .ToFuture()
                    .FirstOrDefault();


                //JObject requestforcontentdetails = new JObject();
                //requestforcontentdetails.Add("pageId", request.Id);


                //string contentobj = JsonConvert.SerializeObject(requestforcontentdetails);
                //var contentmodel = _webClient.DownloadData<string>("Blog/GetContentDetails", new { Js = contentobj });
                //dynamic contentInfo = JObject.Parse(contentmodel);
                //content = new BlogPostContent();
                //content.Id = contentInfo.Id;
                //content.Name = contentInfo.Name;
                //content.IsDeleted = contentInfo.IsDeleted;
                //content.Status = contentInfo.Status;
                //content.Version = contentInfo.Version;
                //if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
                //{

                //    content.Original.Id = new Guid(contentInfo.OriginalId.ToString());
                //}
                //else
                //{
                //    content.Original = null;
                //}
                //content.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
                //content.CreatedByUser = contentInfo.CreatedByUser;
                //content.CreatedOn = contentInfo.CreatedOn;
                //content.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
                //content.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
                //content.ModifiedByUser = contentInfo.ModifiedByUser;
                //content.ModifiedOn = contentInfo.ModifiedOn;
                //content.PublishedOn = contentInfo.PublishedOn;
                //content.PublishedByUser = contentInfo.PublishedByUser;
                //content.ActivationDate = contentInfo.ActivationDate;
                //content.ExpirationDate = (!string.IsNullOrEmpty(contentInfo.ExpirationDate.ToString())) ? contentInfo.ExpirationDate : null;
                //content.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
                //content.UseCustomCss = contentInfo.UseCustomCss;
                //content.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
                //content.UseCustomJs = contentInfo.UseCustomJs;
                //content.Html = contentInfo.Html;
                //content.EditInSourceMode = contentInfo.EditInSourceMode;
                //content.OriginalText = (!string.IsNullOrEmpty(contentInfo.OriginalText.ToString())) ? contentInfo.OriginalText : null;
                //content.ContentTextMode = contentInfo.ContentTextMode;

                //JObject requestforpagecontent = new JObject();
                //requestforpagecontent.Add("contentId", content.Id);
                //string pagecontentobj = JsonConvert.SerializeObject(requestforpagecontent);
                //var pagecontentmodel = _webClient.DownloadData<JArray>("Blog/GetPageContentDetailswithContentId", new { Js = pagecontentobj });
                //Region region1 = new Region();
                //IList<PageContent> pagecontents = new List<PageContent>();
                //List<PageContent> pagecontentslist = new List<PageContent>();
                //for (int l = 0; l < pagecontentmodel.Count; l++)
                //{
                //    PageContent pagecontent = new PageContent();
                //    pagecontent.Id = new Guid(pagecontentmodel[l]["PageContentId"].ToString());
                //    pagecontent.Version = Convert.ToInt32(pagecontentmodel[l]["Version"]);
                //    pagecontent.IsDeleted = (bool)pagecontentmodel[l]["IsDeleted"];
                //    pagecontent.CreatedOn = (DateTime)pagecontentmodel[l]["CreatedOn"];
                //    pagecontent.CreatedByUser = pagecontentmodel[l]["CreatedByUser"].ToString();
                //    pagecontent.ModifiedByUser = pagecontentmodel[l]["ModifiedByUser"].ToString();
                //    pagecontent.ModifiedOn = (DateTime)pagecontentmodel[l]["ModifiedOn"];
                //    if (!string.IsNullOrEmpty(pagecontentmodel[l]["DeletedOn"].ToString()))
                //    {
                //        pagecontent.DeletedOn = (DateTime)pagecontentmodel[l]["DeletedOn"];
                //    }
                //    else
                //    {
                //        pagecontent.DeletedOn = null;
                //    }
                //    pagecontent.DeletedByUser = (!string.IsNullOrEmpty(pagecontentmodel[l]["DeletedByUser"].ToString())) ? pagecontentmodel[l]["DeletedByUser"].ToString() : null;
                //    pagecontent.Order = Convert.ToInt32(pagecontentmodel[l]["Order"]);
                //    pagecontent.Content = content;

                //    Guid contentpageId = new Guid(pagecontentmodel[l]["PageId"].ToString());
                //    //pagedetails-start
                //    JObject requestforpages = new JObject();
                //    requestforpages.Add("pageId", contentpageId);
                //    string pagesobj = JsonConvert.SerializeObject(requestforpages);
                //    var PagesModel = _webClient.DownloadData<string>("Blog/GetPagesForLayout", new { Js = pagesobj });
                //    dynamic PagesInfo = JObject.Parse(PagesModel);
                //    if (PagesInfo.Flag == 1)
                //    {
                //        BlogPost blogpostdetails = new BlogPost();
                //        blogpostdetails.Id = contentpageId;
                //        blogpostdetails.ActivationDate = PagesInfo.ActivationDate;
                //        blogpostdetails.ExpirationDate = (!string.IsNullOrEmpty(PagesInfo.ExpirationDate.ToString())) ? PagesInfo.ExpirationDate : null;
                //        blogpostdetails.Description = PagesInfo.Description;
                //        if (string.IsNullOrEmpty(PagesInfo.ImageId.ToString()))
                //        {
                //            blogpostdetails.Image = null;
                //        }
                //        else
                //        {
                //            MediaImage image = new MediaImage();
                //            image.Id = new Guid(PagesInfo.ImageId.ToString());
                //            JObject requestforimagedetails = new JObject();
                //            requestforimagedetails.Add("imageId", image.Id);
                //            string imageobj = JsonConvert.SerializeObject(requestforimagedetails);
                //            var ImageModel = _webClient.DownloadData<string>("Blog/GetImageDetails", new { Js = imageobj });
                //            dynamic ImageInfo = JObject.Parse(ImageModel);
                //            image.Caption = (!string.IsNullOrEmpty(ImageInfo.Caption.ToString())) ? ImageInfo.Caption : null;
                //            image.ImageAlign = (!string.IsNullOrEmpty(ImageInfo.ImageAlign.ToString())) ? ImageInfo.ImageAlign : null;
                //            image.Width = ImageInfo.Width;
                //            image.Height = ImageInfo.Height;
                //            image.CropCoordX1 = (!string.IsNullOrEmpty(ImageInfo.CropCoordX1.ToString())) ? ImageInfo.CropCoordX1 : null;
                //            image.CropCoordX2 = (!string.IsNullOrEmpty(ImageInfo.CropCoordX2.ToString())) ? ImageInfo.CropCoordX2 : null;
                //            image.CropCoordY1 = (!string.IsNullOrEmpty(ImageInfo.CropCoordY1.ToString())) ? ImageInfo.CropCoordY1 : null;
                //            image.CropCoordY2 = (!string.IsNullOrEmpty(ImageInfo.CropCoordY2.ToString())) ? ImageInfo.CropCoordY2 : null;
                //            image.OriginalWidth = ImageInfo.OriginalWidth;
                //            image.OriginalHeight = ImageInfo.OriginalHeight;
                //            image.OriginalSize = ImageInfo.OriginalSize;
                //            image.OriginalUri = ImageInfo.OriginalUri;
                //            image.PublicOriginallUrl = ImageInfo.PublicOriginallUrl;
                //            image.IsOriginalUploaded = (!string.IsNullOrEmpty(ImageInfo.IsOriginalUploaded.ToString())) ? ImageInfo.IsOriginalUploaded : null;
                //            image.ThumbnailWidth = ImageInfo.ThumbnailWidth;
                //            image.ThumbnailHeight = ImageInfo.ThumbnailHeight;
                //            image.ThumbnailSize = ImageInfo.ThumbnailSize;
                //            image.ThumbnailUri = ImageInfo.ThumbnailUri;
                //            image.PublicThumbnailUrl = ImageInfo.PublicThumbnailUrl;
                //            image.IsThumbnailUploaded = (!string.IsNullOrEmpty(ImageInfo.IsThumbnailUploaded.ToString())) ? ImageInfo.IsThumbnailUploaded : null;
                //            image.Version = ImageInfo.Version;
                //            image.IsDeleted = ImageInfo.IsDeleted;
                //            image.CreatedOn = ImageInfo.CreatedOn;
                //            image.CreatedByUser = ImageInfo.CreatedByUser;
                //            image.ModifiedOn = ImageInfo.ModifiedOn;
                //            image.ModifiedByUser = ImageInfo.ModifiedByUser;
                //            image.DeletedOn = (!string.IsNullOrEmpty(ImageInfo.DeletedOn.ToString())) ? ImageInfo.DeletedOn : null;
                //            image.DeletedByUser = (!string.IsNullOrEmpty(ImageInfo.DeletedByUser.ToString())) ? ImageInfo.DeletedByUser : null;
                //            if (!string.IsNullOrEmpty(ImageInfo.FolderId.ToString()))
                //            {
                //                MediaFolder folder = new MediaFolder();
                //                folder.Id = ImageInfo.FolderId;
                //                image.Folder = folder;
                //            }
                //            else
                //            {
                //                image.Folder = null;
                //            }
                //            image.Title = ImageInfo.Title;
                //            image.Type = ImageInfo.Type;
                //            image.ContentType = ImageInfo.ContentType;
                //            image.IsArchived = ImageInfo.IsArchived;
                //            if (!string.IsNullOrEmpty(ImageInfo.FolderId.ToString()))
                //            {
                //                image.Original.Id = ImageInfo.OriginalId;
                //            }
                //            else
                //            {
                //                image.Original = null;

                //            }

                //            image.PublishedOn = ImageInfo.PublishedOn;
                //            //image.Image.Id = ImageInfo.ImageId;
                //            image.Description = (!string.IsNullOrEmpty(ImageInfo.Description.ToString())) ? ImageInfo.Description : null;

                //            blogpostdetails.Image = image;
                //        }

                //        blogpostdetails.CustomCss = (!string.IsNullOrEmpty(PagesInfo.CustomCss.ToString())) ? PagesInfo.CustomCss : null;
                //        blogpostdetails.CustomJS = (!string.IsNullOrEmpty(PagesInfo.CustomJS.ToString())) ? PagesInfo.CustomJS : null;
                //        blogpostdetails.UseCanonicalUrl = PagesInfo.UseCanonicalUrl;
                //        blogpostdetails.UseNoFollow = PagesInfo.UseNoFollow;
                //        blogpostdetails.UseNoIndex = PagesInfo.UseNoIndex;

                //        if (string.IsNullOrEmpty(PagesInfo.SecondaryImageId.ToString()))
                //        {
                //            blogpostdetails.SecondaryImage = null;
                //        }
                //        else
                //        {
                //            MediaImage secondaryimage = new MediaImage();
                //            secondaryimage.Id = new Guid(PagesInfo.SecondaryImageId.ToString());
                //            blogpostdetails.SecondaryImage = secondaryimage;
                //        }
                //        if (string.IsNullOrEmpty(PagesInfo.FeaturedImageId.ToString()))
                //        {
                //            blogpostdetails.FeaturedImage = null;
                //        }
                //        else
                //        {
                //            MediaImage featuredimage = new MediaImage();
                //            featuredimage.Id = new Guid(PagesInfo.FeaturedImageId.ToString());
                //            blogpostdetails.FeaturedImage = featuredimage;
                //        }

                //        blogpostdetails.IsArchived = PagesInfo.IsArchived;
                //        blogpostdetails.Version = PagesInfo.Version;
                //        blogpostdetails.PageUrl = PagesInfo.PageUrl;
                //        blogpostdetails.Title = PagesInfo.Title;
                //        //if (!string.IsNullOrEmpty(PagesInfo.LayoutId.ToString()))
                //        //{
                //        //    blogpostdetails.Layout = PagesInfo.LayoutId;
                //        //}
                //        //else
                //        //{
                //        //    blogpostdetails.Layout = null;
                //        //}

                //        blogpostdetails.MetaTitle = (!string.IsNullOrEmpty(PagesInfo.MetaTitle.ToString())) ? PagesInfo.MetaTitle : null;
                //        blogpostdetails.MetaKeywords = (!string.IsNullOrEmpty(PagesInfo.MetaKeywords.ToString())) ? PagesInfo.MetaKeywords : null;
                //        blogpostdetails.MetaDescription = (!string.IsNullOrEmpty(PagesInfo.MetaDescription.ToString())) ? PagesInfo.MetaDescription : null;
                //        blogpostdetails.Status = PagesInfo.Status;
                //        blogpostdetails.PageUrlHash = PagesInfo.PageUrlHash;

                //        if (string.IsNullOrEmpty(PagesInfo.MasterPageId.ToString()))
                //        {
                //            blogpostdetails.MasterPage = null;
                //        }
                //        else
                //        {
                //            Page masterpage = new Page();
                //            masterpage.Id = new Guid(PagesInfo.MasterPageId.ToString());
                //            blogpostdetails.MasterPage = masterpage;
                //        }
                //        blogpostdetails.IsMasterPage = PagesInfo.IsMasterPage;
                //        if (string.IsNullOrEmpty(PagesInfo.LanguageId.ToString()))
                //        {
                //            blogpostdetails.Language = null;
                //        }
                //        else
                //        {
                //            Language language = new Language();
                //            language.Id = new Guid(PagesInfo.LanguageId.ToString());
                //            JObject requestforlanguage = new JObject();
                //            requestforlanguage.Add("languageId", language.Id);
                //            string languageobj = JsonConvert.SerializeObject(requestforlanguage);
                //            var LanguageModel = _webClient.DownloadData<string>("Blog/GetLanguageDetails", new { Js = languageobj });
                //            dynamic LanguageInfo = JObject.Parse(LanguageModel);
                //            language.Version = LanguageInfo.Version;
                //            language.IsDeleted = LanguageInfo.IsDeleted;
                //            language.CreatedOn = LanguageInfo.CreatedOn;
                //            language.CreatedByUser = LanguageInfo.CreatedByUser;
                //            language.ModifiedOn = LanguageInfo.ModifiedOn;
                //            language.ModifiedByUser = LanguageInfo.ModifiedByUser;
                //            language.DeletedOn = (!string.IsNullOrEmpty(LanguageInfo.DeletedOn.ToString())) ? LanguageInfo.DeletedOn : null;
                //            language.DeletedByUser = (!string.IsNullOrEmpty(LanguageInfo.DeletedByUser.ToString())) ? LanguageInfo.DeletedByUser : null;
                //            language.Name = LanguageInfo.Name;
                //            language.Code = LanguageInfo.Code;

                //            blogpostdetails.Language = language;
                //        }
                //        blogpostdetails.LanguageGroupIdentifier = (!string.IsNullOrEmpty(PagesInfo.LanguageGroupIdentifier.ToString())) ? PagesInfo.LanguageGroupIdentifier : null;
                //        blogpostdetails.ForceAccessProtocol = PagesInfo.ForceAccessProtocol;


                //        JObject requestforruleid1 = new JObject();
                //        requestforruleid1.Add("pageId", contentpageId);
                //        string ruleidobj1 = JsonConvert.SerializeObject(requestforruleid1);
                //        var RuleIdModel1 = _webClient.DownloadData<JArray>("Blog/GetBlogAccessRulesId", new { Js = ruleidobj1 });
                //        if (RuleIdModel1.Count > 0)
                //        {
                //            IList<AccessRule> accessrules = new List<AccessRule>();
                //            List<AccessRule> accessruleslist = new List<AccessRule>();
                //            for (int k = 0; k < RuleIdModel1.Count; k++)
                //            {
                //                Guid accessruleId = new Guid(RuleIdModel1[k]["AccessruleId"].ToString());
                //                JObject requestforaccessrule = new JObject();
                //                requestforaccessrule.Add("accessrulesId", accessruleId);
                //                string accessruleobj = JsonConvert.SerializeObject(requestforaccessrule);
                //                var accessruleModel = _webClient.DownloadData<string>("Blog/GetAccessRules", new { Js = accessruleobj });
                //                dynamic accessruleinfo = JObject.Parse(accessruleModel);
                //                AccessRule rule = new AccessRule();
                //                rule.Id = accessruleId;
                //                rule.Version = accessruleinfo.Version;
                //                rule.IsDeleted = accessruleinfo.IsDeleted;
                //                rule.CreatedOn = accessruleinfo.CreatedOn;
                //                rule.CreatedByUser = accessruleinfo.CreatedByUser;
                //                rule.ModifiedOn = accessruleinfo.ModifiedOn;
                //                rule.ModifiedByUser = accessruleinfo.ModifiedByUser;
                //                rule.DeletedOn = (!string.IsNullOrEmpty(accessruleinfo.DeletedOn.ToString())) ? accessruleinfo.DeletedOn : null;
                //                rule.DeletedByUser = (!string.IsNullOrEmpty(accessruleinfo.DeletedByUser.ToString())) ? accessruleinfo.DeletedByUser : null;
                //                rule.Identity = accessruleinfo.Identity;
                //                rule.AccessLevel = accessruleinfo.AccessLevel;
                //                rule.IsForRole = accessruleinfo.IsForRole;
                //                accessruleslist.Add(rule);
                //            }
                //            accessrules = accessruleslist;
                //            blogpostdetails.AccessRules = accessrules;


                //        }
                //        pagecontent.Page = blogpostdetails;
                //    }
                //    //pagedetails-end
                //    //regiondetails
                //    Guid contentregionId = new Guid(pagecontentmodel[l]["RegionId"].ToString());
                //    JObject requestforregiondetails = new JObject();
                //    requestforregiondetails.Add("regionId", contentregionId);
                //    string regionobj = JsonConvert.SerializeObject(requestforregiondetails);
                //    var regionmodel = _webClient.DownloadData<string>("Blog/GetRegionDetails", new { Js = regionobj });
                //    dynamic regionInfo = JObject.Parse(regionmodel);

                //    region1.Id = contentregionId;
                //    region1.Version = regionInfo.Version;
                //    region1.IsDeleted = regionInfo.IsDeleted;
                //    region1.CreatedOn = regionInfo.CreatedOn;
                //    region1.CreatedByUser = regionInfo.CreatedByUser;
                //    region1.ModifiedOn = regionInfo.ModifiedOn;
                //    region1.ModifiedByUser = regionInfo.ModifiedByUser;
                //    region1.DeletedOn = (!string.IsNullOrEmpty(regionInfo.DeletedOn.ToString())) ? regionInfo.DeletedOn : null;
                //    region1.DeletedByUser = (!string.IsNullOrEmpty(regionInfo.DeletedByUser.ToString())) ? regionInfo.DeletedByUser : null;
                //    region1.RegionIdentifier = regionInfo.RegionIdentifier;
                //    JObject requestforlayoutregiondetails = new JObject();
                //    requestforlayoutregiondetails.Add("regionId", contentregionId);
                //    string layoutregionobj = JsonConvert.SerializeObject(requestforlayoutregiondetails);
                //    var layoutregionmodel = _webClient.DownloadData<JArray>("Blog/GetLayoutRegionDetails", new { Js = layoutregionobj });

                //    IList<LayoutRegion> layoutregions = new List<LayoutRegion>();
                //    List<LayoutRegion> layoutregionlist = new List<LayoutRegion>();

                //    for (int i = 0; i < layoutregionmodel.Count; i++)
                //    {
                //        LayoutRegion layoutregion = new LayoutRegion();
                //        layoutregion.Id = new Guid(layoutregionmodel[i]["LayoutRegionId"].ToString());
                //        layoutregion.Version = Convert.ToInt32(layoutregionmodel[i]["Version"]);
                //        layoutregion.IsDeleted = (bool)layoutregionmodel[i]["IsDeleted"];
                //        layoutregion.CreatedOn = (DateTime)layoutregionmodel[i]["CreatedOn"];
                //        layoutregion.CreatedByUser = layoutregionmodel[i]["CreatedByUser"].ToString();
                //        layoutregion.ModifiedOn = (DateTime)layoutregionmodel[i]["ModifiedOn"];
                //        layoutregion.ModifiedByUser = layoutregionmodel[i]["ModifiedByUser"].ToString();
                //        if (!string.IsNullOrEmpty(layoutregionmodel[i]["DeletedOn"].ToString()))
                //        {

                //            layoutregion.DeletedOn = (DateTime)layoutregionmodel[i]["DeletedOn"];
                //        }
                //        else
                //        {
                //            layoutregion.DeletedOn = null;
                //        }
                //        layoutregion.DeletedByUser = (!string.IsNullOrEmpty(layoutregionmodel[i]["DeletedByUser"].ToString())) ? layoutregionmodel[i]["DeletedByUser"].ToString() : null;
                //        layoutregion.Description = (!string.IsNullOrEmpty(layoutregionmodel[i]["Description"].ToString())) ? layoutregionmodel[i]["Description"].ToString() : null;
                //        Layout layoutdetails = new Layout();
                //        layoutdetails.Id = new Guid(layoutregionmodel[i]["LayoutId"].ToString());
                //        JObject requestforlayoutdetails = new JObject();
                //        requestforlayoutdetails.Add("layoutId", layoutdetails.Id);
                //        string layoutobj = JsonConvert.SerializeObject(requestforlayoutdetails);
                //        var layoutmodel = _webClient.DownloadData<string>("Blog/GetLayoutDetails", new { Js = layoutobj });
                //        dynamic layoutInfo = JObject.Parse(layoutmodel);
                //        layoutdetails.Version = layoutInfo.Version;
                //        layoutdetails.IsDeleted = layoutInfo.IsDeleted;
                //        layoutdetails.CreatedOn = layoutInfo.CreatedOn;
                //        layoutdetails.CreatedByUser = layoutInfo.CreatedByUser;
                //        layoutdetails.ModifiedOn = layoutInfo.ModifiedOn;
                //        layoutdetails.ModifiedByUser = layoutInfo.ModifiedByUser;
                //        layoutdetails.DeletedOn = (!string.IsNullOrEmpty(layoutInfo.DeletedOn.ToString())) ? layoutInfo.DeletedOn : null;
                //        layoutdetails.DeletedByUser = (!string.IsNullOrEmpty(layoutInfo.DeletedByUser.ToString())) ? layoutInfo.DeletedByUser : null;
                //        layoutdetails.Name = layoutInfo.Name;
                //        layoutdetails.LayoutPath = layoutInfo.LayoutPath;
                //        layoutdetails.PreviewUrl = (!string.IsNullOrEmpty(layoutInfo.PreviewUrl.ToString())) ? layoutInfo.PreviewUrl : null;


                //        layoutregion.Layout = layoutdetails;

                //        layoutregion.Region = region1;
                //        layoutregionlist.Add(layoutregion);



                //    }

                //    layoutregions = layoutregionlist;
                //    region1.LayoutRegion = layoutregions;

                //    JObject requestforregionpagecontent = new JObject();
                //    requestforregionpagecontent.Add("regionId", contentregionId);
                //    string regionpagecontentobj = JsonConvert.SerializeObject(requestforregionpagecontent);
                //    var regionpagecontentmodel = _webClient.DownloadData<JArray>("Blog/GetRegionPageContentDetails", new { Js = regionpagecontentobj });

                //    IList<PageContent> regionpagecontents = new List<PageContent>();
                //    List<PageContent> regionpagecontentslist = new List<PageContent>();
                //    for (int i = 0; i < regionpagecontentmodel.Count; i++)
                //    {
                //        PageContent regionpagecontent = new PageContent();
                //        regionpagecontent.Id = new Guid(regionpagecontentmodel[i]["PageContentId"].ToString());
                //        regionpagecontent.Version = Convert.ToInt32(regionpagecontentmodel[i]["Version"]);
                //        regionpagecontent.IsDeleted = (bool)regionpagecontentmodel[i]["IsDeleted"];
                //        regionpagecontent.CreatedOn = (DateTime)regionpagecontentmodel[i]["CreatedOn"];
                //        regionpagecontent.CreatedByUser = regionpagecontentmodel[i]["CreatedByUser"].ToString();
                //        regionpagecontent.ModifiedByUser = regionpagecontentmodel[i]["ModifiedByUser"].ToString();
                //        regionpagecontent.ModifiedOn = (DateTime)regionpagecontentmodel[i]["ModifiedOn"];
                //        if (!string.IsNullOrEmpty(regionpagecontentmodel[i]["DeletedOn"].ToString()))
                //        {
                //            regionpagecontent.DeletedOn = (DateTime)regionpagecontentmodel[i]["DeletedOn"];
                //        }
                //        else
                //        {
                //            regionpagecontent.DeletedOn = null;
                //        }
                //        regionpagecontent.DeletedByUser = (!string.IsNullOrEmpty(regionpagecontentmodel[i]["DeletedByUser"].ToString())) ? regionpagecontentmodel[i]["DeletedByUser"].ToString() : null;
                //        regionpagecontent.Order = Convert.ToInt32(regionpagecontentmodel[i]["Order"]);

                //        Guid regionpageid = new Guid(regionpagecontentmodel[i]["PageId"].ToString());
                //        JObject requestdetails1 = new JObject();
                //        requestdetails1.Add("pageId", regionpageid);
                //        string jsobj1 = JsonConvert.SerializeObject(requestdetails1);
                //        var blogPostmodel = _webClient.DownloadData<string>("Blog/GetPagesForLayout", new { Js = jsobj1 });
                //        dynamic Info1 = JObject.Parse(blogPostmodel);

                //        if (Info1.Flag == 1)
                //        {
                //            BlogPost blogpostforlayout = new BlogPost();
                //            blogpostforlayout.Id = regionpageid;
                //            blogpostforlayout.ActivationDate = Info1.ActivationDate;
                //            blogpostforlayout.ExpirationDate = (!string.IsNullOrEmpty(Info1.ExpirationDate.ToString())) ? Info1.ExpirationDate : null;
                //            blogpostforlayout.Description = Info1.Description;
                //            if (string.IsNullOrEmpty(Info1.ImageId.ToString()))
                //            {
                //                blogpostforlayout.Image = null;
                //            }
                //            else
                //            {
                //                MediaImage image = new MediaImage();
                //                image.Id = new Guid(Info1.ImageId.ToString());
                //                blogpostforlayout.Image = image;
                //            }

                //            blogpostforlayout.CustomCss = (!string.IsNullOrEmpty(Info1.CustomCss.ToString())) ? Info1.CustomCss : null;
                //            blogpostforlayout.CustomJS = (!string.IsNullOrEmpty(Info1.CustomJS.ToString())) ? Info1.CustomJS : null;
                //            blogpostforlayout.UseCanonicalUrl = Info1.UseCanonicalUrl;
                //            blogpostforlayout.UseNoFollow = Info1.UseNoFollow;
                //            blogpostforlayout.UseNoIndex = Info1.UseNoIndex;

                //            if (string.IsNullOrEmpty(Info1.SecondaryImageId.ToString()))
                //            {
                //                blogpostforlayout.SecondaryImage = null;
                //            }
                //            else
                //            {
                //                MediaImage secondaryimage = new MediaImage();
                //                secondaryimage.Id = new Guid(Info1.SecondaryImageId.ToString());
                //                blogpostforlayout.SecondaryImage = secondaryimage;
                //            }
                //            if (string.IsNullOrEmpty(Info1.FeaturedImageId.ToString()))
                //            {
                //                blogpostforlayout.FeaturedImage = null;
                //            }
                //            else
                //            {
                //                MediaImage featuredimage = new MediaImage();
                //                featuredimage.Id = new Guid(Info1.FeaturedImageId.ToString());
                //                blogpostforlayout.FeaturedImage = featuredimage;
                //            }

                //            blogpostforlayout.IsArchived = Info1.IsArchived;
                //            blogpostforlayout.Version = Info1.Version;
                //            blogpostforlayout.PageUrl = Info1.PageUrl;
                //            blogpostforlayout.Title = Info1.Title;
                //            // blogpostforlayout.Layout = layoutdetails;
                //            blogpostforlayout.MetaTitle = (!string.IsNullOrEmpty(Info1.MetaTitle.ToString())) ? Info1.MetaTitle : null;
                //            blogpostforlayout.MetaKeywords = (!string.IsNullOrEmpty(Info1.MetaKeywords.ToString())) ? Info1.MetaKeywords : null;
                //            blogpostforlayout.MetaDescription = (!string.IsNullOrEmpty(Info1.MetaDescription.ToString())) ? Info1.MetaDescription : null;
                //            blogpostforlayout.Status = Info1.Status;
                //            blogpostforlayout.PageUrlHash = Info1.PageUrlHash;

                //            if (string.IsNullOrEmpty(Info1.MasterPageId.ToString()))
                //            {
                //                blogpostforlayout.MasterPage = null;
                //            }
                //            else
                //            {
                //                Page masterpage = new Page();
                //                masterpage.Id = new Guid(Info1.MasterPageId.ToString());
                //                blogpostforlayout.MasterPage = masterpage;
                //            }
                //            blogpostforlayout.IsMasterPage = Info1.IsMasterPage;
                //            if (string.IsNullOrEmpty(Info1.LanguageId.ToString()))
                //            {
                //                blogpostforlayout.Language = null;
                //            }
                //            else
                //            {
                //                Language language = new Language();
                //                language.Id = new Guid(Info1.LanguageId.ToString());
                //                blogpostforlayout.Language = language;
                //            }
                //            blogpostforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(Info1.LanguageGroupIdentifier.ToString())) ? Info1.LanguageGroupIdentifier : null;
                //            blogpostforlayout.ForceAccessProtocol = Info1.ForceAccessProtocol;

                //            regionpagecontent.Page = blogpostforlayout;

                //        }
                //        else if (Info1.Flag == 2)
                //        {
                //            PageProperties pagepropertiesforlayout = new PageProperties();
                //            pagepropertiesforlayout.Id = regionpageid;
                //            pagepropertiesforlayout.Description = Info1.Description;
                //            if (string.IsNullOrEmpty(Info1.ImageId.ToString()))
                //            {
                //                pagepropertiesforlayout.Image = null;
                //            }
                //            else
                //            {
                //                MediaImage image = new MediaImage();
                //                image.Id = new Guid(Info1.ImageId.ToString());
                //                pagepropertiesforlayout.Image = image;
                //            }

                //            pagepropertiesforlayout.CustomCss = (!string.IsNullOrEmpty(Info1.CustomCss.ToString())) ? Info1.CustomCss : null;
                //            pagepropertiesforlayout.CustomJS = (!string.IsNullOrEmpty(Info1.CustomJS.ToString())) ? Info1.CustomJS : null;
                //            pagepropertiesforlayout.UseCanonicalUrl = Info1.UseCanonicalUrl;
                //            pagepropertiesforlayout.UseNoFollow = Info1.UseNoFollow;
                //            pagepropertiesforlayout.UseNoIndex = Info1.UseNoIndex;

                //            if (string.IsNullOrEmpty(Info1.SecondaryImageId.ToString()))
                //            {
                //                pagepropertiesforlayout.SecondaryImage = null;
                //            }
                //            else
                //            {
                //                MediaImage secondaryimage = new MediaImage();
                //                secondaryimage.Id = new Guid(Info1.SecondaryImageId.ToString());
                //                pagepropertiesforlayout.SecondaryImage = secondaryimage;
                //            }
                //            if (string.IsNullOrEmpty(Info1.FeaturedImageId.ToString()))
                //            {
                //                pagepropertiesforlayout.FeaturedImage = null;
                //            }
                //            else
                //            {
                //                MediaImage featuredimage = new MediaImage();
                //                featuredimage.Id = new Guid(Info1.FeaturedImageId.ToString());
                //                pagepropertiesforlayout.FeaturedImage = featuredimage;
                //            }

                //            pagepropertiesforlayout.IsArchived = Info1.IsArchived;
                //            pagepropertiesforlayout.Version = Info1.Version;
                //            pagepropertiesforlayout.PageUrl = Info1.PageUrl;
                //            pagepropertiesforlayout.Title = Info1.Title;

                //            //pagepropertiesforlayout.Layout = layoutdetails;
                //            pagepropertiesforlayout.MetaTitle = (!string.IsNullOrEmpty(Info1.MetaTitle.ToString())) ? Info1.MetaTitle : null;
                //            pagepropertiesforlayout.MetaKeywords = (!string.IsNullOrEmpty(Info1.MetaKeywords.ToString())) ? Info1.MetaKeywords : null;
                //            pagepropertiesforlayout.MetaDescription = (!string.IsNullOrEmpty(Info1.MetaDescription.ToString())) ? Info1.MetaDescription : null;
                //            pagepropertiesforlayout.Status = Info1.Status;
                //            pagepropertiesforlayout.PageUrlHash = Info1.PageUrlHash;

                //            if (string.IsNullOrEmpty(Info1.MasterPageId.ToString()))
                //            {
                //                pagepropertiesforlayout.MasterPage = null;
                //            }
                //            else
                //            {
                //                Page masterpage = new Page();
                //                masterpage.Id = new Guid(Info1.MasterPageId.ToString());
                //                pagepropertiesforlayout.MasterPage = masterpage;
                //            }
                //            pagepropertiesforlayout.IsMasterPage = Info1.IsMasterPage;
                //            if (string.IsNullOrEmpty(Info1.LanguageId.ToString()))
                //            {
                //                pagepropertiesforlayout.Language = null;
                //            }
                //            else
                //            {
                //                Language language = new Language();
                //                language.Id = new Guid(Info1.LanguageId.ToString());
                //                pagepropertiesforlayout.Language = language;
                //            }
                //            pagepropertiesforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(Info1.LanguageGroupIdentifier.ToString())) ? Info1.LanguageGroupIdentifier : null;
                //            pagepropertiesforlayout.ForceAccessProtocol = Info1.ForceAccessProtocol;

                //            regionpagecontent.Page = pagepropertiesforlayout;

                //        }
                //        else if (Info1.Flag == 3)
                //        {
                //            Page pageforlayout = new Page();
                //            pageforlayout.Id = regionpageid;
                //            pageforlayout.Version = Info1.Version;
                //            pageforlayout.PageUrl = Info1.PageUrl;
                //            pageforlayout.Title = Info1.Title;

                //            //pageforlayout.Layout = layoutdetails;
                //            pageforlayout.MetaTitle = (!string.IsNullOrEmpty(Info1.MetaTitle.ToString())) ? Info1.MetaTitle : null;
                //            pageforlayout.MetaKeywords = (!string.IsNullOrEmpty(Info1.MetaKeywords.ToString())) ? Info1.MetaKeywords : null;
                //            pageforlayout.MetaDescription = (!string.IsNullOrEmpty(Info1.MetaDescription.ToString())) ? Info1.MetaDescription : null;
                //            pageforlayout.Status = Info1.Status;
                //            pageforlayout.PageUrlHash = Info1.PageUrlHash;

                //            if (string.IsNullOrEmpty(Info1.MasterPageId.ToString()))
                //            {
                //                pageforlayout.MasterPage = null;
                //            }
                //            else
                //            {
                //                Page masterpage = new Page();
                //                masterpage.Id = new Guid(Info1.MasterPageId.ToString());
                //                pageforlayout.MasterPage = masterpage;
                //            }
                //            pageforlayout.IsMasterPage = Info1.IsMasterPage;
                //            if (string.IsNullOrEmpty(Info1.LanguageId.ToString()))
                //            {
                //                pageforlayout.Language = null;
                //            }
                //            else
                //            {
                //                Language language = new Language();
                //                language.Id = new Guid(Info1.LanguageId.ToString());
                //                pageforlayout.Language = language;
                //            }
                //            pageforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(Info1.LanguageGroupIdentifier.ToString())) ? Info1.LanguageGroupIdentifier : null;
                //            pageforlayout.ForceAccessProtocol = Info1.ForceAccessProtocol;

                //            regionpagecontent.Page = pageforlayout;

                //        }



                //        Guid regioncontentid = new Guid(regionpagecontentmodel[i]["ContentId"].ToString());
                //        JObject requestforcontentdetails1 = new JObject();
                //        requestforcontentdetails1.Add("contentId", regioncontentid);
                //        string contentobj1 = JsonConvert.SerializeObject(requestforcontentdetails1);
                //        var contentmodel1 = _webClient.DownloadData<string>("Blog/GetContentDetailsForPageContents", new { Js = contentobj1 });
                //        dynamic contentInfo1 = JObject.Parse(contentmodel1);
                //        if (contentInfo.Flag == 1)
                //        {
                //            BlogPostContent blogpostcontent = new BlogPostContent();
                //            blogpostcontent.Id = contentInfo1.ContentId;
                //            blogpostcontent.ActivationDate = contentInfo1.ActivationDate;
                //            blogpostcontent.ExpirationDate = (!string.IsNullOrEmpty(contentInfo1.ExpirationDate.ToString())) ? contentInfo1.ExpirationDate : null;
                //            blogpostcontent.CustomCss = (!string.IsNullOrEmpty(contentInfo1.CustomCss.ToString())) ? contentInfo1.CustomCss : null;
                //            blogpostcontent.UseCustomCss = contentInfo1.UseCustomCss;
                //            blogpostcontent.CustomJs = (!string.IsNullOrEmpty(contentInfo1.CustomJs.ToString())) ? contentInfo1.CustomJs : null;
                //            blogpostcontent.UseCustomJs = contentInfo1.UseCustomJs;
                //            blogpostcontent.Html = contentInfo1.Html;
                //            blogpostcontent.EditInSourceMode = contentInfo1.EditInSourceMode;
                //            blogpostcontent.OriginalText = (!string.IsNullOrEmpty(contentInfo1.OriginalText.ToString())) ? contentInfo1.OriginalText : null;
                //            blogpostcontent.ContentTextMode = contentInfo1.ContentTextMode;
                //            blogpostcontent.Version = contentInfo1.Version;
                //            blogpostcontent.IsDeleted = contentInfo1.IsDeleted;
                //            blogpostcontent.CreatedOn = contentInfo1.CreatedOn;
                //            blogpostcontent.CreatedByUser = contentInfo1.CreatedByUser;
                //            blogpostcontent.ModifiedOn = contentInfo1.ModifiedOn;
                //            blogpostcontent.ModifiedByUser = contentInfo1.ModifiedByUser;
                //            blogpostcontent.DeletedOn = (!string.IsNullOrEmpty(contentInfo1.DeletedOn.ToString())) ? contentInfo1.DeletedOn : null;
                //            blogpostcontent.DeletedByUser = (!string.IsNullOrEmpty(contentInfo1.DeletedByUser.ToString())) ? contentInfo1.DeletedByUser : null;
                //            blogpostcontent.Name = contentInfo1.Name;
                //            blogpostcontent.PreviewUrl = (!string.IsNullOrEmpty(contentInfo1.PreviewUrl.ToString())) ? contentInfo1.PreviewUrl : null;
                //            blogpostcontent.Status = contentInfo1.Status;
                //            blogpostcontent.PublishedOn = (!string.IsNullOrEmpty(contentInfo1.PublishedOn.ToString())) ? contentInfo1.PublishedOn : null;
                //            blogpostcontent.PublishedByUser = (!string.IsNullOrEmpty(contentInfo1.PublishedByUser.ToString())) ? contentInfo1.PublishedByUser : null;
                //            if (!string.IsNullOrEmpty(contentInfo1.OriginalId.ToString()))
                //            {
                //                blogpostcontent.Original.Id = contentInfo1.OriginalId;
                //            }
                //            else
                //            {
                //                blogpostcontent.Original = null;

                //            }

                //            regionpagecontent.Content = blogpostcontent;

                //        }
                //        else if (contentInfo.Flag == 2)
                //        {
                //            HtmlContent htmlcontent = new HtmlContent();
                //            htmlcontent.Id = contentInfo1.ContentId;
                //            htmlcontent.ActivationDate = contentInfo1.ActivationDate;
                //            htmlcontent.ExpirationDate = (!string.IsNullOrEmpty(contentInfo1.ExpirationDate.ToString())) ? contentInfo1.ExpirationDate : null;
                //            htmlcontent.CustomCss = (!string.IsNullOrEmpty(contentInfo1.CustomCss.ToString())) ? contentInfo1.CustomCss : null;
                //            htmlcontent.UseCustomCss = contentInfo1.UseCustomCss;
                //            htmlcontent.CustomJs = (!string.IsNullOrEmpty(contentInfo1.CustomJs.ToString())) ? contentInfo1.CustomJs : null;
                //            htmlcontent.UseCustomJs = contentInfo1.UseCustomJs;
                //            htmlcontent.Html = contentInfo1.Html;
                //            htmlcontent.EditInSourceMode = contentInfo1.EditInSourceMode;
                //            htmlcontent.OriginalText = (!string.IsNullOrEmpty(contentInfo1.OriginalText.ToString())) ? contentInfo1.OriginalText : null;
                //            htmlcontent.ContentTextMode = contentInfo1.ContentTextMode;
                //            htmlcontent.Version = contentInfo1.Version;
                //            htmlcontent.IsDeleted = contentInfo1.IsDeleted;
                //            htmlcontent.CreatedOn = contentInfo1.CreatedOn;
                //            htmlcontent.CreatedByUser = contentInfo1.CreatedByUser;
                //            htmlcontent.ModifiedOn = contentInfo1.ModifiedOn;
                //            htmlcontent.ModifiedByUser = contentInfo1.ModifiedByUser;
                //            htmlcontent.DeletedOn = (!string.IsNullOrEmpty(contentInfo1.DeletedOn.ToString())) ? contentInfo1.DeletedOn : null;
                //            htmlcontent.DeletedByUser = (!string.IsNullOrEmpty(contentInfo1.DeletedByUser.ToString())) ? contentInfo1.DeletedByUser : null;
                //            htmlcontent.Name = contentInfo1.Name;
                //            htmlcontent.PreviewUrl = (!string.IsNullOrEmpty(contentInfo1.PreviewUrl.ToString())) ? contentInfo1.PreviewUrl : null;
                //            htmlcontent.Status = contentInfo1.Status;
                //            htmlcontent.PublishedOn = (!string.IsNullOrEmpty(contentInfo1.PublishedOn.ToString())) ? contentInfo1.PublishedOn : null;
                //            htmlcontent.PublishedByUser = (!string.IsNullOrEmpty(contentInfo1.PublishedByUser.ToString())) ? contentInfo1.PublishedByUser : null;
                //            if (!string.IsNullOrEmpty(contentInfo1.OriginalId.ToString()))
                //            {
                //                htmlcontent.Original.Id = contentInfo1.OriginalId;
                //            }
                //            else
                //            {
                //                htmlcontent.Original = null;

                //            }
                //            regionpagecontent.Content = htmlcontent;
                //        }


                //        else if (contentInfo.Flag == 3)
                //        {
                //            ServerControlWidget servercontrolwidget = new ServerControlWidget();
                //            servercontrolwidget.Id = contentInfo1.ContentId;
                //            servercontrolwidget.Url = contentInfo1.Url;
                //            servercontrolwidget.Version = contentInfo1.Version;
                //            servercontrolwidget.IsDeleted = contentInfo1.IsDeleted;
                //            servercontrolwidget.CreatedOn = contentInfo1.CreatedOn;
                //            servercontrolwidget.CreatedByUser = contentInfo1.CreatedByUser;
                //            servercontrolwidget.ModifiedOn = contentInfo1.ModifiedOn;
                //            servercontrolwidget.ModifiedByUser = contentInfo1.ModifiedByUser;
                //            servercontrolwidget.DeletedOn = (!string.IsNullOrEmpty(contentInfo1.DeletedOn.ToString())) ? contentInfo1.DeletedOn : null;
                //            servercontrolwidget.DeletedByUser = (!string.IsNullOrEmpty(contentInfo1.DeletedByUser.ToString())) ? contentInfo1.DeletedByUser : null;
                //            servercontrolwidget.Name = contentInfo1.Name;
                //            servercontrolwidget.PreviewUrl = (!string.IsNullOrEmpty(contentInfo1.PreviewUrl.ToString())) ? contentInfo1.PreviewUrl : null;
                //            servercontrolwidget.Status = contentInfo1.Status;
                //            servercontrolwidget.PublishedOn = (!string.IsNullOrEmpty(contentInfo1.PublishedOn.ToString())) ? contentInfo1.PublishedOn : null;
                //            servercontrolwidget.PublishedByUser = (!string.IsNullOrEmpty(contentInfo1.PublishedByUser.ToString())) ? contentInfo1.PublishedByUser : null;
                //            if (!string.IsNullOrEmpty(contentInfo1.OriginalId.ToString()))
                //            {
                //                servercontrolwidget.Original.Id = contentInfo1.OriginalId;
                //            }
                //            else
                //            {
                //                servercontrolwidget.Original = null;

                //            }
                //            regionpagecontent.Content = servercontrolwidget;
                //        }
                //        else if (contentInfo.Flag == 4)
                //        {
                //            HtmlContentWidget htmlcontentwidget = new HtmlContentWidget();
                //            htmlcontentwidget.Id = contentInfo1.ContentId;
                //            htmlcontentwidget.CustomCss = (!string.IsNullOrEmpty(contentInfo1.CustomCss.ToString())) ? contentInfo1.CustomCss : null;
                //            htmlcontentwidget.UseCustomCss = contentInfo1.UseCustomCss;
                //            htmlcontentwidget.CustomJs = (!string.IsNullOrEmpty(contentInfo1.CustomJs.ToString())) ? contentInfo1.CustomJs : null;
                //            htmlcontentwidget.UseCustomJs = contentInfo1.UseCustomJs;
                //            htmlcontentwidget.Html = contentInfo1.Html;
                //            htmlcontentwidget.UseHtml = contentInfo1.UseHtml;
                //            htmlcontentwidget.EditInSourceMode = contentInfo1.EditInSourceMode;
                //            htmlcontentwidget.Version = contentInfo1.Version;
                //            htmlcontentwidget.IsDeleted = contentInfo1.IsDeleted;
                //            htmlcontentwidget.CreatedOn = contentInfo1.CreatedOn;
                //            htmlcontentwidget.CreatedByUser = contentInfo1.CreatedByUser;
                //            htmlcontentwidget.ModifiedOn = contentInfo1.ModifiedOn;
                //            htmlcontentwidget.ModifiedByUser = contentInfo1.ModifiedByUser;
                //            htmlcontentwidget.DeletedOn = (!string.IsNullOrEmpty(contentInfo1.DeletedOn.ToString())) ? contentInfo1.DeletedOn : null;
                //            htmlcontentwidget.DeletedByUser = (!string.IsNullOrEmpty(contentInfo1.DeletedByUser.ToString())) ? contentInfo1.DeletedByUser : null;
                //            htmlcontentwidget.Name = contentInfo1.Name;
                //            htmlcontentwidget.PreviewUrl = (!string.IsNullOrEmpty(contentInfo1.PreviewUrl.ToString())) ? contentInfo1.PreviewUrl : null;
                //            htmlcontentwidget.Status = contentInfo1.Status;
                //            htmlcontentwidget.PublishedOn = (!string.IsNullOrEmpty(contentInfo1.PublishedOn.ToString())) ? contentInfo1.PublishedOn : null;
                //            htmlcontentwidget.PublishedByUser = (!string.IsNullOrEmpty(contentInfo1.PublishedByUser.ToString())) ? contentInfo1.PublishedByUser : null;
                //            if (!string.IsNullOrEmpty(contentInfo1.OriginalId.ToString()))
                //            {
                //                htmlcontentwidget.Original.Id = contentInfo1.OriginalId;
                //            }
                //            else
                //            {
                //                htmlcontentwidget.Original = null;

                //            }
                //            regionpagecontent.Content = htmlcontentwidget;
                //        }









                //        regionpagecontent.Region = region1;
                //        regionpagecontentslist.Add(regionpagecontent);




                //    }

                //    regionpagecontents = regionpagecontentslist;
                //    region1.PageContents = regionpagecontents;
                //    pagecontent.Region = region1;
                //    pagecontentslist.Add(pagecontent);

                //}
                //pagecontents = pagecontentslist;
                //content.PageContents = pagecontents;


                blogPost = blogPostFuture.FirstOne();

                if (configuration.Security.AccessControlEnabled)
                {
                    accessControlService.DemandAccess(blogPost, principal, AccessLevel.ReadWrite, roles);
                }

                if (content != null)
                {
                    // Check if user has confirmed the deletion of content
                    if (!request.IsUserConfirmed && blogPost.IsMasterPage)
                    {
                        contentService.CheckIfContentHasDeletingChildrenWithException(blogPost.Id, content.Id, request.Content);
                    }

                    var bpRef = blogPost;
                    var contentRef = content;

                    //JObject requestforpagecontent1 = new JObject();
                    //requestforpagecontent1.Add("pageId", bpRef.Id);
                    //requestforpagecontent1.Add("contentId", contentRef.Id);

                    //string pagecontentobj1 = JsonConvert.SerializeObject(requestforpagecontent1);
                    //var pagecontentmodel1 = _webClient.DownloadData<string>("Blog/GetPageContentDetails", new { Js = pagecontentobj1 });
                    //dynamic pagecontentInfo = JObject.Parse(pagecontentmodel1);
                    //pageContent = new PageContent();
                    //pageContent.Id = pagecontentInfo.PageContentId;
                    //pageContent.Version = pagecontentInfo.Version;
                    //pageContent.IsDeleted = pagecontentInfo.IsDeleted;
                    //pageContent.CreatedOn = pagecontentInfo.CreatedOn;
                    //pageContent.CreatedByUser = pagecontentInfo.CreatedByUser;
                    //pageContent.ModifiedByUser = pagecontentInfo.ModifiedByUser;
                    //pageContent.ModifiedOn = pagecontentInfo.ModifiedOn;
                    //pageContent.DeletedOn = (!string.IsNullOrEmpty(pagecontentInfo.DeletedOn.ToString())) ? pagecontentInfo.DeletedOn : null;
                    //pageContent.DeletedByUser = (!string.IsNullOrEmpty(pagecontentInfo.DeletedByUser.ToString())) ? pagecontentInfo.DeletedByUser : null;
                    //pageContent.Order = pagecontentInfo.Order;
                    //pageContent.Page = blogPost;
                    //pageContent.Content = content;


                    //Region pageContentRegion = new Region();
                    //pageContentRegion.Id = new Guid(pagecontentInfo.RegionId.ToString());

                    //JObject requestforregiondetails = new JObject();
                    //requestforregiondetails.Add("regionId", pageContentRegion.Id);
                    //string regionobj = JsonConvert.SerializeObject(requestforregiondetails);
                    //var regionmodel = _webClient.DownloadData<string>("Blog/GetRegionDetails", new { Js = regionobj });
                    //dynamic regionInfo = JObject.Parse(regionmodel);
                    //pageContentRegion.Version = regionInfo.Version;
                    //pageContentRegion.IsDeleted = regionInfo.IsDeleted;
                    //pageContentRegion.CreatedOn = regionInfo.CreatedOn;
                    //pageContentRegion.CreatedByUser = regionInfo.CreatedByUser;
                    //pageContentRegion.ModifiedOn = regionInfo.ModifiedOn;
                    //pageContentRegion.ModifiedByUser = regionInfo.ModifiedByUser;
                    //pageContentRegion.DeletedOn = (!string.IsNullOrEmpty(regionInfo.DeletedOn.ToString())) ? regionInfo.DeletedOn : null;
                    //pageContentRegion.DeletedByUser = (!string.IsNullOrEmpty(regionInfo.DeletedByUser.ToString())) ? regionInfo.DeletedByUser : null;
                    //pageContentRegion.RegionIdentifier = regionInfo.RegionIdentifier;

                    //JObject requestforlayoutregiondetails = new JObject();
                    //requestforlayoutregiondetails.Add("regionId", pageContentRegion.Id);
                    //string layoutregionobj = JsonConvert.SerializeObject(requestforlayoutregiondetails);
                    //var layoutregionmodel = _webClient.DownloadData<JArray>("Blog/GetLayoutRegionDetails", new { Js = layoutregionobj });

                    //IList<LayoutRegion> layoutregions = new List<LayoutRegion>();
                    //List<LayoutRegion> layoutregionlist = new List<LayoutRegion>();

                    //for (int i = 0; i < layoutregionmodel.Count; i++)
                    //{
                    //    LayoutRegion layoutregion = new LayoutRegion();
                    //    layoutregion.Id = new Guid(layoutregionmodel[i]["LayoutRegionId"].ToString());
                    //    layoutregion.Version = Convert.ToInt32(layoutregionmodel[i]["Version"]);
                    //    layoutregion.IsDeleted = (bool)layoutregionmodel[i]["IsDeleted"];
                    //    layoutregion.CreatedOn = (DateTime)layoutregionmodel[i]["CreatedOn"];
                    //    layoutregion.CreatedByUser = layoutregionmodel[i]["CreatedByUser"].ToString();
                    //    layoutregion.ModifiedOn = (DateTime)layoutregionmodel[i]["ModifiedOn"];
                    //    layoutregion.ModifiedByUser = layoutregionmodel[i]["ModifiedByUser"].ToString();
                    //    if (!string.IsNullOrEmpty(layoutregionmodel[i]["DeletedOn"].ToString()))
                    //    {

                    //        layoutregion.DeletedOn = (DateTime)layoutregionmodel[i]["DeletedOn"];
                    //    }
                    //    else
                    //    {
                    //        layoutregion.DeletedOn = null;
                    //    }
                    //    layoutregion.DeletedByUser = (!string.IsNullOrEmpty(layoutregionmodel[i]["DeletedByUser"].ToString())) ? layoutregionmodel[i]["DeletedByUser"].ToString() : null;
                    //    layoutregion.Description = (!string.IsNullOrEmpty(layoutregionmodel[i]["Description"].ToString())) ? layoutregionmodel[i]["Description"].ToString() : null;
                    //    Layout layoutdetails = new Layout();
                    //    layoutdetails.Id = new Guid(layoutregionmodel[i]["LayoutId"].ToString());
                    //    JObject requestforlayoutdetails = new JObject();
                    //    requestforlayoutdetails.Add("layoutId", layoutdetails.Id);
                    //    string layoutobj = JsonConvert.SerializeObject(requestforlayoutdetails);
                    //    var layoutmodel = _webClient.DownloadData<string>("Blog/GetLayoutDetails", new { Js = layoutobj });
                    //    dynamic layoutInfo = JObject.Parse(layoutmodel);
                    //    layoutdetails.Version = layoutInfo.Version;
                    //    layoutdetails.IsDeleted = layoutInfo.IsDeleted;
                    //    layoutdetails.CreatedOn = layoutInfo.CreatedOn;
                    //    layoutdetails.CreatedByUser = layoutInfo.CreatedByUser;
                    //    layoutdetails.ModifiedOn = layoutInfo.ModifiedOn;
                    //    layoutdetails.ModifiedByUser = layoutInfo.ModifiedByUser;
                    //    layoutdetails.DeletedOn = (!string.IsNullOrEmpty(layoutInfo.DeletedOn.ToString())) ? layoutInfo.DeletedOn : null;
                    //    layoutdetails.DeletedByUser = (!string.IsNullOrEmpty(layoutInfo.DeletedByUser.ToString())) ? layoutInfo.DeletedByUser : null;
                    //    layoutdetails.Name = layoutInfo.Name;
                    //    layoutdetails.LayoutPath = layoutInfo.LayoutPath;

                    //    //layoutdetails.Module.Id = layoutInfo.ModuleId;
                    //    layoutdetails.PreviewUrl = (!string.IsNullOrEmpty(layoutInfo.PreviewUrl.ToString())) ? layoutInfo.PreviewUrl : null;


                    //    layoutregion.Layout = layoutdetails;

                    //    layoutregionlist.Add(layoutregion);
                    //}

                    //layoutregions = layoutregionlist;
                    //pageContentRegion.LayoutRegion = layoutregions;
                    //pageContent.Region = pageContentRegion;
                    
                    
                    //pageContent.Region = region1;
                    pageContent = repository.FirstOrDefault<PageContent>(c => c.Page == bpRef && !c.IsDeleted && c.Content == contentRef);
                }
            }
            else
            {
                blogPost = new BlogPost();
            }

            if (pageContent == null)
            {
                pageContent = new PageContent { Page = blogPost };
            }
        }

        protected virtual void MapExtraProperties(bool isNew, BlogPost entity, BlogPostContent content, PageContent pageContent, BlogPostViewModel model, IPrincipal principal)
        {
            entity.Version = model.Version;
        }

        protected virtual void ValidateData(bool isNew, BlogPostViewModel model)
        {
            // Do nothing
        }

        /// <summary>
        /// Updates the status.
        /// </summary>
        /// <param name="blogPost">The blog post.</param>
        /// <param name="desirableStatus">The desirable status.</param>
        /// <exception cref="CmsException">If <c>desirableStatus</c> is not supported.</exception>
        private void UpdateStatus(BlogPost blogPost, ContentStatus desirableStatus)
        {
            switch (desirableStatus)
            {
                case ContentStatus.Published:
                    if (blogPost.Status != PageStatus.Published)
                    {
                        blogPost.PublishedOn = DateTime.Now;
                    }
                    blogPost.Status = PageStatus.Published;
                    break;
                case ContentStatus.Draft:
                    blogPost.Status = PageStatus.Unpublished;
                    break;
                case ContentStatus.Preview:
                    blogPost.Status = PageStatus.Preview;
                    break;
                default:
                    throw new CmsException(string.Format("Blog post does not support status: {0}.", desirableStatus));
            }
        }

        private void LoadDefaultLayoutAndRegion(out Layout layout, out Page masterPage, out Region region)
        {
            var option = blogOptionService.GetDefaultOption();

            layout = option != null ? option.DefaultLayout : null;
            masterPage = option != null ? option.DefaultMasterPage : null;
            if (layout == null && masterPage == null)
            {
                layout = GetFirstCompatibleLayout();
            }

            if (layout == null && masterPage == null)
            {
                var message = BlogGlobalization.SaveBlogPost_LayoutNotFound_Message;
                const string logMessage = "No compatible layouts found for blog post.";
                throw new ValidationException(() => message, logMessage);
            }

            Guid regionId;
            if (layout != null)
            {
                var regionIdentifier = RegionIdentifier.ToLowerInvariant();
                regionId = layout.LayoutRegions.Count(layoutRegion => !layoutRegion.IsDeleted && !layoutRegion.Region.IsDeleted) == 1
                                   ? layout.LayoutRegions.First(layoutRegion => !layoutRegion.IsDeleted).Region.Id
                                   : layout.LayoutRegions.Where(
                                       layoutRegion =>
                                       !layoutRegion.IsDeleted && !layoutRegion.Region.IsDeleted && layoutRegion.Region.RegionIdentifier.ToLowerInvariant() == regionIdentifier)
                                           .Select(layoutRegion => layoutRegion.Region.Id)
                                           .FirstOrDefault();
            }
            else
            {
                var masterPageRef = masterPage;
                var results = repository.AsQueryable<PageContent>()
                          .Where(pageContent => pageContent.Page == masterPageRef)
                          .SelectMany(pageContent => pageContent.Content.ContentRegions)
                          .Select(contentRegion => new { contentRegion.Region.Id, contentRegion.Region.RegionIdentifier })
                          .ToList();
                var mainContent = results.FirstOrDefault(r => r.RegionIdentifier.ToLowerInvariant() == RegionIdentifier.ToLowerInvariant());
                if (mainContent == null)
                {
                    mainContent = results.FirstOrDefault();
                }

                if (mainContent != null)
                {
                    regionId = mainContent.Id;
                }
                else
                {
                    regionId = Guid.Empty;
                }
            }

            if (regionId.HasDefaultValue())
            {
                var message = string.Format(BlogGlobalization.SaveBlogPost_RegionNotFound_Message, RegionIdentifier);
                var logMessage = string.Format("Region {0} for rendering blog post content not found.", RegionIdentifier);
                throw new ValidationException(() => message, logMessage);
            }

            region = repository.AsProxy<Region>(regionId);
            //JObject requestforregiondetails = new JObject();
            //requestforregiondetails.Add("regionId", regionId);
            //string regionobj = JsonConvert.SerializeObject(requestforregiondetails);
            //var regionmodel = _webClient.DownloadData<string>("Blog/GetRegionDetails", new { Js = regionobj });
            //dynamic regionInfo = JObject.Parse(regionmodel);
            //Region region1 = new Region();
            //region1.Id = regionId;
            //region1.Version = regionInfo.Version;
            //region1.IsDeleted = regionInfo.IsDeleted;
            //region1.CreatedOn = regionInfo.CreatedOn;
            //region1.CreatedByUser = regionInfo.CreatedByUser;
            //region1.ModifiedOn = regionInfo.ModifiedOn;
            //region1.ModifiedByUser = regionInfo.ModifiedByUser;
            //region1.DeletedOn = (!string.IsNullOrEmpty(regionInfo.DeletedOn.ToString())) ? regionInfo.DeletedOn : null;
            //region1.DeletedByUser = (!string.IsNullOrEmpty(regionInfo.DeletedByUser.ToString())) ? regionInfo.DeletedByUser : null;
            //region1.RegionIdentifier = regionInfo.RegionIdentifier;
            //JObject requestforlayoutregiondetails = new JObject();
            //requestforlayoutregiondetails.Add("regionId", regionId);
            //string layoutregionobj = JsonConvert.SerializeObject(requestforlayoutregiondetails);
            //var layoutregionmodel = _webClient.DownloadData<JArray>("Blog/GetLayoutRegionDetails", new { Js = layoutregionobj });

            //IList<LayoutRegion> layoutregions = new List<LayoutRegion>();
            //List<LayoutRegion> layoutregionlist = new List<LayoutRegion>();

            //for (int i = 0; i < layoutregionmodel.Count; i++)
            //{
            //    LayoutRegion layoutregion = new LayoutRegion();
            //    layoutregion.Id = new Guid(layoutregionmodel[i]["LayoutRegionId"].ToString());
            //    layoutregion.Version = Convert.ToInt32(layoutregionmodel[i]["Version"]);
            //    layoutregion.IsDeleted = (bool)layoutregionmodel[i]["IsDeleted"];
            //    layoutregion.CreatedOn = (DateTime)layoutregionmodel[i]["CreatedOn"];
            //    layoutregion.CreatedByUser = layoutregionmodel[i]["CreatedByUser"].ToString();
            //    layoutregion.ModifiedOn = (DateTime)layoutregionmodel[i]["ModifiedOn"];
            //    layoutregion.ModifiedByUser = layoutregionmodel[i]["ModifiedByUser"].ToString();
            //    if (!string.IsNullOrEmpty(layoutregionmodel[i]["DeletedOn"].ToString()))
            //    {

            //        layoutregion.DeletedOn = (DateTime)layoutregionmodel[i]["DeletedOn"];
            //    }
            //    else
            //    {
            //        layoutregion.DeletedOn = null;
            //    }
            //    layoutregion.DeletedByUser = (!string.IsNullOrEmpty(layoutregionmodel[i]["DeletedByUser"].ToString())) ? layoutregionmodel[i]["DeletedByUser"].ToString() : null;
            //    layoutregion.Description = (!string.IsNullOrEmpty(layoutregionmodel[i]["Description"].ToString())) ? layoutregionmodel[i]["Description"].ToString() : null;
            //    Layout layoutdetails = new Layout();
            //    layoutdetails.Id = new Guid(layoutregionmodel[i]["LayoutId"].ToString());
            //    JObject requestforlayoutdetails = new JObject();
            //    requestforlayoutdetails.Add("layoutId", layoutdetails.Id);
            //    string layoutobj = JsonConvert.SerializeObject(requestforlayoutdetails);
            //    var layoutmodel = _webClient.DownloadData<string>("Blog/GetLayoutDetails", new { Js = layoutobj });
            //    dynamic layoutInfo = JObject.Parse(layoutmodel);
            //    layoutdetails.Version = layoutInfo.Version;
            //    layoutdetails.IsDeleted = layoutInfo.IsDeleted;
            //    layoutdetails.CreatedOn = layoutInfo.CreatedOn;
            //    layoutdetails.CreatedByUser = layoutInfo.CreatedByUser;
            //    layoutdetails.ModifiedOn = layoutInfo.ModifiedOn;
            //    layoutdetails.ModifiedByUser = layoutInfo.ModifiedByUser;
            //    layoutdetails.DeletedOn = (!string.IsNullOrEmpty(layoutInfo.DeletedOn.ToString())) ? layoutInfo.DeletedOn : null;
            //    layoutdetails.DeletedByUser = (!string.IsNullOrEmpty(layoutInfo.DeletedByUser.ToString())) ? layoutInfo.DeletedByUser : null;
            //    layoutdetails.Name = layoutInfo.Name;
            //    layoutdetails.LayoutPath = layoutInfo.LayoutPath;


            //    layoutdetails.PreviewUrl = (!string.IsNullOrEmpty(layoutInfo.PreviewUrl.ToString())) ? layoutInfo.PreviewUrl : null;

            //    JObject requestforlayoutregiondetails1 = new JObject();
            //    requestforlayoutregiondetails1.Add("layoutId", layoutdetails.Id);
            //    string layoutregionobj1 = JsonConvert.SerializeObject(requestforlayoutregiondetails1);
            //    var layoutregionmodel1 = _webClient.DownloadData<JArray>("Blog/GetLayoutRegionDetailsForLayout", new { Js = layoutregionobj1 });

            //    IList<LayoutRegion> layoutregions1 = new List<LayoutRegion>();
            //    List<LayoutRegion> layoutregionlist1 = new List<LayoutRegion>();

            //    for (int k = 0; k < layoutregionmodel1.Count; k++)
            //    {
            //        LayoutRegion layoutregion1 = new LayoutRegion();
            //        layoutregion1.Id = new Guid(layoutregionmodel1[k]["LayoutRegionId"].ToString());
            //        layoutregion1.Version = Convert.ToInt32(layoutregionmodel1[k]["Version"]);
            //        layoutregion1.IsDeleted = (bool)layoutregionmodel1[k]["IsDeleted"];
            //        layoutregion1.CreatedOn = (DateTime)layoutregionmodel1[k]["CreatedOn"];
            //        layoutregion1.CreatedByUser = layoutregionmodel1[k]["CreatedByUser"].ToString();
            //        layoutregion1.ModifiedOn = (DateTime)layoutregionmodel1[k]["ModifiedOn"];
            //        layoutregion1.ModifiedByUser = layoutregionmodel1[k]["ModifiedByUser"].ToString();
            //        if (!string.IsNullOrEmpty(layoutregionmodel1[k]["DeletedOn"].ToString()))
            //        {

            //            layoutregion1.DeletedOn = (DateTime)layoutregionmodel1[k]["DeletedOn"];
            //        }
            //        else
            //        {
            //            layoutregion1.DeletedOn = null;
            //        }
            //        layoutregion1.DeletedByUser = (!string.IsNullOrEmpty(layoutregionmodel1[k]["DeletedByUser"].ToString())) ? layoutregionmodel1[k]["DeletedByUser"].ToString() : null;
            //        layoutregion1.Description = (!string.IsNullOrEmpty(layoutregionmodel1[k]["Description"].ToString())) ? layoutregionmodel1[k]["Description"].ToString() : null;
            //        Region region2 = new Region();
            //        region2.Id = new Guid(layoutregionmodel1[k]["RegionId"].ToString());
            //        JObject requestforregiondetails1 = new JObject();
            //        requestforregiondetails1.Add("regionId", region2.Id);
            //        string regionobj1 = JsonConvert.SerializeObject(requestforregiondetails1);
            //        var regionmodel1 = _webClient.DownloadData<string>("Blog/GetRegionDetails", new { Js = regionobj1 });
            //        dynamic regionInfo1 = JObject.Parse(regionmodel1);
            //        //Region region1 = new Region();                
            //        region2.Version = regionInfo1.Version;
            //        region2.IsDeleted = regionInfo1.IsDeleted;
            //        region2.CreatedOn = regionInfo1.CreatedOn;
            //        region2.CreatedByUser = regionInfo1.CreatedByUser;
            //        region2.ModifiedOn = regionInfo1.ModifiedOn;
            //        region2.ModifiedByUser = regionInfo1.ModifiedByUser;
            //        region2.DeletedOn = (!string.IsNullOrEmpty(regionInfo1.DeletedOn.ToString())) ? regionInfo1.DeletedOn : null;
            //        region2.DeletedByUser = (!string.IsNullOrEmpty(regionInfo1.DeletedByUser.ToString())) ? regionInfo1.DeletedByUser : null;
            //        region2.RegionIdentifier = regionInfo1.RegionIdentifier;
            //        layoutregion1.Layout = layoutdetails;
            //        layoutregion1.Region = region2;
            //        layoutregionlist1.Add(layoutregion1);

            //    }
            //    layoutregions1 = layoutregionlist1;
            //    layoutdetails.LayoutRegions = layoutregions1;


            //    JObject requestforpageids = new JObject();
            //    requestforpageids.Add("layoutId", layoutdetails.Id);
            //    string pageidsobj = JsonConvert.SerializeObject(requestforpageids);
            //    var PageIdsModel = _webClient.DownloadData<JArray>("Blog/GetPageIds", new { Js = pageidsobj });
            //    IList<Page> pages = new List<Page>();
            //    List<Page> pageslist = new List<Page>();
            //    for (int k = 0; k < PageIdsModel.Count; k++)
            //    {
            //        Guid pageId = new Guid(PageIdsModel[k]["PageId"].ToString());
            //        JObject requestforpages = new JObject();
            //        requestforpages.Add("pageId", pageId);
            //        string pagesobj = JsonConvert.SerializeObject(requestforpages);
            //        var PagesModel = _webClient.DownloadData<string>("Blog/GetPagesForLayout", new { Js = pagesobj });
            //        dynamic PagesInfo = JObject.Parse(PagesModel);
            //        if (PagesInfo.Flag == 1)
            //        {
            //            BlogPost blogpostforlayout = new BlogPost();
            //            blogpostforlayout.Id = pageId;
            //            blogpostforlayout.ActivationDate = PagesInfo.ActivationDate;
            //            blogpostforlayout.ExpirationDate = (!string.IsNullOrEmpty(PagesInfo.ExpirationDate.ToString())) ? PagesInfo.ExpirationDate : null;
            //            blogpostforlayout.Description = PagesInfo.Description;
            //            if (string.IsNullOrEmpty(PagesInfo.ImageId.ToString()))
            //            {
            //                blogpostforlayout.Image = null;
            //            }
            //            else
            //            {
            //                MediaImage image = new MediaImage();
            //                image.Id = new Guid(PagesInfo.ImageId.ToString());
            //                JObject requestforimagedetails = new JObject();
            //                requestforimagedetails.Add("imageId", image.Id);
            //                string imageobj = JsonConvert.SerializeObject(requestforimagedetails);
            //                var ImageModel = _webClient.DownloadData<string>("Blog/GetImageDetails", new { Js = imageobj });
            //                dynamic ImageInfo = JObject.Parse(ImageModel);
            //                image.Caption = (!string.IsNullOrEmpty(ImageInfo.Caption.ToString())) ? ImageInfo.Caption : null;
            //                image.ImageAlign = (!string.IsNullOrEmpty(ImageInfo.ImageAlign.ToString())) ? ImageInfo.ImageAlign : null;
            //                image.Width = ImageInfo.Width;
            //                image.Height = ImageInfo.Height;
            //                image.CropCoordX1 = (!string.IsNullOrEmpty(ImageInfo.CropCoordX1.ToString())) ? ImageInfo.CropCoordX1 : null;
            //                image.CropCoordX2 = (!string.IsNullOrEmpty(ImageInfo.CropCoordX2.ToString())) ? ImageInfo.CropCoordX2 : null;
            //                image.CropCoordY1 = (!string.IsNullOrEmpty(ImageInfo.CropCoordY1.ToString())) ? ImageInfo.CropCoordY1 : null;
            //                image.CropCoordY2 = (!string.IsNullOrEmpty(ImageInfo.CropCoordY2.ToString())) ? ImageInfo.CropCoordY2 : null;
            //                image.OriginalWidth = ImageInfo.OriginalWidth;
            //                image.OriginalHeight = ImageInfo.OriginalHeight;
            //                image.OriginalSize = ImageInfo.OriginalSize;
            //                image.OriginalUri = ImageInfo.OriginalUri;
            //                image.PublicOriginallUrl = ImageInfo.PublicOriginallUrl;
            //                image.IsOriginalUploaded = (!string.IsNullOrEmpty(ImageInfo.IsOriginalUploaded.ToString())) ? ImageInfo.IsOriginalUploaded : null;
            //                image.ThumbnailWidth = ImageInfo.ThumbnailWidth;
            //                image.ThumbnailHeight = ImageInfo.ThumbnailHeight;
            //                image.ThumbnailSize = ImageInfo.ThumbnailSize;
            //                image.ThumbnailUri = ImageInfo.ThumbnailUri;
            //                image.PublicThumbnailUrl = ImageInfo.PublicThumbnailUrl;
            //                image.IsThumbnailUploaded = (!string.IsNullOrEmpty(ImageInfo.IsThumbnailUploaded.ToString())) ? ImageInfo.IsThumbnailUploaded : null;
            //                image.Version = ImageInfo.Version;
            //                image.IsDeleted = ImageInfo.IsDeleted;
            //                image.CreatedOn = ImageInfo.CreatedOn;
            //                image.CreatedByUser = ImageInfo.CreatedByUser;
            //                image.ModifiedOn = ImageInfo.ModifiedOn;
            //                image.ModifiedByUser = ImageInfo.ModifiedByUser;
            //                image.DeletedOn = (!string.IsNullOrEmpty(ImageInfo.DeletedOn.ToString())) ? ImageInfo.DeletedOn : null;
            //                image.DeletedByUser = (!string.IsNullOrEmpty(ImageInfo.DeletedByUser.ToString())) ? ImageInfo.DeletedByUser : null;
            //                if (!string.IsNullOrEmpty(ImageInfo.FolderId.ToString()))
            //                {
            //                    MediaFolder folder = new MediaFolder();
            //                    folder.Id = ImageInfo.FolderId;
            //                    image.Folder = folder;
            //                }
            //                else
            //                {
            //                    image.Folder = null;
            //                }
            //                image.Title = ImageInfo.Title;
            //                image.Type = ImageInfo.Type;
            //                image.ContentType = ImageInfo.ContentType;
            //                image.IsArchived = ImageInfo.IsArchived;
            //                if (!string.IsNullOrEmpty(ImageInfo.FolderId.ToString()))
            //                {
            //                    image.Original.Id = ImageInfo.OriginalId;
            //                }
            //                else
            //                {
            //                    image.Original = null;

            //                }

            //                image.PublishedOn = ImageInfo.PublishedOn;
            //                //image.Image.Id = ImageInfo.ImageId;
            //                image.Description = (!string.IsNullOrEmpty(ImageInfo.Description.ToString())) ? ImageInfo.Description : null;

            //                blogpostforlayout.Image = image;
            //            }

            //            blogpostforlayout.CustomCss = (!string.IsNullOrEmpty(PagesInfo.CustomCss.ToString())) ? PagesInfo.CustomCss : null;
            //            blogpostforlayout.CustomJS = (!string.IsNullOrEmpty(PagesInfo.CustomJS.ToString())) ? PagesInfo.CustomJS : null;
            //            blogpostforlayout.UseCanonicalUrl = PagesInfo.UseCanonicalUrl;
            //            blogpostforlayout.UseNoFollow = PagesInfo.UseNoFollow;
            //            blogpostforlayout.UseNoIndex = PagesInfo.UseNoIndex;

            //            if (string.IsNullOrEmpty(PagesInfo.SecondaryImageId.ToString()))
            //            {
            //                blogpostforlayout.SecondaryImage = null;
            //            }
            //            else
            //            {
            //                MediaImage secondaryimage = new MediaImage();
            //                secondaryimage.Id = new Guid(PagesInfo.SecondaryImageId.ToString());
            //                blogpostforlayout.SecondaryImage = secondaryimage;
            //            }
            //            if (string.IsNullOrEmpty(PagesInfo.FeaturedImageId.ToString()))
            //            {
            //                blogpostforlayout.FeaturedImage = null;
            //            }
            //            else
            //            {
            //                MediaImage featuredimage = new MediaImage();
            //                featuredimage.Id = new Guid(PagesInfo.FeaturedImageId.ToString());
            //                blogpostforlayout.FeaturedImage = featuredimage;
            //            }

            //            blogpostforlayout.IsArchived = PagesInfo.IsArchived;
            //            blogpostforlayout.Version = PagesInfo.Version;
            //            blogpostforlayout.PageUrl = PagesInfo.PageUrl;
            //            blogpostforlayout.Title = PagesInfo.Title;

            //            blogpostforlayout.Layout = layoutdetails;
            //            blogpostforlayout.MetaTitle = (!string.IsNullOrEmpty(PagesInfo.MetaTitle.ToString())) ? PagesInfo.MetaTitle : null;
            //            blogpostforlayout.MetaKeywords = (!string.IsNullOrEmpty(PagesInfo.MetaKeywords.ToString())) ? PagesInfo.MetaKeywords : null;
            //            blogpostforlayout.MetaDescription = (!string.IsNullOrEmpty(PagesInfo.MetaDescription.ToString())) ? PagesInfo.MetaDescription : null;
            //            blogpostforlayout.Status = PagesInfo.Status;
            //            blogpostforlayout.PageUrlHash = PagesInfo.PageUrlHash;

            //            if (string.IsNullOrEmpty(PagesInfo.MasterPageId.ToString()))
            //            {
            //                blogpostforlayout.MasterPage = null;
            //            }
            //            else
            //            {
            //                Page masterpage = new Page();
            //                masterpage.Id = new Guid(PagesInfo.MasterPageId.ToString());
            //                blogpostforlayout.MasterPage = masterpage;
            //            }
            //            blogpostforlayout.IsMasterPage = PagesInfo.IsMasterPage;
            //            if (string.IsNullOrEmpty(PagesInfo.LanguageId.ToString()))
            //            {
            //                blogpostforlayout.Language = null;
            //            }
            //            else
            //            {
            //                Language language = new Language();
            //                language.Id = new Guid(PagesInfo.LanguageId.ToString());
            //                JObject requestforlanguage = new JObject();
            //                requestforlanguage.Add("languageId", language.Id);
            //                string languageobj = JsonConvert.SerializeObject(requestforlanguage);
            //                var LanguageModel = _webClient.DownloadData<string>("Blog/GetLanguageDetails", new { Js = languageobj });
            //                dynamic LanguageInfo = JObject.Parse(LanguageModel);
            //                language.Version = LanguageInfo.Version;
            //                language.IsDeleted = LanguageInfo.IsDeleted;
            //                language.CreatedOn = LanguageInfo.CreatedOn;
            //                language.CreatedByUser = LanguageInfo.CreatedByUser;
            //                language.ModifiedOn = LanguageInfo.ModifiedOn;
            //                language.ModifiedByUser = LanguageInfo.ModifiedByUser;
            //                language.DeletedOn = (!string.IsNullOrEmpty(LanguageInfo.DeletedOn.ToString())) ? LanguageInfo.DeletedOn : null;
            //                language.DeletedByUser = (!string.IsNullOrEmpty(LanguageInfo.DeletedByUser.ToString())) ? LanguageInfo.DeletedByUser : null;
            //                language.Name = LanguageInfo.Name;
            //                language.Code = LanguageInfo.Code;

            //                blogpostforlayout.Language = language;
            //            }
            //            blogpostforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(PagesInfo.LanguageGroupIdentifier.ToString())) ? PagesInfo.LanguageGroupIdentifier : null;
            //            blogpostforlayout.ForceAccessProtocol = PagesInfo.ForceAccessProtocol;


            //            JObject requestforruleid = new JObject();
            //            requestforruleid.Add("pageId", pageId);
            //            string ruleidobj = JsonConvert.SerializeObject(requestforruleid);
            //            var RuleIdModel = _webClient.DownloadData<JArray>("Blog/GetBlogAccessRulesId", new { Js = ruleidobj });
            //            if (RuleIdModel.Count > 0)
            //            {
            //                IList<AccessRule> accessrules = new List<AccessRule>();
            //                List<AccessRule> accessruleslist = new List<AccessRule>();
            //                for (int l = 0; l < RuleIdModel.Count; l++)
            //                {
            //                    Guid accessruleId = new Guid(RuleIdModel[l]["AccessruleId"].ToString());
            //                    JObject requestforaccessrule = new JObject();
            //                    requestforaccessrule.Add("accessrulesId", accessruleId);
            //                    string accessruleobj = JsonConvert.SerializeObject(requestforaccessrule);
            //                    var accessruleModel = _webClient.DownloadData<string>("Blog/GetAccessRules", new { Js = accessruleobj });
            //                    dynamic accessruleinfo = JObject.Parse(accessruleModel);
            //                    AccessRule rule = new AccessRule();
            //                    rule.Id = accessruleId;
            //                    rule.Version = accessruleinfo.Version;
            //                    rule.IsDeleted = accessruleinfo.IsDeleted;
            //                    rule.CreatedOn = accessruleinfo.CreatedOn;
            //                    rule.CreatedByUser = accessruleinfo.CreatedByUser;
            //                    rule.ModifiedOn = accessruleinfo.ModifiedOn;
            //                    rule.ModifiedByUser = accessruleinfo.ModifiedByUser;
            //                    rule.DeletedOn = (!string.IsNullOrEmpty(accessruleinfo.DeletedOn.ToString())) ? accessruleinfo.DeletedOn : null;
            //                    rule.DeletedByUser = (!string.IsNullOrEmpty(accessruleinfo.DeletedByUser.ToString())) ? accessruleinfo.DeletedByUser : null;
            //                    rule.Identity = accessruleinfo.Identity;
            //                    rule.AccessLevel = accessruleinfo.AccessLevel;
            //                    rule.IsForRole = accessruleinfo.IsForRole;
            //                    accessruleslist.Add(rule);
            //                }
            //                accessrules = accessruleslist;
            //                blogpostforlayout.AccessRules = accessrules;


            //            }

            //            JObject requestforpagecontent = new JObject();
            //            requestforpagecontent.Add("pageId", pageId);
            //            string pagecontentobj = JsonConvert.SerializeObject(requestforpagecontent);
            //            var pagecontentmodel = _webClient.DownloadData<JArray>("Blog/GetPageContentDetailsWithPageId", new { Js = pagecontentobj });

            //            IList<PageContent> pagecontents = new List<PageContent>();
            //            List<PageContent> pagecontentslist = new List<PageContent>();
            //            for (int l = 0; l < pagecontentmodel.Count; l++)
            //            {
            //                PageContent pagecontent = new PageContent();
            //                pagecontent.Id = new Guid(pagecontentmodel[l]["PageContentId"].ToString());
            //                pagecontent.Version = Convert.ToInt32(pagecontentmodel[l]["Version"]);
            //                pagecontent.IsDeleted = (bool)pagecontentmodel[l]["IsDeleted"];
            //                pagecontent.CreatedOn = (DateTime)pagecontentmodel[l]["CreatedOn"];
            //                pagecontent.CreatedByUser = pagecontentmodel[l]["CreatedByUser"].ToString();
            //                pagecontent.ModifiedByUser = pagecontentmodel[l]["ModifiedByUser"].ToString();
            //                pagecontent.ModifiedOn = (DateTime)pagecontentmodel[l]["ModifiedOn"];
            //                if (!string.IsNullOrEmpty(pagecontentmodel[l]["DeletedOn"].ToString()))
            //                {
            //                    pagecontent.DeletedOn = (DateTime)pagecontentmodel[l]["DeletedOn"];
            //                }
            //                else
            //                {
            //                    pagecontent.DeletedOn = null;
            //                }
            //                pagecontent.DeletedByUser = (!string.IsNullOrEmpty(pagecontentmodel[l]["DeletedByUser"].ToString())) ? pagecontentmodel[l]["DeletedByUser"].ToString() : null;
            //                pagecontent.Order = Convert.ToInt32(pagecontentmodel[l]["Order"]);
            //                //contentdetails


            //                JObject requestforcontentdetails = new JObject();
            //                requestforcontentdetails.Add("contentId", pagecontentmodel[l]["ContentId"].ToString());


            //                string contentobj = JsonConvert.SerializeObject(requestforcontentdetails);
            //                var contentmodel = _webClient.DownloadData<string>("Blog/GetContentDetailsForPageContents", new { Js = contentobj });
            //                dynamic contentInfo = JObject.Parse(contentmodel);
            //                if (contentInfo.Flag == 1)
            //                {
            //                    BlogPostContent blogpostcontent = new BlogPostContent();
            //                    blogpostcontent.Id = contentInfo.ContentId;
            //                    blogpostcontent.ActivationDate = contentInfo.ActivationDate;
            //                    blogpostcontent.ExpirationDate = (!string.IsNullOrEmpty(contentInfo.ExpirationDate.ToString())) ? contentInfo.ExpirationDate : null;
            //                    blogpostcontent.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //                    blogpostcontent.UseCustomCss = contentInfo.UseCustomCss;
            //                    blogpostcontent.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //                    blogpostcontent.UseCustomJs = contentInfo.UseCustomJs;
            //                    blogpostcontent.Html = contentInfo.Html;
            //                    blogpostcontent.EditInSourceMode = contentInfo.EditInSourceMode;
            //                    blogpostcontent.OriginalText = (!string.IsNullOrEmpty(contentInfo.OriginalText.ToString())) ? contentInfo.OriginalText : null;
            //                    blogpostcontent.ContentTextMode = contentInfo.ContentTextMode;
            //                    blogpostcontent.Version = contentInfo.Version;
            //                    blogpostcontent.IsDeleted = contentInfo.IsDeleted;
            //                    blogpostcontent.CreatedOn = contentInfo.CreatedOn;
            //                    blogpostcontent.CreatedByUser = contentInfo.CreatedByUser;
            //                    blogpostcontent.ModifiedOn = contentInfo.ModifiedOn;
            //                    blogpostcontent.ModifiedByUser = contentInfo.ModifiedByUser;
            //                    blogpostcontent.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //                    blogpostcontent.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //                    blogpostcontent.Name = contentInfo.Name;
            //                    blogpostcontent.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //                    blogpostcontent.Status = contentInfo.Status;
            //                    blogpostcontent.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //                    blogpostcontent.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //                    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //                    {
            //                        blogpostcontent.Original.Id = contentInfo.OriginalId;
            //                    }
            //                    else
            //                    {
            //                        blogpostcontent.Original = null;

            //                    }

            //                    pagecontent.Content = blogpostcontent;

            //                }
            //                else if (contentInfo.Flag == 2)
            //                {
            //                    HtmlContent htmlcontent = new HtmlContent();
            //                    htmlcontent.Id = contentInfo.ContentId;
            //                    htmlcontent.ActivationDate = contentInfo.ActivationDate;
            //                    htmlcontent.ExpirationDate = (!string.IsNullOrEmpty(contentInfo.ExpirationDate.ToString())) ? contentInfo.ExpirationDate : null;
            //                    htmlcontent.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //                    htmlcontent.UseCustomCss = contentInfo.UseCustomCss;
            //                    htmlcontent.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //                    htmlcontent.UseCustomJs = contentInfo.UseCustomJs;
            //                    htmlcontent.Html = contentInfo.Html;
            //                    htmlcontent.EditInSourceMode = contentInfo.EditInSourceMode;
            //                    htmlcontent.OriginalText = (!string.IsNullOrEmpty(contentInfo.OriginalText.ToString())) ? contentInfo.OriginalText : null;
            //                    htmlcontent.ContentTextMode = contentInfo.ContentTextMode;
            //                    htmlcontent.Version = contentInfo.Version;
            //                    htmlcontent.IsDeleted = contentInfo.IsDeleted;
            //                    htmlcontent.CreatedOn = contentInfo.CreatedOn;
            //                    htmlcontent.CreatedByUser = contentInfo.CreatedByUser;
            //                    htmlcontent.ModifiedOn = contentInfo.ModifiedOn;
            //                    htmlcontent.ModifiedByUser = contentInfo.ModifiedByUser;
            //                    htmlcontent.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //                    htmlcontent.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //                    htmlcontent.Name = contentInfo.Name;
            //                    htmlcontent.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //                    htmlcontent.Status = contentInfo.Status;
            //                    htmlcontent.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //                    htmlcontent.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //                    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //                    {
            //                        htmlcontent.Original.Id = contentInfo.OriginalId;
            //                    }
            //                    else
            //                    {
            //                        htmlcontent.Original = null;

            //                    }
            //                    pagecontent.Content = htmlcontent;
            //                }


            //                else if (contentInfo.Flag == 3)
            //                {
            //                    ServerControlWidget servercontrolwidget = new ServerControlWidget();
            //                    servercontrolwidget.Id = contentInfo.ContentId;
            //                    servercontrolwidget.Url = contentInfo.Url;
            //                    servercontrolwidget.Version = contentInfo.Version;
            //                    servercontrolwidget.IsDeleted = contentInfo.IsDeleted;
            //                    servercontrolwidget.CreatedOn = contentInfo.CreatedOn;
            //                    servercontrolwidget.CreatedByUser = contentInfo.CreatedByUser;
            //                    servercontrolwidget.ModifiedOn = contentInfo.ModifiedOn;
            //                    servercontrolwidget.ModifiedByUser = contentInfo.ModifiedByUser;
            //                    servercontrolwidget.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //                    servercontrolwidget.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //                    servercontrolwidget.Name = contentInfo.Name;
            //                    servercontrolwidget.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //                    servercontrolwidget.Status = contentInfo.Status;
            //                    servercontrolwidget.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //                    servercontrolwidget.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //                    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //                    {
            //                        servercontrolwidget.Original.Id = contentInfo.OriginalId;
            //                    }
            //                    else
            //                    {
            //                        servercontrolwidget.Original = null;

            //                    }
            //                    pagecontent.Content = servercontrolwidget;
            //                }
            //                else if (contentInfo.Flag == 4)
            //                {
            //                    HtmlContentWidget htmlcontentwidget = new HtmlContentWidget();
            //                    htmlcontentwidget.Id = contentInfo.ContentId;
            //                    htmlcontentwidget.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //                    htmlcontentwidget.UseCustomCss = contentInfo.UseCustomCss;
            //                    htmlcontentwidget.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //                    htmlcontentwidget.UseCustomJs = contentInfo.UseCustomJs;
            //                    htmlcontentwidget.Html = contentInfo.Html;
            //                    htmlcontentwidget.UseHtml = contentInfo.UseHtml;
            //                    htmlcontentwidget.EditInSourceMode = contentInfo.EditInSourceMode;
            //                    htmlcontentwidget.Version = contentInfo.Version;
            //                    htmlcontentwidget.IsDeleted = contentInfo.IsDeleted;
            //                    htmlcontentwidget.CreatedOn = contentInfo.CreatedOn;
            //                    htmlcontentwidget.CreatedByUser = contentInfo.CreatedByUser;
            //                    htmlcontentwidget.ModifiedOn = contentInfo.ModifiedOn;
            //                    htmlcontentwidget.ModifiedByUser = contentInfo.ModifiedByUser;
            //                    htmlcontentwidget.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //                    htmlcontentwidget.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //                    htmlcontentwidget.Name = contentInfo.Name;
            //                    htmlcontentwidget.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //                    htmlcontentwidget.Status = contentInfo.Status;
            //                    htmlcontentwidget.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //                    htmlcontentwidget.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //                    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //                    {
            //                        htmlcontentwidget.Original.Id = contentInfo.OriginalId;
            //                    }
            //                    else
            //                    {
            //                        htmlcontentwidget.Original = null;

            //                    }
            //                    pagecontent.Content = htmlcontentwidget;
            //                }
            //                //pagedetails
            //                pagecontent.Page = blogpostforlayout;

            //                pagecontentslist.Add(pagecontent);


            //            }
            //            pagecontents = pagecontentslist;
            //            blogpostforlayout.PageContents = pagecontents;


            //            pageslist.Add(blogpostforlayout);
            //        }


            //        else if (PagesInfo.Flag == 2)
            //        {
            //            PageProperties pagepropertiesforlayout = new PageProperties();
            //            pagepropertiesforlayout.Id = pageId;
            //            pagepropertiesforlayout.Description = PagesInfo.Description;
            //            if (string.IsNullOrEmpty(PagesInfo.ImageId.ToString()))
            //            {
            //                pagepropertiesforlayout.Image = null;
            //            }
            //            else
            //            {
            //                MediaImage image = new MediaImage();
            //                image.Id = new Guid(PagesInfo.ImageId.ToString());
            //                JObject requestforimagedetails = new JObject();
            //                requestforimagedetails.Add("imageId", image.Id);
            //                string imageobj = JsonConvert.SerializeObject(requestforimagedetails);
            //                var ImageModel = _webClient.DownloadData<string>("Blog/GetImageDetails", new { Js = imageobj });
            //                dynamic ImageInfo = JObject.Parse(ImageModel);
            //                image.Caption = (!string.IsNullOrEmpty(ImageInfo.Caption.ToString())) ? ImageInfo.Caption : null;
            //                image.ImageAlign = (!string.IsNullOrEmpty(ImageInfo.ImageAlign.ToString())) ? ImageInfo.ImageAlign : null;
            //                image.Width = ImageInfo.Width;
            //                image.Height = ImageInfo.Height;
            //                image.CropCoordX1 = (!string.IsNullOrEmpty(ImageInfo.CropCoordX1.ToString())) ? ImageInfo.CropCoordX1 : null;
            //                image.CropCoordX2 = (!string.IsNullOrEmpty(ImageInfo.CropCoordX2.ToString())) ? ImageInfo.CropCoordX2 : null;
            //                image.CropCoordY1 = (!string.IsNullOrEmpty(ImageInfo.CropCoordY1.ToString())) ? ImageInfo.CropCoordY1 : null;
            //                image.CropCoordY2 = (!string.IsNullOrEmpty(ImageInfo.CropCoordY2.ToString())) ? ImageInfo.CropCoordY2 : null;
            //                image.OriginalWidth = ImageInfo.OriginalWidth;
            //                image.OriginalHeight = ImageInfo.OriginalHeight;
            //                image.OriginalSize = ImageInfo.OriginalSize;
            //                image.OriginalUri = ImageInfo.OriginalUri;
            //                image.PublicOriginallUrl = ImageInfo.PublicOriginallUrl;
            //                image.IsOriginalUploaded = (!string.IsNullOrEmpty(ImageInfo.IsOriginalUploaded.ToString())) ? ImageInfo.IsOriginalUploaded : null;
            //                image.ThumbnailWidth = ImageInfo.ThumbnailWidth;
            //                image.ThumbnailHeight = ImageInfo.ThumbnailHeight;
            //                image.ThumbnailSize = ImageInfo.ThumbnailSize;
            //                image.ThumbnailUri = ImageInfo.ThumbnailUri;
            //                image.PublicThumbnailUrl = ImageInfo.PublicThumbnailUrl;
            //                image.IsThumbnailUploaded = (!string.IsNullOrEmpty(ImageInfo.IsThumbnailUploaded.ToString())) ? ImageInfo.IsThumbnailUploaded : null;
            //                image.Version = ImageInfo.Version;
            //                image.IsDeleted = ImageInfo.IsDeleted;
            //                image.CreatedOn = ImageInfo.CreatedOn;
            //                image.CreatedByUser = ImageInfo.CreatedByUser;
            //                image.ModifiedOn = ImageInfo.ModifiedOn;
            //                image.ModifiedByUser = ImageInfo.ModifiedByUser;
            //                image.DeletedOn = (!string.IsNullOrEmpty(ImageInfo.DeletedOn.ToString())) ? ImageInfo.DeletedOn : null;
            //                image.DeletedByUser = (!string.IsNullOrEmpty(ImageInfo.DeletedByUser.ToString())) ? ImageInfo.DeletedByUser : null;
            //                if (!string.IsNullOrEmpty(ImageInfo.FolderId.ToString()))
            //                {
            //                    MediaFolder folder = new MediaFolder();
            //                    folder.Id = ImageInfo.FolderId;
            //                    image.Folder = folder;
            //                }
            //                else
            //                {
            //                    image.Folder = null;
            //                }
            //                image.Title = ImageInfo.Title;
            //                image.Type = ImageInfo.Type;
            //                image.ContentType = ImageInfo.ContentType;
            //                image.IsArchived = ImageInfo.IsArchived;
            //                if (!string.IsNullOrEmpty(ImageInfo.OriginalId.ToString()))
            //                {
            //                    image.Original.Id = ImageInfo.OriginalId;
            //                }
            //                else
            //                {
            //                    image.Original = null;

            //                }

            //                image.PublishedOn = ImageInfo.PublishedOn;
            //                //image.Image.Id = ImageInfo.ImageId;
            //                image.Description = (!string.IsNullOrEmpty(ImageInfo.Description.ToString())) ? ImageInfo.Description : null;
            //                pagepropertiesforlayout.Image = image;
            //            }

            //            pagepropertiesforlayout.CustomCss = (!string.IsNullOrEmpty(PagesInfo.CustomCss.ToString())) ? PagesInfo.CustomCss : null;
            //            pagepropertiesforlayout.CustomJS = (!string.IsNullOrEmpty(PagesInfo.CustomJS.ToString())) ? PagesInfo.CustomJS : null;
            //            pagepropertiesforlayout.UseCanonicalUrl = PagesInfo.UseCanonicalUrl;
            //            pagepropertiesforlayout.UseNoFollow = PagesInfo.UseNoFollow;
            //            pagepropertiesforlayout.UseNoIndex = PagesInfo.UseNoIndex;

            //            if (string.IsNullOrEmpty(PagesInfo.SecondaryImageId.ToString()))
            //            {
            //                pagepropertiesforlayout.SecondaryImage = null;
            //            }
            //            else
            //            {
            //                MediaImage secondaryimage = new MediaImage();
            //                secondaryimage.Id = new Guid(PagesInfo.SecondaryImageId.ToString());
            //                pagepropertiesforlayout.SecondaryImage = secondaryimage;
            //            }
            //            if (string.IsNullOrEmpty(PagesInfo.FeaturedImageId.ToString()))
            //            {
            //                pagepropertiesforlayout.FeaturedImage = null;
            //            }
            //            else
            //            {
            //                MediaImage featuredimage = new MediaImage();
            //                featuredimage.Id = new Guid(PagesInfo.FeaturedImageId.ToString());
            //                pagepropertiesforlayout.FeaturedImage = featuredimage;
            //            }

            //            pagepropertiesforlayout.IsArchived = PagesInfo.IsArchived;
            //            pagepropertiesforlayout.Version = PagesInfo.Version;
            //            pagepropertiesforlayout.PageUrl = PagesInfo.PageUrl;
            //            pagepropertiesforlayout.Title = PagesInfo.Title;

            //            pagepropertiesforlayout.Layout = layoutdetails;
            //            pagepropertiesforlayout.MetaTitle = (!string.IsNullOrEmpty(PagesInfo.MetaTitle.ToString())) ? PagesInfo.MetaTitle : null;
            //            pagepropertiesforlayout.MetaKeywords = (!string.IsNullOrEmpty(PagesInfo.MetaKeywords.ToString())) ? PagesInfo.MetaKeywords : null;
            //            pagepropertiesforlayout.MetaDescription = (!string.IsNullOrEmpty(PagesInfo.MetaDescription.ToString())) ? PagesInfo.MetaDescription : null;
            //            pagepropertiesforlayout.Status = PagesInfo.Status;
            //            pagepropertiesforlayout.PageUrlHash = PagesInfo.PageUrlHash;

            //            if (string.IsNullOrEmpty(PagesInfo.MasterPageId.ToString()))
            //            {
            //                pagepropertiesforlayout.MasterPage = null;
            //            }
            //            else
            //            {
            //                Page masterpage = new Page();
            //                masterpage.Id = new Guid(PagesInfo.MasterPageId.ToString());
            //                pagepropertiesforlayout.MasterPage = masterpage;
            //            }
            //            pagepropertiesforlayout.IsMasterPage = PagesInfo.IsMasterPage;
            //            if (string.IsNullOrEmpty(PagesInfo.LanguageId.ToString()))
            //            {
            //                pagepropertiesforlayout.Language = null;
            //            }
            //            else
            //            {
            //                Language language = new Language();
            //                language.Id = new Guid(PagesInfo.LanguageId.ToString());
            //                JObject requestforlanguage = new JObject();
            //                requestforlanguage.Add("languageId", language.Id);
            //                string languageobj = JsonConvert.SerializeObject(requestforlanguage);
            //                var LanguageModel = _webClient.DownloadData<string>("Blog/GetLanguageDetails", new { Js = languageobj });
            //                dynamic LanguageInfo = JObject.Parse(LanguageModel);
            //                language.Version = LanguageInfo.Version;
            //                language.IsDeleted = LanguageInfo.IsDeleted;
            //                language.CreatedOn = LanguageInfo.CreatedOn;
            //                language.CreatedByUser = LanguageInfo.CreatedByUser;
            //                language.ModifiedOn = LanguageInfo.ModifiedOn;
            //                language.ModifiedByUser = LanguageInfo.ModifiedByUser;
            //                language.DeletedOn = (!string.IsNullOrEmpty(LanguageInfo.DeletedOn.ToString())) ? LanguageInfo.DeletedOn : null;
            //                language.DeletedByUser = (!string.IsNullOrEmpty(LanguageInfo.DeletedByUser.ToString())) ? LanguageInfo.DeletedByUser : null;
            //                language.Name = LanguageInfo.Name;
            //                language.Code = LanguageInfo.Code;
            //                pagepropertiesforlayout.Language = language;
            //            }
            //            pagepropertiesforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(PagesInfo.LanguageGroupIdentifier.ToString())) ? PagesInfo.LanguageGroupIdentifier : null;
            //            pagepropertiesforlayout.ForceAccessProtocol = PagesInfo.ForceAccessProtocol;

            //            JObject requestforruleid = new JObject();
            //            requestforruleid.Add("pageId", pageId);
            //            string ruleidobj = JsonConvert.SerializeObject(requestforruleid);
            //            var RuleIdModel = _webClient.DownloadData<JArray>("Blog/GetBlogAccessRulesId", new { Js = ruleidobj });
            //            if (RuleIdModel.Count > 0)
            //            {
            //                IList<AccessRule> accessrules = new List<AccessRule>();
            //                List<AccessRule> accessruleslist = new List<AccessRule>();
            //                for (int l = 0; l < RuleIdModel.Count; l++)
            //                {
            //                    Guid accessruleId = new Guid(RuleIdModel[l]["AccessruleId"].ToString());
            //                    JObject requestforaccessrule = new JObject();
            //                    requestforaccessrule.Add("accessrulesId", accessruleId);
            //                    string accessruleobj = JsonConvert.SerializeObject(requestforaccessrule);
            //                    var accessruleModel = _webClient.DownloadData<string>("Blog/GetAccessRules", new { Js = accessruleobj });
            //                    dynamic accessruleinfo = JObject.Parse(accessruleModel);
            //                    AccessRule rule = new AccessRule();
            //                    rule.Id = accessruleId;
            //                    rule.Version = accessruleinfo.Version;
            //                    rule.IsDeleted = accessruleinfo.IsDeleted;
            //                    rule.CreatedOn = accessruleinfo.CreatedOn;
            //                    rule.CreatedByUser = accessruleinfo.CreatedByUser;
            //                    rule.ModifiedOn = accessruleinfo.ModifiedOn;
            //                    rule.ModifiedByUser = accessruleinfo.ModifiedByUser;
            //                    rule.DeletedOn = (!string.IsNullOrEmpty(accessruleinfo.DeletedOn.ToString())) ? accessruleinfo.DeletedOn : null;
            //                    rule.DeletedByUser = (!string.IsNullOrEmpty(accessruleinfo.DeletedByUser.ToString())) ? accessruleinfo.DeletedByUser : null;
            //                    rule.Identity = accessruleinfo.Identity;
            //                    rule.AccessLevel = accessruleinfo.AccessLevel;
            //                    rule.IsForRole = accessruleinfo.IsForRole;
            //                    accessruleslist.Add(rule);
            //                }
            //                accessrules = accessruleslist;
            //                pagepropertiesforlayout.AccessRules = accessrules;


            //            }

            //            JObject requestforpagecontent = new JObject();
            //            requestforpagecontent.Add("pageId", pageId);
            //            string pagecontentobj = JsonConvert.SerializeObject(requestforpagecontent);
            //            var pagecontentmodel = _webClient.DownloadData<JArray>("Blog/GetPageContentDetailsWithPageId", new { Js = pagecontentobj });

            //            IList<PageContent> pagecontents = new List<PageContent>();
            //            List<PageContent> pagecontentslist = new List<PageContent>();
            //            for (int l = 0; l < pagecontentmodel.Count; l++)
            //            {
            //                PageContent pagecontent = new PageContent();
            //                pagecontent.Id = new Guid(pagecontentmodel[l]["PageContentId"].ToString());
            //                pagecontent.Version = Convert.ToInt32(pagecontentmodel[l]["Version"]);
            //                pagecontent.IsDeleted = (bool)pagecontentmodel[l]["IsDeleted"];
            //                pagecontent.CreatedOn = (DateTime)pagecontentmodel[l]["CreatedOn"];
            //                pagecontent.CreatedByUser = pagecontentmodel[l]["CreatedByUser"].ToString();
            //                pagecontent.ModifiedByUser = pagecontentmodel[l]["ModifiedByUser"].ToString();
            //                pagecontent.ModifiedOn = (DateTime)pagecontentmodel[l]["ModifiedOn"];
            //                if (!string.IsNullOrEmpty(pagecontentmodel[l]["DeletedOn"].ToString()))
            //                {
            //                    pagecontent.DeletedOn = (DateTime)pagecontentmodel[l]["DeletedOn"];
            //                }
            //                else
            //                {
            //                    pagecontent.DeletedOn = null;
            //                }
            //                pagecontent.DeletedByUser = (!string.IsNullOrEmpty(pagecontentmodel[l]["DeletedByUser"].ToString())) ? pagecontentmodel[l]["DeletedByUser"].ToString() : null;
            //                pagecontent.Order = Convert.ToInt32(pagecontentmodel[l]["Order"]);
            //                //contentdetails


            //                JObject requestforcontentdetails = new JObject();
            //                requestforcontentdetails.Add("contentId", pagecontentmodel[l]["ContentId"].ToString());


            //                string contentobj = JsonConvert.SerializeObject(requestforcontentdetails);
            //                var contentmodel = _webClient.DownloadData<string>("Blog/GetContentDetailsForPageContents", new { Js = contentobj });

            //                dynamic contentInfo = JObject.Parse(contentmodel);
            //                if (contentInfo.Flag == 1)
            //                {
            //                    BlogPostContent blogpostcontent = new BlogPostContent();
            //                    blogpostcontent.Id = contentInfo.ContentId;
            //                    blogpostcontent.ActivationDate = contentInfo.ActivationDate;
            //                    blogpostcontent.ExpirationDate = (!string.IsNullOrEmpty(contentInfo.ExpirationDate.ToString())) ? contentInfo.ExpirationDate : null;
            //                    blogpostcontent.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //                    blogpostcontent.UseCustomCss = contentInfo.UseCustomCss;
            //                    blogpostcontent.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //                    blogpostcontent.UseCustomJs = contentInfo.UseCustomJs;
            //                    blogpostcontent.Html = contentInfo.Html;
            //                    blogpostcontent.EditInSourceMode = contentInfo.EditInSourceMode;
            //                    blogpostcontent.OriginalText = (!string.IsNullOrEmpty(contentInfo.OriginalText.ToString())) ? contentInfo.OriginalText : null;
            //                    blogpostcontent.ContentTextMode = contentInfo.ContentTextMode;
            //                    blogpostcontent.Version = contentInfo.Version;
            //                    blogpostcontent.IsDeleted = contentInfo.IsDeleted;
            //                    blogpostcontent.CreatedOn = contentInfo.CreatedOn;
            //                    blogpostcontent.CreatedByUser = contentInfo.CreatedByUser;
            //                    blogpostcontent.ModifiedOn = contentInfo.ModifiedOn;
            //                    blogpostcontent.ModifiedByUser = contentInfo.ModifiedByUser;
            //                    blogpostcontent.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //                    blogpostcontent.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //                    blogpostcontent.Name = contentInfo.Name;
            //                    blogpostcontent.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //                    blogpostcontent.Status = contentInfo.Status;
            //                    blogpostcontent.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //                    blogpostcontent.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //                    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //                    {
            //                        blogpostcontent.Original.Id = contentInfo.OriginalId;
            //                    }
            //                    else
            //                    {
            //                        blogpostcontent.Original = null;

            //                    }

            //                    pagecontent.Content = blogpostcontent;

            //                }
            //                else if (contentInfo.Flag == 2)
            //                {
            //                    HtmlContent htmlcontent = new HtmlContent();
            //                    htmlcontent.Id = contentInfo.ContentId;
            //                    htmlcontent.ActivationDate = contentInfo.ActivationDate;
            //                    htmlcontent.ExpirationDate = (!string.IsNullOrEmpty(contentInfo.ExpirationDate.ToString())) ? contentInfo.ExpirationDate : null;
            //                    htmlcontent.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //                    htmlcontent.UseCustomCss = contentInfo.UseCustomCss;
            //                    htmlcontent.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //                    htmlcontent.UseCustomJs = contentInfo.UseCustomJs;
            //                    htmlcontent.Html = contentInfo.Html;
            //                    htmlcontent.EditInSourceMode = contentInfo.EditInSourceMode;
            //                    htmlcontent.OriginalText = (!string.IsNullOrEmpty(contentInfo.OriginalText.ToString())) ? contentInfo.OriginalText : null;
            //                    htmlcontent.ContentTextMode = contentInfo.ContentTextMode;
            //                    htmlcontent.Version = contentInfo.Version;
            //                    htmlcontent.IsDeleted = contentInfo.IsDeleted;
            //                    htmlcontent.CreatedOn = contentInfo.CreatedOn;
            //                    htmlcontent.CreatedByUser = contentInfo.CreatedByUser;
            //                    htmlcontent.ModifiedOn = contentInfo.ModifiedOn;
            //                    htmlcontent.ModifiedByUser = contentInfo.ModifiedByUser;
            //                    htmlcontent.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //                    htmlcontent.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //                    htmlcontent.Name = contentInfo.Name;
            //                    htmlcontent.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //                    htmlcontent.Status = contentInfo.Status;
            //                    htmlcontent.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //                    htmlcontent.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //                    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //                    {
            //                        htmlcontent.Original.Id = contentInfo.OriginalId;
            //                    }
            //                    else
            //                    {
            //                        htmlcontent.Original = null;

            //                    }
            //                    pagecontent.Content = htmlcontent;
            //                }


            //                else if (contentInfo.Flag == 3)
            //                {
            //                    ServerControlWidget servercontrolwidget = new ServerControlWidget();
            //                    servercontrolwidget.Id = contentInfo.ContentId;
            //                    servercontrolwidget.Url = contentInfo.Url;
            //                    servercontrolwidget.Version = contentInfo.Version;
            //                    servercontrolwidget.IsDeleted = contentInfo.IsDeleted;
            //                    servercontrolwidget.CreatedOn = contentInfo.CreatedOn;
            //                    servercontrolwidget.CreatedByUser = contentInfo.CreatedByUser;
            //                    servercontrolwidget.ModifiedOn = contentInfo.ModifiedOn;
            //                    servercontrolwidget.ModifiedByUser = contentInfo.ModifiedByUser;
            //                    servercontrolwidget.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //                    servercontrolwidget.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //                    servercontrolwidget.Name = contentInfo.Name;
            //                    servercontrolwidget.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //                    servercontrolwidget.Status = contentInfo.Status;
            //                    servercontrolwidget.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //                    servercontrolwidget.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //                    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //                    {
            //                        servercontrolwidget.Original.Id = contentInfo.OriginalId;
            //                    }
            //                    else
            //                    {
            //                        servercontrolwidget.Original = null;

            //                    }
            //                    pagecontent.Content = servercontrolwidget;
            //                }
            //                else if (contentInfo.Flag == 4)
            //                {
            //                    HtmlContentWidget htmlcontentwidget = new HtmlContentWidget();
            //                    htmlcontentwidget.Id = contentInfo.ContentId;
            //                    htmlcontentwidget.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //                    htmlcontentwidget.UseCustomCss = contentInfo.UseCustomCss;
            //                    htmlcontentwidget.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //                    htmlcontentwidget.UseCustomJs = contentInfo.UseCustomJs;
            //                    htmlcontentwidget.Html = contentInfo.Html;
            //                    htmlcontentwidget.UseHtml = contentInfo.UseHtml;
            //                    htmlcontentwidget.EditInSourceMode = contentInfo.EditInSourceMode;
            //                    htmlcontentwidget.Version = contentInfo.Version;
            //                    htmlcontentwidget.IsDeleted = contentInfo.IsDeleted;
            //                    htmlcontentwidget.CreatedOn = contentInfo.CreatedOn;
            //                    htmlcontentwidget.CreatedByUser = contentInfo.CreatedByUser;
            //                    htmlcontentwidget.ModifiedOn = contentInfo.ModifiedOn;
            //                    htmlcontentwidget.ModifiedByUser = contentInfo.ModifiedByUser;
            //                    htmlcontentwidget.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //                    htmlcontentwidget.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //                    htmlcontentwidget.Name = contentInfo.Name;
            //                    htmlcontentwidget.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //                    htmlcontentwidget.Status = contentInfo.Status;
            //                    htmlcontentwidget.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //                    htmlcontentwidget.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //                    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //                    {
            //                        htmlcontentwidget.Original.Id = contentInfo.OriginalId;
            //                    }
            //                    else
            //                    {
            //                        htmlcontentwidget.Original = null;

            //                    }
            //                    pagecontent.Content = htmlcontentwidget;
            //                }
            //                //pagedetails
            //                pagecontent.Page = pagepropertiesforlayout;

            //                pagecontentslist.Add(pagecontent);


            //            }
            //            pagecontents = pagecontentslist;
            //            pagepropertiesforlayout.PageContents = pagecontents;

            //            pageslist.Add(pagepropertiesforlayout);
            //        }
            //        else if (PagesInfo.Flag == 3)
            //        {
            //            Page pageforlayout = new Page();
            //            pageforlayout.Id = pageId;
            //            pageforlayout.Version = PagesInfo.Version;
            //            pageforlayout.PageUrl = PagesInfo.PageUrl;
            //            pageforlayout.Title = PagesInfo.Title;

            //            pageforlayout.Layout = layoutdetails;
            //            pageforlayout.MetaTitle = (!string.IsNullOrEmpty(PagesInfo.MetaTitle.ToString())) ? PagesInfo.MetaTitle : null;
            //            pageforlayout.MetaKeywords = (!string.IsNullOrEmpty(PagesInfo.MetaKeywords.ToString())) ? PagesInfo.MetaKeywords : null;
            //            pageforlayout.MetaDescription = (!string.IsNullOrEmpty(PagesInfo.MetaDescription.ToString())) ? PagesInfo.MetaDescription : null;
            //            pageforlayout.Status = PagesInfo.Status;
            //            pageforlayout.PageUrlHash = PagesInfo.PageUrlHash;

            //            if (string.IsNullOrEmpty(PagesInfo.MasterPageId.ToString()))
            //            {
            //                pageforlayout.MasterPage = null;
            //            }
            //            else
            //            {
            //                Page masterpage = new Page();
            //                masterpage.Id = new Guid(PagesInfo.MasterPageId.ToString());
            //                pageforlayout.MasterPage = masterpage;
            //            }
            //            pageforlayout.IsMasterPage = PagesInfo.IsMasterPage;
            //            if (string.IsNullOrEmpty(PagesInfo.LanguageId.ToString()))
            //            {
            //                pageforlayout.Language = null;
            //            }
            //            else
            //            {
            //                Language language = new Language();
            //                language.Id = new Guid(PagesInfo.LanguageId.ToString());
            //                JObject requestforlanguage = new JObject();
            //                requestforlanguage.Add("languageId", language.Id);
            //                string languageobj = JsonConvert.SerializeObject(requestforlanguage);
            //                var LanguageModel = _webClient.DownloadData<string>("Blog/GetLanguageDetails", new { Js = languageobj });
            //                dynamic LanguageInfo = JObject.Parse(LanguageModel);
            //                language.Version = LanguageInfo.Version;
            //                language.IsDeleted = LanguageInfo.IsDeleted;
            //                language.CreatedOn = LanguageInfo.CreatedOn;
            //                language.CreatedByUser = LanguageInfo.CreatedByUser;
            //                language.ModifiedOn = LanguageInfo.ModifiedOn;
            //                language.ModifiedByUser = LanguageInfo.ModifiedByUser;
            //                language.DeletedOn = (!string.IsNullOrEmpty(LanguageInfo.DeletedOn.ToString())) ? LanguageInfo.DeletedOn : null;
            //                language.DeletedByUser = LanguageInfo.DeletedByUser;
            //                language.Name = LanguageInfo.Name;
            //                language.Code = LanguageInfo.Code;
            //                pageforlayout.Language = language;
            //            }
            //            pageforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(PagesInfo.LanguageGroupIdentifier.ToString())) ? PagesInfo.LanguageGroupIdentifier : null;
            //            pageforlayout.ForceAccessProtocol = PagesInfo.ForceAccessProtocol;

            //            JObject requestforruleid = new JObject();
            //            requestforruleid.Add("pageId", pageId);
            //            string ruleidobj = JsonConvert.SerializeObject(requestforruleid);
            //            var RuleIdModel = _webClient.DownloadData<JArray>("Blog/GetBlogAccessRulesId", new { Js = ruleidobj });
            //            if (RuleIdModel.Count > 0)
            //            {
            //                IList<AccessRule> accessrules = new List<AccessRule>();
            //                List<AccessRule> accessruleslist = new List<AccessRule>();
            //                for (int l = 0; l < RuleIdModel.Count; l++)
            //                {
            //                    Guid accessruleId = new Guid(RuleIdModel[l]["AccessruleId"].ToString());
            //                    JObject requestforaccessrule = new JObject();
            //                    requestforaccessrule.Add("accessrulesId", accessruleId);
            //                    string accessruleobj = JsonConvert.SerializeObject(requestforaccessrule);
            //                    var accessruleModel = _webClient.DownloadData<string>("Blog/GetAccessRules", new { Js = accessruleobj });
            //                    dynamic accessruleinfo = JObject.Parse(accessruleModel);
            //                    AccessRule rule = new AccessRule();
            //                    rule.Id = accessruleId;
            //                    rule.Version = accessruleinfo.Version;
            //                    rule.IsDeleted = accessruleinfo.IsDeleted;
            //                    rule.CreatedOn = accessruleinfo.CreatedOn;
            //                    rule.CreatedByUser = accessruleinfo.CreatedByUser;
            //                    rule.ModifiedOn = accessruleinfo.ModifiedOn;
            //                    rule.ModifiedByUser = accessruleinfo.ModifiedByUser;
            //                    rule.DeletedOn = (!string.IsNullOrEmpty(accessruleinfo.DeletedOn.ToString())) ? accessruleinfo.DeletedOn : null;
            //                    rule.DeletedByUser = (!string.IsNullOrEmpty(accessruleinfo.DeletedByUser.ToString())) ? accessruleinfo.DeletedByUser : null;
            //                    rule.Identity = accessruleinfo.Identity;
            //                    rule.AccessLevel = accessruleinfo.AccessLevel;
            //                    rule.IsForRole = accessruleinfo.IsForRole;
            //                    accessruleslist.Add(rule);
            //                }
            //                accessrules = accessruleslist;
            //                pageforlayout.AccessRules = accessrules;


            //            }


            //            pageslist.Add(pageforlayout);
            //        }

            //    }
            //    pages = pageslist;
            //    layoutdetails.Pages = pages;




            //    layoutregion.Layout = layoutdetails;
            //    //Region region2 = new Region();
            //    //region2.Id = new Guid(layoutregionmodel[i]["RegionId"].ToString());
            //    layoutregion.Region = region1;
            //    layoutregionlist.Add(layoutregion);
            //}

            //layoutregions = layoutregionlist;
            //region1.LayoutRegion = layoutregions;

            //JObject requestforregionpagecontent = new JObject();
            //requestforregionpagecontent.Add("regionId", regionId);
            //string regionpagecontentobj = JsonConvert.SerializeObject(requestforregionpagecontent);
            //var regionpagecontentmodel = _webClient.DownloadData<JArray>("Blog/GetRegionPageContentDetails", new { Js = regionpagecontentobj });

            //IList<PageContent> regionpagecontents = new List<PageContent>();
            //List<PageContent> regionpagecontentslist = new List<PageContent>();
            //for (int i = 0; i < regionpagecontentmodel.Count; i++)
            //{
            //    PageContent regionpagecontent = new PageContent();
            //    regionpagecontent.Id = new Guid(regionpagecontentmodel[i]["PageContentId"].ToString());
            //    regionpagecontent.Version = Convert.ToInt32(regionpagecontentmodel[i]["Version"]);
            //    regionpagecontent.IsDeleted = (bool)regionpagecontentmodel[i]["IsDeleted"];
            //    regionpagecontent.CreatedOn = (DateTime)regionpagecontentmodel[i]["CreatedOn"];
            //    regionpagecontent.CreatedByUser = regionpagecontentmodel[i]["CreatedByUser"].ToString();
            //    regionpagecontent.ModifiedByUser = regionpagecontentmodel[i]["ModifiedByUser"].ToString();
            //    regionpagecontent.ModifiedOn = (DateTime)regionpagecontentmodel[i]["ModifiedOn"];
            //    if (!string.IsNullOrEmpty(regionpagecontentmodel[i]["DeletedOn"].ToString()))
            //    {
            //        regionpagecontent.DeletedOn = (DateTime)regionpagecontentmodel[i]["DeletedOn"];
            //    }
            //    else
            //    {
            //        regionpagecontent.DeletedOn = null;
            //    }
            //    regionpagecontent.DeletedByUser = (!string.IsNullOrEmpty(regionpagecontentmodel[i]["DeletedByUser"].ToString())) ? regionpagecontentmodel[i]["DeletedByUser"].ToString() : null;
            //    regionpagecontent.Order = Convert.ToInt32(regionpagecontentmodel[i]["Order"]);
            //    BlogPost regionpagedetails = new BlogPost();
            //    regionpagedetails.Id = new Guid(regionpagecontentmodel[i]["PageId"].ToString());
            //    regionpagecontent.Page = regionpagedetails;
            //    BlogPostContent regioncontentdetails = new BlogPostContent();
            //    regioncontentdetails.Id = new Guid(regionpagecontentmodel[i]["ContentId"].ToString());
            //    regionpagecontent.Content = regioncontentdetails;


            //    regionpagecontentslist.Add(regionpagecontent);




            //}

            //regionpagecontents = regionpagecontentslist;
            //region1.PageContents = regionpagecontents;


            //region = region1;
        }

        /// <summary>
        /// Gets the first compatible layout.
        /// </summary>
        /// <returns>Layout for blog post.</returns>
        private Layout GetFirstCompatibleLayout()
        {
            var regionIdentifier = RegionIdentifier.ToLowerInvariant();

            //JObject requestforregion = new JObject();
            //requestforregion.Add("regionIdentifier", regionIdentifier);
            //string regionobj = JsonConvert.SerializeObject(requestforregion);
            //var regionmodel = _webClient.DownloadData<string>("Blog/GetRegionId", new { Js = regionobj });
            //dynamic regionInfo = JObject.Parse(regionmodel);


            //JObject requestforlayoutid = new JObject();
            //requestforlayoutid.Add("regionId", regionInfo.Id);
            //string obj = JsonConvert.SerializeObject(requestforlayoutid);
            //var layoutIdmodel = _webClient.DownloadData<string>("Blog/GetLayoutId", new { Js = obj });
            //dynamic layoutIdInfo = JObject.Parse(layoutIdmodel);
            ////Layout layoutdetails = new Layout();
            ////layoutdetails = GetLayoutDetails(new Guid(layoutIdInfo.layoutId.ToString()));
            //JObject requestforlayoutdetails = new JObject();
            //requestforlayoutdetails.Add("layoutId", layoutIdInfo.layoutId);
            //string layoutobj = JsonConvert.SerializeObject(requestforlayoutdetails);
            //var layoutmodel = _webClient.DownloadData<string>("Blog/GetLayoutDetails", new { Js = layoutobj });
            //dynamic layoutInfo = JObject.Parse(layoutmodel);
            //Layout layoutdetails = new Layout();
            //layoutdetails.Id = layoutIdInfo.layoutId;
            //layoutdetails.Version = layoutInfo.Version;
            //layoutdetails.IsDeleted = layoutInfo.IsDeleted;
            //layoutdetails.CreatedOn = layoutInfo.CreatedOn;
            //layoutdetails.CreatedByUser = layoutInfo.CreatedByUser;
            //layoutdetails.ModifiedOn = layoutInfo.ModifiedOn;
            //layoutdetails.ModifiedByUser = layoutInfo.ModifiedByUser;
            //layoutdetails.DeletedOn = (!string.IsNullOrEmpty(layoutInfo.DeletedOn.ToString())) ? layoutInfo.DeletedOn : null;
            //layoutdetails.DeletedByUser = (!string.IsNullOrEmpty(layoutInfo.DeletedByUser.ToString())) ? layoutInfo.DeletedByUser : null;
            //layoutdetails.Name = layoutInfo.Name;
            //layoutdetails.LayoutPath = layoutInfo.LayoutPath;


            //layoutdetails.PreviewUrl = (!string.IsNullOrEmpty(layoutInfo.PreviewUrl.ToString())) ? layoutInfo.PreviewUrl : null;


            //JObject requestforlayoutregiondetails = new JObject();
            //requestforlayoutregiondetails.Add("layoutId", layoutIdInfo.layoutId);
            //string layoutregionobj = JsonConvert.SerializeObject(requestforlayoutregiondetails);
            //var layoutregionmodel = _webClient.DownloadData<JArray>("Blog/GetLayoutRegionDetailsForLayout", new { Js = layoutregionobj });

            //IList<LayoutRegion> layoutregions = new List<LayoutRegion>();
            //List<LayoutRegion> layoutregionlist = new List<LayoutRegion>();

            //for (int i = 0; i < layoutregionmodel.Count; i++)
            //{
            //    LayoutRegion layoutregion = new LayoutRegion();
            //    layoutregion.Id = new Guid(layoutregionmodel[i]["LayoutRegionId"].ToString());
            //    layoutregion.Version = Convert.ToInt32(layoutregionmodel[i]["Version"]);
            //    layoutregion.IsDeleted = (bool)layoutregionmodel[i]["IsDeleted"];
            //    layoutregion.CreatedOn = (DateTime)layoutregionmodel[i]["CreatedOn"];
            //    layoutregion.CreatedByUser = layoutregionmodel[i]["CreatedByUser"].ToString();
            //    layoutregion.ModifiedOn = (DateTime)layoutregionmodel[i]["ModifiedOn"];
            //    layoutregion.ModifiedByUser = layoutregionmodel[i]["ModifiedByUser"].ToString();
            //    if (!string.IsNullOrEmpty(layoutregionmodel[i]["DeletedOn"].ToString()))
            //    {

            //        layoutregion.DeletedOn = (DateTime)layoutregionmodel[i]["DeletedOn"];
            //    }
            //    else
            //    {
            //        layoutregion.DeletedOn = null;
            //    }
            //    layoutregion.DeletedByUser = (!string.IsNullOrEmpty(layoutregionmodel[i]["DeletedByUser"].ToString())) ? layoutregionmodel[i]["DeletedByUser"].ToString() : null;
            //    layoutregion.Description = (!string.IsNullOrEmpty(layoutregionmodel[i]["Description"].ToString())) ? layoutregionmodel[i]["Description"].ToString() : null;
            //    Region region = new Region();
            //    region.Id = new Guid(layoutregionmodel[i]["RegionId"].ToString());
            //    JObject requestforregiondetails = new JObject();
            //    requestforregiondetails.Add("regionId", region.Id);
            //    string regionobj1 = JsonConvert.SerializeObject(requestforregiondetails);
            //    var regionmodel1 = _webClient.DownloadData<string>("Blog/GetRegionDetails", new { Js = regionobj1 });
            //    dynamic regionInfo1 = JObject.Parse(regionmodel1);

            //    region.Version = regionInfo1.Version;
            //    region.IsDeleted = regionInfo1.IsDeleted;
            //    region.CreatedOn = regionInfo1.CreatedOn;
            //    region.CreatedByUser = regionInfo1.CreatedByUser;
            //    region.ModifiedOn = regionInfo1.ModifiedOn;
            //    region.ModifiedByUser = regionInfo1.ModifiedByUser;
            //    region.DeletedOn = (!string.IsNullOrEmpty(regionInfo1.DeletedOn.ToString())) ? regionInfo1.DeletedOn : null;
            //    region.DeletedByUser = (!string.IsNullOrEmpty(regionInfo1.DeletedByUser.ToString())) ? regionInfo1.DeletedByUser : null;
            //    region.RegionIdentifier = regionInfo1.RegionIdentifier;
            //    JObject requestforlayoutregiondetails1 = new JObject();
            //    requestforlayoutregiondetails1.Add("regionId", region.Id);
            //    string layoutregionobj1 = JsonConvert.SerializeObject(requestforlayoutregiondetails1);
            //    var layoutregionmodel1 = _webClient.DownloadData<JArray>("Blog/GetLayoutRegionDetails", new { Js = layoutregionobj1 });

            //    IList<LayoutRegion> layoutregions1 = new List<LayoutRegion>();
            //    List<LayoutRegion> layoutregionlist1 = new List<LayoutRegion>();

            //    for (int j = 0; j < layoutregionmodel1.Count; j++)
            //    {
            //        LayoutRegion layoutregion1 = new LayoutRegion();
            //        layoutregion1.Id = new Guid(layoutregionmodel1[j]["LayoutRegionId"].ToString());
            //        layoutregion1.Version = Convert.ToInt32(layoutregionmodel1[j]["Version"]);
            //        layoutregion1.IsDeleted = (bool)layoutregionmodel1[j]["IsDeleted"];
            //        layoutregion1.CreatedOn = (DateTime)layoutregionmodel1[j]["CreatedOn"];
            //        layoutregion1.CreatedByUser = layoutregionmodel1[j]["CreatedByUser"].ToString();
            //        layoutregion1.ModifiedOn = (DateTime)layoutregionmodel1[j]["ModifiedOn"];
            //        layoutregion1.ModifiedByUser = layoutregionmodel1[j]["ModifiedByUser"].ToString();
            //        if (!string.IsNullOrEmpty(layoutregionmodel1[j]["DeletedOn"].ToString()))
            //        {

            //            layoutregion1.DeletedOn = (DateTime)layoutregionmodel1[j]["DeletedOn"];
            //        }
            //        else
            //        {
            //            layoutregion1.DeletedOn = null;
            //        }
            //        layoutregion1.DeletedByUser = (!string.IsNullOrEmpty(layoutregionmodel1[j]["DeletedByUser"].ToString())) ? layoutregionmodel[j]["DeletedByUser"].ToString() : null;
            //        layoutregion1.Description = (!string.IsNullOrEmpty(layoutregionmodel1[j]["Description"].ToString())) ? layoutregionmodel[j]["Description"].ToString() : null;
            //        //layoutregion1.Layout = layoutdetails;
            //        layoutregion1.Region = region;
            //        layoutregionlist1.Add(layoutregion1);
            //    }
            //    layoutregions1 = layoutregionlist1;
            //    region.LayoutRegion = layoutregions1;

            //    JObject requestforregionpagecontent = new JObject();
            //    requestforregionpagecontent.Add("regionId", region.Id);
            //    string regionpagecontentobj = JsonConvert.SerializeObject(requestforregionpagecontent);
            //    var regionpagecontentmodel = _webClient.DownloadData<JArray>("Blog/GetRegionPageContentDetails", new { Js = regionpagecontentobj });

            //    IList<PageContent> pagecontents = new List<PageContent>();
            //    List<PageContent> pagecontentslist = new List<PageContent>();
            //    for (int k = 0; k < regionpagecontentmodel.Count; k++)
            //    {
            //        PageContent regionpagecontent = new PageContent();
            //        regionpagecontent.Id = new Guid(regionpagecontentmodel[k]["PageContentId"].ToString());
            //        regionpagecontent.Version = Convert.ToInt32(regionpagecontentmodel[k]["Version"]);
            //        regionpagecontent.IsDeleted = (bool)regionpagecontentmodel[k]["IsDeleted"];
            //        regionpagecontent.CreatedOn = (DateTime)regionpagecontentmodel[k]["CreatedOn"];
            //        regionpagecontent.CreatedByUser = regionpagecontentmodel[k]["CreatedByUser"].ToString();
            //        regionpagecontent.ModifiedByUser = regionpagecontentmodel[k]["ModifiedByUser"].ToString();
            //        regionpagecontent.ModifiedOn = (DateTime)regionpagecontentmodel[k]["ModifiedOn"];
            //        if (!string.IsNullOrEmpty(regionpagecontentmodel[k]["DeletedOn"].ToString()))
            //        {
            //            regionpagecontent.DeletedOn = (DateTime)regionpagecontentmodel[k]["DeletedOn"];
            //        }
            //        else
            //        {
            //            regionpagecontent.DeletedOn = null;
            //        }
            //        regionpagecontent.DeletedByUser = (!string.IsNullOrEmpty(regionpagecontentmodel[k]["DeletedByUser"].ToString())) ? regionpagecontentmodel[k]["DeletedByUser"].ToString() : null;
            //        regionpagecontent.Order = Convert.ToInt32(regionpagecontentmodel[k]["Order"]);



            //        //        //blogpostdetails
            //        //        //start
            //        Guid pageId = new Guid(regionpagecontentmodel[k]["PageId"].ToString());
            //        JObject requestdetails = new JObject();
            //        requestdetails.Add("pageId", pageId);
            //        string jsobj = JsonConvert.SerializeObject(requestdetails);
            //        var blogPostmodel = _webClient.DownloadData<string>("Blog/GetPagesForLayout", new { Js = jsobj });
            //        dynamic Info = JObject.Parse(blogPostmodel);

            //        if (Info.Flag == 1)
            //        {
            //            BlogPost blogpostforlayout = new BlogPost();
            //            blogpostforlayout.Id = pageId;
            //            blogpostforlayout.ActivationDate = Info.ActivationDate;
            //            blogpostforlayout.ExpirationDate = (!string.IsNullOrEmpty(Info.ExpirationDate.ToString())) ? Info.ExpirationDate : null;
            //            blogpostforlayout.Description = Info.Description;
            //            if (string.IsNullOrEmpty(Info.ImageId.ToString()))
            //            {
            //                blogpostforlayout.Image = null;
            //            }
            //            else
            //            {
            //                MediaImage image = new MediaImage();
            //                image.Id = new Guid(Info.ImageId.ToString());
            //                blogpostforlayout.Image = image;
            //            }

            //            blogpostforlayout.CustomCss = (!string.IsNullOrEmpty(Info.CustomCss.ToString())) ? Info.CustomCss : null;
            //            blogpostforlayout.CustomJS = (!string.IsNullOrEmpty(Info.CustomJS.ToString())) ? Info.CustomJS : null;
            //            blogpostforlayout.UseCanonicalUrl = Info.UseCanonicalUrl;
            //            blogpostforlayout.UseNoFollow = Info.UseNoFollow;
            //            blogpostforlayout.UseNoIndex = Info.UseNoIndex;

            //            if (string.IsNullOrEmpty(Info.SecondaryImageId.ToString()))
            //            {
            //                blogpostforlayout.SecondaryImage = null;
            //            }
            //            else
            //            {
            //                MediaImage secondaryimage = new MediaImage();
            //                secondaryimage.Id = new Guid(Info.SecondaryImageId.ToString());
            //                blogpostforlayout.SecondaryImage = secondaryimage;
            //            }
            //            if (string.IsNullOrEmpty(Info.FeaturedImageId.ToString()))
            //            {
            //                blogpostforlayout.FeaturedImage = null;
            //            }
            //            else
            //            {
            //                MediaImage featuredimage = new MediaImage();
            //                featuredimage.Id = new Guid(Info.FeaturedImageId.ToString());
            //                blogpostforlayout.FeaturedImage = featuredimage;
            //            }

            //            blogpostforlayout.IsArchived = Info.IsArchived;
            //            blogpostforlayout.Version = Info.Version;
            //            blogpostforlayout.PageUrl = Info.PageUrl;
            //            blogpostforlayout.Title = Info.Title;
            //            blogpostforlayout.Layout = layoutdetails;
            //            blogpostforlayout.MetaTitle = (!string.IsNullOrEmpty(Info.MetaTitle.ToString())) ? Info.MetaTitle : null;
            //            blogpostforlayout.MetaKeywords = (!string.IsNullOrEmpty(Info.MetaKeywords.ToString())) ? Info.MetaKeywords : null;
            //            blogpostforlayout.MetaDescription = (!string.IsNullOrEmpty(Info.MetaDescription.ToString())) ? Info.MetaDescription : null;
            //            blogpostforlayout.Status = Info.Status;
            //            blogpostforlayout.PageUrlHash = Info.PageUrlHash;

            //            if (string.IsNullOrEmpty(Info.MasterPageId.ToString()))
            //            {
            //                blogpostforlayout.MasterPage = null;
            //            }
            //            else
            //            {
            //                Page masterpage = new Page();
            //                masterpage.Id = new Guid(Info.MasterPageId.ToString());
            //                blogpostforlayout.MasterPage = masterpage;
            //            }
            //            blogpostforlayout.IsMasterPage = Info.IsMasterPage;
            //            if (string.IsNullOrEmpty(Info.LanguageId.ToString()))
            //            {
            //                blogpostforlayout.Language = null;
            //            }
            //            else
            //            {
            //                Language language = new Language();
            //                language.Id = new Guid(Info.LanguageId.ToString());
            //                blogpostforlayout.Language = language;
            //            }
            //            blogpostforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(Info.LanguageGroupIdentifier.ToString())) ? Info.LanguageGroupIdentifier : null;
            //            blogpostforlayout.ForceAccessProtocol = Info.ForceAccessProtocol;


            //            regionpagecontent.Page = blogpostforlayout;
            //        }
            //        else if (Info.Flag == 2)
            //        {
            //            PageProperties pagepropertiesforlayout = new PageProperties();
            //            pagepropertiesforlayout.Id = pageId;
            //            pagepropertiesforlayout.Description = Info.Description;
            //            if (string.IsNullOrEmpty(Info.ImageId.ToString()))
            //            {
            //                pagepropertiesforlayout.Image = null;
            //            }
            //            else
            //            {
            //                MediaImage image = new MediaImage();
            //                image.Id = new Guid(Info.ImageId.ToString());
            //                pagepropertiesforlayout.Image = image;
            //            }

            //            pagepropertiesforlayout.CustomCss = (!string.IsNullOrEmpty(Info.CustomCss.ToString())) ? Info.CustomCss : null;
            //            pagepropertiesforlayout.CustomJS = (!string.IsNullOrEmpty(Info.CustomJS.ToString())) ? Info.CustomJS : null;
            //            pagepropertiesforlayout.UseCanonicalUrl = Info.UseCanonicalUrl;
            //            pagepropertiesforlayout.UseNoFollow = Info.UseNoFollow;
            //            pagepropertiesforlayout.UseNoIndex = Info.UseNoIndex;

            //            if (string.IsNullOrEmpty(Info.SecondaryImageId.ToString()))
            //            {
            //                pagepropertiesforlayout.SecondaryImage = null;
            //            }
            //            else
            //            {
            //                MediaImage secondaryimage = new MediaImage();
            //                secondaryimage.Id = new Guid(Info.SecondaryImageId.ToString());
            //                pagepropertiesforlayout.SecondaryImage = secondaryimage;
            //            }
            //            if (string.IsNullOrEmpty(Info.FeaturedImageId.ToString()))
            //            {
            //                pagepropertiesforlayout.FeaturedImage = null;
            //            }
            //            else
            //            {
            //                MediaImage featuredimage = new MediaImage();
            //                featuredimage.Id = new Guid(Info.FeaturedImageId.ToString());
            //                pagepropertiesforlayout.FeaturedImage = featuredimage;
            //            }

            //            pagepropertiesforlayout.IsArchived = Info.IsArchived;
            //            pagepropertiesforlayout.Version = Info.Version;
            //            pagepropertiesforlayout.PageUrl = Info.PageUrl;
            //            pagepropertiesforlayout.Title = Info.Title;

            //            pagepropertiesforlayout.Layout = layoutdetails;
            //            pagepropertiesforlayout.MetaTitle = (!string.IsNullOrEmpty(Info.MetaTitle.ToString())) ? Info.MetaTitle : null;
            //            pagepropertiesforlayout.MetaKeywords = (!string.IsNullOrEmpty(Info.MetaKeywords.ToString())) ? Info.MetaKeywords : null;
            //            pagepropertiesforlayout.MetaDescription = (!string.IsNullOrEmpty(Info.MetaDescription.ToString())) ? Info.MetaDescription : null;
            //            pagepropertiesforlayout.Status = Info.Status;
            //            pagepropertiesforlayout.PageUrlHash = Info.PageUrlHash;

            //            if (string.IsNullOrEmpty(Info.MasterPageId.ToString()))
            //            {
            //                pagepropertiesforlayout.MasterPage = null;
            //            }
            //            else
            //            {
            //                Page masterpage = new Page();
            //                masterpage.Id = new Guid(Info.MasterPageId.ToString());
            //                pagepropertiesforlayout.MasterPage = masterpage;
            //            }
            //            pagepropertiesforlayout.IsMasterPage = Info.IsMasterPage;
            //            if (string.IsNullOrEmpty(Info.LanguageId.ToString()))
            //            {
            //                pagepropertiesforlayout.Language = null;
            //            }
            //            else
            //            {
            //                Language language = new Language();
            //                language.Id = new Guid(Info.LanguageId.ToString());
            //                pagepropertiesforlayout.Language = language;
            //            }
            //            pagepropertiesforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(Info.LanguageGroupIdentifier.ToString())) ? Info.LanguageGroupIdentifier : null;
            //            pagepropertiesforlayout.ForceAccessProtocol = Info.ForceAccessProtocol;


            //            regionpagecontent.Page = pagepropertiesforlayout;
            //        }
            //        else if (Info.Flag == 3)
            //        {
            //            Page pageforlayout = new Page();
            //            pageforlayout.Id = pageId;
            //            pageforlayout.Version = Info.Version;
            //            pageforlayout.PageUrl = Info.PageUrl;
            //            pageforlayout.Title = Info.Title;

            //            pageforlayout.Layout = layoutdetails;
            //            pageforlayout.MetaTitle = (!string.IsNullOrEmpty(Info.MetaTitle.ToString())) ? Info.MetaTitle : null;
            //            pageforlayout.MetaKeywords = (!string.IsNullOrEmpty(Info.MetaKeywords.ToString())) ? Info.MetaKeywords : null;
            //            pageforlayout.MetaDescription = (!string.IsNullOrEmpty(Info.MetaDescription.ToString())) ? Info.MetaDescription : null;
            //            pageforlayout.Status = Info.Status;
            //            pageforlayout.PageUrlHash = Info.PageUrlHash;

            //            if (string.IsNullOrEmpty(Info.MasterPageId.ToString()))
            //            {
            //                pageforlayout.MasterPage = null;
            //            }
            //            else
            //            {
            //                Page masterpage = new Page();
            //                masterpage.Id = new Guid(Info.MasterPageId.ToString());
            //                pageforlayout.MasterPage = masterpage;
            //            }
            //            pageforlayout.IsMasterPage = Info.IsMasterPage;
            //            if (string.IsNullOrEmpty(Info.LanguageId.ToString()))
            //            {
            //                pageforlayout.Language = null;
            //            }
            //            else
            //            {
            //                Language language = new Language();
            //                language.Id = new Guid(Info.LanguageId.ToString());
            //                pageforlayout.Language = language;
            //            }
            //            pageforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(Info.LanguageGroupIdentifier.ToString())) ? Info.LanguageGroupIdentifier : null;
            //            pageforlayout.ForceAccessProtocol = Info.ForceAccessProtocol;


            //            regionpagecontent.Page = pageforlayout;
            //        }




            //        //End

            //        JObject requestforcontentdetails = new JObject();
            //        requestforcontentdetails.Add("contentId", regionpagecontentmodel[k]["ContentId"].ToString());


            //        string contentobj = JsonConvert.SerializeObject(requestforcontentdetails);
            //        var contentmodel = _webClient.DownloadData<string>("Blog/GetContentDetailsForPageContents", new { Js = contentobj });

            //        dynamic contentInfo = JObject.Parse(contentmodel);
            //        if (contentInfo.Flag == 1)
            //        {
            //            BlogPostContent blogpostcontent = new BlogPostContent();
            //            blogpostcontent.Id = contentInfo.ContentId;
            //            blogpostcontent.ActivationDate = contentInfo.ActivationDate;
            //            blogpostcontent.ExpirationDate = (!string.IsNullOrEmpty(contentInfo.ExpirationDate.ToString())) ? contentInfo.ExpirationDate : null;
            //            blogpostcontent.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //            blogpostcontent.UseCustomCss = contentInfo.UseCustomCss;
            //            blogpostcontent.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //            blogpostcontent.UseCustomJs = contentInfo.UseCustomJs;
            //            blogpostcontent.Html = contentInfo.Html;
            //            blogpostcontent.EditInSourceMode = contentInfo.EditInSourceMode;
            //            blogpostcontent.OriginalText = (!string.IsNullOrEmpty(contentInfo.OriginalText.ToString())) ? contentInfo.OriginalText : null;
            //            blogpostcontent.ContentTextMode = contentInfo.ContentTextMode;
            //            blogpostcontent.Version = contentInfo.Version;
            //            blogpostcontent.IsDeleted = contentInfo.IsDeleted;
            //            blogpostcontent.CreatedOn = contentInfo.CreatedOn;
            //            blogpostcontent.CreatedByUser = contentInfo.CreatedByUser;
            //            blogpostcontent.ModifiedOn = contentInfo.ModifiedOn;
            //            blogpostcontent.ModifiedByUser = contentInfo.ModifiedByUser;
            //            blogpostcontent.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //            blogpostcontent.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //            blogpostcontent.Name = contentInfo.Name;
            //            blogpostcontent.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //            blogpostcontent.Status = contentInfo.Status;
            //            blogpostcontent.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //            blogpostcontent.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //            if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //            {
            //                blogpostcontent.Original.Id = contentInfo.OriginalId;
            //            }
            //            else
            //            {
            //                blogpostcontent.Original = null;

            //            }

            //            regionpagecontent.Content = blogpostcontent;

            //        }
            //        else if (contentInfo.Flag == 2)
            //        {
            //            HtmlContent htmlcontent = new HtmlContent();
            //            htmlcontent.Id = contentInfo.ContentId;
            //            htmlcontent.ActivationDate = contentInfo.ActivationDate;
            //            htmlcontent.ExpirationDate = (!string.IsNullOrEmpty(contentInfo.ExpirationDate.ToString())) ? contentInfo.ExpirationDate : null;
            //            htmlcontent.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //            htmlcontent.UseCustomCss = contentInfo.UseCustomCss;
            //            htmlcontent.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //            htmlcontent.UseCustomJs = contentInfo.UseCustomJs;
            //            htmlcontent.Html = contentInfo.Html;
            //            htmlcontent.EditInSourceMode = contentInfo.EditInSourceMode;
            //            htmlcontent.OriginalText = (!string.IsNullOrEmpty(contentInfo.OriginalText.ToString())) ? contentInfo.OriginalText : null;
            //            htmlcontent.ContentTextMode = contentInfo.ContentTextMode;
            //            htmlcontent.Version = contentInfo.Version;
            //            htmlcontent.IsDeleted = contentInfo.IsDeleted;
            //            htmlcontent.CreatedOn = contentInfo.CreatedOn;
            //            htmlcontent.CreatedByUser = contentInfo.CreatedByUser;
            //            htmlcontent.ModifiedOn = contentInfo.ModifiedOn;
            //            htmlcontent.ModifiedByUser = contentInfo.ModifiedByUser;
            //            htmlcontent.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //            htmlcontent.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //            htmlcontent.Name = contentInfo.Name;
            //            htmlcontent.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //            htmlcontent.Status = contentInfo.Status;
            //            htmlcontent.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //            htmlcontent.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //            if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //            {
            //                htmlcontent.Original.Id = contentInfo.OriginalId;
            //            }
            //            else
            //            {
            //                htmlcontent.Original = null;

            //            }
            //            regionpagecontent.Content = htmlcontent;
            //        }


            //        else if (contentInfo.Flag == 3)
            //        {
            //            ServerControlWidget servercontrolwidget = new ServerControlWidget();
            //            servercontrolwidget.Id = contentInfo.ContentId;
            //            servercontrolwidget.Url = contentInfo.Url;
            //            servercontrolwidget.Version = contentInfo.Version;
            //            servercontrolwidget.IsDeleted = contentInfo.IsDeleted;
            //            servercontrolwidget.CreatedOn = contentInfo.CreatedOn;
            //            servercontrolwidget.CreatedByUser = contentInfo.CreatedByUser;
            //            servercontrolwidget.ModifiedOn = contentInfo.ModifiedOn;
            //            servercontrolwidget.ModifiedByUser = contentInfo.ModifiedByUser;
            //            servercontrolwidget.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //            servercontrolwidget.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //            servercontrolwidget.Name = contentInfo.Name;
            //            servercontrolwidget.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //            servercontrolwidget.Status = contentInfo.Status;
            //            servercontrolwidget.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //            servercontrolwidget.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //            if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //            {
            //                servercontrolwidget.Original.Id = contentInfo.OriginalId;
            //            }
            //            else
            //            {
            //                servercontrolwidget.Original = null;

            //            }
            //            regionpagecontent.Content = servercontrolwidget;
            //        }
            //        else if (contentInfo.Flag == 4)
            //        {
            //            HtmlContentWidget htmlcontentwidget = new HtmlContentWidget();
            //            htmlcontentwidget.Id = contentInfo.ContentId;
            //            htmlcontentwidget.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //            htmlcontentwidget.UseCustomCss = contentInfo.UseCustomCss;
            //            htmlcontentwidget.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //            htmlcontentwidget.UseCustomJs = contentInfo.UseCustomJs;
            //            htmlcontentwidget.Html = contentInfo.Html;
            //            htmlcontentwidget.UseHtml = contentInfo.UseHtml;
            //            htmlcontentwidget.EditInSourceMode = contentInfo.EditInSourceMode;
            //            htmlcontentwidget.Version = contentInfo.Version;
            //            htmlcontentwidget.IsDeleted = contentInfo.IsDeleted;
            //            htmlcontentwidget.CreatedOn = contentInfo.CreatedOn;
            //            htmlcontentwidget.CreatedByUser = contentInfo.CreatedByUser;
            //            htmlcontentwidget.ModifiedOn = contentInfo.ModifiedOn;
            //            htmlcontentwidget.ModifiedByUser = contentInfo.ModifiedByUser;
            //            htmlcontentwidget.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //            htmlcontentwidget.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //            htmlcontentwidget.Name = contentInfo.Name;
            //            htmlcontentwidget.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //            htmlcontentwidget.Status = contentInfo.Status;
            //            htmlcontentwidget.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //            htmlcontentwidget.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //            if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //            {
            //                htmlcontentwidget.Original.Id = contentInfo.OriginalId;
            //            }
            //            else
            //            {
            //                htmlcontentwidget.Original = null;

            //            }
            //            regionpagecontent.Content = htmlcontentwidget;
            //        }
            //      //  //End

            //        regionpagecontent.Region = region;



            //        pagecontentslist.Add(regionpagecontent);
            //    }
            //    pagecontents = pagecontentslist;
            //    region.PageContents = pagecontents;


            //    layoutregion.Region = region;
            //    layoutregion.Layout = layoutdetails;
            //    layoutregionlist.Add(layoutregion);

            //}
            //layoutregions = layoutregionlist;
            //layoutdetails.LayoutRegions = layoutregions;

            ////Pages details
            //JObject requestforpageids = new JObject();
            //requestforpageids.Add("layoutId", layoutIdInfo.layoutId);
            //string pageidsobj = JsonConvert.SerializeObject(requestforpageids);
            //var PageIdsModel = _webClient.DownloadData<JArray>("Blog/GetPageIds", new { Js = pageidsobj });
            //IList<Page> pages = new List<Page>();
            //List<Page> pageslist = new List<Page>();
            //for (int k = 0; k < PageIdsModel.Count; k++)
            //{
            //    Guid pageId = new Guid(PageIdsModel[k]["PageId"].ToString());
            //    JObject requestforpages = new JObject();
            //    requestforpages.Add("pageId", pageId);
            //    string pagesobj = JsonConvert.SerializeObject(requestforpages);
            //    var PagesModel = _webClient.DownloadData<string>("Blog/GetPagesForLayout", new { Js = pagesobj });
            //    dynamic PagesInfo = JObject.Parse(PagesModel);
            //    if (PagesInfo.Flag == 1)
            //    {
            //        BlogPost blogpostforlayout = new BlogPost();
            //        blogpostforlayout.Id = pageId;
            //        blogpostforlayout.ActivationDate = PagesInfo.ActivationDate;
            //        blogpostforlayout.ExpirationDate = (!string.IsNullOrEmpty(PagesInfo.ExpirationDate.ToString())) ? PagesInfo.ExpirationDate : null;
            //        blogpostforlayout.Description = PagesInfo.Description;
            //        if (string.IsNullOrEmpty(PagesInfo.ImageId.ToString()))
            //        {
            //            blogpostforlayout.Image = null;
            //        }
            //        else
            //        {
            //            MediaImage image = new MediaImage();
            //            image.Id = new Guid(PagesInfo.ImageId.ToString());
            //            JObject requestforimagedetails = new JObject();
            //            requestforimagedetails.Add("imageId", image.Id);
            //            string imageobj = JsonConvert.SerializeObject(requestforimagedetails);
            //            var ImageModel = _webClient.DownloadData<string>("Blog/GetImageDetails", new { Js = imageobj });
            //            dynamic ImageInfo = JObject.Parse(ImageModel);
            //            image.Caption = (!string.IsNullOrEmpty(ImageInfo.Caption.ToString())) ? ImageInfo.Caption : null;
            //            image.ImageAlign = (!string.IsNullOrEmpty(ImageInfo.ImageAlign.ToString())) ? ImageInfo.ImageAlign : null;
            //            image.Width = ImageInfo.Width;
            //            image.Height = ImageInfo.Height;
            //            image.CropCoordX1 = (!string.IsNullOrEmpty(ImageInfo.CropCoordX1.ToString())) ? ImageInfo.CropCoordX1 : null;
            //            image.CropCoordX2 = (!string.IsNullOrEmpty(ImageInfo.CropCoordX2.ToString())) ? ImageInfo.CropCoordX2 : null;
            //            image.CropCoordY1 = (!string.IsNullOrEmpty(ImageInfo.CropCoordY1.ToString())) ? ImageInfo.CropCoordY1 : null;
            //            image.CropCoordY2 = (!string.IsNullOrEmpty(ImageInfo.CropCoordY2.ToString())) ? ImageInfo.CropCoordY2 : null;
            //            image.OriginalWidth = ImageInfo.OriginalWidth;
            //            image.OriginalHeight = ImageInfo.OriginalHeight;
            //            image.OriginalSize = ImageInfo.OriginalSize;
            //            image.OriginalUri = ImageInfo.OriginalUri;
            //            image.PublicOriginallUrl = ImageInfo.PublicOriginallUrl;
            //            image.IsOriginalUploaded = (!string.IsNullOrEmpty(ImageInfo.IsOriginalUploaded.ToString())) ? ImageInfo.IsOriginalUploaded : null;
            //            image.ThumbnailWidth = ImageInfo.ThumbnailWidth;
            //            image.ThumbnailHeight = ImageInfo.ThumbnailHeight;
            //            image.ThumbnailSize = ImageInfo.ThumbnailSize;
            //            image.ThumbnailUri = ImageInfo.ThumbnailUri;
            //            image.PublicThumbnailUrl = ImageInfo.PublicThumbnailUrl;
            //            image.IsThumbnailUploaded = (!string.IsNullOrEmpty(ImageInfo.IsThumbnailUploaded.ToString())) ? ImageInfo.IsThumbnailUploaded : null;
            //            image.Version = ImageInfo.Version;
            //            image.IsDeleted = ImageInfo.IsDeleted;
            //            image.CreatedOn = ImageInfo.CreatedOn;
            //            image.CreatedByUser = ImageInfo.CreatedByUser;
            //            image.ModifiedOn = ImageInfo.ModifiedOn;
            //            image.ModifiedByUser = ImageInfo.ModifiedByUser;
            //            image.DeletedOn = (!string.IsNullOrEmpty(ImageInfo.DeletedOn.ToString())) ? ImageInfo.DeletedOn : null;
            //            image.DeletedByUser = (!string.IsNullOrEmpty(ImageInfo.DeletedByUser.ToString())) ? ImageInfo.DeletedByUser : null;
            //            if (!string.IsNullOrEmpty(ImageInfo.FolderId.ToString()))
            //            {
            //                MediaFolder folder = new MediaFolder();
            //                folder.Id = ImageInfo.FolderId;
            //                image.Folder = folder;
            //            }
            //            else
            //            {
            //                image.Folder = null;
            //            }
            //            image.Title = ImageInfo.Title;
            //            image.Type = ImageInfo.Type;
            //            image.ContentType = ImageInfo.ContentType;
            //            image.IsArchived = ImageInfo.IsArchived;
            //            if (!string.IsNullOrEmpty(ImageInfo.FolderId.ToString()))
            //            {
            //                image.Original.Id = ImageInfo.OriginalId;
            //            }
            //            else
            //            {
            //                image.Original = null;

            //            }

            //            image.PublishedOn = ImageInfo.PublishedOn;
            //            //image.Image.Id = ImageInfo.ImageId;
            //            image.Description = (!string.IsNullOrEmpty(ImageInfo.Description.ToString())) ? ImageInfo.Description : null;

            //            blogpostforlayout.Image = image;
            //        }

            //        blogpostforlayout.CustomCss = (!string.IsNullOrEmpty(PagesInfo.CustomCss.ToString())) ? PagesInfo.CustomCss : null;
            //        blogpostforlayout.CustomJS = (!string.IsNullOrEmpty(PagesInfo.CustomJS.ToString())) ? PagesInfo.CustomJS : null;
            //        blogpostforlayout.UseCanonicalUrl = PagesInfo.UseCanonicalUrl;
            //        blogpostforlayout.UseNoFollow = PagesInfo.UseNoFollow;
            //        blogpostforlayout.UseNoIndex = PagesInfo.UseNoIndex;

            //        if (string.IsNullOrEmpty(PagesInfo.SecondaryImageId.ToString()))
            //        {
            //            blogpostforlayout.SecondaryImage = null;
            //        }
            //        else
            //        {
            //            MediaImage secondaryimage = new MediaImage();
            //            secondaryimage.Id = new Guid(PagesInfo.SecondaryImageId.ToString());
            //            blogpostforlayout.SecondaryImage = secondaryimage;
            //        }
            //        if (string.IsNullOrEmpty(PagesInfo.FeaturedImageId.ToString()))
            //        {
            //            blogpostforlayout.FeaturedImage = null;
            //        }
            //        else
            //        {
            //            MediaImage featuredimage = new MediaImage();
            //            featuredimage.Id = new Guid(PagesInfo.FeaturedImageId.ToString());
            //            blogpostforlayout.FeaturedImage = featuredimage;
            //        }

            //        blogpostforlayout.IsArchived = PagesInfo.IsArchived;
            //        blogpostforlayout.Version = PagesInfo.Version;
            //        blogpostforlayout.PageUrl = PagesInfo.PageUrl;
            //        blogpostforlayout.Title = PagesInfo.Title;

            //        blogpostforlayout.Layout = layoutdetails;
            //        blogpostforlayout.MetaTitle = (!string.IsNullOrEmpty(PagesInfo.MetaTitle.ToString())) ? PagesInfo.MetaTitle : null;
            //        blogpostforlayout.MetaKeywords = (!string.IsNullOrEmpty(PagesInfo.MetaKeywords.ToString())) ? PagesInfo.MetaKeywords : null;
            //        blogpostforlayout.MetaDescription = (!string.IsNullOrEmpty(PagesInfo.MetaDescription.ToString())) ? PagesInfo.MetaDescription : null;
            //        blogpostforlayout.Status = PagesInfo.Status;
            //        blogpostforlayout.PageUrlHash = PagesInfo.PageUrlHash;

            //        if (string.IsNullOrEmpty(PagesInfo.MasterPageId.ToString()))
            //        {
            //            blogpostforlayout.MasterPage = null;
            //        }
            //        else
            //        {
            //            Page masterpage = new Page();
            //            masterpage.Id = new Guid(PagesInfo.MasterPageId.ToString());
            //            blogpostforlayout.MasterPage = masterpage;
            //        }
            //        blogpostforlayout.IsMasterPage = PagesInfo.IsMasterPage;
            //        if (string.IsNullOrEmpty(PagesInfo.LanguageId.ToString()))
            //        {
            //            blogpostforlayout.Language = null;
            //        }
            //        else
            //        {
            //            Language language = new Language();
            //            language.Id = new Guid(PagesInfo.LanguageId.ToString());
            //            JObject requestforlanguage = new JObject();
            //            requestforlanguage.Add("languageId", language.Id);
            //            string languageobj = JsonConvert.SerializeObject(requestforlanguage);
            //            var LanguageModel = _webClient.DownloadData<string>("Blog/GetLanguageDetails", new { Js = languageobj });
            //            dynamic LanguageInfo = JObject.Parse(LanguageModel);
            //            language.Version = LanguageInfo.Version;
            //            language.IsDeleted = LanguageInfo.IsDeleted;
            //            language.CreatedOn = LanguageInfo.CreatedOn;
            //            language.CreatedByUser = LanguageInfo.CreatedByUser;
            //            language.ModifiedOn = LanguageInfo.ModifiedOn;
            //            language.ModifiedByUser = LanguageInfo.ModifiedByUser;
            //            language.DeletedOn = (!string.IsNullOrEmpty(LanguageInfo.DeletedOn.ToString())) ? LanguageInfo.DeletedOn : null;
            //            language.DeletedByUser = (!string.IsNullOrEmpty(LanguageInfo.DeletedByUser.ToString())) ? LanguageInfo.DeletedByUser : null;
            //            language.Name = LanguageInfo.Name;
            //            language.Code = LanguageInfo.Code;

            //            blogpostforlayout.Language = language;
            //        }
            //        blogpostforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(PagesInfo.LanguageGroupIdentifier.ToString())) ? PagesInfo.LanguageGroupIdentifier : null;
            //        blogpostforlayout.ForceAccessProtocol = PagesInfo.ForceAccessProtocol;


            //        JObject requestforpagecontent = new JObject();
            //        requestforpagecontent.Add("pageId", blogpostforlayout.Id);
            //        string pagecontentobj = JsonConvert.SerializeObject(requestforpagecontent);
            //        var pagecontentmodel = _webClient.DownloadData<JArray>("Blog/GetPageContentDetailsWithPageId", new { Js = pagecontentobj });

            //        IList<PageContent> pagecontents = new List<PageContent>();
            //        List<PageContent> pagecontentslist = new List<PageContent>();
            //        for (int i = 0; i < pagecontentmodel.Count; i++)
            //        {
            //            PageContent pagecontent = new PageContent();
            //            pagecontent.Id = new Guid(pagecontentmodel[i]["PageContentId"].ToString());
            //            pagecontent.Version = Convert.ToInt32(pagecontentmodel[i]["Version"]);
            //            pagecontent.IsDeleted = (bool)pagecontentmodel[i]["IsDeleted"];
            //            pagecontent.CreatedOn = (DateTime)pagecontentmodel[i]["CreatedOn"];
            //            pagecontent.CreatedByUser = pagecontentmodel[i]["CreatedByUser"].ToString();
            //            pagecontent.ModifiedByUser = pagecontentmodel[i]["ModifiedByUser"].ToString();
            //            pagecontent.ModifiedOn = (DateTime)pagecontentmodel[i]["ModifiedOn"];
            //            if (!string.IsNullOrEmpty(pagecontentmodel[i]["DeletedOn"].ToString()))
            //            {
            //                pagecontent.DeletedOn = (DateTime)pagecontentmodel[i]["DeletedOn"];
            //            }
            //            else
            //            {
            //                pagecontent.DeletedOn = null;
            //            }
            //            pagecontent.DeletedByUser = (!string.IsNullOrEmpty(pagecontentmodel[i]["DeletedByUser"].ToString())) ? pagecontentmodel[i]["DeletedByUser"].ToString() : null;
            //            pagecontent.Order = Convert.ToInt32(pagecontentmodel[i]["Order"]);



            //            pagecontent.Page = blogpostforlayout;

            //            //JObject requestforcontentdetails = new JObject();
            //            //requestforcontentdetails.Add("contentId", pagecontentmodel[i]["ContentId"].ToString());


            //            //string contentobj = JsonConvert.SerializeObject(requestforcontentdetails);
            //            //var contentmodel = _webClient.DownloadData<string>("Blog/GetContentDetailsForPageContents", new { Js = contentobj });

            //            //dynamic contentInfo = JObject.Parse(contentmodel);
            //            //if (contentInfo.Flag == 1)
            //            //{
            //            //    BlogPostContent blogpostcontent = new BlogPostContent();
            //            //    blogpostcontent.Id = contentInfo.ContentId;
            //            //    blogpostcontent.ActivationDate = contentInfo.ActivationDate;
            //            //    blogpostcontent.ExpirationDate = (!string.IsNullOrEmpty(contentInfo.ExpirationDate.ToString())) ? contentInfo.ExpirationDate : null;
            //            //    blogpostcontent.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //            //    blogpostcontent.UseCustomCss = contentInfo.UseCustomCss;
            //            //    blogpostcontent.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //            //    blogpostcontent.UseCustomJs = contentInfo.UseCustomJs;
            //            //    blogpostcontent.Html = contentInfo.Html;
            //            //    blogpostcontent.EditInSourceMode = contentInfo.EditInSourceMode;
            //            //    blogpostcontent.OriginalText = (!string.IsNullOrEmpty(contentInfo.OriginalText.ToString())) ? contentInfo.OriginalText : null;
            //            //    blogpostcontent.ContentTextMode = contentInfo.ContentTextMode;
            //            //    blogpostcontent.Version = contentInfo.Version;
            //            //    blogpostcontent.IsDeleted = contentInfo.IsDeleted;
            //            //    blogpostcontent.CreatedOn = contentInfo.CreatedOn;
            //            //    blogpostcontent.CreatedByUser = contentInfo.CreatedByUser;
            //            //    blogpostcontent.ModifiedOn = contentInfo.ModifiedOn;
            //            //    blogpostcontent.ModifiedByUser = contentInfo.ModifiedByUser;
            //            //    blogpostcontent.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //            //    blogpostcontent.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //            //    blogpostcontent.Name = contentInfo.Name;
            //            //    blogpostcontent.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //            //    blogpostcontent.Status = contentInfo.Status;
            //            //    blogpostcontent.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //            //    blogpostcontent.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //            //    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //            //    {
            //            //        blogpostcontent.Original.Id = contentInfo.OriginalId;
            //            //    }
            //            //    else
            //            //    {
            //            //        blogpostcontent.Original = null;

            //            //    }

            //            //    pagecontent.Content = blogpostcontent;

            //            //}
            //            //else if (contentInfo.Flag == 2)
            //            //{
            //            //    HtmlContent htmlcontent = new HtmlContent();
            //            //    htmlcontent.Id = contentInfo.ContentId;
            //            //    htmlcontent.ActivationDate = contentInfo.ActivationDate;
            //            //    htmlcontent.ExpirationDate = (!string.IsNullOrEmpty(contentInfo.ExpirationDate.ToString())) ? contentInfo.ExpirationDate : null;
            //            //    htmlcontent.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //            //    htmlcontent.UseCustomCss = contentInfo.UseCustomCss;
            //            //    htmlcontent.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //            //    htmlcontent.UseCustomJs = contentInfo.UseCustomJs;
            //            //    htmlcontent.Html = contentInfo.Html;
            //            //    htmlcontent.EditInSourceMode = contentInfo.EditInSourceMode;
            //            //    htmlcontent.OriginalText = (!string.IsNullOrEmpty(contentInfo.OriginalText.ToString())) ? contentInfo.OriginalText : null;
            //            //    htmlcontent.ContentTextMode = contentInfo.ContentTextMode;
            //            //    htmlcontent.Version = contentInfo.Version;
            //            //    htmlcontent.IsDeleted = contentInfo.IsDeleted;
            //            //    htmlcontent.CreatedOn = contentInfo.CreatedOn;
            //            //    htmlcontent.CreatedByUser = contentInfo.CreatedByUser;
            //            //    htmlcontent.ModifiedOn = contentInfo.ModifiedOn;
            //            //    htmlcontent.ModifiedByUser = contentInfo.ModifiedByUser;
            //            //    htmlcontent.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //            //    htmlcontent.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //            //    htmlcontent.Name = contentInfo.Name;
            //            //    htmlcontent.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //            //    htmlcontent.Status = contentInfo.Status;
            //            //    htmlcontent.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //            //    htmlcontent.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //            //    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //            //    {
            //            //        htmlcontent.Original.Id = contentInfo.OriginalId;
            //            //    }
            //            //    else
            //            //    {
            //            //        htmlcontent.Original = null;

            //            //    }
            //            //    pagecontent.Content = htmlcontent;
            //            //}


            //            //else if (contentInfo.Flag == 3)
            //            //{
            //            //    ServerControlWidget servercontrolwidget = new ServerControlWidget();
            //            //    servercontrolwidget.Id = contentInfo.ContentId;
            //            //    servercontrolwidget.Url = contentInfo.Url;
            //            //    servercontrolwidget.Version = contentInfo.Version;
            //            //    servercontrolwidget.IsDeleted = contentInfo.IsDeleted;
            //            //    servercontrolwidget.CreatedOn = contentInfo.CreatedOn;
            //            //    servercontrolwidget.CreatedByUser = contentInfo.CreatedByUser;
            //            //    servercontrolwidget.ModifiedOn = contentInfo.ModifiedOn;
            //            //    servercontrolwidget.ModifiedByUser = contentInfo.ModifiedByUser;
            //            //    servercontrolwidget.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //            //    servercontrolwidget.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //            //    servercontrolwidget.Name = contentInfo.Name;
            //            //    servercontrolwidget.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //            //    servercontrolwidget.Status = contentInfo.Status;
            //            //    servercontrolwidget.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //            //    servercontrolwidget.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //            //    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //            //    {
            //            //        servercontrolwidget.Original.Id = contentInfo.OriginalId;
            //            //    }
            //            //    else
            //            //    {
            //            //        servercontrolwidget.Original = null;

            //            //    }
            //            //    pagecontent.Content = servercontrolwidget;
            //            //}
            //            //else if (contentInfo.Flag == 4)
            //            //{
            //            //    HtmlContentWidget htmlcontentwidget = new HtmlContentWidget();
            //            //    htmlcontentwidget.Id = contentInfo.ContentId;
            //            //    htmlcontentwidget.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //            //    htmlcontentwidget.UseCustomCss = contentInfo.UseCustomCss;
            //            //    htmlcontentwidget.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //            //    htmlcontentwidget.UseCustomJs = contentInfo.UseCustomJs;
            //            //    htmlcontentwidget.Html = contentInfo.Html;
            //            //    htmlcontentwidget.UseHtml = contentInfo.UseHtml;
            //            //    htmlcontentwidget.EditInSourceMode = contentInfo.EditInSourceMode;
            //            //    htmlcontentwidget.Version = contentInfo.Version;
            //            //    htmlcontentwidget.IsDeleted = contentInfo.IsDeleted;
            //            //    htmlcontentwidget.CreatedOn = contentInfo.CreatedOn;
            //            //    htmlcontentwidget.CreatedByUser = contentInfo.CreatedByUser;
            //            //    htmlcontentwidget.ModifiedOn = contentInfo.ModifiedOn;
            //            //    htmlcontentwidget.ModifiedByUser = contentInfo.ModifiedByUser;
            //            //    htmlcontentwidget.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //            //    htmlcontentwidget.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //            //    htmlcontentwidget.Name = contentInfo.Name;
            //            //    htmlcontentwidget.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //            //    htmlcontentwidget.Status = contentInfo.Status;
            //            //    htmlcontentwidget.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //            //    htmlcontentwidget.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //            //    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //            //    {
            //            //        htmlcontentwidget.Original.Id = contentInfo.OriginalId;
            //            //    }
            //            //    else
            //            //    {
            //            //        htmlcontentwidget.Original = null;

            //            //    }
            //            //    pagecontent.Content = htmlcontentwidget;
            //            //}



            //            Region region = new Region();
            //            region.Id = new Guid(pagecontentmodel[i]["RegionId"].ToString());
            //            JObject requestforregiondetails = new JObject();
            //            requestforregiondetails.Add("regionId", region.Id);
            //            string regionobj1 = JsonConvert.SerializeObject(requestforregiondetails);
            //            var regionmodel1 = _webClient.DownloadData<string>("Blog/GetRegionDetails", new { Js = regionobj1 });
            //            dynamic regionInfo1 = JObject.Parse(regionmodel1);
            //            region.Version = regionInfo1.Version;
            //            region.IsDeleted = regionInfo1.IsDeleted;
            //            region.CreatedOn = regionInfo1.CreatedOn;
            //            region.CreatedByUser = regionInfo1.CreatedByUser;
            //            region.ModifiedOn = regionInfo1.ModifiedOn;
            //            region.ModifiedByUser = regionInfo1.ModifiedByUser;
            //            region.DeletedOn = (!string.IsNullOrEmpty(regionInfo1.DeletedOn.ToString())) ? regionInfo1.DeletedOn : null;
            //            region.DeletedByUser = (!string.IsNullOrEmpty(regionInfo1.DeletedByUser.ToString())) ? regionInfo1.DeletedByUser : null;
            //            region.RegionIdentifier = regionInfo1.RegionIdentifier;
            //            JObject requestforlayoutregiondetails1 = new JObject();
            //            requestforlayoutregiondetails1.Add("regionId", region.Id);
            //            string layoutregionobj1 = JsonConvert.SerializeObject(requestforlayoutregiondetails1);
            //            var layoutregionmodel1 = _webClient.DownloadData<JArray>("Blog/GetLayoutRegionDetails", new { Js = layoutregionobj1 });

            //            IList<LayoutRegion> layoutregions1 = new List<LayoutRegion>();
            //            List<LayoutRegion> layoutregionlist1 = new List<LayoutRegion>();

            //            for (int j = 0; j < layoutregionmodel1.Count; j++)
            //            {
            //                LayoutRegion layoutregion1 = new LayoutRegion();
            //                layoutregion1.Id = new Guid(layoutregionmodel1[j]["LayoutRegionId"].ToString());
            //                layoutregion1.Version = Convert.ToInt32(layoutregionmodel1[j]["Version"]);
            //                layoutregion1.IsDeleted = (bool)layoutregionmodel1[j]["IsDeleted"];
            //                layoutregion1.CreatedOn = (DateTime)layoutregionmodel1[j]["CreatedOn"];
            //                layoutregion1.CreatedByUser = layoutregionmodel1[j]["CreatedByUser"].ToString();
            //                layoutregion1.ModifiedOn = (DateTime)layoutregionmodel1[j]["ModifiedOn"];
            //                layoutregion1.ModifiedByUser = layoutregionmodel1[j]["ModifiedByUser"].ToString();
            //                if (!string.IsNullOrEmpty(layoutregionmodel1[j]["DeletedOn"].ToString()))
            //                {

            //                    layoutregion1.DeletedOn = (DateTime)layoutregionmodel1[j]["DeletedOn"];
            //                }
            //                else
            //                {
            //                    layoutregion1.DeletedOn = null;
            //                }
            //                layoutregion1.DeletedByUser = (!string.IsNullOrEmpty(layoutregionmodel1[j]["DeletedByUser"].ToString())) ? layoutregionmodel[j]["DeletedByUser"].ToString() : null;
            //                layoutregion1.Description = (!string.IsNullOrEmpty(layoutregionmodel1[j]["Description"].ToString())) ? layoutregionmodel[j]["Description"].ToString() : null;
            //                // layoutregion1.Layout = layoutdetails;
            //                layoutregion1.Region = region;
            //                layoutregionlist1.Add(layoutregion1);
            //            }
            //            layoutregions1 = layoutregionlist1;
            //            region.LayoutRegion = layoutregions1;
            //            pagecontent.Region = region;

            //            pagecontentslist.Add(pagecontent);
            //        }
            //        pagecontents = pagecontentslist;
            //        blogpostforlayout.PageContents = pagecontents;

            //        pageslist.Add(blogpostforlayout);
            //    }
            //    else if (PagesInfo.Flag == 2)
            //    {
            //        PageProperties pagepropertiesforlayout = new PageProperties();
            //        pagepropertiesforlayout.Id = pageId;
            //        pagepropertiesforlayout.Description = PagesInfo.Description;
            //        if (string.IsNullOrEmpty(PagesInfo.ImageId.ToString()))
            //        {
            //            pagepropertiesforlayout.Image = null;
            //        }
            //        else
            //        {
            //            MediaImage image = new MediaImage();
            //            image.Id = new Guid(PagesInfo.ImageId.ToString());
            //            JObject requestforimagedetails = new JObject();
            //            requestforimagedetails.Add("imageId", image.Id);
            //            string imageobj = JsonConvert.SerializeObject(requestforimagedetails);
            //            var ImageModel = _webClient.DownloadData<string>("Blog/GetImageDetails", new { Js = imageobj });
            //            dynamic ImageInfo = JObject.Parse(ImageModel);
            //            image.Caption = (!string.IsNullOrEmpty(ImageInfo.Caption.ToString())) ? ImageInfo.Caption : null;
            //            image.ImageAlign = (!string.IsNullOrEmpty(ImageInfo.ImageAlign.ToString())) ? ImageInfo.ImageAlign : null;
            //            image.Width = ImageInfo.Width;
            //            image.Height = ImageInfo.Height;
            //            image.CropCoordX1 = (!string.IsNullOrEmpty(ImageInfo.CropCoordX1.ToString())) ? ImageInfo.CropCoordX1 : null;
            //            image.CropCoordX2 = (!string.IsNullOrEmpty(ImageInfo.CropCoordX2.ToString())) ? ImageInfo.CropCoordX2 : null;
            //            image.CropCoordY1 = (!string.IsNullOrEmpty(ImageInfo.CropCoordY1.ToString())) ? ImageInfo.CropCoordY1 : null;
            //            image.CropCoordY2 = (!string.IsNullOrEmpty(ImageInfo.CropCoordY2.ToString())) ? ImageInfo.CropCoordY2 : null;
            //            image.OriginalWidth = ImageInfo.OriginalWidth;
            //            image.OriginalHeight = ImageInfo.OriginalHeight;
            //            image.OriginalSize = ImageInfo.OriginalSize;
            //            image.OriginalUri = ImageInfo.OriginalUri;
            //            image.PublicOriginallUrl = ImageInfo.PublicOriginallUrl;
            //            image.IsOriginalUploaded = (!string.IsNullOrEmpty(ImageInfo.IsOriginalUploaded.ToString())) ? ImageInfo.IsOriginalUploaded : null;
            //            image.ThumbnailWidth = ImageInfo.ThumbnailWidth;
            //            image.ThumbnailHeight = ImageInfo.ThumbnailHeight;
            //            image.ThumbnailSize = ImageInfo.ThumbnailSize;
            //            image.ThumbnailUri = ImageInfo.ThumbnailUri;
            //            image.PublicThumbnailUrl = ImageInfo.PublicThumbnailUrl;
            //            image.IsThumbnailUploaded = (!string.IsNullOrEmpty(ImageInfo.IsThumbnailUploaded.ToString())) ? ImageInfo.IsThumbnailUploaded : null;
            //            image.Version = ImageInfo.Version;
            //            image.IsDeleted = ImageInfo.IsDeleted;
            //            image.CreatedOn = ImageInfo.CreatedOn;
            //            image.CreatedByUser = ImageInfo.CreatedByUser;
            //            image.ModifiedOn = ImageInfo.ModifiedOn;
            //            image.ModifiedByUser = ImageInfo.ModifiedByUser;
            //            image.DeletedOn = (!string.IsNullOrEmpty(ImageInfo.DeletedOn.ToString())) ? ImageInfo.DeletedOn : null;
            //            image.DeletedByUser = (!string.IsNullOrEmpty(ImageInfo.DeletedByUser.ToString())) ? ImageInfo.DeletedByUser : null;
            //            if (!string.IsNullOrEmpty(ImageInfo.FolderId.ToString()))
            //            {
            //                MediaFolder folder = new MediaFolder();
            //                folder.Id = ImageInfo.FolderId;
            //                image.Folder = folder;
            //            }
            //            else
            //            {
            //                image.Folder = null;
            //            }
            //            image.Title = ImageInfo.Title;
            //            image.Type = ImageInfo.Type;
            //            image.ContentType = ImageInfo.ContentType;
            //            image.IsArchived = ImageInfo.IsArchived;
            //            if (!string.IsNullOrEmpty(ImageInfo.FolderId.ToString()))
            //            {
            //                image.Original.Id = ImageInfo.OriginalId;
            //            }
            //            else
            //            {
            //                image.Original = null;

            //            }

            //            image.PublishedOn = ImageInfo.PublishedOn;
            //            //image.Image.Id = ImageInfo.ImageId;
            //            image.Description = (!string.IsNullOrEmpty(ImageInfo.Description.ToString())) ? ImageInfo.Description : null;
            //            pagepropertiesforlayout.Image = image;
            //        }

            //        pagepropertiesforlayout.CustomCss = (!string.IsNullOrEmpty(PagesInfo.CustomCss.ToString())) ? PagesInfo.CustomCss : null;
            //        pagepropertiesforlayout.CustomJS = (!string.IsNullOrEmpty(PagesInfo.CustomJS.ToString())) ? PagesInfo.CustomJS : null;
            //        pagepropertiesforlayout.UseCanonicalUrl = PagesInfo.UseCanonicalUrl;
            //        pagepropertiesforlayout.UseNoFollow = PagesInfo.UseNoFollow;
            //        pagepropertiesforlayout.UseNoIndex = PagesInfo.UseNoIndex;

            //        if (string.IsNullOrEmpty(PagesInfo.SecondaryImageId.ToString()))
            //        {
            //            pagepropertiesforlayout.SecondaryImage = null;
            //        }
            //        else
            //        {
            //            MediaImage secondaryimage = new MediaImage();
            //            secondaryimage.Id = new Guid(PagesInfo.SecondaryImageId.ToString());
            //            pagepropertiesforlayout.SecondaryImage = secondaryimage;
            //        }
            //        if (string.IsNullOrEmpty(PagesInfo.FeaturedImageId.ToString()))
            //        {
            //            pagepropertiesforlayout.FeaturedImage = null;
            //        }
            //        else
            //        {
            //            MediaImage featuredimage = new MediaImage();
            //            featuredimage.Id = new Guid(PagesInfo.FeaturedImageId.ToString());
            //            pagepropertiesforlayout.FeaturedImage = featuredimage;
            //        }

            //        pagepropertiesforlayout.IsArchived = PagesInfo.IsArchived;
            //        pagepropertiesforlayout.Version = PagesInfo.Version;
            //        pagepropertiesforlayout.PageUrl = PagesInfo.PageUrl;
            //        pagepropertiesforlayout.Title = PagesInfo.Title;

            //        pagepropertiesforlayout.Layout = layoutdetails;
            //        pagepropertiesforlayout.MetaTitle = (!string.IsNullOrEmpty(PagesInfo.MetaTitle.ToString())) ? PagesInfo.MetaTitle : null;
            //        pagepropertiesforlayout.MetaKeywords = (!string.IsNullOrEmpty(PagesInfo.MetaKeywords.ToString())) ? PagesInfo.MetaKeywords : null;
            //        pagepropertiesforlayout.MetaDescription = (!string.IsNullOrEmpty(PagesInfo.MetaDescription.ToString())) ? PagesInfo.MetaDescription : null;
            //        pagepropertiesforlayout.Status = PagesInfo.Status;
            //        pagepropertiesforlayout.PageUrlHash = PagesInfo.PageUrlHash;

            //        if (string.IsNullOrEmpty(PagesInfo.MasterPageId.ToString()))
            //        {
            //            pagepropertiesforlayout.MasterPage = null;
            //        }
            //        else
            //        {
            //            Page masterpage = new Page();
            //            masterpage.Id = new Guid(PagesInfo.MasterPageId.ToString());
            //            pagepropertiesforlayout.MasterPage = masterpage;
            //        }
            //        pagepropertiesforlayout.IsMasterPage = PagesInfo.IsMasterPage;
            //        if (string.IsNullOrEmpty(PagesInfo.LanguageId.ToString()))
            //        {
            //            pagepropertiesforlayout.Language = null;
            //        }
            //        else
            //        {
            //            Language language = new Language();
            //            language.Id = new Guid(PagesInfo.LanguageId.ToString());
            //            JObject requestforlanguage = new JObject();
            //            requestforlanguage.Add("languageId", language.Id);
            //            string languageobj = JsonConvert.SerializeObject(requestforlanguage);
            //            var LanguageModel = _webClient.DownloadData<string>("Blog/GetLanguageDetails", new { Js = languageobj });
            //            dynamic LanguageInfo = JObject.Parse(LanguageModel);
            //            language.Version = LanguageInfo.Version;
            //            language.IsDeleted = LanguageInfo.IsDeleted;
            //            language.CreatedOn = LanguageInfo.CreatedOn;
            //            language.CreatedByUser = LanguageInfo.CreatedByUser;
            //            language.ModifiedOn = LanguageInfo.ModifiedOn;
            //            language.ModifiedByUser = LanguageInfo.ModifiedByUser;
            //            language.DeletedOn = (!string.IsNullOrEmpty(LanguageInfo.DeletedOn.ToString())) ? LanguageInfo.DeletedOn : null;
            //            language.DeletedByUser = (!string.IsNullOrEmpty(LanguageInfo.DeletedByUser.ToString())) ? LanguageInfo.DeletedByUser : null;
            //            language.Name = LanguageInfo.Name;
            //            language.Code = LanguageInfo.Code;
            //            pagepropertiesforlayout.Language = language;
            //        }
            //        pagepropertiesforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(PagesInfo.LanguageGroupIdentifier.ToString())) ? PagesInfo.LanguageGroupIdentifier : null;
            //        pagepropertiesforlayout.ForceAccessProtocol = PagesInfo.ForceAccessProtocol;

            //        JObject requestforpagecontent = new JObject();
            //        requestforpagecontent.Add("pageId", pagepropertiesforlayout.Id);
            //        string pagecontentobj = JsonConvert.SerializeObject(requestforpagecontent);
            //        var pagecontentmodel = _webClient.DownloadData<JArray>("Blog/GetPageContentDetailsWithPageId", new { Js = pagecontentobj });

            //        IList<PageContent> pagecontents = new List<PageContent>();
            //        List<PageContent> pagecontentslist = new List<PageContent>();
            //        for (int i = 0; i < pagecontentmodel.Count; i++)
            //        {
            //            PageContent pagecontent = new PageContent();
            //            pagecontent.Id = new Guid(pagecontentmodel[i]["PageContentId"].ToString());
            //            pagecontent.Version = Convert.ToInt32(pagecontentmodel[i]["Version"]);
            //            pagecontent.IsDeleted = (bool)pagecontentmodel[i]["IsDeleted"];
            //            pagecontent.CreatedOn = (DateTime)pagecontentmodel[i]["CreatedOn"];
            //            pagecontent.CreatedByUser = pagecontentmodel[i]["CreatedByUser"].ToString();
            //            pagecontent.ModifiedByUser = pagecontentmodel[i]["ModifiedByUser"].ToString();
            //            pagecontent.ModifiedOn = (DateTime)pagecontentmodel[i]["ModifiedOn"];
            //            if (!string.IsNullOrEmpty(pagecontentmodel[i]["DeletedOn"].ToString()))
            //            {
            //                pagecontent.DeletedOn = (DateTime)pagecontentmodel[i]["DeletedOn"];
            //            }
            //            else
            //            {
            //                pagecontent.DeletedOn = null;
            //            }
            //            pagecontent.DeletedByUser = (!string.IsNullOrEmpty(pagecontentmodel[i]["DeletedByUser"].ToString())) ? pagecontentmodel[i]["DeletedByUser"].ToString() : null;
            //            pagecontent.Order = Convert.ToInt32(pagecontentmodel[i]["Order"]);



            //            pagecontent.Page = pagepropertiesforlayout;

            //            //JObject requestforcontentdetails = new JObject();
            //            //requestforcontentdetails.Add("contentId", pagecontentmodel[i]["ContentId"].ToString());


            //            //string contentobj = JsonConvert.SerializeObject(requestforcontentdetails);
            //            //var contentmodel = _webClient.DownloadData<string>("Blog/GetContentDetailsForPageContents", new { Js = contentobj });

            //            //dynamic contentInfo = JObject.Parse(contentmodel);
            //            //if (contentInfo.Flag == 1)
            //            //{
            //            //    BlogPostContent blogpostcontent = new BlogPostContent();
            //            //    blogpostcontent.Id = contentInfo.ContentId;
            //            //    blogpostcontent.ActivationDate = contentInfo.ActivationDate;
            //            //    blogpostcontent.ExpirationDate = (!string.IsNullOrEmpty(contentInfo.ExpirationDate.ToString())) ? contentInfo.ExpirationDate : null;
            //            //    blogpostcontent.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //            //    blogpostcontent.UseCustomCss = contentInfo.UseCustomCss;
            //            //    blogpostcontent.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //            //    blogpostcontent.UseCustomJs = contentInfo.UseCustomJs;
            //            //    blogpostcontent.Html = contentInfo.Html;
            //            //    blogpostcontent.EditInSourceMode = contentInfo.EditInSourceMode;
            //            //    blogpostcontent.OriginalText = (!string.IsNullOrEmpty(contentInfo.OriginalText.ToString())) ? contentInfo.OriginalText : null;
            //            //    blogpostcontent.ContentTextMode = contentInfo.ContentTextMode;
            //            //    blogpostcontent.Version = contentInfo.Version;
            //            //    blogpostcontent.IsDeleted = contentInfo.IsDeleted;
            //            //    blogpostcontent.CreatedOn = contentInfo.CreatedOn;
            //            //    blogpostcontent.CreatedByUser = contentInfo.CreatedByUser;
            //            //    blogpostcontent.ModifiedOn = contentInfo.ModifiedOn;
            //            //    blogpostcontent.ModifiedByUser = contentInfo.ModifiedByUser;
            //            //    blogpostcontent.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //            //    blogpostcontent.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //            //    blogpostcontent.Name = contentInfo.Name;
            //            //    blogpostcontent.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //            //    blogpostcontent.Status = contentInfo.Status;
            //            //    blogpostcontent.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //            //    blogpostcontent.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //            //    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //            //    {
            //            //        blogpostcontent.Original.Id = contentInfo.OriginalId;
            //            //    }
            //            //    else
            //            //    {
            //            //        blogpostcontent.Original = null;

            //            //    }

            //            //    pagecontent.Content = blogpostcontent;

            //            //}
            //            //else if (contentInfo.Flag == 2)
            //            //{
            //            //    HtmlContent htmlcontent = new HtmlContent();
            //            //    htmlcontent.Id = contentInfo.ContentId;
            //            //    htmlcontent.ActivationDate = contentInfo.ActivationDate;
            //            //    htmlcontent.ExpirationDate = (!string.IsNullOrEmpty(contentInfo.ExpirationDate.ToString())) ? contentInfo.ExpirationDate : null;
            //            //    htmlcontent.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //            //    htmlcontent.UseCustomCss = contentInfo.UseCustomCss;
            //            //    htmlcontent.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //            //    htmlcontent.UseCustomJs = contentInfo.UseCustomJs;
            //            //    htmlcontent.Html = contentInfo.Html;
            //            //    htmlcontent.EditInSourceMode = contentInfo.EditInSourceMode;
            //            //    htmlcontent.OriginalText = (!string.IsNullOrEmpty(contentInfo.OriginalText.ToString())) ? contentInfo.OriginalText : null;
            //            //    htmlcontent.ContentTextMode = contentInfo.ContentTextMode;
            //            //    htmlcontent.Version = contentInfo.Version;
            //            //    htmlcontent.IsDeleted = contentInfo.IsDeleted;
            //            //    htmlcontent.CreatedOn = contentInfo.CreatedOn;
            //            //    htmlcontent.CreatedByUser = contentInfo.CreatedByUser;
            //            //    htmlcontent.ModifiedOn = contentInfo.ModifiedOn;
            //            //    htmlcontent.ModifiedByUser = contentInfo.ModifiedByUser;
            //            //    htmlcontent.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //            //    htmlcontent.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //            //    htmlcontent.Name = contentInfo.Name;
            //            //    htmlcontent.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //            //    htmlcontent.Status = contentInfo.Status;
            //            //    htmlcontent.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //            //    htmlcontent.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //            //    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //            //    {
            //            //        htmlcontent.Original.Id = contentInfo.OriginalId;
            //            //    }
            //            //    else
            //            //    {
            //            //        htmlcontent.Original = null;

            //            //    }
            //            //    pagecontent.Content = htmlcontent;
            //            //}


            //            //else if (contentInfo.Flag == 3)
            //            //{
            //            //    ServerControlWidget servercontrolwidget = new ServerControlWidget();
            //            //    servercontrolwidget.Id = contentInfo.ContentId;
            //            //    servercontrolwidget.Url = contentInfo.Url;
            //            //    servercontrolwidget.Version = contentInfo.Version;
            //            //    servercontrolwidget.IsDeleted = contentInfo.IsDeleted;
            //            //    servercontrolwidget.CreatedOn = contentInfo.CreatedOn;
            //            //    servercontrolwidget.CreatedByUser = contentInfo.CreatedByUser;
            //            //    servercontrolwidget.ModifiedOn = contentInfo.ModifiedOn;
            //            //    servercontrolwidget.ModifiedByUser = contentInfo.ModifiedByUser;
            //            //    servercontrolwidget.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //            //    servercontrolwidget.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //            //    servercontrolwidget.Name = contentInfo.Name;
            //            //    servercontrolwidget.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //            //    servercontrolwidget.Status = contentInfo.Status;
            //            //    servercontrolwidget.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //            //    servercontrolwidget.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //            //    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //            //    {
            //            //        servercontrolwidget.Original.Id = contentInfo.OriginalId;
            //            //    }
            //            //    else
            //            //    {
            //            //        servercontrolwidget.Original = null;

            //            //    }
            //            //    pagecontent.Content = servercontrolwidget;
            //            //}
            //            //else if (contentInfo.Flag == 4)
            //            //{
            //            //    HtmlContentWidget htmlcontentwidget = new HtmlContentWidget();
            //            //    htmlcontentwidget.Id = contentInfo.ContentId;
            //            //    htmlcontentwidget.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //            //    htmlcontentwidget.UseCustomCss = contentInfo.UseCustomCss;
            //            //    htmlcontentwidget.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //            //    htmlcontentwidget.UseCustomJs = contentInfo.UseCustomJs;
            //            //    htmlcontentwidget.Html = contentInfo.Html;
            //            //    htmlcontentwidget.UseHtml = contentInfo.UseHtml;
            //            //    htmlcontentwidget.EditInSourceMode = contentInfo.EditInSourceMode;
            //            //    htmlcontentwidget.Version = contentInfo.Version;
            //            //    htmlcontentwidget.IsDeleted = contentInfo.IsDeleted;
            //            //    htmlcontentwidget.CreatedOn = contentInfo.CreatedOn;
            //            //    htmlcontentwidget.CreatedByUser = contentInfo.CreatedByUser;
            //            //    htmlcontentwidget.ModifiedOn = contentInfo.ModifiedOn;
            //            //    htmlcontentwidget.ModifiedByUser = contentInfo.ModifiedByUser;
            //            //    htmlcontentwidget.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //            //    htmlcontentwidget.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //            //    htmlcontentwidget.Name = contentInfo.Name;
            //            //    htmlcontentwidget.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //            //    htmlcontentwidget.Status = contentInfo.Status;
            //            //    htmlcontentwidget.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //            //    htmlcontentwidget.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //            //    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //            //    {
            //            //        htmlcontentwidget.Original.Id = contentInfo.OriginalId;
            //            //    }
            //            //    else
            //            //    {
            //            //        htmlcontentwidget.Original = null;

            //            //    }
            //            //    pagecontent.Content = htmlcontentwidget;
            //            //}



            //            Region region = new Region();
            //            region.Id = new Guid(pagecontentmodel[i]["RegionId"].ToString());
            //            JObject requestforregiondetails = new JObject();
            //            requestforregiondetails.Add("regionId", region.Id);
            //            string regionobj1 = JsonConvert.SerializeObject(requestforregiondetails);
            //            var regionmodel1 = _webClient.DownloadData<string>("Blog/GetRegionDetails", new { Js = regionobj1 });
            //            dynamic regionInfo1 = JObject.Parse(regionmodel1);
            //            region.Version = regionInfo1.Version;
            //            region.IsDeleted = regionInfo1.IsDeleted;
            //            region.CreatedOn = regionInfo1.CreatedOn;
            //            region.CreatedByUser = regionInfo1.CreatedByUser;
            //            region.ModifiedOn = regionInfo1.ModifiedOn;
            //            region.ModifiedByUser = regionInfo1.ModifiedByUser;
            //            region.DeletedOn = (!string.IsNullOrEmpty(regionInfo1.DeletedOn.ToString())) ? regionInfo1.DeletedOn : null;
            //            region.DeletedByUser = (!string.IsNullOrEmpty(regionInfo1.DeletedByUser.ToString())) ? regionInfo1.DeletedByUser : null;
            //            region.RegionIdentifier = regionInfo1.RegionIdentifier;
            //            JObject requestforlayoutregiondetails1 = new JObject();
            //            requestforlayoutregiondetails1.Add("regionId", region.Id);
            //            string layoutregionobj1 = JsonConvert.SerializeObject(requestforlayoutregiondetails1);
            //            var layoutregionmodel1 = _webClient.DownloadData<JArray>("Blog/GetLayoutRegionDetails", new { Js = layoutregionobj1 });

            //            IList<LayoutRegion> layoutregions1 = new List<LayoutRegion>();
            //            List<LayoutRegion> layoutregionlist1 = new List<LayoutRegion>();

            //            for (int j = 0; j < layoutregionmodel1.Count; j++)
            //            {
            //                LayoutRegion layoutregion1 = new LayoutRegion();
            //                layoutregion1.Id = new Guid(layoutregionmodel1[j]["LayoutRegionId"].ToString());
            //                layoutregion1.Version = Convert.ToInt32(layoutregionmodel1[j]["Version"]);
            //                layoutregion1.IsDeleted = (bool)layoutregionmodel1[j]["IsDeleted"];
            //                layoutregion1.CreatedOn = (DateTime)layoutregionmodel1[j]["CreatedOn"];
            //                layoutregion1.CreatedByUser = layoutregionmodel1[j]["CreatedByUser"].ToString();
            //                layoutregion1.ModifiedOn = (DateTime)layoutregionmodel1[j]["ModifiedOn"];
            //                layoutregion1.ModifiedByUser = layoutregionmodel1[j]["ModifiedByUser"].ToString();
            //                if (!string.IsNullOrEmpty(layoutregionmodel1[j]["DeletedOn"].ToString()))
            //                {

            //                    layoutregion1.DeletedOn = (DateTime)layoutregionmodel1[j]["DeletedOn"];
            //                }
            //                else
            //                {
            //                    layoutregion1.DeletedOn = null;
            //                }
            //                layoutregion1.DeletedByUser = (!string.IsNullOrEmpty(layoutregionmodel1[j]["DeletedByUser"].ToString())) ? layoutregionmodel[j]["DeletedByUser"].ToString() : null;
            //                layoutregion1.Description = (!string.IsNullOrEmpty(layoutregionmodel1[j]["Description"].ToString())) ? layoutregionmodel[j]["Description"].ToString() : null;
            //                // layoutregion1.Layout = layoutdetails;
            //                layoutregion1.Region = region;
            //                layoutregionlist1.Add(layoutregion1);
            //            }
            //            layoutregions1 = layoutregionlist1;
            //            region.LayoutRegion = layoutregions1;
            //            pagecontent.Region = region;

            //            pagecontentslist.Add(pagecontent);
            //        }
            //        pagecontents = pagecontentslist;
            //        pagepropertiesforlayout.PageContents = pagecontents;

            //        pageslist.Add(pagepropertiesforlayout);
            //    }
            //    else if (PagesInfo.Flag == 3)
            //    {
            //        Page pageforlayout = new Page();
            //        pageforlayout.Id = pageId;
            //        pageforlayout.Version = PagesInfo.Version;
            //        pageforlayout.PageUrl = PagesInfo.PageUrl;
            //        pageforlayout.Title = PagesInfo.Title;

            //        pageforlayout.Layout = layoutdetails;
            //        pageforlayout.MetaTitle = (!string.IsNullOrEmpty(PagesInfo.MetaTitle.ToString())) ? PagesInfo.MetaTitle : null;
            //        pageforlayout.MetaKeywords = (!string.IsNullOrEmpty(PagesInfo.MetaKeywords.ToString())) ? PagesInfo.MetaKeywords : null;
            //        pageforlayout.MetaDescription = (!string.IsNullOrEmpty(PagesInfo.MetaDescription.ToString())) ? PagesInfo.MetaDescription : null;
            //        pageforlayout.Status = PagesInfo.Status;
            //        pageforlayout.PageUrlHash = PagesInfo.PageUrlHash;

            //        if (string.IsNullOrEmpty(PagesInfo.MasterPageId.ToString()))
            //        {
            //            pageforlayout.MasterPage = null;
            //        }
            //        else
            //        {
            //            Page masterpage = new Page();
            //            masterpage.Id = new Guid(PagesInfo.MasterPageId.ToString());
            //            pageforlayout.MasterPage = masterpage;
            //        }
            //        pageforlayout.IsMasterPage = PagesInfo.IsMasterPage;
            //        if (string.IsNullOrEmpty(PagesInfo.LanguageId.ToString()))
            //        {
            //            pageforlayout.Language = null;
            //        }
            //        else
            //        {
            //            Language language = new Language();
            //            language.Id = new Guid(PagesInfo.LanguageId.ToString());
            //            JObject requestforlanguage = new JObject();
            //            requestforlanguage.Add("languageId", language.Id);
            //            string languageobj = JsonConvert.SerializeObject(requestforlanguage);
            //            var LanguageModel = _webClient.DownloadData<string>("Blog/GetLanguageDetails", new { Js = languageobj });
            //            dynamic LanguageInfo = JObject.Parse(LanguageModel);
            //            language.Version = LanguageInfo.Version;
            //            language.IsDeleted = LanguageInfo.IsDeleted;
            //            language.CreatedOn = LanguageInfo.CreatedOn;
            //            language.CreatedByUser = LanguageInfo.CreatedByUser;
            //            language.ModifiedOn = LanguageInfo.ModifiedOn;
            //            language.ModifiedByUser = LanguageInfo.ModifiedByUser;
            //            language.DeletedOn = (!string.IsNullOrEmpty(LanguageInfo.DeletedOn.ToString())) ? LanguageInfo.DeletedOn : null;
            //            language.DeletedByUser = LanguageInfo.DeletedByUser;
            //            language.Name = LanguageInfo.Name;
            //            language.Code = LanguageInfo.Code;
            //            pageforlayout.Language = language;
            //        }
            //        pageforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(PagesInfo.LanguageGroupIdentifier.ToString())) ? PagesInfo.LanguageGroupIdentifier : null;
            //        pageforlayout.ForceAccessProtocol = PagesInfo.ForceAccessProtocol;


            //        JObject requestforpagecontent = new JObject();
            //        requestforpagecontent.Add("pageId", pageforlayout.Id);
            //        string pagecontentobj = JsonConvert.SerializeObject(requestforpagecontent);
            //        var pagecontentmodel = _webClient.DownloadData<JArray>("Blog/GetPageContentDetailsWithPageId", new { Js = pagecontentobj });

            //        IList<PageContent> pagecontents = new List<PageContent>();
            //        List<PageContent> pagecontentslist = new List<PageContent>();
            //        for (int i = 0; i < pagecontentmodel.Count; i++)
            //        {
            //            PageContent pagecontent = new PageContent();
            //            pagecontent.Id = new Guid(pagecontentmodel[i]["PageContentId"].ToString());
            //            pagecontent.Version = Convert.ToInt32(pagecontentmodel[i]["Version"]);
            //            pagecontent.IsDeleted = (bool)pagecontentmodel[i]["IsDeleted"];
            //            pagecontent.CreatedOn = (DateTime)pagecontentmodel[i]["CreatedOn"];
            //            pagecontent.CreatedByUser = pagecontentmodel[i]["CreatedByUser"].ToString();
            //            pagecontent.ModifiedByUser = pagecontentmodel[i]["ModifiedByUser"].ToString();
            //            pagecontent.ModifiedOn = (DateTime)pagecontentmodel[i]["ModifiedOn"];
            //            if (!string.IsNullOrEmpty(pagecontentmodel[i]["DeletedOn"].ToString()))
            //            {
            //                pagecontent.DeletedOn = (DateTime)pagecontentmodel[i]["DeletedOn"];
            //            }
            //            else
            //            {
            //                pagecontent.DeletedOn = null;
            //            }
            //            pagecontent.DeletedByUser = (!string.IsNullOrEmpty(pagecontentmodel[i]["DeletedByUser"].ToString())) ? pagecontentmodel[i]["DeletedByUser"].ToString() : null;
            //            pagecontent.Order = Convert.ToInt32(pagecontentmodel[i]["Order"]);



            //            pagecontent.Page = pageforlayout;

            //            //JObject requestforcontentdetails = new JObject();
            //            //requestforcontentdetails.Add("contentId", pagecontentmodel[i]["ContentId"].ToString());


            //            //string contentobj = JsonConvert.SerializeObject(requestforcontentdetails);
            //            //var contentmodel = _webClient.DownloadData<string>("Blog/GetContentDetailsForPageContents", new { Js = contentobj });

            //            //dynamic contentInfo = JObject.Parse(contentmodel);
            //            //if (contentInfo.Flag == 1)
            //            //{
            //            //    BlogPostContent blogpostcontent = new BlogPostContent();
            //            //    blogpostcontent.Id = contentInfo.ContentId;
            //            //    blogpostcontent.ActivationDate = contentInfo.ActivationDate;
            //            //    blogpostcontent.ExpirationDate = (!string.IsNullOrEmpty(contentInfo.ExpirationDate.ToString())) ? contentInfo.ExpirationDate : null;
            //            //    blogpostcontent.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //            //    blogpostcontent.UseCustomCss = contentInfo.UseCustomCss;
            //            //    blogpostcontent.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //            //    blogpostcontent.UseCustomJs = contentInfo.UseCustomJs;
            //            //    blogpostcontent.Html = contentInfo.Html;
            //            //    blogpostcontent.EditInSourceMode = contentInfo.EditInSourceMode;
            //            //    blogpostcontent.OriginalText = (!string.IsNullOrEmpty(contentInfo.OriginalText.ToString())) ? contentInfo.OriginalText : null;
            //            //    blogpostcontent.ContentTextMode = contentInfo.ContentTextMode;
            //            //    blogpostcontent.Version = contentInfo.Version;
            //            //    blogpostcontent.IsDeleted = contentInfo.IsDeleted;
            //            //    blogpostcontent.CreatedOn = contentInfo.CreatedOn;
            //            //    blogpostcontent.CreatedByUser = contentInfo.CreatedByUser;
            //            //    blogpostcontent.ModifiedOn = contentInfo.ModifiedOn;
            //            //    blogpostcontent.ModifiedByUser = contentInfo.ModifiedByUser;
            //            //    blogpostcontent.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //            //    blogpostcontent.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //            //    blogpostcontent.Name = contentInfo.Name;
            //            //    blogpostcontent.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //            //    blogpostcontent.Status = contentInfo.Status;
            //            //    blogpostcontent.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //            //    blogpostcontent.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //            //    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //            //    {
            //            //        blogpostcontent.Original.Id = contentInfo.OriginalId;
            //            //    }
            //            //    else
            //            //    {
            //            //        blogpostcontent.Original = null;

            //            //    }

            //            //    pagecontent.Content = blogpostcontent;

            //            //}
            //            //else if (contentInfo.Flag == 2)
            //            //{
            //            //    HtmlContent htmlcontent = new HtmlContent();
            //            //    htmlcontent.Id = contentInfo.ContentId;
            //            //    htmlcontent.ActivationDate = contentInfo.ActivationDate;
            //            //    htmlcontent.ExpirationDate = (!string.IsNullOrEmpty(contentInfo.ExpirationDate.ToString())) ? contentInfo.ExpirationDate : null;
            //            //    htmlcontent.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //            //    htmlcontent.UseCustomCss = contentInfo.UseCustomCss;
            //            //    htmlcontent.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //            //    htmlcontent.UseCustomJs = contentInfo.UseCustomJs;
            //            //    htmlcontent.Html = contentInfo.Html;
            //            //    htmlcontent.EditInSourceMode = contentInfo.EditInSourceMode;
            //            //    htmlcontent.OriginalText = (!string.IsNullOrEmpty(contentInfo.OriginalText.ToString())) ? contentInfo.OriginalText : null;
            //            //    htmlcontent.ContentTextMode = contentInfo.ContentTextMode;
            //            //    htmlcontent.Version = contentInfo.Version;
            //            //    htmlcontent.IsDeleted = contentInfo.IsDeleted;
            //            //    htmlcontent.CreatedOn = contentInfo.CreatedOn;
            //            //    htmlcontent.CreatedByUser = contentInfo.CreatedByUser;
            //            //    htmlcontent.ModifiedOn = contentInfo.ModifiedOn;
            //            //    htmlcontent.ModifiedByUser = contentInfo.ModifiedByUser;
            //            //    htmlcontent.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //            //    htmlcontent.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //            //    htmlcontent.Name = contentInfo.Name;
            //            //    htmlcontent.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //            //    htmlcontent.Status = contentInfo.Status;
            //            //    htmlcontent.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //            //    htmlcontent.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //            //    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //            //    {
            //            //        htmlcontent.Original.Id = contentInfo.OriginalId;
            //            //    }
            //            //    else
            //            //    {
            //            //        htmlcontent.Original = null;

            //            //    }
            //            //    pagecontent.Content = htmlcontent;
            //            //}


            //            //else if (contentInfo.Flag == 3)
            //            //{
            //            //    ServerControlWidget servercontrolwidget = new ServerControlWidget();
            //            //    servercontrolwidget.Id = contentInfo.ContentId;
            //            //    servercontrolwidget.Url = contentInfo.Url;
            //            //    servercontrolwidget.Version = contentInfo.Version;
            //            //    servercontrolwidget.IsDeleted = contentInfo.IsDeleted;
            //            //    servercontrolwidget.CreatedOn = contentInfo.CreatedOn;
            //            //    servercontrolwidget.CreatedByUser = contentInfo.CreatedByUser;
            //            //    servercontrolwidget.ModifiedOn = contentInfo.ModifiedOn;
            //            //    servercontrolwidget.ModifiedByUser = contentInfo.ModifiedByUser;
            //            //    servercontrolwidget.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //            //    servercontrolwidget.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //            //    servercontrolwidget.Name = contentInfo.Name;
            //            //    servercontrolwidget.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //            //    servercontrolwidget.Status = contentInfo.Status;
            //            //    servercontrolwidget.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //            //    servercontrolwidget.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //            //    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //            //    {
            //            //        servercontrolwidget.Original.Id = contentInfo.OriginalId;
            //            //    }
            //            //    else
            //            //    {
            //            //        servercontrolwidget.Original = null;

            //            //    }
            //            //    pagecontent.Content = servercontrolwidget;
            //            //}
            //            //else if (contentInfo.Flag == 4)
            //            //{
            //            //    HtmlContentWidget htmlcontentwidget = new HtmlContentWidget();
            //            //    htmlcontentwidget.Id = contentInfo.ContentId;
            //            //    htmlcontentwidget.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
            //            //    htmlcontentwidget.UseCustomCss = contentInfo.UseCustomCss;
            //            //    htmlcontentwidget.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
            //            //    htmlcontentwidget.UseCustomJs = contentInfo.UseCustomJs;
            //            //    htmlcontentwidget.Html = contentInfo.Html;
            //            //    htmlcontentwidget.UseHtml = contentInfo.UseHtml;
            //            //    htmlcontentwidget.EditInSourceMode = contentInfo.EditInSourceMode;
            //            //    htmlcontentwidget.Version = contentInfo.Version;
            //            //    htmlcontentwidget.IsDeleted = contentInfo.IsDeleted;
            //            //    htmlcontentwidget.CreatedOn = contentInfo.CreatedOn;
            //            //    htmlcontentwidget.CreatedByUser = contentInfo.CreatedByUser;
            //            //    htmlcontentwidget.ModifiedOn = contentInfo.ModifiedOn;
            //            //    htmlcontentwidget.ModifiedByUser = contentInfo.ModifiedByUser;
            //            //    htmlcontentwidget.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
            //            //    htmlcontentwidget.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
            //            //    htmlcontentwidget.Name = contentInfo.Name;
            //            //    htmlcontentwidget.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
            //            //    htmlcontentwidget.Status = contentInfo.Status;
            //            //    htmlcontentwidget.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
            //            //    htmlcontentwidget.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
            //            //    if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
            //            //    {
            //            //        htmlcontentwidget.Original.Id = contentInfo.OriginalId;
            //            //    }
            //            //    else
            //            //    {
            //            //        htmlcontentwidget.Original = null;

            //            //    }
            //            //    pagecontent.Content = htmlcontentwidget;
            //            //}



            //            Region region = new Region();
            //            region.Id = new Guid(pagecontentmodel[i]["RegionId"].ToString());
            //            JObject requestforregiondetails = new JObject();
            //            requestforregiondetails.Add("regionId", region.Id);
            //            string regionobj1 = JsonConvert.SerializeObject(requestforregiondetails);
            //            var regionmodel1 = _webClient.DownloadData<string>("Blog/GetRegionDetails", new { Js = regionobj1 });
            //            dynamic regionInfo1 = JObject.Parse(regionmodel1);
            //            region.Version = regionInfo1.Version;
            //            region.IsDeleted = regionInfo1.IsDeleted;
            //            region.CreatedOn = regionInfo1.CreatedOn;
            //            region.CreatedByUser = regionInfo1.CreatedByUser;
            //            region.ModifiedOn = regionInfo1.ModifiedOn;
            //            region.ModifiedByUser = regionInfo1.ModifiedByUser;
            //            region.DeletedOn = (!string.IsNullOrEmpty(regionInfo1.DeletedOn.ToString())) ? regionInfo1.DeletedOn : null;
            //            region.DeletedByUser = (!string.IsNullOrEmpty(regionInfo1.DeletedByUser.ToString())) ? regionInfo1.DeletedByUser : null;
            //            region.RegionIdentifier = regionInfo1.RegionIdentifier;
            //            JObject requestforlayoutregiondetails1 = new JObject();
            //            requestforlayoutregiondetails1.Add("regionId", region.Id);
            //            string layoutregionobj1 = JsonConvert.SerializeObject(requestforlayoutregiondetails1);
            //            var layoutregionmodel1 = _webClient.DownloadData<JArray>("Blog/GetLayoutRegionDetails", new { Js = layoutregionobj1 });

            //            IList<LayoutRegion> layoutregions1 = new List<LayoutRegion>();
            //            List<LayoutRegion> layoutregionlist1 = new List<LayoutRegion>();

            //            for (int j = 0; j < layoutregionmodel1.Count; j++)
            //            {
            //                LayoutRegion layoutregion1 = new LayoutRegion();
            //                layoutregion1.Id = new Guid(layoutregionmodel1[j]["LayoutRegionId"].ToString());
            //                layoutregion1.Version = Convert.ToInt32(layoutregionmodel1[j]["Version"]);
            //                layoutregion1.IsDeleted = (bool)layoutregionmodel1[j]["IsDeleted"];
            //                layoutregion1.CreatedOn = (DateTime)layoutregionmodel1[j]["CreatedOn"];
            //                layoutregion1.CreatedByUser = layoutregionmodel1[j]["CreatedByUser"].ToString();
            //                layoutregion1.ModifiedOn = (DateTime)layoutregionmodel1[j]["ModifiedOn"];
            //                layoutregion1.ModifiedByUser = layoutregionmodel1[j]["ModifiedByUser"].ToString();
            //                if (!string.IsNullOrEmpty(layoutregionmodel1[j]["DeletedOn"].ToString()))
            //                {

            //                    layoutregion1.DeletedOn = (DateTime)layoutregionmodel1[j]["DeletedOn"];
            //                }
            //                else
            //                {
            //                    layoutregion1.DeletedOn = null;
            //                }
            //                layoutregion1.DeletedByUser = (!string.IsNullOrEmpty(layoutregionmodel1[j]["DeletedByUser"].ToString())) ? layoutregionmodel[j]["DeletedByUser"].ToString() : null;
            //                layoutregion1.Description = (!string.IsNullOrEmpty(layoutregionmodel1[j]["Description"].ToString())) ? layoutregionmodel[j]["Description"].ToString() : null;
            //                // layoutregion1.Layout = layoutdetails;
            //                layoutregion1.Region = region;
            //                layoutregionlist1.Add(layoutregion1);
            //            }
            //            layoutregions1 = layoutregionlist1;
            //            region.LayoutRegion = layoutregions1;
            //            pagecontent.Region = region;

            //            pagecontentslist.Add(pagecontent);
            //        }
            //        pagecontents = pagecontentslist;
            //        pageforlayout.PageContents = pagecontents;
            //        pageslist.Add(pageforlayout);
            //    }

            //}
            //pages = pageslist;
            //layoutdetails.Pages = pages;

            //// End







            //return layoutdetails;








            return
               repository.AsQueryable<Layout>()
                          .Where(layout =>
                              layout.LayoutRegions.Count(region => !region.IsDeleted && !region.Region.IsDeleted).Equals(1)
                                || layout.LayoutRegions.Any(region => !region.IsDeleted && !region.Region.IsDeleted && region.Region.RegionIdentifier.ToLowerInvariant() == regionIdentifier))
                          .FetchMany(layout => layout.LayoutRegions)
                          .ThenFetch(l => l.Region)
                          .ToList()
                          .FirstOrDefault();
        }

        //private Layout GetLayoutDetails(Guid layoutId)
        //{
        //    JObject requestforlayoutdetails = new JObject();
        //    requestforlayoutdetails.Add("layoutId", layoutId);
        //    string layoutobj = JsonConvert.SerializeObject(requestforlayoutdetails);
        //    var layoutmodel = _webClient.DownloadData<string>("Blog/GetLayoutDetails", new { Js = layoutobj });
        //    dynamic layoutInfo = JObject.Parse(layoutmodel);
        //    Layout layoutdetails = new Layout();
        //    layoutdetails.Id = layoutId;
        //    layoutdetails.Version = layoutInfo.Version;
        //    layoutdetails.IsDeleted = layoutInfo.IsDeleted;
        //    layoutdetails.CreatedOn = layoutInfo.CreatedOn;
        //    layoutdetails.CreatedByUser = layoutInfo.CreatedByUser;
        //    layoutdetails.ModifiedOn = layoutInfo.ModifiedOn;
        //    layoutdetails.ModifiedByUser = layoutInfo.ModifiedByUser;
        //    layoutdetails.DeletedOn = (!string.IsNullOrEmpty(layoutInfo.DeletedOn.ToString())) ? layoutInfo.DeletedOn : null;
        //    layoutdetails.DeletedByUser = (!string.IsNullOrEmpty(layoutInfo.DeletedByUser.ToString())) ? layoutInfo.DeletedByUser : null;
        //    layoutdetails.Name = layoutInfo.Name;
        //    layoutdetails.LayoutPath = layoutInfo.LayoutPath;


        //    layoutdetails.PreviewUrl = (!string.IsNullOrEmpty(layoutInfo.PreviewUrl.ToString())) ? layoutInfo.PreviewUrl : null;


        //    JObject requestforlayoutregiondetails = new JObject();
        //    requestforlayoutregiondetails.Add("layoutId", layoutId);
        //    string layoutregionobj = JsonConvert.SerializeObject(requestforlayoutregiondetails);
        //    var layoutregionmodel = _webClient.DownloadData<JArray>("Blog/GetLayoutRegionDetailsForLayout", new { Js = layoutregionobj });

        //    IList<LayoutRegion> layoutregions = new List<LayoutRegion>();
        //    List<LayoutRegion> layoutregionlist = new List<LayoutRegion>();

        //    for (int i = 0; i < layoutregionmodel.Count; i++)
        //    {
        //        LayoutRegion layoutregion = new LayoutRegion();
        //        layoutregion.Id = new Guid(layoutregionmodel[i]["LayoutRegionId"].ToString());
        //        layoutregion.Version = Convert.ToInt32(layoutregionmodel[i]["Version"]);
        //        layoutregion.IsDeleted = (bool)layoutregionmodel[i]["IsDeleted"];
        //        layoutregion.CreatedOn = (DateTime)layoutregionmodel[i]["CreatedOn"];
        //        layoutregion.CreatedByUser = layoutregionmodel[i]["CreatedByUser"].ToString();
        //        layoutregion.ModifiedOn = (DateTime)layoutregionmodel[i]["ModifiedOn"];
        //        layoutregion.ModifiedByUser = layoutregionmodel[i]["ModifiedByUser"].ToString();
        //        if (!string.IsNullOrEmpty(layoutregionmodel[i]["DeletedOn"].ToString()))
        //        {

        //            layoutregion.DeletedOn = (DateTime)layoutregionmodel[i]["DeletedOn"];
        //        }
        //        else
        //        {
        //            layoutregion.DeletedOn = null;
        //        }
        //        layoutregion.DeletedByUser = (!string.IsNullOrEmpty(layoutregionmodel[i]["DeletedByUser"].ToString())) ? layoutregionmodel[i]["DeletedByUser"].ToString() : null;
        //        layoutregion.Description = (!string.IsNullOrEmpty(layoutregionmodel[i]["Description"].ToString())) ? layoutregionmodel[i]["Description"].ToString() : null;
        //        Region region = new Region();
        //        region.Id = new Guid(layoutregionmodel[i]["RegionId"].ToString());
        //        JObject requestforregiondetails = new JObject();
        //        requestforregiondetails.Add("regionId", region.Id);
        //        string regionobj1 = JsonConvert.SerializeObject(requestforregiondetails);
        //        var regionmodel1 = _webClient.DownloadData<string>("Blog/GetRegionDetails", new { Js = regionobj1 });
        //        dynamic regionInfo1 = JObject.Parse(regionmodel1);

        //        region.Version = regionInfo1.Version;
        //        region.IsDeleted = regionInfo1.IsDeleted;
        //        region.CreatedOn = regionInfo1.CreatedOn;
        //        region.CreatedByUser = regionInfo1.CreatedByUser;
        //        region.ModifiedOn = regionInfo1.ModifiedOn;
        //        region.ModifiedByUser = regionInfo1.ModifiedByUser;
        //        region.DeletedOn = (!string.IsNullOrEmpty(regionInfo1.DeletedOn.ToString())) ? regionInfo1.DeletedOn : null;
        //        region.DeletedByUser = (!string.IsNullOrEmpty(regionInfo1.DeletedByUser.ToString())) ? regionInfo1.DeletedByUser : null;
        //        region.RegionIdentifier = regionInfo1.RegionIdentifier;
        //        JObject requestforlayoutregiondetails1 = new JObject();
        //        requestforlayoutregiondetails1.Add("regionId", region.Id);
        //        string layoutregionobj1 = JsonConvert.SerializeObject(requestforlayoutregiondetails1);
        //        var layoutregionmodel1 = _webClient.DownloadData<JArray>("Blog/GetLayoutRegionDetails", new { Js = layoutregionobj1 });

        //        IList<LayoutRegion> layoutregions1 = new List<LayoutRegion>();
        //        List<LayoutRegion> layoutregionlist1 = new List<LayoutRegion>();

        //        for (int j = 0; j < layoutregionmodel1.Count; j++)
        //        {
        //            LayoutRegion layoutregion1 = new LayoutRegion();
        //            layoutregion1.Id = new Guid(layoutregionmodel1[j]["LayoutRegionId"].ToString());
        //            layoutregion1.Version = Convert.ToInt32(layoutregionmodel1[j]["Version"]);
        //            layoutregion1.IsDeleted = (bool)layoutregionmodel1[j]["IsDeleted"];
        //            layoutregion1.CreatedOn = (DateTime)layoutregionmodel1[j]["CreatedOn"];
        //            layoutregion1.CreatedByUser = layoutregionmodel1[j]["CreatedByUser"].ToString();
        //            layoutregion1.ModifiedOn = (DateTime)layoutregionmodel1[j]["ModifiedOn"];
        //            layoutregion1.ModifiedByUser = layoutregionmodel1[j]["ModifiedByUser"].ToString();
        //            if (!string.IsNullOrEmpty(layoutregionmodel1[j]["DeletedOn"].ToString()))
        //            {

        //                layoutregion1.DeletedOn = (DateTime)layoutregionmodel1[j]["DeletedOn"];
        //            }
        //            else
        //            {
        //                layoutregion1.DeletedOn = null;
        //            }
        //            layoutregion1.DeletedByUser = (!string.IsNullOrEmpty(layoutregionmodel1[j]["DeletedByUser"].ToString())) ? layoutregionmodel[j]["DeletedByUser"].ToString() : null;
        //            layoutregion1.Description = (!string.IsNullOrEmpty(layoutregionmodel1[j]["Description"].ToString())) ? layoutregionmodel[j]["Description"].ToString() : null;
        //            //layoutregion1.Layout = layoutdetails;
        //            layoutregion1.Layout = GetLayoutDetails(new Guid(layoutregionmodel1[j]["LayoutId"].ToString()));
        //            layoutregion1.Region = region;
        //            layoutregionlist1.Add(layoutregion1);
        //        }
        //        layoutregions1 = layoutregionlist1;
        //        region.LayoutRegion = layoutregions1;

        //        JObject requestforregionpagecontent = new JObject();
        //        requestforregionpagecontent.Add("regionId", region.Id);
        //        string regionpagecontentobj = JsonConvert.SerializeObject(requestforregionpagecontent);
        //        var regionpagecontentmodel = _webClient.DownloadData<JArray>("Blog/GetRegionPageContentDetails", new { Js = regionpagecontentobj });

        //        IList<PageContent> pagecontents = new List<PageContent>();
        //        List<PageContent> pagecontentslist = new List<PageContent>();
        //        for (int k = 0; k < regionpagecontentmodel.Count; k++)
        //        {
        //            PageContent regionpagecontent = new PageContent();
        //            regionpagecontent.Id = new Guid(regionpagecontentmodel[k]["PageContentId"].ToString());
        //            regionpagecontent.Version = Convert.ToInt32(regionpagecontentmodel[k]["Version"]);
        //            regionpagecontent.IsDeleted = (bool)regionpagecontentmodel[k]["IsDeleted"];
        //            regionpagecontent.CreatedOn = (DateTime)regionpagecontentmodel[k]["CreatedOn"];
        //            regionpagecontent.CreatedByUser = regionpagecontentmodel[k]["CreatedByUser"].ToString();
        //            regionpagecontent.ModifiedByUser = regionpagecontentmodel[k]["ModifiedByUser"].ToString();
        //            regionpagecontent.ModifiedOn = (DateTime)regionpagecontentmodel[k]["ModifiedOn"];
        //            if (!string.IsNullOrEmpty(regionpagecontentmodel[k]["DeletedOn"].ToString()))
        //            {
        //                regionpagecontent.DeletedOn = (DateTime)regionpagecontentmodel[k]["DeletedOn"];
        //            }
        //            else
        //            {
        //                regionpagecontent.DeletedOn = null;
        //            }
        //            regionpagecontent.DeletedByUser = (!string.IsNullOrEmpty(regionpagecontentmodel[k]["DeletedByUser"].ToString())) ? regionpagecontentmodel[k]["DeletedByUser"].ToString() : null;
        //            regionpagecontent.Order = Convert.ToInt32(regionpagecontentmodel[k]["Order"]);



        //            //        //blogpostdetails
        //            //        //start
        //            Guid pageId = new Guid(regionpagecontentmodel[k]["PageId"].ToString());
        //            JObject requestdetails = new JObject();
        //            requestdetails.Add("pageId", pageId);
        //            string jsobj = JsonConvert.SerializeObject(requestdetails);
        //            var blogPostmodel = _webClient.DownloadData<string>("Blog/GetPagesForLayout", new { Js = jsobj });
        //            dynamic Info = JObject.Parse(blogPostmodel);

        //            if (Info.Flag == 1)
        //            {
        //                BlogPost blogpostforlayout = new BlogPost();
        //                blogpostforlayout.Id = pageId;
        //                blogpostforlayout.ActivationDate = Info.ActivationDate;
        //                blogpostforlayout.ExpirationDate = (!string.IsNullOrEmpty(Info.ExpirationDate.ToString())) ? Info.ExpirationDate : null;
        //                blogpostforlayout.Description = Info.Description;
        //                if (string.IsNullOrEmpty(Info.ImageId.ToString()))
        //                {
        //                    blogpostforlayout.Image = null;
        //                }
        //                else
        //                {
        //                    MediaImage image = new MediaImage();
        //                    image.Id = new Guid(Info.ImageId.ToString());
        //                    blogpostforlayout.Image = image;
        //                }

        //                blogpostforlayout.CustomCss = (!string.IsNullOrEmpty(Info.CustomCss.ToString())) ? Info.CustomCss : null;
        //                blogpostforlayout.CustomJS = (!string.IsNullOrEmpty(Info.CustomJS.ToString())) ? Info.CustomJS : null;
        //                blogpostforlayout.UseCanonicalUrl = Info.UseCanonicalUrl;
        //                blogpostforlayout.UseNoFollow = Info.UseNoFollow;
        //                blogpostforlayout.UseNoIndex = Info.UseNoIndex;

        //                if (string.IsNullOrEmpty(Info.SecondaryImageId.ToString()))
        //                {
        //                    blogpostforlayout.SecondaryImage = null;
        //                }
        //                else
        //                {
        //                    MediaImage secondaryimage = new MediaImage();
        //                    secondaryimage.Id = new Guid(Info.SecondaryImageId.ToString());
        //                    blogpostforlayout.SecondaryImage = secondaryimage;
        //                }
        //                if (string.IsNullOrEmpty(Info.FeaturedImageId.ToString()))
        //                {
        //                    blogpostforlayout.FeaturedImage = null;
        //                }
        //                else
        //                {
        //                    MediaImage featuredimage = new MediaImage();
        //                    featuredimage.Id = new Guid(Info.FeaturedImageId.ToString());
        //                    blogpostforlayout.FeaturedImage = featuredimage;
        //                }

        //                blogpostforlayout.IsArchived = Info.IsArchived;
        //                blogpostforlayout.Version = Info.Version;
        //                blogpostforlayout.PageUrl = Info.PageUrl;
        //                blogpostforlayout.Title = Info.Title;
        //                blogpostforlayout.Layout = layoutdetails;
        //                blogpostforlayout.MetaTitle = (!string.IsNullOrEmpty(Info.MetaTitle.ToString())) ? Info.MetaTitle : null;
        //                blogpostforlayout.MetaKeywords = (!string.IsNullOrEmpty(Info.MetaKeywords.ToString())) ? Info.MetaKeywords : null;
        //                blogpostforlayout.MetaDescription = (!string.IsNullOrEmpty(Info.MetaDescription.ToString())) ? Info.MetaDescription : null;
        //                blogpostforlayout.Status = Info.Status;
        //                blogpostforlayout.PageUrlHash = Info.PageUrlHash;

        //                if (string.IsNullOrEmpty(Info.MasterPageId.ToString()))
        //                {
        //                    blogpostforlayout.MasterPage = null;
        //                }
        //                else
        //                {
        //                    Page masterpage = new Page();
        //                    masterpage.Id = new Guid(Info.MasterPageId.ToString());
        //                    blogpostforlayout.MasterPage = masterpage;
        //                }
        //                blogpostforlayout.IsMasterPage = Info.IsMasterPage;
        //                if (string.IsNullOrEmpty(Info.LanguageId.ToString()))
        //                {
        //                    blogpostforlayout.Language = null;
        //                }
        //                else
        //                {
        //                    Language language = new Language();
        //                    language.Id = new Guid(Info.LanguageId.ToString());
        //                    blogpostforlayout.Language = language;
        //                }
        //                blogpostforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(Info.LanguageGroupIdentifier.ToString())) ? Info.LanguageGroupIdentifier : null;
        //                blogpostforlayout.ForceAccessProtocol = Info.ForceAccessProtocol;


        //                regionpagecontent.Page = blogpostforlayout;
        //            }
        //            else if (Info.Flag == 2)
        //            {
        //                PageProperties pagepropertiesforlayout = new PageProperties();
        //                pagepropertiesforlayout.Id = pageId;
        //                pagepropertiesforlayout.Description = Info.Description;
        //                if (string.IsNullOrEmpty(Info.ImageId.ToString()))
        //                {
        //                    pagepropertiesforlayout.Image = null;
        //                }
        //                else
        //                {
        //                    MediaImage image = new MediaImage();
        //                    image.Id = new Guid(Info.ImageId.ToString());
        //                    pagepropertiesforlayout.Image = image;
        //                }

        //                pagepropertiesforlayout.CustomCss = (!string.IsNullOrEmpty(Info.CustomCss.ToString())) ? Info.CustomCss : null;
        //                pagepropertiesforlayout.CustomJS = (!string.IsNullOrEmpty(Info.CustomJS.ToString())) ? Info.CustomJS : null;
        //                pagepropertiesforlayout.UseCanonicalUrl = Info.UseCanonicalUrl;
        //                pagepropertiesforlayout.UseNoFollow = Info.UseNoFollow;
        //                pagepropertiesforlayout.UseNoIndex = Info.UseNoIndex;

        //                if (string.IsNullOrEmpty(Info.SecondaryImageId.ToString()))
        //                {
        //                    pagepropertiesforlayout.SecondaryImage = null;
        //                }
        //                else
        //                {
        //                    MediaImage secondaryimage = new MediaImage();
        //                    secondaryimage.Id = new Guid(Info.SecondaryImageId.ToString());
        //                    pagepropertiesforlayout.SecondaryImage = secondaryimage;
        //                }
        //                if (string.IsNullOrEmpty(Info.FeaturedImageId.ToString()))
        //                {
        //                    pagepropertiesforlayout.FeaturedImage = null;
        //                }
        //                else
        //                {
        //                    MediaImage featuredimage = new MediaImage();
        //                    featuredimage.Id = new Guid(Info.FeaturedImageId.ToString());
        //                    pagepropertiesforlayout.FeaturedImage = featuredimage;
        //                }

        //                pagepropertiesforlayout.IsArchived = Info.IsArchived;
        //                pagepropertiesforlayout.Version = Info.Version;
        //                pagepropertiesforlayout.PageUrl = Info.PageUrl;
        //                pagepropertiesforlayout.Title = Info.Title;

        //                pagepropertiesforlayout.Layout = layoutdetails;
        //                pagepropertiesforlayout.MetaTitle = (!string.IsNullOrEmpty(Info.MetaTitle.ToString())) ? Info.MetaTitle : null;
        //                pagepropertiesforlayout.MetaKeywords = (!string.IsNullOrEmpty(Info.MetaKeywords.ToString())) ? Info.MetaKeywords : null;
        //                pagepropertiesforlayout.MetaDescription = (!string.IsNullOrEmpty(Info.MetaDescription.ToString())) ? Info.MetaDescription : null;
        //                pagepropertiesforlayout.Status = Info.Status;
        //                pagepropertiesforlayout.PageUrlHash = Info.PageUrlHash;

        //                if (string.IsNullOrEmpty(Info.MasterPageId.ToString()))
        //                {
        //                    pagepropertiesforlayout.MasterPage = null;
        //                }
        //                else
        //                {
        //                    Page masterpage = new Page();
        //                    masterpage.Id = new Guid(Info.MasterPageId.ToString());
        //                    pagepropertiesforlayout.MasterPage = masterpage;
        //                }
        //                pagepropertiesforlayout.IsMasterPage = Info.IsMasterPage;
        //                if (string.IsNullOrEmpty(Info.LanguageId.ToString()))
        //                {
        //                    pagepropertiesforlayout.Language = null;
        //                }
        //                else
        //                {
        //                    Language language = new Language();
        //                    language.Id = new Guid(Info.LanguageId.ToString());
        //                    pagepropertiesforlayout.Language = language;
        //                }
        //                pagepropertiesforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(Info.LanguageGroupIdentifier.ToString())) ? Info.LanguageGroupIdentifier : null;
        //                pagepropertiesforlayout.ForceAccessProtocol = Info.ForceAccessProtocol;


        //                regionpagecontent.Page = pagepropertiesforlayout;
        //            }
        //            else if (Info.Flag == 3)
        //            {
        //                Page pageforlayout = new Page();
        //                pageforlayout.Id = pageId;
        //                pageforlayout.Version = Info.Version;
        //                pageforlayout.PageUrl = Info.PageUrl;
        //                pageforlayout.Title = Info.Title;

        //                pageforlayout.Layout = layoutdetails;
        //                pageforlayout.MetaTitle = (!string.IsNullOrEmpty(Info.MetaTitle.ToString())) ? Info.MetaTitle : null;
        //                pageforlayout.MetaKeywords = (!string.IsNullOrEmpty(Info.MetaKeywords.ToString())) ? Info.MetaKeywords : null;
        //                pageforlayout.MetaDescription = (!string.IsNullOrEmpty(Info.MetaDescription.ToString())) ? Info.MetaDescription : null;
        //                pageforlayout.Status = Info.Status;
        //                pageforlayout.PageUrlHash = Info.PageUrlHash;

        //                if (string.IsNullOrEmpty(Info.MasterPageId.ToString()))
        //                {
        //                    pageforlayout.MasterPage = null;
        //                }
        //                else
        //                {
        //                    Page masterpage = new Page();
        //                    masterpage.Id = new Guid(Info.MasterPageId.ToString());
        //                    pageforlayout.MasterPage = masterpage;
        //                }
        //                pageforlayout.IsMasterPage = Info.IsMasterPage;
        //                if (string.IsNullOrEmpty(Info.LanguageId.ToString()))
        //                {
        //                    pageforlayout.Language = null;
        //                }
        //                else
        //                {
        //                    Language language = new Language();
        //                    language.Id = new Guid(Info.LanguageId.ToString());
        //                    pageforlayout.Language = language;
        //                }
        //                pageforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(Info.LanguageGroupIdentifier.ToString())) ? Info.LanguageGroupIdentifier : null;
        //                pageforlayout.ForceAccessProtocol = Info.ForceAccessProtocol;


        //                regionpagecontent.Page = pageforlayout;
        //            }




        //            //End

        //            HtmlContentWidget regioncontentdetails = new HtmlContentWidget();
        //            //BlogPostContent regioncontentdetails= new BlogPostContent();
        //            regioncontentdetails.Id = new Guid(regionpagecontentmodel[k]["ContentId"].ToString());
        //            //blogpostcontentdetails
        //            //Start
        //            JObject requestforcontentdetails = new JObject();
        //            requestforcontentdetails.Add("contentId", regioncontentdetails.Id);


        //            string contentobj = JsonConvert.SerializeObject(requestforcontentdetails);
        //            var contentmodel = _webClient.DownloadData<string>("Blog/GetContentDetailsForLayout", new { Js = contentobj });
        //            dynamic contentInfo = JObject.Parse(contentmodel);


        //            regioncontentdetails.Name = contentInfo.Name;
        //            regioncontentdetails.IsDeleted = contentInfo.IsDeleted;
        //            regioncontentdetails.Status = contentInfo.Status;
        //            regioncontentdetails.Version = contentInfo.Version;
        //            if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
        //            {

        //                regioncontentdetails.Original.Id = new Guid(contentInfo.OriginalId.ToString());
        //            }
        //            else
        //            {
        //                regioncontentdetails.Original = null;
        //            }
        //            regioncontentdetails.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
        //            regioncontentdetails.CreatedByUser = contentInfo.CreatedByUser;
        //            regioncontentdetails.CreatedOn = contentInfo.CreatedOn;
        //            regioncontentdetails.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
        //            regioncontentdetails.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
        //            regioncontentdetails.ModifiedByUser = contentInfo.ModifiedByUser;
        //            regioncontentdetails.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
        //            regioncontentdetails.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
        //            //regioncontentdetails.ActivationDate = contentInfo.ActivationDate;
        //            //regioncontentdetails.ExpirationDate = (!string.IsNullOrEmpty(contentInfo.ExpirationDate.ToString())) ? contentInfo.ExpirationDate : null;
        //            //regioncontentdetails.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
        //            //regioncontentdetails.UseCustomCss = contentInfo.UseCustomCss;
        //            // regioncontentdetails.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
        //            // regioncontentdetails.UseCustomJs = contentInfo.UseCustomJs;
        //            //regioncontentdetails.Html = contentInfo.Html;
        //            // regioncontentdetails.EditInSourceMode = contentInfo.EditInSourceMode;
        //            //regioncontentdetails.OriginalText = (!string.IsNullOrEmpty(contentInfo.OriginalText.ToString())) ? contentInfo.OriginalText : null;
        //            //regioncontentdetails.ContentTextMode = contentInfo.ContentTextMode;









        //            //        //End


        //            regionpagecontent.Content = regioncontentdetails;


        //            pagecontentslist.Add(regionpagecontent);
        //        }
        //        pagecontents = pagecontentslist;
        //        region.PageContents = pagecontents;


        //        layoutregion.Region = region;
        //        layoutregion.Layout = layoutdetails;
        //        layoutregionlist.Add(layoutregion);

        //    }
        //    layoutregions = layoutregionlist;
        //    layoutdetails.LayoutRegions = layoutregions;

        //    //Pages details
        //    JObject requestforpageids = new JObject();
        //    requestforpageids.Add("layoutId", layoutId);
        //    string pageidsobj = JsonConvert.SerializeObject(requestforpageids);
        //    var PageIdsModel = _webClient.DownloadData<JArray>("Blog/GetPageIds", new { Js = pageidsobj });
        //    IList<Page> pages = new List<Page>();
        //    List<Page> pageslist = new List<Page>();
        //    for (int k = 0; k < PageIdsModel.Count; k++)
        //    {
        //        Guid pageId = new Guid(PageIdsModel[k]["PageId"].ToString());
        //        JObject requestforpages = new JObject();
        //        requestforpages.Add("pageId", pageId);
        //        string pagesobj = JsonConvert.SerializeObject(requestforpages);
        //        var PagesModel = _webClient.DownloadData<string>("Blog/GetPagesForLayout", new { Js = pagesobj });
        //        dynamic PagesInfo = JObject.Parse(PagesModel);
        //        if (PagesInfo.Flag == 1)
        //        {
        //            BlogPost blogpostforlayout = new BlogPost();
        //            blogpostforlayout.Id = pageId;
        //            blogpostforlayout.ActivationDate = PagesInfo.ActivationDate;
        //            blogpostforlayout.ExpirationDate = (!string.IsNullOrEmpty(PagesInfo.ExpirationDate.ToString())) ? PagesInfo.ExpirationDate : null;
        //            blogpostforlayout.Description = PagesInfo.Description;
        //            if (string.IsNullOrEmpty(PagesInfo.ImageId.ToString()))
        //            {
        //                blogpostforlayout.Image = null;
        //            }
        //            else
        //            {
        //                MediaImage image = new MediaImage();
        //                image.Id = new Guid(PagesInfo.ImageId.ToString());
        //                JObject requestforimagedetails = new JObject();
        //                requestforimagedetails.Add("imageId", image.Id);
        //                string imageobj = JsonConvert.SerializeObject(requestforimagedetails);
        //                var ImageModel = _webClient.DownloadData<string>("Blog/GetImageDetails", new { Js = imageobj });
        //                dynamic ImageInfo = JObject.Parse(ImageModel);
        //                image.Caption = (!string.IsNullOrEmpty(ImageInfo.Caption.ToString())) ? ImageInfo.Caption : null;
        //                image.ImageAlign = (!string.IsNullOrEmpty(ImageInfo.ImageAlign.ToString())) ? ImageInfo.ImageAlign : null;
        //                image.Width = ImageInfo.Width;
        //                image.Height = ImageInfo.Height;
        //                image.CropCoordX1 = (!string.IsNullOrEmpty(ImageInfo.CropCoordX1.ToString())) ? ImageInfo.CropCoordX1 : null;
        //                image.CropCoordX2 = (!string.IsNullOrEmpty(ImageInfo.CropCoordX2.ToString())) ? ImageInfo.CropCoordX2 : null;
        //                image.CropCoordY1 = (!string.IsNullOrEmpty(ImageInfo.CropCoordY1.ToString())) ? ImageInfo.CropCoordY1 : null;
        //                image.CropCoordY2 = (!string.IsNullOrEmpty(ImageInfo.CropCoordY2.ToString())) ? ImageInfo.CropCoordY2 : null;
        //                image.OriginalWidth = ImageInfo.OriginalWidth;
        //                image.OriginalHeight = ImageInfo.OriginalHeight;
        //                image.OriginalSize = ImageInfo.OriginalSize;
        //                image.OriginalUri = ImageInfo.OriginalUri;
        //                image.PublicOriginallUrl = ImageInfo.PublicOriginallUrl;
        //                image.IsOriginalUploaded = (!string.IsNullOrEmpty(ImageInfo.IsOriginalUploaded.ToString())) ? ImageInfo.IsOriginalUploaded : null;
        //                image.ThumbnailWidth = ImageInfo.ThumbnailWidth;
        //                image.ThumbnailHeight = ImageInfo.ThumbnailHeight;
        //                image.ThumbnailSize = ImageInfo.ThumbnailSize;
        //                image.ThumbnailUri = ImageInfo.ThumbnailUri;
        //                image.PublicThumbnailUrl = ImageInfo.PublicThumbnailUrl;
        //                image.IsThumbnailUploaded = (!string.IsNullOrEmpty(ImageInfo.IsThumbnailUploaded.ToString())) ? ImageInfo.IsThumbnailUploaded : null;
        //                image.Version = ImageInfo.Version;
        //                image.IsDeleted = ImageInfo.IsDeleted;
        //                image.CreatedOn = ImageInfo.CreatedOn;
        //                image.CreatedByUser = ImageInfo.CreatedByUser;
        //                image.ModifiedOn = ImageInfo.ModifiedOn;
        //                image.ModifiedByUser = ImageInfo.ModifiedByUser;
        //                image.DeletedOn = (!string.IsNullOrEmpty(ImageInfo.DeletedOn.ToString())) ? ImageInfo.DeletedOn : null;
        //                image.DeletedByUser = (!string.IsNullOrEmpty(ImageInfo.DeletedByUser.ToString())) ? ImageInfo.DeletedByUser : null;
        //                if (!string.IsNullOrEmpty(ImageInfo.FolderId.ToString()))
        //                {
        //                    MediaFolder folder = new MediaFolder();
        //                    folder.Id = ImageInfo.FolderId;
        //                    image.Folder = folder;
        //                }
        //                else
        //                {
        //                    image.Folder = null;
        //                }
        //                image.Title = ImageInfo.Title;
        //                image.Type = ImageInfo.Type;
        //                image.ContentType = ImageInfo.ContentType;
        //                image.IsArchived = ImageInfo.IsArchived;
        //                if (!string.IsNullOrEmpty(ImageInfo.FolderId.ToString()))
        //                {
        //                    image.Original.Id = ImageInfo.OriginalId;
        //                }
        //                else
        //                {
        //                    image.Original = null;

        //                }

        //                image.PublishedOn = ImageInfo.PublishedOn;
        //                //image.Image.Id = ImageInfo.ImageId;
        //                image.Description = (!string.IsNullOrEmpty(ImageInfo.Description.ToString())) ? ImageInfo.Description : null;

        //                blogpostforlayout.Image = image;
        //            }

        //            blogpostforlayout.CustomCss = (!string.IsNullOrEmpty(PagesInfo.CustomCss.ToString())) ? PagesInfo.CustomCss : null;
        //            blogpostforlayout.CustomJS = (!string.IsNullOrEmpty(PagesInfo.CustomJS.ToString())) ? PagesInfo.CustomJS : null;
        //            blogpostforlayout.UseCanonicalUrl = PagesInfo.UseCanonicalUrl;
        //            blogpostforlayout.UseNoFollow = PagesInfo.UseNoFollow;
        //            blogpostforlayout.UseNoIndex = PagesInfo.UseNoIndex;

        //            if (string.IsNullOrEmpty(PagesInfo.SecondaryImageId.ToString()))
        //            {
        //                blogpostforlayout.SecondaryImage = null;
        //            }
        //            else
        //            {
        //                MediaImage secondaryimage = new MediaImage();
        //                secondaryimage.Id = new Guid(PagesInfo.SecondaryImageId.ToString());
        //                blogpostforlayout.SecondaryImage = secondaryimage;
        //            }
        //            if (string.IsNullOrEmpty(PagesInfo.FeaturedImageId.ToString()))
        //            {
        //                blogpostforlayout.FeaturedImage = null;
        //            }
        //            else
        //            {
        //                MediaImage featuredimage = new MediaImage();
        //                featuredimage.Id = new Guid(PagesInfo.FeaturedImageId.ToString());
        //                blogpostforlayout.FeaturedImage = featuredimage;
        //            }

        //            blogpostforlayout.IsArchived = PagesInfo.IsArchived;
        //            blogpostforlayout.Version = PagesInfo.Version;
        //            blogpostforlayout.PageUrl = PagesInfo.PageUrl;
        //            blogpostforlayout.Title = PagesInfo.Title;

        //            blogpostforlayout.Layout = layoutdetails;
        //            blogpostforlayout.MetaTitle = (!string.IsNullOrEmpty(PagesInfo.MetaTitle.ToString())) ? PagesInfo.MetaTitle : null;
        //            blogpostforlayout.MetaKeywords = (!string.IsNullOrEmpty(PagesInfo.MetaKeywords.ToString())) ? PagesInfo.MetaKeywords : null;
        //            blogpostforlayout.MetaDescription = (!string.IsNullOrEmpty(PagesInfo.MetaDescription.ToString())) ? PagesInfo.MetaDescription : null;
        //            blogpostforlayout.Status = PagesInfo.Status;
        //            blogpostforlayout.PageUrlHash = PagesInfo.PageUrlHash;

        //            if (string.IsNullOrEmpty(PagesInfo.MasterPageId.ToString()))
        //            {
        //                blogpostforlayout.MasterPage = null;
        //            }
        //            else
        //            {
        //                Page masterpage = new Page();
        //                masterpage.Id = new Guid(PagesInfo.MasterPageId.ToString());
        //                blogpostforlayout.MasterPage = masterpage;
        //            }
        //            blogpostforlayout.IsMasterPage = PagesInfo.IsMasterPage;
        //            if (string.IsNullOrEmpty(PagesInfo.LanguageId.ToString()))
        //            {
        //                blogpostforlayout.Language = null;
        //            }
        //            else
        //            {
        //                Language language = new Language();
        //                language.Id = new Guid(PagesInfo.LanguageId.ToString());
        //                JObject requestforlanguage = new JObject();
        //                requestforlanguage.Add("languageId", language.Id);
        //                string languageobj = JsonConvert.SerializeObject(requestforlanguage);
        //                var LanguageModel = _webClient.DownloadData<string>("Blog/GetLanguageDetails", new { Js = languageobj });
        //                dynamic LanguageInfo = JObject.Parse(LanguageModel);
        //                language.Version = LanguageInfo.Version;
        //                language.IsDeleted = LanguageInfo.IsDeleted;
        //                language.CreatedOn = LanguageInfo.CreatedOn;
        //                language.CreatedByUser = LanguageInfo.CreatedByUser;
        //                language.ModifiedOn = LanguageInfo.ModifiedOn;
        //                language.ModifiedByUser = LanguageInfo.ModifiedByUser;
        //                language.DeletedOn = (!string.IsNullOrEmpty(LanguageInfo.DeletedOn.ToString())) ? LanguageInfo.DeletedOn : null;
        //                language.DeletedByUser = (!string.IsNullOrEmpty(LanguageInfo.DeletedByUser.ToString())) ? LanguageInfo.DeletedByUser : null;
        //                language.Name = LanguageInfo.Name;
        //                language.Code = LanguageInfo.Code;

        //                blogpostforlayout.Language = language;
        //            }
        //            blogpostforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(PagesInfo.LanguageGroupIdentifier.ToString())) ? PagesInfo.LanguageGroupIdentifier : null;
        //            blogpostforlayout.ForceAccessProtocol = PagesInfo.ForceAccessProtocol;

        //            pageslist.Add(blogpostforlayout);
        //        }
        //        else if (PagesInfo.Flag == 2)
        //        {
        //            PageProperties pagepropertiesforlayout = new PageProperties();
        //            pagepropertiesforlayout.Id = pageId;
        //            pagepropertiesforlayout.Description = PagesInfo.Description;
        //            if (string.IsNullOrEmpty(PagesInfo.ImageId.ToString()))
        //            {
        //                pagepropertiesforlayout.Image = null;
        //            }
        //            else
        //            {
        //                MediaImage image = new MediaImage();
        //                image.Id = new Guid(PagesInfo.ImageId.ToString());
        //                JObject requestforimagedetails = new JObject();
        //                requestforimagedetails.Add("imageId", image.Id);
        //                string imageobj = JsonConvert.SerializeObject(requestforimagedetails);
        //                var ImageModel = _webClient.DownloadData<string>("Blog/GetImageDetails", new { Js = imageobj });
        //                dynamic ImageInfo = JObject.Parse(ImageModel);
        //                image.Caption = (!string.IsNullOrEmpty(ImageInfo.Caption.ToString())) ? ImageInfo.Caption : null;
        //                image.ImageAlign = (!string.IsNullOrEmpty(ImageInfo.ImageAlign.ToString())) ? ImageInfo.ImageAlign : null;
        //                image.Width = ImageInfo.Width;
        //                image.Height = ImageInfo.Height;
        //                image.CropCoordX1 = (!string.IsNullOrEmpty(ImageInfo.CropCoordX1.ToString())) ? ImageInfo.CropCoordX1 : null;
        //                image.CropCoordX2 = (!string.IsNullOrEmpty(ImageInfo.CropCoordX2.ToString())) ? ImageInfo.CropCoordX2 : null;
        //                image.CropCoordY1 = (!string.IsNullOrEmpty(ImageInfo.CropCoordY1.ToString())) ? ImageInfo.CropCoordY1 : null;
        //                image.CropCoordY2 = (!string.IsNullOrEmpty(ImageInfo.CropCoordY2.ToString())) ? ImageInfo.CropCoordY2 : null;
        //                image.OriginalWidth = ImageInfo.OriginalWidth;
        //                image.OriginalHeight = ImageInfo.OriginalHeight;
        //                image.OriginalSize = ImageInfo.OriginalSize;
        //                image.OriginalUri = ImageInfo.OriginalUri;
        //                image.PublicOriginallUrl = ImageInfo.PublicOriginallUrl;
        //                image.IsOriginalUploaded = (!string.IsNullOrEmpty(ImageInfo.IsOriginalUploaded.ToString())) ? ImageInfo.IsOriginalUploaded : null;
        //                image.ThumbnailWidth = ImageInfo.ThumbnailWidth;
        //                image.ThumbnailHeight = ImageInfo.ThumbnailHeight;
        //                image.ThumbnailSize = ImageInfo.ThumbnailSize;
        //                image.ThumbnailUri = ImageInfo.ThumbnailUri;
        //                image.PublicThumbnailUrl = ImageInfo.PublicThumbnailUrl;
        //                image.IsThumbnailUploaded = (!string.IsNullOrEmpty(ImageInfo.IsThumbnailUploaded.ToString())) ? ImageInfo.IsThumbnailUploaded : null;
        //                image.Version = ImageInfo.Version;
        //                image.IsDeleted = ImageInfo.IsDeleted;
        //                image.CreatedOn = ImageInfo.CreatedOn;
        //                image.CreatedByUser = ImageInfo.CreatedByUser;
        //                image.ModifiedOn = ImageInfo.ModifiedOn;
        //                image.ModifiedByUser = ImageInfo.ModifiedByUser;
        //                image.DeletedOn = (!string.IsNullOrEmpty(ImageInfo.DeletedOn.ToString())) ? ImageInfo.DeletedOn : null;
        //                image.DeletedByUser = (!string.IsNullOrEmpty(ImageInfo.DeletedByUser.ToString())) ? ImageInfo.DeletedByUser : null;
        //                if (!string.IsNullOrEmpty(ImageInfo.FolderId.ToString()))
        //                {
        //                    MediaFolder folder = new MediaFolder();
        //                    folder.Id = ImageInfo.FolderId;
        //                    image.Folder = folder;
        //                }
        //                else
        //                {
        //                    image.Folder = null;
        //                }
        //                image.Title = ImageInfo.Title;
        //                image.Type = ImageInfo.Type;
        //                image.ContentType = ImageInfo.ContentType;
        //                image.IsArchived = ImageInfo.IsArchived;
        //                if (!string.IsNullOrEmpty(ImageInfo.FolderId.ToString()))
        //                {
        //                    image.Original.Id = ImageInfo.OriginalId;
        //                }
        //                else
        //                {
        //                    image.Original = null;

        //                }

        //                image.PublishedOn = ImageInfo.PublishedOn;
        //                //image.Image.Id = ImageInfo.ImageId;
        //                image.Description = (!string.IsNullOrEmpty(ImageInfo.Description.ToString())) ? ImageInfo.Description : null;
        //                pagepropertiesforlayout.Image = image;
        //            }

        //            pagepropertiesforlayout.CustomCss = (!string.IsNullOrEmpty(PagesInfo.CustomCss.ToString())) ? PagesInfo.CustomCss : null;
        //            pagepropertiesforlayout.CustomJS = (!string.IsNullOrEmpty(PagesInfo.CustomJS.ToString())) ? PagesInfo.CustomJS : null;
        //            pagepropertiesforlayout.UseCanonicalUrl = PagesInfo.UseCanonicalUrl;
        //            pagepropertiesforlayout.UseNoFollow = PagesInfo.UseNoFollow;
        //            pagepropertiesforlayout.UseNoIndex = PagesInfo.UseNoIndex;

        //            if (string.IsNullOrEmpty(PagesInfo.SecondaryImageId.ToString()))
        //            {
        //                pagepropertiesforlayout.SecondaryImage = null;
        //            }
        //            else
        //            {
        //                MediaImage secondaryimage = new MediaImage();
        //                secondaryimage.Id = new Guid(PagesInfo.SecondaryImageId.ToString());
        //                pagepropertiesforlayout.SecondaryImage = secondaryimage;
        //            }
        //            if (string.IsNullOrEmpty(PagesInfo.FeaturedImageId.ToString()))
        //            {
        //                pagepropertiesforlayout.FeaturedImage = null;
        //            }
        //            else
        //            {
        //                MediaImage featuredimage = new MediaImage();
        //                featuredimage.Id = new Guid(PagesInfo.FeaturedImageId.ToString());
        //                pagepropertiesforlayout.FeaturedImage = featuredimage;
        //            }

        //            pagepropertiesforlayout.IsArchived = PagesInfo.IsArchived;
        //            pagepropertiesforlayout.Version = PagesInfo.Version;
        //            pagepropertiesforlayout.PageUrl = PagesInfo.PageUrl;
        //            pagepropertiesforlayout.Title = PagesInfo.Title;

        //            pagepropertiesforlayout.Layout = layoutdetails;
        //            pagepropertiesforlayout.MetaTitle = (!string.IsNullOrEmpty(PagesInfo.MetaTitle.ToString())) ? PagesInfo.MetaTitle : null;
        //            pagepropertiesforlayout.MetaKeywords = (!string.IsNullOrEmpty(PagesInfo.MetaKeywords.ToString())) ? PagesInfo.MetaKeywords : null;
        //            pagepropertiesforlayout.MetaDescription = (!string.IsNullOrEmpty(PagesInfo.MetaDescription.ToString())) ? PagesInfo.MetaDescription : null;
        //            pagepropertiesforlayout.Status = PagesInfo.Status;
        //            pagepropertiesforlayout.PageUrlHash = PagesInfo.PageUrlHash;

        //            if (string.IsNullOrEmpty(PagesInfo.MasterPageId.ToString()))
        //            {
        //                pagepropertiesforlayout.MasterPage = null;
        //            }
        //            else
        //            {
        //                Page masterpage = new Page();
        //                masterpage.Id = new Guid(PagesInfo.MasterPageId.ToString());
        //                pagepropertiesforlayout.MasterPage = masterpage;
        //            }
        //            pagepropertiesforlayout.IsMasterPage = PagesInfo.IsMasterPage;
        //            if (string.IsNullOrEmpty(PagesInfo.LanguageId.ToString()))
        //            {
        //                pagepropertiesforlayout.Language = null;
        //            }
        //            else
        //            {
        //                Language language = new Language();
        //                language.Id = new Guid(PagesInfo.LanguageId.ToString());
        //                JObject requestforlanguage = new JObject();
        //                requestforlanguage.Add("languageId", language.Id);
        //                string languageobj = JsonConvert.SerializeObject(requestforlanguage);
        //                var LanguageModel = _webClient.DownloadData<string>("Blog/GetLanguageDetails", new { Js = languageobj });
        //                dynamic LanguageInfo = JObject.Parse(LanguageModel);
        //                language.Version = LanguageInfo.Version;
        //                language.IsDeleted = LanguageInfo.IsDeleted;
        //                language.CreatedOn = LanguageInfo.CreatedOn;
        //                language.CreatedByUser = LanguageInfo.CreatedByUser;
        //                language.ModifiedOn = LanguageInfo.ModifiedOn;
        //                language.ModifiedByUser = LanguageInfo.ModifiedByUser;
        //                language.DeletedOn = (!string.IsNullOrEmpty(LanguageInfo.DeletedOn.ToString())) ? LanguageInfo.DeletedOn : null;
        //                language.DeletedByUser = (!string.IsNullOrEmpty(LanguageInfo.DeletedByUser.ToString())) ? LanguageInfo.DeletedByUser : null;
        //                language.Name = LanguageInfo.Name;
        //                language.Code = LanguageInfo.Code;
        //                pagepropertiesforlayout.Language = language;
        //            }
        //            pagepropertiesforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(PagesInfo.LanguageGroupIdentifier.ToString())) ? PagesInfo.LanguageGroupIdentifier : null;
        //            pagepropertiesforlayout.ForceAccessProtocol = PagesInfo.ForceAccessProtocol;

        //            JObject requestforpagecontent = new JObject();
        //            requestforpagecontent.Add("pageId", pagepropertiesforlayout.Id);
        //            string pagecontentobj = JsonConvert.SerializeObject(requestforpagecontent);
        //            var pagecontentmodel = _webClient.DownloadData<JArray>("Blog/GetPageContentDetailsWithPageId", new { Js = pagecontentobj });

        //            IList<PageContent> pagecontents = new List<PageContent>();
        //            List<PageContent> pagecontentslist = new List<PageContent>();
        //            for (int i = 0; i < pagecontentmodel.Count; i++)
        //            {
        //                PageContent pagecontent = new PageContent();
        //                pagecontent.Id = new Guid(pagecontentmodel[i]["PageContentId"].ToString());
        //                pagecontent.Version = Convert.ToInt32(pagecontentmodel[i]["Version"]);
        //                pagecontent.IsDeleted = (bool)pagecontentmodel[i]["IsDeleted"];
        //                pagecontent.CreatedOn = (DateTime)pagecontentmodel[i]["CreatedOn"];
        //                pagecontent.CreatedByUser = pagecontentmodel[i]["CreatedByUser"].ToString();
        //                pagecontent.ModifiedByUser = pagecontentmodel[i]["ModifiedByUser"].ToString();
        //                pagecontent.ModifiedOn = (DateTime)pagecontentmodel[i]["ModifiedOn"];
        //                if (!string.IsNullOrEmpty(pagecontentmodel[i]["DeletedOn"].ToString()))
        //                {
        //                    pagecontent.DeletedOn = (DateTime)pagecontentmodel[i]["DeletedOn"];
        //                }
        //                else
        //                {
        //                    pagecontent.DeletedOn = null;
        //                }
        //                pagecontent.DeletedByUser = (!string.IsNullOrEmpty(pagecontentmodel[i]["DeletedByUser"].ToString())) ? pagecontentmodel[i]["DeletedByUser"].ToString() : null;
        //                pagecontent.Order = Convert.ToInt32(pagecontentmodel[i]["Order"]);


        //                JObject requestdetails = new JObject();
        //                requestdetails.Add("pageId", pagepropertiesforlayout.Id);
        //                string jsobj = JsonConvert.SerializeObject(requestdetails);
        //                var blogPostmodel = _webClient.DownloadData<string>("Blog/GetPagesForLayout", new { Js = jsobj });
        //                dynamic Info = JObject.Parse(blogPostmodel);
        //                Page pagedetails = new Page();
        //                if (Info.Flag == 1)
        //                {
        //                    BlogPost blogpost = new BlogPost();
        //                    blogpost.Id = pagepropertiesforlayout.Id;
        //                    blogpost.ActivationDate = Info.ActivationDate;
        //                    blogpost.ExpirationDate = (!string.IsNullOrEmpty(Info.ExpirationDate.ToString())) ? Info.ExpirationDate : null;
        //                    blogpost.Description = Info.Description;
        //                    if (string.IsNullOrEmpty(Info.ImageId.ToString()))
        //                    {
        //                        blogpost.Image = null;
        //                    }
        //                    else
        //                    {
        //                        MediaImage image = new MediaImage();
        //                        image.Id = new Guid(Info.ImageId.ToString());
        //                        blogpost.Image = image;
        //                    }

        //                    blogpost.CustomCss = (!string.IsNullOrEmpty(Info.CustomCss.ToString())) ? Info.CustomCss : null;
        //                    blogpost.CustomJS = (!string.IsNullOrEmpty(Info.CustomJS.ToString())) ? Info.CustomJS : null;
        //                    blogpost.UseCanonicalUrl = Info.UseCanonicalUrl;
        //                    blogpost.UseNoFollow = Info.UseNoFollow;
        //                    blogpost.UseNoIndex = Info.UseNoIndex;

        //                    if (string.IsNullOrEmpty(Info.SecondaryImageId.ToString()))
        //                    {
        //                        blogpost.SecondaryImage = null;
        //                    }
        //                    else
        //                    {
        //                        MediaImage secondaryimage = new MediaImage();
        //                        secondaryimage.Id = new Guid(Info.SecondaryImageId.ToString());
        //                        blogpost.SecondaryImage = secondaryimage;
        //                    }
        //                    if (string.IsNullOrEmpty(Info.FeaturedImageId.ToString()))
        //                    {
        //                        blogpost.FeaturedImage = null;
        //                    }
        //                    else
        //                    {
        //                        MediaImage featuredimage = new MediaImage();
        //                        featuredimage.Id = new Guid(Info.FeaturedImageId.ToString());
        //                        blogpost.FeaturedImage = featuredimage;
        //                    }

        //                    blogpost.IsArchived = Info.IsArchived;
        //                    blogpost.Version = Info.Version;
        //                    blogpost.PageUrl = Info.PageUrl;
        //                    blogpost.Title = Info.Title;
        //                    blogpost.Layout = layoutdetails;
        //                    blogpost.MetaTitle = (!string.IsNullOrEmpty(Info.MetaTitle.ToString())) ? Info.MetaTitle : null;
        //                    blogpost.MetaKeywords = (!string.IsNullOrEmpty(Info.MetaKeywords.ToString())) ? Info.MetaKeywords : null;
        //                    blogpost.MetaDescription = (!string.IsNullOrEmpty(Info.MetaDescription.ToString())) ? Info.MetaDescription : null;
        //                    blogpost.Status = Info.Status;
        //                    blogpost.PageUrlHash = Info.PageUrlHash;

        //                    if (string.IsNullOrEmpty(Info.MasterPageId.ToString()))
        //                    {
        //                        blogpost.MasterPage = null;
        //                    }
        //                    else
        //                    {
        //                        Page masterpage = new Page();
        //                        masterpage.Id = new Guid(Info.MasterPageId.ToString());
        //                        blogpost.MasterPage = masterpage;
        //                    }
        //                    blogpost.IsMasterPage = Info.IsMasterPage;
        //                    if (string.IsNullOrEmpty(Info.LanguageId.ToString()))
        //                    {
        //                        blogpost.Language = null;
        //                    }
        //                    else
        //                    {
        //                        Language language = new Language();
        //                        language.Id = new Guid(Info.LanguageId.ToString());
        //                        blogpost.Language = language;
        //                    }
        //                    blogpost.LanguageGroupIdentifier = (!string.IsNullOrEmpty(Info.LanguageGroupIdentifier.ToString())) ? Info.LanguageGroupIdentifier : null;
        //                    blogpost.ForceAccessProtocol = Info.ForceAccessProtocol;

        //                    pagedetails = blogpost;

        //                }
        //                else if (Info.Flag == 2)
        //                {
        //                    PageProperties pageproperties = new PageProperties();
        //                    pageproperties.Id = pagepropertiesforlayout.Id;
        //                    pageproperties.Description = Info.Description;
        //                    if (string.IsNullOrEmpty(Info.ImageId.ToString()))
        //                    {
        //                        pageproperties.Image = null;
        //                    }
        //                    else
        //                    {
        //                        MediaImage image = new MediaImage();
        //                        image.Id = new Guid(Info.ImageId.ToString());
        //                        pageproperties.Image = image;
        //                    }

        //                    pageproperties.CustomCss = (!string.IsNullOrEmpty(Info.CustomCss.ToString())) ? Info.CustomCss : null;
        //                    pageproperties.CustomJS = (!string.IsNullOrEmpty(Info.CustomJS.ToString())) ? Info.CustomJS : null;
        //                    pageproperties.UseCanonicalUrl = Info.UseCanonicalUrl;
        //                    pageproperties.UseNoFollow = Info.UseNoFollow;
        //                    pageproperties.UseNoIndex = Info.UseNoIndex;

        //                    if (string.IsNullOrEmpty(Info.SecondaryImageId.ToString()))
        //                    {
        //                        pageproperties.SecondaryImage = null;
        //                    }
        //                    else
        //                    {
        //                        MediaImage secondaryimage = new MediaImage();
        //                        secondaryimage.Id = new Guid(Info.SecondaryImageId.ToString());
        //                        pageproperties.SecondaryImage = secondaryimage;
        //                    }
        //                    if (string.IsNullOrEmpty(Info.FeaturedImageId.ToString()))
        //                    {
        //                        pageproperties.FeaturedImage = null;
        //                    }
        //                    else
        //                    {
        //                        MediaImage featuredimage = new MediaImage();
        //                        featuredimage.Id = new Guid(Info.FeaturedImageId.ToString());
        //                        pageproperties.FeaturedImage = featuredimage;
        //                    }

        //                    pageproperties.IsArchived = Info.IsArchived;
        //                    pageproperties.Version = Info.Version;
        //                    pageproperties.PageUrl = Info.PageUrl;
        //                    pageproperties.Title = Info.Title;

        //                    pageproperties.Layout = layoutdetails;
        //                    pageproperties.MetaTitle = (!string.IsNullOrEmpty(Info.MetaTitle.ToString())) ? Info.MetaTitle : null;
        //                    pageproperties.MetaKeywords = (!string.IsNullOrEmpty(Info.MetaKeywords.ToString())) ? Info.MetaKeywords : null;
        //                    pageproperties.MetaDescription = (!string.IsNullOrEmpty(Info.MetaDescription.ToString())) ? Info.MetaDescription : null;
        //                    pageproperties.Status = Info.Status;
        //                    pageproperties.PageUrlHash = Info.PageUrlHash;

        //                    if (string.IsNullOrEmpty(Info.MasterPageId.ToString()))
        //                    {
        //                        pageproperties.MasterPage = null;
        //                    }
        //                    else
        //                    {
        //                        Page masterpage = new Page();
        //                        masterpage.Id = new Guid(Info.MasterPageId.ToString());
        //                        pageproperties.MasterPage = masterpage;
        //                    }
        //                    pageproperties.IsMasterPage = Info.IsMasterPage;
        //                    if (string.IsNullOrEmpty(Info.LanguageId.ToString()))
        //                    {
        //                        pageproperties.Language = null;
        //                    }
        //                    else
        //                    {
        //                        Language language = new Language();
        //                        language.Id = new Guid(Info.LanguageId.ToString());
        //                        pageproperties.Language = language;
        //                    }
        //                    pageproperties.LanguageGroupIdentifier = (!string.IsNullOrEmpty(Info.LanguageGroupIdentifier.ToString())) ? Info.LanguageGroupIdentifier : null;
        //                    pageproperties.ForceAccessProtocol = Info.ForceAccessProtocol;

        //                    pagedetails = pageproperties;

        //                }
        //                else if (Info.Flag == 3)
        //                {
        //                    Page page = new Page();
        //                    page.Id = pagepropertiesforlayout.Id;
        //                    page.Version = Info.Version;
        //                    page.PageUrl = Info.PageUrl;
        //                    page.Title = Info.Title;

        //                    page.Layout = layoutdetails;
        //                    page.MetaTitle = (!string.IsNullOrEmpty(Info.MetaTitle.ToString())) ? Info.MetaTitle : null;
        //                    page.MetaKeywords = (!string.IsNullOrEmpty(Info.MetaKeywords.ToString())) ? Info.MetaKeywords : null;
        //                    page.MetaDescription = (!string.IsNullOrEmpty(Info.MetaDescription.ToString())) ? Info.MetaDescription : null;
        //                    page.Status = Info.Status;
        //                    page.PageUrlHash = Info.PageUrlHash;

        //                    if (string.IsNullOrEmpty(Info.MasterPageId.ToString()))
        //                    {
        //                        page.MasterPage = null;
        //                    }
        //                    else
        //                    {
        //                        Page masterpage = new Page();
        //                        masterpage.Id = new Guid(Info.MasterPageId.ToString());
        //                        page.MasterPage = masterpage;
        //                    }
        //                    page.IsMasterPage = Info.IsMasterPage;
        //                    if (string.IsNullOrEmpty(Info.LanguageId.ToString()))
        //                    {
        //                        page.Language = null;
        //                    }
        //                    else
        //                    {
        //                        Language language = new Language();
        //                        language.Id = new Guid(Info.LanguageId.ToString());
        //                        page.Language = language;
        //                    }
        //                    page.LanguageGroupIdentifier = (!string.IsNullOrEmpty(Info.LanguageGroupIdentifier.ToString())) ? Info.LanguageGroupIdentifier : null;
        //                    page.ForceAccessProtocol = Info.ForceAccessProtocol;

        //                    pagedetails = page;

        //                }
        //                pagecontent.Page = pagedetails;


        //                HtmlContentWidget contentdetails = new HtmlContentWidget();

        //                contentdetails.Id = new Guid(pagecontentmodel[i]["ContentId"].ToString());

        //                JObject requestforcontentdetails = new JObject();
        //                requestforcontentdetails.Add("contentId", contentdetails.Id);


        //                string contentobj = JsonConvert.SerializeObject(requestforcontentdetails);
        //                var contentmodel = _webClient.DownloadData<string>("Blog/GetContentDetailsForLayout", new { Js = contentobj });
        //                dynamic contentInfo = JObject.Parse(contentmodel);


        //                contentdetails.Name = contentInfo.Name;
        //                contentdetails.IsDeleted = contentInfo.IsDeleted;
        //                contentdetails.Status = contentInfo.Status;
        //                contentdetails.Version = contentInfo.Version;
        //                if (!string.IsNullOrEmpty(contentInfo.OriginalId.ToString()))
        //                {

        //                    contentdetails.Original.Id = new Guid(contentInfo.OriginalId.ToString());
        //                }
        //                else
        //                {
        //                    contentdetails.Original = null;
        //                }
        //                contentdetails.PreviewUrl = (!string.IsNullOrEmpty(contentInfo.PreviewUrl.ToString())) ? contentInfo.PreviewUrl : null;
        //                contentdetails.CreatedByUser = contentInfo.CreatedByUser;
        //                contentdetails.CreatedOn = contentInfo.CreatedOn;
        //                contentdetails.DeletedByUser = (!string.IsNullOrEmpty(contentInfo.DeletedByUser.ToString())) ? contentInfo.DeletedByUser : null;
        //                contentdetails.DeletedOn = (!string.IsNullOrEmpty(contentInfo.DeletedOn.ToString())) ? contentInfo.DeletedOn : null;
        //                contentdetails.ModifiedByUser = contentInfo.ModifiedByUser;
        //                contentdetails.PublishedOn = (!string.IsNullOrEmpty(contentInfo.PublishedOn.ToString())) ? contentInfo.PublishedOn : null;
        //                contentdetails.PublishedByUser = (!string.IsNullOrEmpty(contentInfo.PublishedByUser.ToString())) ? contentInfo.PublishedByUser : null;
        //                //regioncontentdetails.ActivationDate = contentInfo.ActivationDate;
        //                //regioncontentdetails.ExpirationDate = (!string.IsNullOrEmpty(contentInfo.ExpirationDate.ToString())) ? contentInfo.ExpirationDate : null;
        //                //regioncontentdetails.CustomCss = (!string.IsNullOrEmpty(contentInfo.CustomCss.ToString())) ? contentInfo.CustomCss : null;
        //                //regioncontentdetails.UseCustomCss = contentInfo.UseCustomCss;
        //                // regioncontentdetails.CustomJs = (!string.IsNullOrEmpty(contentInfo.CustomJs.ToString())) ? contentInfo.CustomJs : null;
        //                // regioncontentdetails.UseCustomJs = contentInfo.UseCustomJs;
        //                //regioncontentdetails.Html = contentInfo.Html;
        //                // regioncontentdetails.EditInSourceMode = contentInfo.EditInSourceMode;
        //                //regioncontentdetails.OriginalText = (!string.IsNullOrEmpty(contentInfo.OriginalText.ToString())) ? contentInfo.OriginalText : null;
        //                //regioncontentdetails.ContentTextMode = contentInfo.ContentTextMode;









        //                //        //End


        //                pagecontent.Content = contentdetails;


        //                Region region = new Region();
        //                region.Id = new Guid(pagecontentmodel[i]["RegionId"].ToString());
        //                JObject requestforregiondetails = new JObject();
        //                requestforregiondetails.Add("regionId", region.Id);
        //                string regionobj1 = JsonConvert.SerializeObject(requestforregiondetails);
        //                var regionmodel1 = _webClient.DownloadData<string>("Blog/GetRegionDetails", new { Js = regionobj1 });
        //                dynamic regionInfo1 = JObject.Parse(regionmodel1);
        //                region.Version = regionInfo1.Version;
        //                region.IsDeleted = regionInfo1.IsDeleted;
        //                region.CreatedOn = regionInfo1.CreatedOn;
        //                region.CreatedByUser = regionInfo1.CreatedByUser;
        //                region.ModifiedOn = regionInfo1.ModifiedOn;
        //                region.ModifiedByUser = regionInfo1.ModifiedByUser;
        //                region.DeletedOn = (!string.IsNullOrEmpty(regionInfo1.DeletedOn.ToString())) ? regionInfo1.DeletedOn : null;
        //                region.DeletedByUser = (!string.IsNullOrEmpty(regionInfo1.DeletedByUser.ToString())) ? regionInfo1.DeletedByUser : null;
        //                region.RegionIdentifier = regionInfo1.RegionIdentifier;
        //                JObject requestforlayoutregiondetails1 = new JObject();
        //                requestforlayoutregiondetails1.Add("regionId", region.Id);
        //                string layoutregionobj1 = JsonConvert.SerializeObject(requestforlayoutregiondetails1);
        //                var layoutregionmodel1 = _webClient.DownloadData<JArray>("Blog/GetLayoutRegionDetails", new { Js = layoutregionobj1 });

        //                IList<LayoutRegion> layoutregions1 = new List<LayoutRegion>();
        //                List<LayoutRegion> layoutregionlist1 = new List<LayoutRegion>();

        //                for (int j = 0; j < layoutregionmodel1.Count; j++)
        //                {
        //                    LayoutRegion layoutregion1 = new LayoutRegion();
        //                    layoutregion1.Id = new Guid(layoutregionmodel1[j]["LayoutRegionId"].ToString());
        //                    layoutregion1.Version = Convert.ToInt32(layoutregionmodel1[j]["Version"]);
        //                    layoutregion1.IsDeleted = (bool)layoutregionmodel1[j]["IsDeleted"];
        //                    layoutregion1.CreatedOn = (DateTime)layoutregionmodel1[j]["CreatedOn"];
        //                    layoutregion1.CreatedByUser = layoutregionmodel1[j]["CreatedByUser"].ToString();
        //                    layoutregion1.ModifiedOn = (DateTime)layoutregionmodel1[j]["ModifiedOn"];
        //                    layoutregion1.ModifiedByUser = layoutregionmodel1[j]["ModifiedByUser"].ToString();
        //                    if (!string.IsNullOrEmpty(layoutregionmodel1[j]["DeletedOn"].ToString()))
        //                    {

        //                        layoutregion1.DeletedOn = (DateTime)layoutregionmodel1[j]["DeletedOn"];
        //                    }
        //                    else
        //                    {
        //                        layoutregion1.DeletedOn = null;
        //                    }
        //                    layoutregion1.DeletedByUser = (!string.IsNullOrEmpty(layoutregionmodel1[j]["DeletedByUser"].ToString())) ? layoutregionmodel[j]["DeletedByUser"].ToString() : null;
        //                    layoutregion1.Description = (!string.IsNullOrEmpty(layoutregionmodel1[j]["Description"].ToString())) ? layoutregionmodel[j]["Description"].ToString() : null;
        //                    // layoutregion1.Layout = layoutdetails;
        //                    layoutregion1.Region = region;
        //                    layoutregionlist1.Add(layoutregion1);
        //                }
        //                layoutregions1 = layoutregionlist1;
        //                region.LayoutRegion = layoutregions1;







        //                pagecontentslist.Add(pagecontent);
        //            }
        //            pagecontents = pagecontentslist;
        //            pagepropertiesforlayout.PageContents = pagecontents;

        //            pageslist.Add(pagepropertiesforlayout);
        //        }
        //        else if (PagesInfo.Flag == 3)
        //        {
        //            Page pageforlayout = new Page();
        //            pageforlayout.Id = pageId;
        //            pageforlayout.Version = PagesInfo.Version;
        //            pageforlayout.PageUrl = PagesInfo.PageUrl;
        //            pageforlayout.Title = PagesInfo.Title;
                   
        //            pageforlayout.Layout = layoutdetails;
        //            pageforlayout.MetaTitle = (!string.IsNullOrEmpty(PagesInfo.MetaTitle.ToString())) ? PagesInfo.MetaTitle : null;
        //            pageforlayout.MetaKeywords = (!string.IsNullOrEmpty(PagesInfo.MetaKeywords.ToString())) ? PagesInfo.MetaKeywords : null;
        //            pageforlayout.MetaDescription = (!string.IsNullOrEmpty(PagesInfo.MetaDescription.ToString())) ? PagesInfo.MetaDescription : null;
        //            pageforlayout.Status = PagesInfo.Status;
        //            pageforlayout.PageUrlHash = PagesInfo.PageUrlHash;

        //            if (string.IsNullOrEmpty(PagesInfo.MasterPageId.ToString()))
        //            {
        //                pageforlayout.MasterPage = null;
        //            }
        //            else
        //            {
        //                Page masterpage = new Page();
        //                masterpage.Id = new Guid(PagesInfo.MasterPageId.ToString());
        //                pageforlayout.MasterPage = masterpage;
        //            }
        //            pageforlayout.IsMasterPage = PagesInfo.IsMasterPage;
        //            if (string.IsNullOrEmpty(PagesInfo.LanguageId.ToString()))
        //            {
        //                pageforlayout.Language = null;
        //            }
        //            else
        //            {
        //                Language language = new Language();
        //                language.Id = new Guid(PagesInfo.LanguageId.ToString());
        //                JObject requestforlanguage = new JObject();
        //                requestforlanguage.Add("languageId", language.Id);
        //                string languageobj = JsonConvert.SerializeObject(requestforlanguage);
        //                var LanguageModel = _webClient.DownloadData<string>("Blog/GetLanguageDetails", new { Js = languageobj });
        //                dynamic LanguageInfo = JObject.Parse(LanguageModel);
        //                language.Version = LanguageInfo.Version;
        //                language.IsDeleted = LanguageInfo.IsDeleted;
        //                language.CreatedOn = LanguageInfo.CreatedOn;
        //                language.CreatedByUser = LanguageInfo.CreatedByUser;
        //                language.ModifiedOn = LanguageInfo.ModifiedOn;
        //                language.ModifiedByUser = LanguageInfo.ModifiedByUser;
        //                language.DeletedOn = (!string.IsNullOrEmpty(LanguageInfo.DeletedOn.ToString())) ? LanguageInfo.DeletedOn : null;
        //                language.DeletedByUser = LanguageInfo.DeletedByUser;
        //                language.Name = LanguageInfo.Name;
        //                language.Code = LanguageInfo.Code;
        //                pageforlayout.Language = language;
        //            }
        //            pageforlayout.LanguageGroupIdentifier = (!string.IsNullOrEmpty(PagesInfo.LanguageGroupIdentifier.ToString())) ? PagesInfo.LanguageGroupIdentifier : null;
        //            pageforlayout.ForceAccessProtocol = PagesInfo.ForceAccessProtocol;

        //            pageslist.Add(pageforlayout);
        //        }

        //    }
        //    pages = pageslist;
        //    layoutdetails.Pages = pages;

        //    // End







        //    return layoutdetails;

           
        //}

        /// <summary>
        /// Adds the default access rules to blog post entity.
        /// </summary>
        /// <param name="blogPost">The blog post.</param>
        /// <param name="principal">The principal.</param>
        /// <param name="masterPage">The master page.</param>
        protected void AddDefaultAccessRules(BlogPost blogPost, IPrincipal principal, Page masterPage)
        {
            IEnumerable<IAccessRule> accessRules;

            if (masterPage != null)
            {
                accessRules = masterPage.AccessRules;
            }
            else
            {
                accessRules = accessControlService.GetDefaultAccessList(principal);
            }

            accessControlService.UpdateAccessControl(blogPost, accessRules.ToList());
        }

        /// <summary>
        /// Gets the filtered blog posts query.
        /// </summary>
        /// <param name="request">The filter.</param>
        /// <param name="joinContents">if set to <c>true</c> join contents tables.</param>
        /// <returns>
        /// NHibernate query for getting filtered blog posts
        /// </returns>
        public NHibernate.IQueryOver<BlogPost, BlogPost> GetFilteredBlogPostsQuery(ViewModels.Filter.BlogsFilter request, bool joinContents = false)
        {
            request.SetDefaultSortingOptions("CreatedOn", true);

            BlogPost alias = null;

            var query = unitOfWork.Session
                .QueryOver(() => alias)
                .Where(() => !alias.IsDeleted && alias.Status != PageStatus.Preview);

            if (!request.IncludeArchived)
            {
                query = query.Where(() => !alias.IsArchived);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                var searchQuery = string.Format("%{0}%", request.SearchQuery);
                query = query.Where(Restrictions.InsensitiveLike(Projections.Property(() => alias.Title), searchQuery));
            }



            if (request.LanguageId.HasValue)
            {
                if (request.LanguageId.Value.HasDefaultValue())
                {
                    query = query.Where(Restrictions.IsNull(Projections.Property(() => alias.Language.Id)));
                }
                else
                {
                    query = query.Where(Restrictions.Eq(Projections.Property(() => alias.Language.Id), request.LanguageId.Value));
                }
            }

            if (request.Tags != null)
            {
                foreach (var tagKeyValue in request.Tags)
                {
                    var id = tagKeyValue.Key.ToGuidOrDefault();
                    query = query.WithSubquery.WhereExists(QueryOver.Of<PageTag>().Where(tag => tag.Tag.Id == id && tag.Page.Id == alias.Id).Select(tag => 1));
                }
            }

            if (request.Categories != null)
            {
                var categories = request.Categories.Select(c => new Guid(c.Key)).Distinct().ToList();

                foreach (var category in categories)
                {
                    var childCategories = categoryService.GetChildCategoriesIds(category).ToArray();
                    query = query.WithSubquery.WhereExists(QueryOver.Of<PageCategory>().Where(cat => !cat.IsDeleted && cat.Page.Id == alias.Id).WhereRestrictionOn(cat => cat.Category.Id).IsIn(childCategories).Select(cat => 1));
                }
            }

            if (request.Status.HasValue)
            {
                if (request.Status.Value == PageStatusFilterType.OnlyPublished)
                {
                    query = query.Where(() => alias.Status == PageStatus.Published);
                }
                else if (request.Status.Value == PageStatusFilterType.OnlyUnpublished)
                {
                    query = query.Where(() => alias.Status != PageStatus.Published);
                }
                else if (request.Status.Value == PageStatusFilterType.ContainingUnpublishedContents)
                {
                    PageContent pageContentAlias = null;
                    Root.Models.Content contentAlias = null;
                    Root.Models.Content contentHistoryAlias = null;

                    var subQuery =
                        QueryOver.Of(() => pageContentAlias)
                            .Inner.JoinAlias(() => pageContentAlias.Content, () => contentAlias, () => !contentAlias.IsDeleted)
                            .Left.JoinAlias(() => contentAlias.History, () => contentHistoryAlias, () => !contentHistoryAlias.IsDeleted)
                            .Where(() => pageContentAlias.Page.Id == alias.Id && !pageContentAlias.IsDeleted)
                            .Where(() => contentHistoryAlias.Status == ContentStatus.Draft || contentAlias.Status == ContentStatus.Draft)
                            .Select(p => 1);

                    query = query.WithSubquery.WhereExists(subQuery);
                }
            }

            if (request.SeoStatus.HasValue)
            {
                var subQuery = QueryOver.Of<SitemapNode>()
                    .Where(x => x.Page.Id == alias.Id || x.UrlHash == alias.PageUrlHash)
                    .And(x => !x.IsDeleted)
                    .JoinQueryOver(s => s.Sitemap)
                    .And(x => !x.IsDeleted)
                    .Select(s => 1);

                var hasSeoDisjunction =
                Restrictions.Disjunction()
                    .Add(RestrictionsExtensions.IsNullOrWhiteSpace(Projections.Property(() => alias.MetaTitle)))
                    .Add(RestrictionsExtensions.IsNullOrWhiteSpace(Projections.Property(() => alias.MetaKeywords)))
                    .Add(RestrictionsExtensions.IsNullOrWhiteSpace(Projections.Property(() => alias.MetaDescription)))
                    .Add(Subqueries.WhereNotExists(subQuery));

                if (request.SeoStatus.Value == SeoStatusFilterType.HasSeo)
                {
                    query = query.Where(Restrictions.Not(hasSeoDisjunction));
                }
                else
                {
                    query = query.Where(hasSeoDisjunction);
                }
            }

            if (joinContents)
            {
                PageContent pcAlias = null;
                BlogPostContent bcAlias = null;

                query = query.JoinAlias(() => alias.PageContents, () => pcAlias);
                query = query.JoinAlias(() => pcAlias.Content, () => bcAlias);
            }

            return query;
        }

        private void FillMetaInfo(BlogPost blogPost)
        {
            if (string.IsNullOrEmpty(blogPost.MetaDescription) && !string.IsNullOrEmpty(blogPost.Description))
            {
                blogPost.MetaDescription = blogPost.Description;
            }

            if (string.IsNullOrEmpty(blogPost.MetaKeywords))
            {
                if (blogPost.PageTags != null && blogPost.PageTags.IsNotEmpty())
                {
                    blogPost.MetaKeywords += string.Join(", ", blogPost.PageTags.Select(x => x.Tag).Select(y => y.Name).ToList());
                }

                if (blogPost.Categories != null && blogPost.Categories.IsNotEmpty())
                {
                    if (!string.IsNullOrEmpty(blogPost.MetaKeywords))
                    {
                        blogPost.MetaKeywords += ", ";
                    }
                    blogPost.MetaKeywords += string.Join(", ", blogPost.Categories.Select(x => x.Category).Select(y => y.Name).ToList());
                }
            }
        }
    }
}
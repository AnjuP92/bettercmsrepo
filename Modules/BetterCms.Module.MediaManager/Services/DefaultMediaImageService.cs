// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMediaImageService.cs" company="Devbridge Group LLC">
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterCms.Core.Exceptions;
using BetterCms.Core.Exceptions.Mvc;
using BetterCms.Core.Exceptions.Service;
using BetterCms.Core.Services.Storage;
using BetterCms.Module.MediaManager.Content.Resources;
using BetterCms.Module.MediaManager.Helpers;
using BetterCms.Module.MediaManager.Models;
using BetterCms.Module.MediaManager.Models.Enum;
using BetterCms.Module.Root.Mvc;

using Common.Logging;

using FluentNHibernate.Mapping;
using BetterCms.Core.WebServices;


using NHibernate;
using NHibernate.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BetterCms.Module.MediaManager.Services
{
    /// <summary>
    /// Default media image service.
    /// </summary>
    internal class DefaultMediaImageService : IMediaImageService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The thumbnail size.
        /// </summary>
        private static readonly Size ThumbnailSize = new Size(150, 150);

        /// <summary>
        /// The storage service.
        /// </summary>
        private readonly IStorageService storageService;

        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// The unit of work.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// The session factory provider.
        /// </summary>
        private readonly ISessionFactoryProvider sessionFactoryProvider;


        private ITWebClient _webClient;
        /// <summary>
        /// The media file service
        /// </summary>
        private readonly IMediaFileService mediaFileService;

        private readonly IMediaImageVersionPathService mediaImageVersionPathService;

        /// <summary>
        /// The image file format
        /// </summary>
        public static IDictionary<string, ImageFormat> transparencyFormats = new Dictionary<string, ImageFormat>(StringComparer.OrdinalIgnoreCase) { { "png", ImageFormat.Png }, { "gif", ImageFormat.Gif } };

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMediaImageService" /> class.
        /// </summary>
        /// <param name="mediaFileService">The media file service.</param>
        /// <param name="storageService">The storage service.</param>
        /// <param name="repository">The repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="sessionFactoryProvider">The session factory provider.</param>
        /// <param name="mediaImageVersionPathService"></param>
        public DefaultMediaImageService(IMediaFileService mediaFileService, IStorageService storageService,
            IRepository repository, ISessionFactoryProvider sessionFactoryProvider, IUnitOfWork unitOfWork,
            IMediaImageVersionPathService mediaImageVersionPathService)
        {
            this.mediaFileService = mediaFileService;
            this.sessionFactoryProvider = sessionFactoryProvider;
            this.storageService = storageService;
            this.repository = repository;
            this.unitOfWork = unitOfWork;
            this.mediaImageVersionPathService = mediaImageVersionPathService;
            _webClient = new TWebClient();
        }

        /// <summary>
        /// Removes an image related files from the storage.
        /// </summary>
        /// <param name="mediaImageId">The media image id.</param>
        /// <param name="version">The version.</param>
        public void RemoveImageWithFiles(Guid mediaImageId, int version, bool doNotCheckVersion = false, bool originalWasNotUploaded = false)
        {
            var removeImageFileTasks = new List<Task>();
            var image = repository.AsQueryable<MediaImage>()
                          .Where(f => f.Id == mediaImageId)
                          .Select(f => new
                          {
                              IsUploaded = f.IsUploaded,
                              FileUri = f.FileUri,
                              IsOriginalUploaded = f.IsOriginalUploaded,
                              OriginalUri = f.OriginalUri,
                              IsThumbnailUploaded = f.IsThumbnailUploaded,
                              ThumbnailUri = f.ThumbnailUri
                          })
                          .FirstOrDefault();

            if (image == null)
            {
                throw new CmsException(string.Format("Image not found by given id={0}", mediaImageId));
            }

            try
            {
                if (image.IsUploaded.HasValue && image.IsUploaded.Value)
                {
                    removeImageFileTasks.Add(
                        new Task(
                            () =>
                            { storageService.RemoveObject(image.FileUri); }));
                }

                if (image.IsOriginalUploaded.HasValue && image.IsOriginalUploaded.Value && !originalWasNotUploaded)
                {
                    removeImageFileTasks.Add(
                        new Task(
                            () =>
                            { storageService.RemoveObject(image.OriginalUri); }));
                }

                if (image.IsThumbnailUploaded.HasValue && image.IsThumbnailUploaded.Value)
                {
                    removeImageFileTasks.Add(
                        new Task(
                            () =>
                            { storageService.RemoveObject(image.ThumbnailUri); }));
                }

                if (removeImageFileTasks.Count > 0)
                {
                    Task.Factory.ContinueWhenAll(
                        removeImageFileTasks.ToArray(),
                        result =>
                        {
                            // TODO: add functionality to remove folder if it is empty
                        });

                    removeImageFileTasks.ForEach(task => task.Start());
                }
            }
            finally
            {
                if (doNotCheckVersion)
                {
                    var media = repository.AsQueryable<MediaImage>().FirstOrDefault(f => f.Id == mediaImageId);
                    var archivedImage = RevertChanges(media);
                    if (archivedImage != null)
                    {
                        repository.Delete(archivedImage);
                    }
                }
                else
                {
                    //var media = repository.AsQueryable<MediaImage>().FirstOrDefault(f => f.Id == mediaImageId);
                    //var archivedImage = RevertChanges(media);
                    repository.Delete<MediaImage>(mediaImageId, version);
                }
                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// Uploads the image.
        /// </summary>
        /// <param name="rootFolderId">The root folder id.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileLength">Length of the file.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="reuploadMediaId">The reupload media identifier.</param>
        /// <param name="overrideUrl">if set to <c>true</c> override URL.</param>
        /// <returns>
        /// Image entity.
        /// </returns>
        public MediaImage UploadImage(Guid rootFolderId, string fileName, long fileLength, Stream fileStream, Guid reuploadMediaId, bool overrideUrl = true)
        {
            overrideUrl = false; // TODO: temporary disabling feature #1055.

            var folderName = mediaFileService.CreateRandomFolderName();
            var publicFileName = MediaHelper.RemoveInvalidPathSymbols(MediaImageHelper.CreatePublicFileName(fileName, Path.GetExtension(fileName)));
            var fileExtension = Path.GetExtension(fileName);
            var imageType = ImageHelper.GetImageType(fileExtension);

            MediaImage uploadImage = null;
            var thumbnailFileStream = new MemoryStream();
            Size size;
            Size thumbnailSize = ThumbnailSize;
            long thumbnailImageLength;

            /* Upload standard raster type images */
            if (imageType == ImageType.Raster)
            {
                fileStream = RotateImage(fileStream);
                size = GetSize(fileStream);

                CreatePngThumbnail(fileStream, thumbnailFileStream, ThumbnailSize);
                thumbnailImageLength = thumbnailFileStream.Length;
            }
            /* Upload vector graphics images */
            else
            {
                size = Size.Empty;
                ReadParametersFormVectorImage(fileStream, ref size);
                thumbnailImageLength = fileStream.Length;
                CreateSvgThumbnail(fileStream, thumbnailFileStream, ThumbnailSize);
            }

            try
            {
                if (!reuploadMediaId.HasDefaultValue())
                {
                    // Re-uploading image: Get original image, folder name, file extension, file name
                    uploadImage = (MediaImage)repository.First<MediaImage>(image => image.Id == reuploadMediaId).Clone();
                    uploadImage.IsTemporary = true;

                    // Create new original image and upload file stream to the storage
                    uploadImage = CreateImage(rootFolderId, fileName, fileExtension, Path.GetFileName(fileName), size, fileLength);
                }
                else
                {
                    // Uploading new image
                    // Create new original image and upload file stream to the storage
                    uploadImage = CreateImage(
                        rootFolderId,
                        fileName,
                        fileExtension,
                        Path.GetFileName(fileName),
                        size,
                        fileLength);

                }
                SetThumbnailParameters(uploadImage, thumbnailSize, thumbnailImageLength);
                mediaImageVersionPathService.SetPathForNewOriginal(uploadImage, folderName, publicFileName, imageType);

                unitOfWork.BeginTransaction();

                try
                {
                    //     repository.Save(uploadImage);
                    JObject saveitem = new JObject();
                    saveitem.Add("Id", uploadImage.Id);
                    saveitem.Add("Version", uploadImage.Version);
                    saveitem.Add("IsDeleted", uploadImage.IsDeleted);
                    saveitem.Add("Title", uploadImage.Title);
                    saveitem.Add("Type", uploadImage.Type.ToString());
                    saveitem.Add("ContentType", uploadImage.ContentType.ToString());
                    saveitem.Add("IsArchived", uploadImage.IsArchived);
                    saveitem.Add("Description", uploadImage.Description);
                    saveitem.Add("OriginalFileName", uploadImage.OriginalFileName);
                    saveitem.Add("OriginalFileExtension", uploadImage.OriginalFileExtension);
                    saveitem.Add("FileUri", uploadImage.FileUri);
                    saveitem.Add("PublicUrl", uploadImage.PublicUrl);
                    saveitem.Add("Size", uploadImage.Size);
                    saveitem.Add("IsTemporary", uploadImage.IsTemporary);
                    saveitem.Add("IsUploaded", uploadImage.IsUploaded);
                    saveitem.Add("IsCanceled", uploadImage.IsCanceled);
                    saveitem.Add("IsMovedToTrash", uploadImage.IsMovedToTrash);
                    saveitem.Add("NextTryToMoveToTrash ", uploadImage.NextTryToMoveToTrash);

                    saveitem.Add("ThumbnailUri", uploadImage.ThumbnailUri);
                    saveitem.Add("Caption", uploadImage.Caption);
                    saveitem.Add("ImageAlign", uploadImage.ImageAlign.ToString());
                    saveitem.Add("Width", uploadImage.Width);
                    saveitem.Add("Height", uploadImage.Height);
                    saveitem.Add("CropCoordX1", uploadImage.CropCoordX1);
                    saveitem.Add("CropCoordY1", uploadImage.CropCoordY1);
                    saveitem.Add("CropCoordX2", uploadImage.CropCoordX2);
                    saveitem.Add("CropCoordY2", uploadImage.CropCoordY2);
                    saveitem.Add("OriginalWidth", uploadImage.OriginalWidth);
                    saveitem.Add("OriginalHeight", uploadImage.OriginalHeight);
                    saveitem.Add("OriginalSize", uploadImage.OriginalSize);
                    saveitem.Add("OriginalUri", uploadImage.OriginalUri);
                    saveitem.Add("PublicOriginallUrl", uploadImage.PublicOriginallUrl);
                    saveitem.Add("IsOriginalUploaded", uploadImage.IsOriginalUploaded);
                    saveitem.Add("ThumbnailWidth", uploadImage.ThumbnailWidth);
                    saveitem.Add("ThumbnailHeight", uploadImage.ThumbnailHeight);
                    saveitem.Add("ThumbnailSize", uploadImage.ThumbnailSize);
                   

                    saveitem.Add("PublicThumbnailUrl", uploadImage.PublicThumbnailUrl);
                    saveitem.Add("IsThumbnailUploaded", uploadImage.IsThumbnailUploaded);
                    saveitem.Add("Flag", 1);
                    string js = JsonConvert.SerializeObject(saveitem);
                    var model = _webClient.DownloadData<string>("MediaManager/SaveImageQuery", new { JS = js });
                    uploadImage.Id = new Guid(model);

                }
                catch (Exception e)
                {

                }


                unitOfWork.Commit();

                StartTasksForImage(uploadImage, fileStream, thumbnailFileStream, false);
            }
            finally
            {
                if (thumbnailFileStream != null)
                {
                    thumbnailFileStream.Dispose();
                }
            }

            return uploadImage;
        }

        private void ReadParametersFormVectorImage(Stream fileStream, ref Size size)
        {
            XDocument xdocument = XDocument.Load(fileStream);
            var root = xdocument.Root;
            if (root == null || root.Name.LocalName != "svg")
            {
                const string message = "An error has ocurred while trying to read the file";
                throw new ValidationException(() => message, message);
            }
            var attributes = root.Attributes().ToList();

            var widthAttribute = attributes.FirstOrDefault(x => x.Name == "width");

            var heightAttribute = attributes.FirstOrDefault(x => x.Name == "height");

            /* Assume that attribute may look something like '1024px'. We need to match int value */
            Match match;
            int value;
            var regex = new Regex(@"^\d+");
            if (widthAttribute != null)
            {
                match = regex.Match(widthAttribute.Value);
                if (int.TryParse(match.Value, out value))
                {
                    size.Width = value;
                }
            }
            if (heightAttribute != null)
            {
                match = regex.Match(heightAttribute.Value);
                if (int.TryParse(match.Value, out value))
                {
                    size.Height = value;
                }
            }
            if (size.Height * size.Width == 0)
            {
                size.Height = -1;
                size.Width = -1;
            }
        }

        public MediaImage UploadImageWithStream(Stream fileStream, MediaImage image, bool waitForUploadResult = false)
        {
            using (var thumbnailFileStream = new MemoryStream())
            {
                fileStream = RotateImage(fileStream);
                var size = GetSize(fileStream);

                CreatePngThumbnail(fileStream, thumbnailFileStream, ThumbnailSize);

                var folderName = mediaFileService.CreateRandomFolderName();
                var publicFileName = MediaHelper.RemoveInvalidPathSymbols(MediaImageHelper.CreatePublicFileName(image.OriginalFileName, image.OriginalFileExtension));

                // Create new original image and upload file stream to the storage
                var originalImage = CreateImage(null, image.OriginalFileName, image.OriginalFileExtension, Path.GetFileName(image.Title), size, image.Size, image);
                SetThumbnailParameters(originalImage, ThumbnailSize, thumbnailFileStream.Length);
                mediaImageVersionPathService.SetPathForNewOriginal(originalImage, folderName, publicFileName, ImageHelper.GetImageType(image.OriginalFileExtension));

                unitOfWork.BeginTransaction();
                //  repository.Save(originalImage);
                try
                {

                    JObject saveitem = new JObject();
                    saveitem.Add("Id", originalImage.Id);
                    saveitem.Add("Version", originalImage.Version);
                    saveitem.Add("IsDeleted", originalImage.IsDeleted);
                    saveitem.Add("Title", originalImage.Title);
                    saveitem.Add("Type", originalImage.Type.ToString());
                    saveitem.Add("ContentType", originalImage.ContentType.ToString());
                    saveitem.Add("IsArchived", originalImage.IsArchived);
                    saveitem.Add("Description", originalImage.Description);
                    saveitem.Add("OriginalFileName", originalImage.OriginalFileName);
                    saveitem.Add("OriginalFileExtension", originalImage.OriginalFileExtension);
                    saveitem.Add("FileUri", originalImage.FileUri);
                    saveitem.Add("PublicUrl", originalImage.PublicUrl);
                    saveitem.Add("Size", originalImage.Size);
                    saveitem.Add("IsTemporary", originalImage.IsTemporary);
                    saveitem.Add("IsUploaded", originalImage.IsUploaded);
                    saveitem.Add("IsCanceled", originalImage.IsCanceled);
                    saveitem.Add("IsMovedToTrash", originalImage.IsMovedToTrash);
                    saveitem.Add("NextTryToMoveToTrash ", originalImage.NextTryToMoveToTrash);
                    saveitem.Add("ThumbnailUri ", originalImage.ThumbnailUri.AbsolutePath);
                    saveitem.Add("Caption", originalImage.Caption);
                    saveitem.Add("ImageAlign", originalImage.ImageAlign.ToString());
                    saveitem.Add("Width", originalImage.Width);
                    saveitem.Add("Height", originalImage.Height);
                    saveitem.Add("CropCoordX1", originalImage.CropCoordX1);
                    saveitem.Add("CropCoordY1", originalImage.CropCoordY1);
                    saveitem.Add("CropCoordX2", originalImage.CropCoordX2);
                    saveitem.Add("CropCoordY2", originalImage.CropCoordY2);
                    saveitem.Add("OriginalWidth", originalImage.OriginalWidth);
                    saveitem.Add("OriginalHeight", originalImage.OriginalHeight);
                    saveitem.Add("OriginalSize", originalImage.OriginalSize);
                    saveitem.Add("OriginalUri", originalImage.OriginalUri);
                    saveitem.Add("PublicOriginallUrl", originalImage.PublicOriginallUrl);
                    saveitem.Add("IsOriginalUploaded", originalImage.IsOriginalUploaded);
                    saveitem.Add("ThumbnailWidth", originalImage.ThumbnailWidth);
                    saveitem.Add("ThumbnailHeight", originalImage.ThumbnailHeight);
                    saveitem.Add("ThumbnailSize", originalImage.ThumbnailSize);
                    

                    saveitem.Add("PublicThumbnailUrl", originalImage.PublicThumbnailUrl);
                    saveitem.Add("IsThumbnailUploaded", originalImage.IsThumbnailUploaded);
                    saveitem.Add("Flag", 1);
                    string js = JsonConvert.SerializeObject(saveitem);
                    var model = _webClient.DownloadData<string>("MediaManager/SaveImageQuery", new { JS = js });
                    originalImage.Id = new Guid(model);
                }
                catch (Exception e)
                {

                }



                if (!waitForUploadResult)
                {
                    unitOfWork.Commit();
                }

                StartTasksForImage(originalImage, fileStream, thumbnailFileStream, false, waitForUploadResult);

                if (waitForUploadResult)
                {
                    unitOfWork.Commit();
                    Events.MediaManagerEvents.Instance.OnMediaFileUpdated(originalImage);
                }

                return originalImage;
            }
        }

        /// <summary>
        /// Gets a size of the image.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        /// <returns>A size of the image.</returns>
        public Size GetImageSize(Stream imageStream)
        {
            try
            {
                imageStream.Seek(0, SeekOrigin.Begin);

                using (var img = Image.FromStream(imageStream))
                {
                    return img.Size;
                }
            }
            catch (ArgumentException e)
            {
                throw new ImagingException(string.Format("Stream {0} is not valid image stream. Can not determine image size.", imageStream.GetType()), e);
            }
        }

        /// <summary>
        /// Updates the thumbnail.
        /// </summary>
        /// <param name="mediaImage">The media image.</param>
        /// <param name="size">The size.</param>
        public void UpdateThumbnail(MediaImage mediaImage, Size size)
        {
            if (size.IsEmpty)
            {
                size = ThumbnailSize;
            }

            var downloadResponse = storageService.DownloadObject(mediaImage.FileUri);
            var imageType = ImageHelper.GetImageType(mediaImage.OriginalFileExtension);
            using (var memoryStream = new MemoryStream())
            {
                if (imageType == ImageType.Raster)
                {
                    CreatePngThumbnail(downloadResponse.ResponseStream, memoryStream, size);
                }
                else
                {
                    CreateSvgThumbnail(downloadResponse.ResponseStream, memoryStream, size);
                }

                mediaImage.ThumbnailWidth = size.Width;
                mediaImage.ThumbnailHeight = size.Height;
                mediaImage.ThumbnailSize = memoryStream.Length;

                storageService.UploadObject(new UploadRequest { InputStream = memoryStream, Uri = mediaImage.ThumbnailUri, IgnoreAccessControl = true });
            }
        }

        /// <summary>
        /// Makes image as original.
        /// </summary>
        /// <param name="image">The new original image.</param>
        /// <param name="originalImage">The current original image.</param>
        /// <param name="archivedImage">The archived image.</param>
        /// <param name="overrideUrl">To override public Url ot not.</param>
        /// <returns>The new original image.</returns>
        public MediaImage MakeAsOriginal(MediaImage image, MediaImage originalImage, MediaImage archivedImage, bool overrideUrl = true)
        {
            overrideUrl = false; // TODO: temporary disabling feature #1055.
            var folderName = Path.GetFileName(Path.GetDirectoryName(originalImage.FileUri.OriginalString));

            using (var fileStream = DownloadFileStream(image.PublicUrl))
            {
                string publicUrlTemp = string.Empty,
                    publicThumbnailUrlTemp = string.Empty,
                    publicOriginallUrlTemp = string.Empty;
                Uri fileUriTemp = null, thumbnailUriTemp = null, originalUriTemp = null;

                if (overrideUrl)
                {
                    publicUrlTemp = originalImage.PublicUrl;
                    fileUriTemp = originalImage.FileUri;
                    publicThumbnailUrlTemp = originalImage.PublicThumbnailUrl;
                    thumbnailUriTemp = originalImage.ThumbnailUri;
                    publicOriginallUrlTemp = originalImage.PublicOriginallUrl;
                    originalUriTemp = originalImage.OriginalUri;
                }

                image.CopyDataTo(originalImage, false);
                MediaHelper.SetCollections(repository, image, originalImage);

                if (!overrideUrl)
                {
                    var publicFileName = MediaHelper.RemoveInvalidPathSymbols(MediaImageHelper.CreateVersionedFileName(originalImage.OriginalFileName, GetVersion(originalImage)));
                    mediaImageVersionPathService.SetPathForNewOriginal(originalImage, folderName, publicFileName, ImageHelper.GetImageType(originalImage.OriginalFileExtension), archivedImage.OriginalUri, archivedImage.PublicOriginallUrl);
                }
                else
                {
                    originalImage.PublicUrl = publicUrlTemp;
                    originalImage.FileUri = fileUriTemp;
                    originalImage.PublicThumbnailUrl = publicThumbnailUrlTemp;
                    originalImage.ThumbnailUri = thumbnailUriTemp;
                    originalImage.PublicOriginallUrl = publicOriginallUrlTemp;
                    originalImage.OriginalUri = originalUriTemp;
                }


                originalImage.Original = null;
                originalImage.PublishedOn = DateTime.Now;

                if (image.IsEdited())
                {
                    originalImage.PublicOriginallUrl = image.PublicOriginallUrl;
                    originalImage.OriginalUri = image.OriginalUri;
                }

                unitOfWork.BeginTransaction();
                //repository.Save(originalImage);
                // set flag=2 for update 
                JObject saveitem = new JObject();
                saveitem.Add("Id", originalImage.Id);
                saveitem.Add("Version", originalImage.Version);
                saveitem.Add("IsDeleted", originalImage.IsDeleted);
                saveitem.Add("Title", originalImage.Title);
                saveitem.Add("Type", originalImage.Type.ToString());
                saveitem.Add("ContentType", originalImage.ContentType.ToString());
                saveitem.Add("IsArchived", originalImage.IsArchived);
                saveitem.Add("Description", originalImage.Description);
                saveitem.Add("OriginalFileName", originalImage.OriginalFileName);
                saveitem.Add("OriginalFileExtension", originalImage.OriginalFileExtension);
                saveitem.Add("FileUri", originalImage.FileUri);
                saveitem.Add("PublicUrl", originalImage.PublicUrl);
                saveitem.Add("Size", originalImage.Size);
                saveitem.Add("IsTemporary", originalImage.IsTemporary);
                saveitem.Add("IsUploaded", originalImage.IsUploaded);
                saveitem.Add("IsCanceled", originalImage.IsCanceled);
                saveitem.Add("IsMovedToTrash", originalImage.IsMovedToTrash);
                saveitem.Add("NextTryToMoveToTrash ", originalImage.NextTryToMoveToTrash);

                saveitem.Add("ThumbnailUri ", originalImage.ThumbnailUri.AbsolutePath);
                saveitem.Add("Caption", originalImage.Caption);
                saveitem.Add("ImageAlign", originalImage.ImageAlign.ToString());
                saveitem.Add("Width", originalImage.Width);
                saveitem.Add("Height", originalImage.Height);
                saveitem.Add("CropCoordX1", originalImage.CropCoordX1);
                saveitem.Add("CropCoordY1", originalImage.CropCoordY1);
                saveitem.Add("CropCoordX2", originalImage.CropCoordX2);
                saveitem.Add("CropCoordY2", originalImage.CropCoordY2);
                saveitem.Add("OriginalWidth", originalImage.OriginalWidth);
                saveitem.Add("OriginalHeight", originalImage.OriginalHeight);
                saveitem.Add("OriginalSize", originalImage.OriginalSize);
                saveitem.Add("OriginalUri", originalImage.OriginalUri);
                saveitem.Add("PublicOriginallUrl", originalImage.PublicOriginallUrl);
                saveitem.Add("IsOriginalUploaded", originalImage.IsOriginalUploaded);
                saveitem.Add("ThumbnailWidth", originalImage.ThumbnailWidth);
                saveitem.Add("ThumbnailHeight", originalImage.ThumbnailHeight);
                saveitem.Add("ThumbnailSize", originalImage.ThumbnailSize);
                

                saveitem.Add("PublicThumbnailUrl", originalImage.PublicThumbnailUrl);
                saveitem.Add("IsThumbnailUploaded", originalImage.IsThumbnailUploaded);
                saveitem.Add("Flag", 2);
                string js = JsonConvert.SerializeObject(saveitem);
                var model = _webClient.DownloadData<string>("MediaManager/SaveImageQuery", new { JS = js });

                unitOfWork.Commit();

                if (!image.IsEdited())
                {
                    using (var fileStreamReplica = new MemoryStream())
                    {
                        fileStream.CopyTo(fileStreamReplica);
                        storageService.UploadObject(new UploadRequest { InputStream = fileStreamReplica, Uri = originalImage.OriginalUri, IgnoreAccessControl = true });
                    }
                }
                storageService.UploadObject(new UploadRequest { InputStream = fileStream, Uri = originalImage.FileUri, IgnoreAccessControl = true });

                UpdateThumbnail(originalImage, Size.Empty);

                return originalImage;
            }
        }

        /// <summary>
        /// Saves edited image as original.
        /// </summary>
        /// <param name="image">The edited image.</param>
        /// <param name="archivedImage">The archived image.</param>
        /// <param name="croppedImageFileStream">The stream with edited image.</param>
        /// <param name="overrideUrl">To override public url or not.</param>
        public void SaveEditedImage(MediaImage image, MediaImage archivedImage, MemoryStream croppedImageFileStream, bool overrideUrl = true)
        {
            overrideUrl = false; // TODO: temporary disabling feature #1055.
            var folderName = Path.GetFileName(Path.GetDirectoryName(image.FileUri.OriginalString));

            using (var fileStream = croppedImageFileStream ?? DownloadFileStream(image.PublicUrl))
            {
                image.Original = null;
                image.PublishedOn = DateTime.Now;

                if (!overrideUrl)
                {
                    var publicFileName = MediaHelper.RemoveInvalidPathSymbols(MediaImageHelper.CreateVersionedFileName(image.OriginalFileName, GetVersion(image)));
                    mediaImageVersionPathService.SetPathForNewOriginal(image, folderName, publicFileName, ImageHelper.GetImageType(image.OriginalFileExtension), archivedImage.OriginalUri, archivedImage.PublicOriginallUrl);
                }

                unitOfWork.BeginTransaction();
                // repository.Save(image);

                try
                {

                    JObject saveitem = new JObject();
                    saveitem.Add("Id", image.Id);
                    saveitem.Add("Version", image.Version);
                    saveitem.Add("IsDeleted", image.IsDeleted);
                    saveitem.Add("Title", image.Title);
                    saveitem.Add("Type", image.Type.ToString());
                    saveitem.Add("ContentType", image.ContentType.ToString());
                    saveitem.Add("IsArchived", image.IsArchived);
                    saveitem.Add("Description", image.Description);
                    saveitem.Add("OriginalFileName", image.OriginalFileName);
                    saveitem.Add("OriginalFileExtension", image.OriginalFileExtension);
                    saveitem.Add("FileUri", image.FileUri);
                    saveitem.Add("PublicUrl", image.PublicUrl);
                    saveitem.Add("Size", image.Size);
                    saveitem.Add("IsTemporary", image.IsTemporary);
                    saveitem.Add("IsUploaded", image.IsUploaded);
                    saveitem.Add("IsCanceled", image.IsCanceled);
                    saveitem.Add("IsMovedToTrash", image.IsMovedToTrash);
                    saveitem.Add("ThumbnailUri ", image.ThumbnailUri.AbsolutePath);
                    saveitem.Add("Caption", image.Caption);
                    saveitem.Add("ImageAlign", image.ImageAlign.ToString());
                    saveitem.Add("Width", image.Width);
                    saveitem.Add("Height", image.Height);
                    saveitem.Add("CropCoordX1", image.CropCoordX1);
                    saveitem.Add("CropCoordY1", image.CropCoordY1);
                    saveitem.Add("CropCoordX2", image.CropCoordX2);
                    saveitem.Add("CropCoordY2", image.CropCoordY2);
                    saveitem.Add("OriginalWidth", image.OriginalWidth);
                    saveitem.Add("OriginalHeight", image.OriginalHeight);
                    saveitem.Add("OriginalSize", image.OriginalSize);
                    saveitem.Add("OriginalUri", image.OriginalUri);
                    saveitem.Add("PublicOriginallUrl", image.PublicOriginallUrl);
                    saveitem.Add("IsOriginalUploaded", image.IsOriginalUploaded);
                    saveitem.Add("ThumbnailWidth", image.ThumbnailWidth);
                    saveitem.Add("ThumbnailHeight", image.ThumbnailHeight);
                    saveitem.Add("ThumbnailSize", image.ThumbnailSize);
                    

                    saveitem.Add("PublicThumbnailUrl", image.PublicThumbnailUrl);
                    saveitem.Add("IsThumbnailUploaded", image.IsThumbnailUploaded);
                    saveitem.Add("Flag", 2);

                    string js = JsonConvert.SerializeObject(saveitem);
                    var model = _webClient.DownloadData<string>("MediaManager/SaveImageQuery", new { JS = js });
                    image.Id = new Guid(model);

                }
                catch (Exception e)
                {

                }


                unitOfWork.Commit();

                storageService.UploadObject(new UploadRequest { InputStream = fileStream, Uri = image.FileUri, IgnoreAccessControl = true });
                UpdateThumbnail(image, Size.Empty);
            }
        }

        public void SaveImage(MediaImage image)
        {
            unitOfWork.BeginTransaction();
            //repository.Save(image);
            try
            {

                JObject saveitem = new JObject();
                saveitem.Add("Id", image.Id);
                saveitem.Add("Version", image.Version);
                saveitem.Add("IsDeleted", image.IsDeleted);
                saveitem.Add("Title", image.Title);
                saveitem.Add("Type", image.Type.ToString());
                saveitem.Add("ContentType", image.ContentType.ToString());
                saveitem.Add("IsArchived", image.IsArchived);
                saveitem.Add("Description", image.Description);
                saveitem.Add("OriginalFileName", image.OriginalFileName);
                saveitem.Add("OriginalFileExtension", image.OriginalFileExtension);
                saveitem.Add("FileUri", image.FileUri);
                saveitem.Add("PublicUrl", image.PublicUrl);
                saveitem.Add("Size", image.Size);
                saveitem.Add("IsTemporary", image.IsTemporary);
                saveitem.Add("IsUploaded", image.IsUploaded);
                saveitem.Add("IsCanceled", image.IsCanceled);
                saveitem.Add("IsMovedToTrash", image.IsMovedToTrash);
                saveitem.Add("ThumbnailUri ", image.ThumbnailUri.AbsolutePath);
                saveitem.Add("Caption", image.Caption);
                saveitem.Add("ImageAlign", image.ImageAlign.ToString());
                saveitem.Add("Width", image.Width);
                saveitem.Add("Height", image.Height);
                saveitem.Add("CropCoordX1", image.CropCoordX1);
                saveitem.Add("CropCoordY1", image.CropCoordY1);
                saveitem.Add("CropCoordX2", image.CropCoordX2);
                saveitem.Add("CropCoordY2", image.CropCoordY2);
                saveitem.Add("OriginalWidth", image.OriginalWidth);
                saveitem.Add("OriginalHeight", image.OriginalHeight);
                saveitem.Add("OriginalSize", image.OriginalSize);
                saveitem.Add("OriginalUri", image.OriginalUri);
                saveitem.Add("PublicOriginallUrl", image.PublicOriginallUrl);
                saveitem.Add("IsOriginalUploaded", image.IsOriginalUploaded);
                saveitem.Add("ThumbnailWidth", image.ThumbnailWidth);
                saveitem.Add("ThumbnailHeight", image.ThumbnailHeight);
                saveitem.Add("ThumbnailSize", image.ThumbnailSize);
                

                saveitem.Add("PublicThumbnailUrl", image.PublicThumbnailUrl);
                saveitem.Add("IsThumbnailUploaded", image.IsThumbnailUploaded);
                saveitem.Add("Flag", 1);

                string js = JsonConvert.SerializeObject(saveitem);
                var model = _webClient.DownloadData<string>("MediaManager/SaveImageQuery", new { JS = js });
                image.Id = new Guid(model);
            }
            catch (Exception e)
            {

            }



            unitOfWork.Commit();
        }

        /// <summary>
        /// Moves current original image to history.
        /// </summary>
        /// <param name="originalImage">The current original image.</param>
        /// <returns>The archived image.</returns>
        public MediaImage MoveToHistory(MediaImage originalImage)
        {
            var clonnedOriginalImage = (MediaImage)originalImage.Clone();
            clonnedOriginalImage.Original = originalImage;

            var historicalFileName = MediaImageHelper.CreateHistoricalVersionedFileName(
                                originalImage.OriginalFileName,
                                originalImage.OriginalFileExtension);

            var folderName = Path.GetFileName(Path.GetDirectoryName(originalImage.FileUri.OriginalString));

            using (var originalFileStream = DownloadFileStream(clonnedOriginalImage.PublicUrl))
            {
                using (var originalThumbnailFileStream = DownloadFileStream(clonnedOriginalImage.PublicThumbnailUrl))
                {
                    mediaImageVersionPathService.SetPathForArchive(clonnedOriginalImage, folderName, historicalFileName);

                    unitOfWork.BeginTransaction();
                    repository.Save(clonnedOriginalImage);
                    // set flag=2 for update
                    //JObject saveitem = new JObject();
                    //saveitem.Add("Id", clonnedOriginalImage.Id);
                    //saveitem.Add("Version", clonnedOriginalImage.Version);
                    //saveitem.Add("IsDeleted", clonnedOriginalImage.IsDeleted);
                    //saveitem.Add("Title", clonnedOriginalImage.Title);
                    //saveitem.Add("Type", clonnedOriginalImage.Type.ToString());
                    //saveitem.Add("ContentType", clonnedOriginalImage.ContentType.ToString());
                    //saveitem.Add("IsArchived", clonnedOriginalImage.IsArchived);
                    //saveitem.Add("Description", clonnedOriginalImage.Description);
                    //saveitem.Add("OriginalFileName", clonnedOriginalImage.OriginalFileName);
                    //saveitem.Add("OriginalFileExtension", clonnedOriginalImage.OriginalFileExtension);
                    //saveitem.Add("FileUri", clonnedOriginalImage.FileUri);
                    //saveitem.Add("PublicUrl", clonnedOriginalImage.PublicUrl);
                    //saveitem.Add("Size", clonnedOriginalImage.Size);
                    //saveitem.Add("IsTemporary", clonnedOriginalImage.IsTemporary);
                    //saveitem.Add("IsUploaded", clonnedOriginalImage.IsUploaded);
                    //saveitem.Add("IsCanceled", clonnedOriginalImage.IsCanceled);
                    //saveitem.Add("IsMovedToTrash", clonnedOriginalImage.IsMovedToTrash);
                    //saveitem.Add("ThumbnailUri ", clonnedOriginalImage.ThumbnailUri.AbsolutePath);
                    //saveitem.Add("Caption", clonnedOriginalImage.Caption);
                    //saveitem.Add("ImageAlign", clonnedOriginalImage.ImageAlign.ToString());
                    //saveitem.Add("Width", clonnedOriginalImage.Width);
                    //saveitem.Add("Height", clonnedOriginalImage.Height);
                    //saveitem.Add("CropCoordX1", clonnedOriginalImage.CropCoordX1);
                    //saveitem.Add("CropCoordY1", clonnedOriginalImage.CropCoordY1);
                    //saveitem.Add("CropCoordX2", clonnedOriginalImage.CropCoordX2);
                    //saveitem.Add("CropCoordY2", clonnedOriginalImage.CropCoordY2);
                    //saveitem.Add("OriginalWidth", clonnedOriginalImage.OriginalWidth);
                    //saveitem.Add("OriginalHeight", clonnedOriginalImage.OriginalHeight);
                    //saveitem.Add("OriginalSize", clonnedOriginalImage.OriginalSize);
                    //saveitem.Add("OriginalUri", clonnedOriginalImage.OriginalUri);
                    //saveitem.Add("PublicOriginallUrl", clonnedOriginalImage.PublicOriginallUrl);
                    //saveitem.Add("IsOriginalUploaded", clonnedOriginalImage.IsOriginalUploaded);
                    //saveitem.Add("ThumbnailWidth", clonnedOriginalImage.ThumbnailWidth);
                    //saveitem.Add("ThumbnailHeight", clonnedOriginalImage.ThumbnailHeight);
                    //saveitem.Add("ThumbnailSize", clonnedOriginalImage.ThumbnailSize);
                   

                    //saveitem.Add("PublicThumbnailUrl", clonnedOriginalImage.PublicThumbnailUrl);
                    //saveitem.Add("IsThumbnailUploaded", clonnedOriginalImage.IsThumbnailUploaded);
                    //saveitem.Add("Flag", 2);

                    //string js = JsonConvert.SerializeObject(saveitem);
                    //var model = _webClient.DownloadData<string>("MediaManager/SaveImageQuery", new { JS = js });


                    unitOfWork.Commit();

                    StartTasksForImage(clonnedOriginalImage, originalFileStream, originalThumbnailFileStream, originalImage.IsEdited());
                }
            }
            return clonnedOriginalImage;
        }

        #region Private methods

        private MediaImage RevertChanges(MediaImage canceledImage)
        {
            var previousOriginal =
                repository.AsQueryable<MediaImage>().OrderByDescending(i => i.PublishedOn).FirstOrDefault(f => f.Original != null && f.Original.Id == canceledImage.Id);

            if (previousOriginal != null)
            {
                var folderName = Path.GetFileName(Path.GetDirectoryName(previousOriginal.FileUri.OriginalString));
                var publicFileName = MediaHelper.RemoveInvalidPathSymbols(MediaImageHelper.CreatePublicFileName(previousOriginal.OriginalFileName, previousOriginal.OriginalFileExtension));

                // Get original file stream
                using (var fileStream = DownloadFileStream(previousOriginal.PublicUrl))
                {
                    // Get thumbnail file stream
                    using (var thumbnailFileStream = DownloadFileStream(previousOriginal.PublicThumbnailUrl))
                    {
                        previousOriginal.CopyDataTo(canceledImage);

                        mediaImageVersionPathService.SetPathForArchive(canceledImage, folderName, publicFileName);

                        StartTasksForImage(canceledImage, fileStream, thumbnailFileStream, previousOriginal.IsEdited());

                        canceledImage.Original = null;
                        unitOfWork.BeginTransaction();
                        // repository.Save(canceledImage);

                        try
                        {

                            JObject saveitem = new JObject();
                            saveitem.Add("Id", canceledImage.Id);
                            saveitem.Add("Version", canceledImage.Version);
                            saveitem.Add("IsDeleted", canceledImage.IsDeleted);
                            saveitem.Add("Title", canceledImage.Title);
                            saveitem.Add("Type", canceledImage.Type.ToString());
                            saveitem.Add("ContentType", canceledImage.ContentType.ToString());
                            saveitem.Add("IsArchived", canceledImage.IsArchived);
                            saveitem.Add("Description", canceledImage.Description);
                            saveitem.Add("OriginalFileName", canceledImage.OriginalFileName);
                            saveitem.Add("OriginalFileExtension", canceledImage.OriginalFileExtension);
                            saveitem.Add("FileUri", canceledImage.FileUri);
                            saveitem.Add("PublicUrl", canceledImage.PublicUrl);
                            saveitem.Add("Size", canceledImage.Size);
                            saveitem.Add("IsTemporary", canceledImage.IsTemporary);
                            saveitem.Add("IsUploaded", canceledImage.IsUploaded);
                            saveitem.Add("IsCanceled", canceledImage.IsCanceled);
                            saveitem.Add("IsMovedToTrash", canceledImage.IsMovedToTrash);
                            saveitem.Add("ThumbnailUri ", canceledImage.ThumbnailUri.AbsolutePath);
                            saveitem.Add("Caption", canceledImage.Caption);
                            saveitem.Add("ImageAlign", canceledImage.ImageAlign.ToString());
                            saveitem.Add("Width", canceledImage.Width);
                            saveitem.Add("Height", canceledImage.Height);
                            saveitem.Add("CropCoordX1", canceledImage.CropCoordX1);
                            saveitem.Add("CropCoordY1", canceledImage.CropCoordY1);
                            saveitem.Add("CropCoordX2", canceledImage.CropCoordX2);
                            saveitem.Add("CropCoordY2", canceledImage.CropCoordY2);
                            saveitem.Add("OriginalWidth", canceledImage.OriginalWidth);
                            saveitem.Add("OriginalHeight", canceledImage.OriginalHeight);
                            saveitem.Add("OriginalSize", canceledImage.OriginalSize);
                            saveitem.Add("OriginalUri", canceledImage.OriginalUri);
                            saveitem.Add("PublicOriginallUrl", canceledImage.PublicOriginallUrl);
                            saveitem.Add("IsOriginalUploaded", canceledImage.IsOriginalUploaded);
                            saveitem.Add("ThumbnailWidth", canceledImage.ThumbnailWidth);
                            saveitem.Add("ThumbnailHeight", canceledImage.ThumbnailHeight);
                            saveitem.Add("ThumbnailSize", canceledImage.ThumbnailSize);
                            

                            saveitem.Add("PublicThumbnailUrl", canceledImage.PublicThumbnailUrl);
                            saveitem.Add("IsThumbnailUploaded", canceledImage.IsThumbnailUploaded);
                            saveitem.Add("Flag", 2);


                            string js = JsonConvert.SerializeObject(saveitem);
                            var model = _webClient.DownloadData<string>("MediaManager/SaveImageQuery", new { JS = js });
                            canceledImage.Id = new Guid(model);

                        }
                        catch (Exception e)
                        {

                        }


                        unitOfWork.Commit();
                    }
                }
            }

            return previousOriginal;
        }

        private Stream UpdateCodec(Stream fileStream, Stream originalFileStream)
        {
            var originalCodec = ImageHelper.GetImageCodec(Image.FromStream(originalFileStream));
            var uploadedImage = Image.FromStream(fileStream);
            var updatedWithCodecFileStream = new MemoryStream();
            uploadedImage.Save(updatedWithCodecFileStream, originalCodec, null);
            fileStream = updatedWithCodecFileStream;

            return fileStream;
        }

        private Stream RotateImage(Stream fileStream)
        {
            var originalImage = Image.FromStream(fileStream);

            if (originalImage.PropertyIdList.Contains(0x0112))
            {
                int rotationValue = originalImage.GetPropertyItem(0x0112).Value[0];
                var wasRotated = true;

                switch (rotationValue)
                {
                    case 1: // landscape, do nothing
                        break;

                    case 8: // rotated 90 right
                        // de-rotate:
                        originalImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;

                    case 3: // bottoms up
                        originalImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;

                    case 6: // rotated 90 left
                        originalImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    default:
                        wasRotated = false;
                        break;
                }

                if (wasRotated)
                {
                    var rotatedStream = new MemoryStream();
                    var codec = ImageHelper.GetImageCodec(originalImage);
                    if (codec == null)
                    {
                        originalImage.Save(rotatedStream, ImageFormat.Bmp);
                    }
                    else
                    {
                        originalImage.Save(rotatedStream, codec, null);
                    }
                    fileStream = rotatedStream;
                }
            }

            return fileStream;
        }

        private void UpdateImageProperties(
            MediaImage image,
            Guid? rootFolderId,
            string fileName,
            string extension,
            string imageTitle,
            Size size,
            long fileLength,
            long thumbnailImageLength)
        {
            if (rootFolderId != null && !((Guid)rootFolderId).HasDefaultValue())
            {
                image.Folder = repository.AsProxy<MediaFolder>((Guid)rootFolderId);
            }

            image.Title = Path.GetFileName(imageTitle);
            image.Caption = null;
            image.Size = fileLength;
            image.IsTemporary = true;


            image.OriginalFileName = fileName;
            image.OriginalFileExtension = extension;
            image.Type = MediaType.Image;

            image.Width = size.Width;
            image.Height = size.Height;


            image.CropCoordX1 = null;
            image.CropCoordY1 = null;
            image.CropCoordX2 = null;
            image.CropCoordY2 = null;

            image.OriginalWidth = size.Width;
            image.OriginalHeight = size.Height;
            image.OriginalSize = fileLength;

            image.ThumbnailWidth = ThumbnailSize.Width;
            image.ThumbnailHeight = ThumbnailSize.Height;
            image.ThumbnailSize = thumbnailImageLength;

            image.ImageAlign = null;

            image.IsUploaded = null;
            image.IsThumbnailUploaded = null;
            image.IsOriginalUploaded = null;
        }

        private void SetThumbnailParameters(MediaImage image, Size size, long length)
        {
            image.ThumbnailWidth = size.Width;
            image.ThumbnailHeight = size.Height;
            image.ThumbnailSize = length;
        }

        private MediaImage CreateImage(
            Guid? rootFolderId,
            string fileName,
            string extension,
            string imageTitle,
            Size size,
            long fileLength,
            MediaImage filledInImage = null)
        {
            MediaImage image;

            if (filledInImage == null)
            {
                image = new MediaImage();

                if (rootFolderId != null && !((Guid)rootFolderId).HasDefaultValue())
                {
                    image.Folder = repository.AsProxy<MediaFolder>((Guid)rootFolderId);
                }

                image.Title = imageTitle;
                image.Caption = null;
                image.Size = fileLength;
                image.IsTemporary = true;
            }
            else
            {
                image = filledInImage;
            }


            image.OriginalFileName = fileName;
            image.OriginalFileExtension = extension;
            image.Type = MediaType.Image;

            image.Width = size.Width;
            image.Height = size.Height;


            image.CropCoordX1 = null;
            image.CropCoordY1 = null;
            image.CropCoordX2 = null;
            image.CropCoordY2 = null;

            image.OriginalWidth = size.Width;
            image.OriginalHeight = size.Height;
            image.OriginalSize = fileLength;

            image.ImageAlign = null;

            image.IsUploaded = null;
            image.IsThumbnailUploaded = null;
            image.IsOriginalUploaded = null;

            return image;
        }

        private void CreatePngThumbnail(Stream sourceStream, Stream destinationStream, Size size)
        {
            using (var image = Image.FromStream(sourceStream))
            {
                Image destination = image;

                var diff = (destination.Width - destination.Height) / 2.0;
                if (diff > 0)
                {
                    var x1 = Convert.ToInt32(Math.Floor(diff));
                    var y1 = 0;
                    var x2 = destination.Height;
                    var y2 = destination.Height;
                    var rect = new Rectangle(x1, y1, x2, y2);
                    destination = ImageHelper.Crop(destination, rect);
                }
                else if (diff < 0)
                {
                    diff = Math.Abs(diff);

                    var x1 = 0;
                    var y1 = Convert.ToInt32(Math.Floor(diff));
                    var x2 = destination.Width;
                    var y2 = destination.Width;
                    var rect = new Rectangle(x1, y1, x2, y2);
                    destination = ImageHelper.Crop(destination, rect);
                }

                destination = ImageHelper.Resize(destination, size);

                destination.Save(destinationStream, ImageFormat.Png);
            }
        }

        private void CreateSvgThumbnail(Stream sourceStream, Stream destinationStream, Size size)
        {
            sourceStream.Seek(0, SeekOrigin.Begin);
            var xDocument = XDocument.Load(sourceStream);
            var root = xDocument.Root;
            if (root == null || root.Name.LocalName != "svg")
            {
                const string message = "An error has occured while trying to read the file";
                throw new ValidationException(() => message, message);
            }
            var attributes = root.Attributes().ToList();

            var widthAttribute = attributes.FirstOrDefault(x => x.Name == "width");
            if (widthAttribute == null)
            {
                widthAttribute = new XAttribute("width", size.Width.ToString());
                attributes.Add(widthAttribute);
            }
            else
            {
                widthAttribute.Value = size.Width.ToString();
            }
            var heightAttribute = attributes.FirstOrDefault(x => x.Name == "height");
            if (heightAttribute == null)
            {
                heightAttribute = new XAttribute("height", size.Height.ToString());
                attributes.Add(heightAttribute);
            }
            else
            {
                heightAttribute.Value = size.Height.ToString();
            }
            root.ReplaceAttributes(attributes);
            xDocument.Save(destinationStream);
        }

        private int GetVersion(MediaImage image)
        {
            JObject imagedetails = new JObject();

            imagedetails.Add("OriginalId", image.Id);
            string redirectjs = JsonConvert.SerializeObject(imagedetails);
            var response = _webClient.DownloadData<int>("MediaManager/GetVersionCount", new { JS = redirectjs });

            //var versionsCount = repository.AsQueryable<MediaImage>().Count(i => i.Original != null && i.Original.Id == image.Id);
            var versionsCount = response;
            return versionsCount;
        }

        private void ExecuteActionOnThreadSeparatedSessionWithNoConcurrencyTracking(Action<ISession> work)
        {
            using (var session = sessionFactoryProvider.OpenSession(false))
            {
                try
                {
                    lock (this)
                    {
                        work(session);
                    }
                }
                finally
                {
                    session.Close();
                }
            }
        }

        private MemoryStream DownloadFileStream(string fileUrl)
        {
            byte[] imageData;
            using (var wc = new WebClient())
            {
                imageData = wc.DownloadData(fileUrl);
            }
            return new MemoryStream(imageData);
        }

        private Size GetSize(Stream fileStream)
        {
            try
            {
                var size = GetImageSize(fileStream);
                return size;
            }
            catch (ImagingException ex)
            {
                var message = MediaGlobalization.MultiFileUpload_ImageFormatNotSuported;
                const string logMessage = "Failed to get image size.";
                throw new ValidationException(() => message, logMessage, ex);
            }
        }

        private void StartTasksForImage(
            MediaImage mediaImage,
            Stream fileStream,
            MemoryStream thumbnailFileStream,
            bool shouldNotUploadOriginal = false,
            bool waitForUploadResult = false)
        {
            if (waitForUploadResult)
            {
                StartTasksForImageSync(mediaImage, fileStream, thumbnailFileStream, shouldNotUploadOriginal);
            }
            else
            {
                StartTasksForImageAsync(mediaImage, fileStream, thumbnailFileStream, shouldNotUploadOriginal);
            }
        }

        private void StartTasksForImageSync(
            MediaImage mediaImage,
            Stream fileStream,
            MemoryStream thumbnailFileStream,
            bool shouldNotUploadOriginal = false)
        {
            mediaFileService.UploadMediaFileToStorageSync(
                fileStream,
                mediaImage.FileUri,
                mediaImage,
                img =>
                {
                    if (img != null)
                    {
                        img.IsUploaded = true;
                    }
                },
                img =>
                {
                    if (img != null)
                    {
                        img.IsUploaded = false;
                    }
                },
                true);

            if (!shouldNotUploadOriginal)
            {
                mediaFileService.UploadMediaFileToStorageSync(
                    fileStream,
                    mediaImage.OriginalUri,
                    mediaImage,
                    img =>
                    {
                        if (img != null)
                        {
                            img.IsOriginalUploaded = true;
                        }
                    },
                    img =>
                    {
                        if (img != null)
                        {
                            img.IsOriginalUploaded = false;
                        }
                    },
                    true);
            }

            mediaFileService.UploadMediaFileToStorageSync(
                thumbnailFileStream,
                mediaImage.ThumbnailUri,
                mediaImage,
                img =>
                {
                    if (img != null)
                    {
                        img.IsThumbnailUploaded = true;
                    }
                },
                img =>
                {
                    if (img != null)
                    {
                        img.IsThumbnailUploaded = false;
                    }
                },
                true);

            OnAfterUploadCompleted(mediaImage, shouldNotUploadOriginal);
        }

        private void StartTasksForImageAsync(
            MediaImage mediaImage,
            Stream fileStream,
            MemoryStream thumbnailFileStream,
            bool shouldNotUploadOriginal = false)
        {
            var allTasks = new List<Task>();

            var publicImageUpload = mediaFileService.UploadMediaFileToStorageAsync<MediaImage>(
                fileStream,
                mediaImage.FileUri,
                mediaImage.Id,
                (img, session) =>
                {
                    if (img != null)
                    {
                        img.IsUploaded = true;
                    }
                },
                img =>
                {
                    if (img != null)
                    {
                        img.IsUploaded = false;
                    }
                },
                true);
            allTasks.Add(publicImageUpload);

            var publicThumbnailUpload = mediaFileService.UploadMediaFileToStorageAsync<MediaImage>(
                thumbnailFileStream,
                mediaImage.ThumbnailUri,
                mediaImage.Id,
                (img, session) =>
                {
                    if (img != null)
                    {
                        img.IsThumbnailUploaded = true;
                    }
                },
                img =>
                {
                    if (img != null)
                    {
                        img.IsThumbnailUploaded = false;
                    }
                },
                true);
            allTasks.Add(publicThumbnailUpload);

            Task publicOriginalUpload = null;
            if (!shouldNotUploadOriginal)
            {
                publicOriginalUpload = mediaFileService.UploadMediaFileToStorageAsync<MediaImage>(
                fileStream,
                mediaImage.OriginalUri,
                mediaImage.Id,
                (img, session) =>
                {
                    if (img != null)
                    {
                        img.IsOriginalUploaded = true;
                    }
                },
                img =>
                {
                    if (img != null)
                    {
                        img.IsOriginalUploaded = false;
                    }
                },
                true);
                allTasks.Add(publicOriginalUpload);
            }

            allTasks.ForEach(task => task.ContinueWith((t) => { Log.Error("Error observed while executing parallel task during image upload.", t.Exception); }, TaskContinuationOptions.OnlyOnFaulted));

            Task.Factory.ContinueWhenAll(
                allTasks.ToArray(),
                result =>
                {
                    try
                    {
                        ExecuteActionOnThreadSeparatedSessionWithNoConcurrencyTracking(
                            session =>
                            {
                                var media = session.Get<MediaImage>(mediaImage.Id);
                                if (media != null)
                                {
                                    OnAfterUploadCompleted(media, shouldNotUploadOriginal);
                                }
                            });
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Failed to finalize upload.", ex);
                    }
                });

            publicImageUpload.Start();
            if (publicOriginalUpload != null)
            {
                publicOriginalUpload.Start();
            }
            if (publicThumbnailUpload != null)
            {
                publicThumbnailUpload.Start();
            }
        }

        private void OnAfterUploadCompleted(MediaImage media, bool shouldNotUploadOriginal)
        {
            var isUploaded = (media.IsUploaded.HasValue && media.IsUploaded.Value) || (media.IsThumbnailUploaded.HasValue && media.IsThumbnailUploaded.Value)
                                 || ((media.IsOriginalUploaded.HasValue && media.IsOriginalUploaded.Value) && shouldNotUploadOriginal);
            if (media.IsCanceled && isUploaded)
            {
                RemoveImageWithFiles(media.Id, media.Version, false, shouldNotUploadOriginal);
            }
        }

        #endregion
    }
}
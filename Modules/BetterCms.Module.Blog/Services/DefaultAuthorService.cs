// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAuthorService.cs" company="Devbridge Group LLC">
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

using BetterModules.Core.DataAccess;
using BetterModules.Core.DataAccess.DataContext;
using BetterCms.Module.Blog.Models;
using BetterCms.Module.Root.Models;

using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using BetterCms.Core.WebServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BetterCms.Core.Security;
using BetterCms.Module.Root;

namespace BetterCms.Module.Blog.Services
{
   
    public class DefaultAuthorService : IAuthorService
    {
        /// <summary>
        /// The unit of work
        /// </summary>
        private IUnitOfWork unitOfWork;

        private readonly IRepository repository;
        private ITWebClient _webClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAuthorService" /> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="repository">The repository.</param>
        public DefaultAuthorService(IUnitOfWork unitOfWork, IRepository repository)
        {
            this.unitOfWork = unitOfWork;
            this.repository = repository;
            _webClient = new TWebClient();
        }

        /// <summary>
        /// Gets the list of author lookup values.
        /// </summary>
        /// <returns>
        /// List of author lookup values.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<LookupKeyValue> GetAuthors()
        {
            Models.Author alias = null;
            LookupKeyValue lookupAlias = null;

            return unitOfWork.Session
                .QueryOver(() => alias)
                .SelectList(select => select
                    .Select(Projections.Cast(NHibernateUtil.String, Projections.Property<Models.Author>(c => c.Id))).WithAlias(() => lookupAlias.Key)
                    .Select(() => alias.Name).WithAlias(() => lookupAlias.Value)).Where(c => !c.IsDeleted)
                .OrderBy(o => o.Name).Asc()
                .TransformUsing(Transformers.AliasToBean<LookupKeyValue>())
                .List<LookupKeyValue>();
        }
        
        public Author CreateAuthor(string name, Guid? imageId, string description)
        {
            var author = new Author();

            author.Name = name;
            author.Description = description;
            author.Image = imageId.HasValue ? repository.AsProxy<MediaManager.Models.MediaImage>(imageId.Value) : null;

            //repository.Save(author);
            JObject authordetails = new JObject();

            authordetails.Add("Id", author.Id);
            authordetails.Add("version", author.Version);
            authordetails.Add("isdeleted", author.IsDeleted);
            authordetails.Add("name", name);

            authordetails.Add("imageId", imageId);
            authordetails.Add("description", description);
            string js = JsonConvert.SerializeObject(authordetails);
            var model = _webClient.DownloadData<string>("Blog/SaveAuthor", new { JS = js });
            unitOfWork.Commit();
            author.Id = new Guid(model);
            // Notify.
            Events.BlogEvents.Instance.OnAuthorCreated(author);

            return author;
        }

        public Author UpdateAuthor(Guid authorId, int version, string name, Guid? imageId, string description)
        {
            var author = repository.First<Author>(authorId);

            author.Name = name;
            author.Description = description;
            author.Version = version;
            author.Image = imageId.HasValue ? repository.AsProxy<MediaManager.Models.MediaImage>(imageId.Value) : null;

            JObject authordetails = new JObject();
            authordetails.Add("Id", authorId);
            authordetails.Add("version", version);
            authordetails.Add("name", name);
            authordetails.Add("imageId", imageId);
            authordetails.Add("description", description);
            string js = JsonConvert.SerializeObject(authordetails);
            var model = _webClient.DownloadData<string>("Blog/UpdateAuthor", new { JS = js });
            //repository.Save(author);
            unitOfWork.Commit();

            // Notify.
            Events.BlogEvents.Instance.OnAuthorUpdated(author);

            return author;
        }

        public void DeleteAuthor(Guid authorId, int version)
        {
            JObject response = _webClient.DownloadData<JObject>("Blog/DeleteAuthor", new { Id = authorId });
            var authordetails = new { Isdeleted = response.SelectToken("IsDeleted").Value<bool>(), Version = response.SelectToken("Version").Value<int>() };
            //var author = repository.Delete<Models.Author>(authorId, version);
            var author = new Author();
            author.Id = authorId;
            author.IsDeleted = authordetails.Isdeleted;
            author.Version = authordetails.Version;

            unitOfWork.Commit();

            // Notify.
            Events.BlogEvents.Instance.OnAuthorDeleted(author);
        }
    }
}
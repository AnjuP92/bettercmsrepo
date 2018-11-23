﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRenderingService.cs" company="Devbridge Group LLC">
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

using BetterCms.Core.Modules;
using BetterCms.Core.Modules.Registration;
using BetterCms.Module.Root.ViewModels;
using BetterCms.Module.Root.ViewModels.Rendering;

using Common.Logging;

using BetterModules.Core.Exceptions;

namespace BetterCms.Module.Root.Services
{
    public class DefaultRenderingService : IRenderingService
    {
        /// <summary>
        /// A current class logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The modules registration.
        /// </summary>
        private readonly ICmsModulesRegistration modulesRegistration;

        /// <summary>
        /// The CMS configuration.
        /// </summary>
        private readonly ICmsConfiguration cmsConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRenderingService" /> class.
        /// </summary>
        /// <param name="modulesRegistration">The modules registration.</param>
        /// <param name="cmsConfiguration">The CMS configuration.</param>
        public DefaultRenderingService(ICmsModulesRegistration modulesRegistration, ICmsConfiguration cmsConfiguration)
        {
            this.cmsConfiguration = cmsConfiguration;
            this.modulesRegistration = modulesRegistration;
        }

        /// <summary>
        /// Retrieves a list of registered JavaScript module includes.
        /// </summary>
        /// <returns>Enumerator of JavaScriptModuleViewModel objects.</returns>
        public IEnumerable<JavaScriptModuleInclude> GetJavaScriptIncludes()
        {
            IEnumerable<JavaScriptModuleInclude> model = Enumerable.Empty<JavaScriptModuleInclude>();

            try
            {
                var javaScriptModules = modulesRegistration.GetJavaScriptModules();

                if (javaScriptModules != null)
                {
                    model = javaScriptModules
                        .Select(
                            f => new JavaScriptModuleInclude
                            {
                                Name = f.Name,
                                IsAutoGenerated = f.IsAutoGenerated,
                                Path = f.IsAutoGenerated ? string.Format(RootModuleConstants.AutoGeneratedJsFilePathPattern, f.Name) : f.Path,
                                MinifiedPath = f.MinPath ?? f.Module.MinifiedJsPath,
                                FriendlyName = f.FriendlyName,
                                Links = new ProjectionsViewModel
                                {
                                    Projections = f.Links.OrderBy(x => x.Order)
                                },
                                Globalization = new ProjectionsViewModel
                                {
                                    Projections = f.Globalization.OrderBy(x => x.Order)
                                },
                                ShimConfig = f.ShimConfiguration != null ? new JavaScriptModuleShimConfigurationViewModel
                                                 {
                                                     Exports = f.ShimConfiguration.Exports,
                                                     Depends = f.ShimConfiguration.Depends
                                                 } : null
                            });
                }
            }
            catch (CoreException ex)
            {
                Log.Error("Failed to retrieve java script modules.", ex);
            }

            return model;
        }

        public IEnumerable<string> GetStyleSheetIncludes(bool includePrivateCssFiles, bool includePublicCssFiles, Type moduleDescriptorType = null)
        {
            var allIncludes = new List<CssIncludeDescriptor>();

            if (moduleDescriptorType != null)
            {
                var modules = modulesRegistration.GetCmsModules();
                foreach (var module in modules)
                {
                    if (module.GetType() == moduleDescriptorType)
                    {
                        allIncludes.AddRange(module.RegisterCssIncludes());
                    }
                }
            }
            else
            {
                allIncludes.AddRange(modulesRegistration.GetStyleSheetIncludes());
            }

            var includes = allIncludes
                                .Where(f => f.IsPublic && includePublicCssFiles || !f.IsPublic && includePrivateCssFiles);

            if (cmsConfiguration.UseMinifiedResources)
            {
                return includes.Select(f => string.IsNullOrEmpty(f.MinPath) 
                                                ? f.ContainerModule.MinifiedCssPath
                                                : f.MinPath)
                                .Distinct();
            }

            return includes.Select(f => f.Path).Distinct();
        }
    }
}
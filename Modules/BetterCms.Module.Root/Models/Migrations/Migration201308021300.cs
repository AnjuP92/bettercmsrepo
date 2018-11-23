﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Migration201308021300.cs" company="Devbridge Group LLC">
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
using BetterModules.Core.DataAccess.DataContext.Migrations;

using BetterModules.Core.DataAccess.DataContext.Migrations;
using BetterModules.Core.Models;

using FluentMigrator;

namespace BetterCms.Module.Root.Models.Migrations
{
    [Migration(201308021300)]
    public class Migration201308021300: DefaultMigration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Migration201308021300" /> class.
        /// </summary>
        public Migration201308021300()
            : base(RootModuleDescriptor.ModuleName)
        {
        }

        public override void Up()
        {
            // Create table for layout options
            Create
                .Table("LayoutOptions")
                .InSchema(SchemaName)
                .WithBaseColumns()
                .WithColumn("LayoutId").AsGuid().NotNullable()
                .WithColumn("Key").AsString(MaxLength.Name).NotNullable()
                .WithColumn("Type").AsInt32().NotNullable()
                .WithColumn("DefaultValue").AsString(MaxLength.Max).Nullable();

            Create
                .ForeignKey("FK_Cms_LayoutOptions_Type_Cms_OptionTypes_Id")
                .FromTable("LayoutOptions").InSchema(SchemaName).ForeignColumn("Type")
                .ToTable("OptionTypes").InSchema(SchemaName).PrimaryColumn("Id");

            Create
                .ForeignKey("FK_Cms_LayoutOptions_LayoutId_Cms_Layouts_Id")
                .FromTable("LayoutOptions").InSchema(SchemaName).ForeignColumn("LayoutId")
                .ToTable("Layouts").InSchema(SchemaName).PrimaryColumn("Id");

            Create
                .UniqueConstraint("UX_Cms_LayoutOptions_LayoutId_Key")
                .OnTable("LayoutOptions").WithSchema(SchemaName)
                .Columns(new[] { "LayoutId", "Key", "DeletedOn" });
        }
    }
}
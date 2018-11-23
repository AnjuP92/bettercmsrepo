INSERT [bcms_root].[Layouts] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [Name], [LayoutPath], [ModuleId], [PreviewUrl]) VALUES (N'b33d5da0-a438-4078-8755-2735e4e64cfe', 5, 0, CAST(N'2017-01-17 00:00:00.000' AS DateTime), N'Better CMS', CAST(N'2017-01-17 17:25:09.000' AS DateTime), N'BetterCMS', NULL, NULL, N'NewLayout', N'~/Areas/bcms-installation/Views/Shared/NewLayout.cshtml', NULL, NULL)



--[bcms_root].[Regions] 
INSERT [bcms_root].[Regions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [RegionIdentifier]) VALUES (N'e3e2e7fe-62df-4ba6-8321-6fdcc1691d8a', 1, 0, CAST(N'2017-01-09 14:59:21.000' AS DateTime), N'Better CMS', CAST(N'2017-01-09 14:59:21.000' AS DateTime), N'Better CMS', NULL, NULL, N'CMSMainContent')

--[bcms_root].[Regions]
INSERT [bcms_root].[Regions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [RegionIdentifier]) VALUES (N'9e1601e4-efcd-4ebb-ae67-b2ff10e372ba', 1, 0, CAST(N'2017-01-09 14:59:21.000' AS DateTime), N'Better CMS', CAST(N'2017-01-09 14:59:21.000' AS DateTime), N'Better CMS', NULL, NULL, N'CMSHeader')

--[bcms_root].[Regions]
INSERT [bcms_root].[Regions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [RegionIdentifier]) VALUES (N'd840205e-9580-4906-b6b7-b9a48cbf8aaa', 1, 0, CAST(N'2017-01-09 14:59:21.000' AS DateTime), N'Better CMS', CAST(N'2017-01-09 14:59:21.000' AS DateTime), N'Better CMS', NULL, NULL, N'CMSFooter')


--[bcms_root].[LayoutRegions]
INSERT [bcms_root].[LayoutRegions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [Description], [LayoutId], [RegionId]) VALUES (N'66aa8cc9-60f9-47e7-b3aa-a6f800e9e418', 1, 0, CAST(N'2017-01-11 14:11:34.000' AS DateTime), N'admin', CAST(N'2017-01-11 14:11:34.000' AS DateTime), N'admin', NULL, NULL, NULL, N'b33d5da0-a438-4078-8755-2735e4e64cfe', N'e3e2e7fe-62df-4ba6-8321-6fdcc1691d8a')

--[bcms_root].[LayoutRegions]
INSERT [bcms_root].[LayoutRegions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [Description], [LayoutId], [RegionId]) VALUES (N'eb3f9167-56f1-43a8-95f7-a6f800f3e040', 1, 0, CAST(N'2017-01-11 14:47:55.000' AS DateTime), N'admin', CAST(N'2017-01-11 14:47:55.000' AS DateTime), N'admin', NULL, NULL, NULL, N'b33d5da0-a438-4078-8755-2735e4e64cfe', N'd840205e-9580-4906-b6b7-b9a48cbf8aaa')

--[bcms_root].[LayoutRegions]
INSERT [bcms_root].[LayoutRegions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [Description], [LayoutId], [RegionId]) VALUES (N'4f5e1526-f918-4dc3-9f93-a6f800f3e045', 1, 0, CAST(N'2017-01-11 14:47:55.000' AS DateTime), N'admin', CAST(N'2017-01-11 14:47:55.000' AS DateTime), N'admin', NULL, NULL, NULL, N'b33d5da0-a438-4078-8755-2735e4e64cfe', N'9e1601e4-efcd-4ebb-ae67-b2ff10e372ba')


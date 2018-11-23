
--[bcms_root].[Contents]

INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser],
 [Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId])
 VALUES (N'762b6b50-e60f-479f-ba58-a70800dfdb4a', 1, 0, CAST(N'2017-01-27 13:35:02.000' AS DateTime), N'admin',
 CAST(N'2017-01-27 13:35:02.000' AS DateTime), N'admin', NULL, NULL, N'MyNewWidget', NULL, 3, CAST(N'2017-01-12 15:13:29.000' AS DateTime), 
 N'admin', null)
--[bcms_root].[Widgets]

INSERT [bcms_root].[Widgets] ([Id], [CategoryId]) VALUES (N'762b6b50-e60f-479f-ba58-a70800dfdb4a', NULL)

--[bcms_pages].[ServerControlWidgets]

INSERT [bcms_pages].[ServerControlWidgets] ([Id], [Url]) VALUES (N'762b6b50-e60f-479f-ba58-a70800dfdb4a', N'~/Areas/bcms-installation/Views/Widgets/MyNewWidget.cshtml')

--[bcms_root].[ContentOptions]

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], 
[DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) 
VALUES (N'f237da51-b3a7-4689-950c-a6f900ff899e', 8, 0, CAST(N'2017-01-12 15:30:23.000' AS DateTime), N'admin', 
CAST(N'2017-01-27 13:35:02.000' AS DateTime), N'admin', NULL, NULL, N'762b6b50-e60f-479f-ba58-a70800dfdb4a', N'Text', 1, N'Responsive', 1, NULL)

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], 
[DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) 
VALUES (N'2572e59a-9460-4ac7-8d16-a6f900ff899f', 8, 0, CAST(N'2017-01-12 15:30:23.000' AS DateTime), N'admin', 
CAST(N'2017-01-27 13:35:02.000' AS DateTime), N'admin', NULL, NULL, N'762b6b50-e60f-479f-ba58-a70800dfdb4a', N'Numberofsections', 2, N'4', 1, NULL)




--[bcms_root].[Contents]

INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn],
[DeletedByUser], [Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId]) 
VALUES (N'57cf6487-06da-4066-82eb-a70800ef2ca1', 1, 0, CAST(N'2017-01-27 14:30:48.000' AS DateTime), N'admin',
 CAST(N'2017-01-27 14:30:48.000' AS DateTime), N'admin', NULL, NULL, N'NewWidget', NULL, 3, CAST(N'2017-01-16 09:58:33.000' AS DateTime),
 N'admin', null)

--[bcms_root].[Widgets]

INSERT [bcms_root].[Widgets] ([Id], [CategoryId]) VALUES (N'57cf6487-06da-4066-82eb-a70800ef2ca1', NULL)
--[bcms_pages].[ServerControlWidgets]

INSERT [bcms_pages].[ServerControlWidgets] ([Id], [Url]) VALUES (N'57cf6487-06da-4066-82eb-a70800ef2ca1', N'~/Areas/bcms-installation/Views/Widgets/NewWidget.cshtml')


--[bcms_root].[ContentOptions]

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn],
[DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) 
VALUES (N'2f1c56e9-bf34-4a90-8cb5-a6fd00a4b36a', 7, 0, CAST(N'2017-01-16 09:59:39.000' AS DateTime), N'admin', 
CAST(N'2017-01-27 14:30:48.000' AS DateTime), N'admin', NULL, NULL, N'57cf6487-06da-4066-82eb-a70800ef2ca1', N'Text', 1, N'Responsive', 1, NULL)

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], 
[DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) 
VALUES (N'37b8327c-42f4-4914-95f5-a6fd00a4b36a', 7, 0, CAST(N'2017-01-16 09:59:39.000' AS DateTime), N'admin', 
CAST(N'2017-01-27 14:30:48.000' AS DateTime), N'admin', NULL, NULL, N'57cf6487-06da-4066-82eb-a70800ef2ca1', N'Numberofsections', 2, N'2', 1, NULL)

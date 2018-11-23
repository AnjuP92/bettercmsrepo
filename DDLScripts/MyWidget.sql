--[bcms_root].[Contents]

INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], 
[DeletedByUser], [Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId]) 
VALUES (N'10b7ddb5-6c2a-441e-a20d-a6fa00d0f587', 1, 0, CAST(N'2017-01-13 12:40:47.000' AS DateTime), N'admin', CAST(N'2017-01-13 12:40:47.000' AS DateTime),
 N'admin', NULL, NULL, N'MyWidget', NULL,3, CAST(N'2017-01-13 12:40:47.000' AS DateTime), N'admin', NULL)




--[bcms_root].[Widgets]

INSERT [bcms_root].[Widgets] ([Id], [CategoryId]) VALUES (N'10b7ddb5-6c2a-441e-a20d-a6fa00d0f587', NULL)
--[bcms_pages].[ServerControlWidgets]
INSERT [bcms_pages].[ServerControlWidgets] ([Id], [Url]) VALUES (N'10b7ddb5-6c2a-441e-a20d-a6fa00d0f587', N'~/Areas/bcms-installation/Views/Widgets/MyWidget.cshtml')

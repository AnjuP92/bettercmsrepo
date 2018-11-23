
--[bcms_root].[Contents]
INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser],
 [Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId]) 
 VALUES (N'253b91a0-a43d-4364-b614-a70800d2699a', 1, 0, CAST(N'2017-01-27 12:46:05.000' AS DateTime), N'admin', 
 CAST(N'2017-01-27 12:46:05.000' AS DateTime), N'admin', NULL, NULL, N'ThreeSectionWidget', NULL, 3, CAST(N'2017-01-17 09:37:23.000' AS DateTime),
 N'admin', null)

--[bcms_root].[Widgets]
INSERT [bcms_root].[Widgets] ([Id], [CategoryId]) VALUES (N'253b91a0-a43d-4364-b614-a70800d2699a', NULL)

--[bcms_pages].[ServerControlWidgets]
INSERT [bcms_pages].[ServerControlWidgets] ([Id], [Url]) VALUES (N'253b91a0-a43d-4364-b614-a70800d2699a', N'~/Areas/bcms-installation/Views/Widgets/ThreeSectionWidget.cshtml')

--[bcms_root].[ContentOptions]
INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn],
 [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId])
 VALUES (N'56a91f18-4796-4aef-abd4-a6fe00ae667f', 28, 0, CAST(N'2017-01-17 10:34:58.000' AS DateTime), N'admin', 
 CAST(N'2017-01-27 12:46:05.000' AS DateTime), N'admin', NULL, NULL, N'253b91a0-a43d-4364-b614-a70800d2699a',
 N'JsonFile', 1, N'{"data":[{"imageurl":"http://bettercms.sandbox.mvc4.local.net/uploads/image/03b282cadf9f426eb4e10d29e9f744fa/services_image_01.jpg",
 "heading":"Experienced","text":"Lorem ipsum dolor sit amet, consec adipiscing elit.","iconurl":""},
 {"imageurl":"http://bettercms.sandbox.mvc4.local.net/uploads/image/fd922001b5104020ae23c524b0f2ca1e/services_image_02.jpg","heading":"Construction",
 "text":"Lorem ipsum dolor sit amet, consec adipiscing elit.","iconurl":""},
 {"imageurl":"http://bettercms.sandbox.mvc4.local.net/uploads/image/ad9f0df92f99494db8f2068a14339de4/services_image_03.jpg","heading":"Finish Work",
 "text":"Lorem ipsum dolor sit amet, consec adipiscing elit.","iconurl":""},
 {"imageurl":"http://bettercms.sandbox.mvc4.local.net/uploads/image/03b282cadf9f426eb4e10d29e9f744fa/services_image_01.jpg","heading":"Experienced",
 "text":"Lorem ipsum dolor sit amet, consec adipiscing elit.","iconurl":""}]}', 1, NULL)

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], 
[DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId])
 VALUES (N'614c7146-e2cb-40d6-808e-a6ff00b978f4', 21, 0, CAST(N'2017-01-18 11:15:17.000' AS DateTime), N'admin', 
 CAST(N'2017-01-27 12:46:05.000' AS DateTime), N'admin', NULL, NULL, N'253b91a0-a43d-4364-b614-a70800d2699a', 
 N'Background_color', 1, N'black', 1, NULL)


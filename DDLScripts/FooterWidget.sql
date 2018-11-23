
--[bcms_root].[Contents]
INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn],
 [DeletedByUser], [Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId])
 VALUES (N'0f430a26-3646-46fe-b79f-a70800c8d501', 1, 0, CAST(N'2017-01-27 12:11:12.000' AS DateTime), N'admin', 
 CAST(N'2017-01-27 12:11:12.000' AS DateTime), N'admin', NULL, NULL, N'FooterWidget', NULL, 
 3, CAST(N'2017-01-19 10:26:07.000' AS DateTime), N'admin', null);


 --[bcms_root].[Widgets] 
INSERT [bcms_root].[Widgets] ([Id], [CategoryId]) VALUES (N'0f430a26-3646-46fe-b79f-a70800c8d501', NULL)

--[bcms_pages].[ServerControlWidgets]
INSERT [bcms_pages].[ServerControlWidgets] ([Id], [Url]) VALUES (N'0f430a26-3646-46fe-b79f-a70800c8d501', N'~/Areas/bcms-installation/Views/Widgets/FooterWidget.cshtml')
 --[bcms_root].[ContentOptions]
INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn],
 [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId])
 VALUES (N'eabb1f05-3fdf-4bcd-89af-a70000b19525', 62, 0, CAST(N'2017-01-19 10:46:33.000' AS DateTime), N'admin', 
 CAST(N'2017-01-27 12:11:12.000' AS DateTime), N'admin', NULL, NULL, N'0f430a26-3646-46fe-b79f-a70800c8d501', N'JsonFile', 1,
 N'{"logo": [{ "Maintext": "Spectrum" },{"Subtext":"ALL FOR YOUR BUSINESS"}],"Links": [{ "text": "ABOUT", "url": "#" }, 
 {"text": "CAREERS", "url": "#" }, {"text": "TEAM","url": "#" },{"text": "OWNER PROFILE","url": "#" }],
 "Contact": [{ "Emailid": "info@demolink.org","Phoneno": "(703) 329-06-32","Address": "6036 Richmond hwy., Alexandria, VA, 22303" }],
 "Newletter": [{"text": "SIGN UP FOR UPDATES" }] }', 1, NULL);

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], 
[DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'3f8507a8-7a0a-44e7-934c-a7050108131e', 
36, 0, CAST(N'2017-01-24 16:01:27.000' AS DateTime), N'admin', CAST(N'2017-01-27 12:11:12.000' AS DateTime), N'admin', 
NULL, NULL, N'0f430a26-3646-46fe-b79f-a70800c8d501', N'BackgroundImage', 1,
 N'http://bettercms.sandbox.mvc4.local.net/uploads/image/266a03ab38034d5c9c81e70bb46fae8e/home-bg.jpg', 1, NULL);

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], 
[DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'841bc375-0dbf-4c1c-a601-a70800bb7d8b',
 1, 0, CAST(N'2017-01-27 11:22:37.000' AS DateTime), N'admin',
 CAST(N'2017-01-27 12:11:12.000' AS DateTime), N'admin', NULL, NULL, N'0f430a26-3646-46fe-b79f-a70800c8d501', N'BackgroundColor', 1, 
 N'#404040', 1, NULL);



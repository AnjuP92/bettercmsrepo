--[bcms_root].[Contents]
INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId]) VALUES (N'abbbcb4e-31bc-43de-ae10-a70d00a17ed2', 1, 0, CAST(N'2017-02-01 09:47:59.000' AS DateTime), N'admin', CAST(N'2017-02-01 09:47:59.000' AS DateTime), N'admin', NULL, NULL, N'CounterWidget', NULL,3 , CAST(N'2017-01-27 15:20:41.000' AS DateTime), N'admin', null)

--[bcms_root].[Widgets]
INSERT [bcms_root].[Widgets] ([Id], [CategoryId]) VALUES (N'abbbcb4e-31bc-43de-ae10-a70d00a17ed2', NULL)

--[bcms_pages].[ServerControlWidgets]
INSERT [bcms_pages].[ServerControlWidgets] ([Id], [Url]) VALUES (N'abbbcb4e-31bc-43de-ae10-a70d00a17ed2', N'~/Areas/bcms-installation/Views/Widgets/CounterWidget.cshtml')


--[bcms_root].[ContentOptions]
INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'fd0d1435-9aa1-4795-8577-a70b01286176', 29, 0, CAST(N'2017-01-30 17:59:05.000' AS DateTime), N'admin', CAST(N'2017-02-01 09:47:59.000' AS DateTime), N'admin', NULL, NULL, N'abbbcb4e-31bc-43de-ae10-a70d00a17ed2', N'Json', 1, N'{ "html" :[ { "text":"<div  class=\"col-lg-3\"><div class=\"count\"><i class=\"fa fa-user fa-4x\"><\/i><div class=\"count-numbers\"><h2><span class=\"counter\">373<\/span>+<\/h2><h4>CLIENTS<\/h4><\/div><\/div><\/div>" }, { "text":"<div  class=\"col-lg-3\"><div class=\"count\"><i class=\"fa fa-suitcase fa-4x\"><\/i><div class=\"count-numbers\"><h2><span class=\"counter\">1000<\/span>+<\/h2><h4>PROJECTS<\/h4><\/div><\/div><\/div>" }, { "text":"<div  class=\"col-lg-3\"><div class=\"count\"><i class=\"fa fa-coffee fa-4x\"><\/i><div class=\"count-numbers\"><h2><span class=\"counter\">200<\/span>+<\/h2> <h4>STAFFS<\/h4><\/div><\/div><\/div>" }, { "text":"<div  class=\"col-lg-3\"><div class=\"count\"><i class=\"fa fa-clock-o fa-4x\"><\/i><div class=\"count-numbers\"><h2><span class=\"counter\">1095<\/span>+<\/h2><h4>DAYS<\/h4><\/div><\/div><\/div>" } ] }', 1, NULL)

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'cb0ecf1e-4220-455e-8d40-a70c009f9d52', 26, 0, CAST(N'2017-01-31 09:41:08.000' AS DateTime), N'admin', CAST(N'2017-02-01 09:47:59.000' AS DateTime), N'admin', NULL, NULL, N'abbbcb4e-31bc-43de-ae10-a70d00a17ed2', N'BackgroundColor', 1, N'#404040', 1, NULL)

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'6ec17c71-98a2-454f-a072-a70c009f9d52', 26, 0, CAST(N'2017-01-31 09:41:08.000' AS DateTime), N'admin', CAST(N'2017-02-01 09:47:59.000' AS DateTime), N'admin', NULL, NULL,N'abbbcb4e-31bc-43de-ae10-a70d00a17ed2', N'BackgroundImage', 1, NULL, 1, NULL)




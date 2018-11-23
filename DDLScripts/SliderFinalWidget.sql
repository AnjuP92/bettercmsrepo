--[bcms_root].[Contents]

INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId]) VALUES (N'937bccf0-43cd-43b1-a233-a71500a7ec69', 3, 1, CAST(N'2017-02-09 10:11:23.000' AS DateTime), N'rashmi', CAST(N'2017-02-09 15:14:16.000' AS DateTime), N'rashmi', CAST(N'2017-02-09 15:14:16.000' AS DateTime), N'rashmi', N'SliderWidget', NULL, 3, CAST(N'2017-02-09 10:11:23.000' AS DateTime), N'rashmi', NULL)
INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId]) VALUES (N'13fa969a-135b-40a7-855a-a71500a80b96', 1, 0, CAST(N'2017-02-09 10:11:50.000' AS DateTime), N'rashmi', CAST(N'2017-02-09 10:11:50.000' AS DateTime), N'rashmi', NULL, NULL, N'SliderWidget', NULL, 4, CAST(N'2017-02-09 10:11:23.000' AS DateTime), N'rashmi', N'937bccf0-43cd-43b1-a233-a71500a7ec69')

--[bcms_root].[ContentOptions]


INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'9e1b3ff5-3aad-46c7-a84e-a71500a80b9c', 2, 1, CAST(N'2017-02-09 10:11:50.000' AS DateTime), N'rashmi', CAST(N'2017-02-09 15:14:16.000' AS DateTime), N'rashmi', CAST(N'2017-02-09 15:14:16.000' AS DateTime), N'rashmi', N'937bccf0-43cd-43b1-a233-a71500a7ec69', N'json', 21, N'
{
"Sliderheight":"400px",
"sliderhtml" :[
{"id":"0",
"sliderimage":"http://bettercms.sandbox.mvc4.local.net/uploads/image/f8d3e79f53b548588b9e234ca8c24557/bg-20.jpg",
"slidervideo":"",
"slidertext":"TAKE THE WEB BY STORM BY JANGO",
"buttontext":"LEARN MORE",
"buttoncolor":"aqua",
"bordercolor":"aqua"
},
{"id":"1", 
"sliderimage":"http://bettercms.sandbox.mvc4.local.net/uploads/image/a1f4bc91d46942aab07e507742ba064c/bg-43.jpg",
"slidervideo":"",
"slidertext":"JANGO IS OPTIMIZED TO EVERY DEVELOPMENT",
"buttontext":"LEARN MORE",
"buttoncolor":"aqua",
"bordercolor":"aqua"
},
{"id":"2", 
"sliderimage":"http://bettercms.sandbox.mvc4.local.net/uploads/image/a4596510779d4bb3a13cd5b9cf2bec77/vimagenew.png",
"slidervideo":"http://bettercms.sandbox.mvc4.local.net/uploads/file/acd8efc43b30478c9858519922365e9e/618591532.mp4",
"slidertext":"LET US SHOW YOU UNLIMITED POSSIBILITY",
"buttontext":"PURCHASE",
"buttoncolor":"transparent",
"bordercolor":"white"
}
                             
]
}', 1, NULL)

--[bcms_root].[Widgets]

INSERT [bcms_root].[Widgets] ([Id], [CategoryId]) VALUES (N'13fa969a-135b-40a7-855a-a71500a80b96', NULL)


--[bcms_pages].[ServerControlWidgets]

INSERT [bcms_pages].[ServerControlWidgets] ([Id], [Url]) VALUES (N'13fa969a-135b-40a7-855a-a71500a80b96', N'~/Areas/bcms-installation/Views/Widgets/Slider.cshtml')


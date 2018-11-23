--[bcms_root].[Contents]

INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId]) VALUES (N'2f1fae9f-15ef-44a5-863f-a70601372d47', 2, 0, CAST(N'2017-01-25 18:52:57.000' AS DateTime), N'rashmi', CAST(N'2017-01-25 18:52:57.000' AS DateTime), N'rashmi', NULL, NULL, N'SliderSection', NULL, 3, CAST(N'2017-01-25 18:52:57.000' AS DateTime), N'rashmi', NULL)
INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId]) VALUES (N'7c0edfc2-ddb3-4a93-8c8b-a706013752bb', 1, 0, CAST(N'2017-01-25 18:53:29.000' AS DateTime), N'rashmi', CAST(N'2017-01-25 18:53:29.000' AS DateTime), N'rashmi', NULL, NULL, N'SliderSection', NULL, 3, CAST(N'2017-01-25 18:52:57.000' AS DateTime), N'rashmi', N'2f1fae9f-15ef-44a5-863f-a70601372d47')


--[bcms_root].[ContentOptions]

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'1a67c386-1609-4cd9-a049-a706013752bc', 1, 0, CAST(N'2017-01-25 18:53:29.000' AS DateTime), N'rashmi', CAST(N'2017-01-25 18:53:29.000' AS DateTime), N'rashmi', NULL, NULL, N'2f1fae9f-15ef-44a5-863f-a70601372d47', N'json', 21, N'
{

"sliderhtml" :[
{"id":"0",
"sliderimage":"http://bettercms.sandbox.mvc4.local.net/uploads/image/6e90ce4626524858beb4cc8cc5a6bd91/bg-43.jpg?170121185116",
"slidervideo":"http://bettercms.sandbox.mvc4.local.net/uploads/image/6e90ce4626524858beb4cc8cc5a6bd91/bg-43.jpg?170121185116",
"slidertext":"TAKE THE WEB BY STORM BY JANGO",
"buttontext":"LEARN MORE",
"buttoncolor":"aqua",
"bordercolor":"aqua"
},
{"id":"1", 
"sliderimage":"http://bettercms.sandbox.mvc4.local.net/uploads/image/1a4fb678d47648aabd593093baa94da9/bg-20.jpg?170121185116",
"slidervideo":"http://bettercms.sandbox.mvc4.local.net/uploads/image/1a4fb678d47648aabd593093baa94da9/bg-20.jpg?170121185116",
"slidertext":"JANGO IS OPTIMIZED TO EVERY DEVELOPMENT",
"buttontext":"LEARN MORE",
"buttoncolor":"aqua",
"bordercolor":"aqua"
},
{"id":"2", 
"sliderimage":"http://bettercms.sandbox.mvc4.local.net/uploads/file/d6dbfd4bf3a74c4ba290ea510ab7e2e8/vimagenew.png",
"slidervideo":"http://bettercms.sandbox.mvc4.local.net/uploads/file/d1b39e48830a49878506193e1112077b/618591532.mp4",
"slidertext":"LET US SHOW YOU UNLIMITED POSSIBILITY",
"buttontext":"PURCHASE",
"buttoncolor":"transparent",
"bordercolor":"white"
}





                          
                               
]
}', 1, NULL)


--[bcms_root].[Widgets]

INSERT [bcms_root].[Widgets] ([Id], [CategoryId]) VALUES (N'7c0edfc2-ddb3-4a93-8c8b-a706013752bb', NULL)


--[bcms_pages].[ServerControlWidgets]

INSERT [bcms_pages].[ServerControlWidgets] ([Id], [Url]) VALUES (N'7c0edfc2-ddb3-4a93-8c8b-a706013752bb', N'~/Areas/bcms-installation/Views/Widgets/Slider.cshtml')



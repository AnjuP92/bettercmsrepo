--[bcms_root].[Contents]
INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId]) VALUES (N'925cbb22-4e9f-4eb5-9ddd-a71500b24725', 2, 0, CAST(N'2017-02-09 10:49:05.000' AS DateTime), N'rashmi', CAST(N'2017-02-09 10:49:05.000' AS DateTime), N'rashmi', NULL, NULL, N'ParallaxWidget', NULL, 3, CAST(N'2017-02-09 10:49:05.000' AS DateTime), N'rashmi', NULL)
INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId]) VALUES (N'cc251bd1-b9fe-4f40-9b7e-a71500b2eecb', 1, 0, CAST(N'2017-02-09 10:51:28.000' AS DateTime), N'rashmi', CAST(N'2017-02-09 10:51:28.000' AS DateTime), N'rashmi', NULL, NULL, N'ParallaxWidget', NULL, 4, CAST(N'2017-02-09 10:49:05.000' AS DateTime), N'rashmi', N'925cbb22-4e9f-4eb5-9ddd-a71500b24725')


--[bcms_root].[ContentOptions]

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'a783a894-2fe3-4ec8-b131-a71500b2eed1', 1, 0, CAST(N'2017-02-09 10:51:28.000' AS DateTime), N'rashmi', CAST(N'2017-02-09 10:51:28.000' AS DateTime), N'rashmi', NULL, NULL, N'925cbb22-4e9f-4eb5-9ddd-a71500b24725', N'json', 21, N'{
"parallaximage":"http://kingofwallpapers.com/path/path-007.jpg",
"parallaxheading":"<div class=\"jumbotron text-center\"  style=\" background:transparent\">
  <h1>My First Bootstrap Page<\/h1>
  <p>Resize this responsive page to see the effect!<\/p>
<\/div>",
"parallaxdiv" :[
{ "parallaxtext":"<div  class=\"col-sm-4\" style=\"font-size:36px;overflow:hidden;\"> 
                                            <img class =\"p-img\" src=\"https://unsplash.it/2000/1250?image=397\" \/>
                                            <h3 style=\"background:transparent;\">Lorem ipsum dolor sit amet, consectetur 
<\/h3>
 <\/div>"
},
{ "parallaxtext":"<div  class=\"col-sm-4\" style=\"font-size:36px;overflow:hidden;\"> 
                                            <img class =\"p-img\" src=\"https://unsplash.it/2000/1250?image=689\"\/>
                                            <h3 style=\"background:transparent;\">A mountain is a large landform that stretches <\/h3>
 <\/div>"
},
{ "parallaxtext":"<div  class=\"col-sm-4\" style=\"font-size:36px;overflow:hidden;\"> 
                                            <img class =\"p-img\" src=\"https://unsplash.it/2000/1250?image=658\"  \/>
                                            <h3 style=\"background:transparent;\">Buildings come in a variety of sizes, shapes 
<\/h3>
 <\/div>"
}
]
}
', 1, NULL)



--[bcms_root].[Widgets]
INSERT [bcms_root].[Widgets] ([Id], [CategoryId]) VALUES (N'925cbb22-4e9f-4eb5-9ddd-a71500b24725', NULL)




--[bcms_pages].[ServerControlWidgets]

INSERT [bcms_pages].[ServerControlWidgets] ([Id], [Url]) VALUES (N'925cbb22-4e9f-4eb5-9ddd-a71500b24725', N'~/Areas/bcms-installation/Views/Widgets/ParallaxSection.cshtml')



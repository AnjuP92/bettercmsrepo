//Contents-MyParallaxSection

INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId]) VALUES (N'ab035348-ccd6-43f0-9833-a706013b416b', 2, 0, CAST(N'2017-01-25 19:07:48.000' AS DateTime), N'rashmi', CAST(N'2017-01-25 19:07:48.000' AS DateTime), N'rashmi', NULL, NULL, N'MyParallaxSection', NULL, 3, CAST(N'2017-01-25 19:07:48.000' AS DateTime), N'rashmi', NULL)
INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId]) VALUES (N'79280fd2-ab19-4392-918e-a706013b658f', 1, 0, CAST(N'2017-01-25 19:08:19.000' AS DateTime), N'rashmi', CAST(N'2017-01-25 19:08:19.000' AS DateTime), N'rashmi', NULL, NULL, N'MyParallaxSection', NULL, 3, CAST(N'2017-01-25 19:07:48.000' AS DateTime), N'rashmi', N'ab035348-ccd6-43f0-9833-a706013b416b')

//Widgets-MyParallaxSection


INSERT [bcms_root].[Widgets] ([Id], [CategoryId]) VALUES (N'79280fd2-ab19-4392-918e-a706013b658f', NULL)


//ServerControlWidgets-MyParallaxSection

INSERT [bcms_pages].[ServerControlWidgets] ([Id], [Url]) VALUES (N'79280fd2-ab19-4392-918e-a706013b658f', N'~/Areas/bcms-installation/Views/Widgets/ParallaxSection.cshtml')



//ContentOptions-MyParallaxSection

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'9c4fd88c-bd88-4307-bd51-a706013b6597', 1, 0, CAST(N'2017-01-25 19:08:19.000' AS DateTime), N'rashmi', CAST(N'2017-01-25 19:08:19.000' AS DateTime), N'rashmi', NULL, NULL, N'ab035348-ccd6-43f0-9833-a706013b416b', N'json', 21, N'
{
"parallaximage":"http://kingofwallpapers.com/path/path-007.jpg",
"parallaxheading":"<div class=\"jumbotron text-center\"  style=\" background:transparent\">
  <h1>My First Bootstrap Page<\/h1>
  <p>Resize this responsive page to see the effect!<\/p>
<\/div>",
"parallaxdiv" :[
{ "parallaxtext":"<div  class=\"col-sm-4\" style=\"font-size:36px;\"> 
                                            <img src=\"https://unsplash.it/2000/1250?image=397\" \/>
                                            <h3 style=\"background:transparent;\">Lorem ipsum dolor sit amet, consectetur 
<\/h3>
 <\/div>"
},
{ "parallaxtext":"<div  class=\"col-sm-4\" style=\"font-size:36px;\"> 
                                            <img src=\"https://unsplash.it/2000/1250?image=689\" \/>
                                            <h3 style=\"background:transparent;\">A mountain is a large landform that stretches <\/h3>
 <\/div>"
},
{ "parallaxtext":"<div  class=\"col-sm-4\" style=\"font-size:36px;\"> 
                                            <img src=\"https://unsplash.it/2000/1250?image=658\" \/>
                                            <h3 style=\"background:transparent;\">Buildings come in a variety of sizes, shapes 
<\/h3>
 <\/div>"
}
]
}', 1, NULL)






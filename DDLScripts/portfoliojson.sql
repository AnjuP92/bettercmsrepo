-- bcms_root.Contents table

INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], 
[Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId]) VALUES 
(N'a39987ec-9363-473f-8fd3-a70d012f8ebb', 1, 0, CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', CAST(N'2017-02-01 18:25:13.000' AS DateTime), 
N'nishasubhash', NULL, NULL, N'MyPortfolio Widget', NULL, 3, CAST(N'2017-01-27 11:02:48.000' AS DateTime), N'nishasubhash',Null)

-----------------------------------------------------------------------------------------------------------------------------------------------------------------

-- bcms_root.Widgets table

INSERT [bcms_root].[Widgets] ([Id], [CategoryId]) VALUES (N'a39987ec-9363-473f-8fd3-a70d012f8ebb', NULL)  

-----------------------------------------------------------------------------------------------------------------------------------------------------------------

-- bcms_pages.ServerControlWidgets

INSERT [bcms_pages].[ServerControlWidgets] ([Id], [Url]) VALUES (N'a39987ec-9363-473f-8fd3-a70d012f8ebb', N'~/Areas/bcms-installation/Views/Widgets/MyPortfolioWidget.cshtml')

-----------------------------------------------------------------------------------------------------------------------------------------------------------------
-- bcms-root.ContentOptions

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'c82ad6ca-b370-4b7d-abd3-a70d012f8ebe', 1, 0, CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', NULL, NULL, N'a39987ec-9363-473f-8fd3-a70d012f8ebb', N'filterTextColor', 1, N'#363636', 1, NULL)

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'17efafdf-00ea-44b3-845e-a70d012f8ebf', 1, 0, CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', NULL, NULL, N'a39987ec-9363-473f-8fd3-a70d012f8ebb', N'searchSymbolColor', 1, N'#fff', 1, NULL)

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'cc52ee0a-19df-4af1-8b32-a70d012f8ebf', 1, 0, CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', NULL, NULL, N'a39987ec-9363-473f-8fd3-a70d012f8ebb', N'filterTextColorOnClick', 1, N'#4532BF', 1, NULL)

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'282664af-7d5b-4007-a62c-a70d012f8ebf', 1, 0, CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', NULL, NULL, N'a39987ec-9363-473f-8fd3-a70d012f8ebb', N'filterTextFontFamily', 1, N'cursive', 1, NULL)

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'c8decf8d-08f6-4399-a636-a70d012f8ebf', 1, 0, CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', NULL, NULL, N'a39987ec-9363-473f-8fd3-a70d012f8ebb', N'imagehover', 1, N'imghvr-push-up', 1, NULL)



INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'a1a5d5b8-7bc9-4daf-90d6-a70d012f8ebf', 1, 0, CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', NULL, NULL, N'a39987ec-9363-473f-8fd3-a70d012f8ebb', N'filterTextFontSize', 1, N'14px', 1, NULL)

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'7b157edc-c35c-4fd1-a901-a70d012f8ebf', 1, 0, CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', NULL, NULL, N'a39987ec-9363-473f-8fd3-a70d012f8ebb', N'filterTextFontWeight', 1, N'700', 1, NULL)

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'a8a71bf4-0aaf-472c-befb-a70d012f8ebf', 1, 0, CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', CAST(N'2017-02-01 18:25:13.000' AS DateTime), N'nishasubhash', NULL, NULL, N'a39987ec-9363-473f-8fd3-a70d012f8ebb', N'jsonFile', 1, N'{ 		     "portfolioFilters":[ 			  { 			  "filter":"Layout,HTML,WordPress,Responsive"			    			  } 		      ],             "Images": [              {               "image": "http://bettercms.sandbox.mvc4.local.net/uploads/image/405429e6656f4fff9807dd536ab1fa10/1.jpg",               "group" : "responsive" 			  			               },              {               "image": "http://bettercms.sandbox.mvc4.local.net/uploads/image/10592141c94741248e25cd70cc865d48/2.jpg",               "group": "layout,html" 			   			              },              {               "image": "http://bettercms.sandbox.mvc4.local.net/uploads/image/43d5cdf357cc4aee8916886e32106e7d/3.jpg",               "group": "wordpress" 			   			                },              {               "image": "http://bettercms.sandbox.mvc4.local.net/uploads/image/c5a5f3f4994a46cab380da9a586e6d2c/4.jpg",               "group": "layout,html" 			   			                },              {               "image": "http://bettercms.sandbox.mvc4.local.net/uploads/image/215fb2fd99b44383a4da507a2bbc598f/5.jpg",               "group": "html,responsive" 			   			 		                },              {                "image": "http://bettercms.sandbox.mvc4.local.net/uploads/image/d591a7b8a00f46e7a0ddab793351a1d6/6.jpg", 			   "group": "wordpress,responsive"              }                       ] 		           }', 1, NULL)












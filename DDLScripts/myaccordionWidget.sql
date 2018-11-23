--[bcms_root].[Contents]

INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId]) VALUES (N'ced2b9d5-a80f-4b80-90bb-a70d0107ed66', 2, 0, CAST(N'2017-02-01 16:00:55.000' AS DateTime), N'rashmi', CAST(N'2017-02-01 16:00:55.000' AS DateTime), N'rashmi', NULL, NULL, N'MyaccordionWidget', NULL, 3, CAST(N'2017-02-01 16:00:55.000' AS DateTime), N'rashmi', NULL)
INSERT [bcms_root].[Contents] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [Name], [PreviewUrl], [Status], [PublishedOn], [PublishedByUser], [OriginalId]) VALUES (N'e3ac732f-9a2e-455a-9e4e-a70d01080988', 1, 0, CAST(N'2017-02-01 16:01:19.000' AS DateTime), N'rashmi', CAST(N'2017-02-01 16:01:19.000' AS DateTime), N'rashmi', NULL, NULL, N'MyaccordionWidget', NULL, 3, CAST(N'2017-02-01 16:00:55.000' AS DateTime), N'rashmi', N'ced2b9d5-a80f-4b80-90bb-a70d0107ed66')



--[bcms_root].[ContentOptions]

INSERT [bcms_root].[ContentOptions] ([Id], [Version], [IsDeleted], [CreatedOn], [CreatedByUser], [ModifiedOn], [ModifiedByUser], [DeletedOn], [DeletedByUser], [ContentId], [Key], [Type], [DefaultValue], [IsDeletable], [CustomOptionId]) VALUES (N'5c3deab6-c153-461a-bf72-a70d01080988', 1, 0, CAST(N'2017-02-01 16:01:19.000' AS DateTime), N'rashmi', CAST(N'2017-02-01 16:01:19.000' AS DateTime), N'rashmi', NULL, NULL, N'ced2b9d5-a80f-4b80-90bb-a70d0107ed66', N'json', 21, N'{
"acordionbackground":"blue",
"panelbackground":"pink",
    "accordion" :[
 {
   "accordiontitle":"<div class=\"panel-heading\" style=\"text-align:center;\"><h4 class=\"panel-title\"><a data-toggle=\"collapse\" data-parent=\"#accordion\" href=\"#collapse1\">
    Section-1
	<span class=\"btn\"><\/span><\/a><\/h4><\/div>",
	"accordiontext":"<div id=\"collapse1\" class = \"panel-collapse collapse\" style=\"background-color:@robj.panelbackground;text-align:center;\"><div class=\"panel-body\">Lorem ipsum dolor sit amet, consectetur adipisicing elit<\/div><\/div> "},
 {"accordiontitle":"<div class=\"panel-heading\" style=\"text-align:center;\">
 <h4 class=\"panel-title\">
 <a data-toggle=\"collapse\" data-parent=\"#accordion\" href=\"#collapse2\">
    Section-2
	<span class=\"btn\"><\/span><\/a><\/h4><\/div>",
	"accordiontext":"
	<div id=\"collapse2\" class=\"panel-collapse collapse\" style=\"background-color:@robj.panelbackground;text-align:center;\">
	<div class=\"panel-body\">Lorem ipsum dolor sit amet, consectetur adipisicing elit<\/div><\/div>"

},
 {"accordiontitle":"<div class=\"panel-heading\" style=\"text-align:center;\">
 <h4 class=\"panel-title\">
 <a data-toggle=\"collapse\" data-parent=\"#accordion\" href=\"#collapse3\">
    Section-3<span class=\"btn\"><\/span><\/a><\/h4><\/div>",
	"accordiontext":"
	<div id=\"collapse3\" class=\"panel-collapse collapse\" style=\"background-color:@robj.panelbackground;text-align:center;\">
	<div class=\"panel-body\">Lorem ipsum dolor sit amet, consectetur adipisicing elit<\/div><\/div>"

},
 {"accordiontitle":"<div class=\"panel-heading\">
                                <h4 class=\"panel-title\">
                                    <a data-toggle=\"collapse\" data-parent=\"#accordion\" href=\"#collapseThree\">
                                        Section-4<span class=\"btn\"><\/span>
                                    <\/a>
                                <\/h4>
                            <\/div>",
	"accordiontext":"<div id=\"collapseThree\" class=\"panel-collapse collapse\">
                                <div class=\"panel-body\">

                                    <div class=\"panel-group\" id=\"accordion2\">
                                        <div class=\"panel panel-default\">
                                            <div class=\"panel-heading\">
                                                <h4 class=\"panel-title\">
                                                    <a data-toggle=\"collapse\" data-parent=\"#accordion2\" href=\"#collapseThreeOne\">
                                                        Section-4.1 <span class=\"subbtn\"><\/span>
                                                    <\/a>
                                                <\/h4>
                                            <\/div>
                                            <div id=\"collapseThreeOne\" class=\"panel-collapse collapse\">
                                                <div class=\"panel-body\">Panel 3.1 <\/div>
                                            <\/div>
                                        <\/div>
                                        <div class=\"panel panel-default\">
                                            <div class=\"panel-heading\">
                                                <h4 class=\"panel-title\">
                                                    <a data-toggle=\"collapse\" data-parent=\"#accordion2\" href=\"#collapseThreeTwo\">
                                                        Section-4.2 <span class=\"subbtn\"><\/span>
                                                    <\/a>
                                                <\/h4>
                                            <\/div>
                                            <div id=\"collapseThreeTwo\" class=\"panel-collapse collapse\">
                                                <div class=\"panel-body\">Panel 4.2.1 <\/div>
                                            <\/div>
                                        <\/div>
                                    <\/div>

                                <\/div>
                            <\/div>
                     
	"

}
]}', 1, NULL)

--[bcms_root].[Widgets]

INSERT [bcms_root].[Widgets] ([Id], [CategoryId]) VALUES (N'e3ac732f-9a2e-455a-9e4e-a70d01080988', NULL)

--[bcms_pages].[ServerControlWidgets]

INSERT [bcms_pages].[ServerControlWidgets] ([Id], [Url]) VALUES (N'e3ac732f-9a2e-455a-9e4e-a70d01080988', N'~/Areas/bcms-installation/Views/Widgets/accordion.cshtml')





﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    #line 34 "..\..\Views\Images\ImageEditor.cshtml"
    using BetterCms.Module.MediaManager.Content.Resources;
    
    #line default
    #line hidden
    
    #line 35 "..\..\Views\Images\ImageEditor.cshtml"
    using BetterCms.Module.MediaManager.Controllers;
    
    #line default
    #line hidden
    
    #line 28 "..\..\Views\Images\ImageEditor.cshtml"
    using BetterCms.Module.MediaManager.Models;
    
    #line default
    #line hidden
    
    #line 36 "..\..\Views\Images\ImageEditor.cshtml"
    using BetterCms.Module.MediaManager.ViewModels.Images;
    
    #line default
    #line hidden
    
    #line 33 "..\..\Views\Images\ImageEditor.cshtml"
    using BetterCms.Module.Root.Content.Resources;
    
    #line default
    #line hidden
    
    #line 32 "..\..\Views\Images\ImageEditor.cshtml"
    using BetterCms.Module.Root.Mvc.Helpers;
    
    #line default
    #line hidden
    
    #line 29 "..\..\Views\Images\ImageEditor.cshtml"
    using BetterCms.Module.Root.ViewModels.Category;
    
    #line default
    #line hidden
    
    #line 30 "..\..\Views\Images\ImageEditor.cshtml"
    using BetterCms.Module.Root.ViewModels.Tags;
    
    #line default
    #line hidden
    
    #line 31 "..\..\Views\Images\ImageEditor.cshtml"
    using Microsoft.Web.Mvc;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Images/ImageEditor.cshtml")]
    public partial class _Views_Images_ImageEditor_cshtml : System.Web.Mvc.WebViewPage<ImageViewModel>
    {
        public _Views_Images_ImageEditor_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 38 "..\..\Views\Images\ImageEditor.cshtml"
  
    var tagsTemplateViewModel = new TagsTemplateViewModel
    {
        TooltipDescription = MediaGlobalization.FileEditor_Dialog_AddTags_Tooltip_Description
    };

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 44 "..\..\Views\Images\ImageEditor.cshtml"
  
    var categoriesTemplateViewModel = new CategoryTemplateViewModel
    {
        TooltipDescription = MediaGlobalization.FileEditor_Dialog_Category_Tooltip_Description
    };

            
            #line default
            #line hidden
WriteLiteral("\r\n<div");

WriteLiteral(" class=\"bcms-modal-frame-holder\"");

WriteLiteral(">\r\n");

WriteLiteral("    ");

            
            #line 51 "..\..\Views\Images\ImageEditor.cshtml"
Write(Html.MessagesBox());

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n    <div");

WriteLiteral(" class=\"bcms-window-options\"");

WriteLiteral(">\r\n");

            
            #line 54 "..\..\Views\Images\ImageEditor.cshtml"
        
            
            #line default
            #line hidden
            
            #line 54 "..\..\Views\Images\ImageEditor.cshtml"
         using (Html.BeginForm<ImagesController>(f => f.ImageEditor((ImageViewModel)null), FormMethod.Post, new { @class = "bcms-ajax-form", @enctype = "multipart/form-data" }))
        {
            
            
            #line default
            #line hidden
            
            #line 56 "..\..\Views\Images\ImageEditor.cshtml"
       Write(Html.HiddenFor(model => model.Id));

            
            #line default
            #line hidden
            
            #line 56 "..\..\Views\Images\ImageEditor.cshtml"
                                              
            
            
            #line default
            #line hidden
            
            #line 57 "..\..\Views\Images\ImageEditor.cshtml"
       Write(Html.HiddenFor(model => model.Version, new { @id = "image-version-field" }));

            
            #line default
            #line hidden
            
            #line 57 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                        
            
            
            #line default
            #line hidden
            
            #line 58 "..\..\Views\Images\ImageEditor.cshtml"
       Write(Html.HiddenFor(model => model.CropCoordX1, new { @data_bind = "value: Math.floor(imageEditorViewModel.cropCoordX1())" }));

            
            #line default
            #line hidden
            
            #line 58 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                                                                     
            
            
            #line default
            #line hidden
            
            #line 59 "..\..\Views\Images\ImageEditor.cshtml"
       Write(Html.HiddenFor(model => model.CropCoordX2, new { @data_bind = "value: Math.floor(imageEditorViewModel.cropCoordX2())" }));

            
            #line default
            #line hidden
            
            #line 59 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                                                                     
            
            
            #line default
            #line hidden
            
            #line 60 "..\..\Views\Images\ImageEditor.cshtml"
       Write(Html.HiddenFor(model => model.CropCoordY1, new { @data_bind = "value: Math.floor(imageEditorViewModel.cropCoordY1())" }));

            
            #line default
            #line hidden
            
            #line 60 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                                                                     
            
            
            #line default
            #line hidden
            
            #line 61 "..\..\Views\Images\ImageEditor.cshtml"
       Write(Html.HiddenFor(model => model.CropCoordY2, new { @data_bind = "value: Math.floor(imageEditorViewModel.cropCoordY2())" }));

            
            #line default
            #line hidden
            
            #line 61 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                                                                     
            
            
            #line default
            #line hidden
            
            #line 62 "..\..\Views\Images\ImageEditor.cshtml"
       Write(Html.HiddenFor(model => model.ShouldOverride, new { @id = "image-override-field" }));

            
            #line default
            #line hidden
            
            #line 62 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                                
            
            
            #line default
            #line hidden
            
            #line 63 "..\..\Views\Images\ImageEditor.cshtml"
       Write(Html.HiddenFor(model => model.ImageType, new { @data_bind = "value: imageEditorViewModel.imageType()" }));

            
            #line default
            #line hidden
            
            #line 63 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                                                     


            
            #line default
            #line hidden
WriteLiteral("            <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"bcms-croped-block\"");

WriteLiteral(">\r\n                    <img");

WriteLiteral(" src=\"\"");

WriteLiteral(" data-bind=\"style: { width: imageEditorViewModel.calculatedWidth() + \'px\', height" +
": imageEditorViewModel.calculatedHeight() + \'px\' }\"");

WriteLiteral(" />\r\n                </div>\r\n            </div>\r\n");

            
            #line 70 "..\..\Views\Images\ImageEditor.cshtml"


            
            #line default
            #line hidden
WriteLiteral("            <div");

WriteLiteral(" class=\"bcms-media-manager-form\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"bcms-media-manager-column-left\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 74 "..\..\Views\Images\ImageEditor.cshtml"
                                                    Write(MediaGlobalization.ImageEditor_Dialog_Caption);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n");

WriteLiteral("                        ");

            
            #line 75 "..\..\Views\Images\ImageEditor.cshtml"
                   Write(Html.Tooltip(MediaGlobalization.ImageEditor_Dialog_Caption_Tooltip_Description));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        <div");

WriteLiteral(" class=\"bcms-field-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 77 "..\..\Views\Images\ImageEditor.cshtml"
                       Write(Html.TextBoxFor(f => f.Caption, new { @class = "bcms-field-text", @id = "Caption", data_bind = "event: {change: onValueChange}" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                            ");

            
            #line 78 "..\..\Views\Images\ImageEditor.cshtml"
                       Write(Html.BcmsValidationMessageFor(f => f.Caption));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </div>\r\n                    </div>\r\n\r\n                 " +
"   <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 83 "..\..\Views\Images\ImageEditor.cshtml"
                                                    Write(MediaGlobalization.ImageEditor_Dialog_PublicUrl);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                        <div");

WriteLiteral(" class=\"bcms-field-wrapper\"");

WriteLiteral(">\r\n                            <input");

WriteLiteral(" class=\"bcms-field-text bcms-editor-selectable-field-box\"");

WriteLiteral(" type=\"text\"");

WriteAttribute("value", Tuple.Create(" value=\"", 4696), Tuple.Create("\"", 4714)
            
            #line 85 "..\..\Views\Images\ImageEditor.cshtml"
                               , Tuple.Create(Tuple.Create("", 4704), Tuple.Create<System.Object, System.Int32>(Model.Url
            
            #line default
            #line hidden
, 4704), false)
);

WriteLiteral(" readonly=\"readonly\"");

WriteLiteral(" />\r\n                        </div>\r\n                    </div>\r\n\r\n              " +
"      <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 90 "..\..\Views\Images\ImageEditor.cshtml"
                                                    Write(MediaGlobalization.ImageEditor_Dialog_Description);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n");

WriteLiteral("                        ");

            
            #line 91 "..\..\Views\Images\ImageEditor.cshtml"
                   Write(Html.Tooltip(MediaGlobalization.ImageEditor_Dialog_Description_Tooltip_Description));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        <div");

WriteLiteral(" class=\"bcms-field-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 93 "..\..\Views\Images\ImageEditor.cshtml"
                       Write(Html.TextAreaFor(f => f.Description, new { @class = "bcms-field-textarea", @id = "Description", data_bind = "event: {change: onValueChange}" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                            ");

            
            #line 94 "..\..\Views\Images\ImageEditor.cshtml"
                       Write(Html.BcmsValidationMessageFor(f => f.Description));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </div>\r\n                    </div>\r\n                </d" +
"iv>\r\n\r\n                <div");

WriteLiteral(" class=\"bcms-media-manager-column-right\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" data-bind=\"visible: imageEditorViewModel.imageType() == 1\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">\r\n");

WriteLiteral("                                ");

            
            #line 103 "..\..\Views\Images\ImageEditor.cshtml"
                           Write(MediaGlobalization.ImageEditor_Dialog_CropImage_Title);

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                ");

            
            #line 104 "..\..\Views\Images\ImageEditor.cshtml"
                           Write(Html.Tooltip(MediaGlobalization.ImageEditor_Dialog_CropImage_Tooltip_Description));

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </div>\r\n\r\n                            <div");

WriteLiteral(" class=\"bcms-checkbox-holder\"");

WriteLiteral(">\r\n                                <input");

WriteLiteral(" type=\"checkbox\"");

WriteLiteral(" data-bind=\"checked: imageEditorViewModel.fit\"");

WriteLiteral(" />\r\n                                <div");

WriteLiteral(" class=\"bcms-checkbox-label bcms-js-edit-label\"");

WriteLiteral(" data-bind=\"click: imageEditorViewModel.changeFit\"");

WriteLiteral(">");

            
            #line 109 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                                                                 Write(MediaGlobalization.ImageEditor_Dialog_FitImage_Title);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                                <div");

WriteLiteral(" class=\"bcms-editor-link\"");

WriteLiteral(" data-bind=\"visible: imageEditorViewModel.hasCrop(), click: imageEditorViewModel." +
"removeCrop\"");

WriteLiteral(">Remove crop</div>\r\n                            </div> \r\n\r\n                      " +
"      <div");

WriteLiteral(" class=\"bcms-media-re-upload\"");

WriteLiteral(" data-bind=\"click: reupload\"");

WriteLiteral(">");

            
            #line 113 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                     Write(MediaGlobalization.ImageEditor_Dialog_Reupload_Title);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                        </div>\r\n                    </div>\r\n\r\n           " +
"         <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(" data-bind=\"with: titleEditorViewModel\"");

WriteLiteral(" id=\"bcms-image-title-editor-box\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 119 "..\..\Views\Images\ImageEditor.cshtml"
                       Write(MediaGlobalization.ImageEditor_Dialog_ImageName_Title);

            
            #line default
            #line hidden
WriteLiteral(":\r\n                            <div");

WriteLiteral(" class=\"bcms-editor-link\"");

WriteLiteral(" data-bind=\"click: open\"");

WriteLiteral(">");

            
            #line 120 "..\..\Views\Images\ImageEditor.cshtml"
                                                                             Write(RootGlobalization.Button_Edit);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                        </div>\r\n                        <div");

WriteLiteral(" class=\"bcms-content-subtitle\"");

WriteLiteral(" data-bind=\"text: oldTitle()\"");

WriteLiteral("></div>\r\n\r\n                        <div");

WriteLiteral(" class=\"bcms-editor-box\"");

WriteLiteral(" data-bind=\"style: { \'display\': isOpened() ? \'block\' : \'none\' }\"");

WriteLiteral(" style=\"display: none;\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 125 "..\..\Views\Images\ImageEditor.cshtml"
                                                        Write(MediaGlobalization.ImageEditor_Dialog_ImageTitle_Title);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                            <div");

WriteLiteral(" class=\"bcms-field-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                                ");

            
            #line 127 "..\..\Views\Images\ImageEditor.cshtml"
                           Write(Html.TextBoxFor(f => f.Title, new
                       {
                           @class = "bcms-field-text",
                           @id = "bcms-image-title-editor",
                           @data_bind = "value: title, valueUpdate: 'afterkeydown', enterPress: save, escPress: close, event: {change: $parent.onValueChange}"
                       }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                ");

            
            #line 133 "..\..\Views\Images\ImageEditor.cshtml"
                           Write(Html.BcmsValidationMessageFor(f => f.Title));

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </div>\r\n                            <div");

WriteLiteral(" class=\"bcms-btn-primary\"");

WriteLiteral(" data-bind=\"click: save\"");

WriteLiteral(">");

            
            #line 135 "..\..\Views\Images\ImageEditor.cshtml"
                                                                             Write(RootGlobalization.Button_Ok);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                            <div");

WriteLiteral(" class=\"bcms-btn-cancel\"");

WriteLiteral(" data-bind=\"click: close\"");

WriteLiteral(">");

            
            #line 136 "..\..\Views\Images\ImageEditor.cshtml"
                                                                             Write(RootGlobalization.Button_Cancel);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                        </div>\r\n                    </div>\r\n\r\n           " +
"         <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 141 "..\..\Views\Images\ImageEditor.cshtml"
                                                    Write(MediaGlobalization.ImageEditor_Dialog_FileSize);

            
            #line default
            #line hidden
WriteLiteral(":</div>\r\n                        <div");

WriteLiteral(" class=\"bcms-content-subtitle\"");

WriteLiteral(" id=\"image-file-size\"");

WriteLiteral(">");

            
            #line 142 "..\..\Views\Images\ImageEditor.cshtml"
                                                                           Write(Model.FileSize);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                    </div>\r\n\r\n                    <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(" data-bind=\"with: imageEditorViewModel\"");

WriteLiteral(" id=\"bcms-image-dimensions-editor-box\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 147 "..\..\Views\Images\ImageEditor.cshtml"
                       Write(MediaGlobalization.ImageEditor_Dialog_Dimensions);

            
            #line default
            #line hidden
WriteLiteral(":\r\n                            <div");

WriteLiteral(" class=\"bcms-editor-link\"");

WriteLiteral(" data-bind=\"click: open\"");

WriteLiteral(">");

            
            #line 148 "..\..\Views\Images\ImageEditor.cshtml"
                                                                             Write(RootGlobalization.Button_Edit);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                        </div>\r\n                        <div");

WriteLiteral(" class=\"bcms-content-subtitle\"");

WriteLiteral(" data-bind=\"text: widthAndHeight()\"");

WriteLiteral(">");

            
            #line 150 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                         Write(Model.ImageWidth);

            
            #line default
            #line hidden
WriteLiteral(" x ");

            
            #line 150 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                                             Write(Model.ImageHeight);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n\r\n                        <div");

WriteLiteral(" class=\"bcms-editor-box\"");

WriteLiteral(" data-bind=\"style: { \'display\': isOpened() ? \'block\' : \'none\' }\"");

WriteLiteral(">\r\n                            ");

WriteLiteral("\r\n                            <div");

WriteLiteral(" class=\"bcms-media-editor-container\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" class=\"bcms-media-editor-column\"");

WriteLiteral(">\r\n                                <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 156 "..\..\Views\Images\ImageEditor.cshtml"
                                                            Write(MediaGlobalization.ImageEditor_Dialog_ChangeSize_Width);

            
            #line default
            #line hidden
WriteLiteral(":</div>\r\n                                <div");

WriteLiteral(" class=\"bcms-field-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 158 "..\..\Views\Images\ImageEditor.cshtml"
                               Write(Html.TextBoxFor(f => f.ImageWidth, new { @class = "bcms-field-text", @id = "image-width", @data_bind = "value: width, valueUpdate: 'afterkeydown', enterPress: save, escPress: close, event: { change: changeHeight }" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 159 "..\..\Views\Images\ImageEditor.cshtml"
                               Write(Html.BcmsValidationMessageFor(f => f.ImageWidth));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </div>\r\n                            </div>\r\n\r\n " +
"                           <div");

WriteLiteral(" class=\"bcms-media-editor-column\"");

WriteLiteral(">\r\n                                <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 164 "..\..\Views\Images\ImageEditor.cshtml"
                                                            Write(MediaGlobalization.ImageEditor_Dialog_ChangeSize_Height);

            
            #line default
            #line hidden
WriteLiteral(":</div>\r\n                                <div");

WriteLiteral(" class=\"bcms-field-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 166 "..\..\Views\Images\ImageEditor.cshtml"
                               Write(Html.TextBoxFor(f => f.ImageHeight, new { @class = "bcms-field-text", @id = "image-height", @data_bind = "value: height, valueUpdate: 'afterkeydown', enterPress: save, escPress: close, event: { change: changeWidth }" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 167 "..\..\Views\Images\ImageEditor.cshtml"
                               Write(Html.BcmsValidationMessageFor(f => f.ImageHeight));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </div>\r\n                            </div>\r\n\r\n " +
"                           <div");

WriteLiteral(" class=\"bcms-media-editor-column\"");

WriteLiteral(">\r\n                                <div");

WriteLiteral(" class=\"bcms-btn-primary\"");

WriteLiteral(" data-bind=\"click: save\"");

WriteLiteral(">");

            
            #line 172 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                 Write(RootGlobalization.Button_Ok);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                                <div");

WriteLiteral(" class=\"bcms-btn-cancel\"");

WriteLiteral(" data-bind=\"click: close\"");

WriteLiteral(">");

            
            #line 173 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                 Write(RootGlobalization.Button_Cancel);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                            </div>\r\n                            </div>\r\n\r" +
"\n                            <div");

WriteLiteral(" class=\"bcms-media-restore-size\"");

WriteLiteral(" data-bind=\"click: restoreOriginalSize\"");

WriteLiteral(">");

            
            #line 177 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                                   Write(MediaGlobalization.ImageEditor_Dialog_RestoreOriginalSize_Title);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                        </div>\r\n                    </div>\r\n\r\n           " +
"         <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(" data-bind=\"visible: imageEditorViewModel.enableCrop\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 182 "..\..\Views\Images\ImageEditor.cshtml"
                                                    Write(MediaGlobalization.ImageEditor_Dialog_CroppedDimensions);

            
            #line default
            #line hidden
WriteLiteral(":</div>\r\n                        <div");

WriteLiteral(" class=\"bcms-content-subtitle\"");

WriteLiteral(" id=\"image-file-size\"");

WriteLiteral(" data-bind=\"text: imageEditorViewModel.croppedWidthAndHeight()\"");

WriteLiteral(">");

            
            #line 183 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                                                                          Write(Model.CroppedWidth);

            
            #line default
            #line hidden
WriteLiteral(" x ");

            
            #line 183 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                                                                                                Write(Model.CroppedHeight);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                    </div>\r\n\r\n                    <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 187 "..\..\Views\Images\ImageEditor.cshtml"
                                                    Write(MediaGlobalization.ImageEditor_Dialog_AligmentTitle);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                        <div");

WriteLiteral(" class=\"bcms-alignment-controls\"");

WriteLiteral(">\r\n                            <div");

WriteAttribute("class", Tuple.Create(" class=\"", 12370), Tuple.Create("\"", 12467)
, Tuple.Create(Tuple.Create("", 12378), Tuple.Create("bcms-align-center", 12378), true)
            
            #line 189 "..\..\Views\Images\ImageEditor.cshtml"
, Tuple.Create(Tuple.Create("", 12395), Tuple.Create<System.Object, System.Int32>(Model.ImageAlign == MediaImageAlign.Center ? "-active" : string.Empty
            
            #line default
            #line hidden
, 12395), false)
);

WriteLiteral(">\r\n                                <input");

WriteLiteral(" type=\"radio\"");

WriteLiteral(" name=\"ImageAlign\"");

WriteAttribute("value", Tuple.Create(" value=\"", 12540), Tuple.Create("\"", 12578)
            
            #line 190 "..\..\Views\Images\ImageEditor.cshtml"
, Tuple.Create(Tuple.Create("", 12548), Tuple.Create<System.Object, System.Int32>((int)MediaImageAlign.Center
            
            #line default
            #line hidden
, 12548), false)
);

WriteLiteral(" ");

            
            #line 190 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                                         Write(Model.ImageAlign == MediaImageAlign.Center ? "checked" : string.Empty);

            
            #line default
            #line hidden
WriteLiteral(" data-bind=\"checked: imageAlign.asString()\" />\r\n                            </div" +
">\r\n                            <div");

WriteAttribute("class", Tuple.Create(" class=\"", 12768), Tuple.Create("\"", 12861)
, Tuple.Create(Tuple.Create("", 12776), Tuple.Create("bcms-align-left", 12776), true)
            
            #line 192 "..\..\Views\Images\ImageEditor.cshtml"
, Tuple.Create(Tuple.Create("", 12791), Tuple.Create<System.Object, System.Int32>(Model.ImageAlign == MediaImageAlign.Left ? "-active" : string.Empty
            
            #line default
            #line hidden
, 12791), false)
);

WriteLiteral(">\r\n                                <input");

WriteLiteral(" type=\"radio\"");

WriteLiteral(" name=\"ImageAlign\"");

WriteAttribute("value", Tuple.Create(" value=\"", 12934), Tuple.Create("\"", 12970)
            
            #line 193 "..\..\Views\Images\ImageEditor.cshtml"
, Tuple.Create(Tuple.Create("", 12942), Tuple.Create<System.Object, System.Int32>((int)MediaImageAlign.Left
            
            #line default
            #line hidden
, 12942), false)
);

WriteLiteral(" ");

            
            #line 193 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                                       Write(Model.ImageAlign == MediaImageAlign.Left ? "checked" : string.Empty);

            
            #line default
            #line hidden
WriteLiteral(" data-bind=\"checked: imageAlign.asString()\" />\r\n                            </div" +
">\r\n                            <div");

WriteAttribute("class", Tuple.Create(" class=\"", 13158), Tuple.Create("\"", 13253)
, Tuple.Create(Tuple.Create("", 13166), Tuple.Create("bcms-align-right", 13166), true)
            
            #line 195 "..\..\Views\Images\ImageEditor.cshtml"
, Tuple.Create(Tuple.Create("", 13182), Tuple.Create<System.Object, System.Int32>(Model.ImageAlign == MediaImageAlign.Right ? "-active" : string.Empty
            
            #line default
            #line hidden
, 13182), false)
);

WriteLiteral(">\r\n                                <input");

WriteLiteral(" type=\"radio\"");

WriteLiteral(" name=\"ImageAlign\"");

WriteAttribute("value", Tuple.Create(" value=\"", 13326), Tuple.Create("\"", 13363)
            
            #line 196 "..\..\Views\Images\ImageEditor.cshtml"
, Tuple.Create(Tuple.Create("", 13334), Tuple.Create<System.Object, System.Int32>((int)MediaImageAlign.Right
            
            #line default
            #line hidden
, 13334), false)
);

WriteLiteral(" ");

            
            #line 196 "..\..\Views\Images\ImageEditor.cshtml"
                                                                                                        Write(Model.ImageAlign == MediaImageAlign.Right ? "checked" : string.Empty);

            
            #line default
            #line hidden
WriteLiteral(" data-bind=\"checked: imageAlign.asString()\" />\r\n                            </div" +
">\r\n                        </div>\r\n                    </div>\r\n                <" +
"/div>\r\n            </div>\r\n");

            
            #line 202 "..\..\Views\Images\ImageEditor.cshtml"


            
            #line default
            #line hidden
WriteLiteral("            <div");

WriteLiteral(" class=\"bcms-form-block-holder\"");

WriteLiteral(">\r\n                <div>\r\n                    <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(" data-bind=\"with: categories\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 206 "..\..\Views\Images\ImageEditor.cshtml"
                   Write(Html.Partial("~/Areas/bcms-root/Views/Category/CategoriesTemplate.cshtml", categoriesTemplateViewModel));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n                </div>\r\n\r\n                <div>\r\n  " +
"                  <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(" data-bind=\"with: tags\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 212 "..\..\Views\Images\ImageEditor.cshtml"
                   Write(Html.Partial("~/Areas/bcms-root/Views/Tags/TagsTemplate.cshtml", tagsTemplateViewModel));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n                </div>\r\n            </div>\r\n");

            
            #line 216 "..\..\Views\Images\ImageEditor.cshtml"

            
            
            #line default
            #line hidden
            
            #line 217 "..\..\Views\Images\ImageEditor.cshtml"
       Write(Html.HiddenSubmit());

            
            #line default
            #line hidden
            
            #line 217 "..\..\Views\Images\ImageEditor.cshtml"
                                
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n</div>\r\n");

        }
    }
}
#pragma warning restore 1591

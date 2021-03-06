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
    
    #line 28 "..\..\Views\Page\AddNewPage.cshtml"
    using BetterCms.Module.Pages;
    
    #line default
    #line hidden
    
    #line 29 "..\..\Views\Page\AddNewPage.cshtml"
    using BetterCms.Module.Pages.Content.Resources;
    
    #line default
    #line hidden
    
    #line 30 "..\..\Views\Page\AddNewPage.cshtml"
    using BetterCms.Module.Pages.Controllers;
    
    #line default
    #line hidden
    
    #line 31 "..\..\Views\Page\AddNewPage.cshtml"
    using BetterCms.Module.Pages.ViewModels.Page;
    
    #line default
    #line hidden
    
    #line 32 "..\..\Views\Page\AddNewPage.cshtml"
    using BetterCms.Module.Root;
    
    #line default
    #line hidden
    
    #line 33 "..\..\Views\Page\AddNewPage.cshtml"
    using BetterCms.Module.Root.Content.Resources;
    
    #line default
    #line hidden
    
    #line 34 "..\..\Views\Page\AddNewPage.cshtml"
    using BetterCms.Module.Root.Mvc.Helpers;
    
    #line default
    #line hidden
    
    #line 35 "..\..\Views\Page\AddNewPage.cshtml"
    using BetterCms.Module.Root.ViewModels.Security;
    
    #line default
    #line hidden
    
    #line 36 "..\..\Views\Page\AddNewPage.cshtml"
    using BetterCms.Module.Root.ViewModels.Shared;
    
    #line default
    #line hidden
    
    #line 37 "..\..\Views\Page\AddNewPage.cshtml"
    using Microsoft.Web.Mvc;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Page/AddNewPage.cshtml")]
    public partial class _Views_Page_AddNewPage_cshtml : System.Web.Mvc.WebViewPage<AddNewPageViewModel>
    {
        public _Views_Page_AddNewPage_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

WriteLiteral("<div");

WriteLiteral(" class=\"bcms-tab-header bcms-js-tab-header\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"bcms-modal-frame-holder\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"bcms-tab-ui bcms-tab-item bcms-active\"");

WriteLiteral(" data-name=\"#bcms-tab-1\"");

WriteLiteral(">");

            
            #line 42 "..\..\Views\Page\AddNewPage.cshtml"
                                                                              Write(PagesGlobalization.AddNewPage_Title);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n        <div");

WriteLiteral(" class=\"bcms-tab-ui bcms-tab-item\"");

WriteLiteral(" data-name=\"#bcms-tab-2\"");

WriteLiteral(">");

            
            #line 43 "..\..\Views\Page\AddNewPage.cshtml"
                                                                  Write(PagesGlobalization.AddNewPage_Options);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n    </div>\r\n</div>\r\n\r\n<div");

WriteLiteral(" class=\"bcms-modal-frame-holder\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"bcms-messages-ui\"");

WriteLiteral(">\r\n        <ul");

WriteLiteral(" class=\"bcms-info-messages bcms-js-info-message\"");

WriteLiteral(">\r\n            <li>\r\n                <div");

WriteLiteral(" class=\"bcms-messages-close bcms-js-btn-close\"");

WriteLiteral(" id=\"bcms-addnewpage-closeinfomessage\"");

WriteLiteral(">");

            
            #line 51 "..\..\Views\Page\AddNewPage.cshtml"
                                                                                                    Write(RootGlobalization.Button_Close);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n");

WriteLiteral("                ");

            
            #line 52 "..\..\Views\Page\AddNewPage.cshtml"
           Write(Html.Raw(PagesGlobalization.AddNewPage_Template_InfoMessage_Text));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </li>\r\n        </ul>\r\n    </div>\r\n\r\n    <div");

WriteLiteral(" class=\"bcms-window-tabbed-options\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 58 "..\..\Views\Page\AddNewPage.cshtml"
   Write(Html.MessagesBox());

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 59 "..\..\Views\Page\AddNewPage.cshtml"
        
            
            #line default
            #line hidden
            
            #line 59 "..\..\Views\Page\AddNewPage.cshtml"
         using (Html.BeginForm<PageController>(f => f.AddNewPage(null, null), FormMethod.Post, new { @class = "bcms-ajax-form" }))
        {

            
            #line default
            #line hidden
WriteLiteral("            <div");

WriteLiteral(" id=\"bcms-tab-1\"");

WriteLiteral(" class=\"bcms-tab-single\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"bcms-form-block-holder\"");

WriteLiteral(">\r\n                    <div>\r\n");

            
            #line 64 "..\..\Views\Page\AddNewPage.cshtml"
                        
            
            #line default
            #line hidden
            
            #line 64 "..\..\Views\Page\AddNewPage.cshtml"
                         if (Model.Languages != null && Model.Languages.Any())
                        {
                            
            
            #line default
            #line hidden
            
            #line 66 "..\..\Views\Page\AddNewPage.cshtml"
                       Write(Html.HiddenFor(model => model.LanguageId, new { data_bind = "value: language.languageId()" }));

            
            #line default
            #line hidden
            
            #line 66 "..\..\Views\Page\AddNewPage.cshtml"
                                                                                                                          
                        }

            
            #line default
            #line hidden
WriteLiteral("                        ");

            
            #line 68 "..\..\Views\Page\AddNewPage.cshtml"
                   Write(Html.HiddenSubmit());

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 69 "..\..\Views\Page\AddNewPage.cshtml"
                        
            
            #line default
            #line hidden
            
            #line 69 "..\..\Views\Page\AddNewPage.cshtml"
                         if (Model.ShowLanguages)
                        {

            
            #line default
            #line hidden
WriteLiteral("                            <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(" data-bind=\"with: language\"");

WriteLiteral(">\r\n                                <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 72 "..\..\Views\Page\AddNewPage.cshtml"
                                                            Write(PagesGlobalization.AddNewPage_Language);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n");

WriteLiteral("                                ");

            
            #line 73 "..\..\Views\Page\AddNewPage.cshtml"
                           Write(Html.Tooltip(PagesGlobalization.AddNewPage_Language_Tooltip_Description));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                <div");

WriteLiteral(" class=\"bcms-field-wrapper\"");

WriteLiteral(">\r\n                                    <select");

WriteLiteral(" data-bind=\"options: languages, optionsText: \'value\', optionsValue: \'key\', value:" +
" languageId, select2: { minimumResultsForSearch: -1 }\"");

WriteLiteral("></select>\r\n                                    </div>\r\n                         " +
"       </div>\r\n");

            
            #line 78 "..\..\Views\Page\AddNewPage.cshtml"
                        }

            
            #line default
            #line hidden
WriteLiteral("\r\n                        <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 81 "..\..\Views\Page\AddNewPage.cshtml"
                                                        Write(PagesGlobalization.AddNewPage_PageTitle);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n");

WriteLiteral("                            ");

            
            #line 82 "..\..\Views\Page\AddNewPage.cshtml"
                       Write(Html.Tooltip(PagesGlobalization.AddNewPage_PageTitle_Tooltip_Description));

            
            #line default
            #line hidden
WriteLiteral("\r\n                            <div");

WriteLiteral(" class=\"bcms-field-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                                ");

            
            #line 84 "..\..\Views\Page\AddNewPage.cshtml"
                           Write(Html.TextBoxFor(f => f.PageTitle, new { @class = "bcms-field-text", @id = "PageTitle" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                ");

            
            #line 85 "..\..\Views\Page\AddNewPage.cshtml"
                           Write(Html.BcmsValidationMessageFor(f => f.PageTitle));

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </div>\r\n                        </div>\r\n\r\n");

WriteLiteral("                        ");

            
            #line 89 "..\..\Views\Page\AddNewPage.cshtml"
                   Write(Html.Partial("Partial/AddNewPageEditPermalink"));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n\r\n                    <div>\r\n");

            
            #line 93 "..\..\Views\Page\AddNewPage.cshtml"
                        
            
            #line default
            #line hidden
            
            #line 93 "..\..\Views\Page\AddNewPage.cshtml"
                         if (Model.AccessControlEnabled)
                        {
                            var viewModel = new UserAccessTemplateViewModel
                                                {
                                                    Title = PagesGlobalization.AddNewPage_UserAccess,
                                                    Tooltip = PagesGlobalization.AddNewPage_UserAccess_Tooltip_Description
                                                };
                            
            
            #line default
            #line hidden
            
            #line 100 "..\..\Views\Page\AddNewPage.cshtml"
                       Write(Html.Partial(RootModuleConstants.AccessControlTemplate, viewModel));

            
            #line default
            #line hidden
            
            #line 100 "..\..\Views\Page\AddNewPage.cshtml"
                                                                                               
                        }

            
            #line default
            #line hidden
WriteLiteral("                    </div>\r\n                </div>\r\n\r\n                <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"bcms-content-dialog-title\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"bcms-content-titles-align\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 108 "..\..\Views\Page\AddNewPage.cshtml"
                       Write(PagesGlobalization.AddNewPage_Template);

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                            ");

            
            #line 109 "..\..\Views\Page\AddNewPage.cshtml"
                       Write(Html.Tooltip(PagesGlobalization.AddNewPage_Template_Tooltip_Description));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </div>\r\n\r\n                        <!-- ko with: templat" +
"esList -->\r\n                        <div");

WriteLiteral(" class=\"bcms-top-block-inner\"");

WriteLiteral(" data-bind=\"css: { \'bcms-active-search\': searchEnabled() }\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" class=\"bcms-btn-search\"");

WriteLiteral(" data-bind=\"click: toggleSearch\"");

WriteLiteral(">");

            
            #line 114 "..\..\Views\Page\AddNewPage.cshtml"
                                                                                    Write(RootGlobalization.Button_Search);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                            <div");

WriteLiteral(" class=\"bcms-search-block\"");

WriteLiteral(">\r\n                                <input");

WriteLiteral(" type=\"text\"");

WriteLiteral(" class=\"bcms-search-field-box bcms-js-search-box\"");

WriteAttribute("placeholder", Tuple.Create(" placeholder=\"", 6227), Tuple.Create("\"", 6276)
            
            #line 116 "..\..\Views\Page\AddNewPage.cshtml"
                                 , Tuple.Create(Tuple.Create("", 6241), Tuple.Create<System.Object, System.Int32>(RootGlobalization.WaterMark_Search
            
            #line default
            #line hidden
, 6241), false)
);

WriteLiteral("\r\n                                       data-bind=\"value: searchQuery, valueUpda" +
"te: \'afterkeydown\', enterPress: search, hasFocus: hasFocus, enable: searchEnable" +
"d\"");

WriteLiteral(">\r\n                            </div>\r\n                        </div>\r\n          " +
"              <!-- /ko -->\r\n                    </div>\r\n\r\n");

WriteLiteral("                    ");

            
            #line 123 "..\..\Views\Page\AddNewPage.cshtml"
               Write(Html.Partial("Partial/TemplatesList", Model.Templates));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n\r\n");

WriteLiteral("                ");

            
            #line 126 "..\..\Views\Page\AddNewPage.cshtml"
           Write(Html.HiddenFor(f => f.TemplateId, new { @id = "TemplateId" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 127 "..\..\Views\Page\AddNewPage.cshtml"
           Write(Html.HiddenFor(f => f.MasterPageId, new { @id = "MasterPageId" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 128 "..\..\Views\Page\AddNewPage.cshtml"
           Write(Html.HiddenFor(f => f.ParentPageUrl));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 129 "..\..\Views\Page\AddNewPage.cshtml"
           Write(Html.HiddenFor(f => f.CreateMasterPage));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n");

            
            #line 131 "..\..\Views\Page\AddNewPage.cshtml"


            
            #line default
            #line hidden
WriteLiteral("            <div");

WriteLiteral(" id=\"bcms-tab-2\"");

WriteLiteral(" class=\"bcms-tab-single\"");

WriteLiteral(" data-bind=\"with: options\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 133 "..\..\Views\Page\AddNewPage.cshtml"
           Write(Html.Partial(PagesConstants.OptionValuesGridTemplate, new EditableGridViewModel { CanAddNewItems = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n");

            
            #line 135 "..\..\Views\Page\AddNewPage.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n</div>");

        }
    }
}
#pragma warning restore 1591

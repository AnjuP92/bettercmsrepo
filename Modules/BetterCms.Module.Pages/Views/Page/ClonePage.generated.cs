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
    
    #line 28 "..\..\Views\Page\ClonePage.cshtml"
    using BetterCms.Module.Pages.Content.Resources;
    
    #line default
    #line hidden
    
    #line 29 "..\..\Views\Page\ClonePage.cshtml"
    using BetterCms.Module.Pages.Controllers;
    
    #line default
    #line hidden
    
    #line 30 "..\..\Views\Page\ClonePage.cshtml"
    using BetterCms.Module.Pages.ViewModels.Page;
    
    #line default
    #line hidden
    
    #line 31 "..\..\Views\Page\ClonePage.cshtml"
    using BetterCms.Module.Root;
    
    #line default
    #line hidden
    
    #line 32 "..\..\Views\Page\ClonePage.cshtml"
    using BetterCms.Module.Root.Mvc;
    
    #line default
    #line hidden
    
    #line 33 "..\..\Views\Page\ClonePage.cshtml"
    using BetterCms.Module.Root.Mvc.Helpers;
    
    #line default
    #line hidden
    
    #line 34 "..\..\Views\Page\ClonePage.cshtml"
    using Microsoft.Web.Mvc;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Page/ClonePage.cshtml")]
    public partial class _Views_Page_ClonePage_cshtml : System.Web.Mvc.WebViewPage<ClonePageViewModel>
    {
        public _Views_Page_ClonePage_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

WriteLiteral("<div");

WriteLiteral(" class=\"bcms-modal-frame-holder\"");

WriteLiteral(">\r\n");

WriteLiteral("    ");

            
            #line 38 "..\..\Views\Page\ClonePage.cshtml"
Write(Html.MessagesBox());

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n    <div");

WriteLiteral(" class=\"bcms-window-options\"");

WriteLiteral(">\r\n");

            
            #line 41 "..\..\Views\Page\ClonePage.cshtml"
        
            
            #line default
            #line hidden
            
            #line 41 "..\..\Views\Page\ClonePage.cshtml"
         using (Html.BeginForm<PageController>(f => f.ClonePage((ClonePageViewModel)null), FormMethod.Post, new { @class = "bcms-ajax-form" }))
        {

            
            #line default
            #line hidden
WriteLiteral("            <div");

WriteLiteral(" class=\"bcms-content-info-container\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 44 "..\..\Views\Page\ClonePage.cshtml"
                                            Write(PagesGlobalization.ClonePage_Dialog_ConfirmationMessage_Header);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                <div");

WriteLiteral(" class=\"bcms-content-description\"");

WriteLiteral(">");

            
            #line 45 "..\..\Views\Page\ClonePage.cshtml"
                                                 Write(PagesGlobalization.ClonePage_Dialog_ConfirmationMessage_Text);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n            </div>\r\n");

            
            #line 47 "..\..\Views\Page\ClonePage.cshtml"
            

            
            #line default
            #line hidden
WriteLiteral("            <div");

WriteLiteral(" class=\"bcms-form-block-holder\"");

WriteLiteral(">\r\n                <div>\r\n                    <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 51 "..\..\Views\Page\ClonePage.cshtml"
                                                    Write(PagesGlobalization.ClonePage_Dialog_PageTitle);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n");

WriteLiteral("                        ");

            
            #line 52 "..\..\Views\Page\ClonePage.cshtml"
                   Write(Html.Tooltip(PagesGlobalization.ClonePage_Dialog_PageTitle_Tooltip_Description));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        <div");

WriteLiteral(" class=\"bcms-field-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 54 "..\..\Views\Page\ClonePage.cshtml"
                       Write(Html.TextBoxFor(m => m.PageTitle, new { @class = "bcms-field-text" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                            ");

            
            #line 55 "..\..\Views\Page\ClonePage.cshtml"
                       Write(Html.BcmsValidationMessageFor(f => f.PageTitle));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </div>\r\n                    </div>\r\n\r\n");

WriteLiteral("                    ");

            
            #line 59 "..\..\Views\Page\ClonePage.cshtml"
               Write(Html.Partial("Partial/ClonePageEditUrl"));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

            
            #line 61 "..\..\Views\Page\ClonePage.cshtml"
                    
            
            #line default
            #line hidden
            
            #line 61 "..\..\Views\Page\ClonePage.cshtml"
                     if (!Model.IsMasterPage && (ViewContext.Controller as CmsControllerBase).SecurityService.IsAuthorized(RootModuleConstants.UserRoles.Administration))
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <div");

WriteLiteral(" class=\"bcms-input-list-holder\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 64 "..\..\Views\Page\ClonePage.cshtml"
                                                        Write(PagesGlobalization.ClonePage_Dialog_AsMasterPage_Title);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n");

WriteLiteral("                            ");

            
            #line 65 "..\..\Views\Page\ClonePage.cshtml"
                       Write(Html.Tooltip(PagesGlobalization.ClonePage_Dialog_AsMasterPage_Tooltip_Description));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n                            <div");

WriteLiteral(" class=\"bcms-checkbox-holder\"");

WriteLiteral(">\r\n");

WriteLiteral("                                ");

            
            #line 68 "..\..\Views\Page\ClonePage.cshtml"
                           Write(Html.CheckBoxFor(model => model.CloneAsMasterPage));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                <div");

WriteLiteral(" class=\"bcms-checkbox-label bcms-js-edit-label\"");

WriteLiteral(">");

            
            #line 69 "..\..\Views\Page\ClonePage.cshtml"
                                                                               Write(PagesGlobalization.ClonePage_Dialog_AsMasterPage);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                            </div>\r\n                        </div>\r\n");

            
            #line 72 "..\..\Views\Page\ClonePage.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                </div>\r\n\r\n                <div>\r\n");

            
            #line 76 "..\..\Views\Page\ClonePage.cshtml"
                    
            
            #line default
            #line hidden
            
            #line 76 "..\..\Views\Page\ClonePage.cshtml"
                     if (Model.AccessControlEnabled)
                    {
                        
            
            #line default
            #line hidden
            
            #line 78 "..\..\Views\Page\ClonePage.cshtml"
                   Write(Html.Partial(RootModuleConstants.AccessControlTemplate));

            
            #line default
            #line hidden
            
            #line 78 "..\..\Views\Page\ClonePage.cshtml"
                                                                                
                    }

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 81 "..\..\Views\Page\ClonePage.cshtml"
               Write(Html.HiddenFor(m => m.PageId));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 82 "..\..\Views\Page\ClonePage.cshtml"
               Write(Html.HiddenSubmit());

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n            </div>\r\n");

            
            #line 85 "..\..\Views\Page\ClonePage.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n</div>\r\n");

        }
    }
}
#pragma warning restore 1591
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
    
    #line 28 "..\..\Views\Sitemap\NewPage.cshtml"
    using BetterCms.Module.Pages.Content.Resources;
    
    #line default
    #line hidden
    
    #line 29 "..\..\Views\Sitemap\NewPage.cshtml"
    using BetterCms.Module.Pages.Controllers;
    
    #line default
    #line hidden
    
    #line 30 "..\..\Views\Sitemap\NewPage.cshtml"
    using BetterCms.Module.Pages.ViewModels.Sitemap;
    
    #line default
    #line hidden
    
    #line 31 "..\..\Views\Sitemap\NewPage.cshtml"
    using BetterCms.Module.Root.Mvc.Helpers;
    
    #line default
    #line hidden
    
    #line 32 "..\..\Views\Sitemap\NewPage.cshtml"
    using Microsoft.Web.Mvc;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Sitemap/NewPage.cshtml")]
    public partial class _Views_Sitemap_NewPage_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {
        public _Views_Sitemap_NewPage_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

WriteLiteral("\r\n<div");

WriteLiteral(" class=\"bcms-tab-header bcms-js-tab-header\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"bcms-modal-frame-holder\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"bcms-sitemaps-arrow-left\"");

WriteLiteral(" data-bind=\"css: { \'bcms-sitemaps-arrow-inactive\': !slider.canSlideLeft() }, clic" +
"k: slider.slideLeft, visible: slider.showSliders\"");

WriteLiteral("></div>\r\n        <div");

WriteLiteral(" class=\"bcms-sitemap-tabs-holder\"");

WriteLiteral(">\r\n            <!-- ko foreach: tabs -->\r\n            <div");

WriteLiteral(" class=\"bcms-tab-ui bcms-tab-item\"");

WriteLiteral(" data-bind=\"text: newPageViewModel.sitemap.title(), css: { \'bcms-active\': isActiv" +
"e }, click: activate, attr: { id: tabId }, visible: isVisible\"");

WriteLiteral("></div>\r\n            <!-- /ko -->\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"bcms-sitemaps-arrow-right\"");

WriteLiteral(" data-bind=\"css: { \'bcms-sitemaps-arrow-inactive\': !slider.canSlideRight() }, cli" +
"ck: slider.slideRight, visible: slider.showSliders\"");

WriteLiteral("></div>\r\n    </div>\r\n</div>\r\n\r\n<div");

WriteLiteral(" class=\"bcms-modal-frame-holder\"");

WriteLiteral(" id=\"bcms-sitemap-addnewpage\"");

WriteLiteral(" data-bind=\"with: activeNewPageViewModel\"");

WriteLiteral(">\r\n");

WriteLiteral("    ");

            
            #line 47 "..\..\Views\Sitemap\NewPage.cshtml"
Write(Html.MessagesBox());

            
            #line default
            #line hidden
WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"bcms-window-tabbed-options\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"bcms-tab-single\"");

WriteLiteral(">\r\n            <!-- ko with: sitemap -->\r\n            <div");

WriteLiteral(" style=\"display: none;\"");

WriteLiteral(" data-bind=\"visible: showLanguages, with: language\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"bcms-top-block-holder\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"bcms-language-selector-box\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 54 "..\..\Views\Sitemap\NewPage.cshtml"
                                                    Write(NavigationGlobalization.Sitemap_EditDialog_LanguageSelectionTitle);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                        <div");

WriteLiteral(" class=\"bcms-field-wrapper\"");

WriteLiteral(">\r\n                            <select");

WriteLiteral(" data-bind=\"options: languages, optionsText: \'value\', optionsValue: \'key\', value:" +
" languageId, select2: { minimumResultsForSearch: -1 }\"");

WriteLiteral("></select>\r\n                        </div>\r\n                    </div>\r\n         " +
"       </div>\r\n            </div>\r\n            <!-- /ko -->\r\n            <div");

WriteLiteral(" class=\"bcms-content-dialog-title\"");

WriteLiteral(">\r\n                <!-- ko if: !linkIsDropped() -->\r\n                <div");

WriteLiteral(" class=\"bcms-content-titles-helper\"");

WriteLiteral(">");

            
            #line 64 "..\..\Views\Sitemap\NewPage.cshtml"
                                                   Write(NavigationGlobalization.Sitemap_AddNewPageDialog_PageNodeHeader);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                <!-- /ko -->\r\n                <!-- ko if: linkIsDropped()" +
" -->\r\n                <div");

WriteLiteral(" class=\"bcms-content-titles-helper\"");

WriteLiteral(">");

            
            #line 67 "..\..\Views\Sitemap\NewPage.cshtml"
                                                   Write(NavigationGlobalization.Sitemap_AddNewPageDialog_PageNodeFooter);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                <!-- /ko -->\r\n                <!-- ko with: sitemap -->\r\n" +
"                <div");

WriteLiteral(" class=\"bcms-tree-expander bcms-tree-expander-close\"");

WriteLiteral(" data-bind=\"click: callExpandOrCollapse, css: { \'bcms-tree-expander-close\' : allN" +
"odesExpanded()}, text: allNodesExpanded() ? \'");

            
            #line 70 "..\..\Views\Sitemap\NewPage.cshtml"
                                                                                                                                                                                                 Write(PagesGlobalization.CategoryTree_Button_CollapseAll);

            
            #line default
            #line hidden
WriteLiteral("\' : \'");

            
            #line 70 "..\..\Views\Sitemap\NewPage.cshtml"
                                                                                                                                                                                                                                                         Write(PagesGlobalization.CategoryTree_Button_ExpandAll);

            
            #line default
            #line hidden
WriteLiteral("\'\"");

WriteLiteral("></div>\r\n                <!-- /ko -->\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"bcms-tree-zones-holder\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"bcms-tree-drag-area-ui\"");

WriteLiteral(">\r\n                    <!-- ko if: !linkIsDropped() -->\r\n                    <!--" +
" ko with: pageLink -->\r\n                    <div");

WriteLiteral(" data-bind=\"draggable: $parentContext\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"bcms-drop-page-box\"");

WriteLiteral(" style=\"position: relative; z-index: 0;\"");

WriteLiteral(" data-bind=\"css: { \'bcms-node-box-drag\': isBeingDragged() }\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" class=\"bcms-drop-button\"");

WriteLiteral(">");

            
            #line 79 "..\..\Views\Sitemap\NewPage.cshtml"
                                                     Write(NavigationGlobalization.Sitemap_AddNewPageDialog_DragButton);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                            <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 80 "..\..\Views\Sitemap\NewPage.cshtml"
                                                        Write(NavigationGlobalization.Sitemap_AddNewPageDialog_PageName);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                            <div");

WriteLiteral(" class=\"bcms-drop-text\"");

WriteLiteral(" data-bind=\"text: title()\"");

WriteLiteral("></div>\r\n                            <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 82 "..\..\Views\Sitemap\NewPage.cshtml"
                                                        Write(NavigationGlobalization.Sitemap_AddNewPageDialog_PageUrl);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                            <div");

WriteLiteral(" class=\"bcms-drop-text\"");

WriteLiteral(" data-bind=\"text: url()\"");

WriteLiteral("></div>\r\n                        </div>\r\n                    </div>\r\n            " +
"        <div");

WriteLiteral(" class=\"bcms-node-drop-zone\"");

WriteLiteral(" data-bind=\"visible: isBeingDragged()\"");

WriteLiteral("></div>\r\n                    <div");

WriteLiteral(" class=\"bcms-content-titles\"");

WriteLiteral(">");

            
            #line 87 "..\..\Views\Sitemap\NewPage.cshtml"
                                                Write(NavigationGlobalization.Sitemap_AddNewPageDialog_UndoMessage);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                    <!-- /ko -->\r\n                    <!-- /ko -->\r\n     " +
"               <div");

WriteLiteral(" class=\"bcms-btn-main\"");

WriteLiteral(" data-bind=\"click: skipClicked, visible: !linkIsDropped()\"");

WriteLiteral(">");

            
            #line 90 "..\..\Views\Sitemap\NewPage.cshtml"
                                                                                                    Write(NavigationGlobalization.Sitemap_AddNewPageDialog_SkipButton);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                </div>\r\n                <div");

WriteLiteral(" class=\"bcms-tree-drop-area-ui\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"bcms-sitemap-newpage\"");

WriteLiteral(">\r\n                        ");

WriteLiteral("\r\n");

            
            #line 95 "..\..\Views\Sitemap\NewPage.cshtml"
                        
            
            #line default
            #line hidden
            
            #line 95 "..\..\Views\Sitemap\NewPage.cshtml"
                         using (Html.BeginForm<SitemapController>(f => f.SaveSitemap(null), FormMethod.Post, new { @class = "bcms-sitemap-form bcms-ajax-form" }))
                        {
                            
            
            #line default
            #line hidden
            
            #line 97 "..\..\Views\Sitemap\NewPage.cshtml"
                       Write(Html.Partial("Partial/Sitemap", new SitemapNodeViewModel()));

            
            #line default
            #line hidden
            
            #line 97 "..\..\Views\Sitemap\NewPage.cshtml"
                                                                                        
                        }

            
            #line default
            #line hidden
WriteLiteral("                    </div>\r\n                </div>\r\n            </div>\r\n        <" +
"/div>\r\n    </div>\r\n</div>\r\n");

            
            #line 105 "..\..\Views\Sitemap\NewPage.cshtml"
Write(Html.Partial("Partial/SitemapTemplate", new SitemapNodeViewModel()));

            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
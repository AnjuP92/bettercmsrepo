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
    
    #line 28 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
    using BetterCms.Module.Pages.Content.Resources;
    
    #line default
    #line hidden
    
    #line 29 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
    using BetterCms.Module.Root.Content.Resources;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Filter/PagesFilterTemplate.cshtml")]
    public partial class _Views_Filter_PagesFilterTemplate_cshtml : System.Web.Mvc.WebViewPage<BetterCms.Module.Pages.ViewModels.Filter.PagesGridViewModel<BetterCms.Module.Pages.ViewModels.SiteSettings.SiteSettingPageViewModel>>
    {
        public _Views_Filter_PagesFilterTemplate_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

WriteLiteral("\r\n");

WriteLiteral("<div");

WriteLiteral(" class=\"bcms-filter-holder\"");

WriteLiteral(" id=\"bcms-filter-template\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"bcms-sort-options\"");

WriteLiteral(" id=\"bcms-js-filter-sort\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"bcms-sort-options-label\"");

WriteLiteral(" data-bind=\"click: toggleShowSorting\"");

WriteLiteral(">");

            
            #line 35 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                                                                             Write(PagesGlobalization.SiteSettings_Pages_Sort);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n\r\n        <div");

WriteLiteral(" class=\"bcms-sort-options-block bcms-tooltip-tl\"");

WriteLiteral(" data-bind=\"visible: showSorting\"");

WriteLiteral(">\r\n            <!-- ko foreach: sortFields -->\r\n            <div");

WriteLiteral(" class=\"bcms-sort-option\"");

WriteLiteral(" data-bind=\"click: $parent.applySort.bind($data, title, column, direction)\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" data-bind=\"text: title, css: { \'bcms-sort-options-sorted\':direction == $parent.s" +
"ortDirection() && column == $parent.sortColumn() }\"");

WriteLiteral("></div>\r\n            </div>\r\n            <!-- /ko -->\r\n        </div>\r\n    </div>" +
"\r\n\r\n");

WriteLiteral("    ");

            
            #line 46 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
Write(Html.HiddenFor(m => m.ContentId));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"bcms-grid-filtering\"");

WriteLiteral(" data-bind=\"css: { \'bcms-active-filter\': isVisible() }\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"bcms-filterbox\"");

WriteLiteral(" data-bind=\"click: toggleFilter\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 49 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
       Write(PagesGlobalization.SiteSettings_Pages_Filter);

            
            #line default
            #line hidden
WriteLiteral("\r\n            <div");

WriteLiteral(" class=\"bcms-filter-modified\"");

WriteLiteral(" data-bind=\"style: { display: isEdited() ? \'inline-block\' : \'none\' }\"");

WriteLiteral(">");

            
            #line 50 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                                                                                                              Write(PagesGlobalization.SiteSettings_Pages_FilterIsModified);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n        </div>\r\n    </div>\r\n\r\n    <div");

WriteLiteral(" class=\"bcms-filter-selection-block\"");

WriteLiteral(" style=\"display: none;\"");

WriteLiteral(" data-bind=\"visible: isVisible(), enterPress: searchWithFilter\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"bcms-filter-controls\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"bcms-clearfix\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"bcms-filter-options\"");

WriteLiteral(" data-bind=\"with: tags\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"bcms-filter-text\"");

WriteLiteral(">");

            
            #line 58 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                                             Write(PagesGlobalization.SiteSettings_Pages_FilterByTags);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                    <div");

WriteLiteral(" class=\"bcms-field-wrapper\"");

WriteLiteral(">\r\n                        <input");

WriteLiteral(" type=\"text\"");

WriteLiteral(" class=\"bcms-field-text\"");

WriteLiteral(@" data-bind=""
                            css: { 'bcms-input-validation-error': newItem.hasError() },
                            value: newItem,
                            valueUpdate: 'afterkeydown',
                            escPress: clearItem,
                            autocompleteList: 'onlyExisting'""");

WriteLiteral("/>\r\n                        <!-- ko if: newItem.hasError() -->\r\n                 " +
"       <span");

WriteLiteral(" class=\"bcms-field-validation-error\"");

WriteLiteral(">\r\n                            <span");

WriteLiteral(" data-bind=\"text: newItem.validationMessage()\"");

WriteLiteral("></span>\r\n                        </span>\r\n                        <!-- /ko -->\r\n" +
"                    </div>\r\n                </div>\r\n                <div");

WriteLiteral(" class=\"bcms-filter-options\"");

WriteLiteral(" data-bind=\"with: categories\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"bcms-filter-text\"");

WriteLiteral(">");

            
            #line 74 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                                             Write(PagesGlobalization.SiteSettings_Pages_FilterByCategory);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                    <div");

WriteLiteral(" class=\"bcms-field-wrapper\"");

WriteLiteral(">\r\n                        <input");

WriteLiteral(" type=\"hidden\"");

WriteLiteral(" id=\"bcms-js-categories-select\"");

WriteLiteral("/>\r\n                    </div>\r\n                </div>\r\n");

            
            #line 79 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                
            
            #line default
            #line hidden
            
            #line 79 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                 if (Model.Languages != null && Model.Languages.Any())
                {

            
            #line default
            #line hidden
WriteLiteral("                    <div");

WriteLiteral(" class=\"bcms-filter-options\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"bcms-filter-text\"");

WriteLiteral(">");

            
            #line 82 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                                                 Write(PagesGlobalization.SiteSettings_Pages_FilterByLanguage);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                        <div");

WriteLiteral(" class=\"bcms-field-wrapper\"");

WriteLiteral(">\r\n                            <select");

WriteLiteral(" name=\"LanguageId\"");

WriteLiteral(" data-bind=\"options: languages, value: languageId, optionsText: \'Value\', optionsV" +
"alue: \'Key\', select2: { minimumResultsForSearch: -1 }\"");

WriteLiteral(" id=\"bcms-js-filter-languages\"");

WriteLiteral("/>\r\n                        </div>\r\n                    </div>\r\n");

            
            #line 87 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                }

            
            #line default
            #line hidden
WriteLiteral("                <div");

WriteLiteral(" class=\"bcms-filter-options\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"bcms-filter-text\"");

WriteLiteral(">");

            
            #line 89 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                                             Write(PagesGlobalization.SiteSettings_Pages_FilterByStatus);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                    <div");

WriteLiteral(" class=\"bcms-field-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 91 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                   Write(Html.DropDownListFor(m => m.Status, new List<SelectListItem>(), new { data_bind = "options: statuses, value: status, optionsText: 'Value', optionsValue: 'Key', select2: { minimumResultsForSearch: -1 }", id = "bcms-js-filter-status" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n                </div>\r\n                <div");

WriteLiteral(" class=\"bcms-filter-options\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"bcms-filter-text\"");

WriteLiteral(">");

            
            #line 95 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                                             Write(PagesGlobalization.SiteSettings_Pages_FilterBySEO);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                    <div");

WriteLiteral(" class=\"bcms-field-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 97 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                   Write(Html.DropDownListFor(m => m.SeoStatus, new List<SelectListItem>(), new { data_bind = "options: seoStatuses, value: seoStatus, optionsText: 'Value', optionsValue: 'Key', select2: { minimumResultsForSearch: -1 }", id = "bcms-js-filter-seostatus" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n                </div>\r\n\r\n                <div");

WriteLiteral(" class=\"bcms-filter-options\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"bcms-filter-text\"");

WriteLiteral(">");

            
            #line 102 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                                             Write(PagesGlobalization.SiteSettings_Pages_FilterByLayout);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                    <div");

WriteLiteral(" class=\"bcms-field-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 104 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                   Write(Html.DropDownListFor(m => m.Layout, new List<SelectListItem>(), new { data_bind = "options: layouts, value: layout, optionsText: 'Value', optionsValue: 'Key', select2: { minimumResultsForSearch: -1 }", id = "bcms-js-filter-layout" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n                </div>\r\n            </div>\r\n\r\n     " +
"       <div");

WriteLiteral(" class=\"bcms-single-tag-holder\"");

WriteLiteral(" data-bind=\"foreach: tags.items()\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"bcms-single-tag\"");

WriteLiteral(" data-bind=\"css: { \'bcms-single-tag-active\': isActive() }\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" data-bind=\"text: name()\"");

WriteLiteral("></div>\r\n                    <div");

WriteLiteral(" class=\"bcms-single-tag-remove\"");

WriteLiteral(" data-bind=\"click: remove\"");

WriteLiteral(">");

            
            #line 112 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                                                                             Write(RootGlobalization.Button_Remove);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                </div>\r\n                <input");

WriteLiteral(" type=\"hidden\"");

WriteLiteral(" data-bind=\"attr: { name: getItemInputName($index()) + \'.Key\', value: id() }\"");

WriteLiteral("/>\r\n                <input");

WriteLiteral(" type=\"hidden\"");

WriteLiteral(" data-bind=\"attr: { name: getItemInputName($index()) + \'.Value\', value: name() }\"" +
"");

WriteLiteral("/>\r\n            </div>\r\n\r\n            <div");

WriteLiteral(" class=\"bcms-single-tag-holder\"");

WriteLiteral(" data-bind=\"foreach: categories.items()\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"bcms-single-tag\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" data-bind=\"text: $data.text\"");

WriteLiteral("></div>\r\n                    <div");

WriteLiteral(" class=\"bcms-single-tag-remove\"");

WriteLiteral(" data-bind=\"click: $parent.categories.remove\"");

WriteLiteral(">");

            
            #line 121 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                                                                                                Write(RootGlobalization.Button_Remove);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                </div>\r\n                <input");

WriteLiteral(" type=\"hidden\"");

WriteLiteral(" data-bind=\"attr: { name: \'Categories[\' + $index() + \'].Key\', value: $data.id }\"");

WriteLiteral("/>\r\n                <input");

WriteLiteral(" type=\"hidden\"");

WriteLiteral(" data-bind=\"attr: { name: \'Categories[\' + $index() + \'].Value\', value: $data.text" +
" }\"");

WriteLiteral("/>\r\n            </div>\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"bcms-clearfix\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"bcms-checkbox-block\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"bcms-checkbox-holder\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 131 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
               Write(Html.CheckBoxFor(model => model.IncludeArchived, new { data_bind = "checked: includeArchived" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    <div");

WriteLiteral(" class=\"bcms-checkbox-label bcms-pointer\"");

WriteLiteral(" data-bind=\"click: changeIncludeArchived\"");

WriteLiteral(">");

            
            #line 132 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                                                                                                      Write(PagesGlobalization.SiteSettings_Pages_FilterIncludeArchived);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                </div>\r\n");

            
            #line 134 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                
            
            #line default
            #line hidden
            
            #line 134 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                 if (!Model.HideMasterPagesFiltering)
                {

            
            #line default
            #line hidden
WriteLiteral("                    <div");

WriteLiteral(" class=\"bcms-checkbox-holder\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 137 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                   Write(Html.CheckBoxFor(model => model.IncludeMasterPages, new { data_bind = "checked: includeMasterPages" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        <div");

WriteLiteral(" class=\"bcms-checkbox-label bcms-pointer\"");

WriteLiteral(" data-bind=\"click: changeIncludeMasterPages\"");

WriteLiteral(">");

            
            #line 138 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                                                                                                             Write(PagesGlobalization.SiteSettings_Pages_FilterIncludeMasterPages);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                    </div>\r\n");

            
            #line 140 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                }

            
            #line default
            #line hidden
WriteLiteral("            </div>\r\n\r\n            <div");

WriteLiteral(" class=\"bcms-btn-field-holder\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"bcms-btn-primary\"");

WriteLiteral(" data-bind=\"click: searchWithFilter\"");

WriteLiteral(">");

            
            #line 144 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                                                                             Write(PagesGlobalization.SiteSettings_Pages_FilterSearch);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                <div");

WriteLiteral(" class=\"bcms-btn-cancel\"");

WriteLiteral(" data-bind=\"click: clearFilter\"");

WriteLiteral(">");

            
            #line 145 "..\..\Views\Filter\PagesFilterTemplate.cshtml"
                                                                       Write(PagesGlobalization.SiteSettings_Pages_FilterClear);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n\r\n</div>");

        }
    }
}
#pragma warning restore 1591
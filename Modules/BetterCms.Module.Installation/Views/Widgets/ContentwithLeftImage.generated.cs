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
    
    #line 2 "..\..\Views\Widgets\ContentwithLeftImage.cshtml"
    using BetterCms.Module.Root.Mvc.Helpers;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Views\Widgets\ContentwithLeftImage.cshtml"
    using BetterCms.Module.Root.ViewModels.Cms;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Widgets/ContentwithLeftImage.cshtml")]
    public partial class _Views_Widgets_ContentwithLeftImage_cshtml : System.Web.Mvc.WebViewPage<BetterCms.Module.Root.ViewModels.Cms.RenderWidgetViewModel>
    {
        public _Views_Widgets_ContentwithLeftImage_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

WriteLiteral("<link");

WriteLiteral(" href=\"/file/bcms-installation/Content/Styles/css/bootstrap.css\"");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" />\r\n<link");

WriteLiteral(" href=\"/file/bcms-installation/Content/Styles/css/colors.css\"");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" />\r\n<link");

WriteLiteral(" href=\"/file/bcms-installation/Content/Styles/css/style.css\"");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" />\r\n\r\n<style>\r\n\r\n</style>\r\n\r\n\r\n");

            
            #line 14 "..\..\Views\Widgets\ContentwithLeftImage.cshtml"
 if (Model != null && Model.Options != null)
{
    string guid = Guid.NewGuid().ToString();
    string objId = "contentwithLeftImage" + guid;
    string leftImageSrc = null;
    string bgSize = null;
    string contenttext = null, bgImage = null, padding = null;
    foreach (var options in Model.Options)
    {
        if (options.Key == "leftImageSrc")
        {
            leftImageSrc = options.Value;
        }
        else if (options.Key == "bgSize")
        {
            bgSize = options.Value;
        }
        else if (options.Key == "backgroundImage")
        {
            bgImage = options.Value;
        }
        else if (options.Key == "padding")
        {
            padding = options.Value;
        }
        else if (options.Key == "contenttext")
        {
            contenttext = @options.Value;
        }
    }





            
            #line default
            #line hidden
WriteLiteral("    <div>\r\n        <section");

WriteLiteral(" class=\"section-whitebg\"");

WriteAttribute("style", Tuple.Create(" style=\"", 1376), Tuple.Create("\"", 1433)
, Tuple.Create(Tuple.Create("", 1384), Tuple.Create("background:url(", 1384), true)
            
            #line 49 "..\..\Views\Widgets\ContentwithLeftImage.cshtml"
, Tuple.Create(Tuple.Create("", 1399), Tuple.Create<System.Object, System.Int32>(bgImage
            
            #line default
            #line hidden
, 1399), false)
, Tuple.Create(Tuple.Create("", 1407), Tuple.Create(");background-size:", 1407), true)
            
            #line 49 "..\..\Views\Widgets\ContentwithLeftImage.cshtml"
         , Tuple.Create(Tuple.Create("", 1425), Tuple.Create<System.Object, System.Int32>(bgSize
            
            #line default
            #line hidden
, 1425), false)
, Tuple.Create(Tuple.Create("", 1432), Tuple.Create(";", 1432), true)
);

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"container\"");

WriteLiteral(">\r\n                <div");

WriteAttribute("id", Tuple.Create(" id=\"", 1494), Tuple.Create("\"", 1505)
            
            #line 51 "..\..\Views\Widgets\ContentwithLeftImage.cshtml"
, Tuple.Create(Tuple.Create("", 1499), Tuple.Create<System.Object, System.Int32>(objId
            
            #line default
            #line hidden
, 1499), false)
);

WriteLiteral(" class=\"general_wrapper clearfix\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"col-lg-5 col-md-5 col-sm-5 col-xs-12\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"entry\"");

WriteLiteral(" data-effect=\"slide-right\"");

WriteLiteral("><img");

WriteLiteral(" class=\"img-responsive\"");

WriteLiteral(" alt=\"Web Design Service\"");

WriteAttribute("src", Tuple.Create(" src=\"", 1735), Tuple.Create("\"", 1754)
            
            #line 53 "..\..\Views\Widgets\ContentwithLeftImage.cshtml"
                                               , Tuple.Create(Tuple.Create("", 1741), Tuple.Create<System.Object, System.Int32>(leftImageSrc
            
            #line default
            #line hidden
, 1741), false)
);

WriteLiteral("></div>\r\n                    </div><!-- end col-lg-6 -->\r\n                    <di" +
"v");

WriteLiteral(" class=\"col-lg-7 col-md-7 col-sm-7 col-xs-12\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 56 "..\..\Views\Widgets\ContentwithLeftImage.cshtml"
                   Write(Html.Raw(@contenttext));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div><!-- end col-lg-6 -->\r\n\r\n                </div><!-- g" +
"eneral -->\r\n            </div><!-- end container -->\r\n        </section><!-- end" +
" section-whitebg -->\t\r\n    </div>    \r\n");

            
            #line 63 "..\..\Views\Widgets\ContentwithLeftImage.cshtml"

}
            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591

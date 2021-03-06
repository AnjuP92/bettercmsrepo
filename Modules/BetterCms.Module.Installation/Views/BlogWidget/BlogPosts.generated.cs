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
    
    #line 28 "..\..\Views\BlogWidget\BlogPosts.cshtml"
    using BetterCms.Module.Installation.Models.Blog;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/BlogWidget/BlogPosts.cshtml")]
    public partial class _Views_BlogWidget_BlogPosts_cshtml : System.Web.Mvc.WebViewPage<BlogItemsModel>
    {
        public _Views_BlogWidget_BlogPosts_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\n");

WriteLiteral("<div");

WriteLiteral(" style=\"width:200px;height:100px;background-color:limegreen\"");

WriteLiteral(">\n");

WriteLiteral("    ");

            
            #line 31 "..\..\Views\BlogWidget\BlogPosts.cshtml"
Write(RenderSection("CMSHeader", false));

            
            #line default
            #line hidden
WriteLiteral("\n</div>\n");

            
            #line 33 "..\..\Views\BlogWidget\BlogPosts.cshtml"
 foreach (var item in Model.Items)
{

            
            #line default
            #line hidden
WriteLiteral("    <section");

WriteLiteral(" class=\"blog-post\"");

WriteLiteral(">\n        <h2>\n            <a");

WriteAttribute("href", Tuple.Create(" href=\"", 1559), Tuple.Create("\"", 1575)
            
            #line 37 "..\..\Views\BlogWidget\BlogPosts.cshtml"
, Tuple.Create(Tuple.Create("", 1566), Tuple.Create<System.Object, System.Int32>(item.Url
            
            #line default
            #line hidden
, 1566), false)
);

WriteLiteral(">");

            
            #line 37 "..\..\Views\BlogWidget\BlogPosts.cshtml"
                           Write(item.Title);

            
            #line default
            #line hidden
WriteLiteral("</a>\n        </h2>\n");

            
            #line 39 "..\..\Views\BlogWidget\BlogPosts.cshtml"
        
            
            #line default
            #line hidden
            
            #line 39 "..\..\Views\BlogWidget\BlogPosts.cshtml"
         if (Model.ShowAuthor)
        {

            
            #line default
            #line hidden
WriteLiteral("            <span");

WriteLiteral(" class=\"author\"");

WriteLiteral(">");

            
            #line 41 "..\..\Views\BlogWidget\BlogPosts.cshtml"
                            Write(item.Author);

            
            #line default
            #line hidden
WriteLiteral("</span>\n");

            
            #line 42 "..\..\Views\BlogWidget\BlogPosts.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("        ");

            
            #line 43 "..\..\Views\BlogWidget\BlogPosts.cshtml"
         if (Model.ShowDate)
        {

            
            #line default
            #line hidden
WriteLiteral("            <time");

WriteAttribute("datetime", Tuple.Create(" datetime=\"", 1767), Tuple.Create("\"", 1818)
            
            #line 45 "..\..\Views\BlogWidget\BlogPosts.cshtml"
, Tuple.Create(Tuple.Create("", 1778), Tuple.Create<System.Object, System.Int32>(item.PublishedOn.ToString("yyyy-MM-dd")
            
            #line default
            #line hidden
, 1778), false)
);

WriteLiteral(">");

            
            #line 45 "..\..\Views\BlogWidget\BlogPosts.cshtml"
                                                                 Write(item.PublishedOn.ToString("MMM d, yyyy"));

            
            #line default
            #line hidden
WriteLiteral("</time>\n");

            
            #line 46 "..\..\Views\BlogWidget\BlogPosts.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("        <br />\n        <span>\n");

            
            #line 49 "..\..\Views\BlogWidget\BlogPosts.cshtml"
            
            
            #line default
            #line hidden
            
            #line 49 "..\..\Views\BlogWidget\BlogPosts.cshtml"
             if (item.Tags.Count > 0 && Model.ShowTags)
            {

            
            #line default
            #line hidden
WriteLiteral("                <span>Tags:</span>\n");

            
            #line 52 "..\..\Views\BlogWidget\BlogPosts.cshtml"
                foreach (var tag in item.Tags)
                {

            
            #line default
            #line hidden
WriteLiteral("                    <a");

WriteAttribute("href", Tuple.Create(" href=\"", 2101), Tuple.Create("\"", 2134)
            
            #line 54 "..\..\Views\BlogWidget\BlogPosts.cshtml"
, Tuple.Create(Tuple.Create("", 2108), Tuple.Create<System.Object, System.Int32>(Request.Path
            
            #line default
            #line hidden
, 2108), false)
, Tuple.Create(Tuple.Create("", 2121), Tuple.Create("?blogtag=", 2121), true)
            
            #line 54 "..\..\Views\BlogWidget\BlogPosts.cshtml"
, Tuple.Create(Tuple.Create("", 2130), Tuple.Create<System.Object, System.Int32>(tag
            
            #line default
            #line hidden
, 2130), false)
);

WriteLiteral(" class=\"single-tag\"");

WriteLiteral(">");

            
            #line 54 "..\..\Views\BlogWidget\BlogPosts.cshtml"
                                                                       Write(tag);

            
            #line default
            #line hidden
WriteLiteral("</a>\n");

            
            #line 55 "..\..\Views\BlogWidget\BlogPosts.cshtml"
                }
            }

            
            #line default
            #line hidden
WriteLiteral("        </span>\n        <span>\n");

            
            #line 59 "..\..\Views\BlogWidget\BlogPosts.cshtml"
            
            
            #line default
            #line hidden
            
            #line 59 "..\..\Views\BlogWidget\BlogPosts.cshtml"
             if (item.Categories.Count > 0 && Model.ShowCategories)
            {

            
            #line default
            #line hidden
WriteLiteral("                <span>Categories:</span>\n");

            
            #line 62 "..\..\Views\BlogWidget\BlogPosts.cshtml"
                foreach (var category in item.Categories)
                {

            
            #line default
            #line hidden
WriteLiteral("                    <a");

WriteAttribute("href", Tuple.Create(" href=\"", 2448), Tuple.Create("\"", 2496)
            
            #line 64 "..\..\Views\BlogWidget\BlogPosts.cshtml"
, Tuple.Create(Tuple.Create("", 2455), Tuple.Create<System.Object, System.Int32>(Request.Path
            
            #line default
            #line hidden
, 2455), false)
, Tuple.Create(Tuple.Create("", 2468), Tuple.Create("?blogcategory=", 2468), true)
            
            #line 64 "..\..\Views\BlogWidget\BlogPosts.cshtml"
, Tuple.Create(Tuple.Create("", 2482), Tuple.Create<System.Object, System.Int32>(category.Name
            
            #line default
            #line hidden
, 2482), false)
);

WriteLiteral(" class=\"single-category\"");

WriteLiteral(">");

            
            #line 64 "..\..\Views\BlogWidget\BlogPosts.cshtml"
                                                                                           Write(category.Name);

            
            #line default
            #line hidden
WriteLiteral("</a>\n");

            
            #line 65 "..\..\Views\BlogWidget\BlogPosts.cshtml"
                }
            }

            
            #line default
            #line hidden
WriteLiteral("        </span>\n        <article>\n            <p>");

            
            #line 69 "..\..\Views\BlogWidget\BlogPosts.cshtml"
          Write(Html.Raw(item.IntroText));

            
            #line default
            #line hidden
WriteLiteral("</p>\n\n            <a");

WriteAttribute("href", Tuple.Create(" href=\"", 2667), Tuple.Create("\"", 2683)
            
            #line 71 "..\..\Views\BlogWidget\BlogPosts.cshtml"
, Tuple.Create(Tuple.Create("", 2674), Tuple.Create<System.Object, System.Int32>(item.Url
            
            #line default
            #line hidden
, 2674), false)
);

WriteLiteral(">&lt; Read more &gt;</a>\n        </article>\n    </section>\n");

            
            #line 74 "..\..\Views\BlogWidget\BlogPosts.cshtml"
}

            
            #line default
            #line hidden
WriteLiteral("\n");

            
            #line 76 "..\..\Views\BlogWidget\BlogPosts.cshtml"
 if (Model.ShowPager && Model.Items.Count > 0)
{

            
            #line default
            #line hidden
WriteLiteral("    <section");

WriteLiteral(" class=\"blog-pager\"");

WriteLiteral(">\n");

            
            #line 79 "..\..\Views\BlogWidget\BlogPosts.cshtml"
        
            
            #line default
            #line hidden
            
            #line 79 "..\..\Views\BlogWidget\BlogPosts.cshtml"
         for (int i = 1; i <= Model.NumberOfPages; i++)
        {
            if (Model.CurrentPage == i)
            {

            
            #line default
            #line hidden
WriteLiteral("                <a");

WriteAttribute("href", Tuple.Create(" href=\"", 2966), Tuple.Create("\"", 2998)
            
            #line 83 "..\..\Views\BlogWidget\BlogPosts.cshtml"
, Tuple.Create(Tuple.Create("", 2973), Tuple.Create<System.Object, System.Int32>(Request.Path
            
            #line default
            #line hidden
, 2973), false)
, Tuple.Create(Tuple.Create("", 2986), Tuple.Create("?blogpage=", 2986), true)
            
            #line 83 "..\..\Views\BlogWidget\BlogPosts.cshtml"
, Tuple.Create(Tuple.Create("", 2996), Tuple.Create<System.Object, System.Int32>(i
            
            #line default
            #line hidden
, 2996), false)
);

WriteLiteral(" class=\"blog-page\"");

WriteLiteral(" style=\"pointer-events: none; cursor: default\"");

WriteLiteral(">");

            
            #line 83 "..\..\Views\BlogWidget\BlogPosts.cshtml"
                                                                                                               Write(i);

            
            #line default
            #line hidden
WriteLiteral("</a>\n");

            
            #line 84 "..\..\Views\BlogWidget\BlogPosts.cshtml"
            }
            else
            {

            
            #line default
            #line hidden
WriteLiteral("                <a");

WriteAttribute("href", Tuple.Create(" href=\"", 3134), Tuple.Create("\"", 3166)
            
            #line 87 "..\..\Views\BlogWidget\BlogPosts.cshtml"
, Tuple.Create(Tuple.Create("", 3141), Tuple.Create<System.Object, System.Int32>(Request.Path
            
            #line default
            #line hidden
, 3141), false)
, Tuple.Create(Tuple.Create("", 3154), Tuple.Create("?blogpage=", 3154), true)
            
            #line 87 "..\..\Views\BlogWidget\BlogPosts.cshtml"
, Tuple.Create(Tuple.Create("", 3164), Tuple.Create<System.Object, System.Int32>(i
            
            #line default
            #line hidden
, 3164), false)
);

WriteLiteral(" class=\"blog-page\"");

WriteLiteral(">");

            
            #line 87 "..\..\Views\BlogWidget\BlogPosts.cshtml"
                                                                 Write(i);

            
            #line default
            #line hidden
WriteLiteral("</a>\n");

            
            #line 88 "..\..\Views\BlogWidget\BlogPosts.cshtml"
            }
        }

            
            #line default
            #line hidden
WriteLiteral("    </section>\n");

            
            #line 91 "..\..\Views\BlogWidget\BlogPosts.cshtml"
}

            
            #line default
            #line hidden
WriteLiteral("\n");

        }
    }
}
#pragma warning restore 1591

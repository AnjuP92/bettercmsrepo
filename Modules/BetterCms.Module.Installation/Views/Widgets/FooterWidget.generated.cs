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
    
    #line 1 "..\..\Views\Widgets\FooterWidget.cshtml"
    using BetterCms.Module.Root.Mvc.Helpers;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Views\Widgets\FooterWidget.cshtml"
    using BetterCms.Module.Root.ViewModels.Cms;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Widgets/FooterWidget.cshtml")]
    public partial class _Views_Widgets_FooterWidget_cshtml : System.Web.Mvc.WebViewPage<BetterCms.Module.Root.ViewModels.Cms.RenderWidgetViewModel>
    {
        public _Views_Widgets_FooterWidget_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n\r\n<link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" href=\"/file/bcms-installation/content/styles/newstyle.css\"");

WriteLiteral(" />\r\n\r\n \r\n");

            
            #line 10 "..\..\Views\Widgets\FooterWidget.cshtml"
 if (Model != null)
{
    int objCount = 0;
    if (Model.Options != null)
    {
        foreach (var option in Model.Options)
        {
            string logomaintext = string.Empty;
            string logosubtext =string.Empty;
            string linktext =string.Empty;
            string linkurl=string.Empty;
            string contact =string.Empty;
            string email =string.Empty;
            string address =string.Empty;                              
            string phoneno =string.Empty;
            string newsletter =string.Empty;
            string count =string.Empty;
            string bgimage =string.Empty;
            string bgcolor =string.Empty;
            if (@option.Key == "JsonFile")
            {
               foreach(var opt in Model.Options)
               {
                   if (@opt.Key == "BackgroundImage")
                   {
                       bgimage = @opt.Value;
                   }
                   else if (@opt.Key == "BackgroundColor")
                   {
                       bgcolor = @opt.Value;
                   }
               }
                RenderWidgetViewModel.FooterRootObject robj = null;
                robj = RenderWidgetViewModel.jsSerial.Deserialize<RenderWidgetViewModel.FooterRootObject>(@option.Value);
                if (robj != null && robj.Contact != null) { objCount++; }
                if (robj != null && robj.Links != null) { objCount++; }
                if (robj != null && robj.logo != null) { objCount++; }
                if (robj != null && robj.Newletter != null) { objCount++; }
                
               if (robj != null)
                 {

               

            
            #line default
            #line hidden
WriteLiteral("                         <div");

WriteLiteral(" class=\"footer\"");

WriteAttribute("style", Tuple.Create(" style=\"", 2014), Tuple.Create("\"", 2080)
, Tuple.Create(Tuple.Create("", 2022), Tuple.Create("background-image:", 2022), true)
, Tuple.Create(Tuple.Create(" ", 2039), Tuple.Create("url(", 2040), true)
            
            #line 53 "..\..\Views\Widgets\FooterWidget.cshtml"
, Tuple.Create(Tuple.Create("", 2044), Tuple.Create<System.Object, System.Int32>(bgimage
            
            #line default
            #line hidden
, 2044), false)
, Tuple.Create(Tuple.Create("", 2052), Tuple.Create(");background-color:", 2052), true)
            
            #line 53 "..\..\Views\Widgets\FooterWidget.cshtml"
                     , Tuple.Create(Tuple.Create("", 2071), Tuple.Create<System.Object, System.Int32>(bgcolor
            
            #line default
            #line hidden
, 2071), false)
, Tuple.Create(Tuple.Create("", 2079), Tuple.Create(";", 2079), true)
);

WriteLiteral(">\r\n                             \r\n                             <div");

WriteLiteral(" class=\"footerwidgetcontainer container\"");

WriteLiteral(" >\r\n                                 <div");

WriteLiteral(" class=\"row\"");

WriteLiteral(">\r\n\r\n");

            
            #line 58 "..\..\Views\Widgets\FooterWidget.cshtml"
                                     
            
            #line default
            #line hidden
            
            #line 58 "..\..\Views\Widgets\FooterWidget.cshtml"
                                      if (robj.logo != null && robj.logo.Count > 0)
                                     {

            
            #line default
            #line hidden
WriteLiteral("                                         <div");

WriteLiteral(" class=\"footersection\"");

WriteLiteral(">\r\n");

            
            #line 61 "..\..\Views\Widgets\FooterWidget.cshtml"
                                             
            
            #line default
            #line hidden
            
            #line 61 "..\..\Views\Widgets\FooterWidget.cshtml"
                                              foreach (var obj in robj.logo)
                                             {
                                                 logomaintext = obj.Maintext;
                                                 logosubtext = obj.Subtext;
                                                 if (logomaintext != null)
                                                 { 
                                                     if(logomaintext.StartsWith("http://"))
                                                     {

            
            #line default
            #line hidden
WriteLiteral("                                                     <div");

WriteLiteral(" class=\"footer_first-section\"");

WriteLiteral(" style=\"margin-top:20px;\"");

WriteLiteral(">\r\n                                                         <img class =\"footer_l" +
"ogo\" src=");

            
            #line 70 "..\..\Views\Widgets\FooterWidget.cshtml"
                                                                                  Write(logomaintext);

            
            #line default
            #line hidden
WriteLiteral(" />\r\n\r\n                                                     </div>\r\n");

            
            #line 73 "..\..\Views\Widgets\FooterWidget.cshtml"
                                                     }
                                                     else
                                                     {

            
            #line default
            #line hidden
WriteLiteral("                                                         <div");

WriteLiteral(" class=\"footer_first-section\"");

WriteLiteral(" style=\"margin-top:20px;\"");

WriteLiteral(">\r\n                                                         <p>\r\n                " +
"                                             <a");

WriteLiteral(" class=\"footer_maintext\"");

WriteLiteral(" style=\"color: #ffffff; text-decoration: none; font-weight: 800; font-style: norm" +
"al; font-size:40px;\"");

WriteLiteral(">");

            
            #line 78 "..\..\Views\Widgets\FooterWidget.cshtml"
                                                                                                                                                                                        Write(logomaintext);

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n                                                         </p>\r\n\r\n          " +
"                                               </div>\r\n");

            
            #line 82 "..\..\Views\Widgets\FooterWidget.cshtml"
                                                         
                                                     }
                                                 }
                                                 if (logosubtext != null)
                                                 {

            
            #line default
            #line hidden
WriteLiteral("                                                    <div");

WriteLiteral(" class=\"footer_first-section\"");

WriteLiteral(" style=\"\"");

WriteLiteral(">\r\n                                                        <p>\r\n                 " +
"                                           <a");

WriteLiteral(" class=\"footer_subtext\"");

WriteLiteral(" style=\"color: #ffffff;\"");

WriteLiteral(">");

            
            #line 89 "..\..\Views\Widgets\FooterWidget.cshtml"
                                                                                                         Write(logosubtext);

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n                                                        </p>\r\n             " +
"                                       </div>\r\n");

            
            #line 92 "..\..\Views\Widgets\FooterWidget.cshtml"
                                                 }

                                             }

            
            #line default
            #line hidden
WriteLiteral("                                         </div>\r\n");

            
            #line 96 "..\..\Views\Widgets\FooterWidget.cshtml"
                                     }

            
            #line default
            #line hidden
WriteLiteral("                                     ");

            
            #line 97 "..\..\Views\Widgets\FooterWidget.cshtml"
                                      if (robj.Links != null && robj.Links.Count > 0)
                                     {

            
            #line default
            #line hidden
WriteLiteral("                                         <div");

WriteLiteral(" class=\"footersection\"");

WriteLiteral(">\r\n                                             <div");

WriteLiteral(" class=\"footer_linktext\"");

WriteLiteral(" style=\"\"");

WriteLiteral("> \r\n                                                <p>\r\n");

            
            #line 102 "..\..\Views\Widgets\FooterWidget.cshtml"
                                                  
            
            #line default
            #line hidden
            
            #line 102 "..\..\Views\Widgets\FooterWidget.cshtml"
                                                   foreach (var obj in robj.Links)
                                                    {
                                                      linktext = obj.text;
                                                      linkurl = obj.url;

            
            #line default
            #line hidden
WriteLiteral("                                                      <a");

WriteAttribute("href", Tuple.Create(" href=", 5546), Tuple.Create("", 5560)
            
            #line 106 "..\..\Views\Widgets\FooterWidget.cshtml"
, Tuple.Create(Tuple.Create("", 5552), Tuple.Create<System.Object, System.Int32>(linkurl
            
            #line default
            #line hidden
, 5552), false)
);

WriteLiteral(">");

            
            #line 106 "..\..\Views\Widgets\FooterWidget.cshtml"
                                                                  Write(linktext);

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n");

WriteLiteral("                                                      <br />\r\n");

            
            #line 108 "..\..\Views\Widgets\FooterWidget.cshtml"
                                                      
                                                    }

            
            #line default
            #line hidden
WriteLiteral("                                                </p>\r\n                           " +
"                  </div>\r\n                                          </div>      " +
"                                    \r\n");

            
            #line 113 "..\..\Views\Widgets\FooterWidget.cshtml"
                                                                                 
                                     }

            
            #line default
            #line hidden
WriteLiteral("                                     ");

            
            #line 115 "..\..\Views\Widgets\FooterWidget.cshtml"
                                      if (robj.Contact != null && robj.Contact.Count > 0)
                                     {

            
            #line default
            #line hidden
WriteLiteral("                                         <div");

WriteLiteral(" class=\"footersection\"");

WriteLiteral(">\r\n");

            
            #line 118 "..\..\Views\Widgets\FooterWidget.cshtml"
                                             
            
            #line default
            #line hidden
            
            #line 118 "..\..\Views\Widgets\FooterWidget.cshtml"
                                              foreach (var obj in robj.Contact)
                                             {
                                                 email = obj.Emailid;
                                                 address = obj.Address;
                                                 phoneno = obj.Phoneno;

            
            #line default
            #line hidden
WriteLiteral("                                                 <div");

WriteLiteral(" class=\"footer_contactdetails\"");

WriteLiteral(">\r\n                                                     <p>");

            
            #line 124 "..\..\Views\Widgets\FooterWidget.cshtml"
                                                   Write(email);

            
            #line default
            #line hidden
WriteLiteral("</p>\r\n                                                     <p>");

            
            #line 125 "..\..\Views\Widgets\FooterWidget.cshtml"
                                                   Write(phoneno);

            
            #line default
            #line hidden
WriteLiteral("</p>  \r\n                                                     <p>");

            
            #line 126 "..\..\Views\Widgets\FooterWidget.cshtml"
                                                   Write(address);

            
            #line default
            #line hidden
WriteLiteral("</p>                                                                             " +
"                            \r\n                                                  " +
"   \r\n                                                 </div>\r\n");

            
            #line 129 "..\..\Views\Widgets\FooterWidget.cshtml"
                                             }

            
            #line default
            #line hidden
WriteLiteral("                                         </div>\r\n");

            
            #line 131 "..\..\Views\Widgets\FooterWidget.cshtml"
                                     }

            
            #line default
            #line hidden
WriteLiteral("                                     ");

            
            #line 132 "..\..\Views\Widgets\FooterWidget.cshtml"
                                      if (robj.Newletter != null && robj.Newletter.Count > 0)
                                     {

            
            #line default
            #line hidden
WriteLiteral("                                         <div");

WriteLiteral(" class=\"footersection\"");

WriteLiteral(">\r\n");

            
            #line 135 "..\..\Views\Widgets\FooterWidget.cshtml"
                                             
            
            #line default
            #line hidden
            
            #line 135 "..\..\Views\Widgets\FooterWidget.cshtml"
                                              foreach (var obj in robj.Newletter)
                                             {
                                                 newsletter = obj.text;

            
            #line default
            #line hidden
WriteLiteral("                                                 <div>\r\n                         " +
"                            <p");

WriteLiteral(" style=\"color: #ced2d5;text-align:left\"");

WriteLiteral(">");

            
            #line 139 "..\..\Views\Widgets\FooterWidget.cshtml"
                                                                                          Write(newsletter);

            
            #line default
            #line hidden
WriteLiteral("</p>\r\n                                                 </div>\r\n");

WriteLiteral("                                             <input");

WriteLiteral(" type=\"text\"");

WriteLiteral(" class=\"footer_emailtextbox\"");

WriteLiteral(" value=\"Email *\"");

WriteLiteral(" onblur=\"if(this.value == \'\'){ this.value = \'Email *\'; }\"");

WriteLiteral("\r\n                                                    onfocus=\"if(this.value == \'" +
"Email *\'){ this.value = \'\'; }\"");

WriteLiteral("/>\r\n");

WriteLiteral("                                                 <div");

WriteLiteral(" class=\"footersubscribe\"");

WriteLiteral(">\r\n                                                     <div>\r\n                  " +
"                                       <a");

WriteLiteral(" href=\"#\"");

WriteLiteral("><span>SUBSCRIBE</span></a>\r\n                                                    " +
" </div>\r\n\r\n                                                 </div>\r\n");

            
            #line 149 "..\..\Views\Widgets\FooterWidget.cshtml"
                                             }

            
            #line default
            #line hidden
WriteLiteral("                                         </div>\r\n");

            
            #line 151 "..\..\Views\Widgets\FooterWidget.cshtml"
                                     }

            
            #line default
            #line hidden
WriteLiteral("\r\n                                 </div>\r\n                             </div>   " +
"                          \r\n                         </div>                \r\n");

            
            #line 156 "..\..\Views\Widgets\FooterWidget.cshtml"


              }

            
            #line default
            #line hidden
WriteLiteral("                <input");

WriteLiteral(" type=\"hidden\"");

WriteLiteral(" id=\"hiddenbox\"");

WriteAttribute("value", Tuple.Create(" value=", 8905), Tuple.Create("", 8921)
            
            #line 159 "..\..\Views\Widgets\FooterWidget.cshtml"
, Tuple.Create(Tuple.Create("", 8912), Tuple.Create<System.Object, System.Int32>(objCount
            
            #line default
            #line hidden
, 8912), false)
);

WriteLiteral(" />\r\n");

            
            #line 160 "..\..\Views\Widgets\FooterWidget.cshtml"
            }
        }
    }


}

            
            #line default
            #line hidden
WriteLiteral("\r\n<script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">

    $(document).ready(function () {
        

        var propertyCount = $(""#hiddenbox"").val();        
        var value = 12 / propertyCount;        
        $("".footersection"").each(function () {
            $(this).addClass(""col-sm-"" + value);

        });
    });
</script>");

        }
    }
}
#pragma warning restore 1591

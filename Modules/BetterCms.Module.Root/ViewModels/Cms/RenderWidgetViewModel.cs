// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderWidgetViewModel.cs" company="Devbridge Group LLC">
// 
// Copyright (C) 2015,2016 Devbridge Group LLC
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
// 
// <summary>
// Better CMS is a publishing focused and developer friendly .NET open source CMS.
// 
// Website: https://www.bettercms.com 
// GitHub: https://github.com/devbridge/bettercms
// Email: info@bettercms.com
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

using BetterCms.Core.DataContracts;
using System.Web.Script.Serialization;

namespace BetterCms.Module.Root.ViewModels.Cms
{
    /// <summary>
    /// Render view model for server widgets.
    /// </summary>
    public class RenderWidgetViewModel
    {
        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>
        /// The page.
        /// </value>
        public IRenderPage Page { get; set; }

        /// <summary>
        /// Gets or sets the widget.
        /// </summary>
        /// <value>
        /// The widget.
        /// </value>
        public IWidget Widget { get; set; }

        /// <summary>
        /// Gets or sets the widget options.
        /// </summary>
        /// <value>
        /// The widget options.
        /// </value>
        public IList<IOptionValue> Options { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Page != null)
            {
                sb.AppendFormat("Page: Id={0}, Title={1}", Page.Id, Page.Title);
            }
            else
            {
                sb.AppendFormat("Page property is empty");
            }

            sb.Append("; ");

            if (Widget != null)
            {
                sb.AppendFormat("Widget: Id={0}, Name={1}", Widget.Id, Widget.Name);
            }
            else
            {
                sb.AppendFormat("Widget property is empty");
            }

            sb.Append(".");

            return sb.ToString();
        }
        
        public class Menu
        {
            public string menuname_or_menulogo { get; set; }
           // public string menuLogo { get; set; }
        }

        public class MenuSub
        {
            public string text { get; set; }
            public string link { get; set; }
        }

        public class MenuDatum
        {
            public string text { get; set; }
            public string link { get; set; }
            public List<MenuSub> sub { get; set; }
        }

        public class MenuRootObject
        {
            public List<Menu> menu { get; set; }
            public List<MenuDatum> data { get; set; }
        }

        public class PortfolioFilter
        {
            public string filter { get; set; }
        }

        public class PortfolioImage
        {
            public string image { get; set; }
            public string group { get; set; }
        }

        public class PortfolioRootObject
        {
            public List<PortfolioFilter> portfolioFilters { get; set; }
            public List<PortfolioImage> Images { get; set; }
        }

        public class Marquee
        {
            public string marquee_images { get; set; }
        }

        public class MarqueeRootObject
        {
            public List<Marquee> marquee { get; set; }
        }

        public class SlickSliderDetail
        {
            public string Image { get; set; }
            public string Name { get; set; }
            public string Position { get; set; }
            public string Content { get; set; }
        }

        public class SlickSliderRootObject
        {
            public List<SlickSliderDetail> Details { get; set; }
        }
        public class Datum
        {
            public string imageurl { get; set; }
            public string heading { get; set; }
            public string text { get; set; }
            public string iconurl { get; set; }
        }
        
        public class RootObject
        {
            public List<Datum> data { get; set; }
           
        }
        public class Logo
        {
            public string Maintext { get; set; }
            public string Subtext { get; set; }
        }

        public class Link
        {
            public string text { get; set; }
            public string url { get; set; }
        }

        public class Contact
        {
            public string Emailid { get; set; }
            public string Phoneno { get; set; }
            public string Address { get; set; }
        }

        public class Newletter
        {
            public string text { get; set; }
        }

        public class FooterRootObject
        {
            public List<Logo> logo { get; set; }
            public List<Link> Links { get; set; }
            public List<Contact> Contact { get; set; }
            public List<Newletter> Newletter { get; set; }
        }
		
		public class SliderRootObject
        {
        
            public string parallaximage { get; set; }
            public string parallaxheading { get; set; }
            public List<parallaxdiv> parallaxdiv { get; set; }
            public string Sliderheight { get; set; }
            public List<Sliderhtml> sliderhtml { get; set; }
         	public string acordionbackground { get; set; }
            public string panelbackground { get; set; }
            public List<Accordion> accordion { get; set; }
}
        public class Sliderhtml
        {
            public string id { get; set; }
            public string sliderimage { get; set; }
            public string slidervideo { get; set; }
            public string slidertext { get; set; }
            public string buttontext { get; set; }
            public string buttoncolor { get; set; }
            public string bordercolor { get; set; }
            public string href { get; set; }
        }
        public class parallaxdiv
        {
            public string parallaxtext { get; set; }
        }

        public class Html
        {
            public string text { get; set; }
        }

        public class CounterRootObject
        {
            public List<Html> html { get; set; }
        }
     

		public class Accordion
        {
           
            public string accordiontitle { get; set; }
            public string accordiontext { get; set; }
        }

        public class Sliderdata
        {
            public string id { get; set; }
            public string sliderimage { get; set; }
            public string sliderCaption { get; set; }
            public string slidersmalltext { get; set; }
        }

        public class BasicSliderRootObject
        {
            public List<Sliderdata> sliderdata { get; set; }
        }

        public class Accordiondata
        {
            public string id { get; set; }
            public string heading { get; set; }
            public string content { get; set; }
        }

        public class AccordionRootObject
        {
            public List<Accordiondata> accordiondata { get; set; }
        }

        public class NewsportfolioItem
        {
            public string id { get; set; }
            public string filterbuttonname { get; set; }
            public string filterclassname { get; set; }
            public string imageurl { get; set; }
            public string imagecaption { get; set; }
            public string imagedescrip { get; set; }
            public string backgroundcolor { get; set; }
        }

        public class NewsportfolioRootObject
        {
            public List<NewsportfolioItem> newsportfolio_item { get; set; }
        }

        public static JavaScriptSerializer jsSerial = new JavaScriptSerializer(); 

    }
}
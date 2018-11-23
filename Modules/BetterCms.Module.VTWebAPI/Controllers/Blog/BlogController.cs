using BetterCms.Core.Security;
using BetterCms.Module.Root;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;





namespace BetterCms.Module.VTWebAPI.Controllers.Blog
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
   
    public class BlogController : ApiController
    {

        [HttpGet]
        
        public string SaveAuthor(string JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string id = Info.Id;
            int version = Info.version;
            string isdeleted = Info.isdeleted;
            string name = Info.name;
            string imageId = Info.imageId;
            string description = Info.description;

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spAddNewAuthor";
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@Version", version);
                    cmd.Parameters.AddWithValue("@IsDeleted", isdeleted);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@ImageId", ((object)imageId) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", ((object)description ?? DBNull.Value));
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    return dt.Rows[0][0].ToString();
                }
            }
        }

        [HttpGet]
        public string UpdateAuthor(string JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string id = Info.Id;
            int version = Info.version;
            string name = Info.name;
            string imageId = Info.imageId;
            string description = Info.description;

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spUpdateAuthor";
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@Version", version);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@ImageId", ((object)imageId) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", ((object)description) ?? DBNull.Value);
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    return dt.Rows[0][0].ToString();

                }
            }
        }
        [HttpGet]
        public JObject DeleteAuthor(string id)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spDeleteAuthor";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", id);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        dynamic response = new JObject();
                        response.IsDeleted = dt.Rows[0][0];
                        response.Version = dt.Rows[0][1];
                        return response;
                    }
                    catch (Exception ex)
                    {
                        return new JObject();
                    }

                }
            }

        }

        [HttpGet]
        public string SaveBlogPost(string JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            int status = 4;
            string id = Info.Id;
            string authorId = Info.authorId;
            string activationDate = Info.activationDate;
            string expirationDate = Info.expirationDate;
            int version = Info.version;
            string isDeleted = Info.isDeleted;
            string pageUrl = Info.pageUrl;
            string title = Info.title;
            string layoutId = Info.layoutId;
            string publishedOn = Info.publishedOn;
            string metaTitle = Info.metaTitle;
            string metaKeywords = Info.metaKeywords;
            string metaDescription = Info.metaDescription;

            if (Info.status == "Preview")
            {
                status = 1;
            }
            else if (Info.status == "Draft")
            {
                status = 2;
            }
            else if (Info.status == "Published")
            {
                status = 3;
            }
            else if (Info.status == "Unpublished")
            {
                status = 4;
            }
            string pageUrlHash = Info.pageUrlHash;
            string masterPageId = Info.masterPageId;
            string isMasterPage = Info.isMasterPage;
            string languageId = Info.languageId;
            string languageGroupIdentifier = Info.languageGroupIdentifier;
            int forceAccessProtocol = 0;
            if (Info.forceAccessProtocol == "None")
            {
                forceAccessProtocol = 0;
            }
            else if (Info.forceAccessProtocol == "ForceHttp")
            {
                forceAccessProtocol = 1;
            }
            else if (Info.forceAccessProtocol == "ForceHttps")
            {
                forceAccessProtocol = 2;
            }
            string description = Info.description;
            string imageId = Info.imageId;
            string customCss = Info.customCss;
            string customJs = Info.customJs;
            string useCanonicalUrl = Info.useCanonicalUrl;
            string useNoFollow = Info.useNoFollow;
            string useNoIndex = Info.useNoIndex;
            string catergoryId = Info.catergoryId;
            string secondaryImageId = Info.secondaryImageId;
            string featuredImageId = Info.featuredImageId;
            string isArchived = Info.isArchived;

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spAddBlogPosts";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@AuthorId", ((object)authorId) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActivationDate", activationDate);
                        cmd.Parameters.AddWithValue("@ExpirationDate", ((object)expirationDate) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Version", version);
                        cmd.Parameters.AddWithValue("@IsDeleted", isDeleted);
                        cmd.Parameters.AddWithValue("@PageUrl", pageUrl);
                        cmd.Parameters.AddWithValue("@Title", title);
                        cmd.Parameters.AddWithValue("@LayoutId", ((object)layoutId) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PublishedOn", ((object)publishedOn) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MetaTitle", ((object)metaTitle) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MetaKeywords", ((object)metaKeywords) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MetaDescription", ((object)metaDescription) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@pageUrlHash", pageUrlHash);
                        cmd.Parameters.AddWithValue("@masterPageId", ((object)masterPageId) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@isMasterPage", isMasterPage);
                        cmd.Parameters.AddWithValue("@languageId", ((object)languageId) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@languageGroupIdentifier", ((object)languageGroupIdentifier) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@forceAccessProtocol", forceAccessProtocol);
                        cmd.Parameters.AddWithValue("@Description", ((object)description) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ImageId", ((object)imageId) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CustomCss", ((object)customCss) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CustomJs", ((object)customJs) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UseCanonicalUrl", useCanonicalUrl);
                        cmd.Parameters.AddWithValue("@UseNoFollow", useNoFollow);
                        cmd.Parameters.AddWithValue("@UseNoIndex", useNoIndex);
                        cmd.Parameters.AddWithValue("@CatergoryId", ((object)catergoryId) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SecondaryImageId", ((object)secondaryImageId) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FeaturedImageId", ((object)featuredImageId) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsArchived", isArchived);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return dt.Rows[0][0].ToString();

                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }

        }

        [HttpGet]
        public string SaveCategory(string JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string id = Info.id;
            int version = Info.version;
            string isDeleted = Info.isDeleted;
            string pageId = Info.pageId;
            string categoryId = Info.categoryId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spAddBlogCategory";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Version", version);
                        cmd.Parameters.AddWithValue("@IsDeleted", isDeleted);
                        cmd.Parameters.AddWithValue("@PageId", pageId);
                        cmd.Parameters.AddWithValue("@CategoryId ", categoryId);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return dt.Rows[0][0].ToString();
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }

        }


        [HttpGet]
        public string SaveBlogPostContent(string JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string id = Info.id;
            string activationdDate = Info.activationdDate;
            string expirationDate = Info.expirationDate;
            string customCss = Info.customCss;
            string useCustomCss = Info.useCustomCss;
            string customJs = Info.customJs;
            string useCustomJs = Info.useCustomJs;
            string html = Info.html;
            string editInSourceMode = Info.editInSourceMode;
            string originalText = Info.originalText;
            int contentTextMode = 1;
            if (Info.contentTextMode == "Html")
            {
                contentTextMode = 1;
            }
            else if (Info.contentTextMode == "Markdown")
            {
                contentTextMode = 2;
            }
            else if (Info.contentTextMode == "SimpleText")
            {
                contentTextMode = 3;
            }
            int version = Info.version;
            string isDeleted = Info.isDeleted;
            string name = Info.name;
            string previewUrl = Info.previewUrl;
            int status = 4;
            string publishedOn = Info.publishedOn;
            if (Info.status == "Preview")
            {
                status = 1;

            }
            else if (Info.status == "Draft")
            {
                status = 2;

            }
            else if (Info.status == "Published")
            {
                status = 3;

            }
            else if (Info.status == "Unpublished")
            {
                status = 4;

            }
            string publishedByUser = Info.publishedByUser;
            string originalId = Info.originalId;

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spAddBlogPostContent";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@ActivationdDate", activationdDate);
                        cmd.Parameters.AddWithValue("@ExpirationDate", ((object)expirationDate) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CustomCss", ((object)customCss) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UseCustomCss", useCustomCss);
                        cmd.Parameters.AddWithValue("@CustomJs", ((object)customJs) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UseCustomJs", useCustomJs);
                        cmd.Parameters.AddWithValue("@Html", html);
                        cmd.Parameters.AddWithValue("@EditInSourceMode", editInSourceMode);
                        cmd.Parameters.AddWithValue("@OriginalText", ((object)originalText) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ContentTextMode", contentTextMode);
                        cmd.Parameters.AddWithValue("@Version", version);
                        cmd.Parameters.AddWithValue("@IsDeleted", isDeleted);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@PreviewUrl", ((object)previewUrl) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@PublishedOn", ((object)publishedOn) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PublishedByUser", ((object)publishedByUser) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OriginalId", ((object)originalId) ?? DBNull.Value);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return dt.Rows[0][0].ToString();

                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }

        }
        [HttpGet]
        public string SavePageContent(string JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string id = Info.id;
            int version = Info.version;
            string isDeleted = Info.isDeleted;
            string pageId = Info.pageId;
            string contentId = Info.contentId;
            string regionId = Info.regionId;
            string order = Info.order;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spAddBlogPageContent";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Version", version);
                        cmd.Parameters.AddWithValue("@IsDeleted", isDeleted);
                        cmd.Parameters.AddWithValue("@PageId", pageId);
                        cmd.Parameters.AddWithValue("@ContentId", contentId);
                        cmd.Parameters.AddWithValue("@RegionId", regionId);
                        cmd.Parameters.AddWithValue("@Order", order);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return dt.Rows[0][0].ToString();

                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }

        }
        [HttpGet]
        public string SaveRedirect(string JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string id = Info.id;
            int version = Info.version;
            string isDeleted = Info.isDeleted;
            string pageUrl = Info.pageUrl;
            string redirectUrl = Info.redirectUrl;

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spAddRedirect";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Version", version);
                        cmd.Parameters.AddWithValue("@IsDeleted", isDeleted);
                        cmd.Parameters.AddWithValue("@PageUrl", pageUrl);
                        cmd.Parameters.AddWithValue("@RedirectUrl", redirectUrl);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return dt.Rows[0][0].ToString();
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }
        }

        [HttpGet]
        public string GetMinBlogPostDate()
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetMinBlogPostDate";
                        cmd.Connection = con;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return dt.Rows[0][0].ToString();
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }
        }

        [HttpGet]
        public string GetAuthorId(String title)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetAuthorId";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Name", title);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return dt.Rows[0][0].ToString();
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }

                }
            }
        }
        [HttpGet]
        public string GetBlogPostDetails(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string pageId = Info.pageId;
            string authorId = Info.authorId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetBlogPostDetails";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PageId", pageId);
                        cmd.Parameters.AddWithValue("@AuthorId", authorId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataSet dt = new DataSet();
                        ad.Fill(dt);
                        JObject JReturnitem = new JObject();
                        JReturnitem.Add("ActivationDate", dt.Tables[0].Rows[0]["ActivationDate"].ToString());
                        JReturnitem.Add("ExpirationDate", dt.Tables[0].Rows[0]["ExpirationDate"].ToString());
                        JReturnitem.Add("Description", dt.Tables[0].Rows[0]["Description"].ToString());
                        JReturnitem.Add("ImageId", dt.Tables[0].Rows[0]["ImageId"].ToString());
                        JReturnitem.Add("CustomCss", dt.Tables[0].Rows[0]["CustomCss"].ToString());

                        JReturnitem.Add("CustomJS", dt.Tables[0].Rows[0]["CustomJS"].ToString());
                        JReturnitem.Add("UseCanonicalUrl", dt.Tables[0].Rows[0]["UseCanonicalUrl"].ToString());
                        JReturnitem.Add("UseNoFollow", dt.Tables[0].Rows[0]["UseNoFollow"].ToString());
                        JReturnitem.Add("UseNoIndex", dt.Tables[0].Rows[0]["UseNoIndex"].ToString());
                        JReturnitem.Add("CategoryId", dt.Tables[0].Rows[0]["CategoryId"].ToString());

                        JReturnitem.Add("SecondaryImageId", dt.Tables[0].Rows[0]["SecondaryImageId"].ToString());
                        JReturnitem.Add("FeaturedImageId", dt.Tables[0].Rows[0]["FeaturedImageId"].ToString());
                        JReturnitem.Add("IsArchived", dt.Tables[0].Rows[0]["IsArchived"].ToString());
                        JReturnitem.Add("Version", dt.Tables[0].Rows[0]["Version"].ToString());
                        JReturnitem.Add("PageUrl", dt.Tables[0].Rows[0]["PageUrl"].ToString());
                        JReturnitem.Add("Title", dt.Tables[0].Rows[0]["Title"].ToString());
                        JReturnitem.Add("LayoutId", dt.Tables[0].Rows[0]["LayoutId"].ToString());
                        JReturnitem.Add("MetaTitle", dt.Tables[0].Rows[0]["MetaTitle"].ToString());
                        JReturnitem.Add("MetaKeywords", dt.Tables[0].Rows[0]["MetaKeywords"].ToString());
                        JReturnitem.Add("MetaDescription", dt.Tables[0].Rows[0]["MetaDescription"].ToString());

                        JReturnitem.Add("Status", dt.Tables[0].Rows[0]["Status"].ToString());
                        JReturnitem.Add("PageUrlHash", dt.Tables[0].Rows[0]["PageUrlHash"].ToString());
                        JReturnitem.Add("MasterPageId", dt.Tables[0].Rows[0]["MasterPageId"].ToString());
                        JReturnitem.Add("IsMasterPage", dt.Tables[0].Rows[0]["IsMasterPage"].ToString());
                        JReturnitem.Add("LanguageId", dt.Tables[0].Rows[0]["LanguageId"].ToString());
                        JReturnitem.Add("LanguageGroupIdentifier", dt.Tables[0].Rows[0]["LanguageGroupIdentifier"].ToString());
                        JReturnitem.Add("ForceAccessProtocol", dt.Tables[0].Rows[0]["ForceAccessProtocol"].ToString());

                        JReturnitem.Add("Author_Name", dt.Tables[1].Rows[0]["Name"].ToString());
                        JReturnitem.Add("Author_Description", dt.Tables[1].Rows[0]["Description"].ToString());
                        JReturnitem.Add("Author_ImageId", dt.Tables[1].Rows[0]["ImageId"].ToString());
                        JReturnitem.Add("Author_Version", dt.Tables[1].Rows[0]["Version"].ToString());

                        string js = JsonConvert.SerializeObject(JReturnitem);
                        return js;

                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }

        }
        [HttpGet]
        public JArray GetBlogPageCategoryDetails(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string pageId = Info.pageId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetBlogPageCategoryDetails";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PageId", pageId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);

                        JArray jarrayObj = new JArray();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            JObject NewJObject = new JObject();
                            NewJObject.Add("PagecategoryId", dt.Rows[i]["Id"].ToString());
                            NewJObject.Add("Version", dt.Rows[i]["Version"].ToString());
                            NewJObject.Add("IsDeleted", dt.Rows[i]["IsDeleted"].ToString());
                            NewJObject.Add("CreatedOn", dt.Rows[i]["CreatedOn"].ToString());
                            NewJObject.Add("CreatedByUser", dt.Rows[i]["CreatedByUser"].ToString());
                            NewJObject.Add("ModifiedOn", dt.Rows[i]["ModifiedOn"].ToString());
                            NewJObject.Add("ModifiedByUser", dt.Rows[i]["ModifiedByUser"].ToString());
                            NewJObject.Add("DeletedOn", dt.Rows[i]["DeletedOn"].ToString());
                            NewJObject.Add("DeletedByUser", dt.Rows[i]["DeletedByUser"].ToString());
                            NewJObject.Add("CategoryId", dt.Rows[i]["CategoryId"].ToString());                            
                            jarrayObj.Add(NewJObject);
                        }

                        return jarrayObj;
                    }
                    catch (Exception ex)
                    {
                        return new JArray();
                    }
                }
            }

        }

        [HttpGet]
        public string GetBlogCategoryDetails(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string categoryId = Info.categoryId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetBlogCategoryDetails";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JObject JReturnitem = new JObject();
                        JReturnitem.Add("Version", dt.Rows[0]["Version"].ToString());
                        JReturnitem.Add("IsDeleted", dt.Rows[0]["IsDeleted"].ToString());
                        JReturnitem.Add("CreatedOn", dt.Rows[0]["CreatedOn"].ToString());
                        JReturnitem.Add("CreatedByUser", dt.Rows[0]["CreatedByUser"].ToString());
                        JReturnitem.Add("ModifiedOn", dt.Rows[0]["ModifiedOn"].ToString());
                        JReturnitem.Add("ModifiedByUser", dt.Rows[0]["ModifiedByUser"].ToString());
                        JReturnitem.Add("DeletedOn", dt.Rows[0]["DeletedOn"].ToString());
                        JReturnitem.Add("DeletedByUser", dt.Rows[0]["DeletedByUser"].ToString());
                        JReturnitem.Add("Name", dt.Rows[0]["Name"].ToString());
                        JReturnitem.Add("ParentCategoryId", dt.Rows[0]["ParentCategoryId"].ToString());
                        JReturnitem.Add("DisplayOrder", dt.Rows[0]["DisplayOrder"].ToString());
                        JReturnitem.Add("Macro", dt.Rows[0]["Macro"].ToString());
                        JReturnitem.Add("CategoryTreeId", dt.Rows[0]["CategoryTreeId"].ToString());
                        string js = JsonConvert.SerializeObject(JReturnitem);
                        return js;
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }
        }

        [HttpGet]
        public string GetContentDetails(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string pageId = Info.pageId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetContentDetails";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PageId", pageId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JObject JReturnitem = new JObject();
                        JReturnitem.Add("Id", dt.Rows[0]["Id"].ToString());
                        JReturnitem.Add("Name", dt.Rows[0]["Name"].ToString());
                        JReturnitem.Add("IsDeleted", dt.Rows[0]["IsDeleted"].ToString());
                        JReturnitem.Add("OriginalId", dt.Rows[0]["OriginalId"].ToString());
                        JReturnitem.Add("Status", dt.Rows[0]["Status"].ToString());
                        JReturnitem.Add("Version", dt.Rows[0]["Version"].ToString());
                        JReturnitem.Add("PreviewUrl", dt.Rows[0]["PreviewUrl"].ToString());
                        JReturnitem.Add("CreatedByUser", dt.Rows[0]["CreatedByUser"].ToString());
                        JReturnitem.Add("CreatedOn", dt.Rows[0]["CreatedOn"].ToString());
                        JReturnitem.Add("DeletedByUser", dt.Rows[0]["DeletedByUser"].ToString());
                        JReturnitem.Add("DeletedOn", dt.Rows[0]["DeletedOn"].ToString());
                        JReturnitem.Add("ModifiedByUser", dt.Rows[0]["ModifiedByUser"].ToString());
                        JReturnitem.Add("ModifiedOn", dt.Rows[0]["ModifiedOn"].ToString());
                        JReturnitem.Add("PublishedOn", dt.Rows[0]["PublishedOn"].ToString());
                        JReturnitem.Add("PublishedByUser", dt.Rows[0]["PublishedByUser"].ToString());
                        JReturnitem.Add("ActivationDate", dt.Rows[0]["ActivationDate"].ToString());
                        JReturnitem.Add("ExpirationDate", dt.Rows[0]["ExpirationDate"].ToString());
                        JReturnitem.Add("CustomCss", dt.Rows[0]["CustomCss"].ToString());
                        JReturnitem.Add("UseCustomCss", dt.Rows[0]["UseCustomCss"].ToString());
                        JReturnitem.Add("CustomJs", dt.Rows[0]["CustomJs"].ToString());
                        JReturnitem.Add("UseCustomJs", dt.Rows[0]["UseCustomJs"].ToString());
                        JReturnitem.Add("Html", dt.Rows[0]["Html"].ToString());
                        JReturnitem.Add("EditInSourceMode", dt.Rows[0]["EditInSourceMode"].ToString());
                        JReturnitem.Add("OriginalText", dt.Rows[0]["OriginalText"].ToString());
                        JReturnitem.Add("ContentTextMode", dt.Rows[0]["ContentTextMode"].ToString());
                        string js = JsonConvert.SerializeObject(JReturnitem);
                        return js;
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }
        }
        [HttpGet]
        public string GetPageContentDetails(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string pageId = Info.pageId;
            string contentId = Info.contentId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetPageContentDetails";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PageId", pageId);
                        cmd.Parameters.AddWithValue("@ContentId", contentId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);

                        JObject JReturnitem = new JObject();
                        JReturnitem.Add("PageContentId", dt.Rows[0]["Id"].ToString());
                        JReturnitem.Add("Version", dt.Rows[0]["Version"].ToString());
                        JReturnitem.Add("IsDeleted", dt.Rows[0]["IsDeleted"].ToString());
                        JReturnitem.Add("CreatedOn", dt.Rows[0]["CreatedOn"].ToString());
                        JReturnitem.Add("CreatedByUser", dt.Rows[0]["CreatedByUser"].ToString());
                        JReturnitem.Add("ModifiedByUser", dt.Rows[0]["ModifiedByUser"].ToString());
                        JReturnitem.Add("ModifiedOn", dt.Rows[0]["ModifiedOn"].ToString());
                        JReturnitem.Add("DeletedOn", dt.Rows[0]["DeletedOn"].ToString());
                        JReturnitem.Add("DeletedByUser", dt.Rows[0]["DeletedByUser"].ToString());
                        JReturnitem.Add("RegionId", dt.Rows[0]["RegionId"].ToString());
                        JReturnitem.Add("Order", dt.Rows[0]["Order"].ToString());
                        string js = JsonConvert.SerializeObject(JReturnitem);
                        return js;
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }
        }

        [HttpGet]
        public JArray GetPageContentDetailswithContentId(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string contentId = Info.contentId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetPageContentDetailswithContentId";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ContentId", contentId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JArray jarrayObj = new JArray();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            JObject NewJObject = new JObject();
                            NewJObject.Add("PageContentId", dt.Rows[i]["Id"].ToString());
                            NewJObject.Add("Version", dt.Rows[i]["Version"].ToString());
                            NewJObject.Add("IsDeleted", dt.Rows[i]["IsDeleted"].ToString());
                            NewJObject.Add("CreatedOn", dt.Rows[i]["CreatedOn"].ToString());
                            NewJObject.Add("CreatedByUser", dt.Rows[i]["CreatedByUser"].ToString());
                            NewJObject.Add("ModifiedOn", dt.Rows[i]["ModifiedOn"].ToString());
                            NewJObject.Add("ModifiedByUser", dt.Rows[i]["ModifiedByUser"].ToString());
                            NewJObject.Add("DeletedOn", dt.Rows[i]["DeletedOn"].ToString());
                            NewJObject.Add("DeletedByUser", dt.Rows[i]["DeletedByUser"].ToString());
                            NewJObject.Add("PageId", dt.Rows[i]["PageId"].ToString());
                           
                            NewJObject.Add("RegionId", dt.Rows[i]["RegionId"].ToString());
                            NewJObject.Add("Order", dt.Rows[i]["Order"].ToString());
                            NewJObject.Add("ParentPageContentId", dt.Rows[i]["ParentPageContentId"].ToString());
                            jarrayObj.Add(NewJObject);
                        }

                        return jarrayObj;

                    }
                    catch (Exception ex)
                    {
                        return new JArray();
                    }

                }
            }
        }

        [HttpGet]
        public string GetRegionDetails(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string regionId = Info.regionId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetRegionDetails";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@RegionId", regionId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JObject JReturnitem = new JObject();
                        JReturnitem.Add("Version", dt.Rows[0]["Version"].ToString());
                        JReturnitem.Add("IsDeleted", dt.Rows[0]["IsDeleted"].ToString());
                        JReturnitem.Add("CreatedOn", dt.Rows[0]["CreatedOn"].ToString());
                        JReturnitem.Add("CreatedByUser", dt.Rows[0]["CreatedByUser"].ToString());
                        JReturnitem.Add("ModifiedOn", dt.Rows[0]["ModifiedOn"].ToString());
                        JReturnitem.Add("ModifiedByUser", dt.Rows[0]["ModifiedByUser"].ToString());
                        JReturnitem.Add("DeletedOn", dt.Rows[0]["DeletedOn"].ToString());
                        JReturnitem.Add("DeletedByUser", dt.Rows[0]["DeletedByUser"].ToString());
                        JReturnitem.Add("RegionIdentifier", dt.Rows[0]["RegionIdentifier"].ToString());


                        string js = JsonConvert.SerializeObject(JReturnitem);
                        return js;
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }

                }
            }
        }
        [HttpGet]
        public JArray GetLayoutRegionDetails(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string regionId = Info.regionId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetLayoutRegionDetails";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@RegionId", regionId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);

                        JArray jarrayObj = new JArray();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            JObject NewJObject = new JObject();
                            NewJObject.Add("LayoutRegionId", dt.Rows[i]["Id"].ToString());
                            NewJObject.Add("Version", dt.Rows[i]["Version"].ToString());
                            NewJObject.Add("IsDeleted", dt.Rows[i]["IsDeleted"].ToString());
                            NewJObject.Add("CreatedOn", dt.Rows[i]["CreatedOn"].ToString());
                            NewJObject.Add("CreatedByUser", dt.Rows[i]["CreatedByUser"].ToString());
                            NewJObject.Add("ModifiedOn", dt.Rows[i]["ModifiedOn"].ToString());
                            NewJObject.Add("ModifiedByUser", dt.Rows[i]["ModifiedByUser"].ToString());
                            NewJObject.Add("DeletedOn", dt.Rows[i]["DeletedOn"].ToString());
                            NewJObject.Add("DeletedByUser", dt.Rows[i]["DeletedByUser"].ToString());
                            NewJObject.Add("Description", dt.Rows[i]["Description"].ToString());
                            NewJObject.Add("LayoutId", dt.Rows[i]["LayoutId"].ToString());

                            jarrayObj.Add(NewJObject);
                        }

                        return jarrayObj;
                    }
                    catch (Exception ex)
                    {
                        return new JArray();
                    }
                }
            }

        }
        [HttpGet]
        public string GetLayoutDetails(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string layoutId = Info.layoutId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetLayoutDetails";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@LayoutId", layoutId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);

                        JObject JReturnitem = new JObject();
                        JReturnitem.Add("Version", dt.Rows[0]["Version"].ToString());
                        JReturnitem.Add("IsDeleted", dt.Rows[0]["IsDeleted"].ToString());
                        JReturnitem.Add("CreatedOn", dt.Rows[0]["CreatedOn"].ToString());
                        JReturnitem.Add("CreatedByUser", dt.Rows[0]["CreatedByUser"].ToString());
                        JReturnitem.Add("ModifiedOn", dt.Rows[0]["ModifiedOn"].ToString());
                        JReturnitem.Add("ModifiedByUser", dt.Rows[0]["ModifiedByUser"].ToString());
                        JReturnitem.Add("DeletedOn", dt.Rows[0]["DeletedOn"].ToString());
                        JReturnitem.Add("DeletedByUser", dt.Rows[0]["DeletedByUser"].ToString());
                        JReturnitem.Add("Name", dt.Rows[0]["Name"].ToString());
                        JReturnitem.Add("LayoutPath", dt.Rows[0]["LayoutPath"].ToString());
                        //JReturnitem.Add("ModuleId", dt.Rows[0]["ModuleId"].ToString());
                        JReturnitem.Add("PreviewUrl", dt.Rows[0]["PreviewUrl"].ToString());

                        string js = JsonConvert.SerializeObject(JReturnitem);
                        return js;



                    }
                    catch (Exception ex)
                    {
                        return "";
                    }

                }
            }
        }
        [HttpGet]
        public JArray GetRegionPageContentDetails(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string regionId = Info.regionId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spBlogRegionPageContentDetails";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@RegionId", regionId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JArray jarrayObj = new JArray();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            JObject NewJObject = new JObject();
                            NewJObject.Add("PageContentId", dt.Rows[i]["Id"].ToString());
                            NewJObject.Add("Version", dt.Rows[i]["Version"].ToString());
                            NewJObject.Add("IsDeleted", dt.Rows[i]["IsDeleted"].ToString());
                            NewJObject.Add("CreatedOn", dt.Rows[i]["CreatedOn"].ToString());
                            NewJObject.Add("CreatedByUser", dt.Rows[i]["CreatedByUser"].ToString());
                            NewJObject.Add("ModifiedOn", dt.Rows[i]["ModifiedOn"].ToString());
                            NewJObject.Add("ModifiedByUser", dt.Rows[i]["ModifiedByUser"].ToString());
                            NewJObject.Add("DeletedOn", dt.Rows[i]["DeletedOn"].ToString());
                            NewJObject.Add("DeletedByUser", dt.Rows[i]["DeletedByUser"].ToString());
                            NewJObject.Add("PageId", dt.Rows[i]["PageId"].ToString());
                            NewJObject.Add("ContentId", dt.Rows[i]["ContentId"].ToString());
                            NewJObject.Add("Order", dt.Rows[i]["Order"].ToString());

                            jarrayObj.Add(NewJObject);
                        }

                        return jarrayObj;

                    }
                    catch (Exception ex)
                    {
                        return new JArray();
                    }
                }
            }
        }

        [HttpGet]
        public JArray GetPageContentDetailsWithPageId(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string pageId = Info.pageId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spBlogPageContentDetailsWithPageId";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PageId", pageId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JArray jarrayObj = new JArray();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            JObject NewJObject = new JObject();
                            NewJObject.Add("PageContentId", dt.Rows[i]["Id"].ToString());
                            NewJObject.Add("Version", dt.Rows[i]["Version"].ToString());
                            NewJObject.Add("IsDeleted", dt.Rows[i]["IsDeleted"].ToString());
                            NewJObject.Add("CreatedOn", dt.Rows[i]["CreatedOn"].ToString());
                            NewJObject.Add("CreatedByUser", dt.Rows[i]["CreatedByUser"].ToString());
                            NewJObject.Add("ModifiedOn", dt.Rows[i]["ModifiedOn"].ToString());
                            NewJObject.Add("ModifiedByUser", dt.Rows[i]["ModifiedByUser"].ToString());
                            NewJObject.Add("DeletedOn", dt.Rows[i]["DeletedOn"].ToString());
                            NewJObject.Add("DeletedByUser", dt.Rows[i]["DeletedByUser"].ToString());
                            NewJObject.Add("ContentId", dt.Rows[i]["ContentId"].ToString());
                            NewJObject.Add("RegionId", dt.Rows[i]["RegionId"].ToString());
                            NewJObject.Add("Order", dt.Rows[i]["Order"].ToString());

                            jarrayObj.Add(NewJObject);
                        }

                        return jarrayObj;

                    }
                    catch (Exception ex)
                    {
                        return new JArray();
                    }

                }
            }
        }


        [HttpGet]
        public JArray GetCategorizableItem()
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetBlogCategorizableItem";
                        cmd.Connection = con;

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JArray jarrayObj = new JArray();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            JObject NewJObject = new JObject();
                            NewJObject.Add("ItemId", dt.Rows[i]["Id"].ToString());
                            NewJObject.Add("Version", dt.Rows[i]["Version"].ToString());
                            NewJObject.Add("IsDeleted", dt.Rows[i]["IsDeleted"].ToString());
                            NewJObject.Add("CreatedOn", dt.Rows[i]["CreatedOn"].ToString());
                            NewJObject.Add("CreatedByUser", dt.Rows[i]["CreatedByUser"].ToString());
                            NewJObject.Add("ModifiedOn", dt.Rows[i]["ModifiedOn"].ToString());
                            NewJObject.Add("ModifiedByUser", dt.Rows[i]["ModifiedByUser"].ToString());
                            NewJObject.Add("DeletedOn", dt.Rows[i]["DeletedOn"].ToString());
                            NewJObject.Add("DeletedByUser", dt.Rows[i]["DeletedByUser"].ToString());
                            NewJObject.Add("Name", dt.Rows[i]["Name"].ToString());
                            jarrayObj.Add(NewJObject);
                        }
                        return jarrayObj;
                    }
                    catch (Exception ex)
                    {
                        return new JArray();
                    }
                }
            }
        }

        [HttpGet]
        public string GetRegionId(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string regionIdentifier = Info.regionIdentifier;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetBlogRegionId";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@RegionIdentifier", regionIdentifier);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JObject JReturnitem = new JObject();
                        JReturnitem.Add("Id", dt.Rows[0]["Id"].ToString());
                        string js = JsonConvert.SerializeObject(JReturnitem);
                        return js;


                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }
        }

        [HttpGet]
        public string GetLayoutId(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string regionId = Info.regionId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetBlogLayoutId";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@RegionId", regionId);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JObject JReturnitem = new JObject();
                        JReturnitem.Add("layoutId", dt.Rows[0]["LayoutId"].ToString());
                        string js = JsonConvert.SerializeObject(JReturnitem);
                        return js;
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }
        }
        [HttpGet]
        public JArray GetBlogExistingCategoryIds()
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spBlogExistingCategoryIds";
                        cmd.Connection = con;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JArray jarrayObj = new JArray();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            JObject NewJObject = new JObject();
                            NewJObject.Add("Id", dt.Rows[i]["Id"].ToString());
                            jarrayObj.Add(NewJObject);
                        }
                        return jarrayObj;
                    }
                    catch (Exception ex)
                    {
                        return new JArray();
                    }
                }
            }
        }
        [HttpGet]
        public JArray GetLayoutRegionDetailsForLayout(String Js)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(Js);
            string layoutId = Info.layoutId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spBlogLayoutRegionDetailsForLayout";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@LayoutId", layoutId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);

                        JArray jarrayObj = new JArray();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            JObject NewJObject = new JObject();
                            NewJObject.Add("LayoutRegionId", dt.Rows[i]["Id"].ToString());
                            NewJObject.Add("Version", dt.Rows[i]["Version"].ToString());
                            NewJObject.Add("IsDeleted", dt.Rows[i]["IsDeleted"].ToString());
                            NewJObject.Add("CreatedOn", dt.Rows[i]["CreatedOn"].ToString());
                            NewJObject.Add("CreatedByUser", dt.Rows[i]["CreatedByUser"].ToString());
                            NewJObject.Add("ModifiedOn", dt.Rows[i]["ModifiedOn"].ToString());
                            NewJObject.Add("ModifiedByUser", dt.Rows[i]["ModifiedByUser"].ToString());
                            NewJObject.Add("DeletedOn", dt.Rows[i]["DeletedOn"].ToString());
                            NewJObject.Add("DeletedByUser", dt.Rows[i]["DeletedByUser"].ToString());
                            NewJObject.Add("Description", dt.Rows[i]["Description"].ToString());
                            NewJObject.Add("RegionId", dt.Rows[i]["RegionId"].ToString());

                            jarrayObj.Add(NewJObject);
                        }

                        return jarrayObj;
                    }
                    catch (Exception ex)
                    {
                        return new JArray();
                    }
                }
            }
        }
        [HttpGet]
        public JArray GetPageIds(String Js)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(Js);
            string layoutId = Info.layoutId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetPagesIds";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@LayoutId", layoutId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JArray jarrayObj = new JArray();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            JObject NewJObject = new JObject();
                            NewJObject.Add("PageId", dt.Rows[i]["Id"].ToString());
                            jarrayObj.Add(NewJObject);
                        }
                        return jarrayObj;
                    }
                    catch (Exception ex)
                    {
                        return new JArray();
                    }
                }
            }

        }
        [HttpGet]
        public JArray GetBlogAccessRulesId(String Js)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(Js);
            string pageId = Info.pageId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetAccessRuleIds";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PageId", pageId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            JArray jarrayObj = new JArray();
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                JObject NewJObject = new JObject();
                                NewJObject.Add("AccessruleId", dt.Rows[i]["AccessRuleId"].ToString());
                                jarrayObj.Add(NewJObject);
                            }
                            return jarrayObj;

                        }
                        else
                        {
                            return new JArray();
                        }


                    }
                    catch (Exception ex)
                    {
                        return new JArray();
                    }
                }
            }
        }
        [HttpGet]
        public string GetAccessRules(String Js)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(Js);
            string accessruleId = Info.accessrulesId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetAccessRules";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@AccessRuleId", accessruleId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JObject JReturnitem = new JObject();
                        JReturnitem.Add("Version", dt.Rows[0]["Version"].ToString());
                        JReturnitem.Add("IsDeleted", dt.Rows[0]["IsDeleted"].ToString());
                        JReturnitem.Add("CreatedOn", dt.Rows[0]["CreatedOn"].ToString());
                        JReturnitem.Add("CreatedByUser", dt.Rows[0]["CreatedByUser"].ToString());
                        JReturnitem.Add("ModifiedOn", dt.Rows[0]["ModifiedOn"].ToString());
                        JReturnitem.Add("ModifiedByUser", dt.Rows[0]["ModifiedByUser"].ToString());
                        JReturnitem.Add("DeletedOn", dt.Rows[0]["DeletedOn"].ToString());
                        JReturnitem.Add("DeletedByUser", dt.Rows[0]["DeletedByUser"].ToString());
                        JReturnitem.Add("Identity", dt.Rows[0]["Identity"].ToString());
                        JReturnitem.Add("AccessLevel", dt.Rows[0]["AccessLevel"].ToString());
                        JReturnitem.Add("IsForRole", dt.Rows[0]["IsForRole"].ToString());
                        string js = JsonConvert.SerializeObject(JReturnitem);
                        return js;


                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }

        }


        [HttpGet]
        public string GetPagesForLayout(String Js)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(Js);
            string pageId = Info.pageId;

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetPagesForLayout";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PageId", pageId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataSet dt = new DataSet();
                        ad.Fill(dt);

                        JObject JReturnitem = new JObject();
                        int flag = Convert.ToInt32(dt.Tables[0].Rows[0]["flag"]);
                        if (flag == 1)
                        {


                            JReturnitem.Add("Flag", dt.Tables[0].Rows[0]["flag"].ToString());

                            JReturnitem.Add("ActivationDate", dt.Tables[1].Rows[0]["ActivationDate"].ToString());
                            JReturnitem.Add("ExpirationDate", dt.Tables[1].Rows[0]["ExpirationDate"].ToString());
                            JReturnitem.Add("Description", dt.Tables[1].Rows[0]["Description"].ToString());
                            JReturnitem.Add("ImageId", dt.Tables[1].Rows[0]["ImageId"].ToString());
                            JReturnitem.Add("CustomCss", dt.Tables[1].Rows[0]["CustomCss"].ToString());

                            JReturnitem.Add("CustomJS", dt.Tables[1].Rows[0]["CustomJS"].ToString());
                            JReturnitem.Add("UseCanonicalUrl", dt.Tables[1].Rows[0]["UseCanonicalUrl"].ToString());
                            JReturnitem.Add("UseNoFollow", dt.Tables[1].Rows[0]["UseNoFollow"].ToString());
                            JReturnitem.Add("UseNoIndex", dt.Tables[1].Rows[0]["UseNoIndex"].ToString());
                            JReturnitem.Add("CategoryId", dt.Tables[1].Rows[0]["CategoryId"].ToString());

                            JReturnitem.Add("SecondaryImageId", dt.Tables[1].Rows[0]["SecondaryImageId"].ToString());
                            JReturnitem.Add("FeaturedImageId", dt.Tables[1].Rows[0]["FeaturedImageId"].ToString());
                            JReturnitem.Add("IsArchived", dt.Tables[1].Rows[0]["IsArchived"].ToString());
                            JReturnitem.Add("Version", dt.Tables[1].Rows[0]["Version"].ToString());
                            JReturnitem.Add("IsDeleted", dt.Tables[1].Rows[0]["IsDeleted"].ToString());
                            JReturnitem.Add("CreatedOn", dt.Tables[1].Rows[0]["CreatedOn"].ToString());
                            JReturnitem.Add("CreatedByUser", dt.Tables[1].Rows[0]["CreatedByUser"].ToString());
                            JReturnitem.Add("ModifiedOn", dt.Tables[1].Rows[0]["ModifiedOn"].ToString());
                            JReturnitem.Add("ModifiedByUser", dt.Tables[1].Rows[0]["ModifiedByUser"].ToString());
                            JReturnitem.Add("DeletedOn", dt.Tables[1].Rows[0]["DeletedOn"].ToString());
                            JReturnitem.Add("DeletedByUser", dt.Tables[1].Rows[0]["DeletedByUser"].ToString());
                            JReturnitem.Add("PageUrl", dt.Tables[1].Rows[0]["PageUrl"].ToString());
                            JReturnitem.Add("Title", dt.Tables[1].Rows[0]["Title"].ToString());
                            JReturnitem.Add("LayoutId", dt.Tables[1].Rows[0]["LayoutId"].ToString());
                            JReturnitem.Add("MetaTitle", dt.Tables[1].Rows[0]["MetaTitle"].ToString());
                            JReturnitem.Add("MetaKeywords", dt.Tables[1].Rows[0]["MetaKeywords"].ToString());
                            JReturnitem.Add("MetaDescription", dt.Tables[1].Rows[0]["MetaDescription"].ToString());

                            JReturnitem.Add("Status", dt.Tables[1].Rows[0]["Status"].ToString());
                            JReturnitem.Add("PageUrlHash", dt.Tables[1].Rows[0]["PageUrlHash"].ToString());
                            JReturnitem.Add("MasterPageId", dt.Tables[1].Rows[0]["MasterPageId"].ToString());
                            JReturnitem.Add("IsMasterPage", dt.Tables[1].Rows[0]["IsMasterPage"].ToString());
                            JReturnitem.Add("LanguageId", dt.Tables[1].Rows[0]["LanguageId"].ToString());
                            JReturnitem.Add("LanguageGroupIdentifier", dt.Tables[1].Rows[0]["LanguageGroupIdentifier"].ToString());
                            JReturnitem.Add("ForceAccessProtocol", dt.Tables[1].Rows[0]["ForceAccessProtocol"].ToString());

                            string js = JsonConvert.SerializeObject(JReturnitem);
                            return js;

                        }
                        else if (flag == 2)
                        {
                            JReturnitem.Add("Flag", dt.Tables[0].Rows[0]["flag"].ToString());

                            JReturnitem.Add("Description", dt.Tables[1].Rows[0]["Description"].ToString());
                            JReturnitem.Add("ImageId", dt.Tables[1].Rows[0]["ImageId"].ToString());
                            JReturnitem.Add("CustomCss", dt.Tables[1].Rows[0]["CustomCss"].ToString());

                            JReturnitem.Add("CustomJS", dt.Tables[1].Rows[0]["CustomJS"].ToString());
                            JReturnitem.Add("UseCanonicalUrl", dt.Tables[1].Rows[0]["UseCanonicalUrl"].ToString());
                            JReturnitem.Add("UseNoFollow", dt.Tables[1].Rows[0]["UseNoFollow"].ToString());
                            JReturnitem.Add("UseNoIndex", dt.Tables[1].Rows[0]["UseNoIndex"].ToString());
                            JReturnitem.Add("CategoryId", dt.Tables[1].Rows[0]["CategoryId"].ToString());

                            JReturnitem.Add("SecondaryImageId", dt.Tables[1].Rows[0]["SecondaryImageId"].ToString());
                            JReturnitem.Add("FeaturedImageId", dt.Tables[1].Rows[0]["FeaturedImageId"].ToString());
                            JReturnitem.Add("IsArchived", dt.Tables[1].Rows[0]["IsArchived"].ToString());
                            JReturnitem.Add("Version", dt.Tables[1].Rows[0]["Version"].ToString());
                            JReturnitem.Add("IsDeleted", dt.Tables[1].Rows[0]["IsDeleted"].ToString());
                            JReturnitem.Add("CreatedOn", dt.Tables[1].Rows[0]["CreatedOn"].ToString());
                            JReturnitem.Add("CreatedByUser", dt.Tables[1].Rows[0]["CreatedByUser"].ToString());
                            JReturnitem.Add("ModifiedOn", dt.Tables[1].Rows[0]["ModifiedOn"].ToString());
                            JReturnitem.Add("ModifiedByUser", dt.Tables[1].Rows[0]["ModifiedByUser"].ToString());
                            JReturnitem.Add("DeletedOn", dt.Tables[1].Rows[0]["DeletedOn"].ToString());
                            JReturnitem.Add("DeletedByUser", dt.Tables[1].Rows[0]["DeletedByUser"].ToString());

                            JReturnitem.Add("PageUrl", dt.Tables[1].Rows[0]["PageUrl"].ToString());
                            JReturnitem.Add("Title", dt.Tables[1].Rows[0]["Title"].ToString());
                            JReturnitem.Add("LayoutId", dt.Tables[1].Rows[0]["LayoutId"].ToString());
                            JReturnitem.Add("MetaTitle", dt.Tables[1].Rows[0]["MetaTitle"].ToString());
                            JReturnitem.Add("MetaKeywords", dt.Tables[1].Rows[0]["MetaKeywords"].ToString());
                            JReturnitem.Add("MetaDescription", dt.Tables[1].Rows[0]["MetaDescription"].ToString());

                            JReturnitem.Add("Status", dt.Tables[1].Rows[0]["Status"].ToString());
                            JReturnitem.Add("PageUrlHash", dt.Tables[1].Rows[0]["PageUrlHash"].ToString());
                            JReturnitem.Add("MasterPageId", dt.Tables[1].Rows[0]["MasterPageId"].ToString());
                            JReturnitem.Add("IsMasterPage", dt.Tables[1].Rows[0]["IsMasterPage"].ToString());
                            JReturnitem.Add("LanguageId", dt.Tables[1].Rows[0]["LanguageId"].ToString());
                            JReturnitem.Add("LanguageGroupIdentifier", dt.Tables[1].Rows[0]["LanguageGroupIdentifier"].ToString());
                            JReturnitem.Add("ForceAccessProtocol", dt.Tables[1].Rows[0]["ForceAccessProtocol"].ToString());
                            string js = JsonConvert.SerializeObject(JReturnitem);
                            return js;
                        }
                        else
                        {
                            JReturnitem.Add("Flag", dt.Tables[0].Rows[0]["flag"].ToString());

                            JReturnitem.Add("Version", dt.Tables[1].Rows[0]["Version"].ToString());
                            JReturnitem.Add("IsDeleted", dt.Tables[1].Rows[0]["IsDeleted"].ToString());
                            JReturnitem.Add("CreatedOn", dt.Tables[1].Rows[0]["CreatedOn"].ToString());
                            JReturnitem.Add("CreatedByUser", dt.Tables[1].Rows[0]["CreatedByUser"].ToString());
                            JReturnitem.Add("ModifiedOn", dt.Tables[1].Rows[0]["ModifiedOn"].ToString());
                            JReturnitem.Add("ModifiedByUser", dt.Tables[1].Rows[0]["ModifiedByUser"].ToString());
                            JReturnitem.Add("DeletedOn", dt.Tables[1].Rows[0]["DeletedOn"].ToString());
                            JReturnitem.Add("DeletedByUser", dt.Tables[1].Rows[0]["DeletedByUser"].ToString());

                            JReturnitem.Add("PageUrl", dt.Tables[1].Rows[0]["PageUrl"].ToString());
                            JReturnitem.Add("Title", dt.Tables[1].Rows[0]["Title"].ToString());
                            JReturnitem.Add("LayoutId", dt.Tables[1].Rows[0]["LayoutId"].ToString());
                            JReturnitem.Add("MetaTitle", dt.Tables[1].Rows[0]["MetaTitle"].ToString());
                            JReturnitem.Add("MetaKeywords", dt.Tables[1].Rows[0]["MetaKeywords"].ToString());
                            JReturnitem.Add("MetaDescription", dt.Tables[1].Rows[0]["MetaDescription"].ToString());

                            JReturnitem.Add("Status", dt.Tables[1].Rows[0]["Status"].ToString());
                            JReturnitem.Add("PageUrlHash", dt.Tables[1].Rows[0]["PageUrlHash"].ToString());
                            JReturnitem.Add("MasterPageId", dt.Tables[1].Rows[0]["MasterPageId"].ToString());
                            JReturnitem.Add("IsMasterPage", dt.Tables[1].Rows[0]["IsMasterPage"].ToString());
                            JReturnitem.Add("LanguageId", dt.Tables[1].Rows[0]["LanguageId"].ToString());
                            JReturnitem.Add("LanguageGroupIdentifier", dt.Tables[1].Rows[0]["LanguageGroupIdentifier"].ToString());
                            JReturnitem.Add("ForceAccessProtocol", dt.Tables[1].Rows[0]["ForceAccessProtocol"].ToString());
                            string js = JsonConvert.SerializeObject(JReturnitem);
                            return js;
                        }

                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }

        }

        [HttpGet]
        public string GetImageDetails(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string imageId = Info.imageId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetImageDetails";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ImageId", imageId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);

                        JObject JReturnitem = new JObject();
                        JReturnitem.Add("Caption", dt.Rows[0]["Caption"].ToString());
                        JReturnitem.Add("ImageAlign", dt.Rows[0]["ImageAlign"].ToString());
                        JReturnitem.Add("Width", dt.Rows[0]["Width"].ToString());
                        JReturnitem.Add("Height", dt.Rows[0]["Height"].ToString());
                        JReturnitem.Add("CropCoordX1", dt.Rows[0]["CropCoordX1"].ToString());
                        JReturnitem.Add("CropCoordX2", dt.Rows[0]["CropCoordX2"].ToString());
                        JReturnitem.Add("CropCoordY1", dt.Rows[0]["CropCoordY1"].ToString());
                        JReturnitem.Add("CropCoordY2", dt.Rows[0]["CropCoordY2"].ToString());
                        JReturnitem.Add("OriginalWidth", dt.Rows[0]["OriginalWidth"].ToString());
                        JReturnitem.Add("OriginalHeight", dt.Rows[0]["OriginalHeight"].ToString());
                        JReturnitem.Add("OriginalSize", dt.Rows[0]["OriginalSize"].ToString());
                        JReturnitem.Add("OriginalUri", dt.Rows[0]["OriginalUri"].ToString());
                        JReturnitem.Add("PublicOriginallUrl", dt.Rows[0]["PublicOriginallUrl"].ToString());
                        JReturnitem.Add("IsOriginalUploaded", dt.Rows[0]["IsOriginalUploaded"].ToString());
                        JReturnitem.Add("ThumbnailWidth", dt.Rows[0]["ThumbnailWidth"].ToString());
                        JReturnitem.Add("ThumbnailHeight", dt.Rows[0]["ThumbnailHeight"].ToString());
                        JReturnitem.Add("ThumbnailSize", dt.Rows[0]["ThumbnailSize"].ToString());
                        JReturnitem.Add("ThumbnailUri", dt.Rows[0]["ThumbnailUri"].ToString());
                        JReturnitem.Add("PublicThumbnailUrl", dt.Rows[0]["PublicThumbnailUrl"].ToString());
                        JReturnitem.Add("IsThumbnailUploaded", dt.Rows[0]["IsThumbnailUploaded"].ToString());
                        JReturnitem.Add("Version", dt.Rows[0]["Version"].ToString());
                        JReturnitem.Add("IsDeleted", dt.Rows[0]["IsDeleted"].ToString());
                        JReturnitem.Add("CreatedOn", dt.Rows[0]["CreatedOn"].ToString());
                        JReturnitem.Add("CreatedByUser", dt.Rows[0]["CreatedByUser"].ToString());
                        JReturnitem.Add("ModifiedOn", dt.Rows[0]["ModifiedOn"].ToString());
                        JReturnitem.Add("ModifiedByUser", dt.Rows[0]["ModifiedByUser"].ToString());
                        JReturnitem.Add("DeletedOn", dt.Rows[0]["DeletedOn"].ToString());
                        JReturnitem.Add("DeletedByUser", dt.Rows[0]["DeletedByUser"].ToString());
                        JReturnitem.Add("FolderId", dt.Rows[0]["FolderId"].ToString());
                        JReturnitem.Add("Title", dt.Rows[0]["Title"].ToString());
                        JReturnitem.Add("Type", dt.Rows[0]["Type"].ToString());
                        JReturnitem.Add("ContentType", dt.Rows[0]["ContentType"].ToString());
                        JReturnitem.Add("IsArchived", dt.Rows[0]["IsArchived"].ToString());
                        JReturnitem.Add("OriginalId", dt.Rows[0]["OriginalId"].ToString());
                        JReturnitem.Add("PublishedOn", dt.Rows[0]["PublishedOn"].ToString());
                        JReturnitem.Add("ImageId", dt.Rows[0]["ImageId"].ToString());
                        JReturnitem.Add("Description", dt.Rows[0]["Description"].ToString());

                        string js = JsonConvert.SerializeObject(JReturnitem);
                        return js;
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }
        }

        [HttpGet]
        public string GetLanguageDetails(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string languageId = Info.languageId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetLanguageDetails";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@LanguageId", languageId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JObject JReturnitem = new JObject();
                        JReturnitem.Add("Version", dt.Rows[0]["Version"].ToString());
                        JReturnitem.Add("IsDeleted", dt.Rows[0]["IsDeleted"].ToString());
                        JReturnitem.Add("CreatedOn", dt.Rows[0]["CreatedOn"].ToString());
                        JReturnitem.Add("CreatedByUser", dt.Rows[0]["CreatedByUser"].ToString());
                        JReturnitem.Add("ModifiedOn", dt.Rows[0]["ModifiedOn"].ToString());
                        JReturnitem.Add("ModifiedByUser", dt.Rows[0]["ModifiedByUser"].ToString());
                        JReturnitem.Add("DeletedOn", dt.Rows[0]["DeletedOn"].ToString());
                        JReturnitem.Add("DeletedByUser", dt.Rows[0]["DeletedByUser"].ToString());
                        JReturnitem.Add("Name", dt.Rows[0]["Name"].ToString());
                        JReturnitem.Add("Code", dt.Rows[0]["Code"].ToString());

                        string js = JsonConvert.SerializeObject(JReturnitem);
                        return js;
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }
        }


        [HttpGet]
        public string GetContentDetailsForLayout(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string contentId = Info.contentId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetContentDetailsForLayout";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ContentId", contentId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JObject JReturnitem = new JObject();
                        JReturnitem.Add("Id", dt.Rows[0]["Id"].ToString());
                        JReturnitem.Add("Name", dt.Rows[0]["Name"].ToString());
                        JReturnitem.Add("IsDeleted", dt.Rows[0]["IsDeleted"].ToString());
                        JReturnitem.Add("OriginalId", dt.Rows[0]["OriginalId"].ToString());
                        JReturnitem.Add("Status", dt.Rows[0]["Status"].ToString());
                        JReturnitem.Add("Version", dt.Rows[0]["Version"].ToString());
                        JReturnitem.Add("PreviewUrl", dt.Rows[0]["PreviewUrl"].ToString());
                        JReturnitem.Add("CreatedByUser", dt.Rows[0]["CreatedByUser"].ToString());
                        JReturnitem.Add("CreatedOn", dt.Rows[0]["CreatedOn"].ToString());
                        JReturnitem.Add("DeletedByUser", dt.Rows[0]["DeletedByUser"].ToString());
                        JReturnitem.Add("DeletedOn", dt.Rows[0]["DeletedOn"].ToString());
                        JReturnitem.Add("ModifiedByUser", dt.Rows[0]["ModifiedByUser"].ToString());
                        JReturnitem.Add("ModifiedOn", dt.Rows[0]["ModifiedOn"].ToString());
                        JReturnitem.Add("PublishedOn", dt.Rows[0]["PublishedOn"].ToString());
                        JReturnitem.Add("PublishedByUser", dt.Rows[0]["PublishedByUser"].ToString());
                        //JReturnitem.Add("ActivationDate", dt.Rows[0]["ActivationDate"].ToString());
                        //JReturnitem.Add("ExpirationDate", dt.Rows[0]["ExpirationDate"].ToString());
                        //JReturnitem.Add("CustomCss", dt.Rows[0]["CustomCss"].ToString());
                        //JReturnitem.Add("UseCustomCss", dt.Rows[0]["UseCustomCss"].ToString());
                        //JReturnitem.Add("CustomJs", dt.Rows[0]["CustomJs"].ToString());
                        //JReturnitem.Add("UseCustomJs", dt.Rows[0]["UseCustomJs"].ToString());
                        //JReturnitem.Add("Html", dt.Rows[0]["Html"].ToString());
                        //JReturnitem.Add("EditInSourceMode", dt.Rows[0]["EditInSourceMode"].ToString());
                        //JReturnitem.Add("OriginalText", dt.Rows[0]["OriginalText"].ToString());
                        //JReturnitem.Add("ContentTextMode", dt.Rows[0]["ContentTextMode"].ToString());
                        string js = JsonConvert.SerializeObject(JReturnitem);
                        return js;
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }
        }

        [HttpGet]
        public string GetContentDetailsForPageContents(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string contentId = Info.contentId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetContentDetailsForPageContents";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ContentId", contentId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataSet dt = new DataSet();
                        ad.Fill(dt);
                        JObject JReturnitem = new JObject();
                        int flag = Convert.ToInt32(dt.Tables[0].Rows[0]["flag"]);
                        if (flag == 1)
                        {
                            JReturnitem.Add("Flag", dt.Tables[0].Rows[0]["flag"].ToString());
                            JReturnitem.Add("ContentId", contentId);
                            JReturnitem.Add("ActivationDate", dt.Tables[1].Rows[0]["ActivationDate"].ToString());
                            JReturnitem.Add("ExpirationDate", dt.Tables[1].Rows[0]["ExpirationDate"].ToString());
                            JReturnitem.Add("CustomCss", dt.Tables[1].Rows[0]["CustomCss"].ToString());
                            JReturnitem.Add("UseCustomCss", dt.Tables[1].Rows[0]["UseCustomCss"].ToString());
                            JReturnitem.Add("CustomJs", dt.Tables[1].Rows[0]["CustomJs"].ToString());
                            JReturnitem.Add("UseCustomJs", dt.Tables[1].Rows[0]["UseCustomJs"].ToString());
                            JReturnitem.Add("Html", dt.Tables[1].Rows[0]["Html"].ToString());
                            JReturnitem.Add("EditInSourceMode", dt.Tables[1].Rows[0]["EditInSourceMode"].ToString());
                            JReturnitem.Add("OriginalText", dt.Tables[1].Rows[0]["OriginalText"].ToString());
                            JReturnitem.Add("ContentTextMode", dt.Tables[1].Rows[0]["ContentTextMode"].ToString());
                            JReturnitem.Add("Version", dt.Tables[1].Rows[0]["Version"].ToString());
                            JReturnitem.Add("IsDeleted", dt.Tables[1].Rows[0]["IsDeleted"].ToString());
                            JReturnitem.Add("CreatedOn", dt.Tables[1].Rows[0]["CreatedOn"].ToString());
                            JReturnitem.Add("CreatedByUser", dt.Tables[1].Rows[0]["CreatedByUser"].ToString());
                            JReturnitem.Add("ModifiedOn", dt.Tables[1].Rows[0]["ModifiedOn"].ToString());
                            JReturnitem.Add("ModifiedByUser", dt.Tables[1].Rows[0]["ModifiedByUser"].ToString());
                            JReturnitem.Add("DeletedOn", dt.Tables[1].Rows[0]["DeletedOn"].ToString());
                            JReturnitem.Add("DeletedByUser", dt.Tables[1].Rows[0]["DeletedByUser"].ToString());
                            JReturnitem.Add("Name", dt.Tables[1].Rows[0]["Name"].ToString());
                            JReturnitem.Add("PreviewUrl", dt.Tables[1].Rows[0]["PreviewUrl"].ToString());
                            JReturnitem.Add("Status", dt.Tables[1].Rows[0]["Status"].ToString());
                            JReturnitem.Add("PublishedOn", dt.Tables[1].Rows[0]["PublishedOn"].ToString());
                            JReturnitem.Add("PublishedByUser", dt.Tables[1].Rows[0]["PublishedByUser"].ToString());
                            JReturnitem.Add("OriginalId", dt.Tables[1].Rows[0]["OriginalId"].ToString());
                            
                        }
                        else if (flag == 2)
                        {
                            JReturnitem.Add("Flag", dt.Tables[0].Rows[0]["flag"].ToString());
                            JReturnitem.Add("ContentId", contentId);
                            JReturnitem.Add("ActivationDate", dt.Tables[1].Rows[0]["ActivationDate"].ToString());
                            JReturnitem.Add("ExpirationDate", dt.Tables[1].Rows[0]["ExpirationDate"].ToString());
                            JReturnitem.Add("CustomCss", dt.Tables[1].Rows[0]["CustomCss"].ToString());
                            JReturnitem.Add("UseCustomCss", dt.Tables[1].Rows[0]["UseCustomCss"].ToString());
                            JReturnitem.Add("CustomJs", dt.Tables[1].Rows[0]["CustomJs"].ToString());
                            JReturnitem.Add("UseCustomJs", dt.Tables[1].Rows[0]["UseCustomJs"].ToString());
                            JReturnitem.Add("Html", dt.Tables[1].Rows[0]["Html"].ToString());
                            JReturnitem.Add("EditInSourceMode", dt.Tables[1].Rows[0]["EditInSourceMode"].ToString());
                            JReturnitem.Add("OriginalText", dt.Tables[1].Rows[0]["OriginalText"].ToString());
                            JReturnitem.Add("ContentTextMode", dt.Tables[1].Rows[0]["ContentTextMode"].ToString());
                            JReturnitem.Add("Version", dt.Tables[1].Rows[0]["Version"].ToString());
                            JReturnitem.Add("IsDeleted", dt.Tables[1].Rows[0]["IsDeleted"].ToString());
                            JReturnitem.Add("CreatedOn", dt.Tables[1].Rows[0]["CreatedOn"].ToString());
                            JReturnitem.Add("CreatedByUser", dt.Tables[1].Rows[0]["CreatedByUser"].ToString());
                            JReturnitem.Add("ModifiedOn", dt.Tables[1].Rows[0]["ModifiedOn"].ToString());
                            JReturnitem.Add("ModifiedByUser", dt.Tables[1].Rows[0]["ModifiedByUser"].ToString());
                            JReturnitem.Add("DeletedOn", dt.Tables[1].Rows[0]["DeletedOn"].ToString());
                            JReturnitem.Add("DeletedByUser", dt.Tables[1].Rows[0]["DeletedByUser"].ToString());
                            JReturnitem.Add("Name", dt.Tables[1].Rows[0]["Name"].ToString());
                            JReturnitem.Add("PreviewUrl", dt.Tables[1].Rows[0]["PreviewUrl"].ToString());
                            JReturnitem.Add("Status", dt.Tables[1].Rows[0]["Status"].ToString());
                            JReturnitem.Add("PublishedOn", dt.Tables[1].Rows[0]["PublishedOn"].ToString());
                            JReturnitem.Add("PublishedByUser", dt.Tables[1].Rows[0]["PublishedByUser"].ToString());
                            JReturnitem.Add("OriginalId", dt.Tables[1].Rows[0]["OriginalId"].ToString());
                        }
                        else if (flag == 3)
                        {
                            JReturnitem.Add("Flag", dt.Tables[0].Rows[0]["flag"].ToString());
                            JReturnitem.Add("ContentId", contentId);
                            JReturnitem.Add("Url", dt.Tables[1].Rows[0]["Url"].ToString());
                            JReturnitem.Add("Version", dt.Tables[1].Rows[0]["Version"].ToString());
                            JReturnitem.Add("IsDeleted", dt.Tables[1].Rows[0]["IsDeleted"].ToString());
                            JReturnitem.Add("CreatedOn", dt.Tables[1].Rows[0]["CreatedOn"].ToString());
                            JReturnitem.Add("CreatedByUser", dt.Tables[1].Rows[0]["CreatedByUser"].ToString());
                            JReturnitem.Add("ModifiedOn", dt.Tables[1].Rows[0]["ModifiedOn"].ToString());
                            JReturnitem.Add("ModifiedByUser", dt.Tables[1].Rows[0]["ModifiedByUser"].ToString());
                            JReturnitem.Add("DeletedOn", dt.Tables[1].Rows[0]["DeletedOn"].ToString());
                            JReturnitem.Add("DeletedByUser", dt.Tables[1].Rows[0]["DeletedByUser"].ToString());
                            JReturnitem.Add("Name", dt.Tables[1].Rows[0]["Name"].ToString());
                            JReturnitem.Add("PreviewUrl", dt.Tables[1].Rows[0]["PreviewUrl"].ToString());
                            JReturnitem.Add("Status", dt.Tables[1].Rows[0]["Status"].ToString());
                            JReturnitem.Add("PublishedOn", dt.Tables[1].Rows[0]["PublishedOn"].ToString());
                            JReturnitem.Add("PublishedByUser", dt.Tables[1].Rows[0]["PublishedByUser"].ToString());
                            JReturnitem.Add("OriginalId", dt.Tables[1].Rows[0]["OriginalId"].ToString());

                        }
                        else if (flag == 4)
                        {
                            JReturnitem.Add("Flag", dt.Tables[0].Rows[0]["flag"].ToString());
                            JReturnitem.Add("ContentId", contentId);
                            JReturnitem.Add("CustomCss", dt.Tables[1].Rows[0]["CustomCss"].ToString());
                            JReturnitem.Add("UseCustomCss", dt.Tables[1].Rows[0]["UseCustomCss"].ToString());
                            JReturnitem.Add("CustomJs", dt.Tables[1].Rows[0]["CustomJs"].ToString());
                            JReturnitem.Add("UseCustomJs", dt.Tables[1].Rows[0]["UseCustomJs"].ToString());
                            JReturnitem.Add("Html", dt.Tables[1].Rows[0]["Html"].ToString());
                            JReturnitem.Add("UseHtml", dt.Tables[1].Rows[0]["UseHtml"].ToString());
                            JReturnitem.Add("EditInSourceMode", dt.Tables[1].Rows[0]["EditInSourceMode"].ToString());                            
                            JReturnitem.Add("Version", dt.Tables[1].Rows[0]["Version"].ToString());
                            JReturnitem.Add("IsDeleted", dt.Tables[1].Rows[0]["IsDeleted"].ToString());
                            JReturnitem.Add("CreatedOn", dt.Tables[1].Rows[0]["CreatedOn"].ToString());
                            JReturnitem.Add("CreatedByUser", dt.Tables[1].Rows[0]["CreatedByUser"].ToString());
                            JReturnitem.Add("ModifiedOn", dt.Tables[1].Rows[0]["ModifiedOn"].ToString());
                            JReturnitem.Add("ModifiedByUser", dt.Tables[1].Rows[0]["ModifiedByUser"].ToString());
                            JReturnitem.Add("DeletedOn", dt.Tables[1].Rows[0]["DeletedOn"].ToString());
                            JReturnitem.Add("DeletedByUser", dt.Tables[1].Rows[0]["DeletedByUser"].ToString());
                            JReturnitem.Add("Name", dt.Tables[1].Rows[0]["Name"].ToString());
                            JReturnitem.Add("PreviewUrl", dt.Tables[1].Rows[0]["PreviewUrl"].ToString());
                            JReturnitem.Add("Status", dt.Tables[1].Rows[0]["Status"].ToString());
                            JReturnitem.Add("PublishedOn", dt.Tables[1].Rows[0]["PublishedOn"].ToString());
                            JReturnitem.Add("PublishedByUser", dt.Tables[1].Rows[0]["PublishedByUser"].ToString());
                            JReturnitem.Add("OriginalId", dt.Tables[1].Rows[0]["OriginalId"].ToString());
                        }
                        string js = JsonConvert.SerializeObject(JReturnitem);
                        dynamic js1 = JObject.Parse(js);
                        return js;
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                
                }
            }
        }

        [HttpGet]
        public string GetAuthorDetails(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string authorId = Info.AuthorId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetAuthorDetails";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@AuthorId", authorId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JObject JReturnitem = new JObject();
                        JReturnitem.Add("Version", dt.Rows[0]["Version"].ToString());
                        JReturnitem.Add("IsDeleted", dt.Rows[0]["IsDeleted"].ToString());
                        JReturnitem.Add("CreatedOn", dt.Rows[0]["CreatedOn"].ToString());
                        JReturnitem.Add("CreatedByUser", dt.Rows[0]["CreatedByUser"].ToString());
                        JReturnitem.Add("ModifiedOn", dt.Rows[0]["ModifiedOn"].ToString());
                        JReturnitem.Add("ModifiedByUser", dt.Rows[0]["ModifiedByUser"].ToString());
                        JReturnitem.Add("DeletedOn", dt.Rows[0]["DeletedOn"].ToString());
                        JReturnitem.Add("DeletedByUser", dt.Rows[0]["DeletedByUser"].ToString());
                        JReturnitem.Add("Name", dt.Rows[0]["Name"].ToString());
                        JReturnitem.Add("ImageId", dt.Rows[0]["ImageId"].ToString());
                        JReturnitem.Add("Description", dt.Rows[0]["Description"].ToString());
                        string js = JsonConvert.SerializeObject(JReturnitem);
                        return js;
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }
        }

    }
}

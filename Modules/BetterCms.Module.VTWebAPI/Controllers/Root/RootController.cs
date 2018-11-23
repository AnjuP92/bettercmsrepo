using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Web.Http.Cors;
using System.Data;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using BetterCms.Module.VTWebAPI.Master;
namespace BetterCms.Module.VTWebAPI.Controllers.MediaManager
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RootController : ApiController
    {
        
        

        [HttpGet]
        public int ApiRemoveCategory(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {


                        dynamic Info = JObject.Parse(JS);
                        string id = Info.Id;
                     
                        string flag = Info.Flag;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootDeleteQuery";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@Flag", flag);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return Convert.ToInt32(dt.Rows[0]["result"]);

                    }
                    catch (Exception e)
                    {
                        return 0;
                    }
                }

            }

        }

        [HttpGet]
        public string ApiSaveOrUpdateContent(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {


                        dynamic Info = JObject.Parse(JS);
                        string id = Info.Id;
                        string Version = Info.Version;

                        int IsDeleted = 0;
                        if (Info.IsDeleted == "true") { IsDeleted = 1; }
                        string Name = Info.Name;
                        string PreviewUrl = Info.PreviewUrl;
                        string ModifiedByUser = Info.ModifiedByUser;
                        int Status = 4;
                        if (Info.Status == "Published") { Status = 3; }
                        else if (Info.Status == "Preview") { Status = 1; }
                        else if (Info.Status == "Draft") { Status = 2; }
                        string PublishedByUser = Info.PublishedByUser;
                        string OriginalId = Info.OriginalId;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootSaveOrUpdateContent";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@Version ", Version);
                        cmd.Parameters.AddWithValue("@IsDeleted", IsDeleted);
                        cmd.Parameters.AddWithValue("@Name", Name);
                        cmd.Parameters.AddWithValue("@PreviewUrl", ((object)PreviewUrl) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedByUser", ModifiedByUser);
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@PublishedByUser", ((object)PublishedByUser) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OriginalId", ((object)OriginalId) ?? DBNull.Value);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return dt.Rows[0]["ID"].ToString();

                    }
                    catch (Exception e)
                    {
                        return "";
                    }
                }

            }

        }

       


        [HttpGet]
        public JArray ApiFetchCategory(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {

                        dynamic Info = JObject.Parse(JS);
                        string id = Info.Id;
                        string flag = Info.Flag;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootcategoryFetchQuery";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@Flag",flag );

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);

                      

                        JArray jarrayObj = new JArray();

                       
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            JObject oNew = new JObject();
                            oNew["Id"] = dt.Rows[i]["CategoryId"].ToString();
                            string name = ApiGetCategoryName(dt.Rows[i]["CategoryId"].ToString(), 2);
                            oNew["Name"] = name;
                            jarrayObj.Add(oNew);
                        }

                        return jarrayObj;
                    }
                    catch (Exception e)
                    {
                        return  new JArray();
                    }
                }

            }

        }
       
        public string ApiGetCategoryName(string id,int flag)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootFetchQuery";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                     

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return dt.Rows[0]["Name"].ToString();

                    }
                    catch (Exception e)
                    {
                        return "";
                    }
                }

            }

        }

        [HttpGet]
        public JArray ApiFetchCategoryList(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {

                        dynamic Info = JObject.Parse(JS);
                        string key = Info.key;
                        string flag = Info.Flag;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootCategoryIdQuery";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@key",key);
                        cmd.Parameters.AddWithValue("@Flag", flag);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);





                        JArray jarrayObj = new JArray();


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            List<JObject> categoryList = ApiGetCategorylistbyId(dt.Rows[i]["CategoryTreeId"].ToString(), 1);
                            for (int j = 0; j < categoryList.Count; j++)
                            {
                                JObject oNew = new JObject();
                                oNew["Id"] = categoryList[j]["Id"].ToString();
                                if (categoryList[j]["ParentCategoryId"] != null)
                                {
                                    oNew["Parent"] = categoryList[j]["ParentCategoryId"].ToString();
                                }
                                else
                                { oNew["Parent"] = null; }
                                oNew["text"] = categoryList[j]["Text"].ToString();
                                
                                jarrayObj.Add(oNew);
                            }
                        }

                        return jarrayObj;
                    }
                    catch (Exception e)
                    {
                        return new JArray();
                    }
                }

            }

        }

        public List<JObject> ApiGetCategorylistbyId(string id, int flag)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootGetCategorylistbyId";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@Flag", flag);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        List<JObject> jObj = new List<JObject>();


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            JObject oNew = new JObject();
                            oNew["Id"] = dt.Rows[i]["Id"].ToString();
                            oNew["Parent"] = dt.Rows[i]["ParentCategoryId"].ToString();
                            oNew["Text"] = dt.Rows[i]["Name"].ToString();
                            jObj.Add(oNew);
                        }

                        return jObj;

                    }
                    catch (Exception e)
                    {
                        return new List<JObject>();
                    }
                }

            }

        }

        [HttpGet]
        public JObject ApiFetchContent(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {

                        dynamic Info = JObject.Parse(JS);
                        string Id = Info.Id;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootContentQuery";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id",Id);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                      
                      
                            JObject NewJObject = new JObject();
                            NewJObject["Name"] = dt.Rows[0]["Name"].ToString();
                            NewJObject["PreviewUrl"] = dt.Rows[0]["PreviewUrl"].ToString();
                            NewJObject["PublishedOn"] = dt.Rows[0]["PublishedOn"].ToString();
                            NewJObject["PreviewUrl"] = dt.Rows[0]["PreviewUrl"].ToString();
                            NewJObject["PublishedByUser"] = dt.Rows[0]["PublishedByUser"].ToString();
                            NewJObject["Status"] = dt.Rows[0]["Status"].ToString();
                            NewJObject["OriginalId"] = dt.Rows[0]["OriginalId"].ToString();
                            NewJObject["Version"] = dt.Rows[0]["Version"].ToString();
                            NewJObject["ModifiedOn"] = dt.Rows[0]["ModifiedOn"].ToString();
                            NewJObject["ModifiedByUser"] = dt.Rows[0]["ModifiedByUser"].ToString();
                            if (dt.Rows[0]["IsDeleted"].ToString() == "0") { NewJObject["IsDeleted"] = "false"; }
                            else { NewJObject["IsDeleted"] = "true"; }
                            return NewJObject;
                       
                    }
                    catch (Exception e)
                    {
                        return new JObject();
                    }
                }

            }

        }


        [HttpGet]
        public JObject ApiIdCategory(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {

                        dynamic Info = JObject.Parse(JS);
                        string Id = Info.Id;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootCategoryQuery";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", Id);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);


                        JObject NewJObject = new JObject();
                        NewJObject["Id"] = dt.Rows[0]["Id"].ToString();
                        NewJObject["Version"] = dt.Rows[0]["Version"].ToString();
                        NewJObject["Name"] = dt.Rows[0]["Name"].ToString();
                        NewJObject["DisplayOrder"] = dt.Rows[0]["DisplayOrder"].ToString();
                        NewJObject["Macro"] = dt.Rows[0]["Macro"].ToString();
                        NewJObject["CategoryTreeId"] = dt.Rows[0]["CategoryTreeId"].ToString();
                        if (dt.Rows[0]["IsDeleted"].ToString() == "0") { NewJObject["IsDeleted"] = "false"; }
                        else { NewJObject["IsDeleted"] = "true"; }
                        return NewJObject;

                    }
                    catch (Exception e)
                    {
                        return new JObject();
                    }
                }

            }

        }

       


        [HttpGet]
        public JArray ApiFetchHistoryList(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        dynamic Info = JObject.Parse(JS);
                        string Id = Info.Id;
                      
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootContentHistoryList";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", Id);
                       
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JArray jarrayObj = new JArray();


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            JObject NewJObject = new JObject();
                            NewJObject["Id"] = dt.Rows[i]["Id"].ToString();
                            NewJObject["Name"] = dt.Rows[i]["Name"].ToString();
                            NewJObject["PreviewUrl"] = dt.Rows[i]["PreviewUrl"].ToString();
                            NewJObject["PublishedOn"] = dt.Rows[i]["PublishedOn"].ToString();
                            NewJObject["PreviewUrl"] = dt.Rows[i]["PreviewUrl"].ToString();
                            NewJObject["PublishedByUser"] = dt.Rows[i]["PublishedByUser"].ToString();
                            NewJObject["Status"] = dt.Rows[i]["Status"].ToString();
                            NewJObject["OriginalId"] = dt.Rows[i]["OriginalId"].ToString();
                            NewJObject["Version"] = dt.Rows[i]["Version"].ToString();
                            NewJObject["ModifiedOn"] = dt.Rows[i]["ModifiedOn"].ToString();
                            NewJObject["ModifiedByUser"] = dt.Rows[i]["ModifiedByUser"].ToString();
                            jarrayObj.Add(NewJObject);
                            
                        }

                        return jarrayObj;
                    }
                    catch (Exception e)
                    {
                        return new JArray();
                    }
                }

            }

        }
        [HttpGet]
        public JArray ApiFetchContentoptionList(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        dynamic Info = JObject.Parse(JS);
                        string Id = Info.Id;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootFetchContentoptionQuery";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", Id);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JArray jarrayObj = new JArray();


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            JObject NewJObject = new JObject();
                            NewJObject["Id"] = dt.Rows[i]["Id"].ToString();
                            NewJObject["Key"] = dt.Rows[i]["Key"].ToString();
                            NewJObject["Type"] = dt.Rows[i]["Type"].ToString();
                            NewJObject["DefaultValue"] = dt.Rows[i]["DefaultValue"].ToString();
                            NewJObject["IsDeletable"] = dt.Rows[i]["IsDeletable"].ToString();
                            NewJObject["CustomOptionId"] = dt.Rows[i]["CustomOptionId"].ToString();
                           
                            jarrayObj.Add(NewJObject);

                        }

                        return jarrayObj;
                    }
                    catch (Exception e)
                    {
                        return new JArray();
                    }
                }

            }

        }
        [HttpGet]
        public JArray ApiFetchChildContentList(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        dynamic Info = JObject.Parse(JS);
                        string Id = Info.Id;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootFetchChildContentQuery";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", Id);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JArray jarrayObj = new JArray();


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            
                            JObject jobject = ApiFetchContent(dt.Rows[i]["Id"].ToString());
                            jobject["Id"] = dt.Rows[i]["Id"].ToString();


                           
                            jarrayObj.Add(jobject);

                        }

                        return jarrayObj;
                    }
                    catch (Exception e)
                    {
                        return new JArray();
                    }
                }

            }

        }

        [HttpGet]
        public JArray ApiFetchContentRegionList(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        dynamic Info = JObject.Parse(JS);
                        string Id = Info.Id;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootFetchFetchContentRegionQuery";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", Id);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JArray jarrayObj = new JArray();


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            JObject jobject = new JObject();
                            jobject["Id"] = dt.Rows[i]["Id"].ToString();
                            jobject["RegionIdentifier"] = dt.Rows[i]["RegionIdentifier"].ToString();
                            jarrayObj.Add(jobject);

                        }

                        return jarrayObj;
                    }
                    catch (Exception e)
                    {
                        return new JArray();
                    }
                }

            }

        }

        [HttpGet]
        public JArray ApiFetchLanguage()
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                      

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootFetchLanguage";
                        cmd.Connection = con;

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JArray jarrayObj = new JArray();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            JObject jobject = new JObject();
                            jobject["Name"] = dt.Rows[i]["Name"].ToString();
                            jobject["Code"] = dt.Rows[i]["Code"].ToString();
                            jarrayObj.Add(jobject);

                        }

                        return jarrayObj;
                    }
                    catch (Exception e)
                    {
                        return new JArray();
                    }
                }

            }

        }


        [HttpGet]
        public JArray ApiFetchLayoutRegionList(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        dynamic Info = JObject.Parse(JS);
                        string Id = Info.Id;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootFetchLayoutRegionListQuery";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", Id);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JArray jarrayObj = new JArray();


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            JObject jobj = ApiFetchLayoutRegion(dt.Rows[i]["Id"].ToString());
                            jarrayObj.Add(jobj);
                        }

                        return jarrayObj;
                    }
                    catch (Exception e)
                    {
                        return new JArray();
                    }
                }

            }

        }
        [HttpGet]
        public JObject ApiFetchLayoutRegion(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        dynamic Info = JObject.Parse(JS);
                        string Id = Info.Id;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootFetchLayoutRegionQuery";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", Id);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                       
                        JObject jobject = new JObject();
                        jobject["Id"] = dt.Rows[0]["Id"].ToString();
                        jobject["Version"] = dt.Rows[0]["Version"].ToString();
                        jobject["IsDeleted"] = dt.Rows[0]["IsDeleted"].ToString();
                        jobject["ModifiedOn"] = dt.Rows[0]["ModifiedOn"].ToString();
                        jobject["ModifiedByUser"] = dt.Rows[0]["ModifiedByUser"].ToString();
                        jobject["CreatedOn"] = dt.Rows[0]["CreatedOn"].ToString();
                        jobject["CreatedByUser"] = dt.Rows[0]["CreatedByUser"].ToString();
                        jobject["LayoutId"] = dt.Rows[0]["LayoutId"].ToString();
                        jobject["RegionId"] = dt.Rows[0]["RegionId"].ToString();
                        jobject["Description"] = dt.Rows[0]["Description"].ToString();
                      

                        return jobject;
                    }
                    catch (Exception e)
                    {
                        return new JObject();
                    }
                }

            }

        }
        [HttpGet]
        public JArray ApiFetchPageContentList(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        dynamic Info = JObject.Parse(JS);
                        string Id = Info.Id;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootFetchPageContentlistQuery";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", Id);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JArray jarrayObj = new JArray();


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            JObject jobj = ApiFetchPagecontent(dt.Rows[i]["Id"].ToString());
                            jarrayObj.Add(jobj);
                        }

                        return jarrayObj;
                    }
                    catch (Exception e)
                    {
                        return new JArray();
                    }
                }

            }

        }

        [HttpGet]
        public JObject ApiFetchPagecontent(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        dynamic Info = JObject.Parse(JS);
                        string Id = Info.Id;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootFetchPageContentQuery";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", Id);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);

                        JObject jobject = new JObject();
                        jobject["Id"] = dt.Rows[0]["Id"].ToString();
                        jobject["Version"] = dt.Rows[0]["Version"].ToString();
                        jobject["IsDeleted"] = dt.Rows[0]["IsDeleted"].ToString();
                        jobject["ModifiedOn"] = dt.Rows[0]["ModifiedOn"].ToString();
                        jobject["ModifiedByUser"] = dt.Rows[0]["ModifiedByUser"].ToString();
                        jobject["CreatedOn"] = dt.Rows[0]["CreatedOn"].ToString();
                        jobject["CreatedByUser"] = dt.Rows[0]["CreatedByUser"].ToString();
                        jobject["PageId"] = dt.Rows[0]["PageId"].ToString();
                        jobject["ContentId"] = dt.Rows[0]["ContentId"].ToString();
                        jobject["Order"] = dt.Rows[0]["Order"].ToString();
                        jobject["RegionId"] = dt.Rows[0]["RegionId"].ToString();
                        jobject["ParentPageContentId"] = dt.Rows[0]["ParentPageContentId"].ToString();
                       
                        return jobject;
                    }
                    catch (Exception e)
                    {
                        return new JObject();
                    }
                }

            }

        }

        [HttpGet]
        public JObject ApiFetchRegion(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        dynamic Info = JObject.Parse(JS);
                        string Id = Info.Id;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootFetchRegionsQuery";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id", Id);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);

                        JObject jobject = new JObject();
                        jobject["Version"] = dt.Rows[0]["Version"].ToString();
                        jobject["ModifiedOn"] = dt.Rows[0]["ModifiedOn"].ToString();
                        jobject["ModifiedByUser"] = dt.Rows[0]["ModifiedByUser"].ToString();
                        jobject["RegionIdentifier"] = dt.Rows[0]["RegionIdentifier"].ToString();
                      
                        return jobject;
                    }
                    catch (Exception e)
                    {
                        return new JObject();
                    }
                }

            }

        }

        [HttpGet]
        public JObject ApiSelectContentbyId(String Js)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(Js);
            string Id = Info.Id;

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spRootSelectContentbyId";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Id",Id);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataSet dt = new DataSet();
                        ad.Fill(dt);

                        JObject JReturnitem = new JObject();
                        int flag = Convert.ToInt32(dt.Tables[0].Rows[0]["flag"]);
                        if (flag == 1)
                        {


                            JReturnitem.Add("Flag", dt.Tables[0].Rows[0]["flag"].ToString());

                            JReturnitem.Add("Version", dt.Tables[1].Rows[0]["Version"].ToString());
                            JReturnitem.Add("IsDeleted", dt.Tables[1].Rows[0]["IsDeleted"].ToString());
                            JReturnitem.Add("CreatedOn", dt.Tables[1].Rows[0]["CreatedOn"].ToString());
                            JReturnitem.Add("CreatedByUser", dt.Tables[1].Rows[0]["CreatedByUser"].ToString());
                            JReturnitem.Add("ModifiedOn", dt.Tables[1].Rows[0]["ModifiedOn"].ToString());
                            JReturnitem.Add("ModifiedByUser", dt.Tables[1].Rows[0]["ModifiedByUser"].ToString());

                            JReturnitem.Add("Name", dt.Tables[1].Rows[0]["Name"].ToString());
                            JReturnitem.Add("PreviewUrl", dt.Tables[1].Rows[0]["PreviewUrl"].ToString());
                            JReturnitem.Add("Status", dt.Tables[1].Rows[0]["Status"].ToString());
                            JReturnitem.Add("PublishedOn", dt.Tables[1].Rows[0]["PublishedOn"].ToString());
                            JReturnitem.Add("PublishedByUser", dt.Tables[1].Rows[0]["PublishedByUser"].ToString());

                            JReturnitem.Add("OriginalId", dt.Tables[1].Rows[0]["OriginalId"].ToString());
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



                            return JReturnitem;

                        }
                        else if (flag == 2)
                        {
                            JReturnitem.Add("Flag", dt.Tables[0].Rows[0]["flag"].ToString());

                            JReturnitem.Add("Version", dt.Tables[1].Rows[0]["Version"].ToString());
                            JReturnitem.Add("IsDeleted", dt.Tables[1].Rows[0]["IsDeleted"].ToString());
                            JReturnitem.Add("CreatedOn", dt.Tables[1].Rows[0]["CreatedOn"].ToString());
                            JReturnitem.Add("CreatedByUser", dt.Tables[1].Rows[0]["CreatedByUser"].ToString());
                            JReturnitem.Add("ModifiedOn", dt.Tables[1].Rows[0]["ModifiedOn"].ToString());
                            JReturnitem.Add("ModifiedByUser", dt.Tables[1].Rows[0]["ModifiedByUser"].ToString());

                            JReturnitem.Add("Name", dt.Tables[1].Rows[0]["Name"].ToString());
                            JReturnitem.Add("PreviewUrl", dt.Tables[1].Rows[0]["PreviewUrl"].ToString());
                            JReturnitem.Add("Status", dt.Tables[1].Rows[0]["Status"].ToString());
                            JReturnitem.Add("PublishedOn", dt.Tables[1].Rows[0]["PublishedOn"].ToString());
                            JReturnitem.Add("PublishedByUser", dt.Tables[1].Rows[0]["PublishedByUser"].ToString());

                            JReturnitem.Add("OriginalId", dt.Tables[1].Rows[0]["OriginalId"].ToString());

                            JReturnitem.Add("CustomCss", dt.Tables[1].Rows[0]["CustomCss"].ToString());
                            JReturnitem.Add("UseCustomCss", dt.Tables[1].Rows[0]["UseCustomCss"].ToString());

                            JReturnitem.Add("CustomJs", dt.Tables[1].Rows[0]["CustomJs"].ToString());
                            JReturnitem.Add("UseCustomJs", dt.Tables[1].Rows[0]["UseCustomJs"].ToString());
                            JReturnitem.Add("Html", dt.Tables[1].Rows[0]["Html"].ToString());
                            JReturnitem.Add("UseHtml", dt.Tables[1].Rows[0]["UseHtml"].ToString());
                            JReturnitem.Add("EditInSourceMode", dt.Tables[1].Rows[0]["EditInSourceMode"].ToString());

                            return JReturnitem;
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

                            JReturnitem.Add("Name", dt.Tables[1].Rows[0]["Name"].ToString());
                            JReturnitem.Add("PreviewUrl", dt.Tables[1].Rows[0]["PreviewUrl"].ToString());
                            JReturnitem.Add("Status", dt.Tables[1].Rows[0]["Status"].ToString());
                            JReturnitem.Add("PublishedOn", dt.Tables[1].Rows[0]["PublishedOn"].ToString());
                            JReturnitem.Add("PublishedByUser", dt.Tables[1].Rows[0]["PublishedByUser"].ToString());

                            JReturnitem.Add("OriginalId", dt.Tables[1].Rows[0]["OriginalId"].ToString());
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

                            return JReturnitem;
                        }

                    }
                    catch (Exception ex)
                    {
                        return new JObject();
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
                        cmd.CommandText = "spRootRegionPageContentDetails";
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
     

    }
}
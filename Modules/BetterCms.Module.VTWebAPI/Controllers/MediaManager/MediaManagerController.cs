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
    public class MediaManagerController:ApiController
    {
        [HttpGet]
        public int DeleteQuery(Guid id, int flag)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];
           

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spMediaDeleteQuery";
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Flag", flag);
                    cmd.Connection = con;
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    return Convert.ToInt32(dt.Rows[0][0]);
                }
            }

        }

        [HttpGet]
        public string SaveImageQuery(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic ImageInfo = JObject.Parse(JS);
            string id = ImageInfo.Id;
            int Version = Convert.ToInt32(ImageInfo.Version);
            string IsDeleted = ImageInfo.IsDeleted;
            string FolderId = ImageInfo.FolderId;
            string Title = ImageInfo.Title;
            int Type=1;
            int ContentType=1;
            if (ImageInfo.Type == "Image") { Type = 1; }
            if (ImageInfo.Type == "Video") { Type = 2; }
            if (ImageInfo.Type == "Audio") { Type = 3; }
            if (ImageInfo.Type == "File") { Type = 4; }
            if (ImageInfo.ContentType == "Image") { ContentType = 1; }
            if (ImageInfo.ContentType == "Video") { ContentType = 2; }
            if (ImageInfo.ContentType == "Audio") { ContentType = 3; }
            if (ImageInfo.ContentType == "File") { ContentType = 4; }

            string IsArchived = ImageInfo.IsArchived;
            string OriginalId = ImageInfo.OriginalId;
            string ImageId = ImageInfo.ImageId;
            string Description = ImageInfo.Description;
            string OriginalFileName1 = ImageInfo.OriginalFileName;
            string OriginalFileExtension = ImageInfo.OriginalFileExtension;
            string FileUri = ImageInfo.FileUri;
            string PublicUrl = ImageInfo.PublicUrl;
            string Size = ImageInfo.Size;
            string IsTemporary = ImageInfo.IsTemporary;
            string IsUploaded = ImageInfo.IsUploaded;
            string IsCanceled = ImageInfo.IsCanceled;
            string IsMovedToTrash = ImageInfo.IsMovedToTrash;
            string NextTryToMoveToTrash = ImageInfo.NextTryToMoveToTrash;
            string Caption = ImageInfo.Caption;
            string ImageAlign= ImageInfo.ImageAlign;
            string Width=ImageInfo.Width;
              string Height= ImageInfo.Height;
              string CropCoordX1=ImageInfo.CropCoordX1;
              string CropCoordY1=ImageInfo.CropCoordY1;
              string CropCoordX2= ImageInfo.CropCoordX2;
              string CropCoordY2=ImageInfo.CropCoordY2;
              string OriginalWidth=ImageInfo.OriginalWidth;
              string OriginalHeight=ImageInfo.OriginalHeight;
              string OriginalSize= ImageInfo.OriginalSize;
              string OriginalUri= ImageInfo.OriginalUri;
              string PublicOriginallUrl=ImageInfo.PublicOriginallUrl;
              string IsOriginalUploaded = ImageInfo.IsOriginalUploaded;
              string ThumbnailWidth =ImageInfo.ThumbnailWidth;
              string ThumbnailHeight =  ImageInfo.ThumbnailHeight;
              string ThumbnailSize =ImageInfo.ThumbnailSize;
              string ThumbnailUri = ImageInfo.ThumbnailUri;
              //string ThumbnailUri = ImageInfo.ThumbnailUri;
             // var th = (string)ImageInfo.SelectToken("ThumbnailUri");
              //string timeZone = ImageInfo["ThumbnailUri"].Value<string>();
              string PublicThumbnailUrl =  ImageInfo.PublicThumbnailUrl;
              string IsThumbnailUploaded =ImageInfo.IsThumbnailUploaded;
              string flag = ImageInfo.Flag;


            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spMediaSaveImage";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@Version", ((object)Version) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsDeleted", ((object)IsDeleted) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Title ", ((object)Title) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Type", ((object)Type) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ContentType", ((object)ContentType) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsArchived", ((object)IsArchived) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Description", ((object)Description) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OriginalFileName", ((object)OriginalFileName1) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OriginalFileExtension", OriginalFileExtension);
                        cmd.Parameters.AddWithValue("@FileUri", ((object)FileUri) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PublicUrl", ((object)PublicUrl) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Size", ((object)Size) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsTemporary", ((object)IsTemporary) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsUploaded", ((object)IsUploaded) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsCanceled", ((object)IsCanceled) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsMovedToTrash", ((object)IsMovedToTrash) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NextTryToMoveToTrash", ((object)NextTryToMoveToTrash) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Caption", ((object)Caption) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ImageAlign", ((object)ImageAlign) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Width", ((object)Width) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Height", ((object)Height) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CropCoordX1", ((object)CropCoordX1) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CropCoordY1 ", ((object)CropCoordY1) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CropCoordX2", ((object)CropCoordX2) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CropCoordY2", ((object)CropCoordY2) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OriginalWidth", ((object)OriginalWidth) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OriginalHeight", ((object)OriginalHeight) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OriginalSize", ((object)OriginalSize) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OriginalUri", ((object)OriginalUri) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PublicOriginallUrl", ((object)PublicOriginallUrl) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsOriginalUploaded", ((object)IsOriginalUploaded) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ThumbnailWidth", ((object)ThumbnailWidth) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ThumbnailHeight", ((object)ThumbnailHeight) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ThumbnailSize", ((object)ThumbnailSize) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ThumbnailUri", ((object)ThumbnailUri) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PublicThumbnailUrl", ((object)PublicThumbnailUrl) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsThumbnailUploaded", ((object)IsThumbnailUploaded) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Flag",flag);
                        //cmd.Connection = con;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return dt.Rows[0][0].ToString();

                       
                    }
                    catch (Exception e)
                    {
                        return "";
                    }
                  }
               
            }

        }

        [HttpGet]
        public string SaveMediaFile(string JS)
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
                        int Version = Convert.ToInt32(Info.Version);
                        string IsDeleted = Info.IsDeleted;
                        string FolderId = Info.FolderId;
                        string Title = Info.Title;
                        int Type = 1;
                        int ContentType = 1;
                        if (Info.Type == "Image") { Type = 1; }
                        if (Info.Type == "Video") { Type = 2; }
                        if (Info.Type == "Audio") { Type = 3; }
                        if (Info.Type == "File") { Type = 4; }
                        if (Info.ContentType == "Image") { ContentType = 1; }
                        if (Info.ContentType == "Video") { ContentType = 2; }
                        if (Info.ContentType == "Audio") { ContentType = 3; }
                        if (Info.ContentType == "File") { ContentType = 4; }
                        string IsArchived = Info.IsArchived;
                        string OriginalId = Info.OriginalId;
                     
                        string ImageId = Info.ImageId;
                        string Description = Info.Description;
                        string OriginalFileName1 = Info.OriginalFileName;
                        string OriginalFileExtension = Info.OriginalFileExtension;
                        string FileUri = Info.FileUri;
                        string PublicUrl = Info.PublicUrl;
                        string Size = Info.Size;
                        string IsTemporary = Info.IsTemporary;
                        string IsUploaded = Info.IsUploaded;
                        string IsCanceled = Info.IsCanceled;
                        string IsMovedToTrash = Info.IsMovedToTrash;
                        string NextTryToMoveToTrash = Info.NextTryToMoveToTrash;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spMediaSaveFile";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@Version", ((object)Version) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsDeleted", ((object)IsDeleted) ?? DBNull.Value);
                     
                        cmd.Parameters.AddWithValue("@Title ", ((object)Title) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Type", ((object)Type) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ContentType", ((object)ContentType) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsArchived", ((object)IsArchived) ?? DBNull.Value);
                     
                        cmd.Parameters.AddWithValue("@Description", ((object)Description) ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@OriginalFileName", ((object)OriginalFileName1) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OriginalFileExtension", OriginalFileExtension);
                        cmd.Parameters.AddWithValue("@FileUri", ((object)FileUri) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PublicUrl", ((object)PublicUrl) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Size", ((object)Size) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsTemporary", ((object)IsTemporary) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsUploaded", ((object)IsUploaded) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsCanceled", ((object)IsCanceled) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsMovedToTrash", ((object)IsMovedToTrash) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NextTryToMoveToTrash", ((object)NextTryToMoveToTrash) ?? DBNull.Value);

                        //cmd.Connection = con;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return dt.Rows[0][0].ToString();
                       
                    }
                    catch (Exception e)
                    {
                        return "";
                    }
                }
                
            }

        }

        [HttpGet]
        public string UpdateMediaFile(string JS)
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
                        int Version = Convert.ToInt32(Info.Version);
                        string IsDeleted = Info.IsDeleted;
                        string FolderId = Info.FolderId;
                        string Title = Info.Title;
                        int Type = 4;
                        int ContentType = 4;
                        string IsArchived = Info.IsArchived;
                        string OriginalId = Info.OriginalId;

                        string ImageId = Info.ImageId;
                        string Description = Info.Description;
                        string OriginalFileName1 = Info.OriginalFileName;
                        string OriginalFileExtension = Info.OriginalFileExtension;
                        string FileUri = Info.FileUri;
                        string PublicUrl = Info.PublicUrl;
                        string Size = Info.Size;
                        string IsTemporary = Info.IsTemporary;
                        string IsUploaded = Info.IsUploaded;
                        string IsCanceled = Info.IsCanceled;
                        string IsMovedToTrash = Info.IsMovedToTrash;
                        string NextTryToMoveToTrash = Info.NextTryToMoveToTrash;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spMediaUpdateFile";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@Version", ((object)Version) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsDeleted", ((object)IsDeleted) ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@Title ", ((object)Title) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Type", ((object)Type) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ContentType", ((object)ContentType) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsArchived", ((object)IsArchived) ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@Description", ((object)Description) ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@OriginalFileName", ((object)OriginalFileName1) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OriginalFileExtension", OriginalFileExtension);
                        cmd.Parameters.AddWithValue("@FileUri", ((object)FileUri) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PublicUrl", ((object)PublicUrl) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Size", ((object)Size) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsTemporary", ((object)IsTemporary) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsUploaded", ((object)IsUploaded) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsCanceled", ((object)IsCanceled) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsMovedToTrash", ((object)IsMovedToTrash) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NextTryToMoveToTrash", ((object)NextTryToMoveToTrash) ?? DBNull.Value);

                        //cmd.Connection = con;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return dt.Rows[0][0].ToString();

                    }
                    catch (Exception e)
                    {
                        return "";
                    }
                }

            }

        }



       
        [HttpGet]
        public int SaveMediaMoveToTrash(string JS)
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
                        string IsMovedToTrash = Info.IsMovedToTrash;
                    
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spMediaMoveToTrash";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                       
                        cmd.Parameters.AddWithValue("@IsMovedToTrash", ((object)IsMovedToTrash) ?? DBNull.Value);
                       //cmd.Connection = con;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return Convert.ToInt32(dt.Rows[0][0]);
                       
                    }
                    catch (Exception e)
                    {
                        return 0;
                    }
                }
                
            }

        }
        [HttpGet]
        public int MediaDeleteFile(string JS)
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
                        string IsMovedToTrash = Info.IsMovedToTrash;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spDeleteFile";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);

                     
                        //cmd.Connection = con;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return Convert.ToInt32(dt.Rows[0][0]);

                    }
                    catch (Exception e)
                    {
                        return 0;
                    }
                }

            }

        }

        [HttpGet]
        public int SaveMedias(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string id = Info.Id;
            int Version = Convert.ToInt32(Info.Version);
            string IsDeleted = Info.IsDeleted;
            string FolderId = Info.FolderId;
            string Title = Info.Title;
            int Type = 4;
            int ContentType = 4;
            string IsArchived = Info.IsArchived;
            string OriginalId = Info.OriginalId;
            string ImageId = Info.ImageId;
            string Description = Info.Description;

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spMediaSaveFile";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@Version", ((object)Version) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsDeleted", ((object)IsDeleted) ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@Title ", ((object)Title) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Type", ((object)Type) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ContentType", ((object)ContentType) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsArchived", ((object)IsArchived) ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@Description", ((object)Description) ?? DBNull.Value);

                       
                        //cmd.Connection = con;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return Convert.ToInt32(dt.Rows[0][0]);

                    }
                    catch (Exception e)
                    {
                        return 0;
                    }
                }

            }

        }

        [HttpGet]
        public int GetVersionCount(string JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string id = Info.OriginalId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spMediaGetVersionCount";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ImageId", id);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return Convert.ToInt32(dt.Rows[0][0]);
                    }
                    catch (Exception e)
                    {
                        return 0;
                    }
                }
            }
           
        }

        [HttpGet]
        public string GetMediaDetails(string JS)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string id = Info.mediaId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spMediaGetMediaDetails";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@MediaId", id);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataSet dt = new DataSet();
                        ad.Fill(dt);

                        JObject JReturnitem = new JObject();
                        int flag = Convert.ToInt32(dt.Tables[0].Rows[0]["flag"]);
                        if (flag == 1)
                        {
                            JReturnitem.Add("Flag", dt.Tables[0].Rows[0]["flag"].ToString());
                            JReturnitem.Add("Caption", dt.Tables[1].Rows[0]["Caption"].ToString());
                            JReturnitem.Add("ImageAlign", dt.Tables[1].Rows[0]["ImageAlign"].ToString());
                            JReturnitem.Add("Width", dt.Tables[1].Rows[0]["Width"].ToString());
                            JReturnitem.Add("Height", dt.Tables[1].Rows[0]["Height"].ToString());
                            JReturnitem.Add("CropCoordX1", dt.Tables[1].Rows[0]["CropCoordX1"].ToString());
                            JReturnitem.Add("CropCoordY1", dt.Tables[1].Rows[0]["CropCoordY1"].ToString());
                            JReturnitem.Add("CropCoordX2", dt.Tables[1].Rows[0]["CropCoordX2"].ToString());
                            JReturnitem.Add("CropCoordY2", dt.Tables[1].Rows[0]["CropCoordY2"].ToString());
                            JReturnitem.Add("OriginalWidth", dt.Tables[1].Rows[0]["OriginalWidth"].ToString());
                            JReturnitem.Add("OriginalHeight", dt.Tables[1].Rows[0]["OriginalHeight"].ToString());
                            JReturnitem.Add("OriginalSize", dt.Tables[1].Rows[0]["OriginalSize"].ToString());
                            JReturnitem.Add("OriginalUri", dt.Tables[1].Rows[0]["OriginalUri"].ToString());
                            JReturnitem.Add("PublicOriginallUrl", dt.Tables[1].Rows[0]["PublicOriginallUrl"].ToString());
                            JReturnitem.Add("IsOriginalUploaded", dt.Tables[1].Rows[0]["IsOriginalUploaded"].ToString());
                            JReturnitem.Add("ThumbnailWidth", dt.Tables[1].Rows[0]["ThumbnailWidth"].ToString());
                            JReturnitem.Add("ThumbnailHeight", dt.Tables[1].Rows[0]["ThumbnailHeight"].ToString());
                            JReturnitem.Add("ThumbnailSize", dt.Tables[1].Rows[0]["ThumbnailSize"].ToString());
                            JReturnitem.Add("ThumbnailUri", dt.Tables[1].Rows[0]["ThumbnailUri"].ToString());
                            JReturnitem.Add("PublicThumbnailUrl", dt.Tables[1].Rows[0]["PublicThumbnailUrl"].ToString());
                            JReturnitem.Add("IsThumbnailUploaded", dt.Tables[1].Rows[0]["IsThumbnailUploaded"].ToString());

                            JReturnitem.Add("Version", dt.Tables[1].Rows[0]["Version"].ToString());
                            JReturnitem.Add("IsDeleted", dt.Tables[1].Rows[0]["IsDeleted"].ToString());
                            JReturnitem.Add("CreatedOn", dt.Tables[1].Rows[0]["CreatedOn"].ToString());
                            JReturnitem.Add("CreatedByUser", dt.Tables[1].Rows[0]["CreatedByUser"].ToString());
                            JReturnitem.Add("ModifiedOn", dt.Tables[1].Rows[0]["ModifiedOn"].ToString());
                            JReturnitem.Add("ModifiedByUser", dt.Tables[1].Rows[0]["ModifiedByUser"].ToString());
                            JReturnitem.Add("DeletedOn", dt.Tables[1].Rows[0]["DeletedOn"].ToString());
                            JReturnitem.Add("DeletedByUser", dt.Tables[1].Rows[0]["DeletedByUser"].ToString());
                            JReturnitem.Add("FolderId", dt.Tables[1].Rows[0]["FolderId"].ToString());
                            JReturnitem.Add("Title", dt.Tables[1].Rows[0]["Title"].ToString());
                            JReturnitem.Add("Type", dt.Tables[1].Rows[0]["Type"].ToString());
                            JReturnitem.Add("ContentType", dt.Tables[1].Rows[0]["ContentType"].ToString());
                            JReturnitem.Add("IsArchived", dt.Tables[1].Rows[0]["IsArchived"].ToString());
                            JReturnitem.Add("OriginalId", dt.Tables[1].Rows[0]["OriginalId"].ToString());
                            JReturnitem.Add("PublishedOn", dt.Tables[1].Rows[0]["PublishedOn"].ToString());
                            JReturnitem.Add("ImageId", dt.Tables[1].Rows[0]["ImageId"].ToString());
                            JReturnitem.Add("Description", dt.Tables[1].Rows[0]["Description"].ToString());

                            JReturnitem.Add("OriginalFileName", dt.Tables[1].Rows[0]["OriginalFileName"].ToString());
                            JReturnitem.Add("OriginalFileExtension", dt.Tables[1].Rows[0]["OriginalFileExtension"].ToString());
                            JReturnitem.Add("FileUri", dt.Tables[1].Rows[0]["FileUri"].ToString());
                            JReturnitem.Add("PublicUrl", dt.Tables[1].Rows[0]["PublicUrl"].ToString());
                            JReturnitem.Add("Size", dt.Tables[1].Rows[0]["Size"].ToString());
                            JReturnitem.Add("IsTemporary", dt.Tables[1].Rows[0]["IsTemporary"].ToString());
                            JReturnitem.Add("IsUploaded", dt.Tables[1].Rows[0]["IsUploaded"].ToString());
                            JReturnitem.Add("IsCanceled", dt.Tables[1].Rows[0]["IsCanceled"].ToString());
                            JReturnitem.Add("IsMovedToTrash", dt.Tables[1].Rows[0]["IsMovedToTrash"].ToString());
                            JReturnitem.Add("NextTryToMoveToTrash", dt.Tables[1].Rows[0]["NextTryToMoveToTrash"].ToString());

                            string js = JsonConvert.SerializeObject(JReturnitem);
                            return js;


                        }
                        else if (flag == 2)
                        {
                            JReturnitem.Add("Flag", dt.Tables[0].Rows[0]["flag"].ToString());
                            JReturnitem.Add("OriginalFileName", dt.Tables[1].Rows[0]["OriginalFileName"].ToString());
                            JReturnitem.Add("OriginalFileExtension", dt.Tables[1].Rows[0]["OriginalFileExtension"].ToString());
                            JReturnitem.Add("FileUri", dt.Tables[1].Rows[0]["FileUri"].ToString());
                            JReturnitem.Add("PublicUrl", dt.Tables[1].Rows[0]["PublicUrl"].ToString());
                            JReturnitem.Add("Size", dt.Tables[1].Rows[0]["Size"].ToString());
                            JReturnitem.Add("IsTemporary", dt.Tables[1].Rows[0]["IsTemporary"].ToString());
                            JReturnitem.Add("IsUploaded", dt.Tables[1].Rows[0]["IsUploaded"].ToString());
                            JReturnitem.Add("IsCanceled", dt.Tables[1].Rows[0]["IsCanceled"].ToString());
                            JReturnitem.Add("IsMovedToTrash", dt.Tables[1].Rows[0]["IsMovedToTrash"].ToString());
                            JReturnitem.Add("NextTryToMoveToTrash", dt.Tables[1].Rows[0]["NextTryToMoveToTrash"].ToString());
                            JReturnitem.Add("Version", dt.Tables[1].Rows[0]["Version"].ToString());
                            JReturnitem.Add("IsDeleted", dt.Tables[1].Rows[0]["IsDeleted"].ToString());
                            JReturnitem.Add("CreatedOn", dt.Tables[1].Rows[0]["CreatedOn"].ToString());
                            JReturnitem.Add("CreatedByUser", dt.Tables[1].Rows[0]["CreatedByUser"].ToString());
                            JReturnitem.Add("ModifiedOn", dt.Tables[1].Rows[0]["ModifiedOn"].ToString());
                            JReturnitem.Add("ModifiedByUser", dt.Tables[1].Rows[0]["ModifiedByUser"].ToString());
                            JReturnitem.Add("DeletedOn", dt.Tables[1].Rows[0]["DeletedOn"].ToString());
                            JReturnitem.Add("DeletedByUser", dt.Tables[1].Rows[0]["DeletedByUser"].ToString());
                            JReturnitem.Add("FolderId", dt.Tables[1].Rows[0]["FolderId"].ToString());
                            JReturnitem.Add("Title", dt.Tables[1].Rows[0]["Title"].ToString());
                            JReturnitem.Add("Type", dt.Tables[1].Rows[0]["Type"].ToString());
                            JReturnitem.Add("ContentType", dt.Tables[1].Rows[0]["ContentType"].ToString());
                            JReturnitem.Add("IsArchived", dt.Tables[1].Rows[0]["IsArchived"].ToString());
                            JReturnitem.Add("OriginalId", dt.Tables[1].Rows[0]["OriginalId"].ToString());
                            JReturnitem.Add("PublishedOn", dt.Tables[1].Rows[0]["PublishedOn"].ToString());
                            JReturnitem.Add("ImageId", dt.Tables[1].Rows[0]["ImageId"].ToString());
                            JReturnitem.Add("Description", dt.Tables[1].Rows[0]["Description"].ToString());
                            string js = JsonConvert.SerializeObject(JReturnitem);
                            return js;

                        }
                        else
                        {
                            return "";
                        }
                    }
                    catch (Exception e)
                    {
                        return "";
                    }

                }
            }
        }
        [HttpGet]
        public JArray GetMediaCategoriesDetails(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string mediaId = Info.mediaId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetMediaCategoriesDetails";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@MediaId", mediaId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JArray jarrayObj = new JArray();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            JObject NewJObject = new JObject();
                            NewJObject.Add("Id", dt.Rows[i]["Id"].ToString());
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
        public string GetCategoryDetails(String JS)
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
                        cmd.CommandText = "spGetCategoryDetails";
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
        public string GetCategoryTreeDetails(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string categoryTreeId = Info.categorytreeId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetCategoryTreeDetails";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@CategoryTreeId", categoryTreeId);
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
                        JReturnitem.Add("Title", dt.Rows[0]["Title"].ToString());
                        JReturnitem.Add("Macro", dt.Rows[0]["Macro"].ToString());
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
        public JArray GetAvailableFor(String JS)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            dynamic Info = JObject.Parse(JS);
            string categorytreeId = Info.categorytreeId;
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetAvailableFor";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@CategoryTreeId", categorytreeId);
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        JArray jarrayObj = new JArray();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            JObject NewJObject = new JObject();
                            NewJObject.Add("Id", dt.Rows[i]["Id"].ToString());
                            NewJObject.Add("Version", dt.Rows[i]["Version"].ToString());
                            NewJObject.Add("IsDeleted", dt.Rows[i]["IsDeleted"].ToString());
                            NewJObject.Add("CreatedOn", dt.Rows[i]["CreatedOn"].ToString());
                            NewJObject.Add("CreatedByUser", dt.Rows[i]["CreatedByUser"].ToString());
                            NewJObject.Add("ModifiedOn", dt.Rows[i]["ModifiedOn"].ToString());
                            NewJObject.Add("ModifiedByUser", dt.Rows[i]["ModifiedByUser"].ToString());
                            NewJObject.Add("DeletedOn", dt.Rows[i]["DeletedOn"].ToString());
                            NewJObject.Add("DeletedByUser", dt.Rows[i]["DeletedByUser"].ToString());
                            NewJObject.Add("CategorizableItemId", dt.Rows[i]["CategorizableItemId"].ToString());
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
         public string GetCategorizableItemDetails(String JS)
         {
             string _constring = ConfigurationManager.AppSettings["BetterCms"];
             dynamic Info = JObject.Parse(JS);
             string categorizableItemId = Info.categorizableitemId;
             using (SqlConnection con = new SqlConnection(_constring))
             {
                 using (SqlCommand cmd = new SqlCommand())
                 {
                     try
                     {
                         cmd.CommandType = CommandType.StoredProcedure;
                         cmd.CommandText = "spGetCategorizableItemDetails";
                         cmd.Connection = con;
                         cmd.Parameters.AddWithValue("@CategorizableItemId", categorizableItemId);
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
         public JArray GetCategoriesforCategoryTree(String JS)
         {
             string _constring = ConfigurationManager.AppSettings["BetterCms"];
             dynamic Info = JObject.Parse(JS);
             string categorytreeId = Info.categorytreeId;
             using (SqlConnection con = new SqlConnection(_constring))
             {
                 using (SqlCommand cmd = new SqlCommand())
                 {
                     try
                     {
                         cmd.CommandType = CommandType.StoredProcedure;
                         cmd.CommandText = "spGetCategoriesforCategoryTree";
                         cmd.Connection = con;
                         cmd.Parameters.AddWithValue("@CategoryTreeId", categorytreeId);
                         SqlDataAdapter ad = new SqlDataAdapter(cmd);
                         DataTable dt = new DataTable();
                         ad.Fill(dt);
                         JArray jarrayObj = new JArray();
                         for (int i = 0; i < dt.Rows.Count; i++)
                         {

                             JObject NewJObject = new JObject();
                             NewJObject.Add("Id", dt.Rows[i]["Id"].ToString());
                             NewJObject.Add("Version", dt.Rows[i]["Version"].ToString());
                             NewJObject.Add("IsDeleted", dt.Rows[i]["IsDeleted"].ToString());
                             NewJObject.Add("CreatedOn", dt.Rows[i]["CreatedOn"].ToString());
                             NewJObject.Add("CreatedByUser", dt.Rows[i]["CreatedByUser"].ToString());
                             NewJObject.Add("ModifiedOn", dt.Rows[i]["ModifiedOn"].ToString());
                             NewJObject.Add("ModifiedByUser", dt.Rows[i]["ModifiedByUser"].ToString());
                             NewJObject.Add("DeletedOn", dt.Rows[i]["DeletedOn"].ToString());
                             NewJObject.Add("DeletedByUser", dt.Rows[i]["DeletedByUser"].ToString());
                             NewJObject.Add("Name", dt.Rows[i]["Name"].ToString());
                             NewJObject.Add("ParentCategoryId", dt.Rows[i]["ParentCategoryId"].ToString());
                             NewJObject.Add("DisplayOrder", dt.Rows[i]["DisplayOrder"].ToString());
                             NewJObject.Add("Macro", dt.Rows[i]["Macro"].ToString());                                                       
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
         public JArray GetHistoryDetailsId(String JS)
         {
             string _constring = ConfigurationManager.AppSettings["BetterCms"];
             dynamic Info = JObject.Parse(JS);
             string mediaId = Info.mediaId;
             using (SqlConnection con = new SqlConnection(_constring))
             {
                 using (SqlCommand cmd = new SqlCommand())
                 {
                     try
                     {
                         cmd.CommandType = CommandType.StoredProcedure;
                         cmd.CommandText = "spGetHistoryDetailsId";
                         cmd.Connection = con;
                         cmd.Parameters.AddWithValue("@MediaId", mediaId);
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
         public JArray GetAllVersionsMediaDetails(String JS)
         {
             string _constring = ConfigurationManager.AppSettings["BetterCms"];
             dynamic Info = JObject.Parse(JS);
             string mediaId = Info.mediaId;
             string originalId = Info.originalId;
             using (SqlConnection con = new SqlConnection(_constring))
             {
                 using (SqlCommand cmd = new SqlCommand())
                 {
                     try
                     {
                         cmd.CommandType = CommandType.StoredProcedure;
                         cmd.CommandText = "spGetAllVersionsMediaDetails";
                         cmd.Connection = con;
                         cmd.Parameters.AddWithValue("@MediaId", mediaId);
                         cmd.Parameters.AddWithValue("@OriginalId", originalId);
                         SqlDataAdapter ad = new SqlDataAdapter(cmd);
                         DataTable dt = new DataTable();
                         ad.Fill(dt);
                         JArray jarrayObj = new JArray();
                         for (int i = 0; i < dt.Rows.Count; i++)
                         {
                             JObject NewJObject = new JObject();
                             if(dt.Rows[i]["Type"].ToString()=="Image")
                             {
                                 NewJObject.Add("Id", dt.Rows[i]["Id"].ToString());
                                 NewJObject.Add("Caption", dt.Rows[i]["Caption"].ToString());
                                 NewJObject.Add("ImageAlign", dt.Rows[i]["ImageAlign"].ToString());
                                 NewJObject.Add("Width", dt.Rows[i]["Width"].ToString());
                                 NewJObject.Add("Height", dt.Rows[i]["Height"].ToString());
                                 NewJObject.Add("CropCoordX1", dt.Rows[i]["CropCoordX1"].ToString());
                                 NewJObject.Add("CropCoordY1", dt.Rows[i]["CropCoordY1"].ToString());
                                 NewJObject.Add("CropCoordX2", dt.Rows[i]["CropCoordX2"].ToString());
                                 NewJObject.Add("CropCoordY2", dt.Rows[i]["CropCoordY2"].ToString());
                                 NewJObject.Add("OriginalWidth", dt.Rows[i]["OriginalWidth"].ToString());
                                 NewJObject.Add("OriginalHeight", dt.Rows[i]["OriginalHeight"].ToString());
                                 NewJObject.Add("OriginalSize", dt.Rows[i]["OriginalSize"].ToString());
                                 NewJObject.Add("OriginalUri", dt.Rows[i]["OriginalUri"].ToString());
                                 NewJObject.Add("PublicOriginallUrl", dt.Rows[i]["PublicOriginallUrl"].ToString());
                                 NewJObject.Add("IsOriginalUploaded", dt.Rows[i]["IsOriginalUploaded"].ToString());
                                 NewJObject.Add("ThumbnailWidth", dt.Rows[i]["ThumbnailWidth"].ToString());
                                 NewJObject.Add("ThumbnailHeight", dt.Rows[i]["ThumbnailHeight"].ToString());
                                 NewJObject.Add("ThumbnailSize", dt.Rows[i]["ThumbnailSize"].ToString());
                                 NewJObject.Add("ThumbnailUri", dt.Rows[i]["ThumbnailUri"].ToString());
                                 NewJObject.Add("PublicThumbnailUrl", dt.Rows[i]["PublicThumbnailUrl"].ToString());
                                 NewJObject.Add("IsThumbnailUploaded", dt.Rows[i]["IsThumbnailUploaded"].ToString());

                                 NewJObject.Add("Version", dt.Rows[i]["Version"].ToString());
                                 NewJObject.Add("IsDeleted", dt.Rows[i]["IsDeleted"].ToString());
                                 NewJObject.Add("CreatedOn", dt.Rows[i]["CreatedOn"].ToString());
                                 NewJObject.Add("CreatedByUser", dt.Rows[i]["CreatedByUser"].ToString());
                                 NewJObject.Add("ModifiedOn", dt.Rows[i]["ModifiedOn"].ToString());
                                 NewJObject.Add("ModifiedByUser", dt.Rows[i]["ModifiedByUser"].ToString());
                                 NewJObject.Add("DeletedOn", dt.Rows[i]["DeletedOn"].ToString());
                                 NewJObject.Add("DeletedByUser", dt.Rows[i]["DeletedByUser"].ToString());
                                 NewJObject.Add("FolderId", dt.Rows[i]["FolderId"].ToString());
                                 NewJObject.Add("Title", dt.Rows[i]["Title"].ToString());
                                 NewJObject.Add("Type", dt.Rows[i]["Type"].ToString());
                                 NewJObject.Add("ContentType", dt.Rows[i]["ContentType"].ToString());
                                 NewJObject.Add("IsArchived", dt.Rows[i]["IsArchived"].ToString());
                                 NewJObject.Add("OriginalId", dt.Rows[i]["OriginalId"].ToString());
                                 NewJObject.Add("PublishedOn", dt.Rows[i]["PublishedOn"].ToString());
                                 NewJObject.Add("ImageId", dt.Rows[i]["ImageId"].ToString());
                                 NewJObject.Add("Description", dt.Rows[i]["Description"].ToString());

                                 NewJObject.Add("OriginalFileName", dt.Rows[i]["OriginalFileName"].ToString());
                                 NewJObject.Add("OriginalFileExtension", dt.Rows[i]["OriginalFileExtension"].ToString());
                                 NewJObject.Add("FileUri", dt.Rows[i]["FileUri"].ToString());
                                 NewJObject.Add("PublicUrl", dt.Rows[i]["PublicUrl"].ToString());
                                 NewJObject.Add("Size", dt.Rows[i]["Size"].ToString());
                                 NewJObject.Add("IsTemporary", dt.Rows[i]["IsTemporary"].ToString());
                                 NewJObject.Add("IsUploaded", dt.Rows[i]["IsUploaded"].ToString());
                                 NewJObject.Add("IsCanceled", dt.Rows[i]["IsCanceled"].ToString());
                                 NewJObject.Add("IsMovedToTrash", dt.Rows[i]["IsMovedToTrash"].ToString());
                                 NewJObject.Add("NextTryToMoveToTrash", dt.Rows[i]["NextTryToMoveToTrash"].ToString());
                                 jarrayObj.Add(NewJObject);
                             }
                             else if (dt.Rows[i]["Type"].ToString() == "File")
                             {
                                 NewJObject.Add("Id", dt.Rows[i]["Id"].ToString());
                                 NewJObject.Add("Version", dt.Rows[i]["Version"].ToString());
                                 NewJObject.Add("IsDeleted", dt.Rows[i]["IsDeleted"].ToString());
                                 NewJObject.Add("CreatedOn", dt.Rows[i]["CreatedOn"].ToString());
                                 NewJObject.Add("CreatedByUser", dt.Rows[i]["CreatedByUser"].ToString());
                                 NewJObject.Add("ModifiedOn", dt.Rows[i]["ModifiedOn"].ToString());
                                 NewJObject.Add("ModifiedByUser", dt.Rows[i]["ModifiedByUser"].ToString());
                                 NewJObject.Add("DeletedOn", dt.Rows[i]["DeletedOn"].ToString());
                                 NewJObject.Add("DeletedByUser", dt.Rows[i]["DeletedByUser"].ToString());
                                 NewJObject.Add("FolderId", dt.Rows[i]["FolderId"].ToString());
                                 NewJObject.Add("Title", dt.Rows[i]["Title"].ToString());
                                 NewJObject.Add("Type", dt.Rows[i]["Type"].ToString());
                                 NewJObject.Add("ContentType", dt.Rows[i]["ContentType"].ToString());
                                 NewJObject.Add("IsArchived", dt.Rows[i]["IsArchived"].ToString());
                                 NewJObject.Add("OriginalId", dt.Rows[i]["OriginalId"].ToString());
                                 NewJObject.Add("PublishedOn", dt.Rows[i]["PublishedOn"].ToString());
                                 NewJObject.Add("ImageId", dt.Rows[i]["ImageId"].ToString());
                                 NewJObject.Add("Description", dt.Rows[i]["Description"].ToString());

                                 NewJObject.Add("OriginalFileName", dt.Rows[i]["OriginalFileName"].ToString());
                                 NewJObject.Add("OriginalFileExtension", dt.Rows[i]["OriginalFileExtension"].ToString());
                                 NewJObject.Add("FileUri", dt.Rows[i]["FileUri"].ToString());
                                 NewJObject.Add("PublicUrl", dt.Rows[i]["PublicUrl"].ToString());
                                 NewJObject.Add("Size", dt.Rows[i]["Size"].ToString());
                                 NewJObject.Add("IsTemporary", dt.Rows[i]["IsTemporary"].ToString());
                                 NewJObject.Add("IsUploaded", dt.Rows[i]["IsUploaded"].ToString());
                                 NewJObject.Add("IsCanceled", dt.Rows[i]["IsCanceled"].ToString());
                                 NewJObject.Add("IsMovedToTrash", dt.Rows[i]["IsMovedToTrash"].ToString());
                                 NewJObject.Add("NextTryToMoveToTrash", dt.Rows[i]["NextTryToMoveToTrash"].ToString());
                                 jarrayObj.Add(NewJObject);
                             }
                             
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
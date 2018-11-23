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
using BetterCms.Module.VTWebAPI.Master;


namespace BetterCms.Module.VTWebAPI.Controllers.Users
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AuthenticationController : ApiController
    {

        [HttpGet]
        public string GetUserDetailsBySocialID(string @SocialId)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            //            using (SqlConnection con = new SqlConnection("database=BetterCmsTestsDataSet;server=.;integrated security=true;"))

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spGetUserByScocialId";
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@SocialId", SocialId.ToString());
                    cmd.Connection = con;
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    if (dt.Rows.Count > 0)
                        return dt.Rows[0]["UserName"].ToString();
                    return null;
                }
            }

        }

        [HttpGet]
        public int InsertUserSocialId(string SocialId, string UserId, string Type)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            //            using (SqlConnection con = new SqlConnection("database=BetterCmsTestsDataSet;server=.;integrated security=true;"))

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spInsertUserSocialId";
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@SocialId", SocialId);
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Connection = con;
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    if (con.State == ConnectionState.Closed)
                        con.Open();
                   return cmd.ExecuteNonQuery();
                }
            }

        }

    }
}

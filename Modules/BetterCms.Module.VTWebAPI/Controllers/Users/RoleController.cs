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
//using BetterCms.Module.VTWebAPI.Master;
//using BetterCms.Module.VTWebAPI.Command.Users;
namespace BetterCms.Module.VTWebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RoleController:ApiController
    {
        [HttpGet]
        public string SaveRole(string id, string name, string description, int version)
        {
             
            string _constring = ConfigurationManager.AppSettings["BetterCms"]; 
//            using (SqlConnection con = new SqlConnection("database=BetterCmsTestsDataSet;server=.;integrated security=true;"))

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spaddnewrole";
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@id", id.ToString());
                    cmd.Parameters.AddWithValue("@version", version);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Connection = con;
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    return dt.Rows[0][0].ToString();
                }
            }

        }

        [HttpGet]
        public int ValidateRoleName(Guid Id, string Name)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring ))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spValidateRoleName";
                        cmd.Parameters.AddWithValue("@Name", Name);
                        cmd.Parameters.AddWithValue("@Id", Id.ToString());
                        cmd.Connection = con;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return Convert.ToInt32(dt.Rows[0][0]);
                    }
                    catch (Exception e)
                    {
                        return 1;
                    }
                }
            }
           
        }

        [HttpGet]
        public int DeleteRole(Guid RoleId)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring ))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                   
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spDeleteRole";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@RoleId", RoleId.ToString());
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return Convert.ToInt32(dt.Rows[0][0]);
                    
                }
            }
        }
    }
}

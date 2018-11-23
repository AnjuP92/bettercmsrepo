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

using BetterCms.Module.VTWebAPI.Master;
namespace BetterCms.Module.VTWebAPI.Controllers.Users
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {


        [HttpGet]
        public string ApiSaveUserwithImage(Guid id, string UserName, string FirstName, string LastName, string Email, string ImageId, string Password, string salt, int version)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            //            using (SqlConnection con = new SqlConnection("database=BetterCmsTestsDataSet;server=.;integrated security=true;"))

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spSaveUser";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id.ToString());
                        cmd.Parameters.AddWithValue("@version", version);
                        cmd.Parameters.AddWithValue("@UserName", UserName);
                        cmd.Parameters.AddWithValue("@FirstName", FirstName);
                        cmd.Parameters.AddWithValue("@LastName", LastName);
                        cmd.Parameters.AddWithValue("@Email", Email);
                        cmd.Parameters.AddWithValue("@Password", Password);
                        Guid ImageGuId = Guid.Parse(ImageId);
                        // cmd.Parameters.AddWithValue("@ImageId", (ImageGuId == null ? (object)DBNull.Value : ImageId));
                        cmd.Parameters.AddWithValue("@ImageId", ImageGuId);
                        cmd.Parameters.AddWithValue("@Salt", salt);

                        cmd.Connection = con;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return dt.Rows[0][0].ToString();
                    }
                    catch (Exception e)
                    {
                        return e.ToString();
                    }

                }
            }

        }

        [HttpGet]
        public string ApiSaveUserwithoutimage(Guid id, string UserName, string FirstName, string LastName, string Email, string Password, string salt, int version)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            //            using (SqlConnection con = new SqlConnection("database=BetterCmsTestsDataSet;server=.;integrated security=true;"))

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spSaveUserwithoutImage";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id.ToString());
                        cmd.Parameters.AddWithValue("@version", version);
                        cmd.Parameters.AddWithValue("@UserName", UserName);
                        cmd.Parameters.AddWithValue("@FirstName", FirstName);
                        cmd.Parameters.AddWithValue("@LastName", LastName);
                        cmd.Parameters.AddWithValue("@Email", Email);
                        cmd.Parameters.AddWithValue("@Password", Password);
                        cmd.Parameters.AddWithValue("@Salt", salt);

                        cmd.Connection = con;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return dt.Rows[0][0].ToString();
                    }
                    catch (Exception e)
                    {
                        return e.ToString();
                    }

                }
            }

        }


        [HttpGet]
        public string ApiUpdateUserwithoutImage(Guid id, string UserName, string FirstName, string LastName, string Email, int version)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            //            using (SqlConnection con = new SqlConnection("database=BetterCmsTestsDataSet;server=.;integrated security=true;"))

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spUpdateUserwithoutImage";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id.ToString());
                        cmd.Parameters.AddWithValue("@version", version);
                        cmd.Parameters.AddWithValue("@UserName", UserName);
                        cmd.Parameters.AddWithValue("@FirstName", FirstName);
                        cmd.Parameters.AddWithValue("@LastName", LastName);
                        cmd.Parameters.AddWithValue("@Email", Email);
                        cmd.Connection = con;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return dt.Rows[0][0].ToString();
                    }
                    catch (Exception e)
                    {
                        return e.ToString();
                    }

                }
            }

        }

        [HttpGet]
        public string ApiUpdateUserwithImage(Guid id, string UserName, string FirstName, string LastName, string Email, string ImageId, int version)
        {

            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            //            using (SqlConnection con = new SqlConnection("database=BetterCmsTestsDataSet;server=.;integrated security=true;"))

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spUpdateUserwithImage";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", id.ToString());
                        cmd.Parameters.AddWithValue("@version", version);
                        cmd.Parameters.AddWithValue("@UserName", UserName);
                        cmd.Parameters.AddWithValue("@FirstName", FirstName);
                        cmd.Parameters.AddWithValue("@LastName", LastName);
                        cmd.Parameters.AddWithValue("@Email", Email);
                        cmd.Parameters.AddWithValue("@ImageId", ImageId);
                        cmd.Connection = con;
                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        return dt.Rows[0][0].ToString();
                    }
                    catch (Exception e)
                    {
                        return e.ToString();
                    }

                }
            }

        }












        [HttpGet]
        public int ApiValidateUserName(Guid id, string UserName)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spValidateUserName ";
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Username", UserName);
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    return Convert.ToInt32(dt.Rows[0][0]);

                }
            }
        }




        [HttpGet]
        public int ApiValidateUserEmail(Guid id, string Email)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spValidateUserEmail ";
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    return Convert.ToInt32(dt.Rows[0][0]);

                }
            }
        }


        [HttpGet]
        public JObject ApiGetUserIdIfvalid(string UserName)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spGetUserIdIfvalid ";
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@UseName", UserName);

                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    dynamic resp = new JObject();
                    resp.Id = dt.Rows[0][0];
                    resp.Password = dt.Rows[0][1];
                    resp.Salt = dt.Rows[0][2];
                    return resp;
                    // return Convert.ToInt32(dt.Rows[0][0]);
                    //var response = 
                    //response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                    //return response;
                }
            }
        }



        [HttpGet]
        public JObject ApiGetImageInfo(Guid ImageId)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetImageInfo ";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ImageId", ImageId);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        dynamic resp = new JObject();
                        resp.Id = dt.Rows[0][0];
                        resp.Title = dt.Rows[0][1];

                        return resp;

                    }
                    catch (Exception e)
                    {
                        return new JObject();
                    }
                    // return Convert.ToInt32(dt.Rows[0][0]);
                    //var response = 
                    //response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                    //return response;
                }
            }
        }

        [HttpGet]
        public JObject ApiGetUserInfo(string UserId)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // cmd.CommandText = "spGetUserInfo ";
                        cmd.CommandText = "spGetUserInfoandroles ";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserId", UserId);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        //DataTable dt = new DataTable();
                        DataSet dt = new DataSet();
                        ad.Fill(dt);
                        dynamic resp = new JObject();

                        JArray jarrayObj = new JArray();
                        // jarrayObj = (JArray)resp["roles"];

                        //var parameterNames = dt.Rows[0][7];
                        //foreach (string parameterName in parameterNames)
                        //{
                        //    jarrayObj.Add(parameterName);
                        //}

                        if (dt.Tables[0].Rows.Count > 0)
                        {
                            resp.Id = dt.Tables[0].Rows[0][0];
                            resp.UserName = dt.Tables[0].Rows[0][1];
                            resp.FirstName = dt.Tables[0].Rows[0][2];
                            resp.LastName = dt.Tables[0].Rows[0][3];
                            resp.Email = dt.Tables[0].Rows[0][4];
                            resp.Password = dt.Tables[0].Rows[0][5];
                            resp.Salt = dt.Tables[0].Rows[0][6];
                            resp.ImageId = dt.Tables[0].Rows[0][7];


                            if (dt.Tables[1].Rows.Count > 0)
                            {
                                for (int count = 0; count < dt.Tables[1].Rows.Count; count++)
                                {

                                    //JObject item = (JObject)jarrayObj[count];
                                    JObject oNew = new JObject();
                                    oNew["role" + count] = dt.Tables[1].Rows[count][0].ToString();
                                    jarrayObj.Add(oNew);

                                    //item.Add(dt.Tables[1].Rows[count][0].ToString());

                                    // jarrayObj.Add(dt.Tables[1].Rows[count][0].ToString());
                                }
                                resp.roles = jarrayObj;
                            }

                        }
                        //if (dt.Rows.Count > 0)
                        //{
                        //    resp.Id = dt.Rows[0][0];
                        //    resp.UserName = dt.Rows[0][1];
                        //    resp.FirstName = dt.Rows[0][2];
                        //    resp.LastName = dt.Rows[0][3];
                        //    resp.Email = dt.Rows[0][4];
                        //    resp.Password = dt.Rows[0][5];
                        //    resp.Salt = dt.Rows[0][6];
                        //    resp.ImageId = dt.Rows[0][7];

                        //    return resp;
                        //}
                        // return Convert.ToInt32(dt.Rows[0][0]);
                        //var response = 
                        //response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                        //return response;
                        return resp;
                    }
                    catch (Exception ex)
                    {
                        return new JObject();
                    }
                }
            }
        }

        [HttpGet]
        public JObject ApiGetRolesById(Guid UserId)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetRolesById ";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserId", UserId);

                        SqlDataAdapter ad = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        ad.Fill(dt);
                        dynamic resp = new JObject();
                        dynamic RoleList = resp;
                        resp.Roles = new JArray() as dynamic;

                        for (int count = 0; count < dt.Rows.Count; count++)
                        {
                            dynamic role = new JObject();
                            resp.Roles.Add(role);
                        }
                        return resp;
                    }
                    catch (Exception e)
                    {
                        return new JObject();
                    }
                    //resp.Id = dt.Rows[0][0];
                    //resp.Password = dt.Rows[0][1];
                    //resp.Salt = dt.Rows[0][2];
                    //return resp;
                    // return Convert.ToInt32(dt.Rows[0][0]);
                    //var response = 
                    //response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                    //return response;
                }
            }
        }



        [HttpGet]
        public string ApiGetRoleId(string Rolename)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spGetRoleId ";
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@RoleName", Rolename);

                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    return dt.Rows[0][0].ToString();

                }
            }
        }



        [HttpGet]
        public string ApiGetUserNameByEmail(string Email)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spGetUserNameByEmail ";
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Email", Email);

                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    return dt.Rows[0][0].ToString();

                }
            }
        }



        [HttpGet]
        public int ApiAddUserRole(Guid UserId, Guid RoleId)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spAddUserRole ";
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@RoleId", RoleId);
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    return Convert.ToInt32(dt.Rows[0][0]);

                }
            }
        }


        [HttpGet]
        public int DeleteRole(Guid UserId)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spDeleteUserRole";
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@UserId", UserId.ToString());
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    return Convert.ToInt32(dt.Rows[0][0]);

                }
            }
        }



        [HttpGet]
        public int DeleteUser(Guid UserId)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];
            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spDeleteUser";
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@UserId", UserId.ToString());
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    ad.Fill(dt);
                    return Convert.ToInt32(dt.Rows[0][0]);

                }
            }
        }

        [HttpGet]
        public JArray PageReadOnlyAccess(string UserName, string Xml)
        {
            string _constring = ConfigurationManager.AppSettings["BetterCms"];

            using (SqlConnection con = new SqlConnection(_constring))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spPageReadOnlyAccess";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserName", UserName);
                        cmd.Parameters.AddWithValue("@xml", Xml);

                        cmd.Connection = con;
                        JArray result = new JArray();
                        using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                ad.Fill(ds);
                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    if (ds.Tables[0].Rows[i]["Level2"].ToString().ToLower().Contains(UserName.ToLower()))
                                    {
                                        result.Add(ds.Tables[0].Rows[i]["PageId"].ToString());
                                    }
                                    else if (ds.Tables[0].Rows[i]["Level3"].ToString().ToLower().Contains(UserName.ToLower())
                                        || (ds.Tables[0].Rows[i]["Level2"].ToString() == "" && ds.Tables[0].Rows[i]["Level3"].ToString() == ""))
                                    {
                                        //result.Add(ds.Tables[0].Rows[i]["PageId"].ToString()); 
                                    }
                                    else if (ds.Tables[1].AsEnumerable()
                                            .Where(x => ds.Tables[0].Rows[i]["Level3"].ToString().ToLower().Contains(x.Field<string>("Name").ToLower())
                                            || ds.Tables[0].Rows[i]["Level3"].ToString().ToLower().Contains("authenticated users")).Count() == 0)
                                    {
                                        result.Add(ds.Tables[0].Rows[i]["PageId"].ToString());
                                    }
                                    //else if (true)
                                    //{
                                    //    int level2count = ds.Tables[1].AsEnumerable()
                                    //        .Where(x => ds.Tables[0].Rows[i]["Level2"].ToString().ToLower().Contains(x.Field<string>("Name").ToLower())
                                    //        || ds.Tables[0].Rows[i]["Level2"].ToString().ToLower().Contains("authenticated users")).Count();
                                    //    int level3count = ds.Tables[1].AsEnumerable()
                                    //        .Where(x => ds.Tables[0].Rows[i]["Level3"].ToString().ToLower().Contains(x.Field<string>("Name").ToLower())
                                    //            || ds.Tables[0].Rows[i]["Level3"].ToString().ToLower().Contains("authenticated users")).Count();
                                    //    if (level2count > 0 && level3count <= 0)
                                    //        result.Add(ds.Tables[0].Rows[i]["PageId"].ToString());
                                    //}
                                }
                                return result;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        return null;
                    }

                }
            }

        }
    }
}
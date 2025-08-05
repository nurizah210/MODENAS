using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;


namespace vms.v1
{
    public partial class Login : System.Web.UI.Page
    {
        public MainController oMainCon = new MainController();
        private GenController oGen = new GenController();

        public static string ORACLE_SALES_CONN = ConfigurationManager.AppSettings["OracleConnection1"];
        OraClientConn conn = new OraClientConn();

        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            AuthenticateUser();
        }


        private void AuthenticateUser()
        {
            if (txtPassword.Text.Length > 0 && txtUsername.Text.Length > 0)
            {
                string empNoFromAD;

                using (DirectoryEntry deDirEntry = new DirectoryEntry("LDAP://" + ConfigurationManager.AppSettings["ADServer"],
                        txtUsername.Text, txtPassword.Text, AuthenticationTypes.Secure))
                {
                    try
                    {
                        DirectoryEntry de = GetUser(txtUsername.Text, out empNoFromAD);

                        if (de != null && !string.IsNullOrEmpty(empNoFromAD))
                        {
                            string connStr = ConfigurationManager.AppSettings["ConnectionString"];
                            using (OracleConnection oracleConn = new OracleConnection(connStr))
                            {
                                oracleConn.Open();

                                string query = "SELECT EMP_NAME, DEPT, SECT, EMP_POSITION, ROLE FROM VIS_STAFF WHERE EMP_NO = :EMP_NO";

                                using (OracleCommand cmd = new OracleCommand(query, oracleConn))
                                {
                                    cmd.Parameters.Add(new OracleParameter("EMP_NO", empNoFromAD));

                                    using (OracleDataReader reader = cmd.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            Session["username"] = txtUsername.Text.ToUpper();
                                            Session["EmpID"] = empNoFromAD;
                                            Session["fullname"] = reader["EMP_NAME"].ToString();
                                            Session["department"] = reader["DEPT"].ToString();
                                            Session["section"] = reader["SECT"].ToString();
                                            Session["position"] = reader["EMP_POSITION"].ToString();
                                            Session["role"] = reader["ROLE"].ToString();
                                            Session["adPassword"] = txtPassword.Text;

                                            if (reader["EMP_POSITION"].ToString().ToUpper() == "MANAGER")
                                            {
                                                string hodEmail = de.Properties["mail"].Value?.ToString();
                                                if (!string.IsNullOrEmpty(hodEmail))
                                                {
                                                    Session["email"] = hodEmail.Trim().ToLower(); // use this later to filter VIS_EXITSTAFF
                                                }
                                            }
                                            Response.Redirect("Dashboard.aspx");
                                        }
                                        else
                                        {
                                            lblErrorLogin.Text = "Employee not found in VIS_STAFF table.";
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            lblErrorLogin.Text = "Authentication failed or EMP_NO not found in AD.";
                        }
                    }
                    catch (Exception ex)
                    {
                        oGen.WriteToLogFile("MainController-AuthenticateUser: " + ex.ToString());
                        lblErrorLogin.Text = "Login error: " + ex.Message;
                    }
                }
            }
            else
            {
                lblErrorLogin.Text = "Please fill in the username and password.";
            }
        }

        private DirectoryEntry GetUser(string userName, out string empNoFromAD)
        {
            empNoFromAD = "";

            try
            {
                DirectoryEntry de = GetDirectoryObject();
                DirectorySearcher deSearch = new DirectorySearcher(de)
                {
                    Filter = "(&(objectClass=user)(SAMAccountName=" + userName + "))"
                };

                deSearch.PropertiesToLoad.Add("company"); // <-- EMP_NO from AD

                SearchResult result = deSearch.FindOne();

                if (result != null)
                {
                    DirectoryEntry userEntry = new DirectoryEntry(result.Path, txtUsername.Text, txtPassword.Text, AuthenticationTypes.Secure);

                    if (result.Properties["company"].Count > 0)
                    {
                        empNoFromAD = result.Properties["company"][0].ToString(); // 👈 EMP_NO
                    }

                    return userEntry;
                }
            }
            catch
            {
                // log if needed
            }

            return null;
        }

        private DirectoryEntry GetDirectoryObject()
        {
            DirectoryEntry oDE;
            oDE = new DirectoryEntry("LDAP://" + ConfigurationManager.AppSettings["ADServer"], txtUsername.Text, txtPassword.Text, AuthenticationTypes.Secure);
            return oDE;
        }

     
           

            
              
        private void RedirectByGroup()
        {
            //string tmpAccGroup = "";
            String connStr = ConfigurationManager.AppSettings["scosConnAdmin"];

            using (OleDbConnection cn = new OleDbConnection(connStr))
            {
                try
                {
                    cn.Open();
                    string sqlStr = "SELECT * " + "FROM scos_user " + "WHERE USR_USERNAME = '" + Session["username"] + "'";

                    using (OleDbCommand command = new OleDbCommand(sqlStr, cn))
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["ACS_GROUP"].ToString() == "1" || reader["ACS_GROUP"].ToString() == "ST" || reader["LOCATION"].ToString() == "")
                                {
                                    Response.Redirect("Homepage.aspx");
                                    //Response.Redirect("Location.aspx");
                                }

                                else if (reader["ACS_GROUP"].ToString() == "4" || reader["ACS_GROUP"].ToString() == "5")
                                {
                                    Response.Redirect("Homepage.aspx");
                                }

                                else if (reader["ACS_GROUP"].ToString() != "" && reader["LOCATION"].ToString() != "")
                                {
                                    Response.Redirect("Homepage.aspx");
                                }

                                else
                                {
                                    lblErrorLogin.Text = "Unauthorized access! Please contact your system administrator.";
                                }


                            }
                        }
                    }
                }

                catch (OleDbException e)
                {
                    string errorMessage = "Code: " + e.ErrorCode + "<br />" + "Message: " + e.Message;
                    oGen.WriteToLogFile("MainController-RedirectByGroup: " + errorMessage.ToString());

                    lblErrorLogin.Text = "An exception occurred. Please contact your system administrator. <br />" + errorMessage;
                }
            }

            //Session["ACS_GROUP"] = tmpAccGroup;
            //Response.Redirect("Location.aspx");


        }

        protected void btnReset_Click(object sender, EventArgs e)
        {

            txtUsername.Text = "";
            txtPassword.Text = "";
            lblErrorLogin.Text = "";

        }


    }
}
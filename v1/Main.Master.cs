using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data;


namespace vms.v1
{
    public partial class Main : System.Web.UI.MasterPage
    {
        private string connStr = ConfigurationManager.AppSettings["ConnectionString"];
        public string tmpUsername = "";
        public string year = "";
        public string location = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime myDateTime = DateTime.Now;
            year = myDateTime.Year.ToString();



            if (Session["username"] != null)
            {
                tmpUsername = Session["username"].ToString();
                //location = Session["sc_code"].ToString(); 

            }
            else
            {
                Session["username"] = null;
                tmpUsername = "";
                Response.Redirect("Login.aspx");
            }
            if (!IsPostBack)
            {
                LoadNotifications();
            }
        }

        
        private void LoadNotifications()
        {
            var notifications = CheckOverstayedVisitors(connStr);
            int pendingExitRequests = GetPendingExitRequests(connStr);

            if (pendingExitRequests > 0)
            {
                notifications.Insert(0, $"{pendingExitRequests} Exit Request(s) pending approval");
            }

            // Bind to Repeater
            DataTable dt = new DataTable();
            dt.Columns.Add("Message");

            foreach (var note in notifications)
            {
                dt.Rows.Add(note);
            }

            rptNotifications.DataSource = dt;
            rptNotifications.DataBind();

            badgeNotif.Visible = dt.Rows.Count > 0;
        }
        private List<string> CheckOverstayedVisitors(string connString)
        {
            List<string> notifications = new List<string>();

            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                string query = @"
            SELECT 'VEHICLE' AS CATEGORY, PLATE_NO AS IDENTIFIER, TIME_IN, SYSDATE AS CURRENT_TIME,
                   ROUND((SYSDATE - CAST(TIME_IN AS DATE)) * 24, 2) AS DURATION_HOURS
            FROM VIS_VEHICLE
            WHERE TIME_OUT IS NULL AND (SYSDATE - CAST(TIME_IN AS DATE)) * 24 > 1
            UNION ALL
            SELECT 'TRANSPORTER', NO_PLATE, TIME_IN, SYSDATE, ROUND((SYSDATE - CAST(TIME_IN AS DATE)) * 24, 2)
            FROM VIS_PARKING
            WHERE TIME_OUT IS NULL AND (SYSDATE - CAST(TIME_IN AS DATE)) * 24 > 8
            UNION ALL
            SELECT 'TNB', NO_PLATE, TIME_IN, SYSDATE, ROUND((SYSDATE - CAST(TIME_IN AS DATE)) * 24, 2)
            FROM VIS_TNB
            WHERE TIME_OUT IS NULL AND (SYSDATE - CAST(TIME_IN AS DATE)) * 24 > 2
            UNION ALL
            SELECT 'VISITOR', NAME, REGISTER_DATE, SYSDATE, ROUND((SYSDATE - CAST(REGISTER_DATE AS DATE)) * 24, 2)
            FROM VIS_VISITOR
            WHERE TIME_OUT IS NULL AND (SYSDATE - CAST(REGISTER_DATE AS DATE)) * 24 > 2
            UNION ALL
            SELECT 'CONTAINER', PLATE_NO, REGISTER_DATE, SYSDATE, ROUND((SYSDATE - CAST(REGISTER_DATE AS DATE)) * 24, 2)
            FROM VIS_CONTAINER
            WHERE TIME_OUT IS NULL AND (SYSDATE - CAST(REGISTER_DATE AS DATE)) * 24 > 3";

                using (OracleCommand cmd = new OracleCommand(query, conn))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string category = reader.GetString(0);
                        string id = reader.GetString(1);
                        DateTime timeIn = reader.GetDateTime(2);
                        double duration = reader.GetDouble(4);

                        notifications.Add($"{category} '{id}' overstayed {duration:F1} hrs since {timeIn:dd MMM HH:mm}");
                    }
                }
            }

            return notifications;
        }
        private int GetPendingExitRequests(string connString)
        {
            int count = 0;

            string hodEmail = Session["email"]?.ToString(); // HOD's email from session

            if (string.IsNullOrEmpty(hodEmail))
                return 0; // No email, no access

            using (OracleConnection conn = new OracleConnection(connString))
            {
                string query = @"
            SELECT COUNT(*) 
            FROM VIS_EXITSTAFF 
            WHERE APPROVAL_STATUS = 'Pending' 
              AND HODNAME = :HODEmail";

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(":HODEmail", OracleDbType.Varchar2).Value = hodEmail;

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        count = Convert.ToInt32(result);
                    }
                }
            }

            return count;
        }


    }
}
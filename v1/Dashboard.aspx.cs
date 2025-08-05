using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace vms.v1
{
    public partial class Dashboard : Page
    {
        private string connStr = ConfigurationManager.AppSettings["ConnectionString"];
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTodayVisitorCount();
                LoadTotalExit();
                LoadUpcomingSchedule();

                string fullName = Session["fullname"] as string;
                string department = Session["DEPARTMENT"]?.ToString() ?? "";


                if (Session["isGuard"] != null && (bool)Session["isGuard"])
                {
                    lblWelcome.Text = "Welcome, Guard from " + Session["guardPost"];
                }
                else if (!string.IsNullOrEmpty(fullName))
                {
                    lblWelcome.Text = $"Welcome back, {fullName}!";
                }
                else
                {
                    lblWelcome.Text = "Welcome to the dashboard!";
                }
            }
        }

        protected void btnSubmitExit_Click(object sender, EventArgs e)
        {
            string department = Session["department"]?.ToString()?.ToUpper() ?? "";

            // Office-based departments go to LeaveRequest2.aspx
            string[] officeBasedDepartments = {
        "HCD", "IT", "FN", "PROC", "BD", "EX", "EV", "CEO OFFICE"
    };

            if (officeBasedDepartments.Contains(department))
            {
                Response.Redirect("LeaveRequest2.aspx");
            }
            else
            {
                Response.Redirect("LeaveRequest.aspx");
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            string position = Session["position"]?.ToString()?.ToUpper(); 

            if (position == "MANAGER" || position == "SENIOR MANAGER")
            {
                Response.Redirect("HODApproval.aspx");
            }
            else
            {
               
            }
        }


        protected void btnVisitor_Click(object sender, EventArgs e)
        {
            // Your logic here
            // Example:
            Response.Write("Submit Exit button clicked!");
        }
        private void LoadTodayVisitorCount()
        {

            string query = @"
             SELECT (
      (SELECT COUNT(*) FROM VIS_VEHICLE 
       WHERE TIME_IN >= TRUNC(SYSDATE) AND TIME_IN < TRUNC(SYSDATE+1))
    + (SELECT COUNT(*) FROM VIS_CONTAINER 
       WHERE REGISTER_DATE >= TRUNC(SYSDATE) AND REGISTER_DATE < TRUNC(SYSDATE+1))
    + (SELECT COUNT(*) FROM VIS_TNB
       WHERE TIME_IN >= TRUNC(SYSDATE) AND TIME_IN < TRUNC(SYSDATE+1))
    + (SELECT COUNT(*) FROM VIS_PARKING 
       WHERE TIME_IN >= TRUNC(SYSDATE) AND TIME_IN < TRUNC(SYSDATE +1))
  ) AS TOTAL_VISITOR FROM DUAL";

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand(query, conn);
                    object result = cmd.ExecuteScalar();
                    lblTodayVisitors.Text = result != null ? result.ToString() : "0";
                }
                catch (Exception ex)
                {
                    // Log error
                    lblTodayVisitors.Text = "Err";
                }
            }
        }
        private void LoadTotalExit()
        {
            string staffName = Session["fullname"]?.ToString(); // comes from AD displayName
            if (string.IsNullOrEmpty(staffName))
            {
                lblTotalExit.Text = "-";
                return;
            }


            string query = @"
        SELECT COUNT(*) AS TOTAL_EXIT
        FROM VIS_EXITSTAFF
        WHERE STAFF_NAME = :StaffName
          AND APPROVAL_STATUS = :Status";

            using (OracleConnection conn = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Parameters.Add("StaffName", staffName);
                cmd.Parameters.Add("Status", "Approved");

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    lblTotalExit.Text = result?.ToString() ?? "0";
                }
                catch (Exception ex)
                {
                 
                }
            }
        }
        private void LoadUpcomingSchedule()
        {
            string query = @"
              SELECT 
  STAFF_NAME,
  DEPARTMENT,
  TO_DATE(TO_CHAR(DATE_OUT, 'YYYY-MM-DD') || ' ' || TIME_OUT, 'YYYY-MM-DD HH24:MI') AS FULL_DATETIME
FROM VIS_EXITSTAFF
WHERE DATE_OUT = TRUNC(SYSDATE)  AND APPROVAL_STATUS = 'Approved'
  AND REGEXP_LIKE(TIME_OUT, '^\d{2}:\d{2}$') -- Safety check for format HH24:MI
ORDER BY FULL_DATETIME
FETCH FIRST 5 ROWS ONLY";

            DataTable dt = new DataTable();
            using (OracleConnection conn = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand(query, conn))
            using (OracleDataAdapter da = new OracleDataAdapter(cmd))
            {
                conn.Open();
                
                
                da.Fill(dt);
            }

            List<string> scheduleList = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                string name = row["STAFF_NAME"].ToString();
                string department = row["DEPARTMENT"].ToString();
                DateTime fullTime = Convert.ToDateTime(row["FULL_DATETIME"]);

                string sentence = $"{fullTime:dd MMM yyyy hh:mm tt} – {name} from {department} is scheduled to exit";
                scheduleList.Add(sentence);
            }

            rptSchedule.DataSource = scheduleList;
            rptSchedule.DataBind();
        }

    }
}

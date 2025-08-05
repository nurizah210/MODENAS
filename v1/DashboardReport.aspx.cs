using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Math;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;



namespace vms.v1
{
    public partial class DashboardReport : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.AppSettings["ConnectionString"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtReportDate.Text = GetLastCompletedShiftDate().ToString("yyyy-MM-dd");
                txtAbnormDate.Text = GetLastCompletedShiftDate().ToString("yyyy-MM-dd");
                txtClockingDate.Text = GetLastCompletedShiftDate().ToString("yyyy-MM-dd");
                ddlChartCategory.SelectedValue = "Visitor";
                BindMonthRanges();
                UpdateChartData();
            }

            LoadAttendancePercentage();
            lblClockingStatus.Text = GetClockingStatus();
            LoadCCTVStatus();
            LoadVisitor();
            ViewState["ActiveModal"] = null;



        }
        protected void Page_PreRender(object sender, EventArgs e)
        {

            LoadChartData("Visitor", ddlMonthRange.SelectedValue);


            if (IsPostBack && ViewState["ActiveModal"] != null)
            {

                string activeModal = ViewState["ActiveModal"].ToString();
               
                string script = "$('.modal').modal('hide');"; // Hide any open modal first

                if (activeModal == "Attendance")
                    script += "$('#attendanceModal').modal('show');";
                else if (activeModal == "CCTV")
                    script += "$('#cctvModal').modal('show');";
                else if (activeModal == "Clocking")
                    script += "$('#clockingModal').modal('show');";


                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModalScript", script, true);
            }
        }
        private int GetStaffCount(string condition, bool onlyToday = true)
        {
            int count = 0;
            string query = $"SELECT COUNT(*) FROM VIS_EXITSTAFF WHERE {condition}";

            if (onlyToday)
            {
                query += " AND DATE_OUT >= TRUNC(SYSDATE)";
            }

            using (OracleConnection conn = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        count = Convert.ToInt32(result);
                    }
                }
                catch (Exception)
                {
                    // Optional: log or handle the error
                }
            }

            return count;
        }
        private int GetTotalVisitors(string condition, bool onlyToday = true)
        {
            int totalVisitors = 0;

            // Date filter suffix
            string dateFilter = onlyToday ? " AND REGISTER_DATE >= TRUNC(SYSDATE)" : "";
            string dateFilter2 = onlyToday ? " AND TIME_IN >= TRUNC(SYSDATE)" : "";

            string query = $@"
        SELECT 
            (SELECT COUNT(*) FROM VIS_VEHICLE WHERE {condition}{dateFilter2}) +
            (SELECT COUNT(*) FROM VIS_PARKING WHERE {condition}{dateFilter2}) +
            (SELECT COUNT(*) FROM VIS_TNB WHERE {condition}{dateFilter2}) +
            (SELECT COUNT(*) FROM VIS_VISITOR WHERE {condition}{dateFilter}) +
             (SELECT COUNT(*) FROM VIS_CONTAINER WHERE {condition}{dateFilter}) +
            (SELECT COUNT(*) FROM VIS_STAFFMOVE WHERE {condition}{dateFilter2}) AS TotalVisitors 
        FROM DUAL";

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            totalVisitors = Convert.ToInt32(result);
                        }
                    }
                    catch
                    {
                        // Optional: log error
                    }
                }
            }

            return totalVisitors;
        }

        private void BindMonthRanges()
        {
            ddlMonthRange.Items.Clear();

            HashSet<int> years = new HashSet<int>();

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();

                
                using (OracleCommand cmd = new OracleCommand(@"
            SELECT DISTINCT TO_CHAR(REPORT_DATE, 'YYYY') AS Year 
            FROM SECURITY_REPORT 
            ORDER BY Year", conn))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int year = int.Parse(reader["Year"].ToString());
                        years.Add(year);
                    }
                }

                UpdateChartData();
            }

          
            foreach (int year in years.OrderBy(y => y))
            {
                ddlMonthRange.Items.Add(new ListItem($"Jan–Jun {year}", $"{year}-01:{year}-06"));
                ddlMonthRange.Items.Add(new ListItem($"Jul–Dec {year}", $"{year}-07:{year}-12"));
            }

            if (ddlMonthRange.Items.Count > 0)
                ddlMonthRange.SelectedIndex = 0; 
        }
        private DateTime GetLastCompletedShiftDate()
        {
           
            int nowTime = int.Parse(DateTime.Now.ToString("HHmm"));

            if (nowTime >= 800 && nowTime <= 1959)
            {
            
                return DateTime.Today.AddDays(-1);
            }

            else
            {
               
                return DateTime.Today;
            }
        }

        private void LoadAttendancePercentage()
        {
            string query = @"
 WITH LastCompletedShift AS (SELECT CASE WHEN TO_CHAR(SYSDATE, 'HH24MI') BETWEEN '0800' AND '1959' THEN TRUNC(SYSDATE - 1) ELSE TRUNC(SYSDATE) END AS REPORT_DATE,
                CASE WHEN TO_CHAR(SYSDATE, 'HH24MI') BETWEEN '0800' AND '1959' THEN '2000-0800'
                ELSE '0800-2000'
                END AS SHIFT_TIME FROM DUAL) SELECT ROUND(CASE WHEN lcs.SHIFT_TIME='2000-0800' THEN ((CASE WHEN sr.EMOS_GUARD1 IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN sr.EMOS_GUARD2 IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN sr.POST1_GUARD1 IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN sr.POST1_GUARD2 IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN sr.POST2_GUARD1 IS NOT NULL THEN 1 ELSE 0 END)) * 100 / 5 ELSE ((CASE WHEN sr.POST1_GUARD1 IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN sr.POST1_GUARD2 IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN sr.POST2_GUARD1 IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN sr.POST2_GUARD2 IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN sr.POST2_GUARD3 IS NOT NULL THEN 1 ELSE 0 END)) * 100 / 5 END) AS ATTENDANCE_PERCENTAGE FROM SECURITY_REPORT sr JOIN LastCompletedShift lcs ON sr.REPORT_DATE=lcs.REPORT_DATE AND sr.SHIFT_TIME=lcs.SHIFT_TIME";


            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand(query, conn);
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        lblAttendanceRate.Text = result.ToString() + "%";
                    }

                    else
                    {
                        lblAttendanceRate.Text = "0%";
                    }
                }

                catch (Exception)
                {
                    lblAttendanceRate.Text = "Error";
                }
            }
        }

        protected void btnAttendance_Click(object sender, EventArgs e)
        {
            ViewState["ActiveModal"] = null;  // Clear previous modal flag
            ViewState["ActiveModal"] = "Attendance";

            List<(string GuardName, DateTime ShiftDate, string ShiftTime)> guards = new List<(string, DateTime, string)>();
            string connStr = ConfigurationManager.AppSettings["ConnectionString"];

            DateTime reportDate;
            bool isUserInput = DateTime.TryParse(txtReportDate.Text, out reportDate);

            string query;

            if (!isUserInput)
            {
                // Auto mode: same logic as before to get latest completed shift
                query = @"
WITH LastCompletedShift AS (
    SELECT 
        CASE
            WHEN TO_CHAR(SYSDATE, 'HH24MI') BETWEEN '0000' AND '0759' THEN TRUNC(SYSDATE - 1)
            WHEN TO_CHAR(SYSDATE, 'HH24MI') BETWEEN '0800' AND '1959' THEN TRUNC(SYSDATE - 1)
            ELSE TRUNC(SYSDATE)
        END AS REPORT_DATE,
        CASE
            WHEN TO_CHAR(SYSDATE, 'HH24MI') BETWEEN '0000' AND '0759' THEN '2000-0800'
            WHEN TO_CHAR(SYSDATE, 'HH24MI') BETWEEN '0800' AND '1959' THEN '2000-0800'
            ELSE '0800-2000'
        END AS SHIFT_TIME
    FROM DUAL
)
SELECT DISTINCT 
    TRIM(GUARD_NAME) AS GUARD_NAME,
    lcs.REPORT_DATE,
    lcs.SHIFT_TIME
FROM (
    SELECT POST1_GUARD1 AS GUARD_NAME FROM SECURITY_REPORT sr JOIN LastCompletedShift lcs ON sr.REPORT_DATE = lcs.REPORT_DATE AND sr.SHIFT_TIME = lcs.SHIFT_TIME
    UNION
    SELECT POST1_GUARD2 FROM SECURITY_REPORT sr JOIN LastCompletedShift lcs ON sr.REPORT_DATE = lcs.REPORT_DATE AND sr.SHIFT_TIME = lcs.SHIFT_TIME
    UNION
    SELECT POST2_GUARD1 FROM SECURITY_REPORT sr JOIN LastCompletedShift lcs ON sr.REPORT_DATE = lcs.REPORT_DATE AND sr.SHIFT_TIME = lcs.SHIFT_TIME
    UNION
    SELECT POST2_GUARD2 FROM SECURITY_REPORT sr JOIN LastCompletedShift lcs ON sr.REPORT_DATE = lcs.REPORT_DATE AND sr.SHIFT_TIME = lcs.SHIFT_TIME
    UNION
    SELECT POST2_GUARD3 FROM SECURITY_REPORT sr JOIN LastCompletedShift lcs ON sr.REPORT_DATE = lcs.REPORT_DATE AND sr.SHIFT_TIME = lcs.SHIFT_TIME
    UNION
    SELECT EMOS_GUARD1 FROM SECURITY_REPORT sr JOIN LastCompletedShift lcs ON sr.REPORT_DATE = lcs.REPORT_DATE AND sr.SHIFT_TIME = lcs.SHIFT_TIME
    UNION
    SELECT EMOS_GUARD2 FROM SECURITY_REPORT sr JOIN LastCompletedShift lcs ON sr.REPORT_DATE = lcs.REPORT_DATE AND sr.SHIFT_TIME = lcs.SHIFT_TIME
) 
WHERE GUARD_NAME IS NOT NULL 
ORDER BY GUARD_NAME";
            }
            else
            {
                // User mode: show both shifts for the selected date
                query = @"
SELECT DISTINCT
    TRIM(GUARD_NAME) AS GUARD_NAME,
    REPORT_DATE,
    SHIFT_TIME
FROM (
    SELECT POST1_GUARD1 AS GUARD_NAME, REPORT_DATE, SHIFT_TIME FROM SECURITY_REPORT WHERE REPORT_DATE = :ReportDate AND SHIFT_TIME IN ('0800-2000','2000-0800')
    UNION
    SELECT POST1_GUARD2, REPORT_DATE, SHIFT_TIME FROM SECURITY_REPORT WHERE REPORT_DATE = :ReportDate AND SHIFT_TIME IN ('0800-2000','2000-0800')
    UNION
    SELECT POST2_GUARD1, REPORT_DATE, SHIFT_TIME FROM SECURITY_REPORT WHERE REPORT_DATE = :ReportDate AND SHIFT_TIME IN ('0800-2000','2000-0800')
    UNION
    SELECT POST2_GUARD2, REPORT_DATE, SHIFT_TIME FROM SECURITY_REPORT WHERE REPORT_DATE = :ReportDate AND SHIFT_TIME IN ('0800-2000','2000-0800')
    UNION
    SELECT POST2_GUARD3, REPORT_DATE, SHIFT_TIME FROM SECURITY_REPORT WHERE REPORT_DATE = :ReportDate AND SHIFT_TIME IN ('0800-2000','2000-0800')
    UNION
    SELECT EMOS_GUARD1, REPORT_DATE, SHIFT_TIME FROM SECURITY_REPORT WHERE REPORT_DATE = :ReportDate AND SHIFT_TIME IN ('0800-2000','2000-0800')
    UNION
    SELECT EMOS_GUARD2, REPORT_DATE, SHIFT_TIME FROM SECURITY_REPORT WHERE REPORT_DATE = :ReportDate AND SHIFT_TIME IN ('0800-2000','2000-0800')
)
WHERE GUARD_NAME IS NOT NULL
ORDER BY SHIFT_TIME, GUARD_NAME";

            }

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    if (isUserInput)
                    {
                        cmd.Parameters.Add(new OracleParameter("ReportDate", reportDate.Date));
                    }

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string guardName = reader["GUARD_NAME"].ToString();
                            DateTime shiftDate = Convert.ToDateTime(reader["REPORT_DATE"]);
                            string shiftTime = reader["SHIFT_TIME"].ToString();

                            guards.Add((guardName, shiftDate, shiftTime));
                        }
                    }
                }

                if (guards.Count > 0)
                {
                    var grouped = guards.GroupBy(g => new { g.ShiftDate, g.ShiftTime })
                                        .OrderBy(g => g.Key.ShiftDate)
                                        .ThenBy(g => g.Key.ShiftTime);

                    StringBuilder htmlBuilder = new StringBuilder();

                    htmlBuilder.Append("<div style='font-family: Arial, sans-serif; color: #333;'>");

                    foreach (var group in grouped)
                    {
                        htmlBuilder.AppendFormat(
       "<h6 style='border-bottom: 1.5px solid #6c757d; padding-bottom: 3px; margin-top: 12px; font-weight: 400; color: #2c3e50; font-style: italic; font-size: 0.85rem; '>" +
       " {1}</h6>",
       group.Key.ShiftDate.ToString("dd/MM/yyyy"),
       group.Key.ShiftTime);

                        htmlBuilder.Append("<ul class='list-group mb-3' style='max-width: 350px;'>");
                        foreach (var guard in group)
                        {
                            htmlBuilder.AppendFormat("<li class='list-group-item' style='font-size: 0.95rem; padding: 8px 15px; border-radius: 5px; color: #444;'>{0}</li>",
                                HttpUtility.HtmlEncode(guard.GuardName));
                        }
                        htmlBuilder.Append("</ul>");
                    }

                    htmlBuilder.Append("</div>");

                    attendanceList.Text = htmlBuilder.ToString();
                    UpdateChartData();
                }
                else
                {
                    attendanceList.Text = "<p style='font-family: Arial, sans-serif; color: #888; font-style: italic;'>No guards found for the selected date.</p>";
                    UpdateChartData();
                }


               
            }
            
        }


        protected void txtReportDate_TextChanged(object sender, EventArgs e)
        {
            TextBox source = sender as TextBox;

            if (source != null)
            {
                if (source.ID == "txtReportDate")
                {
                    ViewState["ActiveModal"] = "Attendance";
                    btnAttendance_Click(sender, e); // Only call attendance logic
                }
                else if (source.ID == "txtAbnormDate")
                {
                    ViewState["ActiveModal"] = "CCTV";
                    btnCCTV_Click(sender, e); // Only call CCTV logic
                }
                else if (source.ID == "txtClockingDate")
                {
                    ViewState["ActiveModal"] = "Clocking";
                    btnClocking_Click(sender, e); // Create this method
                }
            }
        }





        private void LoadCCTVStatus()
        {
            string query = @"
 WITH LastCompletedShift AS (SELECT CASE WHEN TO_CHAR(SYSDATE, 'HH24MI') BETWEEN '0800' AND '1959' THEN TRUNC(SYSDATE - 1) ELSE TRUNC(SYSDATE) END AS REPORT_DATE,
                CASE WHEN TO_CHAR(SYSDATE, 'HH24MI') BETWEEN '0800' AND '1959' THEN '2000-0800'
                ELSE '0800-2000'
                END AS SHIFT_TIME FROM DUAL) SELECT SUM(TO_NUMBER(CCTV_WORKING)) AS TOTAL_WORKING,
            SUM(TO_NUMBER(CCTV_OFFLINE)) AS TOTAL_OFFLINE FROM SECURITY_REPORT JOIN LastCompletedShift ON SECURITY_REPORT.REPORT_DATE=LastCompletedShift.REPORT_DATE AND SECURITY_REPORT.SHIFT_TIME=LastCompletedShift.SHIFT_TIME ";


            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand(query, conn);

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblCCTVOnline.Text = reader["TOTAL_WORKING"] != DBNull.Value ? reader["TOTAL_WORKING"].ToString() : "0";
                            lblCCTVOffline.Text = reader["TOTAL_OFFLINE"] != DBNull.Value ? reader["TOTAL_OFFLINE"].ToString() : "0";
                        }
                    }
                }

                catch (Exception)
                {
                    lblCCTVOnline.Text = "Error";
                    lblCCTVOffline.Text = "Error";
                }
            }
        }

        protected void btnCCTV_Click(object sender, EventArgs e)
        {
            ViewState["ActiveModal"] = null;  // Clear previous modal flag
            ViewState["ActiveModal"] = "CCTV";
            List<string> CCTVAbnorm = new List<string>();

            DateTime reportDate;
            bool isUserInput = DateTime.TryParse(txtAbnormDate.Text, out reportDate);
            if (!DateTime.TryParse(txtAbnormDate.Text, out reportDate))
            {
                reportDate = DateTime.Today;
            }

            string connStr = ConfigurationManager.AppSettings["ConnectionString"];
            string query = @"WITH LastCompletedShift AS(
    SELECT
        CASE
            WHEN TO_CHAR(SYSDATE, 'HH24MI') BETWEEN '0000' AND '0759' THEN TRUNC(SYSDATE -1)
            WHEN TO_CHAR(SYSDATE, 'HH24MI') BETWEEN '0800' AND '1959' THEN TRUNC(SYSDATE -1)
            ELSE TRUNC(SYSDATE)
        END AS REPORT_DATE,
        CASE
            WHEN TO_CHAR(SYSDATE, 'HH24MI') BETWEEN '0000' AND '0759' THEN '2000-0800'
            WHEN TO_CHAR(SYSDATE, 'HH24MI') BETWEEN '0800' AND '1959' THEN '2000-0800'
            ELSE '0800-2000'
        END AS SHIFT_TIME
    FROM DUAL
)
SELECT s.CCTVABNORM, lcs.REPORT_DATE, lcs.SHIFT_TIME
FROM SECURITY_REPORT s
JOIN LastCompletedShift lcs
  ON s.REPORT_DATE = TRUNC(lcs.REPORT_DATE)
 AND s.SHIFT_TIME = lcs.SHIFT_TIME
WHERE s.CCTVABNORM IS NOT NULL
  AND TRIM(s.CCTVABNORM) IS NOT NULL"
;

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    if (isUserInput)
                    {
                        cmd.Parameters.Add(new OracleParameter("ReportDate", reportDate.Date));
                    }

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime shiftDate = Convert.ToDateTime(reader["REPORT_DATE"]);
                            CCTVAbnorm.Add(reader["CCTVABNORM"].ToString());
                            string shiftTime = reader["SHIFT_TIME"].ToString();

                        }
                    }
                }
            }

            litCCTVAbnorm.Text = CCTVAbnorm.Count > 0 ? string.Join("<br/>", CCTVAbnorm) : "<p style='font-family: Arial, sans-serif; color: #888; font-style: italic;'>No offline CCTV detected.</p>";

            
            UpdateChartData();
            
        }


        private void LoadVisitor()
        {
            string query = @"
SELECT 
  (SELECT COUNT(*) FROM VIS_VEHICLE 
   WHERE TIME_IN >= TRUNC(SYSDATE - 1) + INTERVAL '8' HOUR 
     AND TIME_IN < TRUNC(SYSDATE)) AS VEHICLE_VISITOR_COUNT,

  (SELECT COUNT(*) FROM VIS_CONTAINER 
   WHERE REGISTER_DATE >= TRUNC(SYSDATE - 1) + INTERVAL '8' HOUR 
     AND REGISTER_DATE < TRUNC(SYSDATE)) AS CONTAINER_COUNT,

  (SELECT COUNT(*) FROM VIS_PARKING 
   WHERE TIME_IN >= TRUNC(SYSDATE - 1) + INTERVAL '8' HOUR 
     AND TIME_IN < TRUNC(SYSDATE)) AS TRANSPORTER_COUNT
FROM DUAL";



            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(query, conn);

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lblTransporter.Text = reader["TRANSPORTER_COUNT"].ToString();
                        lblContainer.Text = reader["CONTAINER_COUNT"].ToString();
                        lblVehicleVisitor.Text = reader["VEHICLE_VISITOR_COUNT"].ToString();
                    }
                }
            }
        }

        private string GetClockingStatus()
        {
            string clockingStatus = "-/21";

            try
            {
                DateTime now = DateTime.Now;
                DateTime shiftStart, shiftEnd;

                // Define night shift time boundaries
                TimeSpan shiftStartTime = new TimeSpan(20, 0, 0); // 20:00 (8 PM)
                TimeSpan shiftEndTime = new TimeSpan(8, 0, 0);    // 08:00 (8 AM)

                if (now.TimeOfDay >= shiftEndTime && now.TimeOfDay < shiftStartTime)
                {
                    // Current time is between 08:00 and 20:00 (day shift),
                    // so last shift is previous night shift (20:00 yesterday to 08:00 today)
                    shiftStart = now.Date.AddDays(-1).Add(shiftStartTime); // Yesterday 20:00
                    shiftEnd = now.Date.Add(shiftEndTime);                 // Today 08:00
                }
                else
                {
                    // Current time is between 20:00 and 08:00 (night shift),
                    // so current shift is today 20:00 to tomorrow 08:00
                    if (now.TimeOfDay >= shiftStartTime)
                    {
                        // Between 20:00 and midnight
                        shiftStart = now.Date.Add(shiftStartTime);        // Today 20:00
                        shiftEnd = now.Date.AddDays(1).Add(shiftEndTime); // Tomorrow 08:00
                    }
                    else
                    {
                        // Between midnight and 08:00
                        shiftStart = now.Date.AddDays(-1).Add(shiftStartTime); // Yesterday 20:00
                        shiftEnd = now.Date.Add(shiftEndTime);                 // Today 08:00
                    }
                }

                // SQL query to count patrols within the shift time range
                string query = @"
            SELECT COUNT(*) FROM PATROLLING_DATA
            WHERE PATROL_TIME >= :shiftStart
              AND PATROL_TIME < :shiftEnd";

                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        // Bind parameters
                        cmd.Parameters.Add(new OracleParameter("shiftStart", shiftStart));
                        cmd.Parameters.Add(new OracleParameter("shiftEnd", shiftEnd));

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            // Assuming you want to show like "X/21"
                            clockingStatus = result.ToString() + "/21";
                        }
                    }
                }
            }
            catch
            {
                clockingStatus = "Error";
                // Optionally log ex.Message
            }

            return clockingStatus;
        }

        protected void btnClocking_Click(object sender, EventArgs e)
        {
            ViewState["ActiveModal"] = "Clocking";

            string date = txtClockingDate.Text;
            
            DateTime reportDate;

            bool isUserInput = DateTime.TryParse(txtClockingDate.Text, out reportDate);
            if (!DateTime.TryParse(txtClockingDate.Text, out reportDate))
            {
                reportDate = DateTime.Today;
            }

          
                StringBuilder html = new StringBuilder();
                string connStr = ConfigurationManager.AppSettings["ConnectionString"];

                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    string query = @"
                SELECT CHECKPOINT_NAME, PATROL_TIME 
                FROM PATROLLING_DATA
                WHERE TRUNC(PATROL_TIME) = :reportDate
                ORDER BY PATROL_TIME";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(":selectedDate", OracleDbType.Date).Value = reportDate.Date;

                        try
                        {
                            conn.Open();
                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    html.Append("<table class='table table-bordered'>");
                                    html.Append("<thead><tr><th>Checkpoint Name</th><th>Patrol Time</th></tr></thead><tbody>");
                                    while (reader.Read())
                                    {
                                        string name = reader["CHECKPOINT_NAME"].ToString();
                                        DateTime patrolTime = Convert.ToDateTime(reader["PATROL_TIME"]);
                                        html.AppendFormat("<tr><td>{0}</td><td>{1:yyyy-MM-dd HH:mm:ss}</td></tr>", name, patrolTime);
                                    }
                                    html.Append("</tbody></table>");
                                }
                                else
                                {
                                    html.Append("<p>No data found for selected date.</p>");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            html.Append("<p class='text-danger'>Error: " + ex.Message + "</p>");
                        }
                    }
                }

                ClockingList.Text = html.ToString();
            UpdateChartData();
        }
           
        

        protected void ddlChartCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

            string category = ddlChartCategory.SelectedValue;
            string range = ddlMonthRange.SelectedValue; 
             LoadChartData(category, range);

        }
        protected void ddlMonthRange_SelectedIndexChanged(object sender, EventArgs e)
        {

            // Get the selected chart category
            string selectedCategory = ddlChartCategory.SelectedValue;

            // Get the selected month range, e.g., "2024-01~2024-06"
            string selectedRange = ddlMonthRange.SelectedValue;

            // ✅ Load chart data with category and range
            LoadChartData(selectedCategory, selectedRange);
        }

        private void LoadChartData(string category, string range)
        {
            string chartDataJson = "";
            var labels = new List<string>();

            // ✅ Get FROM–TO from dropdown
            string[] parts = ddlMonthRange.SelectedValue.Split(':');
            string fromMonth = parts[0]; // e.g., "2024-01"
            string toMonth = parts[1];   // e.g., "2024-06"

            DateTime fromDate = DateTime.Parse(fromMonth + "-01");
            DateTime toDate = DateTime.Parse(toMonth + "-01").AddMonths(1).AddDays(-1);

            if (category == "Visitor")
            {
                var container = new Dictionary<string, int>();
                var visitor = new Dictionary<string, int>();
                var transporter = new Dictionary<string, int>();

                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    // Container
                    using (OracleCommand cmd = new OracleCommand(@"
                SELECT TO_CHAR(REGISTER_DATE, 'YYYY-MM') AS Month, COUNT(*) AS Total 
                FROM VIS_CONTAINER 
                WHERE REGISTER_DATE BETWEEN :fromDate AND :toDate
                GROUP BY TO_CHAR(REGISTER_DATE, 'YYYY-MM')
                ORDER BY Month", conn))
                    {
                        cmd.Parameters.Add(":fromDate", OracleDbType.Date).Value = fromDate;
                        cmd.Parameters.Add(":toDate", OracleDbType.Date).Value = toDate;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string month = reader["Month"].ToString();
                                container[month] = Convert.ToInt32(reader["Total"]);
                                if (!labels.Contains(month)) labels.Add(month);
                            }
                        }
                    }

                    // Visitor
                    using (OracleCommand cmd = new OracleCommand(@"
                SELECT TO_CHAR(CREATED_AT, 'YYYY-MM') AS Month, COUNT(*) AS Total 
                FROM VIS_VEHICLE 
                WHERE CREATED_AT BETWEEN :fromDate AND :toDate
                GROUP BY TO_CHAR(CREATED_AT, 'YYYY-MM')
                ORDER BY Month", conn))
                    {
                        cmd.Parameters.Add(":fromDate", OracleDbType.Date).Value = fromDate;
                        cmd.Parameters.Add(":toDate", OracleDbType.Date).Value = toDate;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string month = reader["Month"].ToString();
                                visitor[month] = Convert.ToInt32(reader["Total"]);
                                if (!labels.Contains(month)) labels.Add(month);
                            }
                        }
                    }

                    // Transporter
                    using (OracleCommand cmd = new OracleCommand(@"
                SELECT TO_CHAR(CREATED_AT, 'YYYY-MM') AS Month, COUNT(*) AS Total 
                FROM VIS_PARKING 
                WHERE CREATED_AT BETWEEN :fromDate AND :toDate
                GROUP BY TO_CHAR(CREATED_AT, 'YYYY-MM')
                ORDER BY Month", conn))
                    {
                        cmd.Parameters.Add(":fromDate", OracleDbType.Date).Value = fromDate;
                        cmd.Parameters.Add(":toDate", OracleDbType.Date).Value = toDate;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string month = reader["Month"].ToString();
                                transporter[month] = Convert.ToInt32(reader["Total"]);
                                if (!labels.Contains(month)) labels.Add(month);
                            }
                        }
                    }
                }

                labels.Sort();
                var containerList = labels.Select(m => container.ContainsKey(m) ? container[m] : 0).ToList();
                var visitorList = labels.Select(m => visitor.ContainsKey(m) ? visitor[m] : 0).ToList();
                var transporterList = labels.Select(m => transporter.ContainsKey(m) ? transporter[m] : 0).ToList();

                var data = new
                {
                    labels = labels,
                    container = containerList,
                    visitor = visitorList,
                    transporter = transporterList
                };

                chartDataJson = new JavaScriptSerializer().Serialize(data);
            }
            else if (category == "CCTV")
            {
                var online = new Dictionary<string, double>();
                var offline = new Dictionary<string, double>();
                labels.Clear();

                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    // --- Online ---
                    using (OracleCommand cmd = new OracleCommand(@"
                SELECT TO_CHAR(REPORT_DATE, 'YYYY-MM') AS Month,
                       AVG(TO_NUMBER(CCTV_WORKING)) AS AvgWorking
                FROM SECURITY_REPORT
                WHERE REGEXP_LIKE(CCTV_WORKING, '^\d+(\.\d+)?$')
                  AND REPORT_DATE BETWEEN :fromDate AND :toDate
                GROUP BY TO_CHAR(REPORT_DATE, 'YYYY-MM')
                ORDER BY Month", conn))
                    {
                        cmd.Parameters.Add(":fromDate", OracleDbType.Date).Value = fromDate;
                        cmd.Parameters.Add(":toDate", OracleDbType.Date).Value = toDate;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string month = reader["Month"].ToString();
                                double avgValue = 0;

                                if (!reader.IsDBNull(reader.GetOrdinal("AvgWorking")))
                                {
                                    var val = reader["AvgWorking"];
                                    if (val is Oracle.ManagedDataAccess.Types.OracleDecimal od)
                                        avgValue = Math.Round((double)od.Value, 2);
                                    else if (val is decimal dec)
                                        avgValue = Math.Round((double)dec, 2);
                                    else if (!double.TryParse(val.ToString(), out avgValue))
                                        avgValue = 0;
                                    else
                                        avgValue = Math.Round(avgValue, 2);
                                }

                                online[month] = avgValue;
                                if (!labels.Contains(month)) labels.Add(month);
                            }
                        }
                    }

                    // --- Offline ---
                    using (OracleCommand cmd = new OracleCommand(@"
                SELECT TO_CHAR(REPORT_DATE, 'YYYY-MM') AS Month,
                       AVG(TO_NUMBER(CCTV_OFFLINE)) AS AvgOffline
                FROM SECURITY_REPORT
                WHERE REPORT_DATE BETWEEN :fromDate AND :toDate
                GROUP BY TO_CHAR(REPORT_DATE, 'YYYY-MM')
                ORDER BY Month", conn))
                    {
                        cmd.Parameters.Add(":fromDate", OracleDbType.Date).Value = fromDate;
                        cmd.Parameters.Add(":toDate", OracleDbType.Date).Value = toDate;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string month = reader["Month"].ToString();
                                double avgValue = 0;

                                if (!reader.IsDBNull(reader.GetOrdinal("AvgOffline")))
                                {
                                    var val = reader["AvgOffline"];
                                    if (val is Oracle.ManagedDataAccess.Types.OracleDecimal od)
                                        avgValue = Math.Round((double)od.Value, 2);
                                    else if (val is decimal dec)
                                        avgValue = Math.Round((double)dec, 2);
                                    else if (!double.TryParse(val.ToString(), out avgValue))
                                        avgValue = 0;
                                    else
                                        avgValue = Math.Round(avgValue, 2);
                                }

                                offline[month] = avgValue;
                                if (!labels.Contains(month)) labels.Add(month);
                            }
                        }
                    }
                }

                labels.Sort();
                var onlineList = labels.Select(m => online.ContainsKey(m) ? online[m] : 0).ToList();
                var offlineList = labels.Select(m => offline.ContainsKey(m) ? offline[m] : 0).ToList();

                var data = new
                {
                    labels = labels,
                    online = onlineList,
                    offline = offlineList
                };

                chartDataJson = new JavaScriptSerializer().Serialize(data);
            }

            ClientScript.RegisterStartupScript(this.GetType(), "renderChart",
                $"renderChart('{category}', {chartDataJson});", true);
            UpdateChartData();
        }


        private void UpdateChartData()
        {
            // Get updated counts
            int outCount = GetStaffCount("TIME_IN IS NULL AND APPROVAL_STATUS= 'Approved'", false);
            int inCount = GetStaffCount("TIME_IN IS NOT NULL", true);
            int visitorIn = GetTotalVisitors("TIME_OUT IS NULL", false);
            int visitorOut = GetTotalVisitors("TIME_OUT IS NOT NULL", true);

            // Pass updated data to JavaScript (called once after action)
            ClientScript.RegisterStartupScript(this.GetType(), "staffData", $@"<script>var staffOut = {outCount}; var staffIn = {inCount};</script>");
            ClientScript.RegisterStartupScript(this.GetType(), "VisitorChartScript", $@"<script>var visitorIn = {visitorIn}; var visitorOut = {visitorOut};</script>");


            // Call updateDonutChart to reflect updated data
            ClientScript.RegisterStartupScript(this.GetType(), "updateDonutChart", @"<script>updateDonutChart();</script>");
        }

    }
}




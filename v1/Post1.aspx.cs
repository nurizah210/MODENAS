using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using Oracle.ManagedDataAccess.Client;

namespace vms.v1
{
    public partial class Post1 : System.Web.UI.Page
    {

        private string connStr = ConfigurationManager.AppSettings["ConnectionString"];

        protected void Page_Load(object sender, EventArgs e)
        {

            CheckOverstayedVisitors(connStr);
            if (!IsPostBack)
            {
                CheckAndUpdateTimeIn();
                BindStaffData(connStr);
                BindVehicleVisitor(connStr);
                BindTNBSUBS(connStr);
                BindParking(connStr);
                BindStaffMove(connStr);
                UpdateChartData();
                CheckOverstayedVisitors(connStr);

            }
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

        public static void CheckAndUpdateTimeIn()
        {
            string connStr = ConfigurationManager.AppSettings["ConnectionString"];

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();

                string updateQuery = @"
                    UPDATE VIS_EXITSTAFF
                    SET TIME_IN = 'N/A'
                    WHERE RETURN_STATUS = 'No'
                      AND (TIME_IN IS NULL OR TIME_IN = '')
                      AND TRUNC(DATE_OUT) < TRUNC(SYSDATE)";

                using (OracleCommand cmd = new OracleCommand(updateQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void BindStaffData(string connString)
        {
            try
            {
                string query = @"
    SELECT 
        STAFF_NAME, 
        TIME_OUT, 
        DATE_OUT, 
        RETURN_STATUS 
    FROM VIS_EXITSTAFF 
    WHERE APPROVAL_STATUS = 'Approved' 
      AND TIME_IN IS NULL";




                using (OracleConnection conn = new OracleConnection(connString)) using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    gvStaffList.DataSource = dt;
                    gvStaffList.DataBind();
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }
        protected void gvStaffList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                string returnStatus = DataBinder.Eval(e.Row.DataItem, "RETURN_STATUS")?.ToString();


                Button btnStaffReturn = (Button)e.Row.FindControl("btnStaffReturn");


                if (btnStaffReturn != null && returnStatus == "No")
                {
                    btnStaffReturn.Visible = false;
                }

                Button btnOut = (Button)e.Row.FindControl("btnOut");
                string staffName = DataBinder.Eval(e.Row.DataItem, "STAFF_NAME").ToString();

                // Only hide if already updated
                List<string> updatedStaff = Session["UpdatedOutStaff"] as List<string>;
                if (updatedStaff != null && updatedStaff.Contains(staffName))
                {
                    btnOut.Visible = false;
                }
            }
        }


        protected void gvStaffList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "TimeOut")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvStaffList.Rows[index];

                Label lblTimeOut = (Label)row.FindControl("lblTimeOut");
                Button btnOut = (Button)row.FindControl("btnOut");
                string currentTime = DateTime.Now.ToString("HH:mm");
                lblTimeOut.Text = currentTime;

                string staffName = row.Cells[0].Text;
                DateTime today = DateTime.Today;

                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    string updateQuery = @"
            UPDATE VIS_EXITSTAFF 
            SET TIME_OUT = :timeOut 
            WHERE STAFF_NAME = :staffName 
              AND TRUNC(DATE_OUT) = TRUNC(:dateOut)";

                    using (OracleCommand cmd = new OracleCommand(updateQuery, conn))
                    {
                        cmd.Parameters.Add(":timeOut", OracleDbType.Varchar2).Value = currentTime;
                        cmd.Parameters.Add(":staffName", OracleDbType.Varchar2).Value = staffName;
                        cmd.Parameters.Add(":dateOut", OracleDbType.Date).Value = today;

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // Save updated staff info to session or ViewState
                            List<string> updatedStaff = Session["UpdatedOutStaff"] as List<string> ?? new List<string>();
                            updatedStaff.Add(staffName);
                            Session["UpdatedOutStaff"] = updatedStaff;
                        }
                    }
                }

                BindStaffData(connStr);
                UpdateChartData();
            }
            else if (e.CommandName == "CheckIn")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                string staffName = args[0];
                DateTime dateOut = DateTime.Parse(args[1]);
                string timeOut = args[2];

                UpdateStaffReturnTime(staffName, dateOut, timeOut);
                BindStaffData(connStr);
                UpdateChartData();
                CheckOverstayedVisitors(connStr);
            }
        }




        protected void btnStaffReturn_Click(object sender, EventArgs e)
        {
            string[] args = ((Button)sender).CommandArgument.ToString().Split('|');
            string staffName = args[0];
            DateTime dateOut = DateTime.Parse(args[1]);
            string timeOut = args[2];

            UpdateStaffReturnTime(staffName, dateOut, timeOut);
            BindStaffData(connStr);
            UpdateChartData();
            CheckOverstayedVisitors(connStr);
        }




        private void UpdateStaffReturnTime(string staffName, DateTime dateOut, string timeOut)
        {
            try
            {
                StaffInfo info = GetStaffInfo(staffName, dateOut, timeOut);
                if (info == null || info.DateOut == null || string.IsNullOrEmpty(info.TimeOut))
                    return;

                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    string currentTime = DateTime.Now.ToString("HH:mm");

                    // ✅ 1️⃣ Update TIME_IN first
                    string updateQuery = @"
                UPDATE VIS_EXITSTAFF 
                SET TIME_IN = :timeIn 
                WHERE STAFF_NAME = :staffName 
                  AND TRUNC(DATE_OUT) = TRUNC(:dateOut) 
                  AND TIME_OUT = :timeOut";

                    using (OracleCommand cmd = new OracleCommand(updateQuery, conn))
                    {
                        cmd.Parameters.Add("timeIn", OracleDbType.Varchar2).Value = currentTime;
                        cmd.Parameters.Add("staffName", OracleDbType.Varchar2).Value = staffName;
                        cmd.Parameters.Add("dateOut", OracleDbType.Date).Value = info.DateOut.Value.Date;
                        cmd.Parameters.Add("timeOut", OracleDbType.Varchar2).Value = info.TimeOut;

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                           
                            if (!string.IsNullOrEmpty(info.Type) && info.Type.Equals("Personal Reason", StringComparison.OrdinalIgnoreCase))
                            {
                                string durationStr = CalculateDuration(
                                    staffName,
                                    currentTime,
                                    info.TimeOut,
                                    info.WorkingTime,
                                    info.BreakTime,
                                   DateTime.Now.DayOfWeek
                                );

                                if (!string.IsNullOrEmpty(durationStr))
                                {
                                    int duration = int.Parse(durationStr);

                                    
                                    string updateLeaveForQuery = @"
                                UPDATE VIS_EXITSTAFF 
                                SET LEAVE_FOR = :leaveFor 
                                WHERE STAFF_NAME = :staffName 
                                  AND TRUNC(DATE_OUT) = TRUNC(:dateOut) 
                                  AND TIME_OUT = :timeOut";

                                    using (OracleCommand updateLeaveForCmd = new OracleCommand(updateLeaveForQuery, conn))
                                    {
                                        updateLeaveForCmd.Parameters.Add("leaveFor", OracleDbType.Int32).Value = duration;
                                        updateLeaveForCmd.Parameters.Add("staffName", OracleDbType.Varchar2).Value = staffName;
                                        updateLeaveForCmd.Parameters.Add("dateOut", OracleDbType.Date).Value = info.DateOut.Value.Date;
                                        updateLeaveForCmd.Parameters.Add("timeOut", OracleDbType.Varchar2).Value = info.TimeOut;

                                        updateLeaveForCmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in UpdateStaffReturnTime: " + ex.Message);
            }
        }


        private List<(DateTime start, DateTime end)> GetFridayBreaksFromDB()
        {
            var breaks = new List<(DateTime start, DateTime end)>();

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("SELECT START_TIME, DURATION_MIN FROM VIS_FRID WHERE ACTIVE = 1 ORDER BY START_TIME", conn))
                {
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string startStr = dr["START_TIME"].ToString(); // e.g. '13:15'
                            int duration = int.Parse(dr["DURATION_MIN"].ToString());

                            DateTime start = DateTime.ParseExact(startStr, "HH:mm", null);
                            breaks.Add((start, start.AddMinutes(duration)));
                        }
                    }
                }
            }

            return breaks;
        }

        private string CalculateDuration(string staffName, string timeIn, string timeOut, string workingTime, string breakTimeString, DayOfWeek dayOfWeek)
        {
            try
            {
                List<(DateTime start, DateTime end)> breaks = new List<(DateTime, DateTime)>();

                // Common breaks (morning + tea)
                breaks.Add((DateTime.ParseExact("1000", "HHmm", null), DateTime.ParseExact("1020", "HHmm", null))); // Morning break
                breaks.Add((DateTime.ParseExact("1530", "HHmm", null), DateTime.ParseExact("1540", "HHmm", null))); // Tea break

                // Friday lunch breaks from DB
                if (dayOfWeek == DayOfWeek.Friday)
                {
                    List<(DateTime start, DateTime end)> fridayBreaks = GetFridayBreaksFromDB();
                    breaks.AddRange(fridayBreaks); // Append Friday lunch (1 or more slots)
                }
                else
                {
                    // Additional breaks based on breakTimeString for non-Friday
                    int[] breakDurations = { 20, 40, 10 };
                    string[] breakTimes = breakTimeString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (breakTimes.Length != breakDurations.Length)
                        throw new Exception("Break time slots and durations do not match.");

                    for (int i = 0; i < breakTimes.Length; i++)
                    {
                        DateTime brStart = DateTime.ParseExact(breakTimes[i].Trim(), "HHmm", null);
                        breaks.Add((brStart, brStart.AddMinutes(breakDurations[i])));
                    }
                }

                return CalculateFinalDuration(timeOut, timeIn, breaks);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        
        private string CalculateFinalDuration(string timeOut, string timeIn, List<(DateTime start, DateTime end)> breaks)
        {
            DateTime staffOut = DateTime.ParseExact(timeOut, "HH:mm", null);
            DateTime staffIn = DateTime.ParseExact(timeIn, "HH:mm", null);

            int totalMinutes = (int)(staffIn - staffOut).TotalMinutes;

            foreach (var br in breaks)
            {
                if (staffOut < br.end && staffIn > br.start)
                {
                    DateTime overlapStart = staffOut > br.start ? staffOut : br.start;
                    DateTime overlapEnd = staffIn < br.end ? staffIn : br.end;

                    if (overlapEnd > overlapStart)
                        totalMinutes -= (int)(overlapEnd - overlapStart).TotalMinutes;
                }
            }

            return totalMinutes.ToString();
        }








        private StaffInfo GetStaffInfo(string staffName, DateTime dateOut, string timeOut)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    string query = @"SELECT TIME_OUT, DATE_OUT, TYPE, RETURN_STATUS, WORKING_TIME, BREAK_TIME 
                FROM VIS_EXITSTAFF 
                WHERE STAFF_NAME = :staffName 
                AND TRUNC(DATE_OUT) = TRUNC(:dateOut)
                AND TIME_OUT = :timeOut";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("staffName", OracleDbType.Varchar2).Value = staffName;
                        cmd.Parameters.Add("dateOut", OracleDbType.Date).Value = dateOut.Date;
                        cmd.Parameters.Add("timeOut", OracleDbType.Varchar2).Value = timeOut;
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new StaffInfo
                                {
                                    TimeOut = reader["TIME_OUT"]?.ToString(),
                                    DateOut = reader["DATE_OUT"] == DBNull.Value ? null : (DateTime?)reader["DATE_OUT"],    
                                    Type = reader["TYPE"]?.ToString(),
                                    ReturnStatus = reader["RETURN_STATUS"]?.ToString(),
                                      WorkingTime = reader["WORKING_TIME"]?.ToString(),
                                    BreakTime = reader["BREAK_TIME"]?.ToString()

                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return null;
        }


        public class StaffInfo
        {
            public string TimeOut
            {
                get;
                set;
            }

            public DateTime? DateOut
            {
                get;
                set;
            }

            public string WorkingTime { get; set; }  
            public string BreakTime { get; set; }
            public string Type
            {
                get;
                set;
            }
            public string ReturnStatus
            {
                get;
                set;
            }
        }


        private void BindVehicleVisitor(string connString)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT PLATE_NO, NAME, TIME_IN FROM VIS_VEHICLE WHERE TIME_OUT IS null";

                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvVehicleVisitor.DataSource = dt;
                    gvVehicleVisitor.DataBind();
                }

                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);
                }
            }

        }


        protected void btnCheckOut1_Click(object sender, EventArgs e)
        {
            string plateNo = ((Button)sender).CommandArgument.ToString();

            DateTime timeOut = DateTime.Now;

            string updateQuery = "UPDATE VIS_VEHICLE SET TIME_OUT = :TimeOut WHERE PLATE_NO = :PlateNo AND TIME_OUT IS NULL";

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand(updateQuery, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("TimeOut", timeOut));
                        cmd.Parameters.Add(new OracleParameter("PlateNo", plateNo));

                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                catch (Exception ex)
                {
                    Response.Write("Error: " + ex.Message);
                }
            }

            BindVehicleVisitor(connStr);
            UpdateChartData();
            CheckOverstayedVisitors(connStr);
        }


        protected void btnCheckOut2_Click(object sender, EventArgs e)
        {
            string plateNo = ((Button)sender).CommandArgument.ToString();

            DateTime timeOut = DateTime.Now;

            string updateQuery = "UPDATE VIS_TNB SET TIME_OUT = :TimeOut WHERE NO_PLATE = :PlateNo AND TIME_OUT IS NULL";

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand(updateQuery, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("TimeOut", timeOut));
                        cmd.Parameters.Add(new OracleParameter("PlateNo", plateNo));

                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                catch (Exception ex)
                {
                    Response.Write("Error: " + ex.Message);
                }
            }

            BindTNBSUBS(connStr);
            UpdateChartData();
            CheckOverstayedVisitors(connStr);

        }

        private void BindTNBSUBS(string connString)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT NO_PLATE, NAME, TIME_IN FROM VIS_TNB WHERE TIME_OUT IS null";

                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvTNB.DataSource = dt;
                    gvTNB.DataBind();
                }

                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);
                }
            }

        }

        protected void btnCheckOut3_Click(object sender, EventArgs e)
        {
            string plateNo = ((Button)sender).CommandArgument.ToString();
            GridViewRow row = ((Button)sender).NamingContainer as GridViewRow;

            DropDownList ddlVehicleTypeOut = row.FindControl("ddlVehicleTypeOut") as DropDownList;
            string vehicleTypeOut = ddlVehicleTypeOut.SelectedValue;
            DateTime timeOut = DateTime.Now;
            TextBox txtPlateNoOut = row.FindControl("txtPlateNoOut") as TextBox;
            string plateNoOut = txtPlateNoOut.Text.Trim();

            string updateQuery = "UPDATE VIS_PARKING SET TIME_OUT = :TimeOut, VEHICLE_TYPE_OUT = :VehicleTypeOut, OUT_PLATE_NO = :PlateNoOut WHERE NO_PLATE = :PlateNo AND TIME_OUT IS NULL";

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand(updateQuery, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("TimeOut", timeOut));
                        cmd.Parameters.Add(new OracleParameter("VehicleTypeOut", vehicleTypeOut));
                        cmd.Parameters.Add(new OracleParameter("PlateNoOut", plateNoOut));
                        cmd.Parameters.Add(new OracleParameter("PlateNo", plateNo));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {

                            Response.Write("Parking record updated successfully.");
                        }

                        else
                        {

                            Response.Write("No matching parking record found or already updated.");
                        }
                    }
                }

                catch (Exception ex)
                {
                    Response.Write("Error: " + ex.Message);
                }

                BindParking(connStr);
                UpdateChartData();
                CheckOverstayedVisitors(connStr);
            }
        }

        private void BindParking(string connString)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT NO_PLATE, DRIVER_NAME, COMPANY, VEHICLE_TYPE, TIME_IN FROM VIS_PARKING WHERE TIME_OUT IS null";

                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvParking.DataSource = dt;
                    gvParking.DataBind();
                }

                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);
                }
            }

        }

        protected void btnCheckOut4_Click(object sender, EventArgs e)
        {
            string plateNo = ((Button)sender).CommandArgument.ToString();

            DateTime timeOut = DateTime.Now;

            string updateQuery = "UPDATE VIS_STAFFMOVE SET TIME_OUT = :TimeOut WHERE NO_PLATE = :PlateNo AND TIME_OUT IS NULL";

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand(updateQuery, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("TimeOut", timeOut));
                        cmd.Parameters.Add(new OracleParameter("PlateNo", plateNo));

                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                catch (Exception ex)
                {
                    Response.Write("Error: " + ex.Message);
                }
            }

            BindStaffMove(connStr);
            UpdateChartData();
            CheckOverstayedVisitors(connStr);
        }

        private void BindStaffMove(string connString)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT NO_PLATE, EMP_NAME, EMP_NO, TIME_IN FROM VIS_STAFFMOVE WHERE TIME_OUT IS null";

                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvMovement.DataSource = dt;
                    gvMovement.DataBind();
                }

                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);
                }
            }

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

        private void CheckOverstayedVisitors(string connString)
        {

            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                string query = @"
         SELECT 
    'VEHICLE' AS CATEGORY, 
    PLATE_NO AS IDENTIFIER, 
    TIME_IN, 
    SYSDATE AS CURRENT_TIME,
    ROUND((SYSDATE - CAST(TIME_IN AS DATE)) * 24, 2) AS DURATION_HOURS,
    LOCATION
FROM VIS_VEHICLE
WHERE TIME_OUT IS NULL AND (SYSDATE - CAST(TIME_IN AS DATE)) * 24 > 1

UNION ALL

SELECT 
    'TRANSPORTER', 
    NO_PLATE, 
    TIME_IN, 
    SYSDATE, 
    ROUND((SYSDATE - CAST(TIME_IN AS DATE)) * 24, 2),
    NULL 
FROM VIS_PARKING
WHERE TIME_OUT IS NULL AND (SYSDATE - CAST(TIME_IN AS DATE)) * 24 > 8

UNION ALL

SELECT 
    'TNB', 
    NO_PLATE, 
    TIME_IN, 
    SYSDATE, 
    ROUND((SYSDATE - CAST(TIME_IN AS DATE)) * 24, 2),
    NULL  
FROM VIS_TNB
WHERE TIME_OUT IS NULL AND (SYSDATE - CAST(TIME_IN AS DATE)) * 24 > 2

UNION ALL

SELECT 
    'VISITOR', 
    NAME, 
    REGISTER_DATE, 
    SYSDATE, 
    ROUND((SYSDATE - CAST(REGISTER_DATE AS DATE)) * 24, 2),
    BLOCK 
FROM VIS_VISITOR
WHERE TIME_OUT IS NULL AND (SYSDATE - CAST(REGISTER_DATE AS DATE)) * 24 > 2

UNION ALL

SELECT 
    'CONTAINER', 
    PLATE_NO, 
    REGISTER_DATE, 
    SYSDATE, 
    ROUND((SYSDATE - CAST(REGISTER_DATE AS DATE)) * 24, 2),
    NULL 
FROM VIS_CONTAINER
WHERE TIME_OUT IS NULL AND (SYSDATE - CAST(REGISTER_DATE AS DATE)) * 24 > 3
";


                using (OracleCommand cmd = new OracleCommand(query, conn))
                {

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();

                            while (reader.Read())
                            {
                                string category = reader["CATEGORY"].ToString();
                                string identifier = reader["IDENTIFIER"].ToString();
                                string timeIn = Convert.ToDateTime(reader["TIME_IN"]).ToString("yyyy-MM-dd HH:mm");
                                string duration = reader["DURATION_HOURS"].ToString();
                                string location = reader["LOCATION"] == DBNull.Value ? "" : reader["LOCATION"].ToString();

                                sb.Append("<tr>");
                                sb.AppendFormat("<td>{0}</td>", category);
                                sb.AppendFormat("<td>{0}</td>", identifier);
                                sb.AppendFormat("<td>{0}</td>", timeIn);
                                sb.AppendFormat("<td>{0}</td>", duration);
                                sb.AppendFormat("<td>{0}</td>", location);  
                                sb.Append("</tr>");
                            }


                            // Inject HTML to client-side
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowVisitorOverstayModal",
                                "$('#modalVisitorData').html(`" + sb.ToString() + "`); $('#visitorOverstayModal').modal('show');", true);
                        }
                    }
                }
            }
        }
       
    }


}
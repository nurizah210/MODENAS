using System;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Web.UI;
using Oracle.ManagedDataAccess.Client;
using System.Text;

namespace vms.v1
{
    public partial class AuxiliaryPolice : Page
    {
        string connStr = ConfigurationManager.AppSettings["ConnectionString"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               
            }
        }

        protected void btnVerified_Click(object sender, EventArgs e)
        {
            string incidentTitle = lblIncidentTitle.Text.Trim();
            string incidentDesc = lblIncidentDesc.Text.Trim();
            string cctvAbnorm = lblCCTVAbnorm.Text.Trim();
            string actionTaken = lblActionTaken.Text.Trim();
            string dayName = lblDayName.Text.Trim();
            string kplComments = txtKPLComments.Text.Trim();
            string shiftTime = ddlShift.SelectedValue;
            string dateText = txtDate.Text; // ✅ FIXED: Use txtDate instead of lblDate

            if (string.IsNullOrEmpty(kplComments) || string.IsNullOrEmpty(shiftTime) || string.IsNullOrEmpty(dateText))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Missing data. Please select date and shift.');", true);
                return;
            }

            if (!DateTime.TryParse(dateText, out DateTime reportDate))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Invalid date format.');", true);
                return;
            }

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                string sql = @"
    UPDATE SECURITY_REPORT 
    SET 
        DAY_NAME = :dayName,
        INCIDENT_TITLE = :incidentTitle,
        INCIDENT_DESC = :incidentDesc,
        CCTVABNORM = :cctvAbnorm,
        ACTION_TAKEN = :actionTaken,
        KPL_COMMENTS = :kplComments 
  WHERE TRUNC(REPORT_DATE) = :reportDate AND SHIFT_TIME = :shiftTime";


                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(":dayName", OracleDbType.Varchar2).Value = dayName;
                    cmd.Parameters.Add(":incidentTitle", OracleDbType.Varchar2).Value = incidentTitle;
                    cmd.Parameters.Add(":incidentDesc", OracleDbType.Varchar2).Value = incidentDesc;
                    cmd.Parameters.Add(":cctvAbnorm", OracleDbType.Varchar2).Value = cctvAbnorm;
                    cmd.Parameters.Add(":actionTaken", OracleDbType.Varchar2).Value = actionTaken;
                    cmd.Parameters.Add(":kplComments", OracleDbType.Varchar2).Value = kplComments;
                    cmd.Parameters.Add(":reportDate", OracleDbType.Date).Value = reportDate.Date;
                    cmd.Parameters.Add(":shiftTime", OracleDbType.Varchar2).Value = shiftTime;

                    conn.Open();
                    int rowsUpdated = cmd.ExecuteNonQuery();
                    conn.Close();

                    string msg = rowsUpdated > 0
                        ? "Koperal comment saved successfully."
                        : "No matching report found.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{msg}');", true);

                    // Now handle file upload separately if there is a file
                    if (FileUpload2.HasFile)
                    {
                        string ext = Path.GetExtension(FileUpload2.FileName).ToLower();
                        if (ext == ".xlsx" || ext == ".xls")
                        {
                            using (var stream = FileUpload2.PostedFile.InputStream)
                            using (var workbook = new ClosedXML.Excel.XLWorkbook(stream))
                            {
                                var worksheet = workbook.Worksheets.FirstOrDefault();
                                if (worksheet != null)
                                {
                                    int lastRow = worksheet.LastRowUsed().RowNumber();
                                    for (int row = 2; row <= lastRow; row++)
                                    {
                                        string checkpointName = worksheet.Cell(row, 3).GetString();
                                        string readerCode = worksheet.Cell(row, 4).GetString();
                                        string patrolTime = worksheet.Cell(row, 5).GetString();
                                        InsertPatrolData(checkpointName, readerCode, patrolTime);
                                    }
                                }
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Invalid file format. Please upload .xlsx or .xls');", true);
                        }
                    }

                }
            }
        }
        

        private void LoadReportData()
        {
            string selectedDate = txtDate.Text.Trim(); 
            string selectedShift = ddlShift.SelectedValue;

            if (string.IsNullOrEmpty(selectedDate) || string.IsNullOrEmpty(selectedShift))
                return;

            if (!DateTime.TryParse(selectedDate, out DateTime reportDate))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Invalid date format.');", true);
                return;
            }

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                string sql = @"
    SELECT * 
    FROM SECURITY_REPORT 
    WHERE TRUNC(REPORT_DATE) = :report_date 
      AND SHIFT_TIME = :shiftTime";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(":report_date", OracleDbType.Date).Value = reportDate;
                    cmd.Parameters.Add(":shiftTime", OracleDbType.Varchar2).Value = selectedShift;

                    conn.Open();
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                       

                            if (reader.Read())
                            {
                                string combinedGuards = "";

                                void AppendGuard(string data)
                                {
                                    if (!string.IsNullOrWhiteSpace(data))
                                    {
                                        combinedGuards += data + "<br />";
                                    }
                                }

                                AppendGuard(reader["POST1_GUARD1"]?.ToString());
                                AppendGuard(reader["POST1_GUARD2"]?.ToString());
                                AppendGuard(reader["POST2_GUARD1"]?.ToString());
                                AppendGuard(reader["POST2_GUARD2"]?.ToString());
                                AppendGuard(reader["POST2_GUARD3"]?.ToString());
                                AppendGuard(reader["EMOS_GUARD1"]?.ToString());
                                AppendGuard(reader["EMOS_GUARD2"]?.ToString());

                               litAllGuards.Text = combinedGuards;

                               lblDayName.Text = reader["DAY_NAME"]?.ToString();

                               lblCCTVAbnorm.Text = reader["CCTVABNORM"]?.ToString();
                                

                                lblIncidentTitle.Text = reader["INCIDENT_TITLE"]?.ToString();
                                

                                lblIncidentDesc.Text = reader["INCIDENT_DESC"]?.ToString();
                                

                                lblActionTaken.Text = reader["ACTION_TAKEN"]?.ToString();

                                ContractorDetails.Text = reader["CONTRACTORS"]?.ToString();


                            string photoPath = reader["INCIDENT_PHOTO_PATH"]?.ToString();

                            if (!string.IsNullOrWhiteSpace(photoPath))
                            {
                                // Split by ;
                                string[] paths = photoPath.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                                StringBuilder sb = new StringBuilder();

                                foreach (string path in paths)
                                {
                                    string imgUrl = ResolveUrl(path.Trim());

                                    sb.Append($@"
            <div class='report-photo' style='margin-top:10px;'>
                <img src='{imgUrl}' alt='Incident Photo' style='max-width:200px;' />
            </div>");
                                }

                                litIncidentPhoto.Text = sb.ToString();
                            }
                            else
                            {
                                litIncidentPhoto.Text = "";
                            }




                            txtKPLComments.Text = reader["KPL_COMMENTS"]?.ToString();
                            }
                            else
                            {
                                lblDayName.Text = "";
                                litAllGuards.Text = "";
                                lblCCTVAbnorm.Text = ""; 
                                lblIncidentTitle.Text = ""; 
                                lblIncidentDesc.Text = ""; 
                                lblActionTaken.Text = "";
                                ContractorDetails.Text = "";
                                litIncidentPhoto.Text = ""; 
                                txtKPLComments.Text = "";


                                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No report found for this date and shift.');", true);
                            }

                            ddlShift.Text = selectedShift;
                        }
                        conn.Close();
                    }
                }
            }
        private void InsertPatrolData(string checkpointName, string readerCode, string patrolTime)
        {
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();
                string sql = @"INSERT INTO PATROLLING_DATA (CHECKPOINT_NAME, READER_CODE, PATROL_TIME) 
                       VALUES (:checkpointName, :readerCode, TO_DATE(:patrolTime, 'DD/MM/YYYY HH24:MI:SS')
)";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(":checkpointName", checkpointName);
                    cmd.Parameters.Add(":readerCode", readerCode);
                    DateTime date = DateTime.Parse(patrolTime);
                    cmd.Parameters.Add(":patrolTime", OracleDbType.Varchar2).Value = date.ToString("dd/MM/yyyy HH:mm:ss");


                    cmd.ExecuteNonQuery();
                }
            }
        }
        

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            LoadReportData();
        }

        protected void ddlShift_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReportData();
        }
    }
}


using System;
using System.Data;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Text;
using System.Web.UI;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;



namespace vms.v1
{
    public partial class ListReport : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.AppSettings["ConnectionString"];

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadIncidents();
         
            if (!IsPostBack)
            {
                string currentMonth = DateTime.Now.ToString("MMMM yyyy");
         
                currentMonthLabel2.Text = currentMonth;

           


            }
        }

     
        private void LoadIncidents()
        {
            string query = "SELECT INCIDENT_TITLE, TRUNC(REPORT_DATE) AS INCIDENT_DATE FROM SECURITY_REPORT WHERE EXTRACT(MONTH FROM REPORT_DATE) = EXTRACT(MONTH FROM CURRENT_DATE) AND EXTRACT(YEAR FROM REPORT_DATE) = EXTRACT(YEAR FROM CURRENT_DATE)";

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataReader reader = cmd.ExecuteReader();

                    Incidents.Controls.Clear();

                    while (reader.Read())
                    {
                        string title = reader["INCIDENT_TITLE"]?.ToString();
                        string date = Convert.ToDateTime(reader["INCIDENT_DATE"]).ToString("yyyy-MM-dd");

                        if (!string.IsNullOrWhiteSpace(title))
                        {
                            LinkButton link = new LinkButton();
                            link.Text = title;
                            link.CssClass = "dropdown-item";
                            link.CommandArgument = date;
                            link.Command += Incident_Click;

                            Incidents.Controls.Add(link);
                        }
                    }

                    reader.Close();
                }

                catch
                {
                    HtmlGenericControl li = new HtmlGenericControl("li");
                    li.Attributes["class"] = "dropdown-item text-danger";
                    li.InnerText = "Error loading incidents";
                    Incidents.Controls.Add(li);
                }
            }
        }

        protected void Incident_Click(object sender, CommandEventArgs e)
        {
            if (DateTime.TryParse(e.CommandArgument.ToString(), out DateTime incidentDate))
            {
                LoadReportForDate(incidentDate);
            }
        }



        protected void btnAddNewReport_Click(object sender, EventArgs e)
        {
            string position = Session["position"]?.ToString().ToUpper().Trim();

            if (position == "AUXILIARY POLICE")
            {
                Response.Redirect("~/v1/SecurityReport.aspx");
            }
            else
            {
      
              ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Only Auxiliary Police can add new reports.');", true);
            }
        
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DateTime selectedDate;

            if (DateTime.TryParse(txtReportDate.Text, out selectedDate))
            {
                LoadReportForDate(selectedDate);
            }
        }

        private void LoadReportForDate(DateTime date)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showProcessingModal", "$('#pleaseWaitDialog').modal('show');", true);

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                string query = @"SELECT * FROM SECURITY_REPORT WHERE TRUNC(REPORT_DATE) = :report_date";

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("report_date", date.Date));

                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        lblNoRecords.Visible = true;
                        lblNoRecords.Text = "No security reports found for the selected date.";
                        ltReportContent.Text = "";
                    }

                    else
                    {
                        lblNoRecords.Visible = false;
                        ltReportContent.Text = GenerateReportSummary(dt);
                    }
                }
            }
        }

        private string GenerateReportSummary(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataRow row in dt.Rows)
            {

                sb.AppendLine("  <div class='report-content'>");
                sb.AppendLine("    <div class='mb-3 d-flex justify-content-between align-items-center'>");
                sb.AppendLine("        <!-- Date and Shift Section -->");
                sb.AppendLine("        <div class='d-flex justify-content-between report-header'>");
                sb.AppendLine("            <span>" + Convert.ToDateTime(row["REPORT_DATE"]).ToString("dd/MM/yyyy") + " ( " + row["DAY_NAME"]?.ToString() + " )</span>");
                sb.AppendLine("            <span>&nbsp; | &nbsp;" + row["SHIFT_TIME"]?.ToString() + "</span>");
                sb.AppendLine("        </div>");



                sb.AppendLine("    </div>");
                sb.AppendLine("    <hr />");
                sb.AppendLine("    <!-- In Charge Section -->");
                sb.AppendLine("    <div class='report-in-charge'>");
                sb.AppendLine("      <strong> In Charge :</strong>");
                sb.AppendLine("      <ul>");
                sb.AppendLine("        <li><strong>Post 1:</strong> " + row["POST1_GUARD1"] + " / " + row["POST1_GUARD2"] + "</li>");
                sb.AppendLine("        <li><strong>Post 2:</strong> " + row["POST2_GUARD1"] + " / " + row["POST2_GUARD2"] + "</li>");
                sb.AppendLine("        <li><strong>EMOS:</strong> " + row["EMOS_GUARD1"] + " / " + row["EMOS_GUARD2"] + "</li>");
                sb.AppendLine("      </ul>");
                sb.AppendLine("    </div>");
                sb.AppendLine("    <hr />");
                sb.AppendLine("    <!-- Remarks Section -->");
                sb.AppendLine("    <div class='report-remarks'>");
                sb.AppendLine("      <strong>📌 Remarks:</strong>");
                sb.AppendLine("      <ul>");
                sb.AppendLine("        <li>Online CCTV : " + row["CCTV_WORKING"] + "</li>");
                sb.AppendLine("        <li> Offline CCTV :" + row["CCTV_OFFLINE"] + "</li>");
                sb.AppendLine("        <li> CCTV Abnormalities :" + row["CCTVABNORM"] + "</li>");

                if (!string.IsNullOrWhiteSpace(row["CONTRACTORS"].ToString()))
                {
                    sb.AppendLine("        <li>" + row["CONTRACTORS"] + "</li>");
                }

                if (!string.IsNullOrWhiteSpace(row["INCIDENT_TITLE"].ToString()) || !string.IsNullOrWhiteSpace(row["INCIDENT_DESC"].ToString()))
                {
                    sb.AppendLine("        <li>" + row["INCIDENT_TITLE"] + " - " + row["INCIDENT_DESC"] + "</li>");
                }

                if (!string.IsNullOrWhiteSpace(row["ACTION_TAKEN"].ToString()))
                {
                    sb.AppendLine("        <li>" + row["ACTION_TAKEN"] + "</li>");
                }

                sb.AppendLine("      </ul>");
                sb.AppendLine("    </div>");

                sb.AppendLine("    <hr />");

                if (!string.IsNullOrWhiteSpace(row["INCIDENT_PHOTO_PATH"].ToString()))
                {
                    // Add image tag with a reasonable size (adjust width/height as needed)
                    sb.AppendLine("    <div class='report-photo' style='margin-top:10px;'>");
                    string imgPath = ResolveUrl(row["INCIDENT_PHOTO_PATH"].ToString());
                    sb.AppendLine($"<img src='{imgPath}' alt='Incident Photo' style='max-width:200px;' />");
                    sb.AppendLine("    </div>");
                }

                sb.AppendLine("  </div>");



            }

            return sb.ToString();
        }


        protected void btnDownloadCSV_Click(object sender, EventArgs e)
        {
            if (DateTime.TryParse(txtReportDate.Text, out DateTime selectedDate))
            {
                DataTable dt = new DataTable();

                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    string query = @"SELECT * FROM SECURITY_REPORT WHERE TRUNC(REPORT_DATE) = :report_date";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("report_date", selectedDate.Date));
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);

                        adapter.Fill(dt);  // Load data once

                        if (dt.Rows.Count > 0)
                        {
                            ExportToCSV(dt, $"SecurityReport_{selectedDate:yyyyMMdd}");
                        }
                        else
                        {
                            lblNoRecords.Text = "No data to export.";
                            lblNoRecords.Visible = true;
                        }
                    }
                }
            }
        }

        private void ExportToCSV(DataTable dt, string filename)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", $"attachment;filename={filename}.csv");
            Response.Charset = "";
            Response.ContentType = "application/text";

            StringBuilder sb = new StringBuilder();

            // Add header
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.Append(dt.Columns[i].ColumnName);
                if (i < dt.Columns.Count - 1)
                    sb.Append(",");
            }
            sb.Append("\r\n");

            // Add data rows
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    object cell = row[i];
                    string value;

                    if (cell is DateTime dateVal)
                    {
                        value = dateVal.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        value = cell.ToString().Replace(",", " "); 
                    }

                    sb.Append(value);
                    if (i < dt.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.Append("\r\n");
            }

            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }




    }
}
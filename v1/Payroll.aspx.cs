using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using Oracle.ManagedDataAccess.Client;

namespace vms.v1
{
    public partial class Payroll : Page
    {
        string connStr = ConfigurationManager.AppSettings["ConnectionString"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
             ddlMonth.SelectedValue = DateTime.Now.Month.ToString();
             BindLateStaff(int.Parse(ddlMonth.SelectedValue));
             BindPersonalReason(int.Parse(ddlMonth.SelectedValue));
             BindOfficeBusiness(int.Parse(ddlMonth.SelectedValue));
            }
        }

        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
           int selectedMonth = int.Parse(ddlMonth.SelectedValue);

            BindLateStaff(selectedMonth);
            BindPersonalReason(selectedMonth);
            BindOfficeBusiness(selectedMonth);
        }

        private void BindLateStaff(int month)
        {
            gvLateStaff.DataSource = GetLateStaffThisMonth(month);
            gvLateStaff.DataBind();
        }

        private void BindPersonalReason(int month)
        {
            gvStaffLeave.DataSource = GetPersonalRecordsThisMonth(month);
            gvStaffLeave.DataBind();
        }
        private void BindOfficeBusiness(int month)
        {
            gvOffice.DataSource = GetOfficeRecordsThisMonth(month);
            gvOffice.DataBind();
        }
        private DataTable GetLateStaffThisMonth(int month)
        {
            DataTable dt = new DataTable();
            string query = @"
            SELECT EMP_NAME, EMP_NO,
       TO_CHAR(REPORT_DATE, 'DD-MM-YYYY') AS REPORT_DATE,
       TIME_IN, REASON_LATE
FROM VIS_LATESTAFF 
WHERE EXTRACT(MONTH FROM REPORT_DATE) = :Month
  AND EXTRACT(YEAR FROM REPORT_DATE) = EXTRACT(YEAR FROM SYSDATE)
ORDER BY REPORT_DATE DESC, TIME_IN DESC
";

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand(query, conn);
                    cmd.Parameters.Add(new OracleParameter("Month", month));
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch (Exception )
                {
                   
                }
            }
            return dt;
        }

        private DataTable GetPersonalRecordsThisMonth(int month)
        {
            DataTable dtPersonalRecords = new DataTable();
            string query = @"
SELECT * 
FROM VIS_EXITSTAFF 
WHERE EXTRACT(MONTH FROM DATE_OUT) = :Month 
  AND EXTRACT(YEAR FROM DATE_OUT) = EXTRACT(YEAR FROM SYSDATE)
  AND TYPE = 'Personal Reason' 
  AND APPROVAL_STATUS = 'Approved'
ORDER BY DATE_OUT DESC, TIME_OUT DESC";

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand(query, conn);
                    cmd.Parameters.Add(new OracleParameter("Month", month));
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    adapter.Fill(dtPersonalRecords);
                }
                catch
                {
                 
                }
            }
            return dtPersonalRecords;
        }
        private DataTable GetOfficeRecordsThisMonth(int month)
        {
            DataTable dtOfficeRecords = new DataTable();
            string query = @"
      SELECT * 
FROM VIS_EXITSTAFF 
WHERE EXTRACT(MONTH FROM DATE_OUT) = :Month 
  AND EXTRACT(YEAR FROM DATE_OUT) = EXTRACT(YEAR FROM SYSDATE)
  AND TYPE = 'Office Matter' 
  AND APPROVAL_STATUS = 'Approved'
ORDER BY DATE_OUT DESC, TIME_OUT DESC
";


            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand(query, conn);
                    cmd.Parameters.Add(new OracleParameter("Month", month));
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    adapter.Fill(dtOfficeRecords);
                }
                catch
                {
                   
                }
            }
            return dtOfficeRecords;
        }
        protected void btnDownloadLate_Click(object sender, EventArgs e)
        {
            int selectedMonth = int.Parse(ddlMonth.SelectedValue);
            DataTable dt = GetLateStaffThisMonth(selectedMonth);
            ExportToCSV(dt, "LateStaff_Report");
        }
        protected void btnDownloadPersonal_Click(object sender, EventArgs e)
        {
            int selectedMonth = int.Parse(ddlMonth.SelectedValue);
            DataTable dt = GetPersonalRecordsThisMonth(selectedMonth);
            ExportToCSV(dt, "PersonalReason_Report");
        }
        protected void btnDownloadOffice_Click(object sender, EventArgs e)
        {
            int selectedMonth = int.Parse(ddlMonth.SelectedValue);
            DataTable dt = GetPersonalRecordsThisMonth(selectedMonth);
            ExportToCSV(dt, "OfficeMatter_Report");
        }
        private void ExportToCSV(DataTable dt, string filename)
        {
            if (dt == null || dt.Rows.Count == 0)
                return;

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", $"attachment;filename={filename}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            Response.Charset = "";
            Response.ContentType = "application/text";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // Column headers
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.Append(dt.Columns[i].ColumnName);
                if (i < dt.Columns.Count - 1)
                    sb.Append(",");
            }
            sb.Append("\r\n");

            // Rows
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string value = row[i].ToString().Replace(",", " "); // Replace commas to keep CSV format safe
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

using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web.UI.WebControls;
using System.Configuration;
using SystemDataTable = System.Data.DataTable;
using SysDataTable = System.Data.DataTable;
using SysDataRow = System.Data.DataRow;
using SysDataColumn = System.Data.DataColumn;

namespace vms.v1
{
    public partial class VisitorList : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.AppSettings["ConnectionString"];

        protected void Page_Load(object sender, EventArgs e)
        {
            txtEndDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtStartDate.Text = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string visitorType = txtVisitorType.SelectedValue;  // Get the selected category (Visitor, Vendor, etc.)

            // Fetch data based on the selected visitor type from the database
            DateTime? startDate = string.IsNullOrWhiteSpace(txtStartDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtStartDate.Text);
            DateTime? endDate = string.IsNullOrWhiteSpace(txtEndDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtEndDate.Text);

            var visitors = GetVisitorsByCategory(visitorType, startDate, endDate);

          

            if (visitors != null && visitors.Rows.Count > 0)
            {
                // Display data on the page
                lblTable.Text = GenerateVisitorListTable(visitors);
            }
            else
            {
                lblTable.Text = "No records found for the selected category.";
            }
        }

        private DataTable GetVisitorsByCategory(string category, DateTime? startDate, DateTime? endDate)
        {
            DataTable dt = new DataTable();
            string baseQuery = "";
            string dateColumn = "";

            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    // Determine base query and date column based on category
                    switch (category.ToLower())
                    {
                        case "vehicle visitor":
                            baseQuery = "SELECT NAME, IC, COMPANY, PLATE_NO, VEHICLE_TYPE, LOCATION, TIME_IN, TIME_OUT FROM VIS_VEHICLE";
                            dateColumn = "TIME_IN";
                            break;
                        case "contractor tnb":
                            baseQuery = "SELECT NAME, NO_PLATE, IC_NO, PURPOSE, TIME_IN, TIME_OUT FROM VIS_TNB";
                            dateColumn = "TIME_IN";
                            break;
                        case "parking":
                            baseQuery = "SELECT DRIVER_NAME, VEHICLE_TYPE, NO_PLATE, PURPOSE, COMPANY, TIME_IN, TIME_OUT, OUT_PLATE_NO, VEHICLE_TYPE_OUT FROM VIS_PARKING";
                            dateColumn = "TIME_IN";
                            break;
                        case "staff movement":
                            baseQuery = "SELECT EMP_NAME, EMP_NO, DEPARTMENT, BLOCK, PURPOSE, TIME_IN, TIME_OUT FROM VIS_STAFFMOVE";
                            dateColumn = "TIME_IN";
                            break;
                        case "vehicle visitor 2":
                            baseQuery = "SELECT V.PLATE_NO, V.NAME, V2.PURPOSE, V2.BLOCK, V2.REGISTER_DATE, V2.ITEM_TYPE, V2.DO_NO, V2.TIME_OUT FROM VIS_VEHICLE V INNER JOIN VIS_VEHICLE2 V2 ON V2.VEHICLE_ID=V.VEHICLE_ID";
                            dateColumn = "V2.REGISTER_DATE";
                            break;
                        case "walkin visitor":
                            baseQuery = "SELECT NAME, IC_NO, COMPANY, PURPOSE, BLOCK, REGISTER_DATE, TIME_OUT FROM VIS_VISITOR";
                            dateColumn = "REGISTER_DATE";
                            break;
                        case "item declaration":
                            baseQuery = "SELECT NAME, EMP_NO, ITEM_TYPE, SERIAL_PART_NO, TOTAL_ITEM_IN, DECLARE_DATE, TOTAL_ITEM_OUT, TIME_OUT FROM VIS_ITEMDECLARE";
                            dateColumn = "DECLARE_DATE";
                            break;
                        case "container":
                            baseQuery = "SELECT DRIVER_NAME, DRIVER_ICNO, PLATE_NO, PRIMEMOVER_NO, COMPANY, CONTAINER_NO, SEAL_NO, REGISTER_DATE , ACKNOWLEDGEMENT, TIME_OUT FROM VIS_CONTAINER";
                            dateColumn = "REGISTER_DATE";
                            break;
                        default:
                            throw new Exception("Invalid category");
                    }

                    // Add WHERE clause only if both dates provided
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        baseQuery += $" WHERE {dateColumn} BETWEEN TO_DATE('{startDate:yyyy-MM-dd}', 'YYYY-MM-DD') AND TO_DATE('{endDate:yyyy-MM-dd 23:59:59}', 'YYYY-MM-DD HH24:MI:SS')";
                    }

                    OracleDataAdapter adapter = new OracleDataAdapter(baseQuery, conn);
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                lblTable.Text = "Error fetching data: " + ex.Message;
            }

            return dt;
        }


        private string GenerateVisitorListTable(DataTable visitors)
        {
            string html = "<div style='overflow-x: auto; overflow-y: auto; max-height: 400px; max-width: 100%;'>";
            html += "<table class='table table-bordered' style='font-size: 12px; min-width: 800px;'><thead><tr>";

            // Add column headers
            foreach (DataColumn column in visitors.Columns)
            {
                html += $"<th>{column.ColumnName}</th>";
            }

            html += "</tr></thead><tbody>";

            // Add rows
            foreach (DataRow row in visitors.Rows)
            {
                html += "<tr>";
                foreach (var item in row.ItemArray)
                {
                    html += $"<td>{item}</td>";
                }
                html += "</tr>";
            }

            html += "</tbody></table>";
            html += "</div>";  

            return html;
        }


        protected void btnDownloadCSV_Click(object sender, EventArgs e)
        {
            string visitorType = txtVisitorType.SelectedValue;
            DateTime? startDate = string.IsNullOrWhiteSpace(txtStartDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtStartDate.Text);
            DateTime? endDate = string.IsNullOrWhiteSpace(txtEndDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtEndDate.Text);

            DataTable visitors = GetVisitorsByCategory(visitorType, startDate, endDate);

            if (visitors != null && visitors.Rows.Count > 0)
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=VisitorList_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
                Response.Charset = "";
                Response.ContentType = "application/text";

                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                // Add column headers
                for (int i = 0; i < visitors.Columns.Count; i++)
                {
                    sb.Append(visitors.Columns[i].ColumnName);
                    if (i < visitors.Columns.Count - 1) sb.Append(",");
                }
                sb.Append("\r\n");

                // Add rows
                foreach (DataRow row in visitors.Rows)
                {
                    for (int i = 0; i < visitors.Columns.Count; i++)
                    {
                        string value = row[i].ToString().Replace(",", " "); // Replace commas to avoid breaking CSV
                        sb.Append(value);
                        if (i < visitors.Columns.Count - 1) sb.Append(",");
                    }
                    sb.Append("\r\n");
                }

                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
            else
            {
                lblTable.Text = "No data available to export.";
            }
        }

    }
}

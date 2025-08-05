using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;



namespace vms.v1
{
    public partial class RequestStatus : Page
    {
        private string connStr = ConfigurationManager.AppSettings["ConnectionString"];
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string empNo = Session["EmpID"]?.ToString(); // This is the actual EMP_NO from AD
                string position = Session["position"]?.ToString();

                PopulateUserInfo(empNo); // Use EmpID here instead of username

                txtEndDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtStartDate.Text = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");

                LoadPendingRequests();
                LoadApprovedRequests();
                LoadRejectedRequests();
            }
        }

        private void PopulateUserInfo(string empNo)
        {
            if (string.IsNullOrEmpty(empNo))
            {
                lblMessage.Text = "Session expired. Please login again.";
                lblMessage.CssClass = "text-danger";
                lblMessage.Visible = true;
                return;
            }

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                string query = "SELECT EMP_NAME, DEPT, EMP_POSITION, WORKING_TIME FROM VIS_STAFF WHERE EMP_NO = :empNo";

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(":empNo", OracleDbType.Varchar2).Value = empNo;

                    try
                    {
                        conn.Open();
                        OracleDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            string staffName = reader["EMP_NAME"].ToString();
                            string department = reader["DEPT"].ToString();
                            string position = reader["EMP_POSITION"].ToString();
                            string workingTime = reader["WORKING_TIME"].ToString();
                          
                            txtName.Text = staffName;
                            txtDepartment.Text = department;
                            txtEmpNo.Text = empNo;
                            lblWorkingTime.Text = workingTime;
                            Session["position"] = position;
                            lblMessage.Visible = false;
                        }
                        else
                        {
                            lblMessage.Text = "User not found in VIS_STAFF.";
                            lblMessage.CssClass = "text-danger";
                            lblMessage.Visible = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Text = "Database error: " + ex.Message;
                        lblMessage.CssClass = "text-danger";
                        lblMessage.Visible = true;
                    }
                }
            }
        }




        private void LoadPendingRequests()
        {
            gvPending.DataSource = GetDataByStatus("Pending");
            gvPending.DataBind();
        }

        private void LoadApprovedRequests()
        {
            gvApproved.DataSource = GetDataByStatus("Approved");
            gvApproved.DataBind();
        }

        private void LoadRejectedRequests()
        {
            gvRejected.DataSource = GetDataByStatus("Rejected");
            gvRejected.DataBind();
        }

        private DataTable GetDataByStatus(string status)
        {
            DataTable dt = new DataTable();
            string connStr = ConfigurationManager.AppSettings["ConnectionString"];

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();

               
                string query = @"
            SELECT DATE_OUT, TIME_OUT, TYPE, STAFF_NAME, CREATOR
            FROM VIS_EXITSTAFF
            WHERE APPROVAL_STATUS = :pStatus
              AND (STAFF_NAME = :pUser OR CREATOR = :pUser)
        ";

                DateTime startDate, endDate;
                bool hasStart = DateTime.TryParse(txtStartDate.Text, out startDate);
                bool hasEnd = DateTime.TryParse(txtEndDate.Text, out endDate);

                if (hasStart && hasEnd)
                {
                    query += " AND DATE_OUT BETWEEN :pStartDate AND :pEndDate";
                }

                query += " ORDER BY DATE_OUT DESC";

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.BindByName = true; 

                    // Add parameters (without colon)
                    cmd.Parameters.Add("pStatus", OracleDbType.Varchar2).Value = status;
                    cmd.Parameters.Add("pUser", OracleDbType.Varchar2).Value = txtName.Text.Trim();

                    if (hasStart && hasEnd)
                    {
                        cmd.Parameters.Add("pStartDate", OracleDbType.Date).Value = startDate;
                        cmd.Parameters.Add("pEndDate", OracleDbType.Date).Value = endDate;
                    }

                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        protected void gvApproved_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string creator = DataBinder.Eval(e.Row.DataItem, "CREATOR")?.ToString();
                Literal litRequestBy = (Literal)e.Row.FindControl("litRequestBy");

                if (litRequestBy != null)
                {
                    litRequestBy.Text = string.IsNullOrEmpty(creator)
                        ? "<span class='badge badge-success'>Self</span>"
                        : "<span class='badge badge-warning text-dark'>On Behalf</span>";
                }
            }
        }



        protected void gvPending_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string creator = DataBinder.Eval(e.Row.DataItem, "CREATOR")?.ToString();
                Literal litRequestBy = (Literal)e.Row.FindControl("litRequestBy");

                if (litRequestBy != null)
                {
                    litRequestBy.Text = string.IsNullOrEmpty(creator)
                        ? "<span class='badge badge-success'>Self</span>"
                        : "<span class='badge badge-warning text-dark'>On Behalf</span>";
                }
            }
        }
        protected void gvRejected_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string creator = DataBinder.Eval(e.Row.DataItem, "CREATOR")?.ToString();
                Literal litRequestBy = (Literal)e.Row.FindControl("litRequestBy");

                if (string.IsNullOrEmpty(creator))
                {
                    litRequestBy.Text = "<span class='badge badge-success'>Self</span>";
                }
                else
                {
                    litRequestBy.Text = "<span class='badge badge-warning text-dark'>On Behalf</span>";
                }
            }
        }


        protected void btnFilter_Click(object sender, EventArgs e)
        {

            LoadPendingRequests();
            LoadApprovedRequests();
            LoadRejectedRequests();
        }


        protected void gvPending_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetails1")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridView gv = (GridView)sender;

                // Adjust for paging
                int rowIndex = index % gv.PageSize;
                GridViewRow selectedRow = gv.Rows[rowIndex];

                // Retrieve bound values
               
                string dateOut = selectedRow.Cells[0].Text;
                string timeOut = selectedRow.Cells[1].Text;
                string type = selectedRow.Cells[2].Text;

                // Get STAFF_NAME (from session or row)
                string staffName = txtName.Text.Trim();

                // Optional: if you want to get 'Request By' from the Literal
                Literal lit = (Literal)selectedRow.FindControl("litRequestBy");
                string requestBy = lit?.Text;

                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    string query = @"SELECT * FROM VIS_EXITSTAFF 
WHERE TYPE = :type 
  AND TO_CHAR(DATE_OUT, 'YYYY-MM-DD') = :dateOut
  AND TIME_OUT = :timeOut
  AND (STAFF_NAME = :pUser OR CREATOR = :pUser)


";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(":type", OracleDbType.Varchar2).Value = type;
                        cmd.Parameters.Add(":dateOut", OracleDbType.Varchar2).Value = dateOut;
                        cmd.Parameters.Add(":timeOut", OracleDbType.Varchar2).Value = timeOut;
                        cmd.Parameters.Add(":pUser", OracleDbType.Varchar2).Value = staffName;



                        conn.Open();
                        OracleDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            lblName1.Text = reader["STAFF_NAME"].ToString();
                            lblDateOut1.Text = Convert.ToDateTime(reader["DATE_OUT"]).ToString("yyyy-MM-dd");
                            lblDepartment1.Text = reader["DEPARTMENT"].ToString();
                            lblReason1.Text = reader["REASONS"].ToString();
                            lblTimeOut1.Text = reader["TIME_OUT"].ToString();
                            lblType1.Text = reader["TYPE"].ToString();
                            lblRequestTo1.Text = reader["HODNAME"].ToString();
                        }
                        conn.Close();
                    }
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "Pop", "$('#viewModal1').modal('show');", true);
            }
            else if (e.CommandName == "WithdrawRequest")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridView gv = (GridView)sender;

                int rowIndex = index % gv.PageSize;
                GridViewRow selectedRow = gv.Rows[rowIndex];

                string dateOut = selectedRow.Cells[0].Text;
                string timeOut = selectedRow.Cells[1].Text;
                string type = selectedRow.Cells[2].Text;
                string staffName = txtName.Text.Trim();

                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    string delete = @"DELETE FROM VIS_EXITSTAFF
                          WHERE TYPE = :type 
                            AND TO_CHAR(DATE_OUT, 'YYYY-MM-DD') = :dateOut
                            AND TIME_OUT = :timeOut
                            AND (STAFF_NAME = :pUser OR CREATOR = :pUser)
                            AND APPROVAL_STATUS = 'Pending'";

                    using (OracleCommand cmd = new OracleCommand(delete, conn))
                    {
                        cmd.Parameters.Add(":type", OracleDbType.Varchar2).Value = type;
                        cmd.Parameters.Add(":dateOut", OracleDbType.Varchar2).Value = dateOut;
                        cmd.Parameters.Add(":timeOut", OracleDbType.Varchar2).Value = timeOut;
                        cmd.Parameters.Add(":pUser", OracleDbType.Varchar2).Value = staffName;

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        conn.Close();

                        if (rowsAffected > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "Success", "alert('Request withdrawn successfully.');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "Error", "alert('No matching request found or already processed.');", true);
                        }

                        LoadPendingRequests(); // Rebind the GridView after deletion
                    }
                }
            }
        }


        // Similarly for gvApproved and gvRejected RowCommand events, you can add if needed:
        protected void gvApproved_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetails2")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridView gv = (GridView)sender;

                // Adjust for paging
                int rowIndex = index % gv.PageSize;
                GridViewRow selectedRow = gv.Rows[rowIndex];

                // Retrieve bound values

                string dateOut = selectedRow.Cells[0].Text;
                string timeOut = selectedRow.Cells[1].Text;
                string type = selectedRow.Cells[2].Text;

                // Get STAFF_NAME (from session or row)
                string staffName = txtName.Text.Trim();

                // Optional: if you want to get 'Request By' from the Literal
                Literal lit = (Literal)selectedRow.FindControl("litRequestBy");
                string requestBy = lit?.Text;

                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    string query = @"SELECT * FROM VIS_EXITSTAFF 
WHERE TYPE = :type 
  AND TO_CHAR(DATE_OUT, 'YYYY-MM-DD') = :dateOut
  AND TIME_OUT = :timeOut
  AND (STAFF_NAME = :pUser OR CREATOR = :pUser)


";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(":type", OracleDbType.Varchar2).Value = type;
                        cmd.Parameters.Add(":dateOut", OracleDbType.Varchar2).Value = dateOut;
                        cmd.Parameters.Add(":timeOut", OracleDbType.Varchar2).Value = timeOut;
                        cmd.Parameters.Add(":pUser", OracleDbType.Varchar2).Value = staffName;



                        conn.Open();
                        OracleDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            lblName2.Text = reader["STAFF_NAME"].ToString();
                            lblDateOut2.Text = Convert.ToDateTime(reader["DATE_OUT"]).ToString("yyyy-MM-dd");
                            lblDepartment2.Text = reader["DEPARTMENT"].ToString();
                            lblReason2.Text = reader["REASONS"].ToString();
                            lblTimeOut2.Text = reader["TIME_OUT"].ToString();
                            lblType2.Text = reader["TYPE"].ToString();
                            lblRequestTo2.Text = reader["HODNAME"].ToString();
                            lblApproveDate.Text = Convert.ToDateTime(reader["DATE_OUT"]).ToString("yyyy-MM-dd");
                        }

                        conn.Close();
                    }
                }

                
                ScriptManager.RegisterStartupScript(this, GetType(), "Pop", "$('#viewModal2').modal('show');", true);
            }

        }

        protected void gvRejected_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetails")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridView gv = (GridView)sender;

                // Adjust for paging
                int rowIndex = index % gv.PageSize;
                GridViewRow selectedRow = gv.Rows[rowIndex];

                // Retrieve bound values

                string dateOut = selectedRow.Cells[0].Text;
                string timeOut = selectedRow.Cells[1].Text;
                string type = selectedRow.Cells[2].Text;

                // Get STAFF_NAME (from session or row)
                string staffName = txtName.Text.Trim();

                // Optional: if you want to get 'Request By' from the Literal
                Literal lit = (Literal)selectedRow.FindControl("litRequestBy");
                string requestBy = lit?.Text;

                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    string query = @"SELECT * FROM VIS_EXITSTAFF 
WHERE TYPE = :type 
  AND TO_CHAR(DATE_OUT, 'YYYY-MM-DD') = :dateOut
  AND TIME_OUT = :timeOut
  AND (STAFF_NAME = :pUser OR CREATOR = :pUser)


";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(":type", OracleDbType.Varchar2).Value = type;
                        cmd.Parameters.Add(":dateOut", OracleDbType.Varchar2).Value = dateOut;
                        cmd.Parameters.Add(":timeOut", OracleDbType.Varchar2).Value = timeOut;
                        cmd.Parameters.Add(":pUser", OracleDbType.Varchar2).Value = staffName;



                        conn.Open();
                        OracleDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            lblName.Text = reader["STAFF_NAME"].ToString();
                            lblDateOut.Text = Convert.ToDateTime(reader["DATE_OUT"]).ToString("yyyy-MM-dd");
                            lblDepartment.Text = reader["DEPARTMENT"].ToString();
                            lblReason.Text = reader["REASONS"].ToString();
                            lblTimeOut.Text = reader["TIME_OUT"].ToString();
                            lblType.Text = reader["TYPE"].ToString();
                            lblRequestTo.Text = reader["HODNAME"].ToString();

                            // Only for rejected requests
                            lblRejectionReason.Text = reader["REJECTIONREASON"]?.ToString();

                        }
                        conn.Close();
                    }
                }

              
                ScriptManager.RegisterStartupScript(this, GetType(), "Pop", "$('#viewModal').modal('show');", true);
            }

        }
    }
}

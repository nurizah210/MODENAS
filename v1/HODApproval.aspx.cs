using System;
using System.Configuration;
using System.Web.UI;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Web.UI.WebControls;
using System.DirectoryServices;
using System.Globalization;

namespace vms.v1
{
    public partial class HODApproval : Page
    {
        private string connStr = ConfigurationManager.AppSettings["ConnectionString"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
              
                string username = Session["username"]?.ToString();
                string hodEmail = Session["email"]?.ToString(); 

                if (string.IsNullOrEmpty(username))
                {
                    lblMessage.Text = "Username is not available.";
                    lblMessage.CssClass = "text-danger";
                    lblMessage.Visible = true;
                    Response.Redirect("~/v1/Login.aspx");
                }

                else
                {
                   
                 

                  
                        LoadRequestsBySelectedType();
                    
                }
            }
        }

        protected void ddlRequestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRequestsBySelectedType();
        }


        private void LoadRequestsBySelectedType()
        {
            string selectedType = ddlRequestType.SelectedValue;

            if (!string.IsNullOrEmpty(selectedType))
            {
                BindPendingRequests(selectedType);
            }

            else
            {
                gvPendingRequests.DataSource = null;
                gvPendingRequests.DataBind();

            }
        }


        private void BindPendingRequests(string requestType)
        {
            ViewState["CurrentRequestType"] = requestType;

            string hodEmail = Session["email"]?.ToString();

            if (string.IsNullOrEmpty(hodEmail))
            {
                lblMessage.Text = "Error: HOD email not found in session.";
                lblMessage.CssClass = "text-danger";
                lblMessage.Visible = true;
                return;
            }

            string query = @"
        SELECT STAFF_NAME, DEPARTMENT, REASONS,
               TO_CHAR(DATE_OUT, 'DD.MM.YYYY') AS FORMATTED_DATE_OUT, 
               TIME_OUT, PLATE_NO 
        FROM VIS_EXITSTAFF 
        WHERE APPROVAL_STATUS = 'Pending' 
          AND TYPE = :RequestType 
          AND HODNAME = :HODEmail";

            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        // Bind both parameters properly
                        cmd.Parameters.Add(":RequestType", OracleDbType.Varchar2).Value = requestType;
                        cmd.Parameters.Add(":HODEmail", OracleDbType.Varchar2).Value = hodEmail;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error: " + ex.Message;
                    lblMessage.CssClass = "text-danger";
                    lblMessage.Visible = true;
                }
            }

            gvPendingRequests.DataSource = dt;
            gvPendingRequests.DataBind();
        }







        protected void gvPendingRequests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Approve" || e.CommandName == "Reject")
            {
                

                string[] args = e.CommandArgument.ToString().Split('|');
                string staffName = args[0];
                string timeOut = args[1];



                string approvalStatus = e.CommandName == "Approve" ? "Approved" : "Rejected";

                
                UpdateApprovalStatus(staffName, approvalStatus, timeOut);


                BindPendingRequests(ddlRequestType.SelectedValue);


            }
        }

        private void UpdateApprovalStatus(string staffName, string approvalStatus, string timeOut)
        {
            string query;
            if (approvalStatus == "Approved")
            {
                 query = @"
    UPDATE VIS_EXITSTAFF 
    SET 
        APPROVAL_STATUS = :ApprovalStatus,
        APPROVAL_DATE = SYSTIMESTAMP
    WHERE 
        STAFF_NAME = :StaffName 
        AND TIME_OUT = :TimeOut";
            }

            else
            {
                query = @"
            UPDATE VIS_EXITSTAFF 
            SET 
                APPROVAL_STATUS = :ApprovalStatus
            WHERE 
                STAFF_NAME = :StaffName 
                AND TIME_OUT = :TimeOut";
            }

                using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter(":ApprovalStatus", approvalStatus));
                        cmd.Parameters.Add(new OracleParameter(":StaffName", staffName));
                        cmd.Parameters.Add(new OracleParameter(":TimeOut", timeOut));
                        cmd.ExecuteNonQuery();
                    }

                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error: " + ex.Message;
                    lblMessage.Visible = true;
                }
            }
        }



       
       
        protected void btnSubmitRejectionReason_Click(object sender, EventArgs e)
        {
            string commandArg = hfRejectCommandArg.Value;
            string reason = txtRejectionReason.Text.Trim();

            if (!string.IsNullOrEmpty(commandArg) && !string.IsNullOrEmpty(reason))
            {
                string[] args = commandArg.Split('|');
                string staffName = args[0];
                string timeOut = args[1];
            
              

                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    string query = @"
                UPDATE VIS_EXITSTAFF 
                SET APPROVAL_STATUS = 'Rejected', 
                    REJECTIONREASON = :Reason 
                WHERE STAFF_NAME = :StaffName 
                  AND TIME_OUT = :TimeOut 
                  ";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(":Reason", reason);
                        cmd.Parameters.Add(":StaffName", staffName);
                        cmd.Parameters.Add(":TimeOut", timeOut);
                   

                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            lblMessage.Text = "Rejection reason saved successfully.";
                            lblMessage.CssClass = "text-danger";
                        }
                        else
                        {
                            lblMessage.Text = "Failed to update request.";
                            lblMessage.CssClass = "text-warning";
                        }

                        lblMessage.Visible = true;
                        BindPendingRequests(ddlRequestType.SelectedValue);

                    }
                }
            }
            else
            {
                lblMessage.Text = "Rejection reason is required.";
                lblMessage.CssClass = "text-warning";
                lblMessage.Visible = true;
            }
        }
    }
}
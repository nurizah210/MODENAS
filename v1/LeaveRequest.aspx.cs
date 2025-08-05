using System;
using System.Configuration;
using System.Globalization;
using System.Web.UI;
using Oracle.ManagedDataAccess.Client;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web;
using System.Net.Mail;

namespace vms.v1
{
    public partial class LeaveRequest : Page
    {
        string connStr = ConfigurationManager.AppSettings["ConnectionString"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlPersonal.Visible = false;
                pnlOffice.Visible = false;

                string username = Session["username"]?.ToString();
                string staffName = Session["fullname"]?.ToString();
                string department = Session["department"]?.ToString();
                string empNo = Session["EmpID"]?.ToString();
                string position = Session["position"]?.ToString();

                if (!string.IsNullOrEmpty(username))
                {
                    txtName.Text = staffName;
                    txtDepartment.Text = department;
                    txtEmpNo.Text = empNo;
                 
                }
                else
                {
                    lblMessage.Text = "No user found in the session. Please log in.";
                    lblMessage.CssClass = "text-danger";
                    lblMessage.Visible = true;
                }


                LoadHODList();

            }
        }

        protected void ddlLeaveType_SelectedIndexChanged(object sender, EventArgs e)
        {

            // Check the selected value of the dropdown
            if (ddlLeaveType.SelectedValue == "Personal Reason")
            {
                // Make the panel visible if "Personal Reason" is selected
                pnlPersonal.Visible = true;
                pnlOffice.Visible = false; // Optional: Hide the office panel if Personal Reason is selected
            }

            else
            {
                // Make the panel visible if "Office Matter" is selected
                pnlOffice.Visible = true;
                pnlPersonal.Visible = false; // Optional: Hide the personal panel if Office Matter is selected
            }
        }



        protected void BtnSubmit_Click(object sender, EventArgs e)
        {

            string selectedReason = "";
            lblMessage.Visible = false;

            // Check which panel is visible and get the selected reason
            if (pnlPersonal.Visible)
            {
                selectedReason = ddlReason.SelectedValue; // This is from the personal panel
            }

            else if (pnlOffice.Visible)
            {
                selectedReason = DropDownList1.SelectedValue; // This is from the office panel
            }

            string empNo = Session["EmpID"]?.ToString();
            string dateOut = txtDateOut.Text.Trim();
            string staffName = txtName.Text.Trim();
            string department = txtDepartment.Text.Trim();
            string timeOut = txtTimeOut.Text.Trim();
            string replacement = rblReplacement.SelectedValue;
            string selectedHodEmail = ddlHodList.SelectedValue;
            string requestType = ddlLeaveType.SelectedValue;
            string plateNo = txtPlateNo.Text.Trim();
            string returnStatus = rblReturnStatus.SelectedValue;
            string HODName = ddlHodList.SelectedValue;
            string workingTime = "";
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();
                OracleTransaction transaction = conn.BeginTransaction();

              

                using (OracleCommand getWorkingTimeCmd = new OracleCommand("SELECT WORKING_TIME FROM VIS_STAFF WHERE EMP_NO = :EmpNo", conn))
                {
                    getWorkingTimeCmd.Transaction = transaction;
                    getWorkingTimeCmd.Parameters.Add("EmpNo", OracleDbType.Varchar2).Value = empNo;

                    using (OracleDataReader reader = getWorkingTimeCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            workingTime = reader["WORKING_TIME"]?.ToString() ?? "";
                        }
                        else
                        {
                            workingTime = "Unknown"; // Fallback in case employee not found
                        }
                    }
                }
            } 


            string breakTime = "1000, 1315, 1530";

            if (string.IsNullOrEmpty(selectedReason) || string.IsNullOrEmpty(dateOut) || string.IsNullOrEmpty(staffName) || string.IsNullOrEmpty(department))
            {
                lblMessage.Text = "Please fill in all required fields.";
                lblMessage.CssClass = "text-danger";
                lblMessage.Visible = true;
                return;
            }
            string selectedHodText = ddlHodList.SelectedItem.Text; 
            string hodDeptFromDropdown = "";

            if (selectedHodText.Contains("-"))
            {
                hodDeptFromDropdown = selectedHodText.Split('-')[1].Trim();
            }

            string altHodNotifyEmail = "";
            if (!hodDeptFromDropdown.Equals(department, StringComparison.OrdinalIgnoreCase))
            {
                // Different department → try find original HOD email
                altHodNotifyEmail = GetHodEmailByDepartment(department);
            }

            // Send email with optional notification to original HOD
         //   bool emailSent = SendApprovalEmail(selectedHodEmail, staffName, department, altHodNotifyEmail);
           // if (!emailSent) return;

     

            if (!DateTime.TryParseExact(dateOut, new[] {
                        "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "dd-MM-yyyy"
                    }

                    , CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateOut))
            {
                lblMessage.Text = "Invalid date format. Please use DD/MM/YYYY.";
                lblMessage.CssClass = "text-danger";
                lblMessage.Visible = true;
                return;
            }

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();
                OracleTransaction transaction = conn.BeginTransaction();

                try
                {
                    string staffQuery = @"INSERT INTO VIS_EXITSTAFF 
                (EMP_NO, STAFF_NAME, DEPARTMENT, REASONS, DATE_OUT, REPLACEMENT, TIME_OUT, APPROVAL_STATUS, 
                RETURN_STATUS, TYPE, PLATE_NO, HODNAME, WORKING_TIME, BREAK_TIME)
                VALUES 
                (:EmpID, :StaffName, :Department, :Reason, :DateOut, :Replacement, :TimeOut, 'Pending',
                :ReturnStatus, :RequestType, :PlateNo, :HODName, :WorkingTime, :BreakTime)";

                    using (OracleCommand staffCmd = new OracleCommand(staffQuery, conn))
                    {

                        staffCmd.Transaction = transaction;
                        staffCmd.Parameters.Add("EmpID", OracleDbType.Varchar2).Value = empNo;
                        staffCmd.Parameters.Add("StaffName", OracleDbType.Varchar2).Value = staffName;
                        staffCmd.Parameters.Add("Department", OracleDbType.Varchar2).Value = department;
                        staffCmd.Parameters.Add("Reason", OracleDbType.Varchar2).Value = selectedReason;
                        staffCmd.Parameters.Add("DateOut", OracleDbType.Date).Value = parsedDateOut;
                        staffCmd.Parameters.Add("Replacement", OracleDbType.Varchar2).Value = replacement;
                        staffCmd.Parameters.Add("TimeOut", OracleDbType.Varchar2).Value = timeOut;
                        staffCmd.Parameters.Add("ReturnStatus", OracleDbType.Varchar2).Value = returnStatus;
                        staffCmd.Parameters.Add("RequestType", OracleDbType.Varchar2).Value = requestType;
                        staffCmd.Parameters.Add("PlateNo", OracleDbType.Varchar2).Value = plateNo;
                        staffCmd.Parameters.Add("HODName", OracleDbType.Varchar2).Value = HODName;
                        staffCmd.Parameters.Add("WorkingTime", OracleDbType.Varchar2).Value = workingTime;
                        staffCmd.Parameters.Add("BreakTime", OracleDbType.Varchar2).Value = breakTime;

                        staffCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    lblMessage.Text = "Successfully submitted and email sent.";
                    lblMessage.CssClass = "text-success";
                    lblMessage.Visible = true;
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    lblMessage.Text = "Error while saving data: " + ex.Message;
                    lblMessage.CssClass = "text-danger";
                    lblMessage.Visible = true;
                }
            }

            string message = $"Request submitted by {staffName} from {department}. Reason: {selectedReason} . Please wait for HOD approval!";
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", $"alert('{message}');", true);

            txtTimeOut.Text = "";

            rblReplacement.ClearSelection();


            ddlHodList.ClearSelection();
            ddlLeaveType.ClearSelection();

            txtPlateNo.Text = "";


            rblReturnStatus.ClearSelection();
        }
        private void LoadHODList()
        {
            try
            {
                List<ListItem> hodList = new List<ListItem>();

                string query = "SELECT EMP_NAME, DEPT, EMAIL FROM VIS_STAFF WHERE UPPER(EMP_POSITION) = 'MANAGER'";

                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader["EMP_NAME"].ToString();
                            string dept = reader["DEPT"].ToString();
                            string email = reader["EMAIL"].ToString();

                            string displayText = $"{name} - {dept}";
                            hodList.Add(new ListItem(displayText, email));
                        }
                    }
                }

                ddlHodList.Items.Clear(); // Clear any existing items
                ddlHodList.Items.Add(new ListItem("- Select Head Of Department -", "")); // Add default first

                // Now add all actual HOD items manually
                ddlHodList.Items.AddRange(hodList.ToArray());
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error loading HOD list: " + ex.Message;
                lblMessage.CssClass = "text-danger";
                lblMessage.Visible = true;
            }
        }


        private string GetHodEmailByDepartment(string department)
        {
            string email = "";

            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    string query = @"SELECT EMAIL 
                             FROM VIS_STAFF 
                             WHERE UPPER(DEPT) = UPPER(:Dept) 
                               AND UPPER(EMP_POSITION) = 'MANAGER'";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("Dept", OracleDbType.Varchar2).Value = department;

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            email = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Optional: log error
            }

            return email;
        }
        private bool SendApprovalEmail(string hodEmail, string staffName, string department, string notifyOriginalHodEmail)
        {
            string subject = "Approval Request for Personal Reason";
            string body = $"Dear HOD,\n\nYou have a pending request for approval from {staffName} ({department}).\n" +
                          $"Please log in to the system to approve or reject the request:\n" +
                          $"{Request.Url.GetLeftPart(UriPartial.Authority)}/HODApproval.aspx?staffEmail={HttpUtility.UrlEncode(staffName)}\n\nBest regards,\nSMS System";

            string notifyBody = $"Dear HOD,\n\n" +
                $"This is to inform you that a leave request from {staffName} ({department}) was submitted for approval to another HOD outside your department.\n\n" +
                "No action is required from you.\n\nBest regards,\nSMS System";

            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
                mail.To.Add(hodEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = false;

                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"]);
                smtp.Send(mail);

                // Notify original HOD if needed
                if (!string.IsNullOrEmpty(notifyOriginalHodEmail) && !notifyOriginalHodEmail.Equals(hodEmail, StringComparison.OrdinalIgnoreCase))
                {
                    MailMessage notifyMail = new MailMessage();
                    notifyMail.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
                    notifyMail.To.Add(notifyOriginalHodEmail);
                    notifyMail.Subject = "Notification: Alternative HOD Approval";
                    notifyMail.Body = notifyBody;
                    notifyMail.IsBodyHtml = false;

                    smtp.Send(notifyMail);
                }

                return true;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error sending email: " + ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "");
                lblMessage.CssClass = "text-danger";
                lblMessage.Visible = true;
                return false;
            }
        }

     
    }
}
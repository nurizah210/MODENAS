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
    public partial class OnBehalfReq2 : Page
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
                string empNo = Session["EmpID"]?.ToString();
                string position = Session["position"]?.ToString();

                if (!string.IsNullOrEmpty(username))
                {
                    txtName.Text = staffName;


                    // Get SECT from database
                    string sect = GetSectionByEmpID(empNo);

                    // Populate Requestor dropdown based on SECT
                    FillRequestorDropdown(sect);

                    // Auto-select creator as the default requestor (optional)
                    if (ddlReqName.Items.FindByValue(empNo) != null)
                        ddlReqName.SelectedValue = empNo;
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


        private string GetSectionByEmpID(string empId)
        {
            string sect = "";

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                string query = "SELECT SECT FROM VIS_STAFF WHERE EMP_NO = :empId";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("empId", empId));
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        sect = result.ToString();
                }
            }
            return sect;
        }

        private void FillRequestorDropdown(string sect)
        {

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                string creatorEmpNo = Session["EmpID"]?.ToString();
                string creatorSect = Session["section"]?.ToString();

                string query = @"SELECT EMP_NO, EMP_NAME 
                 FROM VIS_STAFF 
                 WHERE SECT = :sect AND EMP_NO != :creatorEmpNo 
                 ORDER BY EMP_NAME";


                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add("sect", OracleDbType.Varchar2).Value = creatorSect;
                    cmd.Parameters.Add("creatorEmpNo", OracleDbType.Varchar2).Value = creatorEmpNo;

                    conn.Open();
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        ddlReqName.Items.Clear();
                        ddlReqName.Items.Add(new ListItem("-- Select Requestor --", ""));

                        while (reader.Read())
                        {
                            string empNo = reader["EMP_NO"].ToString();
                            string empName = reader["EMP_NAME"].ToString();
                            ddlReqName.Items.Add(new ListItem(empName, empNo));
                        }
                    }
                }

            }
        }
        protected void BtnSubmit_Click(object sender, EventArgs e)
        {
            string selectedReason = "";
            lblMessage.Visible = false;

            if (pnlPersonal.Visible)
            {
                selectedReason = ddlReason.SelectedValue;
            }
            else if (pnlOffice.Visible)
            {
                selectedReason = DropDownList1.SelectedValue;
            }

            string dateOut = txtDateOut.Text.Trim();
            string creatorName = Session["fullname"]?.ToString();  // only for email
            string department = Session["department"]?.ToString();
            string timeOut = txtTimeOut.Text.Trim();
            string replacement = rblReplacement.SelectedValue;
            string selectedHodEmail = ddlHodList.SelectedValue;
            string requestType = ddlLeaveType.SelectedValue;
            string plateNo = txtPlateNo.Text.Trim();
            string returnStatus = rblReturnStatus.SelectedValue;
            string hodName = ddlHodList.SelectedValue;
            string workingTime = "";
            // Requestor (dropdown selection)
            string requestorId = ddlReqName.SelectedValue;
            string requestorName = ddlReqName.SelectedItem.Text;
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();
                OracleTransaction transaction = conn.BeginTransaction();



                using (OracleCommand getWorkingTimeCmd = new OracleCommand("SELECT WORKING_TIME FROM VIS_STAFF WHERE EMP_NO = :EmpNo", conn))
                {
                    getWorkingTimeCmd.Transaction = transaction;
                    getWorkingTimeCmd.Parameters.Add("EmpNo", OracleDbType.Varchar2).Value = requestorId;


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
            string breakTime = "1030, 1330, 1540";

            

            if (string.IsNullOrEmpty(selectedReason) || string.IsNullOrEmpty(dateOut) || string.IsNullOrEmpty(requestorName) || string.IsNullOrEmpty(department))
            {
                lblMessage.Text = "Please fill in all required fields.";
                lblMessage.CssClass = "text-danger";
                lblMessage.Visible = true;
                return;
            }

            // HOD check
            string selectedHodText = ddlHodList.SelectedItem.Text;
            string hodDeptFromDropdown = "";
            if (selectedHodText.Contains("-"))
            {
                hodDeptFromDropdown = selectedHodText.Split('-')[1].Trim();
            }

            string altHodNotifyEmail = "";
            if (!hodDeptFromDropdown.Equals(department, StringComparison.OrdinalIgnoreCase))
            {
                altHodNotifyEmail = GetHodEmailByDepartment(department);
            }

        
          //  bool emailSent = SendApprovalEmail(selectedHodEmail, requestorName, department, altHodNotifyEmail, creatorName);

             // if (!emailSent) return;


            if (!DateTime.TryParseExact(dateOut, new[] {
            "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "dd-MM-yyyy"
        }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateOut))
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
(EMP_NO, STAFF_NAME, DEPARTMENT, REASONS, DATE_OUT, REPLACEMENT, TIME_OUT, 
 APPROVAL_STATUS, RETURN_STATUS, TYPE, PLATE_NO, HODNAME, WORKING_TIME, BREAK_TIME, CREATOR) 
VALUES 
(:EmpID, :StaffName, :Department, :Reason, :DateOut, :Replacement, :TimeOut, 
 'Pending', :ReturnStatus, :RequestType, :PlateNo, :HODName, :WorkingTime, :BreakTime, :Creator)";


                    using (OracleCommand staffCmd = new OracleCommand(staffQuery, conn))
                    {
                        staffCmd.Transaction = transaction;


                        staffCmd.Parameters.Add("EmpID", OracleDbType.Varchar2).Value = requestorId;
                        staffCmd.Parameters.Add("StaffName", OracleDbType.Varchar2).Value = requestorName;
                        staffCmd.Parameters.Add("Department", OracleDbType.Varchar2).Value = department;
                        staffCmd.Parameters.Add("Reason", OracleDbType.Varchar2).Value = selectedReason;
                        staffCmd.Parameters.Add("DateOut", OracleDbType.Date).Value = parsedDateOut;
                        staffCmd.Parameters.Add("Replacement", OracleDbType.Varchar2).Value = replacement;
                        staffCmd.Parameters.Add("TimeOut", OracleDbType.Varchar2).Value = timeOut;
                        staffCmd.Parameters.Add("ReturnStatus", OracleDbType.Varchar2).Value = returnStatus;
                        staffCmd.Parameters.Add("RequestType", OracleDbType.Varchar2).Value = requestType;
                        staffCmd.Parameters.Add("PlateNo", OracleDbType.Varchar2).Value = plateNo;
                        staffCmd.Parameters.Add("HODName", OracleDbType.Varchar2).Value = hodName;
                        staffCmd.Parameters.Add("WorkingTime", OracleDbType.Varchar2).Value = workingTime;
                        staffCmd.Parameters.Add("BreakTime", OracleDbType.Varchar2).Value = breakTime;
                        staffCmd.Parameters.Add("Creator", OracleDbType.Varchar2).Value = creatorName;



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

            // Confirmation
            string message = $"Exit request submitted for {requestorName}. Reason: {selectedReason}. Awaiting HOD approval.";
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", $"alert('{message}');", true);

            // Clear form
            txtTimeOut.Text = "";
            rblReplacement.ClearSelection();
            ddlHodList.ClearSelection();
            ddlLeaveType.ClearSelection();
            txtPlateNo.Text = "";
            rblReturnStatus.ClearSelection();
        }
        private bool SendApprovalEmail(string hodEmail, string requestorName, string department, string notifyOriginalHodEmail, string creatorName)
        {
            string subject = "Approval Request for Exit on Behalf";
            string body = $"Dear HOD,\n\n" +
                          $"You have a pending request for approval *on behalf* of {requestorName} ({department}).\n" +
                          $"This request was submitted by: {creatorName}.\n\n" +
                          $"Please log in to the system to approve or reject the request:\n" +
                          $"{Request.Url.GetLeftPart(UriPartial.Authority)}/HODApproval.aspx?staffName={HttpUtility.UrlEncode(requestorName)}\n\n" +
                          $"Best regards,\nSMS System";

            string notifyBody = $"Dear HOD,\n\n" +
                                $"This is to inform you that a leave request for {requestorName} ({department}) was submitted for approval to another HOD outside your department.\n" +
                                $"The request was made by: {creatorName}.\n\n" +
                                $"No action is required from you.\n\nBest regards,\nSMS System";

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
    }
}
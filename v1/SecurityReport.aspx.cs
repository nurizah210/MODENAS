using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Net.Mail;
using System.Globalization;
using System.Linq;
using ClosedXML.Excel;
using System.Web;


namespace vms.v1
{
    public partial class SecurityReport : Page
    {
        string connStr = ConfigurationManager.AppSettings["ConnectionString"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<string> guards = GetGuardsFromDatabase();

              

                // Bind to all relevant dropdowns
                BindGuards(ddlPost1Guard1, guards);
                BindGuards(ddlPost1Guard2, guards);
                BindGuards(ddlPost2Guard1, guards);
                BindGuards(ddlPost2Guard2, guards);
                BindGuards(ddlPost2Guard3, guards);
                BindGuards(ddlEmosGuard1, guards);
                BindGuards(ddlEmosGuard2, guards);
            }
        }

        private void BindGuards(DropDownList ddl, List<string> guards)
        {
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem("- Select Guard -", ""));

            foreach (string name in guards)
            {
                ddl.Items.Add(new ListItem(name));
            }
        }
        private List<string> GetGuardsFromDatabase()
        {
            List<string> guards = new List<string>();

            try
            {
               
                string query = "SELECT NAME FROM VIS_GUARD ";

                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            guards.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                guards.Add("ERROR: " + ex.Message);
            }

            return guards;
        }



        protected void btnSubmitReport_Click(object sender, EventArgs e)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    string sql = @"INSERT INTO SECURITY_REPORT (
                REPORT_DATE, SHIFT_TIME, DAY_NAME,
                POST1_GUARD1, POST1_GUARD2,
                POST2_GUARD1, POST2_GUARD2, POST2_GUARD3,
                EMOS_GUARD1, EMOS_GUARD2,
                CCTV_WORKING, CCTV_OFFLINE, CCTVABNORM,
                CONTRACTORS, INCIDENT_TITLE, INCIDENT_DESC, ACTION_TAKEN,
                INCIDENT_PHOTO_PATH
            ) VALUES (
                :REPORT_DATE, :SHIFT_TIME, :DAY_NAME,
                :POST1_GUARD1, :POST1_GUARD2,
                :POST2_GUARD1, :POST2_GUARD2, :POST2_GUARD3,
                :EMOS_GUARD1, :EMOS_GUARD2,
                :CCTV_WORKING, :CCTV_OFFLINE, :CCTVABNORM,
                :CONTRACTORS, :INCIDENT_TITLE, :INCIDENT_DESC, :ACTION_TAKEN,
                :INCIDENT_PHOTO_PATH
            )";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        // ✅ Bind all other params correctly, e.g.
                        if (!DateTime.TryParse(txtDate.Text, out DateTime reportDate))
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Invalid date format');", true);
                            return;
                        }

                        cmd.Parameters.Add(":REPORT_DATE", OracleDbType.Date).Value = reportDate;

                        cmd.Parameters.Add(":SHIFT_TIME", txtShiftTime.Text);
                        cmd.Parameters.Add(":DAY_NAME", ddlDay.SelectedValue);
                        cmd.Parameters.Add(":POST1_GUARD1", ddlPost1Guard1.SelectedValue);
                        cmd.Parameters.Add(":POST1_GUARD2", ddlPost1Guard2.SelectedValue);
                        cmd.Parameters.Add(":POST2_GUARD1", ddlPost2Guard1.SelectedValue);
                        cmd.Parameters.Add(":POST2_GUARD2", ddlPost2Guard2.SelectedValue);
                        cmd.Parameters.Add(":POST2_GUARD3", ddlPost2Guard3.SelectedValue);
                        cmd.Parameters.Add(":EMOS_GUARD1", ddlEmosGuard1.SelectedValue);
                        cmd.Parameters.Add(":EMOS_GUARD2", ddlEmosGuard2.SelectedValue);
                        cmd.Parameters.Add(":CCTV_WORKING", txtCCTVWorking.Text);
                        cmd.Parameters.Add(":CCTV_OFFLINE", txtCCTVOffline.Text);
                        cmd.Parameters.Add(":CCTVABNORM", txtAbnormalities.Text);
                        cmd.Parameters.Add(":CONTRACTORS", txtContractors.Text);
                        cmd.Parameters.Add(":INCIDENT_TITLE", txtIncidentTitle.Text);
                        cmd.Parameters.Add(":INCIDENT_DESC", txtIncidentDesc.Text);
                        cmd.Parameters.Add(":ACTION_TAKEN", txtActionTaken.Text);

                        // ✅ Save files & get photo paths:
                        List<string> photoPaths = SaveFiles(FileUpload1, "IncidentPhotos");
                        string allPhotoPaths = string.Join(";", photoPaths);

                        cmd.Parameters.Add(":INCIDENT_PHOTO_PATH", allPhotoPaths);

                        cmd.ExecuteNonQuery();

                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Report submitted successfully.');", true);
                        ClearForm();
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }
        }







        private void ClearForm()
        {
            txtDate.Text = "";
            txtShiftTime.Text = "";
            ddlDay.SelectedIndex = 0;
            ddlPost1Guard1.SelectedIndex = 0;
            ddlPost1Guard2.SelectedIndex = 0;
            ddlPost2Guard1.SelectedIndex = 0;
            ddlEmosGuard1.SelectedIndex = 0;
            ddlEmosGuard2.SelectedIndex = 0;
            txtCCTVWorking.Text = "";
            txtCCTVOffline.Text = "";
            txtContractors.Text = "";
            txtIncidentTitle.Text = "";
            txtIncidentDesc.Text = "";
            txtActionTaken.Text = "";
           
        }
        private List<string> SaveFiles(FileUpload fileUpload, string folderName)
        {
            List<string> photoPaths = new List<string>();

            foreach (HttpPostedFile postedFile in fileUpload.PostedFiles)
            {
                if (postedFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(postedFile.FileName);
                    string folderPath = Server.MapPath("~/" + folderName);

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = Path.Combine(folderPath, fileName);
                    postedFile.SaveAs(filePath);

                    photoPaths.Add("~/" + folderName + "/" + fileName);
                }
            }

            return photoPaths;
        }





        private void SendCCTVEmail(string toEmail, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("no-reply@yourdomain.com"); // No-reply email sender
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = $@"
    <h2>CCTV Status Report</h2>
    <p><strong>CCTV Working:</strong> {txtCCTVWorking}</p>
    <p><strong>CCTV Offline:</strong> {txtCCTVOffline}</p>
    <p><strong>OFFLINE:</strong> {txtAbnormalities}</p>
    <br />
    <p style='color:red;'><strong>Action Required:</strong> Please investigate and take necessary action as soon as possible.</p>
    <p>Thank you.</p>
";


                SmtpClient smtp = new SmtpClient("smtp.your-email-provider.com");
                smtp.Port = 587; // or your SMTP port
                smtp.Credentials = new System.Net.NetworkCredential("smtp-username", "smtp-password");
                smtp.EnableSsl = true;

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error sending email: " + ex.Message);
            }
        }

      

    }
}
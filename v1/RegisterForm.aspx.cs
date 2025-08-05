using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

namespace vms.v1
{
    public partial class RegisterForm : Page
    {
        string connStr = ConfigurationManager.AppSettings["ConnectionString"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                // Populate Location dropdown
                ddlLocation.Items.Clear();
                ddlLocation.Items.Add(new ListItem("Select Location", ""));
                ddlLocation.Items.Add(new ListItem("Main Office", "Main Office"));
                ddlLocation.Items.Add(new ListItem("R&D Office", "R&D Office"));
                ddlLocation.Items.Add(new ListItem("Learning Centre", "Learning Centre"));
                ddlLocation.Items.Add(new ListItem("Block A", "Block A"));
                ddlLocation.Items.Add(new ListItem("Block B", "Block B"));
                ddlLocation.Items.Add(new ListItem("Block C", "Block C"));
                

            }

            else
            {
                // Call SetFormVisibility() on every postback to ensure correct panel is shown
                SetFormVisibility();
            }
        }


        protected void ddlRecordType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFormVisibility();
        }

        private void SetFormVisibility()
        {
            // Hide both forms first
            pnlVehicleForm.Visible = false;
            pnlTNBForm.Visible = false;
            pnlParkingRentalForm.Visible = false;
            pnlLateStaffForm.Visible = false;
            pnlStaffMovement.Visible = false;
            string recordType = ddlRecordType.SelectedValue.ToLower();

            if (recordType == "vehicle in/out")
            {
                pnlVehicleForm.Visible = true;
            }

            else if (recordType == "tnb")
            {
                pnlTNBForm.Visible = true;
            }

            else if (recordType == "parking")
            {
                pnlParkingRentalForm.Visible = true;
            }

            else if (recordType == "late staff")
            {
                pnlLateStaffForm.Visible = true;
            }

            else if (recordType == "staff movement")
            {
                pnlStaffMovement.Visible = true;
            }
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            string IC = txtIC.Value.Trim();
            string name = txtName.Value.Trim();
            string company = txtCompany.Value.Trim();
            string plateNo = txtNoPlate.Value.Trim();
            string location = ddlLocation.SelectedValue;
            string vehicle = ddlVehicleType.SelectedValue;

            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    string query = @"INSERT INTO VIS_VEHICLE 
(IC, NAME, COMPANY, PLATE_NO, LOCATION, VEHICLE_TYPE) VALUES (:IC, :Name, :Company, :PlateNo, :Location, :VehicleType)";


                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(":IC", IC);
                        cmd.Parameters.Add(":Name", name);
                        cmd.Parameters.Add(":Company", company);
                        cmd.Parameters.Add(":PlateNo", plateNo);
                        cmd.Parameters.Add(":Location", location);
                        cmd.Parameters.Add(":VehicleType", vehicle);

                        cmd.ExecuteNonQuery();
                    }
                }

                ScriptManager.RegisterStartupScript(this, this.GetType(), "Redirect",
       "alert('Success! Redirecting...'); window.location.href='Post1.aspx';", true);


                // Clear form fields
                txtIC.Value = "";
                txtName.Value = "";
                txtCompany.Value = "";
                txtNoPlate.Value = "";
                ddlLocation.SelectedIndex = 0;
                ddlVehicleType.SelectedIndex = 0;
            }

            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", "showToast('Error: " + ex.Message + "');", true);
            }
        }

        protected void btnSaveTNB_Click(object sender, EventArgs e)
        {
            // Retrieve input values from the form
            string noPlate = txtTNBNoPlate.Value.Trim();
            string name = txtTNBName.Value.Trim();
            string icNo = txtTNBIC.Value.Trim();
            string purpose = txtTNBPurpose.Value.Trim();


            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    string query = @"INSERT INTO VIS_TNB
(NO_PLATE, NAME, IC_NO, PURPOSE, TIME_IN) VALUES (:NoPlate, :Name, :ICNo, :Purpose, SYSDATE)"; // TIME_IN will be set to current date/time


                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(":NoPlate", noPlate);
                        cmd.Parameters.Add(":Name", name);
                        cmd.Parameters.Add(":ICNo", icNo);
                        cmd.Parameters.Add(":Purpose", purpose);

                        cmd.ExecuteNonQuery();
                    }
                }

                ScriptManager.RegisterStartupScript(this, this.GetType(), "Redirect",
                "alert('Success! Redirecting...'); window.location.href='Post1.aspx';", true);

                // Clear fields
                txtTNBNoPlate.Value = "";
                txtTNBName.Value = "";
                txtTNBIC.Value = "";
                txtTNBPurpose.Value = "";
            }

            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", "showToast('Error: " + ex.Message + "');", true);
            }
        }


        protected void btnSaveParkingRental_Click(object sender, EventArgs e)
        {
            // Get values from the UI
            string driverName = txtDriverName.Value.Trim();
            string vehicleType = ddlVehicleTypeIn.SelectedValue;
            string noPlate = txtNoPlateIn.Value.Trim();
            string purpose = ddlPurpose.SelectedValue;
            string company = txtCompanyParking.Value.Trim();


            // Optional: Validate required fields
            if (string.IsNullOrWhiteSpace(driverName) || string.IsNullOrWhiteSpace(vehicleType) || string.IsNullOrWhiteSpace(noPlate) || string.IsNullOrWhiteSpace(purpose) || string.IsNullOrWhiteSpace(company))
            {

                return;
            }


            using (OracleConnection conn = new OracleConnection(connStr))
            {
                string query = @"INSERT INTO VIS_PARKING 
(DRIVER_NAME, VEHICLE_TYPE, NO_PLATE, PURPOSE, COMPANY, TIME_IN) VALUES (:driverName, :vehicleType, :noPlate, :purpose, :company, SYSTIMESTAMP)";


                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(":driverName", driverName);
                    cmd.Parameters.Add(":vehicleType", vehicleType);
                    cmd.Parameters.Add(":noPlate", noPlate);
                    cmd.Parameters.Add(":purpose", purpose);
                    cmd.Parameters.Add(":company", company);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Redirect", "alert('Success! Redirecting...'); window.location.href='Post1.aspx';", true);

                        txtDriverName.Value = "";
                        ddlVehicleTypeIn.SelectedIndex = 0;
                        txtNoPlateIn.Value = "";
                        ddlPurpose.SelectedIndex = 0;
                        txtCompanyParking.Value = "";
                    }

                    catch (Exception ex)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", "showToast('Error: " + ex.Message + "');", true);
                    }
                }
            }
        }

        protected void btnSearchEmp_Click(object sender, EventArgs e)
        {
            string empNo = txtEmpNoLate.Value.Trim();

            if (!string.IsNullOrEmpty(empNo))
            {
                var employeeDetails = new StaffHelper().GetEmployeeDetails(empNo);

                if (employeeDetails != null)
                {
                    txtEmpNameLate.Value = employeeDetails.Name;
                    txtDeptLate.Value = employeeDetails.Department;

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess", $"showToast('Employee found: {employeeDetails.Name}');", true);
                }
                else
                {
                    txtEmpNameLate.Value = "";
                    txtDeptLate.Value = "";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "showToast('Employee not found in database.');", true);
                }
            }
        }



        public class StaffHelper
        {
            private string connStr = ConfigurationManager.AppSettings["ConnectionString"];

            public EmployeeDetails GetEmployeeDetails(string empNo)
            {
                EmployeeDetails employee = null;

                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    string query = "SELECT EMP_NAME, DEPT FROM VIS_STAFF WHERE EMP_NO = :EmpNo";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("EmpNo", OracleDbType.Varchar2).Value = empNo;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                employee = new EmployeeDetails
                                {
                                    Name = reader["EMP_NAME"].ToString(),
                                    Department = reader["DEPT"].ToString()
                                };
                            }
                        }
                    }
                }

                return employee;
            }
        }


        public class EmployeeDetails
        {
            public string Name { get; set; }
            public string Department { get; set; }
            public string Email { get; set; }    // Optional, if you plan to use later
            public string Title { get; set; }    // Optional, if you plan to use later
        }



        protected void GetEmployeeDetailsFromDB()
        {
            string empNo = txtEmpNoLate.Value;

            if (!string.IsNullOrEmpty(empNo))
            {
                var employeeDetails = new StaffHelper().GetEmployeeDetails(empNo);

                if (employeeDetails != null)
                {
                    txtEmpNameLate.Value = employeeDetails.Name;
                    txtDeptLate.Value = employeeDetails.Department;
                }
                else
                {
                    txtEmpNameLate.Value = "Employee Not Found";
                    txtDeptLate.Value = "N/A";
                }
            }
        }



        protected void btnSaveLateStaff_Click(object sender, EventArgs e)
        {
            string empName = txtEmpNameLate.Value.Trim();
            string empNo = txtEmpNoLate.Value.Trim();
            string dept = txtDeptLate.Value.Trim();
            string phone = txtPhoneLate.Value.Trim();
            string reason = ddlReasonLate.SelectedValue;


            // Show the modal for additional reporter info
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "$('#reportModal').modal('show');", true);
        }

        protected void btnSubmitReport_Click(object sender, EventArgs e)
        {
            string dateReport = Request.Form["txtDateReportLate"];
            string securityNote = Request.Form["txtSecurityNote"];

            // Retrieve from Session
            string empName = txtEmpNameLate.Value.Trim();
            string empNo = txtEmpNoLate.Value.Trim();
            string dept = txtDeptLate.Value.Trim();
            string phone = txtPhoneLate.Value.Trim();
            string reason = ddlReasonLate.SelectedValue;


            using (OracleConnection conn = new OracleConnection(connStr))
            {
                string query = @"
 INSERT INTO VIS_LATESTAFF (EMP_NAME, EMP_NO, DEPARTMENT, PHONE_NO, REASON_LATE,
                    REPORT_DATE, SECURITY_NOTE, TIME_IN) VALUES (:empName, :empNo, :department, :phoneNo, :reasonLate,
                    TO_DATE(:reportDate, 'YYYY-MM-DD'), :securityNote, SYSTIMESTAMP)";


                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(":empName", empName);
                    cmd.Parameters.Add(":empNo", empNo);
                    cmd.Parameters.Add(":department", dept);
                    cmd.Parameters.Add(":phoneNo", phone);
                    cmd.Parameters.Add(":reasonLate", reason);
                    cmd.Parameters.Add(":reportDate", dateReport);
                    cmd.Parameters.Add(":securityNote", securityNote);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Redirect", "alert('Success! Redirecting...'); window.location.href='Post1.aspx';", true);

                    }

                    catch (Exception ex)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", "showToast('Error: " + ex.Message + "');", true);
                    }
                }
            }

            // Optionally clear session
            txtEmpNameLate.Value = string.Empty;
            txtEmpNoLate.Value = string.Empty;
            txtDeptLate.Value = string.Empty;
            txtPhoneLate.Value = string.Empty;
            ddlReasonLate.SelectedValue = string.Empty;
        }

        protected void btnSearchOTEmp_Click(object sender, EventArgs e)
        {
            string empNo = txtOTEmpNo.Value.Trim();

            if (!string.IsNullOrEmpty(empNo))
            {
                var employeeDetails = new StaffHelper().GetEmployeeDetails(empNo);

                if (employeeDetails != null)
                {
                    txtOTName.Value = employeeDetails.Name;
                    txtOTDepartment.Value = employeeDetails.Department;
                }
                else
                {
                    txtOTName.Value = "";
                    txtOTDepartment.Value = "";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Employee not found in database.');", true);
                }
            }
        }


        protected void btnSaveStaffMove_Click(object sender, EventArgs e)
        {
            string noPlate = txtOTNoPlate.Text.Trim();
            string name = txtOTName.Value.Trim();
            string empNo = txtOTEmpNo.Value.Trim();
            string block = ddlBlock.SelectedValue;
            string department = txtOTDepartment.Value.Trim();
            string purpose = txtOTPurpose.Value.Trim();

            // Validate input
            if (string.IsNullOrWhiteSpace(noPlate) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(empNo) || string.IsNullOrWhiteSpace(block) || string.IsNullOrWhiteSpace(department) || string.IsNullOrWhiteSpace(purpose))
            {
                lblMsg.Text = "Please fill in all required fields.";
                lblMsg.CssClass = "text-danger";
                return;
            }

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                string query = @"
 INSERT INTO VIS_STAFFMOVE (NO_PLATE, EMP_NAME, EMP_NO, BLOCK, DEPARTMENT, PURPOSE, TIME_IN) VALUES (:noPlate, :name, :empNo, :block, :department, :purpose, SYSTIMESTAMP)";


                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(":noPlate", noPlate);
                    cmd.Parameters.Add(":name", name);
                    cmd.Parameters.Add(":empNo", empNo);
                    cmd.Parameters.Add(":block", block);
                    cmd.Parameters.Add(":department", department);
                    cmd.Parameters.Add(":purpose", purpose);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Redirect", "alert('Success! Redirecting...'); window.location.href='Post1.aspx';", true);

                        txtOTNoPlate.Text = "";
                        txtOTName.Value = "";
                        txtOTEmpNo.Value = "";
                        ddlBlock.SelectedIndex = 0;
                        txtOTDepartment.Value = "";
                        txtOTPurpose.Value = "";
                    }

                    catch (Exception ex)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", "showToast('Error: " + ex.Message + "');", true);
                    }
                }
            }
        }


      
    }
}
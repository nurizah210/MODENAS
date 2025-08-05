using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace vms.v1
{
    public partial class ManageStaff : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.AppSettings["ConnectionString"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {

                LoadGuardList();
                LoadRoleDropdown();
                string currentUserRole = Session["ROLE"]?.ToString();
                if (currentUserRole != "SUPER ADMIN")
                {
                    ddlRole.Enabled = false; 
                }
            }
        }
        private void LoadRoleDropdown()
        {
            string role = ddlRole.SelectedValue;

            ddlRole.Items.Clear();
       
            ddlRole.Items.Add(new ListItem("USER", "USER"));
            ddlRole.Items.Add(new ListItem("ADMIN", "ADMIN"));
            ddlRole.Items.Add(new ListItem("SUPER ADMIN", "SUPER ADMIN"));
        }

        protected void btnSearchStaff_Click(object sender, EventArgs e)
        {
            string empNo = txtEmpNo.Text.Trim();

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();

                string query = "SELECT * FROM VIS_STAFF WHERE TRIM(UPPER(EMP_NO)) = TRIM(UPPER(:empNo))";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(":empNo", empNo);

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtEmpName.Text = reader["EMP_NAME"].ToString();
                            txtDept.Text = reader["DEPT"].ToString();
                            txtSect.Text = reader["SECT"].ToString();
                            txtPosition.Text = reader["EMP_POSITION"].ToString();
                            txtHodEmail.Text = reader["EMAIL"].ToString();
                            txtWorkingTime.Text = reader["WORKING_TIME"].ToString();

                            string roleFromDb = reader["ROLE"]?.ToString() ?? "";
                            if (ddlRole.Items.FindByValue(roleFromDb) != null)
                            {
                                ddlRole.SelectedValue = roleFromDb;
                            }
                            else
                            {
                                ddlRole.SelectedIndex = 0; // default to "- Select Role -"
                            }
                        }
                        else
                        {
                            ClearStaffForm();
                        }
                    }
                }
            }
        }

        protected void btnAddOrUpdateStaff_Click(object sender, EventArgs e)
        {
            string empNo = txtEmpNo.Text.Trim();
            string empName = txtEmpName.Text.Trim();
            string dept = txtDept.Text.Trim();
            string sect = txtSect.Text.Trim();
            string position = txtPosition.Text.Trim();
            string email = txtHodEmail.Text.Trim();
            string workingTime = txtWorkingTime.Text.Trim();

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();

                string checkQuery = "SELECT COUNT(*) FROM VIS_STAFF WHERE TRIM(EMP_NO) = :empNo";
                using (OracleCommand checkCmd = new OracleCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.Add(":empNo", empNo);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    string query = (count > 0)
                        ? @"UPDATE VIS_STAFF SET 
 EMP_NAME = :empName, 
 DEPT = :dept, 
 SECT = :sect, 
 EMP_POSITION = :position, 
 EMAIL = :email,
 WORKING_TIME = :workingTime,
 ROLE = :role
WHERE TRIM(EMP_NO) = :empNo
"

                    : @"INSERT INTO VIS_STAFF 
(EMP_NO, EMP_NAME, DEPT, SECT, EMP_POSITION, EMAIL, WORKING_TIME, ROLE) 
VALUES 
(:empNo, :empName, :dept, :sect, :position, :email, :workingTime, :role)
"
;

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(":empNo", empNo);
                        cmd.Parameters.Add(":empName", empName);
                        cmd.Parameters.Add(":dept", dept);
                        cmd.Parameters.Add(":sect", sect);
                        cmd.Parameters.Add(":position", position);
                        cmd.Parameters.Add(":email", email);
                        cmd.Parameters.Add(":workingTime", workingTime);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        string msg = (rowsAffected > 0)
                            ? "Staff info saved/updated successfully."
                            : "Failed to update/insert staff.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{msg}');", true);
                    }
                }
            }

            ClearStaffForm();
           
        }

        protected void btnUpdateStaff_Click(object sender, EventArgs e)
        {
            string empNo = txtEmpNo.Text.Trim();
            string empName = txtEmpName.Text.Trim();
            string dept = txtDept.Text.Trim();
            string sect = txtSect.Text.Trim();
            string position = txtPosition.Text.Trim();
            string email = txtHodEmail.Text.Trim();
            string workingTime = txtWorkingTime.Text.Trim();
            string role = ddlRole.SelectedValue;

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();

         string updateQuery = @"
   UPDATE VIS_STAFF SET 
    EMP_NAME = :empName,
    DEPT = :dept,
    SECT = :sect,
    EMP_POSITION = :position,
    EMAIL = :email,
    WORKING_TIME = :workingTime,
    ROLE = :role
WHERE TRIM(UPPER(EMP_NO)) = TRIM(UPPER(:empNo))
";


                using (OracleCommand cmd = new OracleCommand(updateQuery, conn))
                {
                    cmd.Parameters.Add(":empName", empName);
                    cmd.Parameters.Add(":dept", dept);
                    cmd.Parameters.Add(":sect", sect);
                    cmd.Parameters.Add(":position", position);
                    cmd.Parameters.Add(":email", email);
                    cmd.Parameters.Add(":workingTime", workingTime);
                    cmd.Parameters.Add(":role", role);
                    cmd.Parameters.Add(":empNo", empNo);
               

                    int rowsAffected = cmd.ExecuteNonQuery();

                    string msg = (rowsAffected > 0)
                        ? "Staff info updated successfully."
                        : $"Update failed. No record found for EMP_NO: {empNo}";
                    ScriptManager.RegisterStartupScript(this, GetType(), "result", $"alert('{msg}');", true);
                }
            }

            ClearStaffForm();
    
        }

        protected void btnDeleteStaff_Click(object sender, EventArgs e)
        {
            string empNo = txtEmpNo.Text.Trim();

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();

                string deleteQuery = "DELETE FROM VIS_STAFF WHERE TRIM(EMP_NO) = :empNo";
                using (OracleCommand cmd = new OracleCommand(deleteQuery, conn))
                {
                    cmd.Parameters.Add(":empNo", empNo);
                    cmd.ExecuteNonQuery();
                }
            }

            ClearStaffForm();
            
        }

        private void ClearStaffForm()
        {
            txtEmpNo.Text = "";
            txtEmpName.Text = "";
            txtDept.Text = "";
            txtSect.Text = "";
            txtPosition.Text = "";
            txtHodEmail.Text = "";
            txtWorkingTime.Text = "";
            ddlRole.SelectedIndex = 0;
        }
        protected void btnSaveFridayBreak_Click(object sender, EventArgs e)
        {
            string breakTime = txtFridayBreakTime.Text.Trim(); // e.g. 1315-1445

            if (string.IsNullOrEmpty(breakTime))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Please enter the Friday break time.');", true);
                return;
            }

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();

                string query = @"
            MERGE INTO VIS_FRID t
            USING (SELECT 1 AS ID FROM dual) s
            ON (t.ID = s.ID)
            WHEN MATCHED THEN
              UPDATE SET t.BREAK_TIME = :breakTime,
                         t.CREATED_DATE = SYSDATE
            WHEN NOT MATCHED THEN
              INSERT (ID, BREAK_TIME, CREATED_DATE)
              VALUES (1, :breakTime, SYSDATE)";

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(":breakTime", breakTime);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('Friday break time saved successfully.');", true);
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "error", $"alert('Error: {ex.Message}');", true);
                    }
                }
            }
        }



        protected void btnClearStaff_Click(object sender, EventArgs e)
        {
            ClearStaffForm();
         
        }
        private void LoadGuardList()
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();
                string query = "SELECT NAME, NO_TEL FROM VIS_GUARD ORDER BY NAME";

                using (OracleDataAdapter da = new OracleDataAdapter(query, conn))
                {
                    da.Fill(dt);
                }
            }

            gvGuards.DataSource = dt;
            gvGuards.DataBind();
        }

        protected void btnAddGuard_Click(object sender, EventArgs e)
        {
            string name = txtGuardName.Text.Trim();
            string phone = txtGuardPhone.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Please fill in both fields.');", true);
                return;
            }

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();

                string insertQuery = "INSERT INTO VIS_GUARD (NAME, NO_TEL) VALUES (:name, :phone)";
                using (OracleCommand cmd = new OracleCommand(insertQuery, conn))
                {
                    cmd.Parameters.Add(":name", name);
                    cmd.Parameters.Add(":phone", phone);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('Guard added successfully.');", true);
                        txtGuardName.Text = "";
                        txtGuardPhone.Text = "";
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "error", $"alert('Failed to add guard: {ex.Message}');", true);
                    }
                }
            }

            LoadGuardList();
        }

        protected void gvGuards_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string name = gvGuards.DataKeys[e.RowIndex].Value.ToString();

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();

                string deleteQuery = "DELETE FROM VIS_GUARD WHERE NAME = :name";
                using (OracleCommand cmd = new OracleCommand(deleteQuery, conn))
                {
                    cmd.Parameters.Add(":name", name);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        ScriptManager.RegisterStartupScript(this, GetType(), "deleted", "alert('Guard deleted successfully.');", true);
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "error", $"alert('Failed to delete guard: {ex.Message}');", true);
                    }
                }
            }

            LoadGuardList();
        }
        protected void gvGuards_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowState == DataControlRowState.Normal)
            {
                foreach (TableCell cell in e.Row.Cells)
                {
                    foreach (Control ctrl in cell.Controls)
                    {
                        if (ctrl is Button btn && btn.CommandName == "Delete")
                        {
                            btn.OnClientClick = "return confirm('Are you sure you want to delete this guard?');";
                        }
                    }
                }
            }
        }

    }
}

using System;
using System.Data;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using System.Collections.Generic;


namespace vms.v1
{
    public partial class RegisterForm2 : Page
    {
        string connStr = ConfigurationManager.AppSettings["ConnectionString"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<string> guards = GetGuardsFromDatabase();

                

                ddlSecurityName.DataSource = guards;
                ddlSecurityName.DataBind();
                ddlSecurityName.Items.Insert(0, new ListItem("-- Select Name --", ""));

                SetFormVisibility();
            }
        }

        protected void ddlRecordType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFormVisibility(); // Re-check the record type on change

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


        private void SetFormVisibility()
        {

            // Hide both forms first
            pnlVehicleList.Visible = false;
            pnlVisitorForm.Visible = false;
            pnlItemDeclareForm.Visible = false;
            pnlContainerForm.Visible = false;

            string recordType = ddlRecordType.SelectedValue.ToLower();

            if (recordType == "vehicle")
            {
                pnlVehicleList.Visible = true;
                BindVehicleList();
            }

            else if (recordType == "visitor")
            {
                pnlVisitorForm.Visible = true;
            }

            else if (recordType == "item")
            {
                pnlItemDeclareForm.Visible = true;
            }

            else if (recordType == "container")
            {
                pnlContainerForm.Visible = true;
            }
        }

       

        private DataTable GetPost1Vehicles()
        {
            DataTable dt = new DataTable();

            using (OracleConnection con = new OracleConnection(connStr))
            {
                string query = @"
 SELECT V.VEHICLE_ID,
                V.PLATE_NO,
                V.NAME,
                V.COMPANY,
                V.VEHICLE_TYPE,
                V.LOCATION FROM VIS_VEHICLE V WHERE V.TIME_OUT IS NULL AND NOT EXISTS (SELECT 1 FROM VIS_VEHICLE2 P2 WHERE P2.VEHICLE_ID=V.VEHICLE_ID)";


                using (OracleCommand cmd = new OracleCommand(query, con))
                {
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }


        private void BindVehicleList()
        {
            DataTable dt = GetPost1Vehicles();
            rptVehicles.DataSource = dt;
            rptVehicles.DataBind();
            pnlVehicleList.Visible = dt.Rows.Count > 0;
        }

        protected void rptVehicles_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "RegisterPost2")
            {
                // Access the controls inside the Repeater
                TextBox txtPurpose = (TextBox)e.Item.FindControl("txtPurpose");
                TextBox txtDO = (TextBox)e.Item.FindControl("txtDO");
                TextBox txtItemType = (TextBox)e.Item.FindControl("txtItemType");
                DropDownList ddlBlock = (DropDownList)e.Item.FindControl("ddlBlock");

                // Get values from the controls
                string purpose = txtPurpose?.Text;
                string doNumber = txtDO?.Text;
                string itemType = txtItemType?.Text;
                string block = ddlBlock?.SelectedValue;
                string vehicleId = e.CommandArgument.ToString(); // Vehicle ID passed from CommandArgument

                // Ensure valid values before executing query
                if (string.IsNullOrEmpty(purpose) || string.IsNullOrEmpty(doNumber) || string.IsNullOrEmpty(itemType) || string.IsNullOrEmpty(block))
                {
                    // Optionally add validation feedback to the user
                    return;
                }

                using (OracleConnection con = new OracleConnection(connStr))
                {
                    string insertQuery = @"
 INSERT INTO VIS_VEHICLE2 (VEHICLE_ID, REGISTER_DATE, PURPOSE, ITEM_TYPE, DO_NO, BLOCK) VALUES (:VehicleId, CURRENT_TIMESTAMP, :Purpose, :ItemType, :DO, :Block)";


                    using (OracleCommand cmd = new OracleCommand(insertQuery, con))
                    {
                        // Add parameters for the query
                        cmd.Parameters.Add(":VehicleId", OracleDbType.Int32).Value = Convert.ToInt32(vehicleId);
                        cmd.Parameters.Add(":Purpose", OracleDbType.Varchar2).Value = purpose;
                        cmd.Parameters.Add(":ItemType", OracleDbType.Varchar2).Value = itemType;
                        cmd.Parameters.Add(":DO", OracleDbType.Varchar2).Value = doNumber;
                        cmd.Parameters.Add(":Block", OracleDbType.Varchar2).Value = block;

                        // Open the connection and execute the query
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Redirect", "alert('Success! Redirecting...'); window.location.href='Post2.aspx';", true);

                // Refresh the vehicle list after insertion
                BindVehicleList();
            }
        }

        protected void btnSubmitVisitor_Click(object sender, EventArgs e)
        {
            string name = txtVisitorName.Value.Trim();
            string company = txtVisitorCompany.Value.Trim();
            string ic = txtVisitorIC.Value.Trim();
            string block = ddlVisitorBlock.SelectedValue;
            string purpose = txtVisitorPurpose.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(ic) || string.IsNullOrEmpty(purpose))
            {
                // Optionally show error message
                return;
            }

            using (OracleConnection con = new OracleConnection(connStr))
            {
                string query = @"
 INSERT INTO VIS_VISITOR (NAME, COMPANY, IC_NO, BLOCK, PURPOSE, REGISTER_DATE) VALUES (:Name, :Company, :IC, :Block, :Purpose, CURRENT_TIMESTAMP)";


                using (OracleCommand cmd = new OracleCommand(query, con))
                {
                    cmd.Parameters.Add(":Name", OracleDbType.Varchar2).Value = name;
                    cmd.Parameters.Add(":Company", OracleDbType.Varchar2).Value = company;
                    cmd.Parameters.Add(":IC", OracleDbType.Varchar2).Value = ic;
                    cmd.Parameters.Add(":Block", OracleDbType.Varchar2).Value = block;
                    cmd.Parameters.Add(":Purpose", OracleDbType.Varchar2).Value = purpose;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Redirect", "alert('Success! Redirecting...'); window.location.href='Post2.aspx';", true);
            // Clear form fields
            txtVisitorName.Value = "";
            txtVisitorCompany.Value = "";
            txtVisitorIC.Value = "";
            ddlVisitorBlock.SelectedIndex = 0;
            txtVisitorPurpose.Text = "";

        }

        protected void btnSubmitItemDeclare_Click(object sender, EventArgs e)
        {
            string name = txtItemName.Value.Trim();
            string empNo = txtEmpNo.Value.Trim();
            string itemType = txtDeclaredItemType.Value.Trim();
            string serialPartNo = txtSerialPartNo.Value.Trim();
            string totalItemInStr = txtTotalItemIn.Value.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(itemType) || string.IsNullOrEmpty(totalItemInStr))
            {
                // Optionally show error message
                return;
            }

            int totalItemIn;

            if (!int.TryParse(totalItemInStr, out totalItemIn))
            {
                // Invalid number
                return;
            }

            using (OracleConnection con = new OracleConnection(connStr))
            {
                string query = @"
 INSERT INTO VIS_ITEMDECLARE (NAME, EMP_NO, ITEM_TYPE, SERIAL_PART_NO, TOTAL_ITEM_IN, DECLARE_DATE) VALUES (:Name, :EmpNo, :ItemType, :SerialPartNo, :TotalItemIn, CURRENT_TIMESTAMP)";


                using (OracleCommand cmd = new OracleCommand(query, con))
                {
                    cmd.Parameters.Add(":Name", OracleDbType.Varchar2).Value = name;
                    cmd.Parameters.Add(":EmpNo", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(empNo) ? DBNull.Value : (object)empNo;
                    cmd.Parameters.Add(":ItemType", OracleDbType.Varchar2).Value = itemType;
                    cmd.Parameters.Add(":SerialPartNo", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(serialPartNo) ? DBNull.Value : (object)serialPartNo;
                    cmd.Parameters.Add(":TotalItemIn", OracleDbType.Int32).Value = totalItemIn;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Redirect",
       "alert('Success! Redirecting...'); window.location.href='Post2.aspx';", true);

            txtItemName.Value = "";
            txtEmpNo.Value = "";
            txtDeclaredItemType.Value = "";
            txtSerialPartNo.Value = "";
            txtTotalItemIn.Value = "";
        }

        protected void btnRegisterEntry_Click(object sender, EventArgs e)
        {
            // Get current time for "Time In"
            DateTime timeIn = DateTime.Now;

            string driverName = txtDriverName.Value;
            string driverICNo = txtDriverICNo.Value;
            string containerPlateNo = txtContainerPlateNo.Value;
            string primeMoverNo = txtPrimeMoverNo.Value;
            string containerCompany = txtContainerCompany.Value;
            string containerOption = ddlContainerOption.Value;
            string securityName = ddlSecurityName.SelectedItem.Text;


            string acknowledgement = "";

            if (ackYes.Checked)
            {
                acknowledgement = "Yes";
            }

            else if (ackNo.Checked)
            {
                acknowledgement = "No";
            }

            // Handle container-related fields based on selected option
            string containerNo = "";
            string sealNo = "";


            if (containerOption == "withContainerEmpty" || containerOption == "withContainerFilled")
            {
                containerNo = txtContainerNo.Value;
            }

            if (containerOption == "withContainerFilled")
            {
                sealNo = txtSealNo.Value;

            }

            // Database query to save data
            string query = @"INSERT INTO VIS_CONTAINER (DRIVER_NAME, DRIVER_ICNo, PLATE_NO, PRIMEMOVER_NO, COMPANY, CONTAINER_OPTION, CONTAINER_NO, SEAL_NO, SECURITY_NAME, ACKNOWLEDGEMENT, REGISTER_DATE) " + "VALUES (:driverName, :driverICNo, :containerPlateNo, :primeMoverNo, :containerCompany, :containerOption, :containerNo, :sealNo, :securityName, :acknowledgement, CURRENT_TIMESTAMP)";

            // Initialize Oracle connection and command
            using (OracleConnection con = new OracleConnection(connStr)) // connStr should be defined elsewhere

            {
                using (OracleCommand cmd = new OracleCommand(query, con)) // Initialize OracleCommand with the query and connection

                {
                    // Add parameters to the command
                    cmd.Parameters.Add(new OracleParameter(":driverName", driverName));
                    cmd.Parameters.Add(new OracleParameter(":driverICNo", driverICNo));
                    cmd.Parameters.Add(new OracleParameter(":containerPlateNo", containerPlateNo));
                    cmd.Parameters.Add(new OracleParameter(":primeMoverNo", primeMoverNo));
                    cmd.Parameters.Add(new OracleParameter(":containerCompany", containerCompany));
                    cmd.Parameters.Add(new OracleParameter(":containerOption", containerOption));
                    cmd.Parameters.Add(new OracleParameter(":containerNo", containerNo));
                    cmd.Parameters.Add(new OracleParameter(":sealNo", sealNo));
                    cmd.Parameters.Add(new OracleParameter(":securityName", securityName));
                    cmd.Parameters.Add(new OracleParameter(":acknowledgement", acknowledgement));

                    // Open the connection and execute the query
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Redirect",
     "alert('Success! Redirecting...'); window.location.href='Post2.aspx';", true);

            txtDriverName.Value = "";
            txtDriverICNo.Value = "";
            txtContainerPlateNo.Value = "";
            txtPrimeMoverNo.Value = "";
            txtContainerCompany.Value = "";
            ddlContainerOption.Value = "";
            txtContainerNo.Value = "";
            txtSealNo.Value = "";
            ddlSecurityName.Text = "";
            ackYes.Checked = false;
            ackNo.Checked = false;


        }


    }
}
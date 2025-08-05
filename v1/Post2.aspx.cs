using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.EMMA;
using Oracle.ManagedDataAccess.Client;

namespace vms.v1
{
    public partial class Post2 : System.Web.UI.Page
    {

        private string connStr = ConfigurationManager.AppSettings["ConnectionString"];

        protected void Page_Load(object sender, EventArgs e)
        {

            CheckOverstayedVisitors(connStr);

            if (!IsPostBack)
            {

                BindVehicle2(connStr);
                BindWalkin(connStr);
                BindItemDeclare(connStr);
                BindContainer(connStr);
                CheckOverstayedVisitors(connStr);

            }
        }


        private void BindVehicle2(string connString)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = @"
 SELECT V.PLATE_NO,
                    V.NAME,
                    V2.BLOCK,
                    V2.REGISTER_DATE FROM VIS_VEHICLE V INNER JOIN VIS_VEHICLE2 V2 ON V2.VEHICLE_ID=V.VEHICLE_ID WHERE V2.TIME_OUT IS NULL ";


                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvVehicle2.DataSource = dt;
                    gvVehicle2.DataBind();
                }

                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);
                }
            }

        }

        protected void btnCheckOut5_Click(object sender, EventArgs e)
        {
            string plateNo = ((Button)sender).CommandArgument.ToString();
            DateTime timeOut = DateTime.Now;

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string updateQuery = "UPDATE VIS_VEHICLE2 V2 SET TIME_OUT = :TimeOut WHERE TIME_OUT IS NULL AND V2.VEHICLE_ID IN (SELECT V.VEHICLE_ID FROM VIS_VEHICLE V WHERE V.PLATE_NO = :PlateNo)";

                    using (OracleCommand cmd = new OracleCommand(updateQuery, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("TimeOut", timeOut));
                        cmd.Parameters.Add(new OracleParameter("PlateNo", plateNo));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {

                            Console.WriteLine("No matching record found or already checked out.");
                        }
                    }
                }

                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);
                }

                finally
                {

                    BindVehicle2(connStr);
                    CheckOverstayedVisitors(connStr);
                }
            }
        }

        private void BindWalkin(string connString)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT NAME, IC_NO, BLOCK, REGISTER_DATE FROM VIS_VISITOR WHERE TIME_OUT IS null";

                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvWalkin.DataSource = dt;
                    gvWalkin.DataBind();
                }

                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);
                }
            }

        }

        protected void btnCheckOut6_Click(object sender, EventArgs e)
        {
            string name = ((Button)sender).CommandArgument.ToString();

            DateTime timeOut = DateTime.Now;

            string updateQuery = "UPDATE VIS_VISITOR SET TIME_OUT = :TimeOut WHERE NAME = :Name AND TIME_OUT IS NULL";

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand(updateQuery, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("TimeOut", timeOut));
                        cmd.Parameters.Add(new OracleParameter("Name", name));

                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                catch (Exception ex)
                {
                    Response.Write("Error: " + ex.Message);
                }
            }

            BindWalkin(connStr);
            CheckOverstayedVisitors(connStr);
        }

        protected void btnCheckOut7_Click(object sender, EventArgs e)
        {
            string name = ((Button)sender).CommandArgument.ToString();

            DateTime timeOut = DateTime.Now;

            string updateQuery = "UPDATE VIS_ITEMDECLARE SET TIME_OUT = :TimeOut WHERE NAME = :Name AND TIME_OUT IS NULL";

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand(updateQuery, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("TimeOut", timeOut));
                        cmd.Parameters.Add(new OracleParameter("Name", name));

                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                catch (Exception ex)
                {
                    Response.Write("Error: " + ex.Message);
                }
            }

            BindItemDeclare(connStr);
            CheckOverstayedVisitors(connStr);
        }

        private void BindItemDeclare(string connString)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT NAME, ITEM_TYPE, SERIAL_PART_NO, TOTAL_ITEM_IN, DECLARE_DATE FROM VIS_ITEMDECLARE WHERE TIME_OUT IS null";

                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvItemDeclare.DataSource = dt;
                    gvItemDeclare.DataBind();
                }

                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);
                }
            }

        }

        protected void btnCheckOut8_Click(object sender, EventArgs e)
        {
            GridViewRow row = ((Button)sender).NamingContainer as GridViewRow;

            string plateNo = ((Button)sender).CommandArgument.ToString();

            Label lblContainer = (Label)row.FindControl("lblContainerNo");
            TextBox txtContainer = (TextBox)row.FindControl("txtContainerNo");
            string containerNo = !string.IsNullOrWhiteSpace(lblContainer.Text) ? lblContainer.Text : txtContainer.Text;

            Label lblSeal = (Label)row.FindControl("lblSealNo");
            TextBox txtSeal = (TextBox)row.FindControl("txtSealNo");
            string sealNo = !string.IsNullOrWhiteSpace(lblSeal.Text) ? lblSeal.Text : txtSeal.Text;

            DateTime timeOut = DateTime.Now;

            string updateQuery = @"
 UPDATE VIS_CONTAINER SET TIME_OUT=: TimeOut,
                CONTAINER_NO=:ContainerNo,
                SEAL_NO=:SealNo WHERE PLATE_NO=:PlateNo AND TIME_OUT IS NULL";


            using (OracleConnection conn = new OracleConnection(connStr))
            {
                try
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand(updateQuery, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("TimeOut", timeOut));
                        cmd.Parameters.Add(new OracleParameter("ContainerNo", containerNo));
                        cmd.Parameters.Add(new OracleParameter("SealNo", sealNo));
                        cmd.Parameters.Add(new OracleParameter("PlateNo", plateNo));

                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                catch (Exception ex)
                {
                    Response.Write("Error: " + ex.Message);
                }
            }

            BindContainer(connStr);
            CheckOverstayedVisitors(connStr);
        }


        private void BindContainer(string connString)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT DRIVER_NAME, PLATE_NO, CONTAINER_NO, SEAL_NO, REGISTER_DATE FROM VIS_CONTAINER WHERE TIME_OUT IS null";

                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvContainer.DataSource = dt;
                    gvContainer.DataBind();
                }

                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private void CheckOverstayedVisitors(string connString)
        {

            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                string query = @"
         SELECT 
    'VEHICLE' AS CATEGORY, 
    PLATE_NO AS IDENTIFIER, 
    TIME_IN, 
    SYSDATE AS CURRENT_TIME,
    ROUND((SYSDATE - CAST(TIME_IN AS DATE)) * 24, 2) AS DURATION_HOURS,
    LOCATION
FROM VIS_VEHICLE
WHERE TIME_OUT IS NULL AND (SYSDATE - CAST(TIME_IN AS DATE)) * 24 > 1

UNION ALL

SELECT 
    'TRANSPORTER', 
    NO_PLATE, 
    TIME_IN, 
    SYSDATE, 
    ROUND((SYSDATE - CAST(TIME_IN AS DATE)) * 24, 2),
    NULL 
FROM VIS_PARKING
WHERE TIME_OUT IS NULL AND (SYSDATE - CAST(TIME_IN AS DATE)) * 24 > 8

UNION ALL

SELECT 
    'TNB', 
    NO_PLATE, 
    TIME_IN, 
    SYSDATE, 
    ROUND((SYSDATE - CAST(TIME_IN AS DATE)) * 24, 2),
    NULL  
FROM VIS_TNB
WHERE TIME_OUT IS NULL AND (SYSDATE - CAST(TIME_IN AS DATE)) * 24 > 2

UNION ALL

SELECT 
    'VISITOR', 
    NAME, 
    REGISTER_DATE, 
    SYSDATE, 
    ROUND((SYSDATE - CAST(REGISTER_DATE AS DATE)) * 24, 2),
    BLOCK 
FROM VIS_VISITOR
WHERE TIME_OUT IS NULL AND (SYSDATE - CAST(REGISTER_DATE AS DATE)) * 24 > 2

UNION ALL

SELECT 
    'CONTAINER', 
    PLATE_NO, 
    REGISTER_DATE, 
    SYSDATE, 
    ROUND((SYSDATE - CAST(REGISTER_DATE AS DATE)) * 24, 2),
    NULL 
FROM VIS_CONTAINER
WHERE TIME_OUT IS NULL AND (SYSDATE - CAST(REGISTER_DATE AS DATE)) * 24 > 3
";


                using (OracleCommand cmd = new OracleCommand(query, conn))
                {

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();

                            while (reader.Read())
                            {
                                string category = reader["CATEGORY"].ToString();
                                string identifier = reader["IDENTIFIER"].ToString();
                                string timeIn = Convert.ToDateTime(reader["TIME_IN"]).ToString("yyyy-MM-dd HH:mm");
                                string duration = reader["DURATION_HOURS"].ToString();
                                string location = reader["LOCATION"] == DBNull.Value ? "" : reader["LOCATION"].ToString();

                                sb.Append("<tr>");
                                sb.AppendFormat("<td>{0}</td>", category);
                                sb.AppendFormat("<td>{0}</td>", identifier);
                                sb.AppendFormat("<td>{0}</td>", timeIn);
                                sb.AppendFormat("<td>{0}</td>", duration);
                                sb.AppendFormat("<td>{0}</td>", location);
                                sb.Append("</tr>");
                            }


                            // Inject HTML to client-side
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowVisitorOverstayModal",
                                "$('#modalVisitorData').html(`" + sb.ToString() + "`); $('#visitorOverstayModal').modal('show');", true);
                        }
                    }
                }
            }
        }
    }
}
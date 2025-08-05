using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Collections;

 
    public class OraClientConn
    {
        private enum OracleConnectionOwnership
        {
            Internal,
            External
        }

        public OracleConnection Conn = new OracleConnection();

        public OraClientConn()
        {
            //
            // TODO: Add constructor logic here
            //

        }
        public void InitConnection(string strConnection)
        {
            Conn.ConnectionString = strConnection;
        }
        public void OraClientOpen()
        {
            if (Conn != null)
            {
                if (Conn.State.ToString() == "Open")
                {
                    Conn.Close();
                    Conn.Open();
                }
                else if (Conn.State.ToString() == "Closed")
                {
                    Conn.Open();
                }
            }
        }
        public void OraClientOpen(string strConnection)
        {
            if (Conn != null)
            {
                if (Conn.State.ToString() == "Open")
                {
                    Conn.Close();
                    Conn.Open();
                }
                else if (Conn.State.ToString() == "Closed")
                {
                    Conn.ConnectionString = strConnection;
                    Conn.Open();
                }
            }
        }
        public void OraClientClose()
        {
            if (Conn != null)
            {
                if (Conn.State.ToString() == "Open")
                {
                    Conn.Close();
                    //Conn.Dispose();
                }
            }
        }
        public bool OraClientExcecuteSQL(string sql)
        {

            bool result = false;
            try
            {
                System.Diagnostics.Debug.WriteLine(sql);
                using (OracleCommand Comm = new OracleCommand(sql, Conn))
                {
                    if (Conn.State.ToString() == "Closed")
                    {
                        OraClientOpen();
                    }
                    Comm.ExecuteNonQuery();
                    result = true;
                    System.Diagnostics.Debug.WriteLine(sql);
                }
            }
            catch (Exception Er)
            {
                result = false;
                OraClientClose();
                throw Er;
            }
            finally
            {
                OraClientClose();
            }
            return result;
        }
        public bool OraClientExcecuteSQL(string sql, ArrayList lsParam)
        {

            bool result = false;
            try
            {
                System.Diagnostics.Debug.WriteLine(sql);
                using (OracleCommand Comm = new OracleCommand(sql, Conn))
                {
                    if (Conn.State.ToString() == "Closed")
                    {
                        OraClientOpen();
                    }
                    Comm.ExecuteNonQuery();
                    result = true;
                    System.Diagnostics.Debug.WriteLine(sql);
                }
            }
            catch (Exception Er)
            {
                result = false;
                OraClientClose();
                throw Er;
            }
            finally
            {
                OraClientClose();
            }
            return result;
        }
        public void OraClientExcecuteSQL_NoClose(string sql)
        {
            using (OracleCommand Comm = new OracleCommand(sql, Conn))
            {
                try
                {
                    if (Conn.State.ToString() == "Closed")
                    {
                        OraClientOpen();
                    }
                    Comm.ExecuteNonQuery();
                    Comm.Dispose();
                }
                catch (Exception Er)
                {
                    OraClientClose();
                    throw Er;
                }
            }

        }
        public void OraClientExcecuteSQL_NoClose(string sql, ArrayList lsParam)
        {
            using (OracleCommand Comm = new OracleCommand(sql, Conn))
            {
                try
                {
                    if (Conn.State.ToString() == "Closed")
                    {
                        OraClientOpen();
                    }
                    for (int i = 0; i < lsParam.Count; i++)
                    {
                        OracleParameter blobParameter = new OracleParameter();
                        blobParameter = (OracleParameter)lsParam[i];
                        Comm.Parameters.Add(blobParameter);
                    }
                    Comm.ExecuteNonQuery();
                    Comm.Dispose();
                }
                catch (Exception Er)
                {
                    OraClientClose();
                    throw Er;
                }
            }
        }
        public void OraClientExcecuteSQL_NoClose_Trans(string sql, OracleTransaction trans)
        {
            using (OracleCommand Comm = new OracleCommand(sql, Conn))
            {
                try
                {
                    if (Conn.State.ToString() == "Closed")
                    {
                        OraClientOpen();
                    }
                    Comm.Transaction = trans;
                    Comm.ExecuteNonQuery();
                }
                catch (Exception Er)
                {
                    OraClientClose();
                    throw Er;
                }
            }
        }
        public OracleDataReader OraClientExcecuteSQLtoRDR(string sql)
        {
            OracleDataReader result;
            using (OracleCommand Comm = new OracleCommand(sql, Conn))
            {
                try
                {
                    OraClientOpen();
                    result = Comm.ExecuteReader(CommandBehavior.CloseConnection);
                    //result = Comm.ExecuteReader();		
                }
                catch (Exception Er)
                {
                    result = null;
                    OraClientClose();
                    throw Er;
                }
                finally
                {
                    //OraClientClose();
                    //Conn.Close();                
                }
            }
            //result.Dispose();
            return result;
        }
        public OracleDataReader OraClientExcecuteSQLtoRDR2(string sql)
        {
            OracleDataReader result;
            using (OracleCommand Comm = new OracleCommand(sql, Conn))
            {
                try
                {
                    //OraClientOpen();
                    result = Comm.ExecuteReader(CommandBehavior.Default);
                    //result = Comm.ExecuteReader();		
                }
                catch (Exception Er)
                {
                    result = null;
                    OraClientClose();
                    throw Er;
                }
                finally
                {
                    //Conn.Close();
                    OraClientClose();
                }
            }
            result.Dispose();
            return result;
        }
        public OracleDataReader OraClientExcecuteSQLtoRDR_NoClose(string sql, OracleTransaction trans)
        {
            OracleDataReader result;
            using (OracleCommand Comm = new OracleCommand(sql, Conn))
            {
                try
                {
                    Comm.Transaction = trans;
                    result = Comm.ExecuteReader();
                }
                catch (Exception Er)
                {
                    result = null;
                    OraClientClose();
                    throw Er;
                }
                finally
                {
                    //Conn.Close();
                    OraClientClose();
                }
            }
            result.Dispose();
            return result;
        }
        public System.Data.DataSet OraClientExcecuteSQLtoADAPT(string sql)
        {
            System.Data.DataSet result = new System.Data.DataSet();
            using (OracleDataAdapter oda = new OracleDataAdapter())
            {
                using (OracleCommand Comm = new OracleCommand(sql, Conn))
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine(sql);
                        if (Conn.State.ToString() == "Closed")
                        {
                            OraClientOpen();
                        }
                        oda.SelectCommand = Comm;
                        oda.Fill(result, "ds");
                        //Comm.Fill(result, "ds");                
                    }
                    catch (Exception Er)
                    {
                        result = null;
                        System.Diagnostics.Debug.WriteLine(sql);
                        OraClientClose();
                        throw Er;
                    }
                    finally
                    {
                        //Conn.Close();
                        OraClientClose();
                    }
                    OraClientClose();
                }
                return result;
            }
        }
        public System.Data.DataSet OraClientExcecuteSQLtoADAPT_T(string sql, string tablename)
        {
            System.Data.DataSet result = new System.Data.DataSet();
            using (OracleDataAdapter oda = new OracleDataAdapter())
            {
                using (OracleCommand Comm = new OracleCommand(sql, Conn))
                {
                    try
                    {
                        if (Conn.State.ToString() == "Closed")
                        {
                            OraClientOpen();
                        }
                        oda.SelectCommand = Comm;
                        oda.Fill(result, tablename.ToString().Trim());
                        //Comm.Fill(result, tablename.ToString().Trim());
                    }
                    catch (Exception Er)
                    {
                        result = null;
                        OraClientClose();
                        throw Er;
                    }
                    finally
                    {
                        //Conn.Close();
                        OraClientClose();
                    }
                    OraClientClose();
                }
                return result;
            }
        }
        public string OraClientExcecuteScalar(string sql)
        {
            string ra = "0";
            using (OracleCommand Comm = new OracleCommand(sql, Conn))
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine(sql);
                    if (Conn.State.ToString() == "Closed")
                    {
                        OraClientOpen();
                    }
                    ra = Comm.ExecuteScalar().ToString();
                    System.Diagnostics.Debug.WriteLine(sql);

                }
                catch
                {
                    OraClientClose();
                    //throw Er;
                }
                finally
                {
                    OraClientClose();
                }
            }
            return ra;
        }
        public string OraClientExcecuteScalar_NoClose(string sql)
        {
            string ra = "0";
            using (OracleCommand Comm = new OracleCommand(sql, Conn))
            {
                try
                {
                    if (Conn.State.ToString() == "Closed")
                    {
                        OraClientOpen();
                    }
                    ra = Comm.ExecuteScalar().ToString();

                }
                catch
                {
                    OraClientClose();
                    //throw Er;
                }
            }
            return ra;
        }
        public object OraClientExecuteScalar_NoClose_Trans(string sql, OracleTransaction trans)
        {
            using (OracleCommand Comm = new OracleCommand(sql, Conn))
            {
                try
                {
                    Comm.Transaction = trans;
                    return Comm.ExecuteScalar();
                }
                catch
                {
                    OraClientClose();
                    return null;
                    //throw Er;
                }
            }
        }
        // Execute a OracleCommand (that returns no resultset) against the specified OracleTransaction
        // using the provided parameters.
        // e.g.:  
        // Dim result As Integer = OraClientExecuteNonQuery_NoClose(trans, CommandType.StoredProcedure, "GetOrders", OracleParameter[])
        // Parameters:
        // -transaction - a valid OracleTransaction 
        // -commandType - the CommandType (stored procedure, text, etc.) 
        // -commandText - the stored procedure name or T-Oracle command 
        // -commandParameters - an array of OracleParameters used to execute the command 
        // Returns: An int representing the number of rows affected by the command 
        public int OraClientExecuteNonQuery_NoClose(OracleTransaction transaction, CommandType commandType,
            string commandText, params OracleParameter[] commandParameters)
        {
            if ((transaction == null))
            {
                throw new ArgumentNullException("transaction");
            }
            if (!((transaction == null)) && (transaction.Connection == null))
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            using (OracleCommand cmd = new OracleCommand())
            {
                int retval;
                bool mustCloseConnection = false;
                PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, mustCloseConnection);
                retval = cmd.ExecuteNonQuery();
                return retval;
            }
        }
        // Execute a OracleCommand (that returns no resultset) against the specified OracleConnection 
        // using the provided parameters.
        // e.g.:  
        //  Dim result As Integer = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new OleDBParameter("@prodid", 24))
        // Parameters:
        // -connection - a valid OracleConnection 
        // -commandType - the CommandType (stored procedure, text, etc.)
        // -commandText - the stored procedure name or T-Oracle command 
        // -commandParameters - an array of OracleParameters used to execute the command 
        // Returns: An int representing the number of rows affected by the command 
        public int ExecuteNonQuery(OracleConnection connection, CommandType commandType, string commandText,
            params OracleParameter[] commandParameters)
        {
            if ((connection == null))
            {
                throw new ArgumentNullException("connection");
            }
            using (OracleCommand cmd = new OracleCommand())
            {
                int retval;
                bool mustCloseConnection = false;
                PrepareCommand(cmd, connection, ((OracleTransaction)(null)), commandType, commandText, commandParameters, mustCloseConnection);
                retval = cmd.ExecuteNonQuery();
                if ((mustCloseConnection))
                {
                    connection.Close();
                }
                return retval;
            }
        }
        // This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        // to the provided command.
        // Parameters:
        // -command - the OleDBCommand to be prepared
        // -connection - a valid OleDBConnection, on which to execute this command
        // -transaction - a valid OleDBTransaction, or ' null' 
        // -commandType - the CommandType (stored procedure, text, etc.)
        // -commandText - the stored procedure name or T-OleDB command
        // -commandParameters - an array of OleDBParameters to be associated with the command or ' null' if no parameters are required
        private static void PrepareCommand(OracleCommand command, OracleConnection connection,
            OracleTransaction transaction, CommandType commandType, string commandText, OracleParameter[] commandParameters,
            bool mustCloseConnection)
        {
            if ((command == null))
            {
                throw new ArgumentNullException("command");
            }
            if ((commandText == null || commandText.Length == 0))
            {
                throw new ArgumentNullException("commandText");
            }

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                mustCloseConnection = true;
            }
            else
            {
                mustCloseConnection = false;
            }

            // Associate the connection with the command
            command.Connection = connection;
            command.CommandText = commandText;

            // If we were provided a transaction, assign it.
            if (!((transaction == null)))
            {
                if (transaction.Connection == null)
                {
                    throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                }
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (!((commandParameters == null)))
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }  // PrepareCommand
        private static void AttachParameters(OracleCommand command, OracleParameter[] commandParameters)
        {
            if ((command == null))
            {
                throw new ArgumentNullException("command");
            }
            if ((!(commandParameters == null)))
            {
                foreach (OracleParameter p in commandParameters)
                {
                    if ((!(p == null)))
                    {
                        if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) && p.Value == null)
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }
        // Create and prepare a OracleCommand, and call ExecuteReader with the appropriate CommandBehavior.
        // If we created and opened the connection, we want the connection to be closed when the DataReader is closed.
        // If the caller provided the connection, we want to leave it to them to manage.
        // Parameters:
        // -connection - a valid OracleConnection, on which to execute this command 
        // -transaction - a valid OracleTransaction, or ' null' 
        // -commandType - the CommandType (stored procedure, text, etc.) 
        // -commandText - the stored procedure name or SQL command 
        // -commandParameters - an array of OracleParameters to be associated with the command or ' null' if no parameters are required 
        // -connectionOwnership - indicates whether the connection parameter was provided by the caller, or created by OralceHelper 
        // Returns: OracleReader containing the results of the command 
        private OracleDataReader ExecuteReader(OracleConnection connection, OracleTransaction transaction,
            CommandType commandType, string commandText, OracleParameter[] commandParameters,
            OracleConnectionOwnership connectionOwnership)
        {
            if ((connection == null))
            {
                throw new ArgumentNullException("connection");
            }
            bool mustCloseConnection = false;
            using (OracleCommand cmd = new OracleCommand())
            {
                try
                {
                    OracleDataReader dataReader;
                    PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, mustCloseConnection);
                    if (connectionOwnership == OracleConnectionOwnership.External)
                    {
                        dataReader = cmd.ExecuteReader();
                    }
                    else
                    {
                        dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    }
                    dataReader.Dispose();
                    return dataReader;
                }
                catch
                {
                    if ((mustCloseConnection))
                    {
                        connection.Close();
                    }
                    throw;
                }
            }
        }
        // Execute a OleDBCommand (that returns a resultset) against the specified OracleConnection 
        // using the provided parameters.
        // e.g.:  
        // Dim dr As OracleDataReader = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new OleDBParameter("@prodid", 24))
        // Parameters:
        // -connection - a valid OracleConnection 
        // -commandType - the CommandType (stored procedure, text, etc.) 
        // -commandText - the stored procedure name or SQL command 
        // -commandParameters - an array of OracleParameters used to execute the command 
        // Returns: A OracleDataReader containing the resultset generated by the command 
        public OracleDataReader ExecuteReader(OracleConnection connection, CommandType commandType,
            string commandText, params OracleParameter[] commandParameters)
        {
            return ExecuteReader(connection, ((OracleTransaction)(null)), commandType, commandText, commandParameters, OracleConnectionOwnership.External);
        }
        // Execute a OracleCommand (that returns a resultset) against the specified OracleConnection 
        // using the provided parameters. 
        // e.g.: 
        // Dataset ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24)) 
        // Parameters: 
        // -connection - a valid OracleConnection 
        // -commandType - the CommandType (stored procedure, text, etc.) 
        // -commandText - the stored procedure name or T-Oracle command 
        // -commandParameters - an array of OracleParameters used to execute the command 
        // Returns: A dataset containing the resultset generated by the command 
        public DataSet ExecuteDataset(OracleConnection connection, CommandType commandType, string commandText,
            params OracleParameter[] commandParameters)
        {
            if ((connection == null))
                throw new ArgumentNullException("connection");
            // Create a command and prepare it for execution 
            using (OracleCommand cmd = new OracleCommand())
            {
                DataSet ds = new DataSet();
                OracleDataAdapter dataAdapter = null;
                bool mustCloseConnection = false;

                PrepareCommand(cmd, connection, (OracleTransaction)null, commandType, commandText, commandParameters, mustCloseConnection);

                try
                {
                    // Create the DataAdapter & DataSet 
                    using (dataAdapter = new OracleDataAdapter(cmd))
                    {
                        // Fill the DataSet using default values for DataTable names, etc 
                        dataAdapter.Fill(ds);
                    }
                }
                finally
                {
                    if (((dataAdapter != null)))
                        dataAdapter.Dispose();
                }
                if ((mustCloseConnection))
                    connection.Close();

                // Return the dataset 
                return ds;
            }
            // ExecuteDataset 
        }
    }
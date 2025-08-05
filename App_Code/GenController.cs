using System;
using System.Data;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Collections;
using System.Web;
using System.Net.Mail;
using System.IO;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Globalization;


public class GenController
{
    private OraClientConn OraClient = null;

    public GenController()
    {
        if (OraClient == null)
        {
            OraClient = new OraClientConn();
            OraClient.InitConnection(ConfigurationManager.AppSettings["DBConn"]);
        }
    }

    //General funtions / utilities
    public String replaceNull(OracleDataReader oDataReader, String sField)
    {
        if (oDataReader[sField] == DBNull.Value)
            return "";
        else
            return oDataReader[sField].ToString();
    }
    public String replaceNull(OracleDataReader oDataReader, int iField)
    {
        if (oDataReader.IsDBNull(iField))
            return "";
        else
            return oDataReader[iField].ToString();
    }
    public String replaceNull(OdbcDataReader oDataReader, String sField)
    {
        if (oDataReader[sField] == DBNull.Value)
            return "";
        else
            return oDataReader[sField].ToString();
    }
    public String replaceNull(OdbcDataReader oDataReader, int iField)
    {
        if (oDataReader.IsDBNull(iField))
            return "";
        else
            return oDataReader[iField].ToString();
    }
    public int replaceIntZero(OracleDataReader oDataReader, String sField)
    {
        if (oDataReader[sField] == DBNull.Value)
            return 0;
        else
            return int.Parse(oDataReader[sField].ToString());
    }
    public int replaceIntZero(OracleDataReader oDataReader, int iField)
    {
        if (oDataReader.IsDBNull(iField))
            return 0;
        else
            return int.Parse(oDataReader[iField].ToString());
    }
    public int replaceIntZero(OdbcDataReader oDataReader, String sField)
    {
        if (oDataReader[sField] == DBNull.Value)
            return 0;
        else
            return int.Parse(oDataReader[sField].ToString());
    }
    public int replaceIntZero(OdbcDataReader oDataReader, int iField)
    {
        if (oDataReader.IsDBNull(iField))
            return 0;
        else
            return int.Parse(oDataReader[iField].ToString());
    }
    public double replaceDoubleZero(OracleDataReader oDataReader, String sField)
    {
        if (oDataReader[sField] == DBNull.Value)
            return 0;
        else
            return double.Parse(oDataReader[sField].ToString());
    }
    public double replaceDoubleZero(OracleDataReader oDataReader, int iField)
    {
        if (oDataReader.IsDBNull(iField))
            return 0;
        else
            return double.Parse(oDataReader[iField].ToString());
    }
    public double replaceDoubleZero(OdbcDataReader oDataReader, String sField)
    {
        if (oDataReader[sField] == DBNull.Value)
            return 0;
        else
            return double.Parse(oDataReader[sField].ToString());
    }
    public double replaceDoubleZero(OdbcDataReader oDataReader, int iField)
    {
        if (oDataReader.IsDBNull(iField))
            return 0;
        else
            return double.Parse(oDataReader[iField].ToString());
    }
    public void WriteToLogFile(string strMessage)
    {
        //Open a file for writing
        //Get a StreamWriter class that can be used to write to the file
        //string strlogFile = ConfigurationSettings.AppSettings["LogFile"];
        string strlogFile = System.Configuration.ConfigurationManager.AppSettings["LogFile"];
        System.IO.StreamWriter objStreamWriter;
        objStreamWriter = System.IO.File.AppendText(HttpContext.Current.Server.MapPath(strlogFile));

        objStreamWriter.WriteLine(DateTime.Now + "\t" + strMessage);

        //Close the stream
        objStreamWriter.Close();
    }
    public int compareTwoDate(String sDate1, String sDate2)
    {
        int iCompare = 0;
        OracleDataReader oraDataReader = null;
        String sql = "";
        try
        {
            OraClient.OraClientOpen();
            if (sDate1.Length > 0 && sDate2.Length > 0)
            {
                sql = "";
                sql = sql + " SELECT TO_DATE('" + sDate1 + "','DD/MM/YYYY') - TO_DATE('" + sDate2 + "','DD/MM/YYYY') comp_date ";
                sql = sql + " FROM   DUAL ";
                oraDataReader = OraClient.OraClientExcecuteSQLtoRDR(sql);
                if (oraDataReader.HasRows)
                {
                    if (oraDataReader.Read())
                    {
                        iCompare = int.Parse(replaceNull(oraDataReader, "comp_date"));
                    }
                }
            }
            else
            {
                iCompare = 0;
            }
        }
        catch (Exception e)
        {
            WriteToLogFile(DateTime.Now.ToString() + ": [SSOGenController.cs:getStatusTransList()] " + e.Message.ToString());
        }
        finally
        {
            if (oraDataReader != null)
            {
                oraDataReader.Close();
                oraDataReader = null;
            }
        }
        return iCompare;
    }
    public String getServerDate()
    {
        String sDate = "";
        OracleDataReader oraDataReader = null;
        String sql = "";
        try
        {
            OraClient.OraClientOpen();
            sql = "";
            sql = sql + " SELECT TO_CHAR(SYSDATE,'DD/MM/YYYY') curr_date";
            sql = sql + " FROM   DUAL ";
            oraDataReader = OraClient.OraClientExcecuteSQLtoRDR(sql);
            if (oraDataReader.HasRows)
            {
                if (oraDataReader.Read())
                {
                    sDate = replaceNull(oraDataReader, "curr_date");
                }
            }
        }
        catch (Exception e)
        {
            WriteToLogFile(DateTime.Now.ToString() + ": [SSOGenController.cs:getServerDate()] " + e.Message.ToString());
        }
        finally
        {
            if (oraDataReader != null)
            {
                oraDataReader.Close();
                oraDataReader = null;
            }
        }
        return sDate;
    }
    public String getDateFormatDD_MM_YYYY(String sDateIn)
    {
        String sDateOut = "";
        if (sDateIn.Length >= 10)
        {
            String day = sDateIn.Substring(0, 2);
            String month = sDateIn.Substring(3, 2);
            String year = sDateIn.Substring(6, 4);
            sDateOut = day + "/" + month + "/" + year;
        }
        else if (sDateIn.Length == 8)
        {
            String day = sDateIn.Substring(0, 2);
            String month = sDateIn.Substring(3, 2);
            String year = sDateIn.Substring(6, 4);
            sDateOut = day + "/" + month + "/" + year;
        }
        else
        {
            sDateOut = sDateIn;
        }
        return sDateOut;
    }
    public String getDateFormatDD_MM_YYYY2(String sDateIn)
    {
        String sDateOut = "";
        if (sDateIn.Length >= 10)
        {
            String day = sDateIn.Substring(0, 2);
            String month = sDateIn.Substring(3, 2);
            String year = sDateIn.Substring(6, 4);
            sDateOut = day + "/" + month + "/" + year;
        }
        else if (sDateIn.Length == 8)
        {
            String day = sDateIn.Substring(0, 2);
            String month = sDateIn.Substring(3, 2);
            String year = sDateIn.Substring(6, 4);
            sDateOut = day + "/" + month + "/" + year;
        }
        else
        {
            sDateOut = sDateIn;
        }
        return sDateOut;
    }
    public ArrayList tokenString(String sStr, String sParse)
    {
        ArrayList lsToken = new ArrayList();
        String s;
        int i, j;
        int nLen;
        nLen = sParse.Length;

        i = 0;
        while (true)
        {
            j = sStr.IndexOf(sParse, i);
            if (j < 0)
            {
                // last
                s = replaceNull(sStr.Substring(i));

                if (s.Length > 0)
                    lsToken.Add(s);

                break;
            }
            s = replaceNull(sStr.Substring(i, j - i));
            lsToken.Add(s);
            i = j + nLen;
        }
        return lsToken;
    }
    public String replaceNull(String sString)
    {
        String _String = "";
        if ((sString == null) || (sString.Trim().Equals("null")))
            _String = "";
        else
            _String = sString.Trim();

        return _String;
    } // replaceNull
    public int replaceIntZero(String sString)
    {
        int _result = 0;
        if ((sString == null) || (sString.Trim().Equals("null")))
            _result = 0;
        else
            _result = int.Parse(sString.Trim());

        return _result;
    } // replaceNull
    public Double replaceDoubleZero(String sString)
    {
        Double _result = 0;
        if ((sString == null) || (sString.Trim().Equals("null")))
            _result = 0;
        else
            _result = Double.Parse(sString.Trim());

        return _result;
    } // replaceNull
    public String replaceStr(String sStrValue, String sStrOld, String sStrNew)
    {
        String sStrNewValue = sStrValue;
        if (sStrValue.Length > 0 && sStrOld.Length > 0)
        {
            sStrNewValue = sStrValue.Replace(sStrOld, sStrNew);
        }
        return sStrNewValue;
    }
    public String isContains(ArrayList arrayString, String submoduleid)
    {
        String display = "none";
        bool contains = false;
        if (arrayString.Count > 0)
        {
            contains = arrayString.Contains(submoduleid);
        }
        if (contains)
        {
            display = "";
        }
        return display;
    }
    public void Alert(System.Web.UI.Page pg, string msg)
    {
        msg = msg.Replace("\n", " ");
        msg = msg.Replace("\t", " ");
        msg = msg.Replace("'", " ");

        string strScript = "<script language='JavaScript'>";
        //DateTime dt DateTime.Now;
        strScript += "alert('" + msg + "');";
        strScript += "<" + "/script>";
        Random rand = new Random();
        //if (!pg.IsStartupScriptRegistered("clientScript"))					
        //pg.RegisterStartupScript("Alert" + rand.Next(1, 50000).ToString(), strScript);
        pg.ClientScript.RegisterStartupScript(GetType(), "showalert", strScript, true);

    }
    public String sendEmailNotification(String sSMTPServer, String sFrom, ArrayList lsTo, ArrayList lsCc, ArrayList lsBcc, String sSubject, String sMessage)
    {
        String sSentStatus = "";

        try
        {
            if ((sSMTPServer.Length > 0) &&
                (sFrom.Length > 0) &&
                (lsTo.Count > 0) &&
                (sSubject.Length > 0) &&
                (sMessage.Length > 0))
            {
                MailMessage mailMssg = new MailMessage();
                String sRecipients = "";
                //String sCCs = "";
                //String sBCCs = "";

                mailMssg.From = new MailAddress(sFrom);
                mailMssg.To.Add(new MailAddress("mfadzilm@modenas.com.my"));
                mailMssg.CC.Add(new MailAddress("mfadzilm@modenas.com.my"));

                mailMssg.Subject = sSubject;
                mailMssg.Body = sMessage;

                if (sRecipients.Length > 0)
                {
                    SmtpClient o_Client = new SmtpClient(sSMTPServer);
                    //o_Client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                    o_Client.Send(mailMssg);

                    /* Old approach
                    SmtpMail.SmtpServer = sSMTPServer;
                    MailMessage mail = new MailMessage();
                    mail.From = sFrom;
                    mail.To = sRecipients;
                    mail.Cc = sCCs;
                    mail.Bcc = sBCCs;
                    mail.Subject = sSubject;
                    mail.BodyFormat = MailFormat.Text;
                    mail.Body = sMessage;
                    
                    SmtpMail.Send(mail);
                    */
                    sSentStatus = "Y";
                }
                else
                {
                    sSentStatus = "N";
                }
            }
        }
        catch (Exception e)
        {
            sSentStatus = "N";
            WriteToLogFile(DateTime.Now.ToString() + ": [GenController.cs:sendEmailNotification(String sGroupCode)] " + e.Message.ToString());
        }
        return sSentStatus;
    }
    public void Close()
    {
        if (OraClient != null)
        {
            OraClient.OraClientClose();
            OraClient = null;
        }
    }

    public void checkSession(string strSession)
    {
        if (strSession == "" || strSession == null)
        {

            HttpContext.Current.Response.Redirect("SessionExpired.aspx", true);
        }

    }

}

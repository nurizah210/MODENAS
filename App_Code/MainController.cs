using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Collections;
using System.Globalization;
using DocumentFormat.OpenXml.Spreadsheet;


public class MainController
{
    private GenController oGen = new GenController();
    private OraClientConn OraClient = null;
    private DateTimeFormatInfo ukDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
    public MainModel oMainMod = new MainModel();

    public MainController()
    {
        String connStr = ConfigurationManager.AppSettings["KASTAM"];
        OraClient = new OraClientConn();
        OraClient.InitConnection(connStr);
    }

    public object UpdateRecord(MainModel modFrame)
    {
        object result = new object();

        //OracleParameter oraParameter = new OracleParameter();
        //OracleDataReader oraDataReader = null;
        //ArrayList lsParam = new ArrayList();
        //ArrayList ls = new ArrayList();
        //String query = "";
        //String strquery = "";

        //try
        //{
        //    OraClient.OraClientOpen();
        //    if (OraClient.Conn.State.ToString() == "Open")
        //    {


        //        string DataXML = modFrame.FRM_ARRLIST;

        //        string[] ar = { "~" };
        //        string[] NewList = DataXML.Split(ar, StringSplitOptions.RemoveEmptyEntries);

        //        foreach (string element in NewList)

        //        { 
        //                    MainModel mod = new MainModel();
        //                    mod.FRM_CHASSIS = oGen.replaceNull(oraDataReader, "FRM_NO"); 
        //                    ls.Add(mod);

        //                    strquery = "";
        //                    strquery = strquery + " INSERT INTO G_VIN_NO_DC (MODEL_CODE, COLOUR, VIN_NO,ENG_NO,CUST_CODE,LN_DONO,DELY_DATE) ";
        //                    strquery = strquery + " VALUES ('" + mod.FRM_MODELCD.ToUpper().Trim() + "', '" + mod.FRM_MFG_PRODUCT_CLR.ToUpper().Trim() + "', '" + mod.FRM_CHASSIS.ToUpper().Trim() + "',";
        //                    strquery = strquery + "'" + mod.FRM_ENGINE.ToUpper().Trim() + "' ,'NOVALUE','NOVALUE',TO_DATE('" + DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt") + "','dd-mon-yyyy hh:mi:ss PM'))";



        //                    oGen.WriteToLogFile("MainController-InsertRecord: " + strquery.ToString());
        //                    OraClient.OraClientExcecuteSQL_NoClose(strquery);


        //                    result = new { status = "Y", message = "INSERTED SUCCESSFULLY", result = ls };


        //        }
        //    }

        //}
        //catch (Exception e)
        //{
        //    oGen.WriteToLogFile("MainController-InsertRecord: " + e.Message.ToString());
        //    result = new { status = "N", message = e.Message.ToString() };
        //}
        //finally
        //{
        //    OraClient.OraClientClose();
        //}
        return result;
    }

    public object InsertRecord(MainModel modFrame)
    {
        object result = new object();




        return result;
    }

}
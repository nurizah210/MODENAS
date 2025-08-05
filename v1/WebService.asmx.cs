using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Collections;

namespace vms.v1
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
        public MainController oMainCon = new MainController();

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public String InsertRecord(String CHASSIS)
        {
            String jsonResponse = "";

            HttpContext.Current.Response.ContentType = "text/json";

            MainController oMainCon = new MainController();

            MainModel mod = new MainModel();

            mod.FRM_ARRLIST = CHASSIS; 

            object result = oMainCon.InsertRecord(mod);
            //object result = oMainCon.InsertDuplicate(mod);

            jsonResponse = new JavaScriptSerializer().Serialize(result);

            return jsonResponse;
        }

        public String UpdateRecord(String CHASSIS)
        {
            String jsonResponse = "";

            HttpContext.Current.Response.ContentType = "text/json";

            MainController oMainCon = new MainController();

            MainModel mod = new MainModel();

            mod.FRM_ARRLIST = CHASSIS;

            object result = oMainCon.UpdateRecord(mod);
            //object result = oMainCon.InsertDuplicate(mod);

            jsonResponse = new JavaScriptSerializer().Serialize(result);

            return jsonResponse;
        }



    }
}

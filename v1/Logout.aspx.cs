using System;
using System.Web;
using System.Web.UI;

namespace vms.v1
{
    public partial class Logout : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
          
            GenController gen = new GenController();
            gen.WriteToLogFile("User " + Session["UserID"] + " logged out at " + DateTime.Now);

           
            Session.Clear();
            Session.Abandon();
            Response.Cookies.Clear(); 

            Response.Redirect("Login.aspx");
        }
    }
}

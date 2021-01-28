using System;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using System.Security.Principal;
using Variables;

namespace PageMaster
{
   public partial class Program : MasterPage
   {
      public string home = Variable.homeURL();
   
      public void Page_Init(object sender, EventArgs e)
      {
         if(Request.Form["logout"] == "ログアウト")
         {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
         }
         else
         {
            if(HttpContext.Current.User.Identity.IsAuthenticated == false)
            {
               Response.Redirect("/");
            }
         }
      }
   }
}
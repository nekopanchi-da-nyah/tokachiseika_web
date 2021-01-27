using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace Page_Master
{
   public partial class Program : MasterPage
   {
      public void Page_Init(object sender, EventArgs e)
      {
         if(Request.Form["logout"] == "ログアウト")
         {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
         }
      }
   }
}
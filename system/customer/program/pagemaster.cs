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
      public string imgPath = Variable.homeURL() + "assets/img/";
      public string uploadPath = Variable.homeURL() + "upload/img/";
      
      public void Page_Init(object sender, EventArgs e)
      {
         Session.Timeout = 60;
         if(Request.Form["logout"] == "ログアウト")
         {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
         }
         else
         {
            if(HttpContext.Current.User.Identity.IsAuthenticated == false || Session["担当者CD"] == null)
            {
               Response.Redirect("/");
            }
         }
      }
      
   }
}
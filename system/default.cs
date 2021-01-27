using System;
using System.Web;
using System.Web.UI;

namespace Default
{
   public partial class Program : Page
   {
      protected void Page_Init(object sender, EventArgs e)
      {
         if(User.Identity.IsAuthenticated || Session["権限ランクCD"] != null)
         {   
            if((string)Session["権限ランクCD"] == "1")
            {
               //管理者用セッションの設定
               Response.Redirect("./admin/dashboard.aspx");
            }
            else{
               //カスタマー用セッション設定
               Response.Redirect("./customer/menu.aspx");
            }
         }else
         {
            Response.Write("../login.aspx");
         }
      }
   }
}
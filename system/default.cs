using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using JfosLib;

namespace Default
{
   public partial class Program : Page
   {
      protected void Page_Init(object sender, EventArgs e)
      {
         if(User.Identity.IsAuthenticated && Session["role"] != null)
         {   
            if((string)Session["role"] == "Admin")
            {
               //管理者用
               Response.Redirect("./admin/dashboard.aspx");
            }
            else{
               //カスタマー用Session変数格納
               var sql = @"SELECT * FROM ""VWカスタマー名称"" WHERE ""担当者CD"" = '" + Session["担当者CD"] + "'; ";
               var db = new DB(sql);
               
               using(db.adp)
               {
                  var ds = new DataSet();
                  db.adp.Fill(ds);
                  var dt = ds.Tables[0];
                  var wr = dt.Rows[0];
                  
                  Session["お客様名称"] = (string)wr["お客様名称"];
                  Session["得意先名称"] = (string)wr["得意先名称"];
                  Session["店舗名称"] = (string)wr["店舗名称"];

               }
               
               Response.Redirect("./customer/menu.aspx");
            }
         }else
         {
            FormsAuthentication.SignOut();
            Response.Redirect("../login.aspx");
         }
      }
   }
}
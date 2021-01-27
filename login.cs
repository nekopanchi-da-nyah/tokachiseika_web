using System;
using System.Data;
using System.Web;
using System.Web.Security;
using Npgsql;
using JfosLib;

namespace Login
{
   partial class Program : System.Web.UI.Page
   {
      protected string msg;
      
      protected void Page_Init(object sender, EventArgs e)
      {
         if(User.Identity.IsAuthenticated)
         {
            Response.Redirect("./system/default.aspx");
         }
      }
      
      protected void Click_Login(object sender, EventArgs e)
      {
         var id = Request.Form["login_id"].ToString();
         var pass = Request.Form["password"].ToString();
         var sql = @"SELECT * FROM ""MW025お客様得意先店担当者"" WHERE ""担当者CD"" = '" + id + "' ;";
         var ds = new DataSet();
         var dt = new DataTable();
         
         var db = new DB(sql);
         
         using(db.conn){
            db.adp.Fill(ds);
            dt = ds.Tables[0];
         }
         
         if(dt.Rows.Count > 0)
         {
            /* パスワードハッシュ処理しなさそうな設計のため、ハッシュ化なし。
            var hexa = new Hash(pass);
            */
            if(dt.Rows[0]["PASSWORD"].ToString() == pass)
            {
               if(dt.Rows[0]["お客様CD"].ToString() == "0001")
               {
                  Session["role"] = "Admin";
               }
               else
               {
                  Session["role"] = "customer";
               }
               
               Session["お客様CD"] = dt.Rows[0]["お客様CD"].ToString();
               Session["得意先CD"] = dt.Rows[0]["得意先CD"].ToString();
               Session["得意先店舗CD"] = dt.Rows[0]["得意先店舗CD"].ToString();
               Session["担当者CD"] = id;
               Session["担当者名"] = dt.Rows[0]["担当者名"].ToString();
               FormTicket_Issue(id, (string)Session["role"], 60);
            }
            else{
               msg = "IDとパスワードに誤りがあります。";
            }
         }
         else
         {
            msg = "IDに誤りがあります。";
         }
      }
      
      /******************************************************************
         認証チケット発行用関数
         @param user ログインユーザー名, role 権限ランク, miuntes ログイン有効時間 
      ******************************************************************/
      protected void FormTicket_Issue(string user, string role, int minutes )
      {
         var tkt = new FormsAuthenticationTicket(
            1,
            user,
            DateTime.Now,
            DateTime.Now.AddMinutes(minutes),
            false,
            "role=" + role
         );
         var cookie = new HttpCookie(
            FormsAuthentication.FormsCookieName,
            FormsAuthentication.Encrypt(tkt)
         );
         Response.Cookies.Add(cookie);
         Response.Redirect("./system/default.aspx");
      }
   }
}
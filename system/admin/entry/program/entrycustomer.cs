using System;
using System.Data;
using System.Web;
using System.Web.UI;
using Npgsql;
using JfosLib;

namespace EntryCustomer
{
   public partial class Program : Page
   {
   
      protected DataSet customerDs = new DataSet();
      protected DataTable customerDt =  new DataTable();
      protected string idValid = "";
      protected string psValid = "";
      
      protected void Page_Init(object sender, EventArgs e)
      {
         CommboSet();
         if(Request.Form["cd"] != "")
         {
            Response.Write(Request.Form["cd"]);
         }
      }
      
      protected void CommboSet()
      {
         var sql = @"SELECT * FROM ""M011得意先WEB"" ";
         var db = new DB(sql);
         using(db.adp)
         {
            db.adp.Fill(customerDs);
            customerDt = customerDs.Tables[0];
         }
      }
      
      protected void Entry_Click(object sender, EventArgs e)
      {
         var sql = @"SELECT * FROM ""M341担当者WEB"" ;";
         var db = new DB(sql);
         var ds = new DataSet();
         var dt = new DataTable();
         var id = Request.Form["customer_code"].ToString();
         var pas = Request.Form["password"].ToString();
         var cfm = Request.Form["password_confirm"].ToString();
         var bf = false;
         
         using(db.conn)
         {
            using(db.adp)
            {
               db.adp.Fill(ds);
               dt = ds.Tables[0];
               var str = "担当者CD = '" + id + "'";
               
               var rows = dt.Select(str);
               if(rows.Length > 0)
               {
                  idValid = "そのID名は既に使われいます。別のID名を入力してください。";
                  bf = true;
               }
               if(pas != cfm)
               {
                  psValid = "パスワードが一致していません。";
                  bf = true;
               }
               if(bf == true)
               {
                  return;
               }
               
               db.conn.Open();
               var tran = db.conn.BeginTransaction();
               var hash = new Hash((string)Request.Form["password_confirm"]);
               
               
               try
               {
                  var row = dt.NewRow();
                  row["担当者CD"] = (string)Request.Form["customer_code"];
                  row["担当者名"] = (string)Request.Form["customer_name"];
                  row["権限ランクCD"] = 0;
                  row["PASSWORD"] = hash.str;
                  row["事業所CD"] = (string)Request.Form["office"];
                  row["事業所部門CD"] = (string)Request.Form["department"];
                  row["備考"] = (string)Request.Form["password"];
                  row["得意先CD"] = (string)Request.Form["customer"];
                  dt.Rows.Add(row);
                  
                  var cmd = new NpgsqlCommandBuilder(db.adp);
                  db.cmd.Transaction = tran;
                  db.adp.Update(ds);
                  tran.Commit();
               }
               catch(Exception ex)
               {
                  tran.Rollback();
                  Response.Write(ex);
               }
               finally
               {
                  db.conn.Close();
               }
            }
         }
      }
   }
}
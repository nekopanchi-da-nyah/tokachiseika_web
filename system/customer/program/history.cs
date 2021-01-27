using System;
using System.Web.UI;
using System.Data;
using Npgsql;
using JfosLib;

namespace History
{
   public partial class Program: Page
   {
      protected DataSet ds = new DataSet();
      protected DataTable dt = new DataTable();
      
      protected void Page_Init(object sender, EventArgs e)
      {
         /* initialize時に履歴テーブルを作る */
         setTable();
      }
      
      private void setTable()
      {
         var sql = @"SELECT *, ""発注年月日"" + CAST(""発注時間"" AS time) AS ""発注日時"" FROM ""D020WEB発注"" WHERE ""得意先CD"" = '" + Profile.CUSTOMER_CD + "' ;";
         var db = new DB(sql);
         
         using(db.conn)
         {
            using(var adp = new NpgsqlDataAdapter(db.cmd))
            {
               adp.Fill(ds);
               dt = ds.Tables[0];
            }
         }
      }
   }
}
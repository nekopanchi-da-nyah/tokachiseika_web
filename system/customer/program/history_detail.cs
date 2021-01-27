using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using Npgsql;
using JfosLib;

namespace History_Detail
{
   public partial class Program: Page
   {
      protected DataSet ds = new DataSet();
      protected DataTable dt = new DataTable();
      protected string code;
      protected decimal result = 0;
      
      protected void Page_Init(object sender, EventArgs e)
      {
         code = Request["code"];
         /* initialize時に明細テーブルを作る */
         setTable();
      }
      
      protected void Update_Click(object sender, EventArgs e)
      {
         updateTable();
         updateDetaile();
         Response.Redirect("./history_detail.aspx?code=" + code);

      }
      
      private void setTable()
      {
         var sql = @"
         SELECT t1.*, q1.*, t2.""得意先商品名"" FROM ""D021WEB発注詳細"" AS t1 
         INNER JOIN
         (
            SELECT ""商品CD"", string_agg(""商品画像名"", ',') AS ""商品画像名"", ""商品画像PATH""
            FROM ""M102商品画像WEB"" GROUP BY ""商品CD"", ""商品画像PATH""
         ) AS q1
         ON t1.""商品CD"" = q1.""商品CD""
         INNER JOIN ""M030得意先商品"" AS t2
         ON t1.""商品CD"" = t2.""得意先商品CD""
         WHERE t1.""WEB発注CD"" = '" + code + "' ;";
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
      
      private void updateTable()
      {
         var sql = @"SELECT ""商品CD"", ""発注個数"", ""発注金額"" , ""発注単価"" FROM ""D021WEB発注詳細"" WHERE ""WEB発注CD"" = '" + code + "' ;";
         var db = new DB(sql);
         var uds = new DataSet();
         var udt = new DataTable();
         var postAry = new List<string>();
         var postItem = new DataTable();
         
         /* Postされたデータの整理 */
         foreach(var key in Request.Form.AllKeys)
         {
            for(var i = 0; i < dt.Rows.Count; i++)
            {
               if(key == dt.Rows[i]["商品CD"].ToString())
               {
                  postAry.Add(key.ToString());
               }
            }
         }
         
         using(db.conn)
         {
            db.conn.Open();
            
            var adp = new NpgsqlDataAdapter(db.cmd);
            adp.Fill(uds);
            udt = uds.Tables[0];
            var tran = db.conn.BeginTransaction();
            
            try
            {
               for(var i = 0; i < udt.Rows.Count; i++)
               {
                  
                  for(var j = 0; j < postAry.Count; j++)
                  {
                     if(udt.Rows[i]["商品CD"].ToString() == postAry[j])
                     {
                        var unit = (decimal)udt.Rows[i]["発注単価"];
                        var quan = int.Parse(Request.Form[ postAry[j] ]);
                        udt.Rows[i]["発注個数"] = quan;
                        udt.Rows[i]["発注金額"] = unit * quan;
                        udt.Rows[i]["発注金額"] = udt.Rows[i]["発注金額"].ToString();
                        result += (unit * quan);
                     }
                  }
               }
               
               var cmd = new NpgsqlCommandBuilder(adp);
               db.cmd.Transaction = tran;
               adp.Update(uds);
               tran.Commit();
            }
            catch(Exception ex)
            {
               Response.Write(ex);
               tran.Rollback();
            }
            finally
            {
               db.conn.Close();
            }
         }
      }
      
      
      private void updateDetaile()
      {
         var sql = @"SELECT * FROM ""D020WEB発注"" WHERE ""WEB発注CD"" = '" + code + "' ;";
         var db = new DB(sql);
         var uds = new DataSet();
         var udt = new DataTable();
         
         using(db.conn)
         {
            db.conn.Open();
            
            var adp = new NpgsqlDataAdapter(db.cmd);
            adp.Fill(uds);
            udt = uds.Tables[0];
            var tran = db.conn.BeginTransaction();
            
            try
            {
               udt.Rows[0]["発注合計金額"] = result;
               udt.Rows[0]["発注修正日時"] = DateTime.Now;
               var cmd = new NpgsqlCommandBuilder(adp);
               db.cmd.Transaction = tran;
               adp.Update(uds);
               tran.Commit();
            }
            catch(Exception ex)
            {
               Response.Write(ex);
               tran.Rollback();
            }
            finally
            {
               db.conn.Close();
            }
         }
      }
   }
}
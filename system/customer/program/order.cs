using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using Npgsql;
using JfosLib;

namespace Order
{
   partial class Program : Page
   {
      protected DataSet ItemSet = new DataSet();
      protected DataTable Items;
      protected string orderCD;
      protected DataTable PostItem;
      //protected Dictionary<string, int> postItem = new Dictionary<string, int>();
      
      protected void Page_Init(object sender, EventArgs e)
      {
         setItemTable();
      }
      
      protected void OrderSubmit_Click(object sender, EventArgs e)
      {
         orderCD = DateTime.Now.ToString("yyyyMMddHHmm") + Profile.LOGIN_USER_CD;
         inputTable();
         orderDetailesInsert();
         Response.Redirect("./menu.aspx");
      }
      
      protected void setItemTable()
      {
         var sql = @"
         SELECT * FROM ""M030得意先商品"" AS t1 
         INNER JOIN 
         (
            SELECT ""商品CD"", string_agg(""商品画像名"", ',') AS ""商品画像名"", ""商品画像PATH""
            FROM ""M102商品画像WEB"" GROUP BY ""商品CD"", ""商品画像PATH""
         ) AS q1
         ON t1.""自社商品CD"" = q1.""商品CD""
         WHERE ""得意先CD"" = '" + Profile.CUSTOMER_CD + "' ";
         var db = new DB(sql);
         using(db.conn)
         {
            using(var adp = new NpgsqlDataAdapter(db.cmd))
            {
               adp.Fill(ItemSet);
               Items = ItemSet.Tables[0];
            }
         }
         return;
      }
      
      protected void inputTable()
      {
         /* input情報の整理 */
         PostItem = new DataTable();
         
         /* 商品CD */
         var column = new DataColumn();
         column.ColumnName = "商品CD";
         PostItem.Columns.Add(column);
         
         /* 個数 */
         column = new DataColumn();
         column.ColumnName = "発注個数";
         PostItem.Columns.Add(column);
         
         /* 金額 */
         column = new DataColumn();
         column.ColumnName = "金額";
         PostItem.Columns.Add(column);
         
         /* 単価 */
         column = new DataColumn();
         column.ColumnName = "単価";
         PostItem.Columns.Add(column);
         
         if(Items.Rows.Count != 0)
         {
            for(var i = 0; i < Items.Rows.Count; i++)
            {
               if(Request.Form[Items.Rows[i]["商品CD"].ToString()] != "")
               {                  
                  var row = PostItem.NewRow();
                  row["商品CD"] = Items.Rows[i]["商品CD"].ToString();
                  row["発注個数"] = int.Parse(Request.Form[Items.Rows[i]["商品CD"].ToString()]);
                  row["単価"] = (decimal)Items.Rows[i]["下代単価"];
                  var unit = (decimal)Items.Rows[i]["下代単価"];
                  var quan = int.Parse(Request.Form[Items.Rows[i]["商品CD"].ToString()]);
                  row["金額"] = unit * quan;
                  PostItem.Rows.Add(row);
               }
            }
         }
      }
      
      protected void orderDetailesInsert()
      {
         /* 詳細データ insert */
         var ds = new DataSet();
         DataTable dt;
         var sql = @"SELECT * FROM ""D021WEB発注詳細"" ;";
         var db = new DB(sql);
         var result = 0;
         
         using(db.conn)
         {
            db.conn.Open();
            
            var adp = new NpgsqlDataAdapter(db.cmd);
            adp.Fill(ds);
            dt = ds.Tables[0];
            
            var tran = db.conn.BeginTransaction();
            
            try
            {
               /* D021WEB明細 */
               for(var i = 0; i < PostItem.Rows.Count; i++)
               {
                  var row = dt.NewRow();
                  var price = 0;
                  row["WEB発注CD"] = orderCD;
                  row["商品CD"] = PostItem.Rows[i]["商品CD"];
                  row["発注個数"] = PostItem.Rows[i]["発注個数"];
                  row["発注単価"] = PostItem.Rows[i]["単価"];
                  price = int.Parse(PostItem.Rows[i]["金額"].ToString());;
                  row["発注金額"] = (int)price;
                  dt.Rows.Add(row);
                  result += (int)price;
               }
               
               var npgcb = new NpgsqlCommandBuilder(adp);
               db.cmd.Transaction = tran;
               adp.Update(ds);
               tran.Commit();
            }
            catch(Exception ex)
            {
               Response.Write(ex);
               tran.Rollback();
            }
            finally{
               db.conn.Close();
            }
         }
         
         /* insert受注データ */
         sql = @"SELECT * FROM ""D020WEB発注"" ;";
         db = new DB(sql);
         using(db.conn)
         {
            db.conn.Open();
            var adp = new NpgsqlDataAdapter(db.cmd);
            adp.Fill(ds);
            dt = ds.Tables[0];
            
            var tran = db.conn.BeginTransaction();
            
            try
            {
               /* D020WEB発注 */
               if(PostItem.Rows.Count != 0)
               {
                  var nowHours = DateTime.Now;
                  var row_1 = dt.NewRow();
                  row_1["WEB発注CD"] = orderCD;
                  row_1["得意先CD"] = Profile.CUSTOMER_CD;
                  row_1["発注合計金額"] = (decimal)result;
                  row_1["発注年月日"] = DateTime.Today;
                  row_1["発注時間"] = "1900-01-01 " + nowHours.ToString("HH:mm:ss");
                  row_1["受注区分"] = 3;
                  dt.Rows.Add(row_1);
               }
               
               var npgcb = new NpgsqlCommandBuilder(adp);
               db.cmd.Transaction = tran;
               adp.Update(ds);
               tran.Commit();
            }
            catch(Exception ex)
            {
               Response.Write(ex);
               tran.Rollback();
            }
            finally{
               db.conn.Close();
            }
         }
      }
   }
}
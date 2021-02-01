using System;
using System.Data;
using System.Web;
using System.Web.UI;
using Npgsql;
using JfosLib;

namespace Order
{
   public partial class Program : Page
   {
   
      public DataRow combo = null;
      public DataTable itemList = null;
      public DataTable delivDate = null;
   
      protected void Page_Init(object sender, EventArgs e)
      {
         set_Combo();
      }
      
      protected void set_Combo()
      {
         var sql = @"SELECT * FROM ""VWカスタマー名称"" WHERE ""担当者CD"" = '" + (string)Session["担当者CD"]  + "' ;";
         var db = new DB(sql);
         var ds = new DataSet();
         var dt = new DataTable();
         
         using(db.adp)
         {
            db.adp.Fill(ds);
            dt = ds.Tables[0];
            if(dt.Rows.Count > 0)
            {
               combo = dt.Rows[0];
            }
         }
      }
      
      protected void Search_Click(object sender, EventArgs e)
      {
         var sql = @"
            SELECT * , 
            CASE WHEN (""MW030得意先商品"".""登録年月日"" + 14) >= current_date THEN '新商品' 
               ELSE ''
            END AS ""新商品""
            FROM ""MW030得意先商品""
            INNER JOIN ""MW035商品画像"" 
            ON ""MW030得意先商品"".""自社商品CD"" = ""MW035商品画像"".""自社商品CD"" ";
         
         sql += @"ORDER BY """ + (string)Request.Form["order"]  + @""" ASC ; ";
         
         sql += @"
            SELECT 
            
            FROM ""MW020得意先店舗"" 
            WHERE ""得意先CD"" = '" + (string)Request.Form["branch"] + @"' 
            AND ""店舗CD"" = '" + (string)Request.Form["store"] + @"' ;";
         
         var db = new DB(sql);
         var ds = new DataSet();
         var dt = new DataTable();
         itemList = new DataTable();
         
         using(db.adp)
         {
            db.adp.Fill(ds);
            itemList = ds.Tables[0];
         }
         
         delivDate = currentWeek();
         
      }
      
      protected DataTable currentWeek()
      {
         var intWeek = (int)DateTime.Now.DayOfWeek - 1;
         var sql = @"
            SELECT ""注文曜日月"", ""注文曜日火"", ""注文曜日水"", ""注文曜日木"", ""注文曜日金"", ""注文曜日土"", ""注文曜日日""
            FROM ""MW020得意先店舗""
            WHERE ""お客様CD"" = '" + (string)Session["お客様CD"] + @"' 
            AND ""得意先CD"" = '" + (string)Request.Form["branch"] + @"' 
            AND ""店舗CD"" = '" + (string)Request.Form["store"] + @"' ; 

            SELECT current_date +  ""注文締時間月"", current_date +  ""注文締時間火"", current_date + ""注文締時間水"", current_date + ""注文締時間木"", current_date + ""注文締時間金"", current_date + ""注文締時間土"", current_date + ""注文締時間日"" 
            FROM ""MW020得意先店舗""
            WHERE ""お客様CD"" = '" + (string)Session["お客様CD"] + @"' 
            AND ""得意先CD"" = '" + (string)Request.Form["branch"] + @"' 
            AND ""店舗CD"" = '" + (string)Request.Form["store"] + @"' ; 
            
            SELECT ""納品曜日月"", ""納品曜日火"", ""納品曜日水"", ""納品曜日木"", ""納品曜日金"", ""納品曜日土"", ""納品曜日日""
            FROM ""MW020得意先店舗""
            WHERE ""お客様CD"" = '" + (string)Session["お客様CD"] + @"' 
            AND ""得意先CD"" = '" + (string)Request.Form["branch"] + @"' 
            AND ""店舗CD"" = '" + (string)Request.Form["store"] + @"' ;";
         
         DataTableCollection dt;
         var db = new DB(sql);
         var ds = new DataSet();
         var step = false;
         var tbl = new DataTable();
         var offset = 0;
         var actWeek = 0;
         var deliv = 0;
         
         using(db.adp)
         {
            db.adp.Fill(ds);
            dt = ds.Tables;
         }
         
         /* 現在曜日の締時間の判定 */
         if(int.Parse((string)dt[0].Rows[0][intWeek]) == 1)
         {
            var time = DateTime.Now;
            
            if(time > (DateTime)dt[1].Rows[0][intWeek])
            {
               intWeek++;
               step = true;
            }
            else
            {
               actWeek = intWeek;
               step = false;
               deliv = int.Parse((string)dt[2].Rows[0][intWeek]);
               Response.Write(deliv);
            }
         }
         
         /* 次の注文曜日が1でフラグが立っている場所を取得 */
         if(step){
            for(var i = 0; i < 7; i++)
            {
               var idx = (i + intWeek) % 6;
               if(dt[0].Rows[0][idx] != DBNull.Value){
                  if(int.Parse((string)dt[0].Rows[0][idx]) == 1)
                  {
                     actWeek = idx;
                     deliv = int.Parse((string)dt[2].Rows[0][idx]);
                     break;
                  }
               }
            }
         }
         
         /* 発注日付から納品日を加算 */
         for(var i = 1; i <= 7; i++)
         {
            if((intWeek + i) % 7 == deliv){
               offset = i;
               if(step == false){
                  offset--;
               }
               break;
            }
         }
         
         tbl.Columns.Add("発注日", Type.GetType("System.DateTime"));
         tbl.Columns.Add("加算日", Type.GetType("System.Int32"));
         tbl.Columns.Add("納品日", Type.GetType("System.DateTime"));
         
         var orderDate = DateTime.Now.Add(new TimeSpan(actWeek, 0, 0 , 0));
         var row = tbl.NewRow();
         row["発注日"] = orderDate;
         row["加算日"] = offset;
         row["納品日"] = DateTime.Now.Add(new TimeSpan(offset, 0, 0, 0));
         tbl.Rows.Add(row);
         
         return tbl;
      }
   }
}
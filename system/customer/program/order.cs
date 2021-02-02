using System;
using System.Collections.Generic;
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
      public DataTable delivDate = new DataTable();
   
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
            LEFT JOIN ""MW035商品画像"" 
            ON ""MW030得意先商品"".""自社商品CD"" = ""MW035商品画像"".""自社商品CD"" ";
         
         sql += @"ORDER BY """ + (string)Request.Form["order"]  + @""" ASC ; ";
         
         var db = new DB(sql);
         var ds = new DataSet();
         var dt = new DataTable();
         itemList = new DataTable();
         
         using(db.adp)
         {
            db.adp.Fill(ds);
            itemList = ds.Tables[0];
         }
         setCloseTime();
      }
      
      protected void setCloseTime()
      {
         var orderDate = 0;
         /* 現在曜日(int) */
         var currentWeek = (int)DateTime.Now.DayOfWeek - 1;
         /* 曜日オブジェクトのために Length = 7 固定のテーブルに分割取得 */
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
         
         var db = new DB(sql);
         var ds = new DataSet();
         var weeks = new List<WeekObj>();
         DataTableCollection dt;
         
         using(db.adp)
         {
            db.adp.Fill(ds);
            dt = ds.Tables;
         }
         /* weeks[0] に ダミーオブジェクト挿入 */
         weeks.Add(new WeekObj());
         
         /* 各曜日オブジェクト生成 */
         for(var i = 1; i <= 7; i++){
            weeks.Add( new WeekObj(){
               seq = i,
               str = Week.str[i],
               close = dt[1].Rows[0][i - 1],
               flag = dt[0].Rows[0][i - 1],
               deliv = dt[2].Rows[0][i - 1]
            });
         }
         
         /* 現在曜日含め、翌締日時算出 */
         for(var i = 1; i <= 7; i++)
         {
            var idx = currentWeek + i % 7;
            setCloseDate(weeks[idx], i - 1);
            if(checkOrderWeek(weeks[idx]))
            {
               orderDate = idx;
               break;
            }
         }
         
         /* 割り出した締日時から納品日算出 */
         for(var i = 1; i <= 7; i++)
         {
            var idx = (orderDate + (i - 1)) % 7;
            if(idx == int.Parse(weeks[orderDate].deliv.ToString()))
            {
               var add = i - 1;
               var date = (DateTime)weeks[orderDate].day.AddDays(add);
               weeks[orderDate].delivDate = date;
               break;
            }
         }
         
         /* 曜日情報を格納するメモリテーブル */
         delivDate.Columns.Add("発注日", Type.GetType("System.DateTime"));
         delivDate.Columns.Add("納品日", Type.GetType("System.DateTime"));
         
         var row = delivDate.NewRow();
         row["発注日"] = (DateTime)weeks[orderDate].day;
         row["納品日"] = (DateTime)weeks[orderDate].delivDate;
         delivDate.Rows.Add(row);
         
         
      }
      
      protected void setCloseDate(WeekObj obj, int offset)
      {
         if(obj.flag != DBNull.Value)
         {
            if((string)obj.flag == "1")
            {
               var d = (DateTime)obj.close;
               obj.day = d.AddDays(offset);
            }
         }
      }
      
      protected bool checkOrderWeek(WeekObj obj)
      {
         if(obj.flag != DBNull.Value)
         {
            if((string)obj.flag == "1")
            {
               if((DateTime)obj.day > DateTime.Now)
               {
                  return true;
               }else{
                  return false;
               }
            }
            else{
               return false;
            }
         }
         else
         {
            return false;
         }
      }
   }
   
   public class WeekObj
   {
      public int seq { get; set; }
      public string str {get; set; }
      public DateTime day { get; set; }
      public DateTime delivDate { get; set; }
      
      /* 正しいキャストで入力したかったけど、 DB null が邪魔をしてくるのでobject型でテーブルのセルごと代入 */
      public object close { get; set; }
      public object flag { get; set; }
      public object deliv { get; set; }
   }
}
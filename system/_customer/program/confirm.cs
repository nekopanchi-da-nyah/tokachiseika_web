using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Web;
using System.Web.UI;
using Npgsql;
using JfosLib;

namespace Confirm
{
   public partial class Program : Page
   {
      protected List<Post> postList = new List<Post>();
      protected List<Order> orderList = new List<Order>();
      protected List<OrderDateObj> orderDateList;
      protected AdminSequence adm = new AdminSequence();
      protected decimal total = 0;
      protected int addSeq = 0;
      
      protected void Page_Init(object sender, EventArgs e)
      {
         postOrganaize();
         orderDateList = setCloseTime();
         setItem();
         insertOrder();
      }
      
      protected void postOrganaize()
      {
         var posts = Request.Form;
         var keys = posts.AllKeys;
         
         for(var i = 0; i < keys.Length; i++)
         {
            var key = keys[i];
            var value = posts[key];
            if(value != "" && key != "submit")
            {
               postList.Add(new Post(){
                  code = key,
                  unit = decimal.Parse(value),
               });
            }
         }
      }
      
      protected void setItem()
      {
         var sql = @"
            SELECT * , COALESCE( q1.""管理番号"", '') AS ""COA管理番号"" , 
            CASE WHEN (t1.""登録年月日"" + 14) >= current_date THEN '新商品' 
               ELSE ''
            END AS ""新商品""
            FROM ""MW030得意先商品"" AS t1
            LEFT JOIN ""MW035商品画像"" AS t2
            ON t1.""自社商品CD"" = t2.""自社商品CD"" 
            LEFT JOIN
            (
               SELECT * FROM ""DW010注文明細"" 
               WHERE ""お客様CD"" = '" + Session["お客様CD"] + @"' 
               AND ""得意先CD"" = '" + Session["発注得意先CD"]  + @"' 
               AND ""納品先CD"" = '" + Session["発注納品先CD"] + @"'
               AND ""注文年月日"" = '" + (orderDateList[0].order).ToString("yyyy-MM-dd") + @"' 
            ) AS q1 ON t1.""得意先商品CD"" = q1.""相手商品CD"" 
            WHERE t1.""お客様CD"" = '" + Session["お客様CD"] + @"' 
            AND t1.""得意先CD"" = '" + Session["発注得意先CD"]  + @"' 
            AND t1.""納品先CD"" = '" + Session["発注納品先CD"] + @"' "; 
         
         
         var db = new DB(sql);
         var ds = new DataSet();
         var dt = new DataTable();
         
         using(db.adp)
         {
            db.adp.Fill(ds);
            dt = ds.Tables[0];
         }
         
         foreach(var post in postList)
         {
            var exists = dt.Select( @"得意先商品CD = '" + post.code + "' ");
            if(exists.Length == 1)
            {
               var row = exists[0];
               var sales = (decimal)row["売上単価"] * post.unit;
               var img = "../../upload/img/noimage.png";
               
               if(row["画像名"] != DBNull.Value)
               {
                  img = "../../upload/img/" + (string)row["画像名"];
               }
               
               total += sales;
               var seq = "";
               if(row["COA管理番号"] == "")
               {
                  seq = String.Format("{0: 000000000}", adm.seq++ );
               }
               else
               {
                  seq = (string)row["管理番号"];
               }
               
               orderList.Add(new Order(){
                  strAdminSeq = seq,
                  strAdminRow = "00",
                  intSlip = 1,
                  strCustomerCD = (string)Session["お客様CD"],
                  strBranchCD = (string)Session["発注得意先CD"] ,
                  strStoreCD = (string)Session["発注納品先CD"],
                  strDelivCD = (string)Session["発注納品先CD"],
                  dateOrderDate = orderDateList[0].order,
                  dateDelivDate = orderDateList[0].deliv,
                  //strLetterCD = 
                  strItemCD = (string)row["自社商品CD"],
                  //strJAN = 
                  //strJANCD = 
                  strClientItemCD = (string)row["得意先商品CD"],
                  strItemName = (string)row["得意先商品名"],
                  strChangeLate = (string)row["発注換算数"],
                  strClientItemName = (string)row["得意先商品名"],
                  //strUnit = 
                  decUnit = decimal.Round(post.unit, 2),
                  decSalesUnit = decimal.Round((decimal)row["売上単価"], 2),
                  decSales = sales,
                  strImageName = img
               });
            }
         }
         
         switch( (string)Session["端数処理区分CD"] )
         {
            case "0" :
               total = Math.Floor(total);
               break;
            
            case "1":
               total = Math.Round(total);
               break;
               
            case "2":
               total = Math.Ceiling(total);
               break;
         }
         
         total = decimal.Round(total, 2);
         // Session["orderList"] = orderList;

      }
      
      protected void insertOrder(){
         var sql = @"
            SELECT ""管理番号"", ""管理行番号"", ""お客様CD"", ""伝票種別"", ""得意先CD"", ""店舗CD"", ""納品先CD"", ""換算数"",
            ""売上金額"", ""注文年月日"", ""納品年月日"", ""商品CD"" , ""商品名"", ""相手商品名"", ""売上単価"", ""相手商品CD"", ""数量"" 
            FROM ""DW010注文明細"" 
            WHERE ""お客様CD"" = '" + Session["お客様CD"] + @"' 
            AND ""得意先CD"" = '" + Session["発注得意先CD"]  + @"' 
            AND ""納品先CD"" = '" + Session["発注納品先CD"] + @"'
            AND ""注文年月日"" = '" + orderDateList[0].order.ToString("yyyy-MM-dd") + "' ; ";
         
         var db = new DB(sql);
         var ds = new DataSet();
         var dt = new DataTable();
         
         using(db.conn)
         {
            db.adp.Fill(ds);
            dt = ds.Tables[0];
            dt.PrimaryKey = new DataColumn[] { dt.Columns["管理番号"] };
            db.conn.Open();
            var tran = db.conn.BeginTransaction();
            try
            {
               foreach(var order in orderList)
               {
                  var row = dt.Rows.Find(order.strAdminSeq);
                  if(row != null)
                  {
                     /*update*/
                     row["数量"] = order.decUnit;
                     row["売上金額"] = order.decSales;
                  }
                  else
                  {
                     row = dt.NewRow();
                     /*insert*/
                     row["管理番号"] = order.strAdminSeq;
                     row["管理行番号"] = order.strAdminRow;
                     row["伝票種別"] = order.intSlip;
                     row["お客様CD"] = order.strCustomerCD;
                     row["得意先CD"] = order.strBranchCD;
                     row["店舗CD"] = order.strStoreCD;
                     row["納品先CD"] = order.strDelivCD;
                     row["注文年月日"] = order.dateOrderDate;
                     row["納品年月日"] = order.dateDelivDate;
                     row["商品CD"] = order.strItemCD;
                     row["相手商品CD"] = order.strClientItemCD;
                     row["商品名"] = order.strItemName;
                     row["相手商品名"] = order.strClientItemName;
                     row["換算数"] = order.strChangeLate;
                     row["数量"] = order.decUnit;
                     row["売上単価"] = order.decSalesUnit;
                     row["売上金額"] = order.decSales;
                     dt.Rows.Add(row);
                  }
               }
               var ncb = new NpgsqlCommandBuilder(db.adp);
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
         
         adm.updateSequence(addSeq);
      }
      
      protected void adminSequence()
      {
         /* お客様CD = 0001 固定ということで条件は未使用 */
         var sql = @"SELECT * FROM ""MW001基本設定""; ";
         var db = new DB(sql);
         
      
      }
      
      public List<OrderDateObj> setCloseTime()
      {
         var tbl = new DataTable();
         var orderDate = 0;
         /* 現在曜日(int) */
         var currentWeek = (int)DateTime.Now.DayOfWeek - 1;
         /* 曜日オブジェクトのために Length = 7 固定のテーブルに分割取得 */
         var sql = @"
            SELECT ""注文曜日月"", ""注文曜日火"", ""注文曜日水"", ""注文曜日木"", ""注文曜日金"", ""注文曜日土"", ""注文曜日日""
            FROM ""MW020得意先店舗""
            WHERE ""お客様CD"" = '" + (string)Session["お客様CD"] + @"' 
            AND ""得意先CD"" = '" + (string)Session["発注得意先CD"] + @"' 
            AND ""店舗CD"" = '" + (string)Session["発注納品先CD"] + @"' ; 

            SELECT current_date +  ""注文締時間月"", current_date +  ""注文締時間火"", current_date + ""注文締時間水"", current_date + ""注文締時間木"", current_date + ""注文締時間金"", current_date + ""注文締時間土"", current_date + ""注文締時間日"" 
            FROM ""MW020得意先店舗""
            WHERE ""お客様CD"" = '" + (string)Session["お客様CD"] + @"' 
            AND ""得意先CD"" = '" + (string)Session["発注得意先CD"] + @"' 
            AND ""店舗CD"" = '" + (string)Session["発注納品先CD"] + @"' ; 
            
            SELECT ""納品曜日月"", ""納品曜日火"", ""納品曜日水"", ""納品曜日木"", ""納品曜日金"", ""納品曜日土"", ""納品曜日日""
            FROM ""MW020得意先店舗""
            WHERE ""お客様CD"" = '" + (string)Session["お客様CD"] + @"' 
            AND ""得意先CD"" = '" + (string)Session["発注得意先CD"] + @"' 
            AND ""店舗CD"" = '" + (string)Session["発注納品先CD"] + @"' ; ";
         
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
         
         var list = new List<OrderDateObj>();
         list.Add(new OrderDateObj(){
            order = (DateTime)weeks[orderDate].day,
            deliv = (DateTime)weeks[orderDate].delivDate
         });
         
         return list;
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
   
   public class Post
   {
      public string code;
      public decimal unit;
   }
   
   public class Order
   {
      public string     strAdminSeq; //10
      public string     strAdminRow; //2
      public int        intSlip; //1
      public string     strCustomerCD; //4
      public string     strBranchCD; //6
      public string     strStoreCD; //6
      public string     strDelivCD;  //6
      public DateTime   dateOrderDate;
      public DateTime   dateDelivDate;
      public string     strLetterCD; 
      public string     strItemCD;
      public string     strJAN;
      public string     strJANCD;
      public string     strClientItemCD;
      public string     strItemName;
      public string     strClientItemName;
      public string     strChangeLate;
      public string     strUnit;
      public decimal    decUnit;
      public decimal    decSalesUnit;
      public decimal    decSales;
      public string     strImageName;
   }
   
   public class WeekObj
   {
      public int seq;
      public string str;
      public DateTime day;
      public DateTime delivDate;
      public object close;
      public object flag;
      public object deliv;
   }
   
   public class OrderDateObj
   {
      public DateTime order;
      public DateTime deliv;
   }
   
   public class AdminSequence
   {
      public decimal seq;
      public string row;
      
      public AdminSequence()
      {
         var sql = @"SELECT * FROM ""MW001基本設定"" ";
         var db = new DB(sql);
         var ds = new DataSet();
         
         using(db.adp)
         {
            db.adp.Fill(ds);
            var dt = ds.Tables[0];
            this.seq = (decimal)dt.Rows[0]["管理番号採番値"];
            this.row = (string)dt.Rows[0]["管理行番号"];
         }
      }
      
      public void updateSequence(int add)
      {
         this.seq += add;
         var sql = @"UPDATE ""MW001基本設定"" SET ""管理番号採番値"" = " + this.seq + " ;";
         var db = new DB(sql);
         
         using(db.conn)
         {
            db.conn.Open();
            var tran = db.conn.BeginTransaction();
            try
            {
               db.cmd.ExecuteNonQuery();
               tran.Commit();
            }
            catch(Exception ex)
            {
               tran.Rollback();
               //Response.Write(ex);
            }
            finally
            {
               db.conn.Close();
            }
         }
      }
      
   }
}
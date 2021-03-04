using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using Npgsql;
using NpgsqlTypes;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using JfosLib;
using PdfFontLib;

namespace ConfirmPage
{
   public partial class Program : Page
   {
      protected decimal total = 0;
      protected List<Post> posts = new List<Post>();
      protected List<Order> orders = new List<Order>();
      public DataTable itemList;
      public OrderWeek owk;
      public DateTime orderDay;
      public DateTime delivDay;
      public DateTime closeDay;
      protected DataTable wt;
      public string strPath = "";
      
      protected void Page_Init()
      {
         owk = new OrderWeek((string)Session["お客様CD"], (string)Session["発注得意先CD"], (string)Session["発注納品先CD"]);
         orderDay = owk.weeks[owk.orderDay].day;
         delivDay = owk.weeks[owk.orderDay].deliveryDay;
         closeDay = ((DateTime)owk.weeks[owk.orderDay].day).Add(owk.weeks[owk.orderDay].time);
         postOrganaize();
         itemList = select_Items();
         createOrderObject();
         if(IsPostBack == false)
         {
            //orderWrite();
            DBWrite();
         }
      }
      
      protected void Print_Click(object sender, EventArgs e)
      {
         strPath = CreatePDF();
      }
      
      public void postOrganaize()
      {
         var keys = Request.Form.AllKeys;
         foreach(var k in keys)
         {
            if(Request.Form[k] != "" && Request.Form[k] != "confirm")
            {
               posts.Add(new Post(){
                  code = (string)k,
                  value = (string)Request.Form[k]
               });
            }
         }
      }
      
      protected DataTable select_Items()
      {
         var sql = @"
            SELECT t1.* , t2.""画像名"",
            CASE WHEN (t1.""登録年月日"" + 14) >= current_date THEN '新商品' 
               ELSE ''
            END AS ""新商品""
            FROM ""VW得意先商品"" AS t1
            LEFT JOIN ""MW035商品画像"" AS t2
            ON t1.""自社商品CD"" = t2.""自社商品CD"" 
            WHERE t1.""お客様CD"" = '" + Session["お客様CD"] + @"' 
            AND t1.""得意先CD"" = '" + Session["得意先CD"]  + @"' 
            AND t1.""納品先CD"" = '" + Session["発注納品先CD"] + @"' "; 
            
         if((bool)Session["販売停止商品非表示"])
         {
            sql += @"AND ""販売停止FLG"" <> '" + 1 + "' ";
         }
         
         var db = new DB(sql);
         var ds = new DataSet();
         var dt = new DataTable();
         
         using(db.adp)
         {
            db.adp.Fill(ds);
            dt = ds.Tables[0];
         }
         return dt;
      }
      
      protected void createOrderObject()
      {
         foreach(var post in posts)
         {
            for(var i = 0; i < itemList.Rows.Count; i++)
            {
               if((string)itemList.Rows[i]["得意先商品CD"] == post.code)
               {
                  var sales = (decimal)itemList.Rows[i]["売上単価"] * int.Parse(post.value);
                  
                   switch( (string)Session["端数処理区分CD"] )
                  {
                     case "0" :
                        sales = Math.Floor(sales);
                        break;
                     
                     case "1":
                        sales = Math.Round(sales);
                        break;
                        
                     case "2":
                        sales = Math.Ceiling(sales);
                        break;
                  }
                  
                  var img = "../../upload/img/noimage.png";
                  
                  if(itemList.Rows[i]["画像名"] != DBNull.Value)
                  {
                     img = "../../upload/img/" + (string)itemList.Rows[i]["画像名"];
                  }
                  
                  orders.Add(new Order(){
                     strAdminRow = "00",
                     intSlip = 1,
                     strClientCD = (string)Session["お客様CD"],
                     strBranchCD = (string)Session["得意先CD"],
                     strStoreCD = (string) Session["発注得意先CD"],
                     strDeliveryCD = (string)Session["発注納品先CD"],
                     dateOrderDate = orderDay,
                     dateDeliveryDate = delivDay,
                     strItemCD = (string)itemList.Rows[i]["自社商品CD"],
                     strClientItemCD = (string)itemList.Rows[i]["得意先商品CD"],
                     strItemName = (string)itemList.Rows[i]["得意先商品名"],
                     strClientItemName = (string)itemList.Rows[i]["得意先商品名"],
                     strChange =  (string)itemList.Rows[i]["発注換算数"],
                     //strUnit = 
                     intOrderUnit = int.Parse(post.value),
                     deciOrderSaleUnit = (decimal)itemList.Rows[i]["売上単価"],
                     deciOrderSales = sales,
                     strImageName = img
                  });
                  
                  total += sales;
               }
            }
         }
      }
      
      protected void DBWrite()
      {
         var sql = @"
            SELECT * FROM ""DW010注文明細""
            WHERE ""お客様CD"" = '" + Session["お客様CD"] + @"' 
            AND ""得意先CD"" = '" + Session["発注得意先CD"]  + @"' 
            AND ""納品先CD"" = '" + Session["発注納品先CD"] + @"'
            AND ""注文年月日"" = '" + closeDay.ToString("yyyy-MM-dd") + "' ; ";
         
         var db = new DB(sql);
         var ds = new DataSet();
         var dt = new DataTable();
         var successFlag = false;
         var admin = new AdminSequence();
         
         using(db.conn)
         {
            db.adp.Fill(ds);
            dt = ds.Tables[0];
            db.conn.Open();
            using(var tran = db.conn.BeginTransaction())
            {
               try
               {
                  foreach(var order in orders)
                  {
                     /*ifネスト内でコマンドをつくるので外で定義*/
                     var cmd = new NpgsqlCommand();

                     var now = DateTime.Now;
                     var hour = now.Hour;
                     var minute = now.Minute;
                     var second = now.Second;
                     var span = new TimeSpan(hour, minute, second);
                     
                     var exists = dt.Select("相手商品CD = '" + order.strClientItemCD + "' ");
                     if( exists.Length > 0 )
                     {
                        var cmdtxt = @"
                           UPDATE ""DW010注文明細"" 
                           SET 
                              ""数量"" = @q , 
                              ""売上金額"" = @s, 
                              ""更新年月日"" = @d, 
                              ""更新時間"" = @t,
                              ""更新者CD"" = @u
                           WHERE ""管理番号"" = @id";
                        cmd = new NpgsqlCommand(cmdtxt, db.conn);
                        cmd.Parameters.Add(new NpgsqlParameter("@id", NpgsqlDbType.Varchar) { Value = (string)exists[0]["管理番号"] });
                        cmd.Parameters.Add(new NpgsqlParameter("@q", NpgsqlDbType.Integer) { Value = (int)order.intOrderUnit });
                        cmd.Parameters.Add(new NpgsqlParameter("@s", NpgsqlDbType.Integer) { Value = (int)order.deciOrderSales });
                        cmd.Parameters.Add(new NpgsqlParameter("@d", NpgsqlDbType.Date) { Value = (DateTime)DateTime.Today });
                        cmd.Parameters.Add(new NpgsqlParameter("@t", NpgsqlDbType.Interval) { Value = (TimeSpan)span });
                        cmd.Parameters.Add(new NpgsqlParameter("@u", NpgsqlDbType.Varchar) { Value = (string)Session["担当者CD"] });
                     }
                     else
                     {
                        var step = admin.seq++;
                        var seq = String.Format("{0:000000000}", step);
                        var cmdtxt = @"
                           INSERT INTO ""DW010注文明細""(
                              ""管理番号"", 
                              ""管理行番号"", 
                              ""伝票種別"", 
                              ""お客様CD"", 
                              ""得意先CD"", 
                              ""店舗CD"", 
                              ""納品先CD"", 
                              ""注文年月日"", 
                              ""納品年月日"", 
                              ""商品CD"", 
                              ""相手商品CD"", 
                              ""商品名"", 
                              ""相手商品名"", 
                              ""換算数"", 
                              ""数量"", 
                              ""売上単価"", 
                              ""売上金額"", 
                              ""登録年月日"", 
                              ""登録時間"", 
                              ""登録者CD""
                           ) 
                           VALUES( 
                              @seq, 
                              @strAdminRow, 
                              @intSlip, 
                              @strClientCD, 
                              @strBranchCD, 
                              @strStoreCD, 
                              @strDeliveryCD, 
                              @dateOrderDate, 
                              @dateDeliveryDate,
                              @strItemCD,
                              @strClientItemCD,
                              @strItemName,
                              @strClientItemName,
                              @strChange,
                              @intOrderUnit,
                              @deciOrderSaleUnit,
                              @deciOrderSales,
                              @dateEntry,
                              @spanEntry,
                              @u
                           ); ";
                        
                        cmd = new NpgsqlCommand(cmdtxt, db.conn);
                        cmd.Parameters.Add(new NpgsqlParameter("@seq", NpgsqlDbType.Varchar) { Value = (string)seq });
                        cmd.Parameters.Add(new NpgsqlParameter("@strAdminRow", NpgsqlDbType.Varchar) { Value = (string)order.strAdminRow });
                        cmd.Parameters.Add(new NpgsqlParameter("@intSlip", NpgsqlDbType.Integer) { Value = (int)order.intSlip });
                        cmd.Parameters.Add(new NpgsqlParameter("@strClientCD", NpgsqlDbType.Varchar) { Value = (string)order.strClientCD });
                        cmd.Parameters.Add(new NpgsqlParameter("@strBranchCD", NpgsqlDbType.Varchar) { Value = (string)order.strBranchCD });
                        cmd.Parameters.Add(new NpgsqlParameter("@strStoreCD", NpgsqlDbType.Varchar) { Value = (string)order.strStoreCD });
                        cmd.Parameters.Add(new NpgsqlParameter("@strDeliveryCD", NpgsqlDbType.Varchar) { Value = (string)order.strDeliveryCD });
                        cmd.Parameters.Add(new NpgsqlParameter("@dateOrderDate", NpgsqlDbType.Date) { Value = (DateTime)order.dateOrderDate });
                        cmd.Parameters.Add(new NpgsqlParameter("@dateDeliveryDate", NpgsqlDbType.Date) { Value = (DateTime)order.dateDeliveryDate });
                        cmd.Parameters.Add(new NpgsqlParameter("@strItemCD", NpgsqlDbType.Varchar) { Value = (string)order.strItemCD });
                        cmd.Parameters.Add(new NpgsqlParameter("@strClientItemCD", NpgsqlDbType.Varchar) { Value = (string)order.strClientItemCD });
                        cmd.Parameters.Add(new NpgsqlParameter("@strItemName", NpgsqlDbType.Varchar) { Value = (string)order.strItemName });
                        cmd.Parameters.Add(new NpgsqlParameter("@strClientItemName", NpgsqlDbType.Varchar) { Value = (string)order.strClientItemName });
                        cmd.Parameters.Add(new NpgsqlParameter("@strChange", NpgsqlDbType.Varchar) { Value = (string)order.strChange });
                        cmd.Parameters.Add(new NpgsqlParameter("@intOrderUnit", NpgsqlDbType.Integer) { Value = (int)order.intOrderUnit });
                        cmd.Parameters.Add(new NpgsqlParameter("@deciOrderSaleUnit", NpgsqlDbType.Integer) { Value = (int)order.deciOrderSaleUnit });
                        cmd.Parameters.Add(new NpgsqlParameter("@deciOrderSales", NpgsqlDbType.Integer) { Value = (int)order.deciOrderSales });
                        cmd.Parameters.Add(new NpgsqlParameter("@dateEntry", NpgsqlDbType.Date) { Value = (DateTime)DateTime.Today });
                        cmd.Parameters.Add(new NpgsqlParameter("@spanEntry", NpgsqlDbType.Interval) { Value = (TimeSpan)span });
                        cmd.Parameters.Add(new NpgsqlParameter("@u", NpgsqlDbType.Varchar) { Value = (string)Session["担当者CD"] });
                     }
                     cmd.ExecuteNonQuery();
                  }
                  tran.Commit();
                  successFlag = true;
               }
               catch(Exception ex)
               {
                  tran.Rollback();
                  Response.Write(ex);
                  successFlag = false;
               }
               finally
               {
                  db.conn.Close();
                  if(successFlag)
                  {
                     admin.updateSequence();
                  }
               }
            }
         }
      }
      
      /* adaptarによるUPDATEがバグっているのでこの関数は封印 */
      /*
      protected void orderWrite()
      {
         var sql = @"
            SELECT * FROM ""DW010注文明細""
            WHERE ""お客様CD"" = '" + Session["お客様CD"] + @"' 
            AND ""得意先CD"" = '" + Session["発注得意先CD"]  + @"' 
            AND ""納品先CD"" = '" + Session["発注納品先CD"] + @"'
            AND ""注文年月日"" = '" + closeDay.ToString("yyyy-MM-dd") + "' ; ";
         
         var db = new DB(sql);
         var ds = new DataSet();
         wt = new DataTable();
         var successFlag = false;
         var admin = new AdminSequence();
         
         using(db.conn)
         {
            using(db.adp)
            {
               db.adp.Fill(ds);
               wt = ds.Tables[0];
               db.conn.Open();
               var tran = db.conn.BeginTransaction();
               
               try
               {
                  var ncb = new NpgsqlCommandBuilder(db.adp);
                  for(var i = 0; i < orders.Count; i++)
                  {
                     var order = orders[i];
                     var exists = wt.Select("相手商品CD = '" + order.strClientItemCD + "' ");
                     if(exists.Length > 0)
                     {
                        //update
                        exists[0]["数量"] = order.intOrderUnit;
                        exists[0]["売上金額"] = order.deciOrderSales;
                        
                     }
                     else
                     {
                        var step = admin.seq++;
                        var seq = String.Format("{0:000000000}", step);
                        //insert
                        var row = wt.NewRow();
                        row["管理番号"] = seq;
                        row["管理行番号"] = order.strAdminRow;
                        row["伝票種別"] = order.intSlip;
                        row["お客様CD"] = order.strClientCD;
                        row["得意先CD"] = order.strBranchCD;
                        row["店舗CD"] = order.strStoreCD;
                        row["納品先CD"] = order.strDeliveryCD;
                        row["注文年月日"] = order.dateOrderDate;
                        row["納品年月日"] = order.dateDeliveryDate;
                        row["商品CD"] = order.strItemCD;
                        row["相手商品CD"] = order.strClientItemCD;
                        row["商品名"] = order.strItemName;
                        row["相手商品名"] = order.strClientItemName;
                        row["換算数"] = order.strChange;
                        row["数量"] = order.intOrderUnit;
                        row["売上単価"] = order.deciOrderSaleUnit;
                        row["売上金額"] = order.deciOrderSales;
                        wt.Rows.Add(row);
                     }
                  }
                  
                  db.cmd.Transaction = tran;
                  db.adp.Update(ds);
                  tran.Commit();
                  successFlag = true;
               }
               catch(Exception ex)
               {
                  tran.Rollback();
                  Response.Write(ex);
                  successFlag = false;
               }
               finally
               {
                  db.conn.Close();
                  if(successFlag)
                  {
                     admin.updateSequence();
                  }
               }
            }
         }
      }
      */
      
      public string CreatePDF()
      {
         var sql = @"
            SELECT ""管理番号"" ,""相手商品CD"", ""相手商品名"",  ""数量"", ""売上単価"", ""売上金額""
            FROM ""DW010注文明細""
            WHERE ""お客様CD"" = '" + Session["お客様CD"] + @"' 
            AND ""得意先CD"" = '" + Session["発注得意先CD"]  + @"' 
            AND ""納品先CD"" = '" + Session["発注納品先CD"] + @"'
            AND ""注文年月日"" = '" + closeDay.ToString("yyyy-MM-dd") + @"'  
            ORDER BY ""相手商品CD"" ASC ; ";
         
         var db = new DB(sql);
         var ds = new DataSet();
         var dt = new DataTable();
         
         using(db.conn)
         {
            db.adp.Fill(ds);
            dt = ds.Tables[0];
         }
         
         //var stamp = DateTime.Now.ToString("yyMMddhhmmss");
         var stamp = "test";
         var path = "../../pdf/" + stamp + ".pdf";
         var savePath = Request.ServerVariables["APPL_PHYSICAL_PATH"] + @"pdf\" + stamp + ".pdf";
         
         using(var doc = new PdfDocument())
         {
            var tFont = new XFont( "源ゴシック" , 18, XFontStyle.Regular);
            var rFont = new XFont( "源ゴシック" , 10, XFontStyle.Regular);
            var sFont = new XFont( "源ゴシック" , 7, XFontStyle.Regular);
            
            /* A4 PD フォーマット定義用 F*/
            var width = 840;
            var height = 600; 
            var padding = 30; //縁
            
            decimal sum = 0;
            var maxPage = dt.Rows.Count / 12;
            var pointer = 0;
            string nouhin = null;
            
            for(var p = 0; p <= maxPage; p++){
               var pen = new XPen(XColors.Black, 0.5);
               var x_point = padding;
               var y_point = padding + 60;
               var page = doc.AddPage();
               var gfx = XGraphics.FromPdfPage(page);
               page.Size = PageSize.A4;
               page.Orientation = PageOrientation.Landscape;
               
               /* 補助線 */
               //gfx.DrawRectangle(pen, padding, padding, width - (padding * 2), height - (padding * 2));
               
               /* title */
               var date = DateTime.Today;
               var rect = new XRect(padding, padding, width, 30);
               gfx.DrawString(date.ToString("yyyy年MM月dd日 発注分 発注一覧表"), tFont, XBrushes.Black, rect, XStringFormats.Center);
               
               /* 印刷日 */
               rect = new XRect(width - padding - 200, padding , 200, 15);
               gfx.DrawString(DateTime.Now.ToString("yyyy年MM月dd日 HH時mm分"), rFont, XBrushes.Black, rect, XStringFormats.Center);
               
               /* お客様 */
               gfx.DrawRectangle(pen, padding, padding, 200, 15);
               rect = new XRect(padding + 5, padding , 200, 15);
               gfx.DrawString((string)Session["お客様名称"], rFont, XBrushes.Black, rect, XStringFormats.Center);
               
               /* 得意先 */
               gfx.DrawRectangle(pen, padding, padding + 15, 50, 15);
               rect = new XRect(padding, padding + 15 , 50, 15);
               gfx.DrawString("発注元", sFont, XBrushes.Black, rect, XStringFormats.Center);
               
               gfx.DrawRectangle(pen, padding + 50, padding + 15, 50, 15);
               rect = new XRect(padding + 50, padding + 15 , 50, 15);
               gfx.DrawString((string)Session["得意先CD"], sFont, XBrushes.Black, rect, XStringFormats.Center);
               
               gfx.DrawRectangle(pen, padding + 50 + 50, padding + 15, 100, 15);
               rect = new XRect(padding + 50 + 50, padding + 15 , 100, 15);
               gfx.DrawString((string)Session["得意先名称"], sFont, XBrushes.Black, rect, XStringFormats.Center);
               
               /* 部門 */
               gfx.DrawRectangle(pen, padding, padding + 30, 50, 15);
               rect = new XRect(padding, padding + 30 , 50, 15);
               gfx.DrawString("納品先", sFont, XBrushes.Black, rect, XStringFormats.Center);
               
               gfx.DrawRectangle(pen, padding + 50, padding + 30, 50, 15);
               rect = new XRect(padding + 50, padding + 30 , 50, 15);
               gfx.DrawString((string)Session["発注納品先CD"], sFont, XBrushes.Black, rect, XStringFormats.Center);
               
               gfx.DrawRectangle(pen, padding + 50 + 50, padding + 30, 100, 15);
               rect = new XRect(padding + 50 + 50, padding + 30 , 100, 15);
               gfx.DrawString((string)Session["発注納品先CD"], sFont, XBrushes.Black, rect, XStringFormats.Center);
               
               /* 納品予定 */
               rect = new XRect(padding + 250 , padding + 30 , 280, 15);
               gfx.DrawString("納品予定日：" + nouhin, rFont, XBrushes.Black, rect, XStringFormats.CenterLeft);
               
               /* 印字順 */
               rect = new XRect(padding + 400 , padding + 30 , 80, 15);
               gfx.DrawString("印字順： 商品コード", rFont, XBrushes.Black, rect, XStringFormats.CenterLeft);
               
               
               int[] col = { 0, 100, 250, 400, 550, 650, 700, 800 };
               string[] hed = {"管理番号", "商品コード", "商品名", "発注数量", "発注単価", "発注金額", "備考" };
               
               if(dt.Rows.Count == 0){
                  break;
               }
               
               for(var j = 0; j < 7; j++)
               {
                  rect = new XRect(padding + col[j], y_point, col[j + 1] - col[j] , 15);
                  gfx.DrawString(hed[j], rFont, XBrushes.Black, rect, XStringFormats.Center);
               }
               for(var j = 0; j < 13; j++)
               {
                  y_point += 30;
                  pen.DashStyle = XDashStyle.DashDot;
                  gfx.DrawLine(pen, padding, y_point, width - (padding), y_point);
                  for(var n = 0; n < 6; n++)
                  {
                     rect = new XRect(padding + col[n], y_point, col[n + 1] - col[n] , 30);
                     if(n != 4)
                     {
                        gfx.DrawString( dt.Rows[pointer][n].ToString(), rFont, XBrushes.Black, rect, XStringFormats.Center);
                     }else{
                        var sale = (decimal)dt.Rows[pointer][n];
                        var salestr = sale.ToString("#,###.##");
                        gfx.DrawString( salestr, rFont, XBrushes.Black, rect, XStringFormats.Center);
                     }
                  }
                  var pri = (decimal)dt.Rows[pointer]["売上金額"];
                  total += pri;
                  pointer++;
                  if(dt.Rows.Count == pointer){
                     y_point += 30;
                     pen.DashStyle = XDashStyle.DashDot;
                     gfx.DrawLine(pen, padding, y_point, width - (padding), y_point);
                     
                     rect = new XRect(480, y_point, 100, 45);
                     gfx.DrawString( "発注合計金額：" + total.ToString("#,#") + " 円", rFont, XBrushes.Black, rect, XStringFormats.Center);
                     
                     break;
                  }
               }
               
               if(dt.Rows.Count == 0)
               {
                  break;
               }
            }
            doc.Save(savePath);
         }
         return path;
      }
   }
   
   public class Post
   {
      public string code;
      public string value;
   }
   
   public class Order
   {
      public string strAdminSeq;
      public string strAdminRow;
      public int intSlip;
      public string strClientCD;
      public string strBranchCD;
      public string strStoreCD;
      public string strDeliveryCD;
      public DateTime dateOrderDate;
      public DateTime dateDeliveryDate;
      public string strItemCD;
      public string strClientItemCD;
      public string strItemName;
      public string strClientItemName;
      public string strChange;
      public string strUnit;
      public int intOrderUnit;
      public decimal deciOrderSaleUnit;
      public decimal deciOrderSales;
      public string strImageName;
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
      
      public void updateSequence()
      {
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
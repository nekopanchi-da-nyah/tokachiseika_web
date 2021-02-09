using System;
using System.Data;
using System.Web;
using System.Web.UI;
using JfosLib;

namespace OrderPage
{
   public partial class Program : Page
   {
   
      public DataRow combo = null;
      public DataTable itemList = null;
      public OrderWeek owk;
      public DateTime orderDay;
      public DateTime delivDay;
      public DateTime closeDay;
   
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
               Session["端数処理区分CD"] = combo["端数処理区分CD"];
            }
         }
      }
      
      protected void Search_Click(object sender, EventArgs e)
      {
         Session["発注得意先CD"] = (string)Request.Form["branch"];
         Session["発注納品先CD"] = (string)Request.Form["store"];
         Session["orderby"] = (string)Request.Form["orderby"];
         owk = new OrderWeek((string)Session["お客様CD"], (string)Session["発注得意先CD"], (string)Session["発注納品先CD"]);
         orderDay = owk.weeks[owk.orderDay].day;
         delivDay = owk.weeks[owk.orderDay].deliveryDay;
         closeDay = ((DateTime)owk.weeks[owk.orderDay].day).Add(owk.weeks[owk.orderDay].time);
         itemList = select_Items();
      }
      
      protected DataTable select_Items()
      {
         var sql = @"
            SELECT * 
            FROM ""VW得意先商品"" AS t1
            LEFT JOIN ""MW035商品画像"" AS t2
            ON t1.""自社商品CD"" = t2.""自社商品CD"" 
            LEFT JOIN
            (
               SELECT * FROM ""DW010注文明細"" 
               WHERE ""お客様CD"" = '" + Session["お客様CD"] + @"' 
               AND ""得意先CD"" = '" + Session["発注得意先CD"]  + @"' 
               AND ""納品先CD"" = '" + Session["発注納品先CD"] + @"'
               AND ""注文年月日"" = '" + orderDay.ToString("yyyy-MM-dd") + @"' 
            ) AS q1 ON t1.""得意先商品CD"" = q1.""相手商品CD"" 
            WHERE t1.""お客様CD"" = '" + Session["お客様CD"] + @"' 
            AND t1.""得意先CD"" = '" + Session["得意先CD"]  + @"' 
            AND t1.""納品先CD"" = '" + Session["発注納品先CD"] + @"' "; 
         
         sql += @"ORDER BY """ + (string)Session["orderby"]  + @""" ASC ; ";
         
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
   }
}
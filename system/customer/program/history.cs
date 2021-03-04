using System;
using System.Data;
using System.Web;
using System.Web.UI;
using JfosLib;

namespace HistoryPage
{
   public partial class Program : Page
   {
      protected DataSet ds = new DataSet();
      protected DataTable dt = new DataTable();
      protected DataTable itemList = null;
      protected DataRow combo;
      protected OrderWeek owk;
      public DateTime orderDay;
      public DateTime delivDay;
      public DateTime closeDay;
         
      
      protected void Page_Init(object sender, EventArgs e)
      {
         SetCombo();
         SetOrderList();
      }
      
      protected void Search_Click(object sender, EventArgs e)
      {
         Session["発注得意先CD"] = (string)Request.Form["branch"];
         Session["発注納品先CD"] = (string)Request.Form["store"];
         Session["orderby"] = (string)Request.Form["orderby"];
         Session["販売停止商品非表示"] = Convert.ToBoolean((string)Request.Form["item_condition"]);
         owk = new OrderWeek((string)Session["お客様CD"], (string)Session["発注得意先CD"], (string)Session["発注納品先CD"]);
         orderDay = owk.weeks[owk.orderDay].day;
         delivDay = owk.weeks[owk.orderDay].deliveryDay;
         closeDay = ((DateTime)owk.weeks[owk.orderDay].day).Add(owk.weeks[owk.orderDay].time);
         //itemList = select_Items();
      }
      
      protected void SetCombo()
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
      
      protected void SetOrderList()
      {
         var sql = @"
            SELECT * FROM ""VW注文履歴"" 
            WHERE ""お客様CD"" = '" + (string)Session["お客様CD"] + @"' 
            AND ""得意先CD"" = '" + (string)Session["得意先CD"] + @"' ";
         
         var db = new DB(sql);
         using(db.conn)
         {
            db.adp.Fill(ds);
            dt = ds.Tables[0];
         }
      }
   }
}
 
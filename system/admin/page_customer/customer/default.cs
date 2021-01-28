using System;
using System.Data;
using System.Web;
using System.Web.UI;
using JfosLib;

public partial class Default : Page
{

   protected DataSet ds = new DataSet();
   protected DataTable dt = new DataTable();
   protected string code = "";
   protected string name = "";
   protected bool postback;
   
   protected void Page_Init(object sender, EventArgs e)
   {
      if(Request.Form["search"] == "true")
      {
         postback = true;
         Search_Click();
      }
   }
   
   protected void Search_Click()
   {
      code = Request.Form["code"].ToString();
      name = Request.Form["name"].ToString();
      var sql = @"
         SELECT mt1.*, ct1.""名称"" AS ""回収SIGHT"" 
         FROM ""MW005お客様"" AS mt1 
         LEFT JOIN ""CW005回収SIGHT"" AS ct1 ON mt1.""回収SIGHTCD"" = ct1.""CD"" 
         WHERE ""お客様CD"" LIKE '%" + code + @"%' 
         AND ""お客様名称"" LIKE '%" + name + @"%' ;";
      
      var db = new DB(sql);
      using(db.adp)
      {
         db.adp.Fill(ds);
         dt = ds.Tables[0];
      }
   }
}
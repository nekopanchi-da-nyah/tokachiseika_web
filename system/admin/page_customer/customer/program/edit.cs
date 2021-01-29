using System;
using System.Data;
using System.Web;
using System.Web.UI;
using Npgsql;
using JfosLib;

public partial class Edit : Page
{
   public DataTable wt;
   public DataRow wr;
   public DataTable collectionCD = new DataTable();
   public DataTable collectionSight = new DataTable();
   public string validMsg = "";
   public string errMsg = "";
   
   protected void Page_Init(object sender, EventArgs e)
   {
      if((string)Request.Form["submit"] == "update")
      {
         Submit_Update();
      }
      if((string)Request.Form["submit"] == "delete")
      {
         Submit_Delete();
      }
      else
      {
         
         var code = (string)Request.QueryString["code"];
         var ds = new DataSet();
         
         var sql = @"
            SELECT * FROM ""CW004回収区分"" ;
            SELECT * FROM ""CW005回収SIGHT"" ; 
            SELECT * FROM ""MW005お客様"" WHERE ""お客様CD"" = '" + code + "' ;";
            
         var db = new DB(sql);
         
         using(db.adp)
         {
            db.adp.Fill(ds);
            collectionCD = ds.Tables[0];
            collectionSight = ds.Tables[1];
            wt = ds.Tables[2];
            wr = wt.Rows[0];
         }
      }
   }
   
   private void Submit_Update()
   {
      var code = (string)Request.QueryString["code"];
      var ds = new DataSet();
      var dt = new DataTable();
      var sql = @"SELECT * FROM ""MW005お客様"" WHERE ""お客様CD"" = '" + code + "' ;";
      var db = new DB(sql);
      db.adp.Fill(ds);
      dt = ds.Tables[0];
      
      db.conn.Open();
      var tran = db.conn.BeginTransaction();
      var today = DateTime.Today.ToString("yyyy/MM/dd");
      var time = DateTime.Now.ToString("HH:mm:ss");
      
      try
      {
         var row = dt.Rows[0];
         row["お客様名称"] = Request.Form["customer_name"].ToString();
         row["お客様略称"] = Request.Form["short_name"].ToString();
         row["締日"] = Request.Form["deadline"].ToString();
         row["回収区分CD"] = Request.Form["collection_code"].ToString();
         row["回収SIGHTCD"] = Request.Form["collection_sight"].ToString();
         row["回収日"] = Request.Form["collection_date"].ToString();
         row["お客様MEMO"] = Request.Form["memo"].ToString();
         row["更新年月日"] = today;
         row["更新時間"] = time;
         row["更新者CD"] = Session["担当者CD"];
         
         var npgCmdBld = new NpgsqlCommandBuilder(db.adp);
         db.cmd.Transaction = tran;
         db.adp.Update(ds);
         tran.Commit();
      }
      catch(Exception ex)
      {
         tran.Rollback();
         errMsg = ex.ToString();
      }
      finally
      {
         db.conn.Close();
      }
   }
   
   private void Submit_Delete()
   {
      var code = (string)Request.QueryString["code"];
      var sql = @"DELETE FROM ""MW005お客様"" WHERE ""お客様CD"" = '" + code + "'; ";
      var db = new DB(sql);
      var delFlg = false;
      
      using(db.conn)
      {
         db.conn.Open();
         var tran = db.conn.BeginTransaction();
         
         try
         {
            db.cmd.ExecuteNonQuery();
            tran.Commit();
            delFlg = true;
         }
         catch(Exception ex)
         {
            tran.Rollback();
            errMsg = ex.ToString();
         }
         finally
         {
            db.conn.Close();
         }
      }
      if(delFlg)
      {
         Response.Redirect("default.aspx");
      }
   }
}
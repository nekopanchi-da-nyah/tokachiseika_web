using System;
using System.Data;
using System.Web;
using System.Web.UI;
using Npgsql;
using JfosLib;

public partial class Entry : Page
{
   public DataTable collectionCD = new DataTable();
   public DataTable collectionSight = new DataTable();
   public string validMsg = "";
   public string errMsg = "";
   
   protected void Page_Init(object sender, EventArgs e)
   {
      if((string)Request.Form["submit"] == "add")
      {
         Submit_Add();
      }
   
      var sql = @"
         SELECT * FROM ""CW004回収区分"" ;
         SELECT * FROM ""CW005回収SIGHT"" ; ";
         
      var db = new DB(sql);
      
      using(db.adp)
      {
         var ds = new DataSet();
         db.adp.Fill(ds);
         
         collectionCD = ds.Tables[0];
         collectionSight = ds.Tables[1];
      }
   }
   
   private void Submit_Add()
   {
      var sql = @"SELECT * FROM ""MW005お客様"" ;";
      var db = new DB(sql);
      
      using(db.conn)
      {
         var ds = new DataSet();
         var wt = new DataTable();
         
         db.adp.Fill(ds);
         wt = ds.Tables[0];
         
         var operand = "お客様CD = " + Request.Form["customer_code"].ToString();
         var existsRows = wt.Select(operand);
         if(existsRows.Length == 0){
            db.conn.Open();
            var tran = db.conn.BeginTransaction();
            var today = DateTime.Today.ToString("yyyy/MM/dd");
            var time = new DateTimeOffset(DateTime.Now);
            try
            {
               var row = wt.NewRow();
               row["お客様CD"] = Request.Form["customer_code"].ToString();
               row["お客様名称"] = Request.Form["customer_name"].ToString();
               row["お客様略称"] = Request.Form["short_name"].ToString();
               row["締日"] = Request.Form["deadline"].ToString();
               row["回収区分CD"] = Request.Form["collection_code"].ToString();
               row["回収SIGHTCD"] = Request.Form["collection_sight"].ToString();
               row["回収日"] = Request.Form["collection_date"].ToString();
               row["お客様MEMO"] = Request.Form["memo"].ToString();
               row["登録年月日"] = today;
               row["登録時間"] = time;
               row["登録者CD"] = Session["担当者CD"];
               wt.Rows.Add(row);
               
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
         else{
            validMsg = "この[お客様CD]は既に存在します。";
         }
      }
   }
}
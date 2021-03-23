using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using Npgsql;

namespace JfosLib
{
   public class DB
   {
      /* postgre 接続文字列設定用 */
      private string server =    "localhost";
      private string port =      "7777";
      private string user_id =   "postgres";
      private string database =  "TKSWEBDB01";
      private string password =  "jfos";
      private string enlist =    "true";
      
      public string constr;
      public NpgsqlConnection conn;
      public NpgsqlCommand cmd;
      public NpgsqlDataAdapter adp;
      
      /* ついでにDB運用クラスのコンストラクタ */
      public DB(string SQL)
      {
         this.constr = "Server=" + server + ";Port=" + port + ";User ID=" + user_id + ";Password=" + password + ";Database=" + database + ";Enlist=" + enlist + ";";
         this.conn = new NpgsqlConnection(this.constr);
         this.cmd = new NpgsqlCommand();
         this.cmd.Connection = this.conn;
         this.cmd.CommandText = SQL;
         this.adp = new NpgsqlDataAdapter(this.cmd);
      }
   }
   
   /* パスワード平文処理させないためのクラス */
   public class Hash
   {
      public string str = "";
      
      public Hash(string txt)
      {
         byte[] data = Encoding.UTF8.GetBytes(txt);
         var md5 = MD5.Create();
         byte[] bs = md5.ComputeHash(data);

         //md5のリソース解放//
         md5.Clear();
         
         var hexa = new StringBuilder();
         
         foreach(var b in bs)
         {
            hexa.Append(b.ToString("x2"));
         }
         this.str = hexa.ToString();
      }
   }
   
   public class OrderWeek
   {
      public List<WeekObj> weeks;
      public int current;
      public int orderDay;
      
      public OrderWeek(string clientCD, string branchCD, string orderCD)
      {
         /*言語的に 0が日曜日なので日曜日が先 */
         var sql = @"
            SELECT  
               COALESCE(""注文曜日日"", '0' ) AS ""日"" ,
               COALESCE(""注文曜日月"", '0' ) AS ""月"" ,
               COALESCE(""注文曜日火"", '0' ) AS ""火"" ,
               COALESCE(""注文曜日水"", '0' ) AS ""水"" ,
               COALESCE(""注文曜日木"", '0' ) AS ""木"" ,
               COALESCE(""注文曜日金"", '0' ) AS ""金"" ,
               COALESCE(""注文曜日土"", '0' ) AS ""土"" 
            FROM ""MW020得意先店舗""
            WHERE ""お客様CD"" = '" + clientCD + @"' 
            AND ""得意先CD"" = '" + branchCD + @"' 
            AND ""店舗CD"" = '" + orderCD + @"' ; 

            SELECT 
               COALESCE(""注文締時間日"", '0:00:00' ) AS ""日"" ,
               COALESCE(""注文締時間月"", '0:00:00' ) AS ""月"" ,
               COALESCE(""注文締時間火"", '0:00:00' ) AS ""火"" ,
               COALESCE(""注文締時間水"", '0:00:00' ) AS ""水"" ,
               COALESCE(""注文締時間木"", '0:00:00' ) AS ""木"" ,
               COALESCE(""注文締時間金"", '0:00:00' ) AS ""金"" ,
               COALESCE(""注文締時間土"", '0:00:00' ) AS ""土"" 
            FROM ""MW020得意先店舗""
            WHERE ""お客様CD"" = '" + clientCD + @"' 
            AND ""得意先CD"" = '" + branchCD + @"' 
            AND ""店舗CD"" = '" + orderCD + @"' ; 
            
            SELECT 
               COALESCE(""納品曜日日"", '' ) AS ""日"" ,
               COALESCE(""納品曜日月"", '' ) AS ""月"" ,
               COALESCE(""納品曜日火"", '' ) AS ""火"" ,
               COALESCE(""納品曜日水"", '' ) AS ""水"" ,
               COALESCE(""納品曜日木"", '' ) AS ""木"" ,
               COALESCE(""納品曜日金"", '' ) AS ""金"" ,
               COALESCE(""納品曜日土"", '' ) AS ""土"" 
            FROM ""MW020得意先店舗""
            WHERE ""お客様CD"" = '" + clientCD + @"' 
            AND ""得意先CD"" = '" + branchCD + @"' 
            AND ""店舗CD"" = '" + orderCD + @"' ; ";
         
         var db = new DB(sql);
         var ds = new DataSet();
         DataTableCollection dt;
         
         using(db.adp)
         {
            db.adp.Fill(ds);
            dt = ds.Tables;
         }
         
         this.current = (int)DateTime.Now.DayOfWeek;
         this.weeks = new List<WeekObj>();
         
         /* 曜日オブジェクト生成 */
         for(var i = 0; i < 7; i++)
         {
            this.weeks.Add(new WeekObj(){
               flag = (string)dt[0].Rows[0][i],
               time = (TimeSpan)dt[1].Rows[0][i],
               delivery = (string)dt[2].Rows[0][i],
            });
         }
         
         /* today からの日付オブジェクト整理*/
         for(var i = 0; i < 7; i++)
         {
            var criteria = (this.current + i) % 7;
            this.weeks[criteria].day = DateTime.Today.AddDays(i);
         }
         
         /* 納品日セット */
         foreach(var week in this.weeks)
         {
            if(week.delivery != "")
            {
               week.deliveryDay = this.weeks[int.Parse(week.delivery)].day;
            }
         }
         
         /* 翌(当日含む)受注日の確定処理 */
         for(var i = 0; i < 7; i++)
         {
            var criteria = (this.current + i) % 7;
            
            if(CheckOrderCloseDate(this.weeks[criteria]))
            {
               this.orderDay = criteria;
               break;
            }
         }
      }
      
      public bool CheckOrderCloseDate(WeekObj obj)
      {
         if(obj.flag == "1")
         {
            var now = DateTime.Now;
            var hour = now.Hour;
            var minute = now.Minute;
            var second = now.Second;
            var span = new TimeSpan(hour, minute, second);
            
            if(obj.time >= span)
            {
               return true;
            }
         }
         return false;
      }
   }
   
   public class WeekObj
   {
      public string flag;
      public TimeSpan time;
      public string delivery;
      public DateTime day;
      public DateTime deliveryDay;
   }
   
   public class AdminSequence
   {
      
   }
}
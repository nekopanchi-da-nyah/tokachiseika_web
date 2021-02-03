using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Security.Cryptography;
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
      private string database =  "TSKWEBDB01";
      private string password =  "jfos";
      private string enlist =    "true";
      
      public string constr;
      public NpgsqlConnection conn;
      public NpgsqlCommand cmd;
      public NpgsqlDataAdapter adp;
      
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
   
   public static class Week
   {
      public static Dictionary<int, string> str = new Dictionary<int, string>()
      {
         { 1, "月" },
         { 2, "火" },
         { 3, "水" },
         { 4, "木" },
         { 5, "金" },
         { 6, "土" },
         { 7, "日" }
      };
   }
   
}
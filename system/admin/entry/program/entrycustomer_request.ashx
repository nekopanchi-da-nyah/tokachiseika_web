<%@ webhandler language="C#" Class="Handler" %>

using System;
using System.Data;
using System.Web;
using System.Web.UI;
using JfosLib;
using Npgsql;

public class Handler : IHttpHandler
{
   public bool IsReusable{
      get
      {
         return true;
      }
   }
   
   public void ProcessRequest(HttpContext context)
   {
      var table = context.Request.Form["table"].ToString();
      var column = context.Request.Form["column"].ToString();
      var where = context.Request.Form["where"].ToString();
      var tag1 = context.Request.Form["tag1"].ToString();
      var tag2 = context.Request.Form["tag2"].ToString();
      
      if(where != null || where != "")
      {
         var sql = "SELECT * FROM ";
         sql += @"""" + table + @""" WHERE ";
         sql += @"""" + column + @""" = ";
         sql += @"'" + where + @"'";
         var ds = new DataSet();
         var dt = new DataTable();
         var db = new DB(sql);
         var resText = "";
         using(db.conn)
         {
            using(db.adp)
            {
               db.adp.Fill(ds);
               dt = ds.Tables[0];
            }
         }
         
         for(var i = 0; i < dt.Rows.Count; i++)
         {
            resText += (string)dt.Rows[i][tag1] + "," + (string)dt.Rows[i][tag2];
            if((i + 1) != dt.Rows.Count)
            {
               resText += "!";
            }
         }
         context.Response.Write(resText);
      }
      
      /*
      context.Response.ContentType = "text/plain";
      var txt = context.Items;
      foreach(var h in txt.Keys){
         context.Response.Write(h);
      }
      context.Response.Write(" / ");
      foreach(var h in txt.Values){
         context.Response.Write(h);
      }
      */      
   }
}
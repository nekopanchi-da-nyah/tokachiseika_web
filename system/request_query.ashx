<%@ webhandler language="C#" Class="Handler" %>

using System;
using System.Data
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
      var where = context.Request.Form["cd"].ToString();
      
      if(where != null || where != "")
      {
         context.Response.Write(where);
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
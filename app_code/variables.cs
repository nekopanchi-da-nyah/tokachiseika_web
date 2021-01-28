using System;

namespace Variables
{
   public static class Variable
   {
      static string tcp = "https://";
      static string domain = "localhost:10443";
      
      public static string homeURL()
      {
         return tcp + domain + "/";
      }
   }
}
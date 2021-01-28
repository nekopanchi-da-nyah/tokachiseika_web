using System;
using System.Web.UI;
using Variables;

namespace Dashboard
{
   public partial class Program : Page
   {
      public string home = Variable.homeURL();
      public string imgPath = Variable.homeURL() + "assets/img/";
      
      protected void Page_Init(object sender, EventArgs e)
      {
         
      }
   }
}
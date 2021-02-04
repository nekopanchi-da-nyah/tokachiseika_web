using System;

namespace OrderLib
{
   public class Order
   {
      public string     strAdminSeq; //10
      public string     strAdminRow; //2
      public int        intSlip; //1
      public string     strCustomerCD; //4
      public string     strBranchCD; //6
      public string     strStoreCD; //6
      public string     strDelivCD;  //6
      public DateTime   dateOrderDate;
      public DateTime   dateDelivDate;
      public string     strLetterCD; 
      public string     strItemCD;
      public string     strJAN;
      public string     strJANCD;
      public string     strClientItemCD;
      public string     strItemName;
      public string     strClientItemName;
      public string     strChangeLate;
      public string     strUnit;
      public int        intUnit;
      public decimal    decSalesUnit;
      public decimal    decSales;
      public string     strImageName;
   }
}
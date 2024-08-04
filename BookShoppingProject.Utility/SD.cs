using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject.Utility
{
    public static class SD
    {
        //Stored Procedure, since dont want to use Magic String so that error not occur//
        public const string Proc_CoverType_Create = "SP_CreateCoverType";  //call create SP//
        public const string Proc_CoverType_Update = "SP_UpdateCoverType";
        public const string Proc_CoverType_Delete = "SP_DeleteCoverType";
        public const string Proc_GetCoverTypes = "SP_GetCoverTypes";
        public const string Proc_GetCoverType = "SP_GetCoverType";

        //Roles
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee User";
        public const string Role_Company = "Company User";
        public const string Role_Individual = "Individual User";

        //Session
        public const string Ss_session = "Cart Count Session";

        public static double GetPriceBasedOnQuantity(double quantity,double price,double price50,double price100)
        {
            if (quantity < 50)
                return price;
            else if (quantity < 100)
                return price50;
            else
                return price100;
                
        }
        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;
            for (int i=0; i<source.Length; i++)
            {
                char let = source[i];
                if(let == '<')
                {
                    inside = true;
                    continue;
                }
                if(let == '>')
                {
                    inside = false;
                    continue;
                }
                if(!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        //Order Status
        public const string OrderStatusPending = "Pending";
        public const string OrderStatusApproved = "Approved";
        public const string OrderStatusInProcessing = "Processing";
        public const string OrderStatusShipped = "Shipped";
        public const string orderStatusCancelled= "Cancelled";
        public const string OrderStatusRefunded = "Refunded";

        //Payment Status
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayPayment = "PaymentStatusDelay";
        public const string PaymentStatusRejected = "Rejected";


    }

}

//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class V_AmountSum_Billing
    {
        public int ID { get; set; }
        public string TugName1 { get; set; }
        public Nullable<double> Amount { get; set; }
        public string Currency { get; set; }
        public Nullable<double> Hours { get; set; }
        public Nullable<int> BillingID { get; set; }
        public string BillingCode { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public string CustomerName1 { get; set; }
        public Nullable<int> CustomerShipID { get; set; }
        public string CustomerShipName1 { get; set; }
        public Nullable<int> BillingTypeID { get; set; }
        public string BillingType { get; set; }
        public Nullable<int> TimeTypeID { get; set; }
        public string TimeType { get; set; }
        public Nullable<double> BillTotalAmount { get; set; }
        public string FinanceMonth { get; set; }
        public string BillingYear { get; set; }
        public string BillingMonth { get; set; }
        public Nullable<int> SchedulerID { get; set; }
        public Nullable<int> TugID { get; set; }
        public Nullable<System.DateTime> BillingDateTime { get; set; }
    }
}

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
    
    public partial class V_SpecialBillingSummarizeItem
    {
        public Nullable<int> CustomerID { get; set; }
        public string CustomerName { get; set; }
        public Nullable<int> CustomerShipID { get; set; }
        public string CustomerShipName { get; set; }
        public Nullable<int> SpecialBillingID { get; set; }
        public string BillingDateTime { get; set; }
        public Nullable<int> OrderServiceID { get; set; }
        public int SpecialBillingItemID { get; set; }
        public int SchedulerID { get; set; }
        public Nullable<double> Amount { get; set; }
        public string DepartBaseTime { get; set; }
        public string ArrivalBaseTime { get; set; }
        public string Month { get; set; }
    }
}

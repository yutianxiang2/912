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
    
    public partial class V_Inv_Scheduler
    {
        public int IDX { get; set; }
        public string DepartBaseTime { get; set; }
        public string ArrivalBaseTime { get; set; }
        public string TugName { get; set; }
        public string ServiceName { get; set; }
        public Nullable<int> TugID { get; set; }
        public string RopeUsed { get; set; }
        public string ServiceCode { get; set; }
        public Nullable<int> OrderServiceID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public Nullable<int> ServiceNatureID { get; set; }
        public Nullable<int> BillingID { get; set; }
        public string ServiceWorkDate { get; set; }
        public string ServiceWorkTime { get; set; }
    }
}

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
    
    public partial class UsersRole
    {
        public int IDX { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> RoleID { get; set; }
        public string IsAdmin { get; set; }
        public Nullable<int> OwnerID { get; set; }
        public string CreateDate { get; set; }
        public Nullable<int> AddUserID { get; set; }
        public string LastUpDate { get; set; }
        public string System { get; set; }
        public string UserDefinedCol1 { get; set; }
        public string UserDefinedCol2 { get; set; }
        public string UserDefinedCol3 { get; set; }
        public string UserDefinedCol4 { get; set; }
        public Nullable<double> UserDefinedCol5 { get; set; }
        public Nullable<int> UserDefinedCol6 { get; set; }
        public Nullable<int> UserDefinedCol7 { get; set; }
        public Nullable<int> UserDefinedCol8 { get; set; }
        public string UserDefinedCol9 { get; set; }
        public string UserDefinedCol10 { get; set; }
    
        public virtual UserInfor UserInfor { get; set; }
    }
}
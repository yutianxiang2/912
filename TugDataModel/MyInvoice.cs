using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class MyBillingItem
    {
        public int IDX { get; set; }
        public Nullable<int> ItemID { get; set; }
        public string ItemValue { get; set; }
        public string ItemLabel { get; set; }
        public Nullable<double> UnitPrice { get; set; }
        public Nullable<double> Price { get; set; }
        public string Currency { get; set; }
        public Nullable<int> TypeID { get; set; }
        public string TypeValue { get; set; }
        public string TypeLabel { get; set; }
    }

    public class MyScheduler
    {
        public int SchedulerID { get; set; }
        public int ServiceNatureID { get; set; }
        public int TugID { get; set; }
        public string TugCnName { get; set; }
        //public string TugEnName { get; set; }
        //public string TugSimpleName { get; set; }

        public string TugPower { get; set; }

        //public string InformCaptainTime { get; set; }
        //public string CaptainConfirmTime { get; set; }
        public string DepartBaseTime { get; set; }
        //public string ArrivalShipSideTime { get; set; }
        //public string WorkCommencedTime { get; set; }
        //public string WorkCompletedTime { get; set; }
        public string ArrivalBaseTime { get; set; }

        /// <summary>
        /// 工作时间：ArrivalBaseTime - DepartBaseTime
        /// </summary>
        public string WorkTime { get; set; }
        /// <summary>
        /// 按照计时方式换算后的实际消耗时间
        /// </summary>
        public double WorkTimeConsumption { get; set; }


        /// <summary>
        /// 服务单价
        /// </summary>
        public double UnitPrice { get; set; }

        /// <summary>
        /// 根据计费类型不同，显示的服务价格也不同。比如全包：协议收费；条款：按时间收费（WorkTimeConsumption * UnitPrice）
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// 合计港币
        /// </summary>
        public double SubTotaHKS { get; set; }

        /// <summary>
        /// 折扣后合计港币
        /// </summary>
        public double DiscountSubTotalHKS { get; set; }


        /// <summary>
        /// 折扣价格
        /// </summary>
        public double DiscoutPrice { get; set; }

        public string RopeUsed { get; set; }
        public int RopeNum { get; set; }
        //public string Remark { get; set; }

        public List<MyBillingItem> BillingItems { get; set; }


        /// <summary>
        /// 燃油附加费单价
        /// </summary>
        public double UnitPriceOfFeulFee { get; set; }

        /// <summary>
        /// 燃油附加费折扣价
        /// </summary>
        public double DiscoutPriceOfFeulFee { get; set; }
        /// <summary>
        /// 燃油附加费价格
        /// </summary>
        public double PriceOfFeulFee { get; set; }
        /// <summary>
        /// 总计港币
        /// </summary>
        public double TotalHKs { get; set; }

    }

    public class MyService
    {
        public int OrderServicId { get; set; }
        public int ServiceId { get; set; }

        public string ServiceName { get; set; }

        public string ServiceWorkDate { get; set; }

        public string ServiceWorkPlace { get; set; }
    }

    public class MyInvoice
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }

        public string CustomerShipName { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        //public int OrderID { get; set; }

        /// <summary>
        /// 多个订单ID，以','分隔
        /// </summary>
        public string OrderIDs { get; set; }

        public string IsShowShipLengthRule { get; set; }

        public string IsShowShipTEUSRule { get; set; }

        /// <summary>
        /// 客户船长
        /// </summary>
        public int CustomerShipLength { get; set; }

        /// <summary>
        /// 客户船箱量
        /// </summary>
        public int CustomerShipTEUS { get; set; }
        /// <summary>
        /// 订单流水号
        /// </summary>
        //public string OrderCode { get; set; }


        /// <summary>
        /// 服务内容  key:value = 服务ID : 服务名称
        /// </summary>
        public Dictionary<int, MyService> ServiceNature { get; set; }

        /// <summary>
        /// 账单ID
        /// </summary>
        public int BillingID { get; set; }

        /// <summary>
        /// 客户计费方案ID
        /// </summary>
        public int BillingTemplateID { get; set; }        
        /// <summary>
        /// 账单流水号
        /// </summary>
        public string BillingCode { get; set; }

        /// <summary>
        /// 计费类型ID
        /// </summary>
        public int BillingTypeID { get; set; }

        /// <summary>
        /// 计费类型值
        /// </summary>
        public string BillingTypeValue { get; set; }

        /// <summary>
        /// 计费类型名称
        /// </summary>
        public string BillingTypeLabel { get; set; }

        /// <summary>
        /// 计时方式ID
        /// </summary>
        public int TimeTypeID { get; set; }

        /// <summary>
        /// 计时方式Value
        /// </summary>
        public string TimeTypeValue { get; set; }

        /// <summary>
        /// 计时方式名称
        /// </summary>
        public string TimeTypeLabel { get; set; }

        
        /// <summary>
        /// 折扣系数
        /// </summary>
        public double Discount { get; set; }
        /// <summary>
        /// 多个调度， key:value = 服务ID:调度对象
        /// </summary>
        public Dictionary<int, List<MyScheduler>> Schedulers { get; set; }

        /// <summary>
        /// 共计港币
        /// </summary>
        public double GrandTotalHKS { get; set; }


        /// <summary>
        /// 账单月份
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        /// 作业号，财务手动输入
        /// </summary>
        public string JobNo { get; set; }

        /// <summary>
        /// 账单备注信息
        /// </summary>
        public string Rmark { get; set; }
    }


    public class MyCustomField
    {
        public int IDX { get; set; }

        public string CustomValue { get; set; }

        public string CustomLabel { get; set; }

        public string FormulaStr { get; set; }
    }

    public class MyCredit
    {
        public int IDX { get; set; }
        public Nullable<int> OrderID { get; set; }

        public Nullable<int> BillingID { get; set; }

        public string CreditCode { get; set; }
        public string CreditContent { get; set; }
        public Nullable<double> CreditAmount { get; set; }
        public string Remark { get; set; }

        public Nullable<int> OwnerID { get; set; }
        public string CreateDate { get; set; }
        public Nullable<int> UserID { get; set; }

        public string LastUpDate { get; set; }
    }


    public class MyOrderService
    {
        public int OrderServiceId { get; set; }

        public int ServiceNatureId{get;set;}

        public string ServiceNatureValue { get; set; }

        public string ServiceNatureLabel { get; set; }

        public string ServiceWorkDate { get; set; }

        public string ServiceWorkTime { get; set; }

        public string ServiceEstimatedCompletionTime { get; set; }

        public string ServiceWorkPlace { get; set; }
    }


    public class MyFuelFee
    {
        public int IDX { get; set; }
        public DateTime EffectiveDate { get; set; }
        public double Price { get; set; }
    }

    public class MySpecialInvoice{
        public int BillingID { get; set; }

        public int CustomerID { get; set; }

        public string CustomerName { get; set; }

        public double Amount { get; set; }

        public string Status { get; set; }

        public int  Phase { get; set; }

        public string InvoiceType { get; set; }
        public string Month { get; set; }

        public string BillingCode { get; set; }

        public string BillingRemark { get; set; }

        public double FeulUnitPrice { get; set; }

        public double ServiceUnitPrice { get; set; }

        public List<MySpecialBillingItem> SpecialBillingItems { get; set; }
    }

    public class MySpecialBillingItem
    {
        public int SpecialBillingID { get; set; }
        public int OrderServiceID { get; set; }
        public string ServiceDate { get; set; }
        public int ServiceNatureID { get; set; }
        public string ServiceNatureValue { get; set; }
        public string ServiceNature { get; set; }

        public string CustomerShipName { get; set; }
        public int TugNumber { get; set; }

        public double ServiceUnitPrice { get; set; }

        public double FeulUnitPrice { get; set; }
    }
}

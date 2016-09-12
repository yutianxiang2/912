using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Module
{
    class ConstValue
    {
        public const string ComparisonOperator_BW = "bw";	//开始于
        public const string ComparisonOperator_CN = "cn";	//包含
        public const string ComparisonOperator_EQ = "eq";	//等于
        public const string ComparisonOperator_EW = "ew";  	//结束于
        public const string ComparisonOperator_GE = "ge";   //大于或等于
        public const string ComparisonOperator_GT = "gt"; 	//大于
        public const string ComparisonOperator_LE = "le"; 	//小于或等于
        public const string ComparisonOperator_LT = "lt";	//小于


        #region 订单的账单状态
        public const string HAS_NO_INVOICE = "1";
        public const string HAS_INVOICE_NOT_IN_FLOW = "2";
        public const string HAS_INVOICE_IN_FLOW = "3";
        #endregion
    }
}

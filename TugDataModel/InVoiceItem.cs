using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class InVoiceItem
    {
        public int SchedulerID { get; set; } 

        public int ItemID { get; set; } 

        public double UnitPrice { get; set; } 

        public string Currency { get; set; } 
    }

    public class InVoiceSummaryItem
    {
        public int SchedulerID { get; set; }  
  
        public double Amount { get; set; }  //总计价格
        public double FuelPrice { get; set; } //每个调度的燃油价格
        public double Hours { get; set; }   //耗时
        public string Currency { get; set; }
    }
}

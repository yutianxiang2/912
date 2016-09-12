using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace BusinessLogic.Module
{
    public class FinanceLogic
    {
        static public MyInvoice GenerateInvoice(int orderId, string orderDate)
        {
            DataModel.TugDataEntities db = new DataModel.TugDataEntities();

            MyInvoice _invoice = new MyInvoice();

            //List<V_Invoice> list = db.V_Invoice.Where(u => u.OrderID == orderId).OrderBy(u => u.ServiceNatureID).Select(u => u).ToList();

            var list = db.V_Invoice.Where(u => u.OrderID == orderId).OrderBy(u => u.OrderServiceID).Select(u => u);

            if (list != null)
            {
                //_invoice.CustormerID = (int)list.FirstOrDefault().CustomerID;
                //_invoice.CustomerName = list.FirstOrDefault().CustomerName;
                _invoice.OrderIDs = ((int)list.FirstOrDefault().OrderID).ToString();
                _invoice.JobNo = list.FirstOrDefault().JobNo;
                _invoice.Rmark = list.FirstOrDefault().BillingRemark;
                _invoice.BillingTemplateID = (int)list.FirstOrDefault().BillingTemplateID;
                //_invoice.OrderCode = list.FirstOrDefault().OrderCode;

                Dictionary<int, MyService> dicServiceNature = new Dictionary<int, MyService>();
                //var services2 = list.Select(u => new {u.ServiceNatureID, u.ServiceNatureLabel}).ToList();
                var services = list.Select(u => new { u.OrderServiceID, u.ServiceNatureID, u.ServiceNatureLabel, u.ServiceWorkDate, u.ServiceWorkPlace }).Distinct().ToList();
                //var services = db.V_Invoice.Where(u => u.OrderID == orderId).OrderBy(u => u.ServiceNatureID).Select(u => new {u.ServiceNatureID, u.ServiceNatureLabel}).Distinct().ToList();

                Dictionary<int, List<MyScheduler>> dicSchedulers = new Dictionary<int, List<MyScheduler>>();

                double grandTotal = 0;

                if (services != null && services.Count > 0)
                {
                    foreach (var item in services)
                    {
                        MyService ms = new MyService();
                        ms.ServiceId = (int)item.ServiceNatureID;
                        ms.ServiceName = item.ServiceNatureLabel;
                        ms.ServiceWorkDate = item.ServiceWorkDate;
                        ms.ServiceWorkPlace = item.ServiceWorkPlace;
                        dicServiceNature.Add(ms.ServiceId, ms);

                        var ships = list.Where(u => u.ServiceNatureID == item.ServiceNatureID)
                            .Select(u => new {u.SchedulerID, u.TugID, u.TugName1, u.TugName2 }).Distinct()
                            .OrderBy(u => u.TugName1).ToList();

                        List<MyScheduler> listScheduler = new List<MyScheduler>();

                        if (ships != null && ships.Count > 0)
                        {
                            foreach (var ship in ships)
                            {
                                MyScheduler sch = new MyScheduler();
                                sch.TugID = (int)ship.TugID;
                                sch.TugCnName = ship.TugName1;
                                
                                //sch.TugEnName = ship.TugName2;
                                //sch.TugSimpleName = ship.TugSimpleName;
                                //sch.TugPower = ship.Power;
                                var schedulers = list.Where(u => u.ServiceNatureID == item.ServiceNatureID && u.SchedulerID == ship.SchedulerID)
                                    .OrderBy(u => u.OrderID).OrderBy(u => u.ServiceNatureID)
                                    .Select(u => new
                                    {
                                        u.SchedulerID,
                                        u.TugID,
                                        u.TugName1,
                                        u.TugName2,
                                        u.InformCaptainTime,
                                        u.CaptainConfirmTime,
                                        u.DepartBaseTime,
                                        u.ArrivalShipSideTime,
                                        u.WorkCommencedTime,
                                        u.WorkCompletedTime,
                                        u.ArrivalBaseTime,
                                        u.UnitPrice,
                                        u.RopeUsed,
                                        u.RopeNum,
                                        u.OrderSchedulerRemark,
                                        u.BillingItemIDX,
                                        u.ItemID,
                                        u.BillingItemValue,
                                        u.BillingItemLabel,
                                        u.Currency,
                                        u.PositionTypeID
                                    }).OrderBy(u => u.ItemID).ToList();

                                if (schedulers != null && schedulers.Count > 0)
                                {
                                    sch.SchedulerID = (int)schedulers[0].SchedulerID;
                                    //sch.InformCaptainTime = schedulers[0].InformCaptainTime;
                                    //sch.CaptainConfirmTime = schedulers[0].CaptainConfirmTime;
                                    sch.DepartBaseTime = schedulers[0].DepartBaseTime;
                                    //sch.ArrivalShipSideTime = schedulers[0].ArrivalShipSideTime;
                                    //sch.WorkCommencedTime = schedulers[0].WorkCommencedTime;
                                    //sch.WorkCompletedTime = schedulers[0].WorkCompletedTime;
                                    sch.ArrivalBaseTime = schedulers[0].ArrivalBaseTime;

                                    int iDiffHour, iDiffMinute;
                                    BusinessLogic.Utils.CalculateTimeDiff(sch.DepartBaseTime, sch.ArrivalBaseTime, out iDiffHour, out iDiffMinute);
                                    sch.WorkTime = iDiffHour.ToString() + "h." + iDiffMinute.ToString("D2") + "m.";

                                    //_invoice.BillingTypeID = (int)list[0].BillingTypeID;
                                    //_invoice.BillingTypeValue = list[0].BillingTypeValue;
                                    //_invoice.BillingTypeLabel = list[0].BillingTypeLabel;
                                    //_invoice.TimeTypeID = (int)list[0].TimeTypeID;
                                    //_invoice.TimeTypeValue = list[0].TimeTypeValue;
                                    //_invoice.TimeTypeLabel = list[0].TimeTypeLabel;

                                    sch.WorkTimeConsumption = BusinessLogic.Utils.CalculateTimeConsumption(iDiffHour, iDiffMinute, (int)list.FirstOrDefault().TimeTypeID, list.FirstOrDefault().TimeTypeValue, list.FirstOrDefault().TimeTypeLabel);

                                    sch.UnitPriceOfFeulFee = GetFuelFee(ms.ServiceWorkDate);                             
                                    sch.DiscoutPriceOfFeulFee = GetDiscoutPriceOfFuelFee(_invoice.BillingTemplateID);
                                    sch.PriceOfFeulFee = CalculateFuelFee(ms.ServiceWorkDate, sch.WorkTimeConsumption) + sch.DiscoutPriceOfFeulFee;

                                    double servicePrice = (double)schedulers.FirstOrDefault(u => u.BillingItemValue.StartsWith("A")).UnitPrice;
                                    sch.UnitPrice = servicePrice;
                                    if (((int)list.FirstOrDefault().BillingTypeID == 5 || list.FirstOrDefault().BillingTypeValue == "0" || list.FirstOrDefault().BillingTypeLabel == "全包")
                                        || ((int)list.FirstOrDefault().BillingTypeID == 6 || list.FirstOrDefault().BillingTypeValue == "1" || list.FirstOrDefault().BillingTypeLabel == "半包"))
                                        sch.Price = servicePrice;
                                    else
                                        sch.Price = servicePrice * sch.WorkTimeConsumption;


                                    sch.RopeUsed = schedulers[0].RopeUsed;
                                    sch.RopeNum = (int)schedulers[0].RopeNum;
                                    //sch.Remark = schedulers[0].OrderSchedulerRemark;


                                    double serviceNaturePrice = 0;
                                    double upTotalPrice = 0;
                                    double midTotalPrice = 0;
                                    double discoutPrice = 0;


                                    #region 一条船的费用项目
                                    List<MyBillingItem> billingItems = new List<MyBillingItem>();
                                    foreach (var subItem in schedulers)
                                    {
                                        MyBillingItem bit = new MyBillingItem();
                                        bit.IDX = subItem.BillingItemIDX;
                                        bit.ItemID = subItem.ItemID;
                                        bit.ItemValue = subItem.BillingItemValue;
                                        bit.ItemLabel = subItem.BillingItemLabel;
                                        bit.UnitPrice = subItem.UnitPrice;

                                        //if (subItem.ItemID == 22 || subItem.BillingItemValue == "E80" || subItem.BillingItemLabel == "燃油附加费")
                                        //{
                                        //    bit.UnitPrice = GetFuelFee(orderDate);
                                        //    bit.Price = CalculateFuelFee(orderDate, sch.WorkTimeConsumption);
                                        //    //bit.Price = subItem.UnitPrice * sch.WorkTimeConsumption;
                                        //}
                                        if (subItem.ItemID == 23 || subItem.BillingItemValue == "C82" || subItem.BillingItemLabel == "拖缆费")
                                            bit.Price = subItem.UnitPrice * sch.RopeNum;
                                        else
                                            bit.Price = subItem.UnitPrice;

                                        bit.Currency = subItem.Currency;
                                        bit.TypeID = subItem.PositionTypeID;


                                        if(subItem.BillingItemValue.StartsWith("A"))
                                            serviceNaturePrice += (double)bit.Price;
                                        else if (subItem.BillingItemValue.StartsWith("B"))
                                            upTotalPrice += (double)bit.Price;
                                        else if(subItem.BillingItemValue.StartsWith("C"))
                                        {
                                            if(subItem.BillingItemValue.Equals("C78"))
                                                discoutPrice += (double)bit.Price;
                                            else if(subItem.BillingItemValue.Equals("C82"))
                                                midTotalPrice += (double)bit.Price;

                                        }

                                        //totalPrice += upTotalPrice + midTotalPrice;

                                        billingItems.Add(bit);
                                    }
                                    #endregion

                                    sch.SubTotaHKS = serviceNaturePrice; //+upTotalPrice;
                                    sch.DiscountSubTotalHKS = sch.SubTotaHKS + discoutPrice;

                                    sch.TotalHKs = sch.DiscountSubTotalHKS + midTotalPrice + sch.PriceOfFeulFee;

                                    sch.BillingItems = billingItems;

                                    grandTotal += sch.TotalHKs;
                                }

                                listScheduler.Add(sch);
                            }

                        }

                        dicSchedulers.Add((int)item.ServiceNatureID, listScheduler);

                    }
                }

                _invoice.ServiceNature = dicServiceNature;
                _invoice.Schedulers = dicSchedulers;

                _invoice.BillingID = list.FirstOrDefault().BillingID;
                //_invoice.BillingTemplateID = (int)list.FirstOrDefault().BillingTemplateID;
                _invoice.BillingCode = list.FirstOrDefault().BillingCode;
                _invoice.BillingTypeID = (int)list.FirstOrDefault().BillingTypeID;
                _invoice.BillingTypeValue = list.FirstOrDefault().BillingTypeValue;
                _invoice.BillingTypeLabel = list.FirstOrDefault().BillingTypeLabel;
                _invoice.TimeTypeID = (int)list.FirstOrDefault().TimeTypeID;
                _invoice.TimeTypeValue = list.FirstOrDefault().TimeTypeValue;
                _invoice.TimeTypeLabel = list.FirstOrDefault().TimeTypeLabel;
                _invoice.Month = list.FirstOrDefault().Month;

                _invoice.GrandTotalHKS = Math.Round(grandTotal,2);
            }

            return _invoice;
        }


        static public MyInvoice GenerateInvoice2(int billingId)
        {
            DataModel.TugDataEntities db = new DataModel.TugDataEntities();

            MyInvoice _invoice = new MyInvoice();

            //List<V_Invoice> list = db.V_Invoice.Where(u => u.OrderID == orderId).OrderBy(u => u.ServiceNatureID).Select(u => u).ToList();

            var list = db.V_Invoice2.Where(u => u.BillingID == billingId).OrderBy(u => u.OrderID).ThenBy(u=>u.OrderServiceID).ThenBy(u => u.SchedulerID).Select(u => u);

            string strOrderIds = "";
            var _orderIds = list.Where(u => u.BillingID == billingId).Select(u => new { u.OrderID }).Distinct().ToList();
            if (_orderIds != null)
            {
                foreach (var item in _orderIds)
                {
                    strOrderIds += item.OrderID.ToString() + ",";
                }
            }

            if (strOrderIds != "")
            {
                strOrderIds = strOrderIds.Substring(0, strOrderIds.Length - 1);
            }

            if (list != null)
            {
                _invoice.CustomerID = (int)list.FirstOrDefault().CustomerID;
                _invoice.CustomerName = list.FirstOrDefault().CustomerName;
                _invoice.CustomerShipName = list.FirstOrDefault().ShipName;
                _invoice.OrderIDs = strOrderIds;
                _invoice.BillingID = list.FirstOrDefault().BillingID;
                _invoice.IsShowShipLengthRule = list.FirstOrDefault().IsShowShipLengthRule;
                _invoice.IsShowShipTEUSRule = list.FirstOrDefault().IsShowShipTEUSRule;
                _invoice.CustomerShipLength = BusinessLogic.Module.Util.toint(list.FirstOrDefault().Length);
                _invoice.CustomerShipTEUS = BusinessLogic.Module.Util.toint(list.FirstOrDefault().TEUS);
                _invoice.BillingTemplateID = (int)list.FirstOrDefault().BillingTemplateID;
                _invoice.BillingCode = list.FirstOrDefault().BillingCode;
                _invoice.BillingTypeID = (int)list.FirstOrDefault().BillingTypeID;
                _invoice.BillingTypeValue = list.FirstOrDefault().BillingTemplateTypeValue;
                _invoice.BillingTypeLabel = list.FirstOrDefault().BillingTemplateTypeLabel;
                _invoice.TimeTypeID = (int)list.FirstOrDefault().TimeTypeID;
                _invoice.TimeTypeValue = list.FirstOrDefault().TimeTypeValue;
                _invoice.TimeTypeLabel = list.FirstOrDefault().TimeTypeLabel;
                _invoice.Discount = (double)list.FirstOrDefault().Discount;
                
                _invoice.Month = list.FirstOrDefault().Month;
                _invoice.JobNo = list.FirstOrDefault().JobNo;
                _invoice.Rmark = list.FirstOrDefault().Remark;


                Dictionary<int, MyService> dicServiceNature = new Dictionary<int, MyService>();
                var services = list.Select(u => new { u.OrderServiceID, u.ServiceNatureID, u.ServiceNatureLabel, u.ServiceWorkDate, u.ServiceWorkPlace }).Distinct().ToList();

                Dictionary<int, List<MyScheduler>> dicSchedulers = new Dictionary<int, List<MyScheduler>>();

                double grandTotal = 0;

                if (services != null && services.Count > 0)
                {
                    foreach (var item in services)
                    {
                        MyService ms = new MyService();

                        ms.OrderServicId = (int)item.OrderServiceID;
                        ms.ServiceId = (int)item.ServiceNatureID;
                        ms.ServiceName = item.ServiceNatureLabel;
                        ms.ServiceWorkDate = item.ServiceWorkDate;
                        ms.ServiceWorkPlace = item.ServiceWorkPlace;
                        dicServiceNature.Add(ms.OrderServicId, ms);


                        //同一个服务项下面有多条调度
                        var ships = list.Where(u => u.OrderServiceID == item.OrderServiceID)
                            .Select(u => new { u.OrderServiceID, u.SchedulerID, u.ServiceNatureID, u.TugID, u.TugName1, u.TugName2 }).Distinct()
                            .OrderBy(u => u.TugName1).ToList();

                        List<MyScheduler> listScheduler = new List<MyScheduler>();

                        if (ships != null)
                        {
                            foreach (var ship in ships)
                            {

                                MyScheduler sch = new MyScheduler();
                                sch.TugID = (int)ship.TugID;
                                sch.TugCnName = ship.TugName1;
                                sch.ServiceNatureID = (int)ship.ServiceNatureID;

                                //sch.TugEnName = ship.TugName2;
                                //sch.TugSimpleName = ship.TugSimpleName;
                                //sch.TugPower = ship.Power;

                                var schedulers = list.Where(u => u.OrderServiceID == item.OrderServiceID && u.SchedulerID == ship.SchedulerID)
                                    .OrderBy(u => u.OrderID).ThenBy(u => u.OrderServiceID).ThenBy(u => u.SchedulerID)
                                    .Select(u => new
                                    {
                                        u.SchedulerID,
                                        u.TugID,
                                        u.TugName1,
                                        u.TugName2,
                                        u.DepartBaseTime,
                                        u.ArrivalBaseTime,
                                        u.UnitPrice,
                                        u.RopeUsed,
                                        u.RopeNum,
                                        u.SchedulerRemark,
                                        u.BillingItemIDX,
                                        u.ItemID,
                                        u.BillingItemValue,
                                        u.BillingItemLabel,
                                        u.Currency
                                       
                                    }).OrderBy(u => u.ItemID).ToList();

                                if (schedulers != null && schedulers.Count > 0)
                                {
                                    sch.SchedulerID = (int)schedulers[0].SchedulerID;
                                    sch.DepartBaseTime = schedulers[0].DepartBaseTime;
                                    sch.ArrivalBaseTime = schedulers[0].ArrivalBaseTime;

                                    int iDiffHour, iDiffMinute;
                                    BusinessLogic.Utils.CalculateTimeDiff(sch.DepartBaseTime, sch.ArrivalBaseTime, out iDiffHour, out iDiffMinute);
                                    sch.WorkTime = iDiffHour.ToString() + "h." + iDiffMinute.ToString("D2") + "m.";

                                    sch.WorkTimeConsumption = BusinessLogic.Utils.CalculateTimeConsumption(iDiffHour, iDiffMinute, (int)list.FirstOrDefault().TimeTypeID, list.FirstOrDefault().TimeTypeValue, list.FirstOrDefault().TimeTypeLabel);

                                    sch.RopeUsed = schedulers[0].RopeUsed;
                                    sch.RopeNum = (int)schedulers[0].RopeNum;

                                    sch.UnitPriceOfFeulFee = GetFuelFee(ms.ServiceWorkDate);
                                    sch.DiscoutPriceOfFeulFee = GetDiscoutPriceOfFuelFee(_invoice.BillingTemplateID);
                                    sch.PriceOfFeulFee = CalculateFuelFee(ms.ServiceWorkDate, sch.WorkTimeConsumption) + sch.DiscoutPriceOfFeulFee;

                                    double servicePrice = (double)schedulers.FirstOrDefault(u => u.BillingItemValue.StartsWith("A")).UnitPrice;
                                    sch.UnitPrice = servicePrice;
                                    if (((int)list.FirstOrDefault().BillingTypeID == 5 || list.FirstOrDefault().BillingTemplateTypeValue == "0" || list.FirstOrDefault().BillingTemplateTypeLabel == "全包")
                                        || ((int)list.FirstOrDefault().BillingTypeID == 6 || list.FirstOrDefault().BillingTemplateTypeValue == "1" || list.FirstOrDefault().BillingTemplateTypeLabel == "半包"))
                                        sch.Price = servicePrice;
                                    else
                                        sch.Price = servicePrice * sch.WorkTimeConsumption;

                                    double serviceNaturePrice = 0;
                                    double upTotalPrice = 0;
                                    double midTotalPrice = 0;
                                    double discoutPrice = 0;

                                    #region 一条船的费用项目
                                    List<MyBillingItem> billingItems = new List<MyBillingItem>();
                                    foreach (var subItem in schedulers)
                                    {
                                        MyBillingItem bit = new MyBillingItem();
                                        bit.IDX = subItem.BillingItemIDX;
                                        bit.ItemID = subItem.ItemID;
                                        bit.ItemValue = subItem.BillingItemValue;
                                        bit.ItemLabel = subItem.BillingItemLabel;
                                        bit.UnitPrice = subItem.UnitPrice;
                                        bit.Currency = subItem.Currency;

                                        //if (subItem.ItemID == 22 || subItem.BillingItemValue == "E80" || subItem.BillingItemLabel == "燃油附加费")
                                        //{
                                        //    bit.UnitPrice = GetFuelFee(orderDate);
                                        //    bit.Price = CalculateFuelFee(orderDate, sch.WorkTimeConsumption);
                                        //    //bit.Price = subItem.UnitPrice * sch.WorkTimeConsumption;
                                        //}
                                        if (subItem.ItemID == 23 || subItem.BillingItemValue == "C82" || subItem.BillingItemLabel == "拖缆费")
                                            bit.Price = subItem.UnitPrice * sch.RopeNum;
                                        else
                                            bit.Price = subItem.UnitPrice;



                                        if (subItem.BillingItemValue.StartsWith("A"))
                                            serviceNaturePrice += (double)bit.Price;
                                        else if (subItem.BillingItemValue.StartsWith("B"))
                                            upTotalPrice += (double)bit.Price;
                                        else if (subItem.BillingItemValue.StartsWith("C") || subItem.BillingItemValue.StartsWith("E"))
                                        {
                                            if (subItem.BillingItemValue.Equals("C78")) //折扣
                                                discoutPrice += (double)bit.Price;
                                            else if (subItem.BillingItemValue.Equals("C82") || subItem.BillingItemValue.Equals("C15") || subItem.BillingItemValue.Equals("C80")) //拖缆费、3600以上、燃油费
                                                midTotalPrice += (double)bit.Price;

                                        }

                                        //totalPrice += upTotalPrice + midTotalPrice;

                                        billingItems.Add(bit);
                                    }
                                    #endregion

                                    sch.SubTotaHKS = sch.Price + upTotalPrice;
                                    sch.DiscountSubTotalHKS = sch.SubTotaHKS + discoutPrice;

                                    sch.TotalHKs = sch.DiscountSubTotalHKS + midTotalPrice;

                                    sch.BillingItems = billingItems;

                                    grandTotal += sch.TotalHKs;

                                }

                                listScheduler.Add(sch);
                            }
                        }

                        dicSchedulers.Add((int)item.OrderServiceID, listScheduler);

                    }
                }

                _invoice.ServiceNature = dicServiceNature;
                _invoice.Schedulers = dicSchedulers;

                _invoice.GrandTotalHKS = Math.Round(grandTotal, 2);
            }

            return _invoice;
        }


        static public MyInvoice NewInvoice(int orderId, string customerBillingScheme,
            int billingTypeId, string billingTypeValue, string billingTypeLabel,
            int timeTypeId, string timeTypeValue, string timeTypeLabel, double discount)
        {
            DataModel.TugDataEntities db = new DataModel.TugDataEntities();

            double grandTotal = 0.0;

            MyInvoice _invoice = new MyInvoice();

            //_invoice.OrderID = orderId;
            _invoice.OrderIDs = orderId.ToString() ;


            int billingTemplateId = Convert.ToInt32(customerBillingScheme.Split('%')[0].Split('~')[0]);

            List<MyBillingItem> listBillingItemTemplate = GetCustomerBillSchemeItems(billingTemplateId);

            _invoice.BillingTemplateID = billingTemplateId;
            _invoice.BillingTypeID = billingTypeId;
            _invoice.BillingTypeValue = billingTypeValue;
            _invoice.BillingTypeLabel = billingTypeLabel;
            _invoice.TimeTypeID = timeTypeId;
            _invoice.TimeTypeValue = timeTypeValue;
            _invoice.TimeTypeLabel = timeTypeLabel;
            _invoice.Discount = discount;
            

            var list = db.V_OrderScheduler.Where(u => u.OrderID == orderId).OrderBy(u => u.ServiceNatureID).Select(u => u);

            var services = list.Select(u => new { u.ServiceNatureID, u.ServiceNatureLabel, u.ServiceWorkDate, u.ServiceWorkPlace }).Distinct().ToList();

            if (services != null)
            {
                Dictionary<int, MyService> dicService = new Dictionary<int, MyService>();
                Dictionary<int, List<MyScheduler>> dicScheduler = new Dictionary<int, List<MyScheduler>>();
                foreach (var service in services)
                {
                    MyService ms = new MyService();
                    ms.ServiceId = (int)service.ServiceNatureID;
                    ms.ServiceName = service.ServiceNatureLabel;
                    ms.ServiceWorkDate = service.ServiceWorkDate;
                    ms.ServiceWorkPlace = service.ServiceWorkPlace;
                    dicService.Add(ms.ServiceId, ms);

                    var schedulers = list.Where(u => u.ServiceNatureID == (int)service.ServiceNatureID)
                        .Select(u => new
                        {
                            u.IDX,
                            u.TugID,
                            u.TugName1,
                            u.TugName2,
                            u.DepartBaseTime,
                            u.ArrivalBaseTime,
                            u.RopeUsed,
                            u.RopeNum,
                            u.Remark
                        }).ToList();

                    if (schedulers != null)
                    {
                        List<MyScheduler> lstScheduler = new List<MyScheduler>();    

                        foreach (var scheduler in schedulers)
                        {
                            MyScheduler mySch = new MyScheduler();
                            mySch.SchedulerID = scheduler.IDX;
                            mySch.TugID = (int)scheduler.TugID;
                            mySch.TugCnName = scheduler.TugName1;
                            //mySch.TugEnName = scheduler.TugName2;
                            //mySch.TugSimpleName = scheduler.TugSimpleName;
                            //mySch.TugPower = scheduler.Power;

                            mySch.DepartBaseTime = scheduler.DepartBaseTime;
                            mySch.ArrivalBaseTime = scheduler.ArrivalBaseTime;

                            int iDiffHour, iDiffMinute;
                            BusinessLogic.Utils.CalculateTimeDiff(mySch.DepartBaseTime, mySch.ArrivalBaseTime, out iDiffHour, out iDiffMinute);
                            mySch.WorkTime = iDiffHour.ToString() + "h." + iDiffMinute.ToString("D2") + "m.";

                            mySch.WorkTimeConsumption = BusinessLogic.Utils.CalculateTimeConsumption(iDiffHour, iDiffMinute,
                                timeTypeId, timeTypeValue, timeTypeLabel);

                            mySch.RopeUsed = scheduler.RopeUsed;
                            mySch.RopeNum = (int)scheduler.RopeNum;
                            //mySch.Remark = scheduler.Remark;

                            mySch.UnitPriceOfFeulFee = GetFuelFee(ms.ServiceWorkDate);
                            mySch.DiscoutPriceOfFeulFee = GetDiscoutPriceOfFuelFee(_invoice.BillingTemplateID);
                            mySch.PriceOfFeulFee = CalculateFuelFee(ms.ServiceWorkDate, mySch.WorkTimeConsumption) + mySch.DiscoutPriceOfFeulFee;
                            

                            #region 全包
                            if (_invoice.BillingTypeID == 6 || _invoice.BillingTypeValue == "0" || _invoice.BillingTypeLabel == "全包")
                            {
                                MyBillingItem tmp = listBillingItemTemplate.FirstOrDefault(u => u.ItemID == service.ServiceNatureID);
                                if(tmp != null)
                                {
                                    mySch.UnitPrice = mySch.Price = mySch.SubTotaHKS = (double)tmp.UnitPrice;
                                }
                                else
                                {
                                    mySch.UnitPrice = mySch.Price = mySch.SubTotaHKS = 0;
                                }
                                mySch.DiscountSubTotalHKS = Math.Round(mySch.SubTotaHKS, 2);
                                mySch.TotalHKs = mySch.DiscountSubTotalHKS;
                                grandTotal += mySch.TotalHKs;
                            }
                            #endregion

                            #region 半包
                            if (_invoice.BillingTypeID == 7 || _invoice.BillingTypeValue == "1" || _invoice.BillingTypeLabel == "半包")
                            {
                                double discount_price = 0.0, top_total_price = 0.0, mid_total_price = 0.0, bottom_total_price = 0.0;

                                MyBillingItem tmp = listBillingItemTemplate.FirstOrDefault(u => u.ItemID == service.ServiceNatureID);
                                if (tmp != null)
                                {
                                    mySch.UnitPrice = mySch.Price = (double)tmp.UnitPrice;
                                }
                                else
                                {
                                    mySch.UnitPrice = mySch.Price = 0;
                                }

                                top_total_price += mySch.Price;


                                //List<CustomField> banbaoShowItems = GetBanBaoShowItems();
                                List<MyCustomField> banbaoShowItems = GetBanBaoShowItems();
                                List<MyBillingItem> lstMyBillingItems = new List<MyBillingItem>();

                                #region 条目费用计算

                                if (banbaoShowItems != null)
                                {
                                    foreach (MyCustomField item in banbaoShowItems)
                                    {
                                        tmp = listBillingItemTemplate.FirstOrDefault(u => u.ItemID == item.IDX);

                                        MyBillingItem mbi = new MyBillingItem();
                                        if(tmp == null){

                                            if (item.IDX == 16 || item.CustomValue == "B10" || item.CustomLabel == "25%港外附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price * 0.25, 2);
                                            }
                                            else if (item.IDX == 17 || item.CustomValue == "B11" || item.CustomLabel == "50% 18时至22时附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price * 0.5, 2);
                                            }
                                            else if (item.IDX == 18 || item.CustomValue == "B12" || item.CustomLabel == "100% 22时至08时附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price, 2);
                                            }
                                            else if (item.IDX == 19 || item.CustomValue == "B13" || item.CustomLabel == "100%假日附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price, 2);
                                            }
                                            else if (item.IDX == 20 || item.CustomValue == "B14" || item.CustomLabel == "100%台风附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price, 2);
                                            }
                                            else if (item.IDX == 21 || item.CustomValue == "C15" || item.CustomLabel == "使用3600BHP以上的拖轮+15%")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price * 0.15, 2);
                                            }
                                            else if (item.IDX == 22 || item.CustomValue == "E80" || item.CustomLabel == "燃油附加费") {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = mySch.DiscoutPriceOfFeulFee + (mySch.PriceOfFeulFee - mySch.UnitPriceOfFeulFee);
                                            }

                                            else if (item.IDX == 120 || item.CustomValue == "E81" || item.CustomLabel == "燃油附加费额外费用")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = mySch.PriceOfFeulFee - mySch.UnitPriceOfFeulFee;
                                            }
                                            else if (item.IDX == 121 || item.CustomValue == "E82" || item.CustomLabel == "18时至22时附加费50耗时")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = 0;  //存放耗时
                                            }
                                            else if (item.IDX == 122 || item.CustomValue == "E83" || item.CustomLabel == "22时至08时附加费100耗时")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = 0; //存放耗时
                                            }
                                            else
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = -1;
                                                mbi.ItemValue = "";
                                                mbi.ItemLabel = "";
                                                mbi.UnitPrice = 0;
                                                mbi.Price = 0;
                                            }

                                            if (item.CustomValue.StartsWith("B"))
                                            {
                                                mid_total_price += (double)mbi.Price; 
                                            }

                                            if (item.CustomValue.StartsWith("C"))
                                            {
                                                if (item.CustomValue.Equals("C78"))
                                                    discount_price += (double)mbi.Price;
                                                else if (item.CustomValue.Equals("C82"))
                                                    bottom_total_price += (double)mbi.Price;
                                            }
                                        }
                                        else
                                        {
                                            mbi.Currency = tmp.Currency;
                                            mbi.ItemID = tmp.ItemID;
                                            mbi.ItemValue = tmp.ItemValue;
                                            mbi.ItemLabel = tmp.ItemLabel;
                                            mbi.UnitPrice = tmp.UnitPrice;

                                            if (tmp.ItemID == 23 || tmp.ItemValue == "C82" || tmp.ItemLabel == "拖缆费")
                                            {
                                                mbi.Price = tmp.UnitPrice * mySch.RopeNum;
                                            }
                                            else if (tmp.ItemID == 40 || tmp.ItemValue == "C78" || tmp.ItemLabel == "折扣")
                                            {
                                                mbi.Price = tmp.UnitPrice;
                                                mySch.DiscoutPrice = (double)tmp.UnitPrice;
                                            }
                                            else if (tmp.ItemID == 119 || tmp.ItemValue == "E80" || tmp.ItemLabel == "燃油附加费折扣")
                                            {
                                                mbi.Price = tmp.UnitPrice;
                                            }
                                            else
                                            {
                                                mbi.Price = tmp.UnitPrice;
                                            }

                                            if (item.CustomValue.StartsWith("B"))
                                            {
                                                mid_total_price += (double)mbi.Price; 
                                            }
                                            if (item.CustomValue.StartsWith("C")) {
                                                if (item.CustomValue.Equals("C78")) 
                                                    discount_price += (double)mbi.Price;
                                                else if(item.CustomValue.Equals("C82"))
                                                    bottom_total_price += (double)mbi.Price; 
                                            }

                                            mbi.TypeID = tmp.TypeID;
                                            mbi.TypeValue = tmp.TypeValue;
                                            mbi.TypeLabel = tmp.TypeLabel;
                                        }

                                        lstMyBillingItems.Add(mbi);
                                    }
                                }
                                #endregion

                                bottom_total_price += mySch.PriceOfFeulFee;

                                mySch.BillingItems = lstMyBillingItems;

                                mySch.SubTotaHKS = top_total_price; //+mid_total_price;
                                //mySch.DiscountSubTotalHKS = Math.Round(mySch.SubTotaHKS * _invoice.Discount, 2);
                                mySch.DiscountSubTotalHKS = Math.Round(mySch.SubTotaHKS + mySch.DiscoutPrice, 2);

                                mySch.TotalHKs = mySch.DiscountSubTotalHKS + bottom_total_price;
                                grandTotal += mySch.TotalHKs;

                            }
                            #endregion

                            #region 计时
                            if (_invoice.BillingTypeID == 8 || _invoice.BillingTypeValue == "2" || _invoice.BillingTypeLabel == "计时")
                            {
                                double top_total_price = 0.0, mid_total_price = 0.0, bottom_total_price = 0.0, discount_price = 0.0;

                                MyBillingItem tmp = listBillingItemTemplate.FirstOrDefault(u => u.ItemID == service.ServiceNatureID);
                                if (tmp != null)
                                {
                                    mySch.UnitPrice = (double)tmp.UnitPrice;
                                    mySch.Price = mySch.UnitPrice * mySch.WorkTimeConsumption;
                                }
                                else
                                {
                                    mySch.UnitPrice = mySch.Price = 0;
                                }

                                top_total_price += mySch.Price;


                                //List<CustomField> banbaoShowItems = GetTiaoKuanShowItems();
                                List<MyCustomField> banbaoShowItems = GetTiaoKuanShowItems();
                                List<MyBillingItem> lstMyBillingItems = new List<MyBillingItem>();

                                #region 条目费用计算

                                if (banbaoShowItems != null)
                                {
                                    foreach (MyCustomField item in banbaoShowItems)
                                    {
                                        tmp = listBillingItemTemplate.FirstOrDefault(u => u.ItemID == item.IDX);

                                        MyBillingItem mbi = new MyBillingItem();
                                        if (tmp == null)
                                        {

                                            if (item.IDX == 16 || item.CustomValue == "B10" || item.CustomLabel == "25%港外附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price * 0.25, 2);
                                            }
                                            else if (item.IDX == 17 || item.CustomValue == "B11" || item.CustomLabel == "50% 18时至22时附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price * 0.5, 2);
                                            }
                                            else if (item.IDX == 18 || item.CustomValue == "B12" || item.CustomLabel == "100% 22时至08时附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price, 2);
                                            }
                                            else if (item.IDX == 19 || item.CustomValue == "B13" || item.CustomLabel == "100%假日附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price, 2);
                                            }
                                            else if (item.IDX == 20 || item.CustomValue == "B14" || item.CustomLabel == "100%台风附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price, 2);
                                            }
                                            else if (item.IDX == 22 || item.CustomValue == "E80" || item.CustomLabel == "燃油附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = mySch.DiscoutPriceOfFeulFee + (mySch.PriceOfFeulFee - mySch.UnitPriceOfFeulFee);
                                            }

                                            else if (item.IDX == 120 || item.CustomValue == "E81" || item.CustomLabel == "燃油附加费额外费用")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = mySch.PriceOfFeulFee - mySch.UnitPriceOfFeulFee;
                                            }
                                            else if (item.IDX == 121 || item.CustomValue == "E82" || item.CustomLabel == "18时至22时附加费50耗时")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = 0;  //存放耗时
                                            }
                                            else if (item.IDX == 122 || item.CustomValue == "E83" || item.CustomLabel == "22时至08时附加费100耗时")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = 0; //存放耗时
                                            }
                                            else if (item.IDX == 21 || item.CustomValue == "C15" || item.CustomLabel == "使用3600BHP以上的拖轮+15%")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price * 0.15, 2);
                                            }
                                            else
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = -1;
                                                mbi.ItemValue = "";
                                                mbi.ItemLabel = "";
                                                mbi.UnitPrice = 0;
                                                mbi.Price = 0;
                                            }

                                            if (item.CustomValue.StartsWith("B"))
                                            {
                                                mid_total_price += (double)mbi.Price; 
                                            }
                                            if (item.CustomValue.StartsWith("C"))
                                            {
                                                if (item.CustomValue.Equals("C78"))
                                                    discount_price += (double)mbi.Price;
                                                else if(item.CustomValue.Equals("C82"))
                                                    bottom_total_price += (double)mbi.Price;
                                            }
                                        }
                                        else
                                        {
                                            mbi.Currency = tmp.Currency;
                                            mbi.ItemID = tmp.ItemID;
                                            mbi.ItemValue = tmp.ItemValue;
                                            mbi.ItemLabel = tmp.ItemLabel;
                                            mbi.UnitPrice = tmp.UnitPrice;

                                            if (tmp.ItemID == 23 || tmp.ItemValue == "C82" || tmp.ItemLabel == "拖缆费")
                                            {
                                                mbi.Price = tmp.UnitPrice * mySch.RopeNum;
                                            }
                                            else if (tmp.ItemID == 40 || tmp.ItemValue == "C78" || tmp.ItemLabel == "折扣")
                                            {
                                                mbi.Price = tmp.UnitPrice;
                                                mySch.DiscoutPrice = (double)tmp.UnitPrice;
                                            }
                                            else if (tmp.ItemID == 119 || tmp.ItemValue == "E80" || tmp.ItemLabel == "燃油附加费折扣")
                                            {
                                                mbi.Price = tmp.UnitPrice;
                                            }
                                            else
                                            {
                                                mbi.Price = tmp.UnitPrice;
                                            }

                                            if (item.CustomValue.StartsWith("B")) 
                                            {
                                                mid_total_price += (double)mbi.Price;
                                            }
                                            if (item.CustomValue.StartsWith("C"))
                                            {
                                                if (item.CustomValue.Equals("C78"))
                                                    discount_price += (double)mbi.Price;
                                                else if (item.CustomValue.Equals("C82"))
                                                    bottom_total_price += (double)mbi.Price;
                                            }

                                            mbi.TypeID = tmp.TypeID;
                                            mbi.TypeValue = tmp.TypeValue;
                                            mbi.TypeLabel = tmp.TypeLabel;
                                        }

                                        lstMyBillingItems.Add(mbi);
                                    }
                                }
                                #endregion

                                bottom_total_price += mySch.PriceOfFeulFee;

                                mySch.BillingItems = lstMyBillingItems;

                                mySch.SubTotaHKS = top_total_price; //+mid_total_price;
                                //mySch.DiscountSubTotalHKS = Math.Round(mySch.SubTotaHKS * _invoice.Discount, 2);
                                mySch.DiscountSubTotalHKS = Math.Round(mySch.SubTotaHKS + mySch.DiscoutPrice, 2);

                                mySch.TotalHKs = mySch.DiscountSubTotalHKS + bottom_total_price;
                                grandTotal += mySch.TotalHKs;
                            }
                            #endregion

                            lstScheduler.Add(mySch);
                        }
                        dicScheduler.Add((int)service.ServiceNatureID, lstScheduler);

                    }

                }

                _invoice.ServiceNature = dicService;
                _invoice.Schedulers = dicScheduler;
            }

            _invoice.GrandTotalHKS = grandTotal;
            

            return _invoice;
        }

        static public MyInvoice NewInvoice2(string orderIds, string customerBillingScheme,
            int billingTypeId, string billingTypeValue, string billingTypeLabel,
            int timeTypeId, string timeTypeValue, string timeTypeLabel, double discount)
        {
            DataModel.TugDataEntities db = new DataModel.TugDataEntities();

            double grandTotal = 0.0;

            MyInvoice _invoice = new MyInvoice();

            //_invoice.OrderID = orderId;
            _invoice.OrderIDs = orderIds;
            List<string> strOrderIDs = orderIds.Split(',').ToList();
            List<int> iOrderIDs = new List<int>();
            if (strOrderIDs != null)
            {
                foreach (var item in strOrderIDs)
                {
                    iOrderIDs.Add(BusinessLogic.Module.Util.toint(item));
                }
            }

            //_invoice.OrderCode = ;
            //_invoice

            int billingTemplateId = Convert.ToInt32(customerBillingScheme.Split('%')[0].Split('~')[0]);

            List<MyBillingItem> listBillingItemTemplate = GetCustomerBillSchemeItems(billingTemplateId);

            _invoice.BillingTemplateID = billingTemplateId;
            _invoice.BillingTypeID = billingTypeId;
            _invoice.BillingTypeValue = billingTypeValue;
            _invoice.BillingTypeLabel = billingTypeLabel;
            _invoice.TimeTypeID = timeTypeId;
            _invoice.TimeTypeValue = timeTypeValue;
            _invoice.TimeTypeLabel = timeTypeLabel;
            _invoice.Discount = discount;


            //var list = db.V_OrderScheduler.Where(u => u.OrderID == orderId).OrderBy(u => u.OrderID).Select(u => u);
            var list = db.V_OrderScheduler.Where(u => (iOrderIDs.Contains((int)u.OrderID) && u.HasBilling == "否" && u.HasBillingInFlow == "否")
                || (iOrderIDs.Contains((int)u.OrderID) && u.HasBilling == "是" && u.HasBillingInFlow == "否" && (u.BillingType == 0 || u.BillingType == null))).OrderBy(u => u.OrderID).Select(u => u);

            var services = list.Select(u => new {u.OrderServiceID, u.ServiceNatureID, u.ServiceNatureLabel, u.ServiceWorkDate, u.ServiceWorkPlace }).Distinct().ToList();

            if (services != null)
            {
                Dictionary<int, MyService> dicService = new Dictionary<int, MyService>();
                Dictionary<int, List<MyScheduler>> dicScheduler = new Dictionary<int, List<MyScheduler>>();
                foreach (var service in services)
                {
                    MyService ms = new MyService();
                    ms.OrderServicId = (int)service.OrderServiceID;
                    ms.ServiceId = (int)service.ServiceNatureID;
                    ms.ServiceName = service.ServiceNatureLabel;
                    ms.ServiceWorkDate = service.ServiceWorkDate;
                    ms.ServiceWorkPlace = service.ServiceWorkPlace;
                    dicService.Add(ms.OrderServicId, ms);

                    var schedulers = list.Where(u => u.OrderServiceID == (int)service.OrderServiceID)
                        .Select(u => new
                        {
                            u.IDX,
                            u.ServiceNatureID,
                            u.TugID,
                            u.TugName1,
                            u.TugName2,
                            u.DepartBaseTime,
                            u.ArrivalBaseTime,
                            u.RopeUsed,
                            u.RopeNum,
                            u.Remark
                        }).ToList();

                    if (schedulers != null)
                    {
                        List<MyScheduler> lstScheduler = new List<MyScheduler>();

                        foreach (var scheduler in schedulers)
                        {
                            MyScheduler mySch = new MyScheduler();
                            mySch.SchedulerID = scheduler.IDX;
                            mySch.ServiceNatureID = (int)scheduler.ServiceNatureID;
                            mySch.TugID = (int)scheduler.TugID;
                            mySch.TugCnName = scheduler.TugName1;
                            //mySch.TugEnName = scheduler.TugName2;
                            //mySch.TugSimpleName = scheduler.TugSimpleName;
                            //mySch.TugPower = scheduler.Power;

                            mySch.DepartBaseTime = scheduler.DepartBaseTime;
                            mySch.ArrivalBaseTime = scheduler.ArrivalBaseTime;

                            int iDiffHour, iDiffMinute;
                            BusinessLogic.Utils.CalculateTimeDiff(mySch.DepartBaseTime, mySch.ArrivalBaseTime, out iDiffHour, out iDiffMinute);
                            mySch.WorkTime = iDiffHour.ToString() + "h." + iDiffMinute.ToString("D2") + "m.";

                            mySch.WorkTimeConsumption = BusinessLogic.Utils.CalculateTimeConsumption(iDiffHour, iDiffMinute,
                                timeTypeId, timeTypeValue, timeTypeLabel);

                            mySch.RopeUsed = scheduler.RopeUsed;
                            mySch.RopeNum = (int)scheduler.RopeNum;
                            //mySch.Remark = scheduler.Remark;

                            mySch.UnitPriceOfFeulFee = GetFuelFee(ms.ServiceWorkDate);
                            mySch.DiscoutPriceOfFeulFee = GetDiscoutPriceOfFuelFee(_invoice.BillingTemplateID);
                            mySch.PriceOfFeulFee = CalculateFuelFee(ms.ServiceWorkDate, mySch.WorkTimeConsumption); //打折前+mySch.DiscoutPriceOfFeulFee;


                            #region 全包
                            if (_invoice.BillingTypeID == 6 || _invoice.BillingTypeValue == "0" || _invoice.BillingTypeLabel == "全包")
                            {
                                MyBillingItem tmp = listBillingItemTemplate.FirstOrDefault(u => u.ItemID == service.ServiceNatureID);
                                if (tmp != null)
                                {
                                    mySch.UnitPrice = mySch.Price = mySch.SubTotaHKS = (double)tmp.UnitPrice;
                                }
                                else
                                {
                                    mySch.UnitPrice = mySch.Price = mySch.SubTotaHKS = 0;
                                }
                                mySch.DiscountSubTotalHKS = Math.Round(mySch.SubTotaHKS, 2);
                                mySch.TotalHKs = mySch.DiscountSubTotalHKS;
                                grandTotal += mySch.TotalHKs;
                            }
                            #endregion

                            #region 半包
                            if (_invoice.BillingTypeID == 7 || _invoice.BillingTypeValue == "1" || _invoice.BillingTypeLabel == "半包")
                            {
                                double discount_price = 0.0, top_total_price = 0.0, mid_total_price = 0.0, bottom_total_price = 0.0;

                                MyBillingItem tmp = listBillingItemTemplate.FirstOrDefault(u => u.ItemID == service.ServiceNatureID);
                                if (tmp != null)
                                {
                                    mySch.UnitPrice = mySch.Price = (double)tmp.UnitPrice;
                                }
                                else
                                {
                                    mySch.UnitPrice = mySch.Price = 0;
                                }

                                top_total_price += mySch.Price;


                                //List<CustomField> banbaoShowItems = GetBanBaoShowItems();
                                List<MyCustomField> banbaoShowItems = GetBanBaoShowItems();
                                List<MyBillingItem> lstMyBillingItems = new List<MyBillingItem>();

                                #region 条目费用计算

                                if (banbaoShowItems != null)
                                {
                                    foreach (MyCustomField item in banbaoShowItems)
                                    {
                                        tmp = listBillingItemTemplate.FirstOrDefault(u => u.ItemID == item.IDX);

                                        MyBillingItem mbi = new MyBillingItem();
                                        if (tmp == null)
                                        {

                                            if (item.IDX == 16 || item.CustomValue == "B10" || item.CustomLabel == "25%港外附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price * 0.25, 2);
                                            }
                                            else if (item.IDX == 17 || item.CustomValue == "B11" || item.CustomLabel == "50% 18时至22时附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price * 0.5, 2);
                                            }
                                            else if (item.IDX == 18 || item.CustomValue == "B12" || item.CustomLabel == "100% 22时至08时附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price, 2);
                                            }
                                            else if (item.IDX == 19 || item.CustomValue == "B13" || item.CustomLabel == "100%假日附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price, 2);
                                            }
                                            else if (item.IDX == 20 || item.CustomValue == "B14" || item.CustomLabel == "100%台风附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price, 2);
                                            }
                                            else if (item.IDX == 21 || item.CustomValue == "C15" || item.CustomLabel == "使用3600BHP以上的拖轮+15%")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price * 0.15, 2);
                                            }
                                            else if (item.IDX == 22 || item.CustomValue == "C80" || item.CustomLabel == "燃油附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = mySch.DiscoutPriceOfFeulFee + (mySch.PriceOfFeulFee - mySch.UnitPriceOfFeulFee);
                                            }

                                            else if (item.IDX == 120 || item.CustomValue == "E81" || item.CustomLabel == "燃油附加费额外费用")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = mySch.PriceOfFeulFee - mySch.UnitPriceOfFeulFee;
                                            }
                                            else if (item.IDX == 121 || item.CustomValue == "E82" || item.CustomLabel == "18时至22时附加费50耗时")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = 0;  //存放耗时
                                            }
                                            else if (item.IDX == 122 || item.CustomValue == "E83" || item.CustomLabel == "22时至08时附加费100耗时")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = 0; //存放耗时
                                            }
                                            else
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = -1;
                                                mbi.ItemValue = "";
                                                mbi.ItemLabel = "";
                                                mbi.UnitPrice = 0;
                                                mbi.Price = 0;
                                            }

                                            if (item.CustomValue.StartsWith("B"))
                                            {
                                                mid_total_price += (double)mbi.Price;
                                            }

                                            if (item.CustomValue.StartsWith("C"))
                                            {
                                                if (item.CustomValue.Equals("C78")) //折扣
                                                    discount_price += (double)mbi.Price;
                                                else if (item.CustomValue.Equals("C82")) //拖缆费
                                                    bottom_total_price += (double)mbi.Price;
                                                //else if (item.CustomValue.Equals("E81")) //燃油附加费额外费用
                                                //    bottom_total_price += (double)mbi.Price;
                                            }
                                        }
                                        else
                                        {
                                            mbi.Currency = tmp.Currency;
                                            mbi.ItemID = tmp.ItemID;
                                            mbi.ItemValue = tmp.ItemValue;
                                            mbi.ItemLabel = tmp.ItemLabel;
                                            mbi.UnitPrice = tmp.UnitPrice;

                                            if (tmp.ItemID == 23 || tmp.ItemValue == "C82" || tmp.ItemLabel == "拖缆费")
                                            {
                                                mbi.Price = tmp.UnitPrice * mySch.RopeNum;
                                            }
                                            else if (tmp.ItemID == 40 || tmp.ItemValue == "C78" || tmp.ItemLabel == "折扣")
                                            {
                                                mbi.Price = tmp.UnitPrice;
                                                mySch.DiscoutPrice = (double)tmp.UnitPrice;
                                            }
                                            else if (tmp.ItemID == 119 || tmp.ItemValue == "E80" || tmp.ItemLabel == "燃油附加费折扣")
                                            {
                                                mbi.Price = tmp.UnitPrice;
                                            }
                                            else
                                            {
                                                mbi.Price = tmp.UnitPrice;
                                            }

                                            if (item.CustomValue.StartsWith("B"))
                                            {
                                                mid_total_price += (double)mbi.Price;
                                            }
                                            if (item.CustomValue.StartsWith("C"))
                                            {
                                                if (item.CustomValue.Equals("C78")) //折扣
                                                    discount_price += (double)mbi.Price;
                                                else if (item.CustomValue.Equals("C82"))    //拖缆费
                                                    bottom_total_price += (double)mbi.Price;
                                                //else if (item.CustomValue.Equals("E81")) //燃油附加费额外费用
                                                //    bottom_total_price += (double)mbi.Price;
                                            }

                                            mbi.TypeID = tmp.TypeID;
                                            mbi.TypeValue = tmp.TypeValue;
                                            mbi.TypeLabel = tmp.TypeLabel;
                                        }

                                        lstMyBillingItems.Add(mbi);
                                    }
                                }
                                #endregion

                                //bottom_total_price += mySch.PriceOfFeulFee;
                                bottom_total_price += (mySch.UnitPriceOfFeulFee + mySch.DiscoutPriceOfFeulFee) + (mySch.PriceOfFeulFee - mySch.UnitPriceOfFeulFee); 

                                mySch.BillingItems = lstMyBillingItems;

                                mySch.SubTotaHKS = top_total_price; //+mid_total_price;
                                //mySch.DiscountSubTotalHKS = Math.Round(mySch.SubTotaHKS * _invoice.Discount, 2);
                                mySch.DiscountSubTotalHKS = Math.Round(mySch.SubTotaHKS + mySch.DiscoutPrice, 2);

                                mySch.TotalHKs = mySch.DiscountSubTotalHKS + bottom_total_price;
                                grandTotal += mySch.TotalHKs;

                            }
                            #endregion

                            #region 计时
                            if (_invoice.BillingTypeID == 8 || _invoice.BillingTypeValue == "2" || _invoice.BillingTypeLabel == "计时")
                            {
                                double top_total_price = 0.0, mid_total_price = 0.0, bottom_total_price = 0.0, discount_price = 0.0;

                                MyBillingItem tmp = listBillingItemTemplate.FirstOrDefault(u => u.ItemID == service.ServiceNatureID);
                                if (tmp != null)
                                {
                                    mySch.UnitPrice = (double)tmp.UnitPrice;
                                    mySch.Price = mySch.UnitPrice * mySch.WorkTimeConsumption;
                                }
                                else
                                {
                                    mySch.UnitPrice = mySch.Price = 0;
                                }

                                top_total_price += mySch.Price;


                                //List<CustomField> banbaoShowItems = GetTiaoKuanShowItems();
                                List<MyCustomField> banbaoShowItems = GetTiaoKuanShowItems();
                                List<MyBillingItem> lstMyBillingItems = new List<MyBillingItem>();

                                #region 条目费用计算

                                if (banbaoShowItems != null)
                                {
                                    foreach (MyCustomField item in banbaoShowItems)
                                    {
                                        tmp = listBillingItemTemplate.FirstOrDefault(u => u.ItemID == item.IDX);

                                        MyBillingItem mbi = new MyBillingItem();
                                        if (tmp == null)
                                        {

                                            if (item.IDX == 16 || item.CustomValue == "B10" || item.CustomLabel == "25%港外附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price * 0.25, 2);
                                            }
                                            else if (item.IDX == 17 || item.CustomValue == "B11" || item.CustomLabel == "50% 18时至22时附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price * 0.5, 2);
                                            }
                                            else if (item.IDX == 18 || item.CustomValue == "B12" || item.CustomLabel == "100% 22时至08时附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price, 2);
                                            }
                                            else if (item.IDX == 19 || item.CustomValue == "B13" || item.CustomLabel == "100%假日附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price, 2);
                                            }
                                            else if (item.IDX == 20 || item.CustomValue == "B14" || item.CustomLabel == "100%台风附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price, 2);
                                            }
                                            else if (item.IDX == 21 || item.CustomValue == "C15" || item.CustomLabel == "使用3600BHP以上的拖轮+15%")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = Math.Round(mySch.Price * 0.15, 2);
                                            }
                                            else if (item.IDX == 22 || item.CustomValue == "C80" || item.CustomLabel == "燃油附加费")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = mySch.DiscoutPriceOfFeulFee + (mySch.PriceOfFeulFee - mySch.UnitPriceOfFeulFee);
                                            }

                                            else if (item.IDX == 120 || item.CustomValue == "E81" || item.CustomLabel == "燃油附加费额外费用")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = mySch.PriceOfFeulFee - mySch.UnitPriceOfFeulFee;
                                            }
                                            else if (item.IDX == 121 || item.CustomValue == "E82" || item.CustomLabel == "18时至22时附加费50耗时")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = 0;  //存放耗时
                                            }
                                            else if (item.IDX == 122 || item.CustomValue == "E83" || item.CustomLabel == "22时至08时附加费100耗时")
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = item.IDX;
                                                mbi.ItemValue = item.CustomValue;
                                                mbi.ItemLabel = item.CustomLabel;
                                                mbi.UnitPrice = mbi.Price = 0; //存放耗时
                                            }
                                            else
                                            {
                                                mbi.Currency = "港币";
                                                mbi.ItemID = -1;
                                                mbi.ItemValue = "";
                                                mbi.ItemLabel = "";
                                                mbi.UnitPrice = 0;
                                                mbi.Price = 0;
                                            }

                                            if (item.CustomValue.StartsWith("B"))
                                            {
                                                mid_total_price += (double)mbi.Price;
                                            }
                                            if (item.CustomValue.StartsWith("C"))
                                            {
                                                if (item.CustomValue.Equals("C78"))     //折扣
                                                    discount_price += (double)mbi.Price;
                                                else if (item.CustomValue.Equals("C82"))    //拖缆费
                                                    bottom_total_price += (double)mbi.Price;
                                            }
                                        }
                                        else
                                        {
                                            mbi.Currency = tmp.Currency;
                                            mbi.ItemID = tmp.ItemID;
                                            mbi.ItemValue = tmp.ItemValue;
                                            mbi.ItemLabel = tmp.ItemLabel;
                                            mbi.UnitPrice = tmp.UnitPrice;

                                            if (tmp.ItemID == 23 || tmp.ItemValue == "C82" || tmp.ItemLabel == "拖缆费")
                                            {
                                                mbi.Price = tmp.UnitPrice * mySch.RopeNum;
                                            }
                                            else if (tmp.ItemID == 40 || tmp.ItemValue == "C78" || tmp.ItemLabel == "折扣")
                                            {
                                                mbi.Price = tmp.UnitPrice;
                                                mySch.DiscoutPrice = (double)tmp.UnitPrice;
                                            }
                                            else if (tmp.ItemID == 119 || tmp.ItemValue == "E80" || tmp.ItemLabel == "燃油附加费折扣")
                                            {
                                                mbi.Price = tmp.UnitPrice;
                                            }
                                            else
                                            {
                                                mbi.Price = tmp.UnitPrice;
                                            }

                                            if (item.CustomValue.StartsWith("B"))
                                            {
                                                mid_total_price += (double)mbi.Price;
                                            }
                                            if (item.CustomValue.StartsWith("C"))
                                            {
                                                if (item.CustomValue.Equals("C78"))     //折扣
                                                    discount_price += (double)mbi.Price;
                                                else if (item.CustomValue.Equals("C82"))    //拖缆费
                                                    bottom_total_price += (double)mbi.Price;
                                            }

                                            mbi.TypeID = tmp.TypeID;
                                            mbi.TypeValue = tmp.TypeValue;
                                            mbi.TypeLabel = tmp.TypeLabel;
                                        }

                                        lstMyBillingItems.Add(mbi);
                                    }
                                }
                                #endregion

                                //bottom_total_price += mySch.PriceOfFeulFee;
                                bottom_total_price += (mySch.UnitPriceOfFeulFee + mySch.DiscoutPriceOfFeulFee) + (mySch.PriceOfFeulFee - mySch.UnitPriceOfFeulFee); 

                                mySch.BillingItems = lstMyBillingItems;

                                mySch.SubTotaHKS = top_total_price; //+mid_total_price;
                                //mySch.DiscountSubTotalHKS = Math.Round(mySch.SubTotaHKS * _invoice.Discount, 2);
                                mySch.DiscountSubTotalHKS = Math.Round(mySch.SubTotaHKS + mySch.DiscoutPrice, 2);

                                mySch.TotalHKs = mySch.DiscountSubTotalHKS + bottom_total_price;
                                grandTotal += mySch.TotalHKs;
                            }
                            #endregion

                            lstScheduler.Add(mySch);
                        }
                        dicScheduler.Add((int)service.OrderServiceID, lstScheduler);

                    }

                }

                _invoice.ServiceNature = dicService;
                _invoice.Schedulers = dicScheduler;
            }

            _invoice.GrandTotalHKS = grandTotal;


            return _invoice;
        }


        static public MySpecialInvoice GenerateSpecialInvoice(int billingId)
        {
            DataModel.TugDataEntities db = new DataModel.TugDataEntities();

            MySpecialInvoice _invoice = new MySpecialInvoice();

            List<MySpecialBillingItem> _billingItems = new List<MySpecialBillingItem>();
            V_Billing3 vb3 = db.V_Billing3.FirstOrDefault(u => u.IDX == billingId);
            if (vb3 != null)
            {
                _invoice.BillingID = vb3.IDX;
                _invoice.CustomerID = (int)vb3.CustomerID;
                _invoice.CustomerName = vb3.CustomerName;
                _invoice.Amount = (double)vb3.Amount;
                _invoice.Status = vb3.Status;
                _invoice.Phase = (int)vb3.Phase;
                _invoice.InvoiceType = vb3.InvoiceType;
                _invoice.Month = vb3.Month;
                _invoice.BillingCode = vb3.BillingCode;
                _invoice.BillingRemark = vb3.Remark;

                _billingItems = db.V_SpecialBillingItem.Where(u => u.SpecialBillingID == billingId).Select(u => new MySpecialBillingItem
                {
                    SpecialBillingID = billingId,
                    OrderServiceID = (int)u.OrderServiceID,
                    ServiceNatureID = (int)u.ServiceNatureID,
                    ServiceNatureValue = u.ServiceNatureValue,
                    ServiceDate = u.ServiceDate,
                    ServiceNature = u.ServiceNature,
                    CustomerShipName = u.CustomerShipName,
                    TugNumber = (int)u.TugNumber,
                    ServiceUnitPrice = (double)u.ServiceUnitPrice,
                    FeulUnitPrice = (double)u.FeulUnitPrice,
                }).ToList();

                _invoice.SpecialBillingItems = _billingItems;
            }

            return _invoice;
        }


        static public List<V_BillingTemplate> GetCustomerBillSchemes(int custId)
        {
            DataModel.TugDataEntities db = new DataModel.TugDataEntities();

            List<V_BillingTemplate> list = new List<V_BillingTemplate>();
            if(custId == -1)
                list = db.V_BillingTemplate.Where(u => u.CustomerCode == "-1").OrderBy(u => u.BillingTemplateName).ToList();
            else
                list = db.V_BillingTemplate.Where(u =>u.CustomerCode == "-1" || u.CustomerID == custId).OrderBy(u => u.BillingTemplateName).ToList();

            return list;
        }


        /// <summary>
        /// 根据计费方案的id获取计费方案对象
        /// </summary>
        /// <param name="billSchemeId"></param>
        /// <returns></returns>
        static public V_BillingTemplate GetCustomerBillScheme(int billSchemeId)
        {
            DataModel.TugDataEntities db = new DataModel.TugDataEntities();

            V_BillingTemplate bt = db.V_BillingTemplate.FirstOrDefault(u => u.BillingTemplateIDX == billSchemeId);

            return bt;
        }

        /// <summary>
        /// 获取计费方案的收费条目
        /// </summary>
        /// <param name="billSchemeId">计费方案ID</param>
        /// <returns></returns>
        static public List<MyBillingItem> GetCustomerBillSchemeItems(int billSchemeId)
        {
            DataModel.TugDataEntities db = new DataModel.TugDataEntities();

            List<MyBillingItem> list = db.V_BillingItemTemplate.Where(u => u.BillingTemplateID == billSchemeId).OrderBy(u => u.TypeValue)
                .OrderBy(u => u.ItemValue).Select(u => new MyBillingItem { 
                    IDX = u.IDX, ItemID = u.ItemID, ItemValue = u.ItemValue, ItemLabel = u.ItemLabel,
                    UnitPrice = u.UnitPrice, Currency = u.Currency, TypeID = u.TypeID, TypeValue = u.TypeValue, TypeLabel=u.TypeLabel}).ToList();

            return list;
        }

        /// <summary>
        /// 获取半包类型账单的条目显示项
        /// </summary>
        /// <returns></returns>
        static public List<MyCustomField> GetBanBaoShowItems()
        {
            DataModel.TugDataEntities db = new DataModel.TugDataEntities();

            List<MyCustomField> list = db.CustomField.Where(u => u.CustomName == "BillingItemTemplate.ItemID" && u.FormulaStr.Substring(1, 1) == "1")
                .OrderBy(u => u.CustomValue).Select(u => new MyCustomField { IDX = u.IDX, CustomValue = u.CustomValue,CustomLabel = u.CustomLabel,FormulaStr = u.FormulaStr }).ToList<MyCustomField>();

            return list;
        }

        /// <summary>
        /// 获取条款类型账单的条目显示项
        /// </summary>
        /// <returns></returns>
        //static public List<CustomField> GetTiaoKuanShowItems()
        //{
        //    DataModel.TugDataEntities db = new DataModel.TugDataEntities();

        //    List<CustomField> list = db.CustomField.Where(u => u.CustomName == "BillingItemTemplate.ItemID" && u.FormulaStr.Substring(2, 1) == "1").OrderBy(u => u.CustomValue).ToList();

        //    return list;
        //}

        static public List<MyCustomField> GetTiaoKuanShowItems()
        {
            DataModel.TugDataEntities db = new DataModel.TugDataEntities();

            List<MyCustomField> list = db.CustomField.Where(u => u.CustomName == "BillingItemTemplate.ItemID" && u.FormulaStr.Substring(2, 1) == "1")
                .OrderBy(u => u.CustomValue).Select(u => new MyCustomField {IDX= u.IDX,CustomValue= u.CustomValue,CustomLabel= u.CustomLabel,FormulaStr= u.FormulaStr }).ToList();

            return list;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderMethod">排序方式asc升序；desc降序</param>
        /// <returns></returns>
        //static public List<DataModel.V_OrderBilling> LoadDataForInvoice(string orderField, string orderMethod)
        //{
        //    List<V_OrderBilling> orders = null;

        //    try
        //    {
        //        TugDataEntities db = new TugDataEntities();
        //        orders = db.V_OrderBilling
        //            .Where(u => u.WorkStateID == 5
        //            || u.WorkStateValue == "3"
        //            || u.WorkStateLabel == "已完工")
        //            .Select(u => u).ToList<V_OrderBilling>();

        //        #region 根据排序字段和排序方式排序
        //        switch (orderField)
        //        {
        //            case "":
        //                {
        //                    //if(orderMethod.ToLower().Equals("asc"))
        //                    //    orders = orders.OrderBy(u => u.IDX).ToList();
        //                    //else
        //                    orders = orders.OrderByDescending(u => u.OrderID).ToList();
        //                }
        //                break;
        //            case "CustomerName":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.CustomerName).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.CustomerName).ToList();
        //                }
        //                break;
        //            case "OrderCode":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.OrderCode).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.OrderCode).ToList();
        //                }
        //                break;

        //            case "OrdDate":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.OrdDate).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.OrdDate).ToList();
        //                }
        //                break;
        //            //case "WorkTime":
        //            //    {
        //            //        if (orderMethod.ToLower().Equals("asc"))
        //            //            orders = orders.OrderBy(u => u.WorkTime).ToList();
        //            //        else
        //            //            orders = orders.OrderByDescending(u => u.WorkTime).ToList();
        //            //    }
        //            //    break;
        //            //case "EstimatedCompletionTime":
        //            //    {
        //            //        if (orderMethod.ToLower().Equals("asc"))
        //            //            orders = orders.OrderBy(u => u.EstimatedCompletionTime).ToList();
        //            //        else
        //            //            orders = orders.OrderByDescending(u => u.EstimatedCompletionTime).ToList();
        //            //    }
        //            //    break;
        //            case "ShipName":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.ShipName).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.ShipName).ToList();
        //                }
        //                break;


        //            //case "ServiceNatureNames":
        //            //    {
        //            //        if (orderMethod.ToLower().Equals("asc"))
        //            //            orders = orders.OrderBy(u => u.ServiceNatureNames).ToList();
        //            //        else
        //            //            orders = orders.OrderByDescending(u => u.ServiceNatureNames).ToList();
        //            //    }
        //            //    break;
        //            case "WorkStateLabel":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.WorkStateLabel).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.WorkStateLabel).ToList();
        //                }
        //                break;
        //            case "JobNo":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.JobNo).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.JobNo).ToList();
        //                }
        //                break;
        //            case "BillingCode":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.BillingCode).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.BillingCode).ToList();
        //                }
        //                break;

        //            case "BillingName":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.BillingName).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.BillingName).ToList();
        //                }
        //                break;

        //            case "BillingTypeLabel":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.BillingTypeLabel).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.BillingTypeLabel).ToList();
        //                }
        //                break;
        //            case "TimeTypeLabel":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.TimeTypeLabel).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.TimeTypeLabel).ToList();
        //                }
        //                break;
        //            case "Amount":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.Amount).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.Amount).ToList();
        //                }
        //                break;
        //            case "BillingRemark":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.BillingRemark).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.BillingRemark).ToList();
        //                }
        //                break;
        //            case "Month":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.Month).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.Month).ToList();
        //                }
        //                break;
        //            case "TimesNo":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.TimesNo).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.TimesNo).ToList();
        //                }
        //                break;
        //            case "Status":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.Status).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.Status).ToList();
        //                }
        //                break;
        //            case "Phase":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.Phase).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.Phase).ToList();
        //                }
        //                break;
        //            case "BillingCreateDate":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.BillingCreateDate).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.BillingCreateDate).ToList();
        //                }
        //                break;
        //            case "BillingLastUpDate":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.BillingLastUpDate).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.BillingLastUpDate).ToList();
        //                }
        //                break;


        //            default:
        //                break;
        //        }

        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //    return orders;
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchOptions">搜索选项，格式如下</param>
        /// <returns></returns>
        //static public List<DataModel.V_OrderBilling> SearchForInvoice(string orderField, string orderMethod, string searchOptions)
        //{
        //    List<V_OrderBilling> orders = null;
        //    try
        //    {
        //        //searchOptions的Json字符串格式
        //        //{
        //        //    "groupOp":"AND",
        //        //    "rules":[{"field":"IsGuest","op":"eq","data":"全部"}],
        //        //    "groups":[
        //        //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"BigTugNum"},{"data":"2","op":"le","field":"BigTugNum"}]},
        //        //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"MiddleTugNum"},{"data":"2","op":"le","field":"MiddleTugNum"}]},
        //        //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"SmallTugNum"},{"data":"2","op":"le","field":"SmallTugNum"}]}
        //        //    ]

        //        //}



        //        TugDataEntities db = new TugDataEntities();
        //        //orders = db.V_OrderInfor.Select(u => u).ToList<V_OrderInfor>();

        //        JObject jsonSearchOption = (JObject)JsonConvert.DeserializeObject(searchOptions);
        //        string groupOp = (string)jsonSearchOption["groupOp"];
        //        JArray rules = (JArray)jsonSearchOption["rules"];

        //        Expression condition = Expression.Equal(Expression.Constant(1, typeof(int)), Expression.Constant(1, typeof(int)));
        //        ParameterExpression parameter = Expression.Parameter(typeof(V_OrderBilling));

        //        if (rules != null)
        //        {
        //            foreach (JObject item in rules)
        //            {
        //                string field = (string)item["field"];
        //                string op = (string)item["op"];
        //                string data = (string)item["data"];

        //                #region 根据各字段条件进行条件表达式拼接
        //                switch (field)
        //                {
        //                    #region IsGuest
        //                    case "IsGuest":
        //                        {
        //                            Expression cdt = null;

        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        if (data != "全部")
        //                                        {
        //                                            //orders = orders.Where(u => u.IsGuest == data).ToList();
        //                                            cdt = Expression.Equal(Expression.PropertyOrField(parameter, "IsGuest"), Expression.Constant(data));
        //                                        }
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }

        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region CustomerName
        //                    case "CustomerName":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.CustomerName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
        //                                        cdt = Expression.Equal(Expression.PropertyOrField(parameter, "CustomerName"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_BW:
        //                                    {
        //                                        //orders = orders.Where(u => u.CustomerName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_EW:
        //                                    {
        //                                        //orders = orders.Where(u => u.CustomerName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_CN:
        //                                    {
        //                                        //orders = orders.Where(u => u.CustomerName.ToLower().Contains(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region OrderCode
        //                    case "OrderCode":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.Code.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
        //                                        cdt = Expression.Equal(Expression.PropertyOrField(parameter, "OrderCode"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_BW:
        //                                    {
        //                                        //orders = orders.Where(u => u.Code.ToLower().StartsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "OrderCode"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_EW:
        //                                    {
        //                                        //orders = orders.Where(u => u.Code.ToLower().EndsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "OrderCode"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_CN:
        //                                    {
        //                                        //orders = orders.Where(u => u.Code.ToLower().Contains(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "OrderCode"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }

        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region OrdDate
        //                    case "OrdDate":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkDate == data.Trim()).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_LT:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkDate.CompareTo(data.Trim()) == -1).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_LE:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkDate.CompareTo(data.Trim()) == -1 || u.WorkDate.CompareTo(data.Trim()) == 0).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_GT:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkDate.CompareTo(data.Trim()) == 1).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_GE:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkDate.CompareTo(data.Trim()) == 1 || u.WorkDate.CompareTo(data.Trim()) == 0).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region WorkTime
        //                    case "WorkTime":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkTime == data.Trim()).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_LT:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == -1).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_LE:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == -1 || u.WorkTime.CompareTo(data.Trim()) == 0).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_GT:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == 1).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_GE:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == 1 || u.WorkTime.CompareTo(data.Trim()) == 0).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region EstimatedCompletionTime
        //                    case "EstimatedCompletionTime":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.EstimatedCompletionTime == data.Trim()).ToList();
        //                                        cdt = Expression.Equal(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), Expression.Constant(data.Trim()));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_LT:
        //                                    {
        //                                        //orders = orders.Where(u => u.EstimatedCompletionTime.CompareTo(data.Trim()) == -1).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_LE:
        //                                    {
        //                                        //orders = orders.Where(u => u.EstimatedCompletionTime.CompareTo(data.Trim()) == -1 || u.EstimatedCompletionTime.CompareTo(data.Trim()) == 0).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_GT:
        //                                    {
        //                                        //orders = orders.Where(u => u.EstimatedCompletionTime.CompareTo(data.Trim()) == 1).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_GE:
        //                                    {
        //                                        //orders = orders.Where(u => u.EstimatedCompletionTime.CompareTo(data.Trim()) == 1 || u.EstimatedCompletionTime.CompareTo(data.Trim()) == 0).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region ShipName
        //                    case "ShipName":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
        //                                        cdt = Expression.Equal(Expression.PropertyOrField(parameter, "ShipName"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_BW:
        //                                    {
        //                                        //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_EW:
        //                                    {
        //                                        //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_CN:
        //                                    {
        //                                        //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region ServiceNatureNames
        //                    case "ServiceNatureNames":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.ServiceNatureNames.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
        //                                        cdt = Expression.Equal(Expression.PropertyOrField(parameter, "ServiceNatureNames"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_BW:
        //                                    {
        //                                        //orders = orders.Where(u => u.ServiceNatureNames.ToLower().StartsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceNatureNames"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_EW:
        //                                    {
        //                                        //orders = orders.Where(u => u.ServiceNatureNames.ToLower().EndsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceNatureNames"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_CN:
        //                                    {
        //                                        //orders = orders.Where(u => u.ServiceNatureNames.ToLower().Contains(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceNatureNames"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region WorkStateLabel
        //                    case "WorkStateLabel":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        int workStateId = Convert.ToInt32(data.Split('~')[0]);
        //                                        if (workStateId != -1)
        //                                        {
        //                                            //orders = orders.Where(u => u.WorkStateID == workStateId).ToList();
        //                                            cdt = Expression.Equal(Expression.PropertyOrField(parameter, "WorkStateID"), Expression.Constant(workStateId, typeof(Nullable<int>)));
        //                                        }

        //                                    }
        //                                    break;

        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion


        //                    #region JobNo
        //                    case "JobNo":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
        //                                        cdt = Expression.Equal(Expression.PropertyOrField(parameter, "JobNo"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_BW:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().StartsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "JobNo"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_EW:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().EndsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "JobNo"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_CN:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().Contains(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "JobNo"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region BillingCode
        //                    case "BillingCode":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
        //                                        cdt = Expression.Equal(Expression.PropertyOrField(parameter, "BillingCode"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_BW:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().StartsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingCode"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_EW:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().EndsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingCode"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_CN:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().Contains(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingCode"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region BillingName
        //                    case "BillingName":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
        //                                        cdt = Expression.Equal(Expression.PropertyOrField(parameter, "BillingName"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_BW:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().StartsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_EW:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().EndsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_CN:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().Contains(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region BillingTypeLabel
        //                    case "BillingTypeLabel":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        int workStateId = Convert.ToInt32(data.Split('~')[0]);
        //                                        if (workStateId != -1)
        //                                        {
        //                                            //orders = orders.Where(u => u.WorkStateID == workStateId).ToList();
        //                                            cdt = Expression.Equal(Expression.PropertyOrField(parameter, "BillingTypeID"), Expression.Constant(workStateId, typeof(Nullable<int>)));
        //                                        }

        //                                    }
        //                                    break;

        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region TimeTypeLabel
        //                    case "TimeTypeLabel":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        int workStateId = Convert.ToInt32(data.Split('~')[0]);
        //                                        if (workStateId != -1)
        //                                        {
        //                                            //orders = orders.Where(u => u.WorkStateID == workStateId).ToList();
        //                                            cdt = Expression.Equal(Expression.PropertyOrField(parameter, "TimeTypeID"), Expression.Constant(workStateId, typeof(Nullable<int>)));
        //                                        }

        //                                    }
        //                                    break;

        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region Amount
        //                    case "Amount":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
        //                                        cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Amount"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_LT:
        //                                    {
        //                                        //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
        //                                        cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "Amount"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_LE:
        //                                    {
        //                                        //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
        //                                        cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "Amount"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_GT:
        //                                    {
        //                                        //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
        //                                        cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "Amount"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_GE:
        //                                    {
        //                                        //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
        //                                        cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "Amount"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion


        //                    #region BillingRemark
        //                    case "BillingRemark":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.Remark.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
        //                                        cdt = Expression.Equal(Expression.PropertyOrField(parameter, "BillingRemark"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_BW:
        //                                    {
        //                                        //orders = orders.Where(u => u.Remark.ToLower().StartsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingRemark"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_EW:
        //                                    {
        //                                        //orders = orders.Where(u => u.Remark.ToLower().EndsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingRemark"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_CN:
        //                                    {
        //                                        //orders = orders.Where(u => u.Remark.ToLower().Contains(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingRemark"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region Month
        //                    case "Month":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
        //                                        cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Month"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_BW:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().StartsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "Month"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_EW:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().EndsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "Month"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_CN:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().Contains(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "Month"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region TimesNo
        //                    case "TimesNo":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
        //                                        cdt = Expression.Equal(Expression.PropertyOrField(parameter, "TimesNo"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_LT:
        //                                    {
        //                                        //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
        //                                        cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "TimesNo"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_LE:
        //                                    {
        //                                        //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
        //                                        cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "TimesNo"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_GT:
        //                                    {
        //                                        //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
        //                                        cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "TimesNo"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_GE:
        //                                    {
        //                                        //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
        //                                        cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "TimesNo"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region Status
        //                    case "Status":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
        //                                        cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Status"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_BW:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().StartsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "Status"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_EW:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().EndsWith(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "Status"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_CN:
        //                                    {
        //                                        //orders = orders.Where(u => u.WorkPlace.ToLower().Contains(data.Trim().ToLower())).ToList();
        //                                        cdt = Expression.Call(Expression.PropertyOrField(parameter, "Status"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region Phase
        //                    case "Phase":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
        //                                        cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_LT:
        //                                    {
        //                                        //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
        //                                        cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_LE:
        //                                    {
        //                                        //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
        //                                        cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_GT:
        //                                    {
        //                                        //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
        //                                        cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_GE:
        //                                    {
        //                                        //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
        //                                        cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region BillingCreateDate
        //                    case "BillingCreateDate":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.CreateDate == data.Trim()).ToList();
        //                                        cdt = Expression.Equal(Expression.PropertyOrField(parameter, "BillingCreateDate"), Expression.Constant(data.Trim()));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_LT:
        //                                    {
        //                                        //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1).ToList();
        //                                        //cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "CreateDate"), Expression.Constant(data.Trim()));
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "BillingCreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_LE:
        //                                    {
        //                                        //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "BillingCreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_GT:
        //                                    {
        //                                        //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "BillingCreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_GE:
        //                                    {
        //                                        //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "BillingCreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion

        //                    #region BillingLastUpDate
        //                    case "BillingLastUpDate":
        //                        {
        //                            Expression cdt = null;
        //                            switch (op)
        //                            {
        //                                case ConstValue.ComparisonOperator_EQ:
        //                                    {
        //                                        //orders = orders.Where(u => u.LastUpDate == data.Trim()).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "BillingLastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_LT:
        //                                    {
        //                                        //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "BillingLastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_LE:
        //                                    {
        //                                        //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "BillingLastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_GT:
        //                                    {
        //                                        //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "BillingLastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
        //                                    }
        //                                    break;
        //                                case ConstValue.ComparisonOperator_GE:
        //                                    {
        //                                        //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
        //                                        Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "BillingLastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
        //                                        cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(typeof(Int32)));
        //                                    }
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            if (cdt != null)
        //                            {
        //                                condition = Expression.AndAlso(condition, cdt);
        //                            }
        //                        }
        //                        break;
        //                    #endregion



        //                    default:
        //                        break;
        //                }
        //                #endregion

        //            }

        //        }

        //        #region 执行查询
        //        if (condition != null)
        //        {
        //            var lamda = Expression.Lambda<Func<V_OrderBilling, bool>>(condition, parameter);
        //            orders = db.V_OrderBilling.Where(lamda).Select(u => u).ToList<V_OrderBilling>();
        //        }
        //        else
        //        {
        //            orders = db.V_OrderBilling.Select(u => u).ToList<V_OrderBilling>();
        //        }
        //        #endregion

        //        orders = orders
        //            .Where(u => u.WorkStateID == 5
        //            || u.WorkStateValue == "3"
        //            || u.WorkStateLabel == "已完工")
        //            .Select(u => u).ToList<V_OrderBilling>();

        //        #region 对搜索结果根据排序字段和方式进行排序
        //        switch (orderField)
        //        {
        //            case "":
        //                {
        //                    //if(orderMethod.ToLower().Equals("asc"))
        //                    //    orders = orders.OrderBy(u => u.IDX).ToList();
        //                    //else
        //                    orders = orders.OrderByDescending(u => u.OrderID).ToList();
        //                }
        //                break;
        //            case "CustomerName":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.CustomerName).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.CustomerName).ToList();
        //                }
        //                break;
        //            case "OrderCode":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.OrderCode).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.OrderCode).ToList();
        //                }
        //                break;

        //            case "OrdDate":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.OrdDate).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.OrdDate).ToList();
        //                }
        //                break;
        //            //case "WorkTime":
        //            //    {
        //            //        if (orderMethod.ToLower().Equals("asc"))
        //            //            orders = orders.OrderBy(u => u.WorkTime).ToList();
        //            //        else
        //            //            orders = orders.OrderByDescending(u => u.WorkTime).ToList();
        //            //    }
        //            //    break;
        //            //case "EstimatedCompletionTime":
        //            //    {
        //            //        if (orderMethod.ToLower().Equals("asc"))
        //            //            orders = orders.OrderBy(u => u.EstimatedCompletionTime).ToList();
        //            //        else
        //            //            orders = orders.OrderByDescending(u => u.EstimatedCompletionTime).ToList();
        //            //    }
        //            //    break;
        //            case "ShipName":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.ShipName).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.ShipName).ToList();
        //                }
        //                break;


        //            //case "ServiceNatureNames":
        //            //    {
        //            //        if (orderMethod.ToLower().Equals("asc"))
        //            //            orders = orders.OrderBy(u => u.ServiceNatureNames).ToList();
        //            //        else
        //            //            orders = orders.OrderByDescending(u => u.ServiceNatureNames).ToList();
        //            //    }
        //            //    break;
        //            case "WorkStateLabel":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.WorkStateLabel).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.WorkStateLabel).ToList();
        //                }
        //                break;

        //            case "JobNo":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.JobNo).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.JobNo).ToList();
        //                }
        //                break;

        //            case "BillingCode":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.BillingCode).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.BillingCode).ToList();
        //                }
        //                break;

        //            case "BillingName":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.BillingName).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.BillingName).ToList();
        //                }
        //                break;

        //            case "BillingTypeLabel":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.BillingTypeLabel).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.BillingTypeLabel).ToList();
        //                }
        //                break;
        //            case "TimeTypeLabel":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.TimeTypeLabel).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.TimeTypeLabel).ToList();
        //                }
        //                break;
        //            case "Amount":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.Amount).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.Amount).ToList();
        //                }
        //                break;
        //            case "BillingRemark":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.BillingRemark).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.BillingRemark).ToList();
        //                }
        //                break;
        //            case "Month":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.Month).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.Month).ToList();
        //                }
        //                break;
        //            case "TimesNo":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.TimesNo).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.TimesNo).ToList();
        //                }
        //                break;
        //            case "Status":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.Status).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.Status).ToList();
        //                }
        //                break;
        //            case "Phase":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.Phase).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.Phase).ToList();
        //                }
        //                break;
        //            case "BillingCreateDate":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.BillingCreateDate).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.BillingCreateDate).ToList();
        //                }
        //                break;
        //            case "BillingLastUpDate":
        //                {
        //                    if (orderMethod.ToLower().Equals("asc"))
        //                        orders = orders.OrderBy(u => u.BillingLastUpDate).ToList();
        //                    else
        //                        orders = orders.OrderByDescending(u => u.BillingLastUpDate).ToList();
        //                }
        //                break;


        //            default:
        //                break;
        //        }

        //        #endregion


        //        JArray groups = (JArray)jsonSearchOption["groups"];
        //        if (groups != null)
        //        {
        //            foreach (JObject item in groups)
        //            {
        //                string item_groupOp = (string)item["groupOp"];
        //                JArray item_groups = (JArray)item["groups"];
        //                JArray item_rules = (JArray)item["rules"];
        //                string item_rule0_field = (string)(((JObject)item_rules[0])["field"]);
        //                string item_rule0_op = (string)(((JObject)item_rules[0])["op"]);
        //                string item_rule0_data = (string)(((JObject)item_rules[0])["data"]);

        //                string item_rule1_field = (string)(((JObject)item_rules[1])["field"]);
        //                string item_rule1_op = (string)(((JObject)item_rules[1])["op"]);
        //                string item_rule1_data = (string)(((JObject)item_rules[1])["data"]);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //    return orders;
        //}









        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderMethod">排序方式asc升序；desc降序</param>
        /// <returns></returns>
        static public List<DataModel.V_Billing2> LoadDataForBilling(string orderField, string orderMethod)
        {
            List<V_Billing2> orders = null;

            try
            {
                TugDataEntities db = new TugDataEntities();
                orders = db.V_Billing2.Where(u => u.InvoiceType == "普通账单").Select(u => u).ToList<V_Billing2>();

                #region 根据排序字段和排序方式排序
                switch (orderField)
                {
                    case "":
                        {
                            orders = orders.OrderByDescending(u => u.IDX).ToList();
                        }
                        break;
                    case "JobNo":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.JobNo).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.JobNo).ToList();
                        }
                        break;
                    case "CustomerName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CustomerName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CustomerName).ToList();
                        }
                        break;
                    case "ShipName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipName).ToList();
                        }
                        break;
                    case "BillingCode":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.BillingCode).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.BillingCode).ToList();
                        }
                        break;

                    case "BillingTemplateName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.BillingTemplateName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.BillingTemplateName).ToList();
                        }
                        break;
                    case "BillingTemplateTypeLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.BillingTemplateTypeLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.BillingTemplateTypeLabel).ToList();
                        }
                        break;

                    case "TimeTypeLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.TimeTypeLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.TimeTypeLabel).ToList();
                        }
                        break;

                    case "ShipLength":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipLength).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipLength).ToList();
                        }
                        break;

                    case "ShipTEUS":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipTEUS).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipTEUS).ToList();
                        }
                        break;

                    case "Month":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Month).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Month).ToList();
                        }
                        break;

                    case "Amount":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Amount).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Amount).ToList();
                        }
                        break;

                    case "Status":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Status).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Status).ToList();
                        }
                        break;

                    case "Remark":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Remark).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Remark).ToList();
                        }
                        break;


                    case "CreateDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CreateDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CreateDate).ToList();
                        }
                        break;
                    case "LastUpDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LastUpDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LastUpDate).ToList();
                        }
                        break;


                    default:
                        break;
                }

                #endregion
            }
            catch (Exception ex)
            {
                return null;
            }

            return orders;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchOptions">搜索选项，格式如下</param>
        /// <returns></returns>
        static public List<DataModel.V_Billing2> SearchDataForBilling(string orderField, string orderMethod, string searchOptions)
        {
            List<V_Billing2> orders = null;
            try
            {
                //searchOptions的Json字符串格式
                //{
                //    "groupOp":"AND",
                //    "rules":[{"field":"IsGuest","op":"eq","data":"全部"}],
                //    "groups":[
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"BigTugNum"},{"data":"2","op":"le","field":"BigTugNum"}]},
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"MiddleTugNum"},{"data":"2","op":"le","field":"MiddleTugNum"}]},
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"SmallTugNum"},{"data":"2","op":"le","field":"SmallTugNum"}]}
                //    ]

                //}



                TugDataEntities db = new TugDataEntities();
                //orders = db.V_OrderInfor.Select(u => u).ToList<V_OrderInfor>();

                JObject jsonSearchOption = (JObject)JsonConvert.DeserializeObject(searchOptions);
                string groupOp = (string)jsonSearchOption["groupOp"];
                JArray rules = (JArray)jsonSearchOption["rules"];

                Expression condition = Expression.Equal(Expression.Constant(1, typeof(int)), Expression.Constant(1, typeof(int)));
                ParameterExpression parameter = Expression.Parameter(typeof(V_Billing2));

                Expression condition2 = Expression.Equal(Expression.PropertyOrField(parameter, "InvoiceType"), Expression.Constant("普通账单"));
                condition = Expression.AndAlso(condition, condition2);

                if (rules != null)
                {
                    foreach (JObject item in rules)
                    {
                        string field = (string)item["field"];
                        string op = (string)item["op"];
                        string data = (string)item["data"];

                        #region 根据各字段条件进行条件表达式拼接
                        switch (field)
                        {

                            #region JobNo
                            case "JobNo":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "JobNo"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "JobNo"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "JobNo"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "JobNo"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region BillingCode
                            case "BillingCode":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "BillingCode"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingCode"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingCode"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingCode"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region CustomerName
                            case "CustomerName":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "CustomerName"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region ShipName
                            case "ShipName":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "ShipName"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region BillingTemplateName
                            case "BillingTemplateName":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "BillingTemplateName"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingTemplateName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingTemplateName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingTemplateName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region BillingTemplateTypeLabel
                            case "BillingTemplateTypeLabel":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                int workStateId = Convert.ToInt32(data.Split('~')[0]);
                                                if (workStateId != -1)
                                                {
                                                    //orders = orders.Where(u => u.WorkStateID == workStateId).ToList();
                                                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "BillingTypeID"), Expression.Constant(workStateId, typeof(Nullable<int>)));
                                                }

                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region TimeTypeLabel
                            case "TimeTypeLabel":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                int workStateId = Convert.ToInt32(data.Split('~')[0]);
                                                if (workStateId != -1)
                                                {
                                                    //orders = orders.Where(u => u.WorkStateID == workStateId).ToList();
                                                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "TimeTypeID"), Expression.Constant(workStateId, typeof(Nullable<int>)));
                                                }

                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region ShipLength
                            case "ShipLength":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "ShipLength"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipLength"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipLength"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipLength"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region ShipTEUS
                            case "ShipTEUS":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "ShipTEUS"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipTEUS"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipTEUS"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipTEUS"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion
                    
                            #region Amount
                            case "Amount":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Amount"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "Amount"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "Amount"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "Amount"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "Amount"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Month
                            case "Month":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.CreateDate == data.Trim()).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Month"), Expression.Constant(data.Trim()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1).ToList();
                                                //cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "CreateDate"), Expression.Constant(data.Trim()));
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "Month"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "Month"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "Month"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "Month"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Remark
                            case "Remark":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Remark"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Remark"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Remark"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Remark"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion
             
                            #region Status
                            case "Status":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Status"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Status"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Status"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Status"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Phase
                            case "Phase":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region CreateDate
                            case "CreateDate":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.CreateDate == data.Trim()).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "CreateDate"), Expression.Constant(data.Trim()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1).ToList();
                                                //cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "CreateDate"), Expression.Constant(data.Trim()));
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region LastUpDate
                            case "LastUpDate":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion


                            default:
                                break;
                        }
                        #endregion

                    }

                }

                #region 执行查询
                if (condition != null)
                {
                    var lamda = Expression.Lambda<Func<V_Billing2, bool>>(condition, parameter);
                    orders = db.V_Billing2.Where(lamda).Select(u => u).ToList<V_Billing2>();
                }
                else
                {
                    orders = db.V_Billing2.Where(u => u.InvoiceType == "普通账单").Select(u => u).ToList<V_Billing2>();
                }
                #endregion

                orders = orders.Select(u => u).ToList<V_Billing2>();

                #region 对搜索结果根据排序字段和方式进行排序
                switch (orderField)
                {
                    case "":
                        {
                            orders = orders.OrderByDescending(u => u.IDX).ToList();
                        }
                        break;
                    case "JobNo":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.JobNo).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.JobNo).ToList();
                        }
                        break;
                    case "CustomerName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CustomerName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CustomerName).ToList();
                        }
                        break;
                    case "ShipName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipName).ToList();
                        }
                        break;
                    case "BillingCode":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.BillingCode).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.BillingCode).ToList();
                        }
                        break;

                    case "BillingTemplateName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.BillingTemplateName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.BillingTemplateName).ToList();
                        }
                        break;
                    case "BillingTemplateTypeLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.BillingTemplateTypeLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.BillingTemplateTypeLabel).ToList();
                        }
                        break;

                    case "TimeTypeLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.TimeTypeLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.TimeTypeLabel).ToList();
                        }
                        break;

                    case "ShipLength":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipLength).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipLength).ToList();
                        }
                        break;

                    case "ShipTEUS":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipTEUS).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipTEUS).ToList();
                        }
                        break;

                    case "Month":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Month).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Month).ToList();
                        }
                        break;

                    case "Amount":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Amount).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Amount).ToList();
                        }
                        break;

                    case "Status":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Status).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Status).ToList();
                        }
                        break;

                    case "Remark":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Remark).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Remark).ToList();
                        }
                        break;


                    case "CreateDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CreateDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CreateDate).ToList();
                        }
                        break;
                    case "LastUpDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LastUpDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LastUpDate).ToList();
                        }
                        break;


                    default:
                        break;
                }

                #endregion


                JArray groups = (JArray)jsonSearchOption["groups"];
                if (groups != null)
                {
                    foreach (JObject item in groups)
                    {
                        string item_groupOp = (string)item["groupOp"];
                        JArray item_groups = (JArray)item["groups"];
                        JArray item_rules = (JArray)item["rules"];
                        string item_rule0_field = (string)(((JObject)item_rules[0])["field"]);
                        string item_rule0_op = (string)(((JObject)item_rules[0])["op"]);
                        string item_rule0_data = (string)(((JObject)item_rules[0])["data"]);

                        string item_rule1_field = (string)(((JObject)item_rules[1])["field"]);
                        string item_rule1_op = (string)(((JObject)item_rules[1])["op"]);
                        string item_rule1_data = (string)(((JObject)item_rules[1])["data"]);
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return orders;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderMethod">排序方式asc升序；desc降序</param>
        /// <returns></returns>
        static public List<DataModel.V_Billing4> LoadDataForDiscountBilling(string orderField, string orderMethod)
        {
            List<V_Billing4> orders = null;

            try
            {
                TugDataEntities db = new TugDataEntities();
                orders = db.V_Billing4.Where(u => u.InvoiceType == "优惠单").Select(u => u).ToList<V_Billing4>();

                #region 根据排序字段和排序方式排序
                switch (orderField)
                {
                    case "":
                        {
                            orders = orders.OrderByDescending(u => u.IDX).ToList();
                        }
                        break;

                    case "CustomerName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CustomerName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CustomerName).ToList();
                        }
                        break;

                    case "BillingCode":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.BillingCode).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.BillingCode).ToList();
                        }
                        break;


                    case "Title":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Title).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Title).ToList();
                        }
                        break;

                    case "Content":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Content).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Content).ToList();
                        }
                        break;

                    case "Money":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Money).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Money).ToList();
                        }
                        break;



                    case "Month":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Month).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Month).ToList();
                        }
                        break;



                    case "Status":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Status).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Status).ToList();
                        }
                        break;


                    case "CreateDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CreateDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CreateDate).ToList();
                        }
                        break;
                    case "LastUpDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LastUpDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LastUpDate).ToList();
                        }
                        break;


                    default:
                        break;
                }

                #endregion
            }
            catch (Exception ex)
            {
                return null;
            }

            return orders;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchOptions">搜索选项，格式如下</param>
        /// <returns></returns>
        static public List<DataModel.V_Billing4> SearchDataForDiscountBilling(string orderField, string orderMethod, string searchOptions)
        {
            List<V_Billing4> orders = null;
            try
            {
                //searchOptions的Json字符串格式
                //{
                //    "groupOp":"AND",
                //    "rules":[{"field":"IsGuest","op":"eq","data":"全部"}],
                //    "groups":[
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"BigTugNum"},{"data":"2","op":"le","field":"BigTugNum"}]},
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"MiddleTugNum"},{"data":"2","op":"le","field":"MiddleTugNum"}]},
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"SmallTugNum"},{"data":"2","op":"le","field":"SmallTugNum"}]}
                //    ]

                //}



                TugDataEntities db = new TugDataEntities();
                //orders = db.V_OrderInfor.Select(u => u).ToList<V_OrderInfor>();

                JObject jsonSearchOption = (JObject)JsonConvert.DeserializeObject(searchOptions);
                string groupOp = (string)jsonSearchOption["groupOp"];
                JArray rules = (JArray)jsonSearchOption["rules"];

                Expression condition = Expression.Equal(Expression.Constant(1, typeof(int)), Expression.Constant(1, typeof(int)));
                ParameterExpression parameter = Expression.Parameter(typeof(V_Billing4));

                Expression condition2 = Expression.Equal(Expression.PropertyOrField(parameter, "InvoiceType"), Expression.Constant("优惠单"));
                condition = Expression.AndAlso(condition, condition2);

                if (rules != null)
                {
                    foreach (JObject item in rules)
                    {
                        string field = (string)item["field"];
                        string op = (string)item["op"];
                        string data = (string)item["data"];

                        #region 根据各字段条件进行条件表达式拼接
                        switch (field)
                        {


                            #region BillingCode
                            case "BillingCode":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "BillingCode"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingCode"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingCode"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingCode"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region CustomerName
                            case "CustomerName":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "CustomerName"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Title
                            case "Title":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Title"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Title"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Title"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Title"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Content
                            case "Content":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Content"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Content"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Content"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Content"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion


                            #region Money
                            case "Money":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Money"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "Money"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "Money"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "Money"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "Money"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Month
                            case "Month":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.CreateDate == data.Trim()).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Month"), Expression.Constant(data.Trim()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1).ToList();
                                                //cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "CreateDate"), Expression.Constant(data.Trim()));
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "Month"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "Month"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "Month"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "Month"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion
                 

                            #region Status
                            case "Status":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Status"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Status"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Status"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Status"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Phase
                            case "Phase":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region CreateDate
                            case "CreateDate":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.CreateDate == data.Trim()).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "CreateDate"), Expression.Constant(data.Trim()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1).ToList();
                                                //cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "CreateDate"), Expression.Constant(data.Trim()));
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region LastUpDate
                            case "LastUpDate":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion


                            default:
                                break;
                        }
                        #endregion

                    }

                }

                #region 执行查询
                if (condition != null)
                {
                    var lamda = Expression.Lambda<Func<V_Billing4, bool>>(condition, parameter);
                    orders = db.V_Billing4.Where(lamda).Select(u => u).ToList<V_Billing4>();
                }
                else
                {
                    orders = db.V_Billing4.Where(u => u.InvoiceType == "优惠单").Select(u => u).ToList<V_Billing4>();
                }
                #endregion

                orders = orders.Select(u => u).ToList<V_Billing4>();

                #region 根据排序字段和排序方式排序
                switch (orderField)
                {
                    case "":
                        {
                            orders = orders.OrderByDescending(u => u.IDX).ToList();
                        }
                        break;

                    case "CustomerName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CustomerName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CustomerName).ToList();
                        }
                        break;

                    case "BillingCode":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.BillingCode).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.BillingCode).ToList();
                        }
                        break;


                    case "Title":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Title).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Title).ToList();
                        }
                        break;

                    case "Content":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Content).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Content).ToList();
                        }
                        break;

                    case "Money":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Money).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Money).ToList();
                        }
                        break;



                    case "Month":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Month).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Month).ToList();
                        }
                        break;



                    case "Status":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Status).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Status).ToList();
                        }
                        break;


                    case "CreateDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CreateDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CreateDate).ToList();
                        }
                        break;
                    case "LastUpDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LastUpDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LastUpDate).ToList();
                        }
                        break;


                    default:
                        break;
                }

                #endregion


                JArray groups = (JArray)jsonSearchOption["groups"];
                if (groups != null)
                {
                    foreach (JObject item in groups)
                    {
                        string item_groupOp = (string)item["groupOp"];
                        JArray item_groups = (JArray)item["groups"];
                        JArray item_rules = (JArray)item["rules"];
                        string item_rule0_field = (string)(((JObject)item_rules[0])["field"]);
                        string item_rule0_op = (string)(((JObject)item_rules[0])["op"]);
                        string item_rule0_data = (string)(((JObject)item_rules[0])["data"]);

                        string item_rule1_field = (string)(((JObject)item_rules[1])["field"]);
                        string item_rule1_op = (string)(((JObject)item_rules[1])["op"]);
                        string item_rule1_data = (string)(((JObject)item_rules[1])["data"]);
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return orders;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderMethod">排序方式asc升序；desc降序</param>
        /// <returns></returns>
        static public List<DataModel.V_Billing3> LoadDataForSpecialBilling(string orderField, string orderMethod)
        {
            List<V_Billing3> orders = null;

            try
            {
                TugDataEntities db = new TugDataEntities();
                orders = db.V_Billing3.Where(u => u.InvoiceType == "特殊账单").Select(u => u).ToList<V_Billing3>();

                #region 根据排序字段和排序方式排序
                switch (orderField)
                {
                    case "":
                        {
                            orders = orders.OrderByDescending(u => u.IDX).ToList();
                        }
                        break;
                    
                    case "CustomerName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CustomerName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CustomerName).ToList();
                        }
                        break;

                    case "BillingCode":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.BillingCode).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.BillingCode).ToList();
                        }
                        break;

                    


                    case "Amount":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Amount).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Amount).ToList();
                        }
                        break;

                    case "Status":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Status).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Status).ToList();
                        }
                        break;

                    case "Month":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Month).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Month).ToList();
                        }
                        break;

                    case "Remark":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Remark).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Remark).ToList();
                        }
                        break;


                    case "CreateDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CreateDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CreateDate).ToList();
                        }
                        break;
                    case "LastUpDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LastUpDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LastUpDate).ToList();
                        }
                        break;


                    default:
                        break;
                }

                #endregion
            }
            catch (Exception ex)
            {
                return null;
            }

            return orders;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchOptions">搜索选项，格式如下</param>
        /// <returns></returns>
        static public List<DataModel.V_Billing3> SearchDataForSpecialBilling(string orderField, string orderMethod, string searchOptions)
        {
            List<V_Billing3> orders = null;
            try
            {
                //searchOptions的Json字符串格式
                //{
                //    "groupOp":"AND",
                //    "rules":[{"field":"IsGuest","op":"eq","data":"全部"}],
                //    "groups":[
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"BigTugNum"},{"data":"2","op":"le","field":"BigTugNum"}]},
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"MiddleTugNum"},{"data":"2","op":"le","field":"MiddleTugNum"}]},
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"SmallTugNum"},{"data":"2","op":"le","field":"SmallTugNum"}]}
                //    ]

                //}



                TugDataEntities db = new TugDataEntities();
                //orders = db.V_OrderInfor.Select(u => u).ToList<V_OrderInfor>();

                JObject jsonSearchOption = (JObject)JsonConvert.DeserializeObject(searchOptions);
                string groupOp = (string)jsonSearchOption["groupOp"];
                JArray rules = (JArray)jsonSearchOption["rules"];

                Expression condition = Expression.Equal(Expression.Constant(1, typeof(int)), Expression.Constant(1, typeof(int)));
                ParameterExpression parameter = Expression.Parameter(typeof(V_Billing3));

                Expression condition2 = Expression.Equal(Expression.PropertyOrField(parameter, "InvoiceType"), Expression.Constant("特殊账单"));
                condition = Expression.AndAlso(condition, condition2);

                if (rules != null)
                {
                    foreach (JObject item in rules)
                    {
                        string field = (string)item["field"];
                        string op = (string)item["op"];
                        string data = (string)item["data"];

                        #region 根据各字段条件进行条件表达式拼接
                        switch (field)
                        {

                            #region JobNo
                            case "JobNo":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "JobNo"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "JobNo"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "JobNo"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "JobNo"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region BillingCode
                            case "BillingCode":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "BillingCode"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingCode"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingCode"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingCode"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region CustomerName
                            case "CustomerName":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "CustomerName"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region ShipName
                            case "ShipName":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "ShipName"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region BillingTemplateName
                            case "BillingTemplateName":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "BillingTemplateName"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingTemplateName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingTemplateName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "BillingTemplateName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region BillingTemplateTypeLabel
                            case "BillingTemplateTypeLabel":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                int workStateId = Convert.ToInt32(data.Split('~')[0]);
                                                if (workStateId != -1)
                                                {
                                                    //orders = orders.Where(u => u.WorkStateID == workStateId).ToList();
                                                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "BillingTypeID"), Expression.Constant(workStateId, typeof(Nullable<int>)));
                                                }

                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region TimeTypeLabel
                            case "TimeTypeLabel":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                int workStateId = Convert.ToInt32(data.Split('~')[0]);
                                                if (workStateId != -1)
                                                {
                                                    //orders = orders.Where(u => u.WorkStateID == workStateId).ToList();
                                                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "TimeTypeID"), Expression.Constant(workStateId, typeof(Nullable<int>)));
                                                }

                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Amount
                            case "Amount":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Amount"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "Amount"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "Amount"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "Amount"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "Amount"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Month
                            case "Month":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.CreateDate == data.Trim()).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Month"), Expression.Constant(data.Trim()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1).ToList();
                                                //cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "CreateDate"), Expression.Constant(data.Trim()));
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "Month"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "Month"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "Month"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "Month"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Remark
                            case "Remark":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Remark"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Remark"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Remark"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Remark"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Status
                            case "Status":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Status"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Status"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Status"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Status"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Phase
                            case "Phase":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "Phase"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region CreateDate
                            case "CreateDate":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.CreateDate == data.Trim()).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "CreateDate"), Expression.Constant(data.Trim()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1).ToList();
                                                //cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "CreateDate"), Expression.Constant(data.Trim()));
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region LastUpDate
                            case "LastUpDate":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion


                            default:
                                break;
                        }
                        #endregion

                    }

                }

                #region 执行查询
                if (condition != null)
                {
                    var lamda = Expression.Lambda<Func<V_Billing3, bool>>(condition, parameter);
                    orders = db.V_Billing3.Where(lamda).Select(u => u).ToList<V_Billing3>();
                }
                else
                {
                    orders = db.V_Billing3.Where(u => u.InvoiceType == "特殊账单").Select(u => u).ToList<V_Billing3>();
                }
                #endregion

                orders = orders.Select(u => u).ToList<V_Billing3>();

                #region 对搜索结果根据排序字段和方式进行排序
                switch (orderField)
                {
                    case "":
                        {
                            orders = orders.OrderByDescending(u => u.IDX).ToList();
                        }
                        break;
                    
                    case "CustomerName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CustomerName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CustomerName).ToList();
                        }
                        break;

                    case "BillingCode":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.BillingCode).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.BillingCode).ToList();
                        }
                        break;


                    case "Month":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Month).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Month).ToList();
                        }
                        break;

                    case "Amount":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Amount).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Amount).ToList();
                        }
                        break;

                    case "Status":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Status).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Status).ToList();
                        }
                        break;

                    case "Remark":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Remark).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Remark).ToList();
                        }
                        break;


                    case "CreateDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CreateDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CreateDate).ToList();
                        }
                        break;
                    case "LastUpDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LastUpDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LastUpDate).ToList();
                        }
                        break;


                    default:
                        break;
                }

                #endregion


                JArray groups = (JArray)jsonSearchOption["groups"];
                if (groups != null)
                {
                    foreach (JObject item in groups)
                    {
                        string item_groupOp = (string)item["groupOp"];
                        JArray item_groups = (JArray)item["groups"];
                        JArray item_rules = (JArray)item["rules"];
                        string item_rule0_field = (string)(((JObject)item_rules[0])["field"]);
                        string item_rule0_op = (string)(((JObject)item_rules[0])["op"]);
                        string item_rule0_data = (string)(((JObject)item_rules[0])["data"]);

                        string item_rule1_field = (string)(((JObject)item_rules[1])["field"]);
                        string item_rule1_op = (string)(((JObject)item_rules[1])["op"]);
                        string item_rule1_data = (string)(((JObject)item_rules[1])["data"]);
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return orders;
        }







        static public List<string> GetOrderSchedulerRemarks(int orderId)
        {
            TugDataEntities db = new TugDataEntities();
            List<string> list = db.V_OrderScheduler.Where(u => u.OrderID == orderId).Select(u => u.Remark).ToList();
            return list;
        }


        /// <summary>
        /// 从燃油附加费方案中获取应该使用哪个附加费用
        /// </summary>
        /// <param name="orderDate"></param>
        /// <returns></returns>
        static public double GetFuelFee(string serviceDate)
        {
            //DateTime dt = BusinessLogic.Utils.HKDateTimeToDateTime(orderDate);
            DateTime dt = BusinessLogic.Utils.CNDateTimeToDateTime(serviceDate);

            TugDataEntities db = new TugDataEntities();
            var list = db.Fuelprice.Select(u => u).ToList();
            if (list != null)
            {
                List<MyFuelFee> lstFuelFee = new List<MyFuelFee>();
                foreach (var item in list)
                {
                    MyFuelFee ff = new MyFuelFee();
                    ff.IDX = item.IDX;
                    //ff.EffectiveDate = BusinessLogic.Utils.HKDateTimeToDateTime(item.EffectiveDate);
                    ff.EffectiveDate = BusinessLogic.Utils.CNDateTimeToDateTime(item.EffectiveDate);
                    ff.Price = (double)item.Price;
                    lstFuelFee.Add(ff);
                }

                List<MyFuelFee> orderedFuelFeeList = lstFuelFee.OrderBy(u => u.EffectiveDate).ToList();

                int index = 0;

                for (int i = 0; i < orderedFuelFeeList.Count; i++)
                {
                    int ret = orderedFuelFeeList[i].EffectiveDate.CompareTo(dt);

                    if (ret <= 0)
                    {
                        index = i;
                    }
                    else
                    {
                        break;
                    }
                        
                }

                double v = orderedFuelFeeList[index].Price;

                return v;
            }

            return 0;
        }


        /// <summary>
        /// 获取给定计费方案下的燃油附加费的折扣费
        /// </summary>
        /// <param name="billingTemplateId"></param>
        /// <returns></returns>
        static public double GetDiscoutPriceOfFuelFee(int billingTemplateId)
        {
            double discoutPriceOfFuelFee = 0;

            TugDataEntities db = new TugDataEntities();
            V_BillingItemTemplate bit = db.V_BillingItemTemplate.FirstOrDefault(u => u.BillingTemplateID == billingTemplateId 
                && (u.ItemID == 119 || u.ItemValue == "E80" || u.ItemLabel == "燃油附加费折扣"));
            if (bit != null) {
                discoutPriceOfFuelFee = (double)bit.UnitPrice;
            }

            return discoutPriceOfFuelFee;
        }

        /// <summary>
        /// 计算燃油附加费
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="consumeTime"></param>
        /// <returns></returns>
        static public double CalculateFuelFee(string serviceDate, double consumeTime)
        {
            double unitPrice = GetFuelFee(serviceDate);
            double price = unitPrice;

            if (consumeTime <= 1) {
                price = Math.Round(price, 2);
            }
            else
            {
                price = Math.Round(unitPrice + unitPrice * (consumeTime - 1), 2);
            }

            return price;
        }


        //合并账单用的驳回删除
        static public void RejectInvoice2(int billingId)
        {
            TugDataEntities db = new TugDataEntities();
            var orders = db.BillingOrder.Where(u => u.BillingID == billingId).ToList();
            if (orders != null)
            {
                foreach (var order in orders)
                {
                    //item.OrderID;
                    List<OrderService> services = db.OrderService.Where(u => u.OrderID == order.OrderID).ToList();
                    if (services != null)
                    {
                        foreach (OrderService svc in services)
                        {
                            List<Scheduler> schedulers = db.Scheduler.Where(u => u.OrderServiceID == svc.IDX).ToList();
                            if (schedulers != null)
                            {
                                foreach (Scheduler sch in schedulers)
                                {
                                    //sch.DepartBaseTime = "";
                                    //sch.ArrivalBaseTime = "";
                                    db.Entry(sch).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }

                            //svc.JobStateID = 114;
                            db.Entry(svc).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    OrderInfor ord = db.OrderInfor.First(u => u.IDX == order.OrderID);
                    if (ord != null)
                    {
                        ord.HasInvoice = "否";
                        ord.HasInFlow = "否";
                        //ord.WorkStateID = 2;
                        db.Entry(ord).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }


        /// <summary>
        /// 在普通账单删除后，需要设置其订单下，订单服务的账单有无。
        /// </summary>
        /// <param name="billingId"></param>
        static public void SetOrderServiceInvoiceStatus(int billingId, string hasBilling)
        {
            TugDataEntities db = new TugDataEntities();

            //1.获取账单下的多个订单
            var lstBillingOrder = db.BillingOrder.Where(u => u.BillingID == billingId).ToList();
            if (lstBillingOrder != null)
            {
                foreach (var bo in lstBillingOrder)
                {
                    //2.针对每一个订单，获取该订单下的订单服务，order_service
                    var lstOrderService = db.OrderService.Where(u => u.OrderID == bo.OrderID).ToList();
                    if (lstOrderService != null)
                    {
                        foreach (var os in lstOrderService)
                        {
                            //3.针对每一个订单服务，先在SpecialBillingItem表里面查询，是否有此订单服务；如果有说明该服务已经生成过
                            //特殊账单了，不需要更改这个服务的账单有无状态；如果没有，说明该服务没有生成过特殊账单，需要将其账单有
                            //无状态改为“否”
                            SpecialBillingItem si = db.SpecialBillingItem.FirstOrDefault(u => u.OrderServiceID == os.IDX);
                            if (si == null)
                            {
                                os.HasBilling = hasBilling;
                                db.Entry(os).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 在普通账单提交审核或驳回后，需要设置。
        /// BillingType:0普通账单；1特殊账单
        /// </summary>
        /// <param name="billingId"></param>
        static public void SetOrderServiceFlowingStatus(int BillingType, int billingId, string hasInFlow)
        {
            TugDataEntities db = new TugDataEntities();
            if(BillingType==0)
            {
                //获取账单下的多个订单
                var lstBillingOrder = db.BillingOrder.Where(u => u.BillingID == billingId).ToList();
                if (lstBillingOrder != null)
                {
                    foreach (var bo in lstBillingOrder)
                    {
                        //更新OrderInfor
                        int ordid = Util.toint(bo.OrderID);
                        OrderInfor obj = db.OrderInfor.Where(u => u.IDX == ordid).FirstOrDefault();
                        if (obj != null)
                        {
                            obj.HasInFlow = hasInFlow;
                            db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        //2.针对每一个订单，获取该订单下的订单服务，order_service
                        var lstOrderService = db.OrderService.Where(u => u.OrderID == bo.OrderID).ToList();
                        if (lstOrderService != null)
                        {
                            foreach (var os in lstOrderService)
                            {
                                //SpecialBillingItem si = db.SpecialBillingItem.FirstOrDefault(u => u.OrderServiceID == os.IDX);
                                //if (si == null)
                                if (os.BillingType == 0)
                                {
                                    os.HasBillingInFlow = hasInFlow;
                                    db.Entry(os).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                //特殊账单
                var lstOrderService = db.V_SpecialBillingItem_OrderService.Where(u => u.SpecialBillingID == billingId).ToList();
                if (lstOrderService != null)
                {
                    foreach (var os in lstOrderService)
                    {
                        int osid = Util.toint(os.OrderServiceID);
                        OrderService obj = db.OrderService.Where(u => u.IDX == osid).FirstOrDefault();
                        if (obj != null)
                        {
                            obj.HasBillingInFlow = hasInFlow;
                            db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }

            }
        }


        /// <summary>
        /// 新增或编辑特殊账单之后，插入汇总项目
        /// </summary>
        /// <param name="billingId"></param>
        /// <param name="userId"></param>
        static public void UpdateSpecialBillingSummarizeItems(int billingId, int userId)
        {

            TugDataEntities db = new TugDataEntities();

            var lstSpecialBillingSummarize = db.AmountSum.Where(u => u.BillingID == billingId).ToList();
            if (lstSpecialBillingSummarize != null && lstSpecialBillingSummarize.Count > 0)
            {
                //先删除，再插入
                foreach (var item in lstSpecialBillingSummarize)
                {
                    db.AmountSum.Remove(item);
                    db.SaveChanges();
                }
            }
            //else
            {
                //直接插入
                var list = db.V_SpecialBillingSummarizeItem.Where(u => u.SpecialBillingID == billingId).ToList();
                if (list != null)
                {
                    List<AmountSum> ret = new List<AmountSum>();
                    foreach (var item in list)
                    {
                        AmountSum one = new AmountSum();
                        one.CustomerID = item.CustomerID;
                        one.CustomerShipID = item.CustomerShipID;
                        one.BillingID = billingId;
                        one.BillingDateTime = BusinessLogic.Utils.CNDateTimeToDateTime(item.BillingDateTime);
                        one.SchedulerID = item.SchedulerID;
                        one.Amount = item.Amount;
                        one.Currency = "港币";

                        int iDiffHour, iDiffMinute;
                        BusinessLogic.Utils.CalculateTimeDiff(item.DepartBaseTime, item.ArrivalBaseTime, out iDiffHour, out iDiffMinute);

                        #region 按一小时换算时间
                        {
                            //double consumeTime = 0;
                            //int count = 0;
                            //count += iDiffHour * 60 / 60;
                            //count += iDiffMinute / 60;
                            //if (iDiffMinute % 60 > 0)
                            //{
                            //    count++;
                            //}

                            //consumeTime = (count * 60.0) / 60;

                            double tmp = ((double)iDiffMinute) / 60;
                            one.Hours = iDiffHour + Math.Round(tmp, 2);
                        }
                        #endregion

                        
                        one.Year = item.Month.Split('-')[0];//one.BillingDateTime.Value.Year.ToString();
                        one.Month = item.Month.Split('-')[1];//item.Month;
                        one.OwnerID = -1;
                        one.CreateDate = one.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        one.UserID = userId;

                        ret.Add(one);
                    }

                    db.AmountSum.AddRange(ret);
                    db.SaveChanges();
                }

            }

        }


        static public void RejectInvoice(int orderId = 1)
        {
            //TugDataEntities db = new TugDataEntities();

            //var orders = db.V_OrderBilling.Where(u => u.OrderID == orderId).ToList();
            //if (orders != null)
            //{
            //    foreach (var order in orders)
            //    {
            //        //item.OrderID;
            //        List<OrderService> services = db.OrderService.Where(u => u.OrderID == order.OrderID).ToList();
            //        if (services != null)
            //        {
            //            foreach (OrderService svc in services)
            //            {
            //                List<Scheduler> schedulers = db.Scheduler.Where(u => u.OrderServiceID == svc.IDX).ToList();
            //                if (schedulers != null)
            //                {
            //                    foreach (Scheduler sch in schedulers)
            //                    {
            //                        sch.DepartBaseTime = "";
            //                        sch.ArrivalBaseTime = "";
            //                        db.Entry(sch).State = System.Data.Entity.EntityState.Modified;
            //                        db.SaveChanges();
            //                    }
            //                }

            //                svc.JobStateID = 114;
            //                db.Entry(svc).State = System.Data.Entity.EntityState.Modified;
            //                db.SaveChanges();
            //            }
            //        }

            //        OrderInfor ord = db.OrderInfor.First(u => u.IDX == order.OrderID);
            //        if (ord != null)
            //        {
            //            ord.HasInvoice = "否";
            //            ord.HasInFlow = "否";
            //            ord.WorkStateID = 2;
            //            db.Entry(ord).State = System.Data.Entity.EntityState.Modified;
            //            db.SaveChanges();
            //        }
            //    }
            //}
        }



        
        /// <summary>
        /// 获取用户的计费方案，根据船长
        /// </summary>
        /// <param name="customerId">客户ID</param>
        /// <param name="shipLength">船长</param>
        /// <returns></returns>
        static public List<V_BillingTemplate> GetCustomersBillingTemplateByLength(int customerId, int shipLength)
        {
            List<V_BillingTemplate> retList = new List<V_BillingTemplate>();

            List<V_BillingTemplate> defaultList = GetCustomerBillSchemes(-1);

            if (defaultList != null)
                retList.AddRange(defaultList);

            TugDataEntities db = new TugDataEntities();
            var list = db.V_BillingTemplate.Where(u => u.CustomerID == customerId).ToList();

            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item.ShipLength != null)
                    {
                        int lessValue;
                        if(true == Utils.GetShipLengthLessValue(item.ShipLength, out lessValue))
                        {
                            if (shipLength < lessValue)
                            {
                                retList.Add(item);
                            }
                        }

                        int greaterValue;
                        if(true == Utils.GetShipLengthGreaterValue(item.ShipLength, out greaterValue))
                        {
                            if (shipLength > greaterValue)
                            {
                                retList.Add(item);
                            }
                        }

                        int beginValue, endValue;
                        if (true == Utils.GetShipLengthBetweenValue(item.ShipLength, out beginValue, out endValue))
                        {
                            if (shipLength >= beginValue && shipLength <= endValue)
                            {
                                retList.Add(item);
                            }
                        }

                    }
                }
            }

            retList = retList.OrderBy(u => u.BillingTemplateName).ToList();

            return retList; 
        }


        /// <summary>
        /// 获取用户的计费方案，根据箱量
        /// </summary>
        /// <param name="customerId">客户ID</param>
        /// <param name="shipTEUS">箱量</param>
        /// <returns></returns>
        static public List<V_BillingTemplate> GetCustomersBillingTemplateByTEUS(int customerId, int shipTEUS)
        {
            List<V_BillingTemplate> retList = new List<V_BillingTemplate>();

            List<V_BillingTemplate> defaultList = GetCustomerBillSchemes(-1);

            if (defaultList != null)
                retList.AddRange(defaultList);


            TugDataEntities db = new TugDataEntities();
            var list = db.V_BillingTemplate.Where(u => u.CustomerID == customerId).ToList();

            if (list != null)
            {
                foreach (var item in list)
                {

                    if (item.ShipTEUS != null)
                    {
                        int lessValue;
                        if (true == Utils.GetShipTEUSLessValue(item.ShipTEUS, out lessValue))
                        {
                            if (shipTEUS < lessValue)
                            {
                                retList.Add(item);
                            }
                        }

                        int greaterValue;
                        if (true == Utils.GetShipTEUSGreaterValue(item.ShipTEUS, out greaterValue))
                        {
                            if (shipTEUS > greaterValue)
                            {
                                retList.Add(item);
                            }
                        }

                        int beginValue, endValue;
                        if (true == Utils.GetShipTEUSBetweenValue(item.ShipTEUS, out beginValue, out endValue))
                        {
                            if (shipTEUS >= beginValue && shipTEUS <= endValue)
                            {
                                retList.Add(item);
                            }
                        }
                    }
                }
            }

            retList = retList.OrderBy(u => u.BillingTemplateName).ToList();

            return retList;
        }


        /// <summary>
        /// 获取用户的计费方案，根据船长和箱量
        /// </summary>
        /// <param name="customerId">客户ID</param>
        /// <param name="shipLength">船长</param>
        /// <param name="shipTEUS">箱量</param>
        /// <returns></returns>
        static public List<V_BillingTemplate> GetCustomersBillingTemplateByLengthAndTEUS(int customerId, int shipLength, int shipTEUS)
        {
            List<V_BillingTemplate> retList = new List<V_BillingTemplate>();

            List<V_BillingTemplate> defaultList = GetCustomerBillSchemes(-1);

            if(defaultList != null)
                retList.AddRange(defaultList);

            TugDataEntities db = new TugDataEntities();
            var list = db.V_BillingTemplate.Where(u => u.CustomerID == customerId).ToList();

            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item.ShipLength != null)
                    {
                        int lessValue;
                        if (true == Utils.GetShipLengthLessValue(item.ShipLength, out lessValue))
                        {
                            if (shipLength <= lessValue)
                            {
                                //retList.Add(item);

                                if (item.ShipTEUS != null)
                                {
                                    int lessValue2;
                                    if (true == Utils.GetShipTEUSLessValue(item.ShipTEUS, out lessValue2))
                                    {
                                        if (shipTEUS <= lessValue2)
                                        {
                                            retList.Add(item);
                                        }
                                    }

                                    int greaterValue2;
                                    if (true == Utils.GetShipTEUSGreaterValue(item.ShipTEUS, out greaterValue2))
                                    {
                                        if (shipTEUS >= greaterValue2)
                                        {
                                            retList.Add(item);
                                        }
                                    }

                                    int beginValue2, endValue2;
                                    if (true == Utils.GetShipTEUSBetweenValue(item.ShipTEUS, out beginValue2, out endValue2))
                                    {
                                        if (shipTEUS >= beginValue2 && shipTEUS <= endValue2)
                                        {
                                            retList.Add(item);
                                        }
                                    }
                                }

                            }
                        }

                        int greaterValue;
                        if (true == Utils.GetShipLengthGreaterValue(item.ShipLength, out greaterValue))
                        {
                            if (shipLength >= greaterValue)
                            {
                                //retList.Add(item);
                                if (item.ShipTEUS != null)
                                {
                                    int lessValue2;
                                    if (true == Utils.GetShipTEUSLessValue(item.ShipTEUS, out lessValue2))
                                    {
                                        if (shipTEUS <= lessValue2)
                                        {
                                            retList.Add(item);
                                        }
                                    }

                                    int greaterValue2;
                                    if (true == Utils.GetShipTEUSGreaterValue(item.ShipTEUS, out greaterValue2))
                                    {
                                        if (shipTEUS >= greaterValue2)
                                        {
                                            retList.Add(item);
                                        }
                                    }

                                    int beginValue2, endValue2;
                                    if (true == Utils.GetShipTEUSBetweenValue(item.ShipTEUS, out beginValue2, out endValue2))
                                    {
                                        if (shipTEUS >= beginValue2 && shipTEUS <= endValue2)
                                        {
                                            retList.Add(item);
                                        }
                                    }
                                }
                            }
                        }

                        int beginValue, endValue;
                        if (true == Utils.GetShipLengthBetweenValue(item.ShipLength, out beginValue, out endValue))
                        {
                            if (shipLength >= beginValue && shipLength <= endValue)
                            {
                                //retList.Add(item);
                                if (item.ShipTEUS != null)
                                {
                                    int lessValue2;
                                    if (true == Utils.GetShipTEUSLessValue(item.ShipTEUS, out lessValue2))
                                    {
                                        if (shipTEUS <= lessValue2)
                                        {
                                            retList.Add(item);
                                        }
                                    }

                                    int greaterValue2;
                                    if (true == Utils.GetShipTEUSGreaterValue(item.ShipTEUS, out greaterValue2))
                                    {
                                        if (shipTEUS >= greaterValue2)
                                        {
                                            retList.Add(item);
                                        }
                                    }

                                    int beginValue2, endValue2;
                                    if (true == Utils.GetShipTEUSBetweenValue(item.ShipTEUS, out beginValue2, out endValue2))
                                    {
                                        if (shipTEUS >= beginValue2 && shipTEUS <= endValue2)
                                        {
                                            retList.Add(item);
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
            retList = retList.OrderBy(u => u.BillingTemplateName).ToList();

            return retList;
        }


        static public void GetStatuOfBillings(string selectedBillingIDs, string billingType, out Dictionary<int, int> dicNotInFlow,
            out Dictionary<int, int> dicInFow)
        {

            Dictionary<int, int> dicNotInFlow2 = new Dictionary<int, int>();
            Dictionary<int, int> dicInFow2 = new Dictionary<int, int>();

            if (selectedBillingIDs != "")
            {
                List<string> list = selectedBillingIDs.Split(',').ToList(); //list中的每个元素的rowNo:billingId的格式
                if (list != null)
                {
                    foreach (string item in list)
                    {
                        int rowNo = Util.toint(item.Split(':')[0]);
                        int billId = Util.toint(item.Split(':')[1]);

                        string ret = GetStatusOfBilling(billId, billingType);
                        if (ret == ConstValue.HAS_INVOICE_IN_FLOW)
                        {
                            dicInFow2.Add(rowNo, billId);
                        }
                        else if (ret == ConstValue.HAS_INVOICE_NOT_IN_FLOW)
                        {
                            dicNotInFlow2.Add(rowNo, billId);
                        }
                    }
                }
            }

            dicNotInFlow = dicNotInFlow2;
            dicInFow = dicInFow2;
        }

        /// <summary>
        /// 获取一个订单的账单状态
        /// </summary>
        /// <param name="orderId">订单Id</param>
        static private string GetStatusOfBilling(int billId, string billingType)
        {
            string ret = ConstValue.HAS_INVOICE_NOT_IN_FLOW;

            TugDataEntities db = new TugDataEntities();

            if (billingType == "普通账单")
            {
                V_Billing2 ob = db.V_Billing2.FirstOrDefault(u => u.IDX == billId);
                if (ob != null)
                {
                    {
                        if (ob.Phase != 0)
                        {
                            ret = ConstValue.HAS_INVOICE_IN_FLOW;
                        }
                        else
                        {
                            ret = ConstValue.HAS_INVOICE_NOT_IN_FLOW;
                        }
                    }
                }
            }
            else if (billingType == "特殊账单")
            {
                V_Billing3 ob = db.V_Billing3.FirstOrDefault(u => u.IDX == billId);
                if (ob != null)
                {
                    {
                        if (ob.Phase != 0)
                        {
                            ret = ConstValue.HAS_INVOICE_IN_FLOW;
                        }
                        else
                        {
                            ret = ConstValue.HAS_INVOICE_NOT_IN_FLOW;
                        }
                    }
                }
            }
            else if (billingType == "优惠单")
            {
                V_Billing4 ob = db.V_Billing4.FirstOrDefault(u => u.IDX == billId);
                if (ob != null)
                {
                    {
                        if (ob.Phase != 0)
                        {
                            ret = ConstValue.HAS_INVOICE_IN_FLOW;
                        }
                        else
                        {
                            ret = ConstValue.HAS_INVOICE_NOT_IN_FLOW;
                        }
                    }
                }
            }

            return ret;
        }



        

    }
}

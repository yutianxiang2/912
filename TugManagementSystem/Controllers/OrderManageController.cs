using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataModel;
using BusinessLogic;
using BusinessLogic.Module;
using Newtonsoft.Json;
using System.Transactions;

namespace WMS.Controllers
{
    public class OrderManageController : BaseController
    {

        #region 页面Action
        [Authorize]
        public ActionResult ServiceScheduler2(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            return View();
        }

        [Authorize]
        public ActionResult ServiceStatus(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            return View();
        }


        public ActionResult GetServiceState(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();

            //string s = Request.QueryString[6];

            try
            {
                TugDataEntities db = new TugDataEntities();
                //List<V_OrderService> objs = db.V_OrderService.Where(u => u.ServiceJobStateLabel!="已完工").Select(u => u).OrderByDescending(u => u.ServiceWorkDate).OrderByDescending(u=>u.ServiceWorkTime).ToList<V_OrderService>();
                List<V_OrderService> objs = db.V_OrderService.Select(u => u).OrderByDescending(u => u.ServiceWorkDate).ThenByDescending(u => u.ServiceWorkTime).ToList<V_OrderService>();
                int totalRecordNum = objs.Count;
                if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                int pageSize = rows;
                int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                //List<V_OrderService> page_V_OrderServices = V_OrderServices.Skip((page - 1) * rows).Take(rows).OrderBy(u => u.IDX).ToList<V_OrderService>();

                var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = objs };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        public ActionResult GetUnCompleteServiceState(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();

            //string s = Request.QueryString[6];

            try
            {
                TugDataEntities db = new TugDataEntities();

                ////List<V_OrderService> objs = db.V_OrderService.Where(u => u.ServiceJobStateLabel!="已完工").Select(u => u).OrderByDescending(u => u.ServiceWorkDate).OrderByDescending(u=>u.ServiceWorkTime).ToList<V_OrderService>();
                //List<V_OrderService> objs = db.V_OrderService.Where(u => u.ServiceJobStateLabel != "已完工").Select(u => u).OrderByDescending(u => u.ServiceWorkDate).ThenByDescending(u => u.ServiceWorkTime).ToList<V_OrderService>();
                //int totalRecordNum = objs.Count;
                //if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                //int pageSize = rows;
                //int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                ////List<V_OrderService> page_V_OrderServices = V_OrderServices.Skip((page - 1) * rows).Take(rows).OrderBy(u => u.IDX).ToList<V_OrderService>();

                //var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = objs };
                //return Json(jsonData, JsonRequestBehavior.AllowGet);



                if (_search == true)
                {
                    string searchOption = Request.QueryString["filters"];
                    //List<V_OrderService> objs = db.V_OrderService.Where(u => u.ServiceJobStateLabel != "已完工").Select(u => u)
                    //    .OrderByDescending(u => u.ServiceWorkDate).ThenByDescending(u => u.ServiceWorkTime).ToList<V_OrderService>();

                    List<V_OrderService> objs = BusinessLogic.Module.OrderLogic.SearchForServiceScheduler2(sidx, sord, searchOption);

                    int totalRecordNum = objs.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_OrderService> page_orders = objs.Skip((page - 1) * rows).Take(rows).ToList<V_OrderService>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                    //return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //List<V_OrderInfor> orders = db.V_OrderInfor.Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderInfor>();
                    //List<V_OrderService> objs = db.V_OrderService.Where(u => u.ServiceJobStateLabel != "已完工")
                    //    .Select(u => u).OrderByDescending(u => u.ServiceWorkDate).ThenByDescending(u => u.ServiceWorkTime).ToList<V_OrderService>();

                    List<V_OrderService> objs = BusinessLogic.Module.OrderLogic.LoadDataForServiceScheduler2(sidx, sord);

                    int totalRecordNum = objs.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_OrderService> page_orders = objs.Skip((page - 1) * rows).Take(rows).ToList<V_OrderService>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }


        // GET: OrderManage
        [Authorize]
        public ActionResult OrderManage(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;

            ViewBag.Services = BusinessLogic.Utils.GetServices();

            ViewBag.ServiceLabels = GetServiceLabels();
            ViewBag.Locations = GetLocations();
            return View();
        }
        [HttpGet]
        public ActionResult GetOrder(string ordermark)
        {
            try
            {
                TugDataEntities db = new TugDataEntities();
                OrderInfor aOrder = db.OrderInfor.Where(u => u.UserDefinedCol1 == ordermark).FirstOrDefault();
                if (aOrder != null)
                {
                    return Json(new { CustomerID = aOrder.CustomerID, CustomerName = aOrder.CustomerName, OrdDate = aOrder.OrdDate,
                        ShipID = aOrder.ShipID, ShipName = aOrder.ShipName, LinkMan =aOrder.LinkMan,LinkPhone=aOrder.LinkPhone,
                                      LinkEmail = aOrder.LinkEmail,
                                      Remark=aOrder.Remark}, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_CODE }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public ActionResult GetOrderServiceData(string ordermark)
        {
            try
            {
                TugDataEntities db = new TugDataEntities();
                List<V_OrderService> list = db.V_OrderService.Where(u => u.UserDefinedCol1 == ordermark).OrderBy(u => u.OrderServiceID).ToList<V_OrderService>();

                List<string[]> jsonData = new List<string[]>();
                foreach (var itm in list)
                {
                    string[] sev = new string[7] { itm.ServiceNatureLabel, itm.ServiceWorkDate,itm.ServiceWorkTime, itm.BigTugNum.ToString(),itm.MiddleTugNum.ToString(),itm.SmallTugNum.ToString(),itm.ServiceWorkPlace};
                    jsonData.Add(sev);
                }

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public string GetLocations()
        {
            string[] labels =null;
            int i = 0;
            if (labels == null)
            {
                TugDataEntities db = new TugDataEntities();
                List<CustomField> list = db.CustomField.Where(u => u.CustomName == "OrderService.Location").OrderBy(u => u.CustomValue).ToList<CustomField>();
                labels = new string[list.Count];
                foreach (var itm in list)
                {
                    labels[i] = itm.CustomLabel;
                    i++;
                }
            }
            //return labels;
            return JsonConvert.SerializeObject(labels);
        }
        public string GetServiceLabels()
        {
            string[] labels =null;//
            int i = 0;
            if (labels == null)
            {
                TugDataEntities db = new TugDataEntities();
                List<CustomField> list = db.CustomField.Where(u => u.CustomName == "OrderInfor.ServiceNatureID").OrderBy(u => u.SortCode).ToList<CustomField>();
                labels = new string[list.Count];
                foreach (var itm in list)
                {
                    labels[i] = itm.CustomLabel;
                    i++;
                }
            }
            //return labels;
            return JsonConvert.SerializeObject(labels);
        }
        [Authorize]
        //GET: OrderScheduling
        public ActionResult OrderScheduling(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;

            ViewBag.JobStates = BusinessLogic.Utils.GetJobStates();
            return View();
        }

        //GET: JobInformation
        [Authorize]
        public ActionResult JobInformation(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;

            ViewBag.JobStates = BusinessLogic.Utils.GetJobStates();
            return View();
        }

        #endregion 页面Action


        #region 订单管理页面Action

        public ActionResult GetData(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();

            try
            {
                TugDataEntities db = new TugDataEntities();

                if (_search == true)
                {
                    string searchOption = Request.QueryString["filters"];
                    //List<V_OrderInfor> orders = db.V_OrderInfor.Where(u => u.IDX == -1).Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderInfor>();
                    List<V_OrderInfor> orders = BusinessLogic.Module.OrderLogic.SearchForOrderMange(sidx, sord, searchOption);

                    int totalRecordNum = orders.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_OrderInfor> page_orders = orders.Skip((page - 1) * rows).Take(rows).ToList<V_OrderInfor>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                    //return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //List<V_OrderInfor> orders = db.V_OrderInfor.Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderInfor>();
                    List<V_OrderInfor> orders = BusinessLogic.Module.OrderLogic.LoadDataForOrderManage(sidx, sord);
                    int totalRecordNum = orders.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_OrderInfor> page_orders = orders.Skip((page - 1) * rows).Take(rows).ToList<V_OrderInfor>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        public ActionResult GetDataForLoadOnce(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();

            //string s = Request.QueryString[6];

            try
            {
                TugDataEntities db = new TugDataEntities();
                List<V_OrderInfor> orders = db.V_OrderInfor.Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderInfor>();
                int totalRecordNum = orders.Count;
                if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                int pageSize = rows;
                int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                //List<OrderInfor> page_orders = orders.Skip((page - 1) * rows).Take(rows).OrderBy(u => u.IDX).ToList<OrderInfor>();

                var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = orders };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        public ActionResult AddEdit()
        {
            this.Internationalization();

            #region Add

            if (Request.Form["oper"].Equals("add"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    {
                        DataModel.OrderInfor aOrder = new OrderInfor();

                        aOrder.Code = BusinessLogic.Utils.AutoGenerateOrderSequenceNo();

                        aOrder.CreateDate = aOrder.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        aOrder.CustomerID = Util.toint(Request.Form["CustomerID"]);
                        aOrder.CustomerName = Request.Form["CustomerName"].Trim();
                        aOrder.OrdDate = Request.Form["OrdDate"].Trim();
                        //aOrder.WorkDate = Request.Form["WorkDate"].Trim();
                        //aOrder.WorkTime = Request.Form["WorkTime"].Trim();
                        //aOrder.EstimatedCompletionTime = Request.Form["EstimatedCompletionTime"].Trim();

                        aOrder.IsGuest = "否"; // Request.Form["IsGuest"].Trim();
                        aOrder.LinkMan = Request.Form["LinkMan"].Trim();
                        aOrder.LinkPhone = Request.Form["LinkPhone"].Trim();
                        aOrder.LinkEmail = Request.Form["LinkEmail"].Trim();

                        //if (Request.Form["BigTugNum"].Trim() != "")
                        //    aOrder.BigTugNum = Util.toint(Request.Form["BigTugNum"].Trim());
                        //if (Request.Form["MiddleTugNum"].Trim() != "")
                        //    aOrder.MiddleTugNum = Util.toint(Request.Form["MiddleTugNum"].Trim());
                        //if (Request.Form["SmallTugNum"].Trim() != "")
                        //    aOrder.SmallTugNum = Util.toint(Request.Form["SmallTugNum"].Trim());

                        aOrder.OwnerID = -1;
                        aOrder.Remark = Request.Form["Remark"].Trim();
                        aOrder.ShipID = Util.toint(Request.Form["ShipID"].Trim());
                        aOrder.ShipName = Request.Form["ShipName"].Trim();
                        aOrder.UserID = Session.GetDataFromSession<int>("userid"); 
                        //aOrder.WorkPlace = Request.Form["WorkPlace"].Trim();

                        //Dictionary<string, string> dic = BusinessLogic.Utils.ResolveServices(Request.Form["ServiceNatureNames"].Trim());
                        //aOrder.ServiceNatureIDS = dic["ids"];
                        //aOrder.ServiceNatureNames = dic["labels"];

                        //aOrder.WorkStateID = Util.toint(Request.Form["WorkStateID"].Trim());
                        aOrder.WorkStateID = 2; //CustomField表里面的OrderInfor.WorkStateID的IDX

                        aOrder.UserDefinedCol1 = Request.Form["UserDefinedCol1"].Trim();
                        aOrder.UserDefinedCol2 = Request.Form["UserDefinedCol2"].Trim();
                        aOrder.UserDefinedCol3 = Request.Form["UserDefinedCol3"].Trim();
                        aOrder.UserDefinedCol4 = Request.Form["UserDefinedCol4"].Trim();

                        if (Request.Form["UserDefinedCol5"].Trim() != "")
                            aOrder.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"].Trim());

                        if (Request.Form["UserDefinedCol6"].Trim() != "")
                            aOrder.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"].Trim());

                        if (Request.Form["UserDefinedCol7"].Trim() != "")
                            aOrder.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"].Trim());

                        if (Request.Form["UserDefinedCol8"].Trim() != "")
                            aOrder.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"].Trim());

                        aOrder.UserDefinedCol9 = Request.Form["UserDefinedCol9"].Trim();
                        aOrder.UserDefinedCol10 = Request.Form["UserDefinedCol10"].Trim();

                        aOrder = db.OrderInfor.Add(aOrder);
                        db.SaveChanges();

                        var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                        //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                        return Json(ret);
                    }
                }
                catch (Exception)
                {
                    var ret = new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE };
                    //Response.Write(@Resources.Common.EXCEPTION_MESSAGE);
                    return Json(ret);
                }
            }

            #endregion Add

            #region Edit

            if (Request.Form["oper"].Equals("edit"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();

                    int idx = Util.toint(Request.Form["IDX"].Trim());
                    OrderInfor aOrder = db.OrderInfor.Where(u => u.IDX == idx).FirstOrDefault();

                    if (aOrder == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        aOrder.CustomerID = Util.toint(Request.Form["CustomerID"].Trim());
                        aOrder.CustomerName = Request.Form["CustomerName"].Trim();
                        aOrder.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        //aOrder.IsGuest = Request.Form["IsGuest"].Trim();
                        aOrder.LinkMan = Request.Form["LinkMan"].Trim();
                        aOrder.LinkPhone = Request.Form["LinkPhone"].Trim();
                        aOrder.LinkEmail = Request.Form["LinkEmail"].Trim();

                        aOrder.OrdDate = Request.Form["OrdDate"].Trim();
                        //aOrder.WorkDate = Request.Form["WorkDate"].Trim();
                        //aOrder.WorkTime = Request.Form["WorkTime"].Trim();
                        //aOrder.EstimatedCompletionTime = Request.Form["EstimatedCompletionTime"].Trim();

                        aOrder.ShipID = Util.toint(Request.Form["ShipID"].Trim());
                        aOrder.ShipName = Request.Form["ShipName"].Trim();
                        //if (Request.Form["BigTugNum"].Trim() != "")
                        //    aOrder.BigTugNum = Util.toint(Request.Form["BigTugNum"].Trim());
                        //if (Request.Form["MiddleTugNum"].Trim() != "")
                        //    aOrder.MiddleTugNum = Util.toint(Request.Form["MiddleTugNum"].Trim());
                        //if (Request.Form["SmallTugNum"].Trim() != "")
                        //    aOrder.SmallTugNum = Util.toint(Request.Form["SmallTugNum"].Trim());
                        //aOrder.WorkPlace = Request.Form["WorkPlace"].Trim();

                        //Dictionary<string, string> dic = BusinessLogic.Utils.ResolveServices(Request.Form["ServiceNatureNames"].Trim());
                        //aOrder.ServiceNatureIDS = dic["ids"];
                        //aOrder.ServiceNatureNames = dic["labels"];

                        //aOrder.WorkStateID = Util.toint(Request.Form["WorkStateID"].Trim());
                        
                        aOrder.Remark = Request.Form["Remark"].Trim();

                        aOrder.UserDefinedCol1 = Request.Form["UserDefinedCol1"].Trim();
                        aOrder.UserDefinedCol2 = Request.Form["UserDefinedCol2"].Trim();
                        aOrder.UserDefinedCol3 = Request.Form["UserDefinedCol3"].Trim();
                        aOrder.UserDefinedCol4 = Request.Form["UserDefinedCol4"].Trim();

                        if (Request.Form["UserDefinedCol5"].Trim() != "")
                            aOrder.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"].Trim());

                        if (Request.Form["UserDefinedCol6"].Trim() != "")
                            aOrder.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"].Trim());

                        if (Request.Form["UserDefinedCol7"].Trim() != "")
                            aOrder.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"].Trim());

                        if (Request.Form["UserDefinedCol8"].Trim() != "")
                            aOrder.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"].Trim());

                        aOrder.UserDefinedCol9 = Request.Form["UserDefinedCol9"].Trim();
                        aOrder.UserDefinedCol10 = Request.Form["UserDefinedCol10"].Trim();

                        db.Entry(aOrder).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });
                    }
                }
                catch (Exception exp)
                {
                    return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
                }
            }

            #endregion Edit

            return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
        }
        
        public JsonResult GetInitServiceData()  //初始化服务项table
        {
            var jsonData = new[]
                     {
                         new[] {"","","","","","",""},
                    };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }    
        public ActionResult Add_EditOrder(string oper,string ordermark,int customerId, string customerName, string ordDate,
            int shipId, string shipName, string linkMan, string linkPhone, string linkEmail, string remark,List<string[]> dataListFromTable) 
        {
            DataModel.OrderInfor aOrder=null;
            this.Internationalization();

            using (System.Transactions.TransactionScope trans = new System.Transactions.TransactionScope())
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    {
                        //获取服务项
                        System.Linq.Expressions.Expression<Func<OrderService, bool>> exp = u => u.UserDefinedCol1 == ordermark;
                        List<OrderService> entityos = db.OrderService.Where(exp).ToList();

                        //先刪除該訂單下的排船信息
                        for (int i = 0; i < entityos.Count(); i++)
                        {
                            int ordsrvid = Util.toint(entityos[i].IDX);
                            System.Linq.Expressions.Expression<Func<Scheduler, bool>> exp0 = u => u.OrderServiceID == ordsrvid;
                            var entitysch = db.Scheduler.Where(exp0);
                            entitysch.ToList().ForEach(entity => db.Entry(entity).State = System.Data.Entity.EntityState.Deleted); //不加这句也可以
                            db.Scheduler.RemoveRange(entitysch);
                            db.SaveChanges();
                        }


                        //先删除订单下的所有服务项

                        entityos.ForEach(entity => db.Entry(entity).State = System.Data.Entity.EntityState.Deleted); //不加这句也可以
                        db.OrderService.RemoveRange(entityos);
                        db.SaveChanges();

                        //删除订单
                        System.Linq.Expressions.Expression<Func<OrderInfor, bool>> exp2 = u => u.UserDefinedCol1 == ordermark;
                        var entityord = db.OrderInfor.Where(exp2);
                        entityord.ToList().ForEach(entity => db.Entry(entity).State = System.Data.Entity.EntityState.Deleted); //不加这句也可以
                        db.OrderInfor.RemoveRange(entityord);
                        db.SaveChanges();

                        //保存
                        //获取服务项
                        List<CustomField> listServ;
                        listServ = BusinessLogic.Utils.GetServices();
                        string mcode = BusinessLogic.Utils.AutoGenerateOrderSequenceNo();
                        for (int i = 0; i < dataListFromTable.Count - 1; i++)//最后一行空行
                        {
                            //保存订单
                            aOrder = new OrderInfor();
                            aOrder.Code = mcode;//BusinessLogic.Utils.AutoGenerateOrderSequenceNo();
                            aOrder.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            aOrder.HasInvoice = "否"; //没有账单
                            aOrder.HasInFlow = "否"; //没有在流程中
                            aOrder.IsGuest = "否";
                            //if (oper == "add")
                            //{
                            //    aOrder = new OrderInfor();
                            //    aOrder.Code = BusinessLogic.Utils.AutoGenerateOrderSequenceNo();
                            //    aOrder.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //    aOrder.HasInvoice = "否"; //没有账单
                            //    aOrder.HasInFlow = "否"; //没有在流程中
                            //    aOrder.IsGuest = "否";

                            //}
                            //else if (oper == "edit")
                            //{
                            //    aOrder = db.OrderInfor.Where(u => u.IDX == orderId).FirstOrDefault();

                            //}
                            aOrder.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //aOrder.CustomerID = customerId;
                            aOrder.CustomerID = CustomerLogic.AutoAddCustomer("", customerName, "", "", linkMan, linkPhone, "", linkEmail, "", "", "", Session.GetDataFromSession<int>("userid"));
                            aOrder.CustomerName = customerName;
                            aOrder.OrdDate = ordDate;

                            aOrder.LinkMan = linkMan;
                            aOrder.LinkPhone = linkPhone;
                            aOrder.LinkEmail = linkEmail;

                            aOrder.OwnerID = -1;
                            aOrder.Remark = remark;
                            //aOrder.ShipID = shipId;
                            aOrder.ShipID = CustomerLogic.AutoAddCustomerShip((int)aOrder.CustomerID, shipName, "", "", "", "", "", "", "", "", Session.GetDataFromSession<int>("userid"));
                            aOrder.ShipName = shipName;
                            aOrder.UserID = Session.GetDataFromSession<int>("userid");

                            aOrder.WorkStateID = 2; //CustomField表里面的OrderInfor.WorkStateID的IDX  订单修改后订单改为未排船

                            aOrder.UserDefinedCol1 = mcode;
                            aOrder.UserDefinedCol2 = i.ToString();
                            aOrder.UserDefinedCol3 = dataListFromTable[i][0];
                            aOrder.UserDefinedCol4 = "";

                            //if (Request.Form["UserDefinedCol5"].Trim() != "")
                            //    aOrder.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"].Trim());

                            //if (Request.Form["UserDefinedCol6"].Trim() != "")
                            //    aOrder.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"].Trim());

                            //if (Request.Form["UserDefinedCol7"].Trim() != "")
                            //    aOrder.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"].Trim());

                            //if (Request.Form["UserDefinedCol8"].Trim() != "")
                            //    aOrder.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"].Trim());

                            aOrder.UserDefinedCol9 = "";
                            aOrder.UserDefinedCol10 = "";

                            aOrder = db.OrderInfor.Add(aOrder);
                            //if (oper == "add")
                            //{
                            //    aOrder = db.OrderInfor.Add(aOrder);
                            //}
                            //else if (oper == "edit")
                            //{
                            //    db.Entry(aOrder).State = System.Data.Entity.EntityState.Modified;
                            //}
                            db.SaveChanges();

                            //保存服务项
                            DataModel.OrderService obj = new OrderService();
                            obj.OrderID = aOrder.IDX;
                            string serName = dataListFromTable[i][0];
                            CustomField sv = listServ.Where(u => u.CustomLabel == serName).FirstOrDefault();
                            obj.ServiceNatureID = sv.IDX;
                            obj.ServiceWorkDate = BusinessLogic.Utils.CNDateTimeToDateTime(dataListFromTable[i][1]).ToString("yyyy-MM-dd");
                            obj.ServiceWorkTime = dataListFromTable[i][2];
                            //obj.EstimatedCompletionTime=
                            obj.ServiceWorkPlace = dataListFromTable[i][6];
                            obj.BigTugNum = Util.toint(dataListFromTable[i][3]);
                            obj.MiddleTugNum = Util.toint(dataListFromTable[i][4]);
                            obj.SmallTugNum = Util.toint(dataListFromTable[i][5]);
                            //obj.Remark = "";
                            obj.JobStateID = 114;

                            obj.HasBilling = "否";
                            obj.HasBillingInFlow = "否";

                            obj.OwnerID = -1;
                            obj.CreateDate = aOrder.CreateDate;
                            obj.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;
                            obj.UserID = Session.GetDataFromSession<int>("userid");

                            obj.UserDefinedCol1 = mcode;
                            //obj.UserDefinedCol2 = "";
                            //obj.UserDefinedCol3 = "";
                            //obj.UserDefinedCol4 = "";
                            //if (Request.Form["UserDefinedCol5"] != "")
                            //    obj.UserDefinedCol5 = Util.toint(Request.Form["UserDefinedCol5"]);
                            //obj.UserDefinedCol6 =;
                            //obj.UserDefinedCol7 =;
                            //obj.UserDefinedCol8 =;
                            //obj.UserDefinedCol9 = "";
                            //obj.UserDefinedCol10 = "";
                            obj = db.OrderService.Add(obj);
                            db.SaveChanges();
                        }
                        trans.Complete();

                        var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                        //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                        return Json(ret);
                    }

                }
                catch (Exception ex)
                {
                    trans.Dispose();

                    var ret = new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE };
                    //Response.Write(@Resources.Common.EXCEPTION_MESSAGE);
                    return Json(ret);
                }
            }
        }

        //判断订单下的服务是否有已完工
        public ActionResult IsWanGong()
        {
            var f = Request.Form;
            bool IsWanGong = false;
            //int idx = Util.toint(Request.Form["orderId"]);
            string oderMark = Request.Form["ordermark"];

            TugDataEntities db = new TugDataEntities();
            //System.Linq.Expressions.Expression<Func<V_OrderService_Scheduler, bool>> exps = u => u.OrderID == idx;
            System.Linq.Expressions.Expression<Func<V_OrderService, bool>> exps = u => u.UserDefinedCol1 == oderMark & (u.ServiceJobStateLabel == "已完工" || u.ServiceJobStateValue=="2");
            List<V_OrderService> objs = db.V_OrderService.Where(exps).Select(u => u).ToList<V_OrderService>();
            if (objs.Count != 0)
            {
                IsWanGong = true;  //已有排船信息
                return Json(new { code = Resources.Common.SUCCESS_CODE, message = IsWanGong });
            }
            else
            {
                return Json(new { code = Resources.Common.SUCCESS_CODE, message = IsWanGong });
            }
        }
        public ActionResult IsScheduler()
        {
            var f = Request.Form;
            bool IsScheduler = false;
            //int idx = Util.toint(Request.Form["orderId"]);
            string oderMark = Request.Form["ordermark"];

            TugDataEntities db = new TugDataEntities();
            //System.Linq.Expressions.Expression<Func<V_OrderService_Scheduler, bool>> exps = u => u.OrderID == idx;
            System.Linq.Expressions.Expression<Func<V_OrderService_Scheduler, bool>> exps = u => u.UserDefinedCol1 == oderMark;
            List<V_OrderService_Scheduler> schedulerInfor = db.V_OrderService_Scheduler.Where(exps).Select(u => u).ToList<V_OrderService_Scheduler>();
            if (schedulerInfor.Count != 0)
            {
                IsScheduler = true;  //已有排船信息
                return Json(new { code = Resources.Common.SUCCESS_CODE, message = IsScheduler });
            }
            else
            {
                return Json(new { code = Resources.Common.SUCCESS_CODE, message = IsScheduler });
            }
        }

        public ActionResult Delete()
        {
            this.Internationalization();
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var f = Request.Form;

                    string ordermark = Util.checkdbnull(Request.Form["data[UserDefinedCol1]"]);

                    TugDataEntities db = new TugDataEntities();
                    //System.Linq.Expressions.Expression<Func<V_OrderService_Scheduler, bool>> exps = u => u.UserDefinedCol1 == ordermark;
                    //List<V_OrderService_Scheduler> schedulerInfor = db.V_OrderService_Scheduler.Where(exps).Select(u => u).ToList<V_OrderService_Scheduler>();
                    //if (schedulerInfor.Count != 0)
                    //{
                    //    return Json(new { code = Resources.Common.SUCCESS_CODE, message = "該訂單已排船，無法刪除！"});
                    //}
                    System.Linq.Expressions.Expression<Func<V_OrderService, bool>> exps = u => u.UserDefinedCol1 == ordermark & (u.ServiceJobStateLabel == "已完工" || u.ServiceJobStateValue == "2");
                    List<V_OrderService> objs = db.V_OrderService.Where(exps).Select(u => u).ToList<V_OrderService>();
                    if (objs.Count != 0)
                    {
                        return Json(new { code = Resources.Common.SUCCESS_CODE, message = "該訂單的服务项已完工，無法刪除，如需刪除請先聯繫財務進行駁回操作!！" });//已完工
                    }
                    //获取服务项
                    System.Linq.Expressions.Expression<Func<OrderService, bool>> exp = u => u.UserDefinedCol1 == ordermark;
                    List<OrderService> entityos = db.OrderService.Where(exp).ToList();

                    //先刪除該訂單下的排船信息
                    for (int i = 0; i < entityos.Count(); i++)
                    {
                        int ordsrvid=Util.toint(entityos[i].IDX);
                        System.Linq.Expressions.Expression<Func<Scheduler, bool>> exp0 = u => u.OrderServiceID == ordsrvid;
                        var entitysch = db.Scheduler.Where(exp0);
                        entitysch.ToList().ForEach(entity => db.Entry(entity).State = System.Data.Entity.EntityState.Deleted); //不加这句也可以
                        db.Scheduler.RemoveRange(entitysch);
                        db.SaveChanges();
                    }


                    //先删除订单下的所有服务项

                    entityos.ForEach(entity => db.Entry(entity).State = System.Data.Entity.EntityState.Deleted); //不加这句也可以
                    db.OrderService.RemoveRange(entityos);
                    db.SaveChanges();

                    //删除订单
                    System.Linq.Expressions.Expression<Func<OrderInfor, bool>> exp2 = u => u.UserDefinedCol1 == ordermark;
                    var entityord = db.OrderInfor.Where(exp2);
                    entityord.ToList().ForEach(entity => db.Entry(entity).State = System.Data.Entity.EntityState.Deleted); //不加这句也可以
                    db.OrderInfor.RemoveRange(entityord);
                    db.SaveChanges();

                    OrderInfor aOrder = db.OrderInfor.FirstOrDefault(u => u.UserDefinedCol1 == ordermark);
                    trans.Complete();
                    return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });

                }
                catch (Exception)
                {
                    trans.Dispose();
                    return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
                }
            }
        }

        public ActionResult GetCustomer(string term)
         {
            TugDataEntities db = new TugDataEntities();
            List<Customer> customers = db.Customer.Where(u => (u.Name1.ToLower().Trim().Contains(term.Trim().ToLower())) 
                || u.Code.ToLower().Trim().Contains(term.Trim().ToLower()) 
                || u.SimpleName.ToLower().Trim().Contains(term.Trim().ToLower()))
                .Select(u => u).OrderBy(u => u.Name1).ToList<Customer>();

            List<object> list = new List<object>();

            if (customers != null)
            {
                foreach (Customer item in customers)
                {
                    list.Add(new { CustomerID = item.IDX, CustomerName1 = item.Name1, ContactPerson = item.ContactPerson, Telephone = item.Telephone, Email = item.Email});
                }
            }

            var jsonData = new { list = list };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetServiceLocations(string term)
        {
            TugDataEntities db = new TugDataEntities();
            List<CustomField> customers = db.CustomField.Where(u => u.CustomName == "OrderService.Location" && u.CustomLabel.ToLower().Trim().Contains(term.Trim().ToLower()))
                .Select(u => u).OrderBy(u => u.CustomLabel).ToList<CustomField>();

            List<object> list = new List<object>();

            if (customers != null)
            {
                foreach (CustomField item in customers)
                {
                    list.Add(new { CustomerID = item.IDX, CustomerName1 = item.CustomLabel });
                }
            }

            var jsonData = new { list = list };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetCustomerShips(string term, int customerId)
        {
            TugDataEntities db = new TugDataEntities();
            List<CustomerShip> ships = db.CustomerShip.Where(u => u.CustomerID == customerId && u.Name1.ToLower().Trim().Contains(term.Trim().ToLower()))
                .Select(u => u).OrderBy(u => u.Name1).ToList<CustomerShip>();

            List<object> list = new List<object>();

            if (ships != null)
            {
                foreach (CustomerShip item in ships)
                {
                    list.Add(new { ShipID = item.IDX, ShipName1 = item.Name1 });
                }
            }

            var jsonData = new { list = list };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获得拖轮的服务项
        /// </summary>
        /// <returns></returns>
        public ActionResult GetServices()
        {
            TugDataEntities db = new TugDataEntities();
            List<CustomField> list = db.CustomField.Where(u => u.CustomName == "OrderInfor.ServiceNatureID").OrderBy(u => u.CustomValue).ToList<CustomField>();
            var jsonData = new { list = list };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //public string GetCustomField(string CustomName)
        //{
        //    string s = string.Empty;

        //    try
        //    {
        //        TugDataEntities db = new TugDataEntities();
        //        List<CustomField> list = db.CustomField.Where(u => u.CustomName == CustomName).OrderBy(u => u.CustomValue).ToList<CustomField>();
        //        if (list != null && list.Count > 0)
        //        {
        //            s += "<select>";
        //            foreach (CustomField item in list)
        //            {
        //                s += string.Format("<option value={0}>{1}</option>", item.CustomValue + ":" + item.CustomLabel, item.CustomLabel);
        //            }

        //            s += "</select>";
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //    }

        //    return s;
        //}

        #endregion 订单管理页面Action


        #region 订单调度页面Action

        public ActionResult GetDataOfOrderScheduling(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();

            try
            {
                TugDataEntities db = new TugDataEntities();

                if (_search == true)
                {
                    string searchOption = Request.QueryString["filters"];
                    //List<V_OrderInfor> orders = db.V_OrderInfor.Where(u => u.IDX == -1).Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderInfor>();
                    List<V_OrderInfor> orders = BusinessLogic.Module.OrderLogic.SearchForOrderMange(sidx, sord, searchOption);

                    int totalRecordNum = orders.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_OrderInfor> page_orders = orders.Skip((page - 1) * rows).Take(rows).ToList<V_OrderInfor>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                    //return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //List<V_OrderInfor> orders = db.V_OrderInfor.Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderInfor>();
                    List<V_OrderInfor> orders = BusinessLogic.Module.OrderLogic.LoadDataForOrderScheduling(sidx, sord);
                    int totalRecordNum = orders.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_OrderInfor> page_orders = orders.Skip((page - 1) * rows).Take(rows).ToList<V_OrderInfor>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        public ActionResult GetOrderSubSchedulerData(bool _search, string sidx, string sord, int page, int rows, int orderId)
        {
            this.Internationalization();

            try
            {
                //
                TugDataEntities db = new TugDataEntities();

                if (_search == true)
                {
                    string s = Request.QueryString["filters"];
                    return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //List<V_OrderScheduler> orders = db.V_OrderScheduler.Where(u => u.OrderID == orderId).Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderScheduler>();
                    List<V_OrderScheduler> orders = BusinessLogic.Module.OrderLogic.LoadDataForOrderScheduler(sidx, sord, orderId);
                    int totalRecordNum = orders.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_OrderScheduler> page_orders = orders.Skip((page - 1) * rows).Take(rows).ToList<V_OrderScheduler>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetOrderSubSchedulerDataForLoadOnce(bool _search, string sidx, string sord, int page, int rows, int orderId)
        {
            this.Internationalization();

            try
            {
                TugDataEntities db = new TugDataEntities();

                if (_search == true)
                {
                    string s = Request.QueryString["filters"];
                    return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<V_OrderScheduler> orders = db.V_OrderScheduler.Where(u => u.OrderID == orderId).Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderScheduler>();

                    int totalRecordNum = orders.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTugsByName1(string value)
        {
            TugDataEntities db = new TugDataEntities();
            List<TugInfor> source = db.TugInfor.Where(u => u.Name1.Contains(value))
                .OrderBy(u => u.Name1).ToList<TugInfor>();

            var jsonData = new { list = source };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTugsByName2(string value)
        {
            TugDataEntities db = new TugDataEntities();
            List<TugInfor> source = db.TugInfor.Where(u => u.Name2.Contains(value))
                .OrderBy(u => u.Name1).ToList<TugInfor>();

            var jsonData = new { list = source };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTugsBySimpleName(string value)
        {
            TugDataEntities db = new TugDataEntities();
            List<TugInfor> source = db.TugInfor.Where(u => u.SimpleName.Contains(value))
                .OrderBy(u => u.Name1).ToList<TugInfor>();

            var jsonData = new { list = source };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddEditScheduler()
        {
            this.Internationalization();

            #region Add

            if (Request.Form["oper"].Equals("add"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    {
                        DataModel.Scheduler aScheduler = new Scheduler();
                        aScheduler.ArrivalBaseTime = Request.Form["ArrivalBaseTime"].Trim();
                        aScheduler.ArrivalShipSideTime = Request.Form["ArrivalShipSideTime"].Trim();
                        aScheduler.CaptainConfirmTime = Request.Form["CaptainConfirmTime"].Trim();
                        aScheduler.DepartBaseTime = Request.Form["DepartBaseTime"].Trim();
                        aScheduler.InformCaptainTime = Request.Form["InformCaptainTime"].Trim();
                        aScheduler.WorkCommencedTime = Request.Form["WorkCommencedTime"].Trim();
                        aScheduler.WorkCompletedTime = Request.Form["WorkCompletedTime"].Trim();

                        aScheduler.JobStateID = Util.toint(Request.Form["JobStateID"].Trim()); ;

                        //aScheduler.OrderID = Util.toint(Request.Form["OrderID"].Trim());
                        aScheduler.OwnerID = -1;
                        aScheduler.UserID = Session.GetDataFromSession<int>("userid"); 
                        aScheduler.Remark = Request.Form["Remark"].Trim(); ;

                        //aScheduler.RopeUsed = Request.Form["RopeUsed"].Trim();
                        //if (aScheduler.RopeUsed.Equals("是"))
                        //    aScheduler.RopeNum = Util.toint(Request.Form["RopeNum"].Trim());
                        //else
                        //    aScheduler.RopeNum = 0;

                        //aScheduler.ServiceNatureID = Util.toint(Request.Form["ServiceNatureLabel"].Trim().Split('~')[0]);
                        aScheduler.TugID = Util.toint(Request.Form["TugID"].Trim());
                        aScheduler.CreateDate = aScheduler.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        aScheduler.UserDefinedCol1 = Request.Form["UserDefinedCol1"].Trim();
                        aScheduler.UserDefinedCol2 = Request.Form["UserDefinedCol2"].Trim();
                        aScheduler.UserDefinedCol3 = Request.Form["UserDefinedCol3"].Trim();
                        aScheduler.UserDefinedCol4 = Request.Form["UserDefinedCol4"].Trim();

                        if (Request.Form["UserDefinedCol5"].Trim() != "")
                            aScheduler.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"].Trim());

                        if (Request.Form["UserDefinedCol6"].Trim() != "")
                            aScheduler.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"].Trim());

                        if (Request.Form["UserDefinedCol7"].Trim() != "")
                            aScheduler.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"].Trim());

                        if (Request.Form["UserDefinedCol8"].Trim() != "")
                            aScheduler.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"].Trim());

                        aScheduler.UserDefinedCol9 = Request.Form["UserDefinedCol9"].Trim();
                        aScheduler.UserDefinedCol10 = Request.Form["UserDefinedCol10"].Trim();

                        aScheduler = db.Scheduler.Add(aScheduler);
                        db.SaveChanges();

                        {
                            //更新订单状态
                            //OrderInfor tmpOrder = db.OrderInfor.Where(u => u.IDX == aScheduler.OrderID).FirstOrDefault();
                            //if(tmpOrder != null)
                            //{
                            //    tmpOrder.WorkStateID = 3; //已排船
                            //    db.Entry(tmpOrder).State = System.Data.Entity.EntityState.Modified;
                            //    db.SaveChanges();
                            //}
                        }

                        {
                            //OrderService os = db.OrderService.Where(u => u.OrderID == aScheduler.OrderID && u.ServiceNatureID == aScheduler.ServiceNatureID).FirstOrDefault();
                            //if (os == null)
                            //{
                            //    os = new OrderService();
                            //    os.CreateDate = os.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //    os.OrderID = aScheduler.OrderID;
                            //    os.OwnerID = -1;
                            //    os.ServiceNatureID = aScheduler.ServiceNatureID;
                            //    os.ServiceWorkDate = Request.Form["ServiceWorkDate"].Trim(); 
                            //    os.ServiceWorkPlace = Request.Form["ServiceWorkPlace"].Trim(); 
                            //    os.UserID = Session.GetDataFromSession<int>("userid");
                            //    os = db.OrderService.Add(os);
                            //    db.SaveChanges();
                            //}
                            //else
                            //{
                            //    os.ServiceWorkDate = Request.Form["ServiceWorkDate"].Trim(); 
                            //    os.ServiceWorkPlace = Request.Form["ServiceWorkPlace"].Trim(); 
                            //    os.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //    db.Entry(os).State = System.Data.Entity.EntityState.Modified;
                            //    db.SaveChanges();
                            //}
                        }

                        var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                        //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                        return Json(ret);
                    }
                }
                catch (Exception)
                {
                    var ret = new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE };
                    //Response.Write(@Resources.Common.EXCEPTION_MESSAGE);
                    return Json(ret);
                }
            }

            #endregion Add

            #region Edit

            if (Request.Form["oper"].Equals("edit"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();

                    int idx = Util.toint(Request.Form["IDX"].Trim());
                    Scheduler aScheduler = db.Scheduler.Where(u => u.IDX == idx).FirstOrDefault();

                    if (aScheduler == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        //aScheduler.ArrivalBaseTime = Request.Form["ArrivalBaseTime"].Trim();
                        //aScheduler.ArrivalShipSideTime = Request.Form["ArrivalShipSideTime"].Trim();
                        //aScheduler.CaptainConfirmTime = Request.Form["CaptainConfirmTime"].Trim();
                        //aScheduler.DepartBaseTime = Request.Form["DepartBaseTime"].Trim();
                        //aScheduler.InformCaptainTime = Request.Form["InformCaptainTime"].Trim();
                        //aScheduler.WorkCommencedTime = Request.Form["WorkCommencedTime"].Trim();
                        //aScheduler.WorkCompletedTime = Request.Form["WorkCompletedTime"].Trim();

                        //aScheduler.JobStateID = Util.toint(Request.Form["JobStateID"].Trim()); ;

                        //aScheduler.OrderID = Util.toint(Request.Form["OrderID"].Trim());
                        aScheduler.OwnerID = -1;
                        aScheduler.UserID = Session.GetDataFromSession<int>("userid");
                        aScheduler.Remark = Request.Form["Remark"].Trim();
                        aScheduler.IsCaptainConfirm = Request.Form["IsCaptainConfirm"].Trim(); 

                        //aScheduler.RopeUsed = Request.Form["RopeUsed"].Trim();
                        //if (aScheduler.RopeUsed.Equals("是"))
                        //    aScheduler.RopeNum = Util.toint(Request.Form["RopeNum"].Trim());
                        //else
                        //    aScheduler.RopeNum = 0;

                        //aScheduler.ServiceNatureID = Util.toint(Request.Form["ServiceNatureLabel"].Trim().Split('~')[0]);

                        //aScheduler.TugID = Util.toint(Request.Form["TugID"].Trim());
                        aScheduler.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        //aScheduler.UserDefinedCol1 = Request.Form["UserDefinedCol1"].Trim();
                        //aScheduler.UserDefinedCol2 = Request.Form["UserDefinedCol2"].Trim();
                        //aScheduler.UserDefinedCol3 = Request.Form["UserDefinedCol3"].Trim();
                        //aScheduler.UserDefinedCol4 = Request.Form["UserDefinedCol4"].Trim();

                        //if (Request.Form["UserDefinedCol5"].Trim() != "")
                        //    aScheduler.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"].Trim());

                        //if (Request.Form["UserDefinedCol6"].Trim() != "")
                        //    aScheduler.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"].Trim());

                        //if (Request.Form["UserDefinedCol7"].Trim() != "")
                        //    aScheduler.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"].Trim());

                        //if (Request.Form["UserDefinedCol8"].Trim() != "")
                        //    aScheduler.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"].Trim());

                        //aScheduler.UserDefinedCol9 = Request.Form["UserDefinedCol9"].Trim();
                        //aScheduler.UserDefinedCol10 = Request.Form["UserDefinedCol10"].Trim();

                        db.Entry(aScheduler).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        {
                            //OrderService os = db.OrderService.Where(u => u.OrderID == aScheduler.OrderID && u.ServiceNatureID == aScheduler.ServiceNatureID).FirstOrDefault();
                            //if (os == null)
                            //{
                            //    os = new OrderService();
                            //    os.CreateDate = os.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //    os.OrderID = aScheduler.OrderID;
                            //    os.OwnerID = -1;
                            //    os.ServiceNatureID = aScheduler.ServiceNatureID;
                            //    os.ServiceWorkDate = Request.Form["ServiceWorkDate"].Trim();
                            //    os.ServiceWorkPlace = Request.Form["ServiceWorkPlace"].Trim();
                            //    os.UserID = Session.GetDataFromSession<int>("userid");
                            //    os = db.OrderService.Add(os);
                            //    db.SaveChanges();
                            //}
                            //else
                            //{
                            //    os.ServiceWorkDate = Request.Form["ServiceWorkDate"].Trim();
                            //    os.ServiceWorkPlace = Request.Form["ServiceWorkPlace"].Trim();
                            //    os.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //    db.Entry(os).State = System.Data.Entity.EntityState.Modified;
                            //    db.SaveChanges();
                            //}
                        }

                        return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });
                    }
                }
                catch (Exception exp)
                {
                    return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
                }
            }

            #endregion Edit

            return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
        }


        /// <summary>
        /// 获取订单的服务项
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        public ActionResult GetOrderServices(int orderId)
        {
            try
            {
                TugDataEntities db = new TugDataEntities();

                List<MyOrderService> orderServices = db.V_OrderService.Where(u => u.OrderID == orderId).OrderBy(u => u.OrderServiceID)
                    .Select(u => new MyOrderService
                    {
                        OrderServiceId = u.OrderServiceID,
                        ServiceNatureId = (int)u.ServiceNatureID,
                        ServiceNatureValue = u.ServiceNatureValue,
                        ServiceNatureLabel = u.ServiceNatureLabel,
                        ServiceWorkDate = u.ServiceWorkDate,
                        ServiceWorkTime = u.ServiceWorkTime,
                        ServiceEstimatedCompletionTime = u.EstimatedCompletionTime,
                        ServiceWorkPlace = u.ServiceWorkPlace
                    }).ToList<MyOrderService>();

                var jsonData = new { services = orderServices };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
            }
        }

        public ActionResult AddScheduler(int orderId, int orderServiceId, int serviceNatureId, string serviceWorkDate, string serviceWorkTime, string serviceWorkPlace, string tugId,
            string isCaptainConfirm, string informCaptainTime, string captainConfirmTime,  string ropeUsed, int ropeNum, string remark)
        {
            this.Internationalization();

            try
            {
                TugDataEntities db = new TugDataEntities();
                {
                    //插入多个调度到数据库
                    {
                        List<string> lstTugIds = tugId.Split(',').ToList();
                        if (lstTugIds != null && lstTugIds.Count > 0)
                        {
                            List<Scheduler> lstSchedulers = new List<Scheduler>();
                            foreach (string item in lstTugIds)
                            {
                                DataModel.Scheduler aScheduler = new Scheduler();

                                //aScheduler.OrderID = orderId;
                                //aScheduler.ServiceNatureID = serviceNatureId;
                                aScheduler.OrderServiceID = orderServiceId;

                                aScheduler.TugID = Util.toint(item);

                                aScheduler.RopeUsed = ropeUsed;
                                aScheduler.RopeNum = ropeNum;
                                aScheduler.Remark = remark;

                                aScheduler.IsCaptainConfirm = isCaptainConfirm;

                                aScheduler.InformCaptainTime = informCaptainTime;
                                aScheduler.CaptainConfirmTime = captainConfirmTime;
                                aScheduler.JobStateID = 32;

                                aScheduler.OwnerID = -1;
                                aScheduler.UserID = Session.GetDataFromSession<int>("userid");

                                aScheduler.CreateDate = aScheduler.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                
                                aScheduler.UserDefinedCol1 = "";
                                aScheduler.UserDefinedCol2 = "";
                                aScheduler.UserDefinedCol3 = "";
                                aScheduler.UserDefinedCol4 = "";

                                //if (Request.Form["UserDefinedCol5"].Trim() != "")
                                //    aScheduler.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"].Trim());

                                //if (Request.Form["UserDefinedCol6"].Trim() != "")
                                //    aScheduler.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"].Trim());

                                //if (Request.Form["UserDefinedCol7"].Trim() != "")
                                //    aScheduler.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"].Trim());

                                //if (Request.Form["UserDefinedCol8"].Trim() != "")
                                //    aScheduler.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"].Trim());

                                aScheduler.UserDefinedCol9 = "";
                                aScheduler.UserDefinedCol10 = "";

                                lstSchedulers.Add(aScheduler);
                            }

                            db.Scheduler.AddRange(lstSchedulers);
                            db.SaveChanges();
                        }
                    }


                    {
                        //更新订单状态
                        //OrderInfor tmpOrder = db.OrderInfor.Where(u => u.IDX == orderId).FirstOrDefault();
                        //if (tmpOrder != null)
                        //{
                        //    tmpOrder.WorkStateID = 3; //已排船
                        //    db.Entry(tmpOrder).State = System.Data.Entity.EntityState.Modified;
                        //    db.SaveChanges();
                        //}
                    }

                    //更新服务状态
                    {
                        OrderService os = db.OrderService.Where(u => u.IDX == orderServiceId).FirstOrDefault();
                        if (os == null)
                        {
                            os = new OrderService();
                            os.CreateDate = os.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            os.OrderID = orderId;
                            os.OwnerID = -1;
                            os.ServiceNatureID = serviceNatureId;
                            os.ServiceWorkDate = serviceWorkDate;
                            os.ServiceWorkTime = serviceWorkTime;
                            os.ServiceWorkPlace = serviceWorkPlace;
                            os.JobStateID = 115;
                            os.UserID = Session.GetDataFromSession<int>("userid");
                            os = db.OrderService.Add(os);
                            db.SaveChanges();
                        }
                        else
                        {
                            os.ServiceWorkDate = serviceWorkDate;
                            os.ServiceWorkTime = serviceWorkTime;
                            os.ServiceWorkPlace = serviceWorkPlace;
                            os.JobStateID = 115;
                            os.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            db.Entry(os).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                        }
                    }

                    var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                    //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                    return Json(ret);
                }
            }
            catch (Exception)
            {
                var ret = new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE };
                //Response.Write(@Resources.Common.EXCEPTION_MESSAGE);
                return Json(ret);
            }
        }



        public ActionResult AddScheduler2(int orderServiceId, string tugIds, string tugNames)
        {
            this.Internationalization();

            try
            {
                TugDataEntities db = new TugDataEntities();
                {
                    //1.先删除原有的调度
                    var oldSchedulers = db.Scheduler.Where(u => u.OrderServiceID == orderServiceId).ToList();
                    if (oldSchedulers != null)
                    {
                        db.Scheduler.RemoveRange(oldSchedulers);
                        if (0 < db.SaveChanges())
                        {
                            OrderService os = db.OrderService.FirstOrDefault(u => u.IDX == orderServiceId);
                            os.JobStateID = 114;
                            os.UserDefinedCol9 = tugIds;
                            os.UserDefinedCol10 = tugNames;
                            db.Entry(os).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    //2.插入新更改的调度
                    #region 插入多个调度到数据库
                    {
                        if (tugIds.Trim() != "")
                        {
                            List<string> lstTugIds = tugIds.Split(',').ToList();
                            if (lstTugIds != null && lstTugIds.Count > 0)
                            {
                                List<Scheduler> lstSchedulers = new List<Scheduler>();
                                foreach (string item in lstTugIds)
                                {
                                    DataModel.Scheduler aScheduler = new Scheduler();

                                    aScheduler.OrderServiceID = orderServiceId;

                                    aScheduler.TugID = Util.toint(item);

                                    aScheduler.RopeUsed = "";
                                    aScheduler.RopeNum = 0;
                                    aScheduler.Remark = "";

                                    aScheduler.IsCaptainConfirm = "";

                                    aScheduler.InformCaptainTime = "";
                                    aScheduler.CaptainConfirmTime = "";
                                    aScheduler.JobStateID = 32;

                                    aScheduler.OwnerID = -1;
                                    aScheduler.UserID = Session.GetDataFromSession<int>("userid");

                                    aScheduler.CreateDate = aScheduler.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                    aScheduler.UserDefinedCol1 = "";
                                    aScheduler.UserDefinedCol2 = "";
                                    aScheduler.UserDefinedCol3 = "";
                                    aScheduler.UserDefinedCol4 = "";

                                    aScheduler.UserDefinedCol9 = "";
                                    aScheduler.UserDefinedCol10 = "";

                                    lstSchedulers.Add(aScheduler);
                                }

                                db.Scheduler.AddRange(lstSchedulers);
                                if (0 < db.SaveChanges())
                                {
                                    OrderService os = db.OrderService.FirstOrDefault(u => u.IDX == orderServiceId);
                                    os.JobStateID = 115;
                                    os.UserDefinedCol9 = tugIds;
                                    os.UserDefinedCol10 = tugNames;
                                    db.Entry(os).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    #endregion

                    var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                    //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                    return Json(ret);
                }
            }
            catch (Exception)
            {
                var ret = new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE };
                //Response.Write(@Resources.Common.EXCEPTION_MESSAGE);
                return Json(ret);
            }
        }


        public ActionResult EditScheduler(int orderId, int serviceNatureId, string serviceWorkDate, string serviceWorkPlace, int schedulerId, int oldTugId, string newTugIds,
            string informCaptainTime, string captainConfirmTime, int jobStateId, string ropeUsed, int ropeNum, string remark)
        {
            this.Internationalization();

            try
            {
                TugDataEntities db = new TugDataEntities();
                {
                    #region 修改调度时，没有重新换过拖轮
                    if (newTugIds == "-1")
                    {
                        Scheduler aScheduler = db.Scheduler.Where(u => u.IDX == schedulerId).FirstOrDefault();
                        if(aScheduler != null)
                        {

                            //aScheduler.OrderID = orderId;
                            //aScheduler.ServiceNatureID = serviceNatureId;
                            aScheduler.TugID = oldTugId;
                            aScheduler.JobStateID = jobStateId;
                            aScheduler.InformCaptainTime = informCaptainTime;
                            aScheduler.CaptainConfirmTime = captainConfirmTime;

                            aScheduler.Remark = remark;

                            //aScheduler.RopeUsed = Request.Form["RopeUsed"].Trim();
                            //if (aScheduler.RopeUsed.Equals("是"))
                            //    aScheduler.RopeNum = Util.toint(Request.Form["RopeNum"].Trim());
                            //else
                            //    aScheduler.RopeNum = 0;


                            aScheduler.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            db.Entry(aScheduler).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    #endregion

                    #region 修改调度时，重新选择了拖轮
                    else
                    //插入多个调度到数据库
                    {
                        //1.删除原来的调度
                        db.Scheduler.RemoveRange(db.Scheduler.Where(u => u.IDX == schedulerId).ToList());
                        db.SaveChanges();

                        //2.插入新的拖轮调度
                        List<string> lstTugIds = newTugIds.Split(',').ToList();
                        if (lstTugIds != null && lstTugIds.Count > 0)
                        {
                            List<Scheduler> lstSchedulers = new List<Scheduler>();
                            foreach (string item in lstTugIds)
                            {
                                DataModel.Scheduler aScheduler = new Scheduler();

                                //aScheduler.OrderID = orderId;
                                //aScheduler.ServiceNatureID = serviceNatureId;

                                aScheduler.TugID = Util.toint(item);
                                aScheduler.JobStateID = jobStateId;
                                aScheduler.RopeUsed = ropeUsed;
                                aScheduler.RopeNum = ropeNum;
                                aScheduler.Remark = remark;

                                aScheduler.InformCaptainTime = informCaptainTime;
                                aScheduler.CaptainConfirmTime = captainConfirmTime;

                                aScheduler.OwnerID = -1;
                                aScheduler.UserID = Session.GetDataFromSession<int>("userid");

                                aScheduler.CreateDate = aScheduler.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                aScheduler.UserDefinedCol1 = "";
                                aScheduler.UserDefinedCol2 = "";
                                aScheduler.UserDefinedCol3 = "";
                                aScheduler.UserDefinedCol4 = "";

                                //if (Request.Form["UserDefinedCol5"].Trim() != "")
                                //    aScheduler.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"].Trim());

                                //if (Request.Form["UserDefinedCol6"].Trim() != "")
                                //    aScheduler.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"].Trim());

                                //if (Request.Form["UserDefinedCol7"].Trim() != "")
                                //    aScheduler.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"].Trim());

                                //if (Request.Form["UserDefinedCol8"].Trim() != "")
                                //    aScheduler.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"].Trim());

                                aScheduler.UserDefinedCol9 = "";
                                aScheduler.UserDefinedCol10 = "";

                                lstSchedulers.Add(aScheduler);
                            }

                            db.Scheduler.AddRange(lstSchedulers);
                            db.SaveChanges();
                        }
                    }
                    #endregion

                    {
                        //更新订单状态
                        OrderInfor tmpOrder = db.OrderInfor.Where(u => u.IDX == orderId).FirstOrDefault();
                        if (tmpOrder != null)
                        {
                            tmpOrder.WorkStateID = 3; //已排船
                            db.Entry(tmpOrder).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    {
                        OrderService os = db.OrderService.Where(u => u.OrderID == orderId && u.ServiceNatureID == serviceNatureId).FirstOrDefault();
                        if (os == null)
                        {
                            os = new OrderService();
                            os.CreateDate = os.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            os.OrderID = orderId;
                            os.OwnerID = -1;
                            os.ServiceNatureID = serviceNatureId;
                            os.ServiceWorkDate = serviceWorkDate;
                            os.ServiceWorkPlace = serviceWorkPlace;
                            os.UserID = Session.GetDataFromSession<int>("userid");
                            os = db.OrderService.Add(os);
                            db.SaveChanges();
                        }
                        else
                        {
                            os.ServiceWorkDate = serviceWorkDate;
                            os.ServiceWorkPlace = serviceWorkPlace;
                            os.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            db.Entry(os).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                        }
                    }

                    var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                    //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                    return Json(ret);
                }
            }
            catch (Exception)
            {
                var ret = new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE };
                //Response.Write(@Resources.Common.EXCEPTION_MESSAGE);
                return Json(ret);
            }
        }


        public ActionResult DeleteScheduler()
        {
            this.Internationalization();

            try
            {
                var f = Request.Form;

                int idx = Util.toint(Request.Form["data[IDX]"].Trim());
                int orderId = Util.toint(Request.Form["data[OrderID]"].Trim());
                int orderServiceId = Util.toint(Request.Form["data[OrderServiceID]"].Trim());

                TugDataEntities db = new TugDataEntities();
                Scheduler aScheduler = db.Scheduler.FirstOrDefault(u => u.IDX == idx);
                if (aScheduler != null)
                {
                    //int orderId = (int)aScheduler.OrderID;
                    db.Scheduler.Remove(aScheduler);
                    db.SaveChanges();

                    //删除一个调度之后，要看是否还剩下调度
                    {
                        var list = db.V_OrderScheduler.Where(u => u.OrderID == orderId && u.OrderServiceID == orderServiceId).ToList();
                        if (list == null || list.Count == 0)
                        {
                            //更新服务状态
                            OrderService tmpOrder = db.OrderService.Where(u => u.IDX == orderServiceId).FirstOrDefault();
                            if (tmpOrder != null)
                            {
                                tmpOrder.JobStateID = 114; //未排船
                                db.Entry(tmpOrder).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        //else
                        //{
                        //    //更新订单状态
                        //    OrderInfor tmpOrder = db.OrderInfor.Where(u => u.IDX == orderId).FirstOrDefault();
                        //    if (tmpOrder != null)
                        //    {
                        //        if (true == BusinessLogic.Module.OrderLogic.OrderJobInformationInputIsComplete(orderId))
                        //        {
                        //            tmpOrder.WorkStateID = 5; //已完工
                        //            db.Entry(tmpOrder).State = System.Data.Entity.EntityState.Modified;
                        //            db.SaveChanges();
                        //        }
                        //    }
                        //}
                    }
                    return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });
                }
                else
                {
                    return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        public ActionResult GetTugRelatedOrders(bool _search, string sidx, string sord, int page, int rows, int tugId, string workDate)
        {
            this.Internationalization();

            try
            {
                TugDataEntities db = new TugDataEntities();

                if (_search == true)
                {
                    string s = Request.QueryString["filters"];
                    return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //string now = DateTime.Now.ToString("yyyy-MM-dd");

                    List<V_OrderScheduler> schedulers = db.V_OrderScheduler.Where(u => u.TugID == tugId && u.ServiceWorkDate == workDate)
                        .Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderScheduler>();
                    //List<V_OrderScheduler> orders = BusinessLogic.Module.OrderLogic.LoadDataForOrderScheduler(sidx, sord, orderId);

                    List<V_OrderInfor> orders = new List<V_OrderInfor>();

                    if (schedulers != null)
                    {
                        foreach (V_OrderScheduler item in schedulers)
                        {
                            orders.AddRange(db.V_OrderInfor.Where(u => u.IDX == item.OrderID).Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderInfor>());
                        };
                    }
                    int totalRecordNum = orders.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    //List<V_OrderScheduler> page_orders = orders.Skip((page - 1) * rows).Take(rows).ToList<V_OrderScheduler>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetServiceDateAndPlace(int orderId, int serviceNatureId)
        {
            TugDataEntities db = new TugDataEntities();
            OrderService os = db.OrderService.Where(u => u.OrderID == orderId && u.ServiceNatureID == serviceNatureId).FirstOrDefault();
            if (os != null) {
                return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE, service_place = os.ServiceWorkPlace, service_date = os.ServiceWorkDate }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE, service_place = "", service_date = DateTime.Now.ToString("yyyy-MM-dd") }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetOrderService(int orderServiceId)
        {
            TugDataEntities db = new TugDataEntities();
            V_OrderService os = db.V_OrderService.Where(u => u.OrderServiceID == orderServiceId).FirstOrDefault();
            if (os != null)
            {
                return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE, order_service = os }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE, order_service = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion 订单调度页面Action


        #region 作业信息

        public ActionResult GetDataOfJobInformation(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();

            try
            {
                TugDataEntities db = new TugDataEntities();

                if (_search == true)
                {
                    string searchOption = Request.QueryString["filters"];
                    //List<V_OrderInfor> orders = db.V_OrderInfor.Where(u => u.IDX == -1).Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderInfor>();
                    List<V_OrderInfor> orders = BusinessLogic.Module.OrderLogic.SearchForJobInformation(sidx, sord, searchOption);

                    int totalRecordNum = orders.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_OrderInfor> page_orders = orders.Skip((page - 1) * rows).Take(rows).ToList<V_OrderInfor>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                    //return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //List<V_OrderInfor> orders = db.V_OrderInfor.Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderInfor>();
                    List<V_OrderInfor> orders = BusinessLogic.Module.OrderLogic.LoadDataForJobInformation(sidx, sord);
                    int totalRecordNum = orders.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_OrderInfor> page_orders = orders.Skip((page - 1) * rows).Take(rows).ToList<V_OrderInfor>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        public ActionResult AddEditJobInformation()
        {
            this.Internationalization();

            #region Add

            if (Request.Form["oper"].Equals("add"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    {
                        DataModel.Scheduler aScheduler = new Scheduler();
                        aScheduler.ArrivalBaseTime = Request.Form["ArrivalBaseTime"].Trim();
                        aScheduler.ArrivalShipSideTime = Request.Form["ArrivalShipSideTime"].Trim();
                        aScheduler.CaptainConfirmTime = Request.Form["CaptainConfirmTime"].Trim();
                        aScheduler.DepartBaseTime = Request.Form["DepartBaseTime"].Trim();
                        aScheduler.InformCaptainTime = Request.Form["InformCaptainTime"].Trim();
                        aScheduler.WorkCommencedTime = Request.Form["WorkCommencedTime"].Trim();
                        aScheduler.WorkCompletedTime = Request.Form["WorkCompletedTime"].Trim();

                        aScheduler.JobStateID = Util.toint(Request.Form["JobStateID"].Trim()); ;

                        //aScheduler.OrderID = Util.toint(Request.Form["OrderID"].Trim());
                        aScheduler.OwnerID = -1;
                        aScheduler.UserID = Session.GetDataFromSession<int>("userid");
                        aScheduler.Remark = Request.Form["Remark"].Trim(); ;

                        aScheduler.RopeUsed = Request.Form["RopeUsed"].Trim();
                        if (aScheduler.RopeUsed.Equals("是"))
                            aScheduler.RopeNum = Util.toint(Request.Form["RopeNum"].Trim());
                        else
                            aScheduler.RopeNum = 0;

                        //aScheduler.ServiceNatureID = Util.toint(Request.Form["ServiceNatureLabel"].Trim().Split('~')[0]);
                        aScheduler.TugID = Util.toint(Request.Form["TugID"].Trim());
                        aScheduler.CreateDate = aScheduler.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        aScheduler.UserDefinedCol1 = Request.Form["UserDefinedCol1"].Trim();
                        aScheduler.UserDefinedCol2 = Request.Form["UserDefinedCol2"].Trim();
                        aScheduler.UserDefinedCol3 = Request.Form["UserDefinedCol3"].Trim();
                        aScheduler.UserDefinedCol4 = Request.Form["UserDefinedCol4"].Trim();

                        if (Request.Form["UserDefinedCol5"].Trim() != "")
                            aScheduler.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"].Trim());

                        if (Request.Form["UserDefinedCol6"].Trim() != "")
                            aScheduler.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"].Trim());

                        if (Request.Form["UserDefinedCol7"].Trim() != "")
                            aScheduler.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"].Trim());

                        if (Request.Form["UserDefinedCol8"].Trim() != "")
                            aScheduler.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"].Trim());

                        aScheduler.UserDefinedCol9 = Request.Form["UserDefinedCol9"].Trim();
                        aScheduler.UserDefinedCol10 = Request.Form["UserDefinedCol10"].Trim();

                        aScheduler = db.Scheduler.Add(aScheduler);
                        db.SaveChanges();

                        {
                            //OrderService os = db.OrderService.Where(u => u.OrderID == aScheduler.OrderID && u.ServiceNatureID == aScheduler.ServiceNatureID).FirstOrDefault();
                            //if (os == null)
                            //{
                            //    os = new OrderService();
                            //    os.CreateDate = os.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //    //os.OrderID = aScheduler.OrderID;
                            //    os.OwnerID = -1;
                            //    //os.ServiceNatureID = aScheduler.ServiceNatureID;
                            //    os.ServiceWorkPlace = Request.Form["ServiceWorkPlace"].Trim();
                            //    os.UserID = Session.GetDataFromSession<int>("userid");
                            //    os = db.OrderService.Add(os);
                            //    db.SaveChanges();
                            //}
                            //else
                            //{
                            //    os.ServiceWorkPlace = Request.Form["ServiceWorkPlace"].Trim();
                            //    os.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //    db.Entry(os).State = System.Data.Entity.EntityState.Modified;
                            //    db.SaveChanges();
                            //}
                        }

                        var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                        //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                        return Json(ret);
                    }
                }
                catch (Exception)
                {
                    var ret = new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE };
                    //Response.Write(@Resources.Common.EXCEPTION_MESSAGE);
                    return Json(ret);
                }
            }

            #endregion Add

            #region Edit

            if (Request.Form["oper"].Equals("edit"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();

                    int idx = Util.toint(Request.Form["IDX"].Trim());
                    int orderServiceId = Util.toint(Request.Form["OrderServiceID"].Trim());
                    int orderId = Util.toint(Request.Form["OrderID"].Trim());;
                    Scheduler aScheduler = db.Scheduler.Where(u => u.IDX == idx).FirstOrDefault();

                    if (aScheduler == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        aScheduler.ArrivalBaseTime = Request.Form["ArrivalBaseTime"].Trim();
                        aScheduler.ArrivalShipSideTime = Request.Form["ArrivalShipSideTime"].Trim();
                        //aScheduler.CaptainConfirmTime = Request.Form["CaptainConfirmTime"].Trim();
                        aScheduler.DepartBaseTime = Request.Form["DepartBaseTime"].Trim();
                        //aScheduler.InformCaptainTime = Request.Form["InformCaptainTime"].Trim();
                        aScheduler.WorkCommencedTime = Request.Form["WorkCommencedTime"].Trim();
                        aScheduler.WorkCompletedTime = Request.Form["WorkCompletedTime"].Trim();

                        //aScheduler.JobStateID = Util.toint(Request.Form["JobStateID"].Trim()); ;

                        //aScheduler.OrderID = Util.toint(Request.Form["OrderID"].Trim());
                        aScheduler.OwnerID = -1;
                        aScheduler.UserID = Session.GetDataFromSession<int>("userid");
                        aScheduler.Remark = Request.Form["Remark".Trim()];

                        aScheduler.RopeUsed = Request.Form["RopeUsed"].Trim();
                        if (aScheduler.RopeUsed.Equals("是"))
                            aScheduler.RopeNum = 1;//Util.toint(Request.Form["RopeNum"].Trim());
                        else
                            aScheduler.RopeNum = 0;

                        //aScheduler.ServiceNatureID = Util.toint(Request.Form["ServiceNatureLabel"].Trim().Split('~')[0]);

                        //aScheduler.TugID = Util.toint(Request.Form["TugID"].Trim());
                        aScheduler.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        //aScheduler.UserDefinedCol1 = Request.Form["UserDefinedCol1"].Trim();
                        //aScheduler.UserDefinedCol2 = Request.Form["UserDefinedCol2"].Trim();
                        //aScheduler.UserDefinedCol3 = Request.Form["UserDefinedCol3"].Trim();
                        //aScheduler.UserDefinedCol4 = Request.Form["UserDefinedCol4"].Trim();

                        //if (Request.Form["UserDefinedCol5"].Trim() != "")
                        //    aScheduler.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"].Trim());

                        //if (Request.Form["UserDefinedCol6"].Trim() != "")
                        //    aScheduler.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"].Trim());

                        //if (Request.Form["UserDefinedCol7"].Trim() != "")
                        //    aScheduler.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"].Trim());

                        //if (Request.Form["UserDefinedCol8"].Trim() != "")
                        //    aScheduler.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"].Trim());

                        //aScheduler.UserDefinedCol9 = Request.Form["UserDefinedCol9"].Trim();
                        //aScheduler.UserDefinedCol10 = Request.Form["UserDefinedCol10"].Trim();

                        db.Entry(aScheduler).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        {
                            //OrderService os = db.OrderService.Where(u => u.IDX == orderServiceId).FirstOrDefault();
                            //if (os == null)
                            //{
                            //    os = new OrderService();
                            //    os.CreateDate = os.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //    //os.OrderID = aScheduler.OrderID;
                            //    os.OwnerID = -1;
                            //    //os.ServiceNatureID = aScheduler.ServiceNatureID;
                            //    //os.ServiceWorkPlace = Request.Form["ServiceWorkPlace"].Trim();
                            //    //os.JobStateID = Util.toint(Request.Form["ServiceJobStateID"].Trim());
                            //    os.UserID = Session.GetDataFromSession<int>("userid");
                            //    os = db.OrderService.Add(os);
                            //    db.SaveChanges();
                            //}
                            //else
                            //{
                            //    //os.ServiceWorkPlace = Request.Form["ServiceWorkPlace"].Trim();
                            //    //os.JobStateID = Util.toint(Request.Form["ServiceJobStateID"].Trim());
                            //    os.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //    db.Entry(os).State = System.Data.Entity.EntityState.Modified;
                            //    db.SaveChanges();
                            //}
                        }

                        {
                            //更新订单状态
                            OrderInfor tmpOrder = db.OrderInfor.Where(u => u.IDX == orderId).FirstOrDefault();
                            if (tmpOrder != null)
                            {
                                if (true == BusinessLogic.Module.OrderLogic.OrderJobInformationInputIsComplete(orderId))
                                {
                                    tmpOrder.WorkStateID = 5; //已完工，所有作业信息已输入完，订单状态变为“已完工”
                                    tmpOrder.UserDefinedCol4 = "1"; //1是所有时间录入完成
                                    db.Entry(tmpOrder).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();

                                    var ord_services = db.OrderService.Where(u => u.OrderID == orderId).ToList();
                                    if (ord_services != null)
                                    {
                                        foreach (var item in ord_services)
                                        {
                                            item.UserDefinedCol4 = "1"; //1是所有时间录入完成
                                            db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    tmpOrder.WorkStateID = 2;
                                    db.Entry(tmpOrder).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                }

                            }

                            
                        }

                        return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });
                    }
                }
                catch (Exception exp)
                {
                    return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
                }
            }

            #endregion Edit

            return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
        }

        public ActionResult RejectOrderServiceToScheduler(int orderId)
        {
            int ret = BusinessLogic.Module.OrderLogic.HasBilling(orderId);

            //沒有帳單可以駁回
            if (ret == -1) {

                using (System.Transactions.TransactionScope trans = new System.Transactions.TransactionScope())
                {
                    try
                    {
                        TugDataEntities db = new TugDataEntities();
                        var lstOrderService = db.OrderService.Where(u => u.OrderID == orderId).ToList();
                        if (lstOrderService != null)
                        {
                            foreach (var item in lstOrderService)
                            {
                                var lstScheduler = db.Scheduler.Where(u => u.OrderServiceID == item.IDX).ToList();
                                if (lstScheduler != null)
                                {
                                    foreach (var item2 in lstScheduler)
                                    {
                                        item2.DepartBaseTime = "";
                                        item2.ArrivalBaseTime = "";
                                        item2.RopeUsed = "否";
                                        item2.Remark = "";
                                        db.Entry(item2).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }

                                item.JobStateID = 124;
                                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }

                            //订单状态该为未排船
                            {
                                OrderInfor oi = db.OrderInfor.FirstOrDefault(u => u.IDX == orderId);
                                if (oi != null)
                                {
                                    oi.WorkStateID = 2;  //所有服务都未完成，因此订单状态变成“未排船”
                                    oi.UserDefinedCol4 = "0"; //0是所有时间未录入完成
                                    db.Entry(oi).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();

                                    var ord_services = db.OrderService.Where(u => u.OrderID == orderId).ToList();
                                    if (ord_services != null)
                                    {
                                        foreach (var item in ord_services)
                                        {
                                            item.UserDefinedCol4 = "0"; //0是所有时间未录入完成
                                            db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }

                        trans.Complete();
  
                    }
                    catch(Exception ex)
                    {
                        trans.Dispose();
                        return Json(new
                        {
                            code = Resources.Common.EXCEPTION_CODE,
                            message = Resources.Common.EXCEPTION_MESSAGE,
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            return Json(new
            {
                code = Resources.Common.SUCCESS_CODE,
                message = Resources.Common.SUCCESS_MESSAGE,
                has_invoice = ret
            }, JsonRequestBehavior.AllowGet);
            
        }


        #endregion


        [HttpPost]
        [Authorize]
        public ActionResult CheckOrderInvoiceStatus(int orderId)
        {
            this.Internationalization();

            TugDataEntities db = new TugDataEntities();
            OrderInfor order = db.OrderInfor.FirstOrDefault(u => u.IDX == orderId);

            string ret = "否";
            if (order != null)
            {
                ret = order.HasInvoice;
            }

            return Json(new
            {
                code = Resources.Common.SUCCESS_CODE,
                message = Resources.Common.SUCCESS_MESSAGE,
                has_invoice = ret
            }, JsonRequestBehavior.AllowGet);
        }


        #region 服务状态

        /// <summary>
        /// 服务修改
        /// </summary>
        /// <returns></returns>
        public ActionResult AddEditService()
        {
            this.Internationalization();

            #region Edit

            if (Request.Form["oper"].Equals("edit"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();

                    int idx = Util.toint(Request.Form["OrderServiceID"].Trim());
                    OrderService aScheduler = db.OrderService.Where(u => u.IDX == idx).FirstOrDefault();

                    if (aScheduler == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        //aScheduler.ServiceNatureID = BusinessLogic.Module.Util.toint(Request.Form["ServiceNatureID"]);
                        aScheduler.ServiceWorkDate = Request.Form["ServiceWorkDate"];
                        aScheduler.ServiceWorkPlace = Request.Form["ServiceWorkPlace"];
                        aScheduler.ServiceWorkTime = Request.Form["ServiceWorkTime"];


                        aScheduler.BigTugNum = Util.toint(Request.Form["BigTugNum"].Trim());
                        aScheduler.MiddleTugNum = Util.toint(Request.Form["MiddleTugNum"].Trim());
                        aScheduler.SmallTugNum = Util.toint(Request.Form["SmallTugNum"].Trim());

                        aScheduler.OwnerID = -1;
                        aScheduler.UserID = Session.GetDataFromSession<int>("userid");

                        aScheduler.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


                        db.Entry(aScheduler).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });
                    }
                }
                catch (Exception exp)
                {
                    return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
                }
            }

            #endregion Edit

            return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
        }


        public ActionResult AddEditService2()
        {
            this.Internationalization();

            #region Edit

            if (Request.Form["oper"].Equals("edit"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();

                    int idx = Util.toint(Request.Form["OrderServiceID"].Trim());
                    OrderService aOrderService = db.OrderService.Where(u => u.IDX == idx).FirstOrDefault();

                    if (aOrderService == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        aOrderService.ServiceWorkDate = Request.Form["ServiceWorkDate"];
                        aOrderService.ServiceWorkTime = Request.Form["ServiceWorkTime"];
                        aOrderService.ServiceNatureID = BusinessLogic.Module.Util.toint(Request.Form["ServiceNatureID"]);
                        aOrderService.ServiceWorkPlace = Request.Form["ServiceWorkPlace"];


                        aOrderService.BigTugNum = Util.toint(Request.Form["BigTugNum"].Trim());
                        aOrderService.MiddleTugNum = Util.toint(Request.Form["MiddleTugNum"].Trim());
                        aOrderService.SmallTugNum = Util.toint(Request.Form["SmallTugNum"].Trim());

                        aOrderService.UserDefinedCol9 = Request.Form["TugIDs"];
                        aOrderService.UserDefinedCol10 = Request.Form["TugNames"];

                        aOrderService.OwnerID = -1;
                        aOrderService.UserID = Session.GetDataFromSession<int>("userid");

                        aOrderService.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


                        db.Entry(aOrderService).State = System.Data.Entity.EntityState.Modified;
                        if (0 < db.SaveChanges()) //修改成功
                        {
                            //1.先删除原有的调度
                            var oldSchedulers = db.Scheduler.Where(u => u.OrderServiceID == idx).ToList();
                            if (oldSchedulers != null)
                            {
                                db.Scheduler.RemoveRange(oldSchedulers);
                                if (0 < db.SaveChanges())
                                {
                                    OrderService os = db.OrderService.FirstOrDefault(u => u.IDX == idx);
                                    os.JobStateID = 114;
                                    db.Entry(os).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }

                            //2.插入新更改的调度
                            #region 插入多个调度到数据库
                            {
                                if (aOrderService.UserDefinedCol9.Trim() != "")
                                {
                                    List<string> lstTugIds = aOrderService.UserDefinedCol9.Split(',').ToList();
                                    if (lstTugIds != null && lstTugIds.Count > 0)
                                    {
                                        List<Scheduler> lstSchedulers = new List<Scheduler>();
                                        foreach (string item in lstTugIds)
                                        {
                                            DataModel.Scheduler aScheduler = new Scheduler();

                                            aScheduler.OrderServiceID = idx;

                                            aScheduler.TugID = Util.toint(item);

                                            aScheduler.RopeUsed = "";
                                            aScheduler.RopeNum = 0;
                                            aScheduler.Remark = "";

                                            aScheduler.IsCaptainConfirm = "";

                                            aScheduler.InformCaptainTime = "";
                                            aScheduler.CaptainConfirmTime = "";
                                            aScheduler.JobStateID = 32;

                                            aScheduler.OwnerID = -1;
                                            aScheduler.UserID = Session.GetDataFromSession<int>("userid");

                                            aScheduler.CreateDate = aScheduler.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                            aScheduler.UserDefinedCol1 = "";
                                            aScheduler.UserDefinedCol2 = "";
                                            aScheduler.UserDefinedCol3 = "";
                                            aScheduler.UserDefinedCol4 = "";

                                            aScheduler.UserDefinedCol9 = "";
                                            aScheduler.UserDefinedCol10 = "";

                                            lstSchedulers.Add(aScheduler);
                                        }

                                        db.Scheduler.AddRange(lstSchedulers);
                                        if (0 < db.SaveChanges())
                                        {
                                            OrderService os = db.OrderService.FirstOrDefault(u => u.IDX == idx);
                                            os.JobStateID = 115;
                                            db.Entry(os).State = System.Data.Entity.EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                            #endregion
                        }

                        return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });
                    }
                }
                catch (Exception exp)
                {
                    return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
                }
            }

            #endregion Edit

            return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
        }


        public ActionResult GetServiceSubSchedulerData(bool _search, string sidx, string sord, int page, int rows, int orderServiceId)
        {
            this.Internationalization();

            try
            {
                //
                TugDataEntities db = new TugDataEntities();

                if (_search == true)
                {
                    string s = Request.QueryString["filters"];
                    return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //List<V_OrderScheduler> orders = db.V_OrderScheduler.Where(u => u.OrderID == orderId).Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderScheduler>();
                    List<V_OrderScheduler> orders = BusinessLogic.Module.OrderLogic.LoadDataForServiceScheduler(sidx, sord, orderServiceId);
                    int totalRecordNum = orders.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_OrderScheduler> page_orders = orders.Skip((page - 1) * rows).Take(rows).ToList<V_OrderScheduler>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ChangeServiceStatusToComplete(int orderId, int orderServiceId)
        {
            TugDataEntities db = new TugDataEntities();
            OrderService os = db.OrderService.FirstOrDefault(u => u.IDX == orderServiceId);
            if (os != null)
            {
                int neededTotalShipNumbers = BusinessLogic.Module.Util.toint(os.BigTugNum) + BusinessLogic.Module.Util.toint(os.MiddleTugNum) + BusinessLogic.Module.Util.toint(os.SmallTugNum);
                int realTotalShipNumbers = db.Scheduler.Where(u => u.OrderServiceID == os.IDX).ToList().Count;
                if (realTotalShipNumbers != neededTotalShipNumbers)
                {
                    var retJson = new { code = Resources.Common.FAIL_CODE, message = "已經排的拖輪和所需的拖輪數量不等，無法完成，請檢查實際排船數量！" };
                    //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                    return Json(retJson);
                }

                os.JobStateID = 116;
                db.Entry(os).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var list = db.V_OrderService.Where(u => u.OrderID == orderId).ToList();
                if (list != null)
                {
                    bool flag = true;
                    foreach (var item in list) //查看该订单下有没有未完成的服务，如果有，flag = false; 如果没有flag = true
                    {
                        if(item.ServiceJobStateID != 116 || item.ServiceJobStateValue != "2" ||item.ServiceJobStateLabel != "已完工")
                        {
                            flag = false;
                            break;
                        }

                    }

                    if (flag == true) //flag是true说明该订单下的所有服务都已经是完成状态，将订单状态改为已排船，否则是未排船
                    {
                        //更新订单状态为已排船
                        OrderInfor oi = db.OrderInfor.FirstOrDefault(u => u.IDX == orderId);
                        if(oi != null)
                        {
                            oi.WorkStateID = 3;  //所有服务都完成订单状态变成“已排船”
                            db.Entry(oi).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                return Json(ret);
            }
            else
            {
                var ret = new { code = Resources.Common.FAIL_CODE, message = Resources.Common.FAIL_MESSAGE };
                //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                return Json(ret);
            }
        }
        #endregion


        #region 账单生成

        public ActionResult GetDataOfGenerateInvoicce(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();

            try
            {
                TugDataEntities db = new TugDataEntities();

                if (_search == true)
                {
                    string searchOption = Request.QueryString["filters"];
                    //List<V_OrderInfor> orders = db.V_OrderInfor.Where(u => u.IDX == -1).Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderInfor>();
                    List<V_OrderInfor> orders = BusinessLogic.Module.OrderLogic.SearchForGenerateInvoice(sidx, sord, searchOption);

                    int totalRecordNum = orders.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_OrderInfor> page_orders = orders.Skip((page - 1) * rows).Take(rows).ToList<V_OrderInfor>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                    //return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //List<V_OrderInfor> orders = db.V_OrderInfor.Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderInfor>();
                    List<V_OrderInfor> orders = BusinessLogic.Module.OrderLogic.LoadDataForGenerateInvoice(sidx, sord);
                    int totalRecordNum = orders.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_OrderInfor> page_orders = orders.Skip((page - 1) * rows).Take(rows).ToList<V_OrderInfor>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }


        /// <summary>
        /// 获取普通账单关联的订单
        /// </summary>
        /// <param name="_search"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public ActionResult GetOrdersOfCommonInvoicce(bool _search, string sidx, string sord, int page, int rows, int billingId)
        {
            this.Internationalization();

            try
            {
                TugDataEntities db = new TugDataEntities();

                if (_search == true)
                {
                    string searchOption = Request.QueryString["filters"];
                    List<V_BillingOrders> orders = db.V_BillingOrders.Where(u => u.BillingID == billingId)
                        .Select(u => u).OrderByDescending(u => u.OrdDate).ToList<V_BillingOrders>();

                    int totalRecordNum = orders.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_BillingOrders> page_orders = orders.Skip((page - 1) * rows).Take(rows).ToList<V_BillingOrders>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                    //return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<V_BillingOrders> orders = db.V_BillingOrders.Where(u => u.BillingID == billingId)
                        .Select(u => u).OrderByDescending(u => u.OrdDate).ToList<V_BillingOrders>();
                    //List<V_OrderInfor> orders = BusinessLogic.Module.OrderLogic.LoadDataForGenerateInvoice(sidx, sord);
                    int totalRecordNum = orders.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_BillingOrders> page_orders = orders.Skip((page - 1) * rows).Take(rows).ToList<V_BillingOrders>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        #endregion
    }
}
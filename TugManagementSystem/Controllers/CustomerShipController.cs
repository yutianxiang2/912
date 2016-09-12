using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogic.Module;
using DataModel;
using BusinessLogic;

namespace WMS.Controllers
{
    public class CustomerShipController : BaseController
    {

        private static int _DefaultPageSie = 7;

        [JsonExceptionFilterAttribute]
        public ActionResult AddEdit(int ctmId)
        {
            this.Internationalization();

            #region Add

            if (Request.Form["oper"].Equals("add"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    {
                        DataModel.CustomerShip ship = new CustomerShip();

                        ship.CustomerID = ctmId;// Util.toint(Request.Form["CustomerID"]);
                        ship.ShipTypeID = -1;//Util.toint(Request.Form["ShipTypeID"]);
                        ship.Name1 = Request.Form["Name1"];
                        ship.Name2 = Request.Form["Name2"];
                        ship.SimpleName = Request.Form["SimpleName"];
                        ship.DeadWeight = Util.toint(Request.Form["DeadWeight"]);
                        ship.Length = Util.toint(Request.Form["Length"]);
                        ship.Width = Util.toint(Request.Form["Width"]);
                        ship.TEUS = Util.toint(Request.Form["TEUS"]);
                        ship.Class = Request.Form["Class"];
                        ship.Remark = Request.Form["Remark"];
                        ship.OwnerID = -1;
                        ship.CreateDate = ship.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
                        ship.UserID = Session.GetDataFromSession<int>("userid"); 
                        ship.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        ship.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        ship.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        ship.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            ship.UserDefinedCol5 = Util.tonumeric(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            ship.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            ship.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            ship.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        ship.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        ship.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        ship = db.CustomerShip.Add(ship);
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
                    int idx = Util.toint(Request.Form["IDX"]);
                    string name1 = Request.Form["Name1"];
                    System.Linq.Expressions.Expression<Func<Customer, bool>> exp = u => u.Name1 == name1 && u.IDX != idx;
                    Customer obj = db.Customer.Where(exp).FirstOrDefault();
                    if (obj != null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = "船名称已存在！" });//Resources.Common.ERROR_MESSAGE
                    }
                    
                    CustomerShip ship = db.CustomerShip.Where(u => u.IDX == idx).FirstOrDefault();

                    if (ship == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        ship.CustomerID = ctmId;// Util.toint(Request.Form["CustomerID"]);
                        ship.ShipTypeID = -1; //Util.toint(Request.Form["ShipTypeID"]);
                        ship.Name1 = Request.Form["Name1"];
                        ship.Name2 = Request.Form["Name2"];
                        ship.SimpleName = Request.Form["SimpleName"];
                        ship.DeadWeight = Util.toint(Request.Form["DeadWeight"]);
                        ship.Length = Util.toint(Request.Form["Length"]);
                        ship.Width = Util.toint(Request.Form["Width"]);
                        ship.TEUS = Util.toint(Request.Form["TEUS"]);
                        ship.Class = Request.Form["Class"];
                        ship.Remark = Request.Form["Remark"];
                        ship.OwnerID = -1;
                        ship.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
                        ship.UserID = Session.GetDataFromSession<int>("userid"); 
                        ship.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        ship.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        ship.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        ship.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            ship.UserDefinedCol5 = Util.tonumeric(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            ship.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            ship.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            ship.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        ship.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        ship.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        db.Entry(ship).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        var orderList = db.OrderInfor.Where(u => u.ShipID == idx).ToList();
                        if (orderList != null)
                        {
                            foreach (var item in orderList)
                            {
                                item.ShipName = ship.Name1;
                                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
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

        [JsonExceptionFilterAttribute]
        public ActionResult AddCustomerShip(int ctmId, string Name1, string Name2, string SimpleName, string DeadWeight, string Length,
            string Width, string TEUS, string Class, string Remark)
        {
            this.Internationalization();
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    System.Linq.Expressions.Expression<Func<CustomerShip, bool>> exp = u => u.Name1 == Name1 && u.CustomerID == ctmId;
                    CustomerShip obj = db.CustomerShip.Where(exp).FirstOrDefault();
                    if (obj != null)
                    {
                        throw new Exception("船名称已存在！");
                    }
                    else
                    {
                        DataModel.CustomerShip ship = new CustomerShip();

                        ship.CustomerID = ctmId;// Util.toint(Request.Form["CustomerID"]);
                        ship.ShipTypeID = -1;//Util.toint(Request.Form["ShipTypeID"]);
                        ship.Name1 =Name1;
                        ship.Name2 = Name2;
                        ship.SimpleName = SimpleName;
                        ship.DeadWeight = Util.toint(DeadWeight);
                        ship.Length = Util.toint(Length);
                        ship.Width = Util.toint(Width);
                        ship.TEUS = Util.toint(TEUS);
                        ship.Class = Class;
                        ship.Remark = Remark;
                        ship.OwnerID = -1;
                        ship.CreateDate = ship.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;
                        ship.UserID = Session.GetDataFromSession<int>("userid");
                        ship.UserDefinedCol1 = "";
                        ship.UserDefinedCol2 = "";
                        ship.UserDefinedCol3 = "";
                        ship.UserDefinedCol4 = "";

                        //if (Request.Form["UserDefinedCol5"] != "")
                        //    ship.UserDefinedCol5 = Util.tonumeric(Request.Form["UserDefinedCol5"]);

                        //if (Request.Form["UserDefinedCol6"] != "")
                        //    ship.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        //if (Request.Form["UserDefinedCol7"] != "")
                        //    ship.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        //if (Request.Form["UserDefinedCol8"] != "")
                        //    ship.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        ship.UserDefinedCol9 = "";
                        ship.UserDefinedCol10 = "";

                        ship = db.CustomerShip.Add(ship);
                        db.SaveChanges();

                        var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                        //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                        return Json(ret);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                    //var ret = new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE };
                    //return Json(ret);
                }
        }

        [Authorize]
        public ActionResult CustomerShipManage(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            int totalRecordNum, totalPageNum;
            List<Customer> list = GetCustomers(1, _DefaultPageSie, out totalRecordNum, out totalPageNum);
            ViewBag.TotalPageNum = totalPageNum;
            ViewBag.CurPage = 1;

            return View(list);
        }
        [Authorize]
        public ActionResult CustomerShipPage(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            int totalRecordNum, totalPageNum;
            List<Customer> list = GetCustomers(1, _DefaultPageSie, out totalRecordNum, out totalPageNum);
            ViewBag.TotalPageNum = totalPageNum;
            ViewBag.CurPage = 1;

            return View(list);
        }
        public List<Customer> GetCustomers(int curPage, int pageSize, out int totalRecordNum, out int totalPageNum, string queryName = "")
        {
            try
            {
                TugDataEntities db = new TugDataEntities();

                List<Customer> Customers = null;
                if (queryName == "")
                {
                    Customers = db.Customer.Where(u => u.Code != "-1" && u.Name1 != "非會員客戶").Select(u => u).OrderBy(u => u.Name1).ToList<Customer>();
                }
                else
                {
                    Customers = db.Customer.Where(u => u.Name1.Contains(queryName) && u.Code != "-1" && u.Name1 != "非會員客戶")
                        .Select(u => u).OrderBy(u => u.Name1).ToList<Customer>();
                }

                totalRecordNum = Customers.Count;
                //if (totalRecordNum % pageSize == 0) page -= 1;
                //int pageSize = rows;
                totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                List<Customer> page_Customers = Customers.Skip((curPage - 1) * pageSize).Take(pageSize).ToList<Customer>();
                return page_Customers;
            }
            catch (Exception)
            {
                totalRecordNum = totalPageNum = 0;
                return null;
            }
        }
        
        [HttpGet]
        public ActionResult GetCustomers(int curPage, string queryName = "")
        {
            ViewBag.Language = this.Internationalization();

            int totalRecordNum, totalPageNum;
            List<Customer> list = GetCustomers(curPage, _DefaultPageSie, out totalRecordNum, out totalPageNum, queryName);
            ViewBag.TotalPageNum = totalPageNum;
            ViewBag.CurPage = curPage;
            ViewBag.QueryName = queryName;

            return View("CustomerShipManage", list);
        }
        [JsonExceptionFilterAttribute]
        public ActionResult Delete()
        {
            this.Internationalization();

            try
            {
                var f = Request.Form;

                int idx = Util.toint(Request.Form["data[IDX]"]);

                TugDataEntities db = new TugDataEntities();
                //先判断该客户船是否被使用过
                OrderInfor obj = db.OrderInfor.FirstOrDefault(u => u.ShipID == idx);
                if (obj != null) throw new Exception("此客戶船已在訂單中使用過，不能被刪除！"); 
                //删除客户船
                CustomerShip ship = db.CustomerShip.FirstOrDefault(u => u.IDX == idx);
                if (ship != null)
                { 
                    db.CustomerShip.Remove(ship);
                    db.SaveChanges();
                    var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                    return Json(ret);
                }
                else
                {
                    return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult GetCustomerShips(bool _search, string sidx, string sord, int page, int rows, int ctmId)
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
                    List<CustomerShip> objs = db.CustomerShip.Where(u => u.CustomerID == ctmId).Select(u => u).OrderByDescending(u => u.LastUpDate).ToList<CustomerShip>();
                    int totalRecordNum = objs.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    //List<CustomerShip> page_objs = objs.Skip((page - 1) * rows).Take(rows).ToList<CustomerShip>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = objs };
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
                List<CustomerShip> customers = db.CustomerShip.Select(u => u).OrderByDescending(u => u.IDX).ToList<CustomerShip>();
                int totalRecordNum = customers.Count;
                if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                int pageSize = rows;
                int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                //List<CustomerShip> page_customers = customers.Skip((page - 1) * rows).Take(rows).OrderBy(u => u.IDX).ToList<CustomerShip>();

                var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = customers };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        //
        // GET: /CustomerShip/
        public ActionResult Index()
        {
            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataModel;
using BusinessLogic;
using BusinessLogic.Module;
using Newtonsoft.Json;

namespace WMS.Controllers
{
    public class CustomerController : BaseController
    {

        private static int _DefaultPageSie = 7;

        [JsonExceptionFilterAttribute]
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
                        DataModel.Customer cstmer = new Customer();

                        cstmer.Code = Request.Form["Code"];
                        cstmer.Name1 = Request.Form["Name1"];
                        cstmer.Name2 = Request.Form["Name2"];
                        cstmer.SimpleName = Request.Form["SimpleName"];
                        cstmer.TypeID = Util.toint(Request.Form["TypeID"]);
                        cstmer.ContactPerson = Request.Form["ContactPerson"];
                        cstmer.Telephone = Request.Form["Telephone"];
                        cstmer.Fax = Request.Form["Fax"];
                        cstmer.Email = Request.Form["Email"];
                        cstmer.Address = Request.Form["Address"];
                        cstmer.MailCode = Request.Form["MailCode"];
                        cstmer.Remark = Request.Form["Remark"];
                        cstmer.OwnerID = -1;
                        cstmer.CreateDate = cstmer.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;//.ToString("yyyy-MM-dd");
                        cstmer.UserID = Session.GetDataFromSession<int>("userid"); 
                        cstmer.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        cstmer.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        cstmer.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        cstmer.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            cstmer.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            cstmer.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            cstmer.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            cstmer.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        cstmer.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        cstmer.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        cstmer = db.Customer.Add(cstmer);
                        db.SaveChanges();

                        var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                        Response.Write(@Resources.Common.SUCCESS_MESSAGE);
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
                    int idx = Util.toint(Request.Form["IDX"]);
                    string name1 = Request.Form["Name1"];
                    TugDataEntities db = new TugDataEntities();
                    System.Linq.Expressions.Expression<Func<Customer, bool>> exp = u => u.Name1 == name1 && u.IDX!=idx;
                    Customer obj = db.Customer.Where(exp).FirstOrDefault();
                    if (obj != null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = "客户名称已存在！" });//Resources.Common.ERROR_MESSAGE
                    }

                    Customer cstmer = db.Customer.Where(u => u.IDX == idx).FirstOrDefault();

                    if (cstmer == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        cstmer.Code = Request.Form["Code"];
                        cstmer.Name1 = Request.Form["Name1"];
                        cstmer.Name2 = Request.Form["Name2"];
                        cstmer.SimpleName = Request.Form["SimpleName"];
                        cstmer.TypeID =Util.toint(Request.Form["TypeID"]);
                        cstmer.ContactPerson = Request.Form["ContactPerson"];
                        cstmer.Telephone = Request.Form["Telephone"];
                        cstmer.Fax = Request.Form["Fax"];
                        cstmer.Email = Request.Form["Email"];
                        cstmer.Address = Request.Form["Address"];
                        cstmer.MailCode = Request.Form["MailCode"];
                        cstmer.Remark = Request.Form["Remark"];
                        cstmer.OwnerID = -1;
                        cstmer.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
                        cstmer.UserID = Session.GetDataFromSession<int>("userid"); 
                        cstmer.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        cstmer.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        cstmer.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        cstmer.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            cstmer.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            cstmer.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            cstmer.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            cstmer.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        cstmer.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        cstmer.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        db.Entry(cstmer).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        var orderList = db.OrderInfor.Where(u => u.CustomerID == idx).ToList();
                        if (orderList != null)
                        {
                            foreach (var item in orderList)
                            {
                                item.CustomerName = cstmer.Name1;
                                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                        }

                        return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            #endregion Edit

            return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
        }

       [JsonExceptionFilterAttribute]
        public ActionResult AddCustomer(string Code,string Name1,string Name2,string SimpleName,string ContactPerson,
      string Telephone, string Fax, string Email, string Address, string MailCode, string Remark)
        {
            this.Internationalization();
            try
            {
                TugDataEntities db = new TugDataEntities();
                System.Linq.Expressions.Expression<Func<Customer, bool>> exp = u => u.Name1 == Name1;
                Customer obj = db.Customer.Where(exp).FirstOrDefault();
                if (obj != null)
                {
                    throw new Exception("客户名称已存在！");
                }
                {
                    DataModel.Customer cstmer = new Customer();

                    cstmer.Code = Code;
                    cstmer.Name1 = Name1;
                    cstmer.Name2 = Name2;
                    cstmer.SimpleName = SimpleName;
                    cstmer.ContactPerson = ContactPerson;
                    cstmer.Telephone = Telephone;
                    cstmer.Fax = Fax;
                    cstmer.Email = Email;
                    cstmer.Address = Address;
                    cstmer.MailCode = MailCode;
                    cstmer.Remark = Remark;
                    cstmer.OwnerID = -1;
                    cstmer.CreateDate = cstmer.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;//.ToString("yyyy-MM-dd");
                    cstmer.UserID = Session.GetDataFromSession<int>("userid"); 
                    cstmer.UserDefinedCol1 = "";
                    cstmer.UserDefinedCol2 = "";
                    cstmer.UserDefinedCol3 = "";
                    cstmer.UserDefinedCol4 = "";
                    //if (Request.Form["UserDefinedCol5"].Trim() != "")
                    //    cstmer.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"].Trim());

                    //if (Request.Form["UserDefinedCol6"].Trim() != "")
                    //    cstmer.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"].Trim());

                    //if (Request.Form["UserDefinedCol7"].Trim() != "")
                    //    cstmer.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"].Trim());

                    //if (Request.Form["UserDefinedCol8"].Trim() != "")
                    //    cstmer.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"].Trim());

                    cstmer.UserDefinedCol9 = "";
                    cstmer.UserDefinedCol10 = "";

                    cstmer = db.Customer.Add(cstmer);
                    db.SaveChanges();

                    var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                    //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                    return Json(ret);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Authorize]
        public ActionResult CustomerManage(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            return View();
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
                //先判断此客户是否已被使用过
                OrderInfor obj = db.OrderInfor.FirstOrDefault(u => u.CustomerID == idx);
                if (obj != null) throw new Exception("此客戶已在訂單中使用過，不能被刪除！"); 
                //删除客户
                Customer cstmer = db.Customer.FirstOrDefault(u => u.IDX == idx);
                if (cstmer != null)
                {
                    db.Customer.Remove(cstmer);
                    db.SaveChanges();
                    return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });
                }
                else
                {
                    return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        public ActionResult GetDataForLoadOnce(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();
            //string s = Request.QueryString[6];

            try
            {
                TugDataEntities db = new TugDataEntities();

                //db.Configuration.ProxyCreationEnabled = false;
                List<Customer> customers = db.Customer.Where(u => u.IDX != -1 && u.Name1 != "非會員客戶").Select(u => u).OrderByDescending(u => u.IDX).ToList<Customer>();
                int totalRecordNum = customers.Count;
                if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                int pageSize = rows;
                int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                //List<Customer> page_customers = customers.Skip((page - 1) * rows).Take(rows).OrderBy(u => u.IDX).ToList<Customer>();

                var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = customers };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        //
        // GET: /Customer/
        public ActionResult Index()
        {
            return View();
        }



        #region 计费方案 Written By lg
        public JsonResult GetInitServiceData()  //初始化收费项table
        {
            string[,] labels = null;//
            int i = 0;
            if (labels == null)
            {
                TugDataEntities db = new TugDataEntities();
                List<CustomField> list = db.CustomField.Where(u => u.CustomName == "OrderInfor.ServiceNatureID"
                    || (u.CustomName == "BillingItemTemplate.ItemID" && (u.CustomValue == "C78" || u.IDX == 40 || u.CustomLabel == "折扣"))
                    || (u.CustomName == "BillingItemTemplate.ItemID" && (u.CustomValue == "C82" || u.IDX == 23 || u.CustomLabel == "拖缆费"))
                    || (u.CustomName == "BillingItemTemplate.ItemID" && (u.CustomValue == "E80" || u.IDX == 119 || u.CustomLabel == "燃油附加费折扣")))
                    .OrderBy(u => u.CustomValue).ToList<CustomField>();
                labels = new string[list.Count, 4];
                foreach (var itm in list)
                {
                    labels[i, 0] = itm.IDX.ToString();
                    labels[i, 1] = itm.CustomLabel;
                    labels[i, 3] = "港幣";
                    i++;
                }
            }
            //return labels;
            return Json(labels, JsonRequestBehavior.AllowGet);

            //var jsonData = new[]
            //         {
            //             new[] {"","",""},
            //        };

            //return Json(jsonData, JsonRequestBehavior.AllowGet);
        }  

        public ActionResult BillingScheme(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            //ViewBag.ServiceLabels = CustomerLogic.GetBillingTemplateItems();
            int totalRecordNum, totalPageNum;
            List<Customer> list = GetCustomers(1, _DefaultPageSie, out totalRecordNum, out totalPageNum);
            ViewBag.TotalPageNum = totalPageNum;
            ViewBag.CurPage = 1;

            ViewBag.BillTemplateTypes = GetBillTemplateTypes();
            ViewBag.BillTemplateTimeTypes = GetBillTemplateTimeTypes();
            //ViewBag.BillTemplatePayItems = GetBillTemplatePayItems();
            ViewBag.BillTemplatePayItemPosition = GetBillTemplatePayItemPosition();

            //BusinessLogic.Module.FinanceLogic.GetFuelFee("2016-07-30");
            //BusinessLogic.Module.FinanceLogic.GetFuelFee("30.07.2016");

            //var ll = FinanceLogic.GetCustomersBillingTemplateByLength(40, 5000);
            //ll = FinanceLogic.GetCustomersBillingTemplateByLength(40, 7000);
            //ll = FinanceLogic.GetCustomersBillingTemplateByLength(40, 10000);
            //ll = FinanceLogic.GetCustomersBillingTemplateByLength(40, 12000);


            //ll = FinanceLogic.GetCustomersBillingTemplateByTEUS(40, 200);
            //ll = FinanceLogic.GetCustomersBillingTemplateByTEUS(40, 300);
            //ll = FinanceLogic.GetCustomersBillingTemplateByTEUS(40, 350);
            //ll = FinanceLogic.GetCustomersBillingTemplateByTEUS(40, 12000);

            //ll = FinanceLogic.GetCustomersBillingTemplateByLengthAndTEUS(40, 5000, 200);
            //ll = FinanceLogic.GetCustomersBillingTemplateByLengthAndTEUS(40, 5000, 300);
            //ll = FinanceLogic.GetCustomersBillingTemplateByLengthAndTEUS(40, 5000, 350);
            //ll = FinanceLogic.GetCustomersBillingTemplateByLengthAndTEUS(40, 5000, 12000);

            //ll = FinanceLogic.GetCustomersBillingTemplateByLengthAndTEUS(40, 7000, 200);
            //ll = FinanceLogic.GetCustomersBillingTemplateByLengthAndTEUS(40, 7000, 300);
            //ll = FinanceLogic.GetCustomersBillingTemplateByLengthAndTEUS(40, 7000, 350);
            //ll = FinanceLogic.GetCustomersBillingTemplateByLengthAndTEUS(40, 7000, 12000);

            //ll = FinanceLogic.GetCustomersBillingTemplateByLengthAndTEUS(40, 10000, 200);
            //ll = FinanceLogic.GetCustomersBillingTemplateByLengthAndTEUS(40, 10000, 300);
            //ll = FinanceLogic.GetCustomersBillingTemplateByLengthAndTEUS(40, 10000, 350);
            //ll = FinanceLogic.GetCustomersBillingTemplateByLengthAndTEUS(40, 10000, 12000);

            //ll = FinanceLogic.GetCustomersBillingTemplateByLengthAndTEUS(40, 12000, 200);
            //ll = FinanceLogic.GetCustomersBillingTemplateByLengthAndTEUS(40, 12000, 300);
            //ll = FinanceLogic.GetCustomersBillingTemplateByLengthAndTEUS(40, 12000, 350);
            //ll = FinanceLogic.GetCustomersBillingTemplateByLengthAndTEUS(40, 12000, 12000);

            return View(list);
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

            ViewBag.BillTemplateTypes = GetBillTemplateTypes();
            ViewBag.BillTemplateTimeTypes = GetBillTemplateTimeTypes();
            //ViewBag.BillTemplatePayItems = GetBillTemplatePayItems();
            ViewBag.BillTemplatePayItemPosition = GetBillTemplatePayItemPosition();

            return View("BillingScheme", list);
        }


        public List<Customer> GetCustomers(int curPage, int pageSize, out int totalRecordNum, out int totalPageNum, string queryName = "")
        {
            try
            {
                TugDataEntities db = new TugDataEntities();

                List<Customer> customers = null;
                if (queryName == "")
                {
                    customers = db.Customer.Select(u => u).OrderBy(u => u.Name1).ToList<Customer>();
                }
                else
                {
                    customers = db.Customer.Where(u => u.Name1.Contains(queryName) 
                        /*|| u.Name2.Contains(queryName) || u.SimpleName.Contains(queryName)*/)
                        .Select(u => u).OrderBy(u => u.Name1).ToList<Customer>();
                }

                totalRecordNum = customers.Count;
                //if (totalRecordNum % pageSize == 0) page -= 1;
                //int pageSize = rows;
                totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                List<Customer> page_customers = customers.Skip((curPage - 1) * pageSize).Take(pageSize).ToList<Customer>();
                return page_customers;
            }
            catch (Exception)
            {
                totalRecordNum = totalPageNum = 0;
                return null;
            }
        }


        public ActionResult GetCustomerBillSchemes(bool _search, string sidx, string sord, int page, int rows, int custId)
        {
            this.Internationalization();

            try
            {
                TugDataEntities db = new TugDataEntities();

                if (_search == true)
                {
                    string searchOption = Request.QueryString["filters"];
                    //List<V_OrderInfor> orders = db.V_OrderInfor.Where(u => u.IDX == -1).Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderInfor>();
                    List<V_BillingTemplate> orders = BusinessLogic.Module.CustomerLogic.SearchForCustomerBillingTemplate(sidx, sord, searchOption, custId);

                    int totalRecordNum = orders.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_BillingTemplate> page_orders = orders.Skip((page - 1) * rows).Take(rows).ToList<V_BillingTemplate>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //List<V_BillingTemplate> orders = db.V_BillingTemplate.Where(u => u.CustomerID == custId).Select(u => u).OrderByDescending(u => u.IDX).ToList<V_BillingTemplate>();
                    List<V_BillingTemplate> orders = BusinessLogic.Module.CustomerLogic.LoadDataForCustomerBillingTemplate(sidx, sord, custId);
                    int totalRecordNum = orders.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_BillingTemplate> page_orders = orders.Skip((page - 1) * rows).Take(rows).ToList<V_BillingTemplate>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        public ActionResult AddEditCustomerBillScheme(int custId)
        {
            //this.Internationalization();

            #region Add

            if (Request.Form["oper"].Equals("add"))
            {
                try
                {
                    //重名了
                    if (true == CheckBillingTemplateName(custId, Request.Form["BillingTemplateName"].Trim(), -1))
                    {
                        return Json(new { code = Resources.Common.FAIL_CODE, message = Resources.Common.FAIL_MESSAGE });
                    }
                    else
                    {
                        TugDataEntities db = new TugDataEntities();
                        {
                            DataModel.BillingTemplate cstmer = new BillingTemplate();

                            cstmer.BillingTemplateCode = Request.Form["BillingTemplateCode"].Trim();
                            cstmer.BillingTemplateName = Request.Form["BillingTemplateName"].Trim();
                            cstmer.BillingTemplateTypeID = Util.toint(Request.Form["BillingTemplateTypeID"].Trim());

                            cstmer.CreateDate = cstmer.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            cstmer.CustomerID = custId; // Convert.ToInt32(customerId);
                            cstmer.TemplateCreditContent = Request.Form["TemplateCreditContent"].Trim();
                            cstmer.TimeTypeID = Util.toint(Request.Form["TimeTypeID"].Trim());

                            cstmer.Discount = Convert.ToDouble(Request.Form["Discount"].Trim());

                            cstmer.Remark = Request.Form["Remark"].Trim();
                            cstmer.OwnerID = -1;
                            cstmer.UserID = Session.GetDataFromSession<int>("userid"); 
                            cstmer.UserDefinedCol1 = Request.Form["UserDefinedCol1"].Trim();
                            cstmer.UserDefinedCol2 = Request.Form["UserDefinedCol2"].Trim();
                            cstmer.UserDefinedCol3 = Request.Form["UserDefinedCol3"].Trim();
                            cstmer.UserDefinedCol4 = Request.Form["UserDefinedCol4"].Trim();

                            if (Request.Form["UserDefinedCol5"].Trim() != "")
                                cstmer.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"].Trim());

                            if (Request.Form["UserDefinedCol6"].Trim() != "")
                                cstmer.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"].Trim());

                            if (Request.Form["UserDefinedCol7"].Trim() != "")
                                cstmer.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"].Trim());

                            if (Request.Form["UserDefinedCol8"].Trim() != "")
                                cstmer.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"].Trim());

                            cstmer.UserDefinedCol9 = Request.Form["UserDefinedCol9"].Trim();
                            cstmer.UserDefinedCol10 = Request.Form["UserDefinedCol10"].Trim();

                            cstmer = db.BillingTemplate.Add(cstmer);
                            db.SaveChanges();

                            var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                            //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                            return Json(ret);
                        }
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

                    int idx = Util.toint(Request.Form["BillingTemplateIDX"]);
                    BillingTemplate cstmer = db.BillingTemplate.Where(u => u.IDX == idx).FirstOrDefault();

                    if (cstmer == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    //重名了
                    if (true == CheckBillingTemplateName(Util.toint(Request.Form["CustomerID"].Trim()), Request.Form["BillingTemplateName"].Trim(), idx))
                    {
                        return Json(new { code = Resources.Common.FAIL_CODE, message = Resources.Common.FAIL_MESSAGE });
                    }
                    else
                    {
                        cstmer.BillingTemplateCode = Request.Form["BillingTemplateCode"].Trim();
                        cstmer.BillingTemplateName = Request.Form["BillingTemplateName"].Trim();
                        cstmer.BillingTemplateTypeID = Util.toint(Request.Form["BillingTemplateTypeID"].Trim());

                        cstmer.CustomerID = Util.toint(Request.Form["CustomerID"].Trim());
                        cstmer.TemplateCreditContent = Request.Form["TemplateCreditContent"].Trim();
                        cstmer.TimeTypeID = Util.toint(Request.Form["TimeTypeID"].Trim());

                        cstmer.Discount = Convert.ToDouble(Request.Form["Discount"].Trim());
                        cstmer.ShipLength = (Request.Form["ShipLength"]);
                        cstmer.ShipTEUS = (Request.Form["ShipTEUS"]); 
                        cstmer.ExpiryDate = Request.Form["ExpiryDate"].Trim();

                        cstmer.Remark = Request.Form["Remark"].Trim();
                        cstmer.OwnerID = -1;
                        cstmer.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        cstmer.UserID = Session.GetDataFromSession<int>("userid"); 
                        cstmer.UserDefinedCol1 = Request.Form["UserDefinedCol1"].Trim();
                        cstmer.UserDefinedCol2 = Request.Form["UserDefinedCol2"].Trim();
                        cstmer.UserDefinedCol3 = Request.Form["UserDefinedCol3"].Trim();
                        cstmer.UserDefinedCol4 = Request.Form["UserDefinedCol4"].Trim();

                        if (Request.Form["UserDefinedCol5"].Trim() != "")
                            cstmer.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"].Trim());

                        if (Request.Form["UserDefinedCol6"].Trim() != "")
                            cstmer.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"].Trim());

                        if (Request.Form["UserDefinedCol7"].Trim() != "")
                            cstmer.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"].Trim());

                        if (Request.Form["UserDefinedCol8"].Trim() != "")
                            cstmer.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"].Trim());

                        cstmer.UserDefinedCol9 = Request.Form["UserDefinedCol9"].Trim();
                        cstmer.UserDefinedCol10 = Request.Form["UserDefinedCol10"].Trim();

                        db.Entry(cstmer).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });
                    }
                }
                catch (Exception)
                {
                    return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
                }
            }

            #endregion Edit

            return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
        }

        public ActionResult AddCustomerBillScheme(int custId, int billingTemplateTypeId, string billingTemplateCode, 
            string billingTemplateName, int timeTypeId, double discount, string shipLength, string shipTEUS, string expiryDate,
            string templateCreditContent, string remark, List<string[]> dataListFromTable)
        {
            try
            {
                //重名了
                if (true == CheckBillingTemplateName(custId, billingTemplateName, -1))
                {
                    return Json(new { code = Resources.Common.FAIL_CODE, message = Resources.Common.FAIL_MESSAGE });
                }
                else
                {
                    TugDataEntities db = new TugDataEntities();
                    DataModel.BillingTemplate cstmer = new BillingTemplate();

                    cstmer.BillingTemplateCode = billingTemplateCode;
                    cstmer.BillingTemplateName = billingTemplateName;
                    cstmer.BillingTemplateTypeID = billingTemplateTypeId;
                    cstmer.ShipLength = shipLength;
                    cstmer.ShipTEUS = shipTEUS;
                    cstmer.ExpiryDate = expiryDate;

                    cstmer.CreateDate = cstmer.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    cstmer.CustomerID = custId; // Convert.ToInt32(customerId);
                    cstmer.TemplateCreditContent = templateCreditContent;
                    cstmer.TimeTypeID = timeTypeId;
                    cstmer.Discount = discount;

                    cstmer.Remark = remark;
                    cstmer.OwnerID = -1;
                    cstmer.UserID = Session.GetDataFromSession<int>("userid"); 
                    cstmer.UserDefinedCol1 = "";
                    cstmer.UserDefinedCol2 = "";
                    cstmer.UserDefinedCol3 = "";
                    cstmer.UserDefinedCol4 = "";

                    //if (Request.Form["UserDefinedCol5"] != "")
                    //    cstmer.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                    //if (Request.Form["UserDefinedCol6"] != "")
                    //    cstmer.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                    //if (Request.Form["UserDefinedCol7"] != "")
                    //    cstmer.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                    //if (Request.Form["UserDefinedCol8"] != "")
                    //    cstmer.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                    cstmer.UserDefinedCol9 = "";
                    cstmer.UserDefinedCol10 = "";

                    cstmer = db.BillingTemplate.Add(cstmer);
                    db.SaveChanges();

                    //保存收费项
                    for (int i = 0; i < dataListFromTable.Count - 1; i++)//最后一行空行
                    {
                        if (Util.checkdbnull(dataListFromTable[i][2]) == "") continue;
                        DataModel.BillingItemTemplate aScheduler = new BillingItemTemplate();
                        aScheduler.BillingTemplateID = cstmer.IDX;
                        aScheduler.ItemID =Util.toint(dataListFromTable[i][0]);
                        aScheduler.UnitPrice = Util.tonumeric(dataListFromTable[i][2]);
                        aScheduler.Currency = dataListFromTable[i][3];
                        aScheduler.TypeID = 13;

                        aScheduler.OwnerID = -1;
                        aScheduler.UserID = Session.GetDataFromSession<int>("userid");

                        aScheduler.CreateDate = aScheduler.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        aScheduler.UserDefinedCol1 = "";
                        aScheduler.UserDefinedCol2 = "";
                        aScheduler.UserDefinedCol3 = "";
                        aScheduler.UserDefinedCol4 = "";

                        //if (Request.Form["UserDefinedCol5"] != "")
                        //    aScheduler.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        //if (Request.Form["UserDefinedCol6"] != "")
                        //    aScheduler.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        //if (Request.Form["UserDefinedCol7"] != "")
                        //    aScheduler.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        //if (Request.Form["UserDefinedCol8"] != "")
                        //    aScheduler.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        aScheduler.UserDefinedCol9 = "";
                        aScheduler.UserDefinedCol10 = "";

                        aScheduler = db.BillingItemTemplate.Add(aScheduler);
                        db.SaveChanges();
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

        [JsonExceptionFilterAttribute]
        public ActionResult DeleteCustomerBillScheme()
        {
            this.Internationalization();

            try
            {
                var f = Request.Form;

                int idx = Util.toint(Request.Form["data[BillingTemplateIDX]"]);

                TugDataEntities db = new TugDataEntities();
                //先判断此计费方案有没有被使用过
                Billing obj = db.Billing.FirstOrDefault(u => u.BillingTemplateID == idx);
                if (obj != null) throw new Exception("此計費方案已在賬單中使用過，不能被刪除！"); 
                BillingTemplate cstmer = db.BillingTemplate.FirstOrDefault(u => u.IDX == idx);
                if (cstmer != null)
                {
                    db.BillingTemplate.Remove(cstmer);
                    db.SaveChanges();
                    return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });
                }
                else
                {
                    return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                }
            }
            catch (Exception ex)
            {
                //return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
                throw ex;
            }
        }


        /// <summary>
        /// 检查模板名称是否重名
        /// </summary>
        /// <param name="custId">客户ID</param>
        /// <param name="billingTemplateName">数据的模板名称</param>
        /// <param name="billTemplateId">新增传-1， 编辑传模板ID</param>
        /// <returns>true：重名，false：不重名</returns>
        private bool CheckBillingTemplateName(int custId, string billingTemplateName, int billTemplateId = -1)
        {
            try
            {
                TugDataEntities db = new TugDataEntities();

                if (billTemplateId == -1) //新增
                {
                    int ret = 0;
                    ret = db.BillingTemplate.Where(u => u.CustomerID == custId && u.BillingTemplateName == billingTemplateName).Count();

                    return ret > 0;
                }
                else
                {
                    List<BillingTemplate> list = db.BillingTemplate.Where(u => u.CustomerID == custId && u.BillingTemplateName == billingTemplateName).ToList<BillingTemplate>();
                    if(list == null || list.Count <= 0)
                    {
                        return false;
                    }
                    else
                    {
                        if (billTemplateId == list[0].IDX) {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        public ActionResult GetCustomerBillSchemeItems(bool _search, string sidx, string sord, int page, int rows, int billSchemeId)
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
                    //List<V_BillingItemTemplate> orders = db.V_BillingItemTemplate.Where(u => u.BillingTemplateID == billSchemeId).Select(u => u).OrderByDescending(u => u.IDX).ToList<V_BillingItemTemplate>();
                    List<V_BillingItemTemplate> orders = BusinessLogic.Module.CustomerLogic.LoadDataForCustomerBillingItemTemplate(sidx, sord, billSchemeId);
                    int totalRecordNum = orders.Count;
                    
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    List<V_BillingItemTemplate> page_orders = orders.Skip((page - 1) * rows).Take(rows).ToList<V_BillingItemTemplate>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_orders };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult AddEditCustomerBillSchemeItem()
        {
            this.Internationalization();

            #region Add

            if (Request.Form["oper"].Equals("add"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    {
                        DataModel.BillingItemTemplate aScheduler = new BillingItemTemplate();
                        aScheduler.BillingTemplateID = Util.toint(Request.Form["BillingTemplateID"].Trim());
                        aScheduler.ItemID = Util.toint(Request.Form["ItemID"].Trim());
                        aScheduler.UnitPrice = Convert.ToDouble(Request.Form["UnitPrice"].Trim());
                        aScheduler.Currency = Request.Form["Currency".Trim()];
                        aScheduler.TypeID = Util.toint(Request.Form["TypeID"].Trim());
                        
                        aScheduler.OwnerID = -1;
                        aScheduler.UserID = Session.GetDataFromSession<int>("userid"); 

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

                        aScheduler = db.BillingItemTemplate.Add(aScheduler);
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
                    BillingItemTemplate aScheduler = db.BillingItemTemplate.Where(u => u.IDX == idx).FirstOrDefault();

                    if (aScheduler == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        aScheduler.BillingTemplateID = Util.toint(Request.Form["BillingTemplateID"].Trim());
                        aScheduler.ItemID = Util.toint(Request.Form["ItemID"].Trim());
                        aScheduler.UnitPrice = Convert.ToDouble(Request.Form["UnitPrice"].Trim());
                        aScheduler.Currency = Request.Form["Currency"].Trim();
                        aScheduler.TypeID = Util.toint(Request.Form["TypeID"].Trim());

                        
                        aScheduler.OwnerID = -1;
                        aScheduler.UserID = Session.GetDataFromSession<int>("userid"); 
                        aScheduler.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

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

        public ActionResult AddCustomerBillSchemeItem(int billingTemplateId, int itemId, string unitPrice, string currency, int typeId) 
        {
            this.Internationalization();

            try
            {
                TugDataEntities db = new TugDataEntities();
                {
                    DataModel.BillingItemTemplate aScheduler = new BillingItemTemplate();
                    aScheduler.BillingTemplateID = billingTemplateId;
                    aScheduler.ItemID = itemId;
                    aScheduler.UnitPrice = Convert.ToDouble(unitPrice);
                    aScheduler.Currency = currency;
                    aScheduler.TypeID = typeId;

                    aScheduler.OwnerID = -1;
                    aScheduler.UserID = Session.GetDataFromSession<int>("userid"); 

                    aScheduler.CreateDate = aScheduler.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    aScheduler.UserDefinedCol1 = "";
                    aScheduler.UserDefinedCol2 = "";
                    aScheduler.UserDefinedCol3 = "";
                    aScheduler.UserDefinedCol4 = "";

                    //if (Request.Form["UserDefinedCol5"] != "")
                    //    aScheduler.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                    //if (Request.Form["UserDefinedCol6"] != "")
                    //    aScheduler.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                    //if (Request.Form["UserDefinedCol7"] != "")
                    //    aScheduler.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                    //if (Request.Form["UserDefinedCol8"] != "")
                    //    aScheduler.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                    aScheduler.UserDefinedCol9 = "";
                    aScheduler.UserDefinedCol10 = "";

                    aScheduler = db.BillingItemTemplate.Add(aScheduler);
                    db.SaveChanges();

                    var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                    //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                    return Json(ret);
                }
            }
            catch (Exception ex)
            {
                var ret = new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE };
                //Response.Write(@Resources.Common.EXCEPTION_MESSAGE);
                return Json(ret);
            }
            return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
        }

        public ActionResult DeleteCustomerBillSchemeItem()
        {
            this.Internationalization();

            try
            {
                var f = Request.Form;

                int idx = Util.toint(Request.Form["data[IDX]"]);

                TugDataEntities db = new TugDataEntities();
                BillingItemTemplate aScheduler = db.BillingItemTemplate.FirstOrDefault(u => u.IDX == idx);
                if (aScheduler != null)
                {
                    db.BillingItemTemplate.Remove(aScheduler);
                    db.SaveChanges();
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

        public string GetPayItems()
        {
            string s = string.Empty;

            try
            {
                TugDataEntities db = new TugDataEntities();
                List<CustomField> list = db.CustomField.Where(u => u.CustomName == "OrderInfor.ServiceNatureID"
                    || u.CustomName == "BillingItemTemplate.ItemID").OrderBy(u => u.CustomValue).ToList<CustomField>();

                if (list != null && list.Count > 0)
                {
                    s += "<select><option value=-1~-1~请选择>请选择</option>";
                    foreach (CustomField item in list)
                    {
                        s += string.Format("<option value={0}>{1}</option>", item.IDX + "~" + item.CustomValue + "~" + item.CustomLabel, item.CustomLabel);
                    }
                    s += "</select>";
                }
            }
            catch (Exception ex)
            {
            }
            return s;
        }



        /// <summary>
        /// 得到计费模板类型
        /// </summary>
        /// <returns></returns>
        public List<CustomField> GetBillTemplateTypes()
        {
            TugDataEntities db = new TugDataEntities();
            List<CustomField> list = db.CustomField.Where(u => u.CustomName == "BillingTemplate.BillingTemplateType")
                .OrderBy(u => u.CustomValue).ToList<CustomField>();
            return list;
        }

        /// <summary>
        /// 得到计费模板的计时方式
        /// </summary>
        /// <returns></returns>
        public List<CustomField> GetBillTemplateTimeTypes()
        {
            TugDataEntities db = new TugDataEntities();
            List<CustomField> list = db.CustomField.Where(u => u.CustomName == "BillingTemplate.TimeTypeID")
                .OrderBy(u => u.CustomValue).ToList<CustomField>();
            return list;
        }



        /// <summary>
        /// 得到计费模板付费项目
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetBillTemplatePayItems(int billingTemplateId)
        {
            this.Internationalization();

            try
            {
                TugDataEntities db = new TugDataEntities();
                
                {
                    //已经有的
                    var existList = db.V_BillingItemTemplate.Where(u => u.BillingTemplateID == billingTemplateId);

                    //
                    List<CustomField> srcList = BusinessLogic.Module.CustomerLogic.GetPriceItems();

                    List<MyCustomField> list = new List<MyCustomField>();

                    if (srcList != null)
                    {
                        foreach (var item in srcList)
                        {
                            if (existList != null)
                            {
                                var o = existList.FirstOrDefault(u => u.ItemID == item.IDX || u.ItemValue == item.CustomValue || u.ItemLabel == item.CustomLabel);
                                if(o == null)
                                {
                                    MyCustomField mcf = new MyCustomField();
                                    mcf.IDX = item.IDX;
                                    mcf.CustomValue = item.CustomValue;
                                    mcf.CustomLabel = item.CustomLabel;
                                    mcf.FormulaStr = item.FormulaStr;
                                    list.Add(mcf);
                                }
                            }
                        }
                    }

                    return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE, list = list }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE }, JsonRequestBehavior.AllowGet);
            }           
        }


        /// <summary>
        /// 得到计费模板付费项目
        /// </summary>
        /// <returns></returns>
        public List<CustomField> GetBillTemplatePayItems()
        {

            TugDataEntities db = new TugDataEntities();
            List<CustomField> list = db.CustomField.Where(u => u.CustomName == "OrderInfor.ServiceNatureID"
                || (u.CustomName == "BillingItemTemplate.ItemID" && u.CustomValue.StartsWith("C8"))).OrderBy(u => u.CustomValue).ToList<CustomField>();

            return list;
        }

        /// <summary>
        /// 得到计费模板付费项目在发票中的位置：上中下
        /// </summary>
        /// <returns></returns>
        public List<CustomField> GetBillTemplatePayItemPosition()
        {

            TugDataEntities db = new TugDataEntities();
            List<CustomField> list = db.CustomField.Where(u => u.CustomName == "BillingItemTemplate.TypeID")
                .OrderBy(u => u.CustomValue).ToList<CustomField>();

            return list;
        }

        #endregion 计费方案 Written By lg
    }
}
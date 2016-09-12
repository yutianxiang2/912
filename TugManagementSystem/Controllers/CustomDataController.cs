using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogic;
using BusinessLogic.Module;
using DataModel;

namespace WMS.Controllers
{
    public class CustomEntity
    {
        public int IDX;
        public string Name;
        public string Label;
    }

    public class CustomDataController : BaseController
    {
        //
        // GET: /CommonData/
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult CustomDataManage(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            List<CustomEntity> list = new List<CustomEntity>();
            CustomEntity ctmobj=new CustomEntity();
            ctmobj.IDX = 0;
            ctmobj.Name = "OrderService.Location";
            ctmobj.Label = "作業位置";
            list.Add(ctmobj);

            //CustomEntity ctmobj1 = new CustomEntity();
            //ctmobj1.IDX = 1;
            //ctmobj1.Name = "OrderInfor.ServiceNatureID";
            //ctmobj1.Label = "服務內容";
            //list.Add(ctmobj1);

            ViewBag.TotalPageNum = 1;
            ViewBag.CurPage = 1;
            return View(list);
        }
        public ActionResult GetCustomData(bool _search, string sidx, string sord, int page, int rows, string ctmName)
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
                    List<CustomField> objs = db.CustomField.Where(u => u.CustomName == ctmName).Select(u => u).OrderByDescending(u => u.LastUpDate).ToList<CustomField>();
                    int totalRecordNum = objs.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    //List<CustomField> page_objs = objs.Skip((page - 1) * rows).Take(rows).ToList<CustomField>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = objs };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }
         [JsonExceptionFilterAttribute]
        public ActionResult AddEdit()
        {
            this.Internationalization();
            //int idx = Util.toint(Request.Form["IDX"]);
            //Fuelprice price = db.Fuelprice.Where(u => u.IDX == idx).FirstOrDefault();
            #region Add
            if (Request.Form["oper"].Equals("add"))
            {
                string CustomName = Request.Form["CustomName"].Trim();
                string CustomValue = Request.Form["CustomLabel"].Trim();

                try
                {
                    TugDataEntities db = new TugDataEntities();
                    {
                        DataModel.CustomField custom = new CustomField();
                        custom.CustomName = CustomName;
                        custom.CustomValue = "";
                        custom.CustomLabel = CustomValue;
                        custom.Description = "";
                        custom.UserID = Session.GetDataFromSession<int>("userid");
                        custom.CreateDate = custom.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd");
                        custom = db.CustomField.Add(custom);
                        db.SaveChanges();

                        var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
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
                    CustomField customedit = db.CustomField.Where(u => u.IDX == idx).FirstOrDefault();

                    string oldLocation = customedit.CustomLabel;
                    string newLocation = Request.Form["CustomLabel"].Trim();

                    string CustomName = customedit.CustomName;
                    string CustomValue = Request.Form["CustomLabel"].Trim();

                    CustomField custom = db.CustomField.Where(u => u.CustomName == CustomName && u.CustomLabel==CustomValue).FirstOrDefault();
                    if (custom != null)
                    {
                        throw new Exception(CustomValue + "已存在！");
                    }
                    else
                    {
                        customedit.CustomLabel = CustomValue;
                        customedit.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd");
                        customedit.UserID = Session.GetDataFromSession<int>("userid");
                        db.Entry(customedit).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        var list = db.OrderService.Where(u => u.ServiceWorkPlace.Trim().ToLower() == oldLocation.Trim().ToLower()).Select(u => u).ToList();
                        if (list != null)
                        {
                            foreach (var item in list)
                            {
                                item.ServiceWorkPlace = newLocation.Trim();
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
        public ActionResult AddCustomData(string CustomName, string CustomValue)
        {
            this.Internationalization();
            TugDataEntities db = new TugDataEntities();
            try
            {
            System.Linq.Expressions.Expression<Func<CustomField, bool>> exp = u => u.CustomName == CustomName && u.CustomLabel == CustomValue;
            CustomField tmpCustom = db.CustomField.Where(exp).FirstOrDefault();
            if (tmpCustom != null)
            {
                throw new Exception(CustomValue + "已存在！");
            }
            CustomField custom = new CustomField();
            custom.CustomName = CustomName;
            custom.CustomValue = "";
            custom.CustomLabel = CustomValue;
            custom.Description = "";
            custom.UserID = Session.GetDataFromSession<int>("userid");
            custom.CreateDate = custom.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 
            custom = db.CustomField.Add(custom);
                db.SaveChanges();
             return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult Delete()
        {
            this.Internationalization();

            try
            {
                var f = Request.Form;

                int idx = Util.toint(Request.Form["data[IDX]"]);

                TugDataEntities db = new TugDataEntities();
                CustomField custom = db.CustomField.FirstOrDefault(u => u.IDX == idx);
                if (custom != null)
                {
                    db.CustomField.Remove(custom);
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
	}
}
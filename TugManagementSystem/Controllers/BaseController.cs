using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WMS.MyClass;

namespace WMS.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 重写基类在Action之前执行的方法
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName == "Login") return;
            base.OnActionExecuting(filterContext);

            if (Session["userid"] == null || Session["username"] == null)
            {
                filterContext.Result = new RedirectResult("/Home/Login");
            }
        }
        public string Internationalization()
        {
            if (Request.Cookies["SelectedLanguage"] != null)
            {
                HttpCookie lanCookie = Request.Cookies["SelectedLanguage"];
                //从Cookie里面读取
                string language = lanCookie["lan"];
                //当前线程的语言采用哪种语言（比如zh，en等）
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
                //决定各种数据类型是如何组织，如数字与日期
                Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(language);
                return language;
            }
            else
            {
                HttpCookie lanCookie = new HttpCookie("SelectedLanguage");
                //默认为中文
                lanCookie["lan"] = "zh-HK";
                Response.Cookies.Add(lanCookie);
                return "zh-HK";
            }
        }

        // GET: Base
        public ActionResult SetLanguage(string lan)
        {
            if (string.IsNullOrEmpty(lan))
            {
                lan = "zh-HK";
            }
            ViewBag.Language = lan;
            HttpCookie lanCookie = Request.Cookies["SelectedLanguage"];
            lanCookie["lan"] = lan;
            Response.Cookies.Add(lanCookie);
            //刷新当前页面
            return Redirect(Request.UrlReferrer.ToString());
        }

        public string GetCustomField(string CustomName)
        {
            return BusinessLogic.Utils.GetCustomField(CustomName);
        }

        public string GetOrderStateOfOrderSchedulingPage(string CustomName)
        {
            string s = string.Empty;

            try
            {
                DataModel.TugDataEntities db = new DataModel.TugDataEntities();
                List<DataModel.CustomField> list = db.CustomField
                    .Where(u => u.CustomName == CustomName && (u.IDX == 2 || u.CustomLabel == "未排船" || u.IDX == 3 || u.CustomLabel == "已排船" || u.IDX == 5 || u.CustomLabel == "已完工"))
                    .OrderBy(u => u.CustomValue).ToList<DataModel.CustomField>();

                if (list != null && list.Count > 0)
                {
                    s += "<select><option value=-1~-1~请选择>请选择</option>";
                    foreach (DataModel.CustomField item in list)
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


        public string GetOrderStateOfJobInformationPage(string CustomName)
        {
            string s = string.Empty;

            try
            {
                DataModel.TugDataEntities db = new DataModel.TugDataEntities();
                List<DataModel.CustomField> list = db.CustomField
                    .Where(u => u.CustomName == CustomName && (u.IDX == 3 || u.CustomLabel == "已排船" || u.IDX == 5 || u.CustomLabel == "已完工"))
                    .OrderBy(u => u.CustomValue).ToList<DataModel.CustomField>();

                if (list != null && list.Count > 0)
                {
                    s += "<select><option value=-1~-1~请选择>请选择</option>";
                    foreach (DataModel.CustomField item in list)
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


        public string GetOrderStateOfInvoicePage(string CustomName)
        {
            string s = string.Empty;

            try
            {
                DataModel.TugDataEntities db = new DataModel.TugDataEntities();
                List<DataModel.CustomField> list = db.CustomField
                    .Where(u => u.CustomName == CustomName && (u.IDX == 5 || u.CustomLabel == "已完工"))
                    .OrderBy(u => u.CustomValue).ToList<DataModel.CustomField>();

                if (list != null && list.Count > 0)
                {
                    s += "<select><option value=-1~-1~请选择>请选择</option>";
                    foreach (DataModel.CustomField item in list)
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


        public string GetServiceNatures()
        {
            string s = string.Empty;

            try
            {
                DataModel.TugDataEntities db = new DataModel.TugDataEntities();
                List<DataModel.CustomField> list = db.CustomField
                    .Where(u => u.CustomName == "OrderInfor.ServiceNatureID")
                    .OrderBy(u => u.SortCode).ToList<DataModel.CustomField>();

                if (list != null && list.Count > 0)
                {
                    s += "<select>";
                    foreach (DataModel.CustomField item in list)
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

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}
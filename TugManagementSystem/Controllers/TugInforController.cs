using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataModel;
using BusinessLogic;
using BusinessLogic.Module;

namespace WMS.Controllers
{
    public class TugInforController : BaseController
    {

        public ActionResult TugStateBoard(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            return View();
        }

        [JsonExceptionFilterAttribute]
        public ActionResult AddTug(string Code, string Name1, string Name2, string SimpleName, string Power, string Class, string Speed, string Length, string Width, string Remark)
        {
            this.Internationalization();
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    System.Linq.Expressions.Expression<Func<TugInfor, bool>> exp = u => u.Name1 == Name1;
                    TugInfor tmpName = db.TugInfor.Where(exp).FirstOrDefault();
                    if (tmpName != null)
                    {
                        throw new Exception("拖轮名称1已存在！");
                    }
                    {
                        DataModel.TugInfor tug = new TugInfor();

                        tug.Code = Code;
                        tug.Name1 = Name1;
                        tug.Name2 = Name2;
                        tug.SimpleName = SimpleName;
                        tug.Power = Power;
                        tug.Class = Class;
                        tug.Speed = Speed;
                        tug.Length = Length;
                        tug. Width= Width;
                        tug.Remark =Remark ;
                        tug.OwnerID = -1;
                        tug.CreateDate = tug.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;
                        tug.UserID = Session.GetDataFromSession<int>("userid");
                        //tug.UserDefinedCol1 = "";
                        //tug.UserDefinedCol2 = "";
                        //tug.UserDefinedCol3 = "";
                        //tug.UserDefinedCol4 = "";

                        //if (Request.Form["UserDefinedCol5"] != "")
                        //    tug.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        //if (Request.Form["UserDefinedCol6"] != "")
                        //    tug.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        //if (Request.Form["UserDefinedCol7"] != "")
                        //    tug.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        //if (Request.Form["UserDefinedCol8"] != "")
                        //    tug.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        //tug.UserDefinedCol9 = "";
                        //tug.UserDefinedCol10 = "";

                        tug = db.TugInfor.Add(tug);
                        db.SaveChanges();

                        var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                        //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                        return Json(ret);
                    }
                }
                catch (Exception ex)
                {
                    //var ret = new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE };
                    //return Json(ret);
                    throw ex;
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
                        DataModel.TugInfor tug = new TugInfor();

                        tug.Code = Request.Form["Code"];
                        tug.Name1 = Request.Form["Name1"];
                        tug.Name2 = Request.Form["Name2"];
                        tug.SimpleName = Request.Form["SimpleName"];
                        tug.Power = Request.Form["Power"];
                        tug.Class = Request.Form["Class"];
                        tug.Speed = Request.Form["Speed"];
                        tug.Length = Request.Form["Length"];
                        tug.Width = Request.Form["Width"];
                        tug.Remark = Request.Form["Remark"];
                        tug.OwnerID = -1;
                        tug.CreateDate = tug.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
                        tug.UserID = Session.GetDataFromSession<int>("userid"); 
                        tug.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        tug.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        tug.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        tug.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            tug.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            tug.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            tug.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            tug.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        tug.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        tug.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        tug = db.TugInfor.Add(tug);
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
                    string name1 = Request.Form["Name1"];
                    int idx = Util.toint(Request.Form["IDX"]);
                    System.Linq.Expressions.Expression<Func<TugInfor, bool>> exp = u => u.Name1 == name1 && u.IDX != idx;
                    TugInfor tmpName = db.TugInfor.Where(exp).FirstOrDefault();
                    if (tmpName != null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = "拖轮名称1已存在！" });//Resources.Common.ERROR_MESSAGE
                    }


                    TugInfor tug = db.TugInfor.Where(u => u.IDX == idx).FirstOrDefault();

                    if (tug == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        tug.Code = Request.Form["Code"];
                        tug.Name1 = Request.Form["Name1"];
                        tug.Name2 = Request.Form["Name2"];
                        tug.SimpleName = Request.Form["SimpleName"];
                        tug.Power = Request.Form["Power"];
                        tug.Class = Request.Form["Class"];
                        tug.Speed = Request.Form["Speed"];
                        tug.Length = Request.Form["Length"];
                        tug.Width = Request.Form["Width"];
                        tug.Remark = Request.Form["Remark"];
                        tug.OwnerID = -1;
                        tug.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
                        tug.UserID = Session.GetDataFromSession<int>("userid"); 
                        tug.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        tug.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        tug.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        tug.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            tug.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            tug.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            tug.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            tug.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        tug.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        tug.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        db.Entry(tug).State = System.Data.Entity.EntityState.Modified;
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

        [JsonExceptionFilterAttribute]
        public ActionResult Delete()
        {
            this.Internationalization();

            try
            {
                var f = Request.Form;

                int idx = Util.toint(Request.Form["data[IDX]"]);

                TugDataEntities db = new TugDataEntities();
                //先判断该拖輪是否被使用过
                Scheduler obj = db.Scheduler.FirstOrDefault(u => u.TugID == idx);
                if (obj != null) throw new Exception("此拖輪已在服務調度中使用過，不能被刪除！"); 

                TugInfor tug = db.TugInfor.FirstOrDefault(u => u.IDX == idx);
                if (tug != null)
                {
                    db.TugInfor.Remove(tug);
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
        public ActionResult GetTugStateInfor(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();

            //string s = Request.QueryString[6];

            try
            {
                TugDataEntities db = new TugDataEntities();
                List<V_OrderScheduler> objs = db.V_OrderScheduler.Where(u => u.JobStateLabel !="已完工").Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderScheduler>();
                int totalRecordNum = objs.Count;
                if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                int pageSize = rows;
                int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                //List<V_OrderScheduler> page_V_OrderSchedulers = V_OrderSchedulers.Skip((page - 1) * rows).Take(rows).OrderBy(u => u.IDX).ToList<V_OrderScheduler>();

                var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = objs };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
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
                List<TugInfor> TugInfors = db.TugInfor.Select(u => u).OrderByDescending(u => u.IDX).ToList<TugInfor>();
                int totalRecordNum = TugInfors.Count;
                if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                int pageSize = rows;
                int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                //List<TugInfor> page_TugInfors = TugInfors.Skip((page - 1) * rows).Take(rows).OrderBy(u => u.IDX).ToList<TugInfor>();

                var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = TugInfors };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        //
        // GET: /TugInfor/
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult TugInforManage(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            return View();
        }


        #region written by lg
        public ActionResult GetTugEx(bool _search, string sidx, string sord, int page, int rows, string workDate, int orderServiceId)
        {
            this.Internationalization();

            try
            {
                TugDataEntities db = new TugDataEntities();
                List<TugInfor> TugInfors = db.TugInfor.Where(u=>u.UserDefinedCol1 != "是").Select(u => u).OrderByDescending(u => u.IDX).ToList<TugInfor>();

                int totalRecordNum = TugInfors.Count;
                if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                int pageSize = rows;
                int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                //List<TugInfor> page_TugInfors = TugInfors.Skip((page - 1) * rows).Take(rows).OrderBy(u => u.IDX).ToList<TugInfor>();


                List<BusinessLogic.TugEx> lst = new List<BusinessLogic.TugEx>();

                if (TugInfors != null)
                {
                    foreach (TugInfor tug in TugInfors)
                    {
                        BusinessLogic.TugEx o = new BusinessLogic.TugEx();
                        o.TugID = tug.IDX;
                        o.Name1 = tug.Name1;
                        o.Name2 = tug.Name2;
                        o.SimpleName = tug.SimpleName;
                        o.Code = tug.Code;

                        o = BusinessLogic.Module.OrderLogic.GetTugSchedulerBusyState(tug.IDX, o, workDate);

                        lst.Add(o);
                    }
                }


               List<int?> schedulers = db.Scheduler.Where(u => u.OrderServiceID == orderServiceId).Select(u => u.TugID).ToList();


                var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = lst, schedulers = schedulers };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }
        #endregion
    }
}
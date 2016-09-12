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
    public class OrganizationController : BaseController
    {
        public ActionResult Delete()
        {
            this.Internationalization();

            try
            {
                var f = Request.Form;

                int idx = Util.toint(Request.Form["data[IDX]"]);

                TugDataEntities db = new TugDataEntities();
                UserInfor usobj = db.UserInfor.FirstOrDefault(u => u.IDX == idx);
                if (usobj != null)
                {
                    db.UserInfor.Remove(usobj);
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

        //
        // GET: /UserInfor/
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadUsers(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();

            try
            {
                TugDataEntities db = new TugDataEntities();

                //db.Configuration.ProxyCreationEnabled = false;
                List<V_Users> objs = db.V_Users.Select(u => u).OrderByDescending(u => u.IDX).ToList<V_Users>();
                int totalRecordNum = objs.Count;
                if (totalRecordNum % rows == 0) page -= 1;
                int pageSize = rows;
                int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                //List<V_Users> page_objs = objs.Skip((page - 1) * rows).Take(rows).OrderBy(u => u.IDX).ToList<V_Users>();

                var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = objs };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        public JsonResult LoadUsersFilter(/*bool _search, string sidx, string sord, int page, int rows*/)
        {
            this.Internationalization();
            try
            {
                TugDataEntities db = new TugDataEntities();
                //string incode = "001002001";
                string incode = Request["incode"]; //"001002001"; //
                Console.WriteLine(incode);

                List<V_Users> objs = db.V_Users.Where(u => u.InCode.StartsWith(incode)).OrderByDescending(u => u.IDX).ToList<V_Users>();
                int totalRecordNum = objs.Count;
                //if (totalRecordNum % rows == 0) page -= 1;
                //int pageSize = rows;
                //int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                //List<V_Users> page_objs = objs.Skip((page - 1) * rows).Take(rows).OrderBy(u => u.IDX).ToList<V_Users>();

                //var jsonData = new { page = 1, records = totalRecordNum, total = totalPageNum, rows = objs };
                var jsonData = new { page = 1, records = totalRecordNum, total = 1, rows = objs };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult OrgAddEdit()
        {
            string newcode;
            int level;

            this.Internationalization();

            #region Add

            if (Request.Form["oper"].Equals("add"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    var fatherid = Request.Form["FatherID"];

                    if (fatherid == "")
                    {
                        newcode = NewInCode("O");
                        level = 0;
                    }
                    else
                    {
                        int curid = Util.toint(fatherid);
                        BaseTreeItems curobj;
                        curobj = db.BaseTreeItems.Where(u => u.IDX == curid).FirstOrDefault();
                        string curincode = curobj.InCode;
                        level = Util.toint(curobj.LevelValue) + 1;
                        newcode = NewInCode(curincode);
                    }
                    {
                        DataModel.BaseTreeItems obj = new BaseTreeItems();

                        obj.InCode = newcode;
                        if (fatherid != "") obj.FatherID = Util.toint(fatherid);
                        obj.LevelValue = level;
                        obj.IsLeaf = "true";
                        obj.Name1 = Request.Form["Name1"];
                        obj.Name2 = "";
                        obj.SType = "Organizion";
                        obj.SortNum = 0;
                        obj.Remark = "";
                        obj.OwnerID = -1;
                        obj.CreateDate = obj.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
                        obj.UserID = Session.GetDataFromSession<int>("userid"); 
                        obj.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        obj.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        obj.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        obj.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            obj.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            obj.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            obj.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            obj.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        obj.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        obj.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        obj = db.BaseTreeItems.Add(obj);
                        db.SaveChanges();

                        //将父节点的isleaf设为false
                        if (fatherid != "")
                        {
                            int fid = Util.toint(fatherid);
                            BaseTreeItems fobj = db.BaseTreeItems.Where(u => u.IDX == fid).FirstOrDefault();
                            fobj.IsLeaf = "false";
                            db.Entry(fobj).State = System.Data.Entity.EntityState.Modified;
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

            #endregion Add

            #region Edit

            if (Request.Form["oper"].Equals("edit"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();

                    int idx = Util.toint(Request.Form["id"]);
                    BaseTreeItems obj = db.BaseTreeItems.Where(u => u.IDX == idx).FirstOrDefault();

                    if (obj == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        //obj.InCode = "";
                        //obj.FatherID = System.DBNull.Value;
                        //obj.LevelValue = 0;
                        //obj.IsLeaf = "false";
                        obj.Name1 = Request.Form["Name1"];
                        //obj.Name2 = "";
                        //obj.SType = "Organizion";
                        //obj.SortNum = 0;
                        //obj.Remark = "";
                        //obj.OwnerID = -1;
                        obj.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
                        obj.UserID = Session.GetDataFromSession<int>("userid"); 
                        //obj.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        //obj.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        //obj.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        //obj.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        //if (Request.Form["UserDefinedCol5"] != "")
                        //    obj.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        //if (Request.Form["UserDefinedCol6"] != "")
                        //    obj.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        //if (Request.Form["UserDefinedCol7"] != "")
                        //    obj.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        //if (Request.Form["UserDefinedCol8"] != "")
                        //    obj.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        //obj.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        //obj.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
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

        public JsonResult OrgGetDataForLoadOnce(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();

            //string s = Request.QueryString[6];

            try
            {
                TugDataEntities db = new TugDataEntities();

                //db.Configuration.ProxyCreationEnabled = false;
                List<V_BaseTreeItems> objs = db.V_BaseTreeItems.Select(u => u).OrderBy(u => u.IDX).ToList<V_BaseTreeItems>();
                int totalRecordNum = objs.Count;
                if (totalRecordNum % rows == 0) page -= 1;
                int pageSize = rows;
                int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                //List<V_BaseTreeItems> page_objs = objs.Skip((page - 1) * rows).Take(rows).OrderBy(u => u.IDX).ToList<V_BaseTreeItems>();

                var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = objs };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }
        [Authorize]
        public ActionResult OrgManage(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            return View();
        }
        [Authorize]
        public ActionResult UserManage(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            return View();
        }

        [JsonExceptionFilterAttribute]
        public ActionResult AddUser(string UserName,string Dept,string Sec,string Name1,string Name2,string WorkNumber,string Sex,string Tel,string Email)
        {
            this.Internationalization();
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    {
                        System.Linq.Expressions.Expression<Func<UserInfor, bool>> exp = u => u.UserName == UserName;
                        UserInfor tmpUserName = db.UserInfor.Where(exp).FirstOrDefault();
                        if (tmpUserName != null)
                        {
                            //Response.StatusCode = 404;
                            //return Json(new { message = "用户名已存在！" });
                            throw new Exception("用户名已存在！");
                        }

                        System.Linq.Expressions.Expression<Func<UserInfor, bool>> exp1 = u => u.Name1 == Name1;
                        UserInfor tmpName1 = db.UserInfor.Where(exp).FirstOrDefault();
                        if (tmpName1 != null)
                        {
                            //Response.StatusCode = 404;
                            //var ret = new { message = "中文名已存在！" };
                            //return Json(ret);
                            throw new Exception("姓名1已存在！");
                        }

                        DataModel.UserInfor usobj = new UserInfor();
                        usobj.UserName =UserName;
                        usobj.Pwd = "123";
                        usobj.IsGuest = "false";
                        usobj.LanUserName ="" ;
                        usobj.Lan ="";
                        usobj.Dept =Dept;
                        usobj.Sec =Sec ;
                        usobj.Name1 =Name1 ;
                        usobj.Name2 = Name2;
                        usobj.WorkNumber =WorkNumber ;
                        usobj.Sex =Sex ;
                        usobj.Tel =Tel ;
                        usobj.Email = Email;
                        usobj.OwnerID = -1;
                        usobj.CreateDate = usobj.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;
                        usobj.AddUserID = Session.GetDataFromSession<int>("userid");
                        usobj.System = "Tug";
                        usobj.Status = -1;
                        usobj.LogIP = "";
                        usobj.UserDefinedCol1 = "";
                        usobj.UserDefinedCol2 = "";
                        usobj.UserDefinedCol3 = "";
                        usobj.UserDefinedCol4 = "";
                        //if (Request.Form["UserDefinedCol5"] != "")
                        //    usobj.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        //if (Request.Form["UserDefinedCol6"] != "")
                        //    usobj.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        //if (Request.Form["UserDefinedCol7"] != "")
                        //    usobj.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        //if (Request.Form["UserDefinedCol8"] != "")
                        //    usobj.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        usobj.UserDefinedCol9 = "";
                        usobj.UserDefinedCol10 = "";

                        usobj = db.UserInfor.Add(usobj);
                        db.SaveChanges();

                        var ret1 = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                        //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                        return Json(ret1);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
        }
        public ActionResult UserAddEdit()
        {
            this.Internationalization();
            var treeidx = Request["treeid"];

            if (treeidx == "")
            {
            }
            else
            {
            }

            #region Add

            if (Request.Form["oper"].Equals("add"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    {
                        DataModel.UserInfor usobj = new UserInfor();

                        usobj.UserName = Request.Form["UserName"];
                        usobj.Pwd = Request.Form["Pwd"];
                        usobj.IsGuest = Request.Form["IsGuest"];
                        usobj.LanUserName = Request.Form["LanUserName"];
                        usobj.Lan = Request.Form["Lan"];
                        usobj.Dept = Request.Form["Dept"];
                        usobj.Sec = Request.Form["Sec"];
                        usobj.Name1 = Request.Form["Name1"];
                        usobj.Name2 = Request.Form["Name2"];
                        usobj.WorkNumber = Request.Form["WorkNumber"];
                        usobj.Sex = Request.Form["Sex"];
                        usobj.Tel = Request.Form["Tel"];
                        usobj.Email = Request.Form["Email"];
                        usobj.OwnerID = -1;
                        usobj.CreateDate = usobj.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
                        usobj.AddUserID = Session.GetDataFromSession<int>("userid"); 
                        usobj.System = Request.Form["System"];
                        usobj.Status = -1;
                        usobj.LogIP = Request.Form["LogIP"];
                        usobj.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        usobj.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        usobj.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        usobj.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            usobj.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            usobj.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            usobj.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            usobj.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        usobj.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        usobj.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        usobj = db.UserInfor.Add(usobj);
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
                    string username = Request.Form["UserName"];
                    System.Linq.Expressions.Expression<Func<UserInfor, bool>> exp = u => u.UserName == username && u.IDX != idx;
                    UserInfor tmpUserName = db.UserInfor.Where(exp).FirstOrDefault();
                    if (tmpUserName != null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = "用户名已存在！" });//Resources.Common.ERROR_MESSAGE
                    }
                    string name1 = Request.Form["Name1"];
                    System.Linq.Expressions.Expression<Func<UserInfor, bool>> exp1 = u => u.UserName == username && u.IDX != idx;
                    UserInfor us = db.UserInfor.Where(exp1).FirstOrDefault();
                    if (us != null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = "姓名1已存在！" });//Resources.Common.ERROR_MESSAGE
                    }



                    UserInfor usobj = db.UserInfor.Where(u => u.IDX == idx).FirstOrDefault();

                    if (usobj == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        usobj.UserName = Request.Form["UserName"];
                        usobj.Pwd = Request.Form["Pwd"];
                        usobj.IsGuest = Request.Form["IsGuest"];
                        usobj.LanUserName = Request.Form["LanUserName"];
                        usobj.Lan = Request.Form["Lan"];
                        usobj.Dept = Request.Form["Dept"];
                        usobj.Sec = Request.Form["Sec"];
                        usobj.Name1 = Request.Form["Name1"];
                        usobj.Name2 = Request.Form["Name2"];
                        usobj.WorkNumber = Request.Form["WorkNumber"];
                        usobj.Sex = Request.Form["Sex"];
                        usobj.Tel = Request.Form["Tel"];
                        usobj.Email = Request.Form["Email"];
                        usobj.OwnerID = -1;
                        usobj.CreateDate = usobj.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
                        usobj.AddUserID = Session.GetDataFromSession<int>("userid"); 
                        usobj.System = Request.Form["System"];
                        usobj.Status = -1;
                        usobj.LogIP = Request.Form["LogIP"];
                        usobj.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        usobj.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        usobj.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        usobj.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            usobj.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            usobj.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            usobj.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            usobj.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        usobj.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        usobj.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        db.Entry(usobj).State = System.Data.Entity.EntityState.Modified;
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

        public ActionResult UserGetDataForLoadOnce(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();

            //string s = Request.QueryString[6];

            try
            {
                TugDataEntities db = new TugDataEntities();

                //db.Configuration.ProxyCreationEnabled = false;
                List<UserInfor> UserInfors = db.UserInfor.Select(u => u).OrderByDescending(u => u.IDX).ToList<UserInfor>();
                int totalRecordNum = UserInfors.Count;
                if (totalRecordNum % rows == 0) page -= 1;
                int pageSize = rows;
                int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                //List<UserInfor> page_UserInfors = UserInfors.Skip((page - 1) * rows).Take(rows).OrderBy(u => u.IDX).ToList<UserInfor>();

                var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = UserInfors };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        public ActionResult UserInforManage(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            return View();
        }

        private string NewInCode(string curInCode)
        {
            //获取结构树的已有筛选数据
            int SectionLen = 3;
            string error = null;
            string NewInCode;
            TugDataEntities db = new TugDataEntities();
            BaseTreeItems obj = new BaseTreeItems();
            System.Linq.Expressions.Expression<Func<BaseTreeItems, bool>> expression = u => u.InCode.StartsWith(curInCode) && u.InCode.Length == curInCode.Length + SectionLen;
            List<BaseTreeItems> objs = db.BaseTreeItems.Where(expression).OrderByDescending(u => u.IDX).ToList<BaseTreeItems>();
            if (objs.Count == 0)
            {
                NewInCode = curInCode + string.Format("{0:D" + SectionLen + "}", 1);
            }
            else
            {
                string No;
                string maxInCode;
                maxInCode = objs.First().InCode.ToString();
                No = maxInCode.Substring(maxInCode.Length - SectionLen);
                NewInCode = curInCode + string.Format("{0:D" + SectionLen + "}", Convert.ToInt32(No) + 1);
            }
            return NewInCode;
        }
    }
}
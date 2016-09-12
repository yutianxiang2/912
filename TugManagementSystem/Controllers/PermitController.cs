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
    public class PermitController : BaseController
    {
        private static int _DefaultPageSie = 7;

        #region 角色

        [HttpGet]
        public ActionResult GetRoles(int curPage, string queryName = "")
        {
            ViewBag.Language = this.Internationalization();

            int totalRecordNum, totalPageNum;
            List<Role> list = GetRoles(curPage, _DefaultPageSie, out totalRecordNum, out totalPageNum, queryName);
            ViewBag.TotalPageNum = totalPageNum;
            ViewBag.CurPage = curPage;
            ViewBag.QueryName = queryName;

            return View("RoleUserManage", list);
        }

        public List<Role> GetRoles(int curPage, int pageSize, out int totalRecordNum, out int totalPageNum, string queryName = "")
        {
            try
            {
                TugDataEntities db = new TugDataEntities();

                List<Role> Roles = null;
                if (queryName == "")
                {
                    Roles = db.Role.Select(u => u).OrderByDescending(u => u.IDX).ToList<Role>();
                }
                else
                {
                    Roles = db.Role.Where(u => u.RoleName.Contains(queryName))
                        .Select(u => u).OrderByDescending(u => u.IDX).ToList<Role>();
                }

                totalRecordNum = Roles.Count;
                //if (totalRecordNum % pageSize == 0) page -= 1;
                //int pageSize = rows;
                totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                List<Role> page_Roles = Roles.Skip((curPage - 1) * pageSize).Take(pageSize).ToList<Role>();
                return page_Roles;
            }
            catch (Exception)
            {
                totalRecordNum = totalPageNum = 0;
                return null;
            }
        }

        #endregion 角色

        #region 添加角色用户
        public ActionResult AddRoleUser(List<int> data, int roleId)
        {
            this.Internationalization();
            TugDataEntities db = new TugDataEntities();
            foreach (int id in data)
            {
                System.Linq.Expressions.Expression<Func<UsersRole, bool>> exp = u => u.UserID == id && u.RoleID==roleId;
                UsersRole tmpUserRole = db.UsersRole.Where(exp).FirstOrDefault();
                if(tmpUserRole != null) continue;  //判断该用户是否已存在

                UsersRole userRole = new UsersRole();
                userRole.UserID = id;
                userRole.RoleID = roleId;
                userRole.IsAdmin = "否";
                userRole.OwnerID = -1;
                userRole.CreateDate = userRole.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                userRole.AddUserID = Session.GetDataFromSession<int>("userid");
                userRole.System = "Role";
                userRole.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                userRole.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                userRole.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                userRole.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                if (Request.Form["UserDefinedCol5"] != "")
                    userRole.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                if (Request.Form["UserDefinedCol6"] != "")
                    userRole.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                if (Request.Form["UserDefinedCol7"] != "")
                    userRole.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                if (Request.Form["UserDefinedCol8"] != "")
                    userRole.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                userRole.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                userRole.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                userRole = db.UsersRole.Add(userRole);
                db.SaveChanges();
            }
            return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });
        }
        #endregion 添加角色用户

        #region 添加角色模块
        public ActionResult AddRoleModule(List<int> data, int roleId)
        {
            this.Internationalization();
            TugDataEntities db = new TugDataEntities();
            foreach (int id in data)
            {
                System.Linq.Expressions.Expression<Func<RoleModule, bool>> exp = u => u.ModuleID == id && u.RoleID == roleId;
                RoleModule tmpUserModule = db.RoleModule.Where(exp).FirstOrDefault();
                if (tmpUserModule != null) continue;  //判断该模块是否已存在

                RoleModule roleModule = new RoleModule();
                roleModule.ModuleID = id;
                roleModule.RoleID = roleId;
                roleModule.IsAdmin = "否";
                roleModule.OwnerID = -1;
                roleModule.CreateDate = roleModule.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                roleModule.UserID = Session.GetDataFromSession<int>("userid");
                roleModule.System = "Role";
                roleModule.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                roleModule.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                roleModule.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                roleModule.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                if (Request.Form["UserDefinedCol5"] != "")
                    roleModule.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                if (Request.Form["UserDefinedCol6"] != "")
                    roleModule.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                if (Request.Form["UserDefinedCol7"] != "")
                    roleModule.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                if (Request.Form["UserDefinedCol8"] != "")
                    roleModule.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                roleModule.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                roleModule.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                roleModule = db.RoleModule.Add(roleModule);
                db.SaveChanges();
            }
            return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });
        }
        #endregion 添加角色模块

        #region 角色页面Action

        public ActionResult AddEditRole()
        {
            this.Internationalization();

            #region Add

            if (Request.Form["oper"].Equals("add"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    {
                        DataModel.Role userRole = new Role();

                        userRole.RoleName = Request.Form["RoleName"];
                        userRole.Dept = Request.Form["Dept"];
                        userRole.System = Request.Form["System"];
                        userRole.Remark = Request.Form["Remark"];
                        userRole.OwnerID = -1;
                        userRole.CreateDate = userRole.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;
                        userRole.UserID = Session.GetDataFromSession<int>("userid");
                        userRole.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        userRole.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        userRole.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        userRole.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            userRole.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            userRole.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            userRole.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            userRole.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        userRole.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        userRole.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        userRole = db.Role.Add(userRole);
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
                    Role userRole = db.Role.Where(u => u.IDX == idx).FirstOrDefault();

                    if (userRole == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        userRole.RoleName = Request.Form["RoleName"];
                        userRole.Dept = Request.Form["Dept"];
                        userRole.System = Request.Form["System"];
                        userRole.Remark = Request.Form["Remark"];
                        userRole.OwnerID = -1;
                        userRole.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;
                        userRole.UserID = Session.GetDataFromSession<int>("userid");

                        userRole.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        userRole.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        userRole.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        userRole.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            userRole.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            userRole.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            userRole.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            userRole.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        userRole.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        userRole.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        db.Entry(userRole).State = System.Data.Entity.EntityState.Modified;
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

        public ActionResult DeleteRole()
        {
            this.Internationalization();

            try
            {
                var f = Request.Form;

                int idx = Util.toint(Request.Form["data[IDX]"]);

                TugDataEntities db = new TugDataEntities();
                Role userRole = db.Role.FirstOrDefault(u => u.IDX == idx);
                if (userRole != null)
                {
                    db.Role.Remove(userRole);
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
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = "该角色已关联用户或模块，无法删除！" });
            }
        }

        public ActionResult GetRoleData(bool _search, string sidx, string sord, int page, int rows)
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
                    List<Role> objs = db.Role.Select(u => u).OrderByDescending(u => u.IDX).ToList<Role>();
                    int totalRecordNum = objs.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    //List<Role> page_objs = objs.Skip((page - 1) * rows).Take(rows).ToList<Role>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = objs };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        [Authorize]
        public ActionResult RoleManage(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;

            ViewBag.Services = BusinessLogic.Utils.GetServices();

            return View();
        }

        #endregion 角色页面Action

        #region 模块页面Action

        public ActionResult AddEditModule()
        {
            this.Internationalization();

            #region Add

            if (Request.Form["oper"].Equals("add"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    {
                        DataModel.FunctionModule userRole = new FunctionModule();

                        userRole.ModuleCode = Request.Form["ModuleCode"];
                        userRole.ModuleName = Request.Form["ModuleName"];
                        userRole.System = Request.Form["System"];
                        userRole.Remark = Request.Form["Remark"];
                        //userRole.OwnerID = -1;
                        //userRole.CreateDate = userRole.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
                        //userRole.UserID = Session.GetDataFromSession<int>("userid");
                        userRole.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        userRole.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        userRole.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        userRole.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            userRole.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            userRole.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            userRole.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            userRole.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        userRole.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        userRole.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        userRole = db.FunctionModule.Add(userRole);
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
                    FunctionModule userRole = db.FunctionModule.Where(u => u.IDX == idx).FirstOrDefault();

                    if (userRole == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        userRole.ModuleCode = Request.Form["ModuleCode"];
                        userRole.ModuleName = Request.Form["ModuleName"];
                        userRole.System = Request.Form["System"];
                        userRole.Remark = Request.Form["Remark"];
                        //userRole.OwnerID = -1;
                        //userRole.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
                        //userRole.UserID = Session.GetDataFromSession<int>("userid");

                        userRole.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        userRole.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        userRole.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        userRole.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            userRole.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            userRole.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            userRole.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            userRole.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        userRole.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        userRole.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        db.Entry(userRole).State = System.Data.Entity.EntityState.Modified;
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

        public ActionResult DeleteModule()
        {
            this.Internationalization();

            try
            {
                var f = Request.Form;

                int idx = Util.toint(Request.Form["data[IDX]"]);

                TugDataEntities db = new TugDataEntities();
                FunctionModule userRole = db.FunctionModule.FirstOrDefault(u => u.IDX == idx);
                if (userRole != null)
                {
                    db.FunctionModule.Remove(userRole);
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

        public ActionResult GetModuleDataForOnce(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();

            //string s = Request.QueryString[6];

            try
            {
                TugDataEntities db = new TugDataEntities();

                //db.Configuration.ProxyCreationEnabled = false;
                List<FunctionModule> FunctionModules = db.FunctionModule.Select(u => u).OrderByDescending(u => u.IDX).ToList<FunctionModule>();
                int totalRecordNum = FunctionModules.Count;
                if (totalRecordNum % rows == 0) page -= 1;
                int pageSize = rows;
                int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                //List<FunctionModule> page_FunctionModules = FunctionModules.Skip((page - 1) * rows).Take(rows).OrderBy(u => u.IDX).ToList<FunctionModule>();

                var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = FunctionModules };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        public ActionResult GetModuleData(bool _search, string sidx, string sord, int page, int rows)
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
                    List<FunctionModule> objs = db.FunctionModule.Select(u => u).OrderByDescending(u => u.IDX).ToList<FunctionModule>();
                    int totalRecordNum = objs.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    //List<FunctionModule> page_objs = objs.Skip((page - 1) * rows).Take(rows).ToList<FunctionModule>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = objs };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        [Authorize]
        public ActionResult ModuleManage(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;

            ViewBag.Services = BusinessLogic.Utils.GetServices();

            return View();
        }

        #endregion 模块页面Action

        #region 角色人页面Action

        public ActionResult AddEditRowUser(int rolId)
        {
            //this.Internationalization();

            #region Add

            if (Request.Form["oper"].Equals("add"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    {
                        DataModel.UsersRole userRole = new UsersRole();

                        userRole.UserID = Util.toint(Request.Form["UserID"]);
                        userRole.RoleID = rolId;
                        userRole.IsAdmin = Request.Form["IsAdmin"];
                        userRole.System = "Role";
                        userRole.CreateDate = userRole.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;

                        userRole.OwnerID = -1;
                        userRole.AddUserID = Session.GetDataFromSession<int>("userid");
                        userRole.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        userRole.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        userRole.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        userRole.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            userRole.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            userRole.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            userRole.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            userRole.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        userRole.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        userRole.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        userRole = db.UsersRole.Add(userRole);
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
                    UsersRole userRole = db.UsersRole.Where(u => u.IDX == idx).FirstOrDefault();

                    if (userRole == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        userRole.UserID = Util.toint(Request.Form["UserID"]);
                        userRole.RoleID = Util.toint(Request.Form["RoleID"]);
                        userRole.IsAdmin = Request.Form["IsAdmin"];
                        userRole.System = "Role";
                        userRole.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;

                        userRole.OwnerID = -1;
                        userRole.AddUserID = Session.GetDataFromSession<int>("userid");
                        userRole.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        userRole.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        userRole.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        userRole.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            userRole.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            userRole.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            userRole.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            userRole.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        userRole.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        userRole.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        db.Entry(userRole).State = System.Data.Entity.EntityState.Modified;
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

        public ActionResult DeleteRoleUser()
        {
            this.Internationalization();

            try
            {
                var f = Request.Form;

                int idx = Util.toint(Request.Form["data[IDX]"]);

                TugDataEntities db = new TugDataEntities();
                UsersRole userRole = db.UsersRole.FirstOrDefault(u => u.IDX == idx);
                if (userRole != null)
                {
                    db.UsersRole.Remove(userRole);
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

        public ActionResult GetRowUsers(bool _search, string sidx, string sord, int page, int rows, int rolId)
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
                    List<V_RoleUser> objs = db.V_RoleUser.Where(u => u.RoleID == rolId).Select(u => u).OrderByDescending(u => u.UserID).ToList<V_RoleUser>();
                    int totalRecordNum = objs.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    //List<V_RoleUser> page_objs = objs.Skip((page - 1) * rows).Take(rows).ToList<V_RoleUser>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = objs };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        [Authorize]
        public ActionResult RoleUserManage(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;

            int totalRecordNum, totalPageNum;
            List<Role> list = GetRoles(1, _DefaultPageSie, out totalRecordNum, out totalPageNum);
            ViewBag.TotalPageNum = totalPageNum;
            ViewBag.CurPage = 1;

            return View(list);
        }

        #endregion 角色人页面Action

        #region 角色模块Action

        public ActionResult DeleteRoleModule()
        {
            this.Internationalization();

            try
            {
                var f = Request.Form;

                int idx = Util.toint(Request.Form["data[IDX]"]);

                TugDataEntities db = new TugDataEntities();
                RoleModule userRole = db.RoleModule.FirstOrDefault(u => u.IDX == idx);
                if (userRole != null)
                {
                    db.RoleModule.Remove(userRole);
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

        public ActionResult AddEditRowModule(int rolId)
        {
            //this.Internationalization();

            #region Add

            if (Request.Form["oper"].Equals("add"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    {
                        DataModel.RoleModule userRole = new RoleModule();
                        userRole.RoleID = rolId;
                        userRole.ModuleID = Util.toint(Request.Form["ModuleID"]);

                        userRole.IsAdmin = Request.Form["IsAdmin"];
                        userRole.System = "Role";
                        userRole.CreateDate = userRole.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;

                        userRole.OwnerID = -1;
                        userRole.UserID = Session.GetDataFromSession<int>("userid");
                        userRole.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        userRole.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        userRole.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        userRole.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            userRole.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            userRole.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            userRole.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            userRole.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        userRole.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        userRole.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        userRole = db.RoleModule.Add(userRole);
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
                    RoleModule userRole = db.RoleModule.Where(u => u.IDX == idx).FirstOrDefault();

                    if (userRole == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        userRole.RoleID = rolId;
                        userRole.ModuleID = Util.toint(Request.Form["ModuleID"]);

                        userRole.IsAdmin = Request.Form["IsAdmin"];
                        userRole.System = "Role";
                        userRole.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;

                        userRole.OwnerID = -1;
                        userRole.UserID = Session.GetDataFromSession<int>("userid");
                        userRole.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        userRole.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        userRole.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        userRole.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            userRole.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            userRole.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            userRole.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            userRole.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        userRole.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        userRole.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        db.Entry(userRole).State = System.Data.Entity.EntityState.Modified;
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

        public ActionResult GetRowModules(bool _search, string sidx, string sord, int page, int rows, int rolId)
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
                    List<V_RoleModule> objs = db.V_RoleModule.Where(u => u.RoleID == rolId).Select(u => u).OrderByDescending(u => u.ModuleID).ToList<V_RoleModule>();
                    int totalRecordNum = objs.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    //List<V_RoleModule> page_objs = objs.Skip((page - 1) * rows).Take(rows).ToList<V_RoleModule>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = objs };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        [Authorize]
        public ActionResult RoleModuleManage(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;

            int totalRecordNum, totalPageNum;
            List<Role> list = GetRoles(1, _DefaultPageSie, out totalRecordNum, out totalPageNum);
            ViewBag.TotalPageNum = totalPageNum;
            ViewBag.CurPage = 1;

            return View(list);
        }

        #endregion 角色模块Action

        #region 角色菜单模块Action

        [Authorize]
        public ActionResult RoleMenuManage(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;

            int totalRecordNum, totalPageNum;
            List<Role> list = GetRoles(1, _DefaultPageSie, out totalRecordNum, out totalPageNum);
            ViewBag.TotalPageNum = totalPageNum;
            ViewBag.CurPage = 1;

            return View(list);
        }

        public ActionResult GetRowMenus(bool _search, string sidx, string sord, int page, int rows, int rolId)
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
                    List<V_RoleMenu> objs = db.V_RoleMenu.Where(u => u.RoleID == rolId).Select(u => u).OrderByDescending(u => u.MenuName).ToList<V_RoleMenu>();
                    int totalRecordNum = objs.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                    //List<V_RoleMenu> page_objs = objs.Skip((page - 1) * rows).Take(rows).ToList<V_RoleMenu>();

                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = objs };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        public ActionResult AddEditRowMenu(int rolId)
        {
            //this.Internationalization();

            #region Add

            if (Request.Form["oper"].Equals("add"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    {
                        DataModel.RoleMenu userRole = new RoleMenu();

                        userRole.Page = Request.Form["Page"];
                        userRole.Menu = Request.Form["Menu"];
                        userRole.MenuName = Request.Form["MenuName"];
                        userRole.Visible = Request.Form["Visible"];
                        userRole.IsAdmin = Request.Form["IsAdmin"];
                        userRole.RoleID = rolId;

                        userRole.Remark = Request.Form["Remark"];
                        userRole.System = "Role";
                        //userRole.CreateDate = userRole.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
                        //userRole.OwnerID = -1;
                        //userRole.AddUserID = Session.GetDataFromSession<int>("userid");
                        userRole.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        userRole.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        userRole.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        userRole.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            userRole.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            userRole.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            userRole.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            userRole.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        userRole.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        userRole.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        userRole = db.RoleMenu.Add(userRole);
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
                    RoleMenu userRole = db.RoleMenu.Where(u => u.IDX == idx).FirstOrDefault();

                    if (userRole == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        userRole.Page = Request.Form["Page"];
                        userRole.Menu = Request.Form["Menu"];
                        userRole.MenuName = Request.Form["MenuName"];
                        userRole.Visible = Request.Form["Visible"];
                        userRole.IsAdmin = Request.Form["IsAdmin"];
                        userRole.RoleID = rolId;

                        userRole.Remark = Request.Form["Remark"];
                        userRole.System = "Role";
                        userRole.UserDefinedCol1 = Request.Form["UserDefinedCol1"];
                        userRole.UserDefinedCol2 = Request.Form["UserDefinedCol2"];
                        userRole.UserDefinedCol3 = Request.Form["UserDefinedCol3"];
                        userRole.UserDefinedCol4 = Request.Form["UserDefinedCol4"];

                        if (Request.Form["UserDefinedCol5"] != "")
                            userRole.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

                        if (Request.Form["UserDefinedCol6"] != "")
                            userRole.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                        if (Request.Form["UserDefinedCol7"] != "")
                            userRole.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                        if (Request.Form["UserDefinedCol8"] != "")
                            userRole.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                        userRole.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
                        userRole.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

                        db.Entry(userRole).State = System.Data.Entity.EntityState.Modified;
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

        public ActionResult DeleteRoleMenu()
        {
            this.Internationalization();

            try
            {
                var f = Request.Form;

                int idx = Util.toint(Request.Form["data[IDX]"]);

                TugDataEntities db = new TugDataEntities();
                RoleMenu userRole = db.RoleMenu.FirstOrDefault(u => u.IDX == idx);
                if (userRole != null)
                {
                    db.RoleMenu.Remove(userRole);
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

        #endregion 角色菜单模块Action

        //
        // GET: /Permit/
        public ActionResult Index()
        {
            return View();
        }

         [JsonExceptionFilterAttribute]
        public ActionResult AddRole(string RoleName, string Dept, string System, string Remark)
        {
            this.Internationalization();
            try
            {
                TugDataEntities db = new TugDataEntities();
                {
                    System.Linq.Expressions.Expression<Func<Role, bool>> exp = u => u.RoleName == RoleName;
                    Role tmprole = db.Role.Where(exp).FirstOrDefault();
                    if (tmprole != null)
                    {
                        //Response.StatusCode = 404;
                        //return Json(new { message = RoleName + "已存在！" });
                        throw new Exception(RoleName + "已存在！");
                    }

                    DataModel.Role role = new Role();
                    role.RoleName = RoleName;
                    role.Dept = Dept;
                    role.System = System;
                    role.Remark = Remark;
                    role.OwnerID = -1;
                    role.CreateDate = role.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;//.ToString("yyyy-MM-dd");
                    role.UserID = Session.GetDataFromSession<int>("userid");
                    role.UserDefinedCol1 = "";
                    role.UserDefinedCol2 = "";
                    role.UserDefinedCol3 = "";
                    role.UserDefinedCol4 = "";
                    //if (Request.Form["UserDefinedCol5"].Trim() != "")
                    //    role.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"].Trim());

                    //if (Request.Form["UserDefinedCol6"].Trim() != "")
                    //    role.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"].Trim());

                    //if (Request.Form["UserDefinedCol7"].Trim() != "")
                    //    role.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"].Trim());

                    //if (Request.Form["UserDefinedCol8"].Trim() != "")
                    //    role.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"].Trim());

                    role.UserDefinedCol9 = "";
                    role.UserDefinedCol10 = "";

                    role = db.Role.Add(role);
                    db.SaveChanges();

                    var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                    return Json(ret);
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

         [JsonExceptionFilterAttribute]
        public ActionResult AddModule(string ModuleCode, string ModuleName, string System, string Remark)
        {
            this.Internationalization();
            try
            {
                    TugDataEntities db = new TugDataEntities();
                    System.Linq.Expressions.Expression<Func<FunctionModule, bool>> exp = u => u.ModuleName == ModuleName;
                    FunctionModule tmpmodule = db.FunctionModule.Where(exp).FirstOrDefault();
                    if (tmpmodule!=null)
                    {
                        //Response.StatusCode = 404;
                        //return Json(new { message = ModuleName + "已存在！" });
                        throw new Exception(ModuleName + "已存在！");
                    }
                    
                    DataModel.FunctionModule module = new FunctionModule();
                    module.ModuleCode = ModuleCode;
                    module.ModuleName = ModuleName;
                    module.System = System;
                    module.Remark = Remark;
                    //module.UserID = Session.GetDataFromSession<int>("userid");
                    module.UserDefinedCol1 = "";
                    module.UserDefinedCol2 = "";
                    module.UserDefinedCol3 = "";
                    module.UserDefinedCol4 = "";

                    module.UserDefinedCol9 = "";
                    module.UserDefinedCol10 = "";

                    module = db.FunctionModule.Add(module);
                    db.SaveChanges();

                    var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
                    return Json(ret);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #region 编辑Action
        //public ActionResult RoleAddEdit()
        //{
        //    this.Internationalization();

        //    #region Add

        //    if (Request.Form["oper"].Equals("add"))
        //    {
        //        try
        //        {
        //            TugDataEntities db = new TugDataEntities();
        //            {
        //                DataModel.Role role = new Role();
        //                role.RoleName = Request.Form["RoleName"];
        //                role.Dept = Request.Form["Dept"];
        //                role.System = Request.Form["System"];
        //                role.Remark = Request.Form["Remark"];
        //                role.OwnerID = -1;
        //                role.CreateDate = role.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;//.ToString("yyyy-MM-dd");
        //                role.UserID = Session.GetDataFromSession<int>("userid");
        //                role.UserDefinedCol1 = "";
        //                role.UserDefinedCol2 = "";
        //                role.UserDefinedCol3 = "";
        //                role.UserDefinedCol4 = "";
        //                if (Request.Form["UserDefinedCol5"] != "")
        //                    role.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

        //                if (Request.Form["UserDefinedCol6"] != "")
        //                    role.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

        //                if (Request.Form["UserDefinedCol7"] != "")
        //                    role.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

        //                if (Request.Form["UserDefinedCol8"] != "")
        //                    role.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

        //                role.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
        //                role.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

        //                role = db.Role.Add(role);
        //                db.SaveChanges();

        //                var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
        //                Response.Write(@Resources.Common.SUCCESS_MESSAGE);
        //                return Json(ret);
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            var ret = new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE };
        //            //Response.Write(@Resources.Common.EXCEPTION_MESSAGE);
        //            return Json(ret);
        //        }
        //    }

        //    #endregion Add

        //    #region Edit

        //    if (Request.Form["oper"].Equals("edit"))
        //    {
        //        try
        //        {
        //            TugDataEntities db = new TugDataEntities();

        //            int idx = Util.toint(Request.Form["IDX"]);
        //            Role role = db.Role.Where(u => u.IDX == idx).FirstOrDefault();

        //            if (role == null)
        //            {
        //                return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
        //            }
        //            else
        //            {
        //                role.RoleName = Request.Form["RoleName"];
        //                role.Dept = Request.Form["Dept"];
        //                role.System = Request.Form["System"];
        //                role.Remark = Request.Form["Remark"];
        //                role.OwnerID = -1;
        //                role.CreateDate = role.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;//.ToString("yyyy-MM-dd");
        //                role.UserID = Session.GetDataFromSession<int>("userid");
        //                role.UserDefinedCol1 = "";
        //                role.UserDefinedCol2 = "";
        //                role.UserDefinedCol3 = "";
        //                role.UserDefinedCol4 = "";

        //                if (Request.Form["UserDefinedCol5"] != "")
        //                    role.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

        //                if (Request.Form["UserDefinedCol6"] != "")
        //                    role.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

        //                if (Request.Form["UserDefinedCol7"] != "")
        //                    role.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

        //                if (Request.Form["UserDefinedCol8"] != "")
        //                    role.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

        //                role.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
        //                role.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

        //                db.Entry(role).State = System.Data.Entity.EntityState.Modified;
        //                db.SaveChanges();

        //                return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });
        //            }
        //        }
        //        catch (Exception exp)
        //        {
        //            return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
        //        }
        //    }

        //    #endregion Edit

        //    return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
        //}

        //public ActionResult ModuleAddEdit()
        //{
        //    this.Internationalization();

        //    #region Add

        //    if (Request.Form["oper"].Equals("add"))
        //    {
        //        try
        //        {
        //            TugDataEntities db = new TugDataEntities();
        //            {
        //                DataModel.FunctionModule module = new FunctionModule();
        //                module.ModuleCode = Request.Form["ModuleCode"];
        //                module.ModuleName = Request.Form["ModuleName"];
        //                module.System = Request.Form["System"];
        //                module.Remark = Request.Form["Remark"];
        //                //module.UserID = Session.GetDataFromSession<int>("userid");
        //                module.UserDefinedCol1 = "";
        //                module.UserDefinedCol2 = "";
        //                module.UserDefinedCol3 = "";
        //                module.UserDefinedCol4 = "";
        //                if (Request.Form["UserDefinedCol5"] != "")
        //                    module.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

        //                if (Request.Form["UserDefinedCol6"] != "")
        //                    module.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

        //                if (Request.Form["UserDefinedCol7"] != "")
        //                    module.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

        //                if (Request.Form["UserDefinedCol8"] != "")
        //                    module.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

        //                module.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
        //                module.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

        //                module = db.FunctionModule.Add(module);
        //                db.SaveChanges();

        //                var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE };
        //                Response.Write(@Resources.Common.SUCCESS_MESSAGE);
        //                return Json(ret);
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            var ret = new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE };
        //            //Response.Write(@Resources.Common.EXCEPTION_MESSAGE);
        //            return Json(ret);
        //        }
        //    }

        //    #endregion Add

        //    #region Edit

        //    if (Request.Form["oper"].Equals("edit"))
        //    {
        //        try
        //        {
        //            TugDataEntities db = new TugDataEntities();

        //            int idx = Util.toint(Request.Form["IDX"]);
        //            FunctionModule module = db.FunctionModule.Where(u => u.IDX == idx).FirstOrDefault();

        //            if (module == null)
        //            {
        //                return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
        //            }
        //            else
        //            {
        //                module.ModuleCode = Request.Form["ModuleCode"];
        //                module.ModuleName = Request.Form["ModuleName"];
        //                module.System = Request.Form["System"];
        //                module.Remark = Request.Form["Remark"];
        //                //module.UserID = Session.GetDataFromSession<int>("userid");
        //                module.UserDefinedCol1 = "";
        //                module.UserDefinedCol2 = "";
        //                module.UserDefinedCol3 = "";
        //                module.UserDefinedCol4 = "";

        //                if (Request.Form["UserDefinedCol5"] != "")
        //                    module.UserDefinedCol5 = Convert.ToDouble(Request.Form["UserDefinedCol5"]);

        //                if (Request.Form["UserDefinedCol6"] != "")
        //                    module.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

        //                if (Request.Form["UserDefinedCol7"] != "")
        //                    module.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

        //                if (Request.Form["UserDefinedCol8"] != "")
        //                    module.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

        //                module.UserDefinedCol9 = Request.Form["UserDefinedCol9"];
        //                module.UserDefinedCol10 = Request.Form["UserDefinedCol10"];

        //                db.Entry(module).State = System.Data.Entity.EntityState.Modified;
        //                db.SaveChanges();

        //                return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });
        //            }
        //        }
        //        catch (Exception exp)
        //        {
        //            return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
        //        }
        //    }

        //    #endregion Edit

        //    return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
        //}
        #endregion 编辑Action
    }
}
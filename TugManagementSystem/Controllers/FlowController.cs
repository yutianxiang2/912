using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using BusinessLogic.Module;
using DataModel;
using BusinessLogic;

namespace WMS.Controllers
{
    public class FlowController : BaseController
    {
        #region handsontable方式实现，模态框中下拉、日期均可实现
        public ActionResult testHandsontable(string lan, int? id)  //复杂版，显示组织结构，人员
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            return View();
        }
        public ActionResult FlowView_Handsontable(string lan, int? id)  
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            return View();
        }
        string[] nodes;
        public ActionResult GetNodes()
        {

            int i = 0; 
            if(nodes==null)
            {
                TugDataEntities db = new TugDataEntities();
                List<CustomField> list = db.CustomField.Where(u => u.CustomName == "Task.Node").OrderBy(u => u.CustomValue).ToList<CustomField>();
                nodes=new string[list.Count];
                foreach (var itm in list)
                {
                    nodes[i] = itm.CustomLabel;
                    i++;
                }
            }
            return Json(nodes, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPersons()
        {
            //TugDataEntities db = new TugDataEntities();
            //List<UserInfor> users = db.UserInfor.Select(u => u).OrderBy(u => u.Name1).ToList<UserInfor>();
            //List<object> list = new List<object>();

            //if (users != null)
            //{
            //    foreach (UserInfor item in users)
            //    {
            //        list.Add(new { UserID = item.IDX, UserName1 = item.Name1 });
            //    }
            //}

            //var jsonData = new { list = list };
            //return Json(jsonData, JsonRequestBehavior.AllowGet);       



            int i = 0;
            string[] persons;
            TugDataEntities db = new TugDataEntities();
            List<UserInfor> list = db.UserInfor.Where(u => u.IsGuest != "true").OrderBy(u => u.Name1).ToList<UserInfor>();
            persons = new string[list.Count];
            foreach (var itm in list)
            {
                persons[i] = itm.Name1;
                i++;
            }
            return Json(persons, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UserValid(string vlperson)
        {
            bool isvalid = false;
            TugDataEntities db = new TugDataEntities();
            UserInfor us = db.UserInfor.Where(u => u.Name1 == vlperson).FirstOrDefault();
            if (us == null)
                isvalid = false;
            else
                isvalid = true;
            var ret = new { code = Resources.Common.SUCCESS_CODE, rvalid =isvalid };
            return Json(ret);
        }
        //BillingType:0普通账单，1特殊账单
        public ActionResult SubmitFlow(int BillingType,string billids,List<string[]> dataListFromTable)
        {
            //billid 从Invoice页面传入
            //int[] arrbillid =new int[2]{1,2};
            string[] arrbillid = billids.Split(',');
            try
            {
                TugDataEntities db = new TugDataEntities();
                foreach (string sidx in arrbillid)
                {
                    int idx = Util.toint(sidx);
                    Billing billobj = db.Billing.Where(u => u.IDX == idx).FirstOrDefault();
                    if (billobj == null)
                    {
                        continue ;
                        //return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    int newTimesNo =Util.toint(billobj.TimesNo) + 1;  //第几次流程
                    //将流程信息写入Flow表
                    #region 
                    string mark = Util.GetSequence("F");
                    for(int i=0;i<dataListFromTable.Count-1;i++)//最后一行空行
                    {
                        DataModel.Flow obj = new Flow();
                        obj.BillingID = idx;
                        obj.MarkID = newTimesNo;
                        obj.Phase = i;
                        obj.Task =dataListFromTable[i][0] ;
                        string Name11=dataListFromTable[i][1];
                        UserInfor us1 = db.UserInfor.Where(u => u.Name1 == Name11).FirstOrDefault();
                        obj.FlowUserID = us1.IDX;                        
                        obj.StDate = dataListFromTable[i][2];
                        obj.EndDate =dataListFromTable[i][3] ;
                        obj.System = "Tug";
                        obj.OwnerID = -1;
                        obj.CreateDate = obj.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
                        obj.UserID = Session.GetDataFromSession<int>("userid"); 
                        //obj.State =0;
                        obj.Sign =mark;
                        //obj.UserDefinedCol1 = "";
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
                        obj = db.Flow.Add(obj);
                        db.SaveChanges();
                    }  
#endregion

                    //将提交审核写入Approve
                    #region
                    DataModel.Approve objapp = new Approve();
                    objapp.BillingID = idx;
                    objapp.FlowMark = newTimesNo;
                    objapp.Phase = 0;
                    objapp.Task = dataListFromTable[0][0];
                    objapp.Accept=2;
                    objapp.Remark="";
                    string Name12 = dataListFromTable[0][1];
                    UserInfor us2 = db.UserInfor.Where(u => u.Name1 == Name12).FirstOrDefault();
                    objapp.PersonID = us2.IDX;
                    objapp.System="Tug";
                    objapp.OwnerID=-1;
                    objapp.CreateDate = objapp.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
                    objapp.UserID = Session.GetDataFromSession<int>("userid"); 
                    objapp = db.Approve.Add(objapp);
                    db.SaveChanges();
                    #endregion

                    //更新Billing表的流程信息
                    #region
                    billobj.TimesNo = newTimesNo;
                    billobj.Status = dataListFromTable[1][0];
                    billobj.Phase = 1;
                    db.Entry(billobj).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    #endregion
                    //更新orderinfor、orderservice表中的HasInFlow
                    //OrderLogic.UpdateHasInFlow(idx, "是");
                    FinanceLogic.SetOrderServiceFlowingStatus(BillingType, idx, "是");
                }

            }
            catch (Exception exp)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
            return Json(new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE });
        }
        public JsonResult GetInitData()
        {
            var uName1=Session.GetDataFromSession<string>("Name1");
            var jsonData = new[]
                     {
                         new[] {"创建","" + uName1,"",""},
                         new[] {"审核", "", "",""}
                    };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }    
        #endregion

        #region jqgrid方式实现，有问题：模态框中下拉、日期不能正常显示
        public ActionResult AddEdit()
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

                    int idx = Util.toint(Request.Form["IDX"]);
                    BaseTreeItems obj = db.BaseTreeItems.Where(u => u.IDX == idx).FirstOrDefault();

                    if (obj == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        obj.InCode = "";
                        //obj.FatherID = System.DBNull.Value;
                        obj.LevelValue = 0;
                        obj.IsLeaf = "false";
                        obj.Name1 = Request.Form["Name1"];
                        obj.Name2 = "";
                        obj.SType = "Organizion";
                        obj.SortNum = 0;
                        obj.Remark = "";
                        obj.OwnerID = -1;
                        obj.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
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

        public ActionResult AddFlow(string lan, int? id)  //复杂版，显示组织结构，人员
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            return View();
        }

        public ActionResult CreateFlow(string lan, int? id) //只是流程节点的管理
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            return View();
        }

        public ActionResult FlowAddEdit()
        {
            this.Internationalization();

            #region Add

            if (Request.Form["oper"].Equals("add"))
            {
                try
                {
                    TugDataEntities db = new TugDataEntities();
                    {
                        DataModel.Flow obj = new Flow();

                        obj.BillingID = -1;
                        obj.MarkID = -1;
                        obj.Phase = -1;
                        obj.Task = Request.Form["Task"];
                        obj.FlowUserID = Util.toint(Request.Form["FlowUserID"]);
                        obj.StDate = Request.Form["StDate"];
                        obj.EndDate = Request.Form["EndDate"];
                        obj.System = "Billing";
                        obj.OwnerID = -1;
                        obj.CreateDate = obj.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
                        obj.UserID = Session.GetDataFromSession<int>("userid"); 
                        obj.State = -1;
                        obj.Sign = "";
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

                        obj = db.Flow.Add(obj);
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
                    Flow obj = db.Flow.Where(u => u.IDX == idx).FirstOrDefault();

                    if (obj == null)
                    {
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE });
                    }
                    else
                    {
                        obj.BillingID = -1;
                        obj.MarkID = -1;
                        obj.Phase = -1;
                        obj.Task = Request.Form["Task"];
                        obj.FlowUserID = Util.toint(Request.Form["FlowUserID"]);
                        obj.StDate = Request.Form["StDate"];
                        obj.EndDate = Request.Form["EndDate"];
                        obj.System = "Billing";
                        obj.OwnerID = -1;
                        obj.CreateDate = obj.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");;
                        obj.UserID = Session.GetDataFromSession<int>("userid"); 
                        obj.State = -1;
                        obj.Sign = "";
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

        public ActionResult FlowDelete()
        {
            this.Internationalization();

            try
            {
                var f = Request.Form;

                int idx = Util.toint(Request.Form["data[IDX]"]);

                TugDataEntities db = new TugDataEntities();
                Flow obj = db.Flow.FirstOrDefault(u => u.IDX == idx);
                if (obj != null)
                {
                    db.Flow.Remove(obj);
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

        public ActionResult GetUser(string term)
        {
            List<object> source = new List<object>();
            source.Add(new { FlowUserID = "123", Name1 = "张三" });
            source.Add(new { FlowUserID = "234", Name1 = "李四" });
            source.Add(new { FlowUserID = "345", Name1 = "王五" });
            source.Add(new { FlowUserID = "456", Name1 = "赵六" });

            var p = Request.Params;

            List<object> list = new List<object>();

            list.Add(source[0]);
            list.Add(source[1]);
            list.Add(source[2]);
            list.Add(source[3]);

            var jsonData = new { list = list };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Flow/
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadFlow(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();

            try
            {
                TugDataEntities db = new TugDataEntities();

                //db.Configuration.ProxyCreationEnabled = false;
                List<V_Flow> objs = db.V_Flow.Select(u => u).OrderByDescending(u => u.IDX).ToList<V_Flow>();
                int totalRecordNum = objs.Count;
                if (totalRecordNum % rows == 0) page -= 1;
                int pageSize = rows;
                int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                //List<V_Flow> page_objs = objs.Skip((page - 1) * rows).Take(rows).OrderBy(u => u.IDX).ToList<V_Flow>();

                var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = objs };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        public JsonResult LoadOrganizationOnce(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();

            //string s = Request.QueryString[6];

            try
            {
                TugDataEntities db = new TugDataEntities();

                //db.Configuration.ProxyCreationEnabled = false;
                List<V_BaseTreeItems> objs = db.V_BaseTreeItems.Select(u => u).OrderBy(u => u.IDX).ToList<V_BaseTreeItems>();
                int totalRecordNum = objs.Count;
                if (page != 0 && totalRecordNum % rows == 0) page -= 1;
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
                //if (page != 0 && totalRecordNum % rows == 0) page -= 1;
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
        #endregion
    }
}
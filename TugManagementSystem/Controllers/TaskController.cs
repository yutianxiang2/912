using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic;
using BusinessLogic.Module;
using DataModel;

namespace WMS.Controllers
{
    public class NeedApprove
    {
        int IDX;
	    string InvoiceType ;
	    string  CustomerName ;
	    string ShipName;
	    string JobNo ;
	    string BillingCode ;
	    string BillingTemplateTypeLabel ;
        string TimeTypeValue;
	    string TimeTypeLabel ;
	    float Amount ;
	    string Status ;
	    string Remark ;
	    string CreateDate ;
	    string LastUpDate ;
        int MarkID ;
        int Phase ;
        string Task;
        int FlowUserID;
        string System;
    }
    public class Approved
    {
        int IDX;
        string InvoiceType;
        string CustomerName;
        string ShipName;
        string JobNo;
        string BillingCode;
        string BillingTemplateTypeLabel;
        string TimeTypeValue;
        string TimeTypeLabel;
        float Amount;
        string Status;
        string Remark;
        string CreateDate;
        string LastUpDate;
    }
    public class TaskController : BaseController
    {
        #region 待审核

        public ActionResult GetTaskData(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();

            try
            {
                int curUserId = 0;
                TugDataEntities db = new TugDataEntities();
                curUserId = Session.GetDataFromSession<int>("userid");
                    if (_search == true)
                    {
                        string s = Request.QueryString["filters"];
                        return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var objs = db.proc_needapprove(curUserId).ToList();
                        //List<NeedApprove> objs = new List<NeedApprove>();
                        //SqlParameter[] prams = new SqlParameter[1];
                        //prams[0] = new SqlParameter("@userID", curUserId);
                        //objs = db.Database.SqlQuery<NeedApprove>("exec dbo.proc_needapprove @userID", prams).ToList();
                        int totalRecordNum = objs.Count;
                        if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                        int pageSize = rows;
                        int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);
                        var page_objs = objs.Skip((page - 1) * rows).Take(rows).ToList();
                        var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_objs };
                        return Json(jsonData, JsonRequestBehavior.AllowGet);     
                    }
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        public ActionResult NeedCheck(string lan, int? id)
        {
            lan = this.Internationalization();
            ViewBag.Language = lan;
            ViewBag.Services = BusinessLogic.Utils.GetServices();
            Session.SetDataInSession<string>("HomePage", "/Task/NeedCheck");
            return View();
        }

        #endregion 待审核

        #region 已审核

        public ActionResult Checked(string lan, int? id)
        {
            lan = this.Internationalization();
            return View();
        }

        public ActionResult GetCheckedData(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();
            int curUserId = 0;
             curUserId = Session.GetDataFromSession<int>("userid");
             if (_search == true)
             {
                 string s = Request.QueryString["filters"];
                 return Json(new { code = Resources.Common.ERROR_CODE, message = Resources.Common.ERROR_MESSAGE }, JsonRequestBehavior.AllowGet);
             }
             else
             {
                 try
                 {
                    TugDataEntities db = new TugDataEntities();
                    var objs = db.proc_approved(curUserId).ToList();
                    int totalRecordNum = objs.Count;
                    if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                    int pageSize = rows;
                    int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);
                    var page_objs = objs.Skip((page - 1) * rows).Take(rows).ToList();
                    var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = page_objs };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);  
                 }
                 catch (Exception)
                 {                     
                     throw;
                 }

             }
              
            //try
            //{
            //    int curUserId = 0;
            //    TugDataEntities db = new TugDataEntities();
            //    //System.Linq.Expressions.Expression<Func<UserInfor, bool>> exp = u => u.UserName == User.Identity.Name;
            //    //UserInfor curUser = db.UserInfor.Where(exp).FirstOrDefault();

            //    curUserId = Session.GetDataFromSession<int>("userid");   //當前用戶ID
            //        List<Approve> ApproveList = db.Approve.Where(u => u.PersonID == curUserId).Select(u => u).ToList<Approve>();
            //        if (ApproveList.Count != 0)
            //        {
            //            //List<Billing> BillList = db.Billing.Where(u => u.IDX == -1).Select(u => u).ToList<Billing>();
            //            List<V_OrderBilling> BillList = db.V_OrderBilling.Where(u => u.BillingID == -1).Select(u => u).ToList<V_OrderBilling>();

            //            foreach (Approve obj in ApproveList)
            //            {
            //                if (Convert.ToInt32(obj.Accept) > 2) continue;
            //                //System.Linq.Expressions.Expression<Func<Billing, bool>> expB = u => u.IDX == obj.BillingID;
            //                //Billing billData = db.Billing.Where(expB).FirstOrDefault();
            //                System.Linq.Expressions.Expression<Func<V_OrderBilling, bool>> expB = u => u.BillingID == obj.BillingID;
            //                V_OrderBilling billData = db.V_OrderBilling.Where(expB).FirstOrDefault();

            //                if (billData != null)
            //                {
            //                    //撤销提交的为待提交任务
            //                    if (Convert.ToInt32(billData.Phase) == 0 && billData.Status == "已撤销提交") continue;
            //                    //驳回或撤销通过的为待完成任务
            //                    if (Convert.ToInt32(billData.Phase) == 0 && billData.Status.ToString().Length >= 3) continue;
            //                    //BillList.Add(billData);
            //                    BillList.Add(billData);
            //                }

            //            }
            //            int totalRecordNum = BillList.Count;
            //            if (page != 0 && totalRecordNum % rows == 0) page -= 1;
            //            int pageSize = rows;
            //            int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);
            //            //List<V_OrderBilling> page_objs = BillList.Skip((page - 1) * rows).Take(rows).ToList<V_OrderBilling>();
            //            var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = BillList };
            //            return Json(jsonData, JsonRequestBehavior.AllowGet);
            //        }
            //        else
            //        {
            //            List<V_OrderBilling> BillList = db.V_OrderBilling.Where(u => u.BillingID == -1).Select(u => u).ToList<V_OrderBilling>();
            //            int totalRecordNum = BillList.Count;
            //            if (page != 0 && totalRecordNum % rows == 0) page -= 1;
            //            int pageSize = rows;
            //            int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);
            //            //List<V_OrderBilling> page_objs = BillList.Skip((page - 1) * rows).Take(rows).ToList<V_OrderBilling>();
            //            var jsonData = new { page = page, records = totalRecordNum, total = totalPageNum, rows = BillList };
            //            return Json(jsonData, JsonRequestBehavior.AllowGet);
            //        }
            //        //List<V_NeedApproveBilling> objs = db.V_NeedApproveBilling.Where(u => u.FlowUserID == curUserId).OrderByDescending(u => u.IDX).ToList<V_NeedApproveBilling>();

            //        //int totalRecordNum1 = objs.Count;
            //        //if (page != 0 && totalRecordNum1 % rows == 0) page -= 1;
            //        //int pageSize1 = rows;
            //        //int totalPageNum1 = (int)Math.Ceiling((double)totalRecordNum1 / pageSize1);
            //        //List<V_NeedApproveBilling> page_objs1 = objs.Skip((page - 1) * rows).Take(rows).ToList<V_NeedApproveBilling>();
            //        //var jsonData1 = new { page = page, records = totalRecordNum1, total = totalPageNum1, rows = page_objs1 };
            //        //return Json(jsonData1, JsonRequestBehavior.AllowGet);
               
            //}
            //catch (Exception)
            //{
            //    return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            //}
        }

        #endregion 已审核

        #region 通过


        public ActionResult ApprovePass(List<int> passdata)
        {
            //var ids = Request.Form["data"];
            int curUserId = 0;
            TugDataEntities db = new TugDataEntities();
            curUserId = Session.GetDataFromSession<int>("userid");
            foreach (int id in passdata)
            {
                System.Linq.Expressions.Expression<Func<Billing, bool>> exp = u => u.IDX == id;
                Billing billInfor = db.Billing.Where(exp).FirstOrDefault();

                //写入Approve表
                Approve addApprove = new Approve();
                addApprove.BillingID = id;
                addApprove.FlowMark = billInfor.TimesNo;
                addApprove.Phase = billInfor.Phase;
                addApprove.Task = Task(id, Convert.ToInt32(billInfor.Phase), Convert.ToInt32(billInfor.TimesNo));
                addApprove.Accept = 1;
                addApprove.PersonID = curUserId;
                addApprove.UserID = curUserId;
                addApprove.CreateDate = DateTime.Now.ToShortDateString();
                addApprove.LastUpDate = DateTime.Now.ToShortDateString();
                addApprove = db.Approve.Add(addApprove);
                db.SaveChanges();

                //判断是不是流程最后一步
                System.Linq.Expressions.Expression<Func<Flow, bool>> expFlow = u => u.BillingID == id && u.MarkID == billInfor.TimesNo;
                List<Flow> flowData = db.Flow.Where(expFlow).Select(u => u).ToList<Flow>();
                if (billInfor.Phase + 1 == flowData.Count)  //流程最后一步
                {
                    string billingCode = BusinessLogic.Utils.AutoGenerateBillCode(); 
                    //更改Billing状态
                    billInfor.Phase = -1;
                    billInfor.Status = "完成";
                    billInfor.BillingCode = billingCode;  //生成账单编号
                    db.Entry(billInfor).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //若账单有回扣单生成回扣单编号
                     System.Linq.Expressions.Expression<Func<Credit, bool>> expCredit = u => u.BillingID == id;
                     List<Credit> tmpCredit = db.Credit.Where(expCredit).Select(u => u).ToList<Credit>();
                     //Credit tmpCredit = db.Credit.Where(expCredit).FirstOrDefault();
                     if (tmpCredit.Count  != 0)
                     {
                         foreach (var item in tmpCredit)
                         {
                             item.CreditCode = "C" + billingCode.Substring(1, billingCode.Length - 1);
                             db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                             db.SaveChanges();
                         }
                     }
                }
                else
                {
                    //更改Billing状态
                    billInfor.Phase = billInfor.Phase + 1;
                    billInfor.Status = Task(id, Convert.ToInt32(billInfor.Phase), Convert.ToInt32(billInfor.TimesNo));
                    db.Entry(billInfor).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json(new { message = "审核完成！" });
        }

        private static string Task(int tID, int tPhase, int MarkID)
        {
            TugDataEntities db = new TugDataEntities();
            System.Linq.Expressions.Expression<Func<Flow, bool>> expF = u => u.BillingID == tID && u.MarkID == MarkID && u.Phase == tPhase;
            Flow curFlow = db.Flow.Where(expF).FirstOrDefault();
            return curFlow.Task;
        }

        #endregion 通过

        #region 驳回

        public ActionResult ApproveReject(List<int> rejectdata, string RejectReason)
        {
            int curUserId;
            int BillingType = 0;
            TugDataEntities db = new TugDataEntities();
            curUserId = Session.GetDataFromSession<int>("userid");
            foreach (int id in rejectdata)
            {
                //更改Billing状态
                System.Linq.Expressions.Expression<Func<Billing, bool>> exp = u => u.IDX == id;
                Billing billInfor = db.Billing.Where(exp).FirstOrDefault();
                string billtype = billInfor.InvoiceType.ToString();
                if (billtype == "特殊账单") BillingType = 1;

                billInfor.Phase = 0;
                billInfor.Status = "被駁回";
                db.Entry(billInfor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //修改訂單表
                FinanceLogic.SetOrderServiceFlowingStatus(BillingType, id, "否");

                //写入Approve表
                Approve addApprove = new Approve();
                addApprove.BillingID = id;
                addApprove.FlowMark = billInfor.TimesNo;
                addApprove.Phase = billInfor.Phase;
                addApprove.Task = Task(id, Convert.ToInt32(billInfor.Phase), Convert.ToInt32(billInfor.TimesNo));
                addApprove.Accept = 0;
                addApprove.PersonID = curUserId;
                addApprove.UserID = curUserId;
                addApprove.CreateDate = DateTime.Now.ToShortDateString();
                addApprove.LastUpDate = DateTime.Now.ToShortDateString();
                addApprove = db.Approve.Add(addApprove);
                db.SaveChanges();
            }
            return Json(new { message = "操作完成！" });
        }

        #endregion 驳回

        #region 撤销提交

        public ActionResult RepealSubmit(Billing data)
        {
            int id = data.IDX;
            int idx = Util.toint(Request.Form["data[IDX]"].Trim());
            TugDataEntities db = new TugDataEntities();
            System.Linq.Expressions.Expression<Func<Billing, bool>> exp = u => u.IDX == idx;
            Billing billInfor = db.Billing.Where(exp).FirstOrDefault();

            int Phase = Convert.ToInt32(billInfor.Phase);
            int timeNo = Convert.ToInt32(billInfor.TimesNo);
            int curUserId = Session.GetDataFromSession<int>("userid");
            if (Phase > 1 || Phase == -1)  //流程已进入审核环节或已完成全部审核，不能撤销
            {
                var ret = new { code = Resources.Common.ERROR_CODE, message = "流程已进入审核环节，不能撤销！" };
                return Json(ret);
            }
            else
            {
                //更新Billing状态
                billInfor.Phase = 0;
                billInfor.Status = "已撤销提交";
                db.Entry(billInfor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //写入Approve表
                Approve addApprove = new Approve();
                addApprove.BillingID = idx;
                addApprove.FlowMark = billInfor.TimesNo;
                addApprove.Phase = 0;
                addApprove.Task = "创建";
                addApprove.Accept = 3;
                addApprove.PersonID = curUserId;
                addApprove.UserID = curUserId;
                addApprove.CreateDate = DateTime.Now.ToShortDateString();
                addApprove.LastUpDate = DateTime.Now.ToShortDateString();
                addApprove = db.Approve.Add(addApprove);
                db.SaveChanges();
                return Json(new { message = "撤销成功！" });
            }
        }

        #endregion 撤销提交

        #region 撤销通过

        public ActionResult RepealPass()
        {
            //int idx = Util.toint(Request.Form["data[IDX]"].Trim());
            var f = Request.Form;
            int BillingType = 0;
            int idx = Convert.ToInt32(Request.Form["data[IDX]"]);
            TugDataEntities db = new TugDataEntities();
            System.Linq.Expressions.Expression<Func<Billing, bool>> exp = u => u.IDX == idx;
            Billing billInfor = db.Billing.Where(exp).FirstOrDefault();

            int Phase = Convert.ToInt32(billInfor.Phase);
            int timeNo = Convert.ToInt32(billInfor.TimesNo);
            int tmpUserID =  Convert.ToInt32(Request.Form["data[UserID]"]);
            int curUserId = Session.GetDataFromSession<int>("userid");
            string billtype = billInfor.InvoiceType.ToString();
            if (billtype == "特殊账单") BillingType = 1;

            System.Linq.Expressions.Expression<Func<Flow, bool>> expF = u => u.BillingID == idx && u.MarkID == timeNo && u.FlowUserID == curUserId;
            Flow flowData = db.Flow.Where(expF).FirstOrDefault();
            if (tmpUserID == curUserId)
            
            {
                return Json(new { message = "该记录是您的提交任务，您无法撤销通过！" });
            }
            else if (Phase > flowData.Phase + 1)  //流程已进入下一审核环节，不能撤销
            {
                var ret = new { code = Resources.Common.ERROR_CODE, message = "已进入下一审核环节，不能撤销！" };
                return Json(ret);
            }
            else
            {
                //更新Billing表状态
                billInfor.Phase = 0;
                billInfor.Status = "已撤销通过";
                db.Entry(billInfor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //修改訂單表
                FinanceLogic.SetOrderServiceFlowingStatus(BillingType, idx, "否");

                //写入Approve表
                System.Linq.Expressions.Expression<Func<Approve, bool>> expA = u => u.BillingID == idx && u.FlowMark == timeNo && u.PersonID == curUserId;
                Approve approveInfor = db.Approve.Where(expA).FirstOrDefault(); //获取当前用户的审核信息

                Approve addApprove = new Approve();
                addApprove.BillingID = idx;
                addApprove.FlowMark = billInfor.TimesNo;
                addApprove.Phase = approveInfor.Phase;
                addApprove.Task = approveInfor.Task;
                addApprove.Accept = 4;
                addApprove.PersonID = curUserId;
                addApprove.UserID = curUserId;
                addApprove.CreateDate = DateTime.Now.ToShortDateString();
                addApprove.LastUpDate = DateTime.Now.ToShortDateString();
                addApprove = db.Approve.Add(addApprove);
                db.SaveChanges();
                return Json(new { message = "撤销成功！" });
            }
        }

        #endregion 撤销通过
    }
}
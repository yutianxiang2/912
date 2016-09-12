using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataModel;

namespace WMS.Controllers
{
    public class treeController : BaseController
    {
        public ActionResult GetDataForLoadOnce(bool _search, string sidx, string sord, int page, int rows)
        {
            this.Internationalization();

            //string s = Request.QueryString[6];

            try
            {
                //TugDataEntities db = new TugDataEntities();
                //List<BaseTreeItems> trees = db.BaseTreeItems.Select(u => u).OrderByDescending(u => u.IDX).ToList<BaseTreeItems>();
                //int totalRecordNum = trees.Count;
                //if (page != 0 && totalRecordNum % rows == 0) page -= 1;
                //int pageSize = rows;
                //int totalPageNum = (int)Math.Ceiling((double)totalRecordNum / pageSize);

                ////List<TugInfor> page_trees = trees.Skip((page - 1) * rows).Take(rows).OrderBy(u => u.IDX).ToList<TugInfor>();

                List<object> source = new List<object>();
                source.Add(new { IDX = 10, Name1 = "中国", FatherID = System.DBNull.Value, LevelValue = 0, IsLeaf = "false", loaded = "true", expanded = "true" });
                source.Add(new { IDX = 11, Name1 = "上海", FatherID = 10, LevelValue = 1, IsLeaf = "false", loaded = "true", expanded = "true" });
                source.Add(new { IDX = 12, Name1 = "浦东", FatherID = 11, LevelValue = 2, IsLeaf = "true", loaded = "true", expanded = "true" });
                source.Add(new { IDX = 13, Name1 = "徐汇", FatherID = 11, LevelValue = 2, IsLeaf = "true", loaded = "true", expanded = "true" });
                source.Add(new { IDX = 14, Name1 = "北京", FatherID = 10, LevelValue = 1, IsLeaf = "false", loaded = "true", expanded = "true" });
                source.Add(new { IDX = 15, Name1 = "海淀", FatherID = 11, LevelValue = 2, IsLeaf = "true", loaded = "true", expanded = "true" });
                source.Add(new { IDX = 16, Name1 = "通州", FatherID = 11, LevelValue = 2, IsLeaf = "true", loaded = "true", expanded = "true" });

                List<object> list = new List<object>();

                list.Add(source[0]);
                list.Add(source[1]);
                list.Add(source[2]);
                list.Add(source[3]);
                list.Add(source[4]);
                list.Add(source[5]);
                list.Add(source[6]);
                //var jsonData = new { list = list };
                var jsonData = new { page = 1, records = 10, total = 6, rows = list };

                return Json(jsonData, JsonRequestBehavior.AllowGet);

                //       var aa={
                //{"IDX":"13","name":"Donna","salary":"800.00","boss_id":"12","level":2,"isLeaf":"true","loaded":"true","expanded":"true"},
                //{"IDX":"14","name":"Eddie","salary":"700.00","boss_id":"12","level":2,"isLeaf":"true","loaded":"true","expanded":"true"},
                //{"IDX":"15","name":"Fred","salary":"600.00","boss_id":"12","level":2,"isLeaf":"true","loaded":"true","expanded":"true"}
                //    };
            }
            catch (Exception)
            {
                return Json(new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE });
            }
        }

        //
        // GET: /tree/
        public ActionResult Index()
        {
            return View();
        }
    }
}
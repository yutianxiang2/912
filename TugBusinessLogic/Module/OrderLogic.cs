using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Linq.Expressions;

namespace BusinessLogic.Module
{
    public class OrderLogic
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchOptions">搜索选项，格式如下</param>
        /// <returns></returns>
        static public List<DataModel.V_OrderInfor> SearchForOrderMange(string orderField, string orderMethod, string searchOptions)
        {
            List<V_OrderInfor> orders = null;
            try
            {
                //searchOptions的Json字符串格式
                //{
                //    "groupOp":"AND",
                //    "rules":[{"field":"IsGuest","op":"eq","data":"全部"}],
                //    "groups":[
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"BigTugNum"},{"data":"2","op":"le","field":"BigTugNum"}]},
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"MiddleTugNum"},{"data":"2","op":"le","field":"MiddleTugNum"}]},
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"SmallTugNum"},{"data":"2","op":"le","field":"SmallTugNum"}]}
                //    ]

                //}

                

                TugDataEntities db = new TugDataEntities();
                //orders = db.V_OrderInfor.Select(u => u).ToList<V_OrderInfor>();

                JObject jsonSearchOption = (JObject)JsonConvert.DeserializeObject(searchOptions);
                string groupOp = (string)jsonSearchOption["groupOp"];
                JArray rules = (JArray)jsonSearchOption["rules"];

                ParameterExpression parameter = Expression.Parameter(typeof(V_OrderInfor));
                //Expression condition = Expression.Equal(Expression.Constant(1, typeof(int)), Expression.Constant(1, typeof(int)));
                Expression condition = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol2"), Expression.Constant("0", typeof(string)));//订单展示默认条件：UserDefinedCol2=0，单号同一客户，同一客户船统一在一个界面进行展示
                

                if (rules != null)
                {
                    foreach (JObject item in rules)
                    {
                        string field = (string)item["field"];
                        string op = (string)item["op"];
                        string data = (string)item["data"];

                        #region 根据各字段条件进行条件表达式拼接
                        switch (field)
                        {
                            #region IsGuest
                            case "IsGuest":
                                {
                                    Expression cdt = null;

                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                if (data != "全部")
                                                {
                                                    //orders = orders.Where(u => u.IsGuest == data).ToList();
                                                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "IsGuest"), Expression.Constant(data)); 
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Code
                            case "Code":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Code"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Code"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Code"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(),typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Code"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region CustomerName
                            case "CustomerName":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "CustomerName"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region OrdDate
                            case "OrdDate":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ServiceWorkDate == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == -1 || u.ServiceWorkDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == 1 || u.ServiceWorkDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region WorkDate //已注释
                            //case "WorkDate":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceWorkDate == data.Trim()).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == -1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == -1 || u.ServiceWorkDate.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == 1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == 1 || u.ServiceWorkDate.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region WorkTime //已注释
                            //case "WorkTime":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkTime == data.Trim()).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == -1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == -1 || u.WorkTime.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == 1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == 1 || u.WorkTime.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region EstimatedCompletionTime //已注释
                            //case "EstimatedCompletionTime":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.EstimatedCompletionTime == data.Trim()).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), Expression.Constant(data.Trim()));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.EstimatedCompletionTime.CompareTo(data.Trim()) == -1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.EstimatedCompletionTime.CompareTo(data.Trim()) == -1 || u.EstimatedCompletionTime.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.EstimatedCompletionTime.CompareTo(data.Trim()) == 1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.EstimatedCompletionTime.CompareTo(data.Trim()) == 1 || u.EstimatedCompletionTime.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region ShipName
                            case "ShipName":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "ShipName"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region LinkMan
                            case "LinkMan":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LinkMan.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "LinkMan"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.LinkMan.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkMan"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.LinkMan.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkMan"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(),typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.LinkMan.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkMan"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region LinkPhone
                            case "LinkPhone":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LinkPhone.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "LinkPhone"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.LinkPhone.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkPhone"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.LinkPhone.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkPhone"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(),typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.LinkPhone.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkPhone"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region LinkEmail
                            case "LinkEmail":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LinkEmail.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "LinkEmail"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.LinkEmail.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkEmail"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.LinkEmail.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkEmail"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.LinkEmail.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkEmail"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region WorkPlace //已注释
                            //case "WorkPlace":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkPlace.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "WorkPlace"), Expression.Constant(data.Trim().ToLower()));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_BW:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkPlace.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "WorkPlace"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(),typeof(String)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_EW:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkPlace.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "WorkPlace"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_CN:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkPlace.ToLower().Contains(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "WorkPlace"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region ServiceNatureNames //已注释
                            //case "ServiceNatureNames":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceNatureNames.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "ServiceNatureNames"), Expression.Constant(data.Trim().ToLower()));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_BW:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceNatureNames.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceNatureNames"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(),typeof(String)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_EW:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceNatureNames.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceNatureNames"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_CN:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceNatureNames.ToLower().Contains(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceNatureNames"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region WorkStateLabel
                            case "WorkStateLabel":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                int workStateId = Convert.ToInt32(data.Split('~')[0]);
                                                if (workStateId != -1)
                                                {
                                                    //orders = orders.Where(u => u.WorkStateID == workStateId).ToList();
                                                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "WorkStateID"), Expression.Constant(workStateId, typeof(Nullable<int>)));
                                                }

                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region BigTugNum //已注释
                            //case "BigTugNum":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.BigTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.BigTugNum < Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.BigTugNum < Convert.ToInt32(data.Trim()) || u.BigTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.BigTugNum > Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.BigTugNum > Convert.ToInt32(data.Trim()) || u.BigTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region MiddleTugNum //已注释
                            //case "MiddleTugNum":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.MiddleTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.MiddleTugNum < Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.MiddleTugNum < Convert.ToInt32(data.Trim()) || u.MiddleTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.MiddleTugNum > Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.MiddleTugNum > Convert.ToInt32(data.Trim()) || u.MiddleTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()),typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region SmallTugNum //已注释
                            //case "SmallTugNum":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region SmallTugNum //已注释
                            case "Length":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Length"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "Length"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "Length"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "Length"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "Length"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Remark
                            case "Remark":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Remark"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Remark"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(),typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Remark"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Remark"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region CreateDate
                            case "CreateDate":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.CreateDate == data.Trim()).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "CreateDate"), Expression.Constant(data.Trim()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1).ToList();
                                                //cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "CreateDate"), Expression.Constant(data.Trim()));
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo",new Type[]{typeof(String)}), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region LastUpDate
                            case "LastUpDate":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol1
                            case "UserDefinedCol1":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol1"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol1"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol1"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol1"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol2
                            case "UserDefinedCol2":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol2"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol2"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol2"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol2"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol3
                            case "UserDefinedCol3":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol3"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol3"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol3"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol3"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol4
                            case "UserDefinedCol4":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol4"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol4"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol4"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol4"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol5
                            case "UserDefinedCol5":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol5"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "UserDefinedCol5"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol5"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "UserDefinedCol5"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol5"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol6
                            case "UserDefinedCol6":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol6"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "UserDefinedCol6"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol6"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "UserDefinedCol6"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol6"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol7
                            case "UserDefinedCol7":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol7"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "UserDefinedCol7"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol7"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "UserDefinedCol7"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol7"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol8
                            case "UserDefinedCol8":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol8"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "UserDefinedCol8"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol8"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "UserDefinedCol8"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol8"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol9
                            case "UserDefinedCol9":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol9"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol9"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol9"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol9"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol9"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol10
                            case "UserDefinedCol10":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol10"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol10"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol10"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol10"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol10"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            default:
                                break;
                        }
                        #endregion

                    }

                }

                #region 执行查询
                if (condition != null)
                {
                    var lamda = Expression.Lambda<Func<V_OrderInfor, bool>>(condition, parameter);
                    orders = db.V_OrderInfor.Where(lamda).Select(u => u).ToList<V_OrderInfor>();
                }
                else
                {
                    orders = db.V_OrderInfor.Select(u => u).ToList<V_OrderInfor>();
                }
                #endregion

                

                #region 对搜索结果根据排序字段和方式进行排序
                switch (orderField)
                {
                    case "":
                        {
                            //if (orderMethod.ToLower().Equals("asc"))
                            //    orders = orders.OrderBy(u => u.IDX).ToList();
                            //else
                                orders = orders.OrderByDescending(u => u.IDX).ToList();
                        }
                        break;
                    case "IsGuest":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.IsGuest).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.IsGuest).ToList();
                        }
                        break;
                    case "Code":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Code).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Code).ToList();
                        }
                        break;
                    case "CustomerName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CustomerName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CustomerName).ToList();
                        }
                        break;
                    case "OrdDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.OrdDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.OrdDate).ToList();
                        }
                        break;
                    //case "WorkDate":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.ServiceWorkDate).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.ServiceWorkDate).ToList();
                    //    }
                    //    break;
                    //case "WorkTime":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.WorkTime).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.WorkTime).ToList();
                    //    }
                    //    break;
                    //case "EstimatedCompletionTime":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.EstimatedCompletionTime).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.EstimatedCompletionTime).ToList();
                    //    }
                    //    break;
                    case "ShipName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipName).ToList();
                        }
                        break;
                    case "Length":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Length).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Length).ToList();
                        }
                        break;
                    case "LinkMan":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkMan).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkMan).ToList();
                        }
                        break;
                    case "LinkPhone":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkPhone).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkPhone).ToList();
                        }
                        break;
                    case "LinkEmail":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkEmail).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkEmail).ToList();
                        }
                        break;
                    //case "WorkPlace":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.WorkPlace).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.WorkPlace).ToList();
                    //    }
                    //    break;
                    //case "ServiceNatureNames":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.ServiceNatureNames).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.ServiceNatureNames).ToList();
                    //    }
                    //    break;
                    case "WorkStateLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.WorkStateLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.WorkStateLabel).ToList();
                        }
                        break;
                    //case "BigTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.BigTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.BigTugNum).ToList();
                    //    }
                    //    break;
                    //case "MiddleTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.MiddleTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.MiddleTugNum).ToList();
                    //    }
                    //    break;
                    //case "SmallTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.SmallTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.SmallTugNum).ToList();
                    //    }
                    //    break;
                    case "Remark":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Remark).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Remark).ToList();
                        }
                        break;
                    case "CreateDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CreateDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CreateDate).ToList();
                        }
                        break;
                    case "LastUpDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LastUpDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LastUpDate).ToList();
                        }
                        break;
                    
                   
                    default:
                        break;
                }
                #endregion


                JArray groups = (JArray)jsonSearchOption["groups"];
                if (groups != null)
                {
                    foreach (JObject item in groups)
                    {
                        string item_groupOp = (string)item["groupOp"];
                        JArray item_groups = (JArray)item["groups"];
                        JArray item_rules = (JArray)item["rules"];
                        string item_rule0_field = (string)(((JObject)item_rules[0])["field"]);
                        string item_rule0_op = (string)(((JObject)item_rules[0])["op"]);
                        string item_rule0_data = (string)(((JObject)item_rules[0])["data"]);

                        string item_rule1_field = (string)(((JObject)item_rules[1])["field"]);
                        string item_rule1_op = (string)(((JObject)item_rules[1])["op"]);
                        string item_rule1_data = (string)(((JObject)item_rules[1])["data"]);
                    }
                }

            }
            catch(Exception ex)
            {
                return null;
            }
            return orders;
        }

        /// <summary>
        /// 订单管理页面，加载订单数据
        /// </summary>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderMethod">排序方式asc升序；desc降序</param>
        /// <returns></returns>
        static public List<DataModel.V_OrderInfor> LoadDataForOrderManage(string orderField, string orderMethod)
        {
            List<V_OrderInfor> orders = null;

            try
            {
                TugDataEntities db = new TugDataEntities();
                orders = db.V_OrderInfor.Where(u => u.UserDefinedCol2=="0").Select(u => u).ToList<V_OrderInfor>();

                #region 根据排序字段和排序方式排序
                switch (orderField)
                {
                    case "":
                        {
                            //if(orderMethod.ToLower().Equals("asc"))
                            //    orders = orders.OrderBy(u => u.IDX).ToList();
                            //else
                                orders = orders.OrderByDescending(u => u.IDX).ToList();
                        }
                        break;
                    case "IsGuest":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.IsGuest).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.IsGuest).ToList();
                        }
                        break;
                    case "Code":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Code).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Code).ToList();
                        }
                        break;
                    case "CustomerName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CustomerName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CustomerName).ToList();
                        }
                        break;
                    case "OrdDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.OrdDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.OrdDate).ToList();
                        }
                        break;
                    //case "WorkDate":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.ServiceWorkDate).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.ServiceWorkDate).ToList();
                    //    }
                    //    break;
                    //case "WorkTime":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.WorkTime).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.WorkTime).ToList();
                    //    }
                    //    break;
                    //case "EstimatedCompletionTime":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.EstimatedCompletionTime).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.EstimatedCompletionTime).ToList();
                    //    }
                    //    break;
                    case "ShipName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipName).ToList();
                        }
                        break;
                    case "LinkMan":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkMan).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkMan).ToList();
                        }
                        break;
                    case "LinkPhone":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkPhone).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkPhone).ToList();
                        }
                        break;
                    case "LinkEmail":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkEmail).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkEmail).ToList();
                        }
                        break;
                    //case "WorkPlace":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.WorkPlace).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.WorkPlace).ToList();
                    //    }
                    //    break;
                    //case "ServiceNatureNames":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.ServiceNatureNames).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.ServiceNatureNames).ToList();
                    //    }
                    //    break;
                    case "WorkStateLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.WorkStateLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.WorkStateLabel).ToList();
                        }
                        break;
                    //case "BigTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.BigTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.BigTugNum).ToList();
                    //    }
                    //    break;
                    //case "MiddleTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.MiddleTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.MiddleTugNum).ToList();
                    //    }
                    //    break;
                    //case "SmallTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.SmallTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.SmallTugNum).ToList();
                    //    }
                    //    break;
                    case "Length":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Length).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Length).ToList();
                        }
                        break;
                    case "Remark":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Remark).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Remark).ToList();
                        }
                        break;
                    case "CreateDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CreateDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CreateDate).ToList();
                        }
                        break;
                    case "LastUpDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LastUpDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LastUpDate).ToList();
                        }
                        break;
                    //case "UserDefinedCol1":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol1).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol1).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol2":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol2).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol2).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol3":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol3).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol3).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol4":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol4).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol4).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol5":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol5).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol5).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol6":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol6).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol6).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol7":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol7).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol7).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol8":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol8).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol8).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol9":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol9).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol9).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol10":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol10).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol10).ToList();
                    //    }
                    //    break;
                    default:
                        break;
                }

                #endregion
            }
            catch(Exception ex)
            {
                return null;
            }

            return orders;
        }




        /// <summary>
        /// 订单调度页面，加载订单数据
        /// </summary>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderMethod">排序方式asc升序；desc降序</param>
        /// <returns></returns>
        static public List<DataModel.V_OrderInfor> LoadDataForOrderScheduling(string orderField, string orderMethod)
        {
            List<V_OrderInfor> orders = null;

            try
            {
                TugDataEntities db = new TugDataEntities();
                orders = db.V_OrderInfor.Where(u => u.WorkStateID == 2 || u.WorkStateID == 3 || u.WorkStateID == 5
                    || u.WorkStateValue == "0" || u.WorkStateValue == "1" || u.WorkStateValue == "3"
                    || u.WorkStateLabel == "未排船" || u.WorkStateLabel == "已排船" || u.WorkStateLabel == "已完工").Select(u => u).ToList<V_OrderInfor>();

                #region 根据排序字段和排序方式排序
                switch (orderField)
                {
                    case "":
                        {
                            //if(orderMethod.ToLower().Equals("asc"))
                            //    orders = orders.OrderBy(u => u.IDX).ToList();
                            //else
                            orders = orders.OrderByDescending(u => u.IDX).ToList();
                        }
                        break;
                    case "IsGuest":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.IsGuest).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.IsGuest).ToList();
                        }
                        break;
                    case "Code":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Code).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Code).ToList();
                        }
                        break;
                    case "CustomerName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CustomerName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CustomerName).ToList();
                        }
                        break;
                    case "OrdDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.OrdDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.OrdDate).ToList();
                        }
                        break;
                    //case "WorkDate":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.ServiceWorkDate).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.ServiceWorkDate).ToList();
                    //    }
                    //    break;
                    //case "WorkTime":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.WorkTime).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.WorkTime).ToList();
                    //    }
                    //    break;
                    //case "EstimatedCompletionTime":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.EstimatedCompletionTime).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.EstimatedCompletionTime).ToList();
                    //    }
                    //    break;
                    case "ShipName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipName).ToList();
                        }
                        break;
                    case "LinkMan":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkMan).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkMan).ToList();
                        }
                        break;
                    case "LinkPhone":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkPhone).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkPhone).ToList();
                        }
                        break;
                    case "LinkEmail":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkEmail).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkEmail).ToList();
                        }
                        break;
                    //case "WorkPlace":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.WorkPlace).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.WorkPlace).ToList();
                    //    }
                    //    break;
                    //case "ServiceNatureNames":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.ServiceNatureNames).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.ServiceNatureNames).ToList();
                    //    }
                    //    break;
                    case "WorkStateLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.WorkStateLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.WorkStateLabel).ToList();
                        }
                        break;
                    //case "BigTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.BigTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.BigTugNum).ToList();
                    //    }
                    //    break;
                    //case "MiddleTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.MiddleTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.MiddleTugNum).ToList();
                    //    }
                    //    break;
                    //case "SmallTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.SmallTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.SmallTugNum).ToList();
                    //    }
                    //    break;
                    case "Remark":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Remark).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Remark).ToList();
                        }
                        break;
                    case "CreateDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CreateDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CreateDate).ToList();
                        }
                        break;
                    case "LastUpDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LastUpDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LastUpDate).ToList();
                        }
                        break;
                    //case "UserDefinedCol1":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol1).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol1).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol2":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol2).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol2).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol3":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol3).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol3).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol4":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol4).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol4).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol5":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol5).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol5).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol6":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol6).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol6).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol7":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol7).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol7).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol8":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol8).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol8).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol9":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol9).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol9).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol10":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol10).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol10).ToList();
                    //    }
                    //    break;
                    default:
                        break;
                }

                #endregion
            }
            catch (Exception ex)
            {
                return null;
            }

            return orders;
        }





        static public List<DataModel.V_OrderInfor> SearchForJobInformation(string orderField, string orderMethod, string searchOptions)
        {
            List<V_OrderInfor> orders = null;
            try
            {
                //searchOptions的Json字符串格式
                //{
                //    "groupOp":"AND",
                //    "rules":[{"field":"IsGuest","op":"eq","data":"全部"}],
                //    "groups":[
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"BigTugNum"},{"data":"2","op":"le","field":"BigTugNum"}]},
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"MiddleTugNum"},{"data":"2","op":"le","field":"MiddleTugNum"}]},
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"SmallTugNum"},{"data":"2","op":"le","field":"SmallTugNum"}]}
                //    ]

                //}



                TugDataEntities db = new TugDataEntities();
                //orders = db.V_OrderInfor.Select(u => u).ToList<V_OrderInfor>();

                JObject jsonSearchOption = (JObject)JsonConvert.DeserializeObject(searchOptions);
                string groupOp = (string)jsonSearchOption["groupOp"];
                JArray rules = (JArray)jsonSearchOption["rules"];

                Expression condition = Expression.Equal(Expression.Constant(1, typeof(int)), Expression.Constant(1, typeof(int)));
                ParameterExpression parameter = Expression.Parameter(typeof(V_OrderInfor));

                if (rules != null)
                {
                    foreach (JObject item in rules)
                    {
                        string field = (string)item["field"];
                        string op = (string)item["op"];
                        string data = (string)item["data"];

                        #region 根据各字段条件进行条件表达式拼接
                        switch (field)
                        {
                            #region IsGuest
                            case "IsGuest":
                                {
                                    Expression cdt = null;

                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                if (data != "全部")
                                                {
                                                    //orders = orders.Where(u => u.IsGuest == data).ToList();
                                                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "IsGuest"), Expression.Constant(data));
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Code
                            case "Code":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Code"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Code"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Code"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Code"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region CustomerName
                            case "CustomerName":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "CustomerName"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region OrdDate
                            case "OrdDate":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ServiceWorkDate == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == -1 || u.ServiceWorkDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == 1 || u.ServiceWorkDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region WorkDate //已注释
                            //case "WorkDate":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceWorkDate == data.Trim()).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == -1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == -1 || u.ServiceWorkDate.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == 1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == 1 || u.ServiceWorkDate.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region WorkTime //已注释
                            //case "WorkTime":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkTime == data.Trim()).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == -1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == -1 || u.WorkTime.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == 1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == 1 || u.WorkTime.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region EstimatedCompletionTime //已注释
                            //case "EstimatedCompletionTime":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.EstimatedCompletionTime == data.Trim()).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), Expression.Constant(data.Trim()));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.EstimatedCompletionTime.CompareTo(data.Trim()) == -1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.EstimatedCompletionTime.CompareTo(data.Trim()) == -1 || u.EstimatedCompletionTime.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.EstimatedCompletionTime.CompareTo(data.Trim()) == 1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.EstimatedCompletionTime.CompareTo(data.Trim()) == 1 || u.EstimatedCompletionTime.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region ShipName
                            case "ShipName":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "ShipName"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region LinkMan
                            case "LinkMan":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LinkMan.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "LinkMan"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.LinkMan.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkMan"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.LinkMan.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkMan"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.LinkMan.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkMan"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region LinkPhone
                            case "LinkPhone":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LinkPhone.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "LinkPhone"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.LinkPhone.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkPhone"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.LinkPhone.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkPhone"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.LinkPhone.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkPhone"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region LinkEmail
                            case "LinkEmail":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LinkEmail.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "LinkEmail"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.LinkEmail.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkEmail"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.LinkEmail.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkEmail"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.LinkEmail.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkEmail"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region WorkPlace //已注释
                            //case "WorkPlace":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkPlace.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "WorkPlace"), Expression.Constant(data.Trim().ToLower()));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_BW:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkPlace.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "WorkPlace"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(),typeof(String)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_EW:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkPlace.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "WorkPlace"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_CN:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkPlace.ToLower().Contains(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "WorkPlace"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region ServiceNatureNames //已注释
                            //case "ServiceNatureNames":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceNatureNames.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "ServiceNatureNames"), Expression.Constant(data.Trim().ToLower()));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_BW:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceNatureNames.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceNatureNames"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(),typeof(String)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_EW:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceNatureNames.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceNatureNames"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_CN:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceNatureNames.ToLower().Contains(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceNatureNames"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region WorkStateLabel
                            case "WorkStateLabel":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                int workStateId = Convert.ToInt32(data.Split('~')[0]);
                                                if (workStateId != -1)
                                                {
                                                    //orders = orders.Where(u => u.WorkStateID == workStateId).ToList();
                                                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "WorkStateID"), Expression.Constant(workStateId, typeof(Nullable<int>)));
                                                }

                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region BigTugNum //已注释
                            //case "BigTugNum":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.BigTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.BigTugNum < Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.BigTugNum < Convert.ToInt32(data.Trim()) || u.BigTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.BigTugNum > Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.BigTugNum > Convert.ToInt32(data.Trim()) || u.BigTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region MiddleTugNum //已注释
                            //case "MiddleTugNum":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.MiddleTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.MiddleTugNum < Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.MiddleTugNum < Convert.ToInt32(data.Trim()) || u.MiddleTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.MiddleTugNum > Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.MiddleTugNum > Convert.ToInt32(data.Trim()) || u.MiddleTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()),typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region SmallTugNum //已注释
                            //case "SmallTugNum":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region HasInvoice
                            case "HasInvoice":
                                {
                                    Expression cdt = null;

                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                if (data != "全部")
                                                {
                                                    //orders = orders.Where(u => u.IsGuest == data).ToList();
                                                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "HasInvoice"), Expression.Constant(data));
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region HasInFlow
                            case "HasInFlow":
                                {
                                    Expression cdt = null;

                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                if (data != "全部")
                                                {
                                                    //orders = orders.Where(u => u.IsGuest == data).ToList();
                                                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "HasInFlow"), Expression.Constant(data));
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Remark
                            case "Remark":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Remark"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Remark"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Remark"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Remark"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region CreateDate
                            case "CreateDate":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.CreateDate == data.Trim()).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "CreateDate"), Expression.Constant(data.Trim()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1).ToList();
                                                //cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "CreateDate"), Expression.Constant(data.Trim()));
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region LastUpDate
                            case "LastUpDate":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol1
                            case "UserDefinedCol1":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol1"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol1"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol1"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol1"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol2
                            case "UserDefinedCol2":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol2"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol2"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol2"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol2"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol3
                            case "UserDefinedCol3":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol3"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol3"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol3"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol3"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol4
                            case "UserDefinedCol4":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol4"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol4"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol4"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol4"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol5
                            case "UserDefinedCol5":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol5"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "UserDefinedCol5"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol5"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "UserDefinedCol5"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol5"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol6
                            case "UserDefinedCol6":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol6"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "UserDefinedCol6"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol6"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "UserDefinedCol6"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol6"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol7
                            case "UserDefinedCol7":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol7"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "UserDefinedCol7"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol7"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "UserDefinedCol7"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol7"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol8
                            case "UserDefinedCol8":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol8"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "UserDefinedCol8"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol8"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "UserDefinedCol8"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol8"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol9
                            case "UserDefinedCol9":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol9"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol9"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol9"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol9"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol9"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol10
                            case "UserDefinedCol10":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol10"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol10"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol10"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol10"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol10"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            default:
                                break;
                        }
                        #endregion

                    }

                }

                #region 执行查询
                if (condition != null)
                {
                    var lamda = Expression.Lambda<Func<V_OrderInfor, bool>>(condition, parameter);
                    orders = db.V_OrderInfor.Where(lamda).Select(u => u).ToList<V_OrderInfor>();
                }
                else
                {
                    orders = db.V_OrderInfor.Select(u => u).ToList<V_OrderInfor>();
                }
                #endregion


                orders = orders.Where(u => u.WorkStateID == 3 || u.WorkStateID == 5
                    || u.WorkStateValue == "1" || u.WorkStateValue == "3"
                    || u.WorkStateLabel == "已排船" || u.WorkStateLabel == "已完成").Select(u => u).ToList<V_OrderInfor>();

                #region 对搜索结果根据排序字段和方式进行排序
                switch (orderField)
                {
                    case "":
                        {
                            //if (orderMethod.ToLower().Equals("asc"))
                            //    orders = orders.OrderBy(u => u.IDX).ToList();
                            //else
                            orders = orders.OrderByDescending(u => u.IDX).ToList();
                        }
                        break;
                    case "IsGuest":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.IsGuest).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.IsGuest).ToList();
                        }
                        break;
                    case "Code":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Code).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Code).ToList();
                        }
                        break;
                    case "CustomerName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CustomerName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CustomerName).ToList();
                        }
                        break;
                    case "OrdDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.OrdDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.OrdDate).ToList();
                        }
                        break;
                    //case "WorkDate":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.ServiceWorkDate).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.ServiceWorkDate).ToList();
                    //    }
                    //    break;
                    //case "WorkTime":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.WorkTime).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.WorkTime).ToList();
                    //    }
                    //    break;
                    //case "EstimatedCompletionTime":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.EstimatedCompletionTime).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.EstimatedCompletionTime).ToList();
                    //    }
                    //    break;
                    case "ShipName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipName).ToList();
                        }
                        break;
                    case "LinkMan":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkMan).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkMan).ToList();
                        }
                        break;
                    case "LinkPhone":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkPhone).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkPhone).ToList();
                        }
                        break;
                    case "LinkEmail":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkEmail).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkEmail).ToList();
                        }
                        break;
                    //case "WorkPlace":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.WorkPlace).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.WorkPlace).ToList();
                    //    }
                    //    break;
                    //case "ServiceNatureNames":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.ServiceNatureNames).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.ServiceNatureNames).ToList();
                    //    }
                    //    break;
                    case "WorkStateLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.WorkStateLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.WorkStateLabel).ToList();
                        }
                        break;
                    //case "BigTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.BigTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.BigTugNum).ToList();
                    //    }
                    //    break;
                    //case "MiddleTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.MiddleTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.MiddleTugNum).ToList();
                    //    }
                    //    break;
                    //case "SmallTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.SmallTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.SmallTugNum).ToList();
                    //    }
                    //    break;
                    case "Remark":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Remark).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Remark).ToList();
                        }
                        break;
                    case "CreateDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CreateDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CreateDate).ToList();
                        }
                        break;
                    case "LastUpDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LastUpDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LastUpDate).ToList();
                        }
                        break;


                    default:
                        break;
                }
                #endregion


                JArray groups = (JArray)jsonSearchOption["groups"];
                if (groups != null)
                {
                    foreach (JObject item in groups)
                    {
                        string item_groupOp = (string)item["groupOp"];
                        JArray item_groups = (JArray)item["groups"];
                        JArray item_rules = (JArray)item["rules"];
                        string item_rule0_field = (string)(((JObject)item_rules[0])["field"]);
                        string item_rule0_op = (string)(((JObject)item_rules[0])["op"]);
                        string item_rule0_data = (string)(((JObject)item_rules[0])["data"]);

                        string item_rule1_field = (string)(((JObject)item_rules[1])["field"]);
                        string item_rule1_op = (string)(((JObject)item_rules[1])["op"]);
                        string item_rule1_data = (string)(((JObject)item_rules[1])["data"]);
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return orders;
        }


        /// <summary>
        /// 订单调度页面，加载订单数据
        /// </summary>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderMethod">排序方式asc升序；desc降序</param>
        /// <returns></returns>
        static public List<DataModel.V_OrderInfor> LoadDataForJobInformation(string orderField, string orderMethod)
        {
            List<V_OrderInfor> orders = null;

            try
            {
                TugDataEntities db = new TugDataEntities();
                orders = db.V_OrderInfor.Where(u => u.WorkStateID == 3 || u.WorkStateID == 5
                    ||u.WorkStateValue == "1" || u.WorkStateValue == "3"
                    || u.WorkStateLabel == "已排船" || u.WorkStateLabel == "已完成").Select(u => u).ToList<V_OrderInfor>();

                #region 根据排序字段和排序方式排序
                switch (orderField)
                {
                    case "":
                        {
                            //if(orderMethod.ToLower().Equals("asc"))
                            //    orders = orders.OrderBy(u => u.IDX).ToList();
                            //else
                            orders = orders.OrderByDescending(u => u.IDX).ToList();
                        }
                        break;
                    case "IsGuest":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.IsGuest).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.IsGuest).ToList();
                        }
                        break;
                    case "Code":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Code).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Code).ToList();
                        }
                        break;
                    case "CustomerName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CustomerName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CustomerName).ToList();
                        }
                        break;
                    case "OrdDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.OrdDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.OrdDate).ToList();
                        }
                        break;
                    //case "WorkDate":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.ServiceWorkDate).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.ServiceWorkDate).ToList();
                    //    }
                    //    break;
                    //case "WorkTime":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.WorkTime).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.WorkTime).ToList();
                    //    }
                    //    break;
                    //case "EstimatedCompletionTime":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.EstimatedCompletionTime).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.EstimatedCompletionTime).ToList();
                    //    }
                    //    break;
                    case "ShipName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipName).ToList();
                        }
                        break;
                    case "LinkMan":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkMan).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkMan).ToList();
                        }
                        break;
                    case "LinkPhone":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkPhone).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkPhone).ToList();
                        }
                        break;
                    case "LinkEmail":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkEmail).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkEmail).ToList();
                        }
                        break;
                    //case "WorkPlace":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.WorkPlace).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.WorkPlace).ToList();
                    //    }
                    //    break;
                    //case "ServiceNatureNames":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.ServiceNatureNames).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.ServiceNatureNames).ToList();
                    //    }
                    //    break;
                    case "WorkStateLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.WorkStateLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.WorkStateLabel).ToList();
                        }
                        break;
                    //case "BigTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.BigTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.BigTugNum).ToList();
                    //    }
                    //    break;
                    //case "MiddleTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.MiddleTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.MiddleTugNum).ToList();
                    //    }
                    //    break;
                    //case "SmallTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.SmallTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.SmallTugNum).ToList();
                    //    }
                    //    break;
                    case "Remark":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Remark).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Remark).ToList();
                        }
                        break;
                    case "CreateDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CreateDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CreateDate).ToList();
                        }
                        break;
                    case "LastUpDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LastUpDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LastUpDate).ToList();
                        }
                        break;
                    //case "UserDefinedCol1":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol1).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol1).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol2":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol2).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol2).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol3":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol3).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol3).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol4":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol4).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol4).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol5":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol5).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol5).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol6":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol6).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol6).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol7":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol7).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol7).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol8":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol8).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol8).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol9":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol9).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol9).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol10":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol10).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol10).ToList();
                    //    }
                    //    break;
                    default:
                        break;
                }

                #endregion
            }
            catch (Exception ex)
            {
                return null;
            }

            return orders;
        }





        static public List<DataModel.V_OrderInfor> SearchForGenerateInvoice(string orderField, string orderMethod, string searchOptions)
        {
            List<V_OrderInfor> orders = null;
            try
            {
                //searchOptions的Json字符串格式
                //{
                //    "groupOp":"AND",
                //    "rules":[{"field":"IsGuest","op":"eq","data":"全部"}],
                //    "groups":[
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"BigTugNum"},{"data":"2","op":"le","field":"BigTugNum"}]},
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"MiddleTugNum"},{"data":"2","op":"le","field":"MiddleTugNum"}]},
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"SmallTugNum"},{"data":"2","op":"le","field":"SmallTugNum"}]}
                //    ]

                //}



                TugDataEntities db = new TugDataEntities();
                //orders = db.V_OrderInfor.Select(u => u).ToList<V_OrderInfor>();

                JObject jsonSearchOption = (JObject)JsonConvert.DeserializeObject(searchOptions);
                string groupOp = (string)jsonSearchOption["groupOp"];
                JArray rules = (JArray)jsonSearchOption["rules"];

                Expression condition = Expression.Equal(Expression.Constant(1, typeof(int)), Expression.Constant(1, typeof(int)));
                ParameterExpression parameter = Expression.Parameter(typeof(V_OrderInfor));

                if (rules != null)
                {
                    foreach (JObject item in rules)
                    {
                        string field = (string)item["field"];
                        string op = (string)item["op"];
                        string data = (string)item["data"];

                        #region 根据各字段条件进行条件表达式拼接
                        switch (field)
                        {
                            #region IsGuest
                            case "IsGuest":
                                {
                                    Expression cdt = null;

                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                if (data != "全部")
                                                {
                                                    //orders = orders.Where(u => u.IsGuest == data).ToList();
                                                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "IsGuest"), Expression.Constant(data));
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Code
                            case "Code":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Code"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Code"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Code"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Code.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Code"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region CustomerName
                            case "CustomerName":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "CustomerName"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.CustomerName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region OrdDate
                            case "OrdDate":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ServiceWorkDate == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == -1 || u.ServiceWorkDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == 1 || u.ServiceWorkDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;

                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "OrdDate"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region WorkDate //已注释
                            //case "WorkDate":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceWorkDate == data.Trim()).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == -1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == -1 || u.ServiceWorkDate.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == 1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceWorkDate.CompareTo(data.Trim()) == 1 || u.ServiceWorkDate.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region WorkTime //已注释
                            //case "WorkTime":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkTime == data.Trim()).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == -1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == -1 || u.WorkTime.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == 1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == 1 || u.WorkTime.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "WorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region EstimatedCompletionTime //已注释
                            //case "EstimatedCompletionTime":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.EstimatedCompletionTime == data.Trim()).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), Expression.Constant(data.Trim()));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.EstimatedCompletionTime.CompareTo(data.Trim()) == -1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.EstimatedCompletionTime.CompareTo(data.Trim()) == -1 || u.EstimatedCompletionTime.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.EstimatedCompletionTime.CompareTo(data.Trim()) == 1).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.EstimatedCompletionTime.CompareTo(data.Trim()) == 1 || u.EstimatedCompletionTime.CompareTo(data.Trim()) == 0).ToList();
                            //                    Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "EstimatedCompletionTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                    cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region ShipName
                            case "ShipName":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "ShipName"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region Length
                            case "Length":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Length"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "Length"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "Length"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "Length"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "Length"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region TEUS
                            case "TEUS":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "TEUS"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "TEUS"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "TEUS"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "TEUS"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "TEUS"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region LinkMan
                            case "LinkMan":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LinkMan.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "LinkMan"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.LinkMan.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkMan"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.LinkMan.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkMan"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.LinkMan.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkMan"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region LinkPhone
                            case "LinkPhone":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LinkPhone.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "LinkPhone"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.LinkPhone.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkPhone"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.LinkPhone.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkPhone"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.LinkPhone.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkPhone"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region LinkEmail
                            case "LinkEmail":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LinkEmail.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "LinkEmail"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.LinkEmail.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkEmail"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.LinkEmail.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkEmail"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.LinkEmail.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "LinkEmail"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region WorkPlace //已注释
                            //case "WorkPlace":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkPlace.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "WorkPlace"), Expression.Constant(data.Trim().ToLower()));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_BW:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkPlace.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "WorkPlace"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(),typeof(String)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_EW:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkPlace.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "WorkPlace"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_CN:
                            //                {
                            //                    //orders = orders.Where(u => u.WorkPlace.ToLower().Contains(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "WorkPlace"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region ServiceNatureNames //已注释
                            //case "ServiceNatureNames":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceNatureNames.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "ServiceNatureNames"), Expression.Constant(data.Trim().ToLower()));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_BW:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceNatureNames.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceNatureNames"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(),typeof(String)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_EW:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceNatureNames.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceNatureNames"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_CN:
                            //                {
                            //                    //orders = orders.Where(u => u.ServiceNatureNames.ToLower().Contains(data.Trim().ToLower())).ToList();
                            //                    cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceNatureNames"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region WorkStateLabel
                            case "WorkStateLabel":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                int workStateId = Convert.ToInt32(data.Split('~')[0]);
                                                if (workStateId != -1)
                                                {
                                                    //orders = orders.Where(u => u.WorkStateID == workStateId).ToList();
                                                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "WorkStateID"), Expression.Constant(workStateId, typeof(Nullable<int>)));
                                                }

                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region BigTugNum //已注释
                            //case "BigTugNum":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.BigTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.BigTugNum < Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.BigTugNum < Convert.ToInt32(data.Trim()) || u.BigTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.BigTugNum > Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.BigTugNum > Convert.ToInt32(data.Trim()) || u.BigTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region MiddleTugNum //已注释
                            //case "MiddleTugNum":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.MiddleTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.MiddleTugNum < Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.MiddleTugNum < Convert.ToInt32(data.Trim()) || u.MiddleTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.MiddleTugNum > Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.MiddleTugNum > Convert.ToInt32(data.Trim()) || u.MiddleTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()),typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region SmallTugNum //已注释
                            //case "SmallTugNum":
                            //    {
                            //        Expression cdt = null;
                            //        switch (op)
                            //        {
                            //            case ConstValue.ComparisonOperator_EQ:
                            //                {
                            //                    //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.Equal(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LT:
                            //                {
                            //                    //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_LE:
                            //                {
                            //                    //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GT:
                            //                {
                            //                    //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            case ConstValue.ComparisonOperator_GE:
                            //                {
                            //                    //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                            //                    cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                            //                }
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //        if (cdt != null)
                            //        {
                            //            condition = Expression.AndAlso(condition, cdt);
                            //        }
                            //    }
                            //    break;
                            #endregion

                            #region Remark
                            case "Remark":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "Remark"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Remark"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Remark"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.Remark.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "Remark"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region CreateDate
                            case "CreateDate":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.CreateDate == data.Trim()).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "CreateDate"), Expression.Constant(data.Trim()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1).ToList();
                                                //cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "CreateDate"), Expression.Constant(data.Trim()));
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == -1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.CreateDate.CompareTo(data.Trim()) == 1 || u.CreateDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "CreateDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region LastUpDate
                            case "LastUpDate":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "LastUpDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol1
                            case "UserDefinedCol1":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol1"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol1"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol1"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol1"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol2
                            case "UserDefinedCol2":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol2"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol2"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol2"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol2"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol3
                            case "UserDefinedCol3":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol3"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol3"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol3"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol3"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol4
                            case "UserDefinedCol4":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol4"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol4"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol4"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol4"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol5
                            case "UserDefinedCol5":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol5"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "UserDefinedCol5"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol5"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "UserDefinedCol5"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol5"), Expression.Constant(Convert.ToDouble(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol6
                            case "UserDefinedCol6":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol6"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "UserDefinedCol6"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol6"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "UserDefinedCol6"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol6"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol7
                            case "UserDefinedCol7":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol7"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "UserDefinedCol7"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol7"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "UserDefinedCol7"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol7"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol8
                            case "UserDefinedCol8":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "UserDefinedCol8"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "UserDefinedCol8"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol8"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "UserDefinedCol8"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "UserDefinedCol8"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<int>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol9
                            case "UserDefinedCol9":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol9"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol9"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol9"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol9"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol9"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol10
                            case "UserDefinedCol10":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol10"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol10"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == -1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol10"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol10"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.LastUpDate.CompareTo(data.Trim()) == 1 || u.LastUpDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "UserDefinedCol10"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            default:
                                break;
                        }
                        #endregion

                    }

                }

                #region 执行查询
                if (condition != null)
                {
                    var lamda = Expression.Lambda<Func<V_OrderInfor, bool>>(condition, parameter);
                    orders = db.V_OrderInfor.Where(lamda).Select(u => u).ToList<V_OrderInfor>();
                }
                else
                {
                    orders = db.V_OrderInfor.Select(u => u).ToList<V_OrderInfor>();
                }
                #endregion


                orders = orders.Where(u => (u.WorkStateID == 5
                    || u.WorkStateValue == "3"
                    || u.WorkStateLabel == "已完工") && u.HasInvoice != "是").Select(u => u).ToList<V_OrderInfor>();

                #region 对搜索结果根据排序字段和方式进行排序
                switch (orderField)
                {
                    case "":
                        {
                            //if (orderMethod.ToLower().Equals("asc"))
                            //    orders = orders.OrderBy(u => u.IDX).ToList();
                            //else
                            orders = orders.OrderByDescending(u => u.IDX).ToList();
                        }
                        break;
                    case "IsGuest":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.IsGuest).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.IsGuest).ToList();
                        }
                        break;
                    case "Code":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Code).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Code).ToList();
                        }
                        break;
                    case "CustomerName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CustomerName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CustomerName).ToList();
                        }
                        break;
                    case "OrdDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.OrdDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.OrdDate).ToList();
                        }
                        break;
                    //case "WorkDate":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.ServiceWorkDate).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.ServiceWorkDate).ToList();
                    //    }
                    //    break;
                    //case "WorkTime":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.WorkTime).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.WorkTime).ToList();
                    //    }
                    //    break;
                    //case "EstimatedCompletionTime":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.EstimatedCompletionTime).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.EstimatedCompletionTime).ToList();
                    //    }
                    //    break;
                    case "ShipName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipName).ToList();
                        }
                        break;
                    case "Length":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Length).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Length).ToList();
                        }
                        break;
                    case "TEUS":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.TEUS).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.TEUS).ToList();
                        }
                        break;
                    case "LinkMan":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkMan).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkMan).ToList();
                        }
                        break;
                    case "LinkPhone":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkPhone).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkPhone).ToList();
                        }
                        break;
                    case "LinkEmail":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkEmail).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkEmail).ToList();
                        }
                        break;
                    //case "WorkPlace":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.WorkPlace).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.WorkPlace).ToList();
                    //    }
                    //    break;
                    //case "ServiceNatureNames":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.ServiceNatureNames).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.ServiceNatureNames).ToList();
                    //    }
                    //    break;
                    case "WorkStateLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.WorkStateLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.WorkStateLabel).ToList();
                        }
                        break;
                    //case "BigTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.BigTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.BigTugNum).ToList();
                    //    }
                    //    break;
                    //case "MiddleTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.MiddleTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.MiddleTugNum).ToList();
                    //    }
                    //    break;
                    //case "SmallTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.SmallTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.SmallTugNum).ToList();
                    //    }
                    //    break;
                    case "Remark":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Remark).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Remark).ToList();
                        }
                        break;
                    case "CreateDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CreateDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CreateDate).ToList();
                        }
                        break;
                    case "LastUpDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LastUpDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LastUpDate).ToList();
                        }
                        break;


                    default:
                        break;
                }
                #endregion


                JArray groups = (JArray)jsonSearchOption["groups"];
                if (groups != null)
                {
                    foreach (JObject item in groups)
                    {
                        string item_groupOp = (string)item["groupOp"];
                        JArray item_groups = (JArray)item["groups"];
                        JArray item_rules = (JArray)item["rules"];
                        string item_rule0_field = (string)(((JObject)item_rules[0])["field"]);
                        string item_rule0_op = (string)(((JObject)item_rules[0])["op"]);
                        string item_rule0_data = (string)(((JObject)item_rules[0])["data"]);

                        string item_rule1_field = (string)(((JObject)item_rules[1])["field"]);
                        string item_rule1_op = (string)(((JObject)item_rules[1])["op"]);
                        string item_rule1_data = (string)(((JObject)item_rules[1])["data"]);
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return orders;
        }

        static public List<DataModel.V_OrderInfor> LoadDataForGenerateInvoice(string orderField, string orderMethod)
        {
            List<V_OrderInfor> orders = null;

            try
            {
                TugDataEntities db = new TugDataEntities();
                orders = db.V_OrderInfor.Where(u => (u.WorkStateID == 5
                    || u.WorkStateValue == "3"
                    || u.WorkStateLabel == "已完工") && u.HasInvoice != "是").Select(u => u).ToList<V_OrderInfor>();

                #region 根据排序字段和排序方式排序
                switch (orderField)
                {
                    case "":
                        {
                            //if(orderMethod.ToLower().Equals("asc"))
                            //    orders = orders.OrderBy(u => u.IDX).ToList();
                            //else
                            orders = orders.OrderByDescending(u => u.IDX).ToList();
                        }
                        break;
                    case "IsGuest":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.IsGuest).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.IsGuest).ToList();
                        }
                        break;
                    case "Code":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Code).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Code).ToList();
                        }
                        break;
                    case "CustomerName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CustomerName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CustomerName).ToList();
                        }
                        break;
                    case "OrdDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.OrdDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.OrdDate).ToList();
                        }
                        break;
                    //case "WorkDate":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.ServiceWorkDate).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.ServiceWorkDate).ToList();
                    //    }
                    //    break;
                    //case "WorkTime":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.WorkTime).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.WorkTime).ToList();
                    //    }
                    //    break;
                    //case "EstimatedCompletionTime":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.EstimatedCompletionTime).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.EstimatedCompletionTime).ToList();
                    //    }
                    //    break;
                    case "ShipName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipName).ToList();
                        }
                        break;
                    case "Length":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Length).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Length).ToList();
                        }
                        break;
                    case "TEUS":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.TEUS).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.TEUS).ToList();
                        }
                        break;
                    case "LinkMan":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkMan).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkMan).ToList();
                        }
                        break;
                    case "LinkPhone":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkPhone).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkPhone).ToList();
                        }
                        break;
                    case "LinkEmail":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LinkEmail).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LinkEmail).ToList();
                        }
                        break;
                    //case "WorkPlace":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.WorkPlace).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.WorkPlace).ToList();
                    //    }
                    //    break;
                    //case "ServiceNatureNames":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.ServiceNatureNames).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.ServiceNatureNames).ToList();
                    //    }
                    //    break;
                    case "WorkStateLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.WorkStateLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.WorkStateLabel).ToList();
                        }
                        break;
                    //case "BigTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.BigTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.BigTugNum).ToList();
                    //    }
                    //    break;
                    //case "MiddleTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.MiddleTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.MiddleTugNum).ToList();
                    //    }
                    //    break;
                    //case "SmallTugNum":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.SmallTugNum).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.SmallTugNum).ToList();
                    //    }
                    //    break;
                    case "Remark":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Remark).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Remark).ToList();
                        }
                        break;
                    case "CreateDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CreateDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CreateDate).ToList();
                        }
                        break;
                    case "LastUpDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LastUpDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LastUpDate).ToList();
                        }
                        break;
                    //case "UserDefinedCol1":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol1).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol1).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol2":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol2).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol2).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol3":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol3).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol3).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol4":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol4).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol4).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol5":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol5).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol5).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol6":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol6).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol6).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol7":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol7).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol7).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol8":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol8).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol8).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol9":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol9).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol9).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol10":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol10).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol10).ToList();
                    //    }
                    //    break;
                    default:
                        break;
                }

                #endregion
            }
            catch (Exception ex)
            {
                return null;
            }

            return orders;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderMethod">排序方式asc升序；desc降序</param>
        /// <returns></returns>
        static public List<DataModel.V_OrderScheduler> LoadDataForOrderScheduler(string orderField, string orderMethod, int orderId)
        {
            List<V_OrderScheduler> orders = null;

            try
            {
                TugDataEntities db = new TugDataEntities();
                orders = db.V_OrderScheduler.Where(u => u.OrderID == orderId).Select(u => u).ToList<V_OrderScheduler>();

                orderField = orderField.Split(',')[1];
                orderField = orderField.Trim();
                #region 根据排序字段和排序方式排序
                switch (orderField)
                {
                    case "":
                        {
                            //if (orderMethod.ToLower().Equals("asc"))
                            //    orders = orders.OrderBy(u => u.IDX).ToList();
                            //else
                                orders = orders.OrderByDescending(u => u.ServiceNatureID).ToList();
                        }
                        break;
                    case "ServiceNatureLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ServiceNatureLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ServiceNatureLabel).ToList();
                        }
                        break;
                    case "ServiceWorkDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ServiceWorkDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ServiceWorkDate).ToList();
                        }
                        break;
                    case "ServiceWorkTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ServiceWorkTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ServiceWorkTime).ToList();
                        }
                        break;
                    case "ServiceWorkPlace":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ServiceWorkPlace).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ServiceWorkPlace).ToList();
                        }
                        break;
                    case "ServiceJobStateLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ServiceJobStateLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ServiceJobStateLabel).ToList();
                        }
                        break;
                    case "TugName1":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.TugName1).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.TugName1).ToList();
                        }
                        break;
                    case "TugName2":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.TugName2).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.TugName2).ToList();
                        }
                        break;
                    case "SimpleName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.SimpleName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.SimpleName).ToList();
                        }
                        break;
                    //case "JobStateLabel":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.JobStateLabel).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.JobStateLabel).ToList();
                    //    }
                    //    break;
                    case "IsCaptainConfirm":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.IsCaptainConfirm).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.IsCaptainConfirm).ToList();
                        }
                        break;
                    case "InformCaptainTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.InformCaptainTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.InformCaptainTime).ToList();
                        }
                        break;
                    case "CaptainConfirmTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CaptainConfirmTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CaptainConfirmTime).ToList();
                        }
                        break;
                    case "DepartBaseTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.DepartBaseTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.DepartBaseTime).ToList();
                        }
                        break;
                    case "ArrivalShipSideTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ArrivalShipSideTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ArrivalShipSideTime).ToList();
                        }
                        break;
                    case "WorkCommencedTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.WorkCommencedTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.WorkCommencedTime).ToList();
                        }
                        break;
                    case "WorkCompletedTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.WorkCompletedTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.WorkCompletedTime).ToList();
                        }
                        break;
                    case "ArrivalBaseTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ArrivalBaseTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ArrivalBaseTime).ToList();
                        }
                        break;
                    
                    case "RopeUsed":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.RopeUsed).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.RopeUsed).ToList();
                        }
                        break;
                    case "RopeNum":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.RopeNum).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.RopeNum).ToList();
                        }
                        break;
                    
                    case "Remark":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Remark).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Remark).ToList();
                        }
                        break;
                    case "CreateDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CreateDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CreateDate).ToList();
                        }
                        break;
                    case "LastUpDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LastUpDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LastUpDate).ToList();
                        }
                        break;
                    //case "UserDefinedCol1":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol1).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol1).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol2":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol2).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol2).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol3":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol3).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol3).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol4":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol4).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol4).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol5":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol5).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol5).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol6":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol6).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol6).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol7":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol7).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol7).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol8":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol8).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol8).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol9":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol9).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol9).ToList();
                    //    }
                    //    break;
                    //case "UserDefinedCol10":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.UserDefinedCol10).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.UserDefinedCol10).ToList();
                    //    }
                    //    break;
                    default:
                        break;
                }

                #endregion
            }
            catch (Exception ex)
            {
                return null;
            }

            return orders;
        }




        static public List<DataModel.V_OrderScheduler> LoadDataForServiceScheduler(string orderField, string orderMethod, int orderServiceId)
        {
            List<V_OrderScheduler> orders = null;

            try
            {
                TugDataEntities db = new TugDataEntities();
                orders = db.V_OrderScheduler.Where(u => u.OrderServiceID == orderServiceId).Select(u => u).ToList<V_OrderScheduler>();

                //orderField = orderField.Split(',')[1];
                orderField = orderField.Trim();
                #region 根据排序字段和排序方式排序
                switch (orderField)
                {
                    case "":
                        {
                            //if (orderMethod.ToLower().Equals("asc"))
                            //    orders = orders.OrderBy(u => u.IDX).ToList();
                            //else
                            orders = orders.OrderByDescending(u => u.IDX).ToList();
                        }
                        break;
                    
                    case "TugName1":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.TugName1).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.TugName1).ToList();
                        }
                        break;
                    case "TugName2":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.TugName2).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.TugName2).ToList();
                        }
                        break;
                    case "SimpleName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.SimpleName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.SimpleName).ToList();
                        }
                        break;
                    //case "JobStateLabel":
                    //    {
                    //        if (orderMethod.ToLower().Equals("asc"))
                    //            orders = orders.OrderBy(u => u.JobStateLabel).ToList();
                    //        else
                    //            orders = orders.OrderByDescending(u => u.JobStateLabel).ToList();
                    //    }
                    //    break;
                    case "IsCaptainConfirm":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.IsCaptainConfirm).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.IsCaptainConfirm).ToList();
                        }
                        break;
                    case "InformCaptainTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.InformCaptainTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.InformCaptainTime).ToList();
                        }
                        break;
                    case "CaptainConfirmTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CaptainConfirmTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CaptainConfirmTime).ToList();
                        }
                        break;
                    case "DepartBaseTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.DepartBaseTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.DepartBaseTime).ToList();
                        }
                        break;
                    case "ArrivalShipSideTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ArrivalShipSideTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ArrivalShipSideTime).ToList();
                        }
                        break;
                    case "WorkCommencedTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.WorkCommencedTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.WorkCommencedTime).ToList();
                        }
                        break;
                    case "WorkCompletedTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.WorkCompletedTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.WorkCompletedTime).ToList();
                        }
                        break;
                    case "ArrivalBaseTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ArrivalBaseTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ArrivalBaseTime).ToList();
                        }
                        break;

                    case "RopeUsed":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.RopeUsed).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.RopeUsed).ToList();
                        }
                        break;
                    case "RopeNum":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.RopeNum).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.RopeNum).ToList();
                        }
                        break;

                    case "Remark":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Remark).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Remark).ToList();
                        }
                        break;
                    case "CreateDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CreateDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CreateDate).ToList();
                        }
                        break;
                    case "LastUpDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.LastUpDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.LastUpDate).ToList();
                        }
                        break;
                    default:
                        break;
                }

                #endregion
            }
            catch (Exception ex)
            {
                return null;
            }

            return orders;
        }



        /// <summary>
        /// 获取拖轮一日的时间安排状态
        /// </summary>
        /// <param name="tugId">拖轮ID</param>
        /// <param name="tugEx">拖轮对象</param>
        /// <param name="workDate">指定日期</param>
        /// <returns></returns>
        static public TugEx GetTugSchedulerBusyState(int tugId, TugEx tugEx, string workDate)
        {
            TugDataEntities db = new TugDataEntities();
            //string now = DateTime.Now.ToString("yyyy-MM-dd");

            List<V_OrderScheduler> schedulers = db.V_OrderScheduler.Where(u => u.TugID == tugId && u.ServiceWorkDate == workDate)
                .Select(u => u).OrderByDescending(u => u.IDX).ToList<V_OrderScheduler>();

            //schedulers[0].DepartBaseTime;
            //schedulers[0].ArrivalBaseTime;
            if(schedulers != null)
            {
                foreach (V_OrderScheduler item in schedulers)
                {
                    if(item.DepartBaseTime != null && item.ArrivalBaseTime != null)
                        tugEx = SetTugBusyState(item.DepartBaseTime.Trim(), item.ArrivalBaseTime.Trim(), tugEx);
                    //tugEx = SetTugBusyState("00:20", "00:30", tugEx);
                }
            }

            return tugEx;
        }

        static private TugEx SetTugBusyState(string startTime, string endTime, TugEx tugEx)
        {
            string[] _TimeInterval = {
                                         
                                         "00:30","01:00","01:30","02:00","02:30","03:00","03:30","04:00","04:30","05:00","05:30","06:00", 
                                         "06:30","07:00","07:30","08:00","08:30","09:00","09:30","10:00","10:30","11:00","11:30","12:00", 
                                         "12:30","13:00","13:30","14:00","14:30","15:00","15:30","16:00","16:30","17:00","17:30","18:00", 
                                         "18:30","19:00","19:30","20:00","20:30","21:00","21:30","22:00","22:30","23:00","23:30","24:00"
                                     };

            int[] _OriginCells = {
                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                                 };


            string s = _TimeInterval.First(u => u.CompareTo(startTime) > 0);
            string e = _TimeInterval.First(u => u.CompareTo(endTime) >= 0);

            int startIndex = _TimeInterval.ToList().IndexOf(s);
            int endIndex = _TimeInterval.ToList().IndexOf(e);


            for (int i = Math.Min(startIndex, endIndex); i <= Math.Max(startIndex, endIndex); i++)
            {
                if (_OriginCells[i] == 0) _OriginCells[i] = 1;
            }

            if (_OriginCells[0] == 1) tugEx.Cell0 = _OriginCells[0];
            if (_OriginCells[1] == 1) tugEx.Cell1 = _OriginCells[1];
            if (_OriginCells[2] == 1) tugEx.Cell2 = _OriginCells[2];
            if (_OriginCells[3] == 1) tugEx.Cell3 = _OriginCells[3];
            if (_OriginCells[4] == 1) tugEx.Cell4 = _OriginCells[4];
            if (_OriginCells[5] == 1) tugEx.Cell5 = _OriginCells[5];
            if (_OriginCells[6] == 1) tugEx.Cell6 = _OriginCells[6];
            if (_OriginCells[7] == 1) tugEx.Cell7 = _OriginCells[7];
            if (_OriginCells[8] == 1) tugEx.Cell8 = _OriginCells[8];
            if (_OriginCells[9] == 1) tugEx.Cell9 = _OriginCells[9];
            if (_OriginCells[10] == 1) tugEx.Cell10 = _OriginCells[10];
            if (_OriginCells[11] == 1) tugEx.Cell11 = _OriginCells[11];

            if (_OriginCells[12] == 1) tugEx.Cell12 = _OriginCells[12];
            if (_OriginCells[13] == 1) tugEx.Cell13 = _OriginCells[13];
            if (_OriginCells[14] == 1) tugEx.Cell14 = _OriginCells[14];
            if (_OriginCells[15] == 1) tugEx.Cell15 = _OriginCells[15];
            if (_OriginCells[16] == 1) tugEx.Cell16 = _OriginCells[16];
            if (_OriginCells[17] == 1) tugEx.Cell17 = _OriginCells[17];
            if (_OriginCells[18] == 1) tugEx.Cell18 = _OriginCells[18];
            if (_OriginCells[19] == 1) tugEx.Cell19 = _OriginCells[19];
            if (_OriginCells[20] == 1) tugEx.Cell20 = _OriginCells[20];
            if (_OriginCells[21] == 1) tugEx.Cell21 = _OriginCells[21];
            if (_OriginCells[22] == 1) tugEx.Cell22 = _OriginCells[22];
            if (_OriginCells[23] == 1) tugEx.Cell23 = _OriginCells[23];

            if (_OriginCells[24] == 1) tugEx.Cell24 = _OriginCells[24];
            if (_OriginCells[25] == 1) tugEx.Cell25 = _OriginCells[25];
            if (_OriginCells[26] == 1) tugEx.Cell26 = _OriginCells[26];
            if (_OriginCells[27] == 1) tugEx.Cell27 = _OriginCells[27];
            if (_OriginCells[28] == 1) tugEx.Cell28 = _OriginCells[28];
            if (_OriginCells[29] == 1) tugEx.Cell29 = _OriginCells[29];
            if (_OriginCells[30] == 1) tugEx.Cell30 = _OriginCells[30];
            if (_OriginCells[31] == 1) tugEx.Cell31 = _OriginCells[31];
            if (_OriginCells[32] == 1) tugEx.Cell32 = _OriginCells[32];
            if (_OriginCells[33] == 1) tugEx.Cell33 = _OriginCells[33];
            if (_OriginCells[34] == 1) tugEx.Cell34 = _OriginCells[34];
            if (_OriginCells[35] == 1) tugEx.Cell35 = _OriginCells[35];

            if (_OriginCells[36] == 1) tugEx.Cell36 = _OriginCells[36];
            if (_OriginCells[37] == 1) tugEx.Cell37 = _OriginCells[37];
            if (_OriginCells[38] == 1) tugEx.Cell38 = _OriginCells[38];
            if (_OriginCells[39] == 1) tugEx.Cell39 = _OriginCells[39];
            if (_OriginCells[40] == 1) tugEx.Cell40 = _OriginCells[40];
            if (_OriginCells[41] == 1) tugEx.Cell41 = _OriginCells[41];
            if (_OriginCells[42] == 1) tugEx.Cell42 = _OriginCells[42];
            if (_OriginCells[43] == 1) tugEx.Cell43 = _OriginCells[43];
            if (_OriginCells[44] == 1) tugEx.Cell44 = _OriginCells[44];
            if (_OriginCells[45] == 1) tugEx.Cell45 = _OriginCells[45];
            if (_OriginCells[46] == 1) tugEx.Cell46 = _OriginCells[46];
            if (_OriginCells[47] == 1) tugEx.Cell47 = _OriginCells[47];

            return tugEx;
        }


        /// <summary>
        /// 作业信息是否输入完成（每个调度的作业信息都输入才算完成）
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>true:完成；false:未完成</returns>

        static public bool OrderJobInformationInputIsComplete(int orderId)
        {
            bool ret = false;

            TugDataEntities db = new TugDataEntities();

            var list = db.V_OrderScheduler.Where(u => u.OrderID == orderId).ToList();
            if (list == null || list.Count == 0)
            {
                ret = false;
            }
            else
            {
                int i = 0;
                foreach (var item in list)
                {
                    if (item.ServiceJobStateID != 116 || item.ServiceJobStateValue != "2" || item.ServiceJobStateLabel != "已完工")
                    {
                        ret = false;
                        break;
                    }
                    else
                    {
                        if (item.DepartBaseTime == null || item.ArrivalBaseTime == null || item.DepartBaseTime.Trim() == "" || item.ArrivalBaseTime.Trim() == "")
                        {
                            ret = false;
                            break;
                        }
                    }
                    i++;
                }

                if(i >= list.Count)
                    ret = true;
            }

            return ret;
        }



        static public void GetStatusOfOrderInvoice(string selectedOrderIDs, out Dictionary<int,int> dicHasNoInvoice,
            out Dictionary<int, int> dicHasInvoiceNotInFlow, out Dictionary<int, int> dicHasInvoiceInFow, out Dictionary<int, int> dicHasInvoiceNotInFlowBills)
        {

            Dictionary<int, int> dicHasNoInvoice2 = new Dictionary<int, int>();
            Dictionary<int, int> dicHasInvoiceNotInFlow2 = new Dictionary<int, int>();
            Dictionary<int, int> dicHasInvoiceInFow2 = new Dictionary<int, int>();
            Dictionary<int, int> dicHasInvoiceNotInFlowBills2 = new Dictionary<int, int>();
            if (selectedOrderIDs != "")
            {
                List<string> list = selectedOrderIDs.Split(',').ToList(); //list中的每个元素的rowNo:orderId的格式
                if (list != null)
                {
                    foreach (string item in list)
                    {
                        int rowNo = Util.toint(item.Split(':')[0]);
                        int orderId = Util.toint(item.Split(':')[1]);
                        int billId;

                        string ret = GetStatusOfOrderInvoice(orderId, out billId);
                        if (ret == ConstValue.HAS_INVOICE_IN_FLOW) {
                            dicHasInvoiceInFow2.Add(rowNo, orderId);
                        }
                        else if (ret == ConstValue.HAS_INVOICE_NOT_IN_FLOW) {
                            dicHasInvoiceNotInFlow2.Add(rowNo, orderId);
                            dicHasInvoiceNotInFlowBills2.Add(rowNo, billId);
                        }
                        else
                        {
                            dicHasNoInvoice2.Add(rowNo, orderId);
                        }
                    }
                }
            }

            dicHasNoInvoice = dicHasNoInvoice2;
            dicHasInvoiceNotInFlow = dicHasInvoiceNotInFlow2;
            dicHasInvoiceInFow = dicHasInvoiceInFow2;
            dicHasInvoiceNotInFlowBills = dicHasInvoiceNotInFlowBills2;
        }

        
        /// <summary>
        /// 获取一个订单的账单状态
        /// </summary>
        /// <param name="orderId">订单Id</param>
        static private string GetStatusOfOrderInvoice(int orderId, out int billId)
        {
            billId = -1;

            string ret = ConstValue.HAS_NO_INVOICE;

            TugDataEntities db = new TugDataEntities();

            //V_OrderBilling ob = db.V_OrderBilling.FirstOrDefault(u => u.OrderID == orderId);

            V_BillingOrders ob = db.V_BillingOrders.FirstOrDefault(u => u.OrderID == orderId);
            if (ob != null)
            {
                //合并账单将此处删除
                //if (ob.BillingID == null)
                //{
                //    ret = ConstValue.HAS_NO_INVOICE;
                //}
                //else
                {
                    if (ob.Phase != 0)
                    {
                        ret = ConstValue.HAS_INVOICE_IN_FLOW;
                    }
                    else
                    {
                        billId = (int)ob.BillingID;
                        ret = ConstValue.HAS_INVOICE_NOT_IN_FLOW;
                    }
                }
            }

            return ret;
        }

        #region 更新OrderInfor的HasInFlow状态值
        public static void UpdateHasInFlow(int billid, string HasInFlow)
        {
            TugDataEntities db = new TugDataEntities();
            List<BillingOrder> list = db.BillingOrder.Where(u => u.BillingID == billid).OrderBy(u => u.IDX).ToList<BillingOrder>();
            for (int i = 0; i < list.Count; i++)
            {
                int ordid=Util.toint(list[i].OrderID);
                OrderInfor obj = db.OrderInfor.Where(u => u.IDX ==ordid).FirstOrDefault();
                if (obj != null)
                {
                    obj.HasInFlow = HasInFlow;
                    db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        #endregion



        /// <summary>
        /// 判断该订单下的服务是否生成过账单，包括普通账单和特殊账单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>-1：没有账单；1：特殊账单；其他：普通账单</returns>
        static public int HasBilling(int orderId)
        {
            int ret = -1;

            TugDataEntities db = new TugDataEntities();
            OrderService os = db.OrderService.FirstOrDefault(u => u.OrderID == orderId && u.HasBilling == "是");
            if (os != null)
            {
                ret = BusinessLogic.Module.Util.toint(os.BillingType);
                if (ret == 1)
                {
                    //特殊帳單
                }
                else
                {
                    //普通帳單
                }
            }
            return ret;
        }




        static public List<DataModel.V_OrderService> SearchForServiceScheduler2(string orderField, string orderMethod, string searchOptions)
        {
            List<V_OrderService> orders = null;
            try
            {
                //searchOptions的Json字符串格式
                //{
                //    "groupOp":"AND",
                //    "rules":[{"field":"IsGuest","op":"eq","data":"全部"}],
                //    "groups":[
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"BigTugNum"},{"data":"2","op":"le","field":"BigTugNum"}]},
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"MiddleTugNum"},{"data":"2","op":"le","field":"MiddleTugNum"}]},
                //        {"groupOp":"AND","groups":[],"rules":[{"data":"1","op":"ge","field":"SmallTugNum"},{"data":"2","op":"le","field":"SmallTugNum"}]}
                //    ]

                //}



                TugDataEntities db = new TugDataEntities();
                //orders = db.V_OrderInfor.Select(u => u).ToList<V_OrderInfor>();

                JObject jsonSearchOption = (JObject)JsonConvert.DeserializeObject(searchOptions);
                string groupOp = (string)jsonSearchOption["groupOp"];
                JArray rules = (JArray)jsonSearchOption["rules"];

                
                ParameterExpression parameter = Expression.Parameter(typeof(V_OrderService));
                Expression condition = Expression.NotEqual(Expression.PropertyOrField(parameter, "ServiceJobStateLabel"), Expression.Constant("已完工", typeof(string)));

                if (rules != null)
                {
                    foreach (JObject item in rules)
                    {
                        string field = (string)item["field"];
                        string op = (string)item["op"];
                        string data = (string)item["data"];

                        #region 根据各字段条件进行条件表达式拼接
                        switch (field)
                        {

                            #region ServiceWorkDate
                            case "ServiceWorkDate":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.WorkDate == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "ServiceWorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.WorkDate.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "ServiceWorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.WorkDate.CompareTo(data.Trim()) == -1 || u.WorkDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "ServiceWorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.WorkDate.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "ServiceWorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.WorkDate.CompareTo(data.Trim()) == 1 || u.WorkDate.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "ServiceWorkDate"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region ServiceWorkTime
                            case "ServiceWorkTime":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.WorkTime == data.Trim()).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "ServiceWorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.Equal(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == -1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "ServiceWorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == -1 || u.WorkTime.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "ServiceWorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.LessThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == 1).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "ServiceWorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThan(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.WorkTime.CompareTo(data.Trim()) == 1 || u.WorkTime.CompareTo(data.Trim()) == 0).ToList();
                                                Expression tmp = Expression.Call(Expression.PropertyOrField(parameter, "ServiceWorkTime"), typeof(String).GetMethod("CompareTo", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                                cdt = Expression.GreaterThanOrEqual(tmp, Expression.Constant(0, typeof(Int32)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region ShipName
                            case "ShipName":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "ShipName"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.ShipName.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ShipName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region ServiceNatureLabel
                            case "ServiceNatureLabel":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "ServiceNatureLabel"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceNatureLabel"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceNatureLabel"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceNatureLabel"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region ServiceWorkPlace
                            case "ServiceWorkPlace":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "ServiceWorkPlace"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceWorkPlace"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceWorkPlace"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceWorkPlace"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region SmallTugNum
                            case "SmallTugNum":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "SmallTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region BigTugNum
                            case "BigTugNum":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "BigTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region MiddleTugNum
                            case "MiddleTugNum":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThan(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum < Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.LessThanOrEqual(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThan(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                //orders = orders.Where(u => u.SmallTugNum > Convert.ToInt32(data.Trim()) || u.SmallTugNum == Convert.ToInt32(data.Trim())).ToList();
                                                cdt = Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameter, "MiddleTugNum"), Expression.Constant(Convert.ToInt32(data.Trim()), typeof(Nullable<double>)));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region ServiceJobStateLabel
                            case "ServiceJobStateLabel":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "ServiceJobStateLabel"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceJobStateLabel"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceJobStateLabel"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "ServiceJobStateLabel"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region TugNames
                            case "TugNames":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "TugNames"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "TugNames"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "TugNames"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "TugNames"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            #region CustomerName
                            case "CustomerName":
                                {
                                    Expression cdt = null;
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().CompareTo(data.Trim().ToLower()) == 0).ToList();
                                                cdt = Expression.Equal(Expression.PropertyOrField(parameter, "CustomerName"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().StartsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().EndsWith(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) }), Expression.Constant(data.Trim().ToLower(), typeof(String)));
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                //orders = orders.Where(u => u.WorkPlace.ToLower().Contains(data.Trim().ToLower())).ToList();
                                                cdt = Expression.Call(Expression.PropertyOrField(parameter, "CustomerName"), typeof(string).GetMethod("Contains"), Expression.Constant(data.Trim().ToLower()));
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (cdt != null)
                                    {
                                        condition = Expression.AndAlso(condition, cdt);
                                    }
                                }
                                break;
                            #endregion

                            default:
                                break;
                        }
                        #endregion

                    }

                }

                #region 执行查询
                if (condition != null)
                {
                    var lamda = Expression.Lambda<Func<V_OrderService, bool>>(condition, parameter);
                    orders = db.V_OrderService.Where(lamda).Select(u => u).OrderByDescending(u => u.ServiceWorkDate).ThenByDescending(u => u.ServiceWorkTime).ToList<V_OrderService>();
                }
                else
                {
                    orders = db.V_OrderService.Select(u => u).OrderByDescending(u => u.ServiceWorkDate).ThenByDescending(u => u.ServiceWorkTime).ToList<V_OrderService>();
                }
                #endregion


                #region 对搜索结果根据排序字段和方式进行排序
                switch (orderField)
                {
                    case "":
                        {

                            orders = orders.OrderByDescending(u => u.ServiceWorkDate).ThenByDescending(u => u.ServiceWorkTime).ToList();
                        }
                        break;
                    case "ServiceWorkDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ServiceWorkDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ServiceWorkDate).ToList();
                        }
                        break;
                    case "ServiceWorkTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ServiceWorkTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ServiceWorkTime).ToList();
                        }
                        break;

                    case "ShipName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipName).ToList();
                        }
                        break;

                    case "ServiceNatureLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ServiceNatureLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ServiceNatureLabel).ToList();
                        }
                        break;


                    case "ServiceWorkPlace":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ServiceWorkPlace).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ServiceWorkPlace).ToList();
                        }
                        break;

                    case "MiddleTugNum":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.MiddleTugNum).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.MiddleTugNum).ToList();
                        }
                        break;

                    case "BigTugNum":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.BigTugNum).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.BigTugNum).ToList();
                        }
                        break;

                    case "SmallTugNum":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.SmallTugNum).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.SmallTugNum).ToList();
                        }
                        break;

                    case "ServiceJobStateLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ServiceJobStateLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ServiceJobStateLabel).ToList();
                        }
                        break;
                    case "TugNames":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.TugNames).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.TugNames).ToList();
                        }
                        break;
                    case "CustomerName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CustomerName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CustomerName).ToList();
                        }
                        break;
                    
                    
                    

                    default:
                        break;
                }

                #endregion


                JArray groups = (JArray)jsonSearchOption["groups"];
                if (groups != null)
                {
                    foreach (JObject item in groups)
                    {
                        string item_groupOp = (string)item["groupOp"];
                        JArray item_groups = (JArray)item["groups"];
                        JArray item_rules = (JArray)item["rules"];
                        string item_rule0_field = (string)(((JObject)item_rules[0])["field"]);
                        string item_rule0_op = (string)(((JObject)item_rules[0])["op"]);
                        string item_rule0_data = (string)(((JObject)item_rules[0])["data"]);

                        string item_rule1_field = (string)(((JObject)item_rules[1])["field"]);
                        string item_rule1_op = (string)(((JObject)item_rules[1])["op"]);
                        string item_rule1_data = (string)(((JObject)item_rules[1])["data"]);
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return orders;
        }


        static public List<DataModel.V_OrderService> LoadDataForServiceScheduler2(string orderField, string orderMethod)
        {
            List<V_OrderService> orders = null;

            try
            {
                TugDataEntities db = new TugDataEntities();
                orders = db.V_OrderService.Where(u => u.ServiceJobStateLabel != "已完工").Select(u => u)
                        .OrderByDescending(u => u.ServiceWorkDate).ThenByDescending(u => u.ServiceWorkTime).ToList<V_OrderService>();

                #region 根据排序字段和排序方式进行排序
                switch (orderField)
                {
                    case "":
                        {

                            orders = orders.OrderByDescending(u => u.ServiceWorkDate).ThenByDescending(u => u.ServiceWorkTime).ToList();
                        }
                        break;
                    case "ServiceWorkDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ServiceWorkDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ServiceWorkDate).ToList();
                        }
                        break;
                    case "ServiceWorkTime":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ServiceWorkTime).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ServiceWorkTime).ToList();
                        }
                        break;

                    case "ShipName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipName).ToList();
                        }
                        break;

                    case "ServiceNatureLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ServiceNatureLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ServiceNatureLabel).ToList();
                        }
                        break;


                    case "ServiceWorkPlace":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ServiceWorkPlace).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ServiceWorkPlace).ToList();
                        }
                        break;

                    case "MiddleTugNum":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.MiddleTugNum).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.MiddleTugNum).ToList();
                        }
                        break;

                    case "BigTugNum":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.BigTugNum).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.BigTugNum).ToList();
                        }
                        break;

                    case "SmallTugNum":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.SmallTugNum).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.SmallTugNum).ToList();
                        }
                        break;

                    case "ServiceJobStateLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ServiceJobStateLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ServiceJobStateLabel).ToList();
                        }
                        break;
                    case "TugNames":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.TugNames).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.TugNames).ToList();
                        }
                        break;
                    case "CustomerName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.CustomerName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.CustomerName).ToList();
                        }
                        break;




                    default:
                        break;
                }

                #endregion
            }
            catch (Exception ex)
            {
                return null;
            }

            return orders;
        }




    }



}

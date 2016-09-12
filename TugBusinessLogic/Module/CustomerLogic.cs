using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using System.Web.Mvc;

namespace BusinessLogic.Module
{
    public class CustomerLogic
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchOptions">搜索选项，格式如下</param>
        /// <returns></returns>
        static public List<DataModel.V_BillingTemplate> SearchForCustomerBillingTemplate(string orderField, string orderMethod, string searchOptions, int custId)
        {
            List<V_BillingTemplate> orders = null;
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
                orders = db.V_BillingTemplate.Where(u => u.CustomerID == custId).Select(u => u).ToList<V_BillingTemplate>();

                JObject jsonSearchOption = (JObject)JsonConvert.DeserializeObject(searchOptions);
                string groupOp = (string)jsonSearchOption["groupOp"];
                JArray rules = (JArray)jsonSearchOption["rules"];


                //Expression condition = Expression.Equal(Expression.Constant(1, typeof(int)), Expression.Constant(1, typeof(int)));
                //ParameterExpression parameter = Expression.Parameter(typeof(V_OrderInfor));


                if (rules != null)
                {
                    foreach (JObject item in rules)
                    {
                        string field = (string)item["field"];
                        string op = (string)item["op"];
                        string data = (string)item["data"];

                        #region 根据各自段条件进行搜索
                        switch (field)
                        {
                            #region BillingTemplateTypeLabel
                            case "BillingTemplateTypeLabel":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                int billingTemplateTypeID = Convert.ToInt32(data.Split('~')[0]);
                                                if (billingTemplateTypeID != -1)
                                                    orders = orders.Where(u => u.BillingTemplateTypeID == billingTemplateTypeID).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region BillingTemplateCode
                            case "BillingTemplateCode":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.BillingTemplateCode.ToLower().CompareTo(data.ToLower()) == 0).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                orders = orders.Where(u => u.BillingTemplateCode.ToLower().StartsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                orders = orders.Where(u => u.BillingTemplateCode.ToLower().EndsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                orders = orders.Where(u => u.BillingTemplateCode.ToLower().Contains(data.ToLower())).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region BillingTemplateName
                            case "BillingTemplateName":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.BillingTemplateName.ToLower().CompareTo(data.ToLower()) == 0).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                orders = orders.Where(u => u.BillingTemplateName.ToLower().StartsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                orders = orders.Where(u => u.BillingTemplateName.ToLower().EndsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                orders = orders.Where(u => u.BillingTemplateName.ToLower().Contains(data.ToLower())).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region TimeTypeLabel
                            case "TimeTypeLabel":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                int timeTypeID = Convert.ToInt32(data.Split('~')[0]);
                                                if (timeTypeID != -1)
                                                {
                                                    orders = orders.Where(u => u.TimeTypeID == timeTypeID).ToList();
                                                }

                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region Discount
                            case "Discount":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.Discount == Convert.ToDouble(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                orders = orders.Where(u => u.Discount < Convert.ToDouble(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                orders = orders.Where(u => u.Discount <= Convert.ToDouble(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                orders = orders.Where(u => u.Discount > Convert.ToDouble(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                orders = orders.Where(u => u.Discount >= Convert.ToDouble(data)).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                        

                            

                            #region ShipLength
                            case "ShipLength":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.ShipLength.ToLower().CompareTo(data.ToLower()) == 0).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                orders = orders.Where(u => u.ShipLength.ToLower().StartsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                orders = orders.Where(u => u.ShipLength.ToLower().EndsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                orders = orders.Where(u => u.ShipLength.ToLower().Contains(data.ToLower())).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region ShipTEUS
                            case "ShipTEUS":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.ShipTEUS.ToLower().CompareTo(data.ToLower()) == 0).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                orders = orders.Where(u => u.ShipTEUS.ToLower().StartsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                orders = orders.Where(u => u.ShipTEUS.ToLower().EndsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                orders = orders.Where(u => u.ShipTEUS.ToLower().Contains(data.ToLower())).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region ExpiryDate
                            case "ExpiryDate":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.ExpiryDate == data).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                orders = orders.Where(u => u.ExpiryDate.CompareTo(data) == -1).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                orders = orders.Where(u => u.ExpiryDate.CompareTo(data) == -1 || u.ExpiryDate.CompareTo(data) == 0).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                orders = orders.Where(u => u.ExpiryDate.CompareTo(data) == 1).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                orders = orders.Where(u => u.ExpiryDate.CompareTo(data) == 1 || u.ExpiryDate.CompareTo(data) == 0).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region TemplateCreditContent
                            case "TemplateCreditContent":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.TemplateCreditContent.ToLower().CompareTo(data.ToLower()) == 0).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                orders = orders.Where(u => u.TemplateCreditContent.ToLower().StartsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                orders = orders.Where(u => u.TemplateCreditContent.ToLower().EndsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                orders = orders.Where(u => u.TemplateCreditContent.ToLower().Contains(data.ToLower())).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region Remark
                            case "Remark":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.Remark.ToLower().CompareTo(data.ToLower()) == 0).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                orders = orders.Where(u => u.Remark.ToLower().StartsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                orders = orders.Where(u => u.Remark.ToLower().EndsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                orders = orders.Where(u => u.Remark.ToLower().Contains(data.ToLower())).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region CreateDate
                            case "CreateDate":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.CreateDate == data).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                orders = orders.Where(u => u.CreateDate.CompareTo(data) == -1).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                orders = orders.Where(u => u.CreateDate.CompareTo(data) == -1 || u.CreateDate.CompareTo(data) == 0).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                orders = orders.Where(u => u.CreateDate.CompareTo(data) == 1).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                orders = orders.Where(u => u.CreateDate.CompareTo(data) == 1 || u.CreateDate.CompareTo(data) == 0).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region LastUpDate
                            case "LastUpDate":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.LastUpDate == data).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                orders = orders.Where(u => u.LastUpDate.CompareTo(data) == -1).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                orders = orders.Where(u => u.LastUpDate.CompareTo(data) == -1 || u.LastUpDate.CompareTo(data) == 0).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                orders = orders.Where(u => u.LastUpDate.CompareTo(data) == 1).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                orders = orders.Where(u => u.LastUpDate.CompareTo(data) == 1 || u.LastUpDate.CompareTo(data) == 0).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol1
                            case "UserDefinedCol1":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol1.ToLower().CompareTo(data.ToLower()) == 0).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol1.ToLower().StartsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol1.ToLower().EndsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol1.ToLower().Contains(data.ToLower())).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol2
                            case "UserDefinedCol2":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol2.ToLower().CompareTo(data.ToLower()) == 0).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol2.ToLower().StartsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol2.ToLower().EndsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol2.ToLower().Contains(data.ToLower())).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol3
                            case "UserDefinedCol3":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol3.ToLower().CompareTo(data.ToLower()) == 0).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol3.ToLower().StartsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol3.ToLower().EndsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol3.ToLower().Contains(data.ToLower())).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol4
                            case "UserDefinedCol4":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol4.ToLower().CompareTo(data.ToLower()) == 0).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol4.ToLower().StartsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_EW:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol4.ToLower().EndsWith(data.ToLower())).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_CN:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol4.ToLower().Contains(data.ToLower())).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol5
                            case "UserDefinedCol5":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol5 == Convert.ToDouble(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol5 < Convert.ToDouble(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol5 < Convert.ToDouble(data) || u.UserDefinedCol5 == Convert.ToDouble(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol5 > Convert.ToDouble(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol5 > Convert.ToDouble(data) || u.UserDefinedCol5 == Convert.ToDouble(data)).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol6
                            case "UserDefinedCol6":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol6 == Convert.ToInt32(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol6 < Convert.ToInt32(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol6 < Convert.ToInt32(data) || u.UserDefinedCol6 == Convert.ToInt32(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol6 > Convert.ToInt32(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol6 > Convert.ToInt32(data) || u.UserDefinedCol6 == Convert.ToInt32(data)).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol7
                            case "UserDefinedCol7":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol7 == Convert.ToInt32(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol7 < Convert.ToInt32(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol7 < Convert.ToInt32(data) || u.UserDefinedCol7 == Convert.ToInt32(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol7 > Convert.ToInt32(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol7 > Convert.ToInt32(data) || u.UserDefinedCol7 == Convert.ToInt32(data)).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol8
                            case "UserDefinedCol8":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol8 == Convert.ToInt32(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol8 < Convert.ToInt32(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol8 < Convert.ToInt32(data) || u.UserDefinedCol8 == Convert.ToInt32(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol8 > Convert.ToInt32(data)).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol8 > Convert.ToInt32(data) || u.UserDefinedCol8 == Convert.ToInt32(data)).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol9
                            case "UserDefinedCol9":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol9 == data).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol9.CompareTo(data) == -1).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol9.CompareTo(data) == -1 || u.UserDefinedCol9.CompareTo(data) == 0).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol9.CompareTo(data) == 1).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol9.CompareTo(data) == 1 || u.UserDefinedCol9.CompareTo(data) == 0).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            #region UserDefinedCol10
                            case "UserDefinedCol10":
                                {
                                    switch (op)
                                    {
                                        case ConstValue.ComparisonOperator_EQ:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol10 == data).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LT:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol10.CompareTo(data) == -1).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_LE:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol10.CompareTo(data) == -1 || u.UserDefinedCol10.CompareTo(data) == 0).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GT:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol10.CompareTo(data) == 1).ToList();
                                            }
                                            break;
                                        case ConstValue.ComparisonOperator_GE:
                                        case ConstValue.ComparisonOperator_BW:
                                            {
                                                orders = orders.Where(u => u.UserDefinedCol10.CompareTo(data) == 1 || u.UserDefinedCol10.CompareTo(data) == 0).ToList();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            #endregion

                            default:
                                break;
                        }
                        #endregion

                        #region 对搜索结果根据排序字段和方式进行排序
                        switch (orderField)
                        {
                            case "":
                                {
                                    orders = orders.OrderByDescending(u => u.BillingTemplateIDX).ToList();
                                }
                                break;
                            case "BillingTemplateTypeLabel":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.BillingTemplateTypeLabel).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.BillingTemplateTypeLabel).ToList();
                                }
                                break;
                            case "BillingTemplateCode":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.BillingTemplateCode).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.BillingTemplateCode).ToList();
                                }
                                break;
                            case "BillingTemplateName":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.BillingTemplateName).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.BillingTemplateName).ToList();
                                }
                                break;
                            
                            case "TimeTypeLabel":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.TimeTypeLabel).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.TimeTypeLabel).ToList();
                                }
                                break;

                            case "Discount":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.Discount).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.Discount).ToList();
                                }
                                break;
                            case "ShipLength":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.ShipLength).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.ShipLength).ToList();
                                }
                                break;
                            case "ShipTEUS":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.ShipTEUS).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.ShipTEUS).ToList();
                                }
                                break;
                            case "ExpiryDate":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.ExpiryDate).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.ExpiryDate).ToList();
                                }
                                break;
                            case "TemplateCreditContent":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.TemplateCreditContent).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.TemplateCreditContent).ToList();
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
                            case "UserDefinedCol1":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.UserDefinedCol1).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.UserDefinedCol1).ToList();
                                }
                                break;
                            case "UserDefinedCol2":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.UserDefinedCol2).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.UserDefinedCol2).ToList();
                                }
                                break;
                            case "UserDefinedCol3":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.UserDefinedCol3).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.UserDefinedCol3).ToList();
                                }
                                break;
                            case "UserDefinedCol4":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.UserDefinedCol4).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.UserDefinedCol4).ToList();
                                }
                                break;
                            case "UserDefinedCol5":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.UserDefinedCol5).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.UserDefinedCol5).ToList();
                                }
                                break;
                            case "UserDefinedCol6":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.UserDefinedCol6).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.UserDefinedCol6).ToList();
                                }
                                break;
                            case "UserDefinedCol7":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.UserDefinedCol7).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.UserDefinedCol7).ToList();
                                }
                                break;
                            case "UserDefinedCol8":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.UserDefinedCol8).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.UserDefinedCol8).ToList();
                                }
                                break;
                            case "UserDefinedCol9":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.UserDefinedCol9).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.UserDefinedCol9).ToList();
                                }
                                break;
                            case "UserDefinedCol10":
                                {
                                    if (orderMethod.ToLower().Equals("asc"))
                                        orders = orders.OrderBy(u => u.UserDefinedCol10).ToList();
                                    else
                                        orders = orders.OrderByDescending(u => u.UserDefinedCol10).ToList();
                                }
                                break;
                            default:
                                break;
                        }
                        #endregion
                    }
                }

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
        /// 
        /// </summary>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderMethod">排序方式asc升序；desc降序</param>
        /// <returns></returns>
        static public List<DataModel.V_BillingTemplate> LoadDataForCustomerBillingTemplate(string orderField, string orderMethod, int custId)
        {
            List<V_BillingTemplate> orders = null;

            try
            {
                TugDataEntities db = new TugDataEntities();
                orders = db.V_BillingTemplate.Where(u => u.CustomerID == custId).Select(u => u).ToList<V_BillingTemplate>();

                #region 根据排序字段和排序方式排序
                switch (orderField)
                {
                    case "":
                        {
                            orders = orders.OrderByDescending(u => u.BillingTemplateIDX).ToList();
                        }
                        break;
                    case "BillingTemplateTypeLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.BillingTemplateTypeLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.BillingTemplateTypeLabel).ToList();
                        }
                        break;
                    case "BillingTemplateCode":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.BillingTemplateCode).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.BillingTemplateCode).ToList();
                        }
                        break;
                    case "BillingTemplateName":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.BillingTemplateName).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.BillingTemplateName).ToList();
                        }
                        break;

                    case "TimeTypeLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.TimeTypeLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.TimeTypeLabel).ToList();
                        }
                        break;

                    case "Discount":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Discount).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Discount).ToList();
                        }
                        break;
                    case "ShipLength":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipLength).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipLength).ToList();
                        }
                        break;
                    case "ShipTEUS":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ShipTEUS).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ShipTEUS).ToList();
                        }
                        break;
                    case "ExpiryDate":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ExpiryDate).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ExpiryDate).ToList();
                        }
                        break;
                    case "TemplateCreditContent":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.TemplateCreditContent).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.TemplateCreditContent).ToList();
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
                    case "UserDefinedCol1":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol1).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol1).ToList();
                        }
                        break;
                    case "UserDefinedCol2":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol2).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol2).ToList();
                        }
                        break;
                    case "UserDefinedCol3":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol3).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol3).ToList();
                        }
                        break;
                    case "UserDefinedCol4":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol4).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol4).ToList();
                        }
                        break;
                    case "UserDefinedCol5":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol5).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol5).ToList();
                        }
                        break;
                    case "UserDefinedCol6":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol6).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol6).ToList();
                        }
                        break;
                    case "UserDefinedCol7":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol7).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol7).ToList();
                        }
                        break;
                    case "UserDefinedCol8":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol8).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol8).ToList();
                        }
                        break;
                    case "UserDefinedCol9":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol9).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol9).ToList();
                        }
                        break;
                    case "UserDefinedCol10":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol10).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol10).ToList();
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
        /// 
        /// </summary>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderMethod">排序方式asc升序；desc降序</param>
        /// <returns></returns>
        static public List<DataModel.V_BillingItemTemplate> LoadDataForCustomerBillingItemTemplate(string orderField, string orderMethod, int billSchemeId)
        {
            List<V_BillingItemTemplate> orders = null;

            try
            {
                TugDataEntities db = new TugDataEntities();
                orders = db.V_BillingItemTemplate.Where(u => u.BillingTemplateID == billSchemeId).Select(u => u).ToList<V_BillingItemTemplate>();

                #region 根据排序字段和排序方式排序
                switch (orderField)
                {
                    case "":
                        {
                            orders = orders.OrderByDescending(u => u.IDX).ToList();
                        }
                        break;
                    case "ItemLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.ItemLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.ItemLabel).ToList();
                        }
                        break;
                    case "UnitPrice":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UnitPrice).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UnitPrice).ToList();
                        }
                        break;
                    case "Currency":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.Currency).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.Currency).ToList();
                        }
                        break;

                    case "TypeLabel":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.TypeLabel).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.TypeLabel).ToList();
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
                    case "UserDefinedCol1":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol1).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol1).ToList();
                        }
                        break;
                    case "UserDefinedCol2":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol2).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol2).ToList();
                        }
                        break;
                    case "UserDefinedCol3":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol3).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol3).ToList();
                        }
                        break;
                    case "UserDefinedCol4":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol4).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol4).ToList();
                        }
                        break;
                    case "UserDefinedCol5":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol5).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol5).ToList();
                        }
                        break;
                    case "UserDefinedCol6":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol6).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol6).ToList();
                        }
                        break;
                    case "UserDefinedCol7":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol7).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol7).ToList();
                        }
                        break;
                    case "UserDefinedCol8":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol8).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol8).ToList();
                        }
                        break;
                    case "UserDefinedCol9":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol9).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol9).ToList();
                        }
                        break;
                    case "UserDefinedCol10":
                        {
                            if (orderMethod.ToLower().Equals("asc"))
                                orders = orders.OrderBy(u => u.UserDefinedCol10).ToList();
                            else
                                orders = orders.OrderByDescending(u => u.UserDefinedCol10).ToList();
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

        static public string GetBillingTemplateItems()
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
                labels = new string[list.Count,3];
                foreach (var itm in list)
                {
                    labels[i,0] = itm.CustomLabel;
                    labels[i, 2] = "港幣";
                    i++;
                }
            }
            //return labels;
            return JsonConvert.SerializeObject(labels);
        }

        static public List<CustomField> GetPriceItems()
        {
            TugDataEntities db = new TugDataEntities();
            List<CustomField> src = db.CustomField.Where(u => u.CustomName == "OrderInfor.ServiceNatureID"
                || (u.CustomName == "BillingItemTemplate.ItemID" && (u.CustomValue == "C78" || u.IDX == 40 || u.CustomLabel == "折扣"))
                || (u.CustomName == "BillingItemTemplate.ItemID" && (u.CustomValue == "C82" || u.IDX == 23 || u.CustomLabel == "拖缆费"))
                || (u.CustomName == "BillingItemTemplate.ItemID" && (u.CustomValue == "E80" || u.IDX == 119 || u.CustomLabel == "燃油附加费折扣")))
                .OrderBy(u => u.CustomValue).ToList<CustomField>();

            return src;
        }

        [JsonExceptionFilterAttribute]
        public static int AutoAddCustomer(string Code, string Name1, string Name2, string SimpleName, string ContactPerson,
      string Telephone, string Fax, string Email, string Address, string MailCode, string Remark, int UserID)
        {
            try
            {
                TugDataEntities db = new TugDataEntities();
                System.Linq.Expressions.Expression<Func<Customer, bool>> exp = u => u.Name1 == Name1;
                Customer obj = db.Customer.Where(exp).FirstOrDefault();
                if (obj != null)
                {
                    //var ret = new { code = Resources.Common.FAIL_CODE, message = Resources.Common.FAIL_MESSAGE, objid=obj.IDX };
                    //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                    //return Json(ret);
                    //throw new Exception("客户名称已存在！");
                    return obj.IDX;
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
                    cstmer.UserID = UserID;
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

                    //var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE, objid = cstmer.IDX };
                    //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                    //return Json(ret);
                    return cstmer.IDX;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [JsonExceptionFilterAttribute]
        static public int AutoAddCustomerShip(int ctmId, string Name1, string Name2, string SimpleName, string DeadWeight, string Length,
            string Width, string TEUS, string Class, string Remark, int UserID)
        {
            try
            {
                TugDataEntities db = new TugDataEntities();
                System.Linq.Expressions.Expression<Func<CustomerShip, bool>> exp = u => u.Name1 == Name1;
                CustomerShip obj = db.CustomerShip.Where(exp).FirstOrDefault();
                if (obj != null)
                {
                    //var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE, objid = obj.IDX };
                    //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                    //return Json(ret);
                    //throw new Exception("船名称已存在！");
                    return obj.IDX;
                }
                {
                    DataModel.CustomerShip ship = new CustomerShip();

                    ship.CustomerID = ctmId;// Util.toint(Request.Form["CustomerID"]);
                    ship.ShipTypeID = -1;//Util.toint(Request.Form["ShipTypeID"]);
                    ship.Name1 = Name1;
                    ship.Name2 = Name2;
                    ship.SimpleName = SimpleName;
                    ship.DeadWeight = Util.toint(DeadWeight);
                    ship.Length = Util.toint(Length);
                    ship.Width = Util.toint(Width);
                    ship.TEUS = Util.toint(TEUS);
                    ship.Class = Class;
                    ship.Remark = Remark;
                    ship.OwnerID = -1;
                    ship.CreateDate = ship.LastUpDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;
                    ship.UserID = UserID;
                    ship.UserDefinedCol1 = "";
                    ship.UserDefinedCol2 = "";
                    ship.UserDefinedCol3 = "";
                    ship.UserDefinedCol4 = "";

                    //if (Request.Form["UserDefinedCol5"] != "")
                    //    ship.UserDefinedCol5 = Util.tonumeric(Request.Form["UserDefinedCol5"]);

                    //if (Request.Form["UserDefinedCol6"] != "")
                    //    ship.UserDefinedCol6 = Util.toint(Request.Form["UserDefinedCol6"]);

                    //if (Request.Form["UserDefinedCol7"] != "")
                    //    ship.UserDefinedCol7 = Util.toint(Request.Form["UserDefinedCol7"]);

                    //if (Request.Form["UserDefinedCol8"] != "")
                    //    ship.UserDefinedCol8 = Util.toint(Request.Form["UserDefinedCol8"]);

                    ship.UserDefinedCol9 = "";
                    ship.UserDefinedCol10 = "";

                    ship = db.CustomerShip.Add(ship);
                    db.SaveChanges();

                    //var ret = new { code = Resources.Common.SUCCESS_CODE, message = Resources.Common.SUCCESS_MESSAGE, objid=ship.IDX };
                    //Response.Write(@Resources.Common.SUCCESS_MESSAGE);
                    //return Json(ret);
                    return ship.IDX;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //var ret = new { code = Resources.Common.EXCEPTION_CODE, message = Resources.Common.EXCEPTION_MESSAGE };
                //return Json(ret);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BusinessLogic
{
    public static class SessionExtension
    {
        public static T GetDataFromSession<T>(this HttpSessionStateBase session, string key)
        {
            try
            {
                return (T)session[key];
            }
            catch (Exception ex)
            {
                //return System.Web.HttpContext.Current.Response.Redirect("Home\Login");
                throw;
            }
           

        }

        public static void SetDataInSession<T>(this HttpSessionStateBase session, string key, object value)
        {
            session[key] = value;
        }
    }
}

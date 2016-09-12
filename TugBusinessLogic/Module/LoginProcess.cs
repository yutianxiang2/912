using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace BusinessLogic.Module
{
    public class LoginProcess
    {
        public bool IsValidUser(UserInfor u)
        {
            if (u.UserName == "419" && u.Pwd == "419")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
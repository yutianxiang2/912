using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Module
{
    public static class Util
    {
        public static string GetSequence(string type)
        {
            string sequence = null;
            DateTime? now = DateTime.Now;

            if (now != null)
            {
                sequence = string.Format("{0}{1:D4}{2:D2}{3:D2}{4:D2}{5:D2}{6:D2}{7:D3}", type, now.Value.Year, now.Value.Month, now.Value.Day, now.Value.Hour, now.Value.Minute, now.Value.Second, now.Value.Millisecond);
                //sequence = type + now.Value.Year + now.Value.Month + now.Value.Day + now.Value.Hour + now.Value.Minute + now.Value.Second + now.Value.Millisecond;
            }
            return sequence;
        }
        public static string checkdbnull(object obj)
        {
            try
            {
                if (obj is DBNull)
                    return "";
                else if (obj == null)
                    return "";
                else
                    return obj.ToString().Trim();
            }
            catch (Exception er)
            {
                return "";
            }
        }

        public static int toint(object obj)
        {
            int rValue;
            if (obj is DBNull) rValue = 0;
            else if (obj == null) rValue = 0;
            else
                try
                {
                    rValue = Convert.ToInt32(obj);
                }
                catch (Exception)
                {
                    rValue = 0;
                }
            return rValue;
        }

        public static double tonumeric(object obj)
        {
            double rValue;
            if (obj is DBNull) rValue = 0;
            else if (obj == null) rValue = 0;
            else
                try
                {
                    rValue = Convert.ToDouble(obj);
                }
                catch (Exception)
                {
                    rValue = 0;
                }
            return rValue;
        }
    }
}
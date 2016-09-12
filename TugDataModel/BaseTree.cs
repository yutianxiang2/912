using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class BaseTree
    {
        private string _expanded = "false";
        private string _loaded = "true";

        public string expanded
        {
            get { return _expanded; }
            set { _expanded = value; }
        }

        public string loaded
        {
            get { return _loaded; }
            set { _loaded = value; }
        }
    }
}
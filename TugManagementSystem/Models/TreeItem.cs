using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModel;

namespace WMS.Models
{
    public class TreeItem : V_BaseTreeItems
    {
        public string expanded { get; set; }
        public string loaded { get; set; }
    }
}
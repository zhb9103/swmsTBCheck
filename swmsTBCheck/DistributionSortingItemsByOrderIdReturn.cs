using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swmsTBCheck
{
    public class DistributionSortingItemsByOrderIdReturn
    {
        public Datum[] data { get; set; }
        public string code { get; set; }
        public string msg { get; set; }

        public class Datum
        {
            public int id { get; set; }
            public String tid { get; set; }
            public string tradeOrderNo { get; set; }
            public Tradeorderitem[] tradeOrderItems { get; set; }
        }

        public class Tradeorderitem
        {
            public int id { get; set; }
            public string skuNo { get; set; }
            public string skuName { get; set; }
            public int count { get; set; }
        }

    }
}

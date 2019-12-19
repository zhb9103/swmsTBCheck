using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swmsTBCheck
{
    public class UnfinishedDistributionSortingListReturn
    {

        public Datum[] data { get; set; }
        public string code { get; set; }
        public string msg { get; set; }

        public class Datum
        {
            public int id { get; set; }
            public string sortingOrderNo { get; set; }
            public string tradeOrderNo { get; set; }
            public String tid { get; set; }
            public int status { get; set; }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swmsTBCheck
{
    public class DistributionSortingPara
    {
        public int id { get; set; }
        public Itemlist[] itemList { get; set; }

        public class Itemlist
        {
            public int tradeOrderId { get; set; }
            public string[] epcList { get; set; }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swmsTBCheck
{
    public class FindEPCReturn
    {

        public Data data { get; set; }
        public string code { get; set; }
        public string msg { get; set; }

        public class Data
        {
            public string id { get; set; }
            public string createDate { get; set; }
            public string updateDate { get; set; }
            public string deletedState { get; set; }
            public object createUserId { get; set; }
            public object updateUserId { get; set; }
            public string tenantId { get; set; }
            public string storeId { get; set; }
            public string type { get; set; }
            public string model { get; set; }
            public string price { get; set; }
            public string orderNo { get; set; }
            public string tradeType { get; set; }
            public string tradeOrderNo { get; set; }
            public object returnOrderNo { get; set; }
            public string supplierName { get; set; }
            public string boxCode { get; set; }
            public string epcStatus { get; set; }
            public string sku { get; set; }
            public string epc { get; set; }
        }

    }
}

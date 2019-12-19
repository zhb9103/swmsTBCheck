using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swmsTBCheck
{
    class GenOrder
    {

        public string group { get; set; }
        public string userID { get; set; }
        public string orderNo { get; set; }
        public Expressinfo expressInfo { get; set; }
        public Invoiceinfo invoiceInfo { get; set; }
        public Itemslist[] itemsList { get; set; }

        public class Expressinfo
        {
            public string expressName { get; set; }
            public string expressCell { get; set; }
            public string expressPhone { get; set; }
            public string expressProvince { get; set; }
            public string expressCity { get; set; }
            public string expressDistrict { get; set; }
            public string expressAddr { get; set; }
            public string expressPost { get; set; }
        }

        public class Invoiceinfo
        {
            public string invoiceType { get; set; }
            public string invoiceTitle { get; set; }
            public string invoiceTaxNum { get; set; }
        }

        public class Itemslist
        {
            public string skuNo { get; set; }
            public string skuName { get; set; }
            public string price { get; set; }
            public string salePrice { get; set; }
            public int count { get; set; }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swmsTBCheck
{
    public class OrderItem
    {
        // OrderId;
        public Boolean isChecked;
        public Boolean isCanceled;
        public int id;
        public int epcsTotal;
        public int epcs_i;
        public String tradeOrderNo;
        public String tid;
        public Boolean isFull;
        public TradeOrderItem[] tradeOrderItem;
        //public int tradeOrderItem_i;
        public int tradeOrderItemTotal;
        public int position;
        
        public class TradeOrderItem
        {
            public String skuNo;
            public String skuName;
            public int orderId;
            public String[] epcStr;
            public int epcStr_i;
            public int epcStrTotal;
            public Boolean isFull;
        }
    }
}

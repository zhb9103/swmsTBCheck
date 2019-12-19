using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swmsTBCheck
{
    public class DistributionPlatform
    {
        public static String url_system = "http://222.186.36.185:9105";
        public static String url_order = "http://222.186.36.185:9100";
        public static String ip_address = "222.186.36.185";
	// 47.105.125.47

        public static UnfinishedDistributionSortingListReturn GetUnfinishedDistributionSortingList()
        {
            UnfinishedDistributionSortingListReturn result;
            try
            {
                var client = new RestClient();
                client.EndPoint = @""+ url_system + "/sorting/getUnfinishedDistributionSortingList.json";
                var json = client.MakeRequest();
                result = JsonHelper.DeserializeJsonToObject<UnfinishedDistributionSortingListReturn>(json);
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public static DistributionSortingItemsByOrderIdReturn GetDistributionSortingItemsByOrderId(DistributionSortingItemsByOrderIdPara distributionSortingItemsByOrderIdPara)
        {
            DistributionSortingItemsByOrderIdReturn result;
            try
            {
                var client = new RestClient();
                client.EndPoint = @"" + url_system + "/sorting/getDistributionSortingItemsByOrderId.json"; ;
                client.ContentType = "application/json";
                //DistributionSortingItemsByOrderIdPara distributionSortingItemsByOrderIdPara;
                //distributionSortingItemsByOrderIdPara = new DistributionSortingItemsByOrderIdPara();
                //distributionSortingItemsByOrderIdPara.id = 264;
                client.Method = HttpVerb.POST;
                client.PostData = JsonHelper.SerializeObject(distributionSortingItemsByOrderIdPara);// "{\"id\":263}";
                var json = client.MakeRequest();
                result = JsonHelper.DeserializeJsonToObject<DistributionSortingItemsByOrderIdReturn>(json);

            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        // just for test;
        public static SKUNoByEPCReturn GetSKUNoByEPC(SKUNoByEPCPara sKUNoByEPCPara)
        {
            SKUNoByEPCReturn result;
            try
            {
                var client = new RestClient();
                client.EndPoint = @"" + url_order + "/tagsprint/tags/getSKUNoByEPC.json"; ;
                client.ContentType = "application/json";
                client.Method = HttpVerb.POST;
                //SKUNoByEPCPara sKUNoByEPCPara = new SKUNoByEPCPara();
                //sKUNoByEPCPara.EpcList = new string[1];
                //sKUNoByEPCPara.EpcList[0] = "481000001002B010A00124AA";
                client.PostData = JsonHelper.SerializeObject(sKUNoByEPCPara.EpcList);
                var json = client.MakeRequest();
                result = JsonHelper.DeserializeJsonToObject<SKUNoByEPCReturn>(json);
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public static SKUNoByEPCAndStatusReturn GetSKUNoByEPCAndStatus(SKUNoByEPCAndStatusPara sKUNoByEPCAndStatusPara)
        {
            SKUNoByEPCAndStatusReturn result;
            try
            {
                var client = new RestClient();
                client.EndPoint = @"" + url_order + "/tagsprint/tags/getSKUNoByEPCAndStatus.json";
                client.ContentType = "application/json";
                client.Method = HttpVerb.POST;
                //SKUNoByEPCPara sKUNoByEPCPara = new SKUNoByEPCPara();
                //sKUNoByEPCPara.EpcList = new string[1];
                //sKUNoByEPCPara.EpcList[0] = "481000001002B010A00124AA";
                client.PostData = JsonHelper.SerializeObject(sKUNoByEPCAndStatusPara);
                var json = client.MakeRequest();
                result = JsonHelper.DeserializeJsonToObject<SKUNoByEPCAndStatusReturn>(json);
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }

        public static FindEPCReturn FindEPC(String epcStr)
        {
            FindEPCReturn result;
            try
            {
                var client = new RestClient();
                client.EndPoint = @"" + url_order + "/tagsprint/tags/findEPC.json?EPC="+epcStr;
                client.ContentType = "application/json";
                client.Method = HttpVerb.GET;
                //SKUNoByEPCPara sKUNoByEPCPara = new SKUNoByEPCPara();
                //sKUNoByEPCPara.EpcList = new string[1];
                //sKUNoByEPCPara.EpcList[0] = "481000001002B010A00124AA";
                //client.PostData = JsonHelper.SerializeObject(sKUNoByEPCAndStatusPara);
                var json = client.MakeRequest();
                result = JsonHelper.DeserializeJsonToObject<FindEPCReturn>(json);
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public static DistributionSortingReturn ConfirmDistributionSorting(DistributionSortingPara distributionSortingPara)
        {
            DistributionSortingReturn result;
            try
            {
                var client = new RestClient();
                client.EndPoint = @"" + url_system + "/sorting/confirmDistributionSorting.json"; ;
                client.ContentType = "application/json";
                client.Method = HttpVerb.POST;
                //DistributionSortingPara distributionSortingPara = new DistributionSortingPara();
                //distributionSortingPara.id = 264;
                //distributionSortingPara.itemList = new DistributionSortingPara.Itemlist[2];
                //distributionSortingPara.itemList[0] = new DistributionSortingPara.Itemlist();
                //distributionSortingPara.itemList[0].tradeOrderId = 85;
                //distributionSortingPara.itemList[0].epcList = new String[2];
                //distributionSortingPara.itemList[0].epcList[0] = "481000001002B010900131FC";
                //distributionSortingPara.itemList[0].epcList[1] = "481000001002B010A00125B1";
                //distributionSortingPara.itemList[1] = new DistributionSortingPara.Itemlist();
                //distributionSortingPara.itemList[1].tradeOrderId = 86;
                //distributionSortingPara.itemList[1].epcList = new String[2];
                //distributionSortingPara.itemList[1].epcList[0] = "481000001002B010900130F5";
                //distributionSortingPara.itemList[1].epcList[1] = "481000001002B010A00126B8";
                client.PostData = JsonHelper.SerializeObject(distributionSortingPara);// "[\"481000001002B010A00124AA\"]";
                var json = client.MakeRequest();
                result = JsonHelper.DeserializeJsonToObject<DistributionSortingReturn>(json);
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        //5, api;
        public static TradeOrdersInDistributionSortingReturn CancelTradeOrdersInDistributionSorting(TradeOrdersInDistributionSortingPara tradeOrdersInDistributionSortingPara)
        {
            TradeOrdersInDistributionSortingReturn result;
            try
            {
                var client = new RestClient();
                client.EndPoint = @"" + url_system + "/sorting/cancelTradeOrdersInDistributionSorting.json"; ;
                client.ContentType = "application/json";
                client.Method = HttpVerb.POST;
                client.PostData = JsonHelper.SerializeObject(tradeOrdersInDistributionSortingPara);// "[\"481000001002B010A00124AA\"]";
                var json = client.MakeRequest();
                result = JsonHelper.DeserializeJsonToObject<TradeOrdersInDistributionSortingReturn>(json);
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }


        // ref;
        //var client = new RestClient();

        //string endPoint = @"http://222.186.36.185:9105/sorting/getUnfinishedDistributionSortingList.json";
        //var client = new RestClient(endPoint);
        //var json = client.MakeRequest();
        //UnfinishedDistributionSortingList result = JsonHelper.DeserializeJsonToObject<UnfinishedDistributionSortingList>(json);

        //var client = new RestClient();
        //client.EndPoint = @"http://222.186.36.185:9105/sorting/getDistributionSortingItemsByOrderId.json"; ;
        //client.ContentType = "application/json";
        //DistributionSortingItemsByOrderIdPara distributionSortingItemsByOrderIdPara = new DistributionSortingItemsByOrderIdPara();
        //distributionSortingItemsByOrderIdPara.id = 264;
        //client.Method = HttpVerb.POST;
        //client.PostData = JsonHelper.SerializeObject(distributionSortingItemsByOrderIdPara);// "{\"id\":263}";
        //var json = client.MakeRequest();
        //DistributionSortingItemsByOrderIdReturn result = JsonHelper.DeserializeJsonToObject<DistributionSortingItemsByOrderIdReturn>(json);

        //var client = new RestClient();
        //client.EndPoint = @"http://222.186.36.185:9100/tagsprint/tags/getSKUNoByEPC.json"; ;
        //client.ContentType = "application/json";
        //client.Method = HttpVerb.POST;
        //client.PostData = "[\"481000001002B010A00124AA\"]";
        //var json = client.MakeRequest();
        //SKUNoByEPC result = JsonHelper.DeserializeJsonToObject<SKUNoByEPC>(json);

        //var client = new RestClient();
        //client.EndPoint = @"http://222.186.36.185:9100/tagsprint/tags/getSKUNoByEPC.json"; ;
        //client.ContentType = "application/json";
        //client.Method = HttpVerb.POST;
        //SKUNoByEPCPara sKUNoByEPCPara = new SKUNoByEPCPara();
        //sKUNoByEPCPara.EpcList = new string[1];
        //sKUNoByEPCPara.EpcList[0] = "481000001002B010A00124AA";
        //client.PostData = JsonHelper.SerializeObject(sKUNoByEPCPara.EpcList);// "[\"481000001002B010A00124AA\"]";
        //var json = client.MakeRequest();
        //SKUNoByEPC result = JsonHelper.DeserializeJsonToObject<SKUNoByEPC>(json);

        //try
        //{
        //    var client = new RestClient();
        //    client.EndPoint = @"http://222.186.36.185:9105/tagsprint/tags/getSKUNoByEPC.json"; ;
        //    client.ContentType = "application/json";
        //    client.Method = HttpVerb.POST;
        //    DistributionSortingPara distributionSortingPara = new DistributionSortingPara();
        //    distributionSortingPara.id = 264;
        //    distributionSortingPara.itemList = new DistributionSortingPara.Itemlist[2];
        //    distributionSortingPara.itemList[0] = new DistributionSortingPara.Itemlist();
        //    distributionSortingPara.itemList[0].tradeOrderId = 85;
        //    distributionSortingPara.itemList[0].epcList = new String[2];
        //    distributionSortingPara.itemList[0].epcList[0] = "481000001002B010900131FC";
        //    distributionSortingPara.itemList[0].epcList[1] = "481000001002B010A00125B1";
        //    distributionSortingPara.itemList[1] = new DistributionSortingPara.Itemlist();
        //    distributionSortingPara.itemList[1].tradeOrderId = 86;
        //    distributionSortingPara.itemList[1].epcList = new String[2];
        //    distributionSortingPara.itemList[1].epcList[0] = "481000001002B010900130F5";
        //    distributionSortingPara.itemList[1].epcList[1] = "481000001002B010A00126B8";
        //    client.PostData = JsonHelper.SerializeObject(distributionSortingPara);// "[\"481000001002B010A00124AA\"]";
        //    var json = client.MakeRequest();
        //    DistributionSortingReturn result = JsonHelper.DeserializeJsonToObject<DistributionSortingReturn>(json);

        //}
        //catch (Exception ex)
        //{
        //    ex.ToString();
        //}

    }
}

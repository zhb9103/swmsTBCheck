using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace swmsTBCheck
{
    class HttpSend
    {
        static CookieContainer cookie = new CookieContainer();

        static string path = System.Windows.Forms.Application.StartupPath + "\\config.ini";

        static string url_system = ConfigFile.ReadIniData("server", "url_system", "http://system.q-moo.com:9023/", path);
        static string url_order = ConfigFile.ReadIniData("server", "url_order", "http://order.q-moo.com:9105/", path);

        //for user login
        static public Boolean userLogin(String userName, String password)
        {
            string url = url_system + "system/users/login.json?loginName=" + userName + "&password=" + password;
            string responseContent = "";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "POST";

                request.CookieContainer = cookie;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //process the response message.
                using (Stream resStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(resStream, Encoding.UTF8))
                    {
                        responseContent = reader.ReadToEnd().ToString();
                    }
                }

                JSonResult result = JsonHelper.DeserializeJsonToObject<JSonResult>(responseContent);
                if (result.code == "1")
                {
                    CookieCollection cookieheader = request.CookieContainer.GetCookies(new Uri(url_system + "system/users/getCurrentUser.json"));
                    cookie.Add(cookieheader);
                    return true;
                }
            }
            catch (WebException we)
            {
                return false;
            }

            return false;
        }

        static Dictionary<string, int> orderList = new Dictionary<string, int>();
        // get the sorting order list.
        static public List<string> getOrderList()
        {
            string url = url_order + "sorting/getUnfinishedDistributionSortingList.json";
            string responseContent = "";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "POST";

                request.CookieContainer = cookie;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //process the response message
                using (Stream resStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(resStream, Encoding.UTF8))
                    {
                        responseContent = reader.ReadToEnd().ToString();
                    }
                }
            }
            catch (WebException we)
            {
                return null;
            }

            //parse the data from server.
            JSonR result = JsonHelper.DeserializeJsonToObject<JSonR>(responseContent);
            if (result.code == "1")
            {
                List<string> temp = new List<string>();
                OrderItem[] items = (OrderItem[])result.data;

                foreach (OrderItem item in items)
                {
                    temp.Add(item.sortingOrderNo);
                    if (!orderList.ContainsKey(item.sortingOrderNo))
                        orderList.Add(item.sortingOrderNo, item.id);
                }
                return temp;
            }
            else
            {
                return null;
            }
        }

        //Get the Taobao order number based on epc and sorting number.
        static public int getItemNumber(string order, string epc, out bool isItemDone)
        {
            string url = url_order + "sorting/getDistributionSortingItemsByOrderId.json";
            string responseContent = "";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "POST";

                request.CookieContainer = cookie;

                //input the parameters in body.
                InputPara input = new InputPara();
                int orderNo;
                orderList.TryGetValue(order, out orderNo);
                input.sortingOrderId = orderNo;
                input.epc = order;

                //serialize the object to json string.
                string body = JsonHelper.SerializeObject(input);

                byte[] btBodys = Encoding.UTF8.GetBytes(body);
                request.ContentLength = btBodys.Length;
                request.GetRequestStream().Write(btBodys, 0, btBodys.Length);


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //Process the response
                using (Stream resStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(resStream, Encoding.UTF8))
                    {
                        responseContent = reader.ReadToEnd().ToString();
                    }
                }
            }
            catch (WebException ex)
            {
                isItemDone = false;
                return -1;
            }

            JSonResult result = JsonHelper.DeserializeJsonToObject<JSonResult>(responseContent);
            if (result.code == "1" && result.data != null)
            {
                ResultSoring data = (ResultSoring)result.data;
                isItemDone = data.finishFlag;
                return data.serialNum;
            }

            isItemDone = false;
            return -1;
        }

        //The http json result
        class JSonResult
        {
            public string msg { get; set; }

            public string code { get; set; }

            public Object data { get; set; }
        }

        class JSonR
        {
            public string msg { get; set; }
            public string code { get; set; }
            public OrderItem[] data { get; set; }
        }

        class OrderItem
        {
            public int id { get; set; }
            public string sortingOrderNo { get; set; }
        }

        class InputPara
        {
            public int sortingOrderId { get; set; }
            public string epc { get; set; }
        }

        class ResultSoring
        {
            public bool allFinishFlag { get; set; }
            public bool finishFlag { get; set; }
            public int serialNum { get; set; }
            public int orderCount { get; set; }
        }
    }
}

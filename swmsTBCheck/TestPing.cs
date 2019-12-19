using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;




namespace swmsTBCheck
{
    public class TestPing
    {
        /// <summary>
        /// ping ip,测试能否ping通
        /// </summary>
        /// <param name="strIP">IP地址</param>
        /// <returns></returns>
        /// 
        private static readonly int MAX_TIMEOUT_MS = 5000;
        public static bool PingIp(string strIP)
        {
            bool bRet = false;
            try
            {
                Ping pingSend = new Ping();
                PingReply reply = pingSend.Send(strIP, MAX_TIMEOUT_MS);
                if (reply.Status == IPStatus.Success)
                {
                    bRet = true;
                }
                pingSend.Dispose();
            }
            catch (Exception)
            {
                bRet = false;
            }
            return bRet;
        }
    }
}

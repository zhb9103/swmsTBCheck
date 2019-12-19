using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Management;
using System.IO.Ports;



namespace swmsTBCheck
{
    class SystemPowerState
    {
        /// <summary>
        /// 检查系统电源状态
        /// 该状态指示系统是否运行在交流电或直流电源，是否电池正处流电状态，电池还有多少寿命 
        /// </summary>
        /// <param name="lpSystemPowerStatus"></param>
        [DllImport("kernel32", EntryPoint = "GetSystemPowerStatus")]
        private static extern void GetSystemPowerStatus(ref SYSTEM_POWER_STATUS lpSystemPowerStatus);

        private struct SYSTEM_POWER_STATUS
        {
            public Byte ACLineStatus;                //0 = offline,  1 = Online, 255 = UnKnown Status.   
            public Byte BatteryFlag;
            public Byte BatteryLifePercent;
            public Byte Reserved1;
            public int BatteryLifeTime;
            public int BatteryFullLifeTime;
        }

        /// <summary>
        /// 读取交流电源线的连接状态
        /// </summary>
        /// <returns></returns>
        public static string ReadAlternatingCurrentConnectState()
        {
            string AlternatingCurrentConnectState = ""; //交流供电情况
            //申明一个结构对象，通过引用将值传到这个对象 
            SYSTEM_POWER_STATUS SysPower = new SYSTEM_POWER_STATUS();
            GetSystemPowerStatus(ref SysPower);
            if (SysPower.ACLineStatus == 1)
            {//处于在线状态
                AlternatingCurrentConnectState = "Online";
            }
            else if (SysPower.ACLineStatus == 0)
            {//处于离线状态
                AlternatingCurrentConnectState = "Offline";
            }
            else
            {//未知状态
                AlternatingCurrentConnectState = "UnKnown";
            }
            return AlternatingCurrentConnectState;
        }

         

    /// <summary>
            /// 读取电池状态信息
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
    public static Int32 ReadBatteryInfo()
        {
            Int32 RemainElectricQuantity=0;
            try
            {
                ManagementClass mc = new ManagementClass("Win32_Battery");
                ManagementObjectCollection moc = mc.GetInstances();
                ManagementObjectCollection.ManagementObjectEnumerator mom = moc.GetEnumerator();
                if (mom.MoveNext())
                {
                    //读取当前剩余的电量
                    RemainElectricQuantity = Convert.ToInt32(mom.Current.Properties["EstimatedChargeRemaining"].Value);
                    //this.label3.Text = RemainElectricQuantity.ToString();
                    ////读取当前设备ID编号
                    //this.label4.Text = Convert.ToString(mom.Current.Properties["DeviceID"].Value);
                    ////读取当前电池的名称
                    //this.label5.Text = Convert.ToString(mom.Current.Properties["Name"].Value);
                    //label6.Text = Convert.ToString(mom.Current.Properties["Status"].Value);
                }
                else
                {
                    //当不能正确地获得所需要的值时
                    //this.label3.Text = "Can't Get the value";
                }
            }
            catch (Exception)
            {//写入文件异常，这个地方一般是由于使用了非超级管理员权限向C盘写入了文件，报告权限不够
                //if (MessageBox.Show("Run me in the role of the super administrator!", "Information Tip:", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                //{
                //    this.Close();
                //}
            }

            //从配置文件里面读出电池电量上限值
            //String BatteryMaxVal = (ReadConfigureInfo("$BatteryMaxVal$")).Replace("%", "");
            ////从配置文件里面读取电池电量下限值
            //String BatteryMinVal = (ReadConfigureInfo("$BatteryMinVal$")).Replace("%", "");

            //if ((BatteryMaxVal.Trim() == "") || (BatteryMinVal.Trim() == ""))
            //{//如果上限值或下限值中有一个为空，那么就是配置文件中没有正确地设置配置文件
            //    //MessageBox.Show("Error in configuration file,please modify Upper and lower voltage limits!", "Information Tip:", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            //}
            //if (RemainElectricQuantity <= Convert.ToInt32(BatteryMaxVal) && RemainElectricQuantity >= Convert.ToInt32(BatteryMinVal))
            //{//当电量处于40%至80%之间的时候，满足出货的要求
            //    //this.label1.Text = "Pass";
            //}
            //else
            //{//不满足出货要求
            //    //this.label1.Text = "Fail";
            //}
            return RemainElectricQuantity;
        }
    }
}

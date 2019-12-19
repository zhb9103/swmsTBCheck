using ReaderB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.IO.Ports;
using Tcp517T_Api;
using bt_demo;
using InTheHand.Net;
using InTheHand.Net.Sockets;

//using AForge;
//using AForge.Video;
//using AForge.Video.DirectShow;
using SpeechLib;
using MR6100Api;
using Uwe1NetApi;



// bill.zhang test git;

/**
 * The smart warehouse management system for TaoBao order sorting.
 * */
namespace swmsTBCheck
{
    public partial class MainForm : Form
    {
        Login Login_MainForm;

        //private FilterInfoCollection videoDevices;
        SpVoice voice;
        MR6100Api.MR6100Api mR6100Api = new MR6100Api.MR6100Api();
        String mr6100_ip_address = "";

        Uwe1NetApi.TcpClient tcpClientTester;// = new Uwe1NetApi.TcpClient(true);

        string path = "";// = System.Windows.Forms.Application.StartupPath + "\\config.ini";

        Boolean ioControlEnable = false;
        string ioControlPort = "";// = ConfigFile.ReadIniData("io_control", "port", "", path);
        //ip_address=192.168.1.75
        //tcp_port=502
        string ioControlIp_address = "";
        int ioControlTcp_port = 0;

        // uhf_rfid
        int rfid_decide_timeout = 0;
        int rfid_method = 0;
        int rfid_read_tag_times_ref = 0;
        String rfid_bt_name = "";//bt_name=123456;
        String uwe1_ip_address = "";
        String uwe1_bt_name = "";
        String uwe1_comport = "";
        String uwe1_modempara = "";

        // debug;
        Boolean debug_enable = false;

        // filter;
        Boolean filter_enable = false;
        String filter_data = "";


        private int fCmdRet = 30; //所有执行指令的返回值
        private byte fComAdr = 0xff; //当前操作的ComAdr
        private int frmcomportindex;
        private bool isConnected = false;

        int currentPanelWidth = 0;
        int currentPanelHeight = 0;
        int window_width_offset = 0;
        int window_height_offset = 0;
        Boolean window_maxim = false;

        Thread ThreadShowRealTime;
        Boolean ThreadShowRealTimeStyle = false;
        Thread ThreadLedMonitor;
        Boolean ThreadLedMonitorStyle = false;

        Thread ThreadNetworkMonitor;
        Boolean ThreadNetworkMonitorStyle = false;
        Boolean NetworkState = false;

        bool IoControlSerialPortState = false;
        int IoControlSerialPortReadTimeOutCounter = 0;
        byte[] IoControlSerialPortBuffer = new byte[512];
        int IoControlSerialPortBuffer_Counter = 0;

        Dictionary<string, int> EPCDictionary = new Dictionary<string, int>();

        Dictionary<string, int> SKU_Dictionary = new Dictionary<string, int>();

        Dictionary<string, int> ReadEpcsDictionary = new Dictionary<string, int>();
        Boolean ReadEpcsDictionaryLock = false;

        private string configPath = Application.StartupPath + "\\config.ini";


        UnfinishedDistributionSortingListReturn current_UnfinishedDistributionSortingListReturn;// = DistributionPlatform.GetUnfinishedDistributionSortingList();
        DistributionSortingItemsByOrderIdReturn current_DistributionSortingItemsByOrderIdReturn;

        CurrentOrderInfo currentOrderInfo;
        OrderItem[] orderItems = new OrderItem[24];
        int orderItems_Counter = 0;

        // 单件总数；
        int TotalEpcCount = 0;
        int TotalEpcCount_i = 0;

        private int FinishedOrderCounter = 0;


        Image image_ic_light_red = Image.FromFile("./img/led_red.gif");
        Image image_ic_light_green = Image.FromFile("./img/led_green.gif");
        Image image_ic_light_yellow = Image.FromFile("./img/led_yellow.gif");
        Image image_ic_light_gray = Image.FromFile("./img/led_gray.gif");
        Image image_ic_light_blue = Image.FromFile("./img/led_blue.gif");

        Image image_ic_logo_pos = Image.FromFile("./img/logo_icon_pos.gif");
        Image image_ic_logo_neg = Image.FromFile("./img/logo_icon_neg.gif");

        private void orderItemsInit()
        {
            for (int orderItems_i=0;orderItems_i< orderItems.Length; orderItems_i++)
            {
                orderItems[orderItems_i] = new OrderItem();
                orderItems[orderItems_i].isChecked = false;
                orderItems[orderItems_i].isCanceled = false;
                orderItems[orderItems_i].id = 0 ;
                orderItems[orderItems_i].isFull = false;
                orderItems[orderItems_i].position = 0;
                orderItems[orderItems_i].tradeOrderNo = "";
                orderItems[orderItems_i].tradeOrderItem = null;
            }
            orderItems_Counter = 0;
            FinishedOrderCounter = 0;
            //lastLedNumber = 0;
            LedNumber = 0;
        }

        YTJBluetooth yTJBluetooth = new YTJBluetooth();
        Dictionary<string, BluetoothAddress> dicBluetooth;
        BluetoothAddress MyBluetoothAddress;
        Boolean isConnecting=false;

        Thread ThreadConnectBtRfidReader;
        Boolean ThreadConnectBtRfidReaderStyle = false;
        //Boolean ThreadConnectBtRfidReaderSytle=false;
        Boolean BtRfidIsConn = false;
        Boolean BtRfidIsFind = false;
        Boolean isReconnection = false;
        Boolean isBtConnected = false;
        //Boolean DispatchReadyStyle = false;

        Thread ThreadBtReaderMonitor;
        Boolean ThreadBtReaderMonitorStyle = false;


        public MainForm(Login login)
        {
            InitializeComponent();
            Login_MainForm = login;


            
        }

        private void ConnectBtRfidReaderFunc()
        {
            
            //ThreadConnectBtRfidReaderSytle = true;
            int connectBtRfidReaderTimeoutCounter = 0;
            ThreadConnectBtRfidReaderStyle = true;
            this.Invoke((EventHandler)(delegate
            {
                toolStripStatusLabel_status.Text = "寻找设备";
            }));
            while (true)
            {
                yTJBluetooth.findBluetooth();
                Thread.Sleep(3000);
                //if(connectBtRfidReaderTimeoutCounter++>4)
                //{
                   
                //    LogFile.WriteLog("RFID读卡器未发现设备。");
                //    return;
                //}
                
                if(BtRfidIsFind)
                {
                    break;
                }
            }
            connectBtRfidReaderTimeoutCounter = 0;
            this.Invoke((EventHandler)(delegate
            {
                toolStripStatusLabel_status.Text = "连接中";
            }));
            while (true)
            {
                yTJBluetooth.connectBluetooth(dicBluetooth[rfid_bt_name]);// "ET10_SPPF5289E"
                
                //if (connectBtRfidReaderTimeoutCounter++ > 4)
                //{
                //    toolStripStatusLabel_status.Text = "连接失败";
                //    LogFile.WriteLog("RFID读卡器连接失败。");
                //    return;
                //}
                if (BtRfidIsConn)
                {
                    isBtConnected = true;
                    break;
                }
                Thread.Sleep(1000);
            }
            //yTJBluetooth.offBluetooth();

            //int[] powers = yTJBluetooth.getPower();

            //bool isOk = yTJBluetooth.setPower(powers);
            //if (isOk)
            //{
            //    MessageBox.Show("功率设置成功！");
            //}
            //else
            //{
            //    MessageBox.Show("功率设置失败！");
            //}
            Thread.Sleep(100);
           
            //yTJBluetooth.StartRead();
            //Thread.Sleep(2000);
            //yTJBluetooth.StopRead();

            this.Invoke((EventHandler)(delegate
            {
                toolStripStatusLabel_status.Text = "连接正常";
                LogFile.WriteLog("RFID读卡器连接正常。");
                toolStripStatusLabel_status.ForeColor = Color.Black;
                comboBox_order.Enabled = true;
                
                Application.DoEvents();
                ThreadBtReaderMonitor = new Thread(ThreadBtReaderMonitorFunc);
                ThreadBtReaderMonitor.IsBackground = true;
                ThreadBtReaderMonitor.Start();
                voice.Speak("读卡器连接成功。");
            }));
            ThreadConnectBtRfidReaderStyle = false;
        }

        int findBtCounter = 0;
        private void FindBluetooth(BluetoothDeviceInfo[] devices)
        {
            if (this.IsHandleCreated)
            {
                //委托主线程更新UI
                this.Invoke((EventHandler)(delegate
                {
                    //BtFind.Enabled = true;
                    //if (null == devices)
                    //{
                    //    return;
                    //}
                    dicBluetooth = new Dictionary<string, BluetoothAddress>();
                    foreach (BluetoothDeviceInfo d in devices)
                    {
                        //MyBluetoothAddress = ;
                        //dicBluetooth[d.DeviceName] = MyBluetoothAddress;
                        //if(d.DeviceAddress.Equals(dicBluetooth[rfid_bt_name]))
                        if (d.DeviceName.Equals(rfid_bt_name))
                        {
                            MyBluetoothAddress = d.DeviceAddress;
                            dicBluetooth[d.DeviceName] = MyBluetoothAddress;
                            textBoxShowDebug.AppendText(d.DeviceName+"\r\n");
                           
                            BtRfidIsFind = true;
                        }
                        //lsbDevices.Items.Add("搜索到蓝牙设备：" + "  名称：" + d.DeviceName + "   地址：" + d.DeviceAddress);
                        //lsbDevices.Items.Add(d.DeviceName);
                        
                        //BtRfidIsFind = true;
                    }
                }));
            }
        } //查找结果回调


        private void ConnBluetooth(bool isConn)
        {
            //BtConn.Enabled = true;
            if (isConn)
            {
                //BtFind.Enabled = false;
                //BtConn.Enabled = false;
                //MessageBox.Show("连接成功！");
                BtRfidIsConn = true;
                Thread.Sleep(200);
                if (isReconnection == true)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (yTJBluetooth.StartRead())
                        {
                            this.Invoke((EventHandler)(delegate
                            {
                                toolStripStatusLabel_status.Text = "连接正常";
                                toolStripStatusLabel_status.ForeColor = Color.Black;
                                textBoxShowDebug.AppendText("开始读卡。");
                            }));
                            break;
                        }
                    }
                }
            }
            else
            {
                //MessageBox.Show("连接失败！");
                BtRfidIsConn = false;
            }
            isConnecting = false;

        } //连接结果回调

        //扫描监听回调
        String BtRfidReader_TempEpc = "";
        int BtRfidReader_TempEpcTimeout = 0;
        Boolean BtRfidReader_TempEpcTimeoutEnable = false;
        readonly int TEMPEPCTIMEOUT_MAX = 10;//3 seconds;

        int monitorCounter = 0;
        private void ReadTag(string epc)
        {
            this.Invoke((EventHandler)(delegate
            {
                //this.label_Monitor.Text = (monitorCounter++).ToString();
                //textBoxShowDebug.AppendText(epc + "," + epc.Length + "\r\n");
                if (debug_enable)
                {
                    textBoxShowDebug.AppendText(epc + "," + epc.Length + "\r\n");
                }

                //if ((BtRfidReader_TempEpc.Length==0)&&(epc.Length>=24))
                //{
                //    textBoxShowDebug.AppendText("ok:"+epc + ","+ epc.Length +"\r\n");
                //    BtRfidReader_TempEpcTimeout = 0;
                //    BtRfidReader_TempEpcTimeoutEnable = true;
                //    BtRfidReader_TempEpc = epc;
                //    Application.DoEvents();
                //    RfidActionProcess(epc);
                //    //BtRfidReader_TempEpc = "";
                //    textBoxShowDebug.AppendText("finish:" + epc + "," + epc.Length + "\r\n");
                //}
                
                if(ReadEpcsDictionaryLock)
                {
                    return;
                }
                if (epc.Length == 24)
                {
                    // add epcs to the map;
                    //ReadEpcsDictionaryTimer = 0;
                    try
                    {
                        if (!ReadEpcsDictionary.ContainsKey(epc))
                        {
                            // if epc information is not in the dictionary, add a new epc information to the dictionary;
                            ReadEpcsDictionary.Add(epc, 1);
                        }
                        else
                        {
                            // if epc information is in the dictionary, add the counter for the epc informaiton;
                            ReadEpcsDictionary[epc] = ReadEpcsDictionary[epc] + 1;
                        }
                    }
                    catch
                    {
                        // exception;
                    }
                    
                }
                
            }));

            
        }

        Thread ThreadReadEpcsDictionaryProcess;
        Boolean ThreadReadEpcsDictionaryProcessStyle = false;
        
        int ReadEpcsDictionaryTimer = 0;

        void ThreadReadEpcsDictionaryProcessFunc()
        {
            
            while (true)
            {
                Thread.Sleep(100);
                try
                {
                    if (ReadEpcsDictionary.Count > 0)
                    {
                        if (ReadEpcsDictionaryTimer++ > rfid_decide_timeout)
                        {
                            ReadEpcsDictionaryTimer = 0;
                            ReadEpcsDictionaryLock = true;
                            if (ReadEpcsDictionary.Count == 1)
                            {
                                this.Invoke((EventHandler)(delegate
                                {
                                    foreach (String epc in ReadEpcsDictionary.Keys)
                                    {
                                        if (ReadEpcsDictionary[epc] > rfid_read_tag_times_ref)
                                        {
                                            RfidActionProcess(epc);
                                        }
                                    }
                                    //RfidActionProcess("111111111111111111111111");
                                    //ReadEpcsDictionary.Clear();
                                }));
                            }
                            else
                            {
                                this.Invoke((EventHandler)(delegate
                                {
                                    label_current.Text = "";
                                    MainShowMessage("一次分波商品数量过多：" + ReadEpcsDictionary.Count.ToString(), 1);
                                }));
                            }
                            Thread.Sleep(1500);
                            ReadEpcsDictionaryLock = false;
                            ReadEpcsDictionary.Clear();
                        }
                    }
                }
                catch(Exception ex)
                {
                    LogFile.WriteLog("epc processing exception.");
                }
                
            }
            
            
            //Thread.Sleep(3000);
            
            //Thread.Sleep(3000);
            // clear dictionary;
            //ReadEpcsDictionary.Clear();
            //ThreadReadEpcsDictionaryProcessStyle = false;
        }


        BatteryProcessBar batteryProcessBar = new BatteryProcessBar();

        Tcp517T_Api.TcpClient tcpClient;
        int tcpClientTimeout = 0;
        private void FormLoad(object sender, EventArgs e)
        {
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            Login_MainForm.Visible = false;
            this.WindowState = FormWindowState.Maximized;

            toolStripProgressBarBatteryGauge.Maximum = 100;
            toolStripProgressBarBatteryGauge.Minimum = 0;

            voice = new SpVoice();
            voice.Rate = 0; //语速,[-10,10]
            voice.Volume = 100; //音量,[0,100]
            voice.Voice = voice.GetVoices().Item(0); //语音库
            //voice.Speak("欢迎使用。");
            ThreadVoicePlay = new Thread(ThreadVoicePlayFunc);
            ThreadVoicePlay.IsBackground = true;
            ThreadVoicePlay.Start(" 欢迎使用。");

            //buttonTest.Enabled = false;
            button_confirm.Visible = false;
            button_confirm.Enabled = false;
            buttonCancelTradeOrdersInDistributionSorting.Visible = true;
            buttonCancelTradeOrdersInDistributionSorting.Enabled = false;

            button_1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_11.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_12.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_13.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_14.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_15.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_16.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_17.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_18.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_19.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_20.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_21.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_22.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_23.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            button_24.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            button_1.FlatStyle = FlatStyle.Flat;
            button_1.FlatAppearance.BorderSize = 0;
            button_2.FlatStyle = FlatStyle.Flat;
            button_2.FlatAppearance.BorderSize = 0;
            button_3.FlatStyle = FlatStyle.Flat;
            button_3.FlatAppearance.BorderSize = 0;
            button_4.FlatStyle = FlatStyle.Flat;
            button_4.FlatAppearance.BorderSize = 0;
            button_5.FlatStyle = FlatStyle.Flat;
            button_5.FlatAppearance.BorderSize = 0;
            button_6.FlatStyle = FlatStyle.Flat;
            button_6.FlatAppearance.BorderSize = 0;
            button_7.FlatStyle = FlatStyle.Flat;
            button_7.FlatAppearance.BorderSize = 0;
            button_8.FlatStyle = FlatStyle.Flat;
            button_8.FlatAppearance.BorderSize = 0;
            button_9.FlatStyle = FlatStyle.Flat;
            button_9.FlatAppearance.BorderSize = 0;
            button_10.FlatStyle = FlatStyle.Flat;
            button_10.FlatAppearance.BorderSize = 0;
            button_11.FlatStyle = FlatStyle.Flat;
            button_11.FlatAppearance.BorderSize = 0;
            button_12.FlatStyle = FlatStyle.Flat;
            button_12.FlatAppearance.BorderSize = 0;
            button_13.FlatStyle = FlatStyle.Flat;
            button_13.FlatAppearance.BorderSize = 0;
            button_14.FlatStyle = FlatStyle.Flat;
            button_14.FlatAppearance.BorderSize = 0;
            button_15.FlatStyle = FlatStyle.Flat;
            button_15.FlatAppearance.BorderSize = 0;
            button_16.FlatStyle = FlatStyle.Flat;
            button_16.FlatAppearance.BorderSize = 0;
            button_17.FlatStyle = FlatStyle.Flat;
            button_17.FlatAppearance.BorderSize = 0;
            button_18.FlatStyle = FlatStyle.Flat;
            button_18.FlatAppearance.BorderSize = 0;
            button_19.FlatStyle = FlatStyle.Flat;
            button_19.FlatAppearance.BorderSize = 0;
            button_20.FlatStyle = FlatStyle.Flat;
            button_20.FlatAppearance.BorderSize = 0;
            button_21.FlatStyle = FlatStyle.Flat;
            button_21.FlatAppearance.BorderSize = 0;
            button_22.FlatStyle = FlatStyle.Flat;
            button_22.FlatAppearance.BorderSize = 0;
            button_23.FlatStyle = FlatStyle.Flat;
            button_23.FlatAppearance.BorderSize = 0;
            button_24.FlatStyle = FlatStyle.Flat;
            button_24.FlatAppearance.BorderSize = 0;


            //button_1.BackgroundImage = image_ic_light_gray;
            //button_2.BackgroundImage = image_ic_light_gray;
            //button_3.BackgroundImage = image_ic_light_gray;
            //button_4.BackgroundImage = image_ic_light_gray;
            //button_5.BackgroundImage = image_ic_light_gray;
            //button_6.BackgroundImage = image_ic_light_gray;
            //button_7.BackgroundImage = image_ic_light_gray;
            //button_8.BackgroundImage = image_ic_light_gray;
            //button_9.BackgroundImage = image_ic_light_gray;
            //button_10.BackgroundImage = image_ic_light_gray;
            //button_11.BackgroundImage = image_ic_light_gray;
            //button_12.BackgroundImage = image_ic_light_gray;
            //button_13.BackgroundImage = image_ic_light_gray;
            //button_14.BackgroundImage = image_ic_light_gray;
            //button_15.BackgroundImage = image_ic_light_gray;
            //button_16.BackgroundImage = image_ic_light_gray;
            //button_17.BackgroundImage = image_ic_light_gray;
            //button_18.BackgroundImage = image_ic_light_gray;
            //button_19.BackgroundImage = image_ic_light_gray;
            //button_20.BackgroundImage = image_ic_light_gray;
            //button_21.BackgroundImage = image_ic_light_gray;
            //button_22.BackgroundImage = image_ic_light_gray;
            //button_23.BackgroundImage = image_ic_light_gray;
            //button_24.BackgroundImage = image_ic_light_gray;

            //labelBox_1.Parent = button_1;
            // labelBox_1.Controls.Add(button_1);
            //labelBox_1.BackColor= Color.Transparent;
            


            //labelShowMessage.Text = "";
            MainShowMessage("",0);
            label_totalnum.Text = "0";
            //comboBox_order.Enabled = false;
            comboBox_order.IntegralHeight = false;
            comboBox_order.MaxDropDownItems = 10;

            pictureBoxLogo.BackgroundImage = image_ic_logo_pos;



            batteryProcessBar.Parent = toolStripProgressBarBatteryGauge.Control;
            batteryProcessBar.Minimum = 0;//进度条显示最小值
            batteryProcessBar.Maximum = 100;//进度条显示最大值
            batteryProcessBar.Width = toolStripProgressBarBatteryGauge.Width;
            batteryProcessBar.Height = toolStripProgressBarBatteryGauge.Height;
            batteryProcessBar.ForeColor = Color.Green;


            path = System.Windows.Forms.Application.StartupPath + "\\config.ini";
            //ioControlPort = ConfigFile.ReadIniData("io_control", "port", "", path);
            StringBuilder stringBuilder = new StringBuilder(100);


            // debug;
            ConfigFile.GetPrivateProfileString("debug", "enable", "0", stringBuilder, 100, path);
            String tempDebug = stringBuilder.ToString();
            try
            {
                int tempValue = int.Parse(tempDebug);
                if (tempValue != 0)
                {
                    debug_enable = true;
                }
                else
                {
                    debug_enable = false;
                }
            }
            catch
            {
                debug_enable = false;
            }


            // filter;
            ConfigFile.GetPrivateProfileString("filter", "enable", "0", stringBuilder, 100, path);
            String tempFilter = stringBuilder.ToString();
            try
            {
                int tempValue = int.Parse(tempFilter);
                if (tempValue != 0)
                {
                    filter_enable = true;
                }
                else
                {
                    filter_enable = false;
                }
            }
            catch
            {
                filter_enable = false;
            }

            ConfigFile.GetPrivateProfileString("filter", "data", "F8J", stringBuilder, 100, path);
            filter_data = stringBuilder.ToString();



            // io control;
            ConfigFile.GetPrivateProfileString("io_control", "enable", "0", stringBuilder, 100, path);
            String tempIoControlEnable = stringBuilder.ToString();
            try
            {
                int tempValue = int.Parse(tempIoControlEnable);
                if(tempValue!=0)
                {
                    ioControlEnable = true;
                }
                else
                {
                    ioControlEnable = false;
                }
            }
            catch
            {
                ioControlEnable = false;
            }

            ConfigFile.GetPrivateProfileString("io_control", "port", "COM1", stringBuilder, 100, path);
            ioControlPort = stringBuilder.ToString();

            //ip_address tcp_port;
            ConfigFile.GetPrivateProfileString("io_control", "ip_address", "192.168.1.75", stringBuilder, 100, path);
            ioControlIp_address = stringBuilder.ToString();

            try
            {
                ConfigFile.GetPrivateProfileString("io_control", "tcp_port", "502", stringBuilder, 100, path);
                ioControlTcp_port = int.Parse(stringBuilder.ToString());
            }
            catch
            {
                ioControlTcp_port = 502;
            }

           
            

            ConfigFile.GetPrivateProfileString("server", "ip_address", "222.186.36.185", stringBuilder, 100, path);
            DistributionPlatform.ip_address = stringBuilder.ToString();

            ConfigFile.GetPrivateProfileString("server", "url_system", "http://222.186.36.185:9105", stringBuilder, 100, path);
            DistributionPlatform.url_system = stringBuilder.ToString();

            ConfigFile.GetPrivateProfileString("server", "url_order", "http://222.186.36.185:9100", stringBuilder, 100, path);
            DistributionPlatform.url_order = stringBuilder.ToString();


            // uhf_rfid;
            //uwe1_ip_address = 192.168.1.100
            //uwe1_bt_name = 123456
            //uwe1_comport = COM1
            ConfigFile.GetPrivateProfileString("uhf_rfid", "uwe1_bt_name", "123456", stringBuilder, 100, path);
            uwe1_bt_name = stringBuilder.ToString();

            ConfigFile.GetPrivateProfileString("uhf_rfid", "uwe1_ip_address", "192.168.1.100", stringBuilder, 100, path);
            uwe1_ip_address = stringBuilder.ToString();

            ConfigFile.GetPrivateProfileString("uhf_rfid", "uwe1_comport", "COM1", stringBuilder, 100, path);
            uwe1_comport = stringBuilder.ToString();

            ConfigFile.GetPrivateProfileString("uhf_rfid", "uwe1_modempara", "04050900", stringBuilder, 100, path);
            uwe1_modempara = stringBuilder.ToString();
            

            try
            {
                ConfigFile.GetPrivateProfileString("uhf_rfid", "rfid_decide_timeout", "30", stringBuilder, 100, path);
                rfid_decide_timeout = int.Parse(stringBuilder.ToString());
            }
            catch
            {
                rfid_decide_timeout = 30;
            }

            try
            {
                ConfigFile.GetPrivateProfileString("uhf_rfid", "rfid_method", "0", stringBuilder, 100, path);
                rfid_method = int.Parse(stringBuilder.ToString());
            }
            catch
            {
                rfid_method = 0;
            }

            try
            {
                ConfigFile.GetPrivateProfileString("uhf_rfid", "rfid_read_tag_times_ref", "0", stringBuilder, 100, path);
                rfid_read_tag_times_ref = int.Parse(stringBuilder.ToString());
            }
            catch
            {
                rfid_read_tag_times_ref = 0;
            }
            

            ConfigFile.GetPrivateProfileString("uhf_rfid", "rfid_bt_name", "123456", stringBuilder, 100, path);
            rfid_bt_name = stringBuilder.ToString();

            ConfigFile.GetPrivateProfileString("uhf_rfid", "mr6100_ip_address", "10.0.0.5", stringBuilder, 100, path);
            mr6100_ip_address = stringBuilder.ToString();
            


            try
            {
                ConfigFile.GetPrivateProfileString("window", "width_offset", "0", stringBuilder, 100, path);
                window_width_offset = int.Parse(stringBuilder.ToString());
            }
            catch
            {
                window_width_offset = 0;
            }

            try
            {
                ConfigFile.GetPrivateProfileString("window", "height_offset", "0", stringBuilder, 100, path);
                window_height_offset = int.Parse(stringBuilder.ToString());
            }
            catch
            {
                window_height_offset = 0;
            }

            try
            {
                ConfigFile.GetPrivateProfileString("window", "maxim", "0", stringBuilder, 100, path);
                int tempMaxim = int.Parse(stringBuilder.ToString());
                if (tempMaxim > 0)
                {
                    window_maxim = true;
                }
                else
                {
                    window_maxim = false;
                }
            }
            catch
            {
                window_maxim = false;
            }

            currentPanelWidth = this.panelTBCheck.Width;
            currentPanelHeight = this.panelTBCheck.Height;
            int currentPanel_X = Configuration.iActulaWidth / 2 - currentPanelWidth / 2;
            int currentPanel_Y = Configuration.iActulaHeight / 2 - currentPanelHeight / 2;
            this.panelTBCheck.Location = new System.Drawing.Point(currentPanel_X-window_width_offset, currentPanel_Y-window_height_offset);

            if(window_maxim)
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            }
            else
            {

            }
            

            this.textBoxLoginUserName.Text = Configuration.userName;

            if(debug_enable)
            {
                textBoxShowDebug.Visible = true;
                comboBox_order.Enabled = true;
            }
            else
            {
                textBoxShowDebug.Visible = false;
                comboBox_order.Enabled = false;
            }


            // connect to Reader. Enable the reader timer if the connection is OK.
            switch(rfid_method)
            {
                case 1:// comport;
                    {
                        if (connectToReader())
                        {
                            timer_reader.Enabled = true;
                            timer_checkEPC.Enabled = true;
                            isConnected = true;
                        }
                        break;
                    }
                case 2:// bluetooth;
                    {
                        yTJBluetooth.InventoryTag += ReadTag;
                        yTJBluetooth.bluetooth += FindBluetooth;
                        yTJBluetooth.connBluetooth += ConnBluetooth;

                        ThreadConnectBtRfidReader = new Thread(ConnectBtRfidReaderFunc);
                        ThreadConnectBtRfidReader.IsBackground = true;
                        ThreadConnectBtRfidReader.Start();
                        break;
                    }
                case 3:// network;
                    {
                        int status = 1;
                        byte v1 = 0;
                        byte v2 = 0;
                        if (!mR6100Api.isNetWorkConnect(mr6100_ip_address))
                        {
                            //libInfo.Items.Add("The input ip is not exist.");
                            toolStripStatusLabel_status.Text = "连接失败";
                            LogFile.WriteLog("RFID读卡器连接失败。");
                            break;
                        }
                        status = mR6100Api.TcpConnectReader(mr6100_ip_address, 100);
                        if (status != MR6100Api.MR6100Api.SUCCESS_RETURN)
                        {
                            //libInfo.Items.Add("Connect Reader Failed!  ");
                            toolStripStatusLabel_status.Text = "连接失败";
                            LogFile.WriteLog("RFID读卡器连接失败。");
                            break;
                        }
                        status = mR6100Api.GetFirmwareVersion(255, ref v1, ref v2);
                        if (status != MR6100Api.MR6100Api.SUCCESS_RETURN)
                        {
                            //libInfo.Items.Add("Can not connect with the reader!  ");
                            toolStripStatusLabel_status.Text = "连接失败";
                            LogFile.WriteLog("RFID读卡器连接失败。");
                          
                        }
                        else
                        {
                            toolStripStatusLabel_status.Text = "连接正常";
                            LogFile.WriteLog("RFID读卡器连接正常。");
                            toolStripStatusLabel_status.ForeColor = Color.Black;
                            comboBox_order.Enabled = true;
                            Application.DoEvents();
                            voice.Speak("读卡器连接成功。");
                            
                        }
                        break;
                    }
                case 4:// UWE1 NETWORK;
                    {
                        tcpClientTester = new Uwe1NetApi.TcpClient(true);
                        tcpClientTester.uhfCallBack_Response = FormMain_UhfCallBack_Response;
                        tcpClientTester.uhfCallBack_NotificationEvent = FormMain_UhfCallBack_NotificationEvent;
                        if (tcpClientTester.OpenTcpClient(uwe1_ip_address, 8000) == 0)
                        {
                            toolStripStatusLabel_status.Text = "连接正常";
                            LogFile.WriteLog("RFID读卡器连接正常。");
                            toolStripStatusLabel_status.ForeColor = Color.Black;
                            comboBox_order.Enabled = true;
                            int tempData = Convert.ToInt32(uwe1_modempara.Substring(0, 2), 16);
                            int tempData1 = Convert.ToInt32(uwe1_modempara.Substring(2, 2), 16);
                            int tempData2 = Convert.ToInt32(uwe1_modempara.Substring(4, 4), 16);
                            if (tcpClientTester.SetModemParameters(tempData, tempData1, tempData2) == UhfType.RETURN_VALUE.RETURN_OK)
                            {
                            }
                            Application.DoEvents();
                            voice.Speak("读卡器连接成功。");
                        }
                        else
                        {
                            toolStripStatusLabel_status.Text = "连接失败";
                            LogFile.WriteLog("RFID读卡器连接失败。");
                        }
                        break;
                    }
                case 5:// UWE1 BT;
                    {
                        break;
                    }
                case 6:// UWE1 COMPORT;
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }




            //get the order list
            //List<string> list = HttpSend.getOrderList();
            //if (list != null)
            //{
            //    foreach (string item in list)
            //    {
            //        comboBox_order.Items.Add(item);
            //    }

            //    //select the first one by default
            //    if (comboBox_order.Items.Count > 0)
            //        comboBox_order.SelectedIndex = 0;
            //}

            if (ioControlEnable)
            {
                tcpClient = new Tcp517T_Api.TcpClient(true);
                if(tcpClient.OpenTcpClient(ioControlIp_address, ioControlTcp_port)==0)
                {
                    ioControlEnable = true;
                }
                else
                {
                    ioControlEnable = false;
                }
            }
            

            resetAllButtons();

           
            

            ThreadShowRealTime = new Thread(ThreadShowRealTimeFunc);
            ThreadShowRealTime.IsBackground = true;
            ThreadShowRealTime.Start();

            ThreadLedMonitor = new Thread(ThreadLedMonitorFunc);
            ThreadLedMonitor.IsBackground = true;
            ThreadLedMonitor.Start();

            ThreadNetworkMonitor = new Thread(ThreadNetworkMonitorFunc);
            ThreadNetworkMonitor.IsBackground = true;
            ThreadNetworkMonitor.Start();

            
            ThreadReadEpcsDictionaryProcess = new Thread(ThreadReadEpcsDictionaryProcessFunc);
            ThreadReadEpcsDictionaryProcessStyle = true;
            ThreadReadEpcsDictionaryProcess.IsBackground = true;
            ThreadReadEpcsDictionaryProcess.Start();

            Thread ThreadPingServer=new Thread(ThreadPingServerFunc);
            ThreadPingServer.IsBackground = true;
            ThreadPingServer.Start();

        }

        private void FormMain_UhfCallBack_Response(Information info)
        {
            switch (info.commandType)
            {
                case (int)UhfType.CommandType.START:
                    {
                        break;
                    }
                case (int)UhfType.CommandType.STOP:
                    {
                        break;
                    }
                case (int)UhfType.CommandType.INVENTORY:
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            //this.label_Monitor.Text = (monitorCounter++).ToString();
                            //textBoxShowDebug.AppendText(epc + "," + epc.Length + "\r\n");
                            if (debug_enable)
                            {
                                textBoxShowDebug.AppendText(info.epc + "," + info.epc.Length + "\r\n");
                            }

                            //if ((BtRfidReader_TempEpc.Length==0)&&(epc.Length>=24))
                            //{
                            //    textBoxShowDebug.AppendText("ok:"+epc + ","+ epc.Length +"\r\n");
                            //    BtRfidReader_TempEpcTimeout = 0;
                            //    BtRfidReader_TempEpcTimeoutEnable = true;
                            //    BtRfidReader_TempEpc = epc;
                            //    Application.DoEvents();
                            //    RfidActionProcess(epc);
                            //    //BtRfidReader_TempEpc = "";
                            //    textBoxShowDebug.AppendText("finish:" + epc + "," + epc.Length + "\r\n");
                            //}

                            if (ReadEpcsDictionaryLock)
                            {
                                return;
                            }
                            if (info.epc.Length == 24)
                            {
                                // add epcs to the map;
                                //ReadEpcsDictionaryTimer = 0;
                                try
                                {
                                    if (!ReadEpcsDictionary.ContainsKey(info.epc))
                                    {
                                        // if epc information is not in the dictionary, add a new epc information to the dictionary;
                                        ReadEpcsDictionary.Add(info.epc, 1);
                                    }
                                    else
                                    {
                                        // if epc information is in the dictionary, add the counter for the epc informaiton;
                                        ReadEpcsDictionary[info.epc] = ReadEpcsDictionary[info.epc] + 1;
                                    }
                                }
                                catch
                                {
                                    // exception;
                                }

                            }

                        }));
                        break;
                    }
                case (int)UhfType.CommandType.READ_TAG:
                    {
                        break;
                    }
                case (int)UhfType.CommandType.WRITE_TAG:
                    {
                        break;
                    }
                case (int)UhfType.CommandType.LOCK_TAG:
                    {
                        break;
                    }
                case (int)UhfType.CommandType.KILL_TAG:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            Application.DoEvents();
        }

        private void FormMain_UhfCallBack_NotificationEvent(Boolean reconnect_state)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                if (reconnect_state)
                {
                    //ShowDebugInfo("重连成功。");
                    
                    for (int i = 0; i < 5; i++)
                    {
                        Thread.Sleep(100);
                        int tempData = Convert.ToInt32(uwe1_modempara.Substring(0, 2), 16);
                        int tempData1 = Convert.ToInt32(uwe1_modempara.Substring(2, 2), 16);
                        int tempData2 = Convert.ToInt32(uwe1_modempara.Substring(4, 4), 16);
                        if (tcpClientTester.SetModemParameters(tempData, tempData1, tempData2) == UhfType.RETURN_VALUE.RETURN_OK)
                        {
                            if (tcpClientTester.StartInventory(true) == UhfType.RETURN_VALUE.RETURN_OK)
                            {
                                toolStripStatusLabel_status.Text = "连接正常";
                                //LogFile.WriteLog("RFID读卡器尝试重连。");
                                toolStripStatusLabel_status.ForeColor = Color.Black;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //ShowDebugInfo("重连失败。");
                    toolStripStatusLabel_status.Text = "尝试重连";
                    //LogFile.WriteLog("RFID读卡器尝试重连。");
                    toolStripStatusLabel_status.ForeColor = Color.Red;
                }
            }));
        }


        private void ThreadPingServerFunc()
        {
            for (int ping_i = 0; ping_i < 10; ping_i++)
            {
                try
                {
                    if (TestPing.PingIp(DistributionPlatform.ip_address))
                    {
                        NetworkState = true;
                        break;
                    }
                    else
                    {
                        NetworkState = false;
                        Thread.Sleep(2000);
                    }
                }
                catch
                {
                    NetworkState = false;
                }
            }
        }


        private Thread ThreadInventory;
        private Boolean ThreadInventoryStyle = false;
        private int timerCounter = 0;
        byte[,] tagData = null;
        int tagCount = 0;
        private void ThreadInventoryFunc()
        {
            timerCounter = 0;
            while (true)
            {
                byte tag_flag = 0;
                tagCount = 0;
                tagData = new byte[500, 14];
                int status = mR6100Api.EpcMultiTagIdentify(255, ref tagData, ref tagCount, ref tag_flag);
                //mR6100Api.EpcRead(255,Epc)

                if (status == MR6100Api.MR6100Api.SUCCESS_RETURN)
                {
                    int temp_tagCount = tagCount;
                    string strAnteNo = "", strID = "", strTemp = "";
                    if((temp_tagCount > 0)&&(tag_flag==0))
                    {
                        for (int i = 0; i < temp_tagCount; i++)
                        {
                            int j = 0;
                            strID = "";
                            strAnteNo = string.Format("{0:X2}", tagData[i, 1]);
                            for (j = 2; j < 14; j++) // update: 0->2, 12->14
                            {
                                strTemp = string.Format("{0:X2}", tagData[i, j]);
                                strID += strTemp;
                            }
                            if (strID.Equals("000000000000000000000000"))
                            {
                                // nothing to do, maybe an err read;
                            }
                            else
                            {
                                this.Invoke((EventHandler)(delegate
                                {

                                    textBoxShowDebug.AppendText(strID + "," + strID.Length + "\r\n");

                                    if (ReadEpcsDictionaryLock)
                                    {
                                        return;
                                    }
                                    if (strID.Length == 24)
                                    {
                                        // add epcs to the map;
                                        //ReadEpcsDictionaryTimer = 0;
                                        try
                                        {
                                            if (!ReadEpcsDictionary.ContainsKey(strID))
                                            {
                                                // if epc information is not in the dictionary, add a new epc information to the dictionary;
                                                ReadEpcsDictionary.Add(strID, 1);
                                            }
                                            else
                                            {
                                                // if epc information is in the dictionary, add the counter for the epc informaiton;
                                                ReadEpcsDictionary[strID] = ReadEpcsDictionary[strID] + 1;
                                            }
                                        }
                                        catch
                                        {
                                            // exception;
                                        }

                                    }
                                }));
                            }
                        }
                    }
                    else
                    {
                        // no tag found;
                    }
                }
                Thread.Sleep(100);
            }
        }

        void ThreadNetworkMonitorFunc()
        {
            ThreadNetworkMonitorStyle = true;
            int secondCounter = 0;
            while(true)
            {
                if(secondCounter++>=3600)
                {
                    secondCounter = 0;
                    try
                    {
                        if (TestPing.PingIp(DistributionPlatform.ip_address))
                        {
                            NetworkState = true;
                        }
                        else
                        {
                            NetworkState = false;
                        }
                    }
                    catch
                    {
                        NetworkState = false;
                    }
                }
                Thread.Sleep(1000);
                if(combobox_order_dropdown_timer++>10)
                {
                    combobox_order_dropdown_timer = 10;
                }
            }
        }
        

        int LedMonitorTimer = 0;
        int LedNumber=0;
        void ThreadLedMonitorFunc()
        {
            ThreadLedMonitorStyle = true;
            //LedMonitorTimer = 10;
            while (true)
            {
                //if(LedMonitorTimer > 0)
                //{
                //    LedMonitorTimer--;
                //    LedFlash(LedNumber);
                //    if(LedMonitorTimer==0)
                //    {
                //        if (orderItems[LedNumber - 1].isFull)
                //        {
                //            updateItemStatus(LedNumber, true);
                //            LightControllerItemStatus(LedNumber, true);
                //        }
                //        else
                //        {
                //            updateItemStatus(LedNumber, false);
                //            LightControllerItemStatus(LedNumber, false);
                //        }
                //        //LedNumber++;// for test;
                //        //LedMonitorTimer = 10;
                //    }
                //}

                if (LedNumber > 0)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        LedFlash(LedNumber);
                    }));
                }
                if(BtRfidReader_TempEpcTimeoutEnable)
                {
                    if (BtRfidReader_TempEpcTimeout++ >= TEMPEPCTIMEOUT_MAX)
                    {
                        BtRfidReader_TempEpcTimeoutEnable = false;
                        BtRfidReader_TempEpc = "";
                    }
                }
                


                Thread.Sleep(200);
            }
        }
        void ThreadShowRealTimeFunc()
        {
            int cursor_timer_counter = 0;
            int bt_connect_counter = 0;
            ThreadShowRealTimeStyle = true;
            int logoFlashTimer = 0;
            Font font = new Font("宋体", (float)9, FontStyle.Bold);
            PointF pt = new PointF(batteryProcessBar.Width / 2 - 15, batteryProcessBar.Height / 2 - 5);
            //Label l = new Label();
            //this.Invoke(new MethodInvoker(delegate ()
            //{
                
            //    l.Parent = toolStripProgressBarBatteryGauge.Control;
            //    l.BackColor = Color.Transparent;

            //    l.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //    l.Width = batteryProcessBar.Width;
            //    l.Height = batteryProcessBar.Height;
            //    //batteryProcessBar.CreateGraphics().DrawString(toolStripProgressBarBatteryGauge.Value.ToString() + "%", font, Brushes.Black, pt);
                
            //}));
                
            while (true)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    //this.labelLocalDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
                    if(SystemPowerState.ReadAlternatingCurrentConnectState().Equals("Offline"))
                    {
                        int tempBatteryVar = SystemPowerState.ReadBatteryInfo() % 100;

                        //toolStripProgressBarBatteryGauge.Value = tempBatteryVar;
                        batteryProcessBar.Refresh();
                        toolStripProgressBarBatteryGauge.Value = tempBatteryVar;
                        //l.Text = toolStripProgressBarBatteryGauge.Value.ToString();
                        if (tempBatteryVar >= 30)
                        {
                            batteryProcessBar.BackColor = Color.Green;
                            //l.ForeColor = Color.Green;
                            batteryProcessBar.Refresh();
                            //batteryProcessBar.Value = tempBatteryVar;
                            //batteryProcessBar.CreateGraphics().DrawString(toolStripProgressBarBatteryGauge.Value.ToString() + "%", font, Brushes.Red, pt);
                        }
                        else
                        {
                            batteryProcessBar.BackColor = Color.Red;
                            //l.ForeColor = Color.Red;
                            batteryProcessBar.Refresh();

                            //batteryProcessBar.CreateGraphics().DrawString(toolStripProgressBarBatteryGauge.Value.ToString() + "%", font, Brushes.Red, pt);

                        }
                        batteryProcessBar.CreateGraphics().DrawString(tempBatteryVar.ToString() + "%", font, Brushes.Black, pt);
                        //batteryProcessBar.Value = tempBatteryVar;
                        //toolStripStatusLabelBatteryGauge.Text = batteryProcessBar.Value.ToString()+"%";


                        //toolTipShowBattery.SetToolTip(batteryProcessBar, batteryProcessBar.Value.ToString());
                    }
                    else
                    {
                        batteryProcessBar.BackColor = Color.Green;
                        //l.ForeColor = Color.Green;
                        batteryProcessBar.Refresh();
                        batteryProcessBar.CreateGraphics().DrawString("充电中", font, Brushes.Black, pt);
                        //toolTipShowBattery.SetToolTip(batteryProcessBar, batteryProcessBar.Value.ToString());
                    }
                    this.labelLocalTime.Text = DateTime.Now.ToString("HH:mm:ss");
                    if (button_confirm.Enabled)
                    {
                        if (logoFlashTimer % 2 == 0)
                        {
                            pictureBoxLogo.BackgroundImage = image_ic_logo_neg;
                        }
                        else
                        {
                            pictureBoxLogo.BackgroundImage = image_ic_logo_pos;
                        }
                        logoFlashTimer++;
                    }

                    // check network state;
                    if (NetworkState)
                    {
                        toolStripStatusLabelNetworkStatus.Text = "已连接";
                        toolStripStatusLabelNetworkStatus.ForeColor = Color.Black;
                    }
                    else
                    {
                        toolStripStatusLabelNetworkStatus.Text = "未连接";
                        toolStripStatusLabelNetworkStatus.ForeColor = Color.Red;
                    }
                    //point the cursor to the comboBox_order;
                    if (cursor_timer_counter++ > 60)
                    {
                        cursor_timer_counter = 0;
                        comboBox_order.Focus();
                    }
                    
                   

                    Application.DoEvents();
                }));
               
                int[] returnValue = new int[1];
                if (ioControlEnable)
                {
                    if (tcpClient.IsConnected)
                    {
                        if (tcpClientTimeout++ > 5)
                        {
                            tcpClientTimeout = 0;
                            int tempReturn = tcpClient.ReadDoSwitchOut(0x01, 0x0000, 32, returnValue);
                            if (tempReturn == 0)
                            {
                                // success;
                            }
                        }

                    }
                }
                
                Thread.Sleep(1000);
            }
        }


        private void ThreadBtReaderMonitorFunc()
        {
            ThreadBtReaderMonitorStyle = true;
            while (true)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    if (isBtConnected)
                    {
                        bool isConn = yTJBluetooth.isConnect;// if true, is disconnected;
                        if ((isConn == true) && (isConnecting == false))
                        {
                            //yTJBluetooth.offBluetooth();
                            //yTJBluetooth = new YTJBluetooth();
                            //Thread.Sleep(10);
                            //bt_connect_counter = 0;
                            //Console.WriteLine("蓝牙设备连接已断开--尝试重连---");
                            textBoxShowDebug.AppendText("蓝牙重连。\r\n");
                            toolStripStatusLabel_status.Text = "尝试重连";
                            //LogFile.WriteLog("RFID读卡器尝试重连。");
                            toolStripStatusLabel_status.ForeColor = Color.Red;
                            isReconnection = true;
                            isConnecting = true;
                            //isBtConnected = false;
                            yTJBluetooth.connectBluetooth(MyBluetoothAddress);
                            //ThreadConnectBtRfidReader = new Thread(ConnectBtRfidReaderFunc);
                            //ThreadConnectBtRfidReader.IsBackground = true;
                            //ThreadConnectBtRfidReader.Start();
                        }
                        else
                        {
                            //bt_connect_counter = 3;
                            //toolStripStatusLabel_status.Text = "连接正常";
                            //LogFile.WriteLog("RFID读卡器连接正常。");
                            //toolStripStatusLabel_status.ForeColor = Color.Black;
                        }
                    }
                }));
                   
                Thread.Sleep(1000);
            }
           
        }

        //Connect to the reader.
        private Boolean connectToReader()
        {
            int port = 0;
            byte fBaud = 4;//57600
            int openresult = 30;
            openresult = StaticClassReaderB.AutoOpenComPort(ref port, ref fComAdr, fBaud, ref frmcomportindex);
            if (openresult == 0)
            {
                string temp = "COM" + frmcomportindex + " opened.";
                this.toolStripStatusLabel_status.Text = temp;
                this.toolStripStatusLabel_status.ForeColor = System.Drawing.Color.Green;

                return true;
            }
            return false;
        }

        //The reader timer.
        private void RFIDReader_timer(object sender, EventArgs e)
        {
            if (isConnected)
                Inventory();
        }

        private void Inventory()
        {
            int CardNum = 0;
            int Totallen = 0;
            int EPClen, m;
            byte[] EPC = new byte[5000];
            int CardIndex;
            string temps;
            string sEPC;
            byte AdrTID = 0;
            byte LenTID = 0;
            byte TIDFlag = 0;

            //Call inventory API.
            fCmdRet = StaticClassReaderB.Inventory_G2(ref fComAdr, AdrTID, LenTID, TIDFlag, EPC, ref Totallen, ref CardNum, frmcomportindex);
            if ((fCmdRet == 1) | (fCmdRet == 2) | (fCmdRet == 3) | (fCmdRet == 4) | (fCmdRet == 0xFB))//代表已查找结束，
            {
                byte[] daw = new byte[Totallen];
                Array.Copy(EPC, daw, Totallen);
                temps = ByteArrayToHexString(daw);

                if (CardNum == 0)
                {
                    return;
                }

                m = 0;
                for (CardIndex = 0; CardIndex < CardNum; CardIndex++)
                {
                    EPClen = daw[m];
                    sEPC = temps.Substring(m * 2 + 2, EPClen * 2);
                    m = m + EPClen + 1;
                    if (sEPC.Length != EPClen * 2)
                        return;

                    //add this epc to hashmap if it does not exists.
                    //if (!EPCDictionary.ContainsKey(sEPC))
                    //{
                    //    EPCDictionary.Add(sEPC, 0);
                    //}
                    RfidActionProcess(sEPC);
                }
            }
        }

        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();
        }

        //The tick to check if there is the new epc in EPC Dictionary.
        private void getItemNumber(object sender, EventArgs e)
        {
            //bool isDone = false;
            //if (comboBox_order.SelectedIndex > -1 && EPCDictionary.Count > 0)
            //{
            //    string orderNum = comboBox_order.SelectedItem.ToString();
            //    foreach (string key in EPCDictionary.Keys)
            //    {
            //        //check if this epc is processed
            //        int value;
            //        EPCDictionary.TryGetValue(key, out value);
            //        if (value == 2)
            //            continue;

            //        bool isItemDone = false;//if this TB order soring is done?
            //        int itemNumber = HttpSend.getItemNumber(orderNum, key, out isItemDone);
            //        if (itemNumber > 0 && itemNumber < 25)
            //        {
            //            //display the item number
            //            label_current.Text = itemNumber.ToString();

            //            //update the item to green once it is done.
            //            if (isItemDone && updateItemStatus(itemNumber))
            //            {
            //                try
            //                {
            //                    int doneNum = Convert.ToInt32(label_donenum.Text) + 1;
            //                    label_donenum.Text = doneNum.ToString();
            //                    if (doneNum == Convert.ToInt32(label_totalnum.Text))
            //                    {
            //                        MessageBox.Show(this, "订单:" + comboBox_order.SelectedText + " 分拣完成");
            //                        LogFile.WriteLog("订单:" + comboBox_order.SelectedText + " 分拣完成");
            //                        resetAllButtons();
            //                        isDone = true;
            //                        break;
            //                    }
            //                }
            //                catch (Exception ec)
            //                {

            //                }
            //            }
            //            //updte the value to mark this epc was processed.
            //            EPCDictionary[key] = 2;
            //        }
            //        else
            //        {
            //            //updte the value to mark this epc was processed.
            //            EPCDictionary[key] = 2;
            //            MessageBox.Show(this, "扫描到不属于此订单的商品！");
            //            LogFile.WriteLog("扫描到不属于此订单的商品！");
            //            break;
            //        }
            //    }
            //}

            //if (isDone)
            //    EPCDictionary.Clear();
        }

        //reset all buttons
        private void resetAllButtons()
        {
            //button_1.BackColor = System.Drawing.Color.Red;
            //button_2.BackColor = System.Drawing.Color.Red;
            //button_3.BackColor = System.Drawing.Color.Red;
            //button_4.BackColor = System.Drawing.Color.Red;
            //button_5.BackColor = System.Drawing.Color.Red;
            //button_6.BackColor = System.Drawing.Color.Red;
            //button_7.BackColor = System.Drawing.Color.Red;
            //button_8.BackColor = System.Drawing.Color.Red;
            //button_9.BackColor = System.Drawing.Color.Red;
            //button_10.BackColor = System.Drawing.Color.Red;
            //button_11.BackColor = System.Drawing.Color.Red;
            //button_12.BackColor = System.Drawing.Color.Red;
            //button_13.BackColor = System.Drawing.Color.Red;
            //button_14.BackColor = System.Drawing.Color.Red;
            //button_15.BackColor = System.Drawing.Color.Red;
            //button_16.BackColor = System.Drawing.Color.Red;
            //button_17.BackColor = System.Drawing.Color.Red;
            //button_18.BackColor = System.Drawing.Color.Red;
            //button_19.BackColor = System.Drawing.Color.Red;
            //button_20.BackColor = System.Drawing.Color.Red;
            //button_21.BackColor = System.Drawing.Color.Red;
            //button_22.BackColor = System.Drawing.Color.Red;
            //button_23.BackColor = System.Drawing.Color.Red;
            //button_24.BackColor = System.Drawing.Color.Red;

            button_1.BackgroundImage = image_ic_light_gray;
            button_2.BackgroundImage = image_ic_light_gray;
            button_3.BackgroundImage = image_ic_light_gray;
            button_4.BackgroundImage = image_ic_light_gray;
            button_5.BackgroundImage = image_ic_light_gray;
            button_6.BackgroundImage = image_ic_light_gray;
            button_7.BackgroundImage = image_ic_light_gray;
            button_8.BackgroundImage = image_ic_light_gray;
            button_9.BackgroundImage = image_ic_light_gray;
            button_10.BackgroundImage = image_ic_light_gray;
            button_11.BackgroundImage = image_ic_light_gray;
            button_12.BackgroundImage = image_ic_light_gray;
            button_13.BackgroundImage = image_ic_light_gray;
            button_14.BackgroundImage = image_ic_light_gray;
            button_15.BackgroundImage = image_ic_light_gray;
            button_16.BackgroundImage = image_ic_light_gray;
            button_17.BackgroundImage = image_ic_light_gray;
            button_18.BackgroundImage = image_ic_light_gray;
            button_19.BackgroundImage = image_ic_light_gray;
            button_20.BackgroundImage = image_ic_light_gray;
            button_21.BackgroundImage = image_ic_light_gray;
            button_22.BackgroundImage = image_ic_light_gray;
            button_23.BackgroundImage = image_ic_light_gray;
            button_24.BackgroundImage = image_ic_light_gray;

            checkBox_1.Visible = false;
            checkBox_2.Visible = false;
            checkBox_3.Visible = false;
            checkBox_4.Visible = false;
            checkBox_5.Visible = false;
            checkBox_6.Visible = false;
            checkBox_7.Visible = false;
            checkBox_8.Visible = false;
            checkBox_9.Visible = false;
            checkBox_10.Visible = false;

            checkBox_11.Visible = false;
            checkBox_12.Visible = false;
            checkBox_13.Visible = false;
            checkBox_14.Visible = false;
            checkBox_15.Visible = false;
            checkBox_16.Visible = false;
            checkBox_17.Visible = false;
            checkBox_18.Visible = false;
            checkBox_19.Visible = false;
            checkBox_20.Visible = false;

            checkBox_21.Visible = false;
            checkBox_22.Visible = false;
            checkBox_23.Visible = false;
            checkBox_24.Visible = false;

            checkBoxCheckedAll(false);
            ShowOrderClass_AllShow("");

            if (ioControlEnable)
            {
                int[] returnValue = new int[1];
                if (tcpClient.IsConnected)
                {
                    tcpClient.WriteDoMultiSwitchOut(0x01, 0x0000, 0x20, 0x00000000, returnValue);
                }
            }
            
            for(int i=0;i<24;i++)
            {
                ShowBoxData(i + 1, "");
            }


            TotalEpcCount = 0;
            TotalEpcCount_i = 0;

            //button_1.BackgroundImage = image_ic_light_red;
            //button_2.BackgroundImage = image_ic_light_red;
            //button_3.BackgroundImage = image_ic_light_red;
            //button_4.BackgroundImage = image_ic_light_red;
            //button_5.BackgroundImage = image_ic_light_red;
            //button_6.BackgroundImage = image_ic_light_red;
            //button_7.BackgroundImage = image_ic_light_red;
            //button_8.BackgroundImage = image_ic_light_red;
            //button_9.BackgroundImage = image_ic_light_red;
            //button_10.BackgroundImage = image_ic_light_red;
            //button_11.BackgroundImage = image_ic_light_red;
            //button_12.BackgroundImage = image_ic_light_red;
            //button_13.BackgroundImage = image_ic_light_red;
            //button_14.BackgroundImage = image_ic_light_red;
            //button_15.BackgroundImage = image_ic_light_red;
            //button_16.BackgroundImage = image_ic_light_red;
            //button_17.BackgroundImage = image_ic_light_red;
            //button_18.BackgroundImage = image_ic_light_red;
            //button_19.BackgroundImage = image_ic_light_red;
            //button_20.BackgroundImage = image_ic_light_red;
            //button_21.BackgroundImage = image_ic_light_red;
            //button_22.BackgroundImage = image_ic_light_red;
            //button_23.BackgroundImage = image_ic_light_red;
            //button_24.BackgroundImage = image_ic_light_red;
        }

        private void LedFlash(int number)
        {
            switch (number)
            {
                case 1:
                    if (button_1.BackgroundImage == image_ic_light_green)
                    {
                        button_1.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_1.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 2:
                    if (button_2.BackgroundImage == image_ic_light_green)
                    {
                        button_2.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_2.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 3:
                    if (button_3.BackgroundImage == image_ic_light_green)
                    {
                        button_3.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_3.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 4:
                    if (button_4.BackgroundImage == image_ic_light_green)
                    {
                        button_4.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_4.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 5:
                    if (button_5.BackgroundImage == image_ic_light_green)
                    {
                        button_5.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_5.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 6:
                    if (button_6.BackgroundImage == image_ic_light_green)
                    {
                        button_6.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_6.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 7:
                    if (button_7.BackgroundImage == image_ic_light_green)
                    {
                        button_7.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_7.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 8:
                    if (button_8.BackgroundImage == image_ic_light_green)
                    {
                        button_8.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_8.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 9:
                    if (button_9.BackgroundImage == image_ic_light_green)
                    {
                        button_9.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_9.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 10:
                    if (button_10.BackgroundImage == image_ic_light_green)
                    {
                        button_10.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_10.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 11:
                    if (button_11.BackgroundImage == image_ic_light_green)
                    {
                        button_11.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_11.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 12:
                    if (button_12.BackgroundImage == image_ic_light_green)
                    {
                        button_12.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_12.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 13:
                    if (button_13.BackgroundImage == image_ic_light_green)
                    {
                        button_13.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_13.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 14:
                    if (button_14.BackgroundImage == image_ic_light_green)
                    {
                        button_14.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_14.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 15:
                    if (button_15.BackgroundImage == image_ic_light_green)
                    {
                        button_15.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_15.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 16:
                    if (button_16.BackgroundImage == image_ic_light_green)
                    {
                        button_16.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_16.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 17:
                    if (button_17.BackgroundImage == image_ic_light_green)
                    {
                        button_17.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_17.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 18:
                    if (button_18.BackgroundImage == image_ic_light_green)
                    {
                        button_18.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_18.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 19:
                    if (button_19.BackgroundImage == image_ic_light_green)
                    {
                        button_19.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_19.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 20:
                    if (button_20.BackgroundImage == image_ic_light_green)
                    {
                        button_20.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_20.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 21:
                    if (button_21.BackgroundImage == image_ic_light_green)
                    {
                        button_21.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_21.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 22:
                    if (button_22.BackgroundImage == image_ic_light_green)
                    {
                        button_22.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_22.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 23:
                    if (button_23.BackgroundImage == image_ic_light_green)
                    {
                        button_23.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_23.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
                case 24:
                    if (button_24.BackgroundImage == image_ic_light_green)
                    {
                        button_24.BackgroundImage = image_ic_light_red;
                        LightControllerItemStatus(LedNumber, true);
                    }
                    else
                    {
                        button_24.BackgroundImage = image_ic_light_green;
                        LightControllerItemStatus(LedNumber, false);
                    }
                    break;
            }
        }

        private void ShowOrderClass_AllShow(String str)
        {
            label_ShowOrderClass1.Text = str;
            label_ShowOrderClass2.Text = str;
            label_ShowOrderClass3.Text = str;
            label_ShowOrderClass4.Text = str;
            label_ShowOrderClass5.Text = str;
            label_ShowOrderClass6.Text = str;
            label_ShowOrderClass7.Text = str;
            label_ShowOrderClass8.Text = str;
            label_ShowOrderClass9.Text = str;
            label_ShowOrderClass10.Text = str;

            label_ShowOrderClass11.Text = str;
            label_ShowOrderClass12.Text = str;
            label_ShowOrderClass13.Text = str;
            label_ShowOrderClass14.Text = str;
            label_ShowOrderClass15.Text = str;
            label_ShowOrderClass16.Text = str;
            label_ShowOrderClass17.Text = str;
            label_ShowOrderClass18.Text = str;
            label_ShowOrderClass19.Text = str;
            label_ShowOrderClass20.Text = str;

            label_ShowOrderClass21.Text = str;
            label_ShowOrderClass22.Text = str;
            label_ShowOrderClass23.Text = str;
            label_ShowOrderClass24.Text = str;
        }

        private void ShowOrderClass(int boxNumber, String str, Color color)
        {
            switch (boxNumber)
            {
                case 1:
                    {
                        label_ShowOrderClass1.Text = str;
                        label_ShowOrderClass1.ForeColor = color;
                        break;
                    }
                case 2:
                    {
                        label_ShowOrderClass2.Text = str;
                        label_ShowOrderClass2.ForeColor = color;
                        break;
                    }
                case 3:
                    {
                        label_ShowOrderClass3.Text = str;
                        label_ShowOrderClass3.ForeColor = color;
                        break;
                    }
                case 4:
                    {
                        label_ShowOrderClass4.Text = str;
                        label_ShowOrderClass4.ForeColor = color;
                        break;
                    }
                case 5:
                    {
                        label_ShowOrderClass5.Text = str;
                        label_ShowOrderClass5.ForeColor = color;
                        break;
                    }
                case 6:
                    {
                        label_ShowOrderClass6.Text = str;
                        label_ShowOrderClass6.ForeColor = color;
                        break;
                    }
                case 7:
                    {
                        label_ShowOrderClass7.Text = str;
                        label_ShowOrderClass7.ForeColor = color;
                        break;
                    }
                case 8:
                    {
                        label_ShowOrderClass8.Text = str;
                        label_ShowOrderClass8.ForeColor = color;
                        break;
                    }
                case 9:
                    {
                        label_ShowOrderClass9.Text = str;
                        label_ShowOrderClass9.ForeColor = color;
                        break;
                    }
                case 10:
                    {
                        label_ShowOrderClass10.Text = str;
                        label_ShowOrderClass10.ForeColor = color;
                        break;
                    }
                case 11:
                    {
                        label_ShowOrderClass11.Text = str;
                        label_ShowOrderClass11.ForeColor = color;
                        break;
                    }
                case 12:
                    {
                        label_ShowOrderClass12.Text = str;
                        label_ShowOrderClass12.ForeColor = color;
                        break;
                    }
                case 13:
                    {
                        label_ShowOrderClass13.Text = str;
                        label_ShowOrderClass13.ForeColor = color;
                        break;
                    }
                case 14:
                    {
                        label_ShowOrderClass14.Text = str;
                        label_ShowOrderClass14.ForeColor = color;
                        break;
                    }
                case 15:
                    {
                        label_ShowOrderClass15.Text = str;
                        label_ShowOrderClass15.ForeColor = color;
                        break;
                    }
                case 16:
                    {
                        label_ShowOrderClass16.Text = str;
                        label_ShowOrderClass16.ForeColor = color;
                        break;
                    }
                case 17:
                    {
                        label_ShowOrderClass17.Text = str;
                        label_ShowOrderClass17.ForeColor = color;
                        break;
                    }
                case 18:
                    {
                        label_ShowOrderClass18.Text = str;
                        label_ShowOrderClass18.ForeColor = color;
                        break;
                    }
                case 19:
                    {
                        label_ShowOrderClass19.Text = str;
                        label_ShowOrderClass19.ForeColor = color;
                        break;
                    }
                case 20:
                    {
                        label_ShowOrderClass20.Text = str;
                        label_ShowOrderClass20.ForeColor = color;
                        break;
                    }
                case 21:
                    {
                        label_ShowOrderClass21.Text = str;
                        label_ShowOrderClass21.ForeColor = color;
                        break;
                    }
                case 22:
                    {
                        label_ShowOrderClass22.Text = str;
                        label_ShowOrderClass22.ForeColor = color;
                        break;
                    }
                case 23:
                    {
                        label_ShowOrderClass23.Text = str;
                        label_ShowOrderClass23.ForeColor = color;
                        break;
                    }
                case 24:
                    {
                        label_ShowOrderClass24.Text = str;
                        label_ShowOrderClass24.ForeColor = color;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void SetCancelLedStatus(int number) //
        {
            switch (number)
            {
                case 1:
                    button_1.BackgroundImage = image_ic_light_yellow;
                    break;
                case 2:
                    button_2.BackgroundImage = image_ic_light_yellow;
                    break;
                case 3:
                    button_3.BackgroundImage = image_ic_light_yellow;
                    break;
                case 4:
                    button_4.BackgroundImage = image_ic_light_yellow;
                    break;
                case 5:
                    button_5.BackgroundImage = image_ic_light_yellow;
                    break;
                case 6:
                    button_6.BackgroundImage = image_ic_light_yellow;
                    break;
                case 7:
                    button_7.BackgroundImage = image_ic_light_yellow;
                    break;
                case 8:
                    button_8.BackgroundImage = image_ic_light_yellow;
                    break;
                case 9:
                    button_9.BackgroundImage = image_ic_light_yellow;
                    break;
                case 10:
                    button_10.BackgroundImage = image_ic_light_yellow;
                    break;
                case 11:
                    button_11.BackgroundImage = image_ic_light_yellow;
                    break;
                case 12:
                    button_12.BackgroundImage = image_ic_light_yellow;
                    break;
                case 13:
                    button_13.BackgroundImage = image_ic_light_yellow;
                    break;
                case 14:
                    button_14.BackgroundImage = image_ic_light_yellow;
                    break;
                case 15:
                    button_15.BackgroundImage = image_ic_light_yellow;
                    break;
                case 16:
                    button_16.BackgroundImage = image_ic_light_yellow;
                    break;
                case 17:
                    button_17.BackgroundImage = image_ic_light_yellow;
                    break;
                case 18:
                    button_18.BackgroundImage = image_ic_light_yellow;
                    break;
                case 19:
                    button_19.BackgroundImage = image_ic_light_yellow;
                    break;
                case 20:
                    button_20.BackgroundImage = image_ic_light_yellow;
                    break;
                case 21:
                    button_21.BackgroundImage = image_ic_light_yellow;
                    break;
                case 22:
                    button_22.BackgroundImage = image_ic_light_yellow;
                    break;
                case 23:
                    button_23.BackgroundImage = image_ic_light_yellow;
                    break;
                case 24:
                    button_24.BackgroundImage = image_ic_light_yellow;
                    break;
            }
            Application.DoEvents();
        }

        private int LightControllerItemStatus(int number,Boolean status)
        {
            int controllerReturn = 1;
            if (ioControlEnable)
            {
                int[] returnValue = new int[1];
                tcpClientTimeout = 0;
                if (status)
                {
                    controllerReturn = tcpClient.WriteDoSingleSwitchOut(0x01, number - 1, 0xff00, returnValue);
                }
                else
                {
                    controllerReturn = tcpClient.WriteDoSingleSwitchOut(0x01, number - 1, 0x0000, returnValue);
                }
            }
                
            return controllerReturn;
        }
    
        //Update the item to green
        private Boolean updateItemStatus(int number, Boolean status)
        {
            switch (number)
            {
                case 1:
                    if (status)
                    {
                        button_1.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_1.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 2:
                    if (status)
                    {
                        button_2.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_2.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 3:
                    if (status)
                    {
                        button_3.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_3.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 4:
                    if (status)
                    {
                        button_4.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_4.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 5:
                    if (status)
                    {
                        button_5.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_5.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 6:
                    if (status)
                    {
                        button_6.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_6.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 7:
                    if (status)
                    {
                        button_7.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_7.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 8:
                    if (status)
                    {
                        button_8.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_8.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 9:
                    if (status)
                    {
                        button_9.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_9.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 10:
                    if (status)
                    {
                        button_10.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_10.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 11:
                    if (status)
                    {
                        button_11.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_11.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 12:
                    if (status)
                    {
                        button_12.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_12.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 13:
                    if (status)
                    {
                        button_13.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_13.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 14:
                    if (status)
                    {
                        button_14.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_14.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 15:
                    if (status)
                    {
                        button_15.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_15.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 16:
                    if (status)
                    {
                        button_16.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_16.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 17:
                    if (status)
                    {
                        button_17.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_17.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 18:
                    if (status)
                    {
                        button_18.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_18.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 19:
                    if (status)
                    {
                        button_19.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_19.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 20:
                    if (status)
                    {
                        button_20.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_20.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 21:
                    if (status)
                    {
                        button_21.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_21.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 22:
                    if (status)
                    {
                        button_22.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_22.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 23:
                    if (status)
                    {
                        button_23.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_23.BackgroundImage = image_ic_light_red;
                    }
                    break;
                case 24:
                    if(status)
                    {
                        button_24.BackgroundImage = image_ic_light_green;
                    }
                    else
                    {
                        button_24.BackgroundImage = image_ic_light_red;
                    }
                    break;
            }
            //switch (number)
            //{
            //    case 1:
            //        if (button_1.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_1.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 2:
            //        if (button_2.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_2.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 3:
            //        if (button_3.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_3.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 4:
            //        if (button_4.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_4.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 5:
            //        if (button_5.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_5.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 6:
            //        if (button_6.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_6.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 7:
            //        if (button_7.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_7.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 8:
            //        if (button_8.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_8.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 9:
            //        if (button_9.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_9.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 10:
            //        if (button_10.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_10.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 11:
            //        if (button_11.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_11.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 12:
            //        if (button_12.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_12.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 13:
            //        if (button_13.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_13.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 14:
            //        if (button_14.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_14.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 15:
            //        if (button_15.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_15.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 16:
            //        if (button_16.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_16.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 17:
            //        if (button_17.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_17.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 18:
            //        if (button_18.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_18.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 19:
            //        if (button_19.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_19.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 20:
            //        if (button_20.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_20.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 21:
            //        if (button_21.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_21.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 22:
            //        if (button_22.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_22.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 23:
            //        if (button_23.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_23.BackgroundImage = image_ic_light_green;
            //        break;
            //    case 24:
            //        if (button_24.BackgroundImage == image_ic_light_green)
            //            return false;
            //        button_24.BackgroundImage = image_ic_light_green;
            //        break;
            //}

            return true;
        }

        private void checkBoxCheckedAll(Boolean status)
        {
            checkBox_1.Checked = status;
            checkBox_2.Checked = status;
            checkBox_3.Checked = status;
            checkBox_4.Checked = status;
            checkBox_5.Checked = status;
            checkBox_6.Checked = status;
            checkBox_7.Checked = status;
            checkBox_8.Checked = status;
            checkBox_9.Checked = status;
            checkBox_10.Checked = status;

            checkBox_11.Checked = status;
            checkBox_12.Checked = status;
            checkBox_13.Checked = status;
            checkBox_14.Checked = status;
            checkBox_15.Checked = status;
            checkBox_16.Checked = status;
            checkBox_17.Checked = status;
            checkBox_18.Checked = status;
            checkBox_19.Checked = status;
            checkBox_20.Checked = status;

            checkBox_21.Checked = status;
            checkBox_22.Checked = status;
            checkBox_23.Checked = status;
            checkBox_24.Checked = status;
        }

        private void checkBoxEnable(int number,Boolean status)
        {
            switch(number)
            {
                case 1:
                    if(status)
                    {
                        checkBox_1.Visible = true;
                    }
                    else
                    {
                        checkBox_1.Visible = false;
                    }
                    break;
                case 2:
                    if (status)
                    {
                        checkBox_2.Visible = true;
                    }
                    else
                    {
                        checkBox_2.Visible = false;
                    }
                    break;
                case 3:
                    if (status)
                    {
                        checkBox_3.Visible = true;
                    }
                    else
                    {
                        checkBox_3.Visible = false;
                    }
                    break;
                case 4:
                    if (status)
                    {
                        checkBox_4.Visible = true;
                    }
                    else
                    {
                        checkBox_4.Visible = false;
                    }
                    break;
                case 5:
                    if (status)
                    {
                        checkBox_5.Visible = true;
                    }
                    else
                    {
                        checkBox_5.Visible = false;
                    }
                    break;
                case 6:
                    if (status)
                    {
                        checkBox_6.Visible = true;
                    }
                    else
                    {
                        checkBox_6.Visible = false;
                    }
                    break;
                case 7:
                    if (status)
                    {
                        checkBox_7.Visible = true;
                    }
                    else
                    {
                        checkBox_7.Visible = false;
                    }
                    break;
                case 8:
                    if (status)
                    {
                        checkBox_8.Visible = true;
                    }
                    else
                    {
                        checkBox_8.Visible = false;
                    }
                    break;
                case 9:
                    if (status)
                    {
                        checkBox_9.Visible = true;
                    }
                    else
                    {
                        checkBox_9.Visible = false;
                    }
                    break;
                case 10:
                    if (status)
                    {
                        checkBox_10.Visible = true;
                    }
                    else
                    {
                        checkBox_10.Visible = false;
                    }
                    break;
                case 11:
                    if (status)
                    {
                        checkBox_11.Visible = true;
                    }
                    else
                    {
                        checkBox_11.Visible = false;
                    }
                    break;
                case 12:
                    if (status)
                    {
                        checkBox_12.Visible = true;
                    }
                    else
                    {
                        checkBox_12.Visible = false;
                    }
                    break;
                case 13:
                    if (status)
                    {
                        checkBox_13.Visible = true;
                    }
                    else
                    {
                        checkBox_13.Visible = false;
                    }
                    break;
                case 14:
                    if (status)
                    {
                        checkBox_14.Visible = true;
                    }
                    else
                    {
                        checkBox_14.Visible = false;
                    }
                    break;
                case 15:
                    if (status)
                    {
                        checkBox_15.Visible = true;
                    }
                    else
                    {
                        checkBox_15.Visible = false;
                    }
                    break;
                case 16:
                    if (status)
                    {
                        checkBox_16.Visible = true;
                    }
                    else
                    {
                        checkBox_16.Visible = false;
                    }
                    break;
                case 17:
                    if (status)
                    {
                        checkBox_17.Visible = true;
                    }
                    else
                    {
                        checkBox_17.Visible = false;
                    }
                    break;
                case 18:
                    if (status)
                    {
                        checkBox_18.Visible = true;
                    }
                    else
                    {
                        checkBox_18.Visible = false;
                    }
                    break;
                case 19:
                    if (status)
                    {
                        checkBox_19.Visible = true;
                    }
                    else
                    {
                        checkBox_19.Visible = false;
                    }
                    break;
                case 20:
                    if (status)
                    {
                        checkBox_20.Visible = true;
                    }
                    else
                    {
                        checkBox_20.Visible = false;
                    }
                    break;
                case 21:
                    if (status)
                    {
                        checkBox_21.Visible = true;
                    }
                    else
                    {
                        checkBox_21.Visible = false;
                    }
                    break;
                case 22:
                    if (status)
                    {
                        checkBox_22.Visible = true;
                    }
                    else
                    {
                        checkBox_22.Visible = false;
                    }
                    break;
                case 23:
                    if (status)
                    {
                        checkBox_23.Visible = true;
                    }
                    else
                    {
                        checkBox_23.Visible = false;
                    }
                    break;
                case 24:
                    if (status)
                    {
                        checkBox_24.Visible = true;
                    }
                    else
                    {
                        checkBox_24.Visible = false;
                    }
                    break;
            }
        }

        private void EnableDispatchBench(int boxNumber)
        {
            switch (boxNumber)
            {
                case 1:
                    button_1.BackgroundImage = image_ic_light_red;
                    break;
                case 2:
                    button_2.BackgroundImage = image_ic_light_red;
                    break;
                case 3:
                    button_3.BackgroundImage = image_ic_light_red;
                    break;
                case 4:
                    button_4.BackgroundImage = image_ic_light_red;
                    break;
                case 5:
                    button_5.BackgroundImage = image_ic_light_red;
                    break;
                case 6:
                    button_6.BackgroundImage = image_ic_light_red;
                    break;
                case 7:
                    button_7.BackgroundImage = image_ic_light_red;
                    break;
                case 8:
                    button_8.BackgroundImage = image_ic_light_red;
                    break;
                case 9:
                    button_9.BackgroundImage = image_ic_light_red;
                    break;
                case 10:
                    button_10.BackgroundImage = image_ic_light_red;
                    break;
                case 11:
                    button_11.BackgroundImage = image_ic_light_red;
                    break;
                case 12:
                    button_12.BackgroundImage = image_ic_light_red;
                    break;
                case 13:
                    button_13.BackgroundImage = image_ic_light_red;
                    break;
                case 14:
                    button_14.BackgroundImage = image_ic_light_red;
                    break;
                case 15:
                    button_15.BackgroundImage = image_ic_light_red;
                    break;
                case 16:
                    button_16.BackgroundImage = image_ic_light_red;
                    break;
                case 17:
                    button_17.BackgroundImage = image_ic_light_red;
                    break;
                case 18:
                    button_18.BackgroundImage = image_ic_light_red;
                    break;
                case 19:
                    button_19.BackgroundImage = image_ic_light_red;
                    break;
                case 20:
                    button_20.BackgroundImage = image_ic_light_red;
                    break;
                case 21:
                    button_21.BackgroundImage = image_ic_light_red;
                    break;
                case 22:
                    button_22.BackgroundImage = image_ic_light_red;
                    break;
                case 23:
                    button_23.BackgroundImage = image_ic_light_red;
                    break;
                case 24:
                    button_24.BackgroundImage = image_ic_light_red;
                    break;
            }
            checkBoxEnable(boxNumber, true);
            
        }

        private void ShowBoxData(int boxNumber,String str)
        {
            switch (boxNumber)
            {
                case 1:
                    {
                        button_1.Text = str;
                        break;
                    }
                case 2:
                    {
                        button_2.Text = str;
                        break;
                    }
                case 3:
                    {
                        button_3.Text = str;
                        break;
                    }
                case 4:
                    {
                        button_4.Text = str;
                        break;
                    }
                case 5:
                    {
                        button_5.Text = str;
                        break;
                    }
                case 6:
                    {
                        button_6.Text = str;
                        break;
                    }
                case 7:
                    {
                        button_7.Text = str;
                        break;
                    }
                case 8:
                    {
                        button_8.Text = str;
                        break;
                    }
                case 9:
                    {
                        button_9.Text = str;
                        break;
                    }
                case 10:
                    {
                        button_10.Text = str;
                        break;
                    }
                case 11:
                    {
                        button_11.Text = str;
                        break;
                    }
                case 12:
                    {
                        button_12.Text = str;
                        break;
                    }
                case 13:
                    {
                        button_13.Text = str;
                        break;
                    }
                case 14:
                    {
                        button_14.Text = str;
                        break;
                    }
                case 15:
                    {
                        button_15.Text = str;
                        break;
                    }
                case 16:
                    {
                        button_16.Text = str;
                        break;
                    }
                case 17:
                    {
                        button_17.Text = str;
                        break;
                    }
                case 18:
                    {
                        button_18.Text = str;
                        break;
                    }
                case 19:
                    {
                        button_19.Text = str;
                        break;
                    }
                case 20:
                    {
                        button_20.Text = str;
                        break;
                    }
                case 21:
                    {
                        button_21.Text = str;
                        break;
                    }
                case 22:
                    {
                        button_22.Text = str;
                        break;
                    }
                case 23:
                    {
                        button_23.Text = str;
                        break;
                    }
                case 24:
                    {
                        button_24.Text = str;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        
        private void OrderNumberUpdated(object sender, EventArgs e)
        {
            resetAllButtons();
            label_current.Text = "0";
            EPCDictionary.Clear();
            SKU_Dictionary.Clear();
            pictureBoxLogo.BackgroundImage = image_ic_logo_pos;
            //labelShowMessage.Text = "";
            MainShowMessage("", 0);

            labelTotalEpcCount_i.Text = TotalEpcCount_i.ToString();
            labelTotalEpcCount_slash.Text = "/";
            labelTotalEpcCount.Text = TotalEpcCount.ToString();
            if (TotalEpcCount_i == TotalEpcCount)
            {
                labelTotalEpcCount_i.ForeColor = Color.Black;
            }
            else
            {
                labelTotalEpcCount_i.ForeColor = Color.Red;
            }
            label_totalnum.Text = "0";
            orderItemsInit();
            label_donenum.Text = FinishedOrderCounter.ToString();
            //int tempOrderId = 0;
            for (int unfinishedDistributionSortingListReturn_i = 0; unfinishedDistributionSortingListReturn_i < current_UnfinishedDistributionSortingListReturn.data.Length; unfinishedDistributionSortingListReturn_i++)
            {
                // get id;
                if(current_UnfinishedDistributionSortingListReturn.data[unfinishedDistributionSortingListReturn_i].sortingOrderNo.Equals(this.comboBox_order.Text))
                {
                    currentOrderInfo = new CurrentOrderInfo();
                    currentOrderInfo.Id = current_UnfinishedDistributionSortingListReturn.data[unfinishedDistributionSortingListReturn_i].id;
                    
                    currentOrderInfo.SortingOrderNo = current_UnfinishedDistributionSortingListReturn.data[unfinishedDistributionSortingListReturn_i].sortingOrderNo;
                    break;
                }
            }
            DistributionSortingItemsByOrderIdPara distributionSortingItemsByOrderIdPara=new DistributionSortingItemsByOrderIdPara();
            distributionSortingItemsByOrderIdPara.id = currentOrderInfo.Id;
            current_DistributionSortingItemsByOrderIdReturn = DistributionPlatform.GetDistributionSortingItemsByOrderId(distributionSortingItemsByOrderIdPara);
            // this.label_totalnum.Text = current_DistributionSortingItemsByOrderIdReturn.data.Length.ToString();
            // copy data from current_DistributionSortingItemsByOrderIdReturn to orderItems;
            if(current_DistributionSortingItemsByOrderIdReturn==null)
            {
                button_confirm.Visible = false;
                button_confirm.Enabled = false;
                buttonCancelTradeOrdersInDistributionSorting.Visible = true;
                buttonCancelTradeOrdersInDistributionSorting.Enabled = false;
                MainShowMessage("此分播为空。", 1);
                LogFile.WriteLog("此分播为空。");
                return;
            }
            if(current_DistributionSortingItemsByOrderIdReturn.data==null)
            {
                button_confirm.Visible = false;
                button_confirm.Enabled = false;
                buttonCancelTradeOrdersInDistributionSorting.Visible = true;
                buttonCancelTradeOrdersInDistributionSorting.Enabled = false;
                MainShowMessage("此分播数据为空。", 1);
                LogFile.WriteLog("此分播数据为空。");
                return;
            }
            orderItems_Counter = current_DistributionSortingItemsByOrderIdReturn.data.Length;
            if ((orderItems_Counter > 0)&&(orderItems_Counter<25))
            {
                this.label_totalnum.Text = current_DistributionSortingItemsByOrderIdReturn.data.Length.ToString();
                for (int orderItems_i = 0; orderItems_i < orderItems_Counter; orderItems_i++)
                {
                    orderItems[orderItems_i].id = current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].id;
                    orderItems[orderItems_i].tid= current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].tid;
                    orderItems[orderItems_i].tradeOrderNo = current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].tradeOrderNo;
                    orderItems[orderItems_i].position = orderItems_i+1;
                    orderItems[orderItems_i].tradeOrderItemTotal = current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].tradeOrderItems.Length;
                    orderItems[orderItems_i].tradeOrderItem = new OrderItem.TradeOrderItem[orderItems[orderItems_i].tradeOrderItemTotal];
                    for(int temp_i=0;temp_i< orderItems[orderItems_i].tradeOrderItem.Length; temp_i++)
                    {
                        orderItems[orderItems_i].tradeOrderItem[temp_i] = new OrderItem.TradeOrderItem();
                        orderItems[orderItems_i].tradeOrderItem[temp_i].skuNo = current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].tradeOrderItems[temp_i].skuNo;
                        orderItems[orderItems_i].tradeOrderItem[temp_i].skuName= current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].tradeOrderItems[temp_i].skuName;
                        orderItems[orderItems_i].tradeOrderItem[temp_i].orderId = current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].tradeOrderItems[temp_i].id;
                        orderItems[orderItems_i].tradeOrderItem[temp_i].epcStr_i = 0;
                        orderItems[orderItems_i].tradeOrderItem[temp_i].epcStrTotal = current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].tradeOrderItems[temp_i].count;
                        if (!SKU_Dictionary.ContainsKey(orderItems[orderItems_i].tradeOrderItem[temp_i].skuNo))
                        {
                            SKU_Dictionary.Add(orderItems[orderItems_i].tradeOrderItem[temp_i].skuNo, orderItems[orderItems_i].tradeOrderItem[temp_i].epcStrTotal);
                        }
                        else
                        {
                            int skuNoCounter = SKU_Dictionary[orderItems[orderItems_i].tradeOrderItem[temp_i].skuNo];
                            skuNoCounter+= orderItems[orderItems_i].tradeOrderItem[temp_i].epcStrTotal;
                            SKU_Dictionary[orderItems[orderItems_i].tradeOrderItem[temp_i].skuNo] = skuNoCounter;
                        }
                        TotalEpcCount += orderItems[orderItems_i].tradeOrderItem[temp_i].epcStrTotal;
                        labelTotalEpcCount_i.Text = TotalEpcCount_i.ToString();
                        labelTotalEpcCount_slash.Text = "/";
                        labelTotalEpcCount.Text = TotalEpcCount.ToString();
                        if(TotalEpcCount_i == TotalEpcCount)
                        {
                            labelTotalEpcCount_i.ForeColor = Color.Black;
                        }
                        else
                        {
                            labelTotalEpcCount_i.ForeColor = Color.Red;
                        }
                        orderItems[orderItems_i].epcsTotal += orderItems[orderItems_i].tradeOrderItem[temp_i].epcStrTotal;
                        orderItems[orderItems_i].tradeOrderItem[temp_i].epcStr = new String[orderItems[orderItems_i].tradeOrderItem[temp_i].epcStrTotal];
                        orderItems[orderItems_i].tradeOrderItem[temp_i].isFull = false;
                    }
                    //Array.Copy(current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].tradeOrderItems,0,orderItems[orderItems_i].tradeOrderItem,0, orderItems[orderItems_i].tradeOrderItem.Length);
                    orderItems[orderItems_i].epcs_i = 0;
                    orderItems[orderItems_i].isFull = false;
                    //updateItemStatus(orderItems_i + 1, false);
                    EnableDispatchBench(orderItems_i + 1);
                    //if (orderItems[orderItems_i].tid != null)
                    //{
                    //    ShowOrderClass(orderItems_i + 1, "T", Color.DarkOrange);
                    //}
                    //else
                    //{
                    //    ShowOrderClass(orderItems_i + 1, "W", Color.Green);
                    //}
                    if (orderItems[orderItems_i].tid != null)
                    {
                        switch (orderItems[orderItems_i].tid.Length)
                        {
                            case 27:// taobao;
                                {
                                    ShowOrderClass(orderItems_i + 1, "W", Color.Green);
                                    break;
                                }
                            case 18:
                                {
                                    ShowOrderClass(orderItems_i + 1, "T", Color.DarkOrange);
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }


                    ShowBoxData(orderItems_i + 1, orderItems[orderItems_i].epcs_i.ToString() + "/" + orderItems[orderItems_i].epcsTotal.ToString());
                }
                buttonTest.Enabled = true;
                button_confirm.Visible = false;
                button_confirm.Enabled = false;
                buttonCancelTradeOrdersInDistributionSorting.Visible = true;
                buttonCancelTradeOrdersInDistributionSorting.Enabled = true;
                //DispatchReadyStyle = true;
                switch (rfid_method)
                {
                    case 1:
                        {
                            break;
                        }
                    case 2:
                        {
                            yTJBluetooth.StopRead();
                            Thread.Sleep(100);
                            for(int i=0;i<5;i++)
                            {
                                if (yTJBluetooth.StartRead())
                                {
                                    break;
                                }
                            }
                            
                            break;
                        }
                    case 3:
                        {
                            if(ThreadInventoryStyle)
                            {
                                ThreadInventory.Abort();
                                ThreadInventoryStyle = false;
                            }
                            ThreadInventory = new Thread(ThreadInventoryFunc);
                            ThreadInventory.IsBackground = true;
                            ThreadInventoryStyle = true;
                            ThreadInventory.Start();
                            break;
                        }
                    case 4:
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                if (tcpClientTester.StartInventory(false) == UhfType.RETURN_VALUE.RETURN_OK)
                                {
                                    break;
                                }
                            }
                            break;
                        }
                    case 5:
                        {
                            break;
                        }
                    case 6:
                        {
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                
                
                //yTJBluetooth.StopRead();
            }
            else
            {
                // orderItems_Counter is out of range;
                buttonTest.Enabled = false;
                button_confirm.Visible = false;
                button_confirm.Enabled = false;
                buttonCancelTradeOrdersInDistributionSorting.Visible = true;
                buttonCancelTradeOrdersInDistributionSorting.Enabled = false;
                //labelShowMessage.Text = "此分播超过24个订单。";
                MainShowMessage("此分播超过24个订单。", 1);
                LogFile.WriteLog("此分播超过24个订单。");
                //DispatchReadyStyle = false;
            }
            Application.DoEvents();
            
        }

        

        private void button_confirm_Click(object sender, EventArgs e)
        {
            DistributionSortingPara distributionSortingPara = new DistributionSortingPara() ;
            distributionSortingPara.id = currentOrderInfo.Id;

            int valid_ItemList_Counter = 0;
            for (int temp_i = 0; temp_i < orderItems_Counter; temp_i++)
            {
                if (orderItems[temp_i].isCanceled)
                {
                    continue;
                }
                valid_ItemList_Counter++;
            }

            distributionSortingPara.itemList = new DistributionSortingPara.Itemlist[valid_ItemList_Counter];
            int ok_itemList_i = 0;
            for (int temp_i=0;temp_i< orderItems_Counter; temp_i++)
            {
                if(orderItems[temp_i].isCanceled)
                {
                    continue;
                }
                distributionSortingPara.itemList[ok_itemList_i] = new DistributionSortingPara.Itemlist();
                distributionSortingPara.itemList[ok_itemList_i].tradeOrderId = orderItems[temp_i].id;
                distributionSortingPara.itemList[ok_itemList_i].epcList = new String[orderItems[temp_i].epcsTotal];
                int epcList_i = 0;
                for (int tradeOrder_i=0;tradeOrder_i< orderItems[temp_i].tradeOrderItemTotal;tradeOrder_i++)
                {
                    for (int temp_j = 0; temp_j < orderItems[temp_i].tradeOrderItem[tradeOrder_i].epcStr_i; temp_j++)
                    {
                        distributionSortingPara.itemList[ok_itemList_i].epcList[epcList_i++] = orderItems[temp_i].tradeOrderItem[tradeOrder_i].epcStr[temp_j];
                    }
                }
                ok_itemList_i++;
            }
            
            if(ok_itemList_i<=0)
            {
                MainShowMessage("没有分播数据。", 0);
                LogFile.WriteLog("没有分播数据。");
                updateItemStatus(LedNumber, true);
                LedNumber = 0;
                buttonTest.Enabled = false;
                button_confirm.Visible = true;
                button_confirm.Enabled = false;
                buttonCancelTradeOrdersInDistributionSorting.Visible = false;
                buttonCancelTradeOrdersInDistributionSorting.Enabled = false;
                return;
            }

            DistributionSortingReturn result=DistributionPlatform.ConfirmDistributionSorting(distributionSortingPara);
            if(result!=null)
            {
                if(result.code.Equals("1"))
                {
                    // success;
                    //MessageBox.Show("播种成功！");
                    //labelShowMessage.Text = "播种成功！";
                    LogFile.WriteLog("Start");
                    for (int temp_i = 0; temp_i < orderItems_Counter; temp_i++)
                    {
                        //distributionSortingPara.itemList[temp_i] = new DistributionSortingPara.Itemlist();
                        //distributionSortingPara.itemList[temp_i].tradeOrderId = orderItems[temp_i].id;
                        LogFile.WriteLog(orderItems[temp_i].id.ToString());
                        //distributionSortingPara.itemList[temp_i].epcList = new String[orderItems[temp_i].epcsTotal];
                        //int epcList_i = 0;
                        for (int tradeOrder_i = 0; tradeOrder_i < orderItems[temp_i].tradeOrderItemTotal; tradeOrder_i++)
                        {
                            for (int temp_j = 0; temp_j < orderItems[temp_i].tradeOrderItem[tradeOrder_i].epcStr_i; temp_j++)
                            {
                                //distributionSortingPara.itemList[temp_i].epcList[epcList_i++] = orderItems[temp_i].tradeOrderItem[tradeOrder_i].epcStr[temp_j];
                                LogFile.WriteLog(orderItems[temp_i].tradeOrderItem[tradeOrder_i].epcStr[temp_j]);
                            }
                        }
                    }
                    LogFile.WriteLog("End");
                    MainShowMessage("播种完成！", 0);
                    LogFile.WriteLog("播种完成！");
                    updateItemStatus(LedNumber, true);
                    LedNumber = 0;
                    buttonTest.Enabled = false;
                    button_confirm.Visible = true;
                    button_confirm.Enabled = false;
                    buttonCancelTradeOrdersInDistributionSorting.Visible = false;
                    buttonCancelTradeOrdersInDistributionSorting.Enabled = false;
                    //DispatchReadyStyle = false;
                }
                else
                {
                    // fail;
                    //MessageBox.Show("播种失败！");
                    //labelShowMessage.Text = "播种失败！";
                    MainShowMessage("播种失败，"+ result.msg, 1);
                    LogFile.WriteLog("播种失败," + result.msg);
                    
                }
                //yTJBluetooth.StartRead();
                switch (rfid_method)
                {
                    case 1:
                        {
                            break;
                        }
                    case 2:
                        {
                            yTJBluetooth.StopRead();
                            break;
                        }
                    case 3:
                        {
                            ThreadInventory.Abort();
                            ThreadInventoryStyle = false;
                            break;
                        }
                    case 4:
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                if (tcpClientTester.StopInventory() == UhfType.RETURN_VALUE.RETURN_OK)
                                {
                                    break;
                                }
                            }
                            break;
                        }
                    case 5:
                        {
                            break;
                        }
                    case 6:
                        {
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            else
            {
                //MessageBox.Show("数据异常！");
                //labelShowMessage.Text = "数据异常！";
                MainShowMessage("数据异常！", 1);
                LogFile.WriteLog("数据异常！");
            }
            comboBox_order.Focus();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ThreadShowRealTimeStyle)
            {
                ThreadShowRealTimeStyle = false;
                ThreadShowRealTime.Abort();
            }
            if(ThreadLedMonitorStyle)
            {
                ThreadLedMonitorStyle = false;
                ThreadLedMonitor.Abort();
            }
            if (ioControlEnable)
            {
                tcpClient.CloseTcpClient();
            }

            if(ThreadReadEpcsDictionaryProcessStyle)
            {
                ThreadReadEpcsDictionaryProcessStyle = false;
                ThreadReadEpcsDictionaryProcess.Abort();
            }

            if (ThreadInventoryStyle)
            {
                ThreadInventory.Abort();
                ThreadInventoryStyle = false;
            }
            mR6100Api.TcpCloseConnect();

            if (tcpClientTester != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (tcpClientTester.StopInventory() == UhfType.RETURN_VALUE.RETURN_OK)
                    {
                        break;
                    }
                }
                tcpClientTester.CloseTcpClient();
                //ShowDebugInfo("断开连接成功。");
            }

            switch (rfid_method)
            {
                case 1:
                    {
                        timer_reader.Enabled = false;
                        timer_checkEPC.Enabled = false;
                        isConnected = false;
                        break;
                    }
                case 2:
                    {
                       if(ThreadBtReaderMonitorStyle)
                        {
                            ThreadBtReaderMonitor.Abort();
                        }

                        // stop read;
                        yTJBluetooth.StopRead();
                        // disconnect Bt;
                        yTJBluetooth.offBluetooth();
                        isReconnection = false;
                        break;
                    }
                case 3:
                    {
                        break;
                    }
                case 4:
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (tcpClientTester.StopInventory() == UhfType.RETURN_VALUE.RETURN_OK)
                            {
                                break;
                            }
                        }
                        break;
                    }
                case 5:
                    {
                        break;
                    }
                case 6:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            
            if(ThreadConnectBtRfidReaderStyle)
            {
                ThreadConnectBtRfidReader.Abort();
                ThreadConnectBtRfidReaderStyle = false;
            }

            Login_MainForm.Visible = true;
            Login_MainForm.Refresh();
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void serialPortIoControl_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            SerialPort tempComport = (SerialPort)sender;
            int tempLength = tempComport.BytesToRead;
            byte[] tempBuffer = new byte[tempLength];
            if (tempLength > 0)
            {
                serialPortIoControl.Read(tempBuffer, 0, tempLength);
                for (int temp_i = 0; temp_i < tempLength; temp_i++)
                {
                    IoControlSerialPortBuffer[IoControlSerialPortBuffer_Counter++] = tempBuffer[temp_i];
                }
                IoControlSerialPortReadTimeOutCounter = 0;
            }

        }

        private Boolean OpenComport(String comportNun)
        {
            Boolean tempState = false;
            if (IoControlSerialPortState == false)
            {
                //this.button2.Enabled = false;
                serialPortIoControl.BaudRate = 9600;
                serialPortIoControl.PortName = comportNun;
                serialPortIoControl.DataBits = 8;
                serialPortIoControl.StopBits = System.IO.Ports.StopBits.One;
                serialPortIoControl.Parity = System.IO.Ports.Parity.None;

                try
                {
                    serialPortIoControl.Open();
                }
                catch (Exception)
                {
                    return false;
                }
                serialPortIoControl.ReadTimeout = 500;
                if (serialPortIoControl.IsOpen == true)
                {
                    IoControlSerialPortState = true;
                    tempState = true;
                }
                else
                {
                    tempState = false;
                }
            }
            else
            {
                tempState = false;
            }
            return tempState;
        }

        private Boolean CloseComport()
        {
            Boolean tempState = false;
            if (IoControlSerialPortState)
            {
                if (serialPortIoControl.IsOpen == true)
                {
                    Thread.Sleep(100);
                    try
                    {
                        serialPortIoControl.Close();
                        IoControlSerialPortState = false;
                        tempState = true;
                    }
                    catch (Exception)
                    {
                        tempState = false;
                    }
                }
                else
                {
                    tempState = false;
                }
            }
            else
            {
                tempState = false;
            }
            return tempState;
        }

        private int controlDoPut(ushort ioNumber, bool onoff)
        {
            byte[] tempBuffer = new byte[8];
            IoControl.Do_Single ioContro_Do_Single = new IoControl.Do_Single();
            ioContro_Do_Single.moduleAddress = 0x01;
            ioContro_Do_Single.functionCode = 0x05;
            ioContro_Do_Single.startRegisterAddress = ioNumber;
            if (onoff)
            {
                // on;
                ioContro_Do_Single.writeData = 0xff00;
            }
            else
            {
                // off;
                ioContro_Do_Single.writeData = 0x0000;
            }
            tempBuffer[0] = ioContro_Do_Single.moduleAddress;
            tempBuffer[1] = ioContro_Do_Single.functionCode;
            tempBuffer[2] = (byte)(ioContro_Do_Single.startRegisterAddress >> 8);
            tempBuffer[3] = (byte)(ioContro_Do_Single.startRegisterAddress >> 0);
            tempBuffer[4] = (byte)(ioContro_Do_Single.writeData >> 8);
            tempBuffer[5] = (byte)(ioContro_Do_Single.writeData >> 0);
            ioContro_Do_Single.crcCheck = ModbusCrc16.CRC16(tempBuffer, (ushort)(tempBuffer.Length - 2));
            tempBuffer[6] = (byte)(ioContro_Do_Single.crcCheck >> 8);
            tempBuffer[7] = (byte)(ioContro_Do_Single.crcCheck >> 0);
            if (serialPortIoControl.IsOpen)
            {
                serialPortIoControl.Write(tempBuffer, 0, tempBuffer.Length);
                //Thread.Sleep(10);
                IoControlSerialPortReadTimeOutCounter = 0;
                IoControlSerialPortBuffer_Counter = 0;
                Boolean ioResultStyle = false;
                Boolean myTrue = true;
                while (myTrue)
                {
                    Thread.Sleep(2);
                    if (IoControlSerialPortReadTimeOutCounter++ > 100)
                    {
                        // time out;
                        ioResultStyle = false;
                        break;
                    }
                    // if get content is true;
                    if (IoControlSerialPortBuffer_Counter >= 8)
                    {
                        // check IoControlSerialPortBuffer content;
                        ioResultStyle = true;
                        break;
                    }
                }
                if (ioResultStyle)
                {
                    // success;
                }
                else
                {
                    MessageBox.Show("IO控制器超时");
                    LogFile.WriteLog("IO控制器超时");
                }
            }
            else
            {
                MessageBox.Show("请打开串口");
                LogFile.WriteLog("请打开串口");
            }
            return 0;
        }




        Boolean LedState = false;

        private void RfidActionProcess(String epcStr)
        {
            //labelShowMessage.Text = "";
            MainShowMessage("", 0);
            if (!EPCDictionary.ContainsKey(epcStr))
            {
                //SKUNoByEPCPara sKUNoByEPCPara = new SKUNoByEPCPara();
                //sKUNoByEPCPara.EpcList = new String[1];
                //sKUNoByEPCPara.EpcList[0] = epcStr;
                //SKUNoByEPCReturn sKUNoByEPCReturn = DistributionPlatform.GetSKUNoByEPC(sKUNoByEPCPara);
                //SKUNoByEPCAndStatusPara sKUNoByEPCAndStatusPara = new SKUNoByEPCAndStatusPara();
                //sKUNoByEPCAndStatusPara.epcs = new String[1];
                //sKUNoByEPCAndStatusPara.epcs[0] = epcStr;
                //sKUNoByEPCAndStatusPara.status = 1;
                //SKUNoByEPCAndStatusReturn sKUNoByEPCAndStatusReturn = DistributionPlatform.GetSKUNoByEPCAndStatus(sKUNoByEPCAndStatusPara);
                //textBoxShowDebug.AppendText("get start\r\n");
                FindEPCReturn findEPCReturn = DistributionPlatform.FindEPC(epcStr);
                //textBoxShowDebug.AppendText("get stop\r\n");
                //if (sKUNoByEPCReturn != null)
                if (findEPCReturn != null)
                {
                    //if ((sKUNoByEPCReturn.data.Length > 0) && (sKUNoByEPCReturn.code.Equals("1")))
                    if((findEPCReturn.data!=null)&&(findEPCReturn.code.Equals("1")))
                    {
                        String tempMsg = "";
                        if (!findEPCReturn.data.storeId.Equals("1"))
                        {
                            tempMsg = "非总仓标签";
                            LedNumber = 0;
                            label_current.Text = "";
                            MainShowMessage(tempMsg, 1);
                            LogFile.WriteLog(tempMsg);
                            ThreadVoicePlay = new Thread(ThreadVoicePlayFunc);
                            ThreadVoicePlay.IsBackground = true;
                            ThreadVoicePlay.Start("错误。");
                            return;
                        }
                        switch (findEPCReturn.data.epcStatus)
                        {
                            case "0":
                                {
                                    if(LedNumber>0)
                                    {
                                        if (orderItems[LedNumber - 1].isFull)
                                        {
                                            updateItemStatus(LedNumber, true);
                                            LightControllerItemStatus(LedNumber, true);
                                        }
                                        else
                                        {
                                            updateItemStatus(LedNumber, false);
                                            LightControllerItemStatus(LedNumber, false);
                                        }
                                    }
                                    
                                    tempMsg = "标签未入库";
                                    LedNumber = 0;
                                    label_current.Text = "";
                                    MainShowMessage(tempMsg, 1);
                                    LogFile.WriteLog(tempMsg);
                                    ThreadVoicePlay = new Thread(ThreadVoicePlayFunc);
                                    ThreadVoicePlay.IsBackground = true;
                                    ThreadVoicePlay.Start("错误。");
                                    break;
                                }
                            case "1":
                                {
                                    // get sku data;
                                    //String tempSku = sKUNoByEPCReturn.data[0];
                                    //textBoxShowDebug.AppendText("chech data start\r\n");
                                    String tempSku = findEPCReturn.data.sku;
                                    Boolean FindStyle = false;

                                    // checked style processing;
                                    for (int orderItems_i = 0; orderItems_i < orderItems_Counter; orderItems_i++)
                                    {
                                        if (!orderItems[orderItems_i].isChecked)
                                        {
                                            continue;
                                        }
                                        if (orderItems[orderItems_i].isFull)
                                        {
                                            continue;
                                        }
                                        for (int temp_i = 0; temp_i < orderItems[orderItems_i].tradeOrderItem.Length; temp_i++)
                                        {
                                            if (orderItems[orderItems_i].tradeOrderItem[temp_i].isFull)
                                            {
                                                continue;
                                            }
                                            if (orderItems[orderItems_i].tradeOrderItem[temp_i].skuNo.Equals(tempSku))
                                            {
                                                orderItems[orderItems_i].tradeOrderItem[temp_i].epcStr[orderItems[orderItems_i].tradeOrderItem[temp_i].epcStr_i] = epcStr;
                                                orderItems[orderItems_i].tradeOrderItem[temp_i].epcStr_i++;
                                                orderItems[orderItems_i].epcs_i++;
                                                TotalEpcCount_i++;
                                                labelTotalEpcCount_i.Text = TotalEpcCount_i.ToString();
                                                labelTotalEpcCount_slash.Text = "/";
                                                labelTotalEpcCount.Text = TotalEpcCount.ToString();
                                                if (TotalEpcCount_i == TotalEpcCount)
                                                {
                                                    labelTotalEpcCount_i.ForeColor = Color.Black;
                                                }
                                                else
                                                {
                                                    labelTotalEpcCount_i.ForeColor = Color.Red;
                                                }
                                                ShowBoxData(orderItems_i + 1, orderItems[orderItems_i].epcs_i.ToString() + "/" + orderItems[orderItems_i].epcsTotal);
                                                if (orderItems[orderItems_i].tradeOrderItem[temp_i].epcStr_i == orderItems[orderItems_i].tradeOrderItem[temp_i].epcStrTotal)
                                                {
                                                    orderItems[orderItems_i].tradeOrderItem[temp_i].isFull = true;
                                                }
                                                if (orderItems[orderItems_i].epcs_i == orderItems[orderItems_i].epcsTotal)
                                                {
                                                    //updateItemStatus(orderItems_i + 1, );
                                                    orderItems[orderItems_i].isFull = true;
                                                    FinishedOrderCounter++;
                                                    label_donenum.Text = FinishedOrderCounter.ToString();
                                                    if (orderItems_Counter == FinishedOrderCounter)
                                                    {
                                                        buttonTest.Enabled = false;
                                                        button_confirm.Visible = true;
                                                        button_confirm.Enabled = true;
                                                        buttonCancelTradeOrdersInDistributionSorting.Visible = false;
                                                        buttonCancelTradeOrdersInDistributionSorting.Enabled = false;
                                                    }
                                                }
                                                LedMonitorTimer = 10;
                                                int lastLedNumber = LedNumber;
                                                LedNumber = orderItems_i + 1;
                                                if (lastLedNumber == LedNumber)
                                                {

                                                }
                                                else
                                                {
                                                    if (lastLedNumber > 0)
                                                    {
                                                        if (orderItems[lastLedNumber - 1].isFull)
                                                        {
                                                            updateItemStatus(lastLedNumber, true);
                                                            LightControllerItemStatus(lastLedNumber, true);
                                                        }
                                                        else
                                                        {
                                                            updateItemStatus(lastLedNumber, false);
                                                            LightControllerItemStatus(lastLedNumber, false);
                                                        }
                                                    }
                                                }
                                                label_current.Text = (orderItems_i + 1).ToString();
                                                EPCDictionary.Add(epcStr, orderItems_i + 1);
                                                FindStyle = true;
                                                Application.DoEvents();


                                                break;
                                            }
                                        }
                                        if (FindStyle)
                                        {
                                            break;
                                        }
                                    }

                                    // no checked style processing;
                                    if (!FindStyle)
                                    {
                                        for (int orderItems_i = 0; orderItems_i < orderItems_Counter; orderItems_i++)
                                        {
                                            if (orderItems[orderItems_i].isFull)
                                            {
                                                continue;
                                            }
                                            for (int temp_i = 0; temp_i < orderItems[orderItems_i].tradeOrderItem.Length; temp_i++)
                                            {
                                                if (orderItems[orderItems_i].tradeOrderItem[temp_i].isFull)
                                                {
                                                    continue;
                                                }
                                                if (orderItems[orderItems_i].tradeOrderItem[temp_i].skuNo.Equals(tempSku))
                                                {
                                                    orderItems[orderItems_i].tradeOrderItem[temp_i].epcStr[orderItems[orderItems_i].tradeOrderItem[temp_i].epcStr_i] = epcStr;
                                                    orderItems[orderItems_i].tradeOrderItem[temp_i].epcStr_i++;
                                                    orderItems[orderItems_i].epcs_i++;
                                                    TotalEpcCount_i++;
                                                    labelTotalEpcCount_i.Text = TotalEpcCount_i.ToString();
                                                    labelTotalEpcCount_slash.Text = "/";
                                                    labelTotalEpcCount.Text = TotalEpcCount.ToString();
                                                    if (TotalEpcCount_i == TotalEpcCount)
                                                    {
                                                        labelTotalEpcCount_i.ForeColor = Color.Black;
                                                    }
                                                    else
                                                    {
                                                        labelTotalEpcCount_i.ForeColor = Color.Red;
                                                    }
                                                    ShowBoxData(orderItems_i + 1, orderItems[orderItems_i].epcs_i.ToString() + "/" + orderItems[orderItems_i].epcsTotal);
                                                    if (orderItems[orderItems_i].tradeOrderItem[temp_i].epcStr_i == orderItems[orderItems_i].tradeOrderItem[temp_i].epcStrTotal)
                                                    {
                                                        orderItems[orderItems_i].tradeOrderItem[temp_i].isFull = true;
                                                    }
                                                    if (orderItems[orderItems_i].epcs_i == orderItems[orderItems_i].epcsTotal)
                                                    {
                                                        //updateItemStatus(orderItems_i + 1, );
                                                        orderItems[orderItems_i].isFull = true;
                                                        FinishedOrderCounter++;
                                                        label_donenum.Text = FinishedOrderCounter.ToString();
                                                        if (orderItems_Counter == FinishedOrderCounter)
                                                        {
                                                            buttonTest.Enabled = false;
                                                            button_confirm.Visible = true;
                                                            button_confirm.Enabled = true;
                                                            buttonCancelTradeOrdersInDistributionSorting.Visible = false;
                                                            buttonCancelTradeOrdersInDistributionSorting.Enabled = false;
                                                        }
                                                    }
                                                    LedMonitorTimer = 10;
                                                    int lastLedNumber = LedNumber;
                                                    LedNumber = orderItems_i + 1;
                                                    if (lastLedNumber == LedNumber)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        if (lastLedNumber > 0)
                                                        {
                                                            if (orderItems[lastLedNumber - 1].isFull)
                                                            {
                                                                updateItemStatus(lastLedNumber, true);
                                                                LightControllerItemStatus(lastLedNumber, true);
                                                            }
                                                            else
                                                            {
                                                                updateItemStatus(lastLedNumber, false);
                                                                LightControllerItemStatus(lastLedNumber, false);
                                                            }
                                                        }
                                                    }
                                                    label_current.Text = (orderItems_i + 1).ToString();
                                                    EPCDictionary.Add(epcStr, orderItems_i + 1);
                                                    FindStyle = true;
                                                    Application.DoEvents();


                                                    break;
                                                }
                                            }
                                            if (FindStyle)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    
                                    //textBoxShowDebug.AppendText("chech data stop\r\n");
                                    if (FindStyle)
                                    {
                                        //voice.Speak(LedNumber.ToString() + "号");
                                        ThreadVoicePlay = new Thread(ThreadVoicePlayFunc);
                                        ThreadVoicePlay.IsBackground = true;
                                        ThreadVoicePlay.Start(" "+LedNumber.ToString() + "号");
                                    }
                                    else
                                    {
                                        if (LedNumber > 0)
                                        {
                                            if (orderItems[LedNumber - 1].isFull)
                                            {
                                                updateItemStatus(LedNumber, true);
                                                LightControllerItemStatus(LedNumber, true);
                                            }
                                            else
                                            {
                                                updateItemStatus(LedNumber, false);
                                                LightControllerItemStatus(LedNumber, false);
                                            }
                                        }
                                        LedNumber = 0;
                                        label_current.Text = "";
                                        MainShowMessage("没有找到该SKU号。", 1);
                                        LogFile.WriteLog("没有找到该SKU号。");
                                        ThreadVoicePlay = new Thread(ThreadVoicePlayFunc);
                                        ThreadVoicePlay.IsBackground = true;
                                        ThreadVoicePlay.Start("错误。");

                                    }
                                    break;
                                }
                            case "3":
                                {
                                    if (LedNumber > 0)
                                    {
                                        if (orderItems[LedNumber - 1].isFull)
                                        {
                                            updateItemStatus(LedNumber, true);
                                            LightControllerItemStatus(LedNumber, true);
                                        }
                                        else
                                        {
                                            updateItemStatus(LedNumber, false);
                                            LightControllerItemStatus(LedNumber, false);
                                        }
                                    }
                                    tempMsg = "确认复核";
                                    LedNumber = 0;
                                    label_current.Text = "";
                                    MainShowMessage(tempMsg, 1);
                                    LogFile.WriteLog(tempMsg);
                                    ThreadVoicePlay = new Thread(ThreadVoicePlayFunc);
                                    ThreadVoicePlay.IsBackground = true;
                                    ThreadVoicePlay.Start("错误。");
                                    break;
                                }
                            case "4":
                                {
                                    if (LedNumber > 0)
                                    {
                                        if (orderItems[LedNumber - 1].isFull)
                                        {
                                            updateItemStatus(LedNumber, true);
                                            LightControllerItemStatus(LedNumber, true);
                                        }
                                        else
                                        {
                                            updateItemStatus(LedNumber, false);
                                            LightControllerItemStatus(LedNumber, false);
                                        }
                                    }
                                    tempMsg = "标签已发货";
                                    LedNumber = 0;
                                    label_current.Text = "";
                                    MainShowMessage(tempMsg, 1);
                                    LogFile.WriteLog(tempMsg);
                                    ThreadVoicePlay = new Thread(ThreadVoicePlayFunc);
                                    ThreadVoicePlay.IsBackground = true;
                                    ThreadVoicePlay.Start("错误。");
                                    break;
                                }
                            default:
                                {
                                    
                                    break;
                                }
                                
                        }
                       
                    }
                    else
                    {
                        if (LedNumber > 0)
                        {
                            if (orderItems[LedNumber - 1].isFull)
                            {
                                updateItemStatus(LedNumber, true);
                                LightControllerItemStatus(LedNumber, true);
                            }
                            else
                            {
                                updateItemStatus(LedNumber, false);
                                LightControllerItemStatus(LedNumber, false);
                            }
                        }
                        LedNumber = 0;
                        label_current.Text = "";
                        MainShowMessage("无效数据。", 1);
                        LogFile.WriteLog("无效数据。");
                        ThreadVoicePlay = new Thread(ThreadVoicePlayFunc);
                        ThreadVoicePlay.IsBackground = true;
                        ThreadVoicePlay.Start("错误。");
                    }
                }
                else
                {
                    if (LedNumber > 0)
                    {
                        if (orderItems[LedNumber - 1].isFull)
                        {
                            updateItemStatus(LedNumber, true);
                            LightControllerItemStatus(LedNumber, true);
                        }
                        else
                        {
                            updateItemStatus(LedNumber, false);
                            LightControllerItemStatus(LedNumber, false);
                        }
                    }
                    LedNumber = 0;
                    label_current.Text = "";
                    MainShowMessage("数据获取失败。", 1);
                    LogFile.WriteLog("数据获取失败。");
                    ThreadVoicePlay = new Thread(ThreadVoicePlayFunc);
                    ThreadVoicePlay.IsBackground = true;
                    ThreadVoicePlay.Start("错误。");
                }
            }
            else
            {
                //labelShowMessage.Text = "标签已被分拣。";
                int lastLedNumber = LedNumber;
                LedNumber = EPCDictionary[epcStr];
                if (lastLedNumber == LedNumber)
                {

                }
                else
                {
                    if (lastLedNumber > 0)
                    {
                        if (orderItems[lastLedNumber - 1].isFull)
                        {
                            updateItemStatus(lastLedNumber, true);
                            LightControllerItemStatus(lastLedNumber, true);
                        }
                        else
                        {
                            updateItemStatus(lastLedNumber, false);
                            LightControllerItemStatus(lastLedNumber, false);
                        }
                    }
                }
                MainShowMessage("标签已被分拣， 分拣框号：" + EPCDictionary[epcStr].ToString(), 1);
                LogFile.WriteLog("标签已被分拣， 分拣框号："+ EPCDictionary[epcStr].ToString());
                label_current.Text = EPCDictionary[epcStr].ToString();
                Application.DoEvents();
                // voice.Speak("标签已被分 拣， 分 拣 匡 号：" + EPCDictionary[epcStr].ToString()+"号");
                ThreadVoicePlay = new Thread(ThreadVoicePlayFunc);
                ThreadVoicePlay.IsBackground = true;
                //ThreadVoicePlay.Start(" 标签已被分 拣， 分 拣 匡 号：" + EPCDictionary[epcStr].ToString() + "号");
                ThreadVoicePlay.Start(" " + EPCDictionary[epcStr].ToString() + "号");

            }
            Application.DoEvents();
        }


        private Thread ThreadVoicePlay;
        private Boolean ThreadVoicePlay_Running = false;
        private void ThreadVoicePlayFunc(object content)
        {
            
            if(ThreadVoicePlay_Running)
            {
                return;
            }
            
            try
            {
                ThreadVoicePlay_Running = true;
                voice.Speak(content.ToString());
                ReadEpcsDictionary.Clear();
                ThreadReadEpcsDictionaryProcessStyle = false;
                ThreadVoicePlay_Running = false;
            }
            catch(Exception ex)
            {
                LogFile.WriteLog("voicePlay exception.");
            }
            
        }



        int controllerCounter = 0;
        
        private void buttonTest_Click(object sender, EventArgs e)
        {
            //ThreadVoicePlay = new Thread(ThreadVoicePlayFunc);
            //ThreadVoicePlay.IsBackground = true;
            //ThreadVoicePlay.Start(" 您好， 这里是测试程序， 测试中。");
            //OpenComport(ioControlPort);
            //if(LedState)
            //{
            //    LedState = false;
            //}
            //else
            //{
            //    LedState = true;
            //}
            //controlDoPut(0, LedState);
            ////controlDoPut(0, true);
            //CloseComport();


            //
            //String tempEpc = "";
            String sEPC = this.textBoxTest.Text;
            RfidActionProcess(sEPC);



            //yTJBluetooth.bluetooth += FindBluetooth; //定义委托
            //yTJBluetooth.findBluetooth();//调用查找

            //yTJBluetooth.connBluetooth += ConnBluetooth;//定义委托
            //yTJBluetooth.connectBluetooth(BtAddress);//调用连接


            //int controllerReturn = 0;
            //int[] returnValue = new int[1];
            //tcpClientTimeout = 0;
            //if (controllerCounter++%2==0)
            //{
            //    //controllerReturn = tcpClient.WriteDoSingleSwitchOut(0x01, 0x00, 0xff00, returnValue);
            //    controllerReturn = tcpClient.WriteDoMultiSwitchOut(0x01, 0x0000, 0x20, 0x00000000, returnValue);
            //}
            //else
            //{
            //    //controllerReturn = tcpClient.WriteDoSingleSwitchOut(0x01, 0x00, 0x0000, returnValue);
            //    controllerReturn = tcpClient.WriteDoMultiSwitchOut(0x01, 0x0000, 0x20, 0xffffffff, returnValue);
            //}
            //if (controllerReturn == 0)
            //{
            //    // success;
            //}
            //else
            //{
            //    // fail;
            //}
            //tcpClient.CloseTcpClient();
        }


        int combobox_order_dropdown_timer = 0;
        private void comboBox_order_DropDown(object sender, EventArgs e)
        {
            if(combobox_order_dropdown_timer<5)
            {
                // operating too frequently;
                MainShowMessage("不能频繁请求。", 1);
                LogFile.WriteLog("不能频繁请求。");
                ThreadVoicePlay = new Thread(ThreadVoicePlayFunc);
                ThreadVoicePlay.IsBackground = true;
                ThreadVoicePlay.Start("错误。");
                return;
            }
            MainShowMessage("", 0);
            combobox_order_dropdown_timer = 0;
            comboBox_order.Items.Clear();
            current_UnfinishedDistributionSortingListReturn = DistributionPlatform.GetUnfinishedDistributionSortingList();
            if (current_UnfinishedDistributionSortingListReturn != null)
            {
                if (current_UnfinishedDistributionSortingListReturn.data == null)// code is 1;
                {
                    MainShowMessage("没有分播数据。", 1);
                    LogFile.WriteLog("没有分播数据。");
                    ThreadVoicePlay = new Thread(ThreadVoicePlayFunc);
                    ThreadVoicePlay.IsBackground = true;
                    ThreadVoicePlay.Start("错误。");
                    return;
                }
                for (int unfinishedDistributionSortingListReturn_i = 0; unfinishedDistributionSortingListReturn_i < current_UnfinishedDistributionSortingListReturn.data.Length; unfinishedDistributionSortingListReturn_i++)
                {
                    if(filter_enable)
                    {
                        // use contains on indexof function to search data, if want to know position of the data must be using indexof function;
                        if(current_UnfinishedDistributionSortingListReturn.data[unfinishedDistributionSortingListReturn_i].sortingOrderNo.Contains(filter_data))
                        {
                            this.comboBox_order.Items.Add(current_UnfinishedDistributionSortingListReturn.data[unfinishedDistributionSortingListReturn_i].sortingOrderNo);
                        }
                    }
                    else
                    {
                        this.comboBox_order.Items.Add(current_UnfinishedDistributionSortingListReturn.data[unfinishedDistributionSortingListReturn_i].sortingOrderNo);
                    }
                }
            }
            else
            {
                // err;
                //labelShowMessage.Text = "分播异常。";
                MainShowMessage("当前没有分播单。", 1);
                LogFile.WriteLog("当前没有分播单。");
                ThreadVoicePlay = new Thread(ThreadVoicePlayFunc);
                ThreadVoicePlay.IsBackground = true;
                ThreadVoicePlay.Start("错误。");
            }
        }

        private Boolean ShowOrderItemContent(OrderItem orderItem)
        {
            //String tempStr = "";
            //if(orderItem!=null)
            //{
            //    if(orderItem.tradeOrderItem!=null)
            //    {
            //        //tempStr = JsonHelper.SerializeObject(orderItem);
            //        tempStr += "当前状态：";
            //        if(orderItem.isFull)
            //        {
            //            tempStr += "已满\r\n";
            //        }
            //        else
            //        {
            //            tempStr += "未满\r\n";
            //        }
            //        for(int temp_i=0;temp_i< orderItem.tradeOrderItem.Length;temp_i++)
            //        {
            //            tempStr += "SKU:"+ orderItem.tradeOrderItem[temp_i].skuNo+",描述:"+ orderItem.tradeOrderItem[temp_i].skuName+ ", 共有："+ orderItem.tradeOrderItem[temp_i].epcStrTotal.ToString()+", 实际："+orderItem.tradeOrderItem[temp_i].epcStr_i + "\r\n";
            //            for(int temp_j=0;temp_j< orderItem.tradeOrderItem[temp_i].epcStr_i;temp_j++)
            //            {
            //                tempStr += "  EPC:" + orderItem.tradeOrderItem[temp_i].epcStr[temp_j] +"\r\n";
            //            }

            //        }

            //    }
            //}
            //return tempStr;

            //if (orderItem != null)
            //{
            //    if (orderItem.isFull)
            //    {

            //    }
            //}

            return false;
        }

        private void  ButtonListViewProcess(OrderItem tempOrderItem)
        {
            ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            
            if (tempOrderItem != null)
            {
                if (tempOrderItem.isFull)
                {
                    buttonDialogContentShow.radioButtonIsFull.Checked = true;
                }
                else
                {
                    buttonDialogContentShow.radioButtonIsNotFull.Checked = true;
                }
                buttonDialogContentShow.Text = tempOrderItem.position.ToString()+"号分播框";
                buttonDialogContentShow.labelCurrentOrderQuantity.Text = tempOrderItem.epcsTotal.ToString();
                if(tempOrderItem.tid!=null)
                {
                    buttonDialogContentShow.labelOrderNumber.Text = tempOrderItem.tid;
                }
                else
                {
                    buttonDialogContentShow.labelOrderNumber.Text = tempOrderItem.tradeOrderNo;
                }
                int listViewShowIndexCounter = 0;
                for (int temp_i = 0; temp_i < tempOrderItem.tradeOrderItemTotal; temp_i++)
                {
                    for (int temp_j = 0; temp_j < tempOrderItem.tradeOrderItem[temp_i].epcStrTotal; temp_j++)
                    {
                        buttonDialogContentShow.listViewShowInfo.Items.Add((listViewShowIndexCounter + 1).ToString());
                        buttonDialogContentShow.listViewShowInfo.Items[listViewShowIndexCounter].SubItems.Add(tempOrderItem.tradeOrderItem[temp_i].skuNo);
                        buttonDialogContentShow.listViewShowInfo.Items[listViewShowIndexCounter].SubItems.Add(tempOrderItem.tradeOrderItem[temp_i].skuName);
                        if (tempOrderItem.tradeOrderItem[temp_i].epcStr[temp_j] != null)
                        {
                            buttonDialogContentShow.listViewShowInfo.Items[listViewShowIndexCounter].SubItems.Add(tempOrderItem.tradeOrderItem[temp_i].epcStr[temp_j]);
                            buttonDialogContentShow.listViewShowInfo.Items[listViewShowIndexCounter].SubItems.Add("已分播");
                            buttonDialogContentShow.listViewShowInfo.Items[listViewShowIndexCounter].BackColor = Color.FromArgb(0, 152, 251, 152); 
                        }
                        else
                        {
                            buttonDialogContentShow.listViewShowInfo.Items[listViewShowIndexCounter].SubItems.Add("");
                            buttonDialogContentShow.listViewShowInfo.Items[listViewShowIndexCounter].SubItems.Add("未分播");
                            buttonDialogContentShow.listViewShowInfo.Items[listViewShowIndexCounter].BackColor = Color.FromArgb(0, 250, 235, 215);
                        }
                        listViewShowIndexCounter++;
                    }
                }

                buttonDialogContentShow.ShowDialog();
            }
        }

        private void button_1_Click(object sender, EventArgs e)
        {
            int myPosition = 1;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //OrderItem tempOrderItem;
            //tempOrderItem = orderItems[myPosition - 1];
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition-1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}


        }

        private void button_2_Click(object sender, EventArgs e)
        {
            int myPosition = 2;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}

        }

        private void button_3_Click(object sender, EventArgs e)
        {
            int myPosition = 3;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_4_Click(object sender, EventArgs e)
        {
            int myPosition = 4;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_5_Click(object sender, EventArgs e)
        {
            int myPosition = 5;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_6_Click(object sender, EventArgs e)
        {
            int myPosition = 6;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_7_Click(object sender, EventArgs e)
        {
            int myPosition = 7;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_8_Click(object sender, EventArgs e)
        {
            int myPosition = 8;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_9_Click(object sender, EventArgs e)
        {
            int myPosition = 9;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_10_Click(object sender, EventArgs e)
        {
            int myPosition = 10;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_11_Click(object sender, EventArgs e)
        {
            int myPosition = 11;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_12_Click(object sender, EventArgs e)
        {
            int myPosition = 12;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_13_Click(object sender, EventArgs e)
        {
            int myPosition = 13;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_14_Click(object sender, EventArgs e)
        {
            int myPosition = 14;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_15_Click(object sender, EventArgs e)
        {
            int myPosition = 15;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_16_Click(object sender, EventArgs e)
        {
            int myPosition = 16;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_17_Click(object sender, EventArgs e)
        {
            int myPosition = 17;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_18_Click(object sender, EventArgs e)
        {
            int myPosition = 18;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_19_Click(object sender, EventArgs e)
        {
            int myPosition = 19;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_20_Click(object sender, EventArgs e)
        {
            int myPosition = 20;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_21_Click(object sender, EventArgs e)
        {
            int myPosition = 21;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_22_Click(object sender, EventArgs e)
        {
            int myPosition = 22;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_23_Click(object sender, EventArgs e)
        {
            int myPosition = 23;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }

        private void button_24_Click(object sender, EventArgs e)
        {
            int myPosition = 24;
            ButtonListViewProcess(orderItems[myPosition - 1]);
            //ButtonDialogContentShow buttonDialogContentShow = new ButtonDialogContentShow();
            //buttonDialogContentShow.textBoxShowContent.Text = ShowOrderItemContent(orderItems[myPosition - 1]);
            //if (buttonDialogContentShow.textBoxShowContent.Text.Length > 0)
            //{
            //    buttonDialogContentShow.ShowDialog();
            //}
        }


        private void GetComBox_orer_TextDataFunc()
        {
            int timerCounter = 0;
            while (ThreadGetComBox_orer_TextDataStyle)
            {
                Thread.Sleep(10);
                if (timerCounter++ > 20)
                {
                    ThreadGetComBox_orer_TextDataStyle = false;// break condition;
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        //labelShowMessage.Text = Temp_comboBox_order_Text + comboBox_order.Text;
                        String tempSwapStr= Temp_comboBox_order_Text + comboBox_order.Text;
                        comboBox_order.Text = tempSwapStr;



                        current_UnfinishedDistributionSortingListReturn = DistributionPlatform.GetUnfinishedDistributionSortingList();
                        if (current_UnfinishedDistributionSortingListReturn != null)
                        {
                            for (int unfinishedDistributionSortingListReturn_i = 0; unfinishedDistributionSortingListReturn_i < current_UnfinishedDistributionSortingListReturn.data.Length; unfinishedDistributionSortingListReturn_i++)
                            {
                                //this.comboBox_order.Items.Add(current_UnfinishedDistributionSortingListReturn.data[unfinishedDistributionSortingListReturn_i].sortingOrderNo);
                                if (current_UnfinishedDistributionSortingListReturn.data[unfinishedDistributionSortingListReturn_i].sortingOrderNo.Equals(comboBox_order.Text))
                                {
                                    // reset all;
                                    resetAllButtons();
                                    label_current.Text = "0";
                                    EPCDictionary.Clear();
                                    SKU_Dictionary.Clear();
                                    pictureBoxLogo.BackgroundImage = image_ic_logo_pos;
                                    //labelShowMessage.Text = "";
                                    MainShowMessage("", 0);
                                    //label_donenum.Text = "0";
                                    orderItemsInit();
                                    label_donenum.Text = FinishedOrderCounter.ToString();
                                    //int tempOrderId = 0;
                                    for (int unfinishedDistributionSortingListReturn_ii = 0; unfinishedDistributionSortingListReturn_ii < current_UnfinishedDistributionSortingListReturn.data.Length; unfinishedDistributionSortingListReturn_ii++)
                                    {
                                        // get id;
                                        if (current_UnfinishedDistributionSortingListReturn.data[unfinishedDistributionSortingListReturn_ii].sortingOrderNo.Equals(this.comboBox_order.Text))
                                        {
                                            currentOrderInfo = new CurrentOrderInfo();
                                            currentOrderInfo.Id = current_UnfinishedDistributionSortingListReturn.data[unfinishedDistributionSortingListReturn_ii].id;

                                            currentOrderInfo.SortingOrderNo = current_UnfinishedDistributionSortingListReturn.data[unfinishedDistributionSortingListReturn_ii].sortingOrderNo;
                                            break;
                                        }
                                    }
                                    DistributionSortingItemsByOrderIdPara distributionSortingItemsByOrderIdPara = new DistributionSortingItemsByOrderIdPara();
                                    distributionSortingItemsByOrderIdPara.id = currentOrderInfo.Id;
                                    current_DistributionSortingItemsByOrderIdReturn = DistributionPlatform.GetDistributionSortingItemsByOrderId(distributionSortingItemsByOrderIdPara);
                                    // this.label_totalnum.Text = current_DistributionSortingItemsByOrderIdReturn.data.Length.ToString();
                                    // copy data from current_DistributionSortingItemsByOrderIdReturn to orderItems;

                                    orderItems_Counter = current_DistributionSortingItemsByOrderIdReturn.data.Length;
                                    if ((orderItems_Counter > 0) && (orderItems_Counter < 25))
                                    {
                                        this.label_totalnum.Text = current_DistributionSortingItemsByOrderIdReturn.data.Length.ToString();
                                        for (int orderItems_i = 0; orderItems_i < orderItems_Counter; orderItems_i++)
                                        {
                                            orderItems[orderItems_i].id = current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].id;
                                            orderItems[orderItems_i].tid = current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].tid;
                                            orderItems[orderItems_i].tradeOrderNo = current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].tradeOrderNo;
                                            
                                            orderItems[orderItems_i].tradeOrderItemTotal = current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].tradeOrderItems.Length;
                                            orderItems[orderItems_i].tradeOrderItem = new OrderItem.TradeOrderItem[orderItems[orderItems_i].tradeOrderItemTotal];
                                            for (int temp_i = 0; temp_i < orderItems[orderItems_i].tradeOrderItem.Length; temp_i++)
                                            {
                                                orderItems[orderItems_i].tradeOrderItem[temp_i] = new OrderItem.TradeOrderItem();
                                                orderItems[orderItems_i].tradeOrderItem[temp_i].skuNo = current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].tradeOrderItems[temp_i].skuNo;
                                                orderItems[orderItems_i].tradeOrderItem[temp_i].skuName = current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].tradeOrderItems[temp_i].skuName;
                                                orderItems[orderItems_i].tradeOrderItem[temp_i].orderId = current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].tradeOrderItems[temp_i].id;
                                                orderItems[orderItems_i].tradeOrderItem[temp_i].epcStr_i = 0;
                                                orderItems[orderItems_i].tradeOrderItem[temp_i].epcStrTotal = current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].tradeOrderItems[temp_i].count;
                                                if (!SKU_Dictionary.ContainsKey(orderItems[orderItems_i].tradeOrderItem[temp_i].skuNo))
                                                {
                                                    SKU_Dictionary.Add(orderItems[orderItems_i].tradeOrderItem[temp_i].skuNo, orderItems[orderItems_i].tradeOrderItem[temp_i].epcStrTotal);
                                                }
                                                else
                                                {
                                                    int skuNoCounter = SKU_Dictionary[orderItems[orderItems_i].tradeOrderItem[temp_i].skuNo];
                                                    skuNoCounter += orderItems[orderItems_i].tradeOrderItem[temp_i].epcStrTotal;
                                                    SKU_Dictionary[orderItems[orderItems_i].tradeOrderItem[temp_i].skuNo] = skuNoCounter;
                                                }
                                                TotalEpcCount += orderItems[orderItems_i].tradeOrderItem[temp_i].epcStrTotal;
                                                labelTotalEpcCount_i.Text = TotalEpcCount_i.ToString();
                                                labelTotalEpcCount_slash.Text = "/";
                                                labelTotalEpcCount.Text = TotalEpcCount.ToString();
                                                if (TotalEpcCount_i == TotalEpcCount)
                                                {
                                                    labelTotalEpcCount_i.ForeColor = Color.Red;
                                                }
                                                else
                                                {
                                                    labelTotalEpcCount_i.ForeColor = Color.Black;
                                                }
                                                orderItems[orderItems_i].epcsTotal += orderItems[orderItems_i].tradeOrderItem[temp_i].epcStrTotal;
                                                orderItems[orderItems_i].tradeOrderItem[temp_i].epcStr = new String[orderItems[orderItems_i].tradeOrderItem[temp_i].epcStrTotal];
                                                orderItems[orderItems_i].tradeOrderItem[temp_i].isFull = false;
                                            }
                                            //Array.Copy(current_DistributionSortingItemsByOrderIdReturn.data[orderItems_i].tradeOrderItems,0,orderItems[orderItems_i].tradeOrderItem,0, orderItems[orderItems_i].tradeOrderItem.Length);
                                            orderItems[orderItems_i].epcs_i = 0;
                                            orderItems[orderItems_i].isFull = false;
                                            //updateItemStatus(orderItems_i + 1, false);
                                            EnableDispatchBench(orderItems_i + 1);
                                            //if (orderItems[orderItems_i].tid != null)
                                            //{
                                            //    ShowOrderClass(orderItems_i + 1, "T", Color.DarkOrange);
                                            //}
                                            //else
                                            //{
                                            //    ShowOrderClass(orderItems_i + 1, "W", Color.Green);
                                            //}
                                            if (orderItems[orderItems_i].tid != null)
                                            {
                                                switch (orderItems[orderItems_i].tid.Length)
                                                {
                                                    case 27:// taobao;
                                                        {
                                                            ShowOrderClass(orderItems_i + 1, "W", Color.Green);
                                                            break;
                                                        }
                                                    case 18:
                                                        {
                                                            ShowOrderClass(orderItems_i + 1, "T", Color.DarkOrange);
                                                            break;
                                                        }
                                                    default:
                                                        {
                                                            break;
                                                        }
                                                }
                                            }

                                            ShowBoxData(orderItems_i + 1, orderItems[orderItems_i].epcs_i.ToString() + "/" + orderItems[orderItems_i].epcsTotal.ToString());
                                        }
                                        buttonTest.Enabled = true;
                                        button_confirm.Visible = false;
                                        button_confirm.Enabled = false;
                                        buttonCancelTradeOrdersInDistributionSorting.Visible = true;
                                        buttonCancelTradeOrdersInDistributionSorting.Enabled = true;


                                        switch (rfid_method)
                                        {
                                            case 1:
                                                {
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    yTJBluetooth.StopRead();
                                                    Thread.Sleep(100);
                                                    for (int i = 0; i < 5; i++)
                                                    {
                                                        if (yTJBluetooth.StartRead())
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            case 3:
                                                {
                                                    if (ThreadInventoryStyle)
                                                    {
                                                        ThreadInventory.Abort();
                                                        ThreadInventoryStyle = false;
                                                    }
                                                    ThreadInventory = new Thread(ThreadInventoryFunc);
                                                    ThreadInventory.IsBackground = true;
                                                    ThreadInventoryStyle = true;
                                                    ThreadInventory.Start();
                                                    break;
                                                }
                                            case 4:
                                                {
                                                    for(int i=0;i<5;i++)
                                                    {
                                                        if (tcpClientTester.StartInventory(false) == UhfType.RETURN_VALUE.RETURN_OK)
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            case 5:
                                                {
                                                    break;
                                                }
                                            case 6:
                                                {
                                                    break;
                                                }
                                            default:
                                                {
                                                    break;
                                                }
                                        }
                                    }
                                    else
                                    {
                                        // orderItems_Counter is out of range;
                                        buttonTest.Enabled = false;
                                        button_confirm.Visible = false;
                                        button_confirm.Enabled = false;
                                        buttonCancelTradeOrdersInDistributionSorting.Visible = true;
                                        buttonCancelTradeOrdersInDistributionSorting.Enabled = false;
                                        //labelShowMessage.Text = "此分播超过24个订单。";
                                        MainShowMessage("此分播超过24个订单。", 1);
                                        LogFile.WriteLog("此分播超过24个订单。");
                                    }

                                    break;
                                }
                                else
                                {
                                    //labelShowMessage.Text = "没有发现分播号。";
                                    resetAllButtons();
                                    label_current.Text = "0";
                                    EPCDictionary.Clear();
                                    SKU_Dictionary.Clear();
                                    pictureBoxLogo.BackgroundImage = image_ic_logo_pos;
                                    //labelShowMessage.Text = "";
                                    MainShowMessage("", 0);
                                    //label_donenum.Text = "0";
                                    orderItemsInit();
                                    label_donenum.Text = FinishedOrderCounter.ToString();
                                    MainShowMessage("没有发现分播号。", 1);
                                    LogFile.WriteLog("没有发现分播号。");
                                }
                            }
                        }
                        else
                        {
                            // err;
                            //labelShowMessage.Text = "分播异常。";
                            resetAllButtons();
                            label_current.Text = "0";
                            EPCDictionary.Clear();
                            SKU_Dictionary.Clear();
                            pictureBoxLogo.BackgroundImage = image_ic_logo_pos;
                            //labelShowMessage.Text = "";
                            MainShowMessage("", 0);
                            //label_donenum.Text = "0";
                            orderItemsInit();
                            label_donenum.Text = FinishedOrderCounter.ToString();
                            MainShowMessage("分播异常。", 1);
                            LogFile.WriteLog("分播异常。");
                        }

                    }));
                    break;
                }
            }
        }
        Thread ThreadGetComBox_orer_TextData;
        Boolean ThreadGetComBox_orer_TextDataStyle;
        String Temp_comboBox_order_Text;
        private void comboBox_order_TextUpdate(object sender, EventArgs e)
        {
            if (comboBox_order.Text.Length == 0)
            {
                return;
            }
            // add code for get Enter charactor from combobox_order;
            //String tempStr=comboBox_order.Text;
            //labelShowMessage.Text = tempStr; 
            if (!ThreadGetComBox_orer_TextDataStyle)
            {
                buttonTest.Enabled = false;
                button_confirm.Visible = false;
                button_confirm.Enabled = false;
                buttonCancelTradeOrdersInDistributionSorting.Visible = true;
                buttonCancelTradeOrdersInDistributionSorting.Enabled = true;
                //DispatchReadyStyle = false;
                Temp_comboBox_order_Text = comboBox_order.Text;
                //textBoxShowDebug.AppendText(Temp_comboBox_order_Text + "  1\r\n");
                Temp_comboBox_order_Text = Temp_comboBox_order_Text.Substring(0, 1);
                //textBoxShowDebug.AppendText(Temp_comboBox_order_Text + "  2\r\n");
                comboBox_order.Text = "";
                ThreadGetComBox_orer_TextData = new Thread(GetComBox_orer_TextDataFunc);
                ThreadGetComBox_orer_TextDataStyle = true;
                ThreadGetComBox_orer_TextData.IsBackground = true;
                ThreadGetComBox_orer_TextData.Start();
            }
            
        }


        private void MainShowMessage(String msg, int type)
        {
            labelShowMessage.Text = msg;
            switch (type)
            {
                case 0:// message;
                    {
                        labelShowMessage.ForeColor = Color.Black;
                        break;
                    }
                case 1:// alarm;
                    {
                        labelShowMessage.ForeColor = Color.Red;
                        break;
                    }
            }
        }

        private void labelSystemTimer_DoubleClick(object sender, EventArgs e)
        {
            int skuItemCounter = 0;
            int epcItemCounter = 0;
            String tempStr = "";
            foreach (string key in SKU_Dictionary.Keys)
            {
                //Console.WriteLine("Key = {0}", key);
                epcItemCounter += SKU_Dictionary[key];
                tempStr += "SKU:"+key + ", COUNT:" + SKU_Dictionary[key].ToString() + "\r\n";
                skuItemCounter++;
            }

            //MessageBox.Show(tempStr);
            if(skuItemCounter > 0)
            {
                tempStr += "\r\nSKUs:"+skuItemCounter.ToString() + ", EPCs:"+ epcItemCounter.ToString() + "\r\n";
                DebugMessage debugMessage = new DebugMessage();
                debugMessage.textBoxShowMessage.Text = tempStr;
                debugMessage.ShowDialog();
            }
        }

        private void checkBox_1_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[0].isChecked = checkBox_1.Checked;
        }

        private void checkBox_2_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[1].isChecked = checkBox_2.Checked;
        }

        private void checkBox_3_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[2].isChecked = checkBox_3.Checked;
        }

        private void checkBox_4_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[3].isChecked = checkBox_4.Checked;
        }

        private void checkBox_5_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[4].isChecked = checkBox_5.Checked;
        }

        private void checkBox_6_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[5].isChecked = checkBox_6.Checked;
        }

        private void checkBox_7_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[6].isChecked = checkBox_7.Checked;
        }

        private void checkBox_8_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[7].isChecked = checkBox_8.Checked;
        }

        private void checkBox_9_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[8].isChecked = checkBox_9.Checked;
        }

        private void checkBox_10_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[9].isChecked = checkBox_10.Checked;
        }

        private void checkBox_11_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[10].isChecked = checkBox_11.Checked;
        }

        private void checkBox_12_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[11].isChecked = checkBox_12.Checked;
        }

        private void checkBox_13_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[12].isChecked = checkBox_13.Checked;
        }

        private void checkBox_14_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[13].isChecked = checkBox_14.Checked;
        }

        private void checkBox_15_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[14].isChecked = checkBox_15.Checked;
        }

        private void checkBox_16_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[15].isChecked = checkBox_16.Checked;
        }

        private void checkBox_17_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[16].isChecked = checkBox_17.Checked;
        }

        private void checkBox_18_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[17].isChecked = checkBox_18.Checked;
        }

        private void checkBox_19_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[18].isChecked = checkBox_19.Checked;
        }

        private void checkBox_20_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[19].isChecked = checkBox_20.Checked;
        }

        private void checkBox_21_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[20].isChecked = checkBox_21.Checked;
        }

        private void checkBox_22_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[21].isChecked = checkBox_22.Checked;
        }

        private void checkBox_23_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[22].isChecked = checkBox_23.Checked;
        }

        private void checkBox_24_CheckedChanged(object sender, EventArgs e)
        {
            orderItems[23].isChecked = checkBox_24.Checked;
        }

        private void buttonCancelTradeOrdersInDistributionSorting_Click(object sender, EventArgs e)
        {
            if (orderItems_Counter<=0)
            {
                return;
            }
            TradeOrdersInDistributionSortingPara tradeOrdersInDistributionSortingPara = new TradeOrdersInDistributionSortingPara();
            tradeOrdersInDistributionSortingPara.id = currentOrderInfo.Id;

            int cancel_orderIdList_Counter = 0;
            for (int temp_i = 0; temp_i < orderItems_Counter; temp_i++)
            {
                if (orderItems[temp_i].isFull)
                {
                    continue;
                }
                cancel_orderIdList_Counter++;
            }
            List<CancelItemContent> cancelItemList = new List<CancelItemContent>();
            tradeOrdersInDistributionSortingPara.orderIdList = new String[cancel_orderIdList_Counter];
            int cancel_tradeOrderId_i=0;
            for (int temp_i = 0; temp_i < orderItems_Counter; temp_i++)
            {
                if(orderItems[temp_i].isFull)
                {
                    orderItems[temp_i].isCanceled = false;
                }
                else
                {
                    tradeOrdersInDistributionSortingPara.orderIdList[cancel_tradeOrderId_i++] = orderItems[temp_i].id.ToString();
                    CancelItemContent tempCancelItemContent = new CancelItemContent();
                    if(orderItems[temp_i].tid != null)
                    {
                        tempCancelItemContent.tradeOrderNo = orderItems[temp_i].tid;
                    }
                    else
                    {
                        tempCancelItemContent.tradeOrderNo = orderItems[temp_i].tradeOrderNo;
                    }
                    tempCancelItemContent.id = orderItems[temp_i].id;
                    tempCancelItemContent.position = orderItems[temp_i].position;
                    cancelItemList.Add(tempCancelItemContent);
                    orderItems[temp_i].isCanceled = true;
                }
            }

            CancelTradeOrdersInDistributionSortingDialog cancelTradeOrdersInDistributionSortingDialog = new CancelTradeOrdersInDistributionSortingDialog();
            for(int list_i = 0; list_i < cancelItemList.Count; list_i++)
            {
                cancelTradeOrdersInDistributionSortingDialog.listViewCanceledOrderList.Items.Add((list_i + 1).ToString());
                cancelTradeOrdersInDistributionSortingDialog.listViewCanceledOrderList.Items[list_i].SubItems.Add(cancelItemList[list_i].tradeOrderNo);
                cancelTradeOrdersInDistributionSortingDialog.listViewCanceledOrderList.Items[list_i].SubItems.Add(cancelItemList[list_i].position.ToString());
            }
            cancelTradeOrdersInDistributionSortingDialog.ShowDialog();

            if(cancelTradeOrdersInDistributionSortingDialog.DecideResult)
            {
                TradeOrdersInDistributionSortingReturn result = DistributionPlatform.CancelTradeOrdersInDistributionSorting(tradeOrdersInDistributionSortingPara);
                if (result != null)
                {
                    if (result.code.Equals("1"))
                    {
                        // success; 
                        for (int list_i = 0; list_i < cancelItemList.Count; list_i++)
                        {
                            SetCancelLedStatus(cancelItemList[list_i].position);
                        }
                        button_confirm.Visible = true;
                        button_confirm.Enabled = true;
                        buttonCancelTradeOrdersInDistributionSorting.Visible = false;
                        buttonCancelTradeOrdersInDistributionSorting.Enabled = false;
                        MainShowMessage("取消未完成分播订成功！", 0);
                        LogFile.WriteLog("取消未完成分播订成功！");
                    }
                    else
                    {
                        // fail;
                        MainShowMessage("取消未完成分播订单失败，" + result.msg, 1);
                        LogFile.WriteLog("取消未完成分播订单失败," + result.msg);
                    }
                }
                else
                {
                    MainShowMessage("取消未完成分播订单异常！", 1);
                    LogFile.WriteLog("取消未完成分播订单异常！");
                }
            }
            else
            {
                MainShowMessage("取消操作！", 1);
                LogFile.WriteLog("取消操作！");
            }
            
        }


        // end class;
    }
}


//public class WeakCarInfoEventManager:WeakCarInfoEventManager
//{
//    public static void AddListener();
//}

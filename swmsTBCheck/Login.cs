using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;





namespace swmsTBCheck
{
    public partial class Login : Form
    {
        List<String> userNameList = new List<string>();



        public Login()
        {
            InitializeComponent();
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            String username = this.comboBoxUserName.Text;
            String password = this.textBox_password.Text;

            if (username.Length > 0)
            {
                //if (HttpSend.userLogin(textBox_username.Text, textBox_password.Text))
                if (this.comboBoxUserName.Text.Length > 0)
                {
                    //this.DialogResult = DialogResult.OK;
                    //this.Dispose();
                    //this.Close();
                    LogFile.WriteLog("用户：" + comboBoxUserName.Text + "登录成功");
                    if (userNameList.Contains(username))
                    {
                        // 
                    }
                    else
                    {
                        // if password check success, then save the name to the list;
                        userNameList.Add(username);
                        comboBoxUserName.Items.Add(username);
                        // write userNameList to UserList.txt;
                        if (!System.IO.File.Exists("UserList.txt"))
                        {
                            //没有则创建这个文件
                            FileStream fs1 = new FileStream("UserList.txt", FileMode.Create, FileAccess.Write);//创建写入文件                //设置文件属性为隐藏
                            System.IO.File.SetAttributes(@"UserList.txt", FileAttributes.Hidden);
                            StreamWriter sw = new StreamWriter(fs1);
                            sw.WriteLine(username.Trim());//开始写入值
                            sw.Close();
                            fs1.Close();
                        }
                        else
                        {
                            FileStream fs = new FileStream("UserList.txt", FileMode.Append, FileAccess.Write);
                            System.IO.File.SetAttributes(@"UserList.txt", FileAttributes.Hidden);
                            StreamWriter sr = new StreamWriter(fs);
                            sr.WriteLine(username.Trim());//开始写入值

                            sr.Close();
                            fs.Close();
                        }
                    }

                    Configuration.userName = comboBoxUserName.Text;
                    MainForm mainForm = new MainForm(this);
                    mainForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("用户名或密码不正确", "登录失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogFile.WriteLog("用户："+comboBoxUserName.Text+"非法");
                }
                
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            // load username data from file;
            string strData = "";
            try
            {
                string line;
                // 创建一个 StreamReader 的实例来读取文件 ,using 语句也能关闭 StreamReader
                using (System.IO.StreamReader sr = new System.IO.StreamReader("UserList.txt"))
                {
                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);
                        strData = line;
                        if (strData.Length > 0)
                        {
                            userNameList.Add(strData);
                            comboBoxUserName.Items.Add(strData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 向用户显示出错消息
                //Console.WriteLine("The file could not be read:");
                //Console.WriteLine(e.Message);
            }
            if (comboBoxUserName.Items.Count > 0)
            {
                comboBoxUserName.SelectedIndex = 0;
            }
        }
    }
}

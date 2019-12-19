using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace swmsTBCheck
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Configuration.iActulaWidth = Screen.PrimaryScreen.Bounds.Width;
            Configuration.iActulaHeight = Screen.PrimaryScreen.Bounds.Height;

            //Login login = new Login();
            //login.ShowDialog();
            //if (login.DialogResult == DialogResult.OK)
            //{
            //    login.Dispose();

            //    Application.Run(new MainForm());
            //}
            //else
            //{
            //    login.Dispose();
            //    return;
            //}

            // for test;
            Application.Run(new Login());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;



namespace swmsTBCheck
{
    class BatteryProcessBar : ProgressBar
    {
        public BatteryProcessBar()
        {
            base.SetStyle(ControlStyles.UserPaint, true);
        }

        //重写OnPaint方法
        protected override void OnPaint(PaintEventArgs e)
        {
            SolidBrush brush = null;
            Rectangle bounds = new Rectangle(0, 0, base.Width, base.Height);
            //...
            //e.Graphics.FillRectangle(new SolidBrush(this.BackColor), 1, 1, bounds.Width, bounds.Height);
            bounds.Height -= 4;
            bounds.Width = ((int)(bounds.Width * (((double)base.Value) / ((double)base.Maximum)))) - 4;
            brush = new SolidBrush(Color.Coral);
            e.Graphics.FillRectangle(brush, 2, 2, bounds.Width, bounds.Height);

            

        }
    }
}

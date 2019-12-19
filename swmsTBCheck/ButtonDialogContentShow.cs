using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace swmsTBCheck
{
    public partial class ButtonDialogContentShow : Form
    {
        public ButtonDialogContentShow()
        {
            InitializeComponent();
        }

        private void ButtonDialogContentShow_Load(object sender, EventArgs e)
        {
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private void listViewShowInfo_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            this.listViewShowInfo.ListViewItemSorter = new ListViewItemComparer(e.Column);

            listViewShowInfo.Sort();
        }
    }
}

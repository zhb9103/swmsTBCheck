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
    public partial class DebugMessage : Form
    {
        public DebugMessage()
        {
            InitializeComponent();
        }

        private void DebugMessage_Load(object sender, EventArgs e)
        {

        }

        private void buttonGenOrder_Click(object sender, EventArgs e)
        {
            GenOrderDialog genOrderDialog = new GenOrderDialog();
            genOrderDialog.ShowDialog();
        }
    }
}

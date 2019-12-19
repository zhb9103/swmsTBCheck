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
    public partial class CancelTradeOrdersInDistributionSortingDialog : Form
    {
        public Boolean DecideResult = false;
        public CancelTradeOrdersInDistributionSortingDialog()
        {
            InitializeComponent();
        }

        private void CancelTradeOrdersInDistributionSortingDialog_Load(object sender, EventArgs e)
        {

        }

        private void buttonEnter_Click(object sender, EventArgs e)
        {
            DecideResult = true;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DecideResult = false;
            this.Close();
        }
    }
}

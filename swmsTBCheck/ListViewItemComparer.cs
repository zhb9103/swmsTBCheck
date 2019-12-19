using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;



namespace swmsTBCheck
{
    class ListViewItemComparer : IComparer
    {
        //static public ListViewItemComparer ListViewItemComparerApp=new ListViewItemComparer();
        private int col=0;
        /*
        static public int GetCol()
        {
            return ListViewItemComparerApp.col;
        }
        static public void SetCol(int col)
        {
            ListViewItemComparerApp.col = col;
        }
         * */
        public ListViewItemComparer(int col)
        {
            this.col = col;
        }

        public int Compare(object x, object y) 
        {
             int returnVal = -1;
             returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
             ((ListViewItem)y).SubItems[col].Text);
             return returnVal;
        }
    }
}

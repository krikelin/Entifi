using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Entify
{
    public partial class SearchBox : UserControl
    {
        public SearchBox()
        {
            InitializeComponent();
        }
        public String Text
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }
        public event EventHandler SearchClicked;
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (SearchClicked != null)
            {
                SearchClicked(this, new EventArgs());
            }
        }
    }
}

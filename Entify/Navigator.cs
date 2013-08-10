using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Entify
{
    public partial class Navigator : UserControl
    {
        
        public Navigator()
        {
            InitializeComponent();
        }
        public event EventHandler NavigatedForward;
        public event EventHandler NavigatedBack;

        private void Navigator_Load(object sender, EventArgs e)
        {
        }
        public bool CanGoBack
        {
            get
            {
                return pictureBox1.Enabled;
            }
            set
            {
                pictureBox1.Enabled = value;
            }
        }
        public bool CanGoForward
        {
            get
            {
                return pictureBox2.Enabled;
            }
            set
            {
                pictureBox2.Enabled = value;
            }
        }

        void pictureBox2_EnabledChanged(object sender, EventArgs e)
        {
            pictureBox2.BackgroundImage = pictureBox2.Enabled ? Properties.Resources.ic_forward_vista_enabled : Properties.Resources.ic_forward_vista_disabled;

        }

        void pictureBox1_EnabledChanged(object sender, EventArgs e)
        {
            pictureBox1.BackgroundImage = pictureBox1.Enabled ? Properties.Resources.ic_back_vista_enabled : Properties.Resources.ic_back_vista_disabled;

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (NavigatedForward != null)
            {
                NavigatedForward(this, new EventArgs());
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (NavigatedBack != null)
            {
                NavigatedBack(this, new EventArgs());
            }
        }
    }
}

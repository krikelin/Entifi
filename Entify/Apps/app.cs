using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Entify.Apps
{
    public partial class app : UserControl
    {
        public app()
        {
            InitializeComponent();
        }
        public Form1 Host;
        public app(string uri, Form1 host)
        {
            InitializeComponent();
            this.Host = host;
        }

        private void app_Load(object sender, EventArgs e)
        {

        }
        public virtual void Navigate(string uri)
        {
        }
    }
}

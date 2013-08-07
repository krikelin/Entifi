using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using AeroGlass;
namespace Entify
{
    public partial class Form1 : Form
    {
        internal static class NativeMethods
        {
            [DllImport("dwmapi.dll", EntryPoint="#127")]
            internal static extern void DwmGetColorizationParameters(ref DWMCOLORIZATIONPARAMS color);
        }

        public struct DWMCOLORIZATIONPARAMS
        {
            public uint ColorizationColor, 
                ColorizationAfterglow, 
                ColorizationColorBalance, 
                ColorizationAfterglowBalance, 
                ColorizationBlurBalance, 
                ColorizationGlassReflectionIntensity, 
                ColorizationOpaqueBlend;
        }
        public Dictionary<String, Type> RegistredAppTypes = new Dictionary<string, Type>();
        public Dictionary<String, Apps.app> Applications = new Dictionary<string, Apps.app>();
        public void Navigate(string uri)
        {
            if (uri.StartsWith("entify:"))
            {
                uri = uri.Substring("entify:".Length);
            }
            string[] fragments = uri.Split(':');
            var service = fragments[0];
            var app = fragments[1];
            string identifier = service + ":" + app;
            if (Applications.ContainsKey(identifier))
            {
                Apps.app application = (Apps.app)Applications[identifier];
                application.Show();
                application.BringToFront();
                application.Navigate(uri);
                application.Dock = DockStyle.Fill;
                return;
            }
            else
            {
                if (!RegistredAppTypes.ContainsKey(app))
                {
                    //throw new Exception("App does not exist");
                    app = "entity";
                }
                Apps.app application = (Apps.app)RegistredAppTypes[app].GetConstructor(new System.Type[]{typeof(String), typeof(Form1)}).Invoke(new Object[]{uri, this});
                this.panel1.Controls.Add(application);
                application.Show();
                application.BringToFront();
                application.Dock = DockStyle.Fill;
                application.Navigate(uri);
                Applications.Add(identifier, application);
            }

        }
        public DWMCOLORIZATIONPARAMS color = new DWMCOLORIZATIONPARAMS();
        public Form1()
        {
            
            NativeMethods.DwmGetColorizationParameters(ref color);
            this.BackColor =Color.FromArgb(
                (byte)(true ? 255 : color.ColorizationColor >> 24), 
                (byte)(color.ColorizationColor >> 16), 
                (byte)(color.ColorizationColor >> 8), 
                (byte)(color.ColorizationColor));
            InitializeComponent();
            RegistredAppTypes.Add("entity", typeof(Apps.entity));
            this.Navigate("spotify:user:drsounds");
          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AeroGlass.Glass ew = new Glass();
            ew.extendFrame( glassPane1.Height, 60, 0, 0, this);    
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Navigate(textBox1.Text);
        }
    }
}

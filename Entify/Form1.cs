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
            if (fragments.Length < 3)
                return;
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
            Program.form1 = this;
            this.Navigate("spotify:user:drsounds");
          
        }
        /// <summary>
        /// Creates color with corrected brightness.
        /// </summary>
        /// <param name="color">Color to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
        /// Negative values produce darker colors.</param>
        /// <returns>
        /// Corrected <see cref="Color"/> structure.
        /// </returns>
        public static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.searchBox1.SearchClicked += searchBox1_SearchClicked;
            this.AcceptButton = button1;
            AeroGlass.Glass ew = new Glass();
            
            
        }

        void searchBox1_SearchClicked(object sender, EventArgs e)
        {
            this.Navigate(this.searchBox1.Text);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Navigate(this.searchBox1.Text);
        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }
    }
}

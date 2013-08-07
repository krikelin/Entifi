using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Entify.Apps
{
    class entity : app
    {
        public String LoadLocalResource(string uri)
        {
            EntifyScheme es = new EntifyScheme();
            System.IO.Stream stream = new System.IO.MemoryStream();
            String mimeType = "";
            es.LoadResource(uri, ref mimeType, ref stream);
            String str = new StreamReader(stream).ReadToEnd();
            stream.Close();
            return str;

        }
        public CefSharp.WinForms.WebView webView;
        public string uri;
        public entity(string uri)
            : base(uri)
        {
            this.uri = uri;
            try
            {
                webView = new CefSharp.WinForms.WebView("about:blank", Program.settings);
                CefSharp.CEF.RegisterScheme("entify", new EntifySchemeHandlerFactory());
                webView.PropertyChanged += webView_PropertyChanged;

                webView.Dock = DockStyle.Fill;
                webViewIsReady = true;
                this.Controls.Add(webView);

               
            }
            catch (Exception e)
            {
            }
        }
        public bool webViewIsReady = false;
        void webView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {   
            
            var fragments = uri.Split(':');
            var app = fragments[1];
             if (e.PropertyName.Equals("IsBrowserInitialized", StringComparison.OrdinalIgnoreCase))
             {
               
                if (webView.IsBrowserInitialized)
                {
                        webView.LoadHtml(LoadLocalResource("entify://" + app + "/index.html"));
                    
                }
            }
            
        }
        public override void Navigate(string uri)
        {
        }
    }
}

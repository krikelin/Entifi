﻿using CefSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        public entity(string uri, Form1 host)
            : base(uri, host)
        {
            this.Host = host;

            var fragments = uri.Split(':');
            var app = fragments[1];

            this.uri = uri;
            try
            {
                webView = new CefSharp.WinForms.WebView("about:blank", Program.settings);
                CefSharp.CEF.RegisterScheme("entify", new EntifySchemeHandlerFactory());
                webView.PropertyChanged += webView_PropertyChanged;
                template = LoadLocalResource("entify://" + app + "/index.html");
                webView.Dock = DockStyle.Fill;
                webViewIsReady = true;
                this.Controls.Add(webView);

               
            }
            catch (Exception e)
            {
            }
        }
        public override void Navigate(string uri) 
        {
            base.Navigate(uri);
          //  if(webView.IsBrowserInitialized)
          //  this.webView.LoadHtml("<html></html>");

            Models.IEntifyService service = new Models.W3Service();
            service.ObjectLoaded += service_ObjectLoaded;
            service.RequestObjectAsync(uri);
        }

        void service_ObjectLoaded(object sender, Models.IEntifyService.ObjectLoadedEventArgs e)
        {
            String temp_path = Environment.GetEnvironmentVariable("temp") + Path.DirectorySeparatorChar + "entify_razor.tmp";
            using(StreamWriter sr = new StreamWriter(temp_path)) {
                sr.Write(template);
                sr.Close();
            }
            using (var writer = new StringWriter())
            {
                try
                {
                  
                    var context = new Dictionary<String, Object>();
                    context.Add("data", e.Result);
                    context.Add("uri", e.Uri);
                    NDjango.TemplateManagerProvider r = new NDjango.TemplateManagerProvider();
                    NDjango.Interfaces.ITemplateManager manager =
                     r.GetNewManager();


                    TextReader tr = manager.RenderTemplate(temp_path, context);

                    view = (tr.ReadToEnd().ToString());
                    if (webView.IsBrowserInitialized)
                        webView.LoadHtml(view);
                }
                catch (Exception ex)
                {
                }
            }
        }

        void inspector_Click(object sender, EventArgs e)
        {
            webView.ShowDevTools();
        }
        
        class MyRequestHandler : IRequestHandler
        {
            public delegate void Navigate(string uri);
            public Form1 Host;
            public MyRequestHandler(Form1 host)
            {
                this.Host = host;
            }
            public void Navigating(string uri)
            {
                this.Host.Navigate(uri);
            }
            public bool OnBeforeBrowse(IWebBrowser browser, IRequest request,  NavigationType naigationvType, bool isRedirect)
            {
                Host.Invoke(new Navigate(this.Navigating), new Object[]{request.Url});
                return false;
            }


            public bool GetAuthCredentials(IWebBrowser browser, bool isProxy, string host, int port, string realm, string scheme, ref string username, ref string password)
            {
                return true;
            }

            public bool GetDownloadHandler(IWebBrowser browser, string mimeType, string fileName, long contentLength, ref IDownloadHandler handler)
            {
                return true;
            }

            public bool OnBeforeResourceLoad(IWebBrowser browser, IRequestResponse requestResponse)
            {
                return false;
            }

            public void OnResourceResponse(IWebBrowser browser, string url, int status, string statusText, string mimeType, System.Net.WebHeaderCollection headers)
            {
                
            }
        }
        string template = "";
        string view = "";
        public bool webViewIsReady = false;
        void webView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {   
            
            
             if (e.PropertyName.Equals("IsBrowserInitialized", StringComparison.OrdinalIgnoreCase))
             {
               
                if (webView.IsBrowserInitialized)
                {
                    ContextMenu cm = new System.Windows.Forms.ContextMenu();
                    MenuItem inspector = new MenuItem();
                    cm.MenuItems.Add(inspector);
                    inspector.Text = "Show Inspector";
                    inspector.Click += inspector_Click;
                    webView.ContextMenu = cm;
                    webView.RequestHandler = new MyRequestHandler(this.Host);
                        webView.LoadHtml(view);
                    
                }
            }
            
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // entity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "entity";
            this.Load += new System.EventHandler(this.entity_Load);
            this.ResumeLayout(false);

        }

        private void entity_Load(object sender, EventArgs e)
        {

        }
    }
}

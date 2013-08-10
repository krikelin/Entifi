using CefSharp;
using Entify.Models;
using Entify.Spider;
using Entify.Spider.Scripting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace Entify.Apps
{

    class entity : app, ISpiderView
    {

        public String LoadLocalResource(string uri)
        {
            EntifyScheme es = new EntifyScheme(this);
            System.IO.Stream stream = new System.IO.MemoryStream();
            String mimeType = "";
            es.LoadResource(uri, ref mimeType, ref stream);
            String str = new StreamReader(stream).ReadToEnd();
            stream.Close();
            return str;

        }
        public CefSharp.WinForms.WebView webView;
        EntifyScheme entifyScheme = null;
        public string uri;
        public entity(string uri, Form1 host)
            : base(uri, host)
        {
            this.Host = host;
            this.Runtime = new Spider.Scripting.LuaInterpreter(this);
            this.Preprocessor = new Spider.Preprocessor.LuaMako(this);
            entifyScheme = new EntifyScheme(this);
            var fragments = uri.Split(':');
            var app = fragments[1];

            this.uri = uri;
            try
            {
                Program.settings.UserStyleSheetEnabled = true;
                Program.settings.UserStyleSheetLocation = "resources/resources/css/" + Properties.Settings.Default.Theme + ".css";
                webView = new CefSharp.WinForms.WebView("about:blank", Program.settings);
                CefSharp.CEF.RegisterScheme("entify", new EntifySchemeHandlerFactory(this, entifyScheme));
                
                webView.PropertyChanged += webView_PropertyChanged;
#if(false)
                template = LoadLocalResource("entify://" + app + "/index.html");
#endif
                webView.Dock = DockStyle.Fill;
                webViewIsReady = true;
                this.Controls.Add(webView);

               
            }
            catch (Exception e)
            {
            }
        }
        public void subscribe(string uri)
        {
            if (uri.Contains("$"))
            {
                uri = uri.Split('$')[0];
            }
            Models.IEntifyService service = new Models.MongoService();
            service.ObjectLoaded += service_ObjectLoaded;
            service.RequestObjectAsync(uri);
        }

        public String getUri()
        {
            return this.uri;
        }
        public class JSEntify
        {
            Models.IEntifyService service = new Models.MongoService();
            public delegate void EntifyLoaded(string uri);
            public void entifyLoaded(string uri)
            {
                
            }
            public entity Host;
            public JSEntify(entity host)
            {
                this.Host = host;
            }
            public void subscribe(string uri)
            {
                service.ObjectLoaded += service_ObjectLoaded;
                service.RequestObjectAsync(uri);
            }
            public string getUri()
            {
                return this.Host.uri;
            }
            public Hashtable cache = new Hashtable();
            public Hashtable replies = new Hashtable();
            public void send(string method, string uri, string payload)
            {
                service.Sent += service_Sent;
                service.SendAsync(method, uri, payload);
            }

            void service_Sent(object sender, IEntifyService.ObjectLoadedEventArgs e)
            {
                if (this.replies.ContainsKey(e.Uri))
                {
                    this.replies[e.Uri] = JsonConvert.SerializeObject(e.Result);
                }
                else
                {
                    this.replies.Add(e.Uri, JsonConvert.SerializeObject(e.Result));
                }
                this.Host.webView.ExecuteScript("application.notify('reply', {uri: '" + e.Uri + "', method: '" + e.Method + "', data: JSON.parse(EntifyCore.getReply('" + e.Uri + "'))})");
               
            }
            public string getReply(string uri)
            {
                if (replies.ContainsKey(uri))
                    return (string)replies[uri];
                else
                    return null;
            }
            public string getData(string uri)
            {
                if (cache.ContainsKey(uri))
                    return (string)cache[uri];
                else
                    return null;
            }
            public void service_ObjectLoaded(object sender, Models.IEntifyService.ObjectLoadedEventArgs e)
            {
                try
                {
                    if (this.cache.ContainsKey(e.Uri))
                    {
                        this.cache[e.Uri] = e.Result.GetType() == typeof(string) ? e.Result : JsonConvert.SerializeObject(e.Result);
                    }
                    else
                    {
                        this.cache.Add(e.Uri, e.Result.GetType() == typeof(string) ? e.Result : JsonConvert.SerializeObject(e.Result));
                    }
                    //   this.Host.webView.RegisterJsObject("__data", e.Result);
                    this.Host.webView.ExecuteScript("application.notify('recievedata', {data: JSON.parse(EntifyCore.getData('" + e.Uri + "')), uri: '" + e.Uri + "'})");
                }
                catch (Exception ex)
                {
                    var error = this.Host.entifyScheme.LoadResource("entify://resources/error.html");

                    this.Host.webView.LoadHtml(error);
                }
            }
        }
        public string viewAddress = "";
        public Object Token { get; set; }
        public override void Navigate(string uri) 
        {
            base.Navigate(uri);
            if (webView.Address.EndsWith(".xml")) // If we are in Spider mode
            {
                subscribe(uri);
            }
            var app = uri.Split(':')[1];
            var service = uri.Split(':')[0];
            if (!this.uri.StartsWith(app + ":" + service))
            {
                if(webView.IsBrowserInitialized)
                webView.Load("entify://" + app + "/index.html");
                viewAddress = "entify://" + app + "/index.html";
            }
            if (this.uri.Contains("$")) 
            {
                view = this.uri.Substring(this.uri.IndexOf("$") + 1);
                if (view.EndsWith(".xml")) // If we are in Spider mode
                {
                    webView.RegisterJsObject("SpiderCore", new SpiderCore(this));
                }
                if(webView.IsBrowserInitialized)
                webView.Load("entify://" + app + "/index.xml");
                viewAddress = "entify://" + app + "/index.xml";
            }
            this.uri = uri;
            
            if (webView.IsBrowserInitialized)
                this.webView.ExecuteScript("application.onargumentschanged('" + uri + "'.split(':'))");
#if(false)
            Models.IEntifyService service = new Models.W3Service();
            service.ObjectLoaded += service_ObjectLoaded;
            service.RequestObjectAsync(uri);
#endif

        }

        
        void service_ObjectLoaded(object sender, Models.IEntifyService.ObjectLoadedEventArgs e)
        {
            String temp_path = Environment.GetEnvironmentVariable("temp") + Path.DirectorySeparatorChar + "entify_razor.tmp";

            this.Token = JsonConvert.DeserializeObject<Object>((string)e.Result);
            webView.LoadHtml(Process(entifyScheme.LoadResource(viewAddress)));
#if(false)
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
#else
         //   this.webView.RegisterJsObject("___data", e.Result);
       //     this.webView.ExecuteScript("application.onrecievedata(__data)");
#endif
        }

        void inspector_Click(object sender, EventArgs e)
        {
            webView.ShowDevTools();
        }
        
        class MyRequestHandler : IRequestHandler
        {
            public delegate void Navigate(string uri);
            public Form1 Host;
            public entity App;
            public MyRequestHandler(Form1 host, entity app)
            {
                this.App = app;
                this.Host = host;
            }
            public void Navigating(string uri)
            {
                this.Host.Navigate(uri, true);
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
                if (url.EndsWith(".xml")) // If we are in Spider mode
                {
                   App.subscribe(this.App.Uri);
                }
            }
        }
        public class SpiderCore
        {
            public entity Host { get; set; }
            public SpiderCore(entity host)
            {
                this.Host = host;
            }
            public object execute(string func, params object[] arguments)
            {
               return Host.Runtime.InvokeFunction(func, arguments);
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
                    webView.RequestHandler = new MyRequestHandler(this.Host, this);
                    #if(false)
                        webView.LoadHtml(view);
#else
                     var fragments = uri.Split(':');
                      var app = fragments[1];
                    var view = "index.html";
                    if (this.uri.Contains("$")) 
                        {
                        view = this.uri.Substring(this.uri.IndexOf("$") + 1);
                        if (view.EndsWith(".xml")) // If we are in Spider mode
                        {
                            webView.RegisterJsObject("SpiderCore", new SpiderCore(this));
                        }

                    }
                    webView.Load("entify://" + app + "/" + view);
                    viewAddress = "entify://" + app + "/" + view;
                    if (webView.Address.EndsWith(".xml")) // If we are in Spider mode
                    {
                        subscribe(uri);
                    }
                    try
                    {
                        webView.RegisterJsObject("EntifyCore", new JSEntify(this));
                    }
                    catch (Exception xe)
                    {
                    }
#endif
                    
                }
            }
            
        }
        public string Uri
        {
            get
            {
                return this.uri;
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

        public Spider.Scripting.IScriptEngine Runtime
        {
            get;
            set;
        }

        public Spider.Preprocessor.IPreprocessor Preprocessor
        {
            get;
            set;
        }


        public string Process(string shtml)
        {
           
            shtml = Preprocessor.Preprocess(shtml, Token);

            var styles = "<link class=\"hidden\" href=\"entify://spider/css/spider.css\" rel=\"stylesheet\" type=\"text/css\" />";
            styles += "<link class=\"hidden\" href=\"entify://resources/css/" + Properties.Settings.Default.Theme + ".css\" rel=\"stylesheet\" type=\"text/css\" />";
            
            var polyfills = "<script src=\"entify://resources/scripts/models.js\" type=\"text/javascript\"></script>";
            polyfills += "<script src=\"entify://resources/scripts/views.js\" type=\"text/javascript\"></script>";
            polyfills += "<script src=\"entify://spider/scripts/spider-polyfill.js\" type=\"text/javascript\"></script>";
            shtml = "<html><head>" + styles + "</head><body>" + shtml;

            shtml = shtml + polyfills + "</body></html>";
            return shtml;
        }
    }
}

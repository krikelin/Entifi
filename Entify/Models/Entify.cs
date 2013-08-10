using dotless.Core.configuration;
using dotless.Core.Exceptions;
using dotless.Core.Parser;
using dotless.Core.Parser.Infrastructure;
using dotless.Core.Parser.Tree;
using Entify.Spider;
using Entify.Spider.Preprocessor;
using Entify.Spider.Scripting;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;

namespace Entify.Models
{
    public class EntifySchemeHandlerFactory : CefSharp.ISchemeHandlerFactory
    {
        public EntifySchemeHandlerFactory(ISpiderView spiderView, EntifyScheme entifyScheme)
        {
            this.SpiderView = spiderView;
            this.EntifyScheme = entifyScheme;
        }
        public EntifySchemeHandlerFactory(ISpiderView spiderView)
        {
            this.SpiderView = spiderView;
        }
        public ISpiderView SpiderView;
        public CefSharp.ISchemeHandler Create()
        {
            return EntifyScheme != null ? EntifyScheme : new EntifyScheme(SpiderView);
        }

        public EntifyScheme EntifyScheme { get; set; }
    }
    public class EntifyScheme : CefSharp.ISchemeHandler
    {
        public EntifyScheme(ISpiderView spiderView)
        {
            this.SpiderView = spiderView;
        }
        public ISpiderView SpiderView;
        public bool LoadResource(string url, ref string mimeType, ref System.IO.Stream stream)
        {
           
            if (url.StartsWith("entify://"))
            {
                url = url.Substring("entify://".Length);
                string[] fragments = url.Split('/');
                String bundle = fragments[0];
                String path = ENTIFY_APPS_DIR + Path.DirectorySeparatorChar + bundle + Path.DirectorySeparatorChar + String.Join(Path.DirectorySeparatorChar.ToString(), fragments, 1, fragments.Length - 1);
                if (!File.Exists(path))
                {
                    path = ENTIFY_LOCAL_APPS_DIR + Path.DirectorySeparatorChar + bundle + Path.DirectorySeparatorChar + String.Join(Path.DirectorySeparatorChar.ToString(), fragments, 1, fragments.Length - 1);
                }
                if (File.Exists(path))
                {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    stream = fs;
                    mimeType = GetMimeType(path);

                   

                    if (mimeType == "text/html" || mimeType == "text/xml")
                    {
                        byte[] bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, bytes.Length);
                        MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
                        fs.Close();
                        stream = ms;
                        StreamReader sr = new StreamReader(ms);
                        var css = sr.ReadToEnd();
                        css = css.Replace("{{primary_color}}", ColorTranslator.ToHtml(Program.form1.BackColor));
                        css = css.Replace("{{primary_color|rgb}}", String.Format("%s,%s,%s", Program.form1.BackColor.R, Program.form1.BackColor.G, Program.form1.BackColor.B));
                        css = css.Replace("{{hue}}", Math.Round(Program.form1.BackColor.GetHue()).ToString());
                        css = css.Replace("{{sat}}", Math.Round(Program.form1.BackColor.GetSaturation() * 100).ToString());
                        css = css.Replace("{{bright}}", Math.Round(Program.form1.BackColor.GetBrightness() * 100).ToString());
                        css = css.Replace("{{theme}}", Properties.Settings.Default.Theme);
                  

                        MemoryStream ms2 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(css), false);
                        stream = ms2;

                       
                    }
                    
                    return true;
                }
            }
            return false;
        }
        public string LoadResource(string url)
        {

            if (url.StartsWith("entify://"))
            {
                url = url.Substring("entify://".Length);
                string[] fragments = url.Split('/');
                String bundle = fragments[0];
                String path = ENTIFY_APPS_DIR + Path.DirectorySeparatorChar + bundle + Path.DirectorySeparatorChar + String.Join(Path.DirectorySeparatorChar.ToString(), fragments, 1, fragments.Length - 1);
                if (!File.Exists(path))
                {
                    path = ENTIFY_LOCAL_APPS_DIR + Path.DirectorySeparatorChar + bundle + Path.DirectorySeparatorChar + String.Join(Path.DirectorySeparatorChar.ToString(), fragments, 1, fragments.Length - 1);
                }
                if (File.Exists(path))
                {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs);
                    



                    if (path.EndsWith(".css"))
                    {
                       
                        var css = sr.ReadToEnd();
                        css = css.Replace("{{primary_color}}", ColorTranslator.ToHtml(Program.form1.BackColor));
                        css = css.Replace("{{primary_color|rgb}}", String.Format("%s,%s,%s", Program.form1.BackColor.R, Program.form1.BackColor.G, Program.form1.BackColor.B));
                        css = css.Replace("{{hue}}", Math.Round(Program.form1.BackColor.GetHue()).ToString());
                        css = css.Replace("{{sat}}", Math.Round(Program.form1.BackColor.GetSaturation() * 100).ToString());
                        css = css.Replace("{{bright}}", Math.Round(Program.form1.BackColor.GetBrightness() * 100).ToString());
                        css = css.Replace("{{theme}}", Properties.Settings.Default.Theme);


                      return css;


                    }

                    return sr.ReadToEnd();
                }
            }
            return null;
        }
        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
        public static String ENTIFY_APPS_DIR = "resources" + Path.DirectorySeparatorChar + "apps";
        public static String ENTIFY_LOCAL_APPS_DIR = "resources" + Path.DirectorySeparatorChar + "apps";
        bool Compress = false;
        Parser Parser;
        public string TransformToCss(string source, string fileName)
        {
            try
            {
                this.Parser = new dotless.Core.Parser.Parser();
                Ruleset ruleset = this.Parser.Parse(source, fileName);
                Env env = new Env();
                env.Compress = this.Compress;
                Env env2 = env;
                return ruleset.ToCSS(env2);
            }
            catch (ParserException exception)
            {
            }
            return "";
        }
        
        public bool ProcessRequest(CefSharp.IRequest request, ref string mimeType, ref System.IO.Stream stream)
        {

            if (request.Url.StartsWith("entify://"))
            {
                request.Url = request.Url.Substring("entify://".Length);
                string[] fragments = request.Url.Split('/');
                String bundle = fragments[0];
                String path = ENTIFY_APPS_DIR + Path.DirectorySeparatorChar + bundle + Path.DirectorySeparatorChar + String.Join(Path.DirectorySeparatorChar.ToString(), fragments, 1, fragments.Length - 1);
                if (!File.Exists(path))
                {
                    path = ENTIFY_LOCAL_APPS_DIR + Path.DirectorySeparatorChar + bundle + Path.DirectorySeparatorChar + String.Join(Path.DirectorySeparatorChar.ToString(), fragments, 1, fragments.Length - 1);
                }
                if (File.Exists(path))
                {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
                    fs.Close();
                    stream = ms;
                    mimeType = GetMimeType(path);
                    if (mimeType == "text/css")
                    {
                        StreamReader sr = new StreamReader(ms);
                        var css = sr.ReadToEnd();
                        css = css.Replace("{{primary_color}}", ColorTranslator.ToHtml(Program.form1.BackColor));
                        css = css.Replace("{{primary_color|rgb}}", String.Format("%s,%s,%s", Program.form1.BackColor.R, Program.form1.BackColor.G, Program.form1.BackColor.B));
                        css = css.Replace("{{hue}}", Math.Round(Program.form1.BackColor.GetHue()).ToString());
                        css = css.Replace("{{sat}}", Math.Round(Program.form1.BackColor.GetSaturation() * 100).ToString());
                        css = css.Replace("{{bright}}", Math.Round(Program.form1.BackColor.GetBrightness() * 100).ToString());
                        var f = DotlessConfiguration.GetDefault();
                        css = TransformToCss(css, "a.tmp");

                        MemoryStream ms2 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(css), false);
                        stream = ms2;
                    }
                    if (mimeType == "application/javascript")
                    {
                        return true;
                    }
                    if (mimeType == "text/html")
                    {
                        StreamReader sr = new StreamReader(ms);
                        var css = sr.ReadToEnd();
                        css = css.Replace("{% header %}", LoadResource("entify://resources/header.html"));
                        css = css.Replace("{{primary_color}}", ColorTranslator.ToHtml(Program.form1.BackColor));
                        css = css.Replace("{{primary_color|rgb}}", String.Format("%s,%s,%s", Program.form1.BackColor.R, Program.form1.BackColor.G, Program.form1.BackColor.B));
                        css = css.Replace("{{hue}}", Math.Round(Program.form1.BackColor.GetHue()).ToString());
                        css = css.Replace("{{sat}}", Math.Round(Program.form1.BackColor.GetSaturation() * 100).ToString());
                        css = css.Replace("{{bright}}", Math.Round(Program.form1.BackColor.GetBrightness() * 100).ToString());
                        css = css.Replace("{{theme}}", Properties.Settings.Default.Theme);
                        //var f = DotlessConfiguration.GetDefault();
                        // css = TransformToCss(css, "a.tmp");
                       
                        MemoryStream ms2 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(css), false);
                        stream = ms2;
                    }
                    if (request.Url.EndsWith(".xml")) // Assume spider view
                    {
                        StreamReader sr = new StreamReader(ms);
                     
                        IScriptEngine scripting = SpiderView.Runtime;
                        IPreprocessor preprocessor = SpiderView.Preprocessor;

                        // Read raw data
                       

                        // Extract
                        var shtml = SpiderView.Process(LoadResource(request.Url));
                        mimeType = "text/html";
                        MemoryStream ms2 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(shtml), false);
                        stream = ms2;
                    }
                    return true;
                }
            }
            return false;
        }
    }
    public class Entity
    {
        private Hashtable properties = new Hashtable();
        public Entity(Hashtable properties)
        {
            this.properties = properties;
        }
        public object prop(string key)
        {
            if (properties.ContainsKey(key))
            {
                return properties[key];
            }
            else
            {
                return null;
            }
        }
    }
    public class W3Service : IEntifyService
    {
        public static Hashtable cache = new Hashtable();
        

        public override object Request(string method, string uri, string payload)
        {
            if (method == "GET")
            {
                var id = uri.Split(':')[2];
                var app =uri.Split(':')[1];
                var service = uri.Split(':')[0];
                object result = null;
                if (cache.ContainsKey(uri))
                {
                    return (cache[uri]);
                }

                Thread.Sleep((int)new Random().Next(0, 3000));
                using (StreamReader sr = new StreamReader("resources/sampleentity.json"))
                {
                    var settings = new Newtonsoft.Json.JsonSerializerSettings();
                   var json = sr.ReadToEnd();
                   json = json.Replace("{{id}}", id);
                   json = json.Replace("{{service}}", service);
                   json = json.Replace("{{app}}", app);
                   json = json.Replace("{{uri}}", uri);
                    var r  = JObject.Parse(json);
                    
                    result = Newtonsoft.Json.JsonConvert.SerializeObject(r["node"]);
                }
                if (cache.ContainsKey(uri))
                {
                    cache[uri] = result;
                }
                else
                {
                    cache.Add(uri, result);
                }
                return result;

            }
            if(method == "PUT") {
                JObject json = JObject.Parse(payload);
                if (cache.ContainsKey(uri))
                {
                    object obj = (object)cache[uri];
                    IDictionary data = null;
                    if (obj.GetType() == typeof(string))
                    {
                        data = (IDictionary)JObject.Parse((string)obj);
                    }
                    else
                    {
                        data = (IDictionary)obj;
                    }
                    foreach(KeyValuePair<string, JToken> o in json) 
                    {
                        if (data.Contains(o.Key))
                        {
                            data[o.Key] = o.Value.ToString();
                        }
                        else
                        {
                            data.Add(o.Key, o.Value.ToString());
                        }
                    }
                    if (obj.GetType() == typeof(string))
                    {
                        cache[uri] = data;
                    }
                    return cache[uri];
                }
            }
            if (method == "FOLLOW")
            {
                if (cache.ContainsKey(uri))
                {
                    IDictionary data = null;
                    var obj = cache[uri];
                    if (obj.GetType() == typeof(string))
                    {
                        data = (IDictionary)JObject.Parse((string)obj);
                    }
                    else
                    {
                        data = (IDictionary)data;
                    }
                    if (data.Contains("following"))
                    {
                        data["following"] = !((bool)data["following"]);
                    }
                    else
                    {
                        data.Add("following", true);
                    }
                    return cache[uri];
                }
            }
            return null;
        }
    }
    public class MongoService : IEntifyService
    {
        public override object Request(string method, string uri, string payload)
        {
            MongoClient client = new MongoClient();
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase("local");
            MongoCollection<BsonDocument> nodes = db.GetCollection("nodes");
            if (method == "GET" || method == "SUBSCRIBE")
            {
                MongoCursor<BsonDocument> result = nodes.Find(new QueryDocument("_id", new BsonString(uri.Split(':')[2])));
                foreach (BsonDocument r in result)
                {
                    var data = r.ToJson();
                    server.Disconnect();
                    return data;
                }  
            }
            if (method == "PUT")
            {
                MongoCursor<BsonDocument> result = nodes.Find(new QueryDocument("_id", new BsonString(uri.Split(':')[2])));
                foreach (BsonDocument r in result)
                {
                    var data = r.ToJson();
                    server.Disconnect();
                    return data;
                }
            }
            return null;
        }
    }
    public abstract class IEntifyService
    {
        /// <summary>
        /// Loads object from the given uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public abstract Object Request(string method, string uri, string payload);
        public class ObjectLoadedEventArgs {
            public IEntifyService Service;
            public Object Result;
            public String Method = "GET";
          
            public String Uri;
        }
        public event ObjectLoadedEventHandler Sent;
        public delegate void ObjectLoadedEventHandler(object sender, ObjectLoadedEventArgs e);
        public event ObjectLoadedEventHandler ObjectLoaded;
        public void RequestObjectAsync(string uri)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.RunWorkerAsync(uri);

        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WorkerProcess p = (WorkerProcess)e.Result;
            if (ObjectLoaded != null)
            {
                ObjectLoaded(this, new ObjectLoadedEventArgs() { Result = p.data, Uri = p.uri, Method = p.method });
            }
        }
        public class WorkerProcess {
            public String uri;
            public object data;
            public string method;
        }
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
              
                WorkerProcess data = new WorkerProcess();
                data.data = Request("GET", (string)e.Argument, "{}");
                data.uri = (String)e.Argument;
                e.Result = data;
            }
            catch (Exception ex)
            {
            }
        }

        internal void SendAsync(string method, string uri, string payload)
        {
            BackgroundWorker sendWorker = new BackgroundWorker();
            sendWorker.DoWork += sendWorker_DoWork;
            sendWorker.RunWorkerCompleted += sendWorker_RunWorkerCompleted;
            sendWorker.RunWorkerAsync(new object[] {method, uri, payload});
        }

        void sendWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            WorkerProcess p = (WorkerProcess)e.Result;
            if (Sent != null)
            {
                Sent(this, new ObjectLoadedEventArgs() { Result = p.data, Uri = p.uri, Method = p.method });
            }
        }

        void sendWorker_DoWork(object sender, DoWorkEventArgs e)
        {
           try
            {
                object[] data = (object[])e.Argument;
                string method = (string)data[0];
                string url = (string)data[1];
                string payload = (string)data[2];
                WorkerProcess wp = new WorkerProcess();
                wp.method = method;
                wp.data = Request((string)method, (string)url, payload);
                wp.uri = (String)url;
                e.Result = wp;
            }
             catch (Exception ex)
            {
            }
        }
    }
}

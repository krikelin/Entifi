using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace Entify.Models
{
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
        public override object LoadObject(string uri)
        {
            if (cache.ContainsKey(uri))
            {
                return cache[uri];
            }
            Thread.Sleep((int)new Random().Next(0, 3000));
            Hashtable result = new Hashtable();
            result.Add("title", "Test");
            result.Add("description", "t");
            result.Add("price", 20);
            result.Add("weight", 150);
            result.Add("height", 160);
            result.Add("laps", new Random().Next(1, 16));
            result.Add("depth", 160);
            result.Add("image", "https://encrypted-tbn1.gstatic.com/images?q=tbn:ANd9GcRLwbS1ceNk8GCHaGrDfPwKZWb4CMutkQS7r2rsoDAHC8aqdQiJ");
            
            var res = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            if(!cache.ContainsKey(uri))
            cache.Add(uri, res);
            return res;
        }

        public override object Request(string method, string uri, string payload)
        {
            Thread.Sleep((int)new Random().Next(0, 3000));
            if(method == "PUT") {
                JObject json = JObject.Parse(payload);
                if (cache.ContainsKey(uri))
                {
                    Hashtable obj = (Hashtable)cache[uri];
                    foreach(KeyValuePair<string, JToken> o in json) 
                    {
                        if (cache.ContainsKey(o.Key))
                        {
                            cache[o.Key] = o.Value.ToString();
                        }
                        else
                        {
                            cache.Add(o.Key, o.Value.ToString());
                        }
                    }
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
        public abstract Object LoadObject(string uri);
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
                data.data = LoadObject((string)e.Argument);
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

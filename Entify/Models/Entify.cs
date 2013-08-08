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
    }
    public abstract class IEntifyService
    {
        /// <summary>
        /// Loads object from the given uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public abstract Object LoadObject(string uri);

        public class ObjectLoadedEventArgs {
            public IEntifyService Service;
            public Object Result;
          
            public String Uri;
        }
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
                ObjectLoaded(this, new ObjectLoadedEventArgs() { Result = p.data, Uri = p.uri });
            }
        }
        public class WorkerProcess {
            public String uri;
            public object data;
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
    }
}

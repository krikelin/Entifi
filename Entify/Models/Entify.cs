using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace Entify.Models
{
    public class W3Service : IEntifyService
    {
        public override object LoadObject(string uri)
        {
            return new {
                title = "test",
                uri = uri
            };
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

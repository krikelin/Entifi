using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Entify
{
 
    public class EntifySchemeHandlerFactory : CefSharp.ISchemeHandlerFactory
    {
        public CefSharp.ISchemeHandler Create()
        {
            return new EntifyScheme();
        }
    }
    public class EntifyScheme : CefSharp.ISchemeHandler
    {
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
                    return true;
                }
            }
            return false;
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
        public static String ENTIFY_APPS_DIR =  "resources" + Path.DirectorySeparatorChar + "apps";
        public static String ENTIFY_LOCAL_APPS_DIR =  "resources" + Path.DirectorySeparatorChar + "apps";
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
                    stream = fs;
                    mimeType = GetMimeType(path);
                    return true;
                }
            }
            return false;
        }
    }
    static class Program
    {
        
        public static CefSharp.BrowserSettings settings = new CefSharp.BrowserSettings();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CefSharp.CEF.RegisterScheme("entify", new EntifySchemeHandlerFactory()); 
      //      CefSharp.CEF.RegisterJsObject("Entify", null);
          
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

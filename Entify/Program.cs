using dotless.Core.configuration;
using dotless.Core.Exceptions;
using dotless.Core.Parser;
using dotless.Core.Parser.Infrastructure;
using dotless.Core.Parser.Tree;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
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
                    

                    if (mimeType == "text/html")
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
        bool Compress = false;
        Parser Parser;
        public string TransformToCss(string source, string fileName)
        {
            try
            {
                this.Parser =new  dotless.Core.Parser.Parser();
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
                    if(mimeType == "text/css") 
                    {
                        StreamReader sr = new StreamReader(ms);
                        var css = sr.ReadToEnd();
                        css = css.Replace("{{primary_color}}", ColorTranslator.ToHtml(Program.form1.BackColor));
                        css = css.Replace("{{primary_color|rgb}}", String.Format("%s,%s,%s", Program.form1.BackColor.R, Program.form1.BackColor.G, Program.form1.BackColor.B));
                        css = css.Replace("{{hue}}", Math.Round(Program.form1.BackColor.GetHue()).ToString());
                        css = css.Replace("{{sat}}", Math.Round(Program.form1.BackColor.GetSaturation() * 100).ToString());
                        css = css.Replace("{{bright}}", Math.Round(Program.form1.BackColor.GetBrightness() * 100).ToString());
                        var f =  DotlessConfiguration.GetDefault();
                        css  = TransformToCss(css, "a.tmp");
                       
                        MemoryStream ms2 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(css), false);
                        stream = ms2;
                    }
                    if (mimeType == "text/html")
                    {
                        StreamReader sr = new StreamReader(ms);
                        var css = sr.ReadToEnd();
                        css = css.Replace("{{primary_color}}", ColorTranslator.ToHtml(Program.form1.BackColor));
                        css = css.Replace("{{primary_color|rgb}}", String.Format("%s,%s,%s", Program.form1.BackColor.R, Program.form1.BackColor.G, Program.form1.BackColor.B));
                        css = css.Replace("{{hue}}", Math.Round(Program.form1.BackColor.GetHue()).ToString());
                        css = css.Replace("{{sat}}", Math.Round(Program.form1.BackColor.GetSaturation() * 100).ToString());
                        css = css.Replace("{{bright}}", Math.Round(Program.form1.BackColor.GetBrightness() * 100).ToString());
                        css = css.Replace("{{theme}}", Properties.Settings  .Default.Theme);
                        //var f = DotlessConfiguration.GetDefault();
                       // css = TransformToCss(css, "a.tmp");

                        MemoryStream ms2 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(css), false);
                        stream = ms2;
                    }
                    return true;
                }
            }
            return false;
        }
    }
    static class Program
    {
        public static Form1 form1;
        
        public static CefSharp.BrowserSettings settings = new CefSharp.BrowserSettings();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            settings.WebSecurityDisabled = true;
            settings.FileAccessFromFileUrlsAllowed = true;
            settings.UniversalAccessFromFileUrlsAllowed = true;
            settings.TextAreaResizeDisabled = true;
            CefSharp.CEF.RegisterScheme("entify", new EntifySchemeHandlerFactory()); 
      //      CefSharp.CEF.RegisterJsObject("Entify", null);
          
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form1 = new Form1();
            Application.Run(form1);
        }
    }
}

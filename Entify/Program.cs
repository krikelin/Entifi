using dotless.Core.configuration;
using dotless.Core.Exceptions;
using dotless.Core.Parser;
using dotless.Core.Parser.Infrastructure;
using dotless.Core.Parser.Tree;
using Entify.Spider.Scripting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Entify
{
 
    
       
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
      //      CefSharp.CEF.RegisterJsObject("Entify", null);
          
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form1 = new Form1();
            Application.Run(form1);
        }
    }
}

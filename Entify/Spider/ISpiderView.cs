using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entify.Spider.Preprocessor;
using Entify.Spider.Scripting;

namespace Entify.Spider
{
    public interface ISpiderView
    {
        IScriptEngine Runtime { get; set; }
        Preprocessor.IPreprocessor Preprocessor { get; set; }
        Object Token { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entify.Spider.Preprocessor
{
    /// <summary>
    /// Template engine
    /// </summary>
    public interface IPreprocessor
    {

        /// <summary>
        /// Preprocess
        /// </summary>
        /// <param name="template"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        string Preprocess(String template, Object code, bool onlyPreprocess = false);
    }
}
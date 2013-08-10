﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entify.Spider.Scripting
{
    /// <summary>
    /// A memory table is mapping between script engines and initial script engines pushing
    /// </summary>
    public class MemoryTable : Dictionary<String, Object>
    {


    }
    /// <summary>
    /// Script engine for Spider applications
    /// </summary>
    public interface IScriptEngine
    {
        /// <summary>
        /// Converts a memory table to native table
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        object TableToNative(MemoryTable table);
        /// <summary>
        /// Invokes a function
        /// </summary>
        /// <param name="function">Function name</param>
        /// <param name="arguments">Arguments to send</param>
        object[] InvokeFunction(String function, params Object[] arguments);
        object[] InvokeFunction(Object function, params Object[] arguments);
        /// <summary>
        /// Gets the content type of the script engine
        /// </summary>
        String ContentType { get; }
        /// <summary>
        /// Loads code into memory
        /// </summary>
        /// <param name="fileName"></param>
        void LoadFile(String fileName);

        /// <summary>
        /// Load raw code
        /// </summary>
        /// <param name="code"></param>
        void LoadScript(String code);
        /// <summary>
        /// Runs the code
        /// </summary>
        /// <param name="code"></param>
        object[] RunCode(String code);

        /// <summary>
        /// Register a function in the cloud
        /// </summary>
        /// <param name="function"></param>
        /// <param name="func"></param>
        void RegisterFunction(String function, System.Reflection.MethodBase func, Object target);
        void RegisterFunction(String function, Delegate func, Object target);
        void SetVariable(String variable, object val);
        /// <summary>
        /// Gets the view the script is attached to
        /// </summary>
        ISpiderView View { get; }

        void RegisterFunction(string p, object obj, System.Reflection.MethodInfo methodInfo);
    }
}
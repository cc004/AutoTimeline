using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace PluginLoader
{
    public abstract class Plugin
    {
        protected readonly static Harmony harmony = new Harmony("loader");
        public virtual int Priority => 0;
        public abstract void Initialize();
        protected static void Log(string msg) => Loader.Log(msg);
        protected static void LogTrace(string msg) => Loader.Log(msg + "\n" + new StackTrace());
    }
}

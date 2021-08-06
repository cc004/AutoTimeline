using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;

namespace PluginLoader
{
    public class Loader
    {
        private static void ClearLog()
        {
            File.WriteAllText("loader.log", string.Empty);
        }

        internal static void Log(string msg)
        {
            File.AppendAllText("loader.log", msg + "\n");
        }

        private static Assembly Load(string filename)
        {
            Log($"loading plugin from {filename}");
            return Assembly.LoadFrom(filename);
        }

        private static Plugin Initiate(Type type)
        {
            Log($"initiating {type.Name} from {type.Assembly.GetName().Name}");
            return Activator.CreateInstance(type) as Plugin;
        }

        private static bool loaded = false;
        public static void Main()
        {
            if (loaded) return;
            loaded = true;
            new Thread(() =>
            {
                try
                {
                    ClearLog();
                    Log("plugin loader is now ready to load plugins");
                    Directory.CreateDirectory("Plugins");

                    var plugins = Directory.GetFiles("Plugins").Where(file => file.EndsWith(".dll"))
                        .Select(file => Load(file)).ToDictionary(asm => asm.FullName, asm => asm);


                    AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
                    {
                        if (plugins.TryGetValue(args.Name, out var asm))
                        {
                            Log($"plugin dependency resolved: {args.Name} => {Path.GetFileName(asm.Location)}");
                            return asm;
                        }
                        Log($"plugin dependency resolve failed: {args.Name}");
                        return null;
                    };

                    Application.logMessageReceivedThreaded += (condition, stackTrace, type) =>
                        Log($"[{type}] {condition}\n{stackTrace}{new StackTrace()}");

                    foreach (var plugin in plugins
                        .SelectMany(asm => asm.Value.GetTypes())
                        .Where(type => type.IsSubclassOf(typeof(Plugin)))
                        .Select(type => Initiate(type))
                        .OrderByDescending(plugin => plugin.Priority))
                    {
                        var type = plugin.GetType();
                        Log($"loading {type.Name} from {type.Assembly.GetName().Name}");
                        try
                        {
                            plugin.Initialize();
                        }
                        catch (Exception e)
                        {
                            Log($"ignoring plugin {type.Name} due to failure in loading.\n{e}");
                        }
                    }

                    Log("plugin load finished.");
                }
                catch (Exception e)
                {
                    Log($"Error occurred when trying to load plugins:\n{e}");
                }
            }).Start();
        }
    }
}

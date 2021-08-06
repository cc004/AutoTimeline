using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ResourceLoader
{
    static class Extensions
    {
        public static HarmonyLib.Harmony harmony;

        public static MethodInfo Get(this Type type, string name, params Type[] types)
        {
            return type.GetMethod(name, types);
        }

        public static MethodInfo Get(this Type type, string name)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).Where(m => m.Name == name && !m.IsGenericMethod).First();
        }

        public static void Prefix(this MethodInfo origin, MethodInfo prefix)
        {
            harmony.Patch(origin, prefix: new HarmonyLib.HarmonyMethod(prefix));
        }
        public static void Postfix(this MethodInfo origin, MethodInfo postfix)
        {
            harmony.Patch(origin, postfix: new HarmonyLib.HarmonyMethod(postfix));
        }
    }
}

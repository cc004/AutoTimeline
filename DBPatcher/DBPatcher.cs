using Coneshell;
using Cute;
using PCRCalculator.Tool;
using PluginLoader;
using Sqlite3Plugin;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DBPatcher
{
    public class DBPatcher : Plugin
    {
        public static void PreprocessDB(DKFNICOAOBC __instance, string filename, byte[] cdbbytes)
        {
            if (!PCRSettings.useDBinStreamingAssestPath)
            {
                Log("db patch is only available when db is in streaming assets path");
                return;
            }
            try
            {
                var newpath = PCRSettings.DBPath + ".tmp";
                File.Copy(PCRSettings.DBPath, newpath, true);
                __instance.OpenWritable(newpath);
                __instance.Begin();


                byte[] bytes = Encoding.UTF8.GetBytes(File.ReadAllText("dbdiff.sql")+"\0");
                IntPtr intPtr;
                int num = ADAKPPDHFFB.sqlite3_exec(__instance.CPJHOACKHFI, bytes, IntPtr.Zero, IntPtr.Zero, out intPtr);
                bool flag = num != 0;
                if (flag)
                {
                    string onljihpignl = (intPtr == IntPtr.Zero) ? "" : Marshal.PtrToStringAnsi(intPtr);
                    throw new Exception(onljihpignl);
                }

                __instance.Commit();
                __instance.CloseDB();
                PCRSettings.DBPath = newpath;

                Log("db patched suc");
            }
            catch (Exception e)
            {
                Log($"db patch failed:\n{e}");
            }
        }

        public override void Initialize()
        {
            harmony.Patch(typeof(DKFNICOAOBC).GetMethod("OpenCustomVFS"), new HarmonyLib.HarmonyMethod(typeof(DBPatcher).GetMethod("PreprocessDB")));
        }
    }
}

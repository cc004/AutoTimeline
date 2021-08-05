using PluginLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Elements;
using Cute;

namespace ResourceLoadLogger
{
    public class ResourceLoadLogger : Plugin
    {
        public static void LoadObject(string path)
        {
            Log($"loading resource: {path}");
        }

        public static void SetObjectList(string NACGNDIHOBK)
        {
            Log($"loaded resource: {NACGNDIHOBK}");
        }

        public static void loadAssetBundleImmediatelyByNameLoadOnly(string _bundleFileName)
        {
            Log($"loading bundle: {_bundleFileName}");
        }

        public override void Initialize()
        {
            var load = typeof(AssetBundle).GetMethods().Where(m => m.Name == "LoadFromFile").ToArray();

            var onload = typeof(ResourceLoadLogger).GetMethod("LoadObject");
            var loadAssetBundleImmediatelyByNameLoadOnly = typeof(ResourceLoadLogger).GetMethod("loadAssetBundleImmediatelyByNameLoadOnly");
            var SetObjectList = typeof(ResourceLoadLogger).GetMethod("SetObjectList");

            harmony.Patch(typeof(AssetManager).GetMethods().First(m => m.Name == "SetObjectList")
                , new HarmonyLib.HarmonyMethod(SetObjectList));
            
            foreach (var m1 in load)
            {
                try
                {
                    harmony.Patch(m1, new HarmonyLib.HarmonyMethod(onload));
                    Log("patched.");
                }
                catch (Exception e)
                {
                    Log(e.ToString());
                }
            }

        }
    }
}

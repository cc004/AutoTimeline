using Cute;
using Elements;
using HarmonyLib;
using PluginLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ResourceLoader
{
    public class ResourceLoader : Plugin
    {
        private static bool IsPatching(string path)
        {
            return File.Exists(Path.Combine("Assets", path));
        }

        public static bool BuildAssetLocalCachePath(ref string __result, string path)
        {
            if (IsPatching(path))
            {
                __result = Path.Combine("Assets", path);
                Log($"patched bundle: {__result}");
                return false;
            }
            return true;
        }

        // Token: 0x06012253 RID: 74323 RVA: 0x00549830 File Offset: 0x00547A30
        public static void BuildAssetLocalCachePathPreinSupported(ref string __result, string JAIOIKNODJE, string PPHNKDPCBPE)
        {
            Log($"Sound Load {JAIOIKNODJE}{PPHNKDPCBPE}");
            BuildAssetLocalCachePath(ref __result, JAIOIKNODJE + PPHNKDPCBPE);
            __result = Path.GetFullPath(__result);

        }
        /*
        public static void LoadObject(ref UnityEngine.Object __result, string ACMBKCJFMPE, Type EGOFKFODHFI, bool DPAHNJCCLIA)
        {
            Log($"object loaded {ACMBKCJFMPE} => {__result}");
        }

        public static void getRegistList(AssetBundle _assetBundle, bool _isShader)
        {
            Log($"LoadAllAssets {_isShader} {_assetBundle}");
        }

        public static void AddLoadBundleId(eBundleId _bundleId, long _bundleIndex = 0L, bool _ableAbandon = false, bool _isNothingToError = true)
        {
            Log($"AddLoadBundleId {_bundleId} {_bundleIndex}");
        }

        public static void loadAssetBundleImmediatelyByName(string _bundleFilename, bool _isNothingToError = true)
        {
            Log($"loadAssetBundleImmediatelyByName {_bundleFilename}");
        }

        public static void LoadAcbFile(CriFsBinder binder, string acbPath, string awbPath)
        {
            Log($"criware loading sound from {acbPath} and {awbPath}");
        }
        */
        static Dictionary<string, LFEKLJKFNPE> myAssetHandleDict = new Dictionary<string, LFEKLJKFNPE>();

        public static void GetAssetHandle(ref LFEKLJKFNPE __result, string GLFABALMDMK, bool OJPLPGDIFFA = true)
        {
            if (__result == null && IsPatching(GLFABALMDMK))
            {
                Log($"Empty Handle for {GLFABALMDMK}");
                if (!myAssetHandleDict.TryGetValue(GLFABALMDMK, out __result))
                {
                    __result = new LFEKLJKFNPE(GLFABALMDMK, ""/*hash*/, ""/*cat*/, ""/*size*/, LFEKLJKFNPE.OHMMHCICKCF.AssetBundle);
                    myAssetHandleDict.Add(GLFABALMDMK, __result);
                }
                __result.NJKMBFLNJDE = false; //is loaded=false to force reload
            }
        }

        private static IEnumerator MyConnect(NetworkManager _this, bool FCPNJHAJLBH)
        {
            yield break;
        }

        public static bool PatchConnect(ref IEnumerator __result, bool FCPNJHAJLBH, NetworkManager __instance)
        {
            __result = MyConnect(__instance, FCPNJHAJLBH);
            return false;
        }

        public override void Initialize()
        {
            Extensions.harmony = harmony;

            typeof(NetworkManager).Get("Connect").Prefix(typeof(ResourceLoader).Get("PatchConnect"));

            typeof(AssetManager).Get("BuildAssetLocalCachePath", typeof(string), typeof(bool), typeof(bool), typeof(bool)).Prefix(typeof(ResourceLoader).Get("BuildAssetLocalCachePath"));
            typeof(AssetManager).Get("BuildAssetLocalCachePathPreinSupported", typeof(string), typeof(string)).Postfix(typeof(ResourceLoader).Get("BuildAssetLocalCachePathPreinSupported"));
            typeof(AssetManager).Get("GetAssetHandle").Postfix(typeof(ResourceLoader).Get("GetAssetHandle"));

            Log($"method info get: {typeof(PABCCELMCAJ).Get("get_LHMAODLAGNH")}");
            //typeof(ResourceManager).Get("getRegistList").Postfix(typeof(ResourceLoader).Get("getRegistList"));
            //typeof(ResourceManager).Get("AddLoadBundleId").Postfix(typeof(ResourceLoader).Get("AddLoadBundleId"));
            //typeof(ResourceManager).Get("loadAssetBundleImmediatelyByName").Postfix(typeof(ResourceLoader).Get("loadAssetBundleImmediatelyByName"));
            //typeof(CriAtomExAcb).Get("LoadAcbFile").Postfix(typeof(ResourceLoader).Get("LoadAcbFile"));
        }

        private void Application_logMessageReceivedThreaded(string condition, string stackTrace, LogType type)
        {
        }
    }
}
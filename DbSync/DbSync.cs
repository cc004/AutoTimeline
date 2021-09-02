using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AssetsTools;
using Coneshell;
using Elements;
using Newtonsoft0.Json.Linq;
using PCRCalculator.Tool;
using PluginLoader;
using Sqlite3Plugin;
using UnityEngine.Networking;

namespace DbSync
{
    using SQliteClient = DKFNICOAOBC;
    using SQliteNative = ADAKPPDHFFB;

    public class SyncData
    {
        public string table;
        public string key;
        public string[] values;
    }

    public class DbSync : Plugin
    {
        private static readonly SyncData[] syncs =
        {
            new SyncData()
            {
                table = "enemy_parameter",
                key = "enemy_id",
                values = new []
                {
                    "level", "rarity", "promotion_level", "hp", "atk", "def", "magic_str", "magic_def", "physical_critical", "magic_critical", "dodge",
                    "wave_hp_recovery", "wave_energy_recovery", "physical_penetrate", "magic_penetrate", "life_steal", "hp_recovery_rate", "energy_recovery_rate",
                    "union_burst_level", "main_skill_lv_1", "main_skill_lv_2", "main_skill_lv_3", "main_skill_lv_4", "main_skill_lv_5", "main_skill_lv_6",
                    "main_skill_lv_7", "main_skill_lv_8", "main_skill_lv_9", "main_skill_lv_10", "ex_skill_lv_1", "ex_skill_lv_2", "ex_skill_lv_3", "ex_skill_lv_4",
                    "ex_skill_lv_5", "resist_status_id", "accuracy", "unique_equipment_flag_1"
                }
            },
            new SyncData()
            {
                table = "unit_attack_pattern",
                key = "pattern_id",
                values = new []
                {
                    "loop_start", "loop_end", "atk_pattern_1", "atk_pattern_2", "atk_pattern_3", "atk_pattern_4", "atk_pattern_5", "atk_pattern_6", "atk_pattern_7",
                    "atk_pattern_8", "atk_pattern_9", "atk_pattern_10", "atk_pattern_11", "atk_pattern_12", "atk_pattern_13", "atk_pattern_14", "atk_pattern_15",
                    "atk_pattern_16", "atk_pattern_17", "atk_pattern_18", "atk_pattern_19", "atk_pattern_20"
                }
            },
            new SyncData()
            {
                table = "unit_skill_data",
                key = "unit_id",
                values = new []
                {
                    "union_burst", "main_skill_1", "main_skill_2", "main_skill_3", "main_skill_4", "main_skill_5", "main_skill_6", "main_skill_7", "main_skill_8",
                    "main_skill_9", "main_skill_10", "ex_skill_1", "ex_skill_evolution_1", "ex_skill_2", "ex_skill_evolution_2", "ex_skill_3", "ex_skill_evolution_3",
                    "ex_skill_4", "ex_skill_evolution_4", "ex_skill_5", "ex_skill_evolution_5", "sp_skill_1", "sp_skill_2", "sp_skill_3", "sp_skill_4", "sp_skill_5",
                    "union_burst_evolution", "main_skill_evolution_1", "main_skill_evolution_2"
                }
            },
            new SyncData()
            {
                table = "skill_action",
                key = "action_id",
                values = new []
                {
                    "class_id", "action_type", "action_detail_1", "action_detail_2", "action_detail_3", "action_value_1", "action_value_2", "action_value_3", "action_value_4",
                    "action_value_5", "action_value_6", "action_value_7", "target_assignment", "target_area", "target_range", "target_type", "target_number", "target_count"
                }
            },
            new SyncData()
            {
                table = "skill_data",
                key = "skill_id",
                values = new []
                {
                    "skill_type", "skill_area_width", "skill_cast_time", "action_1", "action_2", "action_3", "action_4", "action_5", "action_6",
                    "action_7", "depend_action_1", "depend_action_2", "depend_action_3", "depend_action_4", "depend_action_5", "depend_action_6", "depend_action_7",
                    "icon_type"
                }
            }
        };

        private static byte[] masterdb;

        private static IEnumerator DownloadDatabase(IEnumerator origin)
        {
            Log($"db downloading...");
            var req = UnityWebRequest.Get("http://zerohero.pcrsf.cf/update/qa.json");
            req.downloadHandler = new DownloadHandlerBuffer();
            yield return req.SendWebRequest();
            var manifestver = JObject.Parse(Encoding.ASCII.GetString(req.downloadHandler.data)).Value<string>("manifest");
            Log($"manifest: {manifestver}");
            req = UnityWebRequest.Get($"http://l1-dev-patch-gzlj.bilibiligame.net/client_qa2_345/Manifest/AssetBundles/Android/{manifestver}/manifest/masterdata_assetmanifest");
            req.downloadHandler = new DownloadHandlerBuffer();
            yield return req.SendWebRequest();
            var hash = Encoding.ASCII.GetString(req.downloadHandler.data).Split(',')[1];
            Log($"hash:{hash}");
            req = UnityWebRequest.Get($"http://l1-dev-patch-gzlj.bilibiligame.net/client_qa2_345/pool/AssetBundles/Android/{hash.Substring(0, 2)}/{hash}");
            req.downloadHandler = new DownloadHandlerBuffer();
            yield return req.SendWebRequest();
            var db = AssetBundleFile.LoadFromMemory(req.downloadHandler.data).Files[0].ToAssetsFile()
                .Objects[0].Data.Skip(16).ToArray();
            Log($"db download completed ({db.Length} bytes in total)");
            masterdb = db;
            while (origin.MoveNext())
                yield return origin.Current;
        }
        
        public static void PostonClickAfterProcess(ref IEnumerator __result)
        {
            __result = DownloadDatabase(__result);
        }

        public static void PreprocessDB(ref bool __result, SQliteClient __instance, string KCCLLCOPPHF, byte[] AIAPAJCMNHD)
        {
            string patchText = "";
            StringBuilder sb = new StringBuilder();

            using (var master = new SQliteClient())
            {
                File.WriteAllBytes("temp.db", masterdb);
                master.OpenWritable("temp.db");
                foreach (var sync in syncs)
                {
                    using (var reader =
                        master.Query(
                            $"SELECT {string.Join(",", new[] { sync.key }.Concat(sync.values))} FROM {sync.table}"))
                        while (reader.Step())
                            sb.Append(
                                $"UPDATE {sync.table} SET {string.Join(",", sync.values.Select((s, i) => $"'{s}'='{reader.GetDouble(1 + i)}'"))} WHERE {sync.key} = {reader.GetInt(0)};\n");
                }
            }

            File.Delete("temp.db");

            patchText += sb.ToString();

            if (File.Exists("dbdiff.sql"))
                patchText += File.ReadAllText("dbdiff.sql");

            __instance.Begin();
            try
            {

                var bytes = Encoding.UTF8.GetBytes(patchText + "\0");
                var num = SQliteNative.sqlite3_exec(__instance.CPJHOACKHFI, bytes, IntPtr.Zero, IntPtr.Zero, out var intPtr);
                if (num != 0)
                    throw new Exception((intPtr == IntPtr.Zero) ? "" : Marshal.PtrToStringAnsi(intPtr));

                __instance.Commit();
                Log("db patched suc");
                __result = true;

            }
            catch (Exception e)
            {
                __instance.Rollback();
                Log($"db patch failed:\n{e}");
            }
        }

        public override void Initialize()
        {
            harmony.Patch(typeof(SQliteClient).GetMethod("OpenCustomVFS"),
                postfix:new HarmonyLib.HarmonyMethod(typeof(DbSync).GetMethod("PreprocessDB")));
            harmony.Patch(typeof(ViewTitle).GetMethod("onClickAfterProcess", BindingFlags.NonPublic | BindingFlags.Instance),
                postfix:new HarmonyLib.HarmonyMethod(typeof(DbSync).GetMethod("PostonClickAfterProcess")));
        }
    }
}

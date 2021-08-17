
using Elements;
using Elements.Battle;
using HarmonyLib;
using Newtonsoft0.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine.Networking;

namespace LogUploader
{
    public class BattleLogExport : PluginLoader.Plugin
    {
        private static int BlackoutToFrame(float time)
        {
            float dtime = 1 / 60f, counter = 0;
            int frame = 1;
            while ((counter += dtime) <= time)
            {
                ++frame;
            }

            return frame;
        }

        private static Dictionary<int, UnitParameter> param;

        private static string UnitDetail(UnitCtrl info)
        {
            UnitParameter data = (param = param ?? Singleton<UserData>.Instance.UnitParameterDictionary)[info.UnitId];
            string estring = string.Concat(data.UniqueData.EquipSlot.Select(e => e.IsSlot ? e.EnhancementLevel.ToString() : "-"));
            string lstring = string.Join(",", new SkillLevelInfo[]
                {
                data.UniqueData.MainSkill[0], data.UniqueData.MainSkill[1], data.UniqueData.UnionBurst[0], data.UniqueData.ExSkill[0]
                }.Select(l => l.SkillLevel));
            string uel = data.UniqueData.UniqueEquipSlot?.FirstOrDefault()?.EnhancementLevel.ToString();
            return $"{info.Rarity}*r{(int)info.PromotionLevel} {info.Level}({estring}){uel} ({lstring})";
        }

        private static string ToTime(long time, int limit)
        {
            return $"{time / 3600}:{(time / 60 % 60):D2}:{(time % 60):D2} ({60 * limit - time:D4}";
        }

        private static FieldInfo battlelog = typeof(UnitCtrl).GetField("staticBattleLog", BindingFlags.NonPublic | BindingFlags.Static);
        private static FieldInfo battleLogList = typeof(GJNIHENNINA).GetField("battleLogList", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo battleProcessor = typeof(BattleManager).GetField("battleProcessor", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo unitActionController = typeof(UnitCtrl).GetField("unitActionController", BindingFlags.NonPublic | BindingFlags.Instance);
        public static void OnWaveEnd(BattleManager __instance)
        {
            Log("appendWaveEndLog hooked, generating lua script to timeline.lua");
            GJNIHENNINA log1 = battlelog.GetValue(null) as GJNIHENNINA;
            List<BattleLogData> log = battleLogList.GetValue(log1) as List<BattleLogData>;
            int seed = (battleProcessor.GetValue(__instance) as MLDKPCCPIOC).GetSeed();
            MasterDataManager dbmgr = ManagerSingleton<MasterDataManager>.Instance;

            var cdict = __instance.GetMyUnitList()
                .Where(u => log.Any(logline => logline.target_unit_id == u.UnitId))
                .ToDictionary(u => u.UnitId, u => new { name = (string)u.UnitParameter.MasterData.UnitName, unit = u });
            CodeStage.AntiCheat.ObscuredTypes.ObscuredString enemy = dbmgr.masterUnitEnemyData.Get(__instance.BossUnit.PrefabId).unit_name;
            Dictionary<int, int> dmgs = __instance.GetMyUnitList().ToDictionary(i => i.UnitId, i => i.UnitDamageInfo.damage);
            StringBuilder msg = new StringBuilder();
            StringBuilder src = new StringBuilder();
            StringBuilder srct = new StringBuilder();

            foreach (var tuple in cdict)
            {
                src.AppendLine($"print(\"calibrate for {tuple.Value.name}\");");
                src.AppendLine($"autopcr.calibrate(\"{tuple.Value.name}\");");
                srct.AppendLine($"print(\"calibrate for {tuple.Value.name}\");");
                srct.AppendLine($"autopcr.calibrate(\"{tuple.Value.name}\");");
            }

            int damage = dmgs.Where(pair => pair.Key <= 999999).Sum(pair => pair.Value);
            msg.AppendLine(
                $"对\"{enemy}\"（{seed}:{__instance.BossUnit.UnitParameter.MasterData.UnitName}-{__instance.BossUnit.UnitId}）造成{damage}伤害：");
            msg.AppendLine(string.Join("\n",
                 cdict.Select(c => $"{c.Value.name}\t（{dmgs[c.Value.unit.UnitId]}）\t{UnitDetail(c.Value.unit)}")));

            //msg.AppendLine($"boss: autopcr.getBossAddr({__instance.BossUnit.UnitId})");
            msg.AppendLine("帧轴：");

            int skippingFrame = 0;
            src.AppendLine("autopcr.setOffset(2, 0); --offset calibration");
            srct.AppendLine("autopcr.setOffset(2, 0); --offset calibration");

            cdict.Add(__instance.BossUnit.UnitId, new { name = string.Empty, unit = __instance.BossUnit });
            int limit = 90;// __instance.GetMiliLimitTime() / 1000;
            int frame = 0;

            foreach (BattleLogData logline in log)
            {
                frame = logline.frame - skippingFrame;
                if (logline.battle_log_type != (int)eBattleLogType.BUTTON_TAP)
                {
                    continue;
                }

                int unit_id = logline.target_unit_id;
                Log(unit_id.ToString());
                if (unit_id <= 999999)
                {
                    CodeStage.AntiCheat.ObscuredTypes.ObscuredString name = dbmgr.masterUnitData.Get(unit_id).UnitName;
                    msg.AppendLine($"{ToTime(limit * 60 - frame, limit)}:{logline.frame})\t{name}");
                    src.AppendLine($"autopcr.waitFrame({logline.frame}); autopcr.press(\"{name}\");");
                    srct.AppendLine($"autopcr.waitLFrame({frame}); autopcr.press(\"{name}\");");
                }
                else
                {
                    CodeStage.AntiCheat.ObscuredTypes.ObscuredString name = dbmgr.masterEnemyParameter.GetFromAllKind(unit_id).name;
                    msg.AppendLine($"{ToTime(60 * limit + skippingFrame - logline.frame, limit)}:{logline.frame})\t{name}");
                }
                skippingFrame += BlackoutToFrame((unitActionController.GetValue(cdict[unit_id].unit) as UnitActionController).UnionBurstList[0].BlackOutTime);
            }

            src.AppendLine($"--[[\n{msg}\n]]");
            srct.AppendLine($"--[[\n{msg}\n]]");
            File.WriteAllText("timeline.lua", src.ToString());
            File.WriteAllText("timeline_logic.lua", srct.ToString());
            
            UnityWebRequest ur = new UnityWebRequest("https://sf.pcrsf.tk:8443/record", "POST");
            ur.SetRequestHeader("Content-Type", "application/json");
            var id = __instance.BossUnit.UnitId;
            ur.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(new JObject
            {
                ["Data"] = src.ToString(),
                ["Name"] = $"{string.Concat(cdict.Values.Select(i => i.name))}-10{id / 100 % 100:d2}-{(char)('A' + id / 10000 % 10 - 1)}{id % 10}-{damage/10000}w.txt"
            }.ToString()));
            ManagerSingleton<ApiManager>.Instance.StartCoroutine(FinishRequest(ur));

            Log("timeline.lua generated.");
        }

        private static IEnumerator FinishRequest(UnityWebRequest ur)
        {
            yield return ur.SendWebRequest();
            ur.Dispose();
        }

        public override void Initialize()
        {
            MethodInfo waveend = typeof(BattleManager).GetMethod("appendWaveEndLog", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            harmony.Patch(waveend, postfix: new HarmonyMethod(typeof(BattleLogExport).GetMethod("OnWaveEnd")));
        }
    }

}

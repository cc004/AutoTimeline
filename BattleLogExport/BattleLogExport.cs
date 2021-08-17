using Elements;
using Elements.Battle;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BattleLogExport
{
    public class BattleLogExport : PluginLoader.Plugin
    {
        private static int BlackoutToFrame(float time)
        {
            float dtime = 1 / 60f, counter = 0;
            var frame = 1;
            while ((counter += dtime) <= time) ++frame;
            return frame;
        }

        private static Dictionary<int, UnitParameter> param;

        private static string UnitDetail(UnitCtrl info)
        {
            var data = (param = param ?? Singleton<UserData>.Instance.UnitParameterDictionary)[info.UnitId];
            var estring = string.Concat(data.UniqueData.EquipSlot.Select(e => e.IsSlot ? e.EnhancementLevel.ToString() : "-"));
            var lstring = string.Join(",", new SkillLevelInfo[]
            {
                data.UniqueData.MainSkill[0], data.UniqueData.MainSkill[1], data.UniqueData.UnionBurst[0], data.UniqueData.ExSkill[0]
            }.Select(l => l.SkillLevel));
            var uel = data.UniqueData.UniqueEquipSlot?[0]?.EnhancementLevel.ToString();
            return $"{info.Rarity}*r{(int)info.PromotionLevel} {info.Level}({estring}){uel} ({lstring})    autopcr.getUnitAddr({info.UnitId}, {info.Rarity}, {info.PromotionLevel})";
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
            var log1 = battlelog.GetValue(null) as GJNIHENNINA;
            var log = battleLogList.GetValue(log1) as List<BattleLogData>;
            int seed = (battleProcessor.GetValue(__instance) as MLDKPCCPIOC).GetSeed();
            var dbmgr = ManagerSingleton<MasterDataManager>.Instance;

            var cdict = __instance.GetMyUnitList().ToDictionary(u => u.UnitId, u => new{ name=(string)(dbmgr.masterUnitData.Get(u.UnitId).UnitName), unit=u });
            var enemy = dbmgr.masterUnitEnemyData.Get(__instance.BossUnit.PrefabId).unit_name;
            var dmgs = __instance.GetMyUnitList().ToDictionary(i => i.UnitId, i => i.UnitDamageInfo.damage);
            var msg = new StringBuilder();
            var src = new StringBuilder();
            var srct = new StringBuilder();

            foreach (var tuple in cdict)
            {
                src.AppendLine($"print(\"calibrate for {tuple.Value.name}\");");
                src.AppendLine($"autopcr.calibrate(\"{tuple.Value.name}\");");
                srct.AppendLine($"print(\"calibrate for {tuple.Value.name}\");");
                srct.AppendLine($"autopcr.calibrate(\"{tuple.Value.name}\");");
            }

            var damage = dmgs.Where(pair => pair.Key <= 999999).Sum(pair => pair.Value);
            msg.AppendLine(
                $"对\"{enemy}\"（{seed}:{__instance.BossUnit.name}-{__instance.BossUnit.UnitId}）造成{damage}伤害：");
            msg.AppendLine(string.Join("\n",
                 cdict.Select(c => $"{c.Value.name}\t（{dmgs[c.Value.unit.UnitId]:D6}）\t{UnitDetail(c.Value.unit)}")));

            msg.AppendLine($"boss: autopcr.getBossAddr({__instance.BossUnit.UnitId})");
            msg.AppendLine("帧轴：");

            var skippingFrame = 0;
            src.AppendLine("autopcr.setOffset(2, 0); --offset calibration");
            srct.AppendLine("autopcr.setOffset(2, 0); --offset calibration");

            cdict.Add(__instance.BossUnit.UnitId, new { name = string.Empty, unit = __instance.BossUnit });
            var limit = 60 * 90;// __instance.GetMiliLimitTime() / 1000;

            foreach (var logline in log)
            {
                Log(logline.battle_log_type.ToString());
                if (logline.battle_log_type != (int)eBattleLogType.BUTTON_TAP) continue;
                var frame = logline.frame - skippingFrame;
                var unit_id = logline.target_unit_id;
                Log(unit_id.ToString());
                if (unit_id <= 999999)
                {
                    var name = dbmgr.masterUnitData.Get(unit_id).UnitName;
                    msg.AppendLine($"{ToTime(limit - frame, limit)}:{logline.frame})\t{name}");
                    src.AppendLine($"autopcr.waitFrame({logline.frame}); autopcr.press(\"{name}\");");
                    srct.AppendLine($"autopcr.waitLFrame({frame}); autopcr.press(\"{name}\");");
                }
                else
                {
                    var name = dbmgr.masterEnemyParameter.GetFromAllKind(unit_id).name;
                    msg.AppendLine($"{ToTime(60 * limit + skippingFrame - logline.frame, limit)}:{logline.frame})\t{name}");
                }
                skippingFrame += BlackoutToFrame((unitActionController.GetValue(cdict[unit_id].unit) as UnitActionController).UnionBurstList[0].BlackOutTime);
            }

            src.AppendLine($"--[[\n{msg}\n]]");
            srct.AppendLine($"--[[\n{msg}\n]]");

            File.WriteAllText("timeline.lua", src.ToString());
            File.WriteAllText("timeline_logic.lua", srct.ToString());
            Log("timeline.lua generated.");
        }

        public override void Initialize()
        {
            var waveend = typeof(BattleManager).GetMethod("appendWaveEndLog", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            harmony.Patch(waveend, prefix: new HarmonyMethod(typeof(BattleLogExport).GetMethod("OnWaveEnd")));
        }
    }
}

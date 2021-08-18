-- scan character handles
---[[
--[[
local heiqi = autopcr.getUnitAddr(104701, 5, 15);
]]
--[[
local lang = autopcr.getUnitAddr(104301, 5, 13);
local yls = autopcr.getUnitAddr(106301, 5, 15);
local bingjiao = autopcr.getUnitAddr(102701, 5, 13);
local kezong = autopcr.getUnitAddr(107101, 5, 13);
local boss = autopcr.getBossAddr(401041402);
--]]
--local shengchui = autopcr.getUnitAddr(108601, 5, 12);

-- data for 1600x900
--[[ minitouch test
minitouch.connect("localhost", 1111);
for i = 0, 4 do
    minitouch.setPos(5 - i, 400 + i * 208, 860);
end
minitouch.setPos(6, 1544, 716); --auto
minitouch.setPos(7, 1544, 839); --forward
minitouch.setPos(8, 1512, 43);  --pause
--]]
--[[

local bingjiao   = autopcr.getUnitAddr(102701, 5, 13);
local boss = autopcr.getBossAddr(401041402);
]]
--[[ mouse calibration
for i = 1, 5 do
    autopcr.calibrate(i);
end
--autopcr.calibrate(8);
--]]
--[[
function getPhysical(unit_id, rarity, rank, name, key, crits, n, def)
    return {
        unit_handle = autopcr.getUnitAddr(unit_id, rarity, rank),
        name = name,
        key = key,
        crits = crits,
        n = n,
        isMagic = false,
        critnum = 0,
        def = def
    }
end

function getMagic(unit_id, rarity, rank, name, key, crits, n, def)
    return {
        unit_handle = autopcr.getUnitAddr(unit_id, rarity, rank),
        name = name,
        key = key,
        crits = crits,
        n = n,
        isMagic = true,
        critnum = 0
    }
end

-- 配置
local charas = {
    -- id, 星级, rank, 通用名, 按键绑定，暴击位置, 暴击数, 自动ub防御上限
    getPhysical(103801, 5, 13, "tp", "Q", {3}, 1, 999), -- 单段伤害就是2， 1代表一共1个暴击
    getPhysical(106301, 4, 13, "yls", "W", {2}, 1, 100),
    getPhysical(170101, 5, 13, "环奈", "E", {2}, 0, 0),
    getPhysical(107101, 5, 13, "克总", "R", {2}, 0, 0),
    getPhysical(104301, 5, 13, "狼", "T", {2}, 1, 0),
}
--boss id
local boss = autopcr.getBossAddr(401031403);


for i = 1, 5 do
    autopcr.calibrate(charas[i].name)
end
autopcr.calibrate("暂停");

while (true)
do
    if (autopcr.getTime() < 0.1)
    then
        autopcr.framePress("暂停");
    end
    local def = autopcr.getDef(boss);
    for i = 1, 5 do
        if (input.keyPressed(charas[i].key) or def < charas[i].def)
        then
            local crits = autopcr.critNum(charas[i].unit_handle, boss, charas[i].isMagic, charas[i].crits);
            print('now crit='..crits, '/'..charas[i].n);
            local iscrit = crits >= charas[i].n;
            if (iscrit) then
                charas[i].critnum = charas[i].critnum + 1;
                if (charas[i].critnum == 5) then
                    autopcr.framePress(charas[i].name);
                    charas[i].critnum = 0;
                end
            else
                charas[i].critnum = 0;
            end
        else
            charas[i].critnum = 0;
        end
    end
end



]]
--[[ detailed logic

while (autopcr.getTime() > .5) --when not end
do
--[[ multiple attack test
    local crit = autopcr.getCrit(bingjiao, boss, false)
    local crits = autopcr.nextCrits({4, 6, 12}) -- identity crit series for bingjiao ub
    print('crit='..(crits[0]<crit), ','..(crits[1]<crit), ','..(crits[2]<crit))
]]

--[[
    local def = autopcr.getDef(boss);
    if (def <= 0 or autopcr.getTime() < 1 or autopcr.getTime() >= 70 and def <= 10)
    then
        if (autopcr.getTp(bingjiao) == 1000 and autopcr.getPhysicalCritical(bingjiao) >= 600)
        then
            print('boss_def='..def);
            autopcr.waitTillCrits(bingjiao, boss, false, 180 + autopcr.getFrame(), 2, {4, 6, 12});
            autopcr.framePress(3);
        end
        if (autopcr.getTp(kezong) == 1000)
        then
            print('boss_def='..def);
            autopcr.framePress(4);
        end
        if (autopcr.getTp(yls) == 1000)
        then
            print('boss_def='..def);
            autopcr.waitTillCrit(yls, boss, false, 180 + autopcr.getFrame());
            autopcr.framePress(5);
        end
    end
    if (def <= 50 or autopcr.getTime() < 1)
    then
        if (autopcr.getTp(lang) == 1000)
        then
            print('boss_def='..def);
            autopcr.waitTillCrit(lang, boss, false, 180 + autopcr.getFrame());
            autopcr.framePress(2);
        end
    end
--]]
--end

--]]
--[[ auto ub
while (autopcr.getTime() > 1) --when not end
do
    print('boss_def='..autopcr.getDef(boss));
    for i = 2, 5 do --judge every chara
        if (autopcr.getTp(charas[i]) == 1000 and
           (autopcr.getDef(boss) <= 160 or i == 2 and autopcr.getDef(boss) <= 250)) --ready for tp
        then
            print('boss_def='..autopcr.getDef(boss));
            autopcr.waitTillCrit(charas[i], boss, false, 180 + autopcr.getFrame());
            print('trying to press '..i, 'frame='..autopcr.getFrame(), 'def='..autopcr.getDef(boss))
            --autopcr.framePress(i);
            minitouch.framePress(i); --trigger ub press
            break;
        end
    end
    autopcr.waitOneFrame();
end
--]]

--[[ benchmark
autopcr.setOffset(2, 0); -- hyperparam, 2 frame offset for minitouch

autopcr.waitFrame(2000);
minitouch.press(1);
autopcr.waitFrame(2500);
minitouch.press(2);
autopcr.waitFrame(3000);
minitouch.press(3);
autopcr.waitFrame(3500);
minitouch.press(4);
autopcr.waitFrame(4000);
minitouch.press(5);
--]]

--[[ crit test
local chara = autopcr.getUnitAddr(106301, 5, 15)
local boss = 85;
local last = 0;
]]
--[[
autopcr.calibrate(0);
while (true)
do
    local crits = autopcr.nextCrits({5});
    local now = crits[0];
    if (now ~= last)
    then
        last = now;
        local crit = autopcr.getCrit(shengchui, shengchui, false);
        if (now < crit)
        then
            print('圣锤必定暴击自己 crit rate = '..crit, 'next crit = '..now);
            autopcr.framePress(0);
        else
            print('圣锤必定不暴击自己 crit rate = '..crit, 'next crit = '..now);
        end
    end
end
--]]

--[[state test
local ke = autopcr.getUnitAddr(107101, 5, 13);

while (true) do
    print(autopcr.getActionState(ke)..' '..autopcr.getCastTimer(ke))
end
--]]

--[[ async test
local count = 0;

function asynctest1()
    print("async1 starting")
    while (true) do
        async.await()
        count=count+1
    end
end

function asynctest2()
    print("async2 starting")
    while (true) do
        async.await()
        count=count+1
    end
end

async.start(asynctest1)
async.start(asynctest2)
print("asynctest stated");
while (true) do
    async.await()
    count=count+1
    
    if (count % 300 == 1) then
        print(count)
    end
end
--]]

--[[ monitor test

local kezong = autopcr.getUnitAddr(107101, 5, 14);
monitor.add("克总", kezong);
autopcr.calibrate("克总");
autopcr.setOffset(2, 0);

while (true) do
    monitor.waitSkill("克总", 1071003, 56);
    if (autopcr.getTp(kezong) == 1000) then
        autopcr.press("克总");
    end
    monitor.waitSkill("克总", 0, 0);
end

--]]

print("测试新api");
autopcr.SwitchToGameInit();
local test_boss_id=401031505;
local test_part_id=401031506;
boss_phase = unitautodata.GetBossPhase(test_boss_id);
boss_battle_id = unitautodata.GetBossClanId(test_boss_id);
boss_name = unitautodata.GetBossName(test_boss_id);
boss_child_list = unitautodata.GetBossChildId(test_boss_id);
print("Boss的会战ID为",boss_battle_id," Boss的阶段为:",boss_phase," Boss的名字为:",boss_name)
for i=0,boss_child_list.Count-1 do
print(boss_child_list[i].Item1,boss_child_list[i].Item2)
autopcr.SwitchToGame()

end
part_father=unitautodata.GetFatherId(test_part_id);
part_name=unitautodata.GetBossPartsName(test_part_id);
print("部位的名字为:",part_name," 部位的宿主为:",part_father," 部位的宿主名字为:",unitautodata.GetBossName(part_father));
fast_useraddr=autopcr.getUnitAddrEasy(107101);
print("快速获取地址",fast_useraddr);

local unitarray=autopcr.AutogetUnitAddr(); --这里unitarray是C#.Net框架标准库的List<(int,long,int,string)>格式，获取数据的方法与C#语法一致
print("角色扫描结束");
local bossarray=autopcr.AutogetBossAddr(); --这里bossarray是C#.Net框架标准库的List<(int,long,int,string)>格式，获取数据的方法与C#语法一致
print("BOSS和部位扫描结束");


print("以下是角色扫描结果：");
for i=0,unitarray.Count-1 do
    print("序号",unitarray[i].Item1," 句柄",unitarray[i].Item2," 角色ID",unitarray[i].Item3," 角色名称",unitarray[i].Item4);
end
print("以下是Boss或部位扫描结果：")
for i=0,bossarray.Count-1 do
    print("序号",bossarray[i].Item1," 句柄",bossarray[i].Item2," BossID",bossarray[i].Item3," Boss或部位名称",bossarray[i].Item4);
end


print("请根据扫描结果输入输入正确Boss的序号：");
boss_order=io.read();
bossid=bossarray[boss_order].Item3;
boss=bossarray[boss_order].Item2;
print("请根据扫描结果输入一号位角色的序号：");
unit_order=io.read();
firstid=unitarray[unit_order].Item3;
first=unitarray[unit_order].Item2;
print("请根据扫描结果输入二号位角色的序号：");
unit_orde=io.read();
secondid=unitarray[unit_order].Item3;
second=unitarray[unit_order].Item2;
print("请根据扫描结果输入三号位角色的序号：");
unit_orde=io.read();
thirdid=unitarray[unit_order].Item3;
third=unitarray[unit_order].Item2;
print("请根据扫描结果输入四号位角色的序号：");
unit_orde=io.read();
fourthid=unitarray[unit_order].Item3;
fourth=unitarray[unit_order].Item2;
print("请根据扫描结果输入五号位角色的序号：");
unit_orde=io.read();
fifthid=unitarray[unit_order].Item3;
fifth=unitarray[unit_order].Item2;

print("开启监视角色状态协程");
monitor.add("一号位", first);
monitor.add("二号位", second);
monitor.add("三号位", third);
monitor.add("四号位", fourth);
monitor.add("五号位", fifth);
print("开启完成");

print(monitor.getSkillId("一号位"));

function end_process()
	lastDef=-1;
	lastMagicDef=-1;
	endflag = 0;
	while (true) do
	    if (autopcr.getLFrame()>5300) then
			if (endflag==0) then
			   print("最后阶段收尾");
			   endflag = 1;
			end
			if (autopcr.getTp(first) == 1000) then
				press_until_Tp(first,"一号位");
			    end
			if (autopcr.getTp(second) == 1000) then
				press_until_Tp(second,"二号位");
			    end
			if (autopcr.getTp(second) == 1000) then
				press_until_Tp(third,"三号位");
			    end
			if (autopcr.getTp(second) == 1000) then
				press_until_Tp(fourth,"四号位");
			    end
			if (autopcr.getTp(second) == 1000) then
				press_until_Tp(fifth,"五号位");
			    end
			end
		if (autopcr.getLFrame()>5330) then
			autopcr.press("暂停");
			end
		nowDef=autopcr.getDef(boss);
		nowMagicDef=autopcr.getMagicDef(boss)
		if (nowDef~=lastDef or nowMagicDef~=lastMagicDef) then
		    print("当前BOSS物理护甲：",nowDef," 当前BOSS魔法护甲：",nowMagicDef);
			lastDef=nowDef;
			lastMagicDef=nowMagicDef;
			end
		autopcr.waitOneLFrame();
		end
	end
async.start(end_process);

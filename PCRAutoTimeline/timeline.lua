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
local shengchui = autopcr.getUnitAddr(108601, 5, 12);

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
autopcr.calibrate(8);
--]]

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

autopcr.waitTime(.2);
autopcr.framePress(8);
# AutoTimeline

代码基于GPLv3开源，仅供交流winapi如ReadProcessMemory的使用方法，禁止用于其他用途以及任何形式的商业用途。

[lua分享站](http://pcr.youtobechina.com/)  

qq群1023837088

仅适配x64模拟器 x32的app

(什么?你不想写代码就想用?请打开auto)  

更新了部分函数的实现方式，增加了逻辑帧相关的操作  

现在可以根据技能生效帧进行自动目押了（但是对有弹道角色支持有BUG） 

使用时请把Data文件夹数据放在根目录下

更新了角色和BOSS内存地址扫描的逻辑，增加了更便利的内存地址搜索方法

增加了对角色和BOSS相关数据的接口，便于查询

增加了角色自buff的判断

## 用法

### 运行

1. 安装python**3.8**
2. 安装wheel下的四个whl文件
3. 运行timeline.py样例

### 编写timeline.py

第一行务必`from autotimeline import *`

AutoPcrApi:

- `void autopcr.calibrate(id)` 对站位id进行校准（可以为字符串或者数字）
- `void autopcr.press(id)` 点击站位为id的角色，不占用时间，但可能点不上
- `void autopcr.framePress(id)` 点击站位为id的角色，保证点上，占用两帧，一般用于连点
- `void autopcr.multipress(id, dur)` 连点站位为id的角色，持续dur
- `UnitCtrl autopcr.getUnit(id)` 懒得说明

- `long autopcr.getUnitAddr(unit_id, rarity, rank)` 根据数据获取角色的句柄，请务必保证搜索时该角色tp为0且满血，否则会搜索失败
- `<Dictionary<int, string>[] autopcr.autoGetUnitAddr()` 自动获取公会战角色的句柄，多个结果以列表形式存放（就一个的话也是个单元素的列表），返回值为Dictionary格式的数组，数据获取方式与lua table类似，其中第一个值为列表元素序号，第二个值为该单位的句柄，第三个值为该单位的数据库id，第四个值为该单位的名称，如果找不到符合的单位，为空列表
- `long autopcr.getUnitAddrEasy(unit_id)` 不用输入星级和rank就进行句柄扫描的方法，没有getUnitAddr可靠
- `long autopcr.getBossAddr(unit_id)` 获取公会战boss的句柄
- `<Dictionary<int, string>[] autopcr.autoGetBossAddr()` 自动获取公会战boss和可能的多目标部位的句柄(目前多部位有BUG扫不出)，多个结果以列表形式存放（就一个的话也是个单元素的列表），返回值为Dictionary格式的数组，数据获取方式与lua table类似，其中第一个值为列表元素序号，第二个值为该单位的句柄，第三个值为该单位的数据库id，第四个值为该单位的名称，如果找不到符合的单位，为空列表
- `float autopcr.getTp(unit_handle)` 根据获得的句柄返回角色当前tp
- `long autopcr.getHp(unit_handle)` 根据获得的句柄返回角色当前hp
- `long autopcr.getMaxHp(unit_handle)` 根据获得的句柄返回角色最大hp
- `int autopcr.getPhysicalCritical(unit_handle)` 根据获得的句柄返回角色物理暴击
- `int autopcr.getMagicCritical(unit_handle)` 根据获得的句柄返回角色法术暴击
- `int autopcr.getDef(unit_handle)` 根据获得的句柄返回角色物理防御
- `int autopcr.getMagicDef(unit_handle)` 根据获得的句柄返回角色法术防御
- `int autopcr.getLevel(unit_handle)` 根据获得的句柄返回角色等级

- `int autopcr.getFrame()` 返回当前帧数
- `int autopcr.getLFrame()` 返回当前逻辑帧数
- `float autopcr.getTime()` 返回当前时间
- `void autopcr.waitFrame(frame)` 暂停协程直到帧数达到
- `void autopcr.waitLFrame(frame)` 暂停协程直到逻辑帧数达到;现在，利用逻辑帧的等待，将不再输出当前时间和逻辑帧，如果轴佬需要观察当前帧数，可以在脚本中单独调用autopcr.getLFrame(),并print
- `void autopcr.waitOneLFrame()` 暂停协程直到过去一个逻辑帧
- `void autopcr.waitTime(frame)` 暂停协程直到时间达到

- `void autopcr.setOffset(frame_offset, time_offset)` 设定延迟校准参数
- `float autopcr.getCrit(unit_handle, target_handle, isMagic)` 获取对某个target攻击的暴击率
- `uint[] autopcr.predRandom(count)` 获取下count个随机数的值

- `float autopcr.nextCrit()` 获取用于下一次攻击判定的随机数，如果小于critrate则暴击
- `float[] autopcr.nextNCrit(count)` 返回下n次攻击判定的随机数，如果小于critrate则暴击（如果ub带随机效果则可能结果不正确，如病娇）
- `float[] autopcr.nextCrits(critlist)` 返回下几个攻击判定的随机数，用来对抗带随即效果的ub，比如病娇ub的critlist填{4,6,12}，更多多段顺序见critlist.md，如果小于critrate则暴击

- `void autopcr.waitTillCrit(unit_handle, target_handle, isMagic, frameMax)` 等待至多到frameMax, 直到unit下一段伤害必定暴击
- `void autopcr.waitTillNCrit(unit_handle, target_handle, isMagic, frameMax, m, n)` 等待直到下次n段伤害有m个暴击（如果ub带随机效果则可能结果不正确，如病娇）
- `void autopcr.waitTillCrits(unit_handle, target_handle, isMagic, frameMax, m, critlist)` 等待直到下几个攻击判定有m个暴击
- `string autopcr.getActionState(unit_handle)` 获取单位当前状态，取值如下：IDLE, ATK, SKILL_1, SKILL, WALK, DAMAGE, SUMMON, DIE, GAME_START, LOSE
- `int autopcr.getSkillId(unit_handle)` 获取当前角色的技能id，普攻为1

- `void autopcr.switchToGameInit()` 初始化dnplayer进程，如果要使用autopcr.switchToGame()，需要先调用这个函数
- `void autopcr.switchToGame()` 令Windows切换到模拟器上，防止误操作造成乱轴（建议加入连点函数上）


MiniTouchApi:

- `void minitouch.getMaxX()` 返回最大X
- `void minitouch.getMaxY()` 返回最大Y
- `void minitouch.connect(host, port)` 链接minitouch server
- `void minitouch.write(text)` 写minitouch指令
- `void minitouch.setPos(id, x, y)` 注册站位id（可以为字符串或者数字）
- `void minitouch.press(id)` 点击站位为id的角色，不占用时间，但可能点不上
- `void minitouch.framePress(id)` 点击站位为id的角色，保证点上，占用两帧，一般用于连点

InputApi:

- `bool input.keyPressed(key)` 返回键盘是否被按下

AsyncApi:

- `void Async.Start(action)` 开始一个新协程
- `void Async.Await()` 协程进入等待，使其他协程进入运行态
- `void Async.Exit()` 运行直到所有协程退出，请务必保证主线程结束前运行该函数

UnitAutoDataApi:

- `int unitautodata.getAtkPrefabFrame(unit_id)` 根据unit_id获取当前角色普攻生效帧(对弹道和部分物理角色有BUG)，没有数据返回-1
- `int unitautodata.getAtkType(unit_id)` 根据unit_id获取当前角色的输出/破甲类型，1表示物理，2表示魔法
- `int unitautodata.getUbTypeFromId(unit_id)` 根据unit_id获取当前角色的UB类型，1表示输出，2表示奶，3表示破甲，4表示增益
- `int unitautodata.getSelfBuffId(unit_id)` 根据unit_id获取当前角色的自buff技能id，-1表示没有自buff技能记录
- `float unitautodata.getSelfBuffTime(unit_id)` 根据unit_id获取当前角色的自buff持续时间，-1表示没有自buff技能记录
- `int unitautodata.getSkillExFrame(name,skillid)` 获取当前角色的该技能所有动作都生效的逻辑帧(对弹道技能有BUG)
- `string unitautodata.getBossName(boss_id)` 根据boss的id获取该boss的名字（包含会战id和阶段信息），找不到boss返回"未找到该Boss"
- `int unitautodata.getBossPhase(boss_id)` 根据boss的id获取该boss所属的阶段，找不到boss返回-1
- `int unitautodata.getBossClanId(boss_id)` 根据boss的id获取该boss所属的会战ID，找不到boss返回-1
- `(string, long)[] unitautodata.getBossChildId(boss_id)` 根据boss的id获取该boss的部位信息列表包含名称和部位id，返回值是数组，获取数据的方法与C#语法一致，元素的第一个值为所属部位，第二个值为部位id，找不到返回空列表
- `string unitautodata.getBossPartsName(part_id)` 根据部位的id获取该部位的名字，找不到返回"未知部位"
- `long unitautodata.getFatherId(long boss_part_id)` 根据部位的id获取该部位所属的Bossid，找不到boss返回-1

MonitorApi: (experimental)

- `void monitor.add(name, unit_handle)` 把单位加入检测列表中
- `void monitor.waitSkill(name, skill, frame)` 等待unit执行skill多少帧后
- `int monitor.waitSkillLFrame(name, skill, frame)` 等待unit执行skill多少逻辑帧后，修改了等待方式，适配BOSSUB打断，其他角色UB;返回值用于标记技能是否被打断，技能等待是否正常，0表示没有被打断，1表示被打断，2表示异常返回（目前只有循环次数过多造成的异常）
- `int monitor.getSkillId(name)` 获取当前角色的技能id，普攻为1
- `int monitor.getSkillFrame(name)` 获取当前角色的技能开始执行时的渲染帧
- `int monitor.getSkillLFrame(name)` 获取当前角色的技能开始执行时的逻辑帧
- `string monitor.getActionState(name)` 同autopcr同名函数，但是速度更快
- `void monitor.updateSelfBuff(name,unit_id)` 开启自buff监视协程
- `bool monitor.getIsSelfBuffed(name)` 返回是否处于自buff状态


### 依赖

项目依赖于`.net 5.0 runtime`，请自行百度

### 延迟校准

校准代表着模拟器处理造成的延迟，一般会保持不变，技能释放时，如果打开技能动画，帧数会暂停，你可以根据暂停时候的值和预期值做出帧数的校准

### 运行程序

1. 必须使用管理员模式运行，设置帧率为60，关闭倍速，先进入对战，然后在开始时暂停
3. 输入模拟器主程序的PID(不要输错成前台ui程序)，如果是雷电模拟器且只有一个实例这步可以直接按回车
4. 等待扫描，结束后会显示当前帧数和剩余时间
5. 继续模拟器运行脚本，脚本中进行站位校准和自动打轴
6. 继续运行后不要乱动鼠标！！！

### 关于Minitouch

Minitouch可以显著减小模拟器层触控延迟，repo内附带bin版minitouch，[使用说明](https://github.com/DeviceFarmer/minitouch)  
如果有的菜鸡弄不明白怎么用，也可以使用传统方法。  
如果adb在path中可以用minitouch文件夹下的两个bat直接把minitouch开到1111端口(先运行adbshell再运行adbforwarding, adbshell不要关)

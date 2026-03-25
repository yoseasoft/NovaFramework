# `NovaFramework` 开发示例集

> 本文档提供完整的业务开发示例、代码模板和反模式，帮助 AI 开发助手理解在 `NovaFramework` 中编写代码的正确方式。  
> 使用前请确保已阅读 `dev_spec.md`（规则）和 `dev_api.md`（API 接口）。

---

## 1. 完整示例：实现邮件系统

### 1.1 需求描述

玩家可以收取邮件、查看邮件列表、领取邮件附件。邮件数据由服务端下发。  
领取附件后需要给玩家加经验，但邮件组件不能直接调用属性组件。

### 1.2 分析与决策

| 判断项 | 结论 | 依据 |
|-------|------|------|
| 邮件是玩家角色的一个业务分支 | 创建 `CComponent` | 实体的独立行为拆分为组件（规则 2.5） |
| 需要展示邮件列表 UI | 创建 `CView` | 每个 UI 资源对应一种视图类型 |
| 邮件数据来自服务端 | 用 `[OnMessage]` 接收 | 消息数据通知 |
| UI 需要知道邮件数据变化 | 通过事件通知 UI 刷新 | 组件通信规则（规则 2.4） |
| 领取附件后加经验 | 通过事件通知属性组件 | 禁止跨组件直接调用 |
| 所有业务对象继承 U 类 | 继承 `Game.UComponent` / `Game.UView` | 二次封装规则（规则 2.8） |

### 1.3 第一步：创建数据类（Game 数据模组）

#### 文件：`Game/Component/Mail/MailComponent.cs`

```csharp
using GameEngine;

[UComponentClass("MailComponent")]
public sealed class MailComponent : Game.UComponent
{
    /// <summary>邮件列表</summary>
    public List<MailData> mailList = new List<MailData>();

    /// <summary>邮件数据结构（原生类型，非实体对象）</summary>
    public class MailData
    {
        public long mailId;
        public string title;
        public string content;
        public bool isRead;
        public bool hasAttachment;
        public int attachmentExp; // 附件中的经验值
    }

    // ====== 事件结构体（必须 public，定义在组件内部） ======

    /// <summary>邮件列表刷新事件（整体刷新）</summary>
    public struct MailListChangedNotify { }

    /// <summary>单封邮件状态变更事件</summary>
    public struct MailStateChangedNotify
    {
        public long mailId;
    }
}
```

#### 文件：`Game/Component/Attribute/AttributeComponent.cs`（已有，补充事件结构体）

```csharp
using GameEngine;

[UComponentClass("AttributeComp")]
public sealed class AttributeComponent : Game.UComponent
{
    public int level;
    public long exp;
    public long maxExp;

    /// <summary>请求增加经验的事件</summary>
    public struct AddExpNotify
    {
        public long exp;
        public string source; // 经验来源描述，便于日志追踪
    }

    /// <summary>等级变更事件</summary>
    public struct LevelChangedNotify
    {
        public int oldLevel;
        public int newLevel;
    }
}
```

#### 文件：`Game/View/MailPanel.cs`

```csharp
using GameEngine;

/// <summary>
/// 邮件面板视图对象
/// UViewClass 名称必须与 UI 资源名一致
/// </summary>
[UViewClass("GameMailPanel")]
public sealed class MailPanel : Game.UView
{
    // 视图对象数据类中只定义 UI 相关的引用数据
    // 具体的 UI 元素绑定根据 UI 框架（FairyGUI/UGUI）而定
}
```

#### 文件：`Game/Object/Player.cs`（修改已有的玩家角色类）

```csharp
using GameEngine;

[UActorClass("LocalPlayer")]
[UComponentAutomaticActivationOfEntity(typeof(AttributeComponent))] // 已有
[UComponentAutomaticActivationOfEntity(typeof(MailComponent))]      // 新增：自动挂载邮件组件
public sealed class Player : Game.UActor
{
    // 玩家角色标识数据
    public long roleId;
    public string roleName;
}
```

### 1.4 第二步：创建逻辑类（GameHotfix 逻辑模组）

#### 文件：`GameHotfix/Component/Mail/MailComponentMailSystem.cs`

```csharp
using Game;
using GameEngine;

/// <summary>
/// 邮件组件 - 邮件业务逻辑
/// 命名规则：MailComponent(实体类名) + Mail(功能) + System
/// 
/// 职责：
/// - 接收服务端邮件数据并更新本地列表
/// - 处理领取附件逻辑
/// - 通过事件通知 UI 和其他组件
/// </summary>
static class MailComponentMailSystem
{
    [OnAwake]
    static void Awake(this MailComponent self)
    {
        // 组件初始化时可以从本地缓存恢复数据
        UnityEngine.Debug.Log("MailComponent 初始化完成");
    }

    [OnDestroy]
    static void Destroy(this MailComponent self)
    {
        self.mailList.Clear();
        UnityEngine.Debug.Log("MailComponent 数据已清理");
    }

    /// <summary>
    /// 接收服务端下发的邮件列表
    /// 使用扩展函数绑定 [OnMessage]，只有挂载了 MailComponent 的实体才会收到
    /// </summary>
    [OnMessage(typeof(MailListResp))]
    static void OnRecvMailList(this MailComponent self, MailListResp message)
    {
        self.mailList.Clear();

        foreach (var proto in message.MailList)
        {
            self.mailList.Add(new MailComponent.MailData
            {
                mailId        = proto.MailId,
                title         = proto.Title,
                content       = proto.Content,
                isRead        = proto.IsRead,
                hasAttachment = proto.HasAttachment,
                attachmentExp = proto.AttachmentExp
            });
        }

        // 全局延迟派发 → 通知所有监听方（如 MailPanel）
        // 用 Send 是因为不需要即时响应，统一在下一帧处理即可
        GameApi.Send(new MailComponent.MailListChangedNotify());
    }

    /// <summary>
    /// 接收服务端下发的领取附件响应
    /// </summary>
    [OnMessage(typeof(ClaimMailAttachmentResp))]
    static void OnRecvClaimAttachment(this MailComponent self, ClaimMailAttachmentResp message)
    {
        if (message.Code != 0)
        {
            UnityEngine.Debug.LogError($"领取附件失败，错误码：{message.Code}");
            return;
        }

        var mail = self.mailList.Find(m => m.mailId == message.MailId);
        if (mail == null) return;

        // 更新本地数据
        int expReward = mail.attachmentExp;
        mail.hasAttachment = false;

        // 1. 通过事件通知属性组件加经验（跨组件通信，不直接调用）
        GameApi.Fire(new AttributeComponent.AddExpNotify
        {
            exp = expReward,
            source = $"邮件附件(id={message.MailId})"
        });

        // 2. 通知 UI 刷新该邮件项（立即派发，因为用户刚点了按钮需要即时反馈）
        GameApi.Fire(new MailComponent.MailStateChangedNotify { mailId = message.MailId });
    }

    /// <summary>发送请求邮件列表的消息给服务端</summary>
    static void RequestMailList(this MailComponent self)
    {
        var req = new MailListReq();
        // 通过网络模组发送请求（具体发送方式取决于项目网络层实现）
    }

    /// <summary>发送领取附件请求</summary>
    static void RequestClaimAttachment(this MailComponent self, long mailId)
    {
        var req = new ClaimMailAttachmentReq { MailId = mailId };
        // 通过网络模组发送请求
    }

    /// <summary>标记邮件为已读</summary>
    static void MarkAsRead(this MailComponent self, long mailId)
    {
        var mail = self.mailList.Find(m => m.mailId == mailId);
        if (mail != null && !mail.isRead)
        {
            mail.isRead = true;
            // 仅通知当前实体及其组件（不需要全局广播）
            // 使用 component 级别的 Send，范围限定在所属实体
            self.Send(new MailComponent.MailStateChangedNotify { mailId = mailId });
        }
    }
}
```

#### 文件：`GameHotfix/Component/Attribute/AttributeComponentExpSystem.cs`

```csharp
using Game;
using GameEngine;

/// <summary>
/// 属性组件 - 经验相关逻辑
/// 命名规则：AttributeComponent(实体类名) + Exp(功能) + System
/// </summary>
static class AttributeComponentExpSystem
{
    /// <summary>
    /// 接收经验增加事件
    /// 这个事件可能由邮件、任务、战斗等多个来源触发
    /// 属性组件不需要知道经验来自哪里，只管处理
    /// </summary>
    [OnEvent(typeof(AttributeComponent.AddExpNotify))]
    static void OnAddExp(this AttributeComponent self, AttributeComponent.AddExpNotify notify)
    {
        int oldLevel = self.level;
        self.exp += notify.exp;

        UnityEngine.Debug.Log($"获得 {notify.exp} 经验，来源：{notify.source}");

        // 检查升级
        while (self.exp >= self.maxExp && self.maxExp > 0)
        {
            self.exp -= self.maxExp;
            self.level++;
            // 根据新等级更新 maxExp（从配置表读取）
            var levelConfig = Config.LevelConfigTable.Get(self.level);
            if (levelConfig != null)
            {
                self.maxExp = levelConfig.maxExp;
            }
        }

        // 如果升级了，通知其他模组
        if (self.level != oldLevel)
        {
            GameApi.Fire(new AttributeComponent.LevelChangedNotify
            {
                oldLevel = oldLevel,
                newLevel = self.level
            });
        }
    }
}
```

#### 文件：`GameHotfix/View/MailPanelUISystem.cs`

```csharp
using Game;
using GameEngine;

/// <summary>
/// 邮件面板 - UI 业务逻辑
/// 命名规则：MailPanel(实体类名) + UI(功能) + System
/// </summary>
static class MailPanelUISystem
{
    [OnAwake]
    static void Awake(this MailPanel self)
    {
        // UI 初始化，绑定按钮事件
        self.RefreshMailList();
    }

    [OnDestroy]
    static void Destroy(this MailPanel self)
    {
        // 清理 UI 资源、解绑事件
    }

    /// <summary>监听邮件列表整体变更</summary>
    [OnEvent(typeof(MailComponent.MailListChangedNotify))]
    static void OnMailListChanged(this MailPanel self, MailComponent.MailListChangedNotify notify)
    {
        self.RefreshMailList();
    }

    /// <summary>监听单封邮件状态变更</summary>
    [OnEvent(typeof(MailComponent.MailStateChangedNotify))]
    static void OnMailStateChanged(this MailPanel self, MailComponent.MailStateChangedNotify notify)
    {
        self.RefreshMailItem(notify.mailId);
    }

    /// <summary>监听等级变更（可能需要刷新邮件中的等级限制提示）</summary>
    [OnEvent(typeof(AttributeComponent.LevelChangedNotify))]
    static void OnLevelChanged(this MailPanel self, AttributeComponent.LevelChangedNotify notify)
    {
        self.RefreshMailList();
    }

    /// <summary>刷新整个邮件列表 UI</summary>
    static void RefreshMailList(this MailPanel self)
    {
        var players = GameApi.GetActor<Player>();
        if (players.Count == 0) return;

        var mailComp = players[0].GetComponent<MailComponent>();
        if (mailComp == null) return;

        foreach (var mailData in mailComp.mailList)
        {
            // 根据 mailData 创建/更新列表项 UI
        }
    }

    /// <summary>刷新单封邮件的 UI 状态</summary>
    static void RefreshMailItem(this MailPanel self, long mailId)
    {
        // 找到对应的列表项并更新显示
    }

    /// <summary>用户点击邮件项时调用</summary>
    static void OnClickMailItem(this MailPanel self, long mailId)
    {
        var players = GameApi.GetActor<Player>();
        if (players.Count == 0) return;

        var mailComp = players[0].GetComponent<MailComponent>();
        if (mailComp == null) return;

        mailComp.MarkAsRead(mailId);
    }

    /// <summary>用户点击领取附件按钮时调用</summary>
    static void OnClickClaimAttachment(this MailPanel self, long mailId)
    {
        var players = GameApi.GetActor<Player>();
        if (players.Count == 0) return;

        var mailComp = players[0].GetComponent<MailComponent>();
        if (mailComp == null) return;

        mailComp.RequestClaimAttachment(mailId);
    }
}
```

### 1.5 第三步：触发入口（在场景 System 中）

```csharp
// GameHotfix/Scene/WorldSceneSystem.cs（片段）
using Game;
using GameEngine;

static class WorldSceneSystem
{
    [OnStart]
    static void Start(this WorldScene self)
    {
        // 创建玩家角色（会自动挂载 MailComponent 和 AttributeComponent）
        var player = GameApi.CreateActor<Player>();

        // 请求邮件列表
        var mailComp = player.GetComponent<MailComponent>();
        mailComp.RequestMailList();
    }

    /// <summary>在某个按钮回调中打开邮件面板</summary>
    static void OnClickMailButton(this WorldScene self)
    {
        GameApi.OpenUI<MailPanel>();
    }
}
```

### 1.6 文件清单

| 文件路径 | 模组 | 类型 | 说明 |
|---------|------|------|------|
| `Game/Component/Mail/MailComponent.cs` | Game | 数据类 | 邮件组件数据 + 事件结构体 |
| `Game/Component/Attribute/AttributeComponent.cs` | Game | 数据类 | 属性组件数据 + 事件结构体（已有，补充） |
| `Game/View/MailPanel.cs` | Game | 数据类 | 邮件面板视图 |
| `Game/Object/Player.cs` | Game | 数据类 | 修改：添加 MailComponent 自动挂载 |
| `GameHotfix/Component/Mail/MailComponentMailSystem.cs` | GameHotfix | 逻辑类 | 邮件业务逻辑 |
| `GameHotfix/Component/Attribute/AttributeComponentExpSystem.cs` | GameHotfix | 逻辑类 | 经验相关逻辑 |
| `GameHotfix/View/MailPanelUISystem.cs` | GameHotfix | 逻辑类 | 邮件面板 UI 逻辑 |

### 1.7 数据流图

```
服务端 ──MailListResp──→ MailComponent(OnMessage) ──Send──→ MailPanel(OnEvent) → 刷新列表
服务端 ──ClaimResp──→ MailComponent(OnMessage) ──Fire──→ AttributeComponent(OnEvent) → 加经验
                                                  └──Fire──→ MailPanel(OnEvent) → 刷新单项
AttributeComponent ──Fire(LevelChanged)──→ MailPanel(OnEvent) → 刷新列表
```

---

## 2. 完整示例：角色行为的组件拆分

### 2.1 需求描述

游戏中有玩家（Player）和怪物（Monster）两种角色。它们都能移动，但只有玩家能攻击，怪物被击中时有受击表现。

### 2.2 分析与决策（组件职责规则 2.5）

| 行为能力 | 组件 | Player 挂载 | Monster 挂载 |
|---------|------|:-----------:|:------------:|
| 移动 | `MoveComponent` | ✅ | ✅ |
| 攻击 | `AttackComponent` | ✅ | ❌ |
| 受击表现 | `HitEffectComponent` | ❌ | ✅ |

### 2.3 数据类定义

```csharp
// Game/Component/Move/MoveComponent.cs
[UComponentClass("MoveComponent")]
public sealed class MoveComponent : Game.UComponent
{
    public UnityEngine.Vector3 position;
    public UnityEngine.Vector3 direction;
    public float speed;
    public bool isMoving;

    public struct MoveStartNotify { public UnityEngine.Vector3 targetPos; }
    public struct MoveStopNotify { }
}

// Game/Component/Attack/AttackComponent.cs
[UComponentClass("AttackComponent")]
public sealed class AttackComponent : Game.UComponent
{
    public int attackPower;
    public float attackRange;
    public float attackCooldown;
    public float lastAttackTime;

    public struct AttackExecutedNotify
    {
        public long targetId;
        public int damage;
    }
}

// Game/Component/HitEffect/HitEffectComponent.cs
[UComponentClass("HitEffectComponent")]
public sealed class HitEffectComponent : Game.UComponent
{
    public bool isPlayingEffect;

    public struct HitReceivedNotify
    {
        public int damage;
        public UnityEngine.Vector3 hitDirection;
    }
}
```

### 2.4 角色对象定义（按需挂载组件）

```csharp
// Game/Object/Player.cs
[UActorClass("LocalPlayer")]
[UComponentAutomaticActivationOfEntity(typeof(MoveComponent))]
[UComponentAutomaticActivationOfEntity(typeof(AttackComponent))]      // 玩家能攻击
[UComponentAutomaticActivationOfEntity(typeof(AttributeComponent))]
public sealed class Player : Game.UActor
{
    public long roleId;
}

// Game/Object/Monster.cs
[UActorClass("Monster")]
[UComponentAutomaticActivationOfEntity(typeof(MoveComponent))]
[UComponentAutomaticActivationOfEntity(typeof(HitEffectComponent))]   // 怪物有受击表现
[UComponentAutomaticActivationOfEntity(typeof(AttributeComponent))]
public sealed class Monster : Game.UActor
{
    public int monsterId;
    public int monsterType;
}
```

### 2.5 逻辑类示例（移动组件，被两种角色共用）

```csharp
// GameHotfix/Component/Move/MoveComponentMoveSystem.cs
using Game;
using GameEngine;

static class MoveComponentMoveSystem
{
    [OnAwake]
    static void Awake(this MoveComponent self)
    {
        self.isMoving = false;
    }

    [OnDestroy]
    static void Destroy(this MoveComponent self)
    {
        self.isMoving = false;
    }

    /// <summary>开始移动到目标位置</summary>
    static void MoveTo(this MoveComponent self, UnityEngine.Vector3 targetPos)
    {
        self.direction = (targetPos - self.position).normalized;
        self.isMoving = true;

        // 通知当前实体及其组件（而非全局广播）
        // 使用 component 级别的 Fire，范围限定在所属实体
        self.Fire(new MoveComponent.MoveStartNotify { targetPos = targetPos });
    }

    /// <summary>停止移动</summary>
    static void StopMove(this MoveComponent self)
    {
        self.isMoving = false;
        self.Fire(new MoveComponent.MoveStopNotify());
    }
}
```

> `self.Fire(...)` 在组件实例上调用时，事件范围限定在该组件所属的实体对象及其所有组件。Player 的 MoveComponent 发出的事件不会被 Monster 的组件收到。

---

## 3. 代码模板

> 将 `XXX` 替换为具体业务名称即可使用。**所有业务对象必须继承 U 类**。

### 3.1 组件对象模板

#### 数据类：`Game/Component/XXX/XXXComponent.cs`

```csharp
using GameEngine;

[UComponentClass("XXXComponent")]
public sealed class XXXComponent : Game.UComponent  // ← 继承 U 类
{
    // ====== 数据字段 ======
    public int someData;

    // ====== 事件结构体（必须 public） ======
    public struct SomeNotify
    {
        public int someField;
    }
}
```

#### 逻辑类：`GameHotfix/Component/XXX/XXXComponentXXXSystem.cs`

```csharp
using Game;
using GameEngine;

static class XXXComponentXXXSystem
{
    [OnAwake]
    static void Awake(this XXXComponent self) { }

    [OnDestroy]
    static void Destroy(this XXXComponent self) { }

    [OnMessage(typeof(SomeResp))]
    static void OnRecvSomeResp(this XXXComponent self, SomeResp message) { }

    [OnEvent(typeof(OtherComponent.SomeNotify))]
    static void OnSomeNotify(this XXXComponent self, OtherComponent.SomeNotify notify) { }

    static void DoSomething(this XXXComponent self)
    {
        GameApi.Send(new XXXComponent.SomeNotify { someField = 1 }); // 全局延迟
        self.Fire(new XXXComponent.SomeNotify { someField = 1 });    // 仅当前实体，立即
    }
}
```

### 3.2 视图对象模板

```csharp
// Game/View/XXXPanel.cs
using GameEngine;

[UViewClass("GameXXXPanel")]
public sealed class XXXPanel : Game.UView { }  // ← 继承 Game.UView

// GameHotfix/View/XXXPanelUISystem.cs
using Game;
using GameEngine;

static class XXXPanelUISystem
{
    [OnAwake]
    static void Awake(this XXXPanel self) { }

    [OnDestroy]
    static void Destroy(this XXXPanel self) { }

    [OnEvent(typeof(XXXComponent.SomeNotify))]
    static void OnSomeNotify(this XXXPanel self, XXXComponent.SomeNotify notify)
    {
        self.RefreshUI();
    }

    static void RefreshUI(this XXXPanel self) { }
}
```

### 3.3 场景对象模板

```csharp
// Game/Scene/XXXScene.cs
using GameEngine;

[USceneClass("XXX")]
[UComponentAutomaticActivationOfEntity(typeof(SomeComponent))]
public sealed class XXXScene : Game.UScene { }  // ← 继承 Game.UScene

// GameHotfix/Scene/XXXSceneSystem.cs
using Game;
using GameEngine;

static class XXXSceneSystem
{
    [OnStart]
    static void Start(this XXXScene self)
    {
        GameApi.CreateActor<Player>();
        GameApi.OpenUI<SomePanel>();
    }

    [OnDestroy]
    static void Destroy(this XXXScene self) { }
}
```

### 3.4 角色对象模板

```csharp
// Game/Object/XXX.cs
using GameEngine;

[UActorClass("XXX")]
[UComponentAutomaticActivationOfEntity(typeof(AttributeComponent))]
[UComponentAutomaticActivationOfEntity(typeof(MoveComponent))]
public sealed class XXX : Game.UActor { }  // ← 继承 Game.UActor

// GameHotfix/Object/XXXSystem.cs
using Game;
using GameEngine;

static class XXXSystem
{
    [OnAwake]
    static void Awake(this XXX self) { }

    [OnDestroy]
    static void Destroy(this XXX self) { }
}
```

---

## 4. 反模式（常见错误写法）

### 4.1 ❌ 直接继承框架 C 类

```csharp
// ❌ 错误：直接继承 GameEngine.CActor
[UActorClass("LocalPlayer")]
public sealed class Player : GameEngine.CActor { ... }

// ❌ 错误：直接继承 GameEngine.CComponent
[UComponentClass("MoveComponent")]
public sealed class MoveComponent : GameEngine.CComponent { ... }
```

```csharp
// ✅ 正确：继承业务封装 U 类
[UActorClass("LocalPlayer")]
public sealed class Player : Game.UActor { ... }

[UComponentClass("MoveComponent")]
public sealed class MoveComponent : Game.UComponent { ... }
```

### 4.2 ❌ 使用 CComponentAutomaticActivationOfEntity 而非 U 版本

```csharp
// ❌ 错误：使用框架原始特性标签
[CComponentAutomaticActivationOfEntity(typeof(MoveComponent))]
public sealed class Player : Game.UActor { ... }
```

```csharp
// ✅ 正确：使用业务封装的 U 版本特性标签
[UComponentAutomaticActivationOfEntity(typeof(MoveComponent))]
public sealed class Player : Game.UActor { ... }
```

### 4.3 ❌ 在数据类中编写业务逻辑

```csharp
// ❌ 错误
[UComponentClass("MailComponent")]
public sealed class MailComponent : Game.UComponent
{
    public List<MailData> mailList = new();
    public void RefreshMailList() { ... }  // ❌ 业务逻辑！
}
```

```csharp
// ✅ 正确：逻辑放在 System 中
static class MailComponentMailSystem
{
    static void RefreshMailList(this MailComponent self) { ... }
}
```

### 4.4 ❌ System 类中定义状态数据

```csharp
// ❌ 错误
static class MailComponentMailSystem
{
    private static List<MailData> _cachedMails = new();  // ❌ 禁止！System 无状态
}
```

```csharp
// ✅ 正确：数据定义在实体对象中
public sealed class MailComponent : Game.UComponent
{
    public List<MailData> mailList = new();  // 数据在这里
}
```

### 4.5 ❌ 组件之间直接调用业务接口

```csharp
// ❌ 错误：邮件组件直接调用属性组件的方法
static void OnRecvClaimAttachment(this MailComponent self, ClaimMailAttachmentResp msg)
{
    var attr = self.Parent.GetComponent<AttributeComponent>();
    attr.AddExp(100);  // ❌ 禁止跨组件直接调用！
}
```

```csharp
// ✅ 正确：通过事件通知
static void OnRecvClaimAttachment(this MailComponent self, ClaimMailAttachmentResp msg)
{
    GameApi.Fire(new AttributeComponent.AddExpNotify { exp = 100, source = "邮件" });
}

// 属性组件自己处理
[OnEvent(typeof(AttributeComponent.AddExpNotify))]
static void OnAddExp(this AttributeComponent self, AttributeComponent.AddExpNotify n)
{
    self.exp += n.exp;
}
```

### 4.6 ❌ 把多种行为写在角色 System 里而不拆分组件

```csharp
// ❌ 错误：所有行为逻辑都堆在 PlayerSystem 中
static class PlayerSystem
{
    static void MoveTo(this Player self, Vector3 pos) { ... }      // ❌ 应拆到 MoveComponent
    static void Attack(this Player self, long targetId) { ... }    // ❌ 应拆到 AttackComponent
    static void PlayHitEffect(this Player self) { ... }            // ❌ 应拆到 HitEffectComponent
}
```

```csharp
// ✅ 正确：按行为拆分为独立组件
// MoveComponent + MoveComponentMoveSystem    → 移动能力
// AttackComponent + AttackComponentSystem    → 攻击能力
// HitEffectComponent + HitEffectSystem       → 受击表现
// PlayerSystem 只处理 Player 级别的整体协调
```

### 4.7 ❌ 重载生命周期函数

```csharp
// ❌ 错误
public sealed class MailComponent : Game.UComponent
{
    protected override void OnAwake() { ... }  // ❌ 禁止重载！
}
```

```csharp
// ✅ 正确：扩展函数 + 标签
static class MailComponentMailSystem
{
    [OnAwake]
    static void Awake(this MailComponent self) { ... }
}
```

### 4.8 ❌ 使用 new 创建实体对象

```csharp
var player = new Player();              // ❌
```

```csharp
var player = GameApi.CreateActor<Player>();  // ✅
```

### 4.9 ❌ 使用底层框架生命周期标签

```csharp
[OnInitialize] static void Init(this Player self) { ... }   // ❌ 框架层专用
[OnShutdown]   static void Down(this Player self) { ... }   // ❌ 框架层专用
```

```csharp
[OnAwake]   static void Awake(this Player self) { ... }     // ✅
[OnStart]   static void Start(this Player self) { ... }     // ✅
[OnDestroy] static void Destroy(this Player self) { ... }   // ✅
```

### 4.10 ❌ System 类不是 static

```csharp
class PlayerSystem { ... }         // ❌ 缺少 static
```

```csharp
static class PlayerSystem { ... }  // ✅
```

### 4.11 ❌ Send / Fire 用错场景

```csharp
// ❌ 用户点击按钮需要即时反馈，却用了 Send
static void OnClickButton(this MailPanel self)
{
    GameApi.Send(new SomeNotify());  // ❌ 延迟一帧
}
```

```csharp
// ✅ 即时响应用 Fire
static void OnClickButton(this MailPanel self)
{
    GameApi.Fire(new SomeNotify());  // ✅ 当前帧立即
}
```

### 4.12 ❌ 主动加载的资源未手动释放

```csharp
// ❌ 加载了没释放 → 资源泄漏
var go = await GameApi.AsyncLoadAsset<GameObject>("Assets/Model/Hero.prefab");
```

```csharp
// ✅ 配对使用
var go = await GameApi.AsyncLoadAsset<GameObject>("Assets/Model/Hero.prefab");
// ... 使用 ...
GameApi.UnloadAsset(go);  // ✅
```

### 4.13 ❌ 实体对象放错模组

```csharp
// ❌ LoadingScene 定义在 Game/ 下，但 System 放到了 BattleHotfix/
// Game/Scene/LoadingScene.cs            ← 数据在 Game
// BattleHotfix/Scene/LoadingSystem.cs   ← ❌ System 放错了模组！
```

```csharp
// ✅ 数据模组和逻辑模组必须对应
// Game/Scene/LoadingScene.cs              ← 数据在 Game
// GameHotfix/Scene/LoadingSceneSystem.cs  ← ✅ System 在 GameHotfix
```

---

## 5. 常见场景 Checklist

### 5.1 新增一个业务功能（组件级别）

- [ ] 在 `Game/Component/<功能名>/` 下创建 `<功能名>Component.cs`，**继承 `Game.UComponent`**
- [ ] 在数据类中定义数据字段和事件结构体（`public struct`）
- [ ] 在 `GameHotfix/Component/<功能名>/` 下创建 System 逻辑类
- [ ] 在逻辑类中用 `[OnAwake]` 和 `[OnDestroy]` 处理初始化和清理
- [ ] 在需要使用此组件的实体对象上添加 **`[UComponentAutomaticActivationOfEntity]`**
- [ ] 如果需要 UI，同步创建对应的视图对象

### 5.2 新增一个 UI 面板

- [ ] 确保 UI 资源已准备好，放置在 `_Resources/Gui/`
- [ ] 在 `Game/View/` 下创建视图数据类，**继承 `Game.UView`**，`[UViewClass]` 名称与资源名一致
- [ ] 在 `GameHotfix/View/` 下创建 System 逻辑类
- [ ] 通过 `[OnEvent]` 监听数据变更事件来刷新 UI
- [ ] 在需要的地方调用 `GameApi.OpenUI<T>()` 打开面板

### 5.3 新增一个场景

- [ ] 在 `Game/Scene/` 下创建场景数据类，**继承 `Game.UScene`**
- [ ] 通过 **`[UComponentAutomaticActivationOfEntity]`** 挂载场景所需的组件
- [ ] 在对应的 Hotfix 模组中创建 System 逻辑类
- [ ] 在 `[OnStart]` 中初始化场景（创建角色、打开 UI 等）
- [ ] 通过 `GameApi.ReplaceScene<T>()` 切换到此场景

### 5.4 实现跨组件通信

- [ ] 在**接收方**组件的数据类中定义事件结构体
- [ ] 在**发送方** System 中通过 `GameApi.Send/Fire` 或 `entity/component.Send/Fire` 派发
- [ ] 在**接收方** System 中通过 `[OnEvent]` 接收
- [ ] **确认没有直接调用其他组件的业务方法**

### 5.5 加载和释放资源

- [ ] 使用 `GameApi.LoadAsset` / `AsyncLoadAsset` 加载
- [ ] 记录已加载的资源引用（一般存在组件数据字段中）
- [ ] 在 `[OnDestroy]` 或不再需要时调用 `GameApi.UnloadAsset` 释放
- [ ] 场景资源用 `LoadAssetScene` / `UnloadAssetScene` 配对管理

---

## 相关文档

- **开发规范**：`dev_spec.md` — 框架规则、架构约束、命名规范
- **API 手册**：`dev_api.md` — 框架提供的所有可调用接口


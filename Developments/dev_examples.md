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
| 组件继承 | 继承 `UComponent` | 与其他实体对象一致，均继承 U 类 |

### 1.3 第一步：创建数据类（Game 数据模组）

#### 文件：`Game/Component/Mail/MailComponent.cs`

```csharp
namespace Game
{
    public sealed class MailComponent : UComponent
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
}
```

#### 文件：`Game/Component/Attribute/AttributeComponent.cs`（已有，补充事件结构体）

```csharp
namespace Game
{
    public sealed class AttributeComponent : UComponent
    {
        public int level;
        public int exp;
        public int health;
        public int energy;
        public float speed;
        public int attack;
        public int defense;

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
}
```

#### 文件：`Game/View/MailPanel.cs`

```csharp
namespace Game
{
    /// <summary>
    /// 邮件面板视图对象
    /// UViewClass 名称必须与 UI 资源名一致
    /// </summary>
    [UViewClass("GameMailPanel")]
    public class MailPanel : UView
    {
        // 视图对象数据类中只定义 UI 相关的引用数据
    }
}
```

#### 文件：`Game/Object/Player.cs`（修改已有的玩家角色类）

```csharp
namespace Game
{
    // Player 通过继承链自动拥有：IdentityComponent + TransformComponent + AttributeComponent
    // 在此基础上额外挂载 AttackComponent 和 MailComponent
    [UComponentAutomaticActivationOfEntity(typeof(AttackComponent))]
    [UComponentAutomaticActivationOfEntity(typeof(MailComponent))]      // 新增：自动挂载邮件组件
    public sealed class Player : Soldier
    {
    }
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
/// </summary>
static class MailComponentMailSystem
{
    [OnAwake]
    static void Awake(this MailComponent self)
    {
        UnityEngine.Debug.Log("MailComponent 初始化完成");
    }

    [OnDestroy]
    static void Destroy(this MailComponent self)
    {
        self.mailList.Clear();
    }

    /// <summary>接收服务端下发的邮件列表</summary>
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
        GameApi.Send(new MailComponent.MailListChangedNotify());
    }

    /// <summary>接收服务端下发的领取附件响应</summary>
    [OnMessage(typeof(ClaimMailAttachmentResp))]
    static void OnRecvClaimAttachment(this MailComponent self, ClaimMailAttachmentResp message)
    {
        if (message.Code != 0) return;

        var mail = self.mailList.Find(m => m.mailId == message.MailId);
        if (mail == null) return;

        int expReward = mail.attachmentExp;
        mail.hasAttachment = false;

        // 1. 通过事件通知属性组件加经验（跨组件通信，不直接调用）
        GameApi.Fire(new AttributeComponent.AddExpNotify
        {
            exp = expReward,
            source = $"邮件附件(id={message.MailId})"
        });

        // 2. 通知 UI 刷新该邮件项（立即派发，用户需要即时反馈）
        GameApi.Fire(new MailComponent.MailStateChangedNotify { mailId = message.MailId });
    }

    /// <summary>标记邮件为已读</summary>
    static void MarkAsRead(this MailComponent self, long mailId)
    {
        var mail = self.mailList.Find(m => m.mailId == mailId);
        if (mail != null && !mail.isRead)
        {
            mail.isRead = true;
            // 仅通知当前实体及其组件
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
/// </summary>
static class AttributeComponentExpSystem
{
    [OnEvent(typeof(AttributeComponent.AddExpNotify))]
    static void OnAddExp(this AttributeComponent self, AttributeComponent.AddExpNotify notify)
    {
        int oldLevel = self.level;
        self.exp += notify.exp;

        // 检查升级（通过配置表刷新属性）
        self.ReloadConfig();

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

static class MailPanelUISystem
{
    [OnAwake]
    static void Awake(this MailPanel self)
    {
        self.RefreshMailList();
    }

    [OnDestroy]
    static void Destroy(this MailPanel self) { }

    [OnEvent(typeof(MailComponent.MailListChangedNotify))]
    static void OnMailListChanged(this MailPanel self, MailComponent.MailListChangedNotify notify)
    {
        self.RefreshMailList();
    }

    [OnEvent(typeof(MailComponent.MailStateChangedNotify))]
    static void OnMailStateChanged(this MailPanel self, MailComponent.MailStateChangedNotify notify)
    {
        self.RefreshMailItem(notify.mailId);
    }

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

    static void RefreshMailItem(this MailPanel self, long mailId) { }
}
```

### 1.5 数据流图

```
服务端 ──MailListResp──→ MailComponent(OnMessage) ──Send──→ MailPanel(OnEvent) → 刷新列表
服务端 ──ClaimResp──→ MailComponent(OnMessage) ──Fire──→ AttributeComponent(OnEvent) → 加经验
                                                  └──Fire──→ MailPanel(OnEvent) → 刷新单项
AttributeComponent ──Fire(LevelChanged)──→ MailPanel(OnEvent) → 刷新列表
```

---

## 2. 完整示例：角色行为的组件拆分与 SendToSelf

### 2.1 需求描述

玩家（Player）攻击怪物（Monster），怪物被击中后以"恐吓"方式反馈攻击者。

### 2.2 数据类定义（展示继承层级共享组件挂载）

```csharp
// Game/Object/Actor.cs — 中间基类，所有角色共享身份和位置
[UComponentAutomaticActivationOfEntity(typeof(IdentityComponent))]
[UComponentAutomaticActivationOfEntity(typeof(TransformComponent))]
public class Actor : UActor { }

// Game/Object/Soldier.cs — 战斗角色基类，额外拥有属性
[UComponentAutomaticActivationOfEntity(typeof(AttributeComponent))]
public class Soldier : Actor { }

// Game/Object/Player.cs — 玩家，额外拥有攻击能力
[UComponentAutomaticActivationOfEntity(typeof(AttackComponent))]
public sealed class Player : Soldier { }

// Game/Object/Monster.cs — 怪物（恐吓组件由配置表驱动挂载）
public sealed class Monster : Soldier { }
```

### 2.3 攻击逻辑——向目标定向派发事件

```csharp
// GameHotfix/Component/AttackComponentSystem.cs
static class AttackComponentSystem
{
    public static void DoAttack(this AttackComponent self, Soldier target)
    {
        if (self.isCooling) return;

        // 使用 SendToSelf 向目标定向派发命中事件
        target.SendToSelf(new AttackComponent.HitTargetReq()
        {
            attackerId = self.GetComponent<IdentityComponent>().uid,
            skillId = 0,
            damage = 50,
        });

        self.coolingTime = NovaEngine.Timestamp.TimeOfMilliseconds + 2000;
        self.isCooling = true;
    }
}
```

### 2.4 恐吓组件——接收命中事件并反馈

```csharp
// GameHotfix/Component/IntimidateOfBlowBubblesComponentSystem.cs
static class IntimidateOfBlowBubblesComponentSystem
{
    [OnEvent(typeof(AttackComponent.HitTargetReq))]
    static void OnHitTargetReq(this IntimidateOfBlowBubblesComponent self, AttackComponent.HitTargetReq eventData)
    {
        WorldDataComponent worldDataComponent = GameEngine.GameApi.GetCurrentScene().GetComponent<WorldDataComponent>();
        Soldier target = worldDataComponent.GetSoldierByUid(eventData.attackerId);

        // 向攻击者反馈
        target.SendToSelf(new AttackComponent.HitTargetResp()
        {
            victimId = self.GetComponent<IdentityComponent>().uid,
            title = "吐泡泡",
            info = "噗、噗、噗",
        });
    }
}
```

> **数据流**：`Player.AttackComponent` →(`SendToSelf`)→ `Monster` →(`OnEvent`)→ `IntimidateOfBlowBubblesComponent` →(`SendToSelf`)→ `Player` →(`OnEvent`)→ `PlayerSystem.OnHitTargetResp`

---

## 3. 完整示例：World/WorldHotfix 跨模组表现层

### 3.1 需求描述

角色在世界场景中需要有 3D 模型表现（加载模型、跟随位置移动）。表现逻辑放在 `World/WorldHotfix` 模组，与 `Game` 核心逻辑解耦。

### 3.2 数据类（World 数据模组）

```csharp
// World/Component/SoldierAnimationComponent.cs
namespace Game
{
    public sealed class SoldierAnimationComponent : UComponent
    {
        public string modelAssetName;
        public UnityEngine.GameObject modelGo;
        public UnityEngine.Vector3 targetPosition;
        public UnityEngine.Vector3 targetRotation;
    }
}
```

### 3.3 逻辑类（WorldHotfix 逻辑模组）

```csharp
// WorldHotfix/Component/SoldierAnimationComponentSystem.cs
namespace Game
{
    static class SoldierAnimationComponentSystem
    {
        [OnAwake]
        static void Awake(this SoldierAnimationComponent self)
        {
            // 通过兄弟组件获取配置，加载 3D 模型
            IdentityComponent identityComponent = self.GetComponent<IdentityComponent>();
            Config.ActorConfig actorConfig = Config.ActorConfigTable.Get(identityComponent.classId);
            Config.ResourceConfig resourceConfig = Config.ResourceConfigTable.Get(actorConfig.resourceId);

            TransformComponent transformComponent = self.GetComponent<TransformComponent>();

            Object assetObject = self.Entity.LoadAsset(resourceConfig.assetName, resourceConfig.assetUrl, typeof(GameObject));
            GameObject go = GameObject.Instantiate(assetObject, Vector3.zero, Quaternion.identity, null) as GameObject;
            go.transform.localPosition = new Vector3(transformComponent.posX, 0f, transformComponent.posY);

            self.modelAssetName = resourceConfig.assetName;
            self.modelGo = go;
            self.targetPosition = go.transform.localPosition;
        }

        [OnDestroy]
        static void Destroy(this SoldierAnimationComponent self)
        {
            Object.Destroy(self.modelGo);
            self.Entity.UnloadAsset(self.modelAssetName);
        }

        /// <summary>
        /// 监听 Game 模组的 TransformComponent 变更事件
        /// 跨模组联动：Game 层改数据 → World 层更新表现
        /// </summary>
        [OnEvent(typeof(TransformComponent.TransformUpdatedNotify))]
        static void OnTransformChangedNotify(this SoldierAnimationComponent self)
        {
            TransformComponent transformComponent = self.GetComponent<TransformComponent>();
            self.targetPosition = new Vector3(transformComponent.posX, 0f, transformComponent.posY);
            self.modelGo.transform.localRotation = Quaternion.LookRotation(
                -new Vector3(transformComponent.lookX, 0f, transformComponent.lookY));
        }
    }
}
```

> **跨模组联动模式**：`Game.SoldierSystem.MoveTo()` 修改 `TransformComponent` 后通过 `self.SendToSelf(new TransformUpdatedNotify())` 通知 → `World.SoldierAnimationComponentSystem` 通过 `[OnEvent]` 接收并更新 3D 表现。

---

## 4. 代码模板

> 将 `XXX` 替换为具体业务名称即可使用。

### 4.1 组件对象模板

```csharp
// Game/Component/XXX/XXXComponent.cs
namespace Game
{
    public sealed class XXXComponent : UComponent
    {
        public int someData;

        public struct SomeNotify
        {
            public int someField;
        }
    }
}

// GameHotfix/Component/XXX/XXXComponentXXXSystem.cs
using Game;
using GameEngine;

namespace Game
{
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
            IdentityComponent identity = self.GetComponent<IdentityComponent>(); // 兄弟组件
            GameApi.Send(new XXXComponent.SomeNotify { someField = 1 }); // 全局延迟
            self.Fire(new XXXComponent.SomeNotify { someField = 1 });    // 仅当前实体，立即
        }
    }
}
```

### 4.2 视图对象模板

```csharp
// Game/View/XXXPanel.cs
namespace Game
{
    [UViewClass("GameXXXPanel")]
    public class XXXPanel : UView { }
}

// GameHotfix/View/XXXPanelUISystem.cs
using Game;
using GameEngine;

namespace Game
{
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
}
```

### 4.3 场景对象模板

```csharp
// Game/Scene/XXXScene.cs
namespace Game
{
    [USceneClass("XXX")]
    [UComponentAutomaticActivationOfEntity(typeof(SomeComponent))]
    public sealed class XXXScene : UScene { }
}

// GameHotfix/Scene/XXXSceneSystem.cs
using Game;
using GameEngine;

namespace Game
{
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
}
```

### 4.4 角色对象模板

```csharp
// Game/Object/XXX.cs
namespace Game
{
    [UComponentAutomaticActivationOfEntity(typeof(SomeComponent))]
    public sealed class XXX : Actor { }  // 或 Soldier、UActor
}

// GameHotfix/Object/XXXSystem.cs
using Game;
using GameEngine;

namespace Game
{
    static class XXXSystem
    {
        [OnAwake]
        static void Awake(this XXX self) { }

        [OnDestroy]
        static void Destroy(this XXX self) { }
    }
}
```

---

## 5. 反模式（常见错误写法）

### 5.1 ❌ 实体对象直接继承框架 C 类

```csharp
// ❌ 错误
public sealed class Player : GameEngine.CActor { ... }
public sealed class AttackComponent : GameEngine.CComponent { ... }

// ✅ 正确（继承 U 类或 U 类的子类）
public sealed class Player : Soldier { ... }
public sealed class AttackComponent : UComponent { ... }
```

### 5.2 ❌ 使用 CComponentAutomaticActivationOfEntity 而非 U 版本

```csharp
// ❌ 错误
[CComponentAutomaticActivationOfEntity(typeof(MoveComponent))]

// ✅ 正确
[UComponentAutomaticActivationOfEntity(typeof(MoveComponent))]
```

### 5.3 ❌ 在数据类中编写业务逻辑

```csharp
// ❌ 错误
public sealed class MailComponent : UComponent
{
    public void RefreshMailList() { ... }  // ❌ 业务逻辑！
}

// ✅ 正确：逻辑放在 System 中
static class MailComponentMailSystem
{
    static void RefreshMailList(this MailComponent self) { ... }
}
```

### 5.4 ❌ System 类中定义状态数据

```csharp
// ❌ 错误
static class MailComponentMailSystem
{
    private static List<MailData> _cachedMails = new();  // ❌ System 无状态
}

// ✅ 正确：数据在组件中
public sealed class MailComponent : UComponent
{
    public List<MailData> mailList = new();
}
```

### 5.5 ❌ 组件之间直接调用业务接口

```csharp
// ❌ 错误
var attr = self.Entity.GetComponent<AttributeComponent>();
attr.AddExp(100);  // ❌ 禁止跨组件直接调用！

// ✅ 正确：通过事件通知
GameApi.Fire(new AttributeComponent.AddExpNotify { exp = 100, source = "邮件" });
// 或定向派发
target.SendToSelf(new AttackComponent.HitTargetReq { ... });
```

### 5.6 ❌ 多种行为写在角色 System 里

```csharp
// ❌ 错误
static class PlayerSystem
{
    static void MoveTo(this Player self, ...) { ... }   // → MoveComponent
    static void Attack(this Player self, ...) { ... }   // → AttackComponent
}

// ✅ 正确：按行为拆分为独立组件
```

### 5.7 ❌ 重载生命周期函数

```csharp
// ❌ 错误
protected override void OnAwake() { ... }

// ✅ 正确
[OnAwake] static void Awake(this MailComponent self) { ... }
```

### 5.8 ❌ 使用 new 创建实体对象

```csharp
var player = new Player();                               // ❌
var player = GameApi.CreateActor<Player>();               // ✅
var player = ApplicationContext.CreateBean<Player>();     // ✅
```

### 5.9 ❌ 使用底层框架生命周期标签

```csharp
[OnInitialize] ...  // ❌ 框架层专用
[OnShutdown]   ...  // ❌ 框架层专用

[OnAwake]   ...     // ✅
[OnStart]   ...     // ✅
[OnDestroy] ...     // ✅
```

### 5.10 ❌ System 类不是 static

```csharp
class PlayerSystem { ... }         // ❌
static class PlayerSystem { ... }  // ✅
```

### 5.11 ❌ Send / Fire 用错场景

```csharp
// ❌ 用户点击需要即时反馈却用了 Send（延迟一帧）
GameApi.Send(new SomeNotify());

// ✅ 即时响应用 Fire
GameApi.Fire(new SomeNotify());
```

### 5.12 ❌ 主动加载的资源未手动释放

```csharp
// ❌ 加载了没释放
var go = await GameApi.AsyncLoadAsset<GameObject>("Assets/Model/Hero.prefab");

// ✅ 配对使用
var go = await GameApi.AsyncLoadAsset<GameObject>("Assets/Model/Hero.prefab");
GameApi.UnloadAsset(go);
```

### 5.13 ❌ 实体对象放错模组

```csharp
// ❌ 数据在 Game，System 在 BattleHotfix
// ✅ 数据在 Game → System 在 GameHotfix
// ✅ 数据在 World → System 在 WorldHotfix
```

---

## 6. 常见场景 Checklist

### 6.1 新增一个业务功能（组件级别）

- [ ] 在 `Game/Component/<功能名>/` 下创建组件，继承 `UComponent`
- [ ] 定义数据字段和事件结构体（`public struct`）
- [ ] 在 `GameHotfix/Component/<功能名>/` 下创建 System 逻辑类
- [ ] 用 `[OnAwake]` / `[OnDestroy]` 处理初始化和清理
- [ ] 在需要的实体上添加 `[UComponentAutomaticActivationOfEntity]`

### 6.2 新增一个 UI 面板

- [ ] 在 `Game/View/` 下创建视图类，继承 `UView`，`[UViewClass]` 名称与资源名一致
- [ ] 在 `GameHotfix/View/` 下创建 System 逻辑类
- [ ] 通过 `[OnEvent]` 监听数据变更刷新 UI
- [ ] 通过 `GameApi.OpenUI<T>()` 打开面板

### 6.3 新增一个场景

- [ ] 在 `Game/Scene/` 下创建场景类，继承 `UScene`，添加 `[USceneClass]`
- [ ] 通过 `[UComponentAutomaticActivationOfEntity]` 挂载组件
- [ ] 在 `GameHotfix/Scene/` 下创建 System，在 `[OnStart]` 中初始化
- [ ] 通过 `GameApi.ReplaceScene<T>()` 切换

### 6.4 实现跨组件通信

- [ ] 在**接收方**组件中定义事件结构体
- [ ] **发送方**通过 `GameApi.Send/Fire`、`entity.Send/Fire` 或 `entity.SendToSelf` 派发
- [ ] **接收方**通过 `[OnEvent]` 接收
- [ ] **确认没有直接调用其他组件的业务方法**

---

## 相关文档

- **开发规范**：`dev_spec.md` — 框架规则、架构约束、命名规范
- **API 手册**：`dev_api.md` — 框架提供的所有可调用接口

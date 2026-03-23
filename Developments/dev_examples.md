# `NovaFramework` 开发示例集

> 本文档提供完整的业务开发示例、代码模板和反模式，帮助 AI 开发助手理解在 `NovaFramework` 中编写代码的正确方式。  
> 使用前请确保已阅读 `dev_spec.md`（规则）和 `dev_api.md`（API 接口）。

---

## 1. 完整示例：实现邮件系统

### 1.1 需求描述

玩家可以收取邮件、查看邮件列表、领取邮件附件。邮件数据由服务端下发。

### 1.2 分析

- 邮件是玩家角色的一个业务分支 → 使用 `CComponent` 实现
- 需要展示邮件列表 UI → 创建一个 `CView` 对象
- 邮件数据来自服务端 → 通过 `[OnMessage]` 接收消息
- UI 需要知道邮件数据变化 → 通过事件通知 UI 刷新

### 1.3 第一步：创建数据类（Game 数据模组）

#### 文件：`Game/Component/Mail/MailComponent.cs`

```csharp
using GameEngine;

[CComponentClass("MailComponent")]
public sealed class MailComponent : CComponent
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
    }

    /// <summary>邮件列表刷新事件</summary>
    public struct MailListChangedNotify { }

    /// <summary>单封邮件状态变更事件</summary>
    public struct MailStateChangedNotify
    {
        public long mailId;
    }
}
```

#### 文件：`Game/View/MailPanel.cs`

```csharp
using GameEngine;

[CViewClass("GameMailPanel")]
public sealed class MailPanel : CView
{
    // 视图对象数据类中只定义 UI 相关的引用数据
}
```

#### 文件：`Game/Object/Player.cs`（修改已有的玩家角色类，添加邮件组件挂载）

```csharp
using GameEngine;

[CActorClass("LocalPlayer")]
[CComponentAutomaticActivationOfEntity(typeof(MailComponent))]       // 新增
[CComponentAutomaticActivationOfEntity(typeof(AttributeComponent))]  // 已有
public sealed class Player : CActor
{
    // 玩家角色数据定义...
}
```

### 1.4 第二步：创建逻辑类（GameHotfix 逻辑模组）

#### 文件：`GameHotfix/Component/Mail/MailComponentMailSystem.cs`

```csharp
using GameEngine;

/// <summary>
/// 邮件组件的邮件业务逻辑系统
/// 命名：实体对象类名(MailComponent) + 业务功能(Mail) + System
/// </summary>
static class MailComponentMailSystem
{
    [OnAwake]
    static void Awake(this MailComponent self)
    {
        // 初始化邮件数据
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
        foreach (var mailProto in message.MailList)
        {
            var mail = new MailComponent.MailData
            {
                mailId = mailProto.MailId,
                title = mailProto.Title,
                content = mailProto.Content,
                isRead = mailProto.IsRead,
                hasAttachment = mailProto.HasAttachment
            };
            self.mailList.Add(mail);
        }
        // 全局延迟派发，通知 UI 刷新
        GameApi.Send(new MailComponent.MailListChangedNotify());
    }

    /// <summary>接收服务端下发的领取附件响应</summary>
    [OnMessage(typeof(ClaimMailAttachmentResp))]
    static void OnRecvClaimAttachment(this MailComponent self, ClaimMailAttachmentResp message)
    {
        if (message.Code != 0) return;

        var mail = self.mailList.Find(m => m.mailId == message.MailId);
        if (mail != null)
        {
            mail.hasAttachment = false;
        }
        // 全局立即派发，让 UI 立即刷新
        GameApi.Fire(new MailComponent.MailStateChangedNotify { mailId = message.MailId });
    }

    /// <summary>请求邮件列表</summary>
    static void RequestMailList(this MailComponent self)
    {
        var req = new MailListReq();
        // 通过网络模组发送请求...
    }
}
```

#### 文件：`GameHotfix/View/MailPanelUISystem.cs`

```csharp
using GameEngine;

/// <summary>
/// 邮件面板的 UI 业务逻辑系统
/// 命名：实体对象类名(MailPanel) + 业务功能(UI) + System
/// </summary>
static class MailPanelUISystem
{
    [OnAwake]
    static void Awake(this MailPanel self)
    {
        self.RefreshMailList();
    }

    [OnDestroy]
    static void Destroy(this MailPanel self)
    {
        // 清理 UI 资源
    }

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
        var player = GameApi.GetActor<Player>()[0];
        var mailComp = player.GetComponent<MailComponent>();
        // 根据 mailComp.mailList 刷新 UI 列表...
    }

    static void RefreshMailItem(this MailPanel self, long mailId)
    {
        // 根据 mailId 更新对应邮件项的 UI 显示...
    }
}
```

### 1.5 第三步：触发入口

```csharp
// 打开邮件面板
GameApi.OpenUI<MailPanel>();

// 请求邮件列表
var player = GameApi.GetActor<Player>()[0];
var mailComp = player.GetComponent<MailComponent>();
mailComp.RequestMailList();
```

### 1.6 文件清单

| 文件路径 | 类型 | 说明 |
|---------|------|------|
| `Game/Component/Mail/MailComponent.cs` | 数据类 | 邮件组件数据和事件结构体 |
| `Game/View/MailPanel.cs` | 数据类 | 邮件面板视图 |
| `Game/Object/Player.cs` | 数据类 | 修改：添加 MailComponent 自动挂载 |
| `GameHotfix/Component/Mail/MailComponentMailSystem.cs` | 逻辑类 | 邮件业务逻辑 |
| `GameHotfix/View/MailPanelUISystem.cs` | 逻辑类 | 邮件面板 UI 逻辑 |

---

## 2. 代码模板

> 将 `XXX` 替换为具体的业务名称即可使用。

### 2.1 组件对象模板

#### 数据类：`Game/Component/XXX/XXXComponent.cs`

```csharp
using GameEngine;

[CComponentClass("XXXComponent")]
public sealed class XXXComponent : CComponent
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
using GameEngine;

static class XXXComponentXXXSystem
{
    [OnAwake]
    static void Awake(this XXXComponent self)
    {
        // 业务初始化
    }

    [OnDestroy]
    static void Destroy(this XXXComponent self)
    {
        // 数据清理
    }

    [OnMessage(typeof(SomeResp))]
    static void OnRecvSomeResp(this XXXComponent self, SomeResp message)
    {
        // 处理服务端消息
    }

    [OnEvent(typeof(OtherComponent.SomeNotify))]
    static void OnSomeNotify(this XXXComponent self, OtherComponent.SomeNotify notify)
    {
        // 处理来自其他组件的事件通知
    }

    static void DoSomething(this XXXComponent self)
    {
        // 全局延迟派发
        GameApi.Send(new XXXComponent.SomeNotify { someField = 1 });
        // 仅通知当前实体，立即派发
        self.Parent.Fire(new XXXComponent.SomeNotify { someField = 1 });
    }
}
```

### 2.2 视图对象模板

#### 数据类：`Game/View/XXXPanel.cs`

```csharp
using GameEngine;

[CViewClass("GameXXXPanel")]
public sealed class XXXPanel : CView
{
    // UI 相关的引用数据
}
```

#### 逻辑类：`GameHotfix/View/XXXPanelUISystem.cs`

```csharp
using GameEngine;

static class XXXPanelUISystem
{
    [OnAwake]
    static void Awake(this XXXPanel self)
    {
        // 初始化 UI，绑定事件
    }

    [OnDestroy]
    static void Destroy(this XXXPanel self)
    {
        // 清理 UI 资源
    }

    [OnEvent(typeof(XXXComponent.SomeNotify))]
    static void OnSomeNotify(this XXXPanel self, XXXComponent.SomeNotify notify)
    {
        self.RefreshUI();
    }

    static void RefreshUI(this XXXPanel self)
    {
        // 获取数据并刷新 UI
    }
}
```

### 2.3 场景对象模板

#### 数据类：`Game/Scene/XXXScene.cs`

```csharp
using GameEngine;

[CSceneClass("XXX")]
[CComponentAutomaticActivationOfEntity(typeof(SomeComponent))]
public sealed class XXXScene : CScene
{
    // 场景数据
}
```

#### 逻辑类：`GameHotfix/Scene/XXXSceneSystem.cs`

```csharp
using GameEngine;

static class XXXSceneSystem
{
    [OnAwake]
    static void Awake(this XXXScene self)
    {
        // 场景初始化：创建角色、打开 UI 等
        GameApi.CreateActor<Player>();
        GameApi.OpenUI<SomePanel>();
    }

    [OnDestroy]
    static void Destroy(this XXXScene self)
    {
        // 场景清理
    }
}
```

### 2.4 角色对象模板

#### 数据类：`Game/Object/XXX.cs`

```csharp
using GameEngine;

[CActorClass("XXX")]
[CComponentAutomaticActivationOfEntity(typeof(AttributeComponent))]
[CComponentAutomaticActivationOfEntity(typeof(MoveComponent))]
public sealed class XXX : CActor
{
    // 角色标识数据
}
```

#### 逻辑类：`GameHotfix/Object/XXXSystem.cs`

```csharp
using GameEngine;

static class XXXSystem
{
    [OnAwake]
    static void Awake(this XXX self)
    {
        // 角色初始化
    }

    [OnDestroy]
    static void Destroy(this XXX self)
    {
        // 角色清理
    }
}
```

---

## 3. 反模式（常见错误写法）

### 3.1 ❌ 在数据类中编写业务逻辑

```csharp
// ❌ 错误
[CComponentClass("MailComponent")]
public sealed class MailComponent : CComponent
{
    public List<MailData> mailList = new List<MailData>();
    public void RefreshMailList() { ... }  // ❌ 业务逻辑不允许在数据类中！
}
```

```csharp
// ✅ 正确：逻辑放在 System 中
static class MailComponentMailSystem
{
    static void RefreshMailList(this MailComponent self) { ... }
}
```

### 3.2 ❌ System 类中定义状态数据

```csharp
// ❌ 错误
static class MailComponentMailSystem
{
    private static List<MailData> _cachedMails = new List<MailData>(); // ❌ 禁止！
}
```

```csharp
// ✅ 正确：数据定义在实体对象中
public sealed class MailComponent : CComponent
{
    public List<MailData> mailList = new List<MailData>();
}
static class MailComponentMailSystem
{
    [OnAwake]
    static void Awake(this MailComponent self) { self.mailList.Clear(); }
}
```

### 3.3 ❌ 组件之间直接调用业务接口

```csharp
// ❌ 错误：跨组件直接调用
static void OnRecvClaimAttachment(this MailComponent self, ClaimMailAttachmentResp message)
{
    var attrComp = self.Parent.GetComponent<AttributeComponent>();
    attrComp.AddExp(100); // ❌ 禁止！
}
```

```csharp
// ✅ 正确：通过事件通知
static void OnRecvClaimAttachment(this MailComponent self, ClaimMailAttachmentResp message)
{
    GameApi.Fire(new AttributeComponent.AddExpNotify { exp = 100 }); // ✅
}

// 在 AttributeComponent 的 System 中接收
[OnEvent(typeof(AttributeComponent.AddExpNotify))]
static void OnAddExp(this AttributeComponent self, AttributeComponent.AddExpNotify notify)
{
    self.exp += notify.exp;
}
```

### 3.4 ❌ 重载生命周期函数

```csharp
// ❌ 错误
public sealed class MailComponent : CComponent
{
    protected override void OnAwake() { ... }  // ❌ 禁止重载！
}
```

```csharp
// ✅ 正确：扩展函数 + 特性标签
static class MailComponentMailSystem
{
    [OnAwake]
    static void Awake(this MailComponent self) { ... }
}
```

### 3.5 ❌ 使用 new 创建实体对象

```csharp
var player = new Player();  // ❌ 禁止！
```

```csharp
Player player = GameApi.CreateActor<Player>();  // ✅
```

### 3.6 ❌ 使用底层框架生命周期标签

```csharp
// ❌ 错误
[OnInitialize] static void Init(this Player self) { ... }   // ❌
[OnShutdown]   static void Shutdown(this Player self) { ... } // ❌
```

```csharp
// ✅ 正确：业务层只用 OnAwake / OnStart / OnDestroy
[OnAwake]   static void Awake(this Player self) { ... }   // ✅
[OnStart]   static void Start(this Player self) { ... }   // ✅
[OnDestroy] static void Destroy(this Player self) { ... } // ✅
```

### 3.7 ❌ System 类不是 static

```csharp
class PlayerSystem { ... }         // ❌ 缺少 static！
```

```csharp
static class PlayerSystem { ... }  // ✅
```

### 3.8 ❌ Send / Fire 用错场景

```csharp
// ❌ 需要即时响应却用了 Send（延迟到下一帧）
static void OnButtonClick(this MailPanel self)
{
    GameApi.Send(new SomeNotify());  // ❌ UI 响应有延迟
}
```

```csharp
// ✅ 需要即时响应用 Fire
static void OnButtonClick(this MailPanel self)
{
    GameApi.Fire(new SomeNotify());  // ✅ 当前帧立即派发
}
```

### 3.9 ❌ 主动加载的资源未手动释放

```csharp
// ❌ 错误：加载了资源但没有释放
static void LoadSomething(this XXXComponent self)
{
    var go = await GameApi.AsyncLoadAsset<GameObject>("Assets/Model/Hero.prefab");
    // 使用完后忘记释放 → 资源泄漏！
}
```

```csharp
// ✅ 正确：配对使用 Load 和 Unload
static void LoadSomething(this XXXComponent self)
{
    var go = await GameApi.AsyncLoadAsset<GameObject>("Assets/Model/Hero.prefab");
    // ... 使用资源 ...
    GameApi.UnloadAsset(go); // ✅ 手动释放
}
```

---

## 4. 常见场景 Checklist

### 4.1 新增一个业务功能（组件级别）

- [ ] 在 `Game/Component/<功能名>/` 下创建 `<功能名>Component.cs` 数据类
- [ ] 在数据类中定义数据字段和事件结构体（`public struct`）
- [ ] 在 `GameHotfix/Component/<功能名>/` 下创建 `<功能名>Component<业务>System.cs` 逻辑类
- [ ] 在逻辑类中通过 `[OnAwake]` 和 `[OnDestroy]` 处理初始化和清理
- [ ] 在需要使用此组件的实体对象上添加 `[CComponentAutomaticActivationOfEntity]`
- [ ] 如果需要 UI，同步创建对应的视图对象数据类和逻辑类

### 4.2 新增一个 UI 面板

- [ ] 确保 UI 资源已准备好（FairyGUI 或 UGUI），放置在 `_Resources/Gui/`
- [ ] 在 `Game/View/` 下创建 `<面板名>.cs` 数据类，`[CViewClass]` 名称与 UI 资源名一致
- [ ] 在 `GameHotfix/View/` 下创建 `<面板名>UISystem.cs` 逻辑类
- [ ] 通过 `[OnEvent]` 监听数据变更事件来刷新 UI
- [ ] 在需要的地方调用 `GameApi.OpenUI<T>()` 打开面板

### 4.3 新增一个场景

- [ ] 在 `Game/Scene/` 下创建 `<场景名>Scene.cs` 数据类
- [ ] 通过 `[CComponentAutomaticActivationOfEntity]` 挂载场景所需的组件
- [ ] 在 `GameHotfix/Scene/` 下创建 `<场景名>SceneSystem.cs` 逻辑类
- [ ] 在 `[OnAwake]` 中初始化场景（创建角色、打开 UI 等）
- [ ] 通过 `GameApi.ReplaceScene<T>()` 切换到此场景

### 4.4 实现跨组件通信

- [ ] 在发送方组件的数据类中定义事件结构体（`public struct XXXNotify`）
- [ ] 在发送方逻辑中通过 `GameApi.Send(...)` 或 `GameApi.Fire(...)` 派发事件
- [ ] 在接收方 System 中通过 `[OnEvent(typeof(XXXComponent.XXXNotify))]` 接收
- [ ] **确认没有直接调用其他组件的业务方法**

### 4.5 加载和释放资源

- [ ] 使用 `GameApi.LoadAsset` 或 `GameApi.AsyncLoadAsset` 加载资源
- [ ] 记录已加载的资源引用
- [ ] 在 `[OnDestroy]` 或不再需要时调用 `GameApi.UnloadAsset` 释放
- [ ] 场景资源使用 `LoadAssetScene` / `UnloadAssetScene` 配对管理

---

## 相关文档

- **开发规范**：`dev_spec.md` — 框架规则、架构约束、命名规范
- **API 手册**：`dev_api.md` — 框架提供的所有可调用接口

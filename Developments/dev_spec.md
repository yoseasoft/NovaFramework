# `NovaFramework` 开发规范指南

> 本文档是 AI 开发助手的核心参考文档，定义在 `NovaFramework` 框架中进行业务开发时**必须遵守的规则、架构约束和代码组织方式**。

---

## 1. 项目概览

`NovaFramework` 是一个基于 `Unity` 引擎研发的客户端游戏框架，使用 `C#` 语言开发。  
框架整体以**数据驱动**为核心，所有的业务流程都围绕数据对象的生命周期流程展开。

---

## 2. 强制规则

> 以下规则为框架的核心约束，**违反即为错误**，在编写任何代码前必须牢记。

### 2.1 模组分离规则

- ✅ 所有实体对象的**数据定义**必须放在数据模组中（如 `Game`）
- ✅ 所有**业务逻辑**必须放在逻辑模组中（如 `GameHotfix`）
- ❌ **禁止**在数据模组中编写任何业务逻辑
- ❌ **禁止**在逻辑模组中定义任何数据字段

### 2.2 System 类规则

- ✅ 所有 `System` 类必须是 `static` 类（无状态）
- ✅ `System` 类内部的所有函数必须是 `static` 函数或指定实体对象类型的扩展函数
- ✅ `System` 类推荐使用默认访问权限（`internal`）
- ✅ `System` 类命名格式：`<实体对象类名><功能描述>System`；若无明显功能描述，则为 `<实体对象类名>System`
- ✅ 一个 `System` 类只服务于一个指定的 `CBean` 对象类
- ❌ **禁止**在 `System` 类中定义任何字段或属性

### 2.3 业务逻辑规则

- ✅ 所有业务逻辑**必须依附于某个实体对象类型**
- ✅ 业务逻辑通过扩展函数 + 特性标签绑定到实体对象的生命周期节点
- ✅ 业务层生命周期推荐使用 `[OnAwake]`、`[OnStart]`、`[OnDestroy]`；按需可使用 `[OnUpdate]`、`[OnLateUpdate]`（如 UI 逐帧驱动）
- ❌ **禁止**重载实体对象的生命周期函数来处理业务逻辑
- ❌ **禁止** `OnInitialize`、`OnStartup`、`OnShutdown`、`OnCleanup` 在业务层使用（这些由底层框架使用）

### 2.4 组件通信规则

- ✅ 不同业务之间必须通过**事件、消息、数据同步**等通知方式进行互通
- ✅ 逻辑层到数据层是**单向访问**
- ❌ **禁止**不同 `CComponent` 之间直接调用彼此的业务接口函数
- ❌ **禁止**不同数据模型之间直接进行接口函数调用

### 2.5 组件职责规则

- ✅ 实体对象（`CActor`）的每一种独立行为/表现能力都应拆分为单独的 `CComponent`
- ✅ 例如：移动能力 → `MoveComponent`、受击表现 → `HitEffectComponent`、攻击能力 → `AttackComponent`
- ❌ **禁止**将多种不同的行为逻辑直接写在实体对象的 System 类中，应通过组件拆分

### 2.6 实体对象创建规则

- ✅ 所有基于 `CBean` 的实体对象必须通过框架提供的 `API` 函数创建和销毁
- ❌ **禁止**通过 `new` 关键字创建实体对象实例
- ❌ **禁止**依赖 `GC` 自动垃圾回收来销毁实体对象

### 2.7 资源管理规则

- ✅ 通过 `GameApi.LoadAsset` / `GameApi.AsyncLoadAsset` 主动加载的资源，必须通过 `GameApi.UnloadAsset` 手动释放
- ✅ 通过 `GameApi.LoadAssetScene` 加载的场景资源，必须通过 `GameApi.UnloadAssetScene` 手动释放
- ❌ **禁止**依赖 GC 自动释放主动加载的资源

### 2.8 业务层二次封装规则

为了将业务代码与框架引擎层解耦，项目在 `Game` 命名空间下对框架的核心基类进行了二次封装（U 类），位于 `Game/Core/Surface/` 目录：

| 框架基类（GameEngine） | 业务封装类（Game） | 说明 |
|----------------------|------------------|------|
| `CScene` | `Game.UScene` | 场景对象封装 |
| `CActor` | `Game.UActor` | 角色对象封装 |
| `CView` | `Game.UView` | 视图对象封装 |
| `CObject` | `Game.UObject` | 常规对象封装 |
| `CComponent` | `Game.UComponent` | 组件对象封装 |

- ✅ 所有业务实体对象**必须继承 U 类**，而非直接继承框架的 C 类
- ✅ U 类可以包含业务层的通用逻辑（公共字段、公共方法等），供所有子类复用
- ❌ **禁止**业务实体对象直接继承 `GameEngine.CActor`、`GameEngine.CScene` 等框架基类

正确示例：
```csharp
// ✅ 正确：继承 Game.UActor
[UActorClass("LocalPlayer")]
public sealed class Player : Game.UActor { ... }

// ✅ 正确：继承 Game.UScene
[USceneClass("Loading")]
public sealed class LoadingScene : Game.UScene { ... }

// ✅ 正确：继承 Game.UComponent
[UComponentClass("MoveComponent")]
public sealed class MoveComponent : Game.UComponent { ... }
```

错误示例：
```csharp
// ❌ 错误：直接继承框架基类
[UActorClass("LocalPlayer")]
public sealed class Player : GameEngine.CActor { ... }
```

---

## 3. 目录结构

```text
Assets
├── _Resources                 # 资源目录
│   ├── Aot                    # 元数据集存放目录
│   ├── Code                   # 模组程序集存放目录
│   ├── Config                 # 配置数据存放目录
│   ├── Contex                 # 上下文配置文件存放目录
│   ├── Gui                    # UI资源存放目录
│   ├── Model                  # 模型资源存放目录
│   └── Scene                  # 场景资源存放目录
│
└── Sources                    # 代码目录
    ├── Agen                   # 自动生成代码模组目录
    │   ├── Config             # 配置文件目录
    │   ├── Proto              # 协议文件目录
    │   └── Api                # API文件目录
    ├── Game                   # 【数据模组】定义实体对象，纯数据容器
    │   ├── Core               # 框架启动管理控制目录（框架提供，不要修改）
    │   ├── Scene              # 场景对象数据类
    │   ├── Object             # 角色对象和通用对象的数据类
    │   ├── View               # 视图对象数据类
    │   └── Component          # 组件对象数据类（按业务功能子目录切分）
    └── GameHotfix             # 【逻辑模组】扩展实体对象的业务逻辑
        ├── Manager            # 管理器目录
        ├── Scene              # 场景对象逻辑类（System）
        ├── Object             # 角色对象和通用对象的逻辑类（System）
        ├── View               # 视图对象逻辑类（System）
        └── Component          # 组件对象逻辑类（System），与数据模组 Component 子目录一一对应
```

### 3.1 文件放置规则

| 文件类型 | 放置位置 | 示例 |
|---------|---------|------|
| 实体对象数据类 | `Game/<对象类型>/` | `Game/Component/Mail/MailComponent.cs` |
| 事件结构体 | 定义在实体对象类内部 | `MailComponent` 类内部的 `public struct` |
| System 逻辑类 | `GameHotfix/<对象类型>/` | `GameHotfix/Component/Mail/MailComponentMailSystem.cs` |
| 管理器 | `GameHotfix/Manager/` | `GameHotfix/Manager/MailManager.cs` |

### 3.2 多模组结构

项目可能存在多个独立模组，每个数据模组都有一个对应的逻辑模组，命名格式为：
- 数据模组目录名：`<模组名称>`（如 `Game`、`World`、`Battle`）
- 逻辑模组目录名：`<模组名称>Hotfix`（如 `GameHotfix`、`WorldHotfix`、`BattleHotfix`）

**模块对应规则**：实体对象的数据类定义在哪个数据模组中，其对应的 System 逻辑类就**必须**放在该数据模组对应的 Hotfix 逻辑模组中。例如：
- `LoadingScene` 定义在 `Game/Scene/` → `LoadingSceneSystem` 必须放在 `GameHotfix/Scene/`
- 若某个实体对象定义在 `World/` → 其 System 类必须放在 `WorldHotfix/`

每个模组的数据模组和逻辑模组目录结构一一对应。

## 4. 核心概念

### 4.1 实体对象继承体系

 +**框架层**（`GameEngine` 命名空间，不可修改）：  
```text
CBean
└── CBase                  # 对象基础抽象类，实现输入、事件、消息和数据的派发
    ├── CComponent         # 组件对象类型，通常作为 CEntity 的某个业务分支存在
    └── CRef               # 对象封装抽象类，提供定时器等功能接口
        ├── CObject        # 通用对象类型
        └── CEntity        # 实体对象封装抽象类，提供组件相关的服务接口
            ├── CScene     # 场景对象类型
            ├── CActor     # 角色对象类型
            └── CView      # 视图对象类型
```

**业务封装层**（`Game` 命名空间，可扩展通用逻辑）：
```text
CComponent ← Game.UComponent      # 业务层组件基类
CObject    ← Game.UObject         # 业务层常规对象基类
CScene     ← Game.UScene          # 业务层场景基类
CActor     ← Game.UActor          # 业务层角色基类
CView      ← Game.UView           # 业务层视图基类
```

**业务层**（具体业务对象，必须继承 U 类）：
```text
Game.UActor     ← Player, Monster, Slime ...
Game.UScene     ← LoadingScene, WorldScene ...
Game.UView      ← LoadingPanel, MailPanel ...
Game.UObject    ← 具体常规对象 ...
Game.UComponent ← MoveComponent, HitEffectComponent ...
```

> 业务对象**必须**继承 U 类（详见 2.8 规则），不可直接继承框架 C 类。


### 4.2 实体对象类型说明

| 类型 | 用途 | 关键约束 |
|-----|------|---------|
| `CScene` | 场景对象，不同功能场景各对应一种类型 | 同一时间**只能存在一个**场景实例 |
| `CActor` | 角色对象，用于定义游戏中的角色类型 | 不同角色区别在于默认挂载的组件不同 |
| `CView` | 视图对象，对应 UI 资源 | 同类型视图**只允许同时存在一个**实例 |
| `CObject` | 通用对象，功能单一但需要生命周期管理 | 无组件挂载能力 |
| `CComponent` | 组件对象，描述实体的某方面业务能力 | 同一实体中同类型组件**只允许一个**实例 |

> 只有继承自 `CEntity` 的对象（`CScene`、`CActor`、`CView`）才具备组件挂载及调度能力。

### 4.3 `CEntity`、`CComponent` 和 `System` 的关系

- **`CEntity`**：实体标识类，唯一标识游戏中的一个对象。自身只包含标识性数据，主要作为 `CComponent` 的容器使用。可通过 `ReplicateId` 特性标签绑定唯一标识，实现不同实例间的数据绑定。
- **`CComponent`**：组件标识类，描述 `CEntity` 的某方面属性/状态/业务。在同一个 `CEntity` 实例中，同类型组件只允许存在一个实例。实体加载某组件后即具备该组件负责的业务能力，所有转发给实体的数据也会转发给其所有组件。
- **`System`**：业务逻辑处理类，必须依附于一个实体对象或组件，提供具体业务的逻辑服务。每个 `System` 类单独服务一个 `CBean` 对象类，包括但不限于生命周期调度及数据通知处理。

### 4.4 声明实体对象的特性标签

| 对象类型 | 特性标签 | 示例 |
|---------|---------|------|
| 场景对象 | `[USceneClass("名称")]` | `[USceneClass("Login")]` |
| 角色对象 | `[UActorClass("名称")]` | `[UActorClass("LocalPlayer")]` |
| 视图对象 | `[UViewClass("名称")]` | `[UViewClass("GameLoginPanel")]` |
| 通用对象 | `[UObjectClass("名称")]` | `[UObjectClass("MonthlyCardActivity")]` |
| 组件对象 | `[UComponentClass("名称")]` | `[UComponentClass("AttributeComp")]` |

自动挂载组件的特性标签：
```csharp
[UComponentAutomaticActivationOfEntity(typeof(MoveComponent))]
```

组件复用示例——不同实体对象挂载同一个组件：
```csharp
[UComponentClass("MoveComponent")]
public sealed class MoveComponent : Game.UComponent { ... }

[UActorClass("LocalPlayer")]
[UComponentAutomaticActivationOfEntity(typeof(MoveComponent))]
public sealed class Player : Game.UActor { ... }

[UActorClass("Monster")]
[UComponentAutomaticActivationOfEntity(typeof(MoveComponent))]
public sealed class Monster : Game.UActor { ... }
```

### 4.5 生命周期

所有实体对象的生命周期流程（按执行顺序）：

| 阶段 | 方法 | 使用层 | 说明 |
|-----|------|-------|------|
| 创建阶段 | `Initialize` | 框架层 | 对象装配 |
| | `Startup` | 框架层 | 模组赋能 |
| | **`Awake`** | **业务层** | 业务初始化逻辑 |
| | **`Start`** | **业务层** | 若正在轮询中则延迟到下一帧 |
| 运行阶段 | `Execute` | 框架层 | 逻辑帧时序每帧调用 |
| | `LateExecute` | 框架层 | 逻辑帧时序每帧调用 |
| | `Update` | 框架层 / 业务层（可选） | 动画帧时序每帧调用；业务层可用于逐帧 UI/表现驱动 |
| | `LateUpdate` | 框架层 / 业务层（可选） | 动画帧时序每帧调用；用于晚于 Update 的收尾处理 |
| 销毁阶段 | **`Destroy`** | **业务层** | 业务数据清理，若正在轮询中则延迟到下一帧控制器轮询前执行 |
| | `Shutdown` | 框架层 | 模组数据清理 |
| | `Cleanup` | 框架层 | 对象清理 |

**业务层推荐使用** `Awake`、`Start`、`Destroy`；在确有逐帧需求时可使用 `Update`、`LateUpdate`。

> 当前项目 `Assets/Sources/GameHotfix/View/LoadingPanelUISystem.cs` 已使用 `[OnUpdate]` 进行进度条逐帧推进。
>
> 场景对象中涉及 `GameApi.OpenUI<T>()` 等依赖 UI 就绪时序的逻辑，推荐放在 `[OnStart]`，避免初始化阶段的时序问题。

绑定方式——通过扩展函数 + 特性标签：
```csharp
using Game;
using GameEngine;

static class PlayerSystem
{
    [OnAwake]
    static void Awake(this Player self) { ... }

    [OnDestroy]
    static void Destroy(this Player self) { ... }
}
```

> **命名空间引用规则**：逻辑模组（Hotfix）中的 System 类需要引用数据模组中定义的实体对象类型。由于实体对象继承自 `Game` 命名空间下的 U 类，System 类必须添加 `using Game;`。同时需要 `using GameEngine;` 以使用框架的特性标签和 API。

> 关联生命周期特性标签的目标函数，**必须是某个实体对象的扩展函数**。

### 4.6 数据推送

框架以数据驱动为核心，数据分为四类：

| 数据类型 | 说明 | 接收标签 |
|---------|------|---------|
| 输入数据 | 设备 IO（键盘、鼠标、摇杆、触屏等） | `[OnInput]` |
| 事件数据 | 程序内部主动发出的通知 | `[OnEvent]` |
| 消息数据 | 服务端网络下行数据 | `[OnMessage]` |
| 同步数据 | 实体对象字段/属性变更通知 | 待补充 |

不同模组的业务系统之间，通过上述数据通知进行联动。

#### 4.6.1 输入通知

输入数据类型：按键编码（虚拟按键编码 + 按键状态）、自定义数据结构（摇杆/触屏等封装后的结构体）。  
**注意**：在Unity平台，虚拟按键编码与Unity按键编码（UnityEngine.KeyCode）一致。

接收函数有三种绑定方式：
```csharp
// 方式一：全局接收（无实体对象绑定）
[OnInput(GameEngine.VirtualKeyCode.Space, GameEngine.InputOperationType.Released)]
static void OnRecvSpaceReleased(GameEngine.VirtualKeyCode keyCode, GameEngine.InputOperationType operationType) { ... }

// 方式二：指定实体类型接收（通过 typeof 参数）
[OnInput(typeof(MainScene), GameEngine.VirtualKeyCode.Space, GameEngine.InputOperationType.Pressed)]
static void OnRecvSpacePressed(MainScene mainScene, GameEngine.VirtualKeyCode keyCode, GameEngine.InputOperationType operationType) { ... }

// 方式三：通过实体对象扩展函数接收（推荐）
[OnInput(GameEngine.VirtualKeyCode.Space, GameEngine.InputOperationType.Moved)]
static void OnRecvSpaceMoved(this MainScene self, GameEngine.VirtualKeyCode keyCode, GameEngine.InputOperationType operationType) { ... }
```

同一个函数可以叠加多个 `[OnInput]` 标签，以响应多个按键：
```csharp
// 同时监听 Space 键和 Return 键，触发同一逻辑
[OnInput(GameEngine.VirtualKeyCode.Space, GameEngine.InputOperationType.Released)]
[OnInput(GameEngine.VirtualKeyCode.Return, GameEngine.InputOperationType.Released)]
static void OnRecvSpaceOrReturnReleased(this MainScene self, GameEngine.VirtualKeyCode keyCode, GameEngine.InputOperationType operationType)
{
    UnityEngine.Debug.Log($"按键：{keyCode}");
}
```

#### 4.6.2 事件通知

事件数据类型：`int` 事件标识、自定义 `struct` 数据结构。

自定义事件结构体定义在实体对象类内部，访问权限必须是 `public`：
```csharp
[UComponentClass("AttributeComp")]
public class AttributeComponent : Game.UComponent
{
    public struct LevelupNotify
    {
        public int level;
    }
}
```

接收函数绑定方式：
```csharp
// 接收 int 类型事件标识
[OnEvent(1001)]
static void OnRecvEvent(int eventId, params object[] args) { ... }

// 接收自定义结构体类型事件
[OnEvent(typeof(AttributeComponent.LevelupNotify))]
static void OnRecvLevelupNotify(AttributeComponent.LevelupNotify eventData) { ... }

// 指定实体类型接收
[OnEvent(typeof(MainScene), typeof(LoginClickEvent))]
static void OnRecvClickEvent(MainScene mainScene, LoginClickEvent eventData) { ... }

// 通过扩展函数接收（推荐）
[OnEvent(1001)]
static void OnRecvEvent(this MainScene self, int eventId, params object[] args) { ... }
```

同一个函数可以叠加多个 `[OnEvent]` 标签，以响应多个事件：
```csharp
// 同时监听 1001 事件和 1002 事件，触发同一逻辑
[OnEvent(1001)]
[OnEvent(1002)]
static void OnRecvEvent(this MainScene self, int eventId, params object[] args)
{
    UnityEngine.Debug.Log($"事件：{eventId}");
}
```

> 事件发送 API（`Send`/`Fire`）详见 `dev_api.md` 中"数据通知与转发"章节。

#### 4.6.3 消息通知

消息数据类型：操作码（唯一编码标识）、消息对象（根据协议类型定义）。

消息对象定义示例：
```csharp
[ProtoContract]
[Message(ProtoOpcode.EnterWorldResp)]
public partial class EnterWorldResp : Object, IMessage
{
    [ProtoMember(1)]
    public int Code { get; set; }
}
```

接收函数绑定方式：
```csharp
// 通过操作码接收
[OnMessage(ProtoOpcode.EnterMapResp)]
static void OnRecvMessage(ProtoBuf.Extension.IMessage msg) { ... }

// 指定实体类型接收
[OnMessage(typeof(MainScene), typeof(EnterMapResp))]
static void OnRecvMessage(MainScene mainScene, EnterMapResp message) { ... }

// 通过扩展函数接收（推荐）
[OnMessage(typeof(EnterMapResp))]
static void OnRecvMessage(this MainScene self, EnterMapResp message) { ... }
```

同一个函数可以叠加多个 `[OnMessage]` 标签，以响应多个消息：
```csharp
// 同时接收 ProtoOpcode.EnterMapResp 消息和 ProtoOpcode.LeaveMapResp 消息，触发同一逻辑
[OnMessage(ProtoOpcode.EnterMapResp)]
[OnMessage(ProtoOpcode.LeaveMapResp)]
static void OnRecvEvent(this MainScene self, ProtoBuf.Extension.IMessage msg)
{
    UnityEngine.Debug.Log($"消息类型：{typeof(msg)}");
}
```

#### 4.6.4 同步通知

暂无，后续再行补充。

---

## 5. 命名规范

### 5.1 通用编码命名规范

| 类型 | 规范 | 示例 |
|------|-----|------|
| 文件 | 帕斯卡命名 | `MainScene.cs`, `MailComponentMailSystem.cs` |
| 类/结构 | 帕斯卡命名 | `MainScene`, `MailComponent` |
| 枚举 | 帕斯卡命名 | `UserState`, `InputCode` |
| 函数 | 帕斯卡命名 | `EnterWorld()`, `SendMail()` |
| 属性 | 帕斯卡命名 | `PlayerName`, `ItemList` |
| 字段/变量 | 驼峰命名 | `playerName`, `itemList` |
| 私有字段 | 下划线前缀 | `_privateVar`, `__data` |
| 常量 | 全大写 + 下划线 | `MAX_RETRY_TIMES` |

### 5.2 `CBean` 对象类型命名规范

| 对象类型 | 命名格式 | 示例 |
|---------|---------|------|
| 场景对象 | `<业务名称>Scene` | `LoginScene`、`MainScene` |
| 角色对象 | `<业务名称>` | `Player`、`Monster` |
| 视图对象 | 与资源名一致 `<资源名称>` | `LoginPanel`、`MailPanel` |
| 通用对象 | `<业务名称>Object` 或 `<业务名称>` | `MonthlyCardActivityObject` |
| 组件对象 | `<业务名称>Component` | `AttributeComponent`、`MoveComponent` |

### 5.3 System 类命名规范

- 有明确功能描述：`<实体对象类名><功能描述>System`（如 `MailComponentMailSystem`）
- 无明确功能描述：`<实体对象类名>System`（如 `PlayerSystem`、`LoginSceneSystem`）

### 5.4 模组命名规范

- 数据模组：自定义名称（如 `Game`、`Battle`）
- 逻辑模组：`<数据模组名称>Hotfix`（如 `GameHotfix`、`BattleHotfix`）

---

## 6. 视图开发与 FGUI 资源规则

> FGUI 资源规则已迁移至独立文档，详见 `dev_fgui.md`。

---

## 相关文档

- **API 手册**：`dev_api.md` — 框架提供的所有可调用接口
- **FairyGUI 开发指南**：`dev_fgui.md` — FGUI 资源规则与操作 API
- **开发示例集**：`dev_examples.md` — 完整业务开发示例、代码模板和反模式


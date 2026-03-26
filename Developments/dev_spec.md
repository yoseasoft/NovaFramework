# `NovaFramework` 开发规范指南

> 本文档是 AI 开发助手的核心参考文档，定义在 `NovaFramework` 框架中进行业务开发时**必须遵守的规则、架构约束和代码组织方式**。

---

## 1. 项目概览

`NovaFramework` 是一个基于 `Unity` 引擎研发的客户端游戏框架，使用 `C#` 语言开发。
框架整体以**数据驱动**为核心，所有的业务流程都围绕数据对象的生命周期流程展开。

---

## 2. 目录结构

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
    ├── GameHotfix             # 【逻辑模组】扩展实体对象的业务逻辑
    │   ├── Manager            # 管理器目录
    │   ├── Msg                # 消息构造目录
    │   ├── Scene              # 场景对象逻辑类（System）
    │   ├── Object             # 角色对象和通用对象的逻辑类（System）
    │   ├── View               # 视图对象逻辑类（System）
    │   └── Component          # 组件对象逻辑类（System），与数据模组 Component 子目录一一对应
    ├── World                  # 【世界/仿真层数据模组】定义表现层组件
    │   └── Component          # 世界层组件数据类（如动画相关组件）
    └── WorldHotfix            # 【世界层逻辑模组】扩展世界层组件的业务逻辑
        ├── Component          # 世界层组件逻辑类（System）
        └── Object             # 世界层角色逻辑类（System）
```

### 2.1 文件放置规则

| 文件类型 | 放置位置 | 示例 |
|---------|---------|------|
| 实体对象数据类 | `Game/<对象类型>/` | `Game/Component/Mail/MailComponent.cs` |
| 事件结构体 | 定义在实体对象类内部 | `MailComponent` 类内部的 `public struct` |
| System 逻辑类 | `GameHotfix/<对象类型>/` | `GameHotfix/Component/Mail/MailComponentMailSystem.cs` |
| 管理器 | `GameHotfix/Manager/` | `GameHotfix/Manager/MailManager.cs` |

### 2.2 多模组结构

项目包含多个独立模组，每个数据模组都有一个对应的逻辑模组，命名格式为：
- 数据模组目录名：`<模组名称>`（如 `Game`、`World`、`Battle`）
- 逻辑模组目录名：`<模组名称>Hotfix`（如 `GameHotfix`、`WorldHotfix`、`BattleHotfix`）

当前项目存在两组模组：
- `Game` / `GameHotfix` — 核心游戏逻辑（场景、角色、UI、组件）
- `World` / `WorldHotfix` — 世界/仿真表现层（动画组件等，依赖 Game + GameHotfix）

**模块对应规则**：实体对象的数据类定义在哪个数据模组中，其对应的 System 逻辑类就**必须**放在该数据模组对应的 Hotfix 逻辑模组中。例如：
- `LoadingScene` 定义在 `Game/Scene/` → `LoadingSceneSystem` 必须放在 `GameHotfix/Scene/`
- `SoldierAnimationComponent` 定义在 `World/Component/` → `SoldierAnimationComponentSystem` 必须放在 `WorldHotfix/Component/`

每个模组的数据模组和逻辑模组目录结构一一对应。

---

## 3. 核心概念

### 3.1 实体对象继承体系

**框架层**（`GameEngine` 命名空间，不可修改）：
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

场景/视图对象直接继承 U 类：
```text
Game.UScene     ← LogoScene, LoadingScene, WorldScene
Game.UView      ← LoginPanel, LoadingPanel, ItemChoicePanel
Game.UObject    ← 具体常规对象
```

角色对象通过中间基类继承，共享组件挂载声明：
```text
Game.UActor
├── Actor              # 中间基类（非 sealed），挂载 IdentityComponent + TransformComponent
│   ├── Soldier        # 中间基类（非 sealed），挂载 AttributeComponent
│   │   ├── Player     # 叶子类（sealed），挂载 AttackComponent
│   │   └── Monster    # 叶子类（sealed）
│   ├── Npc            # 叶子类（sealed）
│   └── Trap           # 叶子类（sealed）
└── Map                # 直接继承 UActor，挂载 IdentityComponent
```

> 中间基类（如 `Actor`、`Soldier`）是非 sealed 的，用于通过继承让多个子类共享 `[UComponentAutomaticActivationOfEntity]` 声明。子类自动继承父类声明的组件挂载。

组件对象继承 U 类：
```text
Game.UComponent ← AttackComponent, AttributeComponent, IdentityComponent, TransformComponent, ...
```

### 3.2 实体对象类型说明

| 类型 | 用途 | 关键约束 |
|-----|------|---------|
| `CScene` | 场景对象，不同功能场景各对应一种类型 | 同一时间**只能存在一个**场景实例 |
| `CActor` | 角色对象，用于定义游戏中的角色类型 | 不同角色区别在于默认挂载的组件不同 |
| `CView` | 视图对象，对应 UI 资源 | 同类型视图**只允许同时存在一个**实例 |
| `CObject` | 通用对象，功能单一但需要生命周期管理 | 无组件挂载能力 |
| `CComponent` | 组件对象，描述实体的某方面业务能力 | 同一实体中同类型组件**只允许一个**实例 |

> 只有继承自 `CEntity` 的对象（`CScene`、`CActor`、`CView`）才具备组件挂载及调度能力。

### 3.3 `CEntity`、`CComponent` 和 `System` 的关系

- **`CEntity`**：实体标识类，唯一标识游戏中的一个对象。自身只包含标识性数据，主要作为 `CComponent` 的容器使用。可通过 `ReplicateId` 特性标签绑定唯一标识，实现不同实例间的数据绑定。
- **`CComponent`**：组件标识类，描述 `CEntity` 的某方面属性/状态/业务。在同一个 `CEntity` 实例中，同类型组件只允许存在一个实例。实体加载某组件后即具备该组件负责的业务能力，所有转发给实体的数据也会转发给其所有组件。
- **`System`**：业务逻辑处理类，必须依附于一个实体对象或组件，提供具体业务的逻辑服务。每个 `System` 类单独服务一个 `CBean` 对象类，包括但不限于生命周期调度及数据通知处理。

### 3.4 声明实体对象的特性标签

| 对象类型 | 特性标签 | 示例 |
|---------|---------|------|
| 场景对象 | `[USceneClass("名称")]` | `[USceneClass("Login")]` |
| 角色对象 | `[UActorClass("名称")]` | `[UActorClass("LocalPlayer")]` |
| 视图对象 | `[UViewClass("名称")]` | `[UViewClass("GameLoginPanel")]` |
| 通用对象 | `[UObjectClass("名称")]` | `[UObjectClass("MonthlyCardActivity")]` |
| 组件对象 | `[UComponentClass("名称")]` | `[UComponentClass("AttributeComp")]` |

> **特性标签的使用场景**：特性标签在通过名称创建对象（如 `ApplicationContext.CreateBean(beanName)` 或 `GameApi.ReplaceScene("Loading")`）时是必需的。若仅通过泛型 API（如 `GameApi.CreateActor<Player>()`）或通过 `[UComponentAutomaticActivationOfEntity]` 自动挂载时，可省略特性标签。当前项目中，场景对象和视图对象均标注了特性标签，角色对象和组件对象未标注。

自动挂载组件的特性标签：
```csharp
[UComponentAutomaticActivationOfEntity(typeof(MoveComponent))]
```

> `[UComponentAutomaticActivationOfEntity]` 支持继承——父类声明的自动挂载组件会被所有子类继承。

组件复用示例——通过中间基类共享组件挂载：
```csharp
// Actor 作为中间基类，所有子类自动挂载 IdentityComponent 和 TransformComponent
[UComponentAutomaticActivationOfEntity(typeof(IdentityComponent))]
[UComponentAutomaticActivationOfEntity(typeof(TransformComponent))]
public class Actor : UActor { }

// Soldier 继承 Actor，额外挂载 AttributeComponent
[UComponentAutomaticActivationOfEntity(typeof(AttributeComponent))]
public class Soldier : Actor { }

// Player 继承 Soldier，额外挂载 AttackComponent
// 最终 Player 拥有：IdentityComponent + TransformComponent + AttributeComponent + AttackComponent
[UComponentAutomaticActivationOfEntity(typeof(AttackComponent))]
public sealed class Player : Soldier { }

// Monster 继承 Soldier，不额外挂载
// 最终 Monster 拥有：IdentityComponent + TransformComponent + AttributeComponent
public sealed class Monster : Soldier { }
```

### 3.5 业务层二次封装规则

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
- ❌ **禁止**业务实体对象直接继承 `GameEngine.CActor`、`GameEngine.CScene`、`GameEngine.CComponent` 等框架基类

正确示例：
```csharp
// ✅ 正确：场景继承 Game.UScene
[USceneClass("Loading")]
public sealed class LoadingScene : Game.UScene { }

// ✅ 正确：角色继承 Game.UActor（或继承 Game.UActor 的中间基类）
public sealed class Player : Game.Soldier { }  // Game.Soldier → Game.Actor → Game.UActor

// ✅ 正确：组件继承 Game.UComponent
public sealed class AttackComponent : Game.UComponent { ... }
```

错误示例：
```csharp
// ❌ 错误：场景直接继承框架基类
public sealed class LoadingScene : GameEngine.CScene { ... }

// ❌ 错误：角色直接继承框架基类
public sealed class Player : GameEngine.CActor { ... }

// ❌ 错误：组件直接继承框架基类
public sealed class AttackComponent : GameEngine.CComponent { ... }
```

### 3.6 生命周期

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

> 当前项目 `Assets/Sources/GameHotfix/Component/AttackComponentSystem.cs` 已使用 `[OnUpdate]` 进行攻击冷却倒计时。
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

### 3.7 数据推送

框架以数据驱动为核心，数据分为四类：

| 数据类型 | 说明 | 接收标签 |
|---------|------|---------|
| 输入数据 | 设备 IO（键盘、鼠标、摇杆、触屏等） | `[OnInput]` |
| 事件数据 | 程序内部主动发出的通知 | `[OnEvent]` |
| 消息数据 | 服务端网络下行数据 | `[OnMessage]` |
| 同步数据 | 实体对象字段/属性变更通知 | `[OnReplicate]` |

不同模组的业务系统之间，通过上述数据通知进行联动。

#### 3.7.1 输入通知

输入数据类型：按键编码（虚拟按键编码 + 按键状态）、自定义数据结构（摇杆/触屏等封装后的结构体）。
**注意**：在Unity平台，虚拟按键编码与Unity按键编码（UnityEngine.KeyCode）一致，使用时需强制类型转换 `(VirtualKeyCode) UnityEngine.KeyCode.A`。

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

**简化签名**：通过扩展函数接收时，`keyCode` 和 `operationType` 参数可省略——仅关心"是否触发"时更简洁：
```csharp
// ✅ 简化签名（推荐，不需要区分具体按键时）
[OnInput((VirtualKeyCode) UnityEngine.KeyCode.A, GameEngine.InputOperationType.Released)]
static void OnStartGameClicked(this LogoScene self) { ... }

// ✅ 完整签名（需要区分按键或状态时）
[OnInput((VirtualKeyCode) UnityEngine.KeyCode.A, GameEngine.InputOperationType.Released)]
static void OnStartGameClicked(this LogoScene self, VirtualKeyCode keyCode, InputOperationType operationType) { ... }
```

同一个函数可以叠加多个 `[OnInput]` 标签，以响应多个按键：
```csharp
// 同时监听 W 键和 UpArrow 键，触发同一逻辑
[OnInput((VirtualKeyCode) UnityEngine.KeyCode.W, GameEngine.InputOperationType.Released)]
[OnInput((VirtualKeyCode) UnityEngine.KeyCode.UpArrow, GameEngine.InputOperationType.Released)]
static void OnMoveUp(this Player self)
{
    self.MoveTo(0f, 1f);
}
```

#### 3.7.2 事件通知

事件数据类型：`int` 事件标识、自定义 `struct` 数据结构。

自定义事件结构体定义在实体对象类内部，访问权限必须是 `public`：
```csharp
[UComponentClass("AttributeComp")]
public sealed class AttributeComponent : Game.UComponent
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

**事件派发方式**：

| 方法 | 派发时机 | 接收范围 |
|-----|---------|---------|
| `GameApi.Send(...)` | 下一帧 | 全局 |
| `GameApi.Fire(...)` | 当前帧立即 | 全局 |
| `entity.Send(...)` / `component.Send(...)` | 下一帧 | 该实体及其组件 |
| `entity.Fire(...)` / `component.Fire(...)` | 当前帧立即 | 该实体及其组件 |
| `entity.SendToSelf(struct)` | 当前帧立即 | 仅该实体自身及其组件（定向派发） |

> `SendToSelf` 与 `Fire` 的区别：`Fire` 在调用方所属的实体上派发，`SendToSelf` 在指定的目标实体上派发。用于向其他实体发送定向事件，典型场景如攻击命中（向目标发送伤害通知）。

```csharp
// SendToSelf 示例：攻击者向被攻击目标发送命中事件
target.SendToSelf(new AttackComponent.HitTargetReq
{
    attackerId = self.GetComponent<IdentityComponent>().uid,
    skillId = 0,
    damage = 50,
});
```

> 事件发送 API（`Send`/`Fire`/`SendToSelf`）详见 `dev_api.md` 中"数据通知与转发"章节。

#### 3.7.3 消息通知

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

#### 3.7.4 同步通知

暂无，后续再行补充。

---

## 4. 强制规则

> 以下规则为框架的核心约束，**违反即为错误**，在编写任何代码前必须牢记。

### 4.1 模组分离规则

- ✅ 所有实体对象的**数据定义**必须放在数据模组中（如 `Game`）
- ✅ 所有**业务逻辑**必须放在逻辑模组中（如 `GameHotfix`）
- ❌ **禁止**在数据模组中编写任何业务逻辑
- ❌ **禁止**在逻辑模组中定义任何数据字段

### 4.2 System 类规则

- ✅ 所有 `System` 类必须是 `static` 类（无状态）
- ✅ `System` 类内部的所有函数必须是 `static` 函数或指定实体对象类型的扩展函数
- ✅ `System` 类推荐使用默认访问权限（`internal`）
- ✅ `System` 类命名格式：`<实体对象类名><功能描述>System`；若无明显功能描述，则为 `<实体对象类名>System`
- ✅ 一个 `System` 类只服务于一个指定的 `CBean` 对象类
- ❌ **禁止**在 `System` 类中定义任何字段或属性

### 4.3 业务逻辑规则

- ✅ 所有业务逻辑**必须依附于某个实体对象类型**
- ✅ 业务逻辑通过扩展函数 + 特性标签绑定到实体对象的生命周期节点
- ✅ 业务层生命周期推荐使用 `[OnAwake]`、`[OnStart]`、`[OnDestroy]`；按需可使用 `[OnUpdate]`、`[OnLateUpdate]`（如 UI 逐帧驱动）
- ❌ **禁止**重载实体对象的生命周期函数来处理业务逻辑
- ❌ **禁止** `OnInitialize`、`OnStartup`、`OnShutdown`、`OnCleanup` 在业务层使用（这些由底层框架使用）

### 4.4 组件通信规则

- ✅ 不同业务之间必须通过**事件、消息、数据同步**等通知方式进行互通
- ✅ 逻辑层到数据层是**单向访问**
- ❌ **禁止**不同 `CComponent` 之间直接调用彼此的业务接口函数
- ❌ **禁止**不同数据模型之间直接进行接口函数调用

### 4.5 组件职责规则

- ✅ 实体对象（`CActor`）的每一种独立行为/表现能力都应拆分为单独的 `CComponent`
- ✅ 例如：移动能力 → `MoveComponent`、受击表现 → `HitEffectComponent`、攻击能力 → `AttackComponent`
- ❌ **禁止**将多种不同的行为逻辑直接写在实体对象的 System 类中，应通过组件拆分

### 4.6 实体对象创建规则

- ✅ 所有基于 `CBean` 的实体对象必须通过框架提供的 `API` 函数创建和销毁
- ❌ **禁止**通过 `new` 关键字创建实体对象实例
- ❌ **禁止**依赖 `GC` 自动垃圾回收来销毁实体对象

### 4.7 资源管理规则

- ✅ 通过 `GameApi.LoadAsset` / `GameApi.AsyncLoadAsset` 主动加载的资源，必须通过 `GameApi.UnloadAsset` 手动释放
- ✅ 通过 `GameApi.LoadAssetScene` 加载的场景资源，必须通过 `GameApi.UnloadAssetScene` 手动释放
- ❌ **禁止**依赖 GC 自动释放主动加载的资源

---

## 5. 反模式（常见错误写法）

> 以下列出常见的违规代码模式，每项均给出错误写法和正确写法对照。

### 5.1 ❌ 直接继承框架 C 类

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

### 5.2 ❌ 使用 CComponentAutomaticActivationOfEntity 而非 U 版本

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

### 5.3 ❌ 在数据类中编写业务逻辑

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

### 5.4 ❌ System 类中定义状态数据

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

### 5.5 ❌ 组件之间直接调用业务接口

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

### 5.6 ❌ 把多种行为写在角色 System 里而不拆分组件

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

### 5.7 ❌ 重载生命周期函数

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

### 5.8 ❌ 使用 new 创建实体对象

```csharp
var player = new Player();              // ❌
```

```csharp
var player = GameApi.CreateActor<Player>();  // ✅
```

### 5.9 ❌ 使用底层框架生命周期标签

```csharp
[OnInitialize] static void Init(this Player self) { ... }   // ❌ 框架层专用
[OnShutdown]   static void Down(this Player self) { ... }   // ❌ 框架层专用
```

```csharp
[OnAwake]   static void Awake(this Player self) { ... }     // ✅
[OnStart]   static void Start(this Player self) { ... }     // ✅
[OnDestroy] static void Destroy(this Player self) { ... }   // ✅
```

### 5.10 ❌ System 类不是 static

```csharp
class PlayerSystem { ... }         // ❌ 缺少 static
```

```csharp
static class PlayerSystem { ... }  // ✅
```

### 5.11 ❌ Send / Fire 用错场景

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

### 5.12 ❌ 主动加载的资源未手动释放

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

### 5.13 ❌ 实体对象放错模组

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

## 6. 命名规范

### 6.1 通用编码命名规范

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

### 6.2 `CBean` 对象类型命名规范

| 对象类型 | 命名格式 | 示例 |
|---------|---------|------|
| 场景对象 | `<业务名称>Scene` | `LoginScene`、`MainScene` |
| 角色对象 | `<业务名称>` | `Player`、`Monster` |
| 视图对象 | 与资源名一致 `<资源名称>` | `LoginPanel`、`MailPanel` |
| 通用对象 | `<业务名称>Object` 或 `<业务名称>` | `MonthlyCardActivityObject` |
| 组件对象 | `<业务名称>Component` | `AttributeComponent`、`MoveComponent` |

### 6.3 System 类命名规范

- 有明确功能描述：`<实体对象类名><功能描述>System`（如 `MailComponentMailSystem`）
- 无明确功能描述：`<实体对象类名>System`（如 `PlayerSystem`、`LoginSceneSystem`）

### 6.4 模组命名规范

- 数据模组：自定义名称（如 `Game`、`Battle`）
- 逻辑模组：`<数据模组名称>Hotfix`（如 `GameHotfix`、`BattleHotfix`）

---

## 7. 组件间数据访问

### 7.1 兄弟组件访问

在组件 System 中，可通过 `self.GetComponent<T>()` 直接获取**同一实体上的兄弟组件**数据：

```csharp
static class AttributeComponentSystem
{
    public static void ReloadConfig(this AttributeComponent self)
    {
        // 获取同一实体上的 IdentityComponent
        IdentityComponent identityComponent = self.GetComponent<IdentityComponent>();
        Config.ActorConfig actorConfig = Config.ActorConfigTable.Get(identityComponent.classId);
        ...
    }
}
```

> `self.GetComponent<T>()` 在组件上调用时等同于 `self.Entity.GetComponent<T>()`——访问的是所属实体上的兄弟组件。

> 这属于**只读数据访问**，不违反组件通信规则（4.4）。规则禁止的是跨组件**调用业务接口函数**，而非读取数据字段。

### 7.2 获取所属实体

```csharp
// 获取组件所属的实体对象（返回 CEntity 基类，需类型转换）
Player player = self.Entity as Player;
```

---

## 8. 常见场景 Checklist

### 8.1 新增一个业务功能（组件级别）

- [ ] 在 `Game/Component/<功能名>/` 下创建 `<功能名>Component.cs`，**继承 `Game.UComponent`**
- [ ] 在数据类中定义数据字段和事件结构体（`public struct`）
- [ ] 在 `GameHotfix/Component/<功能名>/` 下创建 System 逻辑类
- [ ] 在逻辑类中用 `[OnAwake]` 和 `[OnDestroy]` 处理初始化和清理
- [ ] 在需要使用此组件的实体对象上添加 **`[UComponentAutomaticActivationOfEntity]`**
- [ ] 如果需要 UI，同步创建对应的视图对象

### 8.2 新增一个 UI 面板

- [ ] 确保 UI 资源已准备好，放置在 `_Resources/Gui/`
- [ ] 在 `Game/View/` 下创建视图数据类，**继承 `Game.UView`**，`[UViewClass]` 名称与资源名一致
- [ ] 在 `GameHotfix/View/` 下创建 System 逻辑类
- [ ] 通过 `[OnEvent]` 监听数据变更事件来刷新 UI
- [ ] 在需要的地方调用 `GameApi.OpenUI<T>()` 打开面板

### 8.3 新增一个场景

- [ ] 在 `Game/Scene/` 下创建场景数据类，**继承 `Game.UScene`**
- [ ] 通过 **`[UComponentAutomaticActivationOfEntity]`** 挂载场景所需的组件
- [ ] 在对应的 Hotfix 模组中创建 System 逻辑类
- [ ] 在 `[OnStart]` 中初始化场景（创建角色、打开 UI 等）
- [ ] 通过 `GameApi.ReplaceScene<T>()` 切换到此场景

### 8.4 实现跨组件通信

- [ ] 在**接收方**组件的数据类中定义事件结构体
- [ ] 在**发送方** System 中通过 `GameApi.Send/Fire` 或 `entity/component.Send/Fire` 派发
- [ ] 在**接收方** System 中通过 `[OnEvent]` 接收
- [ ] **确认没有直接调用其他组件的业务方法**

### 8.5 加载和释放资源

- [ ] 使用 `GameApi.LoadAsset` / `AsyncLoadAsset` 加载
- [ ] 记录已加载的资源引用（一般存在组件数据字段中）
- [ ] 在 `[OnDestroy]` 或不再需要时调用 `GameApi.UnloadAsset` 释放
- [ ] 场景资源用 `LoadAssetScene` / `UnloadAssetScene` 配对管理

---

## 9. 视图开发与 FGUI 资源规则

> FGUI 资源规则已迁移至独立文档，详见 `dev_fgui.md`。

---

## 相关文档

- **API 手册**：`dev_api.md` — 框架提供的所有可调用接口
- **FairyGUI 开发指南**：`dev_fgui.md` — FGUI 资源规则与操作 API
- **开发示例集**：`dev_examples.md` — 完整业务开发示例和代码模板
- **策划需求文档指南**：`dev_design.md` — 策划如何用配置表语言描述需求

# NovaFramework 开发规范指南

> 该文档是开发助手指南，指导如何在`NovaFramework`框架中进行业务开发；  

## 目录

---

## 1. 项目概览

`NovaFramework`是一个基于`Unity`引擎研发的客户端游戏框架，使用`C#`语言开发；  
框架整体以数据驱动为核心，所有的业务流程都围绕数据对象的生命周期流程展开；  

**关键约束**
- 所有的实体对象都来源于`CBean`
- 实体类自身只定义数据和通知转发相关的结构体，所有的业务逻辑都在`System`中实现
- 实体对象中的不同业务类型通过`CComponent`进行分割，业务与业务之间通过数据或事件、消息等通知的方式进行互通，禁止相互之间直接调用业务接口
- 所有的`System`均为`static`类，也就是说所有的业务逻辑均为无状态模式运行
- 定义`System`时推荐使用默认访问权限，因为逻辑层到数据层是单向访问的，且不同数据模型之间也禁止直接进行接口函数调用

---

## 2. 目录规范

业务系统开发的推荐目录结构如下：

```
Assets/Sources
├── Agen               # 配置文件、协议文件、API文件等自动生成代码
├── Game               # 数据层，定义实体对象
│   ├── Core           # 框架启动配置
│   ├── Scene          # 场景对象，每个类对应一种场景类型
│   ├── Object         # 角色对象，每个类对应一种角色类型
│   ├── View           # 视图对象，每个类对应一种视图类型
│   └── Component      # 组件对象，内部以不同业务功能进行切分
└── GameHotfix         # 逻辑层，扩展实体对象的业务逻辑
    ├── Manager        # 管理器
    ├── Scene          # 场景对象系统逻辑
    ├── Object         # 角色对象系统逻辑
    ├── View           # 视图对象系统逻辑
    └── Component      # 组件对象系统逻辑，内部以不同业务功能进行切分，与`Game`中的组件目录结构一一对应
```

所有的实体对象定义，均放置在`Game`程序集中，包括数据通知的结构体和其它原生对象类型。  
因此`Game`程序集可看作是一个数据容器，禁止在该程序集中添加任何业务逻辑；

框架规定所有的业务逻辑，都必须依附于某个实体对象类型，由该实体对象类型中所涉及到的数据，驱动业务流程的推进。  
所有业务系统，均放置在`GameHotfix`程序集中，包括在业务系统中需要用到的管理器或其它封装接口。  
在`GameHotfix`程序集中不允许定义任何数据，所有类均为静态类，内部的函数也全部为静态函数或指定实体对象类型的扩展函数，  
由于每个业务系统都依附于某个实体对象类型，所以业务系统的类名一般使用“实体对象类名称+业务功能+System”的格式，  
在这个系统类的内部，所有的函数接口或扩展的目标对象，也都是对应的实体对象类型。  

---

## 3. 核心概念

### 3.1 `CBean`实体对象

实体对象是业务系统的基础单元，定义业务所需的核心数据元素；  
以下为实体对象类型的完整继承结构：
```
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

在业务中主要使用的实体对象类型有：
- `CScene`：场景对象，不同功能的场景都分别对应一种场景类型，同一时间游戏中只能存在一个场景实例；
- `CActor`：角色对象，用于定义游戏中的角色类型，不同角色的区别在于默认挂载的组件不同；
- `CView`：视图对象，用于定义游戏中的UI元素，每个用于实际展示的UI资源都分别对应一种视图类型；
- `CObject`：通用对象，适用于功能单一，但又兼具生命周期流程管理的对象类型；
- `CComponent`：组件对象，用于定义某类具体的功能业务，每个组件都是其所属实体对象的一个具体业务的实现；

不同类型的实体对象，可以通过加载同一个业务组件，来实现功能逻辑的复用；  
但只有继承自`CEntity`的实体对象，才具备组件挂载及调度能力；  
```csharp
[CComponentClass()]
public class MoveComponent : CComponent { ... } // 定义一个移动组件

[CActorClass()]
[CComponentAutomaticActivationOfEntity(typeof(MoveComponent))]
public sealed class Player : CActor { ... } // 创建一个玩家类型角色对象，并自动挂载移动组件

[CActorClass()]
[CComponentAutomaticActivationOfEntity(typeof(MoveComponent))]
public sealed class Monster : CActor { ... } // 创建一个怪物类型角色对象，并自动挂载移动组件
```

所有实体对象都具备以下的生命周期流程：
| 方法 | 描述 |
|-----|------|
| Initialize | 初始化接口，创建对象时调用，通常用于框架内的对象装配 |
| Startup | 启动接口，对象初始化完成后调用，通常用于框架内的模组赋能 |
| Awake | 唤醒接口，对象在启动成功后调用，一般由业务层实现具体的功能逻辑 |
| Start | 开始接口，对象唤醒后，会检测当前的实体控制器是否处于实例轮询状态，若当前正在轮询中，将延迟到下一帧执行 |
| Execute | 执行接口，对象开始运行后，会以逻辑帧时序每帧调用一次，通常用于框架内的对象逻辑执行 |
| LateExecute | 延迟执行接口，对象开始运行后，会以逻辑帧时序每帧调用一次，通常用于框架内的对象逻辑执行 |
| Update | 更新接口，对象开始运行后，会以动画帧时序每帧调用一次，通常用于框架内的对象更新 |
| LateUpdate | 延迟更新接口，对象开始运行后，会以动画帧时序每帧调用一次，通常用于框架内的对象更新 |
| Destroy | 销毁接口，对象销毁时调用，一般用于业务层的数据清理，若当前实例正处于轮询中，将延迟到下一帧实体控制器轮询前执行 |
| Shutdown | 停止接口，对象销毁完成后调用，通常用于框架内的模组数据清理 |
| Cleanup | 清理接口，对象完全停止后调用，通常用于框架内的对象清理 |

默认的生命周期函数由框架负责调用，禁止在具体业务开发中通过重载生命周期函数来处理业务逻辑，  
推荐使用`System`来执行具体的业务逻辑开发，并通过框架提供的特性标签将业务逻辑关联到指定的生命周期节点上；  
```csharp
static class PlayerSystem
{
    [OnAwake]
    static void xxx_Awake(this Player self) { ... }
}
```
需要注意的是，我们需要关联生命周期特性标签的目标函数，必须是某个实体对象的扩展函数；  
当某个实体对象类型的扩展函数绑定了指定的生命周期特性标签，则框架会自动在该类型对象实例进入目标生命周期时调用其关联的扩展函数；  
其中`OnInitialize`、`OnStartup`和`OnShutdown`、`OnCleanup`为底层框架使用，业务层推荐使用`OnAwake`、`OnStart`和`OnDestroy`来处理业务逻辑。  

---

### 3.2 数据推送

框架的驱动是以数据为核心，引擎将数据分为以下四类：
- 输入数据：设备IO传入的数据，如键盘、鼠标等；
- 事件数据：程序内部主动发出的通知；
- 消息数据：服务端的网络下行数据；
- 同步数据：实体对象内部被标记的字段或属性发生改变后所发出的通知；

在业务系统中，可以通过绑定标签的方式，来接收数据的通知。  
不同模块的业务系统之间，正是通过上述数据的通知来进行联动的。  

#### 3.2.1 输入通知接口定义

输入数据分为两种类型：
- 按键编码：采集`UnityEngine.KeyCode`按键信息及按键状态（按下、释放、移动或双击等）
- 自定义数据结构：将摇杆、触屏点击等信息封装后的数据结构

在业务逻辑中，通过定义接收函数及对应的输入标签，来捕获对应的输入数据通知
```csharp
// 接收空格和返回按键的释放信息
[OnInput((int) UnityEngine.KeyCode.Space, GameEngine.InputOperationType.Released)]
[OnInput((int) UnityEngine.KeyCode.Return, GameEngine.InputOperationType.Released)]
static void OnRecvSpaceReleased(int keyCode, int operationType)
{ ... }

// 所有 MainScene 实体对象实例将接收到空格按键的按下信息
[OnInput(typeof(MainScene), (int) UnityEngine.KeyCode.Space, GameEngine.InputOperationType.Pressed)]
static void OnRecvSpacePressedByMainScene(MainScene mainScene, int keyCode, int operationType)
{ ... }

// 通过 MainScene 实体对象的扩展函数实现其实例接收到空格按键的按下信息
[OnInput((int) UnityEngine.KeyCode.Space, GameEngine.InputOperationType.Pressed)]
static void OnRecvSpacePressed(this MainScene self, int keyCode, int operationType)
{ ... }
```

当输入行为被触发后（`Unity`捕获到输入按键或通过框架的模拟按键主动发送），将自动触发绑定了相关按键信息的接口函数执行；  

#### 3.2.2 事件通知接口定义

事件数据分为两种类型：
- 事件标识：全局唯一的`int`类型标识，用于区分不同的事件通知
- 自定义数据结构：`struct`类型的对象，可以定义在实体对象类的内部，但访问权限必须是公有的
```csharp
public class AttributeComponent : GameEngine.CComponent
{
    ...
    // 针对属性业务的自定义事件的数据结构
    public struct LevelupNotify
    {
        public int level; // 事件的数据结构中可以自定义数据成员
    }
}
```

在业务逻辑中，通过定义接收函数及对应的事件标签，来捕获对应的事件数据通知
```csharp
// 接收 1001 和 1002 的事件通知
[OnEvent(1001)]
[OnEvent(1002)]
static void OnRecvClickEvent(int eventId, params object[] args)
{ ... }

// 接收 LevelupNotify 的事件通知
[OnEvent(typeof(AttributeComponent.LevelupNotify))]
static void OnRecvLevelupNotify(AttributeComponent.LevelupNotify eventData)
{ ... }

// 针对指定实体对象类型的事件数据转发
[OnGlobalEvent(typeof(MainScene), typeof(LoginClickEvent))]
static void OnRecvClickEvent(MainScene mainScene, LoginClickEvent eventData)
{ ... }

// 针对指定实体对象实例的事件数据转发
[OnBeanInput(1001)]
static void OnRecvClickEvent(this MainScene self, int eventId, params object[] args)
{ ... }
```

当业务层输入行为被触发后（`Unity`捕获到输入按键或通过框架的模拟按键主动发送），将自动触发绑定了相关按键信息的接口函数执行；  

#### 3.2.3 消息通知接口定义
```csharp
// 针对全局的消息数据转发
[OnGlobalMessage(typeof(PlayerLevelup)]
static void OnRecvLevelupMessage(PlayerLevelup message)
{ ... }

// 针对指定实体对象类型的消息数据转发
[OnGlobalMessage(typeof(MainScene), typeof(PlayerLevelup))]
static void OnRecvClickEvent(MainScene mainScene, PlayerLevelup message)
{ ... }

// 针对指定实体对象实例的消息数据转发
[OnBeanMessage(typeof(PlayerLevelup))]
static void OnRecvClickEvent(this MainScene self, PlayerLevelup message)
{ ... }
```

---

## 4. 命名规范

| 类型 | 规范 | 示例 |
|------|-----|------|
| 文件 | 小写字母 + 下划线 | `basic.lua`, `mail_sys.lua` |
| 函数 | 驼峰命名 | `encodeInfo()`, `sendMail()` |
| 变量 | 驼峰命名 | `playerName`, `itemList` |
| 私有变量 | 下划线前缀 | `_privateVar`, `__data` |
| 常量 | 全大写 + 下划线 | `MAX_RETRY_TIMES` |
| 类/类型 | 帕斯卡命名 | `BasicCom`, `MailSys` |

---


`CBean`代表了框架中所有的数据对象的公共基类，其内部定义了所有数据对象通用的属性成员、通知转发及生命周期访问的接口。

## 属性成员

`CBean`对象中定义的属性成员包括：

| 属性名称   | 类型          | 功能                                             |
|------------|---------------|--------------------------------------------------|
| `BeanId`   | `int`         | 自动生成的对象唯一标识                           |
| `BeanName` | `string`      | 当前构建对象所用的名称，可通过`Bean`配置文件定义 |
| `BeanType` | `System.Type` | 对象实例化所使用的具体类型，值等同于`GetType()`  |
| `Userdata` | `object`      | 临时用户数据                                     |

这里定义的所有属性，在对象被创建的那一刻，就已全部初始化完成，且直到对象实例被销毁前，其整个生命周期内属性都不能被修改。  
所以这里的属性，可以认为是由系统为`CBean`对象分配身份标识，您可以通过该标识来查找其所对应的实例。

## 通知转发

`CBean`对象支持消息、事件和输入三类数据的调度转发，分别对应于：

#### 1. 网络消息数据分发

您可以通过特性注入的方式，指定某个接口函数对操作码（`Opcode`）或消息类型（`MessageType`）进行监听，当产生对应的消息数据时，该接口函数将自动接收到此数据。

```csharp
// 对`MessageErrorNotify`和`KickNotify`两个操作码进行监听
[GameEngine.MessageListenerBindingOfTarget(Proto.ProtoOpcode.MessageErrorNotify)]
[GameEngine.MessageListenerBindingOfTarget(Proto.ProtoOpcode.KickNotify)]
private static void OnRecvMessageByCode(this MainRoleComponent self, ProtoBuf.Extension.IMessage message)
{ /* do sth */ }

...

// 对`MessageErrorNotify`消息类型进行监听
[GameEngine.MessageListenerBindingOfTarget(typeof(Proto.MessageErrorNotify))]
private static void OnRecvMessageByType(this MainRoleComponent self, Proto.MessageErrorNotify message)
{ /* do sth */ }
```

当然，您也可以使用全局分发的形式，让指定类型的`CBean`对象实例接收到该消息数据。

```csharp
// 对`EnterWorldResp`消息类型进行监听
[GameEngine.OnMessageDispatchCall(typeof(BattleScene), typeof(Proto.EnterWorldResp))]
private static void OnRecvEnterWorldMessage(BattleScene self, Proto.EnterWorldResp message)
{ /* do sth */ }
```

#### 2. 自定义事件数据分发

您可以通过特性注入的方式，指定某个接口函数对事件码（`EventID`）或事件类型（`EventType`）进行监听，当产生对应的事件数据时，该接口函数将自动接收到此数据。

```csharp
// 对`1101`和`1102`两个事件码进行监听
[GameEngine.EventSubscribeBindingOfTarget(1101)]
[GameEngine.EventSubscribeBindingOfTarget(1102)]
private static void OnRecvEventByCode(this MainBuildComponent self, int eventID, params object[] args)
{ /* do sth */ }

...

// 对`MessageErrorNotify`事件类型进行监听
[GameEngine.EventSubscribeBindingOfTarget(typeof(MainSimpleNotifyData))]
private static void OnRecvEventByType(this MainBuildComponent self, MainSimpleNotifyData eventData)
{ /* do sth */ }
```

同时，您也可以使用全局分发的形式，让指定类型的`CBean`对象实例接收到该事件数据。

```csharp
// 对`1101`和`1102`两个事件码进行监听
[GameEngine.OnEventDispatchCall(typeof(MainBuildComponent), 3101)]
[GameEngine.OnEventDispatchCall(typeof(MainBuildComponent), 3102)]
private static void OnRecvEventByCode(MainBuildComponent self, int eventID, params object[] args)
{ /* do sth */ }

...

// 对`MessageErrorNotify`事件类型进行监听
[GameEngine.OnEventDispatchCall(typeof(MainBuildComponent), typeof(MainSimpleNotifyData))]
private static void OnRecvEventByType(MainBuildComponent self, MainSimpleNotifyData eventData)
{ /* do sth */ }
```

#### 3. 输入编码数据分发

您可以通过特性注入的方式，指定某个接口函数对输入码（`InputCode`）或输入类型（`InputType`）进行监听，当产生对应的输入数据时，该接口函数将自动接收到此数据。

```csharp
// 对`Alpha1`和`Alpha2`两个按键码进行监听
[GameEngine.InputResponseBindingOfTarget((int) UnityEngine.KeyCode.Alpha1, GameEngine.InputOperationType.Released)]
[GameEngine.InputResponseBindingOfTarget((int) UnityEngine.KeyCode.Alpha2, GameEngine.InputOperationType.Released)]
private static void OnInputOperationForNormalFunctionCall(this MainScene self)
{ /* do sth */ }
```

同时，您也可以使用全局分发的形式，让指定类型的`CBean`对象实例接收到该输入数据。

```csharp
// 对`Alpha1`和`Alpha2`两个按键码进行监听
[GameEngine.OnInputDispatchCall((int) UnityEngine.KeyCode.Alpha1, GameEngine.InputOperationType.Released)]
[GameEngine.OnInputDispatchCall((int) UnityEngine.KeyCode.Alpha2, GameEngine.InputOperationType.Released)]
private static void OnBattleSceneInputed(int keycode, int operationType)
{ /* do sth */ }
```

## 生命周期行为管理

`CBean`对象的生命周期总共包含以下阶段：

- Initialize：初始化通知回调函数
- Startup：启动通知回调函数
- Awake：唤醒通知回调函数
- Start：开始通知回调函数
- Execute：逻辑调度通知回调函数
- LateExecute：逻辑后置调度通知回调函数
- Update：动画调度通知回调函数
- LateUpdate：动画后置调度通知回调函数
- Destroy：销毁通知回调函数
- Shutdown：关闭通知回调函数
- Cleanup：清理通知回调函数

在以上的整个生命周期调度流程中，`Initialize`和`Cleanup`两个阶段，主要服务于引擎内，
通常是对实体对象的基础属性成员和管理容器进行初始化和清理操作，以及对由引擎调度的回调句柄进行注册与注销操作。
`Startup`和`Shutdown`两个阶段，主要服务于模组内，用于在业务逻辑执行前，进行一些服务组件的绑定与解绑操作。
而`Awake`、`Start`和`Destroy`这三个阶段，是提供给业务层使用的，
您可以在这里进行具体的业务逻辑的初始化和清理，生命周期的执行流程保证了业务逻辑调度前，
所有的底层接口及服务组件都已经被正确初始化，并且已经进入到工作流程中。


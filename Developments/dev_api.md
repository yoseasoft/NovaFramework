# `NovaFramework`核心`API`手册

> 本文档列出`NovaFramework`提供的所有可直接调用的`API`，方便业务开发时复用。  

## 1. 实体对象

### 1.1 实体对象创建/销毁函数

基于`CBean`实现的实体对象类型，不能通过`new`关键字创建及`GC`自动垃圾回收，只能通过框架提供的`API`函数创建和销毁。
实体对象包括场景对象、角色对象、视图对象、组件对象和常规对象几种类型，分别提供对应的`API`进行相应类型对象实例的创建与销毁。  

#### 1.1.1 场景对象

首先声明一个名称为`Login`的场景对象：
```csharp
[CSceneClass("Login")]
public class LoginScene : GameEngine.CScene { ... }
```

由于同一时间仅允许存在一个场景对象，所以不提供主动创建或销毁场景对象实例的接口，只提供切换场景的方式。  
我们可以通过指定类型进行当前的场景切换操作：
```csharp
GameEngine.GameApi.ReplaceScene<LoginScene>();
```

也可以通过指定名称进行当前的场景切换操作：
```csharp
GameEngine.GameApi.ReplaceScene("Login");
```

上述接口是延迟切换场景，它会确保在所有实体对象被刷新前执行切换操作，若当前实体对象正在刷新，则切换操作会在下一帧开始处执行。  
你也可以选择立即切换场景，它将在调用函数后被立即执行，但这是不安全的，我们不推荐这样做。  

同样，我们可以通过指定类型来立即切换当前场景：
```csharp
LoginScene scene = GameEngine.GameApi.ChangeScene<LoginScene>();
```

也可以通过指定名称来立即切换当前场景：
```csharp
LoginScene scene = GameEngine.GameApi.ChangeScene("Login");
```

由于场景对象在运行时，会有且仅有一个实例，所以对于场景对象实例不需要手动销毁，
在我们切换到下一个新场景时，会自动将当前场景销毁。

#### 1.1.2 角色对象

首先声明一个名称为`LocalPlayer`的角色对象：
```csharp
[CActorClass("LocalPlayer")]
public class Player : GameEngine.CActor { ... }
```

现在，我们可以通过指定类型创建对应的角色对象实例：
```csharp
Player player = GameEngine.GameApi.CreateActor<Player>();
```

也可以通过指定名称创建对应的角色对象实例，不过此接口返回的是`CActor`类型，需要进行类型转换：
```csharp
Player player = GameEngine.GameApi.CreateActor("LocalPlayer") as Player;
```

最后，当角色对象不再使用时，需要手动销毁角色对象实例：
```csharp
GameEngine.GameApi.DestroyActor(player);
```

#### 1.1.3 视图对象

首先需要一个名称为`LoginPanel`的UI资源（可以是`FairyGUI`或`UGUI`类型），再根据该资源名称声明一个对应的视图对象：
```csharp
[CViewClass("GameLoginPanel")]
public class LoginPanel : GameEngine.CView { ... }
```

现在，我们可以通过指定类型创建对应的视图对象实例：
```csharp
GameEngine.GameApi.OpenUI<LoginPanel>();
```

也可以通过指定名称创建对应的视图对象实例：
```csharp
GameEngine.GameApi.OpenUI("GameLoginPanel");
```

视图对象是在UI资源加载完成后才启动（进入`Startup`生命周期节点）的，所以在正常创建流程中我们无法直接获取视图对象实例。  
但是框架提供的异步创建接口来解决这个问题，虽然通常我们不推荐这么使用它。  

以异步方式通过指定类型创建视图对象实例：
```csharp
LoginPanel panel = await GameEngine.GameApi.AsyncOpenUI<LoginPanel>();
```

也可以通过指定名称来异步创建视图对象实例，不过此接口返回的是`CView`类型，需要进行类型转换：
```csharp
LoginPanel panel = await GameEngine.GameApi.AsyncOpenUI("GameLoginPanel") as LoginPanel;
```

最后，当视图对象不再使用时，需要手动销毁视图对象实例：
```csharp
GameEngine.GameApi.CloseUI(panel);
```

#### 1.1.4 常规对象

常规对象在实际业务中应用较少，一般针对业务流程单一，且严格依赖生命周期流程控制，
同时与常规角色对象存在明显差异，无需组件服务时才会进行使用。  

在此，可以定义一个名称为`MonthlyCardActivity`的常规对象：
```csharp
[CObjectClass("MonthlyCardActivity")]
public class MonthlyCardActivityObject : GameEngine.CObject { ... }
```

现在，我们可以通过指定类型创建对应的常规对象实例：
```csharp
MonthlyCardActivityObject obj = GameEngine.GameApi.CreateObject<MonthlyCardActivityObject>();
```

也可以通过指定名称创建对应的常规对象实例，不过此接口返回的是`CObject`类型，需要进行类型转换：
```csharp
MonthlyCardActivityObject obj = GameEngine.GameApi.CreateObject("MonthlyCardActivity") as MonthlyCardActivityObject;
```

最后，当常规对象不再使用时，需要手动销毁常规对象实例：
```csharp
GameEngine.GameApi.DestroyObject(obj);
```

#### 1.1.5 组件对象

通常情况下，我们不能直接创建组件对象实例，它必须依附于某个实体对象实例而存在，
所以我们通常使用实体对象提供的`API`来管理组件对象实例。  

首先声明一个名称为`AttributeComponent`的组件对象：
```csharp
[CComponentClass("AttributeComp")]
public class AttributeComponent : GameEngine.CComponent { ... }
```

现在，我们可以使用之前创建的角色对象实例，通过指定类型创建对应的组件对象实例：
```csharp
player.AddComponent<AttributeComponent>();
```

也可以通过指定名称创建对应的组件对象实例：
```csharp
player.AddComponent("AttributeComp");
```

最后，当组件对象不再使用时，需要手动从角色对象实例中销毁组件对象实例，可以指定组件类型进行销毁：
```csharp
player.RemoveComponent<AttributeComponent>();
```

也可以指定组件名称进行销毁：
```csharp
player.RemoveComponent("AttributeComp");
```

### 1.2 实体对象查询函数

#### 1.2.1 场景对象

获取当前场景对象实例，此接口返回的是`CScene`类型：
```csharp
LoginScene scene = GameEngine.GameApi.GetCurrentScene() as LoginScene;
```

也可以通过指定类型来获取当前场景对象实例，若当前场景与指定类型不一致，则返回`null`：
```csharp
LoginScene scene = GameEngine.GameApi.GetCurrentScene<LoginScene>();
```

由于场景对象在运行时，有且仅有一个实例的特殊情况，所以对于场景对象实例提供了专用的组件访问接口。  
我们可以通过指定组件类型来获取当前场景的组件对象实例：
```csharp
AttributeComponent comp = GameEngine.GameApi.GetCurrentSceneComponent<AttributeComponent>();
```

或者通过指定组件名称来获取当前场景的组件对象实例：
```csharp
AttributeComponent comp = GameEngine.GameApi.GetCurrentSceneComponent("AttributeComp") as AttributeComponent;
```

#### 1.2.2 角色对象

我们可以通过框架提供的`API`来获取当前已创建的所有角色对象实例：
```csharp
IReadOnlyList<CActor> actors = GameEngine.GameApi.GetAllActors();
```

也可以通过指定类型来获取当前已创建的角色对象实例：
```csharp
IReadOnlyList<Player> players = GameEngine.GameApi.GetActor<Player>();
```

或者通过指定名称来获取当前已创建的角色对象实例：
```csharp
IReadOnlyList<CActor> players = GameEngine.GameApi.GetActor("LocalPlayer");
```

#### 1.2.3 视图对象

因为框架限定了相同类型的视图对象只允许同时存在一个实例，所以我们可以通过指定的视图类型来获取对应的视图对象实例：
```csharp
LoginPanel panel = GameEngine.GameApi.FindUI<LoginPanel>();
```

或者通过指定的视图名称来获取对应的视图对象实例：
```csharp
LoginPanel panel = GameEngine.GameApi.FindUI("GameLoginPanel") as LoginPanel;
```

由于视图在创建过程中需要异步加载资源，所以上述接口返回的视图对象实例可能尚未启动完成，无法在业务层直接使用。
因此，我们也提供了一套异步查询接口，用于确保获取到的视图对象实例是完整可用的。  
与常规获取函数类似，通过指定的视图类型以异步的方式来获取对应的视图对象实例：
```csharp
LoginPanel panel = await GameEngine.GameApi.AsyncFindUI<LoginPanel>();
```

也可以通过指定的视图名称以异步的方式来获取对应的视图对象实例：
```csharp
LoginPanel panel = await GameEngine.GameApi.AsyncFindUI("GameLoginPanel") as LoginPanel;
```

#### 1.2.4 常规对象

我们可以通过框架提供的`API`来获取当前已创建的所有常规对象实例：
```csharp
IReadOnlyList<CObject> objects = GameEngine.GameApi.GetAllObjects();
```

也可以通过指定类型来获取当前已创建的常规对象实例：
```csharp
IReadOnlyList<MonthlyCardActivityObject> players = GameEngine.GameApi.GetObject<MonthlyCardActivityObject>();
```

或者通过指定名称来获取当前已创建的常规对象实例：
```csharp
IReadOnlyList<CObject> objects = GameEngine.GameApi.GetObject("MonthlyCardActivity");
```

#### 1.2.5 组件对象

组件对象实例通常依附于某个实体对象实例而存在，因此，我们需要通过实体对象实例才能获取到它内部加载的组件对象实例。  
我们可以通过框架提供的`API`来获取目标实体对象实例中所有组件对象实例：
```csharp
IReadOnlyList<CComponent> components = player.GetAllComponents();
```

也可以通过指定的组件类型来获取目标实体对象实例中对应的组件对象实例：
```csharp
AttributeComponent comp = player.GetComponent<AttributeComponent>();
```

或者通过指定的组件名称来获取目标实体对象实例中对应的组件对象实例：
```csharp
AttributeComponent comp = player.GetComponent("AttributeComp") as AttributeComponent;
```

### 1.3 基于配置管理的实体对象访问函数

暂无，待补充。

---

## 2 数据通知与转发

### 2.1 输入类型的数据通知

此类型的数据一般由引擎底层接收到输入操作后自动进行通知转发，业务层只需要关注接收操作即可。  
同时，框架也提供了模拟输入操作的接口函数，用于业务测试或某些特殊情况下使用：
```csharp
GameEngine.GameApi.OnInputSimulation(UnityEngine.KeyCode.A, GameEngine.InputOperationType.Released);
```

或者模拟结构类型的输入数据通知：
```csharp
GameEngine.GameApi.OnInputSimulation(new JoystickData() { x = 1f, y = 0.5f });
```

### 2.2 事件类型的数据通知

框架提供了基于事件类型的数据通知机制，用于实现不同业务模块之间的通讯。  
我们可以通过使用全局事件`API`接口函数来实现指定事件标识及参数的派发：
```csharp
GameEngine.GameApi.Send(1001, "hurley", 520, "ECMS");
```

同时也提供了基于结构类型的广播接口：
```csharp
GameEngine.GameApi.Send(new JoinMapNotify() { roleId = 1001, type = 1, bean = "goblin" });
```

这里的`Send`函数在调用后并不会立即进行事件的派发，而是推送到事件缓存容器中，在游戏下一帧的开始处进行统一派发。
如果业务逻辑需要在当前帧监听此事件的业务逻辑立即被触发，则需要使用`Fire`函数进行派发：
```csharp
GameEngine.GameApi.Fire(1001, "hurley", 520, "ECMS");
```

同样，对于基于结构类型的事件，也提供了基于实体对象实例的专有`API`进行派发：
```csharp
player.Fire(1001, "hurley", 520, "ECMS");
```

以上主要针对全局的事件通知，如果希望该事件只能被指定的实体对象实例所接收，也提供的专用的`API`来进行派发：
```csharp
player.Send(1001, "hurley", 520, "ECMS");
```

或者针对基于结构类型的事件进行派发：
```csharp
player.Send(new JoinMapNotify() { roleId = 1001, type = 1, bean = "goblin" });
```

如果当前的实体对象实例也需要立即接收并处理该事件，也同样提供了`Fire`函数进行派发：
```csharp
player.Fire(1001, "hurley", 520, "ECMS");
```

对于基于结构类型消息，也可以用如下方式进行派发：
```csharp
player.Fire(new JoinMapNotify() { roleId = 1001, type = 1, bean = "goblin" });
```

### 2.3 消息类型的数据通知

此类型的数据一般在成功进行网络连接后，由引擎底层接收到服务端下行数据后自动进行通知转发，业务层只需要关注接收操作即可。  
同时，框架也提供了模拟操作的接口函数，用于业务测试或某些特殊情况下使用：
```csharp
GameEngine.GameApi.OnMessageSimulation(new EnterWorldResp() { Code = 1, Name = "hurley" });
```

### 2.4 同步类型的数据通知

暂无，待补充。

---

## 3 资源访问

### 3.1 配置数据

如果外部已定义一个角色配置文件`actor`，并导出如下格式的数据类型：
| 字段名称 | 字段类型 | 字段描述 |
|:-------:|:--------:|:--------:|
| id | int | 角色唯一标识 |
| name | string | 角色名称 |
| type | int | 角色类型 |
| bean | string | 角色实体配置名称 |
| attribute_id | int | 角色属性引用标识 |
| model_id | int | 角色模型资源引用标识 |

则我们可以通过如下代码来获取该角色配置数据：
```csharp
Config.ActorConfig actorConfig = Config.ActorConfigTable.Get(actorId);
Console.WriteLine(actorConfig.name); // 输出标识为 actorId 的角色名称
```

### 3.2 上下文配置文件

暂无，待补充。

### 3.3 场景资源访问

我们可以通过框架提供的全局`API`来加载场景资源：

### 3.4 模型资源访问

### 3.5 视图资源访问

---


# `NovaFramework`核心`API`手册

> 本文档列出`NovaFramework`提供的所有可直接调用的`API`，方便业务开发时复用。  

## 1. 实体对象

### 1.1 实体对象创建/销毁函数

基于`CBean`实现的实体对象类型，不能通过`new`关键字创建及`GC`自动垃圾回收，只能通过框架提供的`API`函数创建和销毁。
实体对象包括场景对象、角色对象、视图对象、组件对象和常规对象几种类型，分别提供对应的`API`进行相应类型对象实例的创建与销毁。  

### 1.1.1 场景对象

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

### 1.1.2 角色对象

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

### 1.1.3 视图对象

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

### 1.1.4 常规对象

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

### 1.1.5 组件对象

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

### 1.2.1 场景对象

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

### 1.2.2 角色对象

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

### 1.2.3 视图对象

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

### 1.2.4 常规对象

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

### 1.2.5 组件对象

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

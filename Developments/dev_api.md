# `NovaFramework`核心`API`手册

> 本文档列出`NovaFramework`提供的所有可直接调用的`API`，方便业务开发时复用。  

## 1. 实体对象

### 1.1 实体对象创建/销毁函数

基于`CBean`实现的实体对象类型，不能通过`new`关键字创建及`GC`自动垃圾回收，只能通过框架提供的`API`函数创建和销毁。
实体对象包括场景对象、角色对象、视图对象、组件对象和普通对象几种类型，分别提供对应的`API`进行相应类型对象实例的创建与销毁。  

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

我们可以通过指定类型立即切换当前场景：
```csharp
LoginScene scene = GameEngine.GameApi.ChangeScene<LoginScene>();
```

也可以通过指定名称立即切换当前场景：
```csharp
LoginScene scene = GameEngine.GameApi.ChangeScene("Login");
```

最后，你可以通过以下方式获取当前场景对象实例：
```csharp
LoginScene scene = GameEngine.GameApi.GetCurrentScene() as LoginScene;
```

### 1.1.2 角色对象

首先声明一个名称为`LocalPlayer`的角色对象：
```csharp
[CActorClass("LocalPlayer")]
public class Player : GameEngine.CActor { ... }
```

我们可以通过指定类型创建对应的角色对象实例：
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

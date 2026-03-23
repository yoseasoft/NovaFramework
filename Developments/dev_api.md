# `NovaFramework` 核心 API 手册

> 本文档列出 `NovaFramework` 提供的所有可直接调用的 API，按功能分类。  
> 使用前请确保已阅读 `dev_spec.md` 中的框架规则。

---

## 1. 场景对象 API

### 1.1 场景切换

场景对象同一时间**只能存在一个实例**，不提供主动创建/销毁接口，只能通过切换方式操作。  
切换到新场景时，当前场景会**自动销毁**。

#### 延迟切换（推荐）

在所有实体对象刷新前执行切换，若正在刷新则延迟到下一帧开始处。

```csharp
// 通过类型切换
GameEngine.GameApi.ReplaceScene<LoginScene>();

// 通过名称切换
GameEngine.GameApi.ReplaceScene("Login");
```

#### 立即切换（不推荐，不安全）

```csharp
// 通过类型立即切换
LoginScene scene = GameEngine.GameApi.ChangeScene<LoginScene>();

// 通过名称立即切换
LoginScene scene = GameEngine.GameApi.ChangeScene("Login") as LoginScene;
```

### 1.2 场景查询

```csharp
// 获取当前场景（返回 CScene 基类，需类型转换）
LoginScene scene = GameEngine.GameApi.GetCurrentScene() as LoginScene;

// 通过类型获取当前场景（类型不匹配返回 null）
LoginScene scene = GameEngine.GameApi.GetCurrentScene<LoginScene>();
```

### 1.3 场景组件快捷访问

```csharp
// 通过组件类型获取
AttributeComponent comp = GameEngine.GameApi.GetCurrentSceneComponent<AttributeComponent>();

// 通过组件名称获取
AttributeComponent comp = GameEngine.GameApi.GetCurrentSceneComponent("AttributeComp") as AttributeComponent;
```

---

## 2. 角色对象 API

### 2.1 创建

```csharp
Player player = GameEngine.GameApi.CreateActor<Player>();
Player player = GameEngine.GameApi.CreateActor("LocalPlayer") as Player;
```

### 2.2 销毁

```csharp
GameEngine.GameApi.DestroyActor(player);
```

### 2.3 查询

```csharp
IReadOnlyList<CActor> actors = GameEngine.GameApi.GetAllActors();
IReadOnlyList<Player> players = GameEngine.GameApi.GetActor<Player>();
IReadOnlyList<CActor> players = GameEngine.GameApi.GetActor("LocalPlayer");
```

---

## 3. 视图对象 API

同类型视图**只允许同时存在一个实例**。视图对象需要异步加载 UI 资源。

### 3.1 创建（同步，无法获取实例）

```csharp
GameEngine.GameApi.OpenUI<LoginPanel>();
GameEngine.GameApi.OpenUI("GameLoginPanel");
```

### 3.2 创建（异步，可获取实例）

```csharp
LoginPanel panel = await GameEngine.GameApi.AsyncOpenUI<LoginPanel>();
LoginPanel panel = await GameEngine.GameApi.AsyncOpenUI("GameLoginPanel") as LoginPanel;
```

### 3.3 销毁

```csharp
GameEngine.GameApi.CloseUI(panel);
```

### 3.4 查询（同步）

> 注意：返回的视图对象实例可能尚未启动完成，无法在业务层直接使用。

```csharp
LoginPanel panel = GameEngine.GameApi.FindUI<LoginPanel>();
LoginPanel panel = GameEngine.GameApi.FindUI("GameLoginPanel") as LoginPanel;
```

### 3.5 查询（异步，确保可用）

```csharp
LoginPanel panel = await GameEngine.GameApi.AsyncFindUI<LoginPanel>();
LoginPanel panel = await GameEngine.GameApi.AsyncFindUI("GameLoginPanel") as LoginPanel;
```

---

## 4. 通用对象 API

通用对象适用于业务流程单一、需要生命周期管理、与角色对象有明显差异、且无需组件服务的场景。

### 4.1 创建

```csharp
MonthlyCardActivityObject obj = GameEngine.GameApi.CreateObject<MonthlyCardActivityObject>();
MonthlyCardActivityObject obj = GameEngine.GameApi.CreateObject("MonthlyCardActivity") as MonthlyCardActivityObject;
```

### 4.2 销毁

```csharp
GameEngine.GameApi.DestroyObject(obj);
```

### 4.3 查询

```csharp
IReadOnlyList<CObject> objects = GameEngine.GameApi.GetAllObjects();
IReadOnlyList<MonthlyCardActivityObject> objs = GameEngine.GameApi.GetObject<MonthlyCardActivityObject>();
IReadOnlyList<CObject> objs = GameEngine.GameApi.GetObject("MonthlyCardActivity");
```

---

## 5. 组件对象 API

组件对象**必须依附于某个实体对象实例**，不能直接创建，通过实体对象的实例方法管理。

### 5.1 添加组件

```csharp
player.AddComponent<AttributeComponent>();
player.AddComponent("AttributeComp");
```

### 5.2 移除组件

```csharp
player.RemoveComponent<AttributeComponent>();
player.RemoveComponent("AttributeComp");
```

### 5.3 查询组件

```csharp
IReadOnlyList<CComponent> components = player.GetAllComponents();
AttributeComponent comp = player.GetComponent<AttributeComponent>();
AttributeComponent comp = player.GetComponent("AttributeComp") as AttributeComponent;
```

### 5.4 基于配置管理的实体对象访问函数

暂无，待补充。

---

## 6. 数据通知与转发 API

### 6.1 输入数据通知

输入数据一般由引擎底层自动通知转发，业务层只需关注接收。  
接收方式详见 `dev_spec.md` 中"输入通知"章节。

模拟输入操作（用于测试或特殊场景）：

```csharp
// 模拟按键输入
GameEngine.GameApi.OnInputSimulation(UnityEngine.KeyCode.A, GameEngine.InputOperationType.Released);

// 模拟结构类型的输入数据
GameEngine.GameApi.OnInputSimulation(new JoystickData() { x = 1f, y = 0.5f });
```

### 6.2 事件数据通知

事件是实现不同业务模组间通讯的核心机制。  
接收方式详见 `dev_spec.md` 中"事件通知"章节。

#### 6.2.1 全局事件派发

**延迟派发（`Send`）**——推送到缓存，下一帧开始处统一派发：

```csharp
GameEngine.GameApi.Send(1001, "hurley", 520, "ECMS");
GameEngine.GameApi.Send(new JoinMapNotify() { roleId = 1001, type = 1, bean = "goblin" });
```

**立即派发（`Fire`）**——当前帧立即触发所有监听逻辑：

```csharp
GameEngine.GameApi.Fire(1001, "hurley", 520, "ECMS");
GameEngine.GameApi.Fire(new JoinMapNotify() { roleId = 1001, type = 1, bean = "goblin" });
```

#### 6.2.2 实体对象专有事件派发

将事件范围限定为**指定的实体对象实例**及其组件：

```csharp
// 延迟派发
player.Send(1001, "hurley", 520, "ECMS");
player.Send(new JoinMapNotify() { roleId = 1001, type = 1, bean = "goblin" });

// 立即派发
player.Fire(1001, "hurley", 520, "ECMS");
player.Fire(new JoinMapNotify() { roleId = 1001, type = 1, bean = "goblin" });
```

#### 6.2.3 Send 与 Fire 的区别

| 方法 | 派发时机 | 适用场景 |
|-----|---------|---------|
| `Send` | 下一帧开始处统一派发 | 常规业务通知，无需即时响应 |
| `Fire` | 当前帧立即派发 | 需要监听方立即响应的场景 |

| 调用方式 | 接收范围 |
|---------|---------|
| `GameApi.Send(...)` / `GameApi.Fire(...)` | 全局，所有绑定了该事件的监听函数 |
| `entity.Send(...)` / `entity.Fire(...)` | 仅该实体对象实例及其组件 |
| `component.Send(...)` / `component.Fire(...)` | 仅限该组件对象实例所属的实体对象实例及其组件 |

### 6.3 消息数据通知

消息数据由引擎底层接收服务端下行数据后自动转发，业务层只需关注接收。  
接收方式详见 `dev_spec.md` 中"消息通知"章节。

模拟消息通知（用于测试）：

```csharp
GameEngine.GameApi.OnMessageSimulation(new EnterWorldResp() { Code = 1, Name = "hurley" });
```

### 6.4 同步数据通知

暂无，待补充。

---

## 7. 资源访问 API

### 7.1 配置数据

配置文件由外部定义并自动导出对应的数据类型。  
例如角色配置文件 `actor`，导出字段如下：

| 字段名称 | 字段类型 | 字段描述 |
|:-------:|:-------:|:-------:|
| id | int | 角色唯一标识 |
| name | string | 角色名称 |
| type | int | 角色类型 |
| bean | string | 角色实体配置名称 |
| attribute_id | int | 角色属性引用标识 |
| model_id | int | 角色模型资源引用标识 |

访问方式：
```csharp
Config.ActorConfig actorConfig = Config.ActorConfigTable.Get(actorId);
Console.WriteLine(actorConfig.name);
```

> 配置数据类命名规则：`Config.<配置文件名>Config`，表对象命名：`Config.<配置文件名>ConfigTable`。

### 7.2 上下文配置文件

暂无，待补充。

### 7.3 通用类型资源访问

同步加载通用资源：
```csharp
UnityEngine.Object obj = GameEngine.GameApi.LoadAsset("Assets/_Resources/Gui/LoginPanel_fgui.bytes", typeof(UnityEngine.TextAsset));
```

异步加载通用资源：
```csharp
UnityEngine.TextAsset textAsset = await GameEngine.GameApi.AsyncLoadAsset<UnityEngine.TextAsset>("Assets/_Resources/Gui/LoginPanel_fgui.bytes");
UnityEngine.GameObject go = await GameEngine.GameApi.AsyncLoadAsset<UnityEngine.GameObject>("Assets/Gui/LoginPanel/Main.prefab");
```

手动释放资源（**主动加载的资源必须手动释放**）：
```csharp
GameEngine.GameApi.UnloadAsset(textAsset);
GameEngine.GameApi.UnloadAsset(go);
```

### 7.4 场景类型资源访问

加载场景资源：
```csharp
GooAsset.Scene asset = GameEngine.GameApi.LoadAssetScene("101", "Assets/_Resources/Scene/101.unity");
```

释放场景资源（**必须手动释放**）：
```csharp
GameEngine.GameApi.UnloadAssetScene(asset);
```

### 7.5 原始文件类型资源访问

同步加载原始文件资源：
```csharp
GooAsset.RawFile rawFile = GameEngine.GameApi.LoadRawFile("C:/Users/Public/Downloads/History.log");
```

异步加载原始文件资源：
```csharp
GooAsset.RawFile rawFile = await GameEngine.GameApi.AsyncLoadRawFile("C:/Users/Public/Downloads/History.log");
```

---

## 8. API 速查表

### 8.1 场景对象

| 操作 | API | 返回类型 |
|-----|-----|---------|
| 延迟切换 | `GameApi.ReplaceScene<T>()` / `(name)` | `void` |
| 立即切换 | `GameApi.ChangeScene<T>()` / `(name)` | `T` / `CScene` |
| 获取当前 | `GameApi.GetCurrentScene<T>()` / `()` | `T?` / `CScene` |
| 获取场景组件 | `GameApi.GetCurrentSceneComponent<T>()` / `(name)` | `T` / `CComponent` |

### 8.2 角色对象

| 操作 | API | 返回类型 |
|-----|-----|---------|
| 创建 | `GameApi.CreateActor<T>()` / `(name)` | `T` / `CActor` |
| 销毁 | `GameApi.DestroyActor(actor)` | `void` |
| 查询全部 | `GameApi.GetAllActors()` | `IReadOnlyList<CActor>` |
| 按类型查询 | `GameApi.GetActor<T>()` | `IReadOnlyList<T>` |
| 按名称查询 | `GameApi.GetActor(name)` | `IReadOnlyList<CActor>` |

### 8.3 视图对象

| 操作 | API | 返回类型 |
|-----|-----|---------|
| 创建 | `GameApi.OpenUI<T>()` / `(name)` | `void` |
| 异步创建 | `GameApi.AsyncOpenUI<T>()` / `(name)` | `Task<T>` / `Task<CView>` |
| 销毁 | `GameApi.CloseUI(view)` | `void` |
| 查询 | `GameApi.FindUI<T>()` / `(name)` | `T?` / `CView?` |
| 异步查询 | `GameApi.AsyncFindUI<T>()` / `(name)` | `Task<T>` / `Task<CView>` |

### 8.4 通用对象

| 操作 | API | 返回类型 |
|-----|-----|---------|
| 创建 | `GameApi.CreateObject<T>()` / `(name)` | `T` / `CObject` |
| 销毁 | `GameApi.DestroyObject(obj)` | `void` |
| 查询全部 | `GameApi.GetAllObjects()` | `IReadOnlyList<CObject>` |
| 按类型查询 | `GameApi.GetObject<T>()` | `IReadOnlyList<T>` |
| 按名称查询 | `GameApi.GetObject(name)` | `IReadOnlyList<CObject>` |

### 8.5 组件对象（实体实例方法）

| 操作 | API | 返回类型 |
|-----|-----|---------|
| 添加 | `entity.AddComponent<T>()` / `(name)` | `void` |
| 移除 | `entity.RemoveComponent<T>()` / `(name)` | `void` |
| 查询全部 | `entity.GetAllComponents()` | `IReadOnlyList<CComponent>` |
| 按类型查询 | `entity.GetComponent<T>()` | `T` |
| 按名称查询 | `entity.GetComponent(name)` | `CComponent` |

### 8.6 数据通知与转发

| 操作 | API | 派发时机 | 接收范围 |
|-----|-----|---------|---------|
| 全局延迟派发 | `GameApi.Send(id, args...)` / `GameApi.Send(struct)` | 下一帧 | 全局 |
| 全局立即派发 | `GameApi.Fire(id, args...)` / `GameApi.Fire(struct)` | 当前帧 | 全局 |
| 实体延迟派发 | `entity.Send(id, args...)` / `entity.Send(struct)` | 下一帧 | 该实体及其组件 |
| 实体立即派发 | `entity.Fire(id, args...)` / `entity.Fire(struct)` | 当前帧 | 该实体及其组件 |
| 模拟输入 | `GameApi.OnInputSimulation(keyCode, opType)` / `(struct)` | 当前帧 | 全局 |
| 模拟消息 | `GameApi.OnMessageSimulation(messageObj)` | 当前帧 | 全局 |

### 8.7 资源访问

| 操作 | API | 说明 |
|-----|-----|------|
| 获取配置数据 | `Config.<文件名>ConfigTable.Get(id)` | 返回 `Config.<文件名>Config` |
| 同步加载资源 | `GameApi.LoadAsset(path, type)` | 返回 `UnityEngine.Object` |
| 异步加载资源 | `GameApi.AsyncLoadAsset<T>(path)` | 返回 `Task<T>` |
| 释放资源 | `GameApi.UnloadAsset(obj)` | 主动加载的必须手动释放 |
| 加载场景资源 | `GameApi.LoadAssetScene(name, path)` | 返回 `GooAsset.Scene` |
| 释放场景资源 | `GameApi.UnloadAssetScene(asset)` | 必须手动释放 |

---

## 相关文档

- **开发规范**：`dev_spec.md` — 框架规则、架构约束、命名规范
- **开发示例集**：`dev_examples.md` — 完整业务开发示例、代码模板和反模式

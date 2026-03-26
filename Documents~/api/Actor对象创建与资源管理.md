# Actor 对象创建与资源管理指南

## 一、概述
`Actor`是游戏框架中的重要概念，它继承自`CActor`，而`CActor`又继承自`CEntity`。本指南将详细介绍如何创建Actor对象并有效管理其资源加载和卸载。

`CEntity` 是框架中所有实体对象的抽象基类，定义了资源加载、实例化、卸载等核心管理能力。其子类包括：

●`CActor`（角色基类，继承自 `CEntity`）

●`CView`（视图基类，继承自 `CEntity`）

●`CScene`（场景基类，继承自 `CEntity`）


---

## 二、创建Actor对象
Actor 作为 CEntity 的子类，其创建/销毁流程遵循 CEntity 的生命周期规范（如实体初始化时自动创建 AssetLoader，销毁时自动清理资源）。

### 2.1 通过名称创建Actor
```csharp
//创建Actor对象
Actor playerActor = GameApi.CreateActor("Player") as Actor;
//销毁Actor对象
GameApi.DestroyActor("Player");
```
**适用场景**：运行时根据配置动态创建不同类型的Actor

---

### 2.2 通过泛型创建Actor
```csharp
//创建Actor对象
Actor playerActor = GameApi.CreateActor<Player>();
//销毁Actor对象
GameApi.DestroyActor<Player>();
```
**优势**：类型安全且性能最佳

---

### 2.3 通过Type创建Actor
```csharp
//创建Actor对象
Actor playerActor = GameApi.CreateActor(typeof(Player)) as Actor;
//销毁Actor对象
GameAPi.DestroyActor(typeof(Player));
```
**适用场景**：运行时动态确定类型的场景

---

### 2.4通过Bean配置创建对象
![文件 text](../images/bean_image.png)
```csharp
//创建Actor对象
Actor game_actor = GameEngine.ApplicationContext.CreateBean("game_actor") as Actor;
//销毁Actor对象
GameEngine.ApplicationContext.ReleaseBean(game_actor);
```

---

## 三、销毁Actor对象（触发 CEntity 基类清理逻辑）
Actor 销毁时，会调用 CEntity 基类的 Cleanup()方法，自动清理 _assetLoader资源（如卸载所有加载的资源）。

| 方法                  | 立即销毁 | 延迟销毁 | 适用场景               |
|-----------------------|--------|--------|----------------------|
| `DestroyActor(playerActor)` | 是     | 否     | 立即回收资源          |
| `DestroyActor("Player")` | 是     | 否     | 通过名称销毁          |
| `DestroyActor<Player>()` | 是     | 否     | 通过泛型销毁          |
| `DestroyActor(typeof(Player))` | 是     | 否     | 通过Type销毁          |

---


## 四、同步实例化方法（Instantiate）
以下方法定义在 CEntity 基类，Actor 作为子类直接继承并可使用，CView/CScene/CActor 等其他子类调用方式完全一致。

### 1.1 重载1：指定位置和旋转（最常用）
**功能**：在指定世界坐标位置和旋转角度实例化资源对象  
**语法**（CEntity 基类定义）：
```csharp
public T Instantiate<T>(
    string name, 
    string url, 
    Vector3 position, 
    Quaternion rotation
) where T : UnityEngine.Object
```

**参数说明**（所有子类通用）:

| 参数名      | 类型          | 描述                          |
|-------------|---------------|-------------------------------|
| name        | string        | 资源名称（唯一标识）          |
| url         | string        | 资源路径（Prefab路径）        |
| position    | Vector3       | 实例化位置（世界坐标）        |
| rotation    | Quaternion    | 实例化旋转（四元数）          |

**案例代码**（Actor 实例调用，其他子类如 CView/CScene 同理）：
```csharp
// 场景：在坐标 (2, 0, 30) 处实例化玩家模型
void SyncInstantiate_PositionRotation()  
{
    // 1. 创建 Actor 实例（继承自 CEntity）
    Actor playerActor = GameApi.CreateActor<Actor>();  
    
    // 2. 调用 CEntity 基类的同步实例化方法（指定位置和旋转）
    GameObject playerModel = playerActor.Instantiate<GameObject>(
        name: "playerModel",  // 资源名称（唯一标识）
        url: "Assets/_Resources/Model/jianshi/prefab/001.prefab",  // 资源路径
        position: new Vector3(2, 0, 30f),  // 世界坐标位置
        rotation: Quaternion.identity  // 无旋转
    );

    Debug.Log($"同步实例化成功：{playerModel?.name}");
}
```

---

### 1.2 重载2：指定位置、旋转和父对象
**功能**：将实例化对象挂载到父对象下  
**语法**（CEntity 基类定义）：
```csharp
public T Instantiate<T>(
    string name, 
    string url, 
    Vector3 position, 
    Quaternion rotation, 
    Transform parent
) where T : UnityEngine.Object
```

**案例代码**（Actor 实例调用，其他子类如 CView/CScene 同理）：
```csharp
// 场景：在玩家脚下实例化武器模型（挂载到玩家根节点）
void SyncInstantiate_WithParent()  
{
    Actor playerActor = GameApi.CreateActor<Actor>();
    Transform playerRoot = playerActor.transform;  // 父对象为 Actor 自身 transform
    
    // 调用 CEntity 基类的同步实例化方法（指定位置、旋转和父对象）
    GameObject weapon = playerActor.Instantiate<GameObject>(
        name: "weapon_001",
        url: "Assets/_Resources/Weapon/Sword.prefab",
        position: new Vector3(0, 1, 0),  // 相对于父对象的位置偏移（朝上）
        rotation: Quaternion.Euler(0, 90, 0),  // 旋转 90 度（朝右）
        parent: playerRoot  // 父对象（Actor 根节点）
    );

}
```

---

### 1.3 重载3：仅指定父对象
**功能**：使用资源默认位置（Vector3.zero）和旋转（Quaternion.identity），仅挂载到父对象
**语法**（CEntity 基类定义）：
```csharp
public T Instantiate<T>(
    string name, 
    string url, 
    Transform parent
) where T : UnityEngine.Object
```

**案例代码**（Actor 实例调用，其他子类如 CView/CScene 同理）：
```csharp
// 场景：创建角色 Actor 并在其身上实例化武器装备模型（挂载到角色骨骼节点）
void SyncInstantiate_DefaultTransform()  
{
    // 1. 创建角色 Actor 实例（继承自 CEntity）
    Actor characterActor = GameApi.CreateActor<Actor>();
    
    // 2. 获取角色的右手骨骼节点作为父对象（模拟装备挂载点）
    Transform rightHandBone = characterActor.transform.Find("Skeleton/Right_Hand");

    // 3. 调用 CEntity 基类的同步实例化方法（仅指定父对象，使用默认位置旋转）
    GameObject swordModel = characterActor.Instantiate<GameObject>(
        name: "equipment_sword_001",           // 资源名称（唯一标识）
        url: "Assets/_Resources/Equipment/Sword_Great.prefab",  // 武器模型路径
        parent: rightHandBone                   // 父对象为右手骨骼节点
    );  
}
```

---

## 五、异步实例化方法（InstantiateAsync）
**特点**：通过UniTask实现非阻塞加载，适合大资源

### 2.1 重载1：指定位置和旋转
```csharp
public async UniTask<T> InstantiateAsync<T>(
    string name, 
    string url, 
    Vector3 position, 
    Quaternion rotation
) where T : UnityEngine.Object
```

**案例代码**：
```csharp
async void AsyncInstantiate_PositionRotation()  
{
    GameObject castle = await sceneActor.InstantiateAsync<GameObject>(
        name: "castle_big",
        url: "Assets/_Resources/Map/Castle.prefab",
        position: new Vector3(100, 0, 200),
        rotation: Quaternion.identity
    );
}
```

---

### 2.2 重载2：指定位置、旋转和父对象
```csharp
public async UniTask<T> InstantiateAsync<T>(
    string name, 
    string url, 
    Vector3 position, 
    Quaternion rotation, 
    Transform parent
) where T : UnityEngine.Object
```

**案例代码**：
```csharp
async void AsyncInstantiate_WithParent()  
{
    GameObject enemy = await enemyManager.InstantiateAsync<GameObject>(
        name: "goblin_001",
        url: "Assets/_Resources/Enemy/Goblin.prefab",
        position: new Vector3(-5, 0, 10),
        rotation: Quaternion.Euler(0, 180, 0),
        parent: enemiesParent
    );
}
```

---

### 2.3 重载3：仅指定父对象
**功能**：使用资源默认位置和旋转  
**语法**：
```csharp
public async UniTask<T> InstantiateAsync<T>(
    string name, 
    string url, 
    Transform parent
) where T : UnityEngine.Object
```

**案例代码**：
```csharp
// 场景：在UI面板下实例化货币栏
 async void SyncInstantiate_DefaultTransform()  
{
    Transform uiPanel = uiActor.transform.Find("MoneyPanel");
    
    GameObject moneyBar = await  uiActor.Instantiate<GameObject>(
        name: "momey_bar_prefab",
        url: "Assets/_Resources/UI/MoneyBar.prefab",
        parent: uiPanel
    );
}
```

---

## 六、完整综合案例
```csharp
async void FullCase_PlayerSetup()  
{
    // 1. 创建玩家 Actor
    Actor playerActor = GameApi.CreateActor<Actor>();
    
    try {
        // 2. 同步实例化主角模型
        GameObject playerModel = await playerActor.Instantiate<GameObject>(
            name: "player_main_model",
            url: "Assets/_Resources/Model/jianshi/prefab/001.prefab"
        );

        // 3. 异步加载武器
        GameObject sword = await playerActor.InstantiateAsync<GameObject>(
            name: "weapon_sword_001",
            url: "Assets/_Resources/Weapon/LongSword.prefab",
            parent: playerModel.transform
        );

        // 4. 资源管理
        playerActor.UnloadAsset("old_player_model_v1");
    }
    catch (Exception e) {
        Debug.LogError($"初始化失败：{e.Message}");
    }
}
```

---

## 七、注意事项
1. **资源名称唯一性**：同一Entity内名称必须唯一
2. **异步方法规范**：必须配合`await`使用
3. **对象销毁方式**：销毁对象后，底层框架会自动卸载资源
4. **路径校验**：确保资源路径存在且大小写匹配

---
                        



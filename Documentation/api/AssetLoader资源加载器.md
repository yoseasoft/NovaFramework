# AssetLoader 资源加载器

## 一、概述
`AssetLoader` 是 GameEngine 框架中的核心资源加载管理类，专门用于实体对象（`CEntity`）的资源加载、实例化和卸载管理。该类提供了一套完整的资源生命周期管理方案，确保在实体销毁时所有相关资源都能被正确释放。

---

## 二、类定义

### 构造函数
创建一个新的 `AssetLoader` 实例，绑定到指定的实体对象。

```csharp
public AssetLoader(CEntity entity)
```

| 参数名 | 类型     | 描述             |
|--------|----------|------------------|
| entity | `CEntity`| 关联的实体对象   |

---

### LoadAsset（同步加载对象资源）
```csharp
public AssetSource LoadAsset(string name, string url, Type type)
```

| 参数名 | 类型       | 描述                     |
|--------|------------|--------------------------|
| name   | `string`   | 资源名称（唯一标识）     |
| url    | `string`   | 资源路径                 |
| type   | `Type`     | 资源类型（`UnityEngine.Object` 子类）|

**返回值**：加载的 `AssetSource` 对象

---

### LoadAssetAsync（异步加载对象资源）
```csharp
public async UniTask<AssetSource> LoadAssetAsync<T>(string name, string url) where T : UnityEngine.Object
```

| 参数名 | 类型       | 描述                     |
|--------|------------|--------------------------|
| name   | `string`   | 资源名称（唯一标识）     |
| url    | `string`   | 资源路径                 |

**返回值**：异步加载的 `AssetSource` 对象（需通过 `await` 获取结果）

---

### UnloadAsset（释放已加载资源）
```csharp
public void UnloadAsset(string name)
```

| 参数名 | 类型       | 描述                     |
|--------|------------|--------------------------|
| name   | `string`   | 需要释放的资源名称       |

---

### Instantiate（同步实例化资源对象）
支持三种重载方式：

**重载1：指定位置和旋转**
```csharp
public T Instantiate<T>(string name, string url, Vector3 position, Quaternion rotation) where T : UnityEngine.Object
```

**重载2：指定位置、旋转和父对象**
```csharp
public T Instantiate<T>(string name, string url, Vector3 position, Quaternion rotation, Transform parent) where T : UnityEngine.Object
```

**重载3：指定父对象**
```csharp
public T Instantiate<T>(string name, string url, Transform parent) where T : UnityEngine.Object
```

| 参数名     | 类型           | 描述                     |
|------------|----------------|--------------------------|
| name       | `string`       | 资源名称                 |
| url        | `string`       | 资源路径                 |
| position   | `Vector3`      | 实例化位置（世界坐标）   |
| rotation   | `Quaternion`   | 实例化旋转（四元数）     |
| parent     | `Transform`    | 父对象变换组件（可选）   |

**返回值**：实例化的对象实例（`T` 类型）

---

### InstantiateAsync（异步实例化资源对象）
支持三种重载方式（参数与同步版本一致，需通过 `await` 获取结果）：

```csharp
public async UniTask<T> InstantiateAsync<T>(string name, string url, Vector3 position, Quaternion rotation) where T : UnityEngine.Object
```

---

### DestroyObject（销毁场景对象实例）
```csharp
public void DestroyObject(UnityEngine.Object obj)
```

| 参数名 | 类型               | 描述                     |
|--------|--------------------|--------------------------|
| obj    | `UnityEngine.Object` | 需要销毁的对象实例       |

---

### Clear（清理所有资源）
```csharp
public void Clear()
```

**说明**：释放所有已加载的资源，清理资产数据。

---

## 三、使用示例
```csharp
// 创建 AssetLoader 实例
AssetLoader assetLoader = new AssetLoader(entity);

// 同步加载资源
AssetSource assetSource = assetLoader.LoadAsset("hero_model", "Assets/Resources/Models/Hero.prafab", typeof(GameObject));

// 异步加载资源
AssetSource assetSource = await assetLoader.LoadAssetAsync<GameObject>("hero_model", "Assets/Resources/Models/Hero.prafab");

// 实例化对象（同步）
GameObject instance = assetLoader.Instantiate<GameObject>(
    "hero_model", 
    "Assets/Resources/Models/Hero.prafab", 
    Vector3.zero, 
    Quaternion.identity
);

// 卸载资源
assetLoader.UnloadAsset("hero_model");
assetLoader.Clear();
```

---

## 四、注意事项
1. **强关联性**：AssetLoader 与 `CEntity` 实体强关联，实体销毁时自动清理资源
2. **资源名称唯一性**：同一 AssetLoader 实例中资源名称必须唯一
3. **异步加载优势**：避免主线程阻塞，提升性能
4. **对象销毁方法**：使用 `DestroyObject` 而非 `UnityEngine.Object.Destroy`

## 五、最佳实践
- 🔹 **统一管理**：在 `CEntity` 子类中维护 AssetLoader 实例
- 🔹 **优先异步加载**：减少卡顿，提升流畅度
- 🔹 **及时释放资源**：通过 `UnloadAsset` 释放不再使用的资源
- 🔹 **命名规范**：使用有意义的资源名称（如 `enemy_effect_01`）



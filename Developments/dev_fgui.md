# `NovaFramework` FairyGUI 开发指南

> 本文档整合了 FairyGUI 相关的资源规则与操作 API，是在 `NovaFramework` 中进行 UI 开发的专项参考文档。  
> 使用前请确保已阅读 `dev_spec.md` 中的框架规则。

---

## 1. 视图开发与 FGUI 资源规则

项目使用 FairyGUI 作为 UI 框架。

### 1.1 FGUI 资源路径约定

每个面板的 UI 布局定义文件：

```text
Assets/RawResource/UI/FGUIProject/assets/<面板名>/Main.xml
```

其中 `<面板名>` 与视图对象的 `[CViewClass("xxx")]` 名称一致。

### 1.2 UI 元素命名前缀约定

| 前缀 | 元素类型 | 示例 |
|------|---------|------|
| `btn_` | 按钮 | `btn_start`、`btn_exit` |
| `txt_` | 文本 | `txt_title`、`txt_name` |
| `img_` | 图片 | `img_bg`、`img_icon` |
| `lst_` | 列表 | `lst_bag`、`lst_mail` |
| `prog_` | 进度条 | `prog_loading` |

> 所有元素必须使用对应前缀命名，以 `mapping.md` 中的实际名称为准。

### 1.3 UI 元素映射表

每个面板目录下维护一份 `mapping.md` 文件，由美术在制作 UI 时同步维护：

```text
Assets/RawResource/UI/FGUIProject/assets/<面板名>/mapping.md
```

文件格式：

```markdown
命名规范：按钮以 btn_ 开头，文本以 txt_ 开头，图片以 img_ 开头

| 中文名 | 元素名 |
|-------|--------|
| 开始按钮 | btn_start |
```

### 1.4 需求描述到 UI 元素的匹配流程

开发视图 System 逻辑时：

1. **读取 `mapping.md`**，通过需求中的中文描述直接查表获取元素名
2. 若 `mapping.md` 不存在或缺少所需元素，读取 `Main.xml` 并提示补充映射表
3. **禁止在未查阅映射表或 `Main.xml` 的情况下猜测 UI 元素名称**

---

## 2. 视图对象 FGUI 操作 API

项目使用 FairyGUI 作为 UI 框架。视图对象（`CView`）通过以下 API 与 FGUI 元素交互。

> 使用前需 `using FairyGUI;`。UI 元素名称必须通过面板的 `mapping.md` 映射表获取（详见第 1 节）。

### 2.1 获取 UI 元素

通过 `self.Window` 获取 FGUI 根组件，再用 `GComponent.GetChild()` 查找子元素：

> ⚠️ **禁止使用 `self.GetChild()`**，框架的 `CView.GetChild()` 未实现 FGUI 适配，调用会抛异常。

```csharp
// 获取根窗口组件（GComponent 类型）
GComponent root = self.Window as GComponent;

// 按名称获取子元素
GObject obj = root.GetChild("btn_start");

// 转换为具体类型
GButton btn = root.GetChild("btn_start") as GButton;
GTextField txt = root.GetChild("txt_title") as GTextField;
GList list = root.GetChild("list_mail") as GList;
GProgressBar bar = root.GetChild("schedule") as GProgressBar;
GComponent comp = root.GetChild("panel_info") as GComponent;

// 在 GComponent 上继续查找子元素
GObject child = comp.GetChild("sub_item");

// 按路径查找深层子元素（点号分隔）
GObject deep = root.GetChildByPath("panel_info.sub_item.icon");
```

### 2.2 事件绑定

所有 `GObject` 及其子类都支持事件监听：

```csharp
GComponent root = self.Window as GComponent;
GObject btn = root.GetChild("btn_start") as GObject;

// 添加点击事件（无参回调）
btn.onClick.Add(() =>
{
    UnityEngine.Debug.Log("按钮被点击");
});

// 添加点击事件（带事件上下文）
btn.onClick.Add((EventContext context) =>
{
    UnityEngine.Debug.Log("按钮被点击");
});

// 替换所有回调为单个回调
btn.onClick.Set(() => { ... });

// 移除指定回调（需保存委托引用）
EventCallback0 handler = () => { ... };
btn.onClick.Add(handler);
btn.onClick.Remove(handler);

// 清除所有回调
btn.onClick.Clear();
```

常用事件类型：

| 事件属性 | 触发时机 |
|---------|---------|
| `onClick` | 点击 |
| `onRightClick` | 右键点击 |
| `onTouchBegin` | 按下 |
| `onTouchEnd` | 抬起 |
| `onRollOver` | 鼠标/手指移入 |
| `onRollOut` | 鼠标/手指移出 |
| `onPositionChanged` | 位置变更 |
| `onSizeChanged` | 尺寸变更 |
| `onDragStart` | 开始拖拽 |
| `onDragMove` | 拖拽中 |
| `onDragEnd` | 拖拽结束 |
| `onKeyDown` | 按键（需焦点） |

### 2.3 按钮操作（GButton）

```csharp
GComponent root = self.Window as GComponent;
GButton btn = root.GetChild("btn_start") as GButton;

// 标题文本
btn.title = "开始游戏";
string title = btn.title;

// 图标
btn.icon = "ui://包名/图标名";

// 选中状态（仅 Check/Radio 模式有效）
btn.selected = true;
bool isSelected = btn.selected;

// 选中时的标题和图标
btn.selectedTitle = "已选中";
btn.selectedIcon = "ui://包名/选中图标";

// 选中状态变更事件
btn.onChanged.Add(() =>
{
    UnityEngine.Debug.Log($"选中状态：{btn.selected}");
});

// 模拟点击
btn.FireClick(true);
```

### 2.4 文本操作（GTextField）

```csharp
GTextField txt = self.GetChild("lab_title") as GTextField;

// 设置文本
txt.text = "幸运魔方";

// 文本颜色
txt.color = UnityEngine.Color.red;

// 对齐方式
txt.align = AlignType.Center;
txt.verticalAlign = VertAlignType.Middle;

// 模板变量（文本中用 {变量名} 占位）
txt.SetVar("name", "玩家1").SetVar("level", "10");
txt.FlushVars(); // 刷新显示
```

### 2.5 进度条操作（GProgressBar）

```csharp
GProgressBar bar = self.GetChild("schedule") as GProgressBar;

// 直接设置值
bar.max = 100;
bar.min = 0;
bar.value = 75;

// 缓动动画（从当前值过渡到目标值）
bar.TweenValue(100, 0.5f); // 0.5秒过渡到100
```

### 2.6 列表操作（GList）

```csharp
GList list = self.GetChild("list_mail") as GList;

// 虚拟列表模式（推荐，大量数据时使用）
list.itemRenderer = (int index, GObject item) =>
{
    // 根据 index 设置 item 的显示内容
    GButton itemBtn = item as GButton;
    itemBtn.title = $"第{index}项";
};
list.numItems = 100; // 设置数据总数，自动触发 itemRenderer

// 手动添加/移除子项
GObject item = list.AddItemFromPool();
list.RemoveChildToPool(item);
list.RemoveChildrenToPool(); // 清空列表

// 选中项
list.selectedIndex = 0;
int selected = list.selectedIndex;

// 列表项点击事件
list.onClickItem.Add((EventContext context) =>
{
    GObject clickedItem = context.data as GObject;
});

// 滚动控制
list.scrollPane.ScrollTop();
list.scrollPane.ScrollBottom();
```

### 2.7 通用属性（GObject）

```csharp
GObject obj = self.GetChild("btn_start") as GObject;

// 显示/隐藏
obj.visible = false;

// 启用/禁用（grayed + 不可触摸）
obj.enabled = false;

// 单独控制
obj.grayed = true;      // 置灰
obj.touchable = false;   // 不可触摸
obj.draggable = true;    // 可拖拽

// 位置和尺寸
obj.SetXY(100, 200);
obj.SetPosition(100, 200, 0);
obj.SetSize(300, 100);

// 文本（GObject 通用属性，按钮返回 title，文本返回 text）
obj.text = "通用文本设置";
```

### 2.8 控制器与动效（GComponent）

```csharp
GComponent comp = self.Window as GComponent;

// 控制器（用于切换 UI 状态，如页签、多态按钮）
Controller ctrl = comp.GetController("tab");
ctrl.selectedIndex = 1;           // 按索引切换
ctrl.selectedPage = "page_name";  // 按页面名切换

// 动效（FGUI 编辑器中定义的过渡动画）
Transition trans = comp.GetTransition("anim_show");
trans.Play();                     // 播放
trans.Play(2);                    // 播放2次
trans.PlayReverse();              // 反向播放
trans.Stop();                     // 停止
```

### 2.9 速查表

| 操作 | API | 说明 |
|-----|-----|------|
| 获取子元素 | `root.GetChild(name) as T` | 先通过 `self.Window as GComponent` 获取 root |
| 按路径获取 | `comp.GetChildByPath(path)` | 点号分隔路径 |
| 获取根窗口 | `self.Window as GComponent` | 视图的 FGUI 根组件 |
| 点击事件 | `obj.onClick.Add(callback)` | 支持 Add/Remove/Set/Clear |
| 按钮标题 | `btn.title = "文本"` | GButton 专用 |
| 设置文本 | `txt.text = "文本"` | GTextField 专用 |
| 进度条值 | `bar.value = 75` | GProgressBar 专用 |
| 进度条动画 | `bar.TweenValue(value, duration)` | 缓动过渡 |
| 列表渲染 | `list.itemRenderer = callback` | 虚拟列表回调 |
| 列表数量 | `list.numItems = count` | 触发列表刷新 |
| 显示隐藏 | `obj.visible = bool` | 所有 GObject |
| 启用禁用 | `obj.enabled = bool` | 置灰 + 不可触摸 |
| 控制器切换 | `ctrl.selectedIndex = n` | 状态/页签切换 |
| 播放动效 | `trans.Play()` | FGUI 过渡动画 |

---

## 相关文档

- **开发规范**：`dev_spec.md` — 框架规则、架构约束、命名规范
- **API 手册**：`dev_api.md` — 框架提供的所有可调用接口
- **开发示例集**：`dev_examples.md` — 完整业务开发示例、代码模板和反模式

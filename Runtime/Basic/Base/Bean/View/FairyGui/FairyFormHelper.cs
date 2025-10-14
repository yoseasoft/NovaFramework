/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// -------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

using SystemType = System.Type;
using UnityObject = UnityEngine.Object;
using UnityTime = UnityEngine.Time;
using UnityTextAsset = UnityEngine.TextAsset;
using UnityTexture2D = UnityEngine.Texture2D;
using UnityMathf = UnityEngine.Mathf;

using FairyGUI;

namespace GameEngine
{
    /// <summary>
    /// FairyGUI的窗口对象辅助工具类
    /// </summary>
    internal static class FairyFormHelper
    {
        /// <summary>
        /// alpha分离贴图额外名称(FairyGUI.UIPackage.cs : LoadAtlas(PackageItem item) 内)
        /// </summary>
        const string AlphaTexEndName = "!a";

        /// <summary>
        /// FairyGUI布局文件后缀
        /// </summary>
        const string FairyGuiBinaryFileExtensionName = "_fui.bytes";

        /// <summary>
        /// UI资源目录
        /// </summary>
        static string _fairyGuiResourcePath = null;

        /// <summary>
        /// UI常驻包列表
        /// </summary>
        static readonly List<string> _commonPkgList = new();

        /// <summary>
        /// UI包引用计数
        /// </summary>
        static readonly Dictionary<string, int> _pkgReferenceCount = new();

        /// <summary>
        /// UI资源地址对于的UI资源
        /// </summary>
        static readonly Dictionary<string, UnityObject> _addressToAsset = new();

        static string FairyGuiResourcePath
        {
            get
            {
                if (null == _fairyGuiResourcePath)
                {
                    _fairyGuiResourcePath = NovaEngine.Environment.GetSystemPath("FAIRYGUI_PATH");
                    Debugger.Assert(false == string.IsNullOrEmpty(_fairyGuiResourcePath), "Invalid FairyGui resource path.");
                }

                return _fairyGuiResourcePath;
            }
        }

        /// <summary>
        /// 辅助对象启动接口函数
        /// </summary>
        public static void Startup()
        {
            SetFairyGUICustomConfig();

            //Font font = Resources.Load<Font>("HYWenHei");
            //if (font == null)
            //    return;

            //FontManager.RegisterFont(new DynamicFont("HYWenHei", font));
            //UIConfig.defaultFont = "HYWenHei";
        }

        /// <summary>
        /// 辅助对象关闭接口函数
        /// </summary>
        public static void Shutdown()
        {
        }

        public static void Update()
        {
            CheckNTextureCache();
        }

        #region FairyGUI配置

        /// <summary>
        /// 进行FairyGUI配置
        /// </summary>
        static void SetFairyGUICustomConfig()
        {
            // 已在AppStart调用, 此处不再重复调用
            // SetContentScaleFactor();

            SetCustomDestroyMethod();

            // 关闭自动调节层级
            FairyGUI.UIConfig.bringWindowToFrontOnClick = false;

            // 注册自定义GLoader
            UIObjectFactory.SetLoaderExtension(typeof(FairyGLoader));
        }

        /// <summary>
        /// 初始化UI界面开发时设定的分辨率
        /// </summary>
        static void SetContentScaleFactor()
        {
            GRoot.inst.SetContentScaleFactor(NovaEngine.Environment.designResolutionWidth,
                                             NovaEngine.Environment.designResolutionHeight);
        }

        /// <summary>
        /// 设置FairyGUI自定义卸载函数
        /// </summary>
        static void SetCustomDestroyMethod()
        {
            NTexture.CustomDestroyMethod += obj =>
            {
                RemoveAssetRecord(obj);
                ResourceHandler.Instance.UnloadAsset(obj);
            };
            NAudioClip.CustomDestroyMethod += obj =>
            {
                RemoveAssetRecord(obj);
                ResourceHandler.Instance.UnloadAsset(obj);
            };
        }

        #endregion

        #region FairyGUI Font处理

        /// <summary>
        /// 字体相关处理
        /// </summary>
        /// <param name="url">字体资源url</param>
        /// <param name="fontName">字体文件名</param>
        public static void RegisterFont(string url, string fontName)
        {
            if (FontManager.sFontFactory.ContainsKey(fontName))
            {
                return;
            }

            //Font font = ResourceManager.LoadAsset(url, typeof(Font)) as Font;
            //if (font == null)
            //{
            //    Debugger.Error($"加载字体失败！路径为:{url}");
            //    return;
            //}
            //FontManager.RegisterFont(new DynamicFont(fontName, font));
        }

        #endregion

        #region FairyGUI Package处理

        /// <summary>
        /// 加载常驻包(按钮、声音等)
        /// </summary>
        public static async UniTask AddCommonPackage(string pkgName)
        {
            if (_commonPkgList.Contains(pkgName))
            {
                Debugger.Warn("请勿重复添加常驻包！！！" + pkgName);
                return;
            }

            if (await AddPackage(pkgName))
            {
                _commonPkgList.Add(pkgName);
            }
        }

        /// <summary>
        /// 卸载常驻包
        /// </summary>
        public static void RemoveCommonPackage(string pkgName)
        {
            if (!_commonPkgList.Contains(pkgName))
            {
                return;
            }

            RemovePackage(pkgName);
            _commonPkgList.Remove(pkgName);
        }

        /// <summary>
        /// 判断是否为常驻包
        /// </summary>
        static bool IsCommonPackage(string pkgName)
        {
            return _commonPkgList.Contains(pkgName);
        }

        /// <summary>
        /// 手动加包(仅在自动加包不适用时使用)
        /// </summary>
        public static async UniTask AddPackageManually(string pkgName)
        {
            if (IsCommonPackage(pkgName))
            {
                Debugger.Warn($"资源包“{pkgName}”是通用包, 无需手动添加！");
                return;
            }

            if (await AddPackage(pkgName))
            {
                IncreasePackageReference(pkgName);
            }
        }

        /// <summary>
        /// 手动移除包(仅在自动减包不适用时使用)
        /// </summary>
        public static void RemovePackageManually(string pkgName)
        {
            if (IsCommonPackage(pkgName))
            {
                Debugger.Warn($"资源包“{pkgName}”是通用包, 无需手动移除！");
                return;
            }

            DecreasePackageReference(pkgName);
            RemovePackageWhenNoRef(pkgName);
        }

        /// <summary>
        /// 自定义加载FairyGUI资源
        /// </summary>
        static UnityObject CustomLoadFairyGUIAsset(string name, string extension, SystemType type, out DestroyMethod method)
        {
            method = DestroyMethod.Custom;

            // 不使用FairyGUI的alpha分离,所以直接返回空
            return name.EndsWith(AlphaTexEndName) ? null : _addressToAsset.GetValueOrDefault($"{FairyGuiResourcePath}{name}{extension}");
        }

        /// <summary>
        /// 移除资源记录
        /// </summary>
        static void RemoveAssetRecord(UnityObject obj)
        {
            foreach (var item in _addressToAsset)
            {
                if (item.Value == obj)
                {
                    _addressToAsset.Remove(item.Key);
                    break;
                }
            }
        }

        /// <summary>
        /// 加载资源包
        /// </summary>
        static async UniTask<bool> AddPackage(string pkgName)
        {
            UIPackage pkg;
            UnityTextAsset pkgTextAsset = await ResourceHandler.Instance.LoadAssetAsync<UnityTextAsset>($"{FairyGuiResourcePath}{pkgName}{FairyGuiBinaryFileExtensionName}");
            if (pkgTextAsset != null)
            {
                UIPackage.AddPackage(pkgTextAsset.bytes, pkgName, CustomLoadFairyGUIAsset);
                ResourceHandler.Instance.UnloadAsset(pkgTextAsset);

                pkg = UIPackage.GetByName(pkgName);
                if (pkg is null)
                {
                    Debugger.Error($"{pkgName}的布局文件内容不正确,加载资源包失败");
                    return false;
                }
            }
            else
            {
                Debugger.Error($"加载{pkgName}的布局文件失败");
                return false;
            }

            Dictionary<string, GooAsset.Asset> address2Asset = null;

            foreach (PackageItem pkgItem in pkg.GetItems())
            {
                if (string.IsNullOrEmpty(pkgItem.file))
                {
                    continue;
                }

                string address = $"{FairyGuiResourcePath}{pkgItem.file}";
                address2Asset ??= new Dictionary<string, GooAsset.Asset>();
                GooAsset.Asset asset = ResourceHandler.Instance.LoadAssetAsync<UnityObject>(address, null);
                if (asset is null)
                {
                    return false;
                }

                address2Asset.Add(address, asset);
            }

            if (address2Asset != null)
            {
                GooAsset.Asset[] assets = address2Asset.Values.ToArray();
                await UniTask.WaitUntil(() => { return assets.All(asset => asset.IsDone); });
                foreach (var kv in address2Asset)
                {
                    if (kv.Value.result)
                    {
                        _addressToAsset[kv.Key] = kv.Value.result;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 卸载包资源
        /// </summary>
        static void UnloadAssets(string pkgName)
        {
            UIPackage pkg = UIPackage.GetByName(pkgName);
            pkg?.UnloadAssets();
        }

        /// <summary>
        /// 移除资源包
        /// </summary>
        static void RemovePackage(string pkgName)
        {
            if (_pkgReferenceCount.ContainsKey(pkgName))
            {
                _pkgReferenceCount.Remove(pkgName);
            }

            UIPackage pkg = UIPackage.GetByName(pkgName);
            if (pkg is null)
            {
                return;
            }

            List<string> loadedAssetAddressList = new();
            foreach (PackageItem pkgItem in pkg.GetItems())
            {
                if (string.IsNullOrEmpty(pkgItem.file))
                {
                    continue;
                }

                loadedAssetAddressList.Add($"{FairyGuiResourcePath}{pkgItem.file}");
            }

            UIPackage.RemovePackage(pkgName);

            // 部分资源都已提前加载但可能未被使用, 导致RemovePackage里面不会调用卸载, 故这里需要再次保证卸载
            foreach (string address in loadedAssetAddressList)
            {
                if (_addressToAsset.Remove(address, out UnityObject obj))
                {
                    ResourceHandler.Instance.UnloadAsset(obj);
                }
            }
        }

        /// <summary>
        /// 增加资源包引用计数
        /// </summary>
        static void IncreasePackageReference(string pkgName)
        {
            if (IsCommonPackage(pkgName))
            {
                return;
            }

            _pkgReferenceCount.TryAdd(pkgName, 0);
            _pkgReferenceCount[pkgName]++;
        }

        /// <summary>
        /// 减少资源包引用计数
        /// </summary>
        static void DecreasePackageReference(string pkgName)
        {
            if (_pkgReferenceCount.ContainsKey(pkgName) && _pkgReferenceCount[pkgName] > 0)
            {
                _pkgReferenceCount[pkgName]--;
            }
        }

        /// <summary>
        /// 判断资源包是否存在
        /// </summary>
        static bool IsPackageExist(string pkgName)
        {
            return _pkgReferenceCount.ContainsKey(pkgName);
        }

        /// <summary>
        /// 判断资源包现在是否被引用
        /// </summary>
        static bool IsPackageReferenced(string pkgName)
        {
            return _pkgReferenceCount.ContainsKey(pkgName) && _pkgReferenceCount[pkgName] > 0;
        }

        /// <summary>
        /// 没有引用时卸载包内资源
        /// </summary>
        static void UnloadAssetsWhenNoRef(string pkgName)
        {
            if (_pkgReferenceCount.ContainsKey(pkgName) && _pkgReferenceCount[pkgName] == 0)
            {
                UnloadAssets(pkgName);
            }
        }

        /// <summary>
        /// 没有引用时移除包
        /// </summary>
        static void RemovePackageWhenNoRef(string pkgName)
        {
            if (_pkgReferenceCount.ContainsKey(pkgName) && _pkgReferenceCount[pkgName] == 0)
            {
                RemovePackage(pkgName);
            }
        }

        #endregion

        #region FairyGUI Window处理

        /// <summary>
        /// 重新载入窗口资源
        /// </summary>
        public static async UniTask ReloadWindowAssets(string pkgName)
        {
            UIPackage pkg = UIPackage.GetByName(pkgName);
            if (pkg is null)
            {
                return;
            }

            // 关联包处理
            foreach (Dictionary<string, string> item in pkg.dependencies)
            {
                string dependentPkgName = item["name"];
                if (IsCommonPackage(dependentPkgName))
                {
                    continue;
                }

                if (IsPackageExist(dependentPkgName))
                {
                    if (!IsPackageReferenced(dependentPkgName))
                    {
                        UIPackage.GetByName(dependentPkgName).ReloadAssets();
                    }
                    IncreasePackageReference(dependentPkgName);
                }
                else
                {
                    if (await AddPackage(dependentPkgName))
                    {
                        IncreasePackageReference(dependentPkgName);
                    }
                }
            }

            pkg.ReloadAssets();
            IncreasePackageReference(pkgName);
        }

        /// <summary>
        /// 卸载窗口资源
        /// </summary>
        public static void UnloadWindowAssets(string pkgName)
        {
            UIPackage pkg = UIPackage.GetByName(pkgName);
            if (pkg is null)
            {
                return;
            }

            // 关联包处理
            foreach (Dictionary<string, string> item in pkg.dependencies)
            {
                string dependentPkgName = item["name"];
                if (IsCommonPackage(dependentPkgName))
                {
                    continue;
                }

                DecreasePackageReference(dependentPkgName);
                UnloadAssetsWhenNoRef(dependentPkgName);
            }

            DecreasePackageReference(pkgName);
            UnloadAssetsWhenNoRef(pkgName);
        }

        /// <summary>
        /// 移除窗口资源包
        /// </summary>
        public static void RemoveWindowPackage(string pkgName)
        {
            UIPackage pkg = UIPackage.GetByName(pkgName);
            if (pkg is null)
            {
                return;
            }

            // 关联包处理
            foreach (Dictionary<string, string> item in pkg.dependencies)
            {
                string dependentPkgName = item["name"];
                if (IsCommonPackage(dependentPkgName))
                {
                    continue;
                }

                DecreasePackageReference(dependentPkgName);
                RemovePackageWhenNoRef(dependentPkgName);
            }

            DecreasePackageReference(pkgName);
            RemovePackageWhenNoRef(pkgName);
        }

        /// <summary>
        /// 加载窗口资源包
        /// </summary>
        /// <param name="pkgName">需要加载的资源包</param>
        /// <param name="rootPkgName">仅此方法内部调用时才需要传入,平时忽略</param>
        /// <returns>加载结果</returns>
        static async UniTask<bool> LoadWindowPackage(string pkgName, string rootPkgName = null)
        {
            if (IsPackageExist(pkgName))
            {
                Debugger.Error($"{pkgName}已存在, 不能重复加载！！！");
                return false;
            }

            // 判断加载结果
            if (!await AddPackage(pkgName))
            {
                return false;
            }

            IncreasePackageReference(pkgName);

            // 关联包处理
            UIPackage mainPackage = UIPackage.GetByName(pkgName);
            foreach (Dictionary<string, string> item in mainPackage.dependencies)
            {
                string dependentPkgName = item["name"];
                if (IsCommonPackage(dependentPkgName))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(rootPkgName))
                {
                    Debugger.Error($"不允许存在多层引用，请检查包体引用关系，加载{rootPkgName}时, 当前关联包:{pkgName}又引用了另一个非常驻包:{dependentPkgName}");
                    continue;
                }

                if (IsPackageExist(dependentPkgName))
                {
                    if (IsPackageReferenced(dependentPkgName))
                    {
                        IncreasePackageReference(dependentPkgName);
                    }
                    else
                    {
                        await ReloadWindowAssets(dependentPkgName);
                    }
                }
                else
                {
                    if (!await LoadWindowPackage(dependentPkgName, pkgName))
                    {
                        Debugger.Warn($"加载{pkgName}时, 其关联包:{dependentPkgName}加载失败");
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 异步创建窗口显示组件
        /// </summary>
        public static async void CreateContentPaneAsync(string pkgName, string comName, UIPackage.CreateObjectCallback createObjectFinish)
        {
            if (IsPackageExist(pkgName))
            {
                if (!IsPackageReferenced(pkgName))
                {
                    await ReloadWindowAssets(pkgName);
                }
                else
                {
                    IncreasePackageReference(pkgName);

                    // 关联包处理
                    UIPackage mainPackage = UIPackage.GetByName(pkgName);
                    foreach (Dictionary<string, string> item in mainPackage.dependencies)
                    {
                        string dependentPkgName = item["name"];
                        if (IsCommonPackage(dependentPkgName))
                        {
                            continue;
                        }

                        if (IsPackageExist(dependentPkgName) && IsPackageReferenced(dependentPkgName))
                        {
                            IncreasePackageReference(dependentPkgName);
                        }
                    }
                }

                UIPackage.CreateObjectAsync(pkgName, comName, createObjectFinish);
            }
            else
            {
                if (await LoadWindowPackage(pkgName))
                {
                    UIPackage.CreateObjectAsync(pkgName, comName, createObjectFinish);
                }
                else
                {
                    createObjectFinish(null);
                }
            }
        }

        #endregion

        #region FairyGUI加载外部Texture处理

        /// <summary>
        /// 参数分隔符
        /// </summary>
        public const char SplitSymbol = '@';

        /// <summary>
        /// 检查缓存池时间(秒)
        /// </summary>
        const int CheckPoolTime = 30;

        /// <summary>
        /// 小图片缓存池的容量
        /// </summary>
        const int MaxSmallPoolSize = 0;

        /// <summary>
        /// 清理小图片缓存池时清理到的数量
        /// </summary>
        const int KeepSmallPoolSize = 0;

        /// <summary>
        /// 大图片缓存池的容量
        /// </summary>
        const int MaxBigPoolSize = 0;

        /// <summary>
        /// 清理大图片缓存池时清理到的数量
        /// </summary>
        const int KeepBigPoolSize = 0;

        /// <summary>
        /// 图片的临界长度(大于临界宽长度为大图片,小于等于临界长度视为小图片)
        /// </summary>
        const float ImgCriticalLength = 150;

        /// <summary>
        /// 加载成功回调委托
        /// </summary>
        public delegate void LoadCompleteCallback(NTexture texture, string url);

        /// <summary>
        /// 加载失败回调委托
        /// </summary>
        public delegate void LoadErrorCallback(string url);

        /// <summary>
        /// 检查缓存池计时器
        /// </summary>
        static float _checkPoolTimer;

        /// <summary>
        /// NTexture小图片缓存池
        /// </summary>
        static readonly Dictionary<string, NTexture> _smallNTexturePool = new();

        /// <summary>
        /// NTexture大图片缓存池
        /// </summary>
        static readonly Dictionary<string, NTexture> _bigNTexturePool = new();

        /// <summary>
        /// 加载外部图片
        /// </summary>
        /// <param name="param">图片参数(格式:资源路径@是否同步加载(可选参数,不填默认false),例子:%ORIGINAL_RESOURCE_PATH%/resdir/example.prefab@false)</param>
        /// <param name="onSuccess">加载成功回调</param>
        /// <param name="onFail">加载失败回调</param>
        internal static void LoadExternalIcon(string param, LoadCompleteCallback onSuccess, LoadErrorCallback onFail)
        {
            string[] args = param.Split(SplitSymbol);
            string url = args[0];
            if (_smallNTexturePool.TryGetValue(url, out NTexture smallTexture))
            {
                onSuccess?.Invoke(smallTexture, url);
            }
            else if (_bigNTexturePool.TryGetValue(url, out NTexture bigTexture))
            {
                onSuccess?.Invoke(bigTexture, url);
            }
            else
            {
                bool isSyncLoad = args.Length > 1 && args[1] == "true";
                if (!isSyncLoad)
                {
                    // 异步加载
                    ResourceHandler.Instance.LoadAssetAsync<UnityTexture2D>(url, tex => { OnLoadTextureFinish(url, tex as UnityTexture2D, onSuccess, onFail); });
                }
                else
                {
                    Debugger.Error("此项目暂不支持同步加载~");
                    // 同步加载
                    // UnityTexture2D tex = ResourceHandler.LoadAsset(url, typeof(UnityTexture2D)) as UnityTexture2D;
                    // OnLoadTextureFinish(url, tex, onSuccess, onFail);
                }
            }
        }

        /// <summary>
        /// 图片加载完成处理
        /// </summary>
        static void OnLoadTextureFinish(string url, UnityTexture2D texture2D, LoadCompleteCallback onSuccess, LoadErrorCallback onFail)
        {
            if (texture2D != null)
            {
                NTexture texture;
                if (UnityMathf.Max(texture2D.width, texture2D.height) <= ImgCriticalLength)
                {
                    _smallNTexturePool.TryGetValue(url, out texture);
                }
                else
                {
                    _bigNTexturePool.TryGetValue(url, out texture);
                }

                if (texture == null)
                {
                    texture = new NTexture(texture2D) { destroyMethod = DestroyMethod.Custom };
                    if (UnityMathf.Max(texture.width, texture.height) <= ImgCriticalLength)
                    {
                        _smallNTexturePool[url] = texture;
                    }
                    else
                    {
                        _bigNTexturePool[url] = texture;
                    }
                }
                else
                {
                    ResourceHandler.Instance.UnloadAsset(texture2D);
                }

                onSuccess?.Invoke(texture, url);
            }
            else
            {
                onFail?.Invoke(url);
            }
        }

        /// <summary>
        /// NTexture缓存检查
        /// </summary>
        static void CheckNTextureCache()
        {
            _checkPoolTimer += UnityTime.unscaledDeltaTime;
            if (_checkPoolTimer < CheckPoolTime)
            {
                return;
            }

            _checkPoolTimer = 0;

            // 检查小图片缓存池
            int poolCount = _smallNTexturePool.Count;
            if (poolCount > MaxSmallPoolSize)
            {
                List<string> removeList = null;
                foreach (var item in _smallNTexturePool)
                {
                    if (item.Value.refCount > 0)
                    {
                        continue;
                    }

                    removeList ??= new List<string>();
                    item.Value.Dispose();
                    removeList.Add(item.Key);
                    poolCount--;
                    if (poolCount <= KeepSmallPoolSize)
                    {
                        break;
                    }
                }

                for (int i = 0; i < (removeList?.Count ?? 0); i++)
                {
                    _smallNTexturePool.Remove(removeList[i]);
                }
            }

            // 检查大图片缓存池
            poolCount = _bigNTexturePool.Count;
            if (poolCount > MaxBigPoolSize)
            {
                List<string> removeList = null;
                foreach (var item in _bigNTexturePool)
                {
                    if (item.Value.refCount <= 0)
                    {
                        removeList ??= new List<string>();
                        item.Value.Dispose();
                        removeList.Add(item.Key);
                        poolCount--;
                        if (poolCount <= KeepBigPoolSize)
                        {
                            break;
                        }
                    }
                }

                for (int i = 0; i < (removeList?.Count ?? 0); i++)
                {
                    _bigNTexturePool.Remove(removeList[i]);
                }
            }
        }

        /// <summary>
        /// 清空所有NTexture缓存
        /// </summary>
        public static void ClearNTextureCachePool()
        {
            foreach (var item in _smallNTexturePool)
            {
                item.Value.Dispose();
            }
            _smallNTexturePool.Clear();

            foreach (var item in _bigNTexturePool)
            {
                item.Value.Dispose();
            }
            _bigNTexturePool.Clear();
        }

        #endregion
    }
}

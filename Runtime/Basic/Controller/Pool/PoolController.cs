/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;
using SystemDelegate = System.Delegate;
using SystemActivator = System.Activator;

namespace GameEngine
{
    /// <summary>
    /// 对象池管理类，用于对场景上下文中使用的对象池提供通用的访问操作接口
    /// </summary>
    internal sealed partial class PoolController : BaseController<PoolController>
    {
        /// <summary>
        /// 池对象类型统计列表
        /// </summary>
        private IList<SystemType> _poolObjectTypes = null;

        /// <summary>
        /// 池对象处理信息管理列表
        /// </summary>
        private IDictionary<SystemType, PoolObjectProcessInfo> _poolObjectProcessInfos = null;

        /// <summary>
        /// 池管理对象初始化通知接口函数
        /// </summary>
        protected override sealed void OnInitialize()
        {
            // 初始化池对象类型列表
            _poolObjectTypes = new List<SystemType>();

            // 初始化池对象处理信息管理列表
            _poolObjectProcessInfos = new Dictionary<SystemType, PoolObjectProcessInfo>();
        }

        /// <summary>
        /// 池管理对象清理通知接口函数
        /// </summary>
        protected override sealed void OnCleanup()
        {
            // 清理池对象处理信息管理列表
            RemoveAllPoolObjectProcessInfos();
            _poolObjectProcessInfos = null;

            // 清理池对象类型列表
            RemoveAllPoolObjectTypes();
            _poolObjectTypes = null;
        }

        /// <summary>
        /// 池管理对象刷新调度函数接口
        /// </summary>
        protected override sealed void OnUpdate()
        {
        }

        /// <summary>
        /// 池管理对象后置刷新调度函数接口
        /// </summary>
        protected override sealed void OnLateUpdate()
        {
        }

        /// <summary>
        /// 池管理对象倾泻调度函数接口
        /// </summary>
        protected override void OnDump()
        {
        }

        /// <summary>
        /// 通过指定的对象类型创建一个实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回对象实例，若创建失败则返回null</returns>
        public T CreateObject<T>() where T : class, new()
        {
            return CreateObject(typeof(T)) as T;
        }

        /// <summary>
        /// 通过指定的对象类型创建一个实例
        /// </summary>
        /// <param name="classType">对象类型</param>
        /// <returns>返回对象实例，若创建失败则返回null</returns>
        public object CreateObject(SystemType classType)
        {
            // 非池化管理的目标对象类型
            if (false == IsPoolObjectType(classType))
            {
                object instance = SystemActivator.CreateInstance(classType);
                Debugger.Assert(null != instance, "Invalid arguments.");

                return instance;
            }

            if (TryGetPoolObjectProcessByType(classType, out PoolObjectProcessInfo info))
            {
                return info.ObjectCreateCallback.DynamicInvoke(classType);
            }

            Debugger.Warn("Unsupported create object with target type '{0}' from pool controller, created instance failed.",
                    NovaEngine.Utility.Text.ToString(classType));
            return null;
        }

        /// <summary>
        /// 释放指定的对象实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        public void ReleaseObject(object obj)
        {
            if (null == obj)
            {
                Debugger.Warn("The target object to be released must be non-null.");
                return;
            }

            // 非池化管理的目标对象类型
            if (false == IsPoolObjectType(obj.GetType()))
            {
                // 直接丢弃
                // obj = null;
                return;
            }

            if (TryGetPoolObjectProcessByType(obj.GetType(), out PoolObjectProcessInfo info))
            {
                info.ObjectReleaseCallback.DynamicInvoke(obj);
                return;
            }

            Debugger.Warn("Unsupported release object with target type '{0}' from pool controller, released instance failed.",
                    NovaEngine.Utility.Text.ToString(obj.GetType()));
        }

        #region 池对象类型注册/注销操作接口函数

        /// <summary>
        /// 检测指定的对象类型是否支持池对象管理
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <returns>若给定类型为池化对象则返回true，否则返回false</returns>
        private bool IsPoolObjectType(SystemType targetType)
        {
            for (int n = 0; n < _poolObjectTypes.Count; ++n)
            {
                if (_poolObjectTypes[n] == targetType || _poolObjectTypes[n].IsAssignableFrom(targetType))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 添加指定的对象类型到当前的池对象类型列表中<br/>
        /// 新的对象类型成功添加后，此类型对象在创建实例时将根据类型使用对应的池管理方式
        /// </summary>
        /// <param name="targetType">对象类型</param>
        public void AddPoolObjectType(SystemType targetType)
        {
            if (_poolObjectTypes.Contains(targetType))
            {
                Debugger.Warn("The target object type '{0}' was already exist within pool controller, repeat added it failed.",
                        NovaEngine.Utility.Text.ToString(targetType));
                return;
            }

            _poolObjectTypes.Add(targetType);
        }

        /// <summary>
        /// 从当前的池对象类型列表中移除指定的对象类型
        /// </summary>
        /// <param name="targetType">对象类型</param>
        private void RemovePoolObjectType(SystemType targetType)
        {
            if (_poolObjectTypes.Contains(targetType))
            {
                _poolObjectTypes.Remove(targetType);
            }
        }

        /// <summary>
        /// 移除当前池对象类型列表中的全部对象类型
        /// </summary>
        private void RemoveAllPoolObjectTypes()
        {
            _poolObjectTypes.Clear();
        }

        #endregion

        #region 池对象处理逻辑接口函数

        /// <summary>
        /// 添加指定的对象类型到当前的池对象处理信息管理列表中
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="createAction">创建对象回调函数</param>
        /// <param name="releaseAction">释放对象回调函数</param>
        private void AddPoolObjectProcessInfo<T>(System.Func<SystemType, T> createAction, System.Action<T> releaseAction)
        {
            SystemDelegate createCallback = SystemDelegate.CreateDelegate(typeof(System.Func<SystemType, T>), this, createAction.Method);
            SystemDelegate releaseCallback = SystemDelegate.CreateDelegate(typeof(System.Action<T>), this, releaseAction.Method);

            AddPoolObjectProcessInfo(typeof(T), createCallback, releaseCallback);
        }

        /// <summary>
        /// 添加指定的对象类型到当前的池对象处理信息管理列表中
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="createCallback">创建对象回调函数</param>
        /// <param name="releaseCallback">释放对象回调函数</param>
        private void AddPoolObjectProcessInfo(SystemType targetType, SystemDelegate createCallback, SystemDelegate releaseCallback)
        {
            if (_poolObjectProcessInfos.ContainsKey(targetType))
            {
                Debugger.Warn("The target object type '{0}' was already exist within pool controller's processing map, repeat added it failed.",
                        NovaEngine.Utility.Text.ToString(targetType));
                return;
            }

            PoolObjectProcessInfo info = new PoolObjectProcessInfo(createCallback, releaseCallback);
            _poolObjectProcessInfos.Add(targetType, info);
        }

        /// <summary>
        /// 通过指定的对象类型获取其对应的池对象处理信息
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="info">池对象处理信息</param>
        /// <returns>若存在指定的对象类型则返回true，否则返回false</returns>
        private bool TryGetPoolObjectProcessByType(SystemType targetType, out PoolObjectProcessInfo info)
        {
            IEnumerator<SystemType> e = _poolObjectProcessInfos.Keys.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current == targetType || e.Current.IsAssignableFrom(targetType))
                {
                    info = _poolObjectProcessInfos[e.Current];
                    return true;
                }
            }

            info = null;

            return false;
        }

        /// <summary>
        /// 从当前的池对象处理信息管理列表中移除指定的对象类型
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        private void RemovePoolObjectProcessInfo<T>()
        {
            RemovePoolObjectProcessInfo(typeof(T));
        }

        /// <summary>
        /// 从当前的池对象处理信息管理列表中移除指定的对象类型
        /// </summary>
        /// <param name="targetType">对象类型</param>
        private void RemovePoolObjectProcessInfo(SystemType targetType)
        {
            if (_poolObjectProcessInfos.ContainsKey(targetType))
            {
                _poolObjectProcessInfos.Remove(targetType);
            }
        }

        /// <summary>
        /// 移除当前池对象处理信息管理列表中的全部对象类型
        /// </summary>
        private void RemoveAllPoolObjectProcessInfos()
        {
            _poolObjectProcessInfos.Clear();
        }

        /// <summary>
        /// 池对象处理信息封装对象类
        /// </summary>
        private sealed class PoolObjectProcessInfo
        {
            /// <summary>
            /// 对象创建回调函数
            /// </summary>
            private readonly SystemDelegate _objectCreateCallback;
            /// <summary>
            /// 对象释放回调函数
            /// </summary>
            private readonly SystemDelegate _objectReleaseCallback;

            public SystemDelegate ObjectCreateCallback => _objectCreateCallback;
            public SystemDelegate ObjectReleaseCallback => _objectReleaseCallback;

            public PoolObjectProcessInfo(SystemDelegate objectCreateCallback, SystemDelegate objectReleaseCallback)
            {
                _objectCreateCallback = objectCreateCallback;
                _objectReleaseCallback = objectReleaseCallback;
            }
        }

        #endregion
    }
}

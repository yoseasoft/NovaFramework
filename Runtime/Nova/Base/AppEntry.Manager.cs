/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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
using SystemStringComparison = System.StringComparison;

namespace NovaEngine
{
    /// <summary>
    /// 应用程序的总入口，提供引擎中绑定组件或调度器的统一管理类
    /// </summary>
    public static partial class AppEntry
    {
        /// <summary>
        /// 管理器对象的链表容器
        /// </summary>
        private static readonly CacheLinkedList<IManager> _frameworkManagers = new CacheLinkedList<IManager>();

        /// <summary>
        /// 管理器对象的可更新列表容器
        /// </summary>
        private static readonly CacheLinkedList<IUpdatable> _updatedManagers = new CacheLinkedList<IUpdatable>();

        /// <summary>
        /// 管理器对象的过期列表容器
        /// </summary>
        private static readonly IList<IManager> _expiredManagers = new List<IManager>();

        /// <summary>
        /// 管理器对象处于刷新调度中的状态标识
        /// </summary>
        private static bool _isManagerUpdating = false;

        /// <summary>
        /// 检查当前容器中是否已注册指定类型的管理器实例
        /// </summary>
        /// <typeparam name="T">管理器类型</typeparam>
        /// <returns>若给定类型的管理器实例已存在则返回true，否则返回false</returns>
        public static bool HasManager<T>() where T : IManager
        {
            SystemType managerType = GetActualUsageMamagerType(typeof(T));

            return HasManager(managerType);
        }

        /// <summary>
        /// 检查当前容器中是否已注册指定类型的管理器实例
        /// </summary>
        /// <param name="managerType">管理器类型</param>
        /// <returns>若给定类型的管理器实例已存在则返回true，否则返回false</returns>
        public static bool HasManager(SystemType managerType)
        {
            // 检查是否存在相同类型的管理器实例
            foreach (IManager v in _frameworkManagers)
            {
                if (v.GetType() == managerType) { return true; }
            }

            return false;
        }

        /// <summary>
        /// 从当前容器中查找指定类型对应的管理器实例
        /// </summary>
        /// <typeparam name="T">管理器类型</typeparam>
        /// <returns>返回查找的管理器实例，若实例不存在，则自动创建一个新的实例并返回</returns>
        public static T GetManager<T>() where T : class
        {
            return CreateManager<T>();
        }

        /// <summary>
        /// 获取指定类型的管理器实例<br/>
        /// 如果当前容器中没有对应的实例，则会默认创建一个新的实例并返回
        /// </summary>
        /// <param name="managerType">管理器类型</param>
        /// <returns>返回给定类型对应的管理器实例</returns>
        private static IManager GetManager(SystemType managerType)
        {
            return CreateManager(managerType);
        }

        /// <summary>
        /// 创建一个指定类型的管理器实例
        /// </summary>
        /// <typeparam name="T">管理器类型</typeparam>
        /// <returns>若实例创建成功则返回其引用，否则返回null</returns>
        public static T CreateManager<T>() where T : class
        {
            SystemType managerType = GetActualUsageMamagerType(typeof(T));

            return CreateManager(managerType) as T;
        }

        /// <summary>
        /// 创建一个指定类型的管理器实例
        /// </summary>
        /// <param name="managerType">管理器类型</param>
        /// <returns>若实例创建成功则返回其引用，否则返回null</returns>
        private static IManager CreateManager(SystemType managerType)
        {
            // 检查是否存在相同类型的管理器实例
            foreach (IManager v in _frameworkManagers)
            {
                if (v.GetType() == managerType) { return v; }
            }

            IManager manager = System.Activator.CreateInstance(managerType) as IManager;
            if (null == manager)
            {
                throw new CFrameworkException("Cannot create manager '{0}'.", managerType.FullName);
            }

            // 管理器实例初始化
            if (typeof(IInitializable).IsAssignableFrom(manager.GetType()))
            {
                (manager as IInitializable).Initialize();
            }

            // 按优先级排序
            LinkedListNode<IManager> current = _frameworkManagers.First;
            while (null != current)
            {
                if (manager.Priority > current.Value.Priority)
                {
                    break;
                }

                current = current.Next;
            }

            if (null != current)
            {
                _frameworkManagers.AddBefore(current, manager);
            }
            else
            {
                _frameworkManagers.AddLast(manager);
            }

            // 更新管理器实例的刷新列表
            RefreshManagerUpdateList();

            return manager;
        }

        /// <summary>
        /// 刷新当前总控中所有管理器实例的刷新回调通知列表
        /// </summary>
        private static void RefreshManagerUpdateList()
        {
            _updatedManagers.Clear();

            foreach (IManager manager in _frameworkManagers)
            {
                if (typeof(IUpdatable).IsAssignableFrom(manager.GetType()) && false == IsExpiredManager(manager))
                {
                    _updatedManagers.AddLast(manager as IUpdatable);
                }
            }
        }

        /// <summary>
        /// 移除指定类型的管理器对象实例
        /// </summary>
        /// <typeparam name="T">管理器类型</typeparam>
        public static void RemoveManager<T>()
        {
            SystemType managerType = GetActualUsageMamagerType(typeof(T));

            RemoveManager(managerType);
        }

        /// <summary>
        /// 移除指定类型的管理器对象实例
        /// </summary>
        /// <param name="managerType">管理器类型</param>
        private static void RemoveManager(SystemType managerType)
        {
            IManager manager = GetManager(managerType);

            if (null != manager)
            {
                RemoveManager(manager);
            }
            else
            {
                Logger.Warn("Could not found any manager instance with type '{0}', remove ti failed.", managerType.FullName);
            }
        }

        /// <summary>
        /// 移除指定管理器对象实例
        /// </summary>
        /// <param name="manager">管理器实例</param>
        public static void RemoveManager(IManager manager)
        {
            Logger.Assert(null != manager, "Invalid manager instance.");

            // 处于更新中
            if (_isManagerUpdating)
            {
                if (IsExpiredManager(manager))
                {
                    Logger.Warn("The target manager '{0}' was already expired, repeat removed it failed.", manager.GetType().FullName);
                }
                else
                {
                    _expiredManagers.Add(manager);
                }
                return;
            }

            bool changed = false;
            for (LinkedListNode<IManager> current = _frameworkManagers.First; null != current; current = current.Next)
            {
                if (current.Value == manager)
                {
                    DestroyManager(current.Value);

                    _frameworkManagers.Remove(current);
                    changed = true;
                    break;
                }
            }

            // 管理器列表已改变，重置管理器刷新列表
            if (changed)
            {
                RefreshManagerUpdateList();
            }
        }

        /// <summary>
        /// 清理容器中所有管理器实例
        /// </summary>
        private static void RemoveAllManagers()
        {
            Logger.Assert(!_isManagerUpdating, "Cannot remove manager within update progressing!");
            for (LinkedListNode<IManager> current = _frameworkManagers.Last; null != current; current = current.Next)
            {
                DestroyManager(current.Value);
            }

            _frameworkManagers.Clear();
            _updatedManagers.Clear();
        }

        /// <summary>
        /// 清理容器中的所有过期的管理器对象实例
        /// </summary>
        private static void RemoveAllExpiredManagers()
        {
            for (int n = _expiredManagers.Count - 1; n >= 0; --n)
            {
                RemoveManager(_expiredManagers[n]);
            }

            _expiredManagers.Clear();
        }

        /// <summary>
        /// 销毁管理器实例
        /// </summary>
        /// <param name="manager">管理器实例</param>
        private static void DestroyManager(IManager manager)
        {
            // 清理管理器实例
            if (typeof(IInitializable).IsAssignableFrom(manager.GetType()))
            {
                (manager as IInitializable).Cleanup();
            }
        }

        /// <summary>
        /// 检测目标管理器实例是否为过期状态
        /// </summary>
        /// <param name="manager">管理器实例</param>
        /// <returns>若给定管理器实例为过期状态则返回true，否则返回false</returns>
        private static bool IsExpiredManager(IManager manager)
        {
            if (_expiredManagers.Contains(manager))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 对当前注册的所有管理器对象实例执行刷新调度操作<br/>
        /// 需注意的是，仅有实现了<see cref="NovaEngine.IUpdatable"/>接口的实例，才具备刷新调度的功能
        /// </summary>
        private static void UpdateAllManagers()
        {
            _isManagerUpdating = true;

            foreach (IUpdatable v in _updatedManagers)
            {
                if (IsExpiredManager(v as IManager))
                {
                    continue;
                }

                v.Update();
            }

            _isManagerUpdating = false;
        }

        /// <summary>
        /// 对当前注册的所有管理器对象实例执行后置刷新调度操作<br/>
        /// 需注意的是，仅有实现了<see cref="NovaEngine.IUpdatable"/>接口的实例，才具备后置刷新调度的功能
        /// </summary>
        private static void LateUpdateAllManagers()
        {
            _isManagerUpdating = true;

            foreach (IUpdatable v in _updatedManagers)
            {
                if (IsExpiredManager(v as IManager))
                {
                    continue;
                }

                v.LateUpdate();
            }

            _isManagerUpdating = false;

            // 在更新结束标识后，移除过期管理器实例
            RemoveAllExpiredManagers();
        }

        /// <summary>
        /// 获取实际应用的模块类型<br/>
        /// 若传递参数为接口类型，则自动裁剪掉“I"前缀，生成实际应用的对象类型
        /// </summary>
        /// <param name="managerType">模块类型</param>
        /// <returns>返回真实应用的模块类型</returns>
        private static SystemType GetActualUsageMamagerType(SystemType managerType)
        {
            SystemType actualType = managerType;
            if (actualType.IsInterface)
            {
                // throw new CException("You must get manager by interface, but '{0}' is not.", actualType.FullName);

                string managerName = Utility.Text.Format("{0}.{1}", actualType.Namespace, actualType.Name.Substring(1));

                System.Reflection.Assembly[] _assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                foreach (System.Reflection.Assembly assembly in _assemblies)
                {
                    actualType = SystemType.GetType(string.Format("{0}, {1}", managerName, assembly.FullName));
                    if (null != actualType)
                    {
                        break;
                    }
                }
                // actualType = SystemType.GetType(managerName);

                if (null == actualType)
                {
                    throw new CFrameworkException("Cannot find manager type '{0}'.", managerName);
                }
            }

            /*
            if (false == interfaceType.FullName.StartsWith("NovaEngine.", SystemStringComparison.Ordinal))
            {
                throw new CException("You must get a Nova Engine manager, but '{0}' is not.", interfaceType.FullName);
            }
            */

            return actualType;
        }
    }
}

/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System;
using System.Collections.Generic;
using System.Customize.Extension;
using System.Runtime.CompilerServices;
using UnityEngine.Customize.Extension;

using UnityObject = UnityEngine.Object;
using UnityGameObject = UnityEngine.GameObject;

namespace NovaEngine
{
    /// <summary>
    /// 应用程序的总入口，提供引擎中绑定组件或调度器的统一管理类
    /// </summary>
    internal static partial class AppEntry
    {
        /// <summary>
        /// 节点对象的映射容器
        /// </summary>
        private static readonly IDictionary<string, UnityGameObject> _frameworkGameObjects = new Dictionary<string, UnityGameObject>();
        /// <summary>
        /// 组件对象的映射容器
        /// </summary>
        private static readonly IDictionary<string, CFrameworkComponent> _frameworkComponents = new Dictionary<string, CFrameworkComponent>();

        /// <summary>
        /// 通过指定的对象类型声明获取对应的组件对象实例
        /// </summary>
        /// <typeparam name="T">类型声明</typeparam>
        /// <returns>返回类型声明对应的组件实例，若不存在则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponent<T>() where T : CFrameworkComponent
        {
            return (T) GetComponent(typeof(T));
        }

        /// <summary>
        /// 通过指定的类型标识获取对应的组件对象实例
        /// </summary>
        /// <param name="componentType">类型标识</param>
        /// <returns>返回类型标识对应的组件实例，若不存在则返回null</returns>
        public static CFrameworkComponent GetComponent(Type componentType)
        {
            foreach (KeyValuePair<string, CFrameworkComponent> pair in _frameworkComponents)
            {
                if (pair.Value.GetType() == componentType)
                {
                    return GetComponent(pair.Key);
                }
            }

            return null;
        }

        /// <summary>
        /// 通过指定的名称获取对应的组件对象实例
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <returns>返回名称对应的组件实例，若不存在则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CFrameworkComponent GetComponent(string name)
        {
            if (_frameworkComponents.TryGetValue(name, out CFrameworkComponent component))
            {
                return component;
            }

            return null;
        }

        /// <summary>
        /// 注册一个新的组件对象实例
        /// 需要注意的是，此处注册的组件实例，需要保证唯一性，即不可多次注册同一类型的组件对象
        /// 在注册组件时，将自动创建一个指定名称的新GameObject对象实例
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>返回注册的组件对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RegisterComponent<T>() where T : CFrameworkComponent
        {
            return (T) RegisterComponent(typeof(T));
        }

        /// <summary>
        /// 注册一个新的组件对象实例
        /// 需要注意的是，此处注册的组件实例，需要保证唯一性，即不可多次注册同一类型的组件对象
        /// 在注册组件时，将自动创建一个指定名称的新GameObject对象实例
        /// </summary>
        /// <param name="componentType">组件类型</param>
        /// <returns>返回注册的组件对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CFrameworkComponent RegisterComponent(Type componentType)
        {
            return RegisterComponent(componentType.FullName, componentType);
        }

        /// <summary>
        /// 注册一个新的组件对象实例
        /// 需要注意的是，此处注册的组件实例，需要保证唯一性，即不可多次注册同一类型的组件对象
        /// 在注册组件时，将自动创建一个指定名称的新GameObject对象实例
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="name">节点名称</param>
        /// <returns>返回注册的组件对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RegisterComponent<T>(string name) where T : CFrameworkComponent
        {
            return (T) RegisterComponent(name, typeof(T));
        }

        /// <summary>
        /// 注册一个新的组件对象实例
        /// 需要注意的是，此处注册的组件实例，需要保证唯一性，即不可多次注册同一类型的组件对象
        /// 在注册组件时，将自动创建一个指定名称的新GameObject对象实例
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <param name="componentType">组件类型</param>
        /// <returns>返回注册的组件对象实例</returns>
        public static CFrameworkComponent RegisterComponent(string name, Type componentType)
        {
            if (name.IsNullOrEmpty())
            {
                Logger.Error("The register component name must be non-null.");
                return null;
            }

            // 每个GameObject仅能容许一个CFrameworkComponent实例
            if (_frameworkGameObjects.ContainsKey(name))
            {
                Logger.Error("The register component name '{%s}' is already exist, cannot repeat register it.", name);
                CFrameworkComponent c = _frameworkComponents[name];
                if (c.GetType() == componentType)
                {
                    return c;
                }

                // 已注册的组件类型和新注册组件类型不一致
                Logger.Error("The register component type '{%t}' not matched the exist type '{%t}', get it failed.", componentType, c);
                return null;
            }

            if (false == typeof(CFrameworkComponent).IsAssignableFrom(componentType))
            {
                Logger.Error("The register component type '{%t}' must be inherited from CFrameworkComponent, check the type invalid.", componentType);
                return null;
            }

            UnityGameObject gameObject = new UnityGameObject(name);
            CFrameworkComponent component = (CFrameworkComponent) gameObject.GetOrAddComponent(componentType);

            if (null != _rootGameObject)
            {
                _rootGameObject.AddChild(gameObject);
            }
            else
            {
                Logger.Warn("Could not found root game object, setting component's parent failed.");
            }

            _frameworkGameObjects.Add(name, gameObject);
            _frameworkComponents.Add(name, component);

            return component;
        }

        /// <summary>
        /// 注销指定名称的组件对象实例
        /// </summary>
        /// <param name="name">节点名称</param>
        public static void UnregisterComponent(string name)
        {
            if (name.IsNullOrEmpty())
            {
                Logger.Error("The unregister component name must be non-null.");
                return;
            }

            if (false == _frameworkGameObjects.ContainsKey(name))
            {
                Logger.Error("Could not found any component name '{%s}' in current framework, unregister it failed.", name);
                return;
            }

            UnityGameObject gameObject = _frameworkGameObjects[name];
            CFrameworkComponent component = _frameworkComponents[name];
            UnityObject.Destroy(component);
            UnityObject.Destroy(gameObject);

            _frameworkGameObjects.Remove(name);
            _frameworkComponents.Remove(name);
        }

        /// <summary>
        /// 注销指定类型的组件对象实例
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnregisterComponent<T>()
        {
            UnregisterComponent(typeof(T));
        }

        /// <summary>
        /// 注销指定类型的组件对象实例
        /// </summary>
        /// <param name="componentType">组件类型</param>
        public static void UnregisterComponent(Type componentType)
        {
            foreach (KeyValuePair<string, CFrameworkComponent> pair in _frameworkComponents)
            {
                if (pair.Value.GetType() == componentType)
                {
                    UnregisterComponent(pair.Key);
                    return;
                }
            }
        }

        /// <summary>
        /// 移除当前注册的所有组件对象实例
        /// </summary>
        private static void RemoveAllComponents()
        {
            IList<string> keys = Utility.Collection.ToList(_frameworkGameObjects.Keys);
            for (int n = 0; null != keys && n < keys.Count; ++n)
            {
                UnregisterComponent(keys[n]);
            }
        }
    }
}

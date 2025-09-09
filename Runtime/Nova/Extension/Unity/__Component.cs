/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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

using SystemType = System.Type;
using SystemArray = System.Array;

using UnityObject = UnityEngine.Object;
using UnityGameObject = UnityEngine.GameObject;
using UnityComponent = UnityEngine.Component;
using UnityTransform = UnityEngine.Transform;

namespace NovaEngine
{
    /// <summary>
    /// 基于Unity库组件类的扩展接口支持类
    /// </summary>
    public static class __Component
    {
        /// <summary>
        /// 在当前的GameObject中查找指定类型的组件对象实例<br/>
        /// 若目标类型的组件不存在，则新创建一个该类型的实例并返回
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="self">源组件对象实例</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static T GetOrAddComponent<T>(this UnityComponent self) where T : UnityComponent
        {
            T component = self.GetComponent<T>();
            if (null == component)
            {
                component = self.gameObject.AddComponent<T>();
            }
            return component;
        }

        /// <summary>
        /// 在当前的GameObject中查找指定类型的组件对象实例<br/>
        /// 若目标类型的组件不存在，则新创建一个该类型的实例并返回
        /// </summary>
        /// <param name="self">源组件对象实例</param>
        /// <param name="type">目标组件类型</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static UnityComponent GetOrAddComponent(this UnityComponent self, SystemType type)
        {
            UnityComponent component = self.GetComponent(type);
            if (null == component)
            {
                component = self.gameObject.AddComponent(type);
            }
            return component;
        }

        /// <summary>
        /// 从父节点下指定名称的GameObject中查找指定类型的组件对象实例
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="self">源组件对象实例</param>
        /// <param name="name">目标节点名称</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static T GetComponentInParent<T>(this UnityComponent self, string name) where T : UnityComponent
        {
            UnityTransform[] parents = self.GetComponentsInParent<UnityTransform>();
            int length = parents.Length;
            UnityTransform parentTrans = null;
            for (int n = 0; n < length; ++n)
            {
                if (parents[n].name == name)
                {
                    parentTrans = parents[n];
                    break;
                }
            }

            if (null == parentTrans)
                return null;

            return parentTrans.GetComponent<T>();
        }

        /// <summary>
        /// 从父节点下指定名称的GameObject中查找指定类型的组件对象实例<br/>
        /// 若目标类型的组件不存在，则在当前节点下新创建一个该类型的实例并返回<br/>
        /// 此处若未找到指定名称的父节点，则不会创建该类型的组件对象实例，而是直接返回null
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="self">源组件对象实例</param>
        /// <param name="name">目标节点名称</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static T GetOrAddComponentInParent<T>(this UnityComponent self, string name) where T : UnityComponent
        {
            UnityTransform[] parents = self.GetComponentsInParent<UnityTransform>();
            int length = parents.Length;
            UnityTransform parentTrans = null;
            for (int n = 0; n < length; ++n)
            {
                if (parents[n].name == name)
                {
                    parentTrans = parents[n];
                    break;
                }
            }
            if (null == parentTrans)
                return null;

            T comp = parentTrans.GetComponent<T>();
            if (null == comp)
            {
                comp = self.gameObject.AddComponent<T>();
            }

            return comp;
        }

        /// <summary>
        /// 在子节点下指定名称的GameObject中查找指定类型的组件对象实例
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="self">源组件对象实例</param>
        /// <param name="name">目标节点名称</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static T GetComponentInChildren<T>(this UnityComponent self, string name) where T : UnityComponent
        {
            UnityTransform[] childs = self.GetComponentsInChildren<UnityTransform>();
            int length = childs.Length;
            UnityTransform childTrans = null;
            for (int n = 0; n < length; ++n)
            {
                if (childs[n].name == name)
                {
                    childTrans = childs[n];
                    break;
                }
            }
            if (null == childTrans)
                return null;

            return childTrans.GetComponent<T>();
        }

        /// <summary>
        /// 在子节点下指定名称的GameObject中查找指定类型的组件对象实例<br/>
        /// 若目标类型的组件不存在，则在最后一个子节点下新创建一个该类型的实例并返回<br/>
        /// 此处若未找到指定名称的子节点，则不会创建该类型的组件对象实例，而是直接返回null
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="self">源组件对象实例</param>
        /// <param name="name">目标节点名称</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static T GetOrAddComponentInChildren<T>(this UnityComponent self, string name) where T : UnityComponent
        {
            UnityTransform[] childs = self.GetComponentsInChildren<UnityTransform>();
            int length = childs.Length;
            UnityTransform childTrans = null;
            for (int n = 0; n < length; ++n)
            {
                if (childs[n].name == name)
                {
                    childTrans = childs[n];
                    break;
                }
            }
            if (null == childTrans)
                return null;

            T comp = childTrans.GetComponent<T>();
            if (null == comp)
                comp = childTrans.gameObject.AddComponent<T>();

            return comp;
        }

        /// <summary>
        /// 在同级节点下指定名称的GameObject中查找指定类型的组件对象实例
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="self">源组件对象实例</param>
        /// <param name="name">目标节点名称</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static T GetComponentInPeer<T>(this UnityComponent self, string name) where T : UnityComponent
        {
            UnityTransform tran = self.transform.parent.Find(name);
            if (null != tran)
            {
                return tran.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 在同级节点下指定名称的GameObject中查找指定类型的组件对象实例<br/>
        /// 若目标类型的组件不存在，则在当前节点下新创建一个该类型的实例并返回<br/>
        /// 此处若未找到指定名称的同级节点，则不会创建该类型的组件对象实例，而是直接返回null
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="self">源组件对象实例</param>
        /// <param name="name">目标节点名称</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static T GetOrAddComponentInPeer<T>(this UnityComponent self, string name) where T : UnityComponent
        {
            UnityTransform tran = self.transform.parent.Find(name);
            if (null != tran)
            {
                T comp = tran.GetComponent<T>();
                if (null == comp)
                    self.gameObject.AddComponent<T>();

                return comp;
            }

            return null;
        }

        /// <summary>
        /// 在同级节点下查找出所有节点中包含指定类型的组件对象实例
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="self">源组件对象实例</param>
        /// <param name="includeSrc">是否包含源节点对象</param>
        /// <returns>返回对应类型的组件对象实例的集合</returns>
        public static T[] GetComponentsInPeer<T>(this UnityComponent self, bool includeSrc = false) where T : UnityComponent
        {
            UnityTransform parentTrans = self.transform.parent;
            UnityTransform[] childTrans = parentTrans.GetComponentsInChildren<UnityTransform>();
            int length = childTrans.Length;
            UnityTransform[] trans;

            if (!includeSrc)
                trans = Utility.Collection.FindAll(childTrans, t => t.parent == parentTrans);
            else
                trans = Utility.Collection.FindAll(childTrans, t => t.parent == parentTrans && t != self);

            int transLength = trans.Length;
            T[] src = new T[transLength];
            int idx = 0;
            for (int n = 0; n < transLength; ++n)
            {
                T comp = trans[n].GetComponent<T>();
                if (null != comp)
                {
                    src[idx] = comp;
                    ++idx;
                }
            }

            T[] dst = new T[idx];
            SystemArray.Copy(src, 0, dst, 0, idx);

            return dst;
        }

        /// <summary>
        /// 从当前GameObject节点中移除目标类型的组件对象实例
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="self">源组件对象实例</param>
        /// <returns>返回源组件对象实例</returns>
        public static UnityComponent TryRemoveComponent<T>(this UnityComponent self) where T : UnityComponent
        {
            T comp = self.GetComponent<T>();

            if (null != comp)
            {
                UnityObject.Destroy(comp);
            }

            return self;
        }

        /// <summary>
        /// 从当前GameObject节点中移除目标类型的组件对象实例
        /// </summary>
        /// <param name="self">源组件对象实例</param>
        /// <param name="type">目标组件类型</param>
        /// <returns>返回源组件对象实例</returns>
        public static UnityComponent TryRemoveComponent(this UnityComponent self, SystemType type)
        {
            UnityComponent comp = self.GetComponent(type);

            if (null != comp)
            {
                UnityObject.Destroy(comp);
            }

            return self;
        }

        /// <summary>
        /// 从当前GameObject节点中移除目标类型的组件对象实例
        /// </summary>
        /// <param name="self">源组件对象实例</param>
        /// <param name="type">目标组件类型</param>
        /// <returns>返回源组件对象实例</returns>
        public static UnityComponent TryRemoveComponent(this UnityComponent self, string type)
        {
            UnityComponent comp = self.GetComponent(type);

            if (null != comp)
            {
                UnityObject.Destroy(comp);
            }

            return self;
        }

        /// <summary>
        /// 从当前GameObject节点中移除所有满足目标类型条件的组件对象实例
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="self">源组件对象实例</param>
        /// <returns>返回源组件对象实例</returns>
        public static UnityComponent TryRemoveComponents<T>(this UnityComponent self) where T : UnityComponent
        {
            T[] comps = self.GetComponents<T>();

            for (var n = 0; n < comps.Length; ++n)
            {
                UnityObject.Destroy(comps[n]);
            }

            return self;
        }

        /// <summary>
        /// 从当前GameObject节点中移除所有满足目标类型条件的组件对象实例
        /// </summary>
        /// <param name="self">源组件对象实例</param>
        /// <param name="type">目标组件类型</param>
        /// <returns>返回源组件对象实例</returns>
        public static UnityComponent TryRemoveComponents(this UnityComponent self, SystemType type)
        {
            UnityComponent[] comps = self.GetComponents(type);

            for (var n = 0; n < comps.Length; ++n)
            {
                UnityObject.Destroy(comps[n]);
            }

            return self;
        }
    }
}

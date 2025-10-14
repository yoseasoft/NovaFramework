/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;
using SystemArray = System.Array;
using SystemStringBuilder = System.Text.StringBuilder;

using UnityObject = UnityEngine.Object;
using UnityTransform = UnityEngine.Transform;
using UnityComponent = UnityEngine.Component;
using UnityGameObject = UnityEngine.GameObject;
using UnityAnimation = UnityEngine.Animation;
using UnityAnimationClip = UnityEngine.AnimationClip;
using UnityAnimator = UnityEngine.Animator;
using UnityParticleSystem = UnityEngine.ParticleSystem;
using UnityPlayableDirector = UnityEngine.Playables.PlayableDirector;
using UnityAnimatorClipInfo = UnityEngine.AnimatorClipInfo;
using UnityLayerMask = UnityEngine.LayerMask;
using UnityMathf = UnityEngine.Mathf;

namespace NovaEngine
{
    /// <summary>
    /// 基于Unity库游戏对象类的扩展接口支持类
    /// </summary>
    public static class __GameObject
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static UnityGameObject DontDestroy(this UnityGameObject self)
        {
            UnityGameObject.DontDestroyOnLoad(self);
            return self;
        }

        /// <summary>
        /// 以递归的方式设置游戏对象的层级标签
        /// </summary>
        /// <param name="self">游戏对象实例</param>
        /// <param name="layer">目标层级编号</param>
        public static void SetLayerRecursively(this UnityGameObject self, int layer)
        {
            System.Collections.Generic.List<UnityTransform> children = new System.Collections.Generic.List<UnityTransform>();
            self.GetComponentsInChildren(true, children);
            for (int n = 0; n < children.Count; ++n)
            {
                children[n].gameObject.layer = layer;
            }

            children.Clear();
        }

        /// <summary>
        /// 检测当前的游戏对象是否为指定层级
        /// </summary>
        /// <param name="self">游戏对象实例</param>
        /// <param name="mask">层级标识</param>
        /// <returns>若游戏对象为给定层级返回true，否则返回false</returns>
        public static bool IsOnLayerMask(this UnityGameObject self, UnityLayerMask mask)
        {
            return (mask == (mask | (1 << self.layer)));
        }

        /// <summary>
        /// 获取指定游戏对象当前可播放动画的时长
        /// </summary>
        /// <param name="self">游戏对象实例</param>
        public static float GetPlayableTime(this UnityGameObject self)
        {
            float time = 0.0f;

            UnityPlayableDirector[] directors = self.GetComponentsInChildren<UnityPlayableDirector>(true);
            if (directors.Length > 0)
            {
                for (int n = 0; n < directors.Length; ++n)
                {
                    UnityPlayableDirector director = directors[n];
                    if (director.enabled && director.playableAsset != null)
                    {
                        time = UnityMathf.Max(time, (float) director.playableAsset.duration);
                    }
                }
            }

            // 粒子
            UnityParticleSystem[] particles = self.GetComponentsInChildren<UnityParticleSystem>(true);
            if (particles.Length > 0)
            {
                for (int n = 0; n < particles.Length; ++n)
                {
                    UnityParticleSystem particle = particles[n];
                    float duration = particle.main.startLifetime.constantMax + particle.main.duration + particle.main.startDelay.constantMax;
                    time = UnityMathf.Max(time, duration);
                }
            }

            // Animator
            UnityAnimator[] animators = self.GetComponentsInChildren<UnityAnimator>(true);
            if (animators.Length > 0)
            {
                for (int n = 0; n < animators.Length; ++n)
                {
                    UnityAnimator animator = animators[n];
                    if (animator.runtimeAnimatorController != null)
                    {
                        UnityAnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);
                        if (clipInfos.Length > 0)
                        {
                            UnityAnimationClip clip = clipInfos[0].clip;
                            if (clip != null)
                            {
                                time = UnityMathf.Max(time, clip.length);
                            }
                        }
                    }
                }
            }

            // Animation
            UnityAnimation[] animations = self.GetComponentsInChildren<UnityAnimation>(true);
            if (animations.Length > 0)
            {
                for (int n = 0; n < animations.Length; ++n)
                {
                    UnityAnimation animation = animations[n];
                    if (animation != null && animation.clip != null)
                    {
                        time = UnityMathf.Max(time, animation.clip.length);
                    }
                }
            }

            return time;
        }

        /// <summary>
        /// 获取指定游戏对象当前所在的层级路径
        /// </summary>
        /// <param name="self">游戏对象实例</param>
        /// <returns>返回游戏对象当前所在的层级路径</returns>
        public static string GetHierarchyPath(this UnityGameObject self)
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            IList<string> list = new List<string>();

            UnityTransform transform = self.transform;
            // 遍历搜索上层节点
            while (true)
            {
                list.Add(transform.name);
                if (transform.parent != null)
                {
                    transform = transform.parent;
                }
                else
                {
                    break;
                }
            }

            // 反向添加路径信息
            for (int n = list.Count - 1; n >= 0; --n)
            {
                sb.Append(list[n]);
                if (n > 0)
                {
                    sb.Append(Definition.CCharacter.Slash);
                }
            }

            return sb.ToString();
        }

        #region GameObject对其组件访问操作相关的接口函数

        /// <summary>
        /// 检查当前GameObject节点中是否有指定类型的组件实例
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="self">源节点对象实例</param>
        /// <returns>若存在给定类型的组件实例则返回true，否则返回false</returns>
        public static bool HasComponent<T>(this UnityGameObject self) where T : UnityComponent
        {
            return (null != self.GetComponent<T>());
        }

        /// <summary>
        /// 检查当前GameObject节点中是否有指定类型的组件实例
        /// </summary>
        /// <param name="self">源节点对象实例</param>
        /// <param name="type">组件类型</param>
        /// <returns>若存在给定类型的组件实例则返回true，否则返回false</returns>
        public static bool HasComponent(this UnityGameObject self, string type)
        {
            return (null != self.GetComponent(type));
        }

        /// <summary>
        /// 检查当前GameObject节点中是否有指定类型的组件实例
        /// </summary>
        /// <param name="self">源节点对象实例</param>
        /// <param name="type">组件类型</param>
        /// <returns>若存在给定类型的组件实例则返回true，否则返回false</returns>
        public static bool HasComponent(this UnityGameObject self, SystemType type)
        {
            return (null != self.GetComponent(type));
        }

        /// <summary>
        /// 在当前的GameObject中查找指定类型的组件对象实例<br/>
        /// 若目标类型的组件不存在，则新创建一个该类型的实例并返回
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="self">源节点对象实例</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static T GetOrAddComponent<T>(this UnityGameObject self) where T : UnityComponent
        {
            T component = self.GetComponent<T>();
            if (null == component)
            {
                component = self.AddComponent<T>();
            }
            return component;
        }

        /// <summary>
        /// 在当前的GameObject中查找指定类型的组件对象实例<br/>
        /// 若目标类型的组件不存在，则新创建一个该类型的实例并返回
        /// </summary>
        /// <param name="self">源节点对象实例</param>
        /// <param name="type">目标组件类型</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static UnityComponent GetOrAddComponent(this UnityGameObject self, SystemType type)
        {
            UnityComponent component = self.GetComponent(type);
            if (null == component)
            {
                component = self.AddComponent(type);
            }
            return component;
        }

        /// <summary>
        /// 从父节点下指定名称的GameObject中查找指定类型的组件对象实例
        /// </summary>
        /// <typeparam name="T">目标组件类型</typeparam>
        /// <param name="self">源节点对象实例</param>
        /// <param name="name">目标节点名称</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static T GetComponentInParent<T>(this UnityGameObject self, string name) where T : UnityComponent
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
        /// <param name="self">源节点对象实例</param>
        /// <param name="name">目标节点名称</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static T GetOrAddComponentInParent<T>(this UnityGameObject self, string name) where T : UnityComponent
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
        /// <param name="self">源节点对象实例</param>
        /// <param name="name">目标节点名称</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static T GetComponentInChildren<T>(this UnityGameObject self, string name) where T : UnityComponent
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
        /// <param name="self">源节点对象实例</param>
        /// <param name="name">目标节点名称</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static T GetOrAddComponentInChildren<T>(this UnityGameObject self, string name) where T : UnityComponent
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
        /// <param name="self">源节点对象实例</param>
        /// <param name="name">目标节点名称</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static T GetComponentInPeer<T>(this UnityGameObject self, string name) where T : UnityComponent
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
        /// <param name="self">源节点对象实例</param>
        /// <param name="name">目标节点名称</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static T GetOrAddComponentInPeer<T>(this UnityGameObject self, string name) where T : UnityComponent
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
        /// <param name="self">源节点对象实例</param>
        /// <param name="includeSrc">是否包含源节点对象</param>
        /// <returns>返回对应类型的组件对象实例的集合</returns>
        public static T[] GetComponentsInPeer<T>(this UnityGameObject self, bool includeSrc = false) where T : UnityComponent
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
        /// <param name="self">源节点对象实例</param>
        /// <returns>返回源节点对象实例</returns>
        public static UnityGameObject TryRemoveComponent<T>(this UnityGameObject self) where T : UnityComponent
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
        /// <param name="self">源节点对象实例</param>
        /// <param name="type">目标组件类型</param>
        /// <returns>返回源节点对象实例</returns>
        public static UnityGameObject TryRemoveComponent(this UnityGameObject self, SystemType type)
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
        /// <param name="self">源节点对象实例</param>
        /// <param name="type">目标组件类型</param>
        /// <returns>返回源节点对象实例</returns>
        public static UnityGameObject TryRemoveComponent(this UnityGameObject self, string type)
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
        /// <param name="self">源节点对象实例</param>
        /// <returns>返回源节点对象实例</returns>
        public static UnityGameObject TryRemoveComponents<T>(this UnityGameObject self) where T : UnityComponent
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
        /// <param name="self">源节点对象实例</param>
        /// <param name="type">目标组件类型</param>
        /// <returns>返回源节点对象实例</returns>
        public static UnityGameObject TryRemoveComponents<T>(this UnityGameObject self, SystemType type)
        {
            UnityComponent[] comps = self.GetComponents(type);

            for (var n = 0; n < comps.Length; ++n)
            {
                UnityObject.Destroy(comps[n]);
            }

            return self;
        }

        #endregion

        #region GameObject对其关联的节点访问操作相关的接口函数

        /// <summary>
        /// 在当前的GameObject中查找指定名称的节点对象实例<br/>
        /// 若目标名称的节点对象不存在，则新创建一个该类型的实例并返回
        /// </summary>
        /// <param name="self">源节点对象实例</param>
        /// <param name="name">节点名称</param>
        /// <returns>返回给定名称的节点对象实例</returns>
        public static UnityGameObject FindOrCreateGameObject(this UnityGameObject self, string name)
        {
            UnityTransform trans = self.transform.Find(name);
            if (null == trans)
            {
                UnityGameObject go = new UnityGameObject(name).SetParent(self);
                return go;
            }
            else
            {
                return trans.gameObject;
            }
        }

        /// <summary>
        /// 在当前的GameObject中查找指定名称的节点对象实例<br/>
        /// 若目标名称的节点对象不存在，则新创建一个该类型的实例，并初始化相应的组件列表
        /// </summary>
        /// <param name="self">源节点对象实例</param>
        /// <param name="name">节点名称</param>
        /// <param name="componentTypes">组件类型列表</param>
        /// <returns>返回给定名称的节点对象实例</returns>
        public static UnityGameObject FindOrCreateGameObject(this UnityGameObject self, string name, params SystemType[] componentTypes)
        {
            UnityTransform trans = self.transform.Find(name);
            if (null == trans)
            {
                UnityGameObject go = new UnityGameObject(name, componentTypes).SetParent(self);
                return go;
            }
            else
            {
                return trans.gameObject;
            }
        }

        /// <summary>
        /// 在当前的GameObject节点下创建一个指定名称的节点对象实例
        /// </summary>
        /// <param name="self">源节点对象实例</param>
        /// <param name="name">节点名称</param>
        /// <returns>返回新创建的节点对象实例</returns>
        public static UnityGameObject CreateGameObject(this UnityGameObject self, string name)
        {
            UnityGameObject go = new UnityGameObject(name).SetParent(self);
            return go;
        }

        /// <summary>
        /// 在当前的GameObject节点下创建一个指定名称的节点对象实例，并初始化相应的组件列表
        /// </summary>
        /// <param name="self">源节点对象实例</param>
        /// <param name="name">节点名称</param>
        /// <param name="componentTypes">组件类型列表</param>
        /// <returns>返回新创建的节点对象实例</returns>
        public static UnityGameObject CreateGameObject(this UnityGameObject self, string name, params SystemType[] componentTypes)
        {
            UnityGameObject go = new UnityGameObject(name, componentTypes).SetParent(self);
            return go;
        }

        /// <summary>
        /// 将指定节点添加到当前节点下作为子节点实例
        /// </summary>
        /// <param name="self">源节点对象实例</param>
        /// <param name="childGameObject">子节点对象实例</param>
        /// <returns>返回源节点对象实例</returns>
        public static UnityGameObject AddChild(this UnityGameObject self, UnityGameObject childGameObject)
        {
            childGameObject.SetParent(self);
            return self;
        }

        /// <summary>
        /// 将指定的节点对象设置为当前节点对象的父节点
        /// </summary>
        /// <param name="self">源节点对象实例</param>
        /// <param name="parentGameObject">父节点对象实例</param>
        /// <returns>返回源节点对象实例</returns>
        public static UnityGameObject SetParent(this UnityGameObject self, UnityGameObject parentGameObject)
        {
            self.transform.SetParent(parentGameObject.transform);
            return self;
        }

        /// <summary>
        /// 将指定的 <see cref="UnityEngine.Transform"/> 设置为当前节点对象的父节点
        /// </summary>
        /// <param name="self">源节点对象实例</param>
        /// <param name="parent">父对象实例</param>
        /// <returns>返回源节点对象实例</returns>
        public static UnityGameObject SetParent(this UnityGameObject self, UnityTransform parent)
        {
            self.transform.SetParent(parent);
            return self;
        }

        #endregion
    }
}

/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

using System.Collections.Generic;
using SystemType = System.Type;
using SystemStringBuilder = System.Text.StringBuilder;

using UnityMathf = UnityEngine.Mathf;
using UnityGameObject = UnityEngine.GameObject;
using UnityComponent = UnityEngine.Component;
using UnityTransform = UnityEngine.Transform;
using UnityLayerMask = UnityEngine.LayerMask;
using UnityAnimator = UnityEngine.Animator;
using UnityAnimatorClipInfo = UnityEngine.AnimatorClipInfo;
using UnityAnimation = UnityEngine.Animation;
using UnityAnimationClip = UnityEngine.AnimationClip;
using UnityParticleSystem = UnityEngine.ParticleSystem;
using UnityPlayableDirector = UnityEngine.Playables.PlayableDirector;

namespace NovaEngine
{
    /// <summary>
    /// 基于Unity库GameObject的扩展接口支持类
    /// </summary>
    public static class __GameObject
    {
        /// <summary>
        /// 给当前的实例对象添加一个限定唯一的组件实例，若实例对象中已存在相同类型的组件，则跳过添加操作
        /// </summary>
        /// <typeparam name="T">新增组件类型</typeparam>
        /// <param name="self">目标对象实例</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static T AddUniqueComponent<T>(this UnityGameObject self) where T : UnityComponent
        {
            return self.AddUniqueComponent(typeof(T)) as T;
        }

        /// <summary>
        /// 给当前的实例对象添加一个限定唯一的组件实例，若实例对象中已存在相同类型的组件，则跳过添加操作
        /// </summary>
        /// <param name="self">目标对象实例</param>
        /// <param name="type">新增组件类型</param>
        /// <returns>返回对应类型的组件对象实例</returns>
        public static UnityComponent AddUniqueComponent(this UnityGameObject self, SystemType type)
        {
            UnityComponent component = self.GetComponent(type);
            if (component == null)
            {
                component = self.AddComponent(type);
            }

            return component;
        }

        public static T AddUniqueComponentInChildren<T>(this UnityGameObject self) where T : UnityComponent
        {
            T component = self.GetComponentInChildren<T>();
            if (null == component)
            {
                component = self.AddComponent<T>();
            }

            return component;
        }

        public static T FindComponentOnParent<T>(this UnityGameObject go, int depth = 5) where T : UnityComponent
        {
            if (depth <= 0)
            {
                return null;
            }

            UnityTransform transform = go.transform.parent;
            if (null == transform)
            {
                return null;
            }

            T component = transform.GetComponent<T>();
            if (null == component)
            {
                return transform.gameObject.FindComponentOnParent<T>(depth - 1);
            }

            return component;
        }

        /// <summary>
        /// 将指定节点添加到当前节点下作为子节点实例
        /// </summary>
        /// <param name="self">目标对象实例</param>
        /// <param name="child">子节点对象实例</param>
        public static void AddChild(this UnityGameObject self, UnityGameObject child)
        {
            UnityTransform pTransform = self.transform;
            UnityTransform cTransform = child.transform;
            cTransform.SetParent(pTransform);
        }

        /// <summary>
        /// 通过节点名称查找当前节点下的子节点实例
        /// </summary>
        /// <param name="self">查找起始节点</param>
        /// <param name="name">节点名称</param>
        /// <returns>若查找成功则返回对应的节点实例，否则返回null</returns>
        public static UnityGameObject FindChild(this UnityGameObject self, string name)
        {
            UnityTransform parent = self.transform;
            UnityTransform child = parent.Find(name);
            if (null != child)
            {
                return child.gameObject;
            }

            for (int n = 0; n < parent.childCount; n++)
            {
                child = parent.GetChild(n);

                self = FindChild(child.gameObject, name);
                if (null != self)
                {
                    return self;
                }
            }

            return null;
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
    }
}

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

using UnityGameObject = UnityEngine.GameObject;
using UnityAnimator = UnityEngine.Animator;
using UnityAnimation = UnityEngine.Animation;
using UnityAnimationClip = UnityEngine.AnimationClip;
using UnityRuntimeAnimatorController = UnityEngine.RuntimeAnimatorController;

namespace NovaEngine
{
    /// <summary>
    /// 基于Unity库动画类的扩展接口支持类
    /// </summary>
    public static class __Animator
    {
        /// <summary>
        /// 检测当前动画组件中是否存在指定名称对应的动画实例
        /// </summary>
        /// <param name="self">动画组件</param>
        /// <param name="animationName">动画实例名称</param>
        /// <returns>若存在给定名称的动画实例则返回true，否则返回false</returns>
        public static bool HasAnimatorState(this UnityAnimator self, string animationName)
        {
            if (self && false == string.IsNullOrEmpty(animationName) && self.runtimeAnimatorController != null)
            {
                UnityRuntimeAnimatorController ac = self.runtimeAnimatorController;
                UnityAnimationClip[] animationClips = ac.animationClips;
                if (animationClips != null && animationClips.Length > 0)
                {
                    for (int n = 0; n < animationClips.Length; ++n)
                    {
                        if (animationClips[n].name == animationName)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 获取当前动画组件的切换进度
        /// </summary>
        /// <param name="self">动画组件</param>
        /// <param name="layer">层级</param>
        /// <returns>返回动画切换进度信息</returns>
        public static float GetCrossFadeProgress(this UnityAnimator self, int layer = 0)
        {
            if (self.GetNextAnimatorStateInfo(layer).shortNameHash == 0)
            {
                return 1f;
            }

            return self.GetCurrentAnimatorStateInfo(layer).normalizedTime % 1f;
        }
    }
}

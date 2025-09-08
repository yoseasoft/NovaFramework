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

using UnityGameObject = UnityEngine.GameObject;
using UnityAudioSource = UnityEngine.AudioSource;

namespace NovaEngine
{
    /// <summary>
    /// 基于Unity库AudioSource的扩展接口支持类
    /// </summary>
    public static class __AudioSource
    {
        /// <summary>
        /// 重置当前音频源内部的成员属性
        /// </summary>
        /// <param name="self">音频源组件</param>
        public static void Reset(this UnityAudioSource self)
        {
            self.clip = null;
            self.mute = false;
            self.playOnAwake = true;
            self.loop = false;
            self.priority = 128;
            self.volume = 1;
            self.pitch = 1;
            self.panStereo = 0;
            self.spatialBlend = 0;
            self.reverbZoneMix = 1;
            self.dopplerLevel = 1;
            self.spread = 0;
            self.maxDistance = 500;
        }
    }
}

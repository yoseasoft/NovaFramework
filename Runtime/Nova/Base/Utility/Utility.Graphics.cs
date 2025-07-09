/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using UnityVector3 = UnityEngine.Vector3;
using UnityRect = UnityEngine.Rect;
using UnityColor = UnityEngine.Color;
using UnityTexture = UnityEngine.Texture;

namespace NovaEngine
{
    /// <summary>
    /// 实用函数集合工具类
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// 图形相关实用函数集合
        /// </summary>
        public static class Graphics
        {
            /// <summary>
            /// 在当前激活场景中绘制线段
            /// </summary>
            /// <param name="start">线段起始位置</param>
            /// <param name="end">线段结束位置</param>
            public static void DrawLine(UnityVector3 start, UnityVector3 end)
            {
                UnityEngine.Debug.DrawLine(start, end);
            }

            /// <summary>
            /// 在当前激活场景中绘制线段
            /// </summary>
            /// <param name="start">线段起始位置</param>
            /// <param name="end">线段结束位置</param>
            /// <param name="color">线段颜色</param>
            public static void DrawLine(UnityVector3 start, UnityVector3 end, UnityColor color)
            {
                UnityEngine.Debug.DrawLine(start, end, color);
            }

            public static void DrawTriangle()
            {
            }

            public static void DrawRectangle()
            {
            }

            public static void DrawSector()
            {
            }

            public static void DrawCircle()
            {
            }

            /// <summary>
            /// 在当前激活场景中绘制贴图
            /// </summary>
            /// <param name="screenRect">贴图绘制位置</param>
            /// <param name="texture">贴图纹理实例</param>
            public static void DrawTexture(UnityRect screenRect, UnityTexture texture)
            {
                UnityEngine.Graphics.DrawTexture(screenRect, texture);
            }
        }
    }
}

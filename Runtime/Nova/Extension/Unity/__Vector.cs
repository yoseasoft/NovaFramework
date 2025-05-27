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

using UnityMathf = UnityEngine.Mathf;
using UnityVector2 = UnityEngine.Vector2;
using UnityVector3 = UnityEngine.Vector3;
using UnityVector4 = UnityEngine.Vector4;

namespace NovaEngine
{
    /// <summary>
    /// 基于Unity库Vector的扩展接口支持类
    /// </summary>
    public static class __Vector
    {
        /// <summary>
        /// 获取指定向量的绝对值向量实例
        /// </summary>
        /// <param name="self">目标向量</param>
        /// <returns>返回给定向量的绝对值向量实例</returns>
        public static UnityVector2 Abs(this UnityVector2 self)
        {
            return new UnityVector2(UnityMathf.Abs(self.x), UnityMathf.Abs(self.y));
        }

        /// <summary>
        /// 获取指定向量的绝对值向量实例
        /// </summary>
        /// <param name="self">目标向量</param>
        /// <returns>返回给定向量的绝对值向量实例</returns>
        public static UnityVector3 Abs(this UnityVector3 self)
        {
            return new UnityVector3(UnityMathf.Abs(self.x), UnityMathf.Abs(self.y), UnityMathf.Abs(self.z));
        }

        /// <summary>
        /// 获取指定向量的绝对值向量实例
        /// </summary>
        /// <param name="self">目标向量</param>
        /// <returns>返回给定向量的绝对值向量实例</returns>
        public static UnityVector4 Abs(this UnityVector4 self)
        {
            return new UnityVector4(UnityMathf.Abs(self.x), UnityMathf.Abs(self.y), UnityMathf.Abs(self.z), UnityMathf.Abs(self.w));
        }

        /// <summary>
        /// 将三维向量转换为二维向量
        /// </summary>
        /// <param name="self">目标向量</param>
        /// <returns>返回转换后的二维向量实例</returns>
        public static UnityVector2 ToVector2(this UnityVector3 self)
        {
            return new UnityVector2(self.x, self.z);
        }

        /// <summary>
        /// 将二维向量转换为三维向量
        /// </summary>
        /// <param name="self">目标向量</param>
        /// <returns>返回转换后的三维向量实例</returns>
        public static UnityVector3 ToVector3(this UnityVector2 self)
        {
            return new UnityVector3(self.x, 0f, self.y);
        }

        /// <summary>
        /// 将二维向量转换为三维向量，并提供指定的Y值
        /// </summary>
        /// <param name="self">目标向量</param>
        /// <param name="y">向量Y值</param>
        /// <returns>返回转换后的三维向量实例</returns>
        public static UnityVector3 ToVector3(this UnityVector2 self, float y)
        {
            return new UnityVector3(self.x, y, self.y);
        }
    }
}

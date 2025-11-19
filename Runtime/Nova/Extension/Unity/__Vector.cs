/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
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

namespace UnityEngine.Customize.Extension
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
        public static Vector2 Abs(this Vector2 self)
        {
            return new Vector2(Mathf.Abs(self.x), Mathf.Abs(self.y));
        }

        /// <summary>
        /// 获取指定向量的绝对值向量实例
        /// </summary>
        /// <param name="self">目标向量</param>
        /// <returns>返回给定向量的绝对值向量实例</returns>
        public static Vector3 Abs(this Vector3 self)
        {
            return new Vector3(Mathf.Abs(self.x), Mathf.Abs(self.y), Mathf.Abs(self.z));
        }

        /// <summary>
        /// 获取指定向量的绝对值向量实例
        /// </summary>
        /// <param name="self">目标向量</param>
        /// <returns>返回给定向量的绝对值向量实例</returns>
        public static Vector4 Abs(this Vector4 self)
        {
            return new Vector4(Mathf.Abs(self.x), Mathf.Abs(self.y), Mathf.Abs(self.z), Mathf.Abs(self.w));
        }

        /// <summary>
        /// 将三维向量转换为二维向量
        /// </summary>
        /// <param name="self">目标向量</param>
        /// <returns>返回转换后的二维向量实例</returns>
        public static Vector2 ToVector2(this Vector3 self)
        {
            return new Vector2(self.x, self.z);
        }

        /// <summary>
        /// 将二维向量转换为三维向量
        /// </summary>
        /// <param name="self">目标向量</param>
        /// <returns>返回转换后的三维向量实例</returns>
        public static Vector3 ToVector3(this Vector2 self)
        {
            return new Vector3(self.x, 0f, self.y);
        }

        /// <summary>
        /// 将二维向量转换为三维向量，并提供指定的Y值
        /// </summary>
        /// <param name="self">目标向量</param>
        /// <param name="y">向量Y值</param>
        /// <returns>返回转换后的三维向量实例</returns>
        public static Vector3 ToVector3(this Vector2 self, float y)
        {
            return new Vector3(self.x, y, self.y);
        }

        /// <summary>
        /// 将二维向量围绕指定轴心旋转一定的角度
        /// </summary>
        /// <param name="self">目标向量</param>
        /// <param name="angle">旋转角度</param>
        /// <param name="pivot">轴心位置</param>
        /// <returns>返回旋转后的二维向量实例</returns>
        public static Vector2 Rotate(this Vector2 self, float angle, Vector2 pivot = default(Vector2))
        {
            Vector2 rotated = Quaternion.Euler(new Vector3(0f, 0f, angle)) * (self - pivot);
            return rotated + pivot;
        }
    }
}

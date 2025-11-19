/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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
    /// 基于Unity库Transform的扩展接口支持类
    /// </summary>
    public static class __Transform
    {
        #region 位置、旋转属性操作的便捷函数接口定义

        /// <summary>
        /// 设置绝对位置x坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="x">位置x坐标值</param>
        public static void SetPositionX(this Transform self, float x)
        {
            Vector3 v = self.position;
            v.x = x;
            self.position = v;
        }

        /// <summary>
        /// 获取绝对位置x坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <returns>返回位置x坐标值</returns>
        public static float GetPositionX(this Transform self)
        {
            Vector3 v = self.position;
            return v.x;
        }

        /// <summary>
        /// 增加绝对位置x坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="x">位置x坐标值</param>
        public static void AddPositionX(this Transform self, float x)
        {
            Vector3 v = self.position;
            v.x += x;
            self.position = v;
        }

        /// <summary>
        /// 设置绝对位置y坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="y">位置y坐标值</param>
        public static void SetPositionY(this Transform self, float y)
        {
            Vector3 v = self.position;
            v.y = y;
            self.position = v;
        }

        /// <summary>
        /// 获取绝对位置y坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <returns>返回位置y坐标值</returns>
        public static float GetPositionY(this Transform self)
        {
            Vector3 v = self.position;
            return v.y;
        }

        /// <summary>
        /// 增加绝对位置y坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="y">位置y坐标值</param>
        public static void AddPositionY(this Transform self, float y)
        {
            Vector3 v = self.position;
            v.y += y;
            self.position = v;
        }

        /// <summary>
        /// 设置绝对位置z坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="z">位置z坐标值</param>
        public static void SetPositionZ(this Transform self, float z)
        {
            Vector3 v = self.position;
            v.z = z;
            self.position = v;
        }

        /// <summary>
        /// 获取绝对位置z坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <returns>返回位置z坐标值</returns>
        public static float GetPositionZ(this Transform self)
        {
            Vector3 v = self.position;
            return v.z;
        }

        /// <summary>
        /// 增加绝对位置z坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="z">位置z坐标值</param>
        public static void AddPositionZ(this Transform self, float z)
        {
            Vector3 v = self.position;
            v.z += z;
            self.position = v;
        }

        /// <summary>
        /// 设置相对位置x坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="x">位置x坐标值</param>
        public static void SetLocalPositionX(this Transform self, float x)
        {
            Vector3 v = self.localPosition;
            v.x = x;
            self.localPosition = v;
        }

        /// <summary>
        /// 获取相对位置x坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <returns>返回位置x坐标值</returns>
        public static float GetLocalPositionX(this Transform self)
        {
            Vector3 v = self.localPosition;
            return v.x;
        }

        /// <summary>
        /// 增加相对位置x坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="x">位置x坐标值</param>
        public static void AddLocalPositionX(this Transform self, float x)
        {
            Vector3 v = self.localPosition;
            v.x += x;
            self.localPosition = v;
        }

        /// <summary>
        /// 设置相对位置y坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="y">位置y坐标值</param>
        public static void SetLocalPositionY(this Transform self, float y)
        {
            Vector3 v = self.localPosition;
            v.y = y;
            self.localPosition = v;
        }

        /// <summary>
        /// 获取相对位置y坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <returns>返回位置y坐标值</returns>
        public static float GetLocalPositionY(this Transform self)
        {
            Vector3 v = self.localPosition;
            return v.y;
        }

        /// <summary>
        /// 增加相对位置y坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="y">位置y坐标值</param>
        public static void AddLocalPositionY(this Transform self, float y)
        {
            Vector3 v = self.localPosition;
            v.y += y;
            self.localPosition = v;
        }

        /// <summary>
        /// 设置相对位置z坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="z">位置z坐标值</param>
        public static void SetLocalPositionZ(this Transform self, float z)
        {
            Vector3 v = self.localPosition;
            v.z = z;
            self.localPosition = v;
        }

        /// <summary>
        /// 获取相对位置z坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <returns>返回位置z坐标值</returns>
        public static float GetLocalPositionZ(this Transform self)
        {
            Vector3 v = self.localPosition;
            return v.z;
        }

        /// <summary>
        /// 增加相对位置z坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="z">位置z坐标值</param>
        public static void AddLocalPositionZ(this Transform self, float z)
        {
            Vector3 v = self.localPosition;
            v.z += z;
            self.localPosition = v;
        }

        /// <summary>
        /// 设置相对尺寸x分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <param name="x">尺寸x分量值</param>
        public static void SetLocalScaleX(this Transform self, float x)
        {
            Vector3 v = self.localScale;
            v.x = x;
            self.localScale = v;
        }

        /// <summary>
        /// 获取相对尺寸x分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <returns>返回尺寸x分量值</returns>
        public static float GetLocalScaleX(this Transform self)
        {
            Vector3 v = self.localScale;
            return v.x;
        }

        /// <summary>
        /// 增加相对尺寸x分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <param name="x">尺寸x分量值</param>
        public static void AddLocalScaleX(this Transform self, float x)
        {
            Vector3 v = self.localScale;
            v.x += x;
            self.localScale = v;
        }

        /// <summary>
        /// 设置相对尺寸y分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <param name="y">尺寸y分量值</param>
        public static void SetLocalScaleY(this Transform self, float y)
        {
            Vector3 v = self.localScale;
            v.y = y;
            self.localScale = v;
        }

        /// <summary>
        /// 获取相对尺寸y分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <returns>返回尺寸y分量值</returns>
        public static float GetLocalScaleY(this Transform self)
        {
            Vector3 v = self.localScale;
            return v.y;
        }

        /// <summary>
        /// 增加相对尺寸y分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <param name="y">尺寸y分量值</param>
        public static void AddLocalScaleY(this Transform self, float y)
        {
            Vector3 v = self.localScale;
            v.y += y;
            self.localScale = v;
        }

        /// <summary>
        /// 设置相对尺寸z分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <param name="z">尺寸z分量值</param>
        public static void SetLocalScaleZ(this Transform self, float z)
        {
            Vector3 v = self.localScale;
            v.z = z;
            self.localScale = v;
        }

        /// <summary>
        /// 获取相对尺寸z分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <returns>返回尺寸z分量值</returns>
        public static float GetLocalScaleZ(this Transform self)
        {
            Vector3 v = self.localScale;
            return v.z;
        }

        /// <summary>
        /// 增加相对尺寸z分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <param name="z">尺寸z分量值</param>
        public static void AddLocalScaleZ(this Transform self, float z)
        {
            Vector3 v = self.localScale;
            v.z += z;
            self.localScale = v;
        }

        #endregion

        public static void ResetWorldTransform(this Transform self)
        {
            self.position = Vector3.zero;
            self.rotation = Quaternion.Euler(Vector3.zero);
            self.localScale = Vector3.one;
        }

        public static void ResetLocalTransform(this Transform self)
        {
            self.localPosition = Vector3.zero;
            self.localRotation = Quaternion.Euler(Vector3.zero);
            self.localScale = Vector3.one;
        }

        public static void ResetRectTransform(this RectTransform self)
        {
            self.localPosition = Vector3.zero;
            self.localRotation = Quaternion.Euler(Vector3.zero);
            self.localScale = Vector3.one;
            self.offsetMax = Vector2.zero;
            self.offsetMin = Vector2.zero;
        }

        /// <summary>
        /// 二维空间下使 <see cref="UnityEngine.Transform" /> 指向指向目标点的算法，使用世界坐标
        /// </summary>
        /// <param name="self"><see cref="UnityEngine.Transform" /> 对象</param>
        /// <param name="lookAtPoint2D">要朝向的二维坐标点</param>
        /// <remarks>假定其 forward 向量为 <see cref="Vector3.up" /></remarks>
        public static void LookAt2D(this Transform self, Vector2 lookAtPoint2D)
        {
            Vector3 vector = lookAtPoint2D.ToVector3() - self.position;
            vector.y = 0f;

            if (vector.magnitude > 0f)
            {
                self.rotation = Quaternion.LookRotation(vector.normalized, Vector3.up);
            }
        }
    }
}

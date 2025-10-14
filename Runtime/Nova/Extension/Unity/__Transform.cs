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

using UnityTransform = UnityEngine.Transform;
using UnityRectTransform = UnityEngine.RectTransform;
using UnityVector2 = UnityEngine.Vector2;
using UnityVector3 = UnityEngine.Vector3;
using UnityQuaternion = UnityEngine.Quaternion;

namespace NovaEngine
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
        public static void SetPositionX(this UnityTransform self, float x)
        {
            UnityVector3 v = self.position;
            v.x = x;
            self.position = v;
        }

        /// <summary>
        /// 获取绝对位置x坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <returns>返回位置x坐标值</returns>
        public static float GetPositionX(this UnityTransform self)
        {
            UnityVector3 v = self.position;
            return v.x;
        }

        /// <summary>
        /// 增加绝对位置x坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="x">位置x坐标值</param>
        public static void AddPositionX(this UnityTransform self, float x)
        {
            UnityVector3 v = self.position;
            v.x += x;
            self.position = v;
        }

        /// <summary>
        /// 设置绝对位置y坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="y">位置y坐标值</param>
        public static void SetPositionY(this UnityTransform self, float y)
        {
            UnityVector3 v = self.position;
            v.y = y;
            self.position = v;
        }

        /// <summary>
        /// 获取绝对位置y坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <returns>返回位置y坐标值</returns>
        public static float GetPositionY(this UnityTransform self)
        {
            UnityVector3 v = self.position;
            return v.y;
        }

        /// <summary>
        /// 增加绝对位置y坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="y">位置y坐标值</param>
        public static void AddPositionY(this UnityTransform self, float y)
        {
            UnityVector3 v = self.position;
            v.y += y;
            self.position = v;
        }

        /// <summary>
        /// 设置绝对位置z坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="z">位置z坐标值</param>
        public static void SetPositionZ(this UnityTransform self, float z)
        {
            UnityVector3 v = self.position;
            v.z = z;
            self.position = v;
        }

        /// <summary>
        /// 获取绝对位置z坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <returns>返回位置z坐标值</returns>
        public static float GetPositionZ(this UnityTransform self)
        {
            UnityVector3 v = self.position;
            return v.z;
        }

        /// <summary>
        /// 增加绝对位置z坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="z">位置z坐标值</param>
        public static void AddPositionZ(this UnityTransform self, float z)
        {
            UnityVector3 v = self.position;
            v.z += z;
            self.position = v;
        }

        /// <summary>
        /// 设置相对位置x坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="x">位置x坐标值</param>
        public static void SetLocalPositionX(this UnityTransform self, float x)
        {
            UnityVector3 v = self.localPosition;
            v.x = x;
            self.localPosition = v;
        }

        /// <summary>
        /// 获取相对位置x坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <returns>返回位置x坐标值</returns>
        public static float GetLocalPositionX(this UnityTransform self)
        {
            UnityVector3 v = self.localPosition;
            return v.x;
        }

        /// <summary>
        /// 增加相对位置x坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="x">位置x坐标值</param>
        public static void AddLocalPositionX(this UnityTransform self, float x)
        {
            UnityVector3 v = self.localPosition;
            v.x += x;
            self.localPosition = v;
        }

        /// <summary>
        /// 设置相对位置y坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="y">位置y坐标值</param>
        public static void SetLocalPositionY(this UnityTransform self, float y)
        {
            UnityVector3 v = self.localPosition;
            v.y = y;
            self.localPosition = v;
        }

        /// <summary>
        /// 获取相对位置y坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <returns>返回位置y坐标值</returns>
        public static float GetLocalPositionY(this UnityTransform self)
        {
            UnityVector3 v = self.localPosition;
            return v.y;
        }

        /// <summary>
        /// 增加相对位置y坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="y">位置y坐标值</param>
        public static void AddLocalPositionY(this UnityTransform self, float y)
        {
            UnityVector3 v = self.localPosition;
            v.y += y;
            self.localPosition = v;
        }

        /// <summary>
        /// 设置相对位置z坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="z">位置z坐标值</param>
        public static void SetLocalPositionZ(this UnityTransform self, float z)
        {
            UnityVector3 v = self.localPosition;
            v.z = z;
            self.localPosition = v;
        }

        /// <summary>
        /// 获取相对位置z坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <returns>返回位置z坐标值</returns>
        public static float GetLocalPositionZ(this UnityTransform self)
        {
            UnityVector3 v = self.localPosition;
            return v.z;
        }

        /// <summary>
        /// 增加相对位置z坐标
        /// </summary>
        /// <param name="self">位置对象实例</param>
        /// <param name="z">位置z坐标值</param>
        public static void AddLocalPositionZ(this UnityTransform self, float z)
        {
            UnityVector3 v = self.localPosition;
            v.z += z;
            self.localPosition = v;
        }

        /// <summary>
        /// 设置相对尺寸x分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <param name="x">尺寸x分量值</param>
        public static void SetLocalScaleX(this UnityTransform self, float x)
        {
            UnityVector3 v = self.localScale;
            v.x = x;
            self.localScale = v;
        }

        /// <summary>
        /// 获取相对尺寸x分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <returns>返回尺寸x分量值</returns>
        public static float GetLocalScaleX(this UnityTransform self)
        {
            UnityVector3 v = self.localScale;
            return v.x;
        }

        /// <summary>
        /// 增加相对尺寸x分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <param name="x">尺寸x分量值</param>
        public static void AddLocalScaleX(this UnityTransform self, float x)
        {
            UnityVector3 v = self.localScale;
            v.x += x;
            self.localScale = v;
        }

        /// <summary>
        /// 设置相对尺寸y分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <param name="y">尺寸y分量值</param>
        public static void SetLocalScaleY(this UnityTransform self, float y)
        {
            UnityVector3 v = self.localScale;
            v.y = y;
            self.localScale = v;
        }

        /// <summary>
        /// 获取相对尺寸y分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <returns>返回尺寸y分量值</returns>
        public static float GetLocalScaleY(this UnityTransform self)
        {
            UnityVector3 v = self.localScale;
            return v.y;
        }

        /// <summary>
        /// 增加相对尺寸y分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <param name="y">尺寸y分量值</param>
        public static void AddLocalScaleY(this UnityTransform self, float y)
        {
            UnityVector3 v = self.localScale;
            v.y += y;
            self.localScale = v;
        }

        /// <summary>
        /// 设置相对尺寸z分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <param name="z">尺寸z分量值</param>
        public static void SetLocalScaleZ(this UnityTransform self, float z)
        {
            UnityVector3 v = self.localScale;
            v.z = z;
            self.localScale = v;
        }

        /// <summary>
        /// 获取相对尺寸z分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <returns>返回尺寸z分量值</returns>
        public static float GetLocalScaleZ(this UnityTransform self)
        {
            UnityVector3 v = self.localScale;
            return v.z;
        }

        /// <summary>
        /// 增加相对尺寸z分量
        /// </summary>
        /// <param name="self">尺寸对象实例</param>
        /// <param name="z">尺寸z分量值</param>
        public static void AddLocalScaleZ(this UnityTransform self, float z)
        {
            UnityVector3 v = self.localScale;
            v.z += z;
            self.localScale = v;
        }

        #endregion

        public static void ResetWorldTransform(this UnityTransform self)
        {
            self.position = UnityVector3.zero;
            self.rotation = UnityQuaternion.Euler(UnityVector3.zero);
            self.localScale = UnityVector3.one;
        }

        public static void ResetLocalTransform(this UnityTransform self)
        {
            self.localPosition = UnityVector3.zero;
            self.localRotation = UnityQuaternion.Euler(UnityVector3.zero);
            self.localScale = UnityVector3.one;
        }

        public static void ResetRectTransform(this UnityRectTransform self)
        {
            self.localPosition = UnityVector3.zero;
            self.localRotation = UnityQuaternion.Euler(UnityVector3.zero);
            self.localScale = UnityVector3.one;
            self.offsetMax = UnityVector2.zero;
            self.offsetMin = UnityVector2.zero;
        }

        /// <summary>
        /// 二维空间下使 <see cref="Transform" /> 指向指向目标点的算法，使用世界坐标
        /// </summary>
        /// <param name="this"><see cref="Transform" /> 对象</param>
        /// <param name="lookAtPoint2D">要朝向的二维坐标点</param>
        /// <remarks>假定其 forward 向量为 <see cref="Vector3.up" /></remarks>
        public static void LookAt2D(this UnityTransform self, UnityVector2 lookAtPoint2D)
        {
            UnityVector3 vector = lookAtPoint2D.ToVector3() - self.position;
            vector.y = 0f;

            if (vector.magnitude > 0f)
            {
                self.rotation = UnityQuaternion.LookRotation(vector.normalized, UnityVector3.up);
            }
        }
    }
}

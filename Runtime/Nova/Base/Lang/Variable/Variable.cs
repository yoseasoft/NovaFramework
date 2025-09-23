/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

using SystemType = System.Type;

namespace NovaEngine
{
    /// <summary>
    /// 通用变量抽象类定义
    /// </summary>
    public abstract partial class Variable
    {
        /// <summary>
        /// 变量的新实例构建接口
        /// </summary>
        protected Variable()
        {
        }

        /// <summary>
        /// 获取变量类型
        /// </summary>
        public abstract SystemType Type
        {
            get;
        }

        /// <summary>
        /// 获取变量值
        /// </summary>
        /// <returns>变量值</returns>
        public abstract object GetValue();

        /// <summary>
        /// 设置变量值
        /// </summary>
        /// <param name="value">变量值</param>
        public abstract void SetValue(object value);

        /// <summary>
        /// 清理变量值
        /// </summary>
        public abstract void Clear();
    }

    #region 泛型通用变量类声明

    /// <summary>
    /// 通用泛型变量类定义
    /// </summary>
    public abstract class Variable<T> : Variable
    {
        private T _value;

        /// <summary>
        /// 泛型变量的新实例构建接口
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        protected Variable()
        {
            _value = default(T);
        }

        /// <summary>
        /// 泛型变量的新实例构建接口
        /// </summary>
        /// <param name="value">初始值</param>
        [UnityEngine.Scripting.Preserve]
        protected Variable(T value)
        {
            _value = value;
        }

        /// <summary>
        /// 获取变量类型
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public override System.Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// 获取或设置变量值
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// 获取变量值
        /// </summary>
        /// <returns>返回当前变量值</returns>
        [UnityEngine.Scripting.Preserve]
        public override object GetValue()
        {
            return _value;
        }

        /// <summary>
        /// 设置变量值
        /// </summary>
        /// <param name="value">变量值</param>
        [UnityEngine.Scripting.Preserve]
        public override void SetValue(object value)
        {
            _value = (T) value;
        }

        /// <summary>
        /// 重置变量值
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public override void Clear()
        {
            _value = default(T);
        }

        /// <summary>
        /// 获取变量字符串
        /// </summary>
        /// <returns>返回变量字符串</returns>
        [UnityEngine.Scripting.Preserve]
        public override string ToString()
        {
            return (null != _value) ? _value.ToString() : "<Null>";
        }
    }

    #endregion
}

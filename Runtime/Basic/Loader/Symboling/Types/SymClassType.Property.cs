/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System.Collections.Generic;
using System.Reflection;

using SystemType = System.Type;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// 通用对象属性的标记数据的结构信息
    /// </summary>
    public class SymProperty : SymBase
    {
        /// <summary>
        /// 属性的名称
        /// </summary>
        private string _propertyName;
        /// <summary>
        /// 属性的类型
        /// </summary>
        private SystemType _propertyType;
        /// <summary>
        /// 属性的获取接口函数
        /// </summary>
        private MethodInfo _getMethodInfo;
        /// <summary>
        /// 属性的设置接口函数
        /// </summary>
        private MethodInfo _setMethodInfo;
        /// <summary>
        /// 属性对象实例
        /// </summary>
        private PropertyInfo _propertyInfo;

        public PropertyInfo PropertyInfo
        {
            get { return _propertyInfo; }
            internal set
            {
                _propertyInfo = value;

                _propertyName = _propertyInfo.Name;
                _propertyType = _propertyInfo.PropertyType;

                _getMethodInfo = _propertyInfo.GetGetMethod();
                _setMethodInfo = _propertyInfo.GetSetMethod();
            }
        }

        public string PropertyName => _propertyName;
        public SystemType PropertyType => _propertyType;
        public MethodInfo GetMethodInfo => _getMethodInfo;
        public MethodInfo SetMethodInfo => _setMethodInfo;

        public SymProperty() : base() { }

        ~SymProperty()
        {
            _propertyInfo = null;
        }
    }
}

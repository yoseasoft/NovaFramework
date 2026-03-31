/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameEngine
{
    /// <summary>
    /// 基于组件对象的扩展对象类，该类区别于传统组件对象，是以数据通知为核心<br/>
    /// 它除了具备基础组件的所有能力之外，还支持属性数据变化后的通知回调流程<br/>
    /// <br/>
    /// 属性改变后自动触发通知的应用案例：
    /// <code>
    ///   private string _name;
    ///   public string Name
    ///   {
    ///     get { return _name; }
    ///     set { _name = value; OnPropertyChanged(); } // 使用CallerMemberName特性自动传递属性名
    ///   }
    /// </code>
    /// <br/>
    /// 或者使用装箱接口：
    /// <code>
    ///   public string Name
    ///   {
    ///     get => Get&lt;string&gt;();
    ///     set => Set(value);
    ///   }
    /// </code>
    /// </summary>
    public abstract partial class CDataComponent : CComponent, INotifyPropertyChanged
    {
        // 声明 PropertyChanged 事件
        public event PropertyChangedEventHandler PropertyChanged;

        // 触发 PropertyChanged 事件的方法
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly IDictionary<string, object> _propertyValues = new Dictionary<string, object>();

        protected T Get<T>([CallerMemberName] string propertyName = null)
        {
            if (_propertyValues.TryGetValue(propertyName, out object value))
                return (T) value;

            return default(T);
        }

        protected void Set<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(Get<T>(propertyName), value))
                return;

            _propertyValues[propertyName] = value;
            OnPropertyChanged(propertyName);
        }
    }
}

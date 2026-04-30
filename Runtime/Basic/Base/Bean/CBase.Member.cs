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

using System.Collections;
using System.Collections.Generic;
using System.Customize.Extension;
using System.Runtime.CompilerServices;

namespace GameEngine
{
    /// 基础对象抽象类
    public abstract partial class CBase // : System.ComponentModel.INotifyPropertyChanged
    {
        // 声明 PropertyChanged 事件
        // public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 基础对象的成员通知初始化函数接口
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMemberNotifyInitialize()
        {
        }

        /// <summary>
        /// 基础对象的成员通知清理函数接口
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMemberNotifyCleanup()
        {
        }

        /// <summary>
        /// 触发 PropertyChanged 事件的方法
        /// <br/>
        /// 当属性改变后自动触发通知的应用案例：
        /// <code>
        /// private string _name;
        /// public string Name
        /// {
        ///   get { return _name; }
        ///   set { _name = value; OnPropertyChanged(); } // 使用CallerMemberName特性自动传递属性名
        /// }
        /// </code>
        /// </summary>
        /// <param name="propertyName">变化属性名称</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));

            if (false == IsOnStartingStatus()) return;

            Debugger.IsNotNullOrEmpty(propertyName);

            OnPropertyChangedDispatch(propertyName);
        }

        /// <summary>
        /// 属性变化的分发调度接口函数
        /// </summary>
        /// <param name="propertyName">变化属性名称</param>
        internal abstract void OnPropertyChangedDispatch(string propertyName);

        /**
         * 使用方式：
         * public string Name { get => Get<string>(); set => Set(value); }
         * 
         * private readonly IDictionary<string, object> _propertyValues = new Dictionary<string, object>();
         * 
         * protected T Get<T>([CallerMemberName] string propertyName = null)
         * {
         *     if (_propertyValues.TryGetValue(propertyName, out object value))
         *         return (T) value;
         * 
         *     return default(T);
         * }
         * 
         * protected void Set<T>(T value, [CallerMemberName] string propertyName = null)
         * {
         *     if (Equals(Get<T>(propertyName), value))
         *         return;
         * 
         *     _propertyValues[propertyName] = value;
         *     OnPropertyChanged(propertyName);
         * }
         */
    }
}

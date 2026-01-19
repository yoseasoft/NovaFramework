/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine.Profiler.Statistics
{
    /// <summary>
    /// 业务框架统计模块对象的接口定义类
    /// 我们通过该对象对各个模块进行数据统计，方便我们进行程序优化
    /// </summary>
    public interface IStat
    {
        /// <summary>
        /// 统计模块功能接口注册绑定的声明属性类型定义
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        protected internal sealed class OnStatFunctionRegisterAttribute : Attribute
        {
            /// <summary>
            /// 统计模块的功能标识
            /// </summary>
            private readonly int _funcType;

            public int FuncType => _funcType;

            public OnStatFunctionRegisterAttribute(int funcType) : base()
            {
                _funcType = funcType;
            }
        }

        /// <summary>
        /// 获取统计模块的模块类型标识
        /// </summary>
        int StatType { get; }

        /// <summary>
        /// 引擎统计模块实例初始化接口
        /// </summary>
        // void Initialize();

        /// <summary>
        /// 引擎统计模块实例清理接口
        /// </summary>
        // void Cleanup();

        /// <summary>
        /// 引擎统计模块实例垃圾卸载接口
        /// </summary>
        // void Dump();

        /// <summary>
        /// 获取当前统计模块实例中指定标识对应的记录信息
        /// </summary>
        /// <param name="uid">统计信息标识</param>
        /// <returns>返回给定标识对应的统计项信息</returns>
        StatInfo GetStateInfoByUid(int uid);

        /// <summary>
        /// 获取当前统计模块实例记录的所有统计项信息
        /// </summary>
        /// <returns>返回所有记录的统计项信息</returns>
        IReadOnlyList<StatInfo> GetAllStatInfos();
    }
}

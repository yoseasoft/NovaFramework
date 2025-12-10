/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using SystemAction_object = System.Action<object>;

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 切面控制模块的编码结构信息对象类
    /// </summary>
    internal abstract class AspectCodeInfo : GeneralCodeInfo
    { }

    /// <summary>
    /// 切面调用模块的编码结构信息对象类
    /// </summary>
    internal sealed class AspectCallCodeInfo : AspectCodeInfo
    {
        /// <summary>
        /// 切面调用模块的函数结构信息管理容器
        /// </summary>
        private MethodTypeList<AspectCallMethodTypeCodeInfo> _methodTypes;

        internal MethodTypeList<AspectCallMethodTypeCodeInfo> MethodTypes => _methodTypes;

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        public void AddMethodType(AspectCallMethodTypeCodeInfo codeInfo)
        {
            if (null == _methodTypes)
            {
                _methodTypes = new MethodTypeList<AspectCallMethodTypeCodeInfo>();
            }

            _methodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllMethodTypes()
        {
            _methodTypes?.Clear();
            _methodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetMethodTypeCount()
        {
            return _methodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public AspectCallMethodTypeCodeInfo GetMethodType(int index)
        {
            return _methodTypes?.Get(index);
        }
    }

    /// <summary>
    /// 切面调用模块的函数结构信息
    /// </summary>
    internal sealed class AspectCallMethodTypeCodeInfo : MethodTypeCodeInfo
    {
        /// <summary>
        /// 切面调用模块的目标函数名称
        /// </summary>
        public string MethodName { get; internal set; }
        /// <summary>
        /// 切面调用模块的接入访问方式
        /// </summary>
        public AspectAccessType AccessType { get; internal set; }
        /// <summary>
        /// 切面调用模块的回调句柄实例
        /// </summary>
        public SystemAction_object Callback { get; internal set; }
    }
}

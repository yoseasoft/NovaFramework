/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hurley, Independent Studio.
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
using System.Runtime.Serialization;

namespace NovaEngine
{
    /// <summary>
    /// 引擎框架异常定义基础类
    /// </summary>
    [Serializable]
    public partial class CFrameworkException : Exception
    {
        /// <summary>
        /// 自定义异常类型的错误编码参数
        /// </summary>
        private readonly int _targetCode = 0;

        /// <summary>
        /// 错误码属性接口
        /// </summary>
        public int TargetCode => _targetCode;

        /// <summary>
        /// 引擎框架异常类的新实例构建接口
        /// </summary>
        public CFrameworkException() : base()
        {
        }

        /// <summary>
        /// 使用指定错误码构建引擎框架异常类的新实例
        /// </summary>
        /// <param name="targetCode">错误码标识</param>
        public CFrameworkException(int targetCode) : base()
        {
            _targetCode = targetCode;
        }

        /// <summary>
        /// 使用指定错误码和错误消息构建引擎框架异常类的新实例
        /// </summary>
        /// <param name="targetCode">错误码标识</param>
        /// <param name="message">描述错误的消息</param>
        public CFrameworkException(int targetCode, string message) : base(message)
        {
            _targetCode = targetCode;
        }

        /// <summary>
        /// 使用指定错误码和错误消息构建引擎框架异常类的新实例
        /// </summary>
        /// <param name="targetCode">错误码标识</param>
        /// <param name="format">错误消息的格式化内容</param>
        /// <param name="args">错误消息的格式化参数</param>
        public CFrameworkException(int targetCode, string format, params object[] args) : this(targetCode, Utility.Text.Format(format, args))
        {
        }

        /// <summary>
        /// 使用指定错误消息构建引擎框架异常类的新实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public CFrameworkException(string message) : base(message)
        {
            _targetCode = ErrorCode.UNKNOWN;
        }

        /// <summary>
        /// 使用指定错误消息构建引擎框架异常类的新实例
        /// </summary>
        /// <param name="format">错误消息的格式化内容</param>
        /// <param name="args">错误消息的格式化参数</param>
        public CFrameworkException(string format, params object[] args) : this(Utility.Text.Format(format, args))
        {
        }

        /// <summary>
        /// 使用对作为此异常原因的内部异常的引用来构建引擎框架异常类的新实例
        /// </summary>
        /// <param name="innerException">导致当前异常的异常，如果 innerException 参数不为空引用，则在处理内部异常的 catch 块中引发当前异常</param>
        public CFrameworkException(Exception innerException) : this(innerException.Message, innerException)
        {
        }

        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来构建引擎框架异常类的新实例
        /// </summary>
        /// <param name="message">解释异常原因的错误消息</param>
        /// <param name="innerException">导致当前异常的异常，如果 innerException 参数不为空引用，则在处理内部异常的 catch 块中引发当前异常</param>
        public CFrameworkException(string message, Exception innerException) : base(message, innerException)
        {
            Type targetType = innerException.GetType();

            _targetCode = ErrorCode.GetErrorCodeByExceptionType(targetType);
        }

        /// <summary>
        /// 使用指定异常类型来构建引擎框架异常类的新实例
        /// </summary>
        /// <param name="exceptionType">导致当前异常的异常类型，此类型必须来自于 System.Exception 的子类</param>
        public CFrameworkException(Type exceptionType) : this(ErrorCode.GetErrorCodeByExceptionType(exceptionType))
        {
        }

        /// <summary>
        /// 使用指定异常类型和错误消息来构建引擎框架异常类的新实例
        /// </summary>
        /// <param name="exceptionType">导致当前异常的异常类型，此类型必须来自于 System.Exception 的子类</param>
        /// <param name="message">解释异常原因的错误消息</param>
        public CFrameworkException(Type exceptionType, string message) : this(ErrorCode.GetErrorCodeByExceptionType(exceptionType), message)
        {
        }

        /// <summary>
        /// 使用指定异常类型和错误消息构建引擎框架异常类的新实例
        /// </summary>
        /// <param name="exceptionType">导致当前异常的异常类型，此类型必须来自于 System.Exception 的子类</param>
        /// <param name="format">错误消息的格式化内容</param>
        /// <param name="args">错误消息的格式化参数</param>
        public CFrameworkException(Type exceptionType, string format, params object[] args) : this(ErrorCode.GetErrorCodeByExceptionType(exceptionType), Utility.Text.Format(format, args))
        {
        }

        /// <summary>
        /// 用序列化数据构建引擎框架异常类的新实例
        /// </summary>
        /// <param name="info">存有有关所引发异常的序列化的对象数据</param>
        /// <param name="context">包含有关源或目标的上下文信息</param>
        protected CFrameworkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _targetCode = ErrorCode.UNKNOWN;
        }
    }
}

/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;
using SystemStringBuilder = System.Text.StringBuilder;

namespace NovaEngine
{
    /// <summary>
    /// 引擎框架异常定义基础类
    /// </summary>
    public partial class CFrameworkException : System.Exception
    {
        /// <summary>
        /// 异常类型对应错误编码的字典容器
        /// </summary>
        private static readonly IDictionary<int, SystemType> _ExceptionCode = new Dictionary<int, SystemType>();

        [UnityEngine.RuntimeInitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            RegErrorCodeForTargetException<System.SystemException>();
            RegErrorCodeForTargetException<System.ArgumentException>();
            RegErrorCodeForTargetException<System.ArgumentNullException>();
            RegErrorCodeForTargetException<System.ArgumentOutOfRangeException>();
            RegErrorCodeForTargetException<System.ArithmeticException>();
            RegErrorCodeForTargetException<System.ArrayTypeMismatchException>();
            RegErrorCodeForTargetException<System.DivideByZeroException>();
            RegErrorCodeForTargetException<System.DllNotFoundException>();
            RegErrorCodeForTargetException<System.FormatException>();
            RegErrorCodeForTargetException<System.IndexOutOfRangeException>();
            RegErrorCodeForTargetException<System.InvalidCastException>();
            RegErrorCodeForTargetException<System.InvalidOperationException>();
            RegErrorCodeForTargetException<System.FieldAccessException>();
            RegErrorCodeForTargetException<System.MissingFieldException>();
            RegErrorCodeForTargetException<System.MemberAccessException>();
            RegErrorCodeForTargetException<System.MissingMemberException>();
            RegErrorCodeForTargetException<System.MethodAccessException>();
            RegErrorCodeForTargetException<System.MissingMethodException>();
            RegErrorCodeForTargetException<System.NotImplementedException>();
            RegErrorCodeForTargetException<System.NotSupportedException>();
            RegErrorCodeForTargetException<System.NullReferenceException>();
            RegErrorCodeForTargetException<System.OutOfMemoryException>();
            RegErrorCodeForTargetException<System.StackOverflowException>();
            RegErrorCodeForTargetException<System.PlatformNotSupportedException>();
            RegErrorCodeForTargetException<System.IO.DriveNotFoundException>();
            RegErrorCodeForTargetException<System.IO.DirectoryNotFoundException>();
            RegErrorCodeForTargetException<System.IO.FileNotFoundException>();
            RegErrorCodeForTargetException<System.IO.PathTooLongException>();
            RegErrorCodeForTargetException<System.IO.FileLoadException>();
            RegErrorCodeForTargetException<System.IO.InvalidDataException>();
            RegErrorCodeForTargetException<System.IO.EndOfStreamException>();
        }

        /// <summary>
        /// 为目标异常类型注册其相应关联的错误码值
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        private static void RegErrorCodeForTargetException<T>()
        {
            RegErrorCodeForTargetException(typeof(T));
        }

        /// <summary>
        /// 为目标异常类型注册其相应关联的错误码值
        /// </summary>
        /// <param name="exceptionType">异常类型</param>
        private static void RegErrorCodeForTargetException(SystemType exceptionType)
        {
            int errorCode = ErrorCode.GetErrorCodeByExceptionType(exceptionType);
            Logger.Assert(errorCode > 0, "Invalid arguments.");

            if (_ExceptionCode.ContainsKey(errorCode))
            {
                Logger.Error("The error code {0} was already registed, repeat add will be override old type.", errorCode);
                _ExceptionCode.Remove(errorCode);
            }

            // Logger.Info("Reg: errorCode = {0}, exception = {1}", errorCode, exceptionType.Name);
            _ExceptionCode.Add(errorCode, exceptionType);
        }

        /// <summary>
        /// 获取传入的异常类型的描述字符串名称
        /// 获取方式是将类型名称提取出来，裁剪掉其“Exception”后缀
        /// 然后其它的每个单词单独提取出来转换为大写，单词之间用‘_’进行连接
        /// 最后拼接成该异常的描述字符串名称
        /// </summary>
        /// <param name="type">异常对象类型</param>
        /// <returns>返回异常对象类型的描述字符串名称，若传入类型非法，则返回null</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static string GetExceptionDescriptorName(SystemType type)
        {
            string name = type.Name;
            const string exception_tag = "Exception";
            if (name.Length > exception_tag.Length)
            {
                // 判断是否为“Exception”后缀
                if (name.Substring(name.Length - exception_tag.Length).Equals(exception_tag))
                {
                    // 裁剪掉“Exception”后缀
                    string name_prefix = name.Substring(0, name.Length - exception_tag.Length);

                    // 转换为大写加下划线的形式
                    return Utility.Reflection.ConvertMixedNamesToCapitalizeWithUnderlineNames(name_prefix);
                }
            }

            Logger.Error("The argument must be an exception type.");
            return null;
        }
    }
}

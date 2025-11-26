/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
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

using System.Reflection;

namespace NovaEngine
{
    /// <summary>
    /// 静态函数的抽象接口类，定义了函数的通用访问接口
    /// </summary>
    public class StaticMethod : IStaticMethod
    {
        /// <summary>
        /// 当前绑定的目标函数实例信息
        /// </summary>
        private readonly MethodInfo _methodInfo;

        /// <summary>
        /// 当前绑定函数的参数数量
        /// </summary>
        private readonly int _paramCount;

        /// <summary>
        /// 当前绑定函数的参数列表
        /// </summary>
        private readonly object[] _params;

        /// <summary>
        /// 静态函数对象的构造函数
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="typeName">对象类型名称</param>
        /// <param name="methodName">函数名称</param>
        public StaticMethod(Assembly assembly, string typeName, string methodName)
        {
            _methodInfo = assembly.GetType(typeName).GetMethod(methodName);
            _paramCount = _methodInfo.GetParameters().Length;
            _params = new object[_paramCount];
        }

        /// <summary>
        /// 无参的函数访问调用接口
        /// </summary>
        public virtual void Invoke()
        {
            Logger.Assert(0 == _paramCount, "Invalid parameters length.");

            _methodInfo.Invoke(null, _params);
        }

        /// <summary>
        /// 一个参数的函数访问调用接口
        /// </summary>
        /// <param name="arg1">参数1</param>
        public virtual void Invoke(object arg1)
        {
            Logger.Assert(1 == _paramCount, "Invalid parameters length.");

            _params[0] = arg1;
            _methodInfo.Invoke(null, _params);
        }

        /// <summary>
        /// 两个参数的函数访问调用接口
        /// </summary>
        /// <param name="arg1">参数1</param>
        /// <param name="arg2">参数2</param>
        public virtual void Invoke(object arg1, object arg2)
        {
            Logger.Assert(2 == _paramCount, "Invalid parameters length.");

            _params[0] = arg1;
            _params[1] = arg2;
            _methodInfo.Invoke(null, _params);
        }

        /// <summary>
        /// 三个参数的函数访问调用接口
        /// </summary>
        /// <param name="arg1">参数1</param>
        /// <param name="arg2">参数2</param>
        /// <param name="arg3">参数3</param>
        public virtual void Invoke(object arg1, object arg2, object arg3)
        {
            Logger.Assert(3 == _paramCount, "Invalid parameters length.");

            _params[0] = arg1;
            _params[1] = arg2;
            _params[2] = arg3;
            _methodInfo.Invoke(null, _params);
        }
    }
}

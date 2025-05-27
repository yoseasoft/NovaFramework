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

using SystemAssembly = System.Reflection.Assembly;
using SystemMethodInfo = System.Reflection.MethodInfo;

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
        private readonly SystemMethodInfo m_methodInfo;

        /// <summary>
        /// 当前绑定函数的参数数量
        /// </summary>
        private readonly int m_paramCount;

        /// <summary>
        /// 当前绑定函数的参数列表
        /// </summary>
        private readonly object[] m_params;

        /// <summary>
        /// 静态函数对象的构造函数
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="typeName">对象类型名称</param>
        /// <param name="methodName">函数名称</param>
        public StaticMethod(SystemAssembly assembly, string typeName, string methodName)
        {
            m_methodInfo = assembly.GetType(typeName).GetMethod(methodName);
            m_paramCount = m_methodInfo.GetParameters().Length;
            m_params = new object[m_paramCount];
        }

        /// <summary>
        /// 无参的函数访问调用接口
        /// </summary>
        public virtual void Invoke()
        {
            Logger.Assert(0 == m_paramCount, "Invalid parameters length.");

            m_methodInfo.Invoke(null, m_params);
        }

        /// <summary>
        /// 一个参数的函数访问调用接口
        /// </summary>
        /// <param name="arg1">参数1</param>
        public virtual void Invoke(object arg1)
        {
            Logger.Assert(1 == m_paramCount, "Invalid parameters length.");

            m_params[0] = arg1;
            m_methodInfo.Invoke(null, m_params);
        }

        /// <summary>
        /// 两个参数的函数访问调用接口
        /// </summary>
        /// <param name="arg1">参数1</param>
        /// <param name="arg2">参数2</param>
        public virtual void Invoke(object arg1, object arg2)
        {
            Logger.Assert(2 == m_paramCount, "Invalid parameters length.");

            m_params[0] = arg1;
            m_params[1] = arg2;
            m_methodInfo.Invoke(null, m_params);
        }

        /// <summary>
        /// 三个参数的函数访问调用接口
        /// </summary>
        /// <param name="arg1">参数1</param>
        /// <param name="arg2">参数2</param>
        /// <param name="arg3">参数3</param>
        public virtual void Invoke(object arg1, object arg2, object arg3)
        {
            Logger.Assert(3 == m_paramCount, "Invalid parameters length.");

            m_params[0] = arg1;
            m_params[1] = arg2;
            m_params[2] = arg3;
            m_methodInfo.Invoke(null, m_params);
        }
    }
}

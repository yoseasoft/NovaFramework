/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

namespace GameEngine
{
    // API接口管理工具类
    public static partial class GameApi
    {
        #region 网络模块相关的注册绑定接口

        /// <summary>
        /// 注册指定服务类型对应的消息解析器对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="serviceType">服务类型</param>
        /// <returns>若注册解析器对象实例成功则返回true，否则返回false</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool RegisterMessageTranslator<T>(int serviceType) where T : IMessageTranslator, new()
        {
            return NetworkHandler.Instance.RegMessageTranslator<T>(serviceType);
        }

        /// <summary>
        /// 注册指定服务类型对应的消息解析器对象实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="classType">对象类型</param>
        /// <returns>若注册解析器对象实例成功则返回true，否则返回false</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool RegisterMessageTranslator(int serviceType, SystemType classType)
        {
            return NetworkHandler.Instance.RegMessageTranslator(serviceType, classType);
        }

        /// <summary>
        /// 删除指定服务类型对应的消息解析器对象实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void UnregisterMessageTranslator(int serviceType)
        {
            NetworkHandler.Instance.UnregMessageTranslator(serviceType);
        }

        /// <summary>
        /// 设置网络消息的协议对象类型
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void SetMessageProtocolType<T>() where T : class
        {
            NetworkHandler.Instance.SetMessageProtocolType<T>();
        }

        /// <summary>
        /// 设置网络消息的协议对象类型
        /// </summary>
        /// <param name="classType">对象类型</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void SetMessageProtocolType(SystemType classType)
        {
            NetworkHandler.Instance.SetMessageProtocolType(classType);
        }

        #endregion
    }
}

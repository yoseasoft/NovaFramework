/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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

using System.Collections.Generic;

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 基础对象抽象类，对需要进行对象定义的场景提供一个通用的基类
    /// </summary>
    public abstract partial class CBase
    {
        /// <summary>
        /// 基础对象的转发回调初始化函数接口
        /// </summary>
        private void OnDispatchCallInitialize()
        {
            OnInputResponseCallInitialize();
            OnEventSubscribeCallInitialize();
            OnMessageListenerCallInitialize();
        }

        /// <summary>
        /// 基础对象的转发回调清理函数接口
        /// </summary>
        private void OnDispatchCallCleanup()
        {
            OnInputResponseCallCleanup();
            OnEventSubscribeCallCleanup();
            OnMessageListenerCallCleanup();
        }

        /// <summary>
        /// 处理所有包装类型回调信息数据
        /// </summary>
        /// <typeparam name="SourceType">注册类型</typeparam>
        /// <typeparam name="CallInfoType">回调信息类型</typeparam>
        /// <param name="container">回调信息数据容器</param>
        /// <param name="func">操作回调接口</param>
        private void OnAutomaticallyCallSyntaxInfoProcessHandler<SourceType>(IDictionary<SourceType, IDictionary<string, bool>> container,
                                                                             System.Action<string, SourceType> func)
        {
            IDictionary<SourceType, IList<string>> dict = new Dictionary<SourceType, IList<string>>();

            SystemType targetType = GetType();
            foreach (KeyValuePair<SourceType, IDictionary<string, bool>> kvp in container)
            {
                foreach (KeyValuePair<string, bool> call_kvp in kvp.Value)
                {
                    string fullname = call_kvp.Key;
                    if (call_kvp.Value) // is automatically type
                    {
                        if (false == dict.TryGetValue(kvp.Key, out IList<string> infos))
                        {
                            infos = new List<string>();
                            dict.Add(kvp.Key, infos);
                        }

                        if (infos.Contains(fullname))
                        {
                            Debugger.Warn("The call info was already exist with target type '{0}' and name '{1}', repeat added it will override old value.", targetType, fullname);
                            infos.Remove(fullname);
                        }

                        infos.Add(fullname);
                    }
                }
            }

            if (dict.Count > 0)
            {
                foreach (KeyValuePair<SourceType, IList<string>> kvp in dict)
                {
                    IList<string> infos = kvp.Value;
                    for (int n = 0; n < infos.Count; ++n)
                    {
                        func(infos[n], kvp.Key);
                    }
                }
            }
        }
    }
}

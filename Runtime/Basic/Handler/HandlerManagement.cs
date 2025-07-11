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

using System.Collections.Generic;

using SystemEnum = System.Enum;
using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 句柄对象的封装管理类，对已定义的所有句柄类进行统一的调度派发操作
    /// </summary>
    internal static partial class HandlerManagement
    {
        /// <summary>
        /// 加载器类的后缀名称常量定义
        /// </summary>
        private const string HANDLER_CLASS_SUFFIX_NAME = "Handler";

        /// <summary>
        /// 句柄定义类型的管理容器
        /// </summary>
        private static IList<SystemType> _handlerDeclaringTypes = null;
        /// <summary>
        /// 句柄对象类型的管理容器
        /// </summary>
        private static IDictionary<int, SystemType> _handlerClassTypes = null;
        /// <summary>
        /// 句柄对象实例的管理容器
        /// </summary>
        private static IDictionary<int, IHandler> _handlerRegisterObjects = null;
        /// <summary>
        /// 句柄对象实例的排序容器
        /// </summary>
        private static LinkedList<IHandler> _handlerSortingList = null;

        /// <summary>
        /// 句柄管理器启动操作函数
        /// </summary>
        public static void Startup()
        {
            string namespaceTag = typeof(HandlerManagement).Namespace;

            // 定义类型管理容器初始化
            _handlerDeclaringTypes = new List<SystemType>();
            // 对象类型管理容器初始化
            _handlerClassTypes = new Dictionary<int, SystemType>();
            // 实例管理容器初始化
            _handlerRegisterObjects = new Dictionary<int, IHandler>();
            // 排序容器初始化
            _handlerSortingList = new LinkedList<IHandler>();

            foreach (HandlerClassType enumValue in SystemEnum.GetValues(typeof(HandlerClassType)))
            {
                if (HandlerClassType.Default == enumValue || HandlerClassType.User == enumValue)
                {
                    continue;
                }

                string enumName = enumValue.ToString();
                // 检查句柄类型定义和模块的事件类型定义，在重名的情况下，是否值一致
                NovaEngine.ModuleObject.EEventType eventType = NovaEngine.Utility.Convertion.GetEnumFromString<NovaEngine.ModuleObject.EEventType>(enumName);
                if (NovaEngine.ModuleObject.EEventType.Default != eventType)
                {
                    if (System.Convert.ToInt32(enumValue) != System.Convert.ToInt32(eventType))
                    {
                        Debugger.Error("The handler class type '{0}' was not matched module event type '{1}', registed it failed.", enumName, eventType.ToString());
                        continue;
                    }
                }

                // 类名反射时需要包含命名空间前缀
                string handlerName = NovaEngine.Utility.Text.Format("{0}.{1}{2}", namespaceTag, enumName, HANDLER_CLASS_SUFFIX_NAME);

                SystemType handlerType = SystemType.GetType(handlerName);
                if (null == handlerType)
                {
                    Debugger.Info(LogGroupTag.Module, "Could not found any handler class with target name {0}.", handlerName);
                    continue;
                }

                if (false == typeof(IHandler).IsAssignableFrom(handlerType))
                {
                    Debugger.Warn("The handler type {0} must be inherited from 'IHandler' interface.", handlerName);
                    continue;
                }

                // 创建并初始化实例
                IHandler handler = System.Activator.CreateInstance(handlerType) as IHandler;
                if (null == handler || false == handler.Initialize())
                {
                    Debugger.Error("The handler type {0} create or init failed.", handlerName);
                    continue;
                }

                (handler as BaseHandler).HandlerType = System.Convert.ToInt32(enumValue);

                // Debugger.Info("Register new handler {0} to target class type {1}.", handlerName, handler.HandlerType);

                // 添加的管理容器
                _handlerDeclaringTypes.Add(handlerType);
                _handlerClassTypes.Add(handler.HandlerType, handlerType);
                _handlerRegisterObjects.Add(handler.HandlerType, handler);
                // 添加到排序列表
                _handlerSortingList.AddLast(handler);

                // 添加所有定义的句柄基类
                SystemType baseType = handlerType.BaseType;
                while (null != baseType)
                {
                    if (false == typeof(IHandler).IsAssignableFrom(baseType) || baseType.IsInterface)
                    {
                        break;
                    }

                    if (false == _handlerDeclaringTypes.Contains(baseType))
                    {
                        _handlerDeclaringTypes.Add(baseType);
                    }

                    baseType = baseType.BaseType;
                }
            }

            // 对所有的句柄实例进行排序
            SortingAllHandlers();

            // 添加句柄相关指令事件代理
            HandlerDispatchedCommandAgent commandAgent = new HandlerDispatchedCommandAgent();
            NovaEngine.ModuleController.AddCommandAgent(HandlerDispatchedCommandAgent.COMMAND_AGENT_NAME, commandAgent);

            // 初始化句柄的统计模块
            InitAllStatModules();
        }

        /// <summary>
        /// 句柄管理器关闭操作函数
        /// </summary>
        public static void Shutdown()
        {
            // 清理所有的句柄实例缓存
            CleanupAllHandlerCaches();

            // 清理句柄的统计模块
            CleanupAllStatModules();

            // 遍历执行清理函数
            // foreach (KeyValuePair<int, IHandler> pair in _handlerRegisterObjects.Reverse())
            IEnumerable<IHandler> enumerable = NovaEngine.Utility.Collection.Reverse<IHandler>(_handlerSortingList);
            foreach (IHandler handler in enumerable)
            {
                handler.Cleanup();
            }

            // 移除句柄相关指令事件代理
            NovaEngine.ModuleController.RemoveCommandAgent(HandlerDispatchedCommandAgent.COMMAND_AGENT_NAME);

            _handlerDeclaringTypes.Clear();
            _handlerDeclaringTypes = null;

            _handlerClassTypes.Clear();
            _handlerClassTypes = null;

            _handlerRegisterObjects.Clear();
            _handlerRegisterObjects = null;

            _handlerSortingList.Clear();
            _handlerSortingList = null;
        }

        /// <summary>
        /// 句柄管理器刷新操作函数
        /// </summary>
        public static void Update()
        {
            foreach (IHandler handler in _handlerSortingList)
            {
                handler.Update();
            }
        }

        /// <summary>
        /// 句柄管理器后置刷新操作函数
        /// </summary>
        public static void LateUpdate()
        {
            foreach (IHandler handler in _handlerSortingList)
            {
                handler.LateUpdate();
            }
        }

        /// <summary>
        /// 按优先级对模块列表进行排序
        /// </summary>
        private static void SortingAllHandlers()
        {
            LinkedList<IHandler> result = new LinkedList<IHandler>();

            LinkedListNode<IHandler> lln;
            foreach (IHandler handler in _handlerSortingList)
            {
                lln = result.First;
                while (true)
                {
                    if (null == lln)
                    {
                        result.AddLast(handler);
                        break;
                    }
                    // else if (handler.HandlerType <= lln.Value.HandlerType)
                    else if (GetHandlerPriorityWithClassType(handler.HandlerType) <= GetHandlerPriorityWithClassType(lln.Value.HandlerType))
                    {
                        result.AddBefore(lln, handler);
                        break;
                    }
                    else
                    {
                        lln = lln.Next;
                    }
                }
            }

            _handlerSortingList.Clear();
            lln = result.First;
            while (null != lln)
            {
                _handlerSortingList.AddLast(lln.Value);
                lln = lln.Next;
            }
        }

        /// <summary>
        /// 获取当前注册的全部有效的句柄类型
        /// </summary>
        /// <returns>返回句柄类型列表</returns>
        internal static IList<SystemType> GetAllHandlerTypes()
        {
            IList<SystemType> result = null;
            result = NovaEngine.Utility.Collection.ToListForValues<int, SystemType>(_handlerClassTypes);
            return result;
        }

        /// <summary>
        /// 获取当前注册的全部定义的句柄类型
        /// </summary>
        /// <returns>返回句柄类型列表</returns>
        internal static IList<SystemType> GetAllHandlerDeclaringTypes()
        {
            return _handlerDeclaringTypes;
        }

        /// <summary>
        /// 通过指定的标识获取其对应的句柄类型
        /// </summary>
        /// <param name="handlerType">句柄标识</param>
        /// <returns>返回给定标识对应的句柄类型，若不存在则返回null</returns>
        internal static SystemType GetHandlerType(int handlerType)
        {
            SystemType classType = null;
            if (false == _handlerClassTypes.TryGetValue(handlerType, out classType))
            {
                return null;
            }

            return classType;
        }

        /// <summary>
        /// 通过指定的标识获取对应的句柄对象实例
        /// </summary>
        /// <param name="handlerType">句柄标识</param>
        /// <returns>返回句柄对象实例，若查找失败则返回null</returns>
        public static IHandler GetHandler(int handlerType)
        {
            IHandler handler = null;
            if (false == _handlerRegisterObjects.TryGetValue(handlerType, out handler))
            {
                return null;
            }

            return handler;
        }

        /// <summary>
        /// 通过指定的类型获取对应的句柄对象实例
        /// </summary>
        /// <typeparam name="T">句柄类型</typeparam>
        /// <returns>返回句柄对象实例，若查找失败则返回null</returns>
        public static T GetHandler<T>() where T : IHandler
        {
            SystemType clsType = typeof(T);

            foreach (KeyValuePair<int, IHandler> pair in _handlerRegisterObjects)
            {
                if (clsType.IsAssignableFrom(pair.Value.GetType()))
                    return (T) pair.Value;
            }

            return default(T);
        }

        /// <summary>
        /// 句柄对象的模块事件转发回调接口
        /// </summary>
        /// <param name="e">模块事件参数</param>
        public static bool OnEventDispatch(NovaEngine.ModuleEventArgs e)
        {
            int eventType = e.ID;

            IHandler handler = GetHandler(eventType);
            if (null == handler)
            {
                return false;
            }

            // 无论事件是否能正确被处理，此处转发的返回结果均为true
            handler.OnEventDispatch(e);
            return true;
        }
    }
}

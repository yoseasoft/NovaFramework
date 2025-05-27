/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine
{
    /// <summary>
    /// 原型对象管理类，用于对场景上下文中的所有原型对象提供通用的访问操作接口
    /// </summary>
    public sealed partial class ProtoController : BaseController<ProtoController>
    {
        /// <summary>
        /// 原型对象函数调用前对应生命周期类型的缓存容器
        /// </summary>
        private IDictionary<string, CBase.LifecycleKeypointType> m_protoMethodCallBeforeLifecycleTypes = null;
        /// <summary>
        /// 原型对象函数调用后对应生命周期类型的缓存容器
        /// </summary>
        private IDictionary<string, CBase.LifecycleKeypointType> m_protoMethodCallAfterLifecycleTypes = null;

        /// <summary>
        /// 原型管理对象的查找操作初始化通知接口函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void OnProtoCachedInitialize()
        {
            // 初始化原型对象函数对应生命周期类型的管理容器
            m_protoMethodCallBeforeLifecycleTypes = new Dictionary<string, CBase.LifecycleKeypointType>();
            m_protoMethodCallAfterLifecycleTypes = new Dictionary<string, CBase.LifecycleKeypointType>();
        }

        /// <summary>
        /// 原型管理对象的查找操作清理通知接口函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void OnProtoCachedCleanup()
        {
            // 清理原型对象函数对应生命周期类型的管理容器
            m_protoMethodCallBeforeLifecycleTypes.Clear();
            m_protoMethodCallAfterLifecycleTypes.Clear();

            m_protoMethodCallBeforeLifecycleTypes = null;
            m_protoMethodCallAfterLifecycleTypes = null;
        }

        #region 原型对象函数对应生命周期的映射绑定及查询接口函数

        /// <summary>
        /// 通过指定的函数名称从缓存容器中查找对应的生命周期类型
        /// </summary>
        /// <param name="methodName">函数名称</param>
        /// <param name="isBefore">前置状态标识</param>
        /// <param name="type">生命周期类型</param>
        /// <returns>若查找类型成功返回true，否则返回false</returns>
        internal bool TryGetProtoMethodLifecycleType(string methodName, bool isBefore, out CBase.LifecycleKeypointType type)
        {
            if (isBefore)
            {
                if (m_protoMethodCallBeforeLifecycleTypes.TryGetValue(methodName, out type))
                {
                    return true;
                }
            }
            else
            {
                if (m_protoMethodCallAfterLifecycleTypes.TryGetValue(methodName, out type))
                {
                    return true;
                }
            }

            type = CBase.LifecycleKeypointType.Unknown;
            return false;
        }

        /// <summary>
        /// 新增指定函数名称和对应的生命周期类型
        /// </summary>
        /// <param name="methodName">函数名称</param>
        /// <param name="isBefore">前置状态标识</param>
        /// <param name="type">生命周期类型</param>
        internal void AddProtoMethodLifecycleType(string methodName, bool isBefore, CBase.LifecycleKeypointType type)
        {
            IDictionary<string, CBase.LifecycleKeypointType> container = m_protoMethodCallAfterLifecycleTypes;
            if (isBefore)
            {
                container = m_protoMethodCallBeforeLifecycleTypes;
            }

            if (container.ContainsKey(methodName))
            {
                Debugger.Warn("The method '{0}' was already exists for lifecycle type '{1}', repeated add it will be override old handler.", methodName, type);

                container.Remove(methodName);
            }

            container.Add(methodName, type);
        }

        /// <summary>
        /// 移除指定函数名称对应的生命周期类型
        /// </summary>
        /// <param name="methodName">函数名称</param>
        /// <param name="isBefore">前置状态标识</param>
        internal void RemoveProtoMethodLifecycleType(string methodName, bool isBefore)
        {
            IDictionary<string, CBase.LifecycleKeypointType> container = m_protoMethodCallAfterLifecycleTypes;
            if (isBefore)
            {
                container = m_protoMethodCallBeforeLifecycleTypes;
            }

            if (false == container.ContainsKey(methodName))
            {
                Debugger.Warn("Could not found any lifecycle type with target method '{0}', removed it failed.", methodName);
                return;
            }

            container.Remove(methodName);
        }

        #endregion
    }
}

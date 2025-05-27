/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

using SystemType = System.Type;

using UnityObject = UnityEngine.Object;
using UnityGameObject = UnityEngine.GameObject;
using UnityComponent = UnityEngine.Component;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

namespace NovaEngine
{
    /// <summary>
    /// 基础管理句柄的模块管理部分，对外提供全部模块组件对象的统一访问接口
    /// </summary>
    public partial class Facade
    {
        #region 场景对象相关组件操作接口

        /// <summary>
        /// 检测表现层是否存在启动控制器实例
        /// </summary>
        /// <returns>若当前表现层存在启动控制器实例则返回true，否则返回false</returns>
        public bool HasRootController()
        {
            if (null != AppEntry.RootController)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取当前表现层加载的启动控制器实例
        /// </summary>
        /// <returns>返回当前表现层加载的启动控制器实例，若不存在则返回null</returns>
        public UnityMonoBehaviour GetRootController()
        {
            return AppEntry.RootController;
        }

        /// <summary>
        /// 检测表现层是否存在根节点对象实例
        /// </summary>
        /// <returns>若当前表现层存在根节点对象实例则返回true，否则返回false</returns>
        public bool HasRootGameObject()
        {
            if (null != AppEntry.RootGameObject)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取当前表现层加载的根节点对象实例
        /// </summary>
        /// <returns>返回当前表现层加载的根节点对象实例，若不存在则返回null</returns>
        public UnityEngine.GameObject GetRootGameObject()
        {
            return AppEntry.RootGameObject;
        }

        /// <summary>
        /// 基于表现层的主容器对象上添加新组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>添加组件成功，则返回对应的组件实例对象</returns>
        public T AddComponent<T>() where T : UnityComponent
        {
            UnityGameObject rootGameObject = AppEntry.RootGameObject;
            Logger.Assert(rootGameObject);

            UnityComponent c = rootGameObject.AddComponent<T>();

            return (T) c;
        }

        /// <summary>
        /// 基于表现层的主容器对象上移除目标组件
        /// </summary>
        /// <param name="obj">目标组件对象</param>
        public void RemoveComponent(UnityObject obj)
        {
            UnityGameObject rootGameObject = AppEntry.RootGameObject;
            Logger.Assert(rootGameObject);

            UnityComponent.Destroy(obj);
        }

        #endregion
    }
}

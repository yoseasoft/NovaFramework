/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using UnityObject = UnityEngine.Object;
using UnityGameObject = UnityEngine.GameObject;
using UnityTransform = UnityEngine.Transform;
using UnityVector3 = UnityEngine.Vector3;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace NovaEngine
{
    /// <summary>
    /// 资源管理器，统一处理打包资源的加载读取，缓存释放等功能，为其提供操作接口
    /// </summary>
    public sealed partial class ResourceModule
    {
        /// <summary>
        /// UnityGameObject和实例化对象的对照表
        /// </summary>
        private IDictionary<UnityEngine.GameObject, AssetModule.InstantiateObject> m_gameObjectInstantiateMapping = new Dictionary<UnityEngine.GameObject, AssetModule.InstantiateObject>();

        /// <summary>
        /// 同步实例化对象
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        public UnityGameObject InstantiateObject(string url)
        {
            var instantiateObject = AssetModule.AssetManagement.InstantiateObject(url);
            UnityGameObject gameObject = instantiateObject.gameObject;
            if (null != gameObject)
            {
                m_gameObjectInstantiateMapping.Add(gameObject, instantiateObject);
            }
            return gameObject;
        }

        /// <summary>
        /// 异步实例化对象
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        /// <param name="completed">实例化完成回调</param>
        public AssetModule.InstantiateObject InstantiateObjectAsync(string url, System.Action<UnityGameObject> completed = null)
        {
            var instantiateObject = AssetModule.AssetManagement.InstantiateObjectAsync(url);
            instantiateObject.completed += insObject =>
            {
                UnityGameObject gameObject = insObject.gameObject;
                if (null != gameObject)
                {
                    m_gameObjectInstantiateMapping.Add(gameObject, insObject);
                }
                completed?.Invoke(gameObject);
            };
            return instantiateObject;
        }

        /// <summary>
        /// 卸载基础对象模型组件(加载完成或加载中都可以使用此接口销毁对象)
        /// </summary>
        public void DestroyObject(AssetModule.InstantiateObject instantiateObject)
        {
            UnityGameObject gameObject = instantiateObject.gameObject;
            if (null != gameObject)
            {
                DestroyObject(gameObject);
            }
            else
            {
                instantiateObject.Destroy();
            }
        }

        /// <summary>
        /// 销毁已加载的目标对象模型组件
        /// </summary>
        /// <param name="gameObject">对象模型组件实例</param>
        public void DestroyObject(UnityGameObject gameObject)
        {
            if (m_gameObjectInstantiateMapping.TryGetValue(gameObject, out var instantiateObject))
            {
                m_gameObjectInstantiateMapping.Remove(gameObject);
                instantiateObject.Destroy();
            }
            else
            {
                UnityObject.Destroy(gameObject);
            }
        }

        /// <summary>
        /// 创建指定名称对应视图场景下的对象组件实例
        /// </summary>
        /// <param objectName="">组件名称</param>
        /// <param name="sceneName">场景名称</param>
        /// <returns>若组件创建成功则返回该组件实例，否则返回null</returns>
        public UnityGameObject CreateGameObject(string objectName, string sceneName)
        {
            SceneRecordInfo scene_info = GetModule<SceneModule>().GetSceneRecordInfo(sceneName);
            if (null == scene_info || SceneRecordInfo.EStateType.Complete != scene_info.StateType)
            {
                Logger.Warn("目标场景对象实例‘{0}’尚未准备完成，在此场景上添加节点对象失败！", sceneName);
                return null;
            }

            UnityGameObject go = new UnityGameObject(objectName);
            UnitySceneManager.MoveGameObjectToScene(go, scene_info.Scene);
            return go;
        }
    }
}

/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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
    /// 游戏引擎对外提供的API服务统一接口调用封装类，用于对远程游戏业务提供底层API接口封装<br/>
    /// 外部可以通过统一使用该类提供的API进行逻辑处理，避免内部复杂的层次结构导致其它开发人员上手门槛过高<br/>
    /// 同时因为API封装类的存在，内部的具体实现类也可以更好的进行访问权限的控制管理<br/>
    /// 
    /// 该类基本涵盖了所有Handler及Controller提供的服务接口，若有缺失可联系开发人员进行补充
    /// </summary>
    public static partial class GameApi
    {
        /// <summary>
        /// 发送事件消息到事件管理器中等待派发
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数列表</param>
        public static void Send(int eventID, params object[] args)
        {
            EventController.Instance.Send(eventID, args);
        }

        /// <summary>
        /// 发送事件消息到事件管理器中等待派发
        /// </summary>
        /// <param name="arg">事件数据</param>
        public static void Send<T>(T arg) where T : struct
        {
            EventController.Instance.Send<T>(arg);
        }

        /// <summary>
        /// 发送事件消息到事件管理器中并立即处理掉它
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数列表</param>
        public static void Fire(int eventID, params object[] args)
        {
            EventController.Instance.Fire(eventID, args);
        }

        /// <summary>
        /// 发送事件消息到事件管理器中并立即处理掉它
        /// </summary>
        /// <param name="arg">事件数据</param>
        public static void Fire<T>(T arg) where T : struct
        {
            EventController.Instance.Fire<T>(arg);
        }

        #region 场景业务相关的服务接口函数

        /// <summary>
        /// 获取当前运行的场景实例
        /// </summary>
        /// <returns>返回当前运行的场景实例，若没有则返回null</returns>
        public static CScene GetCurrentScene()
        {
            return SceneHandler.Instance.GetCurrentScene();
        }

        /// <summary>
        /// 根据指定的场景名称替换当前的场景实例
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        public static void ReplaceScene(string sceneName)
        {
            SceneHandler.Instance.ReplaceScene(sceneName);
        }

        /// <summary>
        /// 根据指定的场景类型替换当前的场景实例
        /// </summary>
        /// <typeparam name="T">场景类型</typeparam>
        public static void ReplaceScene<T>() where T : CScene
        {
            SceneHandler.Instance.ReplaceScene<T>();
        }

        /// <summary>
        /// 根据指定的场景类型替换当前的场景实例
        /// </summary>
        /// <param name="sceneType">场景类型</param>
        public static void ReplaceScene(SystemType sceneType)
        {
            SceneHandler.Instance.ReplaceScene(sceneType);
        }

        /// <summary>
        /// 根据指定的场景名称改变当前的场景实例
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <returns>返回改变的目标场景实例，若切换场景失败返回null</returns>
        public static CScene ChangeScene(string sceneName)
        {
            return SceneHandler.Instance.ChangeScene(sceneName);
        }

        /// <summary>
        /// 根据指定的场景类型改变当前的场景实例
        /// </summary>
        /// <typeparam name="T">场景类型</typeparam>
        /// <returns>返回改变的目标场景实例，若切换场景失败返回null</returns>
        public static T ChangeScene<T>() where T : CScene
        {
            return SceneHandler.Instance.ChangeScene<T>();
        }

        /// <summary>
        /// 根据指定的场景类型改变当前的场景实例
        /// </summary>
        /// <param name="sceneType">场景类型</param>
        /// <returns>返回改变的目标场景实例，若切换场景失败返回null</returns>
        public static CScene ChangeScene(SystemType sceneType)
        {
            return SceneHandler.Instance.ChangeScene(sceneType);
        }

        #endregion

        #region 角色业务相关的服务接口函数

        /// <summary>
        /// 根据指定的角色名称动态创建一个对应的角色对象实例
        /// </summary>
        /// <param name="actorName">角色名称</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public static CActor CreateActor(string actorName)
        {
            return ActorHandler.Instance.CreateActor(actorName);
        }

        /// <summary>
        /// 根据指定的角色类型动态创建一个对应的角色对象实例
        /// </summary>
        /// <typeparam name="T">角色类型</typeparam>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public static T CreateActor<T>() where T : CActor
        {
            return ActorHandler.Instance.CreateActor<T>();
        }

        /// <summary>
        /// 根据指定的角色类型动态创建一个对应的角色对象实例
        /// </summary>
        /// <param name="actorType">角色类型</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public static CActor CreateActor(SystemType actorType)
        {
            return ActorHandler.Instance.CreateActor(actorType);
        }

        /// <summary>
        /// 销毁指定的角色对象实例
        /// </summary>
        /// <param name="actor">角色实例</param>
        public static void DestroyActor(CActor actor)
        {
            ActorHandler.Instance.DestroyActor(actor);
        }

        /// <summary>
        /// 销毁指定角色名称对应的所有角色对象实例
        /// </summary>
        /// <param name="actorName">角色名称</param>
        public static void DestroyActor(string actorName)
        {
            ActorHandler.Instance.DestroyActor(actorName);
        }

        /// <summary>
        /// 销毁指定角色类型对应的所有角色对象实例
        /// </summary>
        /// <typeparam name="T">角色类型</typeparam>
        public static void DestroyActor<T>() where T : CActor
        {
            ActorHandler.Instance.DestroyActor<T>();
        }

        /// <summary>
        /// 销毁指定角色类型对应的所有角色对象实例
        /// </summary>
        /// <param name="actorType">角色类型</param>
        public static void DestroyActor(SystemType actorType)
        {
            ActorHandler.Instance.DestroyActor(actorType);
        }

        #endregion

        #region 视图业务相关的服务接口函数

        /// <summary>
        /// 根据指定的视图名称动态创建一个对应的视图对象实例
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public static async Cysharp.Threading.Tasks.UniTask<CView> OpenUI(string viewName)
        {
            return await GuiHandler.Instance.OpenUI(viewName);
        }

        /// <summary>
        /// 根据指定的视图类型动态创建一个对应的视图对象实例
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public static async Cysharp.Threading.Tasks.UniTask<T> OpenUI<T>() where T : CView
        {
            return await GuiHandler.Instance.OpenUI<T>();
        }

        /// <summary>
        /// 根据指定的视图类型动态创建一个对应的视图对象实例
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public static async Cysharp.Threading.Tasks.UniTask<CView> OpenUI(SystemType viewType)
        {
            return await GuiHandler.Instance.OpenUI(viewType);
        }

        /// <summary>
        /// 判断指定名称的视图是否处于打开状态
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <returns>若视图处于打开状态则返回true，否则返回false</returns>
        public static bool HasUI(string viewName)
        {
            return GuiHandler.Instance.HasUI(viewName);
        }

        /// <summary>
        /// 判断指定类型的视图是否处于打开状态
        /// </summary>
        public static bool HasUI<T>() where T : CView
        {
            return GuiHandler.Instance.HasUI<T>();
        }

        /// <summary>
        /// 判断指定类型的视图是否处于打开状态
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>若视图处于打开状态则返回true，否则返回false</returns>
        public static bool HasUI(SystemType viewType)
        {
            return GuiHandler.Instance.HasUI(viewType);
        }

        /// <summary>
        /// 通过指定的视图名称获取对应的视图对象实例
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <returns>返回查找到的视图对象实例，若查找失败则返回null</returns>
        public static CView FindUI(string viewName)
        {
            return GuiHandler.Instance.FindUI(viewName);
        }

        /// <summary>
        /// 根据指定的视图类型获取对应的视图对象实例
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        /// <returns>返回查找到的视图对象实例，若查找失败则返回null</returns>
        public static T FindUI<T>() where T : CView
        {
            return GuiHandler.Instance.FindUI<T>();
        }

        /// <summary>
        /// 根据指定的视图类型查找对应的视图对象实例
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回查找到的视图对象实例，若查找失败则返回null</returns>
        public static CView FindUI(SystemType viewType)
        {
            return GuiHandler.Instance.FindUI(viewType);
        }

        /// <summary>
        /// 根据指定的视图类型获取对应的视图对象实例
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        /// <returns>返回查找到的视图对象实例，若查找失败则返回null</returns>
        public static async Cysharp.Threading.Tasks.UniTask<T> FindUIAsync<T>() where T : CView
        {
            return await GuiHandler.Instance.FindUIAsync<T>();
        }

        /// <summary>
        /// 通过指定的视图类型查找对应的视图对象实例
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回查找到的视图对象实例，若查找失败则返回null</returns>
        public static async Cysharp.Threading.Tasks.UniTask<CView> FindUIAsync(SystemType viewType)
        {
            return await GuiHandler.Instance.FindUIAsync(viewType);
        }

        /// <summary>
        /// 关闭指定的视图对象实例
        /// </summary>
        /// <param name="view">视图对象实例</param>
        public static void CloseUI(CView view)
        {
            GuiHandler.Instance.CloseUI(view); 
        }

        /// <summary>
        /// 关闭指定的视图名称对应的视图对象实例
        /// </summary>
        /// <param name="viewName">视图名称</param>
        public static void CloseUI(string viewName)
        {
            GuiHandler.Instance.CloseUI(viewName);
        }

        /// <summary>
        /// 关闭指定的视图类型对应的视图对象实例
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        public static void CloseUI<T>() where T : CView
        {
            GuiHandler.Instance.CloseUI<T>();
        }

        /// <summary>
        /// 关闭指定的视图类型对应的视图对象实例
        /// </summary>
        /// <param name="viewType">视图类型</param>
        public static void CloseUI(SystemType viewType)
        {
            GuiHandler.Instance.CloseUI(viewType);
        }

        /// <summary>
        /// 关闭当前环境下所有的视图对象实例
        /// </summary>
        public static void CloseAllUI()
        {
            GuiHandler.Instance.CloseAllUI();
        }

        #endregion
    }
}

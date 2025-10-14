/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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
    /// 游戏引擎对外提供的API服务统一接口调用封装类，用于对远程游戏业务提供底层API接口封装<br/>
    /// 外部可以通过统一使用该类提供的API进行逻辑处理，避免内部复杂的层次结构导致其它开发人员上手门槛过高<br/>
    /// 同时因为API封装类的存在，内部的具体实现类也可以更好的进行访问权限的控制管理<br/>
    /// 
    /// 该类几乎涵盖了Handler和Controller内部提供的所有服务接口，若有缺失可联系开发人员进行补充
    /// </summary>
    public static partial class GameApi
    {
        #region 输入响应相关的服务接口函数

        /// <summary>
        /// 通过模拟输入操作的方式发送自定义按键编码的接口函数
        /// </summary>
        /// <param name="inputCode">按键编码</param>
        /// <param name="operationType">按键操作类型</param>
        public static void OnSimulationInputOperation(int inputCode, int operationType)
        {
            InputHandler.Instance.OnSimulationInputOperation(inputCode, operationType);
        }

        /// <summary>
        /// 通过模拟输入操作的方式发送自定义数据的接口函数
        /// </summary>
        /// <param name="inputData">输入数据</param>
        public static void OnSimulationInputOperation(object inputData)
        {
            InputHandler.Instance.OnSimulationInputOperation(inputData);
        }

        #endregion

        #region 事件转发相关的服务接口函数

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

        #endregion

        #region 网络通知相关的服务接口函数

        /// <summary>
        /// 通过模拟的方式接收基于ProtoBuf协议构建的网络消息的接口函数
        /// </summary>
        /// <param name="message">消息对象实例</param>
        public static void OnSimulationReceiveMessageOfProtoBuf(ProtoBuf.Extension.IMessage message)
        {
            NetworkHandler.Instance.OnSimulationReceiveMessageOfProtoBuf(message);
        }

        #endregion

        #region 编程接口相关的服务接口函数

        /// <summary>
        /// 功能模块回调接口函数
        /// </summary>
        /// <param name="obj">目标对象实例</param>
        /// <param name="functionName">功能名称</param>
        /// <param name="args">调用参数</param>
        public static void CallFunction(IBean obj, string functionName, params object[] args)
        {
            ApiController.Instance.CallFunction(obj, functionName, args);
        }

        /// <summary>
        /// 执行指定序列标识对应的表达式对象实例
        /// 该表达式对象实例是通过配置数据构建的，仅在第一次调用是进行构建操作
        /// </summary>
        /// <param name="obj">原型对象实例</param>
        /// <param name="serial">序列标识</param>
        /// <param name="text">配置数据</param>
        public static void Exec(IBean obj, int serial, string text)
        {
            ApiController.Instance.Exec(obj, serial, text);
        }

        #endregion

        #region 状态切换相关的服务接口函数

        /// <summary>
        /// 使用指定的根节点构建一个状态机对象实例
        /// </summary>
        /// <param name="root">根节点实例</param>
        /// <returns>返回一个新构建的状态机对象实例</returns>
        public static HFSM.StateMachine BuildStateMachine(HFSM.State root)
        {
            return HfsmController.Instance.Build(root);
        }

        #endregion

        #region 针对“CObject”对象类型业务相关的服务接口函数

        /// <summary>
        /// 通过指定的对象名称从实例容器中获取对应的基础对象实例列表
        /// </summary>
        /// <param name="objectName">对象名称</param>
        /// <returns>返回基础对象实例列表，若检索失败则返回null</returns>
        public static IList<CObject> GetObject(string objectName)
        {
            return ObjectHandler.Instance.GetObject(objectName);
        }

        /// <summary>
        /// 通过指定的对象类型从实例容器中获取对应的基础对象实例列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回基础对象实例列表，若检索失败则返回null</returns>
        public static IList<T> GetObject<T>() where T : CObject
        {
            return ObjectHandler.Instance.GetObject<T>();
        }

        /// <summary>
        /// 通过指定的对象类型从实例容器中获取对应的基础对象实例列表
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>返回基础对象实例列表，若检索失败则返回null</returns>
        public static IList<CObject> GetObject(SystemType objectType)
        {
            return ObjectHandler.Instance.GetObject(objectType);
        }

        /// <summary>
        /// 获取当前已创建的全部基础对象实例
        /// </summary>
        /// <returns>返回已创建的全部基础对象实例</returns>
        public static IList<CObject> GetAllObjects()
        {
            return ObjectHandler.Instance.GetAllObjects();
        }

        /// <summary>
        /// 通过指定的对象名称动态创建一个对应的基础对象实例
        /// </summary>
        /// <param name="objectName">对象名称</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public static CObject CreateObject(string objectName)
        {
            return ObjectHandler.Instance.CreateObject(objectName);
        }

        /// <summary>
        /// 通过指定的对象类型动态创建一个对应的基础对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public static T CreateObject<T>() where T : CObject
        {
            return ObjectHandler.Instance.CreateObject<T>();
        }

        /// <summary>
        /// 通过指定的对象类型动态创建一个对应的基础对象实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>若动态创建实例成功返回其引用，否则返回null</returns>
        public static CObject CreateObject(SystemType objectType)
        {
            return ObjectHandler.Instance.CreateObject(objectType);
        }

        /// <summary>
        /// 从当前对象管理容器中销毁指定的基础对象实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        public static void DestroyObject(CObject obj)
        {
            ObjectHandler.Instance.DestroyObject(obj);
        }

        /// <summary>
        /// 从当前对象管理容器中销毁指定名称对应的所有基础对象实例
        /// </summary>
        /// <param name="objectName">对象名称</param>
        public static void DestroyObject(string objectName)
        {
            ObjectHandler.Instance.DestroyObject(objectName);
        }

        /// <summary>
        /// 从当前对象管理容器中销毁指定类型对应的所有基础对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        public static void DestroyObject<T>() where T : CObject
        {
            ObjectHandler.Instance.DestroyObject<T>();
        }

        /// <summary>
        /// 从当前对象管理容器中销毁指定类型对应的所有基础对象实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        public static void DestroyObject(SystemType objectType)
        {
            ObjectHandler.Instance.DestroyObject(objectType);
        }

        /// <summary>
        /// 从当前对象管理容器中销毁所有注册的基础对象实例
        /// </summary>
        public static void DestroyAllObjects()
        {
            ObjectHandler.Instance.DestroyAllObjects();
        }

        #endregion

        #region 针对“CScene”场景类型业务相关的服务接口函数

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

        #region 针对“CActor”角色类型业务相关的服务接口函数

        /// <summary>
        /// 通过指定的角色名称从实例容器中获取对应的角色对象实例列表
        /// </summary>
        /// <param name="actorName">对象名称</param>
        /// <returns>返回角色对象实例列表，若检索失败则返回null</returns>
        public static IList<CActor> GetActor(string actorName)
        {
            return ActorHandler.Instance.GetActor(actorName);
        }

        /// <summary>
        /// 通过指定的角色类型从实例容器中获取对应的角色对象实例列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回角色对象实例列表，若检索失败则返回null</returns>
        public static IList<T> GetActor<T>() where T : CActor
        {
            return ActorHandler.Instance.GetActor<T>();
        }

        /// <summary>
        /// 通过指定的角色类型从实例容器中获取对应的角色对象实例列表
        /// </summary>
        /// <param name="actorType">对象类型</param>
        /// <returns>返回角色对象实例列表，若检索失败则返回null</returns>
        public static IList<CActor> GetActor(SystemType actorType)
        {
            return ActorHandler.Instance.GetActor(actorType);
        }

        /// <summary>
        /// 获取当前已创建的全部角色对象实例
        /// </summary>
        /// <returns>返回已创建的全部角色对象实例</returns>
        public static IList<CActor> GetAllActors()
        {
            return ActorHandler.Instance.GetAllActors();
        }

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

        #region 针对“CView”视图类型业务相关的服务接口函数

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

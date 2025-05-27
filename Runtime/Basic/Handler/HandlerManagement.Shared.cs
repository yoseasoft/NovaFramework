/// -------------------------------------------------------------------------------
/// GameEngine Framework
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

namespace GameEngine
{
    /// <summary>
    /// 句柄对象的封装管理类，对已定义的所有句柄类进行统一的调度派发操作
    /// </summary>
    internal static partial class HandlerManagement
    {
        /// <summary>
        /// 定时器管理句柄对象实例
        /// </summary>
        private static TimerHandler m_timerHandler = null;
        /// <summary>
        /// 线程管理句柄对象实例
        /// </summary>
        private static ThreadHandler m_threadHandler = null;
        /// <summary>
        /// 任务管理句柄对象实例
        /// </summary>
        private static TaskHandler m_taskHandler = null;
        /// <summary>
        /// 网络管理句柄对象实例
        /// </summary>
        private static NetworkHandler m_networkHandler = null;
        /// <summary>
        /// 资源管理句柄对象实例
        /// </summary>
        private static ResourceHandler m_resourceHandler = null;
        /// <summary>
        /// 文件管理句柄对象实例
        /// </summary>
        private static FileHandler m_fileHandler = null;
        /// <summary>
        /// 输入管理句柄对象实例
        /// </summary>
        private static InputHandler m_inputHandler = null;
        /// <summary>
        /// 场景管理句柄对象实例
        /// </summary>
        private static SceneHandler m_sceneHandler = null;
        /// <summary>
        /// 角色管理句柄对象实例
        /// </summary>
        private static ActorHandler m_actorHandler = null;
        /// <summary>
        /// 用户界面管理句柄对象实例
        /// </summary>
        private static GuiHandler m_guiHandler = null;
        /// <summary>
        /// 音频管理句柄对象实例
        /// </summary>
        private static SoundHandler m_soundHandler = null;

        /// <summary>
        /// 定时器管理句柄对象实例的获取函数
        /// 该函数具备对象实例的缓存功能，当实例对象发生变更时，需要进行清理缓存的操作
        /// </summary>
        public static TimerHandler TimerHandler
        {
            get
            {
                if (null == m_timerHandler)
                {
                    m_timerHandler = GetHandler<TimerHandler>();
                }
                return m_timerHandler;
            }
        }

        /// <summary>
        /// 线程管理句柄对象实例的获取函数
        /// 该函数具备对象实例的缓存功能，当实例对象发生变更时，需要进行清理缓存的操作
        /// </summary>
        public static ThreadHandler ThreadHandler
        {
            get
            {
                if (null == m_threadHandler)
                {
                    m_threadHandler = GetHandler<ThreadHandler>();
                }
                return m_threadHandler;
            }
        }

        /// <summary>
        /// 任务管理句柄对象实例的获取函数
        /// 该函数具备对象实例的缓存功能，当实例对象发生变更时，需要进行清理缓存的操作
        /// </summary>
        public static TaskHandler TaskHandler
        {
            get
            {
                if (null == m_taskHandler)
                {
                    m_taskHandler = GetHandler<TaskHandler>();
                }
                return m_taskHandler;
            }
        }

        /// <summary>
        /// 网络管理句柄对象实例的获取函数
        /// 该函数具备对象实例的缓存功能，当实例对象发生变更时，需要进行清理缓存的操作
        /// </summary>
        public static NetworkHandler NetworkHandler
        {
            get
            {
                if (null == m_networkHandler)
                {
                    m_networkHandler = GetHandler<NetworkHandler>();
                }
                return m_networkHandler;
            }
        }

        /// <summary>
        /// 资源管理句柄对象实例的获取函数
        /// 该函数具备对象实例的缓存功能，当实例对象发生变更时，需要进行清理缓存的操作
        /// </summary>
        public static ResourceHandler ResourceHandler
        {
            get
            {
                if (null == m_resourceHandler)
                {
                    m_resourceHandler = GetHandler<ResourceHandler>();
                }
                return m_resourceHandler;
            }
        }

        /// <summary>
        /// 文件管理句柄对象实例的获取函数
        /// 该函数具备对象实例的缓存功能，当实例对象发生变更时，需要进行清理缓存的操作
        /// </summary>
        public static FileHandler FileHandler
        {
            get
            {
                if (null == m_fileHandler)
                {
                    m_fileHandler = GetHandler<FileHandler>();
                }
                return m_fileHandler;
            }
        }

        /// <summary>
        /// 输入管理句柄对象实例的获取函数
        /// 该函数具备对象实例的缓存功能，当实例对象发生变更时，需要进行清理缓存的操作
        /// </summary>
        public static InputHandler InputHandler
        {
            get
            {
                if (null == m_inputHandler)
                {
                    m_inputHandler = GetHandler<InputHandler>();
                }
                return m_inputHandler;
            }
        }

        /// <summary>
        /// 场景管理句柄对象实例的获取函数
        /// 该函数具备对象实例的缓存功能，当实例对象发生变更时，需要进行清理缓存的操作
        /// </summary>
        public static SceneHandler SceneHandler
        {
            get
            {
                if (null == m_sceneHandler)
                {
                    m_sceneHandler = GetHandler<SceneHandler>();
                }
                return m_sceneHandler;
            }
        }

        /// <summary>
        /// 角色管理句柄对象实例的获取函数
        /// 该函数具备对象实例的缓存功能，当实例对象发生变更时，需要进行清理缓存的操作
        /// </summary>
        public static ActorHandler ActorHandler
        {
            get
            {
                if (null == m_actorHandler)
                {
                    m_actorHandler = GetHandler<ActorHandler>();
                }
                return m_actorHandler;
            }
        }

        /// <summary>
        /// 用户界面管理句柄对象实例的获取函数
        /// 该函数具备对象实例的缓存功能，当实例对象发生变更时，需要进行清理缓存的操作
        /// </summary>
        public static GuiHandler GuiHandler
        {
            get
            {
                if (null == m_guiHandler)
                {
                    m_guiHandler = GetHandler<GuiHandler>();
                }
                return m_guiHandler;
            }
        }

        /// <summary>
        /// 音频管理句柄对象实例的获取函数
        /// 该函数具备对象实例的缓存功能，当实例对象发生变更时，需要进行清理缓存的操作
        /// </summary>
        public static SoundHandler SoundHandler
        {
            get
            {
                if (null == m_soundHandler)
                {
                    m_soundHandler = GetHandler<SoundHandler>();
                }
                return m_soundHandler;
            }
        }

        /// <summary>
        /// 清除当前管理容器中所有的管理句柄对象实例缓存，在每次句柄管理容器中的单例发生变更时，必须调用此接口
        /// </summary>
        private static void CleanupAllHandlerCaches()
        {
            m_timerHandler = null;
            m_threadHandler = null;
            m_taskHandler = null;
            m_networkHandler = null;
            m_resourceHandler = null;
            m_fileHandler = null;
            m_inputHandler = null;
            m_sceneHandler = null;
            m_actorHandler = null;
            m_guiHandler = null;
            m_soundHandler = null;
        }
    }
}

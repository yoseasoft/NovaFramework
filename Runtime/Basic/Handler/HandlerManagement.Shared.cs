/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hurley, Independent Studio.
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
        private static TimerHandler _timerHandler = null;
        /// <summary>
        /// 线程管理句柄对象实例
        /// </summary>
        private static ThreadHandler _threadHandler = null;
        /// <summary>
        /// 任务管理句柄对象实例
        /// </summary>
        private static TaskHandler _taskHandler = null;
        /// <summary>
        /// 网络管理句柄对象实例
        /// </summary>
        private static NetworkHandler _networkHandler = null;
        /// <summary>
        /// 输入管理句柄对象实例
        /// </summary>
        private static InputHandler _inputHandler = null;
        /// <summary>
        /// 资源管理句柄对象实例
        /// </summary>
        private static ResourceHandler _resourceHandler = null;
        /// <summary>
        /// 文件管理句柄对象实例
        /// </summary>
        private static FileHandler _fileHandler = null;
        /// <summary>
        /// 对象管理句柄对象实例
        /// </summary>
        private static ObjectHandler _objectHandler = null;
        /// <summary>
        /// 场景管理句柄对象实例
        /// </summary>
        private static SceneHandler _sceneHandler = null;
        /// <summary>
        /// 角色管理句柄对象实例
        /// </summary>
        private static ActorHandler _actorHandler = null;
        /// <summary>
        /// 用户界面管理句柄对象实例
        /// </summary>
        private static GuiHandler _guiHandler = null;
        /// <summary>
        /// 音频管理句柄对象实例
        /// </summary>
        private static SoundHandler _soundHandler = null;

        /// <summary>
        /// 定时器管理句柄对象实例的获取函数
        /// 该函数具备对象实例的缓存功能，当实例对象发生变更时，需要进行清理缓存的操作
        /// </summary>
        public static TimerHandler TimerHandler
        {
            get
            {
                if (null == _timerHandler)
                {
                    _timerHandler = GetHandler<TimerHandler>();
                }
                return _timerHandler;
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
                if (null == _threadHandler)
                {
                    _threadHandler = GetHandler<ThreadHandler>();
                }
                return _threadHandler;
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
                if (null == _taskHandler)
                {
                    _taskHandler = GetHandler<TaskHandler>();
                }
                return _taskHandler;
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
                if (null == _networkHandler)
                {
                    _networkHandler = GetHandler<NetworkHandler>();
                }
                return _networkHandler;
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
                if (null == _inputHandler)
                {
                    _inputHandler = GetHandler<InputHandler>();
                }
                return _inputHandler;
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
                if (null == _resourceHandler)
                {
                    _resourceHandler = GetHandler<ResourceHandler>();
                }
                return _resourceHandler;
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
                if (null == _fileHandler)
                {
                    _fileHandler = GetHandler<FileHandler>();
                }
                return _fileHandler;
            }
        }

        /// <summary>
        /// 对象管理句柄对象实例的获取函数
        /// 该函数具备对象实例的缓存功能，当实例对象发生变更时，需要进行清理缓存的操作
        /// </summary>
        public static ObjectHandler ObjectHandler
        {
            get
            {
                if (null == _objectHandler)
                {
                    _objectHandler = GetHandler<ObjectHandler>();
                }
                return _objectHandler;
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
                if (null == _sceneHandler)
                {
                    _sceneHandler = GetHandler<SceneHandler>();
                }
                return _sceneHandler;
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
                if (null == _actorHandler)
                {
                    _actorHandler = GetHandler<ActorHandler>();
                }
                return _actorHandler;
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
                if (null == _guiHandler)
                {
                    _guiHandler = GetHandler<GuiHandler>();
                }
                return _guiHandler;
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
                if (null == _soundHandler)
                {
                    _soundHandler = GetHandler<SoundHandler>();
                }
                return _soundHandler;
            }
        }

        /// <summary>
        /// 清除当前管理容器中所有的管理句柄对象实例缓存，在每次句柄管理容器中的单例发生变更时，必须调用此接口
        /// </summary>
        private static void CleanupAllHandlerCaches()
        {
            _timerHandler = null;
            _threadHandler = null;
            _taskHandler = null;
            _networkHandler = null;
            _inputHandler = null;
            _resourceHandler = null;
            _fileHandler = null;
            _objectHandler = null;
            _sceneHandler = null;
            _actorHandler = null;
            _guiHandler = null;
            _soundHandler = null;
        }
    }
}

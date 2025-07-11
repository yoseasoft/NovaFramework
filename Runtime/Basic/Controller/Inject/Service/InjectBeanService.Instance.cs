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

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 提供注入操作接口的服务类，对整个程序内部的对象实例提供注入操作的服务逻辑处理
    /// </summary>
    public static partial class InjectBeanService
    {
        /// <summary>
        /// 注入实体对象的信息管理容器
        /// </summary>
        private static IDictionary<SystemType, IDictionary<string, GeneralInstantiateGenerator>> _beanInstanceInfos = null;

        /// <summary>
        /// 实体对象实例的管理信息初始化接口函数
        /// </summary>
        [OnServiceProcessInitCallback]
        private static void InitBeanInstanceInfos()
        {
            _beanInstanceInfos = new Dictionary<SystemType, IDictionary<string, GeneralInstantiateGenerator>>();
        }

        /// <summary>
        /// 实体对象实例的管理信息清理接口函数
        /// </summary>
        [OnServiceProcessCleanupCallback]
        private static void CleanupBeanInstanceInfos()
        {
            _beanInstanceInfos.Clear();
            _beanInstanceInfos = null;
        }

        #region 对象实例的创建/销毁接口函数

        /// <summary>
        /// 通过指定的对象类型创建一个对象实例
        /// </summary>
        /// <param name="classType">对象类型</param>
        /// <returns>返回新创建的对象实例</returns>
        public static object CreateObjectInstance(SystemType classType)
        {
            if (typeof(CBean).IsAssignableFrom(classType))
            {
                return CreateBeanInstance(classType);
            }

            return System.Activator.CreateInstance(classType);
        }

        /// <summary>
        /// 通过指定的对象名称创建一个对象实例
        /// </summary>
        /// <param name="beanName">实体名称</param>
        /// <returns>返回新创建的对象实例</returns>
        public static CBean CreateBeanInstance(string beanName)
        {
            Loader.Symboling.Bean bean = Loader.CodeLoader.GetBeanClassByName(beanName);
            if (null == bean)
            {
                Debugger.Warn("Could not found any bean class struct with target name '{0}', create bean instance failed.", beanName);
                return null;
            }

            return CreateBeanInstance(bean);
        }

        /// <summary>
        /// 通过指定的对象类型创建一个对象实例
        /// </summary>
        /// <param name="classType">对象类型</param>
        /// <returns>返回新创建的对象实例</returns>
        public static CBean CreateBeanInstance(SystemType classType)
        {
            Loader.Symboling.SymClass symClass = Loader.CodeLoader.GetSymClassByType(classType);
            if (null == symClass)
            {
                Debugger.Warn("Could not found any bean class struct with target type '{0}', create bean instance failed.", NovaEngine.Utility.Text.ToString(classType));
                return null;
            }

            return CreateBeanInstance(symClass, symClass.DefaultBeanName);
        }

        /// <summary>
        /// 通过指定的标记对象和实体名称，创建一个实体对象实例
        /// </summary>
        /// <param name="symClass">标记对象</param>
        /// <param name="beanName">实体名称</param>
        /// <returns>返回新创建的实体对象实例，若创建失败返回null</returns>
        internal static CBean CreateBeanInstance(Loader.Symboling.SymClass symClass, string beanName)
        {
            Loader.Symboling.Bean bean = symClass.GetBean(beanName);

            return CreateBeanInstance(bean);
        }

        /// <summary>
        /// 通过指定的实体结构信息，创建一个实体对象实例
        /// </summary>
        /// <param name="bean">实体结构信息</param>
        /// <returns>返回新创建的实体对象实例，若创建失败返回null</returns>
        internal static CBean CreateBeanInstance(Loader.Symboling.Bean bean)
        {
            SystemType classType = bean.TargetClass.ClassType;
            string beanName = bean.BeanName;

            Debugger.Assert(classType.IsClass && false == string.IsNullOrEmpty(beanName), "Invalid arguments.");

            if (false == _beanInstanceInfos.TryGetValue(classType, out IDictionary<string, GeneralInstantiateGenerator> classInfos))
            {
                classInfos = new Dictionary<string, GeneralInstantiateGenerator>();
                _beanInstanceInfos.Add(classType, classInfos);
            }

            if (false == classInfos.TryGetValue(beanName, out GeneralInstantiateGenerator instanceInfo))
            {
                if (bean.Singleton)
                {
                    instanceInfo = new SingletonInstantiateGenerator(classType, beanName);
                }
                else
                {
                    instanceInfo = new MultipleInstantiateGenerator(classType, beanName);
                }

                classInfos.Add(beanName, instanceInfo);
            }

            return instanceInfo.Alloc();
        }

        /// <summary>
        /// 释放指定的对象实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        internal static void ReleaseObjectInstance(object obj)
        {
            SystemType classType = obj.GetType();

            if (typeof(CBean).IsAssignableFrom(classType))
            {
                ReleaseBeanInstance(obj as CBean);
            }

            // 普通对象类型不用做任何处理
            // else { obj = null; }
        }

        /// <summary>
        /// 释放指定的对象实例
        /// </summary>
        /// <param name="bean">对象实例</param>
        internal static void ReleaseBeanInstance(CBean bean)
        {
            if (string.IsNullOrEmpty(bean.BeanName))
            {
                Debugger.Warn("The bean object name was null or empty string, released it failed.");
                return;
            }

            SystemType beanType = bean.GetType();
            if (false == _beanInstanceInfos.TryGetValue(beanType, out IDictionary<string, GeneralInstantiateGenerator> classInfos))
            {
                Debugger.Warn("Could not found any bean instance info with target class '{0}', released it failed.", NovaEngine.Utility.Text.ToString(beanType));
                return;
            }

            if (false == classInfos.TryGetValue(bean.BeanName, out GeneralInstantiateGenerator instanceInfo))
            {
                Debugger.Warn("Could not found any bean instance info with target name '{0} - {1}', released it failed.", NovaEngine.Utility.Text.ToString(beanType), bean.BeanName);
                return;
            }

            instanceInfo.Release(bean);
        }

        #endregion

        #region 实例化生成器对象的封装类定义

        /// <summary>
        /// 用于管理通用类型对象实例化生成器类
        /// </summary>
        private abstract class GeneralInstantiateGenerator
        {
            /// <summary>
            /// 对象类型声明
            /// </summary>
            protected SystemType _classType = null;
            /// <summary>
            /// 对象实体名称
            /// </summary>
            protected string _beanName = null;

            /// <summary>
            /// 对象是否为单例模式的状态标识
            /// </summary>
            public abstract bool SingletonMode { get; }

            protected GeneralInstantiateGenerator(SystemType classType, string beanName)
            {
                Debugger.Assert(typeof(CBean).IsAssignableFrom(classType), "Invalid arguments.");

                _classType = classType;
                _beanName = beanName;
            }

            /// <summary>
            /// 分配对象实例
            /// </summary>
            /// <returns>返回分配的对象实例</returns>
            public abstract CBean Alloc();

            /// <summary>
            /// 释放对象实例
            /// </summary>
            public abstract void Release();

            /// <summary>
            /// 释放对象实例
            /// </summary>
            /// <param name="obj">目标对象实例</param>
            public abstract void Release(CBean obj);

            /// <summary>
            /// 通过当前对象的类型标识，创建一个新的对象实例
            /// </summary>
            /// <returns>返回新创建的对象实例</returns>
            protected CBean CreateInstance()
            {
                CBean obj = null;

                if (typeof(CScene).IsAssignableFrom(_classType))
                {
                    // obj = SceneHandler.Instance.CreateScene(m_classType);
                    throw new System.ArgumentException();
                }
                else if (typeof(CActor).IsAssignableFrom(_classType))
                {
                    obj = ActorHandler.Instance.CreateActor(_classType);
                }
                else
                {
                    obj = System.Activator.CreateInstance(_classType) as CBean;

                    AspectController.Instance.Call(obj.Initialize);
                }

                // 记录对象实例的映射名称
                obj.BeanName = _beanName;

                // 自动装配新创建的对象实例
                AutowiredProcessingOnCreateTargetObject(obj);

                return obj;
            }

            /// <summary>
            /// 销毁一个指定的目标对象实例
            /// </summary>
            /// <param name="obj">目标对象实例</param>
            protected void DestroyInstance(CBean obj)
            {
                Debugger.Assert(obj.GetType() == _classType, "Invalid arguments.");

                // 卸载待销毁的对象实例
                AutowiredProcessingOnReleaseTargetObject(obj);

                // 移除对象实例的映射名称
                obj.BeanName = null;

                if (typeof(CActor).IsAssignableFrom(_classType))
                {
                    ActorHandler.Instance.DestroyActor(obj as CActor);
                }
                else
                {
                    AspectController.Instance.Call(obj.Cleanup);
                }
            }
        }

        /// <summary>
        /// 多实例对象的实例化生成器类
        /// </summary>
        private sealed class MultipleInstantiateGenerator : GeneralInstantiateGenerator
        {
            /// <summary>
            /// 对象实例的管理容器
            /// </summary>
            private IList<CBean> _objects;

            /// <summary>
            /// 对象是否为单例模式的状态标识
            /// </summary>
            public override bool SingletonMode => false;

            public MultipleInstantiateGenerator(SystemType classType, string beanName) : base(classType, beanName)
            {
                _objects = new List<CBean>();
            }

            ~MultipleInstantiateGenerator()
            {
                Release();
                _objects = null;
            }

            /// <summary>
            /// 分配对象实例
            /// </summary>
            /// <returns>返回分配的对象实例</returns>
            public override CBean Alloc()
            {
                CBean obj = CreateInstance();
                Debugger.Assert(null != obj, "Invalid class type {0}.", NovaEngine.Utility.Text.ToString(_classType));

                _objects.Add(obj);

                return obj;
            }

            /// <summary>
            /// 释放对象实例
            /// </summary>
            public override void Release()
            {
                while (_objects.Count > 0)
                {
                    Release(_objects[0]);
                }

                _objects.Clear();
            }

            /// <summary>
            /// 释放对象实例
            /// </summary>
            /// <param name="obj">目标对象实例</param>
            public override void Release(CBean obj)
            {
                Debugger.Assert(null != obj, "Invalid arguments.");

                _objects.Remove(obj);
                DestroyInstance(obj);
            }
        }

        /// <summary>
        /// 单实例对象的实例化生成器类
        /// </summary>
        private sealed class SingletonInstantiateGenerator : GeneralInstantiateGenerator
        {
            /// <summary>
            /// 对象实例
            /// </summary>
            private CBean _instance = null;
            /// <summary>
            /// 对象实例的引用计数
            /// </summary>
            private int _referenceCount = 0;

            /// <summary>
            /// 对象是否为单例模式的状态标识
            /// </summary>
            public override bool SingletonMode => true;

            public SingletonInstantiateGenerator(SystemType classType, string beanName) : base(classType, beanName)
            {
                _instance = null;
                _referenceCount = 0;
            }

            ~SingletonInstantiateGenerator()
            {
                Release();
            }

            /// <summary>
            /// 分配对象实例
            /// </summary>
            /// <returns>返回分配的对象实例</returns>
            public override CBean Alloc()
            {
                if (null == _instance)
                {
                    _instance = CreateInstance();
                    Debugger.Assert(null != _instance, "Invalid class type {0}.", NovaEngine.Utility.Text.ToString(_classType));
                }

                ++_referenceCount;
                return _instance;
            }

            /// <summary>
            /// 释放对象实例
            /// </summary>
            public override void Release()
            {
                Release(_instance);
            }

            /// <summary>
            /// 释放对象实例
            /// </summary>
            /// <param name="obj">目标对象实例</param>
            public override void Release(CBean obj)
            {
                Debugger.Assert(obj == _instance, "Invalid arguments.");

                if (null == obj)
                {
                    Debugger.Warn("The singleton instance must be non-null, released it failed.");
                    return;
                }

                --_referenceCount;
                if (_referenceCount <= 0)
                {
                    DestroyInstance(obj);
                    _instance = null;
                    _referenceCount = 0;
                }
            }
        }

        #endregion
    }
}

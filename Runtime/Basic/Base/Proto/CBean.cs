/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// Bean对象抽象类，对需要进行Bean对象定义的场景提供一个通用的基类
    /// </summary>
    public abstract partial class CBean : IProto, IBean, NovaEngine.IInitializable
    {
        /// <summary>
        /// 实体对象的标识
        /// </summary>
        private long _beanId;
        /// <summary>
        /// 实体对象的名称
        /// </summary>
        private string _beanName;

        /// <summary>
        /// 实体对象的符号标记
        /// </summary>
        private Loader.Symboling.SymClass _symbol;

        /// <summary>
        /// 获取或设置实体对象的标识
        /// </summary>
        public long BeanId { get { return _beanId; } internal set { _beanId = value; } }
        /// <summary>
        /// 获取或设置实体对象的名称
        /// </summary>
        public string BeanName { get { return _beanName; } internal set { _beanName = value; } }

        /// <summary>
        /// 获取实体对象对应的符号对象实例
        /// </summary>
        public Loader.Symboling.SymClass Symbol
        {
            get
            {
                if (null == _symbol)
                {
                    _symbol = Loader.CodeLoader.GetSymClassByType(GetType());
                    Debugger.Assert(null != _symbol, "Invalid bean object type.");
                }

                return _symbol;
            }
        }

        /// <summary>
        /// 对象初始化函数接口
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// 对象清理函数接口
        /// </summary>
        public abstract void Cleanup();

        /// <summary>
        /// 对象重载函数接口
        /// </summary>
        internal virtual void Reload()
        {
            // 重载时会卸载掉旧的符号对象，重载解析新的符号对象
            // 所以这里需要重新进行符号对象的加载
            _symbol = null;
        }

        /// <summary>
        /// 获取当前对象实例的Bean名称，若尚未赋值，则返回此对象类型的默认Bean名称
        /// </summary>
        /// <returns>返回对象实例的Bean名称</returns>
        public string GetBeanNameOrDefault()
        {
            if (null == _beanName)
            {
                Loader.Symboling.SymClass symClass = Loader.CodeLoader.GetSymClassByType(GetType());
                Debugger.Assert(null != symClass, "Could not found any symbol class with type '{0}'.", NovaEngine.Utility.Text.ToString(GetType()));

                return symClass.DefaultBeanName;
            }

            return _beanName;
        }

        #region 基于符号标记类的便捷访问接口函数

        /// <summary>
        /// 检测当前对象实例是否具备指定的特性类型
        /// </summary>
        /// <param name="featureType">特性类型</param>
        /// <returns>若当前对象实例具备给定特性类型则返回true，否则返回false</returns>
        public bool HasFeatureType(SystemType featureType)
        {
            return this.Symbol.HasFeatureType(featureType);
        }

        /// <summary>
        /// 检测当前对象实例是否具备指定的接口类型
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns>若当前对象实例具备给定的接口类型则返回true，否则返回false</returns>
        public bool HasInterfaceType(SystemType interfaceType)
        {
            return this.Symbol.HasInterfaceType(interfaceType);
        }

        /// <summary>
        /// 检测当前对象实例是否具备指定的切面行为类型
        /// </summary>
        /// <param name="aspectBehaviourType">切面行为类型</param>
        /// <returns>若当前对象实例具备给定切面行为类型则返回true，否则返回false</returns>
        public bool HasAspectBehaviourType(AspectBehaviourType aspectBehaviourType)
        {
            return this.Symbol.HasAspectBehaviourType(aspectBehaviourType);
        }

        #endregion
    }
}

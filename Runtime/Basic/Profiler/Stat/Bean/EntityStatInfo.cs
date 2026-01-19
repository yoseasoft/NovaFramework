/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

namespace GameEngine.Profiler.Statistics
{
    /// <summary>
    /// 实体模块统计项对象类，对实体对象实例的访问记录进行单项统计的数据单元
    /// </summary>
    public abstract class EntityStatInfo : BeanStatInfo
    {
        /// <summary>
        /// 资产数据管理容器
        /// </summary>
        private readonly IDictionary<string, AssetStatInfo> _assets;
        /// <summary>
        /// 组件数据管理容器
        /// </summary>
        private readonly IDictionary<int, ComponentStatInfo> _components;

        /// <summary>
        /// 获取实体名称
        /// </summary>
        public string EntityName => ClassName;
        /// <summary>
        /// 获取资产数据清单
        /// </summary>
        public IReadOnlyList<AssetStatInfo> Assets => NovaEngine.Utility.Collection.ToReadOnlyListForValues(_assets);
        /// <summary>
        /// 获取组件数据清单
        /// </summary>
        public IReadOnlyList<ComponentStatInfo> Components => NovaEngine.Utility.Collection.ToReadOnlyListForValues(_components);

        protected EntityStatInfo(CEntity entity) : this(entity.BeanId, entity.DeclareClassName, entity.BeanName)
        { }

        protected EntityStatInfo(int uid, string entityName, string beanName) : base(uid, entityName, beanName)
        {
            _assets = new Dictionary<string, AssetStatInfo>();
            _components = new Dictionary<int, ComponentStatInfo>();
        }

        /// <summary>
        /// 组件对象实例添加回调函数
        /// </summary>
        /// <param name="component">组件对象</param>
        internal void OnComponentAdded(CComponent component)
        {
            Debugger.Assert(false == _components.ContainsKey(component.BeanId), NovaEngine.ErrorText.InvalidArguments);

            ComponentStatInfo info = new ComponentStatInfo(component);
            _components.Add(component.BeanId, info);
        }

        /// <summary>
        /// 组件对象实例移除回调函数
        /// </summary>
        /// <param name="component">组件对象</param>
        internal void OnComponentRemoved(CComponent component)
        {
            if (false == _components.TryGetValue(component.BeanId, out ComponentStatInfo info))
            {
                Debugger.Warn(LogGroupTag.Profiler, "Could not found any component stat info by uid '{%d}', removed it failed.", component.BeanId);
                return;
            }
        }
    }
}

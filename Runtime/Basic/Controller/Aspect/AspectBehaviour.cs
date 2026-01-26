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

namespace GameEngine
{
    /// <summary>
    /// 切面行为辅助工具类，针对引擎所有回调接口的切面行为进行统一配置管理
    /// </summary>
    internal static class AspectBehaviour
    {
        /// <summary>
        /// 原型对象自动注入操作处理节点的默认行为类型
        /// </summary>
        public const AspectBehaviourType DefaultBehaviourTypeForAutomaticallyInjectedProcessingNode = AspectBehaviourType.Initialize;
        /// <summary>
        /// 原型对象自动转发操作处理节点的默认行为类型
        /// </summary>
        public const AspectBehaviourType DefaultBehaviourTypeForAutomaticallyDispatchedProcessingNode = AspectBehaviourType.Initialize;
        /// <summary>
        /// 原型对象自动激活操作处理节点的默认行为类型
        /// </summary>
        public const AspectBehaviourType DefaultBehaviourTypeForAutomaticallyActivatedProcessingNode = AspectBehaviourType.Startup;

        /// <summary>
        /// 检测指定行为类型是否为加载阶段使用的行为类型
        /// </summary>
        /// <param name="behaviourType">切面行为类型实例</param>
        /// <returns>若满足检测条件返回true，否则返回false</returns>
        public static bool IsBehaviourTypeOfTheLoadingStage(AspectBehaviourType behaviourType)
        {
            Debugger.Assert(behaviourType > AspectBehaviourType.Unknown, NovaEngine.ErrorText.InvalidArguments);

            if (behaviourType >= AspectBehaviourType.Initialize && behaviourType <= AspectBehaviourType.Start)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测指定行为类型是否为卸载阶段使用的行为类型
        /// </summary>
        /// <param name="behaviourType">切面行为类型实例</param>
        /// <returns>若满足检测条件返回true，否则返回false</returns>
        public static bool IsBehaviourTypeOfTheUnloadingStage(AspectBehaviourType behaviourType)
        {
            Debugger.Assert(behaviourType > AspectBehaviourType.Unknown, NovaEngine.ErrorText.InvalidArguments);

            if (behaviourType >= AspectBehaviourType.Destroy && behaviourType <= AspectBehaviourType.Cleanup)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测指定行为类型是否为执行阶段使用的行为类型
        /// </summary>
        /// <param name="behaviourType">切面行为类型实例</param>
        /// <returns>若满足检测条件返回true，否则返回false</returns>
        public static bool IsBehaviourTypeOfTheProcessingStage(AspectBehaviourType behaviourType)
        {
            Debugger.Assert(behaviourType > AspectBehaviourType.Unknown, NovaEngine.ErrorText.InvalidArguments);

            if (behaviourType > AspectBehaviourType.Start && behaviourType < AspectBehaviourType.Destroy)
            {
                return true;
            }

            return false;
        }

        private readonly static string AspectBehaviourType_Unknown      = AspectBehaviourType.Unknown.ToString();
        private readonly static string AspectBehaviourType_Initialize   = AspectBehaviourType.Initialize.ToString();
        private readonly static string AspectBehaviourType_Startup      = AspectBehaviourType.Startup.ToString();
        private readonly static string AspectBehaviourType_Awake        = AspectBehaviourType.Awake.ToString();
        private readonly static string AspectBehaviourType_Start        = AspectBehaviourType.Start.ToString();
        private readonly static string AspectBehaviourType_Execute      = AspectBehaviourType.Execute.ToString();
        private readonly static string AspectBehaviourType_LateExecute  = AspectBehaviourType.LateExecute.ToString();
        private readonly static string AspectBehaviourType_Update       = AspectBehaviourType.Update.ToString();
        private readonly static string AspectBehaviourType_LateUpdate   = AspectBehaviourType.LateUpdate.ToString();
        private readonly static string AspectBehaviourType_Destroy      = AspectBehaviourType.Destroy.ToString();
        private readonly static string AspectBehaviourType_Shutdown     = AspectBehaviourType.Shutdown.ToString();
        private readonly static string AspectBehaviourType_Cleanup      = AspectBehaviourType.Cleanup.ToString();

        private readonly static int AspectMethodHashCode_Initialize     = AspectBehaviourType_Initialize.GetHashCode();
        private readonly static int AspectMethodHashCode_Startup        = AspectBehaviourType_Startup.GetHashCode();
        private readonly static int AspectMethodHashCode_Awake          = AspectBehaviourType_Awake.GetHashCode();
        private readonly static int AspectMethodHashCode_Start          = AspectBehaviourType_Start.GetHashCode();
        private readonly static int AspectMethodHashCode_Execute        = AspectBehaviourType_Execute.GetHashCode();
        private readonly static int AspectMethodHashCode_LateExecute    = AspectBehaviourType_LateExecute.GetHashCode();
        private readonly static int AspectMethodHashCode_Update         = AspectBehaviourType_Update.GetHashCode();
        private readonly static int AspectMethodHashCode_LateUpdate     = AspectBehaviourType_LateUpdate.GetHashCode();
        private readonly static int AspectMethodHashCode_Destroy        = AspectBehaviourType_Destroy.GetHashCode();
        private readonly static int AspectMethodHashCode_Shutdown       = AspectBehaviourType_Shutdown.GetHashCode();
        private readonly static int AspectMethodHashCode_Cleanup        = AspectBehaviourType_Cleanup.GetHashCode();

        /// <summary>
        /// 通过切面行为类型获取与之对应的行为名称<br/>
        /// 其实可以直接用枚举类型转换名称，但由于该类型调用比较频繁，出于性能考虑，定义了该接口函数
        /// </summary>
        /// <param name="behaviourType">切面行为类型实例</param>
        /// <returns>返回切面行为名称</returns>
        public static string GetAspectBehaviourName(AspectBehaviourType behaviourType)
        {
            return behaviourType switch
            {
                AspectBehaviourType.Initialize  => AspectBehaviourType_Initialize,
                AspectBehaviourType.Startup     => AspectBehaviourType_Startup,
                AspectBehaviourType.Awake       => AspectBehaviourType_Awake,
                AspectBehaviourType.Start       => AspectBehaviourType_Start,
                AspectBehaviourType.Execute     => AspectBehaviourType_Execute,
                AspectBehaviourType.LateExecute => AspectBehaviourType_LateExecute,
                AspectBehaviourType.Update      => AspectBehaviourType_Update,
                AspectBehaviourType.LateUpdate  => AspectBehaviourType_LateUpdate,
                AspectBehaviourType.Destroy     => AspectBehaviourType_Destroy,
                AspectBehaviourType.Shutdown    => AspectBehaviourType_Shutdown,
                AspectBehaviourType.Cleanup     => AspectBehaviourType_Cleanup,
                _ => AspectBehaviourType_Unknown,
            };
        }

        /// <summary>
        /// 通过行为名称获取与之对应的切面行为类型<br/>
        /// 一直在探索性能更好的转换方式：
        /// <para>1. 通过字符串直接转换枚举类型；</para>
        /// <para>2. 通过<see cref="System.Collections.Generic.IDictionary{TKey, TValue}"/>映射的方式管理名称与类型的对应关系；</para>
        /// </summary>
        /// <param name="name">行为名称</param>
        /// <returns>返回切面行为类型</returns>
        public static AspectBehaviourType GetAspectBehaviourType(string name)
        {
            int hash = name.GetHashCode();

            return hash switch
            {
                int when AspectMethodHashCode_Initialize    == hash => AspectBehaviourType.Initialize,
                int when AspectMethodHashCode_Startup       == hash => AspectBehaviourType.Startup,
                int when AspectMethodHashCode_Awake         == hash => AspectBehaviourType.Awake,
                int when AspectMethodHashCode_Start         == hash => AspectBehaviourType.Start,
                int when AspectMethodHashCode_Execute       == hash => AspectBehaviourType.Execute,
                int when AspectMethodHashCode_LateExecute   == hash => AspectBehaviourType.LateExecute,
                int when AspectMethodHashCode_Update        == hash => AspectBehaviourType.Update,
                int when AspectMethodHashCode_LateUpdate    == hash => AspectBehaviourType.LateUpdate,
                int when AspectMethodHashCode_Destroy       == hash => AspectBehaviourType.Destroy,
                int when AspectMethodHashCode_Shutdown      == hash => AspectBehaviourType.Shutdown,
                int when AspectMethodHashCode_Cleanup       == hash => AspectBehaviourType.Cleanup,
                _ => AspectBehaviourType.Unknown,
            };
        }
    }
}

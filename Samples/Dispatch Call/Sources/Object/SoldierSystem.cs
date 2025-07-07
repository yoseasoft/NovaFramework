/// -------------------------------------------------------------------------------
/// NovaEngine Framework Samples
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

using SystemStringBuilder = System.Text.StringBuilder;

namespace Game.Sample.DispatchCall
{
    /// <summary>
    /// 战斗对象逻辑类
    /// </summary>
    [GameEngine.Aspect]
    public static class SoldierSystem
    {
        [GameEngine.OnAspectAfterCallOfTarget(typeof(Soldier), GameEngine.AspectBehaviourType.Awake)]
        static void Awake(this Soldier self)
        {
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Soldier), GameEngine.AspectBehaviourType.Start)]
        static void Start(this Soldier self)
        {
        }

        [GameEngine.OnAspectBeforeCallOfTarget(typeof(Soldier), GameEngine.AspectBehaviourType.Destroy)]
        static void Destroy(this Soldier self)
        {
        }

        public static string ToSoldierString(this Soldier self)
        {
            SystemStringBuilder sb = new SystemStringBuilder();

            IdentityComponent identityComponent = self.GetComponent<IdentityComponent>();
            sb.AppendFormat("Id={0},Type={1},名称={2},",
                identityComponent.objectID,
                identityComponent.objectType,
                identityComponent.objectName);

            AttributeComponent attributeComponent = self.GetComponent<AttributeComponent>();
            sb.AppendFormat("等级={0},经验={1},生命={2},体力={3},攻击={4},",
                attributeComponent.level,
                attributeComponent.exp,
                attributeComponent.health,
                attributeComponent.energy,
                attributeComponent.attack);

            TransformComponent transformComponent = self.GetComponent<TransformComponent>();
            sb.AppendFormat("位置={{{0},{1},{2}}},方向={{{3},{4},{5}}},",
                transformComponent.position.x, transformComponent.position.y, transformComponent.position.z,
                transformComponent.rotation.x, transformComponent.rotation.y, transformComponent.rotation.z);

            SkillComponent skillComponent = self.GetComponent<SkillComponent>();
            sb.Append(@"技能={");
            for (int n = 0; null != skillComponent.skills && n < skillComponent.skills.Count; ++n)
            {
                SkillComponent.Skill skill = skillComponent.skills[n];

                if (n > 0) sb.Append(",");
                sb.AppendFormat("[{0},{1},{2},{3},{4},{5}]", skill.id, skill.name, skill.range, skill.is_coolingdown.ToString(), skill.cooling_time, skill.last_used_time);
            }
            sb.Append(@"}");

            return sb.ToString();
        }
    }
}

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

namespace Game.Sample.DispatchCall
{
    /// <summary>
    /// 技能组件逻辑类
    /// </summary>
    [GameEngine.AspectOfTarget(typeof(SkillComponent))]
    public static class SkillComponentSystem
    {
        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Awake)]
        static void Awake(this SkillComponent self)
        {
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        static void Start(this SkillComponent self)
        {
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Update)]
        static void Update(this SkillComponent self)
        {
            for (int n = 0; null != self.skills && n < self.skills.Count; ++n)
            {
                SkillComponent.Skill skill = self.skills[n];
                if (!skill.is_coolingdown)
                {
                    if (skill.last_used_time >= NovaEngine.Timestamp.RealtimeSinceStartup)
                    {
                        skill.is_coolingdown = true;
                        Debugger.Warn("角色对象‘{%s}’的技能‘{%s}’已冷却！", ((Soldier) self.Entity).GetComponent<IdentityComponent>().objectName, skill.name);
                    }
                }
            }
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Destroy)]
        static void Destroy(this SkillComponent self)
        {
        }

        public static void UseSkill(this SkillComponent self, int skillId = 0)
        {
            SkillComponent.Skill skill = self.GetUnusedSkill(skillId);
            if (null == skill)
            {
                return;
            }

            skill.is_coolingdown = false;
            skill.last_used_time = NovaEngine.Timestamp.RealtimeSinceStartup + skill.cooling_time;
            Debugger.Info("角色对象‘{%s}’的技能‘{%s}’释放成功！", ((Soldier) self.Entity).GetComponent<IdentityComponent>().objectName, skill.name);
        }

        public static SkillComponent.Skill GetUnusedSkill(this SkillComponent self, int skillId = 0)
        {
            if (skillId > 0)
            {
                for (int n = 0; null != self.skills && n < self.skills.Count; ++n)
                {
                    SkillComponent.Skill skill = self.skills[n];
                    if (skill.id == skillId)
                    {
                        if (!skill.is_coolingdown)
                        {
                            Debugger.Warn("角色对象‘{%s}’选定的指定技能‘{%s}’处于冷却CD中，不能使用该技能！", ((Soldier) self.Entity).GetComponent<IdentityComponent>().objectName, skill.name);
                            return null;
                        }

                        return skill;
                    }
                }

                Debugger.Warn("无法从角色对象‘{%s}’的技能列表中找到ID为‘{%d}’的技能实例，获取该技能对象失败！", ((Soldier) self.Entity).GetComponent<IdentityComponent>().objectName, skillId);
                return null;
            }

            for (int n = 0; null != self.skills && n < self.skills.Count; ++n)
            {
                SkillComponent.Skill skill = self.skills[n];
                if (skill.is_coolingdown)
                {
                    return skill;
                }
            }

            Debugger.Warn("角色对象‘{%s}’技能列表中的所有技能当前均处于冷却CD中，获取可使用的技能对象失败！", ((Soldier) self.Entity).GetComponent<IdentityComponent>().objectName);
            return null;
        }
    }
}

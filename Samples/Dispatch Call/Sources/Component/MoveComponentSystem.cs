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
    /// 移动组件逻辑类
    /// </summary>
    public static class MoveComponentSystem
    {
        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Awake)]
        static void Awake(this MoveComponent self)
        {
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Start)]
        static void Start(this MoveComponent self)
        {
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Update)]
        static void Update(this MoveComponent self)
        {
            if (self.escape_time > 0)
            {
                if (false == self.MoveTo())
                {
                    self.escape_time = 0;
                }

                self.escape_time--;
                if (self.escape_time <= 0)
                {
                    TransformComponent transformComponent = self.GetComponent<TransformComponent>();
                    Debugger.Info("角色对象‘{%s}’移动结束，当前位置{{{%d},{%d},{%d}}}！",
                        self.GetComponent<IdentityComponent>().objectName,
                        transformComponent.position.x, transformComponent.position.y, transformComponent.position.z);

                    self.escape_time = 0;
                    self.destination = UnityEngine.Vector3.zero;
                    self.direction = UnityEngine.Vector3.zero;
                }
            }
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        static void Destroy(this MoveComponent self)
        {
        }

        static bool MoveTo(this MoveComponent self)
        {
            TransformComponent ownerTransformComponent = self.GetComponent<TransformComponent>();
            float distance = UnityEngine.Vector3.Distance(ownerTransformComponent.position, self.destination);
            if (distance >= 2f)
            {
                //UnityEngine.Vector3 direction = UnityEngine.Vector3.Normalize(targetTransformComponent.position - ownerTransformComponent.position);
                const float speed = 5f;

                float move_distance = speed * NovaEngine.Timestamp.DeltaTime;
                move_distance = UnityEngine.Mathf.Min(move_distance, distance / 2f);

                UnityEngine.Vector3 old_position = ownerTransformComponent.position;
                ownerTransformComponent.position += self.direction * move_distance;

                //Debugger.Info("角色对象‘{%s}’从源位置{{{%d},{%d},{%d}}}移动到目标位置{{{%d},{%d},{%d}}}！",
                //    self.GetComponent<IdentityComponent>().objectName,
                //    old_position.x, old_position.y, old_position.z,
                //    ownerTransformComponent.position.x, ownerTransformComponent.position.y, ownerTransformComponent.position.z);

                return true;
            }

            return false;
        }

        public static void OnMovingStart(this MoveComponent self)
        {
            if (self.escape_time > 0)
            {
                Debugger.Info("角色对象‘{%s}’正处于移动中，需等待此次移动完成后才可以再次开始新一轮的移动行为！", self.GetComponent<IdentityComponent>().objectName);
                return;
            }

            MainDataComponent mainDataComponent = GameEngine.SceneHandler.Instance.GetCurrentScene().GetComponent<MainDataComponent>();
            Soldier soldier = null;

            AttackComponent attackComponent = self.GetComponent<AttackComponent>();
            if (null == attackComponent)
            {
                soldier = mainDataComponent.player;
            }
            else
            {
                soldier = mainDataComponent.GetSoldierByUid(attackComponent.targetId);
            }

            if (null == soldier)
            {
                Debugger.Info("角色对象‘{%s}’移动目标丢失，无法正确进行移动操作！", self.GetComponent<IdentityComponent>().objectName);
                return;
            }

            TransformComponent ownerTransformComponent = self.GetComponent<TransformComponent>();
            TransformComponent targetTransformComponent = soldier.GetComponent<TransformComponent>();

            self.destination = targetTransformComponent.position;
            self.direction = UnityEngine.Vector3.Normalize(self.destination - ownerTransformComponent.position);

            self.escape_time = 10;
        }

        public static void OnMoveAlongTheDirection(this MoveComponent self, UnityEngine.Vector3 dir)
        {
            TransformComponent ownerTransformComponent = self.GetComponent<TransformComponent>();

            self.destination = ownerTransformComponent.position + 100f * dir;
            self.direction = dir;

            self.escape_time = 10;
        }
    }
}

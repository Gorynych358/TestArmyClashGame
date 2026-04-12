using UnityEngine;

namespace ACT.Scripts
{
    public class UnitAICommandSystem : ICommandSystem
    {
        private readonly BattleManager _battle;

        public UnitAICommandSystem(BattleManager battle)
        {
            _battle = battle;
        }

        public void Update(IUnitContext unit)
        {
            if (unit.CurrentTarget == null || !unit.CurrentTarget.IsAlive)
                unit.CurrentTarget = _battle.GetClosestEnemy(unit);

            var target = unit.CurrentTarget;

            if (target == null)
            {
                unit.MoveDirection = Vector3.zero;
                unit.CanAttack = false;
                return;
            }

            Vector3 dir = target.Transform.position - unit.Transform.position;
            float sqrDist = dir.sqrMagnitude;

            float stopDist = unit.AttackDistance;
            //Debug.Log("AI test: direction = " + dir);
            if (sqrDist <= stopDist * stopDist)
            {
                unit.MoveDirection = Vector3.zero;
                unit.CanAttack = true;
            }
            else
            {
                unit.MoveDirection = dir.normalized;
                unit.CanAttack = false;
            }
        }
    }
}

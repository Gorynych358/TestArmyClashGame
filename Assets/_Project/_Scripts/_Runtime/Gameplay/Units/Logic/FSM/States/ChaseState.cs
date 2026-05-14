using UnityEngine;

namespace ACT.Runtime.Gameplay.Units.Logic.FSM.States
{
    public class ChaseState : BaseUnitState
    {
        public ChaseState(IUnitContext context) : base(context) { }

        public override void Update(float deltaTime)
        {
            if (context.MoveDirection.Equals(Vector3.zero))
            {
                context.ChangeState(UnitStates.Idle);
                return;
            }

            if (context.CanAttack)
            {
                context.ChangeState(UnitStates.Attack);
                return;
            }

            context.Move(context.MoveDirection, deltaTime);
        }
    }
}

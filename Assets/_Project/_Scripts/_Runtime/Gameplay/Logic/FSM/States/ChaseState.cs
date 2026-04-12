using UnityEngine;

namespace ACT.Scripts
{
    public class ChaseState : BaseUnitState
    {
        public ChaseState(IUnitContext context) : base(context) { }

        public override void Update()
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

            context.Move(context.MoveDirection);
        }
    }
}

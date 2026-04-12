using UnityEngine;

namespace ACT.Scripts
{
    public class IdleState : BaseUnitState
    {
        public IdleState(IUnitContext context) : base(context) { }

        public override void Update()
        {
            if (context.MoveDirection != Vector3.zero)
                context.ChangeState(UnitStates.Chase);
            if(context.CanAttack)
                context.ChangeState(UnitStates.Attack);
        }
    }
}

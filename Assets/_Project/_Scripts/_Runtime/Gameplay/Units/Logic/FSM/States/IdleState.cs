using UnityEngine;

namespace ACT.Runtime.Gameplay.Units.Logic.FSM.States
{
    public class IdleState : BaseUnitState
    {
        public IdleState(IUnitContext context) : base(context) { }

        public override void Enter() => context.PlayIdleAnim();

        public override void Update(float deltaTime)
        {
            if (context.MoveDirection != Vector3.zero)
                context.ChangeState(UnitStates.Chase);
            if(context.CanAttack)
                context.ChangeState(UnitStates.Attack);
        }
    }
}

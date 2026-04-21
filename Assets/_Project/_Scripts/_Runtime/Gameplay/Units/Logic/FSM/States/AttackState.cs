using UnityEngine;

namespace ACT.Scripts
{
    public class AttackState : BaseUnitState
    {
        private float _cooldownTime;
        public AttackState(IUnitContext context) : base(context) { }

        public override void Enter()
        {
            _cooldownTime = 0;
        }
        public override void Update()
        {
            if (!context.CanAttack)
            {
                context.ChangeState(UnitStates.Chase);
                return;
            }

            _cooldownTime -= Time.deltaTime;
            
            if(_cooldownTime <= 0)
            {
                _cooldownTime = context.AttackCooldown;
                context.Attack();
            }
        }
    }
}

namespace ACT.Runtime.Gameplay.Units.Logic.FSM.States
{
    public class AttackState : BaseUnitState
    {
        private float _cooldownTime;
        public AttackState(IUnitContext context) : base(context) { }

        public override void Enter()
        {
            _cooldownTime = 0;
        }
        public override void Update(float deltaTime)
        {
            if (!context.CanAttack)
            {
                context.ChangeState(UnitStates.Chase);
                return;
            }

            _cooldownTime -= deltaTime;
            
            if(_cooldownTime <= 0)
            {
                _cooldownTime = context.AttackCooldown;
                context.Attack();
            }
        }
    }
}

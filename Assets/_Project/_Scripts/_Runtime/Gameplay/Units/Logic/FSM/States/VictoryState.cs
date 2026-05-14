using UnityEngine;

namespace ACT.Runtime.Gameplay.Units.Logic.FSM.States
{
    public class VictoryState : BaseUnitState
    {
        public VictoryState(IUnitContext context) : base(context) { }

        public override void Enter()
        {
            var animator = context.Transform.GetComponent<Animator>();
            if (animator != null)
                animator.SetTrigger("Victory");
        }
    }
}

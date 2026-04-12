using UnityEngine;

namespace ACT.Scripts
{
    public class DieState : BaseUnitState
    {
        public DieState(IUnitContext context) : base(context) { }

        public override void Enter()
        {
            //EventBus.RaiseUnitDied(context as Unit);
        }
    }
}

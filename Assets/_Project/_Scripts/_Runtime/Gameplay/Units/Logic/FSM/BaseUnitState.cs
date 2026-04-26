using UnityEngine;

namespace ACT.Scripts
{
    public abstract class BaseUnitState : IState
    {
        protected readonly IUnitContext context;

        protected BaseUnitState(IUnitContext context)
        {
            this.context = context;
        }

        public virtual void Enter() { }
        public virtual void Update(float deltaTime) { }
        public virtual void Exit() { }
    }

}

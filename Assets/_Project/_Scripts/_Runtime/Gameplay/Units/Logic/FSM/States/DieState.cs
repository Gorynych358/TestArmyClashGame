using UnityEngine;

namespace ACT.Scripts
{
    public class DieState : BaseUnitState
    {
        private float _dieAnimTime = 3f;
        private bool _isDieAnimComplete = false;
        public DieState(IUnitContext context) : base(context) { }
        
        public override void Update(float deltaTime)
        {
            
            if(_isDieAnimComplete)
                return;
            
            _dieAnimTime -= deltaTime;
            
            if(_dieAnimTime <= 0)
            {
                _isDieAnimComplete = true;
                context.DispatchDeadEvent();
            }
        }

        public override void Exit()
        {
            _dieAnimTime = 3f;
            _isDieAnimComplete = false;
        }
    }
}

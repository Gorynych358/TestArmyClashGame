using ACT.Runtime.Gameplay.Units.Animations;
using UnityEngine;

namespace ACT.Runtime.Gameplay.Units.Logic.FSM.States
{
    public class DieState : BaseUnitState
    {
        // Длительность нахождения юнита на сцене после поражения:
        private const float DieAnimDuration = 1.5f; 
        private float _elapsedTime = 0f;
        private bool _isDieAnimComplete = false;
        public DieState(IUnitContext context) : base(context) { }

        public override void Enter() 
        {
            _isDieAnimComplete = false;
            context.PlayDieAnim();
        }

        public override void Update(float deltaTime)
        {
            if(_isDieAnimComplete)
                return;
            
            _elapsedTime += deltaTime;
            
            if (_elapsedTime >= DieAnimDuration)
            {
                _elapsedTime = 0f;
                _isDieAnimComplete = true;
                context.DispatchDeadEvent();
            }
        }
    }
}

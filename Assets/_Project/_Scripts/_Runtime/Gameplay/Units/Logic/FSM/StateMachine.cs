using UnityEngine;

namespace ACT.Scripts
{
    public class StateMachine
    {
        private IState _currentState;

        public void ChangeState(IState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public IState GetCurrent() => _currentState;

        public void Update()
        {
            _currentState?.Update();
        }
    }
}

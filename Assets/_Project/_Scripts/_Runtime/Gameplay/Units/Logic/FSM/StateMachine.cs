namespace ACT.Runtime.Gameplay.Units.Logic.FSM
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

        public void Update(float deltaTime)
        {
            _currentState?.Update(deltaTime);
        }
    }
}

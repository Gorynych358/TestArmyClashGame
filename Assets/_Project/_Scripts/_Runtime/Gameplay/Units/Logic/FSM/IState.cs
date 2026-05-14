namespace ACT.Runtime.Gameplay.Units.Logic.FSM
{
    public interface IState
    {
        public void Enter();
        public void Update(float deltaTime);
        public void Exit();
    }
}

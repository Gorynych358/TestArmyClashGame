namespace ACT.Scripts
{
    public readonly struct BattleProgressChangedEvent
    {
        public readonly int DefendersAlive;
        public readonly int InvadersAlive;

        public BattleProgressChangedEvent(int defendersAlive, int invadersAlive)
        {
            DefendersAlive = defendersAlive;
            InvadersAlive = invadersAlive;
        }
    }
}

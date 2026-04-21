namespace ACT.Scripts
{
    public readonly struct ArmyCountChangedEvent : IEvent
    {
        public readonly int DefendersCount;
        public readonly int InvadersCount;

        public ArmyCountChangedEvent(int defendersCount, int invadersCount)
        {
            DefendersCount = defendersCount;
            InvadersCount = invadersCount;
        }
    }
}

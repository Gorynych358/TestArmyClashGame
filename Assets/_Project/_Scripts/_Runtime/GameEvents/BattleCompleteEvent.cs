namespace ACT.Scripts
{
    public readonly struct BattleCompleteEvent : IEvent
    {
        public readonly bool IsPlayerWon;

        public BattleCompleteEvent(bool isPlayerWon)
        {
            IsPlayerWon = isPlayerWon;
        }
    }
}
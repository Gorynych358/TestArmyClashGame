using ACT.Runtime.Infrastructure.EventBus;

namespace ACT.Runtime.GameEvents
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
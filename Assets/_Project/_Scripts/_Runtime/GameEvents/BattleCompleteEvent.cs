using ACT.Runtime.Gameplay.Battle.Session;
using ACT.Runtime.Infrastructure.EventBus;

namespace ACT.Runtime.GameEvents
{
    public readonly struct BattleCompleteEvent : IEvent
    {
        public readonly FinalSessionData SessionData;

        public BattleCompleteEvent(FinalSessionData sessionData) => SessionData = sessionData;
    }
}
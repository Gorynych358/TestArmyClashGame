using ACT.Runtime.Gameplay.Battle;
using ACT.Runtime.Gameplay.Battle.Session;
using ACT.Runtime.Infrastructure.EventBus;

namespace ACT.Runtime.GameEvents
{
    public readonly struct ArmyStatsChangedEvent : IEvent
    {
        public readonly CurrentSessionData SessionData;

        public ArmyStatsChangedEvent(CurrentSessionData sessionData) => SessionData = sessionData;
    }
}

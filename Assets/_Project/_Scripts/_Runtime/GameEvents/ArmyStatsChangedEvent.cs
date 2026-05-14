using ACT.Runtime.Gameplay.Battle;
using ACT.Runtime.Infrastructure.EventBus;

namespace ACT.Runtime.GameEvents
{
    public readonly struct ArmyStatsChangedEvent : IEvent
    {
        public readonly BattleSessionData SessionData;

        public ArmyStatsChangedEvent(BattleSessionData sessionData) => SessionData = sessionData;
    }
}

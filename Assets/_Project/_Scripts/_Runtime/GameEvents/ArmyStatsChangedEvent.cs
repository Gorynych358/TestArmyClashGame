namespace ACT.Scripts
{
    public readonly struct ArmyStatsChangedEvent : IEvent
    {
        public readonly BattleSessionData SessionData;

        public ArmyStatsChangedEvent(BattleSessionData sessionData) => SessionData = sessionData;
    }
}

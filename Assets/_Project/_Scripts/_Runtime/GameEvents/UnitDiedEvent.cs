using ACT.Runtime.Gameplay.Units;
using ACT.Runtime.Infrastructure.EventBus;

namespace ACT.Runtime.GameEvents
{
    public readonly struct UnitDiedEvent : IEvent
    {
        public readonly Unit Unit;
        public UnitDiedEvent(Unit unit) => Unit = unit;
    }
}

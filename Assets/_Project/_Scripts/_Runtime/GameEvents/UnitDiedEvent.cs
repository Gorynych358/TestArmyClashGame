using UnityEngine;

namespace ACT.Scripts
{
    public readonly struct UnitDiedEvent : IEvent
    {
        public readonly Unit Unit;
        public UnitDiedEvent(Unit unit) => Unit = unit;
    }
}

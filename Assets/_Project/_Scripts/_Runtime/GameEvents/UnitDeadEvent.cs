using UnityEngine;

namespace ACT.Scripts
{
    public readonly struct UnitDeadEvent : IEvent
    {
        public readonly Unit Unit;
        public UnitDeadEvent(Unit unit) => Unit = unit;
    }
}

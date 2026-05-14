using UnityEngine;

namespace ACT.Runtime.Gameplay.Units
{
    public interface IUnitFactory
    {
        Unit Create(UnitTypes type, Transform parent, Vector3 position = default);
    }
}

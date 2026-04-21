using UnityEngine;

namespace ACT.Scripts
{
    public interface IUnitFactory
    {
        Unit Create(UnitTypes type, Transform parent, Vector3 position = default);
    }
}

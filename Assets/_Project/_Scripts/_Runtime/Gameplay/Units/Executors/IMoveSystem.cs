using UnityEngine;

namespace ACT.Runtime.Gameplay.Units.Executors
{
    public interface IMoveSystem
    {
        void Move(IUnitContext ctx, Vector3 direction, float speed, float deltaTime);
    }
}

using UnityEngine;

namespace ACT.Runtime.Gameplay.Units.Executors
{
    public class UnitMover : IMoveSystem
    {
        public void Move(IUnitContext ctx, Vector3 direction, float speed, float deltaTime)
        {
            ctx.Transform.position += speed * deltaTime * direction;
        }
    }
}
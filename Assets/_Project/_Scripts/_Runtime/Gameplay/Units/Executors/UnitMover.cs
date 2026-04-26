using UnityEngine;

namespace ACT.Scripts
{
    public class UnitMover : IMoveSystem
    {
        public void Move(IUnitContext ctx, Vector3 direction, float speed, float deltaTime)
        {
            ctx.Transform.position += speed * deltaTime * direction;
        }
        public void ApplyKnockback(IUnitContext ctx, Vector3 direction, float knockbackPower)
        {
            
        }
    }
}

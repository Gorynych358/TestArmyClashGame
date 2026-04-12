using UnityEngine;

namespace ACT.Scripts
{
    public class UnitMover : IUnitMover
    {
        public void Move(IUnitContext ctx, Vector3 direction, float speed)
        {
            //Debug.Log("Mover move: " + direction);
            ctx.Transform.position += speed * Time.deltaTime * direction;
        }
        public void KnockBack(IUnitContext ctx, Vector3 direction, float knockBackPower)
        {
            
        }
    }
}

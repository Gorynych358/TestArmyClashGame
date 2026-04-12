using UnityEngine;

namespace ACT.Scripts
{
    public interface IUnitMover
    {
        void Move(IUnitContext ctx, Vector3 direction, float speed);
        void KnockBack(IUnitContext ctx, Vector3 direction, float knockBackPower);
    }
}

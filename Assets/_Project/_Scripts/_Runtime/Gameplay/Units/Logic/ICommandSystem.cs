using UnityEngine;

namespace ACT.Scripts
{
    public interface ICommandSystem
    {
        void Update(IUnitContext unit);
    }
}

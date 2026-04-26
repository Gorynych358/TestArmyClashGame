using UnityEngine;

namespace ACT.Scripts
{
    using UnityEngine;

    public interface IUnitContext
    {
        ArmyTypes ArmyType { get; }
        Transform Transform { get; }

        Vector3 MoveDirection { get;set;}
        bool CanAttack { get;set;}

        float AttackDistance { get; }
        float AttackCooldown { get; }

        bool IsAttackTarget { get; }

        IUnitContext CurrentTarget { get; set; }

        void ChangeState(UnitStates newState);

        void Move(Vector3 direction, float deltaTime);
        void Attack();
        void DispatchDeadEvent();
    }
}

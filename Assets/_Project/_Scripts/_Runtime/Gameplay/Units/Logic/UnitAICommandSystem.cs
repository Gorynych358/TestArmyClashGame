using System.Collections.Generic;
using ACT.Runtime.Gameplay.Battle;
using UnityEngine;

namespace ACT.Runtime.Gameplay.Units.Logic
{
    public class UnitAICommandSystem : ICommandSystem
    {
        private readonly BattleManager _battle;
        private readonly SteeringBehaviorProfile _profile;
        private readonly List<Unit> _neighbors = new(128);

        public UnitAICommandSystem(BattleManager battle, SteeringBehaviorProfile profile)
        {
            _battle = battle;
            _profile = profile;
        }

        public void Update(IUnitContext unit)
        {
            // --- Обновление цели ---
            if (unit.CurrentTarget == null || !unit.CurrentTarget.IsAttackTarget)
                unit.CurrentTarget = _battle.GetClosestEnemy(unit);

            var target = unit.CurrentTarget;

            // --- Нет цели: стоим, не атакуем ---
            if (target == null)
            {
                unit.MoveDirection = Vector3.zero;
                unit.CanAttack = false;
                return;
            }

            Vector3 unitPos   = unit.Transform.position;
            Vector3 targetPos = target.Transform.position;

            // --- Базовое направление на цель ---
            Vector3 dirToTarget = (targetPos - unitPos).normalized;

            // --- Получаем соседей через SpatialGrid ---
            _battle.FillNeighbors(unit, _neighbors);

            // --- Steering-компоненты ---
            Vector3 avoidance  = ComputeAvoidance(unit);
            Vector3 separation = ComputeSeparation(unit);
            Vector3 alignment  = ComputeAlignment(unit);
            Vector3 cohesion   = ComputeCohesion(unit);

            // --- Итоговое направление ---
            Vector3 steering =
                dirToTarget   * _profile.targetWeight     +
                avoidance     * _profile.avoidanceWeight  +
                separation    * _profile.separationWeight +
                alignment     * _profile.alignmentWeight  +
                cohesion      * _profile.cohesionWeight;

            Vector3 desired = steering.sqrMagnitude > 0.0001f
                ? steering.normalized
                : Vector3.zero;

            // --- Сглаживание поворота ---
            unit.MoveDirection = Vector3.Lerp(
                unit.MoveDirection,
                desired,
                _profile.turnSpeed * Time.deltaTime
            );

            // --- Решение об атаке ---
            float sqrDist = (targetPos - unitPos).sqrMagnitude;
            float attackRange = unit.AttackDistance * unit.AttackDistance;

            unit.CanAttack = sqrDist <= attackRange;
        }

        // ------------------------------
        // Steering Behaviors
        // ------------------------------

        private Vector3 ComputeAvoidance(IUnitContext unit)
        {
            Vector3 force = Vector3.zero;
            int count = 0;

            Vector3 selfPos = unit.Transform.position;

            foreach (var other in _neighbors)
            {
                if (other == null) continue;

                Vector3 diff = selfPos - other.Transform.position;
                float dist = diff.magnitude;

                if (dist > 0f && dist < _profile.avoidanceRadius)
                {
                    force += diff.normalized / dist;
                    count++;
                }
            }

            if (count == 0)
                return Vector3.zero;

            return (force / count).normalized;
        }

        private Vector3 ComputeSeparation(IUnitContext unit)
        {
            Vector3 force = Vector3.zero;
            int count = 0;

            Vector3 selfPos = unit.Transform.position;

            foreach (var other in _neighbors)
            {
                if (other == null) continue;

                Vector3 diff = selfPos - other.Transform.position;
                float dist = diff.magnitude;

                if (dist > 0f && dist < _profile.separationRadius)
                {
                    force += diff.normalized;
                    count++;
                }
            }

            if (count == 0)
                return Vector3.zero;

            return (force / count).normalized;
        }

        private Vector3 ComputeAlignment(IUnitContext unit)
        {
            Vector3 sumDir = Vector3.zero;
            int count = 0;

            Vector3 selfPos = unit.Transform.position;

            foreach (var other in _neighbors)
            {
                if (other == null) continue;

                float dist = (other.Transform.position - selfPos).magnitude;
                if (dist > _profile.alignmentRadius)
                    continue;

                if (other.MoveDirection.sqrMagnitude > 0.0001f)
                {
                    sumDir += other.MoveDirection.normalized;
                    count++;
                }
            }

            if (count == 0)
                return Vector3.zero;

            return (sumDir / count).normalized;
        }

        private Vector3 ComputeCohesion(IUnitContext unit)
        {
            Vector3 center = Vector3.zero;
            int count = 0;

            Vector3 selfPos = unit.Transform.position;

            foreach (var other in _neighbors)
            {
                if (other == null) continue;

                float dist = (other.Transform.position - selfPos).magnitude;
                if (dist > _profile.cohesionRadius)
                    continue;

                center += other.Transform.position;
                count++;
            }

            if (count == 0)
                return Vector3.zero;

            center /= count;
            Vector3 toCenter = center - selfPos;

            return toCenter.sqrMagnitude > 0.0001f
                ? toCenter.normalized
                : Vector3.zero;
        }
    }
}
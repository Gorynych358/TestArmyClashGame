using System.Collections.Generic;
using UnityEngine;

namespace ACT.Scripts
{
    public class UnitAICommandSystem : ICommandSystem
    {
        private readonly BattleManager _battle;
        private readonly SteeringBehaviorProfile _profile;
        private List<Unit> _neighbors;
        public UnitAICommandSystem(BattleManager battle, SteeringBehaviorProfile profile)
        {
            _battle = battle;
            _profile = profile;
        }

        public void Update(IUnitContext unit)
        {
            if (unit.CurrentTarget == null || !unit.CurrentTarget.IsAttackTarget)
                unit.CurrentTarget = _battle.GetClosestEnemy(unit);

            var target = unit.CurrentTarget;
            
            if (target == null)
            {
                unit.MoveDirection = Vector3.zero;
                unit.CanAttack = false;
                return;
            }

            Vector3 unitPos = unit.Transform.position;
            Vector3 targetPos = target.Transform.position;

            Vector3 dirToTarget = (targetPos - unitPos).normalized;
            //Получаем список соседей через SpatialGrid:
            _neighbors = _battle.GetNeighbors(unit);
            //Считаем поправки в направлении движения юнита:
            Vector3 avoidance  = ComputeAvoidance(unit);
            Vector3 separation = ComputeSeparation(unit);
            Vector3 alignment  = ComputeAlignment(unit);
            Vector3 cohesion   = ComputeCohesion(unit);

            // --- Steering: чистое направление ---
            Vector3 steering =
                dirToTarget   * _profile.targetWeight     +
                avoidance     * _profile.avoidanceWeight  +
                separation    * _profile.separationWeight +
                alignment     * _profile.alignmentWeight  +
                cohesion      * _profile.cohesionWeight;

            Vector3 desired = steering.sqrMagnitude > 0.0001f
                ? steering.normalized
                : Vector3.zero;

            // --- Сглаживание направления(типа, как на льду) ---
            unit.MoveDirection = Vector3.Lerp(
                unit.MoveDirection,
                desired,
                _profile.turnSpeed * Time.deltaTime
            );

            float sqrDist = (targetPos - unitPos).sqrMagnitude;
            unit.CanAttack = sqrDist <= unit.AttackDistance * unit.AttackDistance;
        }

        private Vector3 ComputeAvoidance(IUnitContext unit)
        {
            Vector3 force = Vector3.zero;
            int count = 0;

            Vector3 selfPos = unit.Transform.position;

            foreach (var other in _neighbors)
            {
                if (other == null)
                    continue;

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

            force /= count;
            return force.normalized;
        }

        private Vector3 ComputeSeparation(IUnitContext unit)
        {
            Vector3 force = Vector3.zero;
            int count = 0;

            Vector3 selfPos = unit.Transform.position;

            foreach (var other in _neighbors)
            {
                if (other == null)
                    continue;

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

            force /= count;
            return force.normalized;
        }

        private Vector3 ComputeAlignment(IUnitContext unit)
        {
            Vector3 sumDir = Vector3.zero;
            int count = 0;

            foreach (var other in _neighbors)
            {
                if (other == null)
                    continue;

                float dist = (other.Transform.position - unit.Transform.position).magnitude;
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

            sumDir /= count;
            return sumDir.normalized;
        }

        private Vector3 ComputeCohesion(IUnitContext unit)
        {
            Vector3 center = Vector3.zero;
            int count = 0;

            Vector3 selfPos = unit.Transform.position;

            foreach (var other in _neighbors)
            {
                if (other == null)
                    continue;

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

            if (toCenter.sqrMagnitude < 0.0001f)
                return Vector3.zero;

            return toCenter.normalized;
        }
    }
}

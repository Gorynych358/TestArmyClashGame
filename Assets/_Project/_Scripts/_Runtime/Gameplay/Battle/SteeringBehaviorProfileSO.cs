using UnityEngine;

namespace ACT.Scripts
{
    [CreateAssetMenu(fileName = "SteeringBehaviorProfileSO", menuName = "Configs/Behavior/SteeringBehaviorProfileSO")]
    public class SteeringBehaviorProfile : ScriptableObject
    {
        [Header("Turning")]

        [Tooltip("Скорость поворота юнита. Чем выше значение — тем быстрее он разворачивается. "
            + "Малое значение создаёт эффект 'скольжения', как на льду.")]
        [Range(0.1f, 10f)]
        public float turnSpeed = 3f;

        [Header("Radii")]

        [Tooltip("Минимальная дистанция, на которой юнит начинает уворачиваться от столкновений.")]
        [Range(0.1f, 3f)]
        public float avoidanceRadius = 1.2f;

        [Tooltip("Дистанция, на которой юнит начинает раздвигать толпу вокруг себя.")]
        [Range(0.1f, 5f)]
        public float separationRadius = 1.5f;

        [Tooltip("Радиус, в котором юнит учитывает направление движения соседей.")]
        [Range(0.5f, 10f)]
        public float alignmentRadius = 4f;

        [Tooltip("Радиус, в котором юнит стремится держаться ближе к центру группы.")]
        [Range(0.5f, 10f)]
        public float cohesionRadius = 4f;


        [Header("Weights")]

        [Tooltip("Насколько сильно юнит стремится к цели (врагу).")]
        [Range(0f, 5f)]
        public float targetWeight = 1.0f;

        [Tooltip("Важность избегания столкновений.")]
        [Range(0f, 5f)]
        public float avoidanceWeight = 1.5f;

        [Tooltip("Важность расталкивания толпы.")]
        [Range(0f, 5f)]
        public float separationWeight = 1.2f;

        [Tooltip("Важность выравнивания направления движения(эффект выдерживания строя).")]
        [Range(0f, 5f)]
        public float alignmentWeight = 0.8f;

        [Tooltip("Важность стремления к центру группы(кучность).")]
        [Range(0f, 5f)]
        public float cohesionWeight = 0.6f;
    }
}

using UnityEngine;

namespace ACT.Runtime.Gameplay.Units.UnitConfigurationSystem
{
    [CreateAssetMenu(fileName = "ShapeModifier", menuName = "Configs/Unit/Modifiers/Shape")]
    public class ShapeModifierSO : UnitModifierSO
    {
        [Header("Shape type of the unit"), SerializeField] private ShapeModifierType _shape;
        public ShapeModifierType Shape => _shape;
    }
}

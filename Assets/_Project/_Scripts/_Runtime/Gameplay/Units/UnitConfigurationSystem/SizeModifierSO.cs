using UnityEngine;

namespace ACT
{
    [CreateAssetMenu(fileName = "SizeModifier", menuName = "Configs/Unit/Modifiers/Size")]
    public class SizeModifierSO : UnitModifierSO
    {
        [Header("Size type of the unit"), SerializeField] private SizeModifierType _size;
        [Header("Unit scale factor"), SerializeField]private float _sizeScaleFactor = 1.0f;

        public SizeModifierType Size => _size;
        public float SizeScaleFactor => _sizeScaleFactor;
    }
}

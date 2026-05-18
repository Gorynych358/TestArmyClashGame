using UnityEngine;

namespace ACT.Runtime.Gameplay.Units.UnitConfigurationSystem
{
    [CreateAssetMenu(fileName = "ColorModifier", menuName = "Configs/Unit/Modifiers/Color")]
    public class ColorModifierSO : UnitModifierSO
    {
        [Header("Color type of the unit"), SerializeField] private ColorModifierType _colorType;
        [Header("Unit color"), SerializeField]private Color _colorDef = Color.white;
        public Color ColorDef => _colorDef;
        public ColorModifierType ColorType => _colorType;
    }
}

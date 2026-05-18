using UnityEngine;

namespace ACT.Runtime.Gameplay.Units.UnitConfigurationSystem
{
    [CreateAssetMenu(fileName = "ModifiersProvider", menuName = "Configs/Unit/ModifiersProvider")]
    public class ModifiersProviderSO : ScriptableObject
    {
        [System.Serializable]
        public struct ShapeEntry
        {
            public ShapeModifierType type;
            public ShapeModifierSO modifier;
        }

        [System.Serializable]
        public struct SizeEntry
        {
            public SizeModifierType type;
            public SizeModifierSO modifier;
        }

        [System.Serializable]
        public struct ColorEntry
        {
            public ColorModifierType type;
            public ColorModifierSO modifier;
        }

        [SerializeField] private ShapeEntry[] _shapes;
        [SerializeField] private SizeEntry[] _sizes;
        [SerializeField] private ColorEntry[] _colors;

        public ShapeModifierSO GetShape(ShapeModifierType type)
            => System.Array.Find(_shapes, x => x.type == type).modifier;

        public SizeModifierSO GetSize(SizeModifierType type)
            => System.Array.Find(_sizes, x => x.type == type).modifier;

        public ColorModifierSO GetColor(ColorModifierType type)
            => System.Array.Find(_colors, x => x.type == type).modifier;
    }
}

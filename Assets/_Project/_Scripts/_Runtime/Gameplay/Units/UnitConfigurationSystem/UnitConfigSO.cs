using ACT.Runtime.Gameplay.Units;
using UnityEngine;

namespace ACT.Runtime.Gameplay.Units.UnitConfigurationSystem
{
    [CreateAssetMenu(fileName = "UnitConfigSO", menuName = "Configs/Unit/UnitConfigSO")]
    
    public class UnitConfigSO : ScriptableObject
    {
        [Header("Unit Identity")]
        [SerializeField] private UnitTypes _unitType;
        public UnitTypes UnitType => _unitType;

        [Header("Base Stats")]
        [SerializeField] private int _baseHP = 100;
        [SerializeField] private int _baseATK = 10;
        [SerializeField] private float _baseSPEED = 5f;
        [SerializeField] private float _baseATKSPD = 1f;
        [SerializeField] private float _baseKnockback = 0f;

        public int BaseHP => _baseHP;
        public int BaseATK => _baseATK;
        public float BaseSPEED => _baseSPEED;
        public float BaseATKSPD => _baseATKSPD;
        public float BaseKNOCKBACK => _baseKnockback;

        [Header("Modifiers")]
        [SerializeField] private ModifiersProviderSO _provider;
        [SerializeField] private ShapeModifierType _shape;
        [SerializeField] private SizeModifierType _size;
        [SerializeField] private ColorModifierType _color;

        public ModifiersProviderSO Provider => _provider;
        public ShapeModifierType Shape => _shape;
        public SizeModifierType Size => _size;
        public ColorModifierType Color => _color;

        public ShapeModifierSO ShapeMod => _provider.GetShape(_shape);
        public SizeModifierSO SizeMod => _provider.GetSize(_size);
        public ColorModifierSO ColorMod => _provider.GetColor(_color);

        public int FinalHP => BaseHP + ShapeMod.HP + SizeMod.HP + ColorMod.HP;
        public int FinalATK => BaseATK + ShapeMod.ATK + SizeMod.ATK + ColorMod.ATK;
        public float FinalSPEED => BaseSPEED + ShapeMod.SPEED + SizeMod.SPEED + ColorMod.SPEED;
        public float FinalATKSPD => BaseATKSPD + ShapeMod.ATKSPD + SizeMod.ATKSPD + ColorMod.ATKSPD;
        public float FinalKNOCKBACK => BaseKNOCKBACK + ShapeMod.Knockback + SizeMod.Knockback + ColorMod.Knockback;

        // Power Score (условная ценность юнита для баланса):
        private float _powerScore = -1f;
        public float PowerScore => _powerScore;

    #if UNITY_EDITOR
        private void OnValidate()
        {
            if (_provider == null) return;

            RecalculatePowerScore();

            if (ShapeMod == null || SizeMod == null || ColorMod == null)
                Debug.LogWarning($"{name}: Missing modifier in provider!");

            var guids = UnityEditor.AssetDatabase.FindAssets("t:UnitConfigSO");
            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var cfg = UnityEditor.AssetDatabase.LoadAssetAtPath<UnitConfigSO>(path);
                if (cfg != this && cfg.UnitType == this.UnitType)
                    Debug.LogWarning($"Duplicate UnitType: {UnitType} in {cfg.name} and {name}");
            }
        }
    #endif
        public void RecalculatePowerScore()
        {
            // Считаем DPS
            float dps = FinalATK * (1f + 0.25f * FinalATKSPD);

            // Считаем EHP (Effective HP)
            float ehp = FinalHP * (1f + FinalSPEED / 20f);

            // Power Score - считаем финальный показатель, который учитывает и урон, и выживаемость
            _powerScore = Mathf.Sqrt(Mathf.Max(0.01f, ehp * dps));
        }
    }
}

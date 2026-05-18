using UnityEngine;

namespace ACT.Runtime.Infrastructure
{
    [CreateAssetMenu(fileName = "ArmyPowerSettingsSO", menuName = "Configs/Behavior/Army power settings")]
    public sealed class ArmyPowerSettingsSO : ScriptableObject
    {
        private const int DEFAULT_MIN = 0;
        private const int DEFAULT_MAX = 100000;
        private const int DEFAULT_POWER = 300;

        [Header("Config (read-only)")]
        [ReadOnly] [SerializeField] private int _minPower = DEFAULT_MIN;
        [ReadOnly] [SerializeField] private int _maxPower = DEFAULT_MAX;

        [Header("Runtime Value")]
        [SerializeField] private int _armyPower = DEFAULT_POWER;

        public int MinPower => _minPower;
        public int MaxPower => _maxPower;

        public int ArmyPower
        {
            get => _armyPower;
            set => _armyPower = Mathf.Clamp(value, _minPower, _maxPower);
        }

        private void OnValidate()
        {
            _armyPower = Mathf.Clamp(_armyPower, _minPower, _maxPower);
        }
    }
}
    
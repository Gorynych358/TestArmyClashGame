using System;
using UnityEngine;

namespace ACT.Runtime.Gameplay.Battle.Session
{
    public sealed class BattleSession
    {
        // Конструктор с минимальной инициализацией
        public BattleSession(float armyPower)
        {
            ArmyPower = armyPower;
            // Инициализируем начальные значения как 0 (будут установлены позже)
            _initialDefendersCount = 0;
            _initialInvadersCount = 0;
            _initialDefendersPower = 0f;
            _initialInvadersPower = 0f;
        }

        // Публичные свойства
        public float ArmyPower { get; private set; }
        public Color DefendersColor { get; private set; }
        public Color InvadersColor { get; private set; }

        // Значения мощности армий и количество юнитов в начале боя:
        private int _initialDefendersCount;
        private int _initialInvadersCount;
        private float _initialDefendersPower;
        private float _initialInvadersPower;

        // Значения мощности армий и количество юнитов в конце боя:
        private int _defendersCount;
        private int _invadersCount;
        private float _defendersPower;
        private float _invadersPower;

        private bool _isSessionComplete = false;

        // Методы изменения
        public void SetColors(Color defender, Color invader)
        {
            DefendersColor = defender;
            InvadersColor = invader;
        }

        /// <summary>
        /// Устанавливает начальные статы армии.
        /// </summary>
        public void FixInitialArmyStats()
        {
            // Валидация данных
            if(_defendersCount <= 0 || _defendersPower <= 0)
                throw new InvalidOperationException("BattleSession Exception -> The defenders' army count and power must be greater than zero");
            if(_invadersCount <= 0 || _invadersPower <= 0)
                throw new InvalidOperationException("BattleSession Exception -> The invaders' army count and power must be greater than zero");

            _initialDefendersCount = _defendersCount;
            _initialDefendersPower = _defendersPower;
            _initialInvadersCount = _invadersCount;
            _initialInvadersPower = _invadersPower;
        }

        /// <summary>
        /// Устанавливает текущие статы армии.
        /// </summary>
        public void SetArmyStats(ArmyTypes armyType, int count, float power)
        {
            // Валидация данных
            if (count < 0)
                throw new ArgumentException($"Count value is {count}. Count must be non-negative.");
            if (power < 0)
                throw new ArgumentException($"Power value is {power}. Power must be non-negative.");
            
            if(armyType == ArmyTypes.Defenders)
            {
                _defendersCount = count;
                _defendersPower = power;
            }
            else if(armyType == ArmyTypes.Invaders)
            {
                _invadersCount = count;
                _invadersPower = power;
            }
        }

        public void SessionComplete()
        {
            if (_isSessionComplete)
                throw new InvalidOperationException("Session is already complete");

            _isSessionComplete = true;
        }

        // Получение данных для текущего состояния
        public CurrentSessionData GetCurrentData()
        {
            return new CurrentSessionData(
                defendersCount: _defendersCount,
                defendersPower: _defendersPower,
                invadersCount: _invadersCount,
                invadersPower: _invadersPower);
        }

        // Получение финальных данных
        public FinalSessionData GetFinalData()
        {
            // Валидация данных
            if (!_isSessionComplete)
                throw new InvalidOperationException("Session is not complete. Call SessionComplte first");
            
            return new FinalSessionData(
                armyPower: ArmyPower,
                defendersColor: DefendersColor,
                invadersColor: InvadersColor,
                initialDefendersCount: _initialDefendersCount,
                initialDefendersPower: _initialDefendersPower,
                initialInvadersCount: _initialInvadersCount,
                initialInvadersPower: _initialInvadersPower,
                defendersCount: _defendersCount,
                defendersPower: _defendersPower,
                invadersCount: _invadersCount,
                invadersPower: _invadersPower);
        }
    }
}
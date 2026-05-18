using UnityEngine;

namespace ACT.Runtime.Gameplay.Battle.Session
{
    public struct FinalSessionData
    {
        public float ArmyPower { get; private set; }
        // Цвета армий
        public Color DefendersColor { get; private set; }
        public Color InvadersColor { get; private set; }

        // Начальное количество юнитов
        public int InitialDefendersCount { get; private set; }
        public int InitialInvadersCount { get; private set; }

        // Начальное мощность армий
        public float InitialDefendersPower { get; private set; }
        public float InitialInvadersPower { get; private set; }

        // Количество юнитов
        public int DefendersCount { get; private set; }
        public int InvadersCount { get; private set; }

        // Мощность армий
        public float DefendersPower { get; private set; }
        public float InvadersPower { get; private set; }

        public FinalSessionData(
            float armyPower,
            Color defendersColor, Color invadersColor,
            int initialDefendersCount, int initialInvadersCount,
            float initialDefendersPower, float initialInvadersPower,
            int defendersCount, int invadersCount,
            float defendersPower, float invadersPower)
        {
            ArmyPower = armyPower;
            DefendersColor = defendersColor;
            InvadersColor = invadersColor;
            InitialDefendersCount = initialDefendersCount;
            InitialInvadersCount = initialInvadersCount;
            InitialDefendersPower = initialDefendersPower;
            InitialInvadersPower = initialInvadersPower;
            DefendersCount = defendersCount;
            InvadersCount = invadersCount;
            DefendersPower = defendersPower;
            InvadersPower = invadersPower;
        }
    }
}
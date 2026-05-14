using UnityEngine;

namespace ACT.Runtime.Gameplay.Battle
{
    public sealed class BattleSessionData
    {
        public BattleSessionData(float armyPower)
        {
            this.ArmyPower = armyPower;
        }
        
        public float ArmyPower { get; private set; }
        // Цвета армий
        public Color DefendersColor { get; private set; }
        public Color InvadersColor { get; private set; }

        // Количество юнитов
        public int DefendersCount { get; private set; }
        public int InvadersCount { get; private set; }

        // Мощность армий
        public float DefendersPower { get; private set; }
        public float InvadersPower { get; private set; }

        // Результат боя
        public BattleResult Result { get; private set; }

        // Можно добавить время боя, награды и т.п.
        public int EarnedGold { get; private set; }

        // --- Методы изменения доступны только BattleManager ---
        public void SetColors(Color defender, Color invader)
        {
            DefendersColor = defender;
            InvadersColor = invader;
        }

        public void SetDefendersArmyStats(int defenderUnits, float defenderPower)
        {
            DefendersCount = defenderUnits;
            DefendersPower = defenderPower;
        }

        public void SetInvadersArmyStats(int invaderUnits, float invaderPower)
        {
            InvadersCount = invaderUnits;
            InvadersPower = invaderPower;
        }

        public void SetResult(BattleResult result, int earnedGold)
        {
            Result = result;
            EarnedGold = earnedGold;
        }
    }
}
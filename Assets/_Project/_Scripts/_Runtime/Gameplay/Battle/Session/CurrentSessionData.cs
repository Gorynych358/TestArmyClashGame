namespace ACT.Runtime.Gameplay.Battle.Session
{
    public struct CurrentSessionData
    {
        // Количество юнитов
        public int DefendersCount { get; private set; }
        public int InvadersCount { get; private set; }

        // Мощность армий
        public float DefendersPower { get; private set; }
        public float InvadersPower { get; private set; }

        public CurrentSessionData(int defendersCount, float defendersPower, int invadersCount, float invadersPower)
        {
            DefendersCount = defendersCount;
            DefendersPower = defendersPower;
            InvadersCount = invadersCount;
            InvadersPower = invadersPower;
        }
    }
}
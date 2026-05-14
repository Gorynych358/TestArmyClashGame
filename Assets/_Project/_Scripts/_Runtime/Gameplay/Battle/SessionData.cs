namespace ACT.Runtime.Gameplay.Battle
{
    public struct SessionData
    {
        // Количество юнитов
        public int DefendersCount { get; private set; }
        public int InvadersCount { get; private set; }

        // Мощность армий
        public float DefendersPower { get; private set; }
        public float InvadersPower { get; private set; }
    }
}
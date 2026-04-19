using UnityEngine;

namespace ACT.Scripts
{
    public readonly struct ArmyCountChangedEvent : IEvent
    {
        public readonly int PlayerCount;
        public readonly int EnemyCount;

        public ArmyCountChangedEvent(int playerCount, int enemyCount)
        {
            PlayerCount = playerCount;
            EnemyCount = enemyCount;
        }
    }
}

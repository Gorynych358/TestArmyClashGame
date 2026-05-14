using UnityEngine;

namespace ACT.Runtime.Infrastructure.Economy
{
    /// <summary>
    /// Тут храним все параметры экономики. 
    /// В данном случае Battle Rewards, но можно также и другие, 
    /// вроде: Upgrade Costs, Daily Rewards, Ads Rewards и т.п.
    /// </summary>
    [CreateAssetMenu(fileName = "GameEconomyProfileSO", menuName = "Configs/Economy/GameEconomyProfileSO")]
    public class GameEconomyProfileSO : ScriptableObject
    {
        [Header("Battle Rewards")]
        [SerializeField] private int _coinsPerKill = 10;
        public int CoinsPerKill => _coinsPerKill;
    }
}

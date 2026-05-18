using ACT.Runtime.Gameplay.Battle;
using UnityEngine;

namespace ACT.Runtime.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        private BattleManager _battleManager;
        public void BindBattleManager(BattleManager battleManager) => _battleManager = battleManager;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
        
        }

        // Update is called once per frame
        private void Update()
        {
        
        }
    }
}

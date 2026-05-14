using System.Collections.Generic;
using System.Linq;
using ACT.Runtime.Gameplay.Units;
using UnityEngine;

namespace ACT.Runtime.Gameplay.Battle
{
    // ============================
    //  RANDOM ARMY CALCULATOR
    //  Отбираем рандомную армию в зависимости от заправшиваемой мощности.
    // ============================
    public class RandomArmyCalculator
    {
        private readonly Dictionary<UnitTypes, UnitConfigSO> _configs;

        public RandomArmyCalculator(Dictionary<UnitTypes, UnitConfigSO> configs)
        {
            _configs = configs;
        }

        public List<UnitTypes> GenerateArmy(float targetPower)
        {
            if(targetPower < 0)
            {
                Debug.LogError("RandomArmyCalculater.GenerateArmy error: A targetPower parameter must be a positive number!");
                return null;
            }

            if(_configs == null)
            {
                Debug.LogError("RandomArmyCalculater.GenerateArmy error: An unit configs is null!");
                return null;
            }

            // 1. Сортируем по мощности
            var sorted = _configs
                .OrderByDescending(kv => kv.Value.PowerScore)
                .Select(kv => kv.Key)
                .ToList();

            int count = sorted.Count;

            // 2. Делим на три группы
            int eliteCount = Mathf.CeilToInt(count * 0.20f);
            int guardCount = Mathf.CeilToInt(count * 0.30f);
            int soldierCount = count - eliteCount - guardCount;

            var elite = sorted.Take(eliteCount).ToList();
            var guard = sorted.Skip(eliteCount).Take(guardCount).ToList();
            var soldiers = sorted.Skip(eliteCount + guardCount).Take(soldierCount).ToList();

            // 3. Лимиты мощности
            float eliteLimit = targetPower * 0.20f;//Элитных воинов 20% если вмещаются по заправшиваемой мощи
            float guardLimit = targetPower * 0.30f;//Гвардейцев 30% если вмещаются по заправшиваемой мощи
            float soldierLimit = targetPower * 0.50f;//Пехота - оставшиеся 50%, либо все, если запращиваемая мощность мала

            // 4. Проверяем, помещается ли хотя бы один юнит категории
            bool eliteAllowed = elite.Count > 0 && _configs[elite.Last()].PowerScore <= eliteLimit;
            bool guardAllowed = guard.Count > 0 && _configs[guard.Last()].PowerScore <= guardLimit;
            bool soldierAllowed = soldiers.Count > 0 && _configs[soldiers.Last()].PowerScore <= soldierLimit;

            List<UnitTypes> result = new();
            float currentPower = 0f;

            // 5. ЭЛИТА
            if (eliteAllowed)
            {
                float pwr = 0f;
                float min = _configs[elite.Last()].PowerScore;

                while (pwr + min <= eliteLimit)
                {
                    var type = elite[UnityEngine.Random.Range(0, elite.Count)];
                    float p = _configs[type].PowerScore;

                    if (pwr + p > eliteLimit) break;

                    result.Add(type);
                    pwr += p;
                    currentPower += p;
                }
            }

            // 6. ГВАРДИЯ
            if (guardAllowed)
            {
                float pwr = 0f;
                float min = _configs[guard.Last()].PowerScore;

                while (pwr + min <= guardLimit)
                {
                    var type = guard[UnityEngine.Random.Range(0, guard.Count)];
                    float p = _configs[type].PowerScore;

                    if (pwr + p > guardLimit) break;

                    result.Add(type);
                    pwr += p;
                    currentPower += p;
                }
            }

            // 7. СОЛДАТЫ
            if (soldierAllowed)
            {
                float pwr = 0f;
                float min = _configs[soldiers.Last()].PowerScore;

                while (currentPower < targetPower && pwr + min <= soldierLimit)
                {
                    var type = soldiers[UnityEngine.Random.Range(0, soldiers.Count)];
                    float p = _configs[type].PowerScore;

                    if (currentPower + p > targetPower * 1.05f) break;

                    result.Add(type);
                    pwr += p;
                    currentPower += p;
                }
            }

            return result;
        }
    }
}
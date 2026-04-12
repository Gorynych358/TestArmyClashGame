using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ACT.Scripts
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [SerializeField] private UnitConfigSO[] _configs;
        [SerializeField] private GameObject _unitPrefab;
        [SerializeField] private GameObject _cubeShapePrefab;
        [SerializeField] private GameObject _sphereShapePrefab;

        protected override void Configure(IContainerBuilder builder)
        {

            builder.Register<BattleManager>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<GameEntryPoint>();
            builder.Register<IUnitFactory, UnitFactory>(Lifetime.Singleton);
            builder.Register<UnitObjectPool>(Lifetime.Singleton);
            builder.Register<FormationGenerator>(Lifetime.Singleton);
            builder.Register<ICommandSystem, UnitAICommandSystem>(Lifetime.Transient);
            var configMap = new Dictionary<UnitTypes, UnitConfigSO>();
            foreach (var cfg in _configs)
                configMap[cfg.UnitType] = cfg;

            var prefabsMap = new Dictionary<string, GameObject>
            {
                ["UnitPrefab"] = _unitPrefab,//Контейнер с анимациями в который помещаем модели юнитов
                ["CubePrefab"] = _cubeShapePrefab,//Модель юнита - куб
                ["SpherePrefab"] = _sphereShapePrefab//Модель юнита - сфера
            };

            builder.RegisterInstance(configMap);
            builder.RegisterInstance(prefabsMap);
        }
    }
}

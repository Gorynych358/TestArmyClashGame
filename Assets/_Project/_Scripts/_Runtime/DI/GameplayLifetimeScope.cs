using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ACT.Scripts
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [Header("Unit resources data:")]
        [SerializeField] private UnitConfigSO[] _configs;
        [SerializeField] private GameObject _unitPrefab;
        [SerializeField] private GameObject _cubeShapePrefab;
        [SerializeField] private GameObject _sphereShapePrefab;
        [Header("UI coins pool data:")]
        [SerializeField] private int _coinPrewarmCount = 20;
        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private RectTransform _coinPoolStorage;
        [Header("Spatial Grid Settings")]
        [SerializeField] private float _spatialGridCellSize = 3f;
        [Header("Steering Settings")]
        [SerializeField] SteeringBehaviorProfile _steeringProfile;

        protected override void Configure(IContainerBuilder builder)
        {
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
            builder.RegisterInstance(_steeringProfile);

            builder.RegisterComponentInHierarchy<BattleManager>();
            
            // UI coins pool dependencies:
            builder.RegisterInstance(_coinPrefab);
            builder.RegisterInstance(_coinPoolStorage);

            builder.Register<CoinObjectPool>(Lifetime.Singleton)
                .WithParameter(_coinPrefab)
                .WithParameter(_coinPoolStorage)
                .WithParameter(_coinPrewarmCount);
            //UI views:
            builder.RegisterComponentInHierarchy<BattleProgressView>();
            builder.RegisterComponentInHierarchy<CoinsView>();
            builder.RegisterComponentInHierarchy<UIVisualEffectsService>();
            builder.RegisterComponentInHierarchy<FightButtonView>();
            //UI presenters:
            builder.Register<BattleProgressPresenter>(Lifetime.Singleton);
            builder.Register<CoinsPresenter>(Lifetime.Singleton);
            builder.Register<FightButtonPresenter>(Lifetime.Singleton);
            //Services:
            // SpatialGrid - service for fast neighbor search in battle:
            builder.Register<SpatialGrid>(Lifetime.Singleton)
                .WithParameter(_spatialGridCellSize);
            builder.Register<EconomyManager>(Lifetime.Singleton);
            builder.Register<IUnitFactory, UnitFactory>(Lifetime.Singleton);
            builder.Register<UnitObjectPool>(Lifetime.Singleton);
            builder.Register<FormationGenerator>(Lifetime.Singleton);
            builder.Register<ICommandSystem, UnitAICommandSystem>(Lifetime.Transient);
            //Scene context entry point:
            builder.RegisterEntryPoint<GameBootstrap>();
        }
    }
}

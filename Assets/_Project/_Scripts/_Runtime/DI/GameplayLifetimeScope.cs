using System.Collections.Generic;
using ACT.Runtime.Gameplay;
using ACT.Runtime.Gameplay.Battle;
using ACT.Runtime.Gameplay.Battle.Formations;
using ACT.Runtime.Gameplay.UI;
using ACT.Runtime.Gameplay.UI.Presenters;
using ACT.Runtime.Gameplay.UI.Views;
using ACT.Runtime.Gameplay.Units;
using ACT.Runtime.Gameplay.Units.Executors;
using ACT.Runtime.Gameplay.Units.Logic;
using ACT.Runtime.Infrastructure.Economy;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ACT.Runtime.DI
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
        [Header("Economy Settings")]
        [SerializeField] GameEconomyProfileSO _economyProfile;

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
            builder.RegisterInstance(_economyProfile);

            builder.RegisterComponentInHierarchy<BattleManager>();
            
            // UI coins pool dependencies:
            builder.RegisterInstance(_coinPrefab);
            builder.RegisterInstance(_coinPoolStorage);

            builder.Register<CoinObjectPool>(Lifetime.Singleton)
                .WithParameter(_coinPrefab)
                .WithParameter(_coinPoolStorage)
                .WithParameter(_coinPrewarmCount);
            
            builder.RegisterComponentInHierarchy<UIVisualEffectsService>();
            //UI views:
            builder.RegisterComponentInHierarchy<BattleProgressView>();
            builder.RegisterComponentInHierarchy<CoinsView>();
            builder.RegisterComponentInHierarchy<FightButtonView>();
            builder.RegisterComponentInHierarchy<ChangeFormationView>();
            builder.RegisterComponentInHierarchy<BattleCompleteView>();
            //UI presenters:
            builder.Register<BattleProgressPresenter>(Lifetime.Singleton);
            builder.Register<CoinsPresenter>(Lifetime.Singleton);
            builder.Register<FightButtonPresenter>(Lifetime.Singleton);
            builder.Register<ChangeFormationPresenter>(Lifetime.Singleton);
            builder.Register<BattleCompletePresenter>(Lifetime.Singleton);
            //Services:
            // SpatialGrid - service for fast neighbor search in battle:
            builder.Register<SpatialGrid>(Lifetime.Singleton)
                .WithParameter(_spatialGridCellSize);
            builder.Register<EconomyManager>(Lifetime.Singleton);
            builder.Register<IUnitFactory, UnitFactory>(Lifetime.Singleton);
            builder.Register<UnitObjectPool>(Lifetime.Singleton);
            builder.Register<RandomArmyCalculator>(Lifetime.Singleton);
            builder.Register<FormationBuilder>(Lifetime.Singleton);
            builder.Register<ArmySpawner>(Lifetime.Singleton);
            //Common unit services
            builder.Register<ICommandSystem, UnitAICommandSystem>(Lifetime.Singleton);
            builder.Register<IAttackSystem, UnitAttacker>(Lifetime.Singleton);
            builder.Register<IMoveSystem, UnitMover>(Lifetime.Singleton);
            //Scene context entry point:
            builder.RegisterEntryPoint<GameplayBootstrap>();
        }
    }
}

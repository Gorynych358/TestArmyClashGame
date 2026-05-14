using UnityEngine;
using VContainer;
using VContainer.Unity;
using ACT.Runtime.Infrastructure;
using ACT.Runtime.Infrastructure.Audio;
using ACT.Runtime.Infrastructure.Economy;
using ACT.Runtime.Infrastructure.EventBus;
using ACT.Runtime.Infrastructure.SceneManagement;

namespace ACT.Runtime.DI
{
    
    public sealed class RootLifetimeScope : LifetimeScope
    {
        [SerializeField] private AudioLibrary _audioLibrary;
        protected override void Configure(IContainerBuilder builder)
        {
            //Шина событий:
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);
            //Библиотека игровых звуков:
            builder.RegisterInstance(_audioLibrary);
            //Ссылки на AudioSource для музыки и звуков. И конфиг настроек громкости:
            builder.RegisterComponentInHierarchy<AudioRoot>();
            //Звуковой менеджер. Управляет всеми звуками и фоновой музыкой.
            builder.Register<ISoundManager, SoundManager>(Lifetime.Singleton);
            //Вьюха смены сцен. Затемнение экрана + прогресс бар загрузки сцены:
            builder.RegisterComponentInHierarchy<SceneTransitionView>();
            //Менеджер перехода между сценами. Асинхронная загрузка сцен + UniTask-и:
            builder.Register<ISceneTransitionManager, SceneTransitionManager>(Lifetime.Singleton);
            //Экономика игры, монетки, ресурсы и т.п.:
            builder.Register<IEconomyManager, EconomyManager>(Lifetime.Singleton);
            //Точка входа в приложение. 
            builder.RegisterEntryPoint<AppEntryPoint>();
        }
    }
}

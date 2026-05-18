using VContainer;
using VContainer.Unity;
using ACT.Runtime.MainMenu;
using ACT.Runtime.MainMenu.Views;
using ACT.Runtime.MainMenu.Presenters;

namespace ACT.Runtime.DI
{
    public class MainMenuLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            //MainMenu logic
            builder.RegisterComponentInHierarchy<MainMenuManager>();
            //UI views:
            builder.RegisterComponentInHierarchy<PlayButtonView>();
            builder.RegisterComponentInHierarchy<SelectArmyPowerView>();
            //UI presenters:
            builder.Register<PlayButtonPresenter>(Lifetime.Singleton);
            builder.Register<SelectArmyPowerPresenter>(Lifetime.Singleton);
            //Scene context entry point:
            builder.RegisterEntryPoint<MainMenuBootstrap>();
        }
    }
}

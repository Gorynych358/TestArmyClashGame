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
            builder.RegisterComponentInHierarchy<MainMenuManager>();
            //UI view:
            builder.RegisterComponentInHierarchy<PlayButtonView>();
            //UI presenter:
            builder.Register<PlayButtonPresenter>(Lifetime.Singleton);
            //Scene context entry point:
            builder.RegisterEntryPoint<MainMenuBootstrap>();
        }
    }
}

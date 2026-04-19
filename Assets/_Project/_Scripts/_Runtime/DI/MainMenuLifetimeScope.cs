using VContainer;
using VContainer.Unity;

namespace ACT.Scripts
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

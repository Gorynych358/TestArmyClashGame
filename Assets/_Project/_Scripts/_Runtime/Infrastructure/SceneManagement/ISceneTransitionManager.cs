using Cysharp.Threading.Tasks;

namespace ACT.Runtime.Infrastructure.SceneManagement
{
    public interface ISceneTransitionManager
    {
        UniTask LoadMainMenu();
        UniTask LoadGameplay();
    }
}

using Cysharp.Threading.Tasks;

namespace ACT.Scripts
{
    public interface ISceneTransitionManager
    {
        UniTask LoadMainMenu();
        UniTask LoadGameplay();
    }
}

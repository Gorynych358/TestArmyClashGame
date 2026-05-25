using UnityEngine;
using VContainer.Unity;

namespace ACT.Runtime.Infrastructure.DebugUtils
{
    public sealed class PerformanceOverlayPresenter : ITickable
    {
        private readonly PerformanceOverlayView _view;

        private float _smoothedDeltaTime;

        private float _timer;

        private const float UpdateInterval = 0.25f;

        public PerformanceOverlayPresenter(
            PerformanceOverlayView view)
        {
            _view = view;
        }

        public void Tick()
        {
            float deltaTime = Time.unscaledDeltaTime;

            _smoothedDeltaTime +=
                (deltaTime - _smoothedDeltaTime) * 0.1f;

            _timer += deltaTime;

            if (_timer < UpdateInterval)
                return;

            int fps = Mathf.RoundToInt(1f / _smoothedDeltaTime);

            float milliseconds = _smoothedDeltaTime * 1000f;

            _view.SetFPS(fps);
            _view.SetMilliseconds(milliseconds);

            _timer = 0f;
        }
    }
}
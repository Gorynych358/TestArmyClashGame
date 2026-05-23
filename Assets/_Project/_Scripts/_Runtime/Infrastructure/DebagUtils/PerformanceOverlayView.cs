using TMPro;
using UnityEngine;

namespace ACT.Runtime.Infrastructure.DebugUtils
{
    public sealed class PerformanceOverlayView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _fpsValueText;
        [SerializeField] private TMP_Text _msValueText;

        public void SetFPS(int fps)
        {
            _fpsValueText.text = fps.ToString();
        }

        public void SetMilliseconds(float ms)
        {
            _msValueText.text = ms.ToString("F1");
        }
    }
}
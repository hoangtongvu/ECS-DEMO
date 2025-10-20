using Core.UI.TextMeshProUGUIs;
using UnityEngine;

namespace Core.UI.GameOverPanel
{
    public class TotalWaveSurvivedText : BaseTextMeshProUGUI
    {
        public void SetWaveCount(int value)
        {
            this.text.text = $"{value}";
        }
    }
}
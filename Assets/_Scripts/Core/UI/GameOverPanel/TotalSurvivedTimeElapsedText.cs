using Core.UI.TextMeshProUGUIs;
using UnityEngine;

namespace Core.UI.GameOverPanel
{
    public class TotalSurvivedTimeElapsedText : BaseTextMeshProUGUI
    {
        public void SetTimeElapsed(double value)
        {
            this.text.text = $"{value:0.##} seconds";
        }
    }
}
using Core.UI.TextMeshProUGUIs;
using UnityEngine;

namespace Core.UI.GameOverPanel.QuitGameTimer
{
    public class TimerText : BaseTextMeshProUGUI
    {
        public void SetTime(float value)
        {
            this.text.text = $"{value:0.##}";
        }
    }
}
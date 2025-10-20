using LitMotion;

namespace Core.UI.GameOverPanel;

public class GameOverPanelTweener
{
    public GameOverPanelCtrl GameOverPanelCtrl;

    public MotionHandle MotionHandle;
    public float Duration = 1f;
    public float StartAlpha = 0;
    public float EndAlpha = 1f;
    public Ease Ease;

    public void TriggerTweenOnAppear()
    {
        this.MotionHandle.TryCancel();

        float startValue = this.StartAlpha;
        float endValue = this.EndAlpha;

        this.MotionHandle = LMotion.Create(startValue, endValue, this.Duration)
            .WithEase(this.Ease)
            .Bind(tempValue => this.GameOverPanelCtrl.CanvasGroup.alpha = tempValue);
    }
}
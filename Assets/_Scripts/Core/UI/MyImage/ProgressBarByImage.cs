
namespace Core.UI.MyImage
{
    public class ProgressBarByImage : BaseImage
    {
        public void SetProgress(float value) => this.image.fillAmount = value;

        public void ClearProgress() => this.SetProgress(0);

    }
}
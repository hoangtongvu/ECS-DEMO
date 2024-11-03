using Core.UI.MyImage;

namespace Core.UI.FlowField.GridNodePresenter
{
    public class TargetMarkImage : BaseImage
    {
        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);

    }


}
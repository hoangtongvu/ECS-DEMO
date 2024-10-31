using Core.Misc.FlowField;
using Core.UI.MyImage;
using UnityEngine;

namespace Core.UI.FlowField.GridNodePresenter
{
    public class DirectionImage : BaseImage
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private SimpleDirection simpleDirection;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(ref this.rectTransform);

        }

        public void SetDirection(SimpleDirection simpleDirection)
        {
            float rotateAngle = simpleDirection switch
            {
                SimpleDirection.Top => 0,
                SimpleDirection.Bottom => 180,
                SimpleDirection.Left => 90,
                SimpleDirection.Right => -90,
                SimpleDirection.TopLeft => 45,
                SimpleDirection.TopRight => -45,
                SimpleDirection.BottomLeft => 135,
                SimpleDirection.BottomRight => -135,
                _ => 0,
            };

            this.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotateAngle);

        }




    }


}
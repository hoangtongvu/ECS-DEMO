using Core.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.MyImage
{
    public class BaseImage : SaiMonoBehaviour
    {
        [SerializeField] protected Image image;

        public Image Image => image;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(out this.image);
        }

    }

}
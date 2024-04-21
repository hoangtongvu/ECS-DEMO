using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.MyImage
{
    public class BaseImage : SaiMonoBehaviour
    {
        [SerializeField] protected Image image;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(ref this.image);
        }

    }
}
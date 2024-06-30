using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.LegacyText
{
    public class BaseTextWithId : BaseUICtrl
    {
        [Header("Base Text")]
        [SerializeField] protected Text text;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(ref this.text);
        }

    }
}
using TMPro;
using UnityEngine;

namespace Core.UI.TextMeshProUGUIs
{
    public abstract class BaseTextMeshProUGUI : BaseUICtrl
    {
        [Header("Base TextMeshPro")]
        [SerializeField] protected TextMeshProUGUI text;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(ref this.text);
        }

    }
}
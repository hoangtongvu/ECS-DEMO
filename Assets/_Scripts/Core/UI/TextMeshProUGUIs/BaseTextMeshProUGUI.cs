using Core.Misc;
using TMPro;
using UnityEngine;

namespace Core.UI.TextMeshProUGUIs
{
    public class BaseTextMeshProUGUI : SaiMonoBehaviour
    {
        [Header("Base TextMeshPro")]
        [SerializeField] protected TextMeshProUGUI text;

        public TextMeshProUGUI TextMeshProUGUI => text;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(out this.text);
        }

    }

}
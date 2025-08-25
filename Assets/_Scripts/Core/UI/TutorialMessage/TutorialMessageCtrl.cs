using EasyTextEffects;
using TMPro;
using UnityEngine;

namespace Core.UI.TutorialMessage
{
    [GenerateUIType("TutorialMessage")]
    [RequireComponent(typeof(TextMeshProUGUI))]
    [RequireComponent(typeof(TextEffect))]
    [RequireComponent(typeof(TutorialMessageTextEffectHandler))]
    public partial class TutorialMessageCtrl : BaseUICtrl
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;
        [SerializeField] private TutorialMessageTextEffectHandler tutorialMessageTextEffectHandler;

        public TextMeshProUGUI TextMeshPro => textMeshPro;
        public TutorialMessageTextEffectHandler TutorialMessageTextEffectHandler => tutorialMessageTextEffectHandler;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(ref this.textMeshPro);
            this.LoadComponentInCtrl(ref this.tutorialMessageTextEffectHandler);
        }

        public override void OnRent()
        {
        }

        public override void OnReturn()
        {
        }
    }
}

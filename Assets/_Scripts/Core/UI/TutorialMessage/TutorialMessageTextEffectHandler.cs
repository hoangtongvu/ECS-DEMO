using Core.Misc;
using Core.Misc.TutorialMessage;
using Core.MyEvent.PubSub.Messengers;
using EasyTextEffects;
using EasyTextEffects.Effects;
using TMPro;
using UnityEngine;
using ZBase.Foundation.PubSub;
using static EasyTextEffects.TextEffectEntry;

namespace Core.UI.TutorialMessage
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    [RequireComponent(typeof(TextEffect))]
    public class TutorialMessageTextEffectHandler : SaiMonoBehaviour
    {
        [SerializeField] private TutorialMessageCtrl tutorialMessageCtrl;
        [SerializeField] private TextEffect textEffect;

        [Header("TextEffectInstances")]
        [SerializeField] private TextEffectInstance appearEffect;
        [SerializeField] private TextEffectInstance mainLoopTextEffect;
        [SerializeField] private TextEffectInstance infFadeInOutEffect;
        [SerializeField] private TextEffectInstance disappearEffect;

        [Header("TextEffectEntries")]
        [SerializeField] private GlobalTextEffectEntry onAppearEffectEntry;
        [SerializeField] private GlobalTextEffectEntry disappearEffectEntry;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(out this.tutorialMessageCtrl);
            this.LoadComponentInCtrl(out this.textEffect);
            this.LoadEffects();
        }

        private void LoadEffects()
        {
            this.textEffect.globalEffects.Clear();

            this.onAppearEffectEntry = new GlobalTextEffectEntry()
            {
                effect = this.appearEffect,
                triggerWhen = TriggerWhen.OnStart,
            };
            this.textEffect.globalEffects.Add(this.onAppearEffectEntry);

            this.textEffect.globalEffects.Add(new()
            {
                effect = this.mainLoopTextEffect,
                triggerWhen = TriggerWhen.OnStart,
            });

            this.textEffect.globalEffects.Add(new GlobalTextEffectEntry()
            {
                effect = this.infFadeInOutEffect,
                triggerWhen = TriggerWhen.Manual,
            });

            this.disappearEffectEntry = new GlobalTextEffectEntry()
            {
                effect = this.disappearEffect,
                triggerWhen = TriggerWhen.Manual,
            };
            this.textEffect.globalEffects.Add(this.disappearEffectEntry);

            this.textEffect.Refresh();
        }

        public void TriggerTextDisappear()
        {
            this.textEffect.StopManualEffects();
            this.textEffect.FindManualEffect(this.disappearEffect.effectTag).StartEffect();
        }

        // NOTE: As TextEffect package was made the best for Editor, this function is for assign to UnityEvent in the Editor only
        public void StartInfFadeInOutEffect()
        {
            this.textEffect.FindManualEffect(this.infFadeInOutEffect.effectTag).StartEffect();
        }

        // NOTE: As TextEffect package was made the best for Editor, this function is for assign to UnityEvent in the Editor only
        public void HandleOnMessageDisappeared()
        {
            GameplayMessenger.MessagePublisher
                .Publish<TutorialMessageDespawnedMessage>();
        }
    }
}

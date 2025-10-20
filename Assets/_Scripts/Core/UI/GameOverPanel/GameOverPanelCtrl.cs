using UnityEngine;

namespace Core.UI.GameOverPanel
{
    [GenerateUIType("GameOverPanel")]
    public partial class GameOverPanelCtrl : BaseUICtrl
    {
        private GameOverPanelTweener gameOverPanelTweener;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TotalSurvivedTimeElapsedText totalSurvivedTimeElapsedText;
        [SerializeField] private TotalWaveSurvivedText totalWaveSurvivedText;

        public CanvasGroup CanvasGroup => canvasGroup;
        public TotalSurvivedTimeElapsedText TotalSurvivedTimeElapsedText => totalSurvivedTimeElapsedText;
        public TotalWaveSurvivedText TotalWaveSurvivedText => totalWaveSurvivedText;

        protected override void Awake()
        {
            base.Awake();
            this.gameOverPanelTweener = new()
            {
                GameOverPanelCtrl = this,
            };
        }

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(ref this.canvasGroup);
            this.LoadComponentInChildren(ref this.totalSurvivedTimeElapsedText);
            this.LoadComponentInChildren(ref this.totalWaveSurvivedText);
        }

        public override void OnRent()
        {
            this.gameOverPanelTweener.TriggerTweenOnAppear();
        }

        public override void OnReturn()
        {
        }

    }

}
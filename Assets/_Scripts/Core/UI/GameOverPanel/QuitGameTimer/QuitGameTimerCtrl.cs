using Core.Misc;
using System;
using UnityEngine;

namespace Core.UI.GameOverPanel.QuitGameTimer
{
    public class QuitGameTimerCtrl : SaiMonoBehaviour
    {
        [Serializable]
        private class Timer
        {
            public bool IsActivated = true;
            public float MaxTimeSeconds = 7f;
            public float TimeCounterSeconds;

            public event Action OnTimeUp;

            public void InitState() => this.TimeCounterSeconds = this.MaxTimeSeconds;

            public void Tick(in float deltaTime)
            {
                if (!this.IsActivated) return;

                this.TimeCounterSeconds -= deltaTime;

                if (this.TimeCounterSeconds > 0) return;

                this.TimeCounterSeconds = 0;
                this.IsActivated = false;
                this.OnTimeUp?.Invoke();
            }
        }

        [SerializeField] private Timer timer = new();
        [SerializeField] private TimerText timerText;

        private void Awake()
        {
            this.timer.InitState();
            this.timer.OnTimeUp += ExitApplication;
        }

        private void OnDestroy()
        {
            this.timer.OnTimeUp -= ExitApplication;
        }

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.timerText);
        }

        private void FixedUpdate()
        {
            if (!this.timer.IsActivated) return;

            this.timer.Tick(Time.fixedDeltaTime);
            this.UpdateText();
        }

        private void UpdateText()
        {
            this.timerText.SetTime(this.timer.TimeCounterSeconds);
        }

        private static void ExitApplication() => Application.Quit();

    }

}
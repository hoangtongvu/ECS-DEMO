using JSAM;
using Audio.JSAM;
using UnityEngine;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;

namespace Core.Misc
{
    public class SimpleBGMPlayer : SaiMonoBehaviour
    {
        private HashSet<int> playedBGMs = new();
        private Unity.Mathematics.Random rand = new(37);
        [SerializeField] private float loopCheckDelay = 1f;

        [SerializeField] private int bgmCount;
        [SerializeField] private int currentBGMIndex;

        private void Awake()
        {
            this.bgmCount = Enum.GetNames(typeof(BGM_LibraryMusic)).Length;
        }

        private void Start()
        {
            this.PlayLoopAsync().Forget();
        }

        private async UniTaskVoid PlayLoopAsync()
        {
            while (true)
            {
                bool isCurrentBGMPlaying = AudioManager.IsMusicPlaying(this.GetCurrentMusicType());

                if (!isCurrentBGMPlaying)
                {
                    this.RollNextMusicIndex(out this.currentBGMIndex);
                    this.playedBGMs.Add(this.currentBGMIndex);
                    AudioManager.PlayMusic(this.GetCurrentMusicType());
                }

                await UniTask.WaitForSeconds(this.loopCheckDelay);
            }
        }

        private BGM_LibraryMusic GetCurrentMusicType()
        {
            return (BGM_LibraryMusic)this.currentBGMIndex;
        }

        private void RollNextMusicIndex(out int newIndex)
        {
            if (this.playedBGMs.Count == this.bgmCount) this.playedBGMs.Clear();

            do
            {
                newIndex = this.rand.NextInt(0, this.bgmCount);
            } while (this.playedBGMs.Contains(newIndex));
        }

    }

}

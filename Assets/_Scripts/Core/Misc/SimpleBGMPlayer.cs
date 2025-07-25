using JSAM;
using Audio.JSAM;
using UnityEngine;

namespace Core.Misc
{
    public class SimpleBGMPlayer : SaiMonoBehaviour
    {
        [SerializeField] private BGM_LibraryMusic musicType;
        public bool IsPlaying { get; private set; } = false;

        public void TogglePlayMusic()
        {
            if (!this.IsPlaying)
            {
                AudioManager.PlayMusic(this.musicType);
                this.IsPlaying = true;
            }
            else
            {
                AudioManager.FadeMusicOut(this.musicType, 5);
                this.IsPlaying = false;
            }
        }

    }

}

using UnityEngine;
using System;
using Core.Misc;

namespace Core.Animator
{
    public class BaseAnimator : SaiMonoBehaviour
    {
        [SerializeField] protected UnityEngine.Animator animator;
        [SerializeField] protected AnimationClip[] clipArray;
        [SerializeField] protected string currentStateName;

        #region LoadComponents

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(ref this.animator);
            this.LoadAnimationClip();
        }

        protected virtual void LoadAnimationClip()
        {
            this.clipArray = animator.runtimeAnimatorController.animationClips;
        }

        #endregion LoadComponents

        public virtual void ChangeAnimationState<TEnum>(TEnum @newState) where TEnum : Enum
        {
            string newStateName = newState.ToString();
            if (currentStateName == newStateName) return;
            animator.Play(newStateName);
            currentStateName = newStateName;
        }

        public virtual float GetAnimationLength<TEnum>(TEnum @newState) where TEnum : Enum
        {
            string aniName = newState.ToString();
            foreach (AnimationClip clip in clipArray)
            {
                if (clip.name == aniName)
                {
                    return clip.length;
                }
            }
            Debug.LogError($"{transform.parent.name} can't find animationClip with name: {aniName}");
            return 0;
        }

        public virtual void PlayImmediately(string newStateName)
        {
            if (currentStateName == newStateName) return;
            animator.Play(newStateName);
            currentStateName = newStateName;
        }
        
        public virtual void WaitPlay(string newStateName, float transitionDuration)
        {
            if (currentStateName == newStateName) return;
            animator.CrossFadeInFixedTime(newStateName, transitionDuration);
            currentStateName = newStateName;
        }

        public virtual float GetAnimationLength_Old(string aniName)
        {
            foreach (AnimationClip clip in clipArray)
            {
                if (clip.name == aniName)
                {
                    return clip.length;
                }
            }
            Debug.LogError($"{transform.parent.name} can't find animationClip with name: {aniName}");
            return 0;
        }

        public virtual float GetAnimationLength(string aniName)
        {

            int length = this.clipArray.Length;

            for (int i = 0; i < length; i++)
            {
                var clip = clipArray[i];
                if (clip.name != aniName) continue;
                return clip.length;
            }

            Debug.LogError($"Can't find animationClip in {transform.parent.name} with name: {aniName}", transform.parent.gameObject);
            return -1;

        }

        public bool IsAnimatorPlaying()
        {
            return this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1 && !this.animator.IsInTransition(0);
        }

    }

}
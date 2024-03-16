using UnityEngine;
using System;
using System.Collections.Generic;

namespace Core.Animator
{
    public class BaseAnimator : SaiMonoBehaviour
    {
        [SerializeField] protected UnityEngine.Animator animator;
        [SerializeField] protected AnimationClip[] clipArray;
        [SerializeField] protected string currentStateName;

        [SerializeField] protected List<AnimationClip> clipList;

        public List<AnimationClip> ClipList => clipList;

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
            foreach (AnimationClip clip in clipArray) this.clipList.Add(clip);
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



        public virtual void ChangeAnimationState(string newStateName)
        {
            if (currentStateName == newStateName) return;
            animator.Play(newStateName);
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
            var clip = this.clipList.Find(ac => ac.name == aniName);
            if (clip == null)
            {
                Debug.LogError($"Can't find animationClip in {transform.parent.name} with name: {aniName}", transform.parent.gameObject);
                return -1;
            }


            return clip.length;

        }

        public bool IsAnimatorPlaying()
        {
            return this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1 && !this.animator.IsInTransition(0);
        }

    }
}
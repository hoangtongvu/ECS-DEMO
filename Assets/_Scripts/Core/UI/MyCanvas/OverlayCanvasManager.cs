using Core.Misc;
using System;
using System.ComponentModel;
using UnityEngine;

namespace Core.UI.MyCanvas
{
	public class OverlayCanvasManager : SaiMonoBehaviour
    {
		[SerializeField] [ReadOnly(true)] private Transform[] anchorPresetTransforms = new Transform[9];

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadAnchorPresets();
        }

        private void LoadAnchorPresets()
        {
            var length = Enum.GetNames(typeof(CanvasAnchorPreset)).Length;

            for (int i = 0; i < length; i++)
            {
                var preset = (CanvasAnchorPreset)i;
                string gameObjName = preset.ToString();
                this.LoadTransformInChildrenByName(out this.anchorPresetTransforms[i], gameObjName);
            }
        }

        public Transform GetAnchorPresetTransform(int index)
        {
            var anchorPreset = anchorPresetTransforms[index];
            var presetType = (CanvasAnchorPreset)index;

            if (anchorPreset == null)
            {
                Debug.LogError($"Can't find CanvasAnchorPreset: <b>{presetType}</b>", this);
            }

            return anchorPreset;
        }

    }

}
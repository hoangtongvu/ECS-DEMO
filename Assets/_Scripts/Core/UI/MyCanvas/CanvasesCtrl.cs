using Core.Misc;
using UnityEngine;

namespace Core.UI.MyCanvas
{
    public class CanvasesCtrl : SaiMonoBehaviour
    {
        [SerializeField] private OverlayCanvasManager overlayCanvasManager;
        [SerializeField] private Transform worldSpaceCanvasTransform;

        private static CanvasesCtrl instance;

        public OverlayCanvasManager OverlayCanvasManager => overlayCanvasManager;
        public Transform WorldSpaceCanvasTransform => worldSpaceCanvasTransform;
        public static CanvasesCtrl Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<CanvasesCtrl>();
                return instance;
            }
        }

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.overlayCanvasManager);
            this.LoadTransformInChildrenByName(out this.worldSpaceCanvasTransform, "WorldSpaceCanvas");
        }

    }

}
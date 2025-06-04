using Core.Misc;
using UnityEngine;

namespace Core.MyCamera
{
    public class CameraHolderCtrl : SaiMonoBehaviour
    {
        private static CameraHolderCtrl instance;

        [SerializeField] private Camera mainCam;

        public Camera MainCam => mainCam;
        public static CameraHolderCtrl Instance
        {
            get
            {
                if (instance == null) instance = FindObjectOfType<CameraHolderCtrl>();
                return instance;
            }
        }

        protected override void LoadComponents()
        {
            base.LoadComponents();
            LoadMainCam();
        }

        private void LoadMainCam()
        {
            mainCam = transform.Find("Main Camera").GetComponent<Camera>();
        }

    }

}
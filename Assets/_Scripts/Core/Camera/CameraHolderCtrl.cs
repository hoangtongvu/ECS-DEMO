using Core;
using UnityEngine;

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
        this.LoadMainCam();
    }

    private void LoadMainCam()
    {
        this.mainCam = transform.Find("Main Camera").GetComponent<Camera>();
    }
}

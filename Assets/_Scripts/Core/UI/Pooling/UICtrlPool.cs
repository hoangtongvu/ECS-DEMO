using DSPool;
using UnityEngine;

namespace Core.UI.Pooling;

public class UICtrlPool : ComponentPool<BaseUICtrl>
{
    public Transform DefaultHolderTransform { get; set; }

    protected override BaseUICtrl InstantiateElement()
    {
        return Object.Instantiate(this.Prefab, this.DefaultHolderTransform, false).GetComponent<BaseUICtrl>();
    }

    protected override void OnReturn(BaseUICtrl element)
    {
        base.OnReturn(element);
        element.transform
            .SetParent(this.DefaultHolderTransform);
    }
}
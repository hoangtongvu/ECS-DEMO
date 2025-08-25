using DSPool;
using UnityEngine;

namespace Core.UI.Pooling;

public class UICtrlPool : ComponentPool<BaseUICtrl>
{
    public uint GlobalID;
    public Transform DefaultHolderTransform { get; set; }

    protected override BaseUICtrl InstantiateElement()
    {
        var baseUICtrl = Object.Instantiate(this.Prefab, this.DefaultHolderTransform, false).GetComponent<BaseUICtrl>();

        this.GlobalID++;
        baseUICtrl.RuntimeUIID.Type = baseUICtrl.GetUIType();
        baseUICtrl.RuntimeUIID.LocalId = this.GlobalID;

        return baseUICtrl;
    }

    protected override void OnRent(BaseUICtrl element)
    {
        base.OnRent(element);
        element.OnRent();
    }

    protected override void OnReturn(BaseUICtrl element)
    {
        base.OnReturn(element);
        element.OnReturn();
        element.transform
            .SetParent(this.DefaultHolderTransform);
    }
}
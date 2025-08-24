using Core.UI.Identification;
using DSPool;

namespace Core.UI.Pooling
{
    [DSPoolSingleton]
    public partial class UICtrlPoolMap : PoolMap<UIType, UICtrlPool, BaseUICtrl>
    {
    }
}
using UnityEngine;
using DSPool;

namespace Core.Misc.Presenter
{
    [DSPoolSingleton]
    public partial class BasePresenterPoolMap : PoolMap<GameObject, ComponentPool<BasePresenter>, BasePresenter>
    {
    }
}
using UnityEngine;
using DSPool;

namespace Core.Misc.Presenter
{
    [DSPoolSingleton]
    public partial class BasePresenterPoolMap : PoolMap<GameObject, MonoPool<BasePresenter>, BasePresenter>
    {
    }
}
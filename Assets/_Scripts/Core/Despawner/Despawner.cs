using Core.Misc;

namespace Core.Despawner
{
    public abstract class Despawner : SaiMonoBehaviour
    {
        public virtual void DespawnObject()
        {
            transform.parent.gameObject.SetActive(false);
        }

        protected abstract bool CanDespawn();

    }

}
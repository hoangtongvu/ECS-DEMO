using System.Collections.Generic;
using UnityEngine;

namespace Core.Misc
{
    public class ObjPool<T> : SaiMonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private Queue<T> objs = new();

        public void AddToPool(T t) => objs.Enqueue(t);

        public bool TryGetFromPool(out T t)
        {
            return objs.TryDequeue(out t);
        }

    }

}
using Core;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class ObjPool<T> : SaiMonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private Queue<T> objs = new();

        public void AddToPool(T t) => this.objs.Enqueue(t);

        public bool TryGetFromPool(out T t)
        {
            return this.objs.TryDequeue(out t);
        }

    }
}
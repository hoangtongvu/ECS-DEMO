using System.Collections.Generic;
using UnityEngine;

namespace Core.Spawner
{

    // Store prefabs in Dictionary instead, use int/enum as key.
    public abstract class SpawnerGeneric<T> : SaiMonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] protected Transform holder;
        [SerializeField] protected int spawnedCount = 0;
        [SerializeField] protected List<T> prefabs;
        [SerializeField] protected List<T> poolObjs;


        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadPrefabs();
            this.LoadHolder();
        }

        protected virtual void LoadPrefabs()
        {
            Transform prefabObj = transform.Find("Prefabs");
            foreach (Transform prefab in prefabObj)
            {
                this.prefabs.Add(prefab.GetComponent<T>());
            }
            this.HidePrefabs();
        }

        protected virtual void LoadHolder() => this.holder = transform.Find("Holder");

        protected virtual void HidePrefabs() => this.prefabs.ForEach(p => p.gameObject.SetActive(false));


        public virtual T Spawn(string prefabName, Vector3 spawnPos, Quaternion rotation)
        {
            T prefab = this.GetPrefabByName(prefabName);
            if (prefab == null)
            {
                Debug.LogWarning("Prefab not found: " + prefabName);
                return null;
            }

            return this.Spawn(prefab.transform, spawnPos, rotation);
        }


        public virtual T Spawn(Transform prefab, Vector3 spawnPos, Quaternion rotation)
        {
            T newPrefab = this.GetObjectFromPool(prefab);
            newPrefab.transform.SetPositionAndRotation(spawnPos, rotation);

            newPrefab.transform.SetParent(this.holder);
            this.spawnedCount++;
            return newPrefab;

        }

        protected virtual T GetPrefabByName(string prefabName) => this.prefabs.Find(p => p.transform.name == prefabName);


        protected virtual T GetObjectFromPool(Transform prefab)
        {

            for (int i = 0; i < this.poolObjs.Count; i++)
            {
                T poolObj = this.poolObjs[i];
                if (poolObj.name != prefab.name) continue;
                this.poolObjs.Remove(poolObj);
                return poolObj;
            }

            T newPrefab = Instantiate(prefab).GetComponent<T>();
            newPrefab.name = prefab.name;
            return newPrefab;
        }

        public virtual void Despawn(T component)
        {
            if (this.poolObjs.Find(p => p == component)) return;

            this.poolObjs.Add(component);
            component.gameObject.SetActive(false);
            this.spawnedCount--;
            //if (this.spawnedCount < 0) Debug.LogError("SpawnCount < 0 in spawner: " + transform.name);
        }

        public virtual void Despawn(Transform transform)
        {
            if (this.poolObjs.Find(p => p == transform)) return;

            this.poolObjs.Add(transform.GetComponent<T>());
            transform.gameObject.SetActive(false);
            this.spawnedCount--;
            //if (this.spawnedCount < 0) Debug.LogError("SpawnCount < 0 in spawner: " + transform.name);
        }

        public virtual T RandomPrefab()
        {
            int randIndex = Random.Range(0, this.prefabs.Count);
            return this.prefabs[randIndex];
        }


    }
}
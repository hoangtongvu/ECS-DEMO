using Components.ComponentMap;
using Core.UI;
using Core.UI.Identification;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Core
{
    public class UIPoolMapRegister : MonoBehaviour
    {
        [System.Serializable]
        private class RegisterValue
        {
            public UIType Type;
            public GameObject Prefab;
            public ObjPool<BaseUICtrl> UIPool;
        }

        [SerializeField] private List<RegisterValue> tempWrappers;
        private EntityManager em;


        private void Awake()
        {
            this.em = World.DefaultGameObjectInjectionWorld.EntityManager;

            if (this.tempWrappers.Count == 0) return;


            UIPoolMap uiMap = this.GetMap();

            if (uiMap == null)
            {
                Debug.LogError("UIPoolMap not found");
                return;
            }

            foreach (var wrapper in this.tempWrappers)
            {
                if (!uiMap.Value
                    .TryAdd(
                        wrapper.Type
                        , new UIPoolMapValue
                        {
                            GlobalID = 0,
                            Prefab = wrapper.Prefab,
                            UIPool = wrapper.UIPool,

                        }))
                {
                    Debug.LogError($"Another BaseUICtrl has already been registered with UIType = {wrapper.Type}");
                }

            }
        }

        private UIPoolMap GetMap()
        {
            EntityQuery entityQuery = this.em.CreateEntityQuery(typeof(UIPoolMap));
            return entityQuery.GetSingleton<UIPoolMap>();
        }


    }
}
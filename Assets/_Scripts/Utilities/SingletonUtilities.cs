using Components.Tag;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Utilities
{

    public struct SingletonUtilities
    {

        private Entity singletonEntity;
        private EntityManager entityManager;
        private bool isValid;


        public EntityManager EntityManager => entityManager;
        public Entity DefaultSingletonEntity => singletonEntity;



        private static readonly SharedStatic<SingletonUtilities> InstanceField
            = SharedStatic<SingletonUtilities>.GetOrCreate<InstanceFieldKey>();

        // Define a Key type to identify InstanceField
        private class InstanceFieldKey { }

        SingletonUtilities(EntityManager em)
        {
            this.isValid = false;
            this.singletonEntity = em.CreateEntity();
            em.AddComponent<DefaultSingletonTag>(this.singletonEntity);
            em.SetName(this.singletonEntity, "*DefaultSingletonEntity");
            this.entityManager = em;
        }

        public static SingletonUtilities GetInstance(EntityManager entityManager)
        {
            if (!InstanceField.Data.isValid)
            {
                InstanceField.Data = new(entityManager);
                InstanceField.Data.isValid = true;
            }
            InstanceField.Data.entityManager = entityManager;
            return InstanceField.Data;
        }


#if UNITY_EDITOR

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ClearOnLoad() => DestroyInstance();

#endif

        public static void DestroyInstance() => InstanceField.Data = default;


        public bool HasSingleton<T>() where T : IComponentData => this.entityManager.HasComponent<T>(this.singletonEntity);

        public T GetSingleton<T>() where T : unmanaged, IComponentData => this.entityManager.GetComponentData<T>(this.singletonEntity);


        public DynamicBuffer<T> AddBuffer<T>() where T : unmanaged, IBufferElementData
        {
            if (!this.entityManager.HasBuffer<T>(this.singletonEntity))
                return this.entityManager.AddBuffer<T>(this.singletonEntity);

            throw new System.Exception($"{typeof(T).Name} DynamicBuffer already existed");
        }

        public Entity AddOrSetComponentData<T>(T data) where T : unmanaged, IComponentData
        {
            if (this.entityManager.HasComponent<T>(this.singletonEntity))
                this.entityManager.SetComponentData(this.singletonEntity, data);
            else
                this.entityManager.AddComponentData(this.singletonEntity, data);

            return this.singletonEntity;
        }

        public Entity AddComponent<T>() where T : IComponentData
        {
            if (!this.entityManager.HasComponent<T>(this.singletonEntity))
                this.entityManager.AddComponent<T>(this.singletonEntity);

            return this.singletonEntity;
        }

        /// <summary>
        /// Should not use this because Unity ECS has bad support GetSingleton IEnableable.
        /// </summary>
        public void SetComponentEnabled<T>(bool value) where T : IComponentData, IEnableableComponent
        {
            if (!this.entityManager.HasComponent<T>(this.singletonEntity))
                Debug.LogError($"Singleton Entity doesn't have Component {nameof(T)} to enable");

            this.entityManager.SetComponentEnabled<T>(this.singletonEntity, value);
        }

    }

    public static class SingletonUtilitiesManagedExtensions
    {
        public static Entity AddOrSetComponentData<T>(this SingletonUtilities su, T data) where T : class, IComponentData, new()
        {
            EntityManager em = su.EntityManager;
            Entity singletonEntity = su.DefaultSingletonEntity;

            if (em.HasComponent<T>(singletonEntity))
                em.SetComponentData(singletonEntity, data);
            else
                em.AddComponentData(singletonEntity, data);

            return singletonEntity;
        }


    }
}
using Components.Tag;
using Unity.Entities;
using UnityEngine;

namespace Utilities
{

    public struct SingletonUtilities
    {

        private Entity singletonEntity;
        private static SingletonUtilities instance;
        private static bool isValid;

        private EntityManager entityManager;

        public EntityManager EntityManager => entityManager;
        public Entity DefaultSingletonEntity => singletonEntity;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ClearOnLoad() => DestroyInstance();

#endif


        private SingletonUtilities(EntityManager em)
        {
            this.singletonEntity = em.CreateEntity(typeof(DefaultSingletonTag));
            em.SetName(this.singletonEntity, "*DefaultSingletonEntity");

            this.entityManager = em;
        }


        public static SingletonUtilities GetInstance(EntityManager em)
        {
            if (!isValid)
            {
                isValid = true;
                instance = new(em);
            }

            // Just to make sure use UpToDated EntityManager.
            instance.entityManager = em;
            return instance;
        }

        private static void DestroyInstance() => isValid = false;



        public bool HasSingleton<T>() where T : IComponentData => this.entityManager.HasComponent<T>(this.singletonEntity);

        public T GetSingleton<T>() where T : unmanaged, IComponentData => this.entityManager.GetComponentData<T>(this.singletonEntity);


        public Entity AddBuffer<T>() where T : unmanaged, IBufferElementData
        {
            if (!this.entityManager.HasBuffer<T>(this.singletonEntity))
                this.entityManager.AddBuffer<T>(this.singletonEntity);
            return this.singletonEntity;
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
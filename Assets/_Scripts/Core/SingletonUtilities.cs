using Components.Tag;
using Unity.Entities;
using UnityEngine;

namespace Core
{
    //public static class SingletonUtilities
    //{
    //    private static Entity singletonEntity;

    //    public static void Setup(EntityManager em)
    //    {
    //        if (em.Exists(singletonEntity))
    //        {
    //            UnityEngine.Debug.LogWarning("Default singleton already created");
    //            return;
    //        }

    //        singletonEntity = em.CreateEntity(typeof(DefaultSingletonTag));
    //        em.SetName(singletonEntity, "*DefaultSingletonEntity");
    //    }

    //    public static Entity GetDefaultSingletonEntity(this EntityManager em) => singletonEntity;

    //    public static bool HasSingleton<T>(this EntityManager em) where T : IComponentData
    //    {
    //        return em.HasComponent<T>(singletonEntity);
    //    }

    //    public static Entity CreateOrAddSingletonICD<T>(this EntityManager em) where T : IComponentData
    //    {
    //        if (!em.HasComponent<T>(singletonEntity))
    //            em.AddComponent<T>(singletonEntity);

    //        return singletonEntity;
    //    }

    //    public static Entity CreateOrSetSingletonICD<T>(this EntityManager em, T data) where T : unmanaged, IComponentData
    //    {
    //        if (em.HasComponent<T>(singletonEntity))
    //            em.SetComponentData(singletonEntity, data);
    //        else
    //            em.AddComponentData(singletonEntity, data);

    //        return singletonEntity;
    //    }

    //    public static Entity CreateOrAddSingletonBuffer<T>(this EntityManager em) where T : unmanaged, IBufferElementData
    //    {
    //        if (!em.HasBuffer<T>(singletonEntity))
    //            em.AddBuffer<T>(singletonEntity);

    //        return singletonEntity;
    //    }

    //    public static T GetSingleton<T>(this EntityManager em) where T : unmanaged, IComponentData
    //    {
    //        return em.GetComponentData<T>(singletonEntity);
    //    }


    //}

    public struct SingletonUtilities
    {

        private static Entity singletonEntity;
        private static SingletonUtilities instance;
        private static bool isValid;

        private EntityManager entityManager;


#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ClearOnLoad() => DestroyInstance();

#endif


        private SingletonUtilities(EntityManager em)
        {
            singletonEntity = em.CreateEntity(typeof(DefaultSingletonTag));
            em.SetName(singletonEntity, "*DefaultSingletonEntity");

            this.entityManager = em;
        }


        public static SingletonUtilities GetInstance(EntityManager em)
        {
            if (!isValid)
            {
                isValid = true;
                instance = new(em);
            }
            return instance;
        }

        private static void DestroyInstance() => isValid = false;

        public Entity GetDefaultSingletonEntity() => singletonEntity;

        public bool HasSingleton<T>() where T : IComponentData => this.entityManager.HasComponent<T>(singletonEntity);

        public Entity CreateOrAddSingletonICD<T>() where T : IComponentData
        {
            if (!this.entityManager.HasComponent<T>(singletonEntity))
                this.entityManager.AddComponent<T>(singletonEntity);

            return singletonEntity;
        }

        public Entity CreateOrSetSingletonICD<T>(T data) where T : unmanaged, IComponentData
        {
            if (this.entityManager.HasComponent<T>(singletonEntity))
                this.entityManager.SetComponentData(singletonEntity, data);
            else
                this.entityManager.AddComponentData(singletonEntity, data);

            return singletonEntity;
        }

        public Entity CreateOrAddSingletonBuffer<T>() where T : unmanaged, IBufferElementData
        {
            if (!this.entityManager.HasBuffer<T>(singletonEntity))
                this.entityManager.AddBuffer<T>(singletonEntity);
            return singletonEntity;
        }

        public T GetSingleton<T>() where T : unmanaged, IComponentData => this.entityManager.GetComponentData<T>(singletonEntity);

    }
}
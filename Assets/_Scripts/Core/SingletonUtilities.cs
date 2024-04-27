using Components.Tag;
using Unity.Entities;

namespace Core
{
    public static class SingletonUtilities
    {
        private static Entity singletonEntity;

        public static void Setup(EntityManager em)
        {
            if (em.Exists(singletonEntity))
            {
                UnityEngine.Debug.LogWarning("Default singleton already created");
                return;
            }

            singletonEntity = em.CreateEntity(typeof(DefaultSingletonTag));
            em.SetName(singletonEntity, "*DefaultSingletonEntity");
        }

        public static Entity GetDefaultSingletonEntity(this EntityManager em)
        {
            return singletonEntity;
        }

        public static bool HasSingleton<T>(this EntityManager em) where T : IComponentData
        {
            return em.HasComponent<T>(singletonEntity);
        }

        public static Entity CreateOrAddSingletonICD<T>(this EntityManager em) where T : IComponentData
        {
            if (!em.HasComponent<T>(singletonEntity))
                em.AddComponent<T>(singletonEntity);

            return singletonEntity;
        }
        public static Entity CreateOrSetSingletonICD<T>(this EntityManager em, T data) where T : unmanaged, IComponentData
        {
            if (em.HasComponent<T>(singletonEntity))
                em.SetComponentData(singletonEntity, data);
            else
                em.AddComponentData(singletonEntity, data);

            return singletonEntity;
        }

        public static Entity CreateOrAddSingletonBuffer<T>(this EntityManager em) where T : unmanaged, IBufferElementData
        {
            if (!em.HasBuffer<T>(singletonEntity))
                em.AddBuffer<T>(singletonEntity);

            return singletonEntity;
        }



    }
}
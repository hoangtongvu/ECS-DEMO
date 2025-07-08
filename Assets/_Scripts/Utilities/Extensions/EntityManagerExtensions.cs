using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Utilities.Extensions
{
    [BurstCompile]
    public static class EntityManagerExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public static void AddComponentData<T>(this EntityManager em, NativeArray<Entity> entities, T componentData)
            where T : unmanaged, IComponentData
        {
            int length = entities.Length;

            for (int i = 0; i < length; i++)
            {
                em.AddComponentData(entities[i], componentData);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public static void SetComponentData<T>(this EntityManager em, NativeArray<Entity> entities, T componentData)
            where T : unmanaged, IComponentData
        {
            int length = entities.Length;

            for (int i = 0; i < length; i++)
            {
                em.SetComponentData(entities[i], componentData);
            }
        }

    }

}

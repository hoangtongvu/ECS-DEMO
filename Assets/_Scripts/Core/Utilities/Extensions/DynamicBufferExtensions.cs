using Unity.Burst;
using Unity.Entities;

namespace Core.Utilities.Extensions
{
    [BurstCompile]
    public static class DynamicBufferExtensions
    {
        [BurstCompile]
        public static void RemoveAtSwapBack<T>(this DynamicBuffer<T> buffer, int index)
            where T : unmanaged
        {
            int length = buffer.Length;
            buffer.RemoveAtSwapBack(length, index);
        }

        [BurstCompile]
        public static void RemoveAtSwapBack<T>(this DynamicBuffer<T> buffer, int length, int index)
            where T : unmanaged
        {
            buffer[index] = buffer[length - 1];
            buffer.RemoveAt(length - 1);
        }

    }

}
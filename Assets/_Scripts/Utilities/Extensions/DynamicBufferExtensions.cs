using Unity.Burst;
using Unity.Entities;

namespace Utilities.Extensions
{
    [BurstCompile]
    public static class DynamicBufferExtensions
    {
        /// <summary>
        /// Pop the last element in the Buffer.
        /// </summary>
        [BurstCompile]
        public static bool TryPop<T>(ref this DynamicBuffer<T> buffer, out T item)
            where T : unmanaged
        {
            int lastItemIndex = buffer.Length - 1;
            if (lastItemIndex < 0)
            {
                item = default(T);
                return false;
            }

            item = buffer[lastItemIndex];
            buffer.RemoveAt(lastItemIndex);
            return true;

        }

    }

}

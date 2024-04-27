using Unity.Entities;

namespace Core.Helpers
{
    public static class DynamicBufferQueueHelper
    {
        /// <summary>
        /// Dequeue the first element of the buffer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool TryDequeue<T>(DynamicBuffer<T> buffer, out T element) where T : unmanaged, IBufferElementData
        {

            if (buffer.IsEmpty)
            {
                element = default(T);
                return false;
            }

            element = buffer[0];
            return true;
        }


        /// <summary>
        /// Enqueue means Add element to the end of the buffer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="element"></param>
        public static void Enqueue<T>(DynamicBuffer<T> buffer, T element) where T : unmanaged, IBufferElementData => buffer.Add(element);

    }
}
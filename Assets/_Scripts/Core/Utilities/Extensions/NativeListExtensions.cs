using Unity.Burst;
using Unity.Collections;

namespace Core.Utilities.Extensions
{

    [BurstCompile]
    public static class NativeListExtensions
    {

        [BurstCompile]
        public static void QuickRemoveAt<T>(this NativeList<T> list, int index)
            where T : unmanaged
        {
            int length = list.Length;
            list.QuickRemoveAt(length, index);
        }

        [BurstCompile]
        public static void QuickRemoveAt<T>(this NativeList<T> list, int length, int index)
            where T : unmanaged
        {
            list[index] = list[length - 1];
            list.RemoveAt(length - 1);
        }



    }
}
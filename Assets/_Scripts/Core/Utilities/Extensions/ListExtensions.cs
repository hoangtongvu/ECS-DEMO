using System.Collections.Generic;

namespace Core.Utilities.Extensions
{
    public static class ListExtensions
    {
        public static void RemoveAtSwapBack<T>(this List<T> list, int index)
        {
            int length = list.Count;
            list.RemoveAtSwapBack(length, index);
        }

        public static void RemoveAtSwapBack<T>(this List<T> list, int length, int index)
        {
            list[index] = list[length - 1];
            list.RemoveAt(length - 1);
        }

    }

}
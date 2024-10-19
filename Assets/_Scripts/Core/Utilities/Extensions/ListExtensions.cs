using System.Collections.Generic;

namespace Core.Utilities.Extensions
{
    public static class ListExtensions
    {

        public static void QuickRemoveAt<T>(this List<T> list, int index)
        {
            int length = list.Count;
            list.QuickRemoveAt(length, index);
        }

        public static void QuickRemoveAt<T>(this List<T> list, int length, int index)
        {
            list[index] = list[length - 1];
            list.RemoveAt(length - 1);
        }



    }
}
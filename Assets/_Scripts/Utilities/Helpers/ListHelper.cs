using System.Collections.Generic;
using Unity.Entities;

namespace Utilities.Helpers
{
    public static class ListHelper
    {

        /// <summary>
        /// High performance Remove element at index for un-ordered list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        private static void RemoveAt<T>(List<T> list, int index)
        {
            int length = list.Count;
            list[index] = list[length - 1];
            list.RemoveAt(length - 1);
        }

    }
}
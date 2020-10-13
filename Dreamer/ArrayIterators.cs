using System;
using System.Drawing;

namespace Dreamer
{
    public static class ArrayIterators
    {
        public static void Function<T>(this T[] Array, Func<int, T, T> Function)
        {
            for (int i = 0; i < Array.Length; i++)
                Array[i] = Function(i, Array[i]);
        }
        public static void Function<T>(this T[,] Array, Func<int, int, T, T> Function)
        {
            for (int y = 0; y <= Array.GetUpperBound(1); y++)
                for (int x = 0; x <= Array.GetUpperBound(0); x++)
                    Array[x, y] = Function(x, y, Array[x, y]);
        }
        public static void Action<T>(this T[] Array, Action<int, T> Action)
        {
            for (int i = 0; i < Array.Length; i++)
                Action(i, Array[i]);
        }
        public static void Action<T>(this T[,] Array, Action<int, int, T> Action)
        {
            for (int y = 0; y <= Array.GetUpperBound(1); y++)
                for (int x = 0; x <= Array.GetUpperBound(0); x++)
                    Action(x, y, Array[x, y]);
        }
    }
}

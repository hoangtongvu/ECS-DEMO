using UnityEngine;

namespace Core.Utilities.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 Add(this Vector2 vector, float x = 0, float y = 0)
        {
            return new Vector2(vector.x + x, vector.y + y);
        }

        public static Vector2 With(this Vector2 vector, float x = float.NaN, float y = float.NaN)
        {
            return new Vector2(
                !float.IsNaN(x) ? x : vector.x,
                !float.IsNaN(y) ? y : vector.y
            );
        }

    }

}
using UnityEngine;

namespace Core.Utilities.Helpers
{
    public static class GizmosHelper
    {
        public static void DrawWireCube(Vector3 center, Vector3 size, int thicknessPixel)
        {
            Vector3 worldUnitPerPixel = new(0.01f, 0f, 0.01f);

            for (int i = 0; i < thicknessPixel; i++)
            {
                Gizmos.DrawWireCube(center, size - worldUnitPerPixel * i);
            }
        }

    }


}

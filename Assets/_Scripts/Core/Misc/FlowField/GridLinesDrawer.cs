using Unity.Mathematics;
using UnityEngine;

namespace Core.Misc.FlowField
{
    public class GridLinesDrawer : MonoBehaviour
    {
        public int2 GridMapSize { get; set; }
        public float DrawCellRadius { get; set; }
        public int2 GridOffset { get; set; }
        public Color DrawColor { get; set; }


        private void OnDrawGizmos()
        {
            DrawGridLines(this.GridMapSize, this.GridOffset, this.DrawColor, this.DrawCellRadius);
        }

        private static void DrawGridLines(int2 gridMapSize, int2 gridOffset, Color drawColor, float drawCellRadius)
        {
            Gizmos.color = drawColor;
            for (int x = gridOffset.x; x < gridMapSize.x + gridOffset.x; x++)
            {
                for (int y = gridOffset.y; y < gridMapSize.y + gridOffset.y; y++)
                {
                    Vector3 center = new Vector3(
                        drawCellRadius * 2 * x + drawCellRadius
                        , 0
                        , -(drawCellRadius * 2 * y + drawCellRadius));

                    Vector3 gridSize = Vector3.one * drawCellRadius * 2;
                    gridSize.y = 0;

                    Gizmos.DrawWireCube(center, gridSize);

                }
            }

        }


    }


}

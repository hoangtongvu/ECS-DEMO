using Unity.Mathematics;
using UnityEngine;

namespace Core.Misc.FlowField
{
    public class GridDebugger : MonoBehaviour
    {
        [SerializeField] private int2 gridMapSize = new(10, 10);
        [SerializeField] private float drawCellRadius = 0.5f;


        private void OnDrawGizmos()
        {
            this.DrawGrid(this.gridMapSize, Color.yellow, this.drawCellRadius);
        }

        private void DrawGrid(int2 gridMapSize, Color drawColor, float drawCellRadius)
        {
            Gizmos.color = drawColor;
            for (int x = 0; x < gridMapSize.x; x++)
            {
                for (int y = 0; y < gridMapSize.y; y++)
                {
                    Vector3 center = new Vector3(
                        drawCellRadius * 2 * x + drawCellRadius
                        , 0
                        , drawCellRadius * 2 * y + drawCellRadius);

                    Vector3 gridSize = Vector3.one * drawCellRadius * 2;

                    Gizmos.DrawWireCube(center, gridSize);

                }
            }

        }


    }


}

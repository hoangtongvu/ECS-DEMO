using Core.Utilities.Helpers;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Misc.WorldMap
{
    public struct ChunksDrawerConfig
    {
        public NativeList<Chunk> Chunks;
        public NativeArray<Color> ColorPalette;
        public half CellRadius;
    }

    public class ChunksDrawer : MonoBehaviour
    {
        private readonly List<Color> chunkColors = new();
        private ChunksDrawerConfig config;


        public void ApplyConfig(in ChunksDrawerConfig config)
        {
            this.config = config;
            this.GenerateChunkColors();
        }

        public void GenerateChunkColors()
        {
            int length = this.config.Chunks.Length;

            this.chunkColors.Clear();
            for (int i = 0; i < length; i++)
            {
                Color color = this.GetRandomColorFromPalette();
                this.chunkColors.Add(color);
            }
        }

        private Color GetRandomColorFromPalette()
        {
            int randIndex = UnityEngine.Random.Range(0, this.config.ColorPalette.Length);
            return this.config.ColorPalette[randIndex];
        }

        private void OnDrawGizmos()
        {
            if (!this.config.Chunks.IsCreated) return;

            DrawChunks(
                this.config.Chunks
                , this.chunkColors
                , this.config.CellRadius);

        }

        private static void DrawChunks(
            NativeList<Chunk> chunks
            , List<Color> chunkColors
            , half cellRadius)
        {
            int length = chunks.Length;

            for (int i = 0; i < length; i++)
            {
                var chunk = chunks[i];
                Gizmos.color = chunkColors[i];

                float chunkWorldSize = (chunk.BottomRightCellPos.x - chunk.TopLeftCellPos.x + 1) * cellRadius * 2;

                Vector3 center = new(
                    cellRadius * 2 * chunk.TopLeftCellPos.x + chunkWorldSize / 2
                    , 0
                    , -(cellRadius * 2 * chunk.TopLeftCellPos.y + chunkWorldSize / 2));

                Vector3 cubeDrawSize = Vector3.one * chunkWorldSize;
                cubeDrawSize.y = 0;

                GizmosHelper.DrawWireCube(center, cubeDrawSize, 10);

            }

        }

    }

}

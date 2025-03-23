using NUnit.Framework;
using Core.Misc.WorldMap;
using Unity.Collections;
using Components.Misc.WorldMap;
using Utilities.Helpers.Misc.WorldMap;

namespace Tests.EditMode.Utilities.Helpers.Misc.WorldMap
{
    public class ChunkExitHelperTest
    {
        private ChunkIndexToExitIndexesMap chunkIndexToExitIndexesMap;
        private ChunkExitIndexesContainer exitIndexesContainer;
        private ChunkExitsContainer exitsContainer;

        [SetUp]
        public void SetUp()
        {
            this.chunkIndexToExitIndexesMap = new()
            {
                Value = new(100, Allocator.Temp),
            };

            this.exitIndexesContainer = new()
            {
                Value = new(100, Allocator.Temp),
            };

            this.exitsContainer = new()
            {
                Value = new(100, Allocator.Temp),
            };

        }

        [TearDown]
        public void TearDown()
        {
            this.chunkIndexToExitIndexesMap.Value.Dispose();
            this.exitIndexesContainer.Value.Dispose();
            this.exitsContainer.Value.Dispose();
        }

        [Test]
        public void GetUnsafeCellsInRightOrder_ValidChunkIndex_ReturnsTrue()
        {
            // Arrange
            var cells = new Cell[]
            {
                new() { ChunkIndex = 1 },
                new() { ChunkIndex = 2 },
            };

            var costMap = new WorldTileCostMap
            {
                Value = new NativeArray<Cell>(cells, Allocator.Temp),
            };

            ChunkExit exit = new(0, 1);

            // Act
            bool isExitOrdered = ChunkExitHelper.GetUnsafeCellsInRightOrder(
                in costMap, in exit, 1,
                out Cell innerCell, out int innerIndex,
                out Cell outerCell, out int outerIndex);

            // Assert
            Assert.IsTrue(isExitOrdered);
            Assert.AreEqual(0, innerIndex);
            Assert.AreEqual(1, outerIndex);
            Assert.AreEqual(costMap.Value[0], innerCell);
            Assert.AreEqual(costMap.Value[1], outerCell);

            costMap.Value.Dispose();

        }

        [Test]
        public void GetUnsafeCellsInRightOrder_ValidChunkIndex_ReturnsFalse()
        {
            // Arrange
            var cells = new Cell[]
            {
                new() { ChunkIndex = 1 },
                new() { ChunkIndex = 2 },
            };

            var costMap = new WorldTileCostMap
            {
                Value = new NativeArray<Cell>(cells, Allocator.Temp),
            };

            ChunkExit exit = new(0, 1);

            // Act
            bool isExitOrdered = ChunkExitHelper.GetUnsafeCellsInRightOrder(
                in costMap, in exit, 2,
                out Cell innerCell, out int innerIndex,
                out Cell outerCell, out int outerIndex);

            // Assert
            Assert.IsFalse(isExitOrdered);
            Assert.AreEqual(1, innerIndex);
            Assert.AreEqual(0, outerIndex);
            Assert.AreEqual(costMap.Value[1], innerCell);
            Assert.AreEqual(costMap.Value[0], outerCell);

            costMap.Value.Dispose();

        }

    }

}
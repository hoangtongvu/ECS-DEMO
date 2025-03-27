using NUnit.Framework;
using Unity.Mathematics;
using Utilities.Helpers;

namespace Tests.EditMode.Utilities.Helpers.Misc.WorldMap
{
    public class WorldMapHelperTest
    {
        [TestCase(0.5f, 2, 2f)]
        [TestCase(0.3f, 2, 1.2f)]
        [TestCase(1f, 5, 10f)]
        public void GridLengthToWorldLength_ValidParams(float cellRadiusFloat, int gridLength, float expected)
        {
            float actual = WorldMapHelper.GridLengthToWorldLength(new half(cellRadiusFloat), gridLength);

            Assert.AreEqual(expected, actual, 0.01f, $"actual: {actual}, expected: {expected}");

        }

    }

}
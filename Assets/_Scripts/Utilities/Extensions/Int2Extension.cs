using Unity.Burst;
using Unity.Mathematics;


namespace Utilities.Extensions
{
    [BurstCompile]
    public static class Int2Extension
    {
        public static int2 Add(this int2 int2, in int addAmountX = 0, in int addAmountY = 0)
        {
            return new(int2.x + addAmountX, int2.y + addAmountY);
        }
    }
}

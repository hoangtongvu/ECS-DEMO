
namespace Core.UI.MyCanvas
{
    public enum CanvasAnchorPreset : byte
    {
        // Center positions
        MiddleCenter = 0,

        // Top row positions
        TopLeft = 1,
        TopCenter = 2,
        TopRight = 3,

        // Middle row positions
        MiddleLeft = 4,
        // MiddleCenter is already defined as 0
        MiddleRight = 5,

        // Bottom row positions
        BottomLeft = 6,
        BottomCenter = 7,
        BottomRight = 8
    }
}
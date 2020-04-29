namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a ImagePositionType
    /// </summary>
    public enum WImagePositionType : byte
    {
        //位置类型：9宫格
        LeftTop = 1,
        CenterTop = 2,
        RightTop = 3,
        LeftCenter = 4,
        CenterCenter = 5,
        RightCenter = 6,
        LeftBottom = 7,
        CenterBottom = 8,
        RightBottom = 9,
    }
}

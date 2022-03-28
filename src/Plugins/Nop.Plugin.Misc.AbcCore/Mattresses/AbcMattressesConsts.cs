using System.Linq;

namespace Nop.Plugin.Misc.AbcCore.Mattresses
{
    public static class AbcMattressesConsts
    {
        private static string[] _bases = new string[]
        {
            BaseNameTwin,
            BaseNameTwinXL,
            BaseNameFull,
            BaseNameQueen,
            BaseNameQueenFlexhead,
            BaseNameKing,
            BaseNameKingFlexhead,
            BaseNameCaliforniaKing,
            BaseNameCaliforniaKingFlexhead
        };

        private static string[] _mattressProtectors = new string[]
        {
            MattressProtectorTwin,
            MattressProtectorTwinXL,
            MattressProtectorFull,
            MattressProtectorQueen,
            MattressProtectorQueenFlexhead,
            MattressProtectorKing,
            MattressProtectorKingFlexhead,
            MattressProtectorCaliforniaKing,
            MattressProtectorCaliforniaKingFlexhead
        };

        private static string[] _frames = new string[]
        {
            FrameTwin,
            FrameTwinXL,
            FrameFull,
            FrameQueen,
            FrameQueenFlexhead,
            FrameKing,
            FrameKingFlexhead,
            FrameCaliforniaKing,
            FrameCaliforniaKingFlexhead
        };

        public static bool IsBase(string value)
        {
            return _bases.Contains(value);
        }
        public static bool IsMattressProtector(string value)
        {
            return _mattressProtectors.Contains(value);
        }
        public static bool IsFrame(string value)
        {
            return _frames.Contains(value);
        }

        public const string MattressSizeName = "Mattress Size";
        public const string BaseNameTwin = "Base (Twin)";
        public const string BaseNameTwinXL = "Base (TwinXL)";
        public const string BaseNameFull = "Base (Full)";
        public const string BaseNameQueen = "Base (Queen)";
        public const string BaseNameQueenFlexhead = "Base (Queen-Flexhead)";
        public const string BaseNameKing = "Base (King)";
        public const string BaseNameKingFlexhead = "Base (King-Flexhead)";
        public const string BaseNameCaliforniaKing = "Base (California King)";
        public const string BaseNameCaliforniaKingFlexhead = "Base (California King-Flexhead)";
        public const string MattressProtectorTwin = "Mattress Protector (Twin)";
        public const string MattressProtectorTwinXL = "Mattress Protector (TwinXL)";
        public const string MattressProtectorFull = "Mattress Protector (Full)";
        public const string MattressProtectorQueen = "Mattress Protector (Queen)";
        public const string MattressProtectorQueenFlexhead = "Mattress Protector (Queen-Flexhead)";
        public const string MattressProtectorKing = "Mattress Protector (King)";
        public const string MattressProtectorKingFlexhead = "Mattress Protector (King-Flexhead)";
        public const string MattressProtectorCaliforniaKing = "Mattress Protector (California King)";
        public const string MattressProtectorCaliforniaKingFlexhead = "Mattress Protector (California King-Flexhead)";
        public const string FrameTwin = "Frame (Twin)";
        public const string FrameTwinXL = "Frame (TwinXL)";
        public const string FrameFull = "Frame (Full)";
        public const string FrameQueen = "Frame (Queen)";
        public const string FrameQueenFlexhead = "Frame (Queen-Flexhead)";
        public const string FrameKing = "Frame (King)";
        public const string FrameKingFlexhead = "Frame (King-Flexhead)";
        public const string FrameCaliforniaKing = "Frame (California King)";
        public const string FrameCaliforniaKingFlexhead = "Frame (California King-Flexhead)";
        public const string FreeGiftName = "Free Gift";

        public const string Twin = "Twin";
        public const string TwinXL = "TwinXL";
        public const string Full = "Full";
        public const string Queen = "Queen";
        public const string QueenFlexhead = "Queen-Flexhead";
        public const string King = "King";
        public const string KingFlexhead = "King-Flexhead";
        public const string CaliforniaKing = "California King";
        public const string CaliforniaKingFlexhead = "California King-Flexhead";
    }
}
// SPDX-AI-Disclosure: ai-assisted
namespace Game.Core
{
    /// <summary>
    /// ゲーム盤面の幾何構造とセルの占有状態を管理するクラス（RL-01 / RL-02 / RL-03 / RL-18 / PR-06 / PR-07 / PR-08）。
    /// </summary>
    public sealed class Board
    {
        /// <summary>盤面幅（PR-06 / RL-01）。</summary>
        public const int Width = 10;

        /// <summary>表示領域の高さ（PR-07 / RL-01）。</summary>
        public const int VisibleHeight = 20;

        /// <summary>バッファ領域の高さ（RL-02）。</summary>
        public const int BufferHeight = 2;

        /// <summary>内部行数（PR-08 / RL-02）。VisibleHeight + BufferHeight と一致する。</summary>
        public const int Height = 22;

        private readonly bool[,] _cells = new bool[Width, Height];

        /// <summary>表示領域 Y=0..19 の判定（RL-01 / RL-03）。</summary>
        public static bool IsVisibleRow(int y) => y >= 0 && y < VisibleHeight;

        /// <summary>バッファ領域 Y=20..21 の判定（RL-02 / RL-03）。</summary>
        public static bool IsBufferRow(int y) => y >= VisibleHeight && y < Height;

        /// <summary>盤面内座標の判定（RL-03 / RL-18）。</summary>
        public static bool InBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

        /// <summary>
        /// 指定座標の占有判定（RL-18）。盤面内はセルの状態を返し、盤面外は常に true。
        /// </summary>
        public bool IsOccupied(int x, int y)
        {
            if (!InBounds(x, y))
            {
                return true;
            }

            return _cells[x, y];
        }
    }
}

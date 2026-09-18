// SPDX-AI-Disclosure: ai-assisted
namespace Game.Core
{
    /// <summary>
    /// 盤面上を移動・落下中のアクティブミノ（IF-02 / RL-12）。
    /// </summary>
    public readonly struct ActiveMino
    {
        /// <summary>ミノの種類（IF-02）。</summary>
        public MinoType Type { get; }

        /// <summary>基準位置 X 座標（IF-02 / RL-12）。</summary>
        public int X { get; }

        /// <summary>基準位置 Y 座標（IF-02 / RL-12）。</summary>
        public int Y { get; }

        /// <summary>回転状態（IF-02）。</summary>
        public Rotation Rotation { get; }

        /// <summary>
        /// アクティブミノを初期化する（IF-02 / RL-12）。
        /// </summary>
        public ActiveMino(MinoType type, int x, int y, Rotation rotation)
        {
            Type = type;
            X = x;
            Y = y;
            Rotation = rotation;
        }
    }
}

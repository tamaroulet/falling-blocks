// SPDX-AI-Disclosure: ai-assisted
namespace Game.Core
{
    /// <summary>
    /// ミノの回転状態を表す列挙型（RL-14 / RL-15）。
    /// 仕様の向き 0 / R / 2 / L に対応し、時計回りの巡回順（Spawn, Right, Two, Left）。
    /// </summary>
    public enum Rotation
    {
        /// <summary>初期回転状態 0（RL-14 / RL-15）。</summary>
        Spawn,

        /// <summary>右回転状態 R（RL-14）。</summary>
        Right,

        /// <summary>180度回転状態 2（RL-14）。</summary>
        Two,

        /// <summary>左回転状態 L（RL-14）。</summary>
        Left
    }
}

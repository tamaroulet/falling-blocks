// SPDX-AI-Disclosure: ai-assisted
namespace Game.Core
{
    /// <summary>
    /// ミノの種類を表す列挙型（RL-08 / PR-10）。
    /// 宣言順は 7-Bag 初期配列順（I, O, T, S, Z, J, L）に固定される。
    /// </summary>
    public enum MinoType
    {
        /// <summary>I型ミノ（RL-08 / PR-10）。</summary>
        I,

        /// <summary>O型ミノ（RL-08 / PR-10）。</summary>
        O,

        /// <summary>T型ミノ（RL-08 / PR-10）。</summary>
        T,

        /// <summary>S型ミノ（RL-08 / PR-10）。</summary>
        S,

        /// <summary>Z型ミノ（RL-08 / PR-10）。</summary>
        Z,

        /// <summary>J型ミノ（RL-08 / PR-10）。</summary>
        J,

        /// <summary>L型ミノ（RL-08 / PR-10）。</summary>
        L
    }
}

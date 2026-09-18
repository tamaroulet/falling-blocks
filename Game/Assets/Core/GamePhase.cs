// SPDX-AI-Disclosure: ai-assisted
namespace Game.Core
{
    /// <summary>
    /// ゲームの進行フェーズを表す列挙型（IF-09 / ST-08）。
    /// </summary>
    public enum GamePhase
    {
        /// <summary>ゲーム開始前の初期状態（IF-09 / ST-08）。</summary>
        Ready,

        /// <summary>ゲームプレイ中の状態（ST-08）。</summary>
        Playing,

        /// <summary>ゲームオーバー状態（ST-08）。</summary>
        GameOver
    }
}

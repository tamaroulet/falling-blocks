// SPDX-AI-Disclosure: ai-assisted
using System;
using System.Collections.Generic;

namespace Game.Core
{
    /// <summary>
    /// ゲーム全体の進行状態を保持するクラス（IF-01 / IF-03 / IF-05 / IF-06 / IF-07 / IF-08 / IF-09 / IF-10 / ST-06 / RL-07 / RL-36）。
    /// </summary>
    public sealed class GameState
    {
        private uint _seed;

        /// <summary>ゲームの進行フェーズ（IF-09 / ST-06）。初期状態は Ready。</summary>
        public GamePhase Phase { get; } = GamePhase.Ready;

        /// <summary>ゲーム盤面（IF-01）。インスタンスごとに独立した盤面を持つ。</summary>
        public Board Board { get; } = new Board();

        /// <summary>現在操作中のアクティブミノ（IF-03）。Ready では未配置（null）。</summary>
        public ActiveMino? ActiveMino { get; }

        /// <summary>ネクストキュー（IF-05）。Ready では長さ 0 の空コレクション。</summary>
        public IReadOnlyList<MinoType> NextQueue { get; } = Array.Empty<MinoType>();

        /// <summary>現在のスコア（IF-06）。初期値は 0。</summary>
        public int Score { get; }

        /// <summary>累計ライン消去数（IF-07）。初期値は 0。</summary>
        public int LinesCleared { get; }

        /// <summary>経過ティック数（IF-08）。初期値は 0。</summary>
        public int TickCount { get; }

        /// <summary>
        /// Ready 初期状態の GameState を生成する。
        /// </summary>
        public GameState()
        {
        }

        /// <summary>
        /// 乱数シードを注入する（IF-10 / RL-07）。
        /// Ready 状態でのみ有効。0 が渡された場合は 1 として保持する。
        /// </summary>
        /// <param name="seed">初期化シード（0 の場合は 1 に補正される）。</param>
        public void InjectSeed(uint seed)
        {
            if (Phase == GamePhase.Ready)
            {
                _seed = seed == 0 ? 1u : seed;
            }
        }
    }
}

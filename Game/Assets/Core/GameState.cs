// SPDX-AI-Disclosure: ai-assisted
using System;
using System.Collections.Generic;

namespace Game.Core
{
    /// <summary>
    /// ゲーム全体の進行状態を保持するクラス（IF-01 / IF-03 / IF-05 / IF-06 / IF-07 / IF-08 / IF-09 / IF-10 / IF-11 / IF-12 / IF-13 / IF-14 / IF-15 / ST-01 / ST-04 / ST-06 / ST-07 / RL-07 / RL-09 / RL-10 / RL-11 / RL-15 / RL-36 / LP-03 / LP-04 / LP-05 / PR-05 / PR-09 / PR-16）。
    /// </summary>
    public sealed class GameState
    {
        private uint _seed = 1u;
        private XorShift32? _rng;
        private readonly List<MinoType> _nextQueue = new List<MinoType>();

        // 内部カウンタ（後続単位用）
#pragma warning disable CS0414
        private int _naturalFallCounter;
        private int _softDropCounter;
        private int _lockDelayTimer;
        private int _lockResetCount;
#pragma warning restore CS0414

        /// <summary>ゲームの進行フェーズ（IF-09 / ST-01 / ST-06 / ST-08）。初期状態は Ready。</summary>
        public GamePhase Phase { get; private set; } = GamePhase.Ready;

        /// <summary>ゲーム盤面（IF-01）。インスタンスごとに独立した盤面を持つ。</summary>
        public Board Board { get; } = new Board();

        /// <summary>現在操作中のアクティブミノ（IF-03 / RL-15 / PR-09）。Ready では未配置（null）。</summary>
        public ActiveMino? ActiveMino { get; private set; }

        /// <summary>ネクストキュー（IF-04 / IF-05 / PR-16）。Ready では長さ 0 の空コレクション。外から変更不可。</summary>
        public IReadOnlyList<MinoType> NextQueue => _nextQueue.AsReadOnly();

        /// <summary>現在のスコア（IF-06）。初期値は 0。</summary>
        public int Score { get; }

        /// <summary>累計ライン消去数（IF-07）。初期値は 0。</summary>
        public int LinesCleared { get; }

        /// <summary>経過ティック数（IF-08 / LP-03 / LP-04）。初期値は 0。</summary>
        public int TickCount { get; private set; }

        /// <summary>
        /// Ready 初期状態の GameState を生成する。
        /// </summary>
        public GameState()
        {
        }

        /// <summary>
        /// 乱数シードを注入する（IF-10 / RL-07 / ST-06 / ST-07）。
        /// Ready 状態でのみ有効。0 が渡された場合は 1 として保持する。
        /// Playing / GameOver で受けた注入は無視される。
        /// </summary>
        /// <param name="seed">初期化シード（0 の場合は 1 に補正される）。</param>
        public void InjectSeed(uint seed)
        {
            if (Phase == GamePhase.Ready)
            {
                _seed = seed == 0u ? 1u : seed;
            }
        }

        /// <summary>
        /// 1 ティック進める（IF-11 / LP-03 / LP-04 / ST-01）。
        /// </summary>
        public void Tick()
        {
            if (Phase == GamePhase.Ready)
            {
                Phase = GamePhase.Playing;
                _rng = new XorShift32(_seed == 0u ? 1u : _seed);
                _nextQueue.AddRange(SevenBag.Generate(_rng));

                // 出現の段: キューが 0 個なら先に補充する（RL-11）
                if (_nextQueue.Count == 0)
                {
                    _nextQueue.AddRange(SevenBag.Generate(_rng));
                }

                var spawnType = _nextQueue[0];
                _nextQueue.RemoveAt(0);
                ActiveMino = new ActiveMino(spawnType, 3, 19, Rotation.Spawn);

                _naturalFallCounter = 0;
                _softDropCounter = 0;
                _lockDelayTimer = 30;
                _lockResetCount = 0;
                TickCount = 0;

                // 終了判定（盤面が空なので GameOver には遷移しない: LP-05 / ST-02 / ST-03）
            }
            else if (Phase == GamePhase.Playing)
            {
                TickCount++;
            }
        }

        /// <summary>
        /// 指定された回数ティックを進める（IF-11）。
        /// ticks &lt;= 0 の場合は No-op（状態変更なし）。
        /// </summary>
        /// <param name="ticks">進めるティック数。</param>
        public void Tick(int ticks)
        {
            if (ticks <= 0)
            {
                return;
            }

            for (var i = 0; i < ticks; i++)
            {
                Tick();
            }
        }

        /// <summary>
        /// 左移動入力を処理する（IF-12 / ST-04）。この単位では状態変化なし。
        /// </summary>
        public void MoveLeft()
        {
        }

        /// <summary>
        /// 右移動入力を処理する（IF-13 / ST-04）。この単位では状態変化なし。
        /// </summary>
        public void MoveRight()
        {
        }

        /// <summary>
        /// 時計回り回転入力を処理する（IF-14 / ST-04）。この単位では状態変化なし。
        /// </summary>
        public void RotateClockwise()
        {
        }

        /// <summary>
        /// 反時計回り回転入力を処理する（IF-15 / ST-04）。この単位では状態変化なし。
        /// </summary>
        public void RotateCounterClockwise()
        {
        }

        /// <summary>
        /// ソフトドロップ入力を処理する（IF-16 / ST-04）。この単位では状態変化なし。
        /// </summary>
        /// <param name="pressed">押下中なら true、離されたら false。</param>
        public void SetSoftDrop(bool pressed)
        {
        }
    }
}

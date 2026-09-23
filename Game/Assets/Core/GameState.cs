// SPDX-AI-Disclosure: ai-assisted
using System;
using System.Collections.Generic;

namespace Game.Core
{
    /// <summary>
    /// ゲーム全体の進行状態を保持するクラス（IF-01 / IF-02 / IF-03 / IF-04 / IF-05 / IF-06 / IF-07 / IF-08 / IF-09 / IF-10 / IF-11 / IF-16 / IF-17 / ST-01 / LP-03 / LP-04 / RL-06 / RL-07 / RL-09 / RL-10 / RL-15 / RL-24 / RL-36）。
    /// </summary>
    public sealed class GameState
    {
        private const int GravityInterval = 60;
        private const uint DefaultSeed = 1u;
        private const int SpawnX = 3;
        private const int SpawnY = 19;
        private const int MaxNextQueueCount = 6;

        private uint _rngState = DefaultSeed;
        private int _fallCounter;

        /// <summary>ゲームの進行フェーズ（IF-09 / ST-01 / ST-06）。初期状態は Ready。</summary>
        public GamePhase Phase { get; private set; } = GamePhase.Ready;

        /// <summary>ゲーム盤面（IF-01）。インスタンスごとに独立した盤面を持つ。</summary>
        public Board Board { get; } = new Board();

        /// <summary>現在操作中のアクティブミノ（IF-02 / IF-03 / RL-15 / RL-24）。Ready では未配置（null）。</summary>
        public ActiveMino? ActiveMino { get; private set; }

        /// <summary>ネクストキュー（IF-04 / IF-05 / RL-10 / PR-16）。Ready では長さ 0 の空コレクション。</summary>
        public IReadOnlyList<MinoType> NextQueue { get; private set; } = Array.Empty<MinoType>();

        /// <summary>現在のスコア（IF-06 / RL-34 / RL-35）。初期値は 0。</summary>
        public int Score { get; }

        /// <summary>累計ライン消去数（IF-07 / RL-30 / RL-36）。初期値は 0。</summary>
        public int LinesCleared { get; }

        /// <summary>経過ティック数（IF-08 / LP-03 / LP-04）。初期値は 0。</summary>
        public int TickCount { get; private set; }

        /// <summary>盤面上の固定ブロックのマス数（IF-16 / RL-36）。初期値は 0。</summary>
        public int BlockCount { get; }

        /// <summary>ロックしたミノの累計数（IF-17 / RL-36）。初期値は 0。</summary>
        public int LockedMinoCount { get; }

        /// <summary>
        /// Ready 初期状態の GameState を生成する（IF-03 / IF-05 / IF-08）。
        /// </summary>
        public GameState()
        {
        }

        /// <summary>
        /// 乱数シードを注入する（IF-10 / RL-07 / ST-06 / ST-07）。
        /// Ready 状態でのみ有効。0 が渡された場合は 1 として保持する。
        /// </summary>
        /// <param name="seed">初期化シード（0 の場合は 1 に補正される）。</param>
        public void InjectSeed(uint seed)
        {
            if (Phase == GamePhase.Ready)
            {
                _rngState = seed == 0 ? DefaultSeed : seed;
            }
        }

        /// <summary>
        /// 1 ティック進める（IF-11 / LP-03 / LP-04 / ST-01 / RL-09 / RL-15 / RL-24）。
        /// </summary>
        public void Tick()
        {
            if (Phase == GamePhase.GameOver)
            {
                return;
            }

            if (Phase == GamePhase.Ready)
            {
                MinoType[] bag = GenerateBag();
                MinoType spawnType = bag[0];
                ActiveMino = new ActiveMino(spawnType, SpawnX, SpawnY, Rotation.Spawn);

                MinoType[] next = new MinoType[MaxNextQueueCount];
                Array.Copy(bag, 1, next, 0, MaxNextQueueCount);
                NextQueue = next;

                Phase = GamePhase.Playing;
                TickCount = 0;
                _fallCounter = 0;
                return;
            }

            if (Phase == GamePhase.Playing)
            {
                TickCount++;

                _fallCounter++;
                if (_fallCounter >= GravityInterval)
                {
                    if (ActiveMino.HasValue)
                    {
                        ActiveMino current = ActiveMino.Value;
                        ActiveMino = new ActiveMino(current.Type, current.X, current.Y - 1, current.Rotation);
                    }
                    _fallCounter = 0;
                }
            }
        }

        private uint NextRandom()
        {
            _rngState ^= _rngState << 13;
            _rngState ^= _rngState >> 17;
            _rngState ^= _rngState << 5;
            return _rngState;
        }

        private MinoType[] GenerateBag()
        {
            MinoType[] bag = new MinoType[]
            {
                MinoType.I,
                MinoType.O,
                MinoType.T,
                MinoType.S,
                MinoType.Z,
                MinoType.J,
                MinoType.L
            };

            for (int i = 6; i >= 1; i--)
            {
                int j = (int)(NextRandom() % (uint)(i + 1));
                MinoType temp = bag[i];
                bag[i] = bag[j];
                bag[j] = temp;
            }

            return bag;
        }
    }
}

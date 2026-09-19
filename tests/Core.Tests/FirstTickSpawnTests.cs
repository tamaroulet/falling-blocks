// SPDX-AI-Disclosure: ai-generated
using System.Collections.Generic;
using Game.Core;
using NUnit.Framework;

namespace StandaloneCore.Tests
{
    /// <summary>
    /// issue_14: 最初のティックでの Ready → Playing 遷移、7-Bag の補充、先頭ミノの出現、再現性。
    /// 対応する構造化仕様: LP-02 / LP-03 / LP-04 / LP-05 / ST-01 / ST-04 / ST-06 / ST-07 / RL-04 / RL-06 / RL-07 / RL-09 / RL-10 / RL-12 / RL-15 / RL-36 / IF-01..IF-11 / PR-05 / PR-09 / PR-16
    /// </summary>
    public class FirstTickSpawnTests
    {
        private const int SpawnX = 3;
        private const int SpawnY = 19;

        /// <summary>仕様 RL-06 / RL-09 をテスト側で独立に書き下した参照実装。</summary>
        private static MinoType[] ReferenceBag(uint seed)
        {
            var x = seed == 0u ? 1u : seed;
            var bag = new[] { MinoType.I, MinoType.O, MinoType.T, MinoType.S, MinoType.Z, MinoType.J, MinoType.L };

            for (var i = 6; i >= 1; i--)
            {
                x ^= x << 13;
                x ^= x >> 17;
                x ^= x << 5;

                var j = (int)(x % (uint)(i + 1));
                var swap = bag[i];
                bag[i] = bag[j];
                bag[j] = swap;
            }

            return bag;
        }

        /// <summary>外から読める出現順（先頭ミノ + ネクストキュー）。</summary>
        private static MinoType[] ObservedOrder(GameState state)
        {
            Assert.IsTrue(state.ActiveMino.HasValue, "出現順を読むには操作中のミノが必要（IF-03）");

            var order = new MinoType[1 + state.NextQueue.Count];
            order[0] = state.ActiveMino.Value.Type;

            for (var i = 0; i < state.NextQueue.Count; i++)
            {
                order[i + 1] = state.NextQueue[i];
            }

            return order;
        }

        private static int CountBlocks(GameState state)
        {
            var blocks = 0;

            for (var y = 0; y < Board.Height; y++)
            {
                for (var x = 0; x < Board.Width; x++)
                {
                    if (state.Board.IsOccupied(x, y))
                    {
                        blocks++;
                    }
                }
            }

            return blocks;
        }

        [Test]
        public void NewGameState_IsReadyAndCompletelyEmpty()
        {
            var state = new GameState();

            Assert.AreEqual(GamePhase.Ready, state.Phase, "生成直後は開始待機（IF-09 / ST-08）");
            Assert.AreEqual(0, state.Score, "生成直後のスコアは 0（IF-06）");
            Assert.AreEqual(0, state.LinesCleared, "生成直後の累計消去ライン数は 0（IF-07）");
            Assert.AreEqual(0, state.TickCount, "生成直後の経過ティック数は 0（IF-08）");
            Assert.AreEqual(0, state.NextQueue.Count, "生成直後のネクストキューは空（IF-05）");
            Assert.IsFalse(state.ActiveMino.HasValue, "生成直後に操作中のミノは無い（IF-03）");
            Assert.AreEqual(0, CountBlocks(state), "生成直後の盤面は全マス空（IF-01）");
        }

        [Test]
        public void FirstTick_FromReady_TransitionsToPlaying()
        {
            var state = new GameState();

            state.Tick();

            Assert.AreEqual(GamePhase.Playing, state.Phase, "開始待機でティックを進める操作を最初に受けたら進行中へ遷移する（ST-01 / IF-11）");
        }

        [Test]
        public void FirstTick_SpawnsHeadMinoAtSpawnPositionAndSpawnRotation()
        {
            var state = new GameState();

            state.Tick();

            Assert.IsTrue(state.ActiveMino.HasValue, "遷移が起きたティックで先頭ミノが出現する（LP-03 / IF-03）");
            Assert.AreEqual(SpawnX, state.ActiveMino.Value.X, "出現時の基準位置 X は 3（RL-15 / PR-09）");
            Assert.AreEqual(SpawnY, state.ActiveMino.Value.Y, "出現時の基準位置 Y は 19（RL-15 / PR-09）");
            Assert.AreEqual(Rotation.Spawn, state.ActiveMino.Value.Rotation, "出現時の向きは 0（RL-15）");
        }

        [Test]
        public void FirstTick_SpawnedMinoOccupiesOnlyBufferRows()
        {
            var state = new GameState();

            state.Tick();
            var mino = state.ActiveMino.Value;
            var cells = MinoShape.SpawnCells(mino.Type);

            Assert.AreEqual(4, cells.Length, "出現したミノは 4 マスを占める（RL-08）");

            foreach (var cell in cells)
            {
                var boardX = mino.X + cell.X;
                var boardY = mino.Y + cell.Y;

                Assert.That(boardX, Is.InRange(0, Board.Width - 1), $"盤面座標 X = Bx + x = {boardX} は 0..9（RL-12 / RL-01）");
                Assert.That(boardY, Is.InRange(Board.VisibleHeight, Board.Height - 1), $"盤面座標 Y = By + y = {boardY} はバッファ領域 20..21（RL-12 / RL-15）");
            }
        }

        [Test]
        public void FirstTick_LeavesSixMinosInNextQueue()
        {
            var state = new GameState();

            state.Tick();

            Assert.AreEqual(6, state.NextQueue.Count, "1 セット 7 個から先頭を取り出した残りは 6 個（RL-10 / IF-04 / PR-16）");
        }

        [Test]
        public void FirstTick_HeadMinoAndQueue_FormExactlyOneCompleteBag()
        {
            var state = new GameState();

            state.Tick();
            var order = ObservedOrder(state);

            Assert.AreEqual(7, order.Length, "先頭ミノ + ネクストキュー = 1 セット 7 個（RL-10）");
            Assert.That(order, Is.EquivalentTo(new[] { MinoType.I, MinoType.O, MinoType.T, MinoType.S, MinoType.Z, MinoType.J, MinoType.L }), "1 セットは 7 種類をちょうど 1 つずつ含む。重複も欠けもない（RL-08 / RL-10）");
        }

        [Test]
        public void FirstTick_TickCountIsZero()
        {
            var state = new GameState();

            state.Tick();

            Assert.AreEqual(0, state.TickCount, "遷移が起きたティックを終えた時点の経過ティック数は 0（LP-03 / IF-08）");
        }

        [Test]
        public void SecondTick_TickCountIsOne_AndActiveMinoIsUnchanged()
        {
            var state = new GameState();

            state.Tick();
            var spawned = state.ActiveMino.Value;

            state.Tick();

            Assert.AreEqual(1, state.TickCount, "さらにもう一度ティックを進めた時点で経過ティック数は 1（LP-04 / IF-08）");
            Assert.AreEqual(spawned.Type, state.ActiveMino.Value.Type, "この単位では操作中のミノの種類は変わらない");
            Assert.AreEqual(SpawnX, state.ActiveMino.Value.X, "この単位では移動を扱わないので X は 3 のまま（範囲外: 左右移動）");
            Assert.AreEqual(SpawnY, state.ActiveMino.Value.Y, "この単位では落下を扱わないので Y は 19 のまま（範囲外: 自然落下・ソフトドロップ）");
            Assert.AreEqual(Rotation.Spawn, state.ActiveMino.Value.Rotation, "この単位では回転を扱わないので向きは 0 のまま（範囲外: 回転）");
        }

        [Test]
        public void ThirdTick_TickCountIsTwo()
        {
            var state = new GameState();

            state.Tick();
            state.Tick();
            state.Tick();

            Assert.AreEqual(2, state.TickCount, "遷移ティックの後はティックごとに 1 ずつ増える（LP-04 / IF-08）");
            Assert.AreEqual(GamePhase.Playing, state.Phase, "進行中のまま（ST-08）");
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-60)]
        [TestCase(int.MinValue)]
        public void TickWithNonPositiveCount_InReady_ChangesNothing(int ticks)
        {
            var state = new GameState();

            state.Tick(ticks);

            Assert.AreEqual(GamePhase.Ready, state.Phase, $"Tick({ticks}) は状態を進めない（IF-11 / 異常系）");
            Assert.AreEqual(0, state.TickCount, $"Tick({ticks}) は経過ティック数を変えない（IF-08 / IF-11）");
            Assert.IsFalse(state.ActiveMino.HasValue, $"Tick({ticks}) ではミノが出現しない（IF-03 / IF-11）");
            Assert.AreEqual(0, state.NextQueue.Count, $"Tick({ticks}) ではネクストキューを補充しない（IF-05 / IF-11）");
            Assert.AreEqual(0, CountBlocks(state), $"Tick({ticks}) では盤面が変わらない（IF-01 / IF-11）");
        }

        [Test]
        public void TickWithCountOne_EqualsOneSingleTick()
        {
            var a = new GameState();
            var b = new GameState();

            a.Tick(1);
            b.Tick();

            Assert.AreEqual(b.Phase, a.Phase, "Tick(1) は Tick() と同じ遷移（IF-11）");
            Assert.AreEqual(b.TickCount, a.TickCount, "Tick(1) は Tick() と同じ経過ティック数（IF-08 / IF-11）");
            Assert.That(ObservedOrder(a), Is.EqualTo(ObservedOrder(b)), "Tick(1) は Tick() と同じ出現順（IF-11）");
        }

        [Test]
        public void TickWithCountTwo_EqualsTwoSingleTicks()
        {
            var a = new GameState();
            var b = new GameState();

            a.Tick(2);
            b.Tick();
            b.Tick();

            Assert.AreEqual(1, a.TickCount, "Tick(2) は遷移ティック + 1 ティックで経過ティック数 1（LP-03 / LP-04 / IF-11）");
            Assert.AreEqual(b.TickCount, a.TickCount, "まとめ進行と 1 ティックずつの進行は一致する（IF-11）");
            Assert.That(ObservedOrder(a), Is.EqualTo(ObservedOrder(b)), "まとめ進行でも出現順は一致する（IF-11）");
        }

        [Test]
        public void AfterSpawn_BoardIsStillCompletelyEmpty()
        {
            var state = new GameState();

            state.Tick();

            for (var y = 0; y < Board.Height; y++)
            {
                for (var x = 0; x < Board.Width; x++)
                {
                    Assert.IsFalse(state.Board.IsOccupied(x, y), $"出現したミノは固定ブロックではない。セル ({x}, {y}) は空（RL-04 / IF-01）");
                }
            }

            Assert.AreEqual(0, CountBlocks(state), "出現後も盤面の 10 × 22 マスはすべて空（IF-01）");
        }

        [Test]
        public void AfterSpawn_ScoreAndLinesClearedStayZero()
        {
            var state = new GameState();

            state.Tick();
            state.Tick();
            state.Tick();

            Assert.AreEqual(0, state.Score, "進行中へ遷移して先頭ミノが出現した後もスコアは 0（IF-06 / 範囲外: 得点加算）");
            Assert.AreEqual(0, state.LinesCleared, "進行中へ遷移して先頭ミノが出現した後も累計消去ライン数は 0（IF-07 / 範囲外: ライン消去）");
        }

        [Test]
        public void LineInvariant_HoldsThroughEveryTickOfThisUnit()
        {
            var state = new GameState();

            Assert.AreEqual(0, (state.LinesCleared * 10) + CountBlocks(state), "消去ライン数 × 10 ＋ 盤面の固定ブロック数 ＝ 4 × 固定したミノ数 ＝ 0（RL-36）: 開始待機");

            for (var i = 1; i <= 5; i++)
            {
                state.Tick();

                Assert.AreEqual(0, (state.LinesCleared * 10) + CountBlocks(state), $"消去ライン数 × 10 ＋ 盤面の固定ブロック数 ＝ 4 × 固定したミノ数 ＝ 0（RL-36）: {i} ティック目");
            }
        }

        [Test]
        public void EmptyBoard_NeverReachesGameOverInThisUnit()
        {
            var state = new GameState();

            for (var i = 1; i <= 10; i++)
            {
                state.Tick();

                Assert.AreEqual(GamePhase.Playing, state.Phase, $"盤面が空である以上、この単位の範囲では終了状態へ遷移しない（LP-05 / ST-02 / ST-03）: {i} ティック目");
            }
        }

        [Test]
        public void WithoutSeedInjection_UsesDefaultSeedOne()
        {
            var state = new GameState();

            state.Tick();

            Assert.That(ObservedOrder(state), Is.EqualTo(ReferenceBag(1u)), "シードを注入しなかった場合の既定値は 1（RL-07 / PR-05）");
        }

        [Test]
        public void SeedZero_ProducesSameOrderAsNoInjection()
        {
            var injected = new GameState();
            injected.InjectSeed(0u);
            injected.Tick();

            var defaulted = new GameState();
            defaulted.Tick();

            Assert.That(ObservedOrder(injected), Is.EqualTo(ObservedOrder(defaulted)), "シード 0 は 1 に置き換えて初期化するので、未注入と同じミノ順序になる（RL-07）");
        }

        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(7u)]
        [TestCase(12345u)]
        [TestCase(2147483648u)]
        [TestCase(uint.MaxValue)]
        public void InjectedSeed_InReady_ProducesFisherYatesOrder(uint seed)
        {
            var state = new GameState();

            state.InjectSeed(seed);
            state.Tick();

            Assert.That(ObservedOrder(state), Is.EqualTo(ReferenceBag(seed)), $"seed={seed} の出現順は XorShift32 + Fisher-Yates の結果と一致する（ST-06 / RL-06 / RL-09）");
        }

        [TestCase(1u)]
        [TestCase(12345u)]
        [TestCase(uint.MaxValue)]
        public void SameSeed_TwiceProducesIdenticalOrder(uint seed)
        {
            var first = new GameState();
            first.InjectSeed(seed);
            first.Tick();

            var second = new GameState();
            second.InjectSeed(seed);
            second.Tick();

            Assert.That(ObservedOrder(second), Is.EqualTo(ObservedOrder(first)), $"同じシード {seed} を注入して同じ手順を踏めば出現順は常に一致する（RL-05 / RL-06）");
        }

        [Test]
        public void ReadyPhase_IgnoresMoveRotateAndSoftDropInputs()
        {
            var state = new GameState();

            state.MoveLeft();
            state.MoveRight();
            state.RotateClockwise();
            state.RotateCounterClockwise();
            state.SetSoftDrop(true);
            state.SetSoftDrop(false);

            Assert.AreEqual(GamePhase.Ready, state.Phase, "開始待機のまま遷移しない（ST-04）");
            Assert.AreEqual(0, state.TickCount, "経過ティック数は変わらない（ST-04 / IF-08）");
            Assert.AreEqual(0, state.Score, "スコアは変わらない（ST-04 / IF-06）");
            Assert.AreEqual(0, state.LinesCleared, "累計消去ライン数は変わらない（ST-04 / IF-07）");
            Assert.AreEqual(0, state.NextQueue.Count, "ネクストキューは補充されない（ST-04 / IF-05）");
            Assert.IsFalse(state.ActiveMino.HasValue, "ミノは出現しない（ST-04 / IF-03）");
            Assert.AreEqual(0, CountBlocks(state), "盤面は全マス空のまま（ST-04 / IF-01）");
        }

        [Test]
        public void ReadyPhase_InputsDoNotConsumeRandomness()
        {
            var state = new GameState();

            state.InjectSeed(12345u);
            state.MoveLeft();
            state.RotateClockwise();
            state.RotateCounterClockwise();
            state.SetSoftDrop(true);
            state.MoveRight();
            state.Tick();

            Assert.That(ObservedOrder(state), Is.EqualTo(ReferenceBag(12345u)), "開始待機での入力は乱数を 1 回も消費しない（ST-04 / RL-06）");
        }

        [Test]
        public void InjectSeed_InPlaying_ChangesNothingObservable()
        {
            var state = new GameState();

            state.InjectSeed(1u);
            state.Tick();

            var before = ObservedOrder(state);
            var beforeX = state.ActiveMino.Value.X;
            var beforeY = state.ActiveMino.Value.Y;

            state.InjectSeed(987654321u);

            Assert.AreEqual(GamePhase.Playing, state.Phase, "進行中で受けたシード注入は状態を変えない（ST-07 / IF-10）");
            Assert.AreEqual(0, state.TickCount, "進行中で受けたシード注入は経過ティック数を変えない（ST-07 / IF-08）");
            Assert.AreEqual(0, state.Score, "進行中で受けたシード注入はスコアを変えない（ST-07 / IF-06）");
            Assert.AreEqual(0, state.LinesCleared, "進行中で受けたシード注入は累計消去ライン数を変えない（ST-07 / IF-07）");
            Assert.AreEqual(beforeX, state.ActiveMino.Value.X, "操作中のミノの X は変わらない（ST-07 / IF-02）");
            Assert.AreEqual(beforeY, state.ActiveMino.Value.Y, "操作中のミノの Y は変わらない（ST-07 / IF-02）");
            Assert.That(ObservedOrder(state), Is.EqualTo(before), "進行中で受けたシード注入は出現順を変えない（ST-07 / IF-10）");
            Assert.That(ObservedOrder(state), Is.EqualTo(ReferenceBag(1u)), "進行中で注入したシードは無視され、開始時のシードのままである（ST-07 / RL-06）");
        }

        [Test]
        public void NextQueue_IsNotWritableFromOutside()
        {
            var state = new GameState();

            state.Tick();
            var snapshot = ObservedOrder(state);

            if (state.NextQueue is IList<MinoType> writable && !writable.IsReadOnly)
            {
                writable[0] = writable[0] == MinoType.I ? MinoType.O : MinoType.I;
            }

            Assert.That(ObservedOrder(state), Is.EqualTo(snapshot), "外から書き換えても内部のネクストキューは変わらない（IF-04）");
        }

        [Test]
        public void TwoGameStates_KeepIndependentRandomState()
        {
            var a = new GameState();
            var b = new GameState();

            a.InjectSeed(1u);
            b.InjectSeed(12345u);
            a.Tick();
            b.Tick();

            Assert.AreNotSame(a.NextQueue, b.NextQueue, "ネクストキューは static に共有された 1 個であってはならない（IF-04）");
            Assert.That(ObservedOrder(a), Is.EqualTo(ReferenceBag(1u)), "インスタンスごとに独立した乱数状態を持つ（IF-10 / RL-06）");
            Assert.That(ObservedOrder(b), Is.EqualTo(ReferenceBag(12345u)), "インスタンスごとに独立した乱数状態を持つ（IF-10 / RL-06）");
        }
    }
}

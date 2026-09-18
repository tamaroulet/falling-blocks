// SPDX-AI-Disclosure: ai-generated
using Game.Core;
using NUnit.Framework;

namespace StandaloneCore.Tests
{
    /// <summary>
    /// issue_12: 初期状態（Ready）の盤面と各カウンタ。
    /// 対応する構造化仕様: IF-01 / IF-03 / IF-05 / IF-06 / IF-07 / IF-08 / IF-09 / IF-10 / ST-06 / RL-01 / RL-02 / RL-36
    /// </summary>
    public class InitialStateTests
    {
        [Test]
        public void NewGameState_Phase_IsReady()
        {
            var state = new GameState();

            Assert.AreEqual(GamePhase.Ready, state.Phase, "初期化した直後の状態は Ready（IF-09）");
        }

        [Test]
        public void NewGameState_Board_IsExposed()
        {
            var state = new GameState();

            Assert.IsNotNull(state.Board, "盤面が外から読み取れる形で公開されていること（IF-01）");
        }

        [Test]
        public void NewGameState_EveryCell_IsEmpty()
        {
            var state = new GameState();
            var visited = 0;

            for (var y = 0; y < Board.Height; y++)
            {
                for (var x = 0; x < Board.Width; x++)
                {
                    Assert.IsFalse(state.Board.IsOccupied(x, y), $"初期状態のセル ({x}, {y}) は空でなければならない（IF-01 / RL-01 / RL-02）");
                    visited++;
                }
            }

            Assert.AreEqual(220, visited, "走査した範囲は幅 10 × 22 行 = 220 マス（RL-01 / RL-02）");
        }

        [Test]
        public void NewGameState_ActiveMino_IsNotPlaced()
        {
            var state = new GameState();

            Assert.IsFalse(state.ActiveMino.HasValue, "Ready ではアクティブミノは未配置（IF-03）");
        }

        [Test]
        public void NewGameState_NextQueue_IsEmpty()
        {
            var state = new GameState();

            Assert.IsNotNull(state.NextQueue, "ネクストキューは null ではなく空のコレクション（IF-05）");
            Assert.AreEqual(0, state.NextQueue.Count, "Ready ではネクストキューの長さは 0（IF-05）");
        }

        [Test]
        public void NewGameState_Score_IsZero()
        {
            var state = new GameState();

            Assert.AreEqual(0, state.Score, "スコアの初期値は 0（IF-06）");
        }

        [Test]
        public void NewGameState_LinesCleared_IsZero()
        {
            var state = new GameState();

            Assert.AreEqual(0, state.LinesCleared, "累計消去ライン数の初期値は 0（IF-07）");
        }

        [Test]
        public void NewGameState_TickCount_IsZero()
        {
            var state = new GameState();

            Assert.AreEqual(0, state.TickCount, "経過ティック数の初期値は 0（IF-08）");
        }

        [Test]
        public void NewGameState_SatisfiesLineInvariant()
        {
            var state = new GameState();
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

            Assert.AreEqual(0, blocks, "初期状態の盤面ブロック数は 0（IF-01）");
            Assert.AreEqual(0, (state.LinesCleared * 10) + blocks, "消去ライン数 × 10 ＋ 盤面ブロック数 ＝ 4 × ロック数 ＝ 0（RL-36）");
        }

        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(123456789u)]
        [TestCase(uint.MaxValue)]
        public void InjectSeed_InReady_KeepsReadyAndLeavesObservableStateUnchanged(uint seed)
        {
            var state = new GameState();

            state.InjectSeed(seed);

            Assert.AreEqual(GamePhase.Ready, state.Phase, $"シード {seed} を注入しても Ready のまま（ST-06 / IF-10）");
            Assert.IsFalse(state.ActiveMino.HasValue, $"シード {seed} の注入でアクティブミノは配置されない（ST-06 / IF-03）");
            Assert.AreEqual(0, state.NextQueue.Count, $"シード {seed} の注入でネクストキューは補充されない（ST-06 / IF-05）");
            Assert.AreEqual(0, state.Score, $"シード {seed} の注入でスコアは変わらない（ST-06 / IF-06）");
            Assert.AreEqual(0, state.LinesCleared, $"シード {seed} の注入で累計消去ライン数は変わらない（ST-06 / IF-07）");
            Assert.AreEqual(0, state.TickCount, $"シード {seed} の注入で経過ティック数は変わらない（ST-06 / IF-08）");

            for (var y = 0; y < Board.Height; y++)
            {
                for (var x = 0; x < Board.Width; x++)
                {
                    Assert.IsFalse(state.Board.IsOccupied(x, y), $"シード {seed} の注入後もセル ({x}, {y}) は空（ST-06 / IF-01）");
                }
            }
        }

        [Test]
        public void InjectSeed_CalledRepeatedlyInReady_StaysReady()
        {
            var state = new GameState();

            state.InjectSeed(1u);
            state.InjectSeed(0u);
            state.InjectSeed(uint.MaxValue);

            Assert.AreEqual(GamePhase.Ready, state.Phase, "シードを何度注入しても Ready のまま（ST-06 / IF-10）");
            Assert.AreEqual(0, state.TickCount, "シード注入はティックを進めない（IF-08 / IF-10）");
            Assert.IsFalse(state.ActiveMino.HasValue, "シード注入はミノを出現させない（IF-03）");
            Assert.AreEqual(0, state.NextQueue.Count, "シード注入はキューを補充しない（IF-05）");
        }

        [Test]
        public void TwoGameStates_DoNotShareBoardInstance()
        {
            var a = new GameState();
            var b = new GameState();

            Assert.AreNotSame(a.Board, b.Board, "盤面は static に共有された 1 個であってはならない（IF-01）");
        }
    }
}

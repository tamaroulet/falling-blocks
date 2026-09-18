// SPDX-AI-Disclosure: ai-generated
using Game.Core;
using NUnit.Framework;

namespace StandaloneCore.Tests
{
    /// <summary>
    /// issue_12: 盤面の寸法と、表示領域／バッファ領域の区別。
    /// 対応する構造化仕様: RL-01 / RL-02 / RL-03 / RL-18 / PR-06 / PR-07 / PR-08
    /// </summary>
    public class BoardGeometryTests
    {
        private static Board FreshBoard()
        {
            return new GameState().Board;
        }

        [Test]
        public void Dimensions_AreTenByTwentyTwo()
        {
            Assert.AreEqual(10, Board.Width, "盤面幅は 10（PR-06 / RL-01）");
            Assert.AreEqual(20, Board.VisibleHeight, "表示領域の高さは 20（PR-07 / RL-01）");
            Assert.AreEqual(2, Board.BufferHeight, "バッファ領域は 2 行（RL-02）");
            Assert.AreEqual(22, Board.Height, "内部行数は 22（PR-08 / RL-02）");
            Assert.AreEqual(Board.Height, Board.VisibleHeight + Board.BufferHeight, "22 行は表示 20 行とバッファ 2 行の合計（RL-02）");
        }

        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(18, true)]
        [TestCase(19, true)]
        [TestCase(20, false)]
        [TestCase(21, false)]
        [TestCase(22, false)]
        [TestCase(-1, false)]
        [TestCase(int.MinValue, false)]
        [TestCase(int.MaxValue, false)]
        public void IsVisibleRow_IsTrueOnlyForYZeroToNineteen(int y, bool expected)
        {
            Assert.AreEqual(expected, Board.IsVisibleRow(y), $"Y={y} の表示領域判定（RL-01 / RL-03）");
        }

        [TestCase(0, false)]
        [TestCase(19, false)]
        [TestCase(20, true)]
        [TestCase(21, true)]
        [TestCase(22, false)]
        [TestCase(-1, false)]
        [TestCase(int.MinValue, false)]
        [TestCase(int.MaxValue, false)]
        public void IsBufferRow_IsTrueOnlyForYTwentyAndTwentyOne(int y, bool expected)
        {
            Assert.AreEqual(expected, Board.IsBufferRow(y), $"Y={y} のバッファ領域判定（RL-02 / RL-03）");
        }

        [Test]
        public void EveryInternalRow_IsExactlyOneOfVisibleOrBuffer()
        {
            for (var y = 0; y < Board.Height; y++)
            {
                Assert.AreNotEqual(Board.IsVisibleRow(y), Board.IsBufferRow(y), $"Y={y} は表示領域とバッファ領域のどちらか一方でなければならない（RL-01 / RL-02）");
            }
        }

        [TestCase(0, 0, true)]
        [TestCase(9, 0, true)]
        [TestCase(0, 19, true)]
        [TestCase(0, 20, true)]
        [TestCase(9, 21, true)]
        [TestCase(-1, 0, false)]
        [TestCase(10, 0, false)]
        [TestCase(0, -1, false)]
        [TestCase(0, 22, false)]
        [TestCase(int.MinValue, 0, false)]
        [TestCase(int.MaxValue, 0, false)]
        [TestCase(0, int.MinValue, false)]
        [TestCase(0, int.MaxValue, false)]
        public void InBounds_CoversXZeroToNineAndYZeroToTwentyOne(int x, int y, bool expected)
        {
            Assert.AreEqual(expected, Board.InBounds(x, y), $"({x}, {y}) の盤面内判定（RL-03 / RL-18）");
        }

        [TestCase(-1, 0)]
        [TestCase(10, 0)]
        [TestCase(0, -1)]
        [TestCase(0, 22)]
        [TestCase(-1, -1)]
        [TestCase(int.MinValue, 0)]
        [TestCase(int.MaxValue, 0)]
        [TestCase(0, int.MinValue)]
        [TestCase(0, int.MaxValue)]
        [TestCase(int.MinValue, int.MinValue)]
        [TestCase(int.MaxValue, int.MaxValue)]
        public void IsOccupied_OutsideBoard_IsTreatedAsOccupied(int x, int y)
        {
            Assert.IsTrue(FreshBoard().IsOccupied(x, y), $"盤面外 ({x}, {y}) は固定ブロックが存在するものとして扱う（RL-18）");
        }

        [TestCase(0, 0)]
        [TestCase(9, 0)]
        [TestCase(0, 19)]
        [TestCase(9, 19)]
        [TestCase(0, 20)]
        [TestCase(9, 21)]
        public void IsOccupied_InsideFreshBoard_IsEmpty(int x, int y)
        {
            Assert.IsFalse(FreshBoard().IsOccupied(x, y), $"初期盤面の ({x}, {y}) は空（IF-01 / RL-01 / RL-02）");
        }
    }
}

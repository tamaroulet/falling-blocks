// SPDX-AI-Disclosure: ai-generated
using System;
using System.Collections.Generic;
using Game.Core;
using NUnit.Framework;

namespace StandaloneCore.Tests
{
    /// <summary>
    /// issue_14: 出現向き（向き 0）の相対座標と、基準位置 (3, 19) での盤面座標。
    /// 対応する構造化仕様: RL-08 / RL-12 / RL-15 / PR-06 / PR-09 / PR-18 / PR-23 / PR-28 / PR-33 / PR-38 / PR-43 / PR-48
    /// </summary>
    public class MinoShapeSpawnTests
    {
        private const int SpawnX = 3;
        private const int SpawnY = 19;

        private static readonly MinoType[] AllTypes =
        {
            MinoType.I, MinoType.O, MinoType.T, MinoType.S, MinoType.Z, MinoType.J, MinoType.L
        };

        /// <summary>構造化仕様 §5 の形状表（向き 0）をテスト側へ書き写したもの。</summary>
        private static (int X, int Y)[] SpecCells(MinoType type)
        {
            switch (type)
            {
                case MinoType.I: return new (int, int)[] { (0, 2), (1, 2), (2, 2), (3, 2) };
                case MinoType.O: return new (int, int)[] { (1, 1), (2, 1), (1, 2), (2, 2) };
                case MinoType.T: return new (int, int)[] { (0, 1), (1, 1), (2, 1), (1, 2) };
                case MinoType.S: return new (int, int)[] { (0, 1), (1, 1), (1, 2), (2, 2) };
                case MinoType.Z: return new (int, int)[] { (0, 2), (1, 2), (1, 1), (2, 1) };
                case MinoType.J: return new (int, int)[] { (0, 2), (0, 1), (1, 1), (2, 1) };
                case MinoType.L: return new (int, int)[] { (2, 2), (0, 1), (1, 1), (2, 1) };
                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        [Test]
        public void SpawnCells_MatchSpecShapeTable()
        {
            foreach (var type in AllTypes)
            {
                var cells = MinoShape.SpawnCells(type);

                Assert.AreEqual(4, cells.Length, $"{type} は 4 つの正方形ブロックで構成される（RL-08）");
                Assert.AreEqual(4, new HashSet<(int, int)>(cells).Count, $"{type} の 4 マスは相異なる（RL-08）");
                Assert.That(cells, Is.EquivalentTo(SpecCells(type)), $"{type} の向き 0 の相対座標は構造化仕様の形状表どおり（PR-18 / PR-23 / PR-28 / PR-33 / PR-38 / PR-43 / PR-48）");
            }
        }

        [Test]
        public void SpawnCells_AtSpawnOrigin_AllFourCellsLandInBufferRows()
        {
            foreach (var type in AllTypes)
            {
                foreach (var cell in MinoShape.SpawnCells(type))
                {
                    var boardX = SpawnX + cell.X;
                    var boardY = SpawnY + cell.Y;

                    Assert.IsTrue(Board.InBounds(boardX, boardY), $"{type}: 盤面座標 ({boardX}, {boardY}) は盤面内（RL-12 / RL-18）");
                    Assert.IsTrue(Board.IsBufferRow(boardY), $"{type}: 出現直後の 4 マスはすべてバッファ領域 Y=20..21 に入る（RL-15）");
                    Assert.IsFalse(Board.IsVisibleRow(boardY), $"{type}: 出現直後の 4 マスは表示領域 Y=0..19 に入らない（RL-15）");
                    Assert.That(boardY, Is.InRange(Board.VisibleHeight, Board.Height - 1), $"{type}: 盤面座標 Y = {boardY} は 20..21（RL-02 / RL-15）");
                    Assert.That(boardX, Is.InRange(0, Board.Width - 1), $"{type}: 盤面座標 X = {boardX} は 0..9（RL-01 / PR-06）");
                }
            }
        }

        [Test]
        public void SpawnCells_ReturnsIndependentArrayEachCall()
        {
            var first = MinoShape.SpawnCells(MinoType.T);
            first[0] = (99, 99);

            var second = MinoShape.SpawnCells(MinoType.T);

            Assert.AreNotSame(first, second, "同じ配列インスタンスを返し回してはならない（PR-28）");
            Assert.That(second, Is.EquivalentTo(SpecCells(MinoType.T)), "呼び出しのたびに独立した配列を返し、内部テーブルを外へ露出しない（PR-28）");
        }

        [TestCase(7)]
        [TestCase(-1)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public void SpawnCells_WithUndefinedMinoType_Throws(int raw)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => MinoShape.SpawnCells((MinoType)raw), $"列挙に定義の無い値 {raw} は例外（RL-08 / 異常系）");
        }
    }
}

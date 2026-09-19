// SPDX-AI-Disclosure: ai-generated
using System;
using System.Collections.Generic;
using Game.Core;
using NUnit.Framework;

namespace StandaloneCore.Tests
{
    /// <summary>
    /// issue_14: 7-Bag の生成（Fisher-Yates・乱数消費ちょうど 6 回・7 種類ちょうど 1 つずつ）。
    /// 対応する構造化仕様: RL-06 / RL-07 / RL-08 / RL-09 / RL-10 / RL-11 / PR-10
    /// </summary>
    public class SevenBagTests
    {
        private static readonly MinoType[] InitialOrder =
        {
            MinoType.I, MinoType.O, MinoType.T, MinoType.S, MinoType.Z, MinoType.J, MinoType.L
        };

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

        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(7u)]
        [TestCase(12345u)]
        [TestCase(2147483648u)]
        [TestCase(uint.MaxValue)]
        public void Generate_ContainsSevenKindsExactlyOnce(uint seed)
        {
            var bag = SevenBag.Generate(new XorShift32(seed));

            Assert.AreEqual(7, bag.Length, $"1 セットは 7 個（RL-10 / PR-10）: seed={seed}");
            Assert.That(bag, Is.EquivalentTo(InitialOrder), $"7 種類がちょうど 1 つずつ入る。欠けも重複もない（RL-08 / RL-10）: seed={seed}");
            Assert.AreEqual(7, new HashSet<MinoType>(bag).Count, $"同じ種類が 2 つ以上入らない（RL-10）: seed={seed}");
        }

        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(7u)]
        [TestCase(12345u)]
        [TestCase(2147483648u)]
        [TestCase(uint.MaxValue)]
        public void Generate_MatchesFisherYatesReference(uint seed)
        {
            var bag = SevenBag.Generate(new XorShift32(seed));

            Assert.That(bag, Is.EqualTo(ReferenceBag(seed)), $"初期配列 I,O,T,S,Z,J,L を i=6..1 の降順に j = 乱数 % (i + 1) と交換した結果と一致する（RL-09 / PR-10）: seed={seed}");
        }

        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(12345u)]
        [TestCase(uint.MaxValue)]
        public void Generate_ConsumesExactlySixRandomValues(uint seed)
        {
            var used = new XorShift32(seed);
            var reference = new XorShift32(seed);

            SevenBag.Generate(used);

            for (var i = 0; i < 6; i++)
            {
                reference.Next();
            }

            Assert.AreEqual(reference.Next(), used.Next(), $"1 セットの生成で乱数をちょうど 6 回消費する（RL-09）: seed={seed}");
        }

        [Test]
        public void Generate_WithSeedOne_ActuallyShuffles()
        {
            var bag = SevenBag.Generate(new XorShift32(1u));

            Assert.That(bag, Is.Not.EqualTo(InitialOrder), "seed=1 の 1 回目の乱数は 270369、270369 % 7 = 1 なのでインデックス 6 と 1 が入れ替わり、初期配列のままにはならない（RL-09）");
            Assert.AreEqual(MinoType.I, bag[1], "インデックス 1 は i=6 で L になるが、その後 i=5(j=1) と i=1(j=0) の交換で動く。最終値は I（RL-09）");
            Assert.AreNotEqual(MinoType.L, bag[6], "i=6 の交換で末尾は L ではなくなる（RL-09）");
        }

        [Test]
        public void Generate_WithSeedZeroRng_EqualsSeedOneRng()
        {
            var zero = SevenBag.Generate(new XorShift32(0u));
            var one = SevenBag.Generate(new XorShift32(1u));

            Assert.That(zero, Is.EqualTo(one), "シード 0 は 1 に置き換えて初期化するので同じ 1 セットになる（RL-07）");
        }

        [Test]
        public void Generate_ReturnsIndependentArrayEachCall()
        {
            var first = SevenBag.Generate(new XorShift32(1u));
            first[0] = first[1];

            var second = SevenBag.Generate(new XorShift32(1u));

            Assert.AreNotSame(first, second, "同じ配列インスタンスを使い回してはならない（RL-10）");
            Assert.That(second, Is.EqualTo(ReferenceBag(1u)), "呼び出しごとに独立した配列を返し、内部の共有配列を露出しない（RL-10）");
        }

        [Test]
        public void Generate_TwiceFromSameRng_ProducesTwoCompleteSets()
        {
            var rng = new XorShift32(1u);

            var first = SevenBag.Generate(rng);
            var second = SevenBag.Generate(rng);

            Assert.That(first, Is.EquivalentTo(InitialOrder), "1 セット目も 7 種類ちょうど 1 つずつ（RL-10）");
            Assert.That(second, Is.EquivalentTo(InitialOrder), "補充した 2 セット目も 7 種類ちょうど 1 つずつ（RL-10 / RL-11）");
            Assert.That(first, Is.EqualTo(ReferenceBag(1u)), "1 セット目は seed=1 の参照実装と一致（RL-09）");
        }

        [Test]
        public void Generate_WithNullRng_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => SevenBag.Generate(null!), "乱数器が無ければ生成できない（異常系）");
        }
    }
}

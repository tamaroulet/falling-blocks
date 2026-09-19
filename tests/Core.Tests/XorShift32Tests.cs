// SPDX-AI-Disclosure: ai-generated
using Game.Core;
using NUnit.Framework;

namespace StandaloneCore.Tests
{
    /// <summary>
    /// issue_14: 決定論乱数 XorShift32 と既定シード。
    /// 対応する構造化仕様: RL-05 / RL-06 / RL-07 / PR-05
    /// </summary>
    public class XorShift32Tests
    {
        /// <summary>仕様 RL-06 をテスト側で独立に書き下した参照実装。</summary>
        private static uint ReferenceNext(ref uint x)
        {
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            return x;
        }

        [Test]
        public void Next_WithSeedOne_ReturnsHandComputedGoldenValues()
        {
            var rng = new XorShift32(1u);

            Assert.AreEqual(270369u, rng.Next(), "seed=1 の 1 回目は 0x00042021 = 270369（RL-06）");
            Assert.AreEqual(67634689u, rng.Next(), "seed=1 の 2 回目は 0x04080601 = 67634689（RL-06）");
        }

        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(7u)]
        [TestCase(12345u)]
        [TestCase(2147483648u)]
        [TestCase(uint.MaxValue)]
        public void Next_MatchesReferenceImplementation_For64Steps(uint seed)
        {
            var rng = new XorShift32(seed);
            var x = seed;

            for (var step = 1; step <= 64; step++)
            {
                var expected = ReferenceNext(ref x);

                Assert.AreEqual(expected, rng.Next(), $"seed={seed} の {step} 回目は x ^= x << 13; x ^= x >> 17; x ^= x << 5; の更新後の値（RL-06）");
            }
        }

        [Test]
        public void Next_WithSeedZero_BehavesExactlyAsSeedOne()
        {
            var zero = new XorShift32(0u);
            var one = new XorShift32(1u);

            for (var step = 1; step <= 16; step++)
            {
                Assert.AreEqual(one.Next(), zero.Next(), $"シード 0 は 1 に置き換えて初期化する（RL-07 / PR-05）: {step} 回目");
            }
        }

        [Test]
        public void Next_SameSeed_ProducesSameSequence()
        {
            var a = new XorShift32(123456789u);
            var b = new XorShift32(123456789u);

            for (var step = 1; step <= 32; step++)
            {
                Assert.AreEqual(a.Next(), b.Next(), $"同じシードからは常に同じ列が出る。実時間や実行環境に依存しない（RL-05 / RL-06）: {step} 回目");
            }
        }

        [Test]
        public void Next_SeedOneAndSeedTwo_DivergeImmediately()
        {
            var a = new XorShift32(1u);
            var b = new XorShift32(2u);

            Assert.AreEqual(270369u, a.Next(), "seed=1 の 1 回目（RL-06）");
            Assert.AreEqual(540738u, b.Next(), "seed=2 の 1 回目は 0x00084042 = 540738（RL-06）");
        }

        [Test]
        public void Next_WithMaxSeed_NeverReturnsZeroAndNeverSticks()
        {
            var rng = new XorShift32(uint.MaxValue);
            var previous = rng.Next();

            Assert.AreNotEqual(0u, previous, "非零状態の XorShift32 は 0 を返さない（RL-06）: 1 回目");

            for (var step = 2; step <= 1000; step++)
            {
                var value = rng.Next();

                Assert.AreNotEqual(0u, value, $"非零状態の XorShift32 は 0 を返さない（RL-06）: {step} 回目");
                Assert.AreNotEqual(previous, value, $"内部状態が同じ値に固着してはならない（RL-06）: {step} 回目");
                previous = value;
            }
        }
    }
}

// SPDX-AI-Disclosure: ai-assisted
using System;

namespace Game.Core
{
    /// <summary>
    /// 7-Bag 方式によるテトリミノ生成クラス（RL-08 / RL-09 / RL-10 / RL-11 / PR-10）。
    /// </summary>
    public static class SevenBag
    {
        /// <summary>
        /// 7 種類のテトリミノ（I, O, T, S, Z, J, L）を Fisher-Yates アルゴリズムでシャッフルした 1 セットを生成する（RL-09 / RL-10 / PR-10）。
        /// 乱数生成器から乱数をちょうど 6 回消費する。
        /// </summary>
        /// <param name="rng">乱数生成器（RL-06）。</param>
        /// <returns>シャッフルされた長さ 7 のテトリミノ配列。</returns>
        /// <exception cref="ArgumentNullException">rng が null の場合にスローされる。</exception>
        public static MinoType[] Generate(XorShift32 rng)
        {
            if (rng == null)
            {
                throw new ArgumentNullException(nameof(rng));
            }

            var bag = new[]
            {
                MinoType.I, MinoType.O, MinoType.T, MinoType.S, MinoType.Z, MinoType.J, MinoType.L
            };

            for (var i = 6; i >= 1; i--)
            {
                var j = (int)(rng.Next() % (uint)(i + 1));
                var temp = bag[i];
                bag[i] = bag[j];
                bag[j] = temp;
            }

            return bag;
        }
    }
}

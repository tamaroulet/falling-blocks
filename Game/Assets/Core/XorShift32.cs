// SPDX-AI-Disclosure: ai-assisted
namespace Game.Core
{
    /// <summary>
    /// 32 ビット XorShift 擬似乱数生成器（RL-05 / RL-06 / RL-07 / PR-05）。
    /// </summary>
    public sealed class XorShift32
    {
        private uint _x;

        /// <summary>
        /// 指定されたシード値で乱数生成器を初期化する（RL-07 / PR-05）。
        /// シードが 0 の場合は 1 として初期化する。
        /// </summary>
        /// <param name="seed">初期化シード値。</param>
        public XorShift32(uint seed)
        {
            _x = seed == 0u ? 1u : seed;
        }

        /// <summary>
        /// 乱数を 1 つ進めて次の 32 ビット符号なし整数を返す（RL-06）。
        /// x ^= x &lt;&lt; 13; x ^= x &gt;&gt; 17; x ^= x &lt;&lt; 5; の順に更新した後の値を返す。
        /// </summary>
        /// <returns>生成された 32 ビット符号なし整数。</returns>
        public uint Next()
        {
            _x ^= _x << 13;
            _x ^= _x >> 17;
            _x ^= _x << 5;
            return _x;
        }
    }
}

// SPDX-AI-Disclosure: ai-assisted
using System;

namespace Game.Core
{
    /// <summary>
    /// 各テトリミノのブロック配置形状を定義するクラス（RL-08 / RL-15 / PR-06 / PR-18 / PR-23 / PR-28 / PR-33 / PR-38 / PR-43 / PR-48）。
    /// </summary>
    public static class MinoShape
    {
        /// <summary>
        /// 指定されたテトリミノの出現向き（Rotation.Spawn）における 4 つのブロック相対座標を返す（PR-18 / PR-23 / PR-28 / PR-33 / PR-38 / PR-43 / PR-48）。
        /// </summary>
        /// <param name="type">テトリミノの種類。</param>
        /// <returns>長さ 4 の相対座標配列 (X, Y)。</returns>
        /// <exception cref="ArgumentOutOfRangeException">未定義のミノ種別が渡された場合にスローされる。</exception>
        public static (int X, int Y)[] SpawnCells(MinoType type)
        {
            switch (type)
            {
                case MinoType.I:
                    return new (int, int)[] { (0, 2), (1, 2), (2, 2), (3, 2) };
                case MinoType.O:
                    return new (int, int)[] { (1, 1), (2, 1), (1, 2), (2, 2) };
                case MinoType.T:
                    return new (int, int)[] { (0, 1), (1, 1), (2, 1), (1, 2) };
                case MinoType.S:
                    return new (int, int)[] { (0, 1), (1, 1), (1, 2), (2, 2) };
                case MinoType.Z:
                    return new (int, int)[] { (0, 2), (1, 2), (1, 1), (2, 1) };
                case MinoType.J:
                    return new (int, int)[] { (0, 2), (0, 1), (1, 1), (2, 1) };
                case MinoType.L:
                    return new (int, int)[] { (2, 2), (0, 1), (1, 1), (2, 1) };
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "未定義のテトリミノ種類です。");
            }
        }
    }
}

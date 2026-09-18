// SPDX-AI-Disclosure: ai-generated
using System;
using Game.Core;
using NUnit.Framework;

namespace StandaloneCore.Tests
{
    /// <summary>
    /// issue_12: 初期状態を読み取るために公開される型の形。
    /// 対応する構造化仕様: IF-02 / IF-09 / ST-08 / RL-08 / RL-14 / PR-10
    /// </summary>
    public class CoreContractTests
    {
        [Test]
        public void GamePhase_HasReadyPlayingGameOverInThatOrder()
        {
            Assert.AreEqual(3, Enum.GetValues(typeof(GamePhase)).Length, "ゲーム状態は Ready / Playing / GameOver の 3 つだけ（IF-09）");
            Assert.AreEqual(0, (int)GamePhase.Ready, "Ready が先頭（IF-09 / ST-08）");
            Assert.AreEqual(1, (int)GamePhase.Playing, "Playing が 2 番目（ST-08）");
            Assert.AreEqual(2, (int)GamePhase.GameOver, "GameOver が 3 番目。状態は Ready → Playing → GameOver の順にのみ進む（ST-08）");
        }

        [Test]
        public void MinoType_HasSevenKindsInBagOrder()
        {
            var values = (MinoType[])Enum.GetValues(typeof(MinoType));

            Assert.That(values, Is.EqualTo(new[] { MinoType.I, MinoType.O, MinoType.T, MinoType.S, MinoType.Z, MinoType.J, MinoType.L }), "ミノは 7 種類で、宣言順は 7-Bag 初期配列 I, O, T, S, Z, J, L（RL-08 / PR-10）");
        }

        [Test]
        public void Rotation_HasFourOrientationsInClockwiseOrder()
        {
            Assert.AreEqual(4, Enum.GetValues(typeof(Rotation)).Length, "向きは 4 つ（RL-14）");
            Assert.AreEqual(0, (int)Rotation.Spawn, "向き 0 が初期回転状態（RL-15）");
            Assert.AreEqual(1, (int)Rotation.Right, "時計回りに 0 → R（RL-14）");
            Assert.AreEqual(2, (int)Rotation.Two, "時計回りに R → 2（RL-14）");
            Assert.AreEqual(3, (int)Rotation.Left, "時計回りに 2 → L（RL-14）");
        }

        [Test]
        public void ActiveMino_ExposesTypeCoordinatesAndRotation()
        {
            var mino = new ActiveMino(MinoType.T, 3, 19, Rotation.Spawn);

            Assert.AreEqual(MinoType.T, mino.Type, "アクティブミノは種類を持つ（IF-02）");
            Assert.AreEqual(3, mino.X, "アクティブミノは基準位置 X を整数で持つ（IF-02 / RL-12）");
            Assert.AreEqual(19, mino.Y, "アクティブミノは基準位置 Y を整数で持つ（IF-02 / RL-12）");
            Assert.AreEqual(Rotation.Spawn, mino.Rotation, "アクティブミノは向きを持つ（IF-02）");
        }

        [TestCase(-1, -1)]
        [TestCase(0, 21)]
        [TestCase(9, 0)]
        [TestCase(int.MinValue, int.MaxValue)]
        public void ActiveMino_KeepsGivenCoordinatesVerbatim(int x, int y)
        {
            var mino = new ActiveMino(MinoType.I, x, y, Rotation.Left);

            Assert.AreEqual(x, mino.X, $"渡した X={x} をそのまま保持する（IF-02）");
            Assert.AreEqual(y, mino.Y, $"渡した Y={y} をそのまま保持する（IF-02）");
            Assert.AreEqual(MinoType.I, mino.Type, "渡した種類をそのまま保持する（IF-02）");
            Assert.AreEqual(Rotation.Left, mino.Rotation, "渡した向きをそのまま保持する（IF-02）");
        }
    }
}

// SPDX-AI-Disclosure: ai-generated
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    /// <summary>
    /// 対照群。harness は、この名前のテストがエンジン側のスイートで実行されて Passed であることを要求する
    /// （.harness.toml の oracle.control_must_pass）。落ちたらテスト実行環境そのものの故障。
    /// </summary>
    public class ControlGroupTests
    {
        [Test]
        public void AlwaysPasses_ControlGroup()
        {
            Assert.Pass();
        }
    }
}

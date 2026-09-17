using NUnit.Framework;

namespace StandaloneCore.Tests
{
    /// <summary>
    /// 対照群。harness は、この名前のテストが dotnet 側のスイートで実行されて Passed であることを要求する
    /// （.harness.toml の oracle.control_must_pass）。CI の core-tests も、ゴールデンが無い間はこれを見る。
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

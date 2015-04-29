namespace Wacton.Colonies.Tests
{
    using NUnit.Framework;

    using Wacton.Colonies.Domain.Core;

    [TestFixture]
    public class LoggingTests
    {
        [Test]
        public void SettingFlagChangesLogger()
        {
            Assert.IsFalse(Logger.GetFlag());

            Logger.SetFlag(true);
            Assert.IsTrue(Logger.GetFlag());

            Logger.SetFlag(false);
            Assert.IsFalse(Logger.GetFlag());
        }
    }
}
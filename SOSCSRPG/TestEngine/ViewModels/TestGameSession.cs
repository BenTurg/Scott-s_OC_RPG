using Engine.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestEngine.ViewModels
{
    [TestClass]// -> meaning this is UnitTest Class
    public class TestGameSession
    {
        [TestMethod]// -> meaning this is UnitTest function
        public void TestCreateGameSession()
        {
            GameSession gameSession = new GameSession();
            Assert.IsNotNull(gameSession.CurrentPlayer);
            Assert.AreEqual("Town square", gameSession.CurrentLocation.Name);
        }
        [TestMethod]
        public void TestPlayerMovesHomeAndIsCompletelyHealedOnKilled()
        {
            GameSession gameSession = new GameSession();
            gameSession.CurrentPlayer.TakeDamage(gameSession.CurrentPlayer.MaximumHitPoints);
            Assert.AreEqual("Home", gameSession.CurrentLocation.Name);
            Assert.AreEqual(gameSession.CurrentPlayer.Level * 10, gameSession.CurrentPlayer.CurrentHitPoints);
        }
    }
}

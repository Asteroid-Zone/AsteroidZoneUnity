using NUnit.Framework;
using PlayGame.Pings;
using Ping = PlayGame.Pings.Ping;

namespace Tests
{
    public class FirstAttemptAtTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void CreateAPing()
        {
          Ping testPing = new Ping('F', 4, PingType.Asteroid);
          Assert.AreEqual(6, testPing.GetGridCoord().GetX());
          Assert.AreEqual(4, testPing.GetGridCoord().GetZ());
          Assert.AreEqual("Asteroid", testPing.GetPingType().ToString());
            // Use the Assert class to test conditions
        }


    }
}

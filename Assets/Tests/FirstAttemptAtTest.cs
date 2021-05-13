using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;
using Ping = PlayGame.Pings.Ping;

namespace Tests
{
    public class FirstAttemptAtTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void CreateAPing()
        {
          new Ping('A', 1, 1);
          Assert.AreEqual(1, GetGridCoord());
            // Use the Assert class to test conditions
        }


    }
}

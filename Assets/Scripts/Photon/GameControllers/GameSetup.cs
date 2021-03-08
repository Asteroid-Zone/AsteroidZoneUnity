using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhotonClass.GameController
{
    public class GameSetup : MonoBehaviour
    {
        public static GameSetup GS;

        public Transform[] spawnPoints;

        private void OnEnable()
        {
            if (GameSetup.GS == null)
            {
                GameSetup.GS = this;
            }
        }
    }
}

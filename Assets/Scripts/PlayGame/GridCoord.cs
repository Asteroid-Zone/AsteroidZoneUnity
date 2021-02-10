using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PlayGame {
    public struct GridCoord {

        private const int gridSize = 10;

        private char x { get; }
        private int y { get; }
        private int z { get; }

        public GridCoord(char x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public int getWorldX() {
            return (x - 97) * gridSize;
        }

        public int getWorldY() {
            return y * gridSize;
        }

        public int getWorldZ() {
            return z * gridSize;
        }

        public Vector3 getVector() {
            return new Vector3(getWorldX(), getWorldY(), getWorldZ());
        }

    }
}

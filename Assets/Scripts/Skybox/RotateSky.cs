using UnityEngine;

namespace Skybox {
    
    /// <summary>
    /// This class is used to rotate the skybox.
    /// </summary>
    public class RotateSky : MonoBehaviour {
        
        public float rotateSpeed = 1.2f;
        private static readonly int Rotation = Shader.PropertyToID("_Rotation");

        private void Update() {
            RenderSettings.skybox.SetFloat(Rotation, Time.time * rotateSpeed);
        }
    }
}

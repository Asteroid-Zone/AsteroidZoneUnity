using UnityEngine;

namespace PlayGame.Player {
    
    /// <summary>
    /// This class controls the ships thruster particle effect.
    /// </summary>
    public class Thruster : MonoBehaviour {
        
        private Transform _shipTransform;
        private ParticleSystem _thrusterParticles;
        private Vector3 _prevPosition;

        private void Start() {
            _shipTransform = transform.parent.parent.parent;
            _thrusterParticles = GetComponent<ParticleSystem>();
            _prevPosition = _shipTransform.position;
        }

        /// <summary>
        /// Enables the thrusters particle effect if the players ship has moved.
        /// <para>Otherwise disables the thrusters particle effect.</para>
        /// </summary>
        private void Update() {
            Vector3 currentPosition = _shipTransform.position;
            if (currentPosition != _prevPosition) {
                _thrusterParticles.Play();
                _prevPosition = currentPosition;
            } else _thrusterParticles.Stop();
        }
    }
}

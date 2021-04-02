using UnityEngine;

namespace PlayGame.Player
{
    public class Thruster : MonoBehaviour
    {
        private Transform _shipTransform;
        private ParticleSystem _thrusterParticles;
        private Vector3 _prevPosition;

        // Start is called before the first frame update
        private void Start()
        {
            _shipTransform = transform.parent.parent.parent;
            _thrusterParticles = GetComponent<ParticleSystem>();
            _prevPosition = _shipTransform.position;
        }

        // Update is called once per frame
        private void Update()
        {
            Vector3 currentPosition = _shipTransform.position;
            // Probably a better way to do this
            //float speed = (currentPosition - _prevPosition).sqrMagnitude / Time.deltaTime;
            if (currentPosition != _prevPosition)
            {
                _thrusterParticles.Play();
                _prevPosition = currentPosition;
            }
            else
            {
                _thrusterParticles.Stop();
            }
        }
    }
}

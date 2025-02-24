
using SpaceShooterPro;
using UnityEngine;

namespace SpaceShooterPro
{
    public class SpeedPowerUpPickUp : MonoBehaviour, ICollectable
    {
        [SerializeField] private float _boostDuration = 5f;
        [SerializeField] private float _boostFactor = 1.5f;
        private Player _player;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == GlobalVariables.PLAYER_TAG)
            {
                Collect(collision.gameObject);
                AudioManager.Instance.PlayPowerUpPickup(transform.position);
                Destroy(this.gameObject);
            }
        }

        public void Collect(GameObject collector)
        {
            if (collector.TryGetComponent<Player>(out _player))
            {
                _player.EnableSpeedBoost(_boostDuration, _boostFactor);
            }
        }
    }
}
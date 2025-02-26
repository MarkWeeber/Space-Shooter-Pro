using UnityEngine;

namespace SpaceShooterPro
{
    public class HommingProjectilePickUp : MonoBehaviour
    {
        [SerializeField] private float _duration = 5f;
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
                _player.EnableHommingProjectile(_duration);
            }
        }
    }
}
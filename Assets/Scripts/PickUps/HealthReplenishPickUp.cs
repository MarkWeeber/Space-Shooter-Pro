using UnityEngine;

namespace SpaceShooterPro
{
    public class HealthReplenishPickUp : MonoBehaviour, ICollectable
    {
        [SerializeField] private float _healthReplenishAmount = 15f;
        private Health _health;
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
            if (collector.TryGetComponent<Health>(out _health))
            {
                _health.Heal(_healthReplenishAmount);
            }
        }
    }
}
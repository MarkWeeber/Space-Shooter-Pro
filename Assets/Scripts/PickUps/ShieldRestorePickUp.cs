using UnityEngine;

namespace SpaceShooterPro
{
    public class ShieldRestorePickUp : MonoBehaviour, ICollectable
    {
        [SerializeField] private float _shieldRestoreValue = 25;
        private ProtectiveShield _shield;
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
            if (collector.TryGetComponent<ProtectiveShield>(out _shield))
            {
                _shield.RestoreShield(_shieldRestoreValue);
            }
        }
    }
}
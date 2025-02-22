using UnityEngine;

namespace Assets.Scripts._Refactoring
{
    public class AmmoPickUp : MonoBehaviour, ICollectable
    {
        [SerializeField] private int _ammo = 15;
        private Player _player;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == GlobalVariables.PLAYER_TAG)
            {
                Collect(collision.gameObject);
                Destroy(this.gameObject);
            }
        }

        public void Collect(GameObject collector)
        {
            if (collector.TryGetComponent<Player>(out _player))
            {
                _player.AmmoCount += _ammo;
            }
        }
    }
}
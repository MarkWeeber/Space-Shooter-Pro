using UnityEngine;

namespace SpaceShooterPro
{
    public class AreaDamager : MonoBehaviour
    {
        [SerializeField] private float _damage = 25f;
        [SerializeField] private float _radius = 3f;
        [SerializeField] private string _targetTag = "";

        private RaycastHit2D[] _hits;
        private Health _health;

        private void OnEnable()
        {
            DealAreaDamage();
        }

        public void DealAreaDamage()
        {
            _hits = Physics2D.CircleCastAll(transform.position, _radius, Vector2.zero);
            foreach (var hit in _hits)
            {
                if (hit.transform.CompareTag(_targetTag))
                {
                    if (hit.transform.gameObject.TryGetComponent<Health>(out _health))
                    {
                        _health.TakeDamage(_damage);
                    }
                }
            }
        }
    }
}
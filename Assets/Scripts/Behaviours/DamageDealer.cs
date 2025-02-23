
using System;
using UnityEngine;

namespace SpaceShooterPro
{
    public class DamageDealer : MonoBehaviour
    {
        [SerializeField] protected float _damage = 25f;
        [SerializeField] protected string _targetTag = "";
        [SerializeField] protected Transform _remainsAfterDestroy;
        public float Damage { get => _damage; set => _damage = value; }
        public Action DamageDealt;
        

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == _targetTag)
            {
                if (other.gameObject.TryGetComponent<Health>(out var healthComponent))
                {
                    healthComponent.TakeDamage(_damage);
                    DamageDealt?.Invoke();
                    LeaveRemainsAfterDestroy();
                    Destroy(this.gameObject);
                }
            }
        }

        private void LeaveRemainsAfterDestroy()
        {
            if (_remainsAfterDestroy != null)
            {
                _remainsAfterDestroy.gameObject.SetActive(true);
                _remainsAfterDestroy.parent = null;
            }
        }
    }
}
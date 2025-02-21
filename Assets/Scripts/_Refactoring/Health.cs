using System;
using UnityEngine;

namespace Assets.Scripts._Refactoring
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float _startHealth = 100f;
        [SerializeField] private float _currentHealth = 100f;

        public Action<float, float> HealthUpdated;
        public Action GotHealed, TookDamage, OnDeath;


        public void TakeDamage(float damageValue)
        {
            _currentHealth -= damageValue;
            if (_currentHealth < 0f)
            {
                // death
                OnDeath?.Invoke();
            }
            HealthUpdated?.Invoke(_startHealth, _currentHealth);
            TookDamage?.Invoke();
        }

        public void Heal(float healValue)
        {
            _currentHealth += healValue;
            HealthUpdated?.Invoke(_startHealth, _currentHealth);
            GotHealed?.Invoke();
        }
    }
}

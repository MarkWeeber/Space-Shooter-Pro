using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts._Refactoring
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float _startHealth = 100f;
        [SerializeField] private float _currentHealth = 100f;
        private ProtectiveShield _protectiveShield;
        public ProtectiveShield ProtectiveShield { get => _protectiveShield; set => _protectiveShield = value; }
        private float _shieldExessDamage;

        public Action<float, float> HealthUpdated;
        public Action GotHealed, TookDamage;
        public UnityEvent OnDeath; // it's needed to be UnityEvent because some scripted events in Scene depend on this


        public void TakeDamage(float damageValue)
        {
            _shieldExessDamage = ShieldExcessDamage(damageValue);
            if (_shieldExessDamage > 0) // damage was taken
            {
                _currentHealth -= _shieldExessDamage;
                if (_currentHealth < 0f)
                {
                    OnDeath?.Invoke(); // death event
                }
                HealthUpdated?.Invoke(_startHealth, _currentHealth);
                TookDamage?.Invoke();
            }
        }

        public void Heal(float healValue)
        {
            _currentHealth += healValue;
            HealthUpdated?.Invoke(_startHealth, _currentHealth);
            GotHealed?.Invoke();
        }

        private float ShieldExcessDamage(float damageValue)
        {
            if (_protectiveShield != null)
            {
                if (_protectiveShield.IsShieldActive)
                {
                    float shieldExessDamage = damageValue - _protectiveShield.ShieldValue;
                    _protectiveShield.TakeDamage(damageValue);
                    if (shieldExessDamage > 0f)
                    {
                        return shieldExessDamage;
                    }
                    else
                    {
                        return 0f;
                    }
                }
            }
            return damageValue;
        }
    }
}

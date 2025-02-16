using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float _health = 100f;
    private float _shieldValue;
    public float ShieldValue { get => _shieldValue; set => _shieldValue = value; }
    public Action OnShieldDepleted;

    private float _damageToHealth;

    public void TakeDamage(float damage)
    {
        _damageToHealth = damage;
        if (_shieldValue > 0f)
        {
            _shieldValue -= damage;
            if (_shieldValue <= 0f)
            {
                _damageToHealth = 0f - _shieldValue;
                _shieldValue = 0f;
                OnShieldDepleted?.Invoke();
            }
            else
            {
                _damageToHealth = 0f;
            }
        }
        if (_shieldValue <= 0f)
        {
            _health -= _damageToHealth;
            if (_health <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
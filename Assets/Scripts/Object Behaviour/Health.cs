using UnityEngine;

public abstract class Health : MonoBehaviour
{
    [SerializeField] protected float _startingHealth = 100f;
    protected float _currentHealth = 100f;
    protected float _shieldValue;
    private float _damageToHealth;

    private void Start()
    {
        _currentHealth = _startingHealth;
    }

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
                ShieldDepletedEvent();
            }
            else
            {
                _damageToHealth = 0f;
            }
        }
        if (_shieldValue <= 0f)
        {
            _currentHealth = Mathf.Clamp(_currentHealth - _damageToHealth, 0f, _startingHealth);
            DamageTakenEvent();
            if (_currentHealth <= 0)
            {
                KilledEvent();
                Destroy(this.gameObject);
            }
        }
    }

    protected virtual void ShieldDepletedEvent()
    {
    }

    protected virtual void DamageTakenEvent()
    {
    }

    protected virtual void KilledEvent()
    {
    }
}
using System;

public class PlayerHealth : Health
{
    public float StartingHealth { get => _startingHealth; }
    public float CurrentHealth { get => _currentHealth; set { _currentHealth = value; HealthChanged(); } }
    public float ShieldValue { get => _shieldValue; set => _shieldValue = value; }
    public Action OnShieldDepleted;
    public Action<float, float> OnDamageTaken;
    public Action<float> ShieldDamaged;

    protected override void ShieldDepletedEvent()
    {
        OnShieldDepleted?.Invoke();
    }

    protected override void DamageTakenEvent()
    {
        UIManager.Instance.UpdateHealth(_startingHealth, _currentHealth);
        OnDamageTaken?.Invoke(_startingHealth, _currentHealth);
    }

    private void HealthChanged()
    {
        UIManager.Instance.UpdateHealth(_startingHealth, _currentHealth);
    }

    protected override void KilledEvent()
    {
        UIManager.Instance.SetGameOver();
        GameManager.Instance.SetGameOver();
    }

    protected override void ShieldDamagedEvent()
    {
        ShieldDamaged?.Invoke(_shieldValue);
    }

}
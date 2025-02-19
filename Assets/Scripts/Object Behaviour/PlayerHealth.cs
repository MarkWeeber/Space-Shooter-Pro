using System;

public class PlayerHealth : Health
{
    public float ShieldValue { get => _shieldValue; set => _shieldValue = value; }
    public Action OnShieldDepleted;
    public Action<float, float> OnDamageTaken;

    protected override void ShieldDepletedEvent()
    {
        OnShieldDepleted?.Invoke();
    }

    protected override void DamageTakenEvent()
    {
        UIManager.Instance.UpdateHealth(_startingHealth, _currentHealth);
        OnDamageTaken?.Invoke(_startingHealth, _currentHealth);
    }

    protected override void KilledEvent()
    {
        UIManager.Instance.SetGameOver();
        GameManager.Instance.SetGameOver();
    }
}
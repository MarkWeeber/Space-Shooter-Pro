using System;
using UnityEngine;

public class PlayerHealth : Health
{
    public float ShieldValue { get => _shieldValue; set => _shieldValue = value; }
    public Action OnShieldDepleted;

    protected override void ShieldDepletedEvent()
    {
        OnShieldDepleted?.Invoke();
    }

    protected override void DamageTakenEvent()
    {
        UIManager.Instance.UpdateHealth(_startingHealth, _currentHealth);
    }

    protected override void KilledEvent()
    {
        UIManager.Instance.SetGameOver();
    }
}
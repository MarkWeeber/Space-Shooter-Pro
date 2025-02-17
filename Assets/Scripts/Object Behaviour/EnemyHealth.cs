using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private int _scoreValue = 10;

    protected override void KilledEvent()
    {
        UIManager.Instance.AddScore(_scoreValue);
    }
}
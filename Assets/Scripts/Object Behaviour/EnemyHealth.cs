using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private int _scoreValue = 10;
    [SerializeField] protected Transform _objectOnDestroy;
    protected override void KilledEvent()
    {
        UIManager.Instance.AddScore(_scoreValue);
        if (_objectOnDestroy != null)
        {
            _objectOnDestroy.gameObject.SetActive(true);
            _objectOnDestroy.parent = null;
        }
    }
}
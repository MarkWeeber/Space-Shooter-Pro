using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private int _scoreValue = 10;
    [SerializeField] protected Transform _objectOnDestroy;
    protected override void KilledEvent()
    {
        UIManager.Instance.AddScore(_scoreValue);
        PlayExplosionSound();
        LeaveDestroyedObjectAfterKilled();
    }

    public void LeaveDestroyedObjectAfterKilled()
    {
        if (_objectOnDestroy != null)
        {
            _objectOnDestroy.gameObject.SetActive(true);
            _objectOnDestroy.parent = null;
        }
    }

    private void PlayExplosionSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayExplosion(transform.position);
        }
    }
}
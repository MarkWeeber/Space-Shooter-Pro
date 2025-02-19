using UnityEngine;

public class EnemyDamageDealer : DamageDealer
{
    [SerializeField] private EnemyHealth _enemyHealth;

    protected override void OnDamageDealt()
    {
        if (_enemyHealth != null)
        {
            _enemyHealth.LeaveDestroyedObjectAfterKilled();
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayExplosion(transform.position);
        }
    }
}
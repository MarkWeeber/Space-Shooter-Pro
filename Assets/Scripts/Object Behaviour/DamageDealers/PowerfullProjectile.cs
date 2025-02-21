using UnityEngine;

public class PowerfullProjectile : DamageDealer
{
    [SerializeField] private Transform _damageHalo;
    [SerializeField] private float _areaDamage = 20;
    [SerializeField] private float _areaRadiusDamage = 3;
    private RaycastHit2D[] _affectedObjects;
    private Health _targetHealth;
    protected override void OnDamageDealt()
    {
        _affectedObjects = Physics2D.CircleCastAll(transform.position, _areaRadiusDamage, Vector2.zero);
        _damageHalo.gameObject.SetActive(true);
        _damageHalo.parent = null;
        foreach (var hit in _affectedObjects)
        {
            if (hit.collider.gameObject.CompareTag(_targetTag))
            {
                if (hit.collider.gameObject.TryGetComponent<Health>(out _targetHealth))
                {
                    _targetHealth.TakeDamage(_areaDamage);
                }
            }
        }
    }
}

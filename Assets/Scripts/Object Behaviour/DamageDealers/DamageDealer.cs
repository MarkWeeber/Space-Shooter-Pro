using UnityEngine;

public abstract class DamageDealer : MonoBehaviour
{
    [SerializeField] protected float _damage = 25f;
    [SerializeField] protected string _targetTag = "";

    public float Damage { get => _damage; set => _damage = value; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == _targetTag)
        {
            if (other.gameObject.TryGetComponent<Health>(out var healthComponent))
            {
                healthComponent.TakeDamage(_damage);
                OnDamageDealt();
                Destroy(this.gameObject);
            }
        }
    }

    protected virtual void OnDamageDealt()
    {

    }
}

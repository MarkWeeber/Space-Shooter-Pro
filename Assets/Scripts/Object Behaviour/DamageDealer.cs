using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private float _damage = 25f;
    [SerializeField] private string _targetTag = "";

    public float Damage { get => _damage; set => _damage = value; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == _targetTag)
        {
            if (other.gameObject.TryGetComponent<Health>(out var healthComponent))
            {
                healthComponent.TakeDamage(_damage);
                Destroy(this.gameObject);
            }
        }
    }
}

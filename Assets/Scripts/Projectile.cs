using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _lifeTime = 5f;
    private bool _setOff;
    private float _damage = 0f;

    private void Start()
    {
        Destroy(this.gameObject, _lifeTime);
    }

    private void Update()
    {
        ManageTravelling();
    }

    private void ManageTravelling()
    {
        if (_setOff)
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
        }
    }

    public void SetOff(float damage)
    {
        _setOff = true;
        _damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == GlobalVariables.ENEMY_TAG)
        {
            if (other.gameObject.TryGetComponent<Health>(out var healthComponent))
            {
                healthComponent.TakeDamage(_damage);
            }
            Destroy(this.gameObject);
        }
    }
}
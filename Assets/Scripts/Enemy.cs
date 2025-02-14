using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _damage = 25f;
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _lifeTime = 10f;
    [SerializeField] private Vector3 _bounds = new Vector3(5f, 10f, 0f);

    private void Start()
    {
        Destroy(this.gameObject, _lifeTime);
        _bounds = new Vector3(
                Mathf.Abs(_bounds.x),
                Mathf.Abs(_bounds.y),
                Mathf.Abs(_bounds.z)
            );
    }

    private void Update()
    {
        ManageTravelling();
        ManageOutOfBounds();
    }

    private void ManageOutOfBounds()
    {
        if (transform.position.x < -_bounds.x || transform.position.x > _bounds.x)
        {
            Destroy(this.gameObject);
        }
        if (transform.position.y < -_bounds.y || transform.position.y > _bounds.y)
        {
            Destroy(this.gameObject);
        }
    }

    private void ManageTravelling()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == GlobalVariables.PLAYER_TAG)
        {
            if (other.gameObject.TryGetComponent<Health>(out var healthComponent))
            {
                healthComponent.TakeDamage(_damage);
                Destroy(this.gameObject);
            }
        }
    }
}
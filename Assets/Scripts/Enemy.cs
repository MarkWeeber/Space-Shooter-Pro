using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _damage = 25f;
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _lifeTime = 10f;

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
        if (other.tag == GlobalVariables.OUT_OF_BOUNDS_TAG)
        {
            Destroy(this.gameObject);
        }
    }
}
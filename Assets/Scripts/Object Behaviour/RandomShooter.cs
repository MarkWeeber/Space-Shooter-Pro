using UnityEngine;

public class RandomShooter : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Vector3 _shootingPortOffest = new Vector3(0, -0.5f, 0f);
    [SerializeField] private float _fireRate = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float _fireProbability = 0.25f;

    private Vector3 _shootingPort;
    private SimpleRateLimiter fireRateLimiter;
    private float _chance;

    private void Start()
    {
        fireRateLimiter.DropTime = Time.deltaTime + _fireRate;
    }

    private void Update()
    {
        ManageRandomShooting();
    }

    private void ManageRandomShooting()
    {
        if (fireRateLimiter.IsReady(Time.time))
        {
            _chance = Random.Range(0f, 1f);
            if (_chance <= _fireProbability)
            {
                Fire();
            }
        }
        fireRateLimiter.SetNewRate(Time.time, _fireRate);
    }

    private void Fire()
    {
        if (_projectilePrefab != null)
        {
            _shootingPort = transform.position + _shootingPortOffest;
            Instantiate(_projectilePrefab, _shootingPort, Quaternion.identity);
        }
    }
}

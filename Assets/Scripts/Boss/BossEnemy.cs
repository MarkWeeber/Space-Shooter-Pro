using System.Collections;
using UnityEngine;

namespace SpaceShooterPro
{
    public class BossEnemy : MonoBehaviour
    {
        [Header("Basic")]
        [SerializeField] private Health _health;
        [SerializeField] protected Transform _remainsAfterDestroy;
        [Header("Movement")]
        [SerializeField] private float _minMoveSpeed = 4f;
        [SerializeField] private float _maxMoveSpeed = 6f;
        [SerializeField] private Vector3[] _waypoints;
        [SerializeField] private float _minDistanceTillWaypoint = 0.05f;

        #region service variables
        private Vector3 _currentDestination;
        private Vector3 _currentMovementDirection;
        private float _distanceToDestination;
        private float _movementSpeed;
        #endregion

        #region init

        private void Start()
        {
            if (_waypoints.Length > 0)
            {
                SetNewDestination();
            }
            AssignToEvents();
            UIManager.Instance.BossSpawned();
        }

        #endregion

        private void OnDestroy()
        {
            RemoveEventAssignments();
        }

        #region movement
        private void Update()
        {
            if (_waypoints.Length > 0)
            {
                ManageMovement();
            }
        }

        private void ManageMovement()
        {
            _distanceToDestination = Vector3.Distance(_currentDestination, transform.position);
            if (_distanceToDestination < _minDistanceTillWaypoint)
            {
                SetNewDestination();
            }
            else
            {
                _currentMovementDirection = _currentDestination - transform.position;
                _currentMovementDirection.Normalize();
                transform.position += _currentMovementDirection * _movementSpeed * Time.deltaTime;
            }
        }

        private void SetNewDestination()
        {
            _currentDestination = _waypoints[Random.Range(0, _waypoints.Length)];
            _movementSpeed = Random.Range(_minMoveSpeed, _maxMoveSpeed);
        }

        private void LeaveRemainsAfterDestroy()
        {
            if (_remainsAfterDestroy != null)
            {
                _remainsAfterDestroy.gameObject.SetActive(true);
                _remainsAfterDestroy.parent = null;
            }
        }

        #endregion

        #region events

        private void AssignToEvents()
        {
            _health.OnDeath.AddListener(OnDeath);
            _health.HealthUpdated += HealthUpdated;
        }

        private void RemoveEventAssignments()
        {
            _health.HealthUpdated -= HealthUpdated;
        }

        private void OnDeath()
        {
            UIManager.Instance.BossDefeated();
            LeaveRemainsAfterDestroy();
            Destroy(gameObject);
        }

        private void HealthUpdated(float startHealth, float currentHealth)
        {
            UIManager.Instance.UpdateBossHealth(startHealth, currentHealth);
        }


        #endregion

    }
}
using System.Collections;
using UnityEngine;

namespace SpaceShooterPro
{
    public class BossEnemy : MonoBehaviour
    {
        [Header("Basic")]
        [SerializeField] private float _startingHealth = 1000f;
        [SerializeField] private Health _health;
        [Header("Movement")]
        [SerializeField] private float _minMoveSpeed = 4f;
        [SerializeField] private float _maxMoveSpeed = 6f;
        [SerializeField] private Vector2[] _waypoints;
        [SerializeField] private float _minDistanceTillWaypoint = 0.05f;
        [Header("Firing")]
        [SerializeField] private float _simpleFireRate = 0.4f;
        [SerializeField] private GameObject _simpleProjectile;
        [SerializeField] private GameObject _homingProjectile;
        [SerializeField] private float _minHommingFireRate = 2f;
        [SerializeField] private float _maxHommingFireRate = 4f;

        #region service variables
        private Vector2 _currentDestination;
        private Vector2 _currentMovementDirection;
        private float _distanceToDestination;
        private float _movementSpeed;
        private IEnumerator _movementCoroutine;
        #endregion

        #region init

        private void Start()
        {
            if (_waypoints.Length > 0)
            {
                SetNewDestination();
            }
        }

        #endregion

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
            _distanceToDestination = Vector2.Distance(_currentDestination, transform.position);
            if (_distanceToDestination < _minDistanceTillWaypoint)
            {
                SetNewDestination();
            }
            else
            {
                _currentMovementDirection = (Vector2)transform.position - _currentDestination;
                _currentMovementDirection.Normalize();
                transform.Translate(_currentMovementDirection * _movementSpeed * Time.deltaTime);
            }
        }

        private void SetNewDestination()
        {
            _currentDestination = _waypoints[Random.Range(0, _waypoints.Length)];
            _movementSpeed = Random.Range(_minMoveSpeed, _maxMoveSpeed);
        }

        #endregion

        #region events

        #endregion

    }
}
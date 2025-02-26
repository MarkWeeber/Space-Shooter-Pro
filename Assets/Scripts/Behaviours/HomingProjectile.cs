using UnityEngine;

namespace SpaceShooterPro
{
    public class HomingProjectile : MonoBehaviour
    {
        [SerializeField] private float _travellingSpeed = 20f;
        [SerializeField] private Traveller _traveller;
        [SerializeField] private TriggerCaller2D _triggerCaller2D;

        private bool _homing;
        private Transform _targetTransform;
        private Vector3 _directionToTarget;
        private RaycastHit2D[] _raycastHits2D;

        private void Start()
        {
            _triggerCaller2D.OnTriggerEnterWithColliderData += OnTargetFound;
        }

        private void OnDestroy()
        {
            _triggerCaller2D.OnTriggerEnterWithColliderData -= OnTargetFound;
        }

        private void Update()
        {
            if (_homing)
            {
                TravelToTargetPosition(Time.deltaTime);
            }
        }

        private void OnTargetFound(Collider2D collider2D)
        {
            _homing = true;
            _traveller.enabled = false;
            _targetTransform = collider2D.transform;
        }

        private void TravelToTargetPosition(float deltaTime)
        {
            if (_targetTransform == null) // if somehow target is missing
            {
                _homing = false;
                _traveller.enabled = true;
                return;
            }
            _directionToTarget = _targetTransform.position - transform.position;
            _directionToTarget.Normalize();
            LookAtTarget();
            MoveTowardsTarget(deltaTime);
        }

        private void LookAtTarget()
        {
            transform.up = _directionToTarget;
        }

        private void MoveTowardsTarget(float deltaTime)
        {
            transform.Translate(_directionToTarget * _travellingSpeed * deltaTime, Space.World);
        }
    }
}
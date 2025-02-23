using System;
using UnityEngine;

namespace SpaceShooterPro
{
    public class Traveller : MonoBehaviour
    {
        [Serializable]
        public enum TravelSpaceDirection
        {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3
        }

        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _lifeTime = 5f;
        [SerializeField] private float _rotationSpeed = 0f;
        [SerializeField] private Vector3 _bounds = new Vector3(10f, 10f, 0f);
        [SerializeField] private TravelSpaceDirection _travelDirection = TravelSpaceDirection.Down;
        [SerializeField] private bool _enableComplexMovement = false;
        [SerializeField] private AnimationCurve _xCurveAdjustment;
        [SerializeField] private float _xCurveAmplitudeMultiplier = 2f;
        [SerializeField] private float _xCurveTimeMultiplier = 0.5f;

        private Vector3 _intialDirection = Vector3.zero;
        private Vector3 _adjustedDirection = Vector3.zero;
        private float _randomValue;

        private void Start()
        {
            SetUpVectors();
        }

        private void OnEnable()
        {
            if (_lifeTime > 0)
            {
                Destroy(this.gameObject, _lifeTime);
            }
        }

        private void Update()
        {
            if (enabled)
            {
                ManageTravelling(Time.deltaTime);
            }
            ManageOutOfBounds();
        }

        private void SetUpVectors()
        {
            _bounds = new Vector3(
                    Mathf.Abs(_bounds.x),
                    Mathf.Abs(_bounds.y),
                    Mathf.Abs(_bounds.z)
                    );
            switch (_travelDirection)
            {
                case TravelSpaceDirection.Up:
                    _intialDirection = Vector2.up;
                    break;
                case TravelSpaceDirection.Down:
                    _intialDirection = Vector2.down;
                    break;
                case TravelSpaceDirection.Left:
                    _intialDirection = Vector2.left;
                    break;
                case TravelSpaceDirection.Right:
                    _intialDirection = Vector2.right;
                    break;
                default:
                    _intialDirection = Vector2.up;
                    break;
            }
            if (_enableComplexMovement)
            {
                _randomValue = UnityEngine.Random.Range(0f, 10f);
            }
        }

        private void ManageTravelling(float deltaTime)
        {
            if (_enableComplexMovement)
            {
                _adjustedDirection = new Vector3(
                        _intialDirection.x + _xCurveAdjustment.Evaluate(((Time.time + _randomValue) * _xCurveTimeMultiplier) % 1) * _xCurveAmplitudeMultiplier,
                        _intialDirection.y,
                        _intialDirection.z
                    );
            }
            else
            {
                _adjustedDirection = _intialDirection;
            }
            if (_speed > 0)
            {
                transform.Translate(_adjustedDirection * _speed * deltaTime);
            }
            if (_rotationSpeed > 0f)
            {
                transform.Rotate(Vector3.forward * _rotationSpeed * deltaTime);
            }
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
    }
}
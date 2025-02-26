using System;
using UnityEngine;

namespace SpaceShooterPro
{
    public class Traveller : MonoBehaviour
    {
        [Serializable]
        public enum TravelWorldSpaceDirection
        {
            Up_Forward = 0,
            Down_Backward = 1,
            Left = 2,
            Right = 3
        }
        [SerializeField] private TravelWorldSpaceDirection _travelDirection = TravelWorldSpaceDirection.Down_Backward;
        public TravelWorldSpaceDirection TravelDirection { get => _travelDirection; set { _travelDirection = value; SetUpVectors(); } }
        [SerializeField] private Space _selectedSpace = Space.World;
        [SerializeField] private float _speed = 5f;
        public float Speed { get => _speed; set => _speed = value; }
        [SerializeField] private float _lifeTime = 5f;
        [SerializeField] private float _rotationSpeed = 0f;
        [SerializeField] private Vector3 _bounds = new Vector3(10f, 10f, 0f);

        [Header("Complex Adjustment")]
        [SerializeField] private bool _enableComplexMovement = false;
        [SerializeField] private AnimationCurve _xCurveAdjustment;
        [SerializeField] private float _xCurveAmplitudeMultiplier = 2f;
        [SerializeField] private float _xCurveTimeMultiplier = 0.5f;

        private Vector3 _intialDirection = Vector3.zero;
        private Vector3 _finalDirection = Vector3.zero;
        private float _randomValue;

        private void Start()
        {
            _bounds = new Vector3(
                    Mathf.Abs(_bounds.x),
                    Mathf.Abs(_bounds.y),
                    Mathf.Abs(_bounds.z)
                    );
            SetUpVectors();
            if (_enableComplexMovement)
            {
                _randomValue = UnityEngine.Random.Range(0f, 10f);
            }
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
            switch (_travelDirection)
            {
                case TravelWorldSpaceDirection.Up_Forward:
                    _intialDirection = Vector2.up;
                    break;
                case TravelWorldSpaceDirection.Down_Backward:
                    _intialDirection = Vector2.down;
                    break;
                case TravelWorldSpaceDirection.Left:
                    _intialDirection = Vector2.left;
                    break;
                case TravelWorldSpaceDirection.Right:
                    _intialDirection = Vector2.right;
                    break;
                default:
                    _intialDirection = Vector2.up;
                    break;
            }
        }

        private void ManageTravelling(float deltaTime)
        {
            if (_enableComplexMovement)
            {
                _finalDirection = new Vector3(
                        _intialDirection.x + _xCurveAdjustment.Evaluate((Time.time + _randomValue) * _xCurveTimeMultiplier % 1) * _xCurveAmplitudeMultiplier,
                        _intialDirection.y,
                        _intialDirection.z
                    );
            }
            else
            {
                _finalDirection = _intialDirection;
            }
            if (_speed > 0)
            {
                transform.Translate(_finalDirection * _speed * deltaTime, _selectedSpace);
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
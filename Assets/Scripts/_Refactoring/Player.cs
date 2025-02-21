using UnityEngine;

namespace Assets.Scripts._Refactoring
{
    public class Player : MonoBehaviour
    {
        [Header("Player positioning")]
        [SerializeField] private float _speed = 3f;
        [SerializeField] private float _sprintSpeedFactor = 1.5f;
        [SerializeField] private float _sprintMaxDuration = 10f;
        [SerializeField] private float _sprintConsumeRate = 5f;
        [SerializeField] private float _sprintRestoreRate = 5f;
        [SerializeField] private Vector2 _startPosition = new Vector2(0f, 0f);
        [SerializeField] private Vector2 _bounds = new Vector2(10f, 4f);

        [Header("Player shooting")]
        [SerializeField] private int _ammouCount = 15;
        [SerializeField] private float _fireRate = 0.2f;
        [SerializeField] private float _damage = 20f;
        [SerializeField] private Transform[] _firePorts = new Transform[0];
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private GameObject _powerfullProjectile;
        [SerializeField] private Transform _projectileRoot;

        [Header("Player Health")]
        [SerializeField] private Health _health;

        #region service variables
        private float _horizontalInput, _verticalInput, _sprintAmmount, _sprintFactor, _speedBoostFactor, _maxX, _maxY, _minX, _minY;
        private bool _fireAllowed, _sprintReady;
        private Vector3 _movementVector, _currentPosition;
        private SimpleRateLimiter _sprintReteLimiter, _fireRateLimiter;
        #endregion

        private void Start()
        {
            AssignToEvents();
        }

        private void OnDestroy()
        {
            RemoveAssignments();
        }

        #region Update
        private void Update()
        {
            ManageControls();
            ManagePlayerBounds();
        }

        private void ManageControls()
        {
            // getting input axies
            _horizontalInput = Input.GetAxis(GlobalVariables.HORIZONTAL_AXIS);
            _verticalInput = Input.GetAxis(GlobalVariables.VERTICAL_AXIS);
            _movementVector = new Vector3(_horizontalInput, _verticalInput, 0f);

            // sprinting check
            _sprintReady = _sprintReteLimiter.IsReady(Time.time);
            if (Input.GetKey(GlobalVariables.SPRINT_KEYCODE) && _sprintReady)
            {
                // consume sprinting
                if (_sprintAmmount > 0f)
                {
                    _sprintAmmount -= Time.deltaTime * _sprintConsumeRate;
                    _sprintFactor = _sprintSpeedFactor;
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.SetSprintBarFillAmout(_sprintAmmount / _sprintMaxDuration);
                    }
                }
                // sprint fully consumed - reset cooldown
                else
                {
                    _sprintFactor = 1f;
                    _sprintAmmount = _sprintMaxDuration;
                    _sprintReteLimiter.SetNewRate(Time.time, _sprintRestoreRate);
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.CooldownSprintBar(_sprintRestoreRate);
                    }
                }
            }
            else if (Input.GetKeyUp(GlobalVariables.SPRINT_KEYCODE))
            {
                _sprintFactor = 1f;
            }

            // moving
            transform.Translate(_movementVector * _speed * _speedBoostFactor * _sprintFactor * Time.deltaTime);

            // firing check
            _fireAllowed = _fireRateLimiter.IsReady(Time.time);
            if (Input.GetKeyDown(GlobalVariables.JUMP_KEYCODE) && _fireAllowed)
            {
                //Fire();
                _fireRateLimiter.SetNewRate(Time.time, _fireRate);
            }
        }

        private void ManagePlayerBounds()
        {
            _maxX = _startPosition.x + _bounds.x;
            _maxY = _startPosition.y + _bounds.y;
            _minX = _startPosition.x - _bounds.x;
            _minY = _startPosition.y - _bounds.y;
            _currentPosition = transform.position;
            transform.position = new Vector3(
                Mathf.Clamp(_currentPosition.x, _minX, _maxX),
                Mathf.Clamp(_currentPosition.y, _minY, _maxY),
                transform.position.z);
        }

        #endregion

        #region Events
        private void AssignToEvents()
        {
            _health.OnDeath += OnDeath;
        }

        private void RemoveAssignments()
        {
            _health.OnDeath -= OnDeath;
        }

        private void OnDeath()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
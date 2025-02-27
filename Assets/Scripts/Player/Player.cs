using System.Collections;
using UnityEngine;

namespace SpaceShooterPro
{
    public class Player : MonoBehaviour
    {
        [Header("Player positioning")]
        [SerializeField] private float _speed = 8f;
        [SerializeField] private float _sprintSpeedFactor = 2f;
        [SerializeField] private float _sprintMaxDuration = 10f;
        [SerializeField] private float _sprintConsumeRate = 5f;
        [SerializeField] private float _sprintRestoreRate = 5f;
        [SerializeField] private Vector2 _startPosition = new Vector2(0f, 0f);
        [SerializeField] private Vector2 _bounds = new Vector2(9f, 4f);

        [Header("Player shooting")]
        [SerializeField] private int _ammoCount = 15;
        public int AmmoCount { get => _ammoCount; set { _ammoCount = value; UpdateAmmoUI(); } }
        [SerializeField] private float _fireRate = 0.2f;
        [SerializeField] private float _damage = 25;
        [SerializeField] private Transform[] _firePorts = new Transform[0];
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private GameObject _powerfullProjectile;
        [SerializeField] private GameObject _homingProjectile;
        [SerializeField] private Transform _projectileRoot;

        [Header("Player Health")]
        [SerializeField] private Health _health;
        [SerializeField] private ProtectiveShield _protectiveShield;
        [SerializeField] private Transform[] _damageEffects;
        [Header("Player Collecting")]
        [SerializeField] private float _attractionSpeed = 1f;
        [SerializeField] private float _attractionRadius = 4f;

        #region service variables
        private float _horizontalInput, _verticalInput, _sprintAmmount, _sprintFactor, _speedBoostFactor, _maxX, _maxY, _minX, _minY;
        private bool _fireAllowed, _sprintReady, _powerfulProjectileAllowed, _homingProjectileAllowed;
        private Vector3 _movementVector, _currentPosition, _spawnPosition, _directionToPlayer;
        private SimpleRateLimiter _sprintReteLimiter, _fireRateLimiter;
        private GameObject _projectile;
        private DamageDealer _damageDealer;
        private IEnumerator _trippleProjectileCoroutine, _speedCoroutine, _powerfullProjectileCoroutine, _hommingProjectileRoutine;
        private RaycastHit2D[] _attractedPickupsHits;
        #endregion

        #region init
        private void Start()
        {
            if (UIManager.Instance == null)
            {
                Debug.LogError("UI Manager instance not found");
            }
            AssignToEvents();
            SetStartPosition();
            InitializeCoroutines();
            SetRateLimiters();
            UpdateAmmoUI();
            _health.ProtectiveShield = _protectiveShield;
        }

        private void SetStartPosition()
        {
            _sprintAmmount = _sprintMaxDuration;
            _speedBoostFactor = 1f;
            _sprintFactor = 1f;
            transform.position = new Vector3(_startPosition.x, _startPosition.y, 0f);
        }

        private void SetRateLimiters()
        {
            _sprintReteLimiter.DropTime = Time.time;
            _fireRateLimiter.DropTime = Time.time;
        }

        #endregion
        private void OnDestroy()
        {
            RemoveEventAssignments();
        }

        #region Update
        private void Update()
        {
            ManageControls();
            Move();
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

            // setting movement vector
            _movementVector = _movementVector * _speed * _speedBoostFactor * _sprintFactor;

            // firing check
            _fireAllowed = _fireRateLimiter.IsReady(Time.time);
            if (Input.GetKeyDown(GlobalVariables.JUMP_KEYCODE) && _fireAllowed)
            {
                Fire();
                _fireRateLimiter.SetNewRate(Time.time, _fireRate);
            }

            // collecting pickups
            if (Input.GetKey(GlobalVariables.COLLECT_KEYCODE))
            {
                AttractPickUps();
            }
        }

        private void Move()
        {
            transform.Translate(_movementVector * Time.deltaTime);
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

        #region Firing
        private void Fire()
        {
            if (_ammoCount > 0)
            {
                if (_powerfulProjectileAllowed && _powerfullProjectile != null)
                {
                    SendProjectile(_powerfullProjectile);
                }
                else if (_homingProjectileAllowed && _homingProjectile != null)
                {
                    SendProjectile(_homingProjectile);
                }
                else if (_projectilePrefab != null)
                {
                    SendProjectile(_projectilePrefab);
                }
                _ammoCount--;
                UpdateAmmoUI();
                AudioManager.Instance.PlayProjectileShoot(transform.position);
            }
            else
            {
                AudioManager.Instance.PlayOutOfAmmo();
            }
        }

        private void SendProjectile(GameObject prefab)
        {
            foreach (var firePort in _firePorts)
            {
                if (firePort.gameObject.activeSelf)
                {
                    _spawnPosition = firePort.position;
                    _projectile = Instantiate(prefab, _spawnPosition, Quaternion.identity);
                    if (_projectileRoot != null)
                    {
                        _projectile.transform.parent = _projectileRoot;
                    }
                    if (_projectile.TryGetComponent<DamageDealer>(out _damageDealer))
                    {
                        _damageDealer.Damage = _damage;
                    }
                }
            }
        }

        private void UpdateAmmoUI()
        {
            UIManager.Instance.SetAmmo(_ammoCount);
        }
        #endregion

        #region PickUps and abilities

        public void EnablePowerfullProjectile(float timeInSeconds)
        {
            _powerfulProjectileAllowed = true;
            StopCoroutine(_powerfullProjectileCoroutine);
            _powerfullProjectileCoroutine = PowerfullProjectileRoutine(timeInSeconds);
            StartCoroutine(_powerfullProjectileCoroutine);
        }

        public void EnableTrippleProjectile(float timeInSeconds)
        {
            if (_firePorts.Length > 2)
            {
                _firePorts[1].gameObject.SetActive(true);
                _firePorts[2].gameObject.SetActive(true);
            }
            StopCoroutine(_trippleProjectileCoroutine);
            _trippleProjectileCoroutine = CooldownTrippleProjectileAbilityRoutine(timeInSeconds);
            StartCoroutine(_trippleProjectileCoroutine);
        }

        public void EnableSpeedBoost(float timeInSeconds, float speedBoostFactor)
        {
            _speedBoostFactor = speedBoostFactor;
            StopCoroutine(_speedCoroutine);
            _speedCoroutine = CooldownSpeedBoostRoutine(timeInSeconds);
            StartCoroutine(_speedCoroutine);
        }

        public void EnableHommingProjectile(float timeInSeconds)
        {
            _homingProjectileAllowed = true;
            StopCoroutine(_hommingProjectileRoutine);
            _hommingProjectileRoutine = HommingProjectileRoutine(timeInSeconds);
            StartCoroutine(_hommingProjectileRoutine);
        }

        private void AttractPickUps()
        {
            _attractedPickupsHits = Physics2D.CircleCastAll(transform.position, _attractionRadius, Vector2.zero);
            foreach (var hit in _attractedPickupsHits)
            {
                if (hit.collider.CompareTag(GlobalVariables.PICKUP_TAG))
                {
                    _directionToPlayer = transform.position - hit.transform.position;
                    _directionToPlayer.Normalize();
                    hit.transform.position += _directionToPlayer * _attractionSpeed * Time.deltaTime;
                }
            }
        }

        #endregion

        #region Events
        private void AssignToEvents()
        {
            _health.OnDeath.AddListener(OnDeath);
            _health.TookDamage += OnDamageTaken;
            _health.GotHealed += OnHealed;
            _health.HealthUpdated += OnHealthChanged;
        }

        private void RemoveEventAssignments()
        {
            _health.TookDamage -= OnDamageTaken;
            _health.GotHealed -= OnHealed;
            _health.HealthUpdated -= OnHealthChanged;
        }

        private void OnDeath()
        {
            UIManager.Instance.SetGameOver();
            GameManager.Instance.SetGameOver();
            Destroy(gameObject);
        }

        private void OnDamageTaken()
        {
            UIManager.Instance.ShakeCamera();
        }


        private void OnHealed()
        {
            // TO-DO white vignette for a fraction of second and fade out
        }

        private void OnHealthChanged(float startingHealth, float currentHealth) // updates how player's ship wrecked look
        {
            UIManager.Instance.UpdateHealth(startingHealth, currentHealth);
            if (_damageEffects != null)
            {
                float _damagedRatio = 1f - currentHealth / startingHealth;
                for (int i = 0; i < _damageEffects.Length; i++)
                {
                    float _damageEffectsIndexRatio = (i + 1) / (float)_damageEffects.Length;
                    if (_damageEffectsIndexRatio < _damagedRatio)
                    {
                        _damageEffects[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        _damageEffects[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        #endregion

        #region coroutines
        private void InitializeCoroutines()
        {
            _trippleProjectileCoroutine = CooldownTrippleProjectileAbilityRoutine(0f);
            _speedCoroutine = CooldownSpeedBoostRoutine(0f);
            _powerfullProjectileCoroutine = PowerfullProjectileRoutine(0f);
            _hommingProjectileRoutine = HommingProjectileRoutine(0f);
        }

        IEnumerator CooldownTrippleProjectileAbilityRoutine(float time)
        {
            yield return new WaitForSeconds(time);
            if (_firePorts.Length > 2)
            {
                _firePorts[1].gameObject.SetActive(false);
                _firePorts[2].gameObject.SetActive(false);
            }
        }

        IEnumerator CooldownSpeedBoostRoutine(float time)
        {
            yield return new WaitForSeconds(time);
            _speedBoostFactor = 1f;
        }

        IEnumerator PowerfullProjectileRoutine(float time)
        {
            yield return new WaitForSeconds(time);
            _powerfulProjectileAllowed = false;
        }

        IEnumerator HommingProjectileRoutine(float time)
        {
            yield return new WaitForSeconds(time);
            _homingProjectileAllowed = false;
        }

        #endregion
    }
}
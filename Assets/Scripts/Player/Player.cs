using System.Collections;
using UnityEngine;

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
    public int AmmouCount
    {
        get => _ammouCount; set { _ammouCount = value; UpdateAmmo(); }
    }
    [SerializeField] private float _fireRate = 0.2f;
    [SerializeField] private float _damage = 20f;
    [SerializeField] private Vector3 _shootingPortOffest = new Vector3(0, 0.8f, 0f);
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _specialProjectilePrefab;
    [SerializeField] private Transform _projectileRoot;
    [Header("Player health")]
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private Transform _shieldingTransform;
    [SerializeField] private Transform[] _damageEffects;

    private float _horizontalInput, _verticalInput, _maxX, _maxY, _minX, _minY;
    private float _speedBoostFactor = 1f, _sprintFactor = 1f, _damagedRatio, _damageEffectsIndexRatio, _sprintAmmount, _maxShieldValue;
    private Vector3 _movementVector = Vector3.zero;
    private Vector3 _currentPosition = Vector3.zero;
    private Vector3 _spawnPosition = Vector3.zero;
    private GameObject _projectile;
    private DamageDealer _damageDealer;
    private bool _tripleProjectileAbility;
    private IEnumerator _trippleProjectileCoroutine, _speedCoroutine;
    private SimpleRateLimiter _fireRateLimiter;
    private SimpleRateLimiter _sprintReteLimiter;
    private Material _shieldMaterial;
    private bool _sprintReady, _fireAllowed;

    private void Start()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        if (_playerHealth != null)
        {
            _playerHealth.OnShieldDepleted -= OnShieldDepleted;
            _playerHealth.OnDamageTaken -= OnDamageTaken;
            _playerHealth.ShieldDamaged -= OnShieldDamaged;
        }
    }

    private void Update()
    {
        ManageControls();
        ManagePlayerBounds();
    }

    private void Initialize()
    {
        _sprintAmmount = _sprintMaxDuration;
        _sprintReteLimiter.DropTime = Time.time;
        _fireRateLimiter.DropTime = Time.time;
        transform.position = new Vector3(
            _startPosition.x,
            _startPosition.y,
            transform.position.z);
        _trippleProjectileCoroutine = CooldownTrippleProjectileAbilityRoutine(0);
        _speedCoroutine = CooldownSpeedBoostRoutine(0);
        if (_playerHealth != null)
        {
            _playerHealth.OnShieldDepleted += OnShieldDepleted;
            _playerHealth.OnDamageTaken += OnDamageTaken;
            _playerHealth.ShieldDamaged += OnShieldDamaged;
        }
        _shieldMaterial = _shieldingTransform.GetComponent<Renderer>().material;
        UpdateAmmo();
    }

    public void EnableTrippleProjectile(float timeInSeconds)
    {
        _tripleProjectileAbility = true;
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

    public void EnableShield(float shieldValue)
    {
        if (_playerHealth != null)
        {
            _playerHealth.ShieldValue = shieldValue;
            _maxShieldValue = shieldValue;
            if (_shieldingTransform != null)
            {
                _shieldingTransform.gameObject.SetActive(true);
                _shieldMaterial.color = new Color(
                    _shieldMaterial.color.r,
                    _shieldMaterial.color.g,
                    _shieldMaterial.color.b,
                    1f);
            }
        }
    }

    public void ReplenishHealth(float replenishAmmount)
    {
        _playerHealth.CurrentHealth = Mathf.Clamp(replenishAmmount + _playerHealth.CurrentHealth, 0f, _playerHealth.StartingHealth);
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
            Fire();
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

    private void Fire()
    {
        if (_ammouCount > 0)
        {
            if (_tripleProjectileAbility && _specialProjectilePrefab != null)
            {
                SendProjectile(_specialProjectilePrefab);
            }
            else if (_projectilePrefab != null)
            {
                SendProjectile(_projectilePrefab);
            }
            _ammouCount--;
            UpdateAmmo();
        }
        else
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayOutOfAmmo();
            }
        }

    }

    private void SendProjectile(GameObject prefab)
    {
        _spawnPosition = transform.position + _shootingPortOffest;
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

    IEnumerator CooldownTrippleProjectileAbilityRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        _tripleProjectileAbility = false;
    }

    IEnumerator CooldownSpeedBoostRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        _speedBoostFactor = 1f;
    }

    private void OnShieldDepleted()
    {
        if (_shieldingTransform != null)
        {
            _shieldingTransform.gameObject.SetActive(false);
        }
    }

    private void OnDamageTaken(float startingHealth, float currentHealth)
    {
        if (_damageEffects != null)
        {
            _damagedRatio = 1f - currentHealth / startingHealth;
            for (int i = 0; i < _damageEffects.Length; i++)
            {
                _damageEffectsIndexRatio = (i + 1) / (float)_damageEffects.Length;
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

    private void OnShieldDamaged(float currentShieldValue)
    {
        if (_shieldingTransform != null)
        {
            _shieldingTransform.gameObject.SetActive(true);
            _shieldMaterial.color = new Color(
                _shieldMaterial.color.r,
                _shieldMaterial.color.g,
                _shieldMaterial.color.b,
                currentShieldValue / _maxShieldValue);
        }
    }

    private void UpdateAmmo()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetAmmo(_ammouCount);
        }
    }
}

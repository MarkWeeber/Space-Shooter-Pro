using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player positioning")]
    [SerializeField] private float _speed = 3f;
    [SerializeField] private Vector2 _startPosition = new Vector2(0f, 0f);
    [SerializeField] private Vector2 _bounds = new Vector2(10f, 4f);
    [Header("Player shooting")]
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

    private float _canFire = -1f;
    private float _horizontalInput, _verticalInput, _maxX, _maxY, _minX, _minY;
    private float _speedBoostFactor = 1f, _damagedRatio, _damageEffectsIndexRatio;
    private Vector3 _movementVector = Vector3.zero;
    private Vector3 _currentPosition = Vector3.zero;
    private Vector3 _spawnPosition = Vector3.zero;
    private GameObject _projectile;
    private DamageDealer _damageDealer;
    private bool _tripleProjectileAbility;
    private IEnumerator _trippleProjectileCoroutine, _speedCoroutine;

    private void Start()
    {
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
        }
    }

    private void OnDestroy()
    {
        if (_playerHealth != null)
        {
            _playerHealth.OnShieldDepleted -= OnShieldDepleted;
            _playerHealth.OnDamageTaken -= OnDamageTaken;
        }
    }

    private void Update()
    {
        ManageControls();
        ManagePlayerBounds();
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
            if (_shieldingTransform != null)
            {
                _shieldingTransform.gameObject.SetActive(true);
            }
        }
    }

    private void ManageControls()
    {
        _horizontalInput = Input.GetAxis(GlobalVariables.HORIZONTAL_AXIS);
        _verticalInput = Input.GetAxis(GlobalVariables.VERTICAL_AXIS);
        _movementVector = new Vector3(_horizontalInput, _verticalInput, 0f);
        transform.Translate(_movementVector * _speed * _speedBoostFactor * Time.deltaTime);
        if (Input.GetKeyDown(GlobalVariables.JUMP_KEYCODE) && Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;
            Fire();
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
        if (_tripleProjectileAbility && _specialProjectilePrefab != null)
        {
            SendProjectile(_specialProjectilePrefab);
        }
        else if (_projectilePrefab != null)
        {
            SendProjectile(_projectilePrefab);
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
}

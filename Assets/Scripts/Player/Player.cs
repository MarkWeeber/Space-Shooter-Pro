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
    [SerializeField] private Transform _projectileRoot;

    private float _canFire = -1f;
    private float _horizontalInput, _verticalInput, _maxX, _maxY, _minX, _minY;
    private Vector3 _movementVector = Vector3.zero;
    private Vector3 _currentPosition = Vector3.zero;
    private Vector3 _spawnPosition = Vector3.zero;
    private GameObject _projectile;
    private DamageDealer _damageDealer;

    private void Start()
    {
        transform.position = new Vector3(
            _startPosition.x,
            _startPosition.y,
            transform.position.z);
    }

    private void Update()
    {
        ManageControls();
        ManagePlayerBounds();
    }

    private void ManageControls()
    {
        _horizontalInput = Input.GetAxis(GlobalVariables.HORIZONTAL_AXIS);
        _verticalInput = Input.GetAxis(GlobalVariables.VERTICAL_AXIS);
        _movementVector = new Vector3(_horizontalInput, _verticalInput, 0f);
        transform.Translate(_movementVector * _speed * Time.deltaTime);
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
        if (_projectilePrefab != null)
        {
            _spawnPosition = transform.position + _shootingPortOffest;
            _projectile = Instantiate(_projectilePrefab, _spawnPosition, Quaternion.identity);
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

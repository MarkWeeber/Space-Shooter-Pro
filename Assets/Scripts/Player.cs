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
        var horizontalInput = Input.GetAxis(GlobalVariables.HORIZONTAL_AXIS);
        var verticalInput = Input.GetAxis(GlobalVariables.VERTICAL_AXIS);
        var movementVector = new Vector3(horizontalInput, verticalInput, 0f);
        transform.Translate(movementVector * _speed * Time.deltaTime);
        if (Input.GetKeyDown(GlobalVariables.JUMP_KEYCODE) && Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;
            Fire();
        }
    }

    private void ManagePlayerBounds()
    {
        var maxX = _startPosition.x + _bounds.x;
        var maxY = _startPosition.y + _bounds.y;
        var minX = _startPosition.x - _bounds.x;
        var minY = _startPosition.y - _bounds.y;
        var currentPosition = transform.position;
        transform.position = new Vector3(
            Mathf.Clamp(currentPosition.x, minX, maxX),
            Mathf.Clamp(currentPosition.y, minY, maxY),
            transform.position.z);
    }

    private void Fire()
    {
        if (_projectilePrefab != null)
        {
            var spawnPosition = transform.position + _shootingPortOffest;
            var projectile = Instantiate(_projectilePrefab, spawnPosition, Quaternion.identity);
            if (_projectileRoot != null)
            {
                projectile.transform.parent = _projectileRoot;
            }
            if (projectile.TryGetComponent<Projectile>(out var projectileComponent))
            {
                projectileComponent.SetOff(_damage);
            }
        }
    }
}

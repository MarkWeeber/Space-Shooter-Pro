using UnityEngine;

namespace SpaceShooterPro
{
    public class Enemy : MonoBehaviour
    {
        [Header("Basic")]
        [SerializeField] private int _scoreValue = 10;
        [SerializeField] private float _hitDamageValue = 15f;
        [SerializeField] private float _shootDamageValue = 10f;
        [SerializeField] private Health _enemyHealth;
        [SerializeField] private DamageDealer _bodyDamageDealer;
        [SerializeField] private Transform _destroyedBody;
        [Header("Random Shooter")]
        [SerializeField] private bool _randomShooter;
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Vector3 _shootingPortOffest = new Vector3(0, -0.5f, 0f);
        [SerializeField] private float _fireRate = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float _fireProbability = 0.25f;
        [Header("With protective Shield")]
        [SerializeField] private bool _withProtectiveShield;
        [SerializeField] private float _maxShieldValue = 50f;
        [SerializeField] private float _shieldCurrenttValue = 25f;
        [SerializeField] private ProtectiveShield _protectiveShield;
        [Header("Dodger")]
        [SerializeField] private bool _dodger;

        #region service vars
        private Vector3 _shootingPort;
        private SimpleRateLimiter fireRateLimiter;
        private GameObject _projectile;
        private float _chance;
        #endregion

        private void Awake()
        {
            AssignToEvents();
            Initialize();
        }

        private void Update()
        {
            ManageAbilities();
        }

        private void OnDestroy()
        {
            RemoveEventAssignments();
        }

        #region inits
        private void Initialize()
        {
            InitializeBasicType();
            if (_randomShooter)
            {
                InitializeRandomShooter();
            }
            if (_withProtectiveShield)
            {
                InitializeWithShield();
            }
        }

        private void InitializeBasicType()
        {
            if (_bodyDamageDealer != null)
            {
                _bodyDamageDealer.Damage = _hitDamageValue;
            }
        }

        private void InitializeRandomShooter()
        {
            fireRateLimiter.DropTime = Time.deltaTime + _fireRate;
        }

        private void InitializeWithShield()
        {
            _enemyHealth.ProtectiveShield = _protectiveShield;
            _protectiveShield.MaxShieldVale = _maxShieldValue;
            _protectiveShield.SetShieldValue(_shieldCurrenttValue);
        }

        #endregion

        #region enemy abilities and roles

        private void ManageAbilities()
        {
            if (_randomShooter)
            {
                ManageRandomShooting();
            }
        }
        private void ManageRandomShooting()
        {
            if (fireRateLimiter.IsReady(Time.time))
            {
                _chance = Random.Range(0f, 1f);
                if (_chance <= _fireProbability)
                {
                    Fire();
                }
            }
            fireRateLimiter.SetNewRate(Time.time, _fireRate);
        }

        private void Fire()
        {
            if (_projectilePrefab != null)
            {
                _shootingPort = transform.position + _shootingPortOffest;
                _projectile = Instantiate(_projectilePrefab, _shootingPort, Quaternion.identity);
                if (_projectile.TryGetComponent<DamageDealer>(out var _projectileDamageDealer))
                {
                    _projectileDamageDealer.Damage = _shootDamageValue;
                }
                AudioManager.Instance.PlayProjectileShoot(transform.position);
            }
        }
        #endregion

        #region events

        private void AssignToEvents()
        {
            _enemyHealth.OnDeath.AddListener(OnKilled);
            _enemyHealth.OnDeath.AddListener(OnDeath);
            if (_bodyDamageDealer != null)
            {
                _bodyDamageDealer.DamageDealt += OnDeath;
            }
        }

        private void RemoveEventAssignments()
        {
            if (_bodyDamageDealer != null)
            {
                _bodyDamageDealer.DamageDealt -= OnDeath;
            }
        }

        private void OnKilled()
        {
            UIManager.Instance.AddScore(_scoreValue);
        }

        private void OnDeath()
        {
            AudioManager.Instance.PlayExplosion(transform.position);
            _destroyedBody.gameObject.SetActive(true);
            _destroyedBody.parent = null;
            Destroy(gameObject);
        }

        #endregion
    }
}
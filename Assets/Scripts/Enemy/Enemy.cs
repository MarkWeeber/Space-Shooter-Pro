using System.Collections;
using UnityEngine;

namespace SpaceShooterPro
{
    public class Enemy : MonoBehaviour
    {
        [Header("Basic")]
        [SerializeField] private int _scoreValue = 10;
        [SerializeField] private float _hitDamageValue = 15f;
        [SerializeField] private Health _enemyHealth;
        [SerializeField] private DamageDealer _bodyDamageDealer;
        [SerializeField] private Transform _destroyedBody;
        [SerializeField] private Traveller _traveller;
        [Header("Random Shooter Type")]
        [SerializeField] private bool _randomShooter;
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Vector3 _shootingPortOffest = new Vector3(0f, -0.5f, 0f);
        [SerializeField] private float _fireRate = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float _fireProbability = 0.25f;
        [Header("With protective Shield Type")]
        [SerializeField] private bool _withProtectiveShield;
        [SerializeField] private float _maxShieldValue = 50f;
        [SerializeField] private float _shieldCurrenttValue = 25f;
        [SerializeField] private ProtectiveShield _protectiveShield;
        [Header("Rammer Type")]
        [SerializeField] private bool _rammer;
        [SerializeField] private float _rammingSpeed = 25f;
        [SerializeField] private TriggerCaller2D _rammingTriggerCaller;
        [Header("Back Firer Type")]
        [Tooltip("Enable Random shooter also")]
        [SerializeField] private bool _backFirer;
        [SerializeField] private Vector3 _backShootingPortOffset = new Vector3(0f, 0.5f, 0f);
        [SerializeField] private TriggerCaller2D _backViewShooterTriggerCaller;
        [SerializeField] private GameObject _backFireProjectile;
        [Header("Pick Up Destroyer Type")]
        [Tooltip("Enable Random shooter also")]
        [SerializeField] private bool _pickUpDestroyer;
        [SerializeField] private TriggerCaller2D _pickUpDestroyTriggerCaller;
        [SerializeField] private GameObject _pickUpDestroyProjectilePrefab;
        [Header("Dodger Type")]
        [SerializeField] private bool _dodger;
        [SerializeField] private TriggerCaller2D _dodgeTriggerCaller;
        [SerializeField] private float _dodgeDuration = 1f;
        [SerializeField] private float _dodgeSpeed = 10f;

        #region service vars
        private Vector3 _shootingPort;
        private SimpleRateLimiter _fireRateLimiter, _backFireRateLimiter, _pickUpDestroyFireRateLimiter;
        private float _chance;
        private IEnumerator _dodgeroutine;
        private Traveller.TravelWorldSpaceDirection _travelSpaceDirection;
        private float _initialTravelerSpeed;
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
            if (_dodger)
            {
                _dodgeroutine = DodgeRoutine(0f);
                _travelSpaceDirection = _traveller.TravelDirection;
                _initialTravelerSpeed = _traveller.Speed;
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
            _fireRateLimiter.DropTime = Time.deltaTime + _fireRate;
            _backFireRateLimiter.DropTime = Time.deltaTime + _fireRate;
            _pickUpDestroyFireRateLimiter.DropTime = Time.deltaTime + _fireRate;
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
            if (_fireRateLimiter.IsReady(Time.time))
            {
                _chance = Random.Range(0f, 1f);
                if (_chance <= _fireProbability)
                {
                    Fire(_projectilePrefab, _shootingPortOffest);
                }
                _fireRateLimiter.SetNewRate(Time.time, _fireRate);
            }

        }

        private void FireBackWards()
        {
            if (_backFireRateLimiter.IsReady(Time.time))
            {
                Fire(_backFireProjectile, _backShootingPortOffset);
                _backFireRateLimiter.SetNewRate(Time.time, _fireRate);
            }

        }

        private void FireAtPickUp()
        {
            if (_pickUpDestroyFireRateLimiter.IsReady(Time.time))
            {
                Fire(_pickUpDestroyProjectilePrefab, _shootingPortOffest);
                _pickUpDestroyFireRateLimiter.SetNewRate(Time.time, _fireRate);
            }

        }

        private void Fire(GameObject prefab, Vector3 shootingPortOffset)
        {
            if (prefab != null)
            {
                _shootingPort = transform.position + shootingPortOffset;
                Instantiate(prefab, _shootingPort, Quaternion.identity);
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
            if (_rammer && _rammingTriggerCaller != null)
            {
                _rammingTriggerCaller.OnTriggerEneterEvent += OnStartRamming;
            }
            if (_backFirer && _backViewShooterTriggerCaller != null)
            {
                _backViewShooterTriggerCaller.OnTriggerEneterEvent += OnPlayerBehindDetected;
            }
            if (_pickUpDestroyer && _pickUpDestroyTriggerCaller != null)
            {
                _pickUpDestroyTriggerCaller.OnTriggerEneterEvent += OnPickupDetected;
            }
            if (_dodger && _dodgeTriggerCaller)
            {
                _dodgeTriggerCaller.OnTriggerEneterEvent += Dodge;
            }
        }

        private void RemoveEventAssignments()
        {
            if (_bodyDamageDealer != null)
            {
                _bodyDamageDealer.DamageDealt -= OnDeath;
            }
            if (_rammer && _rammingTriggerCaller != null)
            {
                _rammingTriggerCaller.OnTriggerEneterEvent -= OnStartRamming;
            }
            if (_backFirer && _backViewShooterTriggerCaller != null)
            {
                _backViewShooterTriggerCaller.OnTriggerEneterEvent -= OnPlayerBehindDetected;
            }
            if (_pickUpDestroyer && _pickUpDestroyTriggerCaller != null)
            {
                _pickUpDestroyTriggerCaller.OnTriggerEneterEvent -= OnPickupDetected;
            }
            if (_dodger && _dodgeTriggerCaller)
            {
                _dodgeTriggerCaller.OnTriggerEneterEvent -= Dodge;
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

        private void OnStartRamming()
        {
            if (_traveller != null)
            {
                _traveller.Speed = _rammingSpeed;
            }
        }

        private void OnPlayerBehindDetected()
        {
            FireBackWards();
        }

        private void OnPickupDetected()
        {
            FireAtPickUp();
        }

        private void Dodge()
        {
            StopCoroutine(_dodgeroutine);
            _dodgeroutine = DodgeRoutine(_dodgeDuration);
            StartCoroutine(_dodgeroutine);
        }

        #endregion

        #region coroutines
        IEnumerator DodgeRoutine(float time)
        {
            if (Random.Range(0f, 1f) < 0.5f)
            {
                _traveller.TravelDirection = Traveller.TravelWorldSpaceDirection.Left;
            }
            else
            {
                _traveller.TravelDirection = Traveller.TravelWorldSpaceDirection.Right;
            }
            _traveller.Speed = _dodgeSpeed;
            yield return new WaitForSeconds(time);
            _traveller.TravelDirection = _travelSpaceDirection;
            _traveller.Speed = _initialTravelerSpeed;
        }
        #endregion
    }
}
using System.Collections;
using UnityEngine;

namespace SpaceShooterPro
{
    public class BossTurret : MonoBehaviour
    {
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Vector3 _firePortOffest = new Vector3(0f, 0.3f, 0f);
        [SerializeField] private Transform _firePort;
        [SerializeField] private float _fireRate = 0.4f;
        [Range(0f, 1f)]
        [SerializeField] private float _fireProbability = 0.25f;
        [Header("Wave Shooting")]
        [SerializeField] private bool _waveShooter = false;
        [SerializeField] private int _waveShootCount = 15;
        [SerializeField] private float _waveDuration = 2f;
        [Header("Always Target Player")]
        [SerializeField] private bool _alwaysTargetPlayer = true;

        #region service variables
        private Transform _playerTransform;
        private IEnumerator _waveShootRoutine;
        private SimpleRateLimiter _fireLimiter;
        private Vector3 _directionToPlayer;
        private Vector3 _up;
        private float _chance;
        #endregion

        #region init
        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_alwaysTargetPlayer)
            {
                _playerTransform = GameObject.Find(GlobalVariables.PLAYER_OBJECT_NAME).transform;
                if (_playerTransform == null)
                {
                    Debug.LogError("Player object not found");
                }
            }
            if (_waveShooter)
            {
                _waveShootRoutine = WaveShootRoutine(_waveDuration, _waveShootCount);
            }
            _fireLimiter.DropTime = Time.time + _fireRate;
        }
        #endregion

        private void Update()
        {
            ManageAiming();
            ManageFiring();
            _up = transform.up;
        }

        #region firing
        private void ManageFiring()
        {
            if (_fireLimiter.IsReady(Time.time))
            {
                _chance = Random.Range(0f, 1f);
                if (_chance <= _fireProbability)
                {
                    if (_waveShooter)
                    {
                        StopCoroutine(_waveShootRoutine);
                        _waveShootRoutine = WaveShootRoutine(_waveDuration, _waveShootCount);
                        StartCoroutine(_waveShootRoutine);
                    }
                    else
                    {
                        Fire();
                    }

                }
                _fireLimiter.SetNewRate(Time.time, _fireRate);
            }
        }

        private void Fire()
        {
            Instantiate(_projectilePrefab, _firePort.position, transform.rotation);
            AudioManager.Instance.PlayProjectileShoot(transform.position);
        }
        #endregion

        private void ManageAiming()
        {
            if (_alwaysTargetPlayer && _playerTransform != null)
            {
                _directionToPlayer = _playerTransform.position - transform.position;
                transform.up = _directionToPlayer; // make sure that turret facing up in 0 degree rotation!!!
            }
        }

        #region coroutine
        IEnumerator WaveShootRoutine(float time, int steps)
        {
            if (steps < 1)
            {
                yield return null;
            }
            int shootCounter = steps;
            while (shootCounter > 0)
            {
                yield return new WaitForSeconds(time / (float)steps);
                Fire();
                shootCounter--;
            }
        }
        #endregion
    }
}
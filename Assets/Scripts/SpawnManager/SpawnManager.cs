using System.Collections;
using UnityEngine;

namespace SpaceShooterPro
{
    public class SpawnManager : MonoBehaviour
    {
        [System.Serializable]
        private struct SpawningObject
        {
            public GameObject Prefab;
            public float MinSpawnRate;
            public float MaxSpawnRate;
            public int MinSpawnCount;
            public int MaxSpawnCount;
        }

        [System.Serializable]
        private struct EnemyWaveSpawn
        {
            public SpawningObject[] EnemiesSpawnSetup;
            public string SpawnAnnounceText;
            public float SpawnPreTime;
            public float NormalSpawnDuration;
            public float OutbreakSpawnDuration;
            public float OutbreakSpawnRateMultiplier;
            public float RestingDuration;
            public string OutBreakStartWarning;
            public string SpawnFinishText;
        }

        private enum WaveStage
        {
            None = 0,
            WaveStarted = 1,
            NormalWaveStarted = 2,
            OutBreakStarted = 3,
            Resting = 4
        }

        [SerializeField] private Vector3 _spawnRange = new Vector3(5f, 0f, 0f);
        [SerializeField] private SpawningObject[] _spawnObjects;
        [SerializeField] private EnemyWaveSpawn[] _enemyWavesSpawn;
        [SerializeField] private GameObject _bossEnemyPrefab;
        [SerializeField] private float _startDelay = 4f;

        private IEnumerator _stageSwitchRoutine;
        private SpawningObject _spawningObject;
        private EnemyWaveSpawn _enemyWaveSpawn;
        private Vector3 _spawnPosition = Vector3.zero;
        private float[] _objectsSpawnTimers;
        private float[][] _waveSpawnTimers;
        private int _spawnCount;
        private WaveStage _currentWaveStageIndex = WaveStage.None;
        private int _currentWaveIndex = -1;
        private bool _enabled;
        private bool _spawnWaveEnabled;
        private bool _stageSwitchRoutineRunning;

        private void Start()
        {
            Invoke(nameof(Initialize), _startDelay);
        }

        private void Update()
        {
            if (!_enabled)
            {
                return;
            }
            if (_spawnObjects.Length > 0)
            {
                CirculateSpawnObjects(_spawnObjects, Time.deltaTime, ref _objectsSpawnTimers);
            }
            if (_spawnWaveEnabled && _enemyWavesSpawn.Length > 0)
            {
                CirculateEnemyWavesSpawn(Time.deltaTime);
            }
        }

        private void Initialize()
        {
            _enabled = true;
            UIManager.Instance.OnGameOver.AddListener(OnGameOver);
            if (_spawnObjects.Length > 0)
            {
                _objectsSpawnTimers = new float[_spawnObjects.Length];
            }
            if (_enemyWavesSpawn.Length > 0)
            {
                _stageSwitchRoutine = StageSwitchRoutine(0);
                _waveSpawnTimers = new float[_enemyWavesSpawn.Length][];
                for (int i = 0; i < _enemyWavesSpawn.Length; i++)
                {
                    _waveSpawnTimers[i] = new float[_enemyWavesSpawn[i].EnemiesSpawnSetup.Length];
                }
                _spawnWaveEnabled = true;
            }
            _spawnRange = new Vector3(
                            Mathf.Abs(_spawnRange.x),
                            Mathf.Abs(_spawnRange.y),
                            Mathf.Abs(_spawnRange.z)
                        );
        }

        private void CirculateSpawnObjects(SpawningObject[] spawnObjects, float deltaTime, ref float[] timer, float spawnRateMultiplier = 1f)
        {
            for (int i = 0; i < spawnObjects.Length; i++)
            {
                _spawningObject = spawnObjects[i];
                if (timer[i] < 0f)
                {
                    timer[i] = Random.Range(_spawningObject.MinSpawnRate, _spawningObject.MaxSpawnRate);
                    _spawnCount = Random.Range(
                        (int)((float)_spawningObject.MinSpawnCount * spawnRateMultiplier),
                        (int)((float)(_spawningObject.MaxSpawnCount + 1) * spawnRateMultiplier));
                    for (int j = _spawnCount; j > 0; j--)
                    {
                        SpawnPrefab(_spawningObject.Prefab);
                    }
                }
                else
                {
                    timer[i] -= deltaTime;
                }
            }
        }

        private void CirculateEnemyWavesSpawn(float deltaTime)
        {
            if (_currentWaveIndex == -1 || _currentWaveStageIndex == WaveStage.None) // initial start
            {
                _currentWaveIndex = 0;
                ShiftStaging();

            }
            _enemyWaveSpawn = _enemyWavesSpawn[_currentWaveIndex];
            CheckCurrentStaging();
            float spawnRateMultiplier = 1f;
            if (_currentWaveStageIndex == WaveStage.NormalWaveStarted)
            {
                CirculateSpawnObjects(_enemyWaveSpawn.EnemiesSpawnSetup, deltaTime, ref _waveSpawnTimers[_currentWaveIndex], spawnRateMultiplier);
            }
            if (_currentWaveStageIndex == WaveStage.OutBreakStarted)
            {
                spawnRateMultiplier = _enemyWaveSpawn.OutbreakSpawnRateMultiplier;
                CirculateSpawnObjects(_enemyWaveSpawn.EnemiesSpawnSetup, deltaTime, ref _waveSpawnTimers[_currentWaveIndex], spawnRateMultiplier);
            }

        }

        private void SpawnPrefab(GameObject prefab, bool usePrefabsRotation = false)
        {
            if (prefab != null)
            {
                _spawnPosition = new Vector3(
                    Random.Range(-_spawnRange.x, _spawnRange.x),
                    Random.Range(-_spawnRange.y, _spawnRange.y),
                    Random.Range(-_spawnRange.z, _spawnRange.z)
                );
                _spawnPosition += transform.position;
                var rotation = Quaternion.identity;
                if (usePrefabsRotation)
                {
                    rotation = prefab.transform.rotation;
                }
                Instantiate(prefab, _spawnPosition, rotation, this.transform);
            }
        }

        private void OnGameOver()
        {
            _enabled = false;
        }

        IEnumerator StageSwitchRoutine(float time)
        {
            _stageSwitchRoutineRunning = true;
            yield return new WaitForSeconds(time);
            ShiftStaging();
            _stageSwitchRoutineRunning = false;
        }

        private void CheckCurrentStaging()
        {
            if (!_stageSwitchRoutineRunning)
            {
                float waitTime = 0;
                switch (_currentWaveStageIndex)
                {
                    case WaveStage.WaveStarted:
                        waitTime = _enemyWaveSpawn.SpawnPreTime;
                        break;
                    case WaveStage.NormalWaveStarted:
                        waitTime = _enemyWaveSpawn.NormalSpawnDuration;
                        break;
                    case WaveStage.OutBreakStarted:
                        waitTime = _enemyWaveSpawn.OutbreakSpawnDuration;
                        break;
                    case WaveStage.Resting:
                        waitTime = _enemyWaveSpawn.RestingDuration;
                        break;
                    default:
                        break;
                }
                _stageSwitchRoutine = StageSwitchRoutine(waitTime);
                StartCoroutine(_stageSwitchRoutine);
            }
        }

        private void ShiftStaging()
        {
            switch (_currentWaveStageIndex)
            {
                case WaveStage.None:
                    _currentWaveStageIndex = WaveStage.WaveStarted;
                    UIManager.Instance.MakeAnnouncement(_enemyWavesSpawn[_currentWaveIndex].SpawnAnnounceText);
                    break;
                case WaveStage.WaveStarted:
                    _currentWaveStageIndex = WaveStage.NormalWaveStarted;

                    break;
                case WaveStage.NormalWaveStarted:
                    _currentWaveStageIndex = WaveStage.OutBreakStarted;
                    UIManager.Instance.MakeAnnouncement(_enemyWavesSpawn[_currentWaveIndex].OutBreakStartWarning);
                    break;
                case WaveStage.OutBreakStarted:
                    _currentWaveStageIndex = WaveStage.Resting;
                    UIManager.Instance.MakeAnnouncement(_enemyWavesSpawn[_currentWaveIndex].SpawnFinishText);
                    break;
                case WaveStage.Resting:
                    if (_currentWaveIndex < _enemyWavesSpawn.Length - 1) // there's still some wave left
                    {
                        _currentWaveStageIndex = WaveStage.WaveStarted;
                        _currentWaveIndex++;
                        UIManager.Instance.MakeAnnouncement(_enemyWavesSpawn[_currentWaveIndex].SpawnAnnounceText);
                    }
                    else // it was the last wave
                    {
                        _spawnWaveEnabled = false;
                        WaveSpawnEndReached();
                    }
                    break;
                default:
                    break;
            }
        }

        private void WaveSpawnEndReached()
        {
            SpawnPrefab(_bossEnemyPrefab, usePrefabsRotation: true);
        }

    }
}
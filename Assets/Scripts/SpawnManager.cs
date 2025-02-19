using UnityEngine;

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

    [SerializeField] private Vector3 _spawnRange = new Vector3(5f, 0f, 0f);
    [SerializeField] private SpawningObject[] _spawnObjects;
    [SerializeField] private float _startDelay = 4f;

    private SpawningObject _spawningObject;
    private Vector3 _spawnPosition = Vector3.zero;
    private float[] _spawnTimers;
    private int _spawnCount;
    private bool _enabled;

    private void Start()
    {
        Invoke(nameof(Initialize), _startDelay);
    }

    private void Update()
    {
        if (_spawnObjects.Length > 0 && _enabled)
        {
            CirculateSpawnObjects(Time.deltaTime);
        }
    }

    private void Initialize()
    {
        _enabled = true;
        UIManager.Instance.OnGameOver.AddListener(OnGameOver);
        if (_spawnObjects.Length > 0)
        {
            _spawnTimers = new float[_spawnObjects.Length];
        }
        _spawnRange = new Vector3(
                        Mathf.Abs(_spawnRange.x),
                        Mathf.Abs(_spawnRange.y),
                        Mathf.Abs(_spawnRange.z)
                    );
    }

    private void CirculateSpawnObjects(float deltaTime)
    {
        for (int i = 0; i < _spawnObjects.Length; i++)
        {
            _spawningObject = _spawnObjects[i];
            if (_spawnTimers[i] < 0f)
            {
                _spawnTimers[i] = Random.Range(_spawningObject.MinSpawnRate, _spawningObject.MaxSpawnRate);
                _spawnCount = Random.Range(_spawningObject.MinSpawnCount, _spawningObject.MaxSpawnCount + 1);
                for (int j = _spawnCount; j > 0; j--)
                {
                    SpawnPrefab(_spawningObject.Prefab);
                }
            }
            else
            {
                _spawnTimers[i] -= deltaTime;
            }
        }
    }

    private void SpawnPrefab(GameObject prefab)
    {
        if (prefab != null)
        {
            _spawnPosition = new Vector3(
                Random.Range(-_spawnRange.x, _spawnRange.x),
                Random.Range(-_spawnRange.y, _spawnRange.y),
                Random.Range(-_spawnRange.z, _spawnRange.z)
            );
            _spawnPosition += transform.position;
            Instantiate(prefab, _spawnPosition, Quaternion.identity, this.transform);
        }
    }

    private void OnGameOver()
    {
        _enabled = false;
    }
}

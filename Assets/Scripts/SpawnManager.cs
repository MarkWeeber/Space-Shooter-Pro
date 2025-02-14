using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Vector3 _spawnRange = new Vector3(5f, 0f, 0f);
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _spawnRate = 0.5f;

    private Vector3 _spawnPosition = Vector3.zero;

    private void Start()
    {
        _spawnRange = new Vector3(
                        Mathf.Abs(_spawnRange.x),
                        Mathf.Abs(_spawnRange.y),
                        Mathf.Abs(_spawnRange.z)
                    );
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnPrefab();
            yield return new WaitForSeconds(_spawnRate);
        }
    }

    private void SpawnPrefab()
    {
        _spawnPosition = new Vector3(
            Random.Range(-_spawnRange.x, _spawnRange.x),
            Random.Range(-_spawnRange.y, _spawnRange.y),
            Random.Range(-_spawnRange.z, _spawnRange.z)
        );
        _spawnPosition += transform.position;
        Instantiate(_prefab, _spawnPosition, Quaternion.identity);
    }
}

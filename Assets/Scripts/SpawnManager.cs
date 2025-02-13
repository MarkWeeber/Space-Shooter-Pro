using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Vector3 _spawnRange = new Vector3(5f, 0f, 0f);
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _spawnRate = 0.5f;

    private void Start()
    {
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
        var spawnRange = new Vector3(
                        Mathf.Abs(_spawnRange.x),
                        Mathf.Abs(_spawnRange.y),
                        Mathf.Abs(_spawnRange.z)
                    );
        var spawnPosition = new Vector3(
            Random.Range(-spawnRange.x, spawnRange.x),
            Random.Range(-spawnRange.y, spawnRange.y),
            Random.Range(-spawnRange.z, spawnRange.z)
        );
        spawnPosition += transform.position;
        Instantiate(_prefab, spawnPosition, Quaternion.identity);
    }
}

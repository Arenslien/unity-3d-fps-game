using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private Zombie _zombiePrefab;
    [SerializeField] private int _spawnCount = 1;
    private float _spawnHeight = 1f;

    private void Start()
    {
        Spawn(_spawnCount);
    }

    private void Spawn(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = transform.position;
            spawnPosition.y = _spawnHeight;

            Zombie zombie = Instantiate(_zombiePrefab, spawnPosition, Quaternion.identity);
            zombie.Initialize(spawnPosition);
        }
    }
}
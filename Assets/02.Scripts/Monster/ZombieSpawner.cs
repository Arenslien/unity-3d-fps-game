using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private Zombie _zombiePrefab;

    private void Start()
    {
        Spawn();
    }

    private void Spawn()
    {
        Zombie zombie = Instantiate(_zombiePrefab);
        Vector3 spawnPosition = transform.position + Vector3.up;
        zombie.Initialize(spawnPosition);
    }
}
using System.Collections.Generic;
using UnityEngine;

public class PlayerBombPool : MonoBehaviour
{
    // 프리팹
    [SerializeField] private Bomb _bombPrefab;

    // 풀(Queue 기반) & 사이즈
    private readonly Queue<Bomb> _pool = new Queue<Bomb>();
    private const int MaxSize = 10;

    // 싱글톤 패턴
    private static PlayerBombPool _instance = null;
    public static PlayerBombPool Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < MaxSize; i++)
        {
            Bomb bomb = Instantiate(_bombPrefab, transform.position, Quaternion.identity, transform);
            bomb.Deactivate();
            _pool.Enqueue(bomb);
        }
    }

    public Bomb GetBomb(Transform firePoint)
    {
        if (_pool.Count <= 0) return null;

        Bomb bomb = _pool.Dequeue();
        bomb.Activate(firePoint);

        return bomb;
    }

    public void ReturnBomb(Bomb bomb)
    {
        bomb.Deactivate();
        _pool.Enqueue(bomb);
    }
}
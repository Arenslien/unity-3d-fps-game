using System;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    // 싱글톤 패턴
    private static ZombieManager _instance = null;
    public static ZombieManager Instance => _instance;

    // 전체 Zombie 관리 필드
    private int _maxZombieCount;
    private int _zombieCount;

    public int MaxZombieCount => _maxZombieCount;
    public int ZombieCount => _zombieCount;

    // 이벤트
    public event Action OnZombieCountChanged;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void Start()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
        _maxZombieCount = zombies.Length;
        _zombieCount = _maxZombieCount;
    }

    private void IncreaseZombie()
    {
        _zombieCount++;
    }

    public void DecreaseZombie()
    {
        if (_zombieCount <= 0) return;

        _zombieCount--;
    }
}
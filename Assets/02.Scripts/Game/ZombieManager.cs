using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [SerializeField] private UI_ZombieCountText _uiZombieCountText;

    // 싱글톤 패턴
    private static ZombieManager _instance = null;
    public static ZombieManager Instance => _instance;

    // 전체 Zombie 관리 필드
    private int _maxZombieCount;
    private int _zombieCount;

    public int MaxZombieCount => _maxZombieCount;
    public int ZombieCount => _zombieCount;

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
        _uiZombieCountText.RefreshUI();
    }

    private void IncreaseZombie()
    {
        _zombieCount++;
    }

    public void DecreaseZombie()
    {
        if (_zombieCount <= 0) return;

        _zombieCount--;
        _uiZombieCountText.RefreshUI();
    }
}
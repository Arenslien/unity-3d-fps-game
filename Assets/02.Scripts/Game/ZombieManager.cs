using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [SerializeField] private UI_ZombieCountText _uiZombieCountText;

    private static ZombieManager _instance = null;
    public static ZombieManager Instance => _instance;

    private int _maxZombieCount;
    private int _zombieCount = 0;

    public int MaxZombieCount => _maxZombieCount;
    public int ZombieCount => _zombieCount;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
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

    public void IncreaseZombieCount()
    {
        _zombieCount++;
    }

    public void DecreaseZombieCount()
    {
        if (_zombieCount <= 0) return;

        _zombieCount--;
        _uiZombieCountText.RefreshUI();
    }
}
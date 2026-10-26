using TMPro;
using UnityEngine;

public class UI_ZombieCountText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _uiZombieCountText;

    private void Start()
    {
        ZombieManager.Instance.OnZombieCountChanged += RefreshUI;
        RefreshUI();
    }

    public void RefreshUI()
    {
        _uiZombieCountText.text =
            $"Zombie : {ZombieManager.Instance.ZombieCount}/{ZombieManager.Instance.MaxZombieCount}";
    }
}
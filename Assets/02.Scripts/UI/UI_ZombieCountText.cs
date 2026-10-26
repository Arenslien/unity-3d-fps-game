using TMPro;
using UnityEngine;

public class UI_ZombieCountText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _uiZombieCountText;

    public void RefreshUI()
    {
        _uiZombieCountText.text =
            $"Zombie : {ZombieManager.Instance.ZombieCount}/{ZombieManager.Instance.MaxZombieCount}";
    }
}
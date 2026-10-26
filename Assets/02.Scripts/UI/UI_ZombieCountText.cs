using TMPro;
using UnityEngine;

public class UI_ZombieCountText : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text;

    private void Start()
    {
        if (_text == null)
        {
            Debug.Log($"[UI_ZombieCountText.cs] - Text is null");
        }
    }

    public void RefreshUI()
    {
        _text.text = $"Zombie : {ZombieManager.Instance.ZombieCount}/{ZombieManager.Instance.MaxZombieCount}";
    }
}
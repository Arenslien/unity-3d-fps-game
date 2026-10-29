using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerWeapons : MonoBehaviour
{
    [SerializeField] private GameObject _rifleImage;
    [SerializeField] private GameObject _sniperImage;
    [SerializeField] private TextMeshProUGUI _bulletText;
    [SerializeField] private TextMeshProUGUI _bombText;

    private void Start()
    {
        Player.Instance.OnWeaponChange += ChangeWeaponImage;
    }

    private void Update()
    {
    }

    private void ChangeWeaponImage()
    {
        bool isRifle = Player.Instance.WeaponMode == WeaponMode.Normal;
        _rifleImage.SetActive(isRifle);
        _sniperImage.SetActive(!isRifle);
    }
}
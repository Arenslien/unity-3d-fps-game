using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerWeapons : MonoBehaviour
{
    [SerializeField] private GameObject _rifleImage;
    [SerializeField] private GameObject _sniperImage;
    [SerializeField] private TextMeshProUGUI _bulletText;
    [SerializeField] private TextMeshProUGUI _bombText;

    private PlayerGunFire _playerGunFire;

    private void Start()
    {
        Player.Instance.OnWeaponChange += RefreshWeaponImage;
        _playerGunFire = Player.Instance.GetComponent<PlayerGunFire>();
        _playerGunFire.OnFire += RefreshBulletCount;
    }

    private void Update()
    {
    }

    private void RefreshWeaponImage()
    {
        bool isRifle = Player.Instance.WeaponMode == WeaponMode.Normal;
        _rifleImage.SetActive(isRifle);
        _sniperImage.SetActive(!isRifle);
    }

    private void RefreshBulletCount()
    {
        _bulletText.text = $"{_playerGunFire.BulletCount}/{_playerGunFire.MaxBulletCount}";
    }
}
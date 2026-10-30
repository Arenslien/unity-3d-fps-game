using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerWeapons : MonoBehaviour
{
    [SerializeField] private GameObject[] _weaponImages;
    [SerializeField] private TextMeshProUGUI _bulletText;
    [SerializeField] private TextMeshProUGUI _bombText;

    private PlayerGunFire _playerGunFire;
    private PlayerBombFire _playerBombFire;

    private void Start()
    {
        Player.Instance.OnWeaponChange += RefreshWeaponImage;
        _playerGunFire = Player.Instance.GetComponent<PlayerGunFire>();
        _playerBombFire = Player.Instance.GetComponent<PlayerBombFire>();

        _playerGunFire.OnChangedBullet += RefreshBulletCount;
        _playerBombFire.OnBombFire += RefreshBombCount;

        RefreshBulletCount();
        RefreshBombCount();
    }

    private void RefreshWeaponImage()
    {
        int current = (int)Player.Instance.WeaponMode;

        for (int i = 0; i < _weaponImages.Length; i++)
        {
            _weaponImages[i].SetActive(i == current);
        }
    }

    private void RefreshBulletCount()
    {
        _bulletText.text = $"{_playerGunFire.BulletCount}/{_playerGunFire.MaxBulletCount}";
    }

    private void RefreshBombCount()
    {
        _bombText.text = $"폭탄 {_playerBombFire.BombCount}";
    }
}
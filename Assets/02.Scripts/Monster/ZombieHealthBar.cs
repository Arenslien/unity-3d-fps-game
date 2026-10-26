using UnityEngine;
using UnityEngine.UI;

public class ZombieHealthBar : MonoBehaviour
{
    private Slider _slider;

    private Zombie _zombie;

    private void Awake()
    {
        _slider = GetComponentInChildren<Slider>(true);
        _zombie = GetComponent<Zombie>();
    }

    private void Start()
    {
        if (_slider == null)
        {
            Debug.Log("[ZombieHealthBar.cs] - Slider is null");
        }

        _zombie.OnDamaged += RefreshHealthBar;
        RefreshHealthBar();
    }

    private void LateUpdate()
    {
        // 빌보드: 체력바가 카메라를 쳐다보게 한다.
        _slider.transform.forward = Camera.main.transform.forward;
    }

    private void RefreshHealthBar()
    {
        // 0 ~ 100% --> 0 ~ 1
        _slider.value = _zombie.Health / _zombie.MaxHealth;
    }
}
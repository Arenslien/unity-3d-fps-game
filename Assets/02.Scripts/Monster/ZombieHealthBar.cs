using System;
using UnityEngine;
using UnityEngine.UI;

public class ZombieHealthBar : MonoBehaviour
{
    private Camera _mainCamera;
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

        _mainCamera = Camera.main;

        _zombie.OnDamaged += RefreshHealthBar;
        RefreshHealthBar();
    }

    private void LateUpdate()
    {
        // 빌보드: 체력바가 카메라를 쳐다보게 한다.
        _slider.transform.forward = _mainCamera.transform.forward;
    }

    private void RefreshHealthBar()
    {
        // 0 ~ 100% --> 0 ~ 1
        _slider.value = _zombie.Health / _zombie.MaxHealth;
    }

    private void OnDestroy()
    {
        _zombie.OnDamaged -= RefreshHealthBar;
    }
}
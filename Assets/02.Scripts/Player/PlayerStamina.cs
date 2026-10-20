using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private Slider _staminaSlider;

    private float _maxGauge = 10f;
    private float _currentGauge = 10f;

    public float Gauge => _currentGauge;

    private void Start()
    {
        if (_staminaSlider == null)
        {
            Debug.LogWarning("[PlayerStamina.cs] - Slider is null");
        }

        _staminaSlider.value = _maxGauge;
    }

    private void Update()
    {
        _staminaSlider.value = _currentGauge;
    }

    public void UseGauge(float value)
    {
        if (value < 0) return;

        _currentGauge -= value;
    }

    public void RestoreGauge(float value)
    {
        if (value < 0) return;

        _currentGauge = Mathf.Min(_currentGauge + value, 10f);
    }
}
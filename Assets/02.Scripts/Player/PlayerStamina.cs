using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    // 컴포넌트
    [SerializeField] private Slider _slider;

    private const float MaxGauge = 100f;
    private float _gauge = MaxGauge;

    public float Gauge => _gauge;

    private void Start()
    {
        _slider.maxValue = MaxGauge;
        _slider.value = MaxGauge;
    }

    public void UseGauge(float value)
    {
        if (value < 0) return;

        _gauge -= value;
        _slider.value = _gauge;
    }

    public void RestoreGauge(float value)
    {
        if (value < 0) return;

        _gauge += value;
        _slider.value = _gauge;
    }

    public void SetGauge(float value)
    {
        if (value < 0) return;

        _gauge = value;
        _slider.value = _gauge;
    }
}
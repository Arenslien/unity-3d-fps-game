using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour, IDamageable
{
    [Header("슬라이더")]
    [SerializeField] private Slider _slider;

    // 체력 설정
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _health = 100f;

    public float MaxHealth => _maxHealth;
    public float Health => _health;

    private void LateUpdate()
    {
        Debug.Log($"현재 비율: {_health / _maxHealth}");
        _slider.value = _health / _maxHealth;
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.Damage < 0) return;

        _health -= damageInfo.Damage;
        Debug.Log($"현재 체력: {_health}");
    }
}
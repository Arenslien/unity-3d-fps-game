using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [Header("스탯")]
    [SerializeField] private float _maxHealth = 100f;
    public const float MaxStamina = 100f;
    private float _stamina = MaxStamina;
    private float _health;

    public float MaxHealth => _maxHealth;
    public float Health => _health;
    public float Stamina => _stamina;

    [SerializeField] private UI_HitEffect _uiHitEffect;

    private void Awake()
    {
        _health = _maxHealth;
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.Damage <= 0 || _health <= 0) return;

        _health -= damageInfo.Damage;

        _uiHitEffect.Fade(0.7f);

        Debug.Log($"[Player.cs] - 플레이어 피격 당함. HP: {_health}/{_maxHealth}");
    }

    public void SetStamina(float value)
    {
        if (value < 0) return;

        _stamina = value;
    }

    public void UseStamina(float value)
    {
        if (value < 0) return;

        _stamina -= value;
    }
}
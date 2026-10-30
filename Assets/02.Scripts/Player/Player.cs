using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMeleeAttack))]
public class Player : MonoBehaviour, IDamageable
{
    // 싱글톤 패턴
    private static Player _instance = null;
    public static Player Instance => _instance;

    [Header("스탯")]
    [SerializeField] private float _maxHealth = 100f;
    public const float MaxStamina = 100f;
    private float _stamina = MaxStamina;
    private float _health;

    private Animator _animator;
    private int _injuredLayerIndex;

    public float MaxHealth => _maxHealth;
    public float Health => _health;
    public float Stamina => _stamina;

    private WeaponMode _weaponMode = WeaponMode.Normal;
    public WeaponMode WeaponMode => _weaponMode;

    // 이벤트 관리: 
    public event Action OnDamaged = null;
    public event Action OnSprint = null;
    public event Action OnWeaponChange = null;

    private void Awake()
    {
        _health = _maxHealth;
        _animator = GetComponentInChildren<Animator>();
        _injuredLayerIndex = _animator.GetLayerIndex("Injured Layer");

        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _weaponMode = WeaponMode.Normal;
            OnWeaponChange?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _weaponMode = WeaponMode.Sniper;
            OnWeaponChange?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("근접 모드 변경");
            _weaponMode = WeaponMode.Melee;
            OnWeaponChange?.Invoke();
        }
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.Damage <= 0 || _health <= 0) return;

        _health -= damageInfo.Damage;

        // 체력에 따라 Injured Layer의 가중치를 수정
        float healthPercent = _health / _maxHealth;
        float injured = Mathf.InverseLerp(0.5f, 0f, healthPercent);
        _animator.SetLayerWeight(_injuredLayerIndex, injured);

        OnDamaged?.Invoke();

        Debug.Log($"[Player.cs] - 플레이어 피격 당함. HP: {_health}/{_maxHealth}");

        if (_health <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void SetStamina(float value)
    {
        if (value < 0) return;

        _stamina = value;

        OnSprint?.Invoke();
    }

    public void UseStamina(float value)
    {
        if (value < 0) return;

        _stamina -= value;

        OnSprint?.Invoke();
    }
}
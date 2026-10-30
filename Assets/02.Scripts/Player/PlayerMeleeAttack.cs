using System;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    [SerializeField] private Collider _swordCollider;
    private Player _player;
    private Animator _animator;
    private int _meleeLayerIndex;
    private Sword _sword;

    private void Awake()
    {
        _sword = _swordCollider.GetComponent<Sword>();
        _animator = GetComponentInChildren<Animator>();
        _player = GetComponentInParent<Player>();
        _meleeLayerIndex = _animator.GetLayerIndex("Melee Layer");
    }

    private void Update()
    {
        bool isPlaying = GameManager.Instance.State == GameState.Playing;
        bool isMelee = _player.WeaponMode == WeaponMode.Melee;
        if (!isPlaying || !isMelee) return;

        // Todo: 근접 공격 애니메이션 중일 땐 중복 안되게 수정
        if (Input.GetButtonDown("Fire2"))
        {
            _animator.SetTrigger("Slash");
        }
    }

    public void ActivateCollider()
    {
        Debug.Log("액티브");
        _sword.ResetHits();
        _swordCollider.enabled = true;
    }

    public void DeactivateCollider()
    {
        Debug.Log("디액티브");
        _swordCollider.enabled = false;
    }

    // public void ActivateCollider() => _swordCollider.enabled = true;
    // public void DeactivateCollider() => _swordCollider.enabled = false;
}
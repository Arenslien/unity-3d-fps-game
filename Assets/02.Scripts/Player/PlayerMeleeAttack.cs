using System;
using UnityEngine;

public enum MeleeHitMode
{
    Collider,
    Fan
}

public class PlayerMeleeAttack : MonoBehaviour
{
    private Player _player;
    private Animator _animator;
    private int _meleeLayerIndex;
    [SerializeField] private Collider _swordCollider;
    [SerializeField] private MeleeHitMode _meleeHitMode = MeleeHitMode.Collider;

    [Header("부채꼴")]
    [SerializeField] private float _range = 2f;
    [SerializeField] private float _angle = 90f;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstacleMask;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _player = GetComponentInParent<Player>();
        _meleeLayerIndex = _animator.GetLayerIndex("Melee Layer");

        _swordCollider.enabled = false;
    }

    public void FanAttack()
    {
        if (_meleeHitMode != MeleeHitMode.Fan) return;

        Collider[] colliders = Physics.OverlapSphere(_player.transform.position, _range, _targetMask);

        Vector3 origin = _player.transform.position;

        foreach (Collider col in colliders)
        {
            Vector3 hitPoint = col.ClosestPoint(origin);    // 적의 타격점
            Vector3 direction = (hitPoint - origin).normalized;          // 방향 판단
            direction.y = 0f;
            float angle = Vector3.Angle(_player.transform.forward, direction);
            if (angle > _angle / 2f) continue;

            // 적과 나 사이에 
            if (Physics.Linecast(origin, hitPoint, _obstacleMask)) continue;


            if (col.TryGetComponent(out IDamageable damagealbe))
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.Damage = 25f;
                damageInfo.HitPoint = hitPoint;
                damageInfo.HitNormal = (origin - hitPoint).normalized;
                damagealbe.TakeDamage(damageInfo);
            }
        }
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
        if (_meleeHitMode != MeleeHitMode.Collider) return;

        _swordCollider.enabled = true;
    }

    public void DeactivateCollider()
    {
        _swordCollider.enabled = false;
    }

    // public void ActivateCollider() => _swordCollider.enabled = true;
    // public void DeactivateCollider() => _swordCollider.enabled = false;
}
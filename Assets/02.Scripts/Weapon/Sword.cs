using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private float _damage = 25f;

    private Player _player;
    // 이번 휘두름에서 이미 맞은 대상 (트리거 재진입으로 인한 중복 대미지 방지)
    private readonly HashSet<IDamageable> _hitTargets = new HashSet<IDamageable>();

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 자기 자신(플레이어)은 베지 않는다
        if (other.transform.root.CompareTag("Player")) return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null && _hitTargets.Add(damageable))
        {
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.Damage = _damage;
            Vector3 hitPoint = other.ClosestPoint(_player.transform.position);
            damageInfo.HitPoint = hitPoint;
            damageInfo.HitNormal = (_player.transform.position - hitPoint).normalized;

            damageable.TakeDamage(damageInfo);
        }
    }
}
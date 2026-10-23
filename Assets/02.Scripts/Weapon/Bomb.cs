using UnityEngine;

// 목표: 무언가에 부딪히면 폭발 이펙트를 남기고 사라지는 폭탄

[RequireComponent(typeof(Rigidbody))]
public class Bomb : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionEffectPrefab;
    [SerializeField] private float _damage = 100f;
    [SerializeField] private float _explosionRange = 4.5f;

    private void OnCollisionEnter(Collision collision)
    {
        ParticleSystem effect = Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
        effect.Play();

        ApplyAreaDamage();
        PlayerBombPool.Instance.ReturnBomb(this);
    }

    public void Activate(Transform firePoint)
    {
        gameObject.SetActive(true);
        transform.position = firePoint.position;
        transform.rotation = Quaternion.identity;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void ApplyAreaDamage()
    {
        // 0. 주위에 있는 드럼통들을 모두 찾는다.
        // - Physics.OverlapSpere(원점, 반지름, 찾을 레이어들)
        Collider[] colliders =
            Physics.OverlapSphere(transform.position, _explosionRange, LayerMask.GetMask("Drum", "Zombie"));

        // Unity의 Collider.ClosestPoint
        // - 지정한 위치(주어진 위치)에서 해당 콜라이더(Collider)의 표면 또는 내부에서
        // - 가장 가까운 지점의 좌표를 반환하는 함수

        // 1. 드럼통들에게 대미지를 준다.
        foreach (Collider collider in colliders)
        {
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.Damage = _damage;
            damageInfo.hitPoint = collider.ClosestPoint(transform.position);
            damageInfo.hitNormal = (transform.position - collider.ClosestPoint(transform.position)).normalized;
            collider.GetComponent<IDamageable>()?.TakeDamage(damageInfo);
        }
    }
}
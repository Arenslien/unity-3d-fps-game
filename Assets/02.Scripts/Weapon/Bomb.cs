using UnityEngine;

// 목표: 무언가에 부딪히면 폭발 이펙트를 남기고 사라지는 폭탄

[RequireComponent(typeof(Rigidbody))]
public class Bomb : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionEffectPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        ParticleSystem effect = Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
        effect.Play();

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
}
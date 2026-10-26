using System.Collections;
using UnityEngine;

// 목표: 마우스 왼쪽 버튼 클릭 시 - 바라보는 방향으로 총알 발사
public class PlayerGunFire : MonoBehaviour
{
    [SerializeField] private ParticleSystem _hitEffectPrefab;

    [Header("스탯")]
    [SerializeField] private float _range = 100f;

    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _fireInterval = 0.1f; // 발사 속도
    private float _fireTimer = 0f;

    // 탄창 기능
    [SerializeField] private int _maxBulletCount = 30;
    private int _bulletCount = 0;
    private const float ReloadingTime = 3f;

    // 재장전 코루틴
    private Coroutine _reloadCoroutine = null;

    private void Awake()
    {
        _bulletCount = _maxBulletCount;
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;

        // 0. 발사 타이머를 지난 시간만큼 줄인다.
        _fireTimer -= Time.deltaTime;

        // 1. 마우스 왼쪽 버튼 입력 받기
        bool isMouseVisible = Cursor.visible;
        if (!isMouseVisible && Input.GetButton("Fire1") && _fireTimer <= 0f && _bulletCount > 0) // 0(왼), 1(오), 2(휠) 
        {
            _fireTimer = _fireInterval;
            Shoot();

            // StopAllCoroutines();
            if (_reloadCoroutine != null)
            {
                StopCoroutine(_reloadCoroutine);
                _reloadCoroutine = null;
                Debug.Log("재장전 취소!");
            }
        }

        // 2. 장전 기능
        if (Input.GetButtonDown("Reload") && _reloadCoroutine == null)
        {
            _reloadCoroutine = StartCoroutine(Reload_Coroutine());
        }
    }

    private void Shoot()
    {
        Transform cameraTransform = Camera.main.transform;
        // 2. 레이(광선)를 생성하고 발사될 위치와 진행 방향 설정
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        // 3. 레이가 부딪힌 대상의 정보를 저장할 변수 생성
        RaycastHit hitInfo = new RaycastHit();

        // 4. 레이를 발사하고, 만일 부딪힌 물체가 있다면 이펙트 표시
        bool isHit = Physics.Raycast(ray, out hitInfo, _range);

        if (isHit)
        {
            ParticleSystem effect = Instantiate(_hitEffectPrefab);
            effect.transform.position = hitInfo.point;
            effect.transform.forward = hitInfo.normal;
            effect.Play();

            DamageInfo damageInfo = new DamageInfo();
            damageInfo.Damage = _damage;
            damageInfo.hitPoint = hitInfo.point;
            damageInfo.hitNormal = hitInfo.normal;

            hitInfo.collider.GetComponent<IDamageable>()?.TakeDamage(damageInfo);

            // hitInfo.collider.GetComponent<Zombie>()?.TakeDamage(_damage);
            // if (hitInfo.collider.TryGetComponent(out Zombie zombie))
            // {
            //     zombie.TakeDamage(_damage);
            // }
            //
            // hitInfo.collider.GetComponent<Drum>()?.TakeDamage(_damage);
            // hitInfo.collider.GetComponent<Box>()?.TakeDamage(_damage);
        }

        // 5. 총알 소모
        _bulletCount--;
    }

    private IEnumerator Reload_Coroutine()
    {
        Debug.Log("재장전 시작");

        yield return new WaitForSeconds(ReloadingTime);

        _bulletCount = _maxBulletCount;
        _reloadCoroutine = null;

        Debug.Log("재장전 종료");
    }
}
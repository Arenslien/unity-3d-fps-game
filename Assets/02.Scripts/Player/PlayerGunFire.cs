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

    // 애니메이터 변수 필드
    private Animator _animator;

    private void Awake()
    {
        _bulletCount = _maxBulletCount;
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // 게임 중이 아니면 쏘는 동작 정지, 행동 그만
        if (GameManager.Instance.State != GameState.Playing)
        {
            _animator.SetBool("Fire", false);
            return;
        }

        // 1. 발사 타이머를 지난 시간만큼 줄인다.
        _fireTimer -= Time.deltaTime;

        // 2. 커서가 숨어 있고, 버튼을 누르고 있고, 총알이 남아 있는 동안은 쏘는 동작
        bool isMouseVisible = Cursor.visible;
        bool isFiring = !isMouseVisible && Input.GetButton("Fire1") && _bulletCount > 0;
        _animator.SetBool("Fire", isFiring);

        // 3. 쏘는 중 & 쏠 때가 되었으면 쏜다.
        if (isFiring && _fireTimer <= 0f) // 0(왼), 1(오), 2(휠) 
        {
            // 재장전 취소
            if (_reloadCoroutine != null)
            {
                StopCoroutine(_reloadCoroutine);
                _reloadCoroutine = null;
                Debug.Log("재장전 취소!");
            }

            _fireTimer = _fireInterval;
            Shoot();
            // StopAllCoroutines();
        }

        // 4. 장전 기능
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
            damageInfo.HitPoint = hitInfo.point;
            damageInfo.HitNormal = hitInfo.normal;

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
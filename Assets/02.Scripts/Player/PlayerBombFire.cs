using Unity.VisualScripting;
using UnityEngine;

// 목표: 마우스 오른쪽 클릭 시 : 시선이 바라보는 방향으로 수류탄 투척
public class PlayerBombFire : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _throwPower = 15f;

    // 폭탄 개수
    private const int MaxBombCount = 10;
    private int _bombCount = MaxBombCount;

    // 폭탄 게이지
    private const float MaxBombGauge = 10f;
    private float _bombGauge = 1f;
    private const float BombChargeRate = 2f;

    // 투척 애니메이션: 전체 길이의 이 비율까지가 팔을 뒤로 젖히는 구간이다
    private const float WindUpNormalizedTime = 0.66f;

    private Animator _animator;
    private int _bombLayerIndex;
    private bool _isCharging;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _bombLayerIndex = _animator.GetLayerIndex("Bomb Layer");
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;

        // 1. 마우스 오른쪽 버튼을 누르면 팔 젖히기 애니메이션 시작
        if (Input.GetButtonDown("Fire2") && _bombCount > 0)
        {
            _isCharging = true;
            _animator.SetFloat("BombSpeed", 1f);
            _animator.SetBool("BombCharge", true);
        }

        // 2. 길게 누를 수록 게이지 충전, 애니메이션은 젖힌 자세에서 멈춘다
        if (_isCharging && Input.GetButton("Fire2"))
        {
            if (_bombGauge < MaxBombGauge)
            {
                _bombGauge += BombChargeRate * Time.deltaTime;
            }

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(_bombLayerIndex);
            if (stateInfo.IsName("Grenade Throw") && stateInfo.normalizedTime >= WindUpNormalizedTime)
            {
                _animator.SetFloat("BombSpeed", 0f);
            }
        }

        // 3. 마우스 오른쪽 버튼 땔 때 던지는 동작 재생 및 투척
        if (Input.GetButtonUp("Fire2") && _isCharging)
        {
            _isCharging = false;
            _animator.SetFloat("BombSpeed", 1f);
            _animator.SetBool("BombCharge", false);

            Throw();
            _bombCount--;
            _bombGauge = 1f;
        }
    }

    private void Throw()
    {
        // 2. 발사 위치에서 수류탄 생성 --> 오브젝트 풀링
        Bomb bomb = PlayerBombPool.Instance.GetBomb(_firePoint);
        // Bomb bomb = Instantiate(_bombPrefab, _firePoint.position, Quaternion.identity);

        // 3. 생성된 수류탄을 시선 방향으로 던지기
        Rigidbody rigidbody = bomb.GetComponent<Rigidbody>();
        rigidbody.AddForce(Camera.main.transform.forward * _throwPower * _bombGauge, ForceMode.Impulse);
    }
}
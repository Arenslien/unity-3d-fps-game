using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// FSM(유한 상태 머신) : 유한 개의 상태가 있고, 상태마다 행동을 다르게 함으로써 AI를 구현하는 방법

// 좀비가 가질 수 있는 상태
public enum ZombieState
{
    Idle,       // 대기: 제자리에서 플레이어를 기다린다.
    Patrol,     // 정찰: 정찰 roaming Position 근처를 배회
    Roaming,    // 로밍: 다음 Roaming 위치로 이동
    Trace,      // 추격: 플레이어를 쫓아간다.
    Comeback,   // 복귀: 처음 자리로 돌아간다.
    Attack,     // 공격: 일정 시간마다 플레이어를 때린다.
    Hit,        // 피격: 공격(총알, 폭탄)을 받아 잠깐 멈칫한다.
    Death       // 죽음: 쓰러지고 잠시 뒤 사라진다.
}

[RequireComponent(typeof(NavMeshAgent))]
public class Zombie : MonoBehaviour, IDamageable
{
    [Header("스탯")]
    [SerializeField] private float _detectDistance = 10f; // 플레이어를 발견하는 거리
    [SerializeField] private float _comebackDistance = 30f; // 처음 자리에서 멀어지면 돌아가는 거리
    [SerializeField] private float _attackRange = 1.5f; // 공격 범위
    [SerializeField] private float _attackInterval = 1.2f; // 공격 간격
    [SerializeField] private float _moveSpeed = 5f; // 이동속도
    [SerializeField] private float _maxHealth = 50;
    [SerializeField] private float _damage = 10f;
    private float _health;

    [Header("이펙트")]
    [SerializeField] private GameObject _bloodSprayPrefab;

    private float _attackTimer = 1.2f;
    private Vector3 _startPosition;
    private GameObject _player;
    private NavMeshAgent _agent;

    public float Health => _health;
    public float MaxHealth => _maxHealth;

    private ZombieState _state = ZombieState.Idle;

    // Animator
    private Animator _animator;

    // Zombie Event Action
    public event Action OnDamaged = null;
    public event Action OnDeath = null;

    private void Awake()
    {
        Initialize(transform.position);
        _animator = GetComponentInChildren<Animator>();
        _health = _maxHealth;
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _agent = GetComponent<NavMeshAgent>();                  // 길을 찾아 걷게 하는 컴포넌트 NavMeshAgent 참조

        OnDeath += ZombieManager.Instance.DecreaseZombie;
        _agent.speed = _moveSpeed;
    }

    private void OnDestroy()
    {
        OnDeath -= ZombieManager.Instance.DecreaseZombie;
    }

    public void Initialize(Vector3 position)
    {
        _startPosition = position;
        transform.position = _startPosition;
    }

    private void Update()
    {
        if (_state == ZombieState.Death) return;

        _animator.SetFloat("Speed", _agent.velocity.magnitude);

        if (GameManager.Instance.State != GameState.Playing) return;

        switch (_state)
        {
            case ZombieState.Idle:
                Idle();
                break;
            case ZombieState.Trace:
                Trace();
                break;
            case ZombieState.Comeback:
                Comeback();
                break;
            case ZombieState.Attack:
                Attack();
                break;
            case ZombieState.Hit:
                Hit();
                break;
        }
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.Damage <= 0 || _health <= 0) return;

        _health -= damageInfo.Damage;
        OnDamaged?.Invoke();

        if (_health > 0)
        {
            // Todo. 맞은 부위에 피 이펙트를 실행하고 싶다.
            GameObject bloodSpray = Instantiate(_bloodSprayPrefab, transform);
            bloodSpray.transform.position = damageInfo.HitPoint;
            bloodSpray.transform.forward = damageInfo.HitNormal;

            Debug.Log($"{_state} -> Hit");
            _state = ZombieState.Hit;
            _animator.SetTrigger("Hit");
            StartCoroutine(Hit_Coroutine());
        }
        else
        {
            _state = ZombieState.Death;
            Death();
        }
    }

    private IEnumerator Hit_Coroutine()
    {
        StopAgent();

        yield return new WaitForSeconds(0.5f); // 잠깐 멈칫

        Debug.Log($"{_state} -> Trace");
        _state = ZombieState.Trace;
    }

    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    public void Idle()
    {
        StopAgent();

        // 플레이어가 가까워지면 Trace 상태로 전이한다.
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance <= _detectDistance)
        {
            Debug.Log($"{_state} -> Trace");
            _state = ZombieState.Trace;
        }
    }

    public void Trace()
    {
        // 0. Comeback 조건
        float distance = Vector3.Distance(transform.position, _startPosition);

        if (distance > _comebackDistance)
        {
            Debug.Log($"{_state} -> Comeback");
            _state = ZombieState.Comeback;
            return;
        }

        // 0. 플레이어 공격 조건
        if (Vector3.Distance(transform.position, _player.transform.position) <= _attackRange)
        {
            Debug.Log($"{_state} -> Attack");
            _state = ZombieState.Attack;
            return;
        }

        // SetDestination은 목적지 설정
        // - 호출할 때마다 목적지를 향해 조금씩 이동 (X)
        // - 한 번 세팅하면 내부에서 계속해서 이동  (O)
        _agent.SetDestination(_player.transform.position);
    }

    public void Comeback()
    {
        // if (_agent.remainingDistance <= _agent.stoppingDistance)
        // if (_agent.pathStatus == NavMeshPathStatus.PathComplete)
        if (Vector3.Distance(_startPosition, transform.position) < _agent.stoppingDistance)
        {
            Debug.Log($"{_state} -> Idle");
            _state = ZombieState.Idle;
        }

        _agent.SetDestination(_startPosition);
    }

    public void Attack()
    {
        StopAgent();

        if (Vector3.Distance(transform.position, _player.transform.position) > _attackRange)
        {
            Debug.Log($"{_state} -> Trace");
            _state = ZombieState.Trace;
            return;
        }

        _attackTimer += Time.deltaTime;

        if (_attackTimer >= _attackInterval)
        {
            _attackTimer = 0f;
            _animator.SetTrigger("Attack");
            Debug.Log("플레이어 공격!");
        }
    }

    public void OnAttackHit()
    {
        bool isInAttackRange = Vector3.Distance(transform.position, _player.transform.position) > _attackRange;
        if (_state != ZombieState.Attack || isInAttackRange) return;

        DamageInfo damageInfo = new DamageInfo
        (
            _damage,
            _player.transform.position,
            (transform.position - _player.transform.position),
            gameObject
        );

        _player.GetComponent<Player>().TakeDamage(damageInfo);
    }

    public void Hit()
    {
        StartCoroutine(Hit_Coroutine());
    }

    public void Death()
    {
        StopAgent();
        _animator.SetTrigger("Death");
        OnDeath?.Invoke();
        StartCoroutine(Death_Coroutine());
    }

    private void StopAgent()
    {
        _agent.ResetPath(); // 에이전트의 목적지 삭제
        _agent.velocity = Vector3.zero;
    }
}
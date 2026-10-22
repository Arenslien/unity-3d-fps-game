using TMPro;
using UnityEngine;

// FSM(유한 상태 머신) : 유한 개의 상태가 있고, 상태마다 행동을 다르게 함으로써 AI를 구현하는 방법

// 좀비가 가질 수 있는 상태
public enum ZombieState
{
    Idle, // 대기: 제자리에서 플레이어를 기다린다.
    Trace, // 추격: 플레이어를 쫓아간다.
    Comeback, // 복귀: 처음 자리로 돌아간다.
    Attack, // 공격: 일정 시간마다 플레이어를 때린다.
    Hit, // 피격: 공격(총알, 폭탄)을 받아 잠깐 멈칫한다.
    Death // 죽음: 쓰러지고 잠시 뒤 사라진다.
}

public class Zombie : MonoBehaviour
{
    [Header("스탯")]
    [SerializeField] private float _detectDistance = 8f; // 플레이어를 발견하는 거리

    [SerializeField] private float _combackDistance = 15f; // 처음 자리에서 멀어지면 돌아가는 거리
    [SerializeField] private float _moveSpeed = 5f;

    private CharacterController _controller;
    private Vector3 _startPosition;
    private GameObject _player;

    private ZombieState _state = ZombieState.Idle;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _startPosition = transform.position;
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
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
            case ZombieState.Death:
                Death();
                break;
        }
    }

    private void Idle()
    {
        // Todo: Idle 애니메이션 추가

        // 플레이어가 가까워지면 Trace 상태로 전이한다.
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance <= _detectDistance)
        {
            Debug.Log($"{_state} -> Trace");
            _state = ZombieState.Trace;
        }
    }

    private void Trace()
    {
        // Todo: Run 애니메이션 실행

        // 0. Comeback 조건
        float distance = Vector3.Distance(transform.position, _startPosition);

        if (distance > _detectDistance)
        {
            Debug.Log($"{_state} -> Comeback");
            _state = ZombieState.Comeback;
        }

        // 1. 방향 구하기
        Vector3 direction = _player.transform.position - transform.position;
        direction.y = 0;
        direction.Normalize();

        // 2. 해당 방향으로 몸 회전
        transform.forward = direction;

        // 3. 이동
        _controller.SimpleMove(direction * _moveSpeed);
    }

    private void Comeback()
    {
        if (Vector3.Distance(_startPosition, transform.position) < 1f)
        {
            Debug.Log($"{_state} -> Idle");
            _state = ZombieState.Idle;
        }


        // 1. 방향 구하기
        Vector3 direction = _startPosition - transform.position;
        direction.y = 0;
        direction.Normalize();

        // 2. 해당 방향으로 몸 회전
        transform.forward = direction;

        // 3. 이동
        _controller.SimpleMove(direction * _moveSpeed);
    }

    private void Attack()
    {
        Debug.Log("[Zombie.cs] - Attack");
    }

    private void Hit()
    {
        Debug.Log("[Zombie.cs] - Hit");
    }

    private void Death()
    {
        Debug.Log("[Zombie.cs] - Death");
    }
}
using UnityEngine;

// 목표: 키보드 방향키에 따라 그 방향으로 이동
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _jumpPower = 7f;
    private CharacterController _characterController;
    private PlayerStamina _playerStamina;
    private bool _isSprinting = false;

    // 현재 속도
    private Vector3 _currentVelocity = Vector3.zero;

    // 중력
    private const float Gravity = -20f;
    private float _yVelocity = 0f; // Y축 속도: 매 프레임마다 

    // 점프
    private int _jumpCount = 0;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerStamina = GetComponent<PlayerStamina>();
    }

    private void Update()
    {
        // 1. 질주
        if (Input.GetKey(KeyCode.LeftShift) && _playerStamina.Gauge > 0f)
        {
            StartSprint();
            _playerStamina.UseGauge(2.0f * Time.deltaTime);
            _isSprinting = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            EndSprint();
            _isSprinting = false;
        }

        // 2. 점프
        if (Input.GetButtonDown("Jump") && _jumpCount < 2 && _playerStamina.Gauge > 1f)
        {
            Jump();
            _playerStamina.UseGauge(3f); // 스태미나 차감
        }

        // 3. 매 프레임마다 항상 해야할 것
        Move();
        ApplyGravity();

        // 스태미나 회복
        if (!_isSprinting)
        {
            _playerStamina.RestoreGauge(2.0f * Time.deltaTime);
        }

        Debug.Log($"현재 스태미나: {_playerStamina.Gauge}");
    }

    private void Move()
    {
        // 1. 키보드 입력을 받는다.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2. 키보드 입력에 따라 방향을 구한다.
        // - Local  좌표계: 한 대상을 중심
        // - Global 좌표계: 세상을 중심
        Vector3 direction = new Vector3(h, 0f, v);
        direction = transform.TransformDirection(direction);
        direction.Normalize();

        // 3. 속도를 구한다.
        _currentVelocity = direction * _moveSpeed;
        _currentVelocity.y = _yVelocity;

        // 4. 방향과 속력에 따라 이동한다.
        // transform.position += direction * (_moveSpeed * Time.deltaTime);
        _characterController.Move(_currentVelocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        // 1. 중력 적용 및 중력과 점프 횟수 초기화
        if (!_characterController.isGrounded)
        {
            _yVelocity += Gravity * Time.deltaTime;
        }
        else
        {
            _yVelocity = 0f;
            _jumpCount = 0;
        }
    }

    private void Jump()
    {
        // 1. 점프
        Debug.Log("점프");
        _yVelocity = _jumpPower;
        _jumpCount++;
    }

    public void StartSprint()
    {
        _moveSpeed = 12f;
    }

    public void EndSprint()
    {
        _moveSpeed = 7f;
    }
}
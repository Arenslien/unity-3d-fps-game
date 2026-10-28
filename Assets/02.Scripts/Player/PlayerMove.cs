using UnityEngine;

// 목표: 키보드 방향키에 따라 그 방향으로 이동
public class PlayerMove : MonoBehaviour
{
    [Header("스탯")]
    [SerializeField] private float _moveSpeed = 7f;

    [SerializeField] private float _sprintSpeed = 12f;
    [SerializeField] private float _jumpPower = 10f;
    [SerializeField] private float _staminaRecovery = 15f;
    [SerializeField] private float _sprintCost = 10f;
    [SerializeField] private float _jumpCost = 20f;

    [Header("컴포넌트")]
    [SerializeField] private CharacterController _characterController;

    // 중력
    private const float Gravity = -20f;
    private float _yVelocity = 0f; // Y축 속도: 매 프레임마다 

    // 점프 및 달리기
    private int _jumpCount = 0;
    private const int MaxJumps = 2;
    private bool _isSprint = false;

    // 플레이어
    private Player _player;

    // 애니메이터
    private Animator _animator;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _player = GetComponent<Player>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;

        // 0. 스태미나 회복
        if (!_isSprint)
        {
            float stamina = Mathf.Min(Player.MaxStamina, _player.Stamina + _staminaRecovery * Time.deltaTime);
            _player.SetStamina(stamina);
        }

        // 1. 키보드 입력을 받는다.
        float h = Input.GetAxisRaw("Horizontal"); // -1, 0, 1
        float v = Input.GetAxisRaw("Vertical"); // -1, 0, 1

        // 2. 키보드 입력에 따라 방향을 구한다.
        Vector3 direction = new Vector3(h, 0f, v);
        direction.Normalize();

        _animator.SetFloat("MoveMotion", direction.magnitude);

        direction = transform.TransformDirection(direction);
        // Local  좌표계: 한 대상을 중심
        // Global 좌표계: 세상을 중심

        // 3. 중력을 누적한다. --> 땅에 서 있지 않을 때만 누적
        if (!_characterController.isGrounded)
        {
            _yVelocity += Gravity * Time.deltaTime;
        }
        else
        {
            _yVelocity = -1f;
            _jumpCount = 0;
        }

        // 4. 만약 점프 키를 누르면 점프
        if (Input.GetButtonDown("Jump") && _jumpCount < MaxJumps && _player.Stamina >= _jumpCost) // 중재자 패턴
        {
            _jumpCount++;
            _yVelocity = _jumpPower;
            _player.UseStamina(_jumpCost);
        }

        // 5. 속도를 구한다.
        float requiredStamina = _sprintCost * Time.deltaTime;
        _isSprint = Input.GetButton("Sprint") && _player.Stamina > requiredStamina;
        float currentSpeed = _moveSpeed;

        if (_isSprint)
        {
            currentSpeed = _sprintSpeed;
            _player.UseStamina(requiredStamina);
        }

        Vector3 velocity = direction * currentSpeed;
        velocity.y = _yVelocity;

        // 6. 방향과 속력에 따라 이동한다.
        // transform.position += direction * (_moveSpeed * Time.deltaTime);
        _characterController.Move(velocity * Time.deltaTime);
    }
}
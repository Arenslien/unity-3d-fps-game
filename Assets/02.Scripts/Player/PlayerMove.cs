using UnityEngine;

// 목표: 키보드 방향키에 따라 그 방향으로 이동
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _jumpPower = 7f;
    [SerializeField] private CharacterController _characterController;

    // 중력
    private const float Gravity = -20f;
    private float _yVelocity = 0f; // Y축 속도: 매 프레임마다 

    // 점프

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // 1. 키보드 입력을 받는다.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2. 키보드 입력에 따라 방향을 구한다.
        Vector3 direction = new Vector3(h, 0f, v);
        direction.Normalize();
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
            _yVelocity = 0f;
        }

        // 4. 만약 점프 키를 누르면 점프
        if (Input.GetButtonDown("Jump")) // 중재자 패턴
        {
            _yVelocity = _jumpPower;
        }

        // 5. 속도를 구한다.
        Vector3 velocity = direction * _moveSpeed;
        velocity.y = _yVelocity;

        // 4. 방향과 속력에 따라 이동한다.
        // transform.position += direction * (_moveSpeed * Time.deltaTime);
        _characterController.Move(velocity * Time.deltaTime);
    }
}
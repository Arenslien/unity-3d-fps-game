using UnityEngine;

// 목표: 키보드 방향키에 따라 그 방향으로 이동
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private CharacterController _characterController;

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

        // 3. 방향과 속력에 따라 이동한다.
        // transform.position += direction * (_moveSpeed * Time.deltaTime);
        _characterController.Move(direction * (_moveSpeed * Time.deltaTime));
    }
}
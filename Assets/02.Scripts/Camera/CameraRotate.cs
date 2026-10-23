using UnityEngine;

// 목표: 마우스를 움직이면 카메라를 그 방향으로 회전하고 싶다.
public class CameraRotate : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 200f;

    private float _accumulationX = 0f; // 좌우 회전 각도
    private float _accumulationY = 0f; // 상하 회전 각도

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;

        // 1. 마우스 입력을 받는다.
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Todo: Unity 조건부 실행 어트리뷰트 : Conditional Attribute
        // GameLogger.Log($"mouseX : {mouseX}, mouseY : {mouseY}");

        // 2. 마우스 입력에 따라 회전할 방향을 구한다.
        // - 마우스 입력값 누적
        _accumulationX += mouseX * _rotationSpeed * Time.deltaTime;
        _accumulationY += -mouseY * _rotationSpeed * Time.deltaTime;

        // 누적하기 전에 내 변수에서 먼저 -90 ~ 90도로 제한
        _accumulationY = Mathf.Clamp(_accumulationY, -90, 90);

        // 누적한 회전값 카메라에 적용
        transform.eulerAngles = new Vector3(_accumulationY, _accumulationX);

        // 3. 그 방향으로 회전시킨다.
        // - 새로운 각도 = 현재 각도 + 회전 속도 * 시간
        // transform.eulerAngles += direction * _rotationSpeed * Time.deltaTime;

        // // 4. 위 아래 회전 값을 -90 ~ 90도로 제한한다.
        // Vector3 rotation = transform.eulerAngles;
        // // rotation.x = Mathf.Max(-90f, Mathf.Min(90, rotation.x));
        // // - 유니티는 0~360 체계를 쓰기 때문에 음수는 자동으로 변환되어서 반환된다.
        // rotation.x = Mathf.Clamp(rotation.x, -90, 90);

        // transform.eulerAngles = rotation;
    }
}
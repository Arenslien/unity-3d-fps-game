using Unity.VisualScripting;
using UnityEngine;

// 목표: 마우스 오른쪽 클릭 시 : 시선이 바라보는 방향으로 수류탄 투척
public class PlayerBombFire : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Bomb _bombPrefab;
    [SerializeField] private float _throwPower = 15f;

    // 폭탄 개수
    private const int MaxBombCount = 10;
    private int _bombCount = MaxBombCount;

    // 폭탄 게이지
    private const float MaxBombGauge = 10f;
    private float _bombGauge = 1f;
    private const float BombChargeRate = 2f;

    private void Update()
    {
        // 1. 마우스 오른쪽 버튼 길게 누를 수록 게이지 충전
        if (Input.GetButton("Fire2") && _bombGauge < MaxBombGauge)
        {
            _bombGauge += BombChargeRate * Time.deltaTime;
        }

        // 2. 마우스 오른쪽 버튼 땔 때 입력 받기
        if (Input.GetButtonUp("Fire2") && _bombCount > 0)
        {
            Throw();
            _bombCount--;
            _bombGauge = 1f;
        }
    }

    private void Throw()
    {
        // 2. 발사 위치에서 수류탄 생성
        Bomb bomb = Instantiate(_bombPrefab, _firePoint.position, Quaternion.identity);

        // 3. 생성된 수류탄을 시선 방향으로 던지기
        Rigidbody rigidbody = bomb.GetComponent<Rigidbody>();
        rigidbody.AddForce(Camera.main.transform.forward * _throwPower * _bombGauge, ForceMode.Impulse);
    }
}
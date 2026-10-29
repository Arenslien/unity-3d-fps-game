using UnityEngine;

// 저격모드에서 마우스 오른쪽 버튼을 누르고 있는 동안 시야각을 좁혀 확대
[RequireComponent(typeof(Player))]
public class PlayerZoom : MonoBehaviour
{
    [SerializeField] private GameObject _crosshair;  // 조준점 그림
    [SerializeField] private GameObject _sniperZoom; // 조준경 그림
    [SerializeField] private float _normalFov = 60f;
    [SerializeField] private float _zoomFov = 15f;
    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _sniperZoom.SetActive(false);
    }

    private void Update()
    {
        bool isPlaying = GameManager.Instance.State == GameState.Playing;
        bool isSniperMode = _player.WeaponMode == WeaponMode.Sniper;

        if (!isPlaying || !isSniperMode) return;

        bool isZooming = Input.GetMouseButton(1);
        _crosshair.SetActive(!isZooming);
        _sniperZoom.SetActive(isZooming);
        Camera.main.fieldOfView = isZooming ? _zoomFov : _normalFov;
    }
}
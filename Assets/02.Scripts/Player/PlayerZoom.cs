using UnityEngine;

// 저격모드에서 마우스 오른쪽 버튼을 누르고 있는 동안 시야각을 좁혀 확대
[RequireComponent(typeof(Player))]
public class PlayerZoom : MonoBehaviour
{
    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        bool isPlaying = GameManager.Instance.State == GameState.Playing;
        bool isSniperMode = _player.WeaponMode == WeaponMode.Sniper;

        if (!isPlaying || !isSniperMode) return;

        bool isZooming = Input.GetMouseButton(1);

        Camera.main.fieldOfView = isZooming ? 15f : 60f;
    }
}
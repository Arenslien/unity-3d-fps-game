using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStats : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;

    [SerializeField] private Player _player;

    private void LateUpdate()
    {
        // _healthSlider.value = 플레이어 체력 / 플레이어 최대 체력;
        _staminaSlider.value = _player.Stamina / Player.MaxStamina;
    }
}
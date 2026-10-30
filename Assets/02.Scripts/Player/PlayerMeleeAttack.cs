using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    private Player _player;
    private Animator _animator;
    private int _meleeLayerIndex;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _player = GetComponent<Player>();
        _meleeLayerIndex = _animator.GetLayerIndex("Melee Layer");
    }

    private void Update()
    {
        bool isPlaying = GameManager.Instance.State == GameState.Playing;
        bool isMelee = _player.WeaponMode == WeaponMode.Melee;
        if (!isPlaying || !isMelee) return;

        // Todo: 근접 공격 애니메이션 중일 땐 중복 안되게 수정
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("근접공격");
            _animator.SetTrigger("Slash");
        }
    }
}
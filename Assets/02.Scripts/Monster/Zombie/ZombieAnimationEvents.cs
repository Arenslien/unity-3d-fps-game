using UnityEngine;

public class ZombieAnimationEvents : MonoBehaviour
{
    private Zombie _zombie;

    private void Awake()
    {
        _zombie = GetComponentInParent<Zombie>();
    }

    public void AttackHit()
    {
        _zombie.OnAttackHit();
    }
}
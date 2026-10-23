using UnityEngine;

// Todo: 공격 받으면 산산조각 나듯 부셔진다.
public class Box : MonoBehaviour, IDamageable
{
    public void TakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.Damage < 0) return;

        // Todo: 뭔가 해야 함.
    }
}
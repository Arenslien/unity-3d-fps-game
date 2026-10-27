using UnityEngine;

public struct DamageInfo
{
    public float Damage;             // 대미지 양
    public Vector3 HitPoint;        // 맞은 자리
    public Vector3 HitNormal;       // 맞은 자리가 수직으로 바라보는 방향
    public GameObject Attacker;

    public DamageInfo(float damage, Vector3 hitPoint, Vector3 hitNormal, GameObject attacker)
    {
        Damage = damage;
        HitPoint = hitPoint;
        HitNormal = hitNormal;
        Attacker = attacker;
    }
}
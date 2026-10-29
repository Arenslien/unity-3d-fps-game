using UnityEngine;

public class HitState : ZombieStateBase
{
    public HitState(Zombie zombie) : base(zombie)
    {
    }

    public override void Enter()
    {
        _zombie.Hit();
    }

    public override void Execute()
    {
    }

    public override void Exit()
    {
    }
}
using UnityEngine;

public class DeathState : ZombieStateBase
{
    public DeathState(Zombie zombie) : base(zombie)
    {
    }

    public override void Enter()
    {
        _zombie.Death();
    }

    public override void Execute()
    {
    }

    public override void Exit()
    {
    }
}
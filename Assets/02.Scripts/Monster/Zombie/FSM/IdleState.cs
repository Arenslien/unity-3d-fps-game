public class IdleState : ZombieStateBase
{
    public IdleState(Zombie zombie) : base(zombie)
    {
    }

    public override void Enter()
    {
    }

    public override void Execute()
    {
        _zombie.Idle();
    }

    public override void Exit()
    {
    }
}
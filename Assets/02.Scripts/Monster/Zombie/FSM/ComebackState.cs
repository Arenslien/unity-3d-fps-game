public class ComebackState : ZombieStateBase
{
    public ComebackState(Zombie zombie) : base(zombie)
    {
    }

    public override void Enter()
    {
    }

    public override void Execute()
    {
        _zombie.Comeback();
    }

    public override void Exit()
    {
    }
}
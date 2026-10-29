public class TraceState : ZombieStateBase
{
    public TraceState(Zombie zombie) : base(zombie)
    {
    }

    public override void Enter()
    {
    }

    public override void Execute()
    {
        _zombie.Trace();
    }

    public override void Exit()
    {
    }
}
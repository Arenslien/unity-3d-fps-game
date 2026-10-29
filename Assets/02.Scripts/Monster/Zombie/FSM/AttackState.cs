public class AttackState : ZombieStateBase
{
    public AttackState(Zombie zombie) : base(zombie)
    {
    }

    public override void Enter()
    {
    }

    public override void Execute()
    {
        _zombie.Attack();
    }

    public override void Exit()
    {
    }
}
public abstract class ZombieStateBase
{
    protected readonly Zombie _zombie;

    protected ZombieStateBase(Zombie zombie)
    {
        _zombie = zombie;
    }

    public abstract void Enter();

    public abstract void Execute();

    public abstract void Exit();
}
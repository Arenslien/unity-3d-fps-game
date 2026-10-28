using UnityEngine;

public class ZombieStateMachine : MonoBehaviour
{
    private ZombieStateBase _currentState;

    public void ChangeState(ZombieStateBase nextState)
    {
        Debug.Log($"[ZombieStateMachine] - {_currentState} -> {nextState}");

        _currentState?.Exit();
        _currentState = nextState;
        _currentState.Enter();
    }

    public void Update()
    {
        _currentState?.Execute();
    }
}
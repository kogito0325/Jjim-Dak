using UnityEngine;

// Idle 상태
public class IdleState : IState
{
    private readonly GameObject _gameObject;

    public IdleState(GameObject gameObject)
    {
        _gameObject = gameObject;
    }

    public void Enter()
    {
        Debug.Log("Idle 상태 시작");
        // Idle 상태 진입 시 필요한 초기화 작업
    }

    public void Execute()
    {
        // Idle 상태 동안 수행할 작업 (예: 입력 대기)
        Debug.Log("Idle 상태 유지");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 스페이스바를 누르면 Move 상태로 전환
            _gameObject.GetComponent<StateMachine>().ChangeState(new MoveState(_gameObject));
        }
    }

    public void Exit()
    {
        Debug.Log("Idle 상태 종료");
        // Idle 상태 종료 시 필요한 정리 작업
    }
}

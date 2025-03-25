using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Move 상태
public class MoveState : IState
{
    private readonly GameObject _gameObject;

    public MoveState(GameObject gameObject)
    {
        _gameObject = gameObject;
    }

    public void Enter()
    {
        Debug.Log("Move 상태 시작");
        // Move 상태 진입 시 필요한 초기화 작업
    }

    public void Execute()
    {
        // Move 상태 동안 수행할 작업 (예: 이동)
        Debug.Log("Move 상태 유지");
        _gameObject.transform.Translate(Vector3.forward * Time.deltaTime * 5); // 앞으로 이동

        if (Input.GetKeyDown(KeyCode.S))
        {
            // S키를 누르면 Idle 상태로 전환
            _gameObject.GetComponent<StateMachine>().ChangeState(new IdleState(_gameObject));
        }
    }

    public void Exit()
    {
        Debug.Log("Move 상태 종료");
        // Move 상태 종료 시 필요한 정리 작업
    }
}

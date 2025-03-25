using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// StateMachine을 사용하는 예제
public class PlayerController : MonoBehaviour
{
    private StateMachine _stateMachine;

    void Start()
    {
        _stateMachine = GetComponent<StateMachine>();

        // StateMachine 컴포넌트가 있는지 확인
        if (_stateMachine == null)
        {
            _stateMachine = gameObject.AddComponent<StateMachine>();
        }

        _stateMachine.SetInitialState(new IdleState(gameObject)); // 초기 상태 설정
    }
}
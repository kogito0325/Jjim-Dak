using UnityEngine;
using System.Collections.Generic;

/*StateMachine 클래스:

_currentState: 현재 활성화된 IState 인스턴스를 저장합니다.

_states: (선택 사항) 상태들을 관리하기 위한 딕셔너리입니다. 상태 이름을 키로 사용하여 각 상태에 접근할 수 있습니다.

AddState(): (선택 사항) 상태를 _states 딕셔너리에 추가합니다.

SetInitialState(): 초기 상태를 설정하고 Enter() 메서드를 호출합니다.

ChangeState(): 현재 상태를 종료(Exit())하고 새로운 상태로 전환(Enter())합니다.

Update(): 매 프레임마다 현재 상태의 Execute() 메서드를 호출합니다.*/


// 상태 머신 클래스
public class StateMachine : MonoBehaviour
{
    // 현재 상태
    private IState _currentState;

    // 상태 목록 (선택 사항: 필요에 따라 사용)
    private Dictionary<string, IState> _states = new Dictionary<string, IState>();

    // 현재 상태 반환 (읽기 전용)
    public IState CurrentState { get { return _currentState; } }

    // 상태를 추가합니다. (선택 사항: 필요에 따라 사용)
    public void AddState(string stateName, IState state)
    {
        _states.Add(stateName, state);
    }

    // 초기 상태를 설정합니다.
    public void SetInitialState(IState initialState)
    {
        _currentState = initialState;
        _currentState.Enter();
    }

    // 상태를 변경합니다.
    public void ChangeState(IState newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
        }

        _currentState = newState;
        _currentState.Enter();
    }

    // 매 프레임마다 현재 상태의 Execute 메서드를 호출합니다.
    private void Update()
    {
        if (_currentState != null)
        {
            _currentState.Execute();
        }
    }
}
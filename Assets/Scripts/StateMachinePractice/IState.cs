
/*IState 인터페이스:

Enter(): 상태가 시작될 때 호출됩니다. 필요한 초기화 작업을 수행합니다.

Execute(): 매 프레임마다 호출됩니다. 상태의 주요 로직을 수행합니다.

Exit(): 상태가 종료될 때 호출됩니다. 정리 작업을 수행합니다.*/


// 상태 머신 인터페이스
public interface IState
{
    // 상태가 시작될 때 호출됩니다.
    void Enter();

    // 매 프레임마다 호출됩니다.
    void Execute();

    // 상태가 종료될 때 호출됩니다.
    void Exit();
}


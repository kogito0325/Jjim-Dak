using UnityEngine;


// StateMachine을 사용하는 예제
public class PlayerController : MonoBehaviour
{
    private StateMachine _stateMachine;
    private PlayerData _data;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidBody;
    Animator animator;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        _stateMachine = GetComponent<StateMachine>();
        _data = GetComponent<PlayerData>();

        InitSelf();

        // StateMachine 컴포넌트가 있는지 확인
        if (_stateMachine == null)
        {
            _stateMachine = gameObject.AddComponent<StateMachine>();
        }

        _stateMachine.SetInitialState(new IdleState(gameObject)); // 초기 상태 설정
    }

    private void InitSelf()
    {
        _data.InitializeFlags();
        _data.energy = _data.maxEnergy;
    }
}


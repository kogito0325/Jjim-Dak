using System.Collections.Generic;
using UnityEngine;

public enum PlayerAnimState
{
    IDLE,
    MOVE,
    JUMP,
    DASH,
    ATTACK,
    ATTACK2,
    DEAD,
    HEAL,
    GUARD,
    COUNTER
}

public class PlayerAniManager : MonoBehaviour
{
    public Animator animator { get; private set; }

    private Dictionary<PlayerAnimState, string> animationNames = new Dictionary<PlayerAnimState, string>()
    {
        { PlayerAnimState.IDLE, "Idle" },
        { PlayerAnimState.MOVE, "Move" },
        { PlayerAnimState.JUMP, "Jump" },
        { PlayerAnimState.DASH, "Dash" },
        { PlayerAnimState.ATTACK, "Attack" },
        { PlayerAnimState.ATTACK2, "Attack2" },
        { PlayerAnimState.DEAD, "Dead" },
        { PlayerAnimState.HEAL, "Heal" },
        { PlayerAnimState.GUARD, "Guard" },
        { PlayerAnimState.COUNTER, "Counter" }
    };

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Play(PlayerAnimState state)
    {
        animator.Play(animationNames[state]);
    }
}

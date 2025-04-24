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

public enum HealAnimState
{
    IDLE,
    HEALSTART,
    HEALEND
}

public class PlayerAniManager : MonoBehaviour
{
    public Animator animator { get; private set; }

    private Dictionary<PlayerAnimState, string> playerAnimNames = new Dictionary<PlayerAnimState, string>()
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

    private Dictionary<HealAnimState, string> healAnimNames = new Dictionary<HealAnimState, string>()
    {
        {HealAnimState.IDLE, "Idle" },
        {HealAnimState.HEALSTART, "HealStart" },
        {HealAnimState.HEALEND, "HealEnd" }
    };

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Play(PlayerAnimState state)
    {
        animator.Play(playerAnimNames[state]);
    }

    public void Play(HealAnimState state, GameObject obj)
    {
        obj.GetComponent<Animator>().Play(healAnimNames[state]);
    }


    public void SwitchAnimType()
    {
        animator.updateMode = animator.updateMode == AnimatorUpdateMode.UnscaledTime
            ? AnimatorUpdateMode.Normal : AnimatorUpdateMode.UnscaledTime;
    }
}

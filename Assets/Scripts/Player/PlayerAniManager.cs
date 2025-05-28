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
    HIT,
    GUARD,
    COUNTER,
    DEAD
}

public enum HealAnimState
{
    IDLE,
    HEALSTART,
    HEALEND
}

public enum GuardAnimState
{
    IDLE,
    GUARD,
    COUNTER
}

public class PlayerAniManager
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
        { PlayerAnimState.HIT, "Hit" },
        { PlayerAnimState.GUARD, "Guard" },
        { PlayerAnimState.COUNTER, "Counter" },
        { PlayerAnimState.DEAD, "Dead" }
    };

    private Dictionary<HealAnimState, string> healAnimNames = new Dictionary<HealAnimState, string>()
    {
        { HealAnimState.IDLE, "Idle" },
        { HealAnimState.HEALSTART, "HealStart" },
        { HealAnimState.HEALEND, "HealEnd" }
    };

    private Dictionary<GuardAnimState, string> guardAnimNames = new Dictionary<GuardAnimState, string>()
    {
        { GuardAnimState.IDLE, "Idle" },
        { GuardAnimState.GUARD, "Guard" },
        { GuardAnimState.COUNTER, "Counter"}
    };

    public PlayerAniManager(Animator animator)
    {
        this.animator = animator;
    }

    public void Play(PlayerAnimState state)
    {
        animator.Play(playerAnimNames[state]);
    }

    public void Play(HealAnimState state, GameObject obj)
    {
        obj.GetComponent<Animator>().Play(healAnimNames[state]);
    }

    public void Play(GuardAnimState state, GameObject obj)
    {
        obj.GetComponent<Animator>().Play(guardAnimNames[state]);
    }



    public void SwitchAnimType()
    {
        animator.updateMode = animator.updateMode == AnimatorUpdateMode.UnscaledTime
            ? AnimatorUpdateMode.Normal : AnimatorUpdateMode.UnscaledTime;
    }
}

using System.Collections.Generic;
using UnityEngine;

public enum BossAnimState
{
    Idle,
    Chase,
    Swing,
    Smash,
    Charge,
    JumpReady,
    Jump,
    Ground,
    GroggyStart,
    GroggyEnd,
    Die
}

public class BossAnimControl
{
    public Animator animator { get; private set; }

    public BossAnimControl(Animator animator)
    {
        this.animator = animator;
    }

    public void Play(BossAnimState state)
    {
        animator.Play(state.ToString());
    }

    public void SwitchAnimType()
    {
        animator.updateMode = animator.updateMode == AnimatorUpdateMode.UnscaledTime
            ? AnimatorUpdateMode.Normal : AnimatorUpdateMode.UnscaledTime;
    }
}
using UnityEngine;

public class PlayerGeyserGlideState : PlayerBaseState, IRootState
{
    public PlayerGeyserGlideState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    
    public override void EnterState()
    {
        Debug.Log("Entered Player Geyser Glide State");
        InitializeSubState();
        Ctx.Animator.SetBool(Ctx.IsGeyserGlidingHash, true);
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsGeyserGlidingHash, false);
    }

    public override void InitializeSubState()
    {
        if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        } 
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        }
        else
        {
            SetSubState(Factory.Run());
        }
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsInGeyser)
        {
            SwitchState(Factory.Glide());
        }
        else if (!Ctx.IsGlidePressed)
        {
            SwitchState(Factory.Fall());
        }
    }

    public void HandleGravity()
    {
        Ctx.CurrentMovementY = Ctx.GeyserLiftForce;
        Ctx.AppliedMovementY = Ctx.GeyserLiftForce;
    }
}

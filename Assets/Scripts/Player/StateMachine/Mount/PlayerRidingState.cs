using UnityEngine;

public class PlayerRidingState : PlayerBaseState, IRootState
{
    public PlayerRidingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public void HandleGravity()
    {
        Ctx.CurrentMovementY = Ctx.Gravity;
        Ctx.AppliedMovementY = Ctx.Gravity;
    }

    public void HandleAnimation()
    {
        if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            Ctx.RaptorAnimator.SetBool(Ctx.IsWalkingHash, false);
            Ctx.RaptorAnimator.SetBool(Ctx.IsRunningHash, false);
        } 
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            Ctx.RaptorAnimator.SetBool(Ctx.IsWalkingHash, true);
            Ctx.RaptorAnimator.SetBool(Ctx.IsRunningHash, false);
        }
        else
        {
            Ctx.RaptorAnimator.SetBool(Ctx.IsWalkingHash, true);
            Ctx.RaptorAnimator.SetBool(Ctx.IsRunningHash, true);
        }
    }
    
    public override void EnterState()
    {
        Debug.Log("Entered Player Riding State");
        InitializeSubState();
        HandleGravity();
        HandleAnimation();
        Ctx.CurrentNumberOfRaptorJumps = 0;
        Ctx.Animator.SetBool(Ctx.IsRidingHash, true);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState(){}

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
        if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress)
        {
            SwitchState(Factory.RideJump());
        }
        else if (!Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.RideFall());
        }
        else if (!Ctx.IsRidePressed)
        {
            SwitchState(Factory.Grounded());
        }
    }
}

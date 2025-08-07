using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoubleJumpState : PlayerBaseState
{
    public PlayerDoubleJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {}
    
    public override void EnterState()
    {
        Debug.Log("Entered Player Double Jump State");
        if (Ctx.IsRidePressed)
        {
            Ctx.RaptorAnimator.SetBool(Ctx.IsJumpingHash, true);
            Ctx.RaptorAnimator.SetBool(Ctx.IsDoubleJumpingHash, true);
            Ctx.CurrentNumberOfRaptorJumps++;
        }
        else
        {
            Ctx.Animator.SetBool(Ctx.IsJumpingHash, true);
            Ctx.Animator.SetBool(Ctx.IsDoubleJumpingHash, true);
            Ctx.CurrentNumberOfJumps++;
        }
        HandleJump();
    }
    
    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState(){}

    public override void InitializeSubState(){}
    
    public override void CheckSwitchStates()
    {
        if (!Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SwitchState(Factory.Run());
        } 
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SwitchState(Factory.Walk());
        }
    }
    
    void HandleJump()
    {
        Ctx.IsAirJumpPressed = false;
        Ctx.CurrentMovementY = Ctx.InitialJumpVelocity;
        Ctx.AppliedMovementY = Ctx.InitialJumpVelocity;
    }
}

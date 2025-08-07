using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState, IRootState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    
    public override void EnterState()
    {
        Debug.Log("Entered Player Jump State");
        InitializeSubState();
        HandleJump();
        Ctx.Animator.SetBool(Ctx.IsGroundedHash, false);
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, true);
    }
    
    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }
    
    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
        Ctx.Animator.SetBool(Ctx.IsDoubleJumpingHash, false);
        if (Ctx.IsJumpPressed)
        {
            Ctx.RequireNewJumpPress = true;
        }
    }

    public override void InitializeSubState()
    {
        if (Ctx.IsAirJumpPressed && Ctx.CurrentNumberOfJumps < Ctx.MaxNumberOfJumps)
        {
            SetSubState(Factory.DoubleJump());
        }
        else if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
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
        if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }

        if (Ctx.IsGlidePressed)
        {
            SwitchState(Factory.Glide());
        }
    }
    
    void HandleJump()
    {
        Ctx.CurrentMovementY = Ctx.InitialJumpVelocity;
        Ctx.AppliedMovementY = Ctx.InitialJumpVelocity;
    }
    
    public void HandleGravity()
    {
        bool isFalling = Ctx.CurrentMovementY <= 0.0f;
        float fallMultiplier = 2f;
        
        if (isFalling)
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY += (Ctx.Gravity * fallMultiplier * Time.deltaTime);
            Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * 0.5f, -20.0f);
        }
        else
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY += (Ctx.Gravity * Time.deltaTime);
            Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * 0.5f, -20.0f);
        }
    }
}

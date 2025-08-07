using UnityEngine;

public class PlayerRideFallState : PlayerBaseState, IRootState
{
    public PlayerRideFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    
    public override void EnterState()
    {
        Debug.Log("Entered Player Fall State");
        InitializeSubState();
        Ctx.RaptorAnimator.SetBool(Ctx.IsFallingHash, true);
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.RaptorAnimator.SetBool(Ctx.IsFallingHash, false);
        Ctx.RaptorAnimator.SetBool(Ctx.IsJumpingHash, false);
        Ctx.RaptorAnimator.SetBool(Ctx.IsDoubleJumpingHash, false);
    }

    public override void InitializeSubState()
    {
        if (Ctx.IsAirJumpPressed && Ctx.CurrentNumberOfRaptorJumps < Ctx.MaxNumberOfRaptorJumps)
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
        if (Ctx.CharacterController.isGrounded && Ctx.IsRidePressed)
        {
            SwitchState(Factory.Ride());
        }
        else if (Ctx.CharacterController.isGrounded && !Ctx.IsRidePressed)
        {
            SwitchState(Factory.Grounded());
        }
        else if (!Ctx.CharacterController.isGrounded && !Ctx.IsRidePressed)
        {
            SwitchState(Factory.Fall());
        }
    }

    public void HandleGravity()
    {
        float previousYVelocity = Ctx.CurrentMovementY;
        Ctx.CurrentMovementY += Ctx.Gravity * Time.deltaTime;
        Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * 0.5f, -20.0f);
    }
}

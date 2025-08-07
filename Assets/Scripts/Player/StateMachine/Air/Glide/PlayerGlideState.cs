using UnityEngine;

public class PlayerGlideState : PlayerBaseState, IRootState
{
    public PlayerGlideState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    
    public override void EnterState()
    {
        Debug.Log("Entered Player Glide State");
        InitializeSubState();
        Ctx.Animator.SetBool(Ctx.IsGlidingHash, true);
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsGlidingHash, false);
        Ctx.IsGlidePressed = false;
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
        if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }

        if (!Ctx.IsGlidePressed)
        {
            SwitchState(Factory.Fall());
        }
    }

    public void HandleGravity()
    {
        Ctx.CurrentMovementY = Ctx.GlideGravity;
        Ctx.AppliedMovementY = Mathf.Max(Ctx.CurrentMovementY, -20.0f);
    }
}

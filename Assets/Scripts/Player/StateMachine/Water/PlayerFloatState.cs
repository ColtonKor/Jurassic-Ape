using UnityEngine;

public class PlayerFloatState : PlayerBaseState, IRootState
{
    public PlayerFloatState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Debug.Log("Entered Player Float State");
        InitializeSubState();
        Ctx.IsFloating = true;
        Ctx.CurrentMovementY = 0;
        Ctx.AppliedMovementY = 0;
        Ctx.Animator.SetBool(Ctx.IsSwimmingHash, true);
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.IsFloating = false;
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
        if (Ctx.IsDodgePressed)
        {
            SwitchState(Factory.Swim());
        }
        else if (!Ctx.IsInWater && Ctx.CharacterController.isGrounded && Ctx.CurrentMovementY <= 0f)
        {
            Ctx.Animator.SetBool(Ctx.IsSwimmingHash, false);
            SwitchState(Factory.Grounded());
        }
    }

    public void HandleGravity()
    {
        
    }
}

using UnityEngine;

public class PlayerSwimState : PlayerBaseState, IRootState
{
    public PlayerSwimState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Debug.Log("Entered Player Swim State");
        InitializeSubState();
        Ctx.CurrentMovementY = 0;
        Ctx.AppliedMovementY = 0;
    }

    public override void UpdateState()
    {
        HandleGravity();
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
        if (Ctx.IsFloating)
        {
            SwitchState(Factory.Floating());
        }
        else if (!Ctx.IsInWater)
        {
            Ctx.Animator.SetBool(Ctx.IsSwimmingHash, false);
            SwitchState(Factory.Grounded());
        }
    }

    public void HandleGravity()
    {
        
    }
}

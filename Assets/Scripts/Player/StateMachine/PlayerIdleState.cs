using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base (currentContext, playerStateFactory){ }

    public override void EnterState()
    {
        Debug.Log("Enter Player Idle State");
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
        
        if(Ctx.IsRidePressed)
        {
            Ctx.RaptorAnimator.SetBool(Ctx.IsWalkingHash, false);
            Ctx.RaptorAnimator.SetBool(Ctx.IsRunningHash, false);
        }
            
        Ctx.AppliedMovementX = 0;
        Ctx.AppliedMovementZ = 0;
        Ctx.Direction = Vector3.zero;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    
    public override void ExitState(){}
    
    public override void InitializeSubState(){}

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsRidePressed && Ctx.IsAirJumpPressed && Ctx.CurrentNumberOfJumps < Ctx.MaxNumberOfJumps && !Ctx.IsGlidePressed)
        {
            SwitchState(Factory.DoubleJump());
        }
        else if (Ctx.IsRidePressed && Ctx.IsAirJumpPressed && Ctx.CurrentNumberOfRaptorJumps < Ctx.MaxNumberOfRaptorJumps)
        {
            SwitchState(Factory.DoubleJump());
        }
        else if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }
        else if (Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Walk());
        }
    }
}

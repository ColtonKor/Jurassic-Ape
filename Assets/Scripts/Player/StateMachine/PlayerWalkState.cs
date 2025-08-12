using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base (currentContext, playerStateFactory){ }

    public override void EnterState()
    {
        // Debug.Log("Enter Player Walk State");
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
        
        if(Ctx.IsRidePressed)
        {
            Ctx.RaptorAnimator.SetBool(Ctx.IsWalkingHash, true);
            Ctx.RaptorAnimator.SetBool(Ctx.IsRunningHash, false);
        }
    }

    public override void UpdateState()
    {
        Ctx.AppliedMovementX = Ctx.Direction.x * Ctx.Speed * Ctx.RaptorSpeed;
        Ctx.AppliedMovementZ = Ctx.Direction.z * Ctx.Speed * Ctx.RaptorSpeed;
        CheckSwitchStates();
    }

    public override void ExitState(){}
    
    public override void InitializeSubState(){}

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsRidePressed && Ctx.IsAirJumpPressed && Ctx.CurrentNumberOfJumps < Ctx.MaxNumberOfJumps && !Ctx.IsGlidePressed && !Ctx.IsInWater)
        {
            SwitchState(Factory.DoubleJump());
        }
        else if (Ctx.IsRidePressed && Ctx.IsAirJumpPressed && Ctx.CurrentNumberOfRaptorJumps < Ctx.MaxNumberOfRaptorJumps)
        {
            SwitchState(Factory.DoubleJump());
        }
        else if (!Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }
    }
}

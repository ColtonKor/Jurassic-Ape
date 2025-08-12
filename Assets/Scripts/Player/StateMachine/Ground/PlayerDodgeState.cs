using UnityEngine;
public class PlayerDodgeState : PlayerBaseState, IRootState
{
    public PlayerDodgeState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public void HandleGravity()
    {
        Ctx.CurrentMovementY = Ctx.Gravity;
        Ctx.AppliedMovementY = Ctx.Gravity;
    }
    
    public void HandleRoll()
    {
        Ctx.AppliedMovementX = Ctx.Direction.x * Ctx.RollSpeedTemp;
        Ctx.AppliedMovementZ = Ctx.Direction.z * Ctx.RollSpeedTemp;
        
        float rollSpeedDropMultiplier = 1.5f;
        Ctx.RollSpeedTemp -= Ctx.RollSpeedTemp * rollSpeedDropMultiplier * Time.deltaTime;

        if (Ctx.RollSpeedTemp <= Ctx.RollMinimum)
        {
            Ctx.AppliedMovementX = 0;
            Ctx.AppliedMovementZ = 0;
            if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
            {
                Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
                Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
            } 
            else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
            {
                Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
                Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
            }
            else
            {
                Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
                Ctx.Animator.SetBool(Ctx.IsRunningHash, true);
            }
            Ctx.IsDodging = false;
        }
    }
    
    public override void EnterState()
    {
        Debug.Log("Entered Player Dodge State");
        InitializeSubState();
        HandleGravity();
        Ctx.IsDodging = true;
        Ctx.RollSpeedTemp = Ctx.RollSpeed;
        Ctx.Animator.SetBool(Ctx.IsDodgeHash, true);
    }

    public override void UpdateState()
    {
        HandleRoll();
        CheckSwitchStates();
    }
    
    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsDodgeHash, false);
        if (Ctx.IsDodgePressed)
        {
            Ctx.RequireNewDodgePress = true;
        }
    }

    public override void InitializeSubState() {}

    public override void CheckSwitchStates()
    {
        if (Ctx.CharacterController.isGrounded && !Ctx.IsDodging)
        {
            SwitchState(Factory.Grounded());
        }
        else if (!Ctx.CharacterController.isGrounded && !Ctx.IsDodging)
        {
            SwitchState(Factory.Fall());
        }
        else if (Ctx.IsInWater)
        {
            SwitchState(Factory.Floating());
        }
    }
}

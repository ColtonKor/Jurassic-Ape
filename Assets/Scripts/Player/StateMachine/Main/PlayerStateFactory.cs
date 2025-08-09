using System.Collections.Generic;

enum PlayerStates
{
    idle,
    walk,
    run,
    jump,
    doubleJump,
    glide,
    geyserGlide,
    grounded,
    fall,
    dodge,
    ride,
    rideJump,
    rideFall
}

public class PlayerStateFactory
{
    private PlayerStateMachine context;
    private Dictionary<PlayerStates, PlayerBaseState> states = new Dictionary<PlayerStates, PlayerBaseState>();

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        context = currentContext;
        states[PlayerStates.idle] = new PlayerIdleState(context, this);
        states[PlayerStates.walk] = new PlayerWalkState(context, this);
        states[PlayerStates.run] = new PlayerRunState(context, this);
        states[PlayerStates.jump] = new PlayerJumpState(context, this);
        states[PlayerStates.doubleJump] = new PlayerDoubleJumpState(context, this);
        states[PlayerStates.glide] = new PlayerGlideState(context, this);
        states[PlayerStates.geyserGlide] = new PlayerGeyserGlideState(context, this);
        states[PlayerStates.grounded] = new PlayerGroundedState(context, this);
        states[PlayerStates.fall] = new PlayerFallState(context, this);
        states[PlayerStates.dodge] = new PlayerDodgeState(context, this);
        states[PlayerStates.ride] = new PlayerRidingState(context, this);
        states[PlayerStates.rideJump] = new PlayerRideJumpState(context, this);
        states[PlayerStates.rideFall] = new PlayerRideFallState(context, this);
    }

    public PlayerBaseState Idle()
    {
        return states[PlayerStates.idle];
    }

    public PlayerBaseState Walk()
    {
        return states[PlayerStates.walk];
    }

    public PlayerBaseState Run()
    {
        return states[PlayerStates.run];
    }

    public PlayerBaseState Jump()
    {
        return states[PlayerStates.jump];
    }
    
    public PlayerBaseState DoubleJump()
    {
        return states[PlayerStates.doubleJump];
    }

    public PlayerBaseState Glide()
    {
        return states[PlayerStates.glide];
    }
    
    public PlayerBaseState GeyserGlide()
    {
        return states[PlayerStates.geyserGlide];
    }

    public PlayerBaseState Grounded()
    {
        return states[PlayerStates.grounded];
    }

    public PlayerBaseState Fall()
    {
        return states[PlayerStates.fall];
    }

    public PlayerBaseState Dodge()
    {
        return states[PlayerStates.dodge];
    }

    public PlayerBaseState Ride()
    {
        return states[PlayerStates.ride];
    }
    
    public PlayerBaseState RideJump()
    {
        return states[PlayerStates.rideJump];
    }
    
    public PlayerBaseState RideFall()
    {
        return states[PlayerStates.rideFall];
    }
}

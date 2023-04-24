using Godot;
using System;

public partial class PlayerFallState : IStateMachine {
    public IStateMachine EnterState(Node actor) {
        var player = actor as Player;
        var direction = player.GetDirectionInput();
        player.DoMovement(player.GetProcessDeltaTime(), direction);

        if (player.WasGrounded) {
            player.CoyoteTime -= (float)player.GetProcessDeltaTime();
            if (player.CoyoteTime <= 0.0f) {
                player.WasGrounded = false;
                player.NumJumps--;
            }

        }

        switch (player.Facing) {
            case Facing.RIGHT:
                player.Scale = new Vector2(1, 1);
                break;
            case Facing.LEFT:
                player.Scale = new Vector2(-1, 1);
                break;
        }

        player.AnimatedSprite.SpeedScale = 1f;
        player.AnimatedSprite.Play("Fall");

        //if (player.IsJumping && player.JumpForgiveness > 0.0f) {
        //    player.WasGrounded = player.IsGrounded();
        //    return player.playerJumpState;
        //}
        if (player.IsJumping) {
            if (player.IsGrounded()) {
                // This feels bad.  Should this even be here?  Should this be in the player code instead?
                // The player touched the ground, so reset their grounded stats
                // being coyote time, jump buffer, and number of jumps.
                // However, the player is also jumping during the jump buffer, so expend a jump.
                player.ResetGroundedStats();
                player.NumJumps--;
            }
            return player.playerJumpState;
        }


        if (player.IsGrounded()) {
            player.ResetGroundedStats();
            return player.playerIdleState;
        }

        return player.playerFallState;
    }

    public void EmitStateChanged(Node actor, IStateMachine state) {
        throw new NotImplementedException();
    }
}

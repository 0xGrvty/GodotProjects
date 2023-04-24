using Godot;
using System;

public partial class PlayerRunState : IStateMachine {
    public IStateMachine EnterState(Node actor) {
        var jumpSquatEnabled = false;
        var player = actor as Player;
        var direction = player.GetDirectionInput();
        player.DoMovement(player.GetProcessDeltaTime(), direction);

        //switch (player.Facing) {
        //    case Facing.RIGHT:
        //        player.Scale = new Vector2(1, 1);
        //        break;
        //    case Facing.LEFT:
        //        player.Scale = new Vector2(-1, 1);
        //        break;
        //}

        player.AnimatedSprite.SpeedScale = 1;
        player.AnimatedSprite.Play("Run");

        if (direction == 0) {
           //EmitStateChanged(player, player.playerIdleState);
            return player.playerIdleState;
        }

        if (player.IsJumping) {
            //EmitStateChanged(player, player.playerJumpState);
            return player.playerJumpState;
        }

        if (player.Velocity.Y > 0.0 && !player.IsGrounded()) {
            player.WasGrounded = true;
            return player.playerFallState;
        }

        return player.playerRunState;
    }

    public void EmitStateChanged(Node actor, IStateMachine state) {
        GD.Print(actor.Name, " Is now in ", state, " state");
    }

}

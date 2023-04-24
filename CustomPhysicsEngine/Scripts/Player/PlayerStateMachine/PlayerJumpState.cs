using Godot;
using System;

public partial class PlayerJumpState : IStateMachine {
    public IStateMachine EnterState(Node actor) {
        var player = actor as Player;
        var direction = player.GetDirectionInput();
        player.DoMovement(player.GetProcessDeltaTime(), direction);

        switch (player.Facing) {
            case Facing.RIGHT:
                player.Scale = new Vector2(1, 1);
                break;
            case Facing.LEFT:
                player.Scale = new Vector2(-1, 1);
                break;
        }

        player.AnimatedSprite.SpeedScale = 1f;

        if (!player.AnimatedSprite.Animation.Equals("Jump")) {
            player.AnimatedSprite.Play("Jump");
        }

        if (player.Velocity.Y > 0.0) {
            return player.playerFallState;
        }

        return player.playerJumpState;
    }
    public void EmitStateChanged(Node actor, IStateMachine state) {

    }

}

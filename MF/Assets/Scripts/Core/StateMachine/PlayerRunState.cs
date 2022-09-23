using Godot;
using System;

public class PlayerRunState : IStateMachine {
    public IStateMachine EnterState(Player player) {
        // Tell the player to do their movement whenever they are in the Run state
        player.DoMovement();
        player.GetAnimatedSprite().Animation = "PlayerRun";
        if (Mathf.Sign(player.GetVelocity().x) > 0) {
            player.Scale = new Vector2(player.Scale.x, 2);
            player.Rotation = 0;
        } else if (Mathf.Sign(player.GetVelocity().x) < 0) {
            player.Scale = new Vector2(-player.Scale.x, 2);
            player.Rotation = 0;
        }
        player.GetAnimatedSprite().SpeedScale = 1;
        player.GetAnimatedSprite().Play();
        // We can attack while running
        // And if we look into PlayerAttackState, we can see that we can't move or act while attacking
        if (Input.IsActionPressed("BlessedHammer")) {
            EmitChangeStateSignal(player, player.playerAttackState);
            return player.playerAttackState;
        }

        // if the player isn't moving, then we can assume they are standing still
        if (!player.IsMoving) {
            EmitChangeStateSignal(player, player.playerIdleState);
            return player.playerIdleState;
        }

        // Otherwise, we can assume they are still running

        return player.playerRunState;
    }
    public void EmitChangeStateSignal(Player player, IStateMachine state) {
        player.EmitSignal("StateChanged", state.GetType().ToString());
    }

}

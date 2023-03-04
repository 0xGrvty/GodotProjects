using Godot;
using Godot.Collections;
using System;

public class PlayerIdleState : IStateMachine {
    public IStateMachine EnterState(Node2D p) {
        var player = (PlayerBody)p;


        switch (player.GetFacing()) {
            case FaceDir.UP:
            case FaceDir.UP_LEFT:
            case FaceDir.UP_RIGHT:
                player.GetAnimatedSprite().Animation = "PlayerIdleUp";
                break;
            case FaceDir.DOWN:
                player.GetAnimatedSprite().Animation = "PlayerIdleDown";
                break;
            case FaceDir.RIGHT:
            case FaceDir.LEFT:
                player.GetAnimatedSprite().Animation = "PlayerIdleRight";
                break;
            case FaceDir.DOWN_RIGHT:
            case FaceDir.DOWN_LEFT:
                player.GetAnimatedSprite().Animation = "PlayerIdleDownRight";
                break;
        }
        //player.GetAnimatedSprite().Animation = "PlayerIdle";
        player.GetAnimatedSprite().SpeedScale = 1;
        player.GetAnimatedSprite().Play();
        // Tell the player to do their movement while in idle state.
        // If they move, then we are no longer idle.
        player.DoMovement();
        if (player.IsMoving) {
            EmitChangeStateSignal(player, player.playerRunState);
            return player.playerRunState;
        }

        // If they aren't running, check to see if they pressed the "BlessedHammer" button
        if (Input.IsActionPressed("BlessedHammer")) {
            EmitChangeStateSignal(player, player.playerAttackState);
            return player.playerAttackState;
        }

        // Otherwise, we can assume the player is standing still

        return player.playerIdleState;
    }
    public void EmitChangeStateSignal(Node2D p, IStateMachine state) {
        var player = (PlayerBody)p;
        player.EmitSignal("StateChanged", state.GetType().ToString());
    }

}

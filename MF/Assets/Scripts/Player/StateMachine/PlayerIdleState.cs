using Godot;
using Godot.Collections;
using System;

public class PlayerIdleState : IStateMachine {
    public IStateMachine EnterState(PlayerBody player) {
        
        
        switch (player.GetFacing()) {
            case PlayerBody.FaceDir.UP:
            case PlayerBody.FaceDir.UP_LEFT:
            case PlayerBody.FaceDir.UP_RIGHT:
                player.GetAnimatedSprite().Animation = "PlayerIdleUp";
                break;
            case PlayerBody.FaceDir.DOWN:
                player.GetAnimatedSprite().Animation = "PlayerIdleDown";
                break;
            case PlayerBody.FaceDir.RIGHT:
            case PlayerBody.FaceDir.LEFT:
                player.GetAnimatedSprite().Animation = "PlayerIdleRight";
                break;
            case PlayerBody.FaceDir.DOWN_RIGHT:
            case PlayerBody.FaceDir.DOWN_LEFT:
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
    public void EmitChangeStateSignal(PlayerBody player, IStateMachine state) {
        player.EmitSignal("StateChanged", state.GetType().ToString());
    }

}

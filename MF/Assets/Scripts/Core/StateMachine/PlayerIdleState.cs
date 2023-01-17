using Godot;
using Godot.Collections;
using System;

public class PlayerIdleState : IStateMachine {
    
    private bool takenDamage = false;
    public IStateMachine EnterState(Player player) {
        switch (player.GetFacing()) {
            case Player.FaceDir.UP:
            case Player.FaceDir.UP_LEFT:
            case Player.FaceDir.UP_RIGHT:
                player.GetAnimatedSprite().Animation = "PlayerIdleUp";
                break;
            case Player.FaceDir.DOWN:
                player.GetAnimatedSprite().Animation = "PlayerIdleDown";
                break;
            case Player.FaceDir.RIGHT:
            case Player.FaceDir.LEFT:
                player.GetAnimatedSprite().Animation = "PlayerIdleRight";
                break;
            case Player.FaceDir.DOWN_RIGHT:
            case Player.FaceDir.DOWN_LEFT:
                player.GetAnimatedSprite().Animation = "PlayerIdleDownRight";
                break;
        }
        //player.GetAnimatedSprite().Animation = "PlayerIdle";
        player.GetAnimatedSprite().SpeedScale = 1;
        player.GetAnimatedSprite().Play();
        // We can check to see if the player is running
        if (player.IsMoving) {
            EmitChangeStateSignal(player, player.playerRunState);
            return player.playerRunState;
        }

        // If they aren't running, check to see if they pressed the "BlessedHammer" button
        if (Input.IsActionPressed("BlessedHammer")) {
            EmitChangeStateSignal(player, player.playerAttackState);
            return player.playerAttackState;
        }

        if (Input.IsActionPressed("DamageTest")) {
            //player.GetNode<Node>("Health").CallDeferred("TakeDamage", -5);
            if (!takenDamage) {
                takenDamage = true;
                player.GetHealth().TakeDamage(5);
            }
        }

        if (Input.IsActionJustReleased("DamageTest")) {
            takenDamage = false;
        }

        // Otherwise, we can assume the player is standing still

        return player.playerIdleState;
    }
    public void EmitChangeStateSignal(Player player, IStateMachine state) {
        player.EmitSignal("StateChanged", state.GetType().ToString());
    }

}

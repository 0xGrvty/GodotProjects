using Godot;
using System;

public class PlayerAttackState : IStateMachine {
    public IStateMachine EnterState(PlayerBody player) {
        // Check to see if we are attacking, then choose the correct attack direction animation to play
        // if we aren't
        if (!player.IsAttacking) {
            switch (player.GetFacing()) {
                case PlayerBody.FaceDir.UP:
                case PlayerBody.FaceDir.UP_LEFT:
                case PlayerBody.FaceDir.UP_RIGHT:
                    player.GetAnimatedSprite().Animation = "PlayerAttackUp";
                    break;
                case PlayerBody.FaceDir.DOWN:
                    player.GetAnimatedSprite().Animation = "PlayerAttackDown";
                    break;
                case PlayerBody.FaceDir.RIGHT:
                case PlayerBody.FaceDir.LEFT:
                    player.GetAnimatedSprite().Animation = "PlayerAttackRight";
                    break;
                case PlayerBody.FaceDir.DOWN_RIGHT:
                case PlayerBody.FaceDir.DOWN_LEFT:
                    player.GetAnimatedSprite().Animation = "PlayerAttackDownRight";
                    break;
            }
            player.GetAnimatedSprite().SpeedScale = 1;
            player.GetAnimatedSprite().Play();
            
            // Check the sprite index of the attacking frame.  This is hard coded, we should move this into the player when we get the chance.
            // Then create a hammer.  Then set IsAttacking to true so that we don't create infinite hammers or update our player animation sprite
            // until he is done attacking.
            if (player.GetAnimatedSprite().Frame == 2) {
                player.IsAttacking = true;
                Hammer h = (Hammer)player.hammerScene.Instance();
                h.Init(player);
                player.GetParent().AddChild(h);
            }
        }

        // If the currently playing animation is on the last frame, we can check to see 
        // This looks so bad, wtf.  Why do we have to provide the animation name of the animation we're trying to get the frame count from??
        if (player.GetAnimatedSprite().Frame >= player.GetAnimatedSprite().Frames.GetFrameCount(player.GetAnimatedSprite().Animation) - 1) {
            // We can attack again if the player attack cooldown is up, so reset the attack cooldown
            player.ResetAttackCooldown();
            if (Input.IsActionPressed("BlessedHammer")) {
                EmitChangeStateSignal(player, player.playerAttackState);
                return player.playerAttackState;
            }
            // Check to see if the player is holding a direction after attacking
            if (player.GetVelocity() != Vector2.Zero) {
                EmitChangeStateSignal(player, player.playerRunState);
                return player.playerRunState;

                // Otherwise, change their state to Idle
            } else {
                EmitChangeStateSignal(player, player.playerIdleState);
                return player.playerIdleState;
            }
        }
        // Continue returning this state, we don't want them to be able to move while attacking (Diablo 2-esque)

        return player.playerAttackState;

    }
    public void EmitChangeStateSignal(PlayerBody player, IStateMachine state) {
        player.EmitSignal("StateChanged", state.GetType().ToString());
    }
}

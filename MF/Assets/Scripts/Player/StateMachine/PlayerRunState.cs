using Godot;
using System;

public class PlayerRunState : IStateMachine {
    public IStateMachine EnterState(PlayerBody player) {
        // Tell the player to do their movement whenever they are in the Run state
        player.DoMovement();
        var tempScale = player.Scale;
        switch (player.GetFacing()) {
            case PlayerBody.FaceDir.UP:
            case PlayerBody.FaceDir.UP_LEFT:
            case PlayerBody.FaceDir.UP_RIGHT:
                player.GetAnimatedSprite().Animation = "PlayerRunUp";
                break;
            case PlayerBody.FaceDir.DOWN:
                player.GetAnimatedSprite().Animation = "PlayerRunDown";
                break;
            case PlayerBody.FaceDir.RIGHT:
            case PlayerBody.FaceDir.LEFT:
                player.GetAnimatedSprite().Animation = "PlayerRunRight";
                break;
            case PlayerBody.FaceDir.DOWN_RIGHT:
            case PlayerBody.FaceDir.DOWN_LEFT:
                player.GetAnimatedSprite().Animation = "PlayerRunDownRight";
                break;
        }
        if (Mathf.Sign(player.GetVelocity().x) > 0 && player.GetAnimatedSprite().FlipH || Mathf.Sign(player.GetVelocity().x) < 0 && !player.GetAnimatedSprite().FlipH) {
            //player.Scale = new Vector2(tempScale.x, 3);
            //player.Rotation = 0;
            player.GetAnimatedSprite().FlipH = !player.GetAnimatedSprite().FlipH; //<-- Super simple way to flip the graphics, but need to find a way to flip or rotate the whole KinematicBody2D so that things that spawn under it as a child use its local coordinates
            //player.Scale = new Vector2(tempScale.x * -1, 3);
        }
        //else if (Mathf.Sign(player.GetVelocity().x) < 0) {
        //    //player.Scale = new Vector2(-tempScale.x, 3);
        //    //player.Rotation = 0;
        //}
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

    public void EmitChangeStateSignal(PlayerBody player, IStateMachine state) {
        player.EmitSignal("StateChanged", state.GetType().ToString());
    }

}

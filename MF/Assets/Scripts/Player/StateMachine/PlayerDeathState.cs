using Godot;
using System;
using System.Collections;

public class PlayerDeathState : IStateMachine {
    private bool isDead= false;
    private bool deathSoundPlayed;
    public IStateMachine EnterState(Node2D p) {
        var player = (PlayerBody)p;
        switch (player.GetFacing()) {
            case FaceDir.UP:
            case FaceDir.UP_LEFT:
            case FaceDir.UP_RIGHT:
                player.GetAnimatedSprite().Animation = "PlayerDeathUp";
                break;
            case FaceDir.DOWN:
                player.GetAnimatedSprite().Animation = "PlayerDeathDown";
                break;
            case FaceDir.RIGHT:
            case FaceDir.LEFT:
                player.GetAnimatedSprite().Animation = "PlayerDeathRight";
                break;
            case FaceDir.DOWN_RIGHT:
            case FaceDir.DOWN_LEFT:
                player.GetAnimatedSprite().Animation = "PlayerDeathDownRight";
                break;
        }
        player.GetAnimatedSprite().SpeedScale = 1;
        player.GetAnimatedSprite().Play();
        if (!player.GetDeathSound().Playing && !deathSoundPlayed) {
            deathSoundPlayed = true;
            player.GetDeathSound().Play();
        }
        

        if (player.GetAnimatedSprite().Frame >= player.GetAnimatedSprite().Frames.GetFrameCount(player.GetAnimatedSprite().Animation) - 1 && !isDead) {
            isDead = true;
            EventBus.Instance.EmitSignal(nameof(EventBus.PlayerDied));
        }

        return player.playerDeathState;
    }

    public void EmitChangeStateSignal(Node2D p, IStateMachine state) {
        var player = (PlayerBody)p;
        player.EmitSignal("StateChanged", state.GetType().ToString());
    }

}

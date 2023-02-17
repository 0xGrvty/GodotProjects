using Godot;
using System;
using System.Collections;

public class PlayerDeathState : IStateMachine {
    private bool isDead= false;
    public IStateMachine EnterState(PlayerBody player) {
        player.GetAnimatedSprite().Animation = "PlayerDeath";
        player.GetAnimatedSprite().SpeedScale = 1;
        player.GetAnimatedSprite().Play();

        if (player.GetAnimatedSprite().Frame >= player.GetAnimatedSprite().Frames.GetFrameCount(player.GetAnimatedSprite().Animation) - 1 && !isDead) {
            isDead = true;
            EventBus.Instance.EmitSignal(nameof(EventBus.PlayerDied));
        }

        return player.playerDeathState;
    }

    public void EmitChangeStateSignal(PlayerBody player, IStateMachine state) {
        player.EmitSignal("StateChanged", state.GetType().ToString());
    }

}

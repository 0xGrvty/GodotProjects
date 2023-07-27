using Godot;
using System;

public partial class PlayerSceneTransitionState : IStateMachine {


    public IStateMachine EnterState(Node actor) {
        var player = actor as Player;
        player.DoMovement(player.GetProcessDeltaTime(), player.GetSnapshotDirection(), true);

        if (player.IsGrounded()) {
            player.AnimatedSprite.Play("Run");
        }
        return player.playerSceneTransitionState;
    }

    public void EmitStateChanged(Node actor, IStateMachine state) {
        GD.Print(actor.Name, " Is now in ", state, " state");
    }
}

using Godot;
using System;

public partial class PlayerSceneTransitionState : IStateMachine {

    // Currently unused and unfinished class
    public IStateMachine EnterState(Node actor) {
        var player = actor as Player;

        if (player.IsGrounded()) {
            player.AnimatedSprite.Play("Run");
        }
        return player.playerSceneTransitionState;
    }

    public void EmitStateChanged(Node actor, IStateMachine state) {
        GD.Print(actor.Name, " Is now in ", state, " state");
    }
}

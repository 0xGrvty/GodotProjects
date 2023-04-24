using Godot;
using System;

public partial class PlayerJumpSquatState : IStateMachine {

    public IStateMachine EnterState(Node actor) {
        var player = actor as Player;
        var jumpSquat = "JumpSquat";
        player.AnimatedSprite.SpeedScale = 1f;
        player.AnimatedSprite.Play(jumpSquat);

        return player.playerJumpSquatState;
    }
    public void EmitStateChanged(Node actor, IStateMachine state) {
        GD.Print(actor.Name, " Is now in ", state, " state");
    }
}

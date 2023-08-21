using Godot;
using System;

public partial class BatRocketState : IStateMachine {
    public IStateMachine EnterState(Node actor) {
        var bat = actor as Bat;

        bat.Move();
        return bat.rocketState;
    }

    public void EmitStateChanged(Node actor, IStateMachine state) {
        throw new NotImplementedException();
    }
}

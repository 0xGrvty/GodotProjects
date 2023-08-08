using Godot;
using System;

public partial class BatCVaniaState : IStateMachine {
    public IStateMachine EnterState(Node actor) {
        var bat = actor as Bat;

        bat.Move();
        return bat.cvaniaState;
    }

    public void EmitStateChanged(Node actor, IStateMachine state) {
        throw new NotImplementedException();
    }
}

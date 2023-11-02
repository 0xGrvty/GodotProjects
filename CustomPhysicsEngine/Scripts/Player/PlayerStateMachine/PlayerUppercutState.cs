using Godot;
using System;

public partial class PlayerUppercutState : IStateMachine {
    private Attack attack;
    private int activeFrame = 0;
    public PlayerUppercutState(Node hitboxes) {
        attack = new Attack(hitboxes);
    }

    public IStateMachine EnterState(Node actor) {
        var player = actor as Player;

        return this;
    }

    public void EmitStateChanged(Node actor, IStateMachine state) {
        throw new NotImplementedException();
    }
}

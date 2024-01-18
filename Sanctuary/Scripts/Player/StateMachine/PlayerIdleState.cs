using Godot;
using System;

public partial class PlayerIdleState : State {
    [Export]
    private Player p;
    public override void EnterState() {
        base.EnterState();
    }

    public override void ExitState() {
        base.ExitState();
    }

    public override void Update(double delta) {
        if (Input.IsActionPressed("Move")) {
            p.SetMoveTo(p.GetGlobalMousePosition());
            EmitSignal(State.SignalName.StateFinished, this, Globals.PLAYER_RUN);
        }

        if (Input.IsActionJustPressed("Spell1")) {
            p.SetAttackDir(p.GetLocalMousePosition().Normalized());
            EmitSignal(State.SignalName.StateFinished, this, Globals.PLAYER_ATTACK);
        }
    }

    public override void PhysicsUpdate(double delta) {
        base.PhysicsUpdate(delta);
    }

    
}

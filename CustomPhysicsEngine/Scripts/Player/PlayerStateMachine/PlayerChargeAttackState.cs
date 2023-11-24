using Godot;
using System;

public partial class PlayerChargeAttackState : State, IAttackState {
    [Export]
    private Player p;
    [Export]
    private AnimationPlayer ap;

    public override void EnterState() {
        ap.Play("Charge_Hold");
    }

    public override void PhysicsUpdate(double delta) {
        ap.Play("Charge_Hold");
        if (Input.IsActionJustReleased("Charge")) {
            ChangeState();
        }
    }

    public void ChangeState() {
        if (p.GetInputBufferContents().Contains(1)) {
            EmitSignal(nameof(StateFinished), this, "Attack2");
        } else {
            EmitSignal(nameof(StateFinished), this, "Idle");
        }
    }
}

using Godot;
using System;

public partial class PlayerJumpState : State {

    [Export]
    private Player actor;
    [Export]
    private AnimationPlayer ap;

    public override void EnterState() {
        ap.Play("Jump");
    }

    public override void PhysicsUpdate(double delta) {
        var direction = actor.GetDirectionInput();
        actor.DoMovement(actor.GetPhysicsProcessDeltaTime(), direction);

        // Transition from jumping to falling
        if (actor.Velocity.Y > 0.0) {
            EmitSignal(nameof(StateFinished), this, "Fall");
        }
    }

}

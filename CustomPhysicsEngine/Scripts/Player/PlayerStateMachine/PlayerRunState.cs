using Godot;
using System;

public partial class PlayerRunState : State {

    [Export]
    private Player actor;
    [Export]
    private AnimationPlayer ap;
    public override void EnterState() {
        ap.Play("Run");
    }

    public override void PhysicsUpdate(double delta) {
        var direction = actor.GetDirectionInput();
        actor.DoMovement(actor.GetPhysicsProcessDeltaTime(), direction);

        // If the player is not holding a direction at all
        if (direction == 0) {
            EmitSignal(nameof(StateFinished), this, "Idle");
        }

        // If the player jumps while running
        if (actor.IsJumping) {
            EmitSignal(nameof(StateFinished), this, "Jump");
        }

        // If the player suddenly falls and is no longer grounded
        if (actor.Velocity.Y > 0.0 && !actor.IsGrounded()) {
            actor.WasGrounded = true;
            EmitSignal(nameof(StateFinished), this, "Fall");
        }
    }

}

using Godot;
using System;

public partial class PlayerJumpState : State {

    [Export]
    private Player p;
    [Export]
    private AnimationPlayer ap;

    public override void EnterState() {
        ap.Play("Jump");
        p.WasGrounded = false;
    }

    public override void PhysicsUpdate(double delta) {
        var direction = p.GetDirectionInput();
        p.DoMovement(GetPhysicsProcessDeltaTime(), direction);
        p.Jump(GetPhysicsProcessDeltaTime());

        // Transition from jumping to falling
        if (p.Velocity.Y > 0.0) {
            EmitSignal(nameof(StateFinished), this, "Fall");
        }
    }

}

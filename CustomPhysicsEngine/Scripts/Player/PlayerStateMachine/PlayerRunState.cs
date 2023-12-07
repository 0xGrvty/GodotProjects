using Godot;
using System;

public partial class PlayerRunState : State {

    [Export]
    private Player p;
    [Export]
    private AnimationPlayer ap;
    public override void EnterState() {
        ap.Play("Run");
    }

    public override void PhysicsUpdate(double delta) {
        var direction = p.GetDirectionInput();
        p.DoMovement(GetPhysicsProcessDeltaTime(), direction);
        p.Jump(GetPhysicsProcessDeltaTime());

        // If the player is not holding a direction at all
        if (direction == 0) {
            EmitSignal(nameof(StateFinished), this, "Idle");
        }

        // If the player jumps while running
        if (p.IsJumping) {
            EmitSignal(nameof(StateFinished), this, "Jump");
        }

        // If the player suddenly falls and is no longer grounded
        if (p.Velocity.Y > 0.0 && !p.IsGrounded()) {
            p.WasGrounded = true;
            EmitSignal(nameof(StateFinished), this, "Fall");
        }

        if (Input.IsActionJustPressed("Charge")) {
            EmitSignal(nameof(OnAttack), this);
            EmitSignal(nameof(StateFinished), this, "Charge");
        }

        if (Input.IsActionJustPressed("Attack")) {
            var attackButton = new StringName("Attack");
            // Technically don't *need* this, but just for consistency's sake, we should
            // add the input into the buffer when we press it for the first time

            // For good practice, make this a signal.  Signal up, call down!
            p.AttackInputBuffer.AddInput(attackButton);
            //EmitSignal(nameof(OnAttack), this);
            EmitSignal(nameof(StateFinished), this, "Attack1");
        }
    }

}

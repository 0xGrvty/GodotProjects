using Godot;
using System;

public partial class PlayerFallState : State {
    [Export]
    private Player p;
    [Export]
    private AnimationPlayer ap;

    public override void EnterState() {
        ap.Play("Fall");
    }

    public override void PhysicsUpdate(double delta) {
        // We need to get the direction in every state (besides the attack state)
        // because we need to know if we need to turn the player
        // and just in case if the player is stuck in the animation (such as hitstun, if we choose to implement it)
        // we would still want to know which direction they are holding.
        var direction = p.GetDirectionInput();
        p.DoMovement(GetPhysicsProcessDeltaTime(), direction);
        p.Jump(GetPhysicsProcessDeltaTime());

        // If the player was grounded, and there is still time on the Coyote Timer, let them jump
        if (p.WasGrounded) {
            p.CoyoteTime -= (float)p.GetPhysicsProcessDeltaTime();
            if (p.CoyoteTime <= 0.0f) {
                p.WasGrounded = false;
                p.NumJumps--;
                EmitSignal(nameof(StateFinished), this, "Jump");
            }
        }

        // If the player becomes grounded, reset their number of jumps and transition to the next state
        if (p.IsGrounded()) {
            p.ResetGroundedStats();

            // If the player jumps as soon as they hit the ground
            if (p.IsJumping) {
                p.NumJumps--;
                EmitSignal(nameof(StateFinished), this, "Jump");
            }
            // If the player is holding a direction, put them in the run animation if they are falling and they touch the ground
            if (direction != 0) {
                EmitSignal(nameof(StateFinished), this, "Run");
            } else {
                EmitSignal(nameof(StateFinished), this, "Idle");
            }
        }
    }
}

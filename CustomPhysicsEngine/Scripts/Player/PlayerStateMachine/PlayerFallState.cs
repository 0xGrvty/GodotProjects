using Godot;
using System;

public partial class PlayerFallState : State {
    [Export]
    private Player actor;
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
        var direction = actor.GetDirectionInput();
        actor.DoMovement(actor.GetPhysicsProcessDeltaTime(), direction);

        // If the player was grounded, and there is still time on the Coyote Timer, let them jump
        if (actor.WasGrounded) {
            actor.CoyoteTime -= (float)actor.GetPhysicsProcessDeltaTime();
            if (actor.CoyoteTime <= 0.0f) {
                actor.WasGrounded = false;
                actor.NumJumps--;
            }
        }

        
        if (actor.IsJumping) {
            if (actor.IsGrounded()) {
                actor.ResetGroundedStats();
                actor.NumJumps--;
            }
        }

        // If the player becomes grounded, reset their number of jumps and transition to the next state
        if (actor.IsGrounded()) {
            actor.ResetGroundedStats();
            // If the player is holding a direction, put them in the run animation if they are falling and they touch the ground
            if (direction != 0) {
                EmitSignal(nameof(StateFinished), this, "Run");
            } else {
                EmitSignal(nameof(StateFinished), this, "Idle");
            }
        }
    }
}

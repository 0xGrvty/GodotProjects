using Godot;
using System;

public partial class PlayerIdleState : State {

    [Export]
    private Player p;
    [Export]
    private AnimationPlayer ap;

    public override void EnterState() {
        ap.Play("Idle");
    }

    public override void PhysicsUpdate(double delta) {
        // We need to get the direction in every state (besides the attack state)
        // because we need to know if we need to turn the player
        // and just in case if the player is stuck in the animation (such as hitstun, if we choose to implement it)
        // we would still want to know which direction they are holding.

        // For good practice, emit a signal.  Signal up, call down!.
        var direction = p.GetDirectionInput();
        p.DoMovement(GetPhysicsProcessDeltaTime(), direction);
        p.Jump(GetPhysicsProcessDeltaTime());


        // Check to see if the player is jumping and if they are grounded first
        if (p.IsJumping && p.IsGrounded()) {
            EmitSignal(nameof(StateFinished), this, "Jump");
        }

        // If the player actually presses the left or right keys, and we should make sure they are moving
        if (p.IsGrounded() && p.Velocity.X != 0 && direction != 0) {
            //EmitStateChanged(player, player.playerRunState);
            EmitSignal(nameof(StateFinished), this, "Run");
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

        // If the player suddenly falls, such as a platform disappearing under them
        if (p.Velocity.Y > 0.0 && !p.IsGrounded()) {
            p.WasGrounded = true;
            EmitSignal(nameof(StateFinished), this, "Fall");
        }

        if (Input.IsActionJustPressed("Charge")) {
            EmitSignal(nameof(OnAttack), this);
            EmitSignal(nameof(StateFinished), this, "Charge");
        }
    }
}

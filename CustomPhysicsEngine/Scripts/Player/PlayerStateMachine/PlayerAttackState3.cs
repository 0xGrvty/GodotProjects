using Godot;
using System;

public partial class PlayerAttackState3 : State {

    [Export]
    private Player actor;
    [Export]
    private AnimationPlayer ap;

    public override void EnterState() {
        ap.Play("Attack3");
    }

    public override void PhysicsUpdate(double delta) {
        actor.DoAttack();
    }
    public void ChangeState() {

        // If the player presses attack within a few frames of the animation finishing
        if (actor.GetInputBufferContents().Contains(1)) {
            EmitSignal(nameof(StateFinished), this, "Attack1");

        // Else if the player does not press the attack button
        } else {
            EmitSignal(nameof(StateFinished), this, "Idle");
        }
    }
}
